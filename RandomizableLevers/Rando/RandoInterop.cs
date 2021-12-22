using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
}
