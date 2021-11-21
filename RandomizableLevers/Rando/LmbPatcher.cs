using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using RandomizerMod.Settings;
using RandomizerMod.RC;
using RandomizerCore;
using RandomizerCore.Json;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;

namespace RandomizableLevers.Rando
{
    /// <summary>
    /// Class that modifies the lmb to allow for lever rando logic
    /// </summary>
    internal static class LmbPatcher
    {
        public static void Hook()
        {
            RCData.RuntimeLogicOverride.Subscribe(0, LogicPatcher);
        }

        private static Dictionary<string, Term> Terms = new();

        private static void LogicPatcher(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (gs.TransitionSettings.GetLogicMode() != RandomizerMod.LogicMode.Room)
            {
                return;
            }

            AddTermsAndItemsToLmb(gs, lmb);
            ModifyExistingMacros(gs, lmb);
            ModifyExistingLogic(gs, lmb);
            AddLeverLocations(gs, lmb);
        }

        // Add terms, so that they can be used for logic
        private static void AddTermsAndItemsToLmb(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            foreach (string lever in LeverNames.ToArray())
            {
                // Exclude Dirtmouth and RG stag levers, because for logic they'll be treated as the same as the stags
                if (lever == LeverNames.Switch_Dirtmouth_Stag || lever == LeverNames.Lever_Resting_Grounds_Stag) continue;
                Term leverTerm = lmb.GetOrAddTerm(lever);
                lmb.AddItem(new SingleItem(lever, new TermValue(leverTerm, 1)));

                Terms[lever] = leverTerm;
            }
        }

        private static void ModifyExistingMacros(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            using Stream s = typeof(LmbPatcher).Assembly.GetManifestResourceStream("RandomizableLevers.Resources.Logic.MacroOverrides.json");
            lmb.DeserializeJson(LogicManagerBuilder.JsonType.Macros, s);
        }

        private static void ModifyExistingLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            using Stream s = typeof(LmbPatcher).Assembly.GetManifestResourceStream("RandomizableLevers.Resources.Logic.LogicOverrides.json");
            lmb.DeserializeJson(LogicManagerBuilder.JsonType.Logic, s);
        }

        private static void AddLeverLocations(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            using Stream s = typeof(LmbPatcher).Assembly.GetManifestResourceStream("RandomizableLevers.Resources.Logic.LeverLocationLogic.json");
            lmb.DeserializeJson(LogicManagerBuilder.JsonType.Locations, s);
        }
    }
}
