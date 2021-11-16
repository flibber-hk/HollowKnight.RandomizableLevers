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

            if (LeverDB.ModifyLeverDefinitions())
            {
                Serializer.SerializeLevers();
            }

            LanguageData.Load();

            // TODO - get an "on hook itemchanger" event, and subscribe this to that
            LanguageData.Hook();

            On.UIManager.StartNewGame += UIManager_StartNewGame;

            
        }

        

        private void UIManager_StartNewGame(On.UIManager.orig_StartNewGame orig, UIManager self, bool permaDeath, bool bossRush)
        {
            ItemChangerMod.CreateSettingsProfile(false, false);
            ItemChangerMod.Modules.GetOrAdd<TransitionFixes>();

            AbstractPlacement pmt = Finder.GetLocation(LeverNames.Lever_Dirtmouth_Elevator).Wrap();
            pmt.Add(Finder.GetItem(ItemNames.Soul_Refill));
            pmt.Add(Finder.GetItem(LeverNames.Lever_Emilitia));
            pmt.Add(Finder.GetItem(ItemNames.Herrah));
            pmt.Add(Finder.GetItem(ItemNames.Lumafly_Escape));
            ItemChangerMod.AddPlacements(pmt.Yield());

            // ItemChangerMod.AddPlacements(GetRandomPlacements());

            orig(self, permaDeath, bossRush);
        }

        private List<AbstractPlacement> GetRandomPlacements()
        {
            List<AbstractPlacement> ret = null;
            int ct = 0;

            Random rand = new Random();

            while (ret == null)
            {
                Log($"Attempt {ct++}...");
                ret = TryGetRandomPlacements(rand);
            }

            return ret;
        }

        private List<AbstractPlacement> TryGetRandomPlacements(Random rand)
        {
            List<AbstractPlacement> placements = LeverNames.ToArray().Select(x => Finder.GetLocation(x).Wrap()).ToList();
            List<AbstractItem> items = LeverNames.ToArray().Select(x => Finder.GetItem(x)).ToList();

            Dictionary<string, AbstractLocation> itemToLoc = new();

            while (items.Count > 0)
            {
                int n = rand.Next(items.Count);
                placements[items.Count - 1].Add(items[n]);

                itemToLoc[items[n].name] = Finder.GetLocation(placements[items.Count - 1].Name);
                
                items.RemoveAt(n);
            }

            if (itemToLoc[LeverNames.Lever_Mantis_Lords_Access].sceneName == SceneNames.Fungus2_15) return null;

            bool inSanctum(string locName)
            {
                return locName == LeverNames.Lever_Sanctum_Bottom
                    || locName == LeverNames.Lever_Sanctum_East
                    || locName == LeverNames.Lever_Sanctum_West_Lower
                    || locName == LeverNames.Lever_Sanctum_West_Upper
                    || locName == LeverNames.Lever_Below_Spell_Twister;
            }

            if (inSanctum(itemToLoc[LeverNames.Lever_Sanctum_Soul_Warrior].name) && inSanctum(itemToLoc[LeverNames.Lever_Sanctum_Bottom].name)) return null;

            if (itemToLoc[LeverNames.Lever_Palace_Entrance_Orb].sceneName.Contains("Palace") && itemToLoc[LeverNames.Lever_Palace_Entrance_Orb].name != LeverNames.Lever_Palace_Entrance_Orb) return null;

            if (itemToLoc[LeverNames.Lever_Palace_Left_Orb].name == LeverNames.Lever_Path_of_Pain || itemToLoc[LeverNames.Lever_Palace_Left_Orb].name == LeverNames.Lever_Palace_Final) return null;
            if (itemToLoc[LeverNames.Lever_Palace_Right_Orb].name == LeverNames.Lever_Path_of_Pain || itemToLoc[LeverNames.Lever_Palace_Right_Orb].name == LeverNames.Lever_Palace_Final) return null;
            if (itemToLoc[LeverNames.Lever_Palace_Right].name == LeverNames.Lever_Path_of_Pain || itemToLoc[LeverNames.Lever_Palace_Right].name == LeverNames.Lever_Palace_Final) return null;
            if (itemToLoc[LeverNames.Lever_Palace_Right].name == LeverNames.Lever_Palace_Right_Orb) return null;
            return placements;
        }
    }
}