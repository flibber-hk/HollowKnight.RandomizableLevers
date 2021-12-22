using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuChanger;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using MenuChanger.Extensions;
using RandomizerMod.Menu;

namespace RandomizableLevers.Rando
{
    public static class RandoMenuPage
    {
        internal static MenuPage LeverRandoPage;
        internal static MenuElementFactory<LeverRandomizationSettings> leverMEF;
        internal static VerticalItemPanel leverVIP;
        
        internal static SmallButton JumpToLeverRandoButton;

        public static void Hook()
        {
            RandomizerMenuAPI.AddMenuPage(ConstructMenu, HandleButton);
        }

        private static bool HandleButton(MenuPage landingPage, out SmallButton button)
        {
            JumpToLeverRandoButton = new(landingPage, "Levers");
            JumpToLeverRandoButton.AddHideAndShowEvent(landingPage, LeverRandoPage);
            button = JumpToLeverRandoButton;
            return true;
        }


        private static void ConstructMenu(MenuPage landingPage)
        {
            LeverRandoPage = new MenuPage("Levers", landingPage);
            leverMEF = new(LeverRandoPage, RandoInterop.Settings);
            leverVIP = new(LeverRandoPage, new(0, 300), 50f, false, leverMEF.Elements);
        }
    }
}
