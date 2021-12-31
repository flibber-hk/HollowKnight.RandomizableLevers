using System;
using System.Linq;
using ItemChanger;
using RandomizerMod.RC;

namespace RandomizableLevers.Rando
{
    public static class RequestModifier
    {
        public static void Hook()
        {
            RequestBuilder.OnUpdate.Subscribe(5.3f, AddLevers);
        }

        private static void AddLevers(RequestBuilder rb)
        {
            if (!RandoInterop.Settings.RandomizeLevers)
            {
                return;
            }

            string[] leverPool = LeverNames.ToArray();
            if (!RandoInterop.Settings.StagsAsLevers)
            {
                leverPool = leverPool.Where(i => i != LeverNames.Switch_Dirtmouth_Stag && i != LeverNames.Lever_Resting_Grounds_Stag).ToArray();
            }
            if (rb.gs.LongLocationSettings.RandomizationInWhitePalace == RandomizerMod.Settings.LongLocationSettings.WPSetting.ExcludeWhitePalace)
            {
                leverPool = leverPool.Where(i => !i.StartsWith("Lever-Palace") && i != LeverNames.Lever_Path_of_Pain).ToArray();
            }
            else if (rb.gs.LongLocationSettings.RandomizationInWhitePalace == RandomizerMod.Settings.LongLocationSettings.WPSetting.ExcludePathOfPain)
            {
                leverPool = leverPool.Where(i => i != LeverNames.Lever_Path_of_Pain).ToArray();
            }

            if (RandoInterop.Settings.StagsAsLevers)
            {
                // Remove Dirtmouth and RG stag items, because we're randomizing them as levers
                if (rb.gs.PoolSettings.Stags)
                {
                    rb.RemoveItemByName(ItemNames.Dirtmouth_Stag);
                    rb.RemoveLocationByName(LocationNames.Dirtmouth_Stag);
                    rb.RemoveItemByName(ItemNames.Resting_Grounds_Stag);
                    rb.RemoveLocationByName(LocationNames.Resting_Grounds_Stag);

                    // Remove levers from start items
                    // It's easiest to simply not support this setting, plus if the stags are levers
                    // then they're not starting with stags :)
                    rb.StartItems.RemoveAll(ItemNames.Dirtmouth_Stag);
                    rb.StartItems.RemoveAll(ItemNames.Resting_Grounds_Stag);
                }
                else
                {
                    rb.Vanilla.RemoveAll(new(ItemNames.Dirtmouth_Stag, LocationNames.Dirtmouth_Stag));
                    rb.Vanilla.RemoveAll(new(ItemNames.Resting_Grounds_Stag, LocationNames.Resting_Grounds_Stag));
                }
            }

            if (!RandoInterop.Settings.LeversToLevers)
            {
                foreach (string lever in leverPool)
                {
                    rb.AddItemByName(lever);
                    rb.AddLocationByName(lever);
                }
            }
            else
            {
                StageBuilder sb = rb.MainItemStage;
                ItemGroupBuilder gb = sb.AddItemGroup("Lever Rando Group");
                gb.Items.AddRange(leverPool);
                gb.Locations.AddRange(leverPool);
            }

            foreach (string lever in LeverNames.ToArray().Except(leverPool))
            {
                rb.AddToVanilla(lever, lever);
            }
        }
    }
}
