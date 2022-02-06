using System;
using System.Collections.Generic;
using System.Linq;
using ItemChanger;
using RandomizerMod.RC;

namespace RandomizableLevers.Rando
{
    public static class LateRandoChanges
    {
        public static void Hook()
        {
            RandoController.OnExportCompleted += RemoveExtraPlatforms;
        }

        private static void RemoveExtraPlatforms(RandoController rc)
        {
            if (rc.ctx.itemPlacements.Any(x => x.Item.Name == LeverNames.Lever_Waterways_Hwurmp_Arena))
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
