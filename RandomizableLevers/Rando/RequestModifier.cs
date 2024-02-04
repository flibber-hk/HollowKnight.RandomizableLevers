using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using ItemChanger;
using RandomizerCore.Randomization;
using RandomizerMod.RC;

namespace RandomizableLevers.Rando
{
    public static class RequestModifier
    {
        public static void Hook()
        {
            // Define the lever group, add the group matcher, and define item and location defs.
            RequestBuilder.OnUpdate.Subscribe(-499.7f, SetupRefs);

            // Add levers to the pool shortly after the rest of the items; they will be
            // added to the correct group if necessary because of the matcher we added in SetupRefs.
            // Settings like the RandomizationInWhitePalace setting will not correctly be applied
            // by the builtin request, because we need to mark the levers as vanilla if unrandomized.
            RequestBuilder.OnUpdate.Subscribe(0.3f, AddLevers);

            // The deranged constraint must be applied separately
            RequestBuilder.OnUpdate.Subscribe(100.3f, DerangeLevers);
        }

        private static void DerangeLevers(RequestBuilder rb)
        {
            if (!rb.gs.CursedSettings.Deranged) return;
            if (!RandoInterop.Settings.Any) return;

            foreach (ItemGroupBuilder igb in rb.EnumerateItemGroups())
            {
                if (igb.strategy is DefaultGroupPlacementStrategy dgps)
                {
                    HashSet<string> leverNames = new(LeverNames.ToArray());
                    dgps.ConstraintList.Add(new DefaultGroupPlacementStrategy.Constraint(
                        (x, y) => !(leverNames.Contains(x.Name) && x.Name == y.Name),
                        Label: "Lever Derangement"
                        ));
                }
            }
        }

        private static void SetupRefs(RequestBuilder rb)
        {
            if (!RandoInterop.Settings.Any)
            {
                return;
            }

            foreach (string lever in LeverNames.ToArray())
            {
                rb.EditItemRequest(lever, info =>
                {
                    info.getItemDef = () => new()
                    {
                        Name = lever,
                        Pool = "Levers",
                        MajorItem = false,
                        PriceCap = 500
                    };
                });
                rb.EditLocationRequest(lever, info =>
                {
                    string sceneName = Finder.GetLocation(lever).sceneName;

                    info.getLocationDef = () => new()
                    {
                        Name = lever,
                        SceneName = sceneName,
                        FlexibleCount = false,
                        AdditionalProgressionPenalty = false
                    };
                });
            }

            HashSet<string> leverNames = new(LeverNames.ToArray());
            
            if (rb.gs.SplitGroupSettings.RandomizeOnStart && RandoInterop.Settings.LeverGroup >= 0 && RandoInterop.Settings.LeverGroup <= 2)
            {
                RandoInterop.Settings.LeverGroup = rb.rng.Next(3);
            }
            // If the value is 0 or -1, then levers will be placed in the main item group by default, so we don't need a resolver.
            if (RandoInterop.Settings.LeverGroup > 0)
            {
                ItemGroupBuilder leverGroup = null;
                string label = RBConsts.SplitGroupPrefix + RandoInterop.Settings.LeverGroup;
                foreach (ItemGroupBuilder igb in rb.EnumerateItemGroups())
                {
                    if (igb.label == label)
                    {
                        leverGroup = igb;
                        break;
                    }
                }
                leverGroup ??= rb.MainItemStage.AddItemGroup(label);

                rb.OnGetGroupFor.Subscribe(0.01f, ResolveLeverGroup);
                bool ResolveLeverGroup(RequestBuilder rb, string item, RequestBuilder.ElementType type, out GroupBuilder gb)
                {
                    if (type == RequestBuilder.ElementType.Transition)
                    {
                        gb = default;
                        return false;
                    }

                    if (!leverNames.Contains(item))
                    {
                        gb = default;
                        return false;
                    }

                    gb = leverGroup;
                    return true;
                }
            }
        }

        private static void AddLevers(RequestBuilder rb)
        {
            if (!RandoInterop.Settings.Any)
            {
                return;
            }

            if (!RandoInterop.Settings.RandomizeLevers)
            {
                foreach (string lever in LeverNames.ToArray())
                {
                    if (lever == LeverNames.Switch_Dirtmouth_Stag || lever == LeverNames.Lever_Resting_Grounds_Stag)
                    {
                        continue;
                    }

                    rb.AddToVanilla(lever, lever);
                }

                return;
            }

            // Remove Dirtmouth and RG stag items, because we're randomizing them as levers
            {
                rb.RemoveItemByName(ItemNames.Dirtmouth_Stag);
                rb.RemoveLocationByName(LocationNames.Dirtmouth_Stag);
                rb.StartItems.RemoveAll(ItemNames.Dirtmouth_Stag);
                rb.RemoveFromVanilla(LocationNames.Dirtmouth_Stag, ItemNames.Dirtmouth_Stag);

                rb.RemoveItemByName(ItemNames.Resting_Grounds_Stag);
                rb.RemoveLocationByName(LocationNames.Resting_Grounds_Stag);
                rb.StartItems.RemoveAll(ItemNames.Resting_Grounds_Stag);
                rb.RemoveFromVanilla(LocationNames.Resting_Grounds_Stag, ItemNames.Resting_Grounds_Stag);
            }

            string[] leverPool = LeverNames.ToArray();
            if (rb.gs.LongLocationSettings.WhitePalaceRando == RandomizerMod.Settings.LongLocationSettings.WPSetting.ExcludeWhitePalace)
            {
                leverPool = leverPool.Where(i => !i.StartsWith("Lever-Palace") && i != LeverNames.Lever_Path_of_Pain).ToArray();
            }
            else if (rb.gs.LongLocationSettings.WhitePalaceRando == RandomizerMod.Settings.LongLocationSettings.WPSetting.ExcludePathOfPain)
            {
                leverPool = leverPool.Where(i => i != LeverNames.Lever_Path_of_Pain).ToArray();
            }

            // These will get added to the lever group because of the GroupResolver we applied.
            foreach (string lever in leverPool)
            {
                rb.AddItemByName(lever);
                rb.AddLocationByName(lever);
            }
            foreach (string lever in LeverNames.ToArray().Except(leverPool))
            {
                rb.AddToVanilla(lever, lever);
            }
        }
    }
}
