using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ItemChanger;
using ItemChanger.Extensions;
using ItemChanger.Locations;
using ItemChanger.Modules;
using ItemChanger.UIDefs;
using Modding;
using RandomizableLevers.IC;

namespace RandomizableLevers
{
    public class RandomizableLevers : Mod
    {
        internal static RandomizableLevers instance;
        
        public RandomizableLevers() : base(null)
        {
            instance = this;
        }
        
		public override string GetVersion()
		{
			return "0.1";
		}

        public override void Initialize()
        {
            Log("Initializing Mod...");

            #region Hook Itemchanger
            Serializer.DeserializeLevers();

            // Define the lever container, so the lever locations (which are implemented as ExistingContainerLocations) know how to work
            Container.DefineContainer<LeverContainer>();

            // Define items and locations in Finder, so any mods calling GetItem and GetLocation with the name of one of the levers
            // are given the item/location they expect
            foreach (string name in LeverNames.ToArray())
            {
                if (Serializer.Items.TryGetValue(name, out AbstractItem item))
                {
                    Finder.DefineCustomItem(item.Clone());
                }
                else
                {
                    LogWarn($"Lever item definition not found! " + name);
                }

                if (Serializer.Locations.TryGetValue(name, out AbstractLocation loc))
                {
                    Finder.DefineCustomLocation(loc.Clone());
                }
                else
                {
                    LogWarn($"Lever location definition not found! " + name);
                }
            }

            LanguageData.Load();

            ItemChanger.Events.OnItemChangerHook += LanguageData.Hook;
            ItemChanger.Events.OnItemChangerUnhook += LanguageData.Unhook;
            #endregion

            if (LeverDB.ModifyLeverDefinitions())
            {
                Serializer.SerializeLevers();
            }

            // if (ModHooks.GetMod("Randomizer 4") is Mod)
            //    Rando.LmbPatcher.Hook();

            On.UIManager.StartNewGame += UIManager_StartNewGame;
        }

        private void UIManager_StartNewGame(On.UIManager.orig_StartNewGame orig, UIManager self, bool permaDeath, bool bossRush)
        {
            ItemChangerMod.CreateSettingsProfile(false, false);
            ItemChangerMod.Modules.GetOrAdd<TransitionFixes>();

            Dictionary<string, string> pairs = Vanilla.GetRandomPairs(new Random());
            foreach (KeyValuePair<string, string> pair in pairs)
            {
                AbstractPlacement pmt = Finder.GetLocation(pair.Key).Wrap();
                pmt.Add(Finder.GetItem(pair.Value));
                ItemChangerMod.AddPlacements(pmt.Yield());
            }

            orig(self, permaDeath, bossRush);
        }
    }
}