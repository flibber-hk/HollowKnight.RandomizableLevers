using System.Collections.Generic;
using Modding;
using RandomizerMod.RandomizerData;
using RandomizerMod.Logging;

namespace RandomizableLevers.Rando
{
    internal static class RandoInterop
    {
        public static LeverRandomizationSettings Settings
        {
            get => RandomizableLevers.GS.RandoSettings;
            set => RandomizableLevers.GS.RandoSettings = value;
        }

        public static void HookRandomizer()
        {
            LogicPatcher.Hook();
            RandoMenuPage.Hook();
            RequestModifier.Hook();
            MiscRandoChanges.Hook();

            SettingsLog.AfterLogSettings += AddLeverRandoSettings;

            // Add important levers to the condensed spoiler log
            CondensedSpoilerLogger.AddCategory("Access Levers", (args) => true, AccessLevers);
            CondensedSpoilerLogger.AddCategory("Useful Levers", (args) => true, UsefulLevers);
            CondensedSpoilerLogger.AddCategory("Palace Levers", (args) => true, PalaceLevers);

            if (ModHooks.GetMod(nameof(CondensedSpoilerLogger)) is not null)
            {
                LogManager.AddLogger(new LeverByAreaLog());
            }

            RandoVanillaTracker.AddInterop("Levers", GetVanillaPlacements);

            if (ModHooks.GetMod("RandoSettingsManager") is not null)
            {
                RandoSettingsManagerInterop.Hook();
            }
        }

        private static void AddLeverRandoSettings(LogArguments args, System.IO.TextWriter tw)
        {
            tw.WriteLine("Logging Lever Rando settings:");
            using Newtonsoft.Json.JsonTextWriter jtw = new(tw) { CloseOutput = false, };
            RandomizerMod.RandomizerData.JsonUtil._js.Serialize(jtw, Settings);
            tw.WriteLine();
        }

        private static readonly List<string> AccessLevers = new()
        {
            LeverNames.Lever_Mantis_Claw,
            LeverNames.Lever_Mantis_Lords_Access,
            LeverNames.Lever_Sanctum_Soul_Warrior,
            LeverNames.Lever_Sanctum_Bottom,
            LeverNames.Lever_Failed_Tramway_Left,
            LeverNames.Lever_Failed_Tramway_Right,
            LeverNames.Lever_City_Spire_Sentry_Lower,
            LeverNames.Lever_City_Spire_Sentry_Upper,
            LeverNames.Lever_City_Bridge_Above_Fountain,
        };

        private static readonly List<string> UsefulLevers = new()
        {
            LeverNames.Switch_Dirtmouth_Stag,
            LeverNames.Lever_Resting_Grounds_Stag,
            LeverNames.Lever_Queens_Gardens_Ground_Block,
            LeverNames.Switch_Petra_Arena,
            LeverNames.Switch_Queens_Gardens_Stag,
            LeverNames.Lever_Waterways_Hwurmp_Arena,
            LeverNames.Lever_City_Storerooms,
            LeverNames.Switch_Lower_Resting_Grounds,
        };

        private static readonly List<string> PalaceLevers = new()
        {
            LeverNames.Lever_Palace_Entrance_Orb,
            LeverNames.Lever_Palace_Left_Orb,
            LeverNames.Lever_Palace_Right_Orb,
            LeverNames.Lever_Palace_Atrium,
            LeverNames.Lever_Palace_Right,
            LeverNames.Lever_Palace_Final,
            LeverNames.Lever_Path_of_Pain,
        };

        private static List<VanillaDef> GetVanillaPlacements()
        {
            List<VanillaDef> vanillaDefs = new();
            foreach (string lever in LeverNames.ToArray())
            {
                vanillaDefs.Add(new VanillaDef(lever, lever));
            }
            return vanillaDefs;
        }
    }
}
