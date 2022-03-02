using ItemChanger;
using Modding;
using RandomizableLevers.IC;

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
        }
    }
}