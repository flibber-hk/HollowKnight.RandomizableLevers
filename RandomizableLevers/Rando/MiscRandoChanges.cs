using ItemChanger;
using Modding;
using RandomizerMod.Menu;
using RandomizerMod.RC;
using RandomizerMod.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

        private static bool TryGetStartForScene(
            this Dictionary<string, StartDef> startDefs, string sceneName, out string name, out StartDef start, Func<StartDef, bool> predicate = null
            )
        {
            foreach ((string sName, StartDef sStart) in startDefs)
            {
                if (sStart.SceneName == sceneName && (predicate?.Invoke(sStart) ?? true))
                {
                    name = sName;
                    start = sStart;
                    return true;
                }
            }
            name = default;
            start = default;
            return false;
        }

        private static void PatchStarts(Dictionary<string, StartDef> startDefs)
        {
            bool leaveAlone = !RandoInterop.Settings.RandomizeLevers;

            // Waterways
            if (startDefs.TryGetStartForScene(SceneNames.Waterways_09, out string westWaterwaysName, out StartDef westWaterwaysStart))
            {
                if (leaveAlone)
                {
                    startDefs[westWaterwaysName] = westWaterwaysStart with
                    {
                        // We can safely keep this because the logic is unchanged unless levers are randomized
                        // This is needed in the menu so we need to apply the change unconditionally
                        Logic = $"({westWaterwaysStart.Logic}) + (ROOMRANDO | ITEMRANDO | {LeversUnrandomized})",
                    };
                }
                else
                {
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
            }

            // AMound
            if (startDefs.TryGetStartForScene(SceneNames.Crossroads_ShamanTemple, out string amoundName, out StartDef amoundStart) && !leaveAlone)
            {
                startDefs[amoundName] = amoundStart with
                {
                    // Exclude from randomization because dirtmouth isn't reachable itemless
                    RandoLogic = $"({amoundStart.RandoLogic ?? amoundStart.Logic}) + (ROOMRANDO | MAPAREARANDO | FULLAREARANDO | {LeversUnrandomized})"
                };
            }

            // East blue lake
            if (startDefs.TryGetStartForScene(SceneNames.Crossroads_50, out string blueLakeName, out StartDef blueLakeStart, start => start.X > 200f) && !leaveAlone)
            {
                startDefs[blueLakeName] = blueLakeStart with
                {
                    // Exclude from randomization because dirtmouth isn't reachable itemless
                    RandoLogic = $"({blueLakeStart.RandoLogic ?? blueLakeStart.Logic}) + (ROOMRANDO | FULLAREARANDO | {LeversUnrandomized})"
                };
            }

            // Mantis
            if (startDefs.TryGetStartForScene(SceneNames.Fungus2_14, out string mantisVName, out StartDef mantisVStart) && !leaveAlone)
            {
                startDefs[mantisVName] = mantisVStart with
                {
                    // Exclude from randomization because dirtmouth isn't reachable itemless
                    RandoLogic = $"({mantisVStart.RandoLogic ?? mantisVStart.Logic}) + (ROOMRANDO | FULLAREARANDO | {LeversUnrandomized})"
                };
            }
        }

        private static void RemoveExtraPlatforms(RandoController rc)
        {
            // Check for presence of item rather than the value of the setting so that adding this item
            // through the CustomPoolInjector works
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
