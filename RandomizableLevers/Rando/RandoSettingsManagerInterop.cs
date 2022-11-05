using RandomizableLevers.Rando;
using RandoSettingsManager;
using RandoSettingsManager.SettingsManagement;
using RandoSettingsManager.SettingsManagement.Versioning;
using GlobalSettings = RandomizableLevers.GlobalSettings;

namespace RandoPlus.Rando
{
    internal static class RandoSettingsManagerInterop
    {
        public static void Hook()
        {
            RandoSettingsManagerMod.Instance.RegisterConnection(new LeverRandoSettingsProxy());
        }
    }

    internal class LeverRandoSettingsProxy : RandoSettingsProxy<LeverRandomizationSettings, string>
    {
        public override string ModKey => RandomizableLevers.RandomizableLevers.instance.GetName();

        public override VersioningPolicy<string> VersioningPolicy { get; }
            = new EqualityVersioningPolicy<string>(RandomizableLevers.RandomizableLevers.instance.GetVersion());

        public override void ReceiveSettings(LeverRandomizationSettings settings)
        {
            settings ??= new();
            RandoMenuPage.Instance.leverMEF.SetMenuValues(settings);
        }

        public override bool TryProvideSettings(out LeverRandomizationSettings settings)
        {
            settings = RandoInterop.Settings;
            return settings.Any;
        }
    }
}
