using MenuChanger;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using MenuChanger.Extensions;
using RandomizerMod.Menu;
using static RandomizerMod.Localization;

namespace RandomizableLevers.Rando
{
    public class RandoMenuPage
    {
        internal MenuPage LeverRandoPage;
        internal MenuElementFactory<LeverRandomizationSettings> leverMEF;
        internal VerticalItemPanel leverVIP;
        
        internal SmallButton JumpToLeverRandoButton;
        internal static RandoMenuPage Instance { get; private set; }

        public static void OnExitMenu()
        {
            Instance = null;
        }

        public static void Hook()
        {
            RandomizerMenuAPI.AddMenuPage(ConstructMenu, HandleButton);
            MenuChangerMod.OnExitMainMenu += OnExitMenu;
        }

        private static bool HandleButton(MenuPage landingPage, out SmallButton button)
        {
            button = Instance.JumpToLeverRandoButton;
            return true;
        }

        private void SetTopLevelButtonColor()
        {
            if (JumpToLeverRandoButton != null)
            {
                JumpToLeverRandoButton.Text.color = RandoInterop.Settings.Any ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
            }
        }

        private static void ConstructMenu(MenuPage landingPage) => Instance = new(landingPage);

        private RandoMenuPage(MenuPage landingPage)
        {
            LeverRandoPage = new MenuPage(Localize("Levers"), landingPage);
            leverMEF = new(LeverRandoPage, RandoInterop.Settings);
            leverVIP = new(LeverRandoPage, new(0, 300), 75f, true, leverMEF.Elements);
            Localize(leverMEF);

            leverMEF.ElementLookup[nameof(LeverRandomizationSettings.RandomizeLevers)].SelfChanged += _ => RandomizerMenuAPI.Menu.UpdateStartLocationSwitch();
            foreach (IValueElement e in leverMEF.Elements)
            {
                e.SelfChanged += obj => SetTopLevelButtonColor();
            }

            JumpToLeverRandoButton = new(landingPage, Localize("Levers"));
            JumpToLeverRandoButton.AddHideAndShowEvent(landingPage, LeverRandoPage);
            SetTopLevelButtonColor();
        }
    }
}
