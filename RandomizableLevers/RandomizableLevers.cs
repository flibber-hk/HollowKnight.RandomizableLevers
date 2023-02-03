using ItemChanger;
using ItemChanger.Tags;
using Modding;
using RandomizableLevers.IC;
using System;
using System.Linq;

namespace RandomizableLevers
{
    public class RandomizableLevers : Mod, IGlobalSettings<GlobalSettings>
    {
        internal static RandomizableLevers instance;
        
        public RandomizableLevers() : base(null)
        {
            instance = this;
        }
        
        public override string GetVersion()
        {
            return GetType().Assembly.GetName().Version.ToString();
        }

        public static GlobalSettings GS { get; set; } = new();
        public void OnLoadGlobal(GlobalSettings s)
        {
            if (s.RandoSettings == null)
            {
                LogDebug("GlobalSettings: RandoSettings null");
                s.RandoSettings = new();
            }
            GS = s;
        }
        public GlobalSettings OnSaveGlobal() => GS;

        public override void Initialize()
        {
            Log("Initializing Mod...");

            HookItemChanger();

            if (ModHooks.GetMod("Randomizer 4") is Mod)
            {
                Rando.RandoInterop.HookRandomizer();
            }
        }

        private void HookItemChanger()
        {
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

            Finder.GetLocationOverride += ReplaceStagLevers;
        }

        private void ReplaceStagLevers(GetLocationEventArgs args)
        {
            if (!GS.LeverStagLocations) return;

            AbstractLocation newLoc;
            switch (args.LocationName)
            {
                case LocationNames.Dirtmouth_Stag:
                    newLoc = Finder.GetLocationInternal(LeverNames.Switch_Dirtmouth_Stag);
                    newLoc.name = args.Current?.name ?? args.LocationName;
                    break;
                case LocationNames.Resting_Grounds_Stag:
                    newLoc = Finder.GetLocationInternal(LeverNames.Lever_Resting_Grounds_Stag);
                    newLoc.name = args.Current?.name ?? args.LocationName;
                    break;

                default:
                    return;
            }

            newLoc.tags = newLoc.tags.Where(t => !IsLeverCmiTag(t)).ToList();

            args.Current = newLoc;
        }

        private bool IsLeverCmiTag(Tag tag)
        {
            if (tag is not InteropTag it) return false;
            if (!it.TryGetProperty("ModSource", out string source)) return false;
            if (source != "RandomizableLevers") return false;

            return true;
        }
    }
}