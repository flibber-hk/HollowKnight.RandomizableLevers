using System;
using System.Collections.Generic;
using System.Linq;
using ItemChanger;
using Modding;
using RandomizerMod.Menu;
using RandomizerMod.RC;
using RandomizerMod.Settings;
using StartDef = RandomizerMod.RandomizerData.StartDef;

namespace RandomizableLevers.Rando
{
    public static class MiscRandoChanges
    {
        public const string LeversUnrandomized = "LEVERSUNRANDOMIZED";

        public static void Hook()
        {
            RandomizerMenuAPI.OnGenerateStartLocationDict += PatchStarts;
            SettingsPM.OnResolveBoolTerm += CatchLeversRandomizedTerm;
            RandoController.OnExportCompleted += RemoveExtraPlatforms;
        }

        private static bool CatchLeversRandomizedTerm(string term, out bool result)
        {
            if (term == LeversUnrandomized)
            {
                result = !RandoInterop.Settings.RandomizeLevers;
                return true;
            }

            result = default;
            return false;
        }

        private static void PatchStarts(Dictionary<string, StartDef> startDefs)
        {
            try
            {
                (string westWaterwaysName, StartDef westWaterwaysStart) 
                    = startDefs.First(pair => pair.Value.SceneName == SceneNames.Waterways_09);

                startDefs[westWaterwaysName] = westWaterwaysStart with
                {
                    // Change the start transition for WW_09 start
                    Transition = SceneNames.Waterways_09 + "[right1]",
                    // Exclude West Waterways entirely from area rando because no transitions are reachable with common settings
                    Logic = $"({westWaterwaysStart.Logic}) + (ROOMRANDO | ITEMRANDO | {LeversUnrandomized})",
                    // Exclude West Waterways from randomization because only 1 check is reachable with common settings
                    RandoLogic = $"({westWaterwaysStart.RandoLogic ?? westWaterwaysStart.Logic}) + (ROOMRANDO | {LeversUnrandomized})"
                };
            }
            catch (InvalidOperationException)
            {
                // No west waterways start
                RandomizableLevers.instance.LogWarn("No West Waterways start found");
            }

            try
            {
                (string startName, StartDef start)
                    = startDefs.First(pair => pair.Value.SceneName == SceneNames.Crossroads_ShamanTemple);

                startDefs[startName] = start with
                {
                    // Exclude from randomization because dirtmouth isn't reachable itemless
                    RandoLogic = $"({start.RandoLogic ?? start.Logic}) + (ROOMRANDO | MAPAREARANDO | FULLAREARANDO | {LeversUnrandomized})"
                };
            }
            catch (InvalidOperationException)
            {
                RandomizableLevers.instance.LogWarn("No Ancestral Mound start found");
            }

            try
            {
                (string startName, StartDef start)
                    = startDefs.First(pair => pair.Value.SceneName == SceneNames.Crossroads_50 && pair.Value.X > 200f);

                startDefs[startName] = start with
                {
                    // Exclude from randomization because dirtmouth isn't reachable itemless
                    RandoLogic = $"({start.RandoLogic ?? start.Logic}) + (ROOMRANDO | FULLAREARANDO | {LeversUnrandomized})"
                };
            }
            catch (InvalidOperationException)
            {
                RandomizableLevers.instance.LogWarn("No East Blue Lake start found");
            }
        }

        private static void RemoveExtraPlatforms(RandoController rc)
        {
            if (rc.ctx.itemPlacements.Any(x => x.Item.Name == LeverNames.Lever_Waterways_Hwurmp_Arena))
            {
                // Remove platforms in Waterways_04 in case they arrive there from the Waterways exit gate
                List<IDeployer> deployers = ItemChanger.Internal.Ref.Settings.Deployers;

                foreach (IDeployer deployer in deployers)
                {
                    if (ShouldRemoveDeployer(deployer))
                    {
                        Events.RemoveSceneChangeEdit(deployer.SceneName, deployer.OnSceneChange);
                    }
                }

                deployers.RemoveAll(ShouldRemoveDeployer);
            }
        }

        public static bool ShouldRemoveDeployer(IDeployer d)
        {
            return d is SmallPlatform sp && sp.SceneName == SceneNames.Waterways_04;
        }
    }
}
