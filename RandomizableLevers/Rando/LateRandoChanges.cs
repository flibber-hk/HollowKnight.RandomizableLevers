using System;
using System.Collections.Generic;
using System.Linq;
using ItemChanger;

namespace RandomizableLevers.Rando
{
    public static class LateRandoChanges
    {
        public static void Hook()
        {
            Events.AfterStartNewGame += RemoveExtraPlatforms;
        }

        private static void RemoveExtraPlatforms()
        {
            if (RandoInterop.Settings.RandomizeLevers && RandomizerMod.RandomizerMod.RS.GenerationSettings is not null)
            {
                // Remove platforms in Waterways_04 in case they arrive there from the Waterways exit gate
                List<IDeployer> deployers = ItemChanger.Internal.Ref.Settings.Deployers;

                foreach (IDeployer deployer in deployers)
                {
                    if (ShouldRemoveDeployer(deployer))
                    {
                        Events.RemoveSceneChangeEdit(deployer.SceneName, deployer.OnSceneChange);
                    }
                }

                deployers.RemoveAll(ShouldRemoveDeployer);
            }
        }

        public static bool ShouldRemoveDeployer(IDeployer d)
        {
            return d is SmallPlatform sp && sp.SceneName == SceneNames.Waterways_04;
        }
    }
}
