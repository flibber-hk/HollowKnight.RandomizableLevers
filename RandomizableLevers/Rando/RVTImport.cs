using System;
using System.Collections.Generic;
using MonoMod.ModInterop;
using RandomizerMod.RandomizerData;

namespace RandomizableLevers.Rando
{
    internal static class RandoVanillaTracker
    {
        [ModImportName("RandoVanillaTracker")]
        private static class RandoVanillaTrackerImport
        {
            public static Action<string, Func<bool>, Func<List<VanillaDef>>> AddInterop = null;
        }
        static RandoVanillaTracker()
        {
            typeof(RandoVanillaTracker).ModInterop();
        }

        public static void AddInterop(string pool, Func<bool> RandomizePool, Func<List<VanillaDef>> GetPlacements)
            => RandoVanillaTrackerImport.AddInterop?.Invoke(pool, RandomizePool, GetPlacements);
    }
}
