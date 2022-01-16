namespace RandomizableLevers.Rando
{
    internal static class RandoInterop
    {
        public static LeverRandomizationSettings Settings => RandomizableLevers.GS.RandoSettings;

        public static void HookRandomizer()
        {
            LogicPatcher.Hook();
            RandoMenuPage.Hook();
            RequestModifier.Hook();
            LateRandoChanges.Hook();

            RandomizerMod.Logging.SettingsLog.AfterLogSettings += AddLeverRandoSettings;
        }

        private static void AddLeverRandoSettings(RandomizerMod.Logging.LogArguments args, System.IO.TextWriter tw)
        {
            tw.WriteLine("Logging Lever Rando settings:");
            using Newtonsoft.Json.JsonTextWriter jtw = new(tw) { CloseOutput = false, };
            RandomizerMod.RandomizerData.JsonUtil._js.Serialize(jtw, Settings);
            tw.WriteLine();
        }
    }
}
