using ItemChanger;
using ItemChanger.Locations;
using UnityEngine;

namespace RandomizableLevers.IC.BridgeLevers
{
    public class BridgeLeverLocation : AutoLocation
    {
        public int bridgeNum;

        protected override void OnLoad()
        {
            ItemChangerMod.Modules.GetOrAdd<BridgeLeverModule>().OnHitBridgeLever += OnHitLever;
        }
        protected override void OnUnload()
        {
            ItemChangerMod.Modules.Get<BridgeLeverModule>().OnHitBridgeLever -= OnHitLever;
        }

        public override GiveInfo GetGiveInfo()
        {
            GiveInfo info = base.GetGiveInfo();
            info.MessageType = MessageType.Corner;
            if (GameManager.instance.sceneName == sceneName)
            {
                info.Transform = GameObject.Find($"Bridge Lever {bridgeNum}").transform;
            }            
            return info;
        }

        private bool OnHitLever(int arg)
        {
            if (arg != bridgeNum) return false;

            Placement.GiveAll(GetGiveInfo());
            return true;
        }
    }
}
