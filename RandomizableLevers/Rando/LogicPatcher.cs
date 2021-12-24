using System.Collections.Generic;
using System.IO;
using ItemChanger;
using RandomizerMod.Settings;
using RandomizerMod.RC;
using RandomizerCore;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;

namespace RandomizableLevers.Rando
{
    /// <summary>
    /// Class that modifies the lmb to allow for lever rando logic
    /// </summary>
    internal static class LogicPatcher
    {
        public static void Hook()
        {
            RCData.RuntimeLogicOverride.Subscribe(0.3f, PatchLogic);
        }

        private static Dictionary<string, Term> Terms = new();

        private static void PatchLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!RandoInterop.Settings.RandomizeLevers)
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
                if (lever == LeverNames.Switch_Dirtmouth_Stag)
                {
                    Term term = lmb.GetTerm(ItemNames.Dirtmouth_Stag);
                    LogicItem item = new CappedItem(lever, new TermValue[] { new(term, 1)}, new(term, 1));
                    lmb.AddItem(item);
                }
                else if (lever == LeverNames.Lever_Resting_Grounds_Stag)
                {
                    Term term = lmb.GetTerm(ItemNames.Resting_Grounds_Stag);
                    Term stags = lmb.GetTerm("STAGS");
                    LogicItem item = new CappedItem(lever, new TermValue[] { new(term, 1), new(stags, 1) }, new(term, 1));
                    lmb.AddItem(item);
                }
                else
                {
                    Term leverTerm = lmb.GetOrAddTerm(lever);
                    lmb.AddItem(new SingleItem(lever, new TermValue(leverTerm, 1)));
                    Terms[lever] = leverTerm;
                }
            }
        }

        private static void ModifyExistingMacros(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            using Stream s = typeof(LogicPatcher).Assembly.GetManifestResourceStream("RandomizableLevers.Resources.Logic.MacroOverrides.json");
            lmb.DeserializeJson(LogicManagerBuilder.JsonType.MacroEdit, s);
        }

        private static void ModifyExistingLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            using Stream s = typeof(LogicPatcher).Assembly.GetManifestResourceStream("RandomizableLevers.Resources.Logic.LogicOverrides.json");
            lmb.DeserializeJson(LogicManagerBuilder.JsonType.LogicEdit, s);
        }

        private static void AddLeverLocations(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            using Stream s = typeof(LogicPatcher).Assembly.GetManifestResourceStream("RandomizableLevers.Resources.Logic.LeverLocationLogic.json");
            lmb.DeserializeJson(LogicManagerBuilder.JsonType.Locations, s);
        }
    }
}
