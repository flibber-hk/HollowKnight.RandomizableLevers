using System;
using System.Collections.Generic;
using System.IO;
using ItemChanger;
using Newtonsoft.Json;
using RandomizerMod.Settings;
using RandomizerMod.RC;
using RandomizerCore;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using RandomizerCore.StringLogic;
using RandomizerCore.Json;

namespace RandomizableLevers.Rando
{
    /// <summary>
    /// Class that modifies the lmb to allow for lever rando logic
    /// </summary>
    internal static class LogicPatcher
    {
        public static void Hook()
        {
            RCData.RuntimeLogicOverride.Subscribe(-999.7f, TryDefineTerms);
            RCData.RuntimeLogicOverride.Subscribe(0.3f, TryPatchLogic);
            RCData.RuntimeLogicOverride.Subscribe(1000.3f, TryMakeSubstitutions);
        }

        private static void TryMakeSubstitutions(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!RandoInterop.Settings.Any)
            {
                return;
            }

            JsonLogicFormat fmt = new();

            // Make substitutions late to allow for more compatibility with route-adding connections
            ApplyEmbeddedSubstitutions(gs, lmb, fmt);
        }

        private static void TryDefineTerms(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!RandoInterop.Settings.Any)
            {
                return;
            }

            // For each lever, define a term to indicate that the lever has been obtained, and a capped item to correspond to giving the term.
            AddTermsAndItemsToLmb(gs, lmb);
        }

        private static void TryPatchLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!RandoInterop.Settings.Any)
            {
                return;
            }

            JsonLogicFormat fmt = new();

            // For many existing locations and transitions, we need to apply edits to be compatible with lever rando.
            ApplyEmbeddedOverrides(gs, lmb, fmt);

            // For some lever locations, the logic will be identical to existing locations. (When it is identical to an existing transition, we simply mark its
            // logic as the logic for the transition - when the transition becomes accessible RandomizerCore will add it to logic.
            ApplyClonedLogic(gs, lmb);

            // For some levers, we can use a base rando event (waypoint) to provide the logic for the lever. In those cases, we effectively
            // treat the waypoint as no longer pre-placed.
            BifurcateLevers(gs, lmb);

            // Provide the logic for the remaining lever locations.
            AddLeverLocations(gs, lmb, fmt);
        }

        private static void BifurcateLevers(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            foreach ((string leverName, string eventName) in new (string, string)[]
            {
                (LeverNames.Lever_Shade_Soul_Exit, "Lever-Shade_Soul"),
                // (LeverNames.Lever_Dung_Defender, "Lever_Dung_Defender")
                (LeverNames.Switch_Lower_Resting_Grounds, "Opened_Resting_Grounds_Floor"),
                (LeverNames.Switch_Queens_Gardens_Stag, "Opened_Gardens_Stag_Exit"),
                // (LeverNames.Switch_Outside_Ancestral_Mound, "Opened_Shaman_Pillar"),
                (LeverNames.Lever_Failed_Tramway_Left, "Opened_Tramway_Exit_Gate"),
                (LeverNames.Lever_Emilitia, "Opened_Emilitia_Door"),
                (LeverNames.Lever_Abyss_Lighthouse, "Lit_Abyss_Lighthouse"),
                (LeverNames.Lever_Waterways_Hwurmp_Arena, "Opened_Waterways_Exit"),
                (LeverNames.Lever_Palace_Entrance_Orb, "Palace_Entrance_Lantern_Lit"),
                (LeverNames.Lever_Palace_Left_Orb, "Palace_Left_Lantern_Lit"),
                // (LeverNames.Lever_Palace_Right_Orb, "Palace_Right_Lantern_Lit"),
                (LeverNames.Lever_City_Fountain, "Lever-City_Fountain"),
                (LeverNames.Lever_Path_of_Pain, "Lever-Path_of_Pain"),
            })
            {
                if (leverName != eventName)
                {
                    // The logic for the event is "obtain the lever". The logic for the lever is what the old logic for the event was.
                    lmb.LogicLookup[leverName] = lmb.LogicLookup[eventName];
                    lmb.LogicLookup[eventName] = lmb.LP.ParseInfixToClause(leverName);
                }
                else
                {
                    // Turn the old waypoint into a location
                    lmb.Waypoints.Remove(leverName);
                }
            }

            // Manual changes
            {
                // Lever name = event name here
                // In-built logic is incorrect - vertical is needed to get to the location.
                LogicClauseBuilder lcb = lmb.LP.ParseInfixToBuilder("ORIG + (ANYCLAW | WINGS)");
                lcb.Subst(lmb.LP.GetTermToken("ORIG"), lmb.LogicLookup["Lever-Dung_Defender"]);
                lmb.LogicLookup[LeverNames.Lever_Dung_Defender] = new(lcb);
                lmb.Waypoints.Remove("Lever-Dung_Defender");
            }

            {
                // Dreamer gets the shaman pillar, rather than access to the switch
                lmb.LogicLookup[LeverNames.Switch_Outside_Ancestral_Mound] = lmb.LP.ParseInfixToClause("Crossroads_06[left1] | Crossroads_06[door1] | Crossroads_06[right1]");
                lmb.LogicLookup["Opened_Shaman_Pillar"] = lmb.LP.ParseInfixToClause("Switch-Outside_Ancestral_Mound | DREAMER");
            }

            {
                // Right orb logic is different because of the right lever
                lmb.LogicLookup[LeverNames.Lever_Palace_Right] = lmb.LogicLookup["Palace_Right_Lantern_Lit"];
                lmb.LogicLookup[LeverNames.Lever_Palace_Right_Orb] = lmb.LP.ParseInfixToClause("Lever-Palace_Right + (White_Palace_15[right1] | White_Palace_15[left1] | White_Palace_15[right2])");
                lmb.LogicLookup["Palace_Right_Lantern_Lit"] = lmb.LP.ParseInfixToClause(LeverNames.Lever_Palace_Right_Orb);
            }

            {
                // Clone existing logic for RG and Dirtmouth stags
                lmb.LogicLookup[LeverNames.Switch_Dirtmouth_Stag] = lmb.LogicLookup[LocationNames.Dirtmouth_Stag];
                lmb.LogicLookup[LeverNames.Lever_Resting_Grounds_Stag] = lmb.LogicLookup[LocationNames.Resting_Grounds_Stag];
            }
        }

        private static void ApplyClonedLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            lmb.LogicLookup[LeverNames.Lever_Queens_Station_Mask_Shard] = lmb.LogicLookup[LocationNames.Mask_Shard_Queens_Station];
            lmb.LogicLookup[LeverNames.Lever_Fungal_Wastes_Thorns_Gauntlet] = lmb.LogicLookup[LocationNames.Wanderers_Journal_Fungal_Wastes_Thorns_Gauntlet];
            lmb.LogicLookup[LeverNames.Lever_Crystal_Peak_Spike_Grub] = lmb.LogicLookup[LocationNames.Grub_Crystal_Peak_Spike];
        }

        // Add terms, so that they can be used for logic
        private static void AddTermsAndItemsToLmb(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            foreach (string lever in LeverNames.ToArray())
            {
                if (lever == LeverNames.Switch_Dirtmouth_Stag)
                {
                    Term term = lmb.GetTerm(ItemNames.Dirtmouth_Stag);
                    LogicItem item = new CappedItem(lever, new TermValue[] { new(term, 1) }, new(term, 1));
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
                }
            }
        }

        private static void ApplyEmbeddedOverrides(GenerationSettings gs, LogicManagerBuilder lmb, JsonLogicFormat fmt)
        {
            

            using Stream s = typeof(LogicPatcher).Assembly.GetManifestResourceStream("RandomizableLevers.Resources.Logic.LogicOverrides.json");
            lmb.DeserializeFile(LogicFileType.LogicEdit, fmt, s);
        }
        private static void ApplyEmbeddedSubstitutions(GenerationSettings gs, LogicManagerBuilder lmb, JsonLogicFormat fmt)
        {
            using Stream r = typeof(LogicPatcher).Assembly.GetManifestResourceStream("RandomizableLevers.Resources.Logic.LogicSubstitutions.json");
            lmb.DeserializeFile(LogicFileType.LogicSubst, fmt, r);
        }

        private static void AddLeverLocations(GenerationSettings gs, LogicManagerBuilder lmb, JsonLogicFormat fmt)
        {
            using Stream s = typeof(LogicPatcher).Assembly.GetManifestResourceStream("RandomizableLevers.Resources.Logic.LeverLocationLogic.json");
            lmb.DeserializeFile(LogicFileType.Locations, fmt, s);
        }
    }
}