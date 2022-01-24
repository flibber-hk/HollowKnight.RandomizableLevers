using ItemChanger;

namespace RandomizableLevers.IC.BridgeLevers
{
    public class BridgeLeverActionItem : AbstractItem
    {
        public int bridgeNum;

        public override string GetPreferredContainer() => LeverContainer.Lever;
        public override void GiveImmediate(GiveInfo info)
        {
            ItemChangerMod.Modules.GetOrAdd<BridgeLeverModule>().OpenBridge(bridgeNum);
        }

        public override bool Redundant() => ItemChangerMod.Modules.GetOrAdd<BridgeLeverModule>().IsOpened(bridgeNum);
    }
}
