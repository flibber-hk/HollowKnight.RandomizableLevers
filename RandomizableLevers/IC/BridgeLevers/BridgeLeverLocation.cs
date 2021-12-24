using ItemChanger;
using ItemChanger.Extensions;
using ItemChanger.Locations;
using ItemChanger.Placements;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RandomizableLevers.IC.BridgeLevers
{
    public class BridgeLeverLocation : ExistingContainerLocation
    {
        public int bridgeNum;
        public float elevation;

        [Newtonsoft.Json.JsonIgnore]
        public string ObjectName => $"Bridge Lever {bridgeNum}";

        protected override void OnLoad()
        {
            ItemChangerMod.Modules.GetOrAdd<BridgeLeverModule>().OnHitBridgeLever += OnHitLever;
            if (WillBeReplaced()) Events.AddSceneChangeEdit(sceneName, ReplaceOnSceneChange);
        }

        private void ReplaceOnSceneChange(Scene scene)
        {
            Container c = Container.GetContainer(Placement.MainContainerType);
            GameObject obj = c.GetNewContainer(Placement, Placement.Items, flingType, (Placement as ISingleCostPlacement)?.Cost);
            GameObject target = scene.FindGameObject(ObjectName);
            c.ApplyTargetContext(obj, target, elevation);
            target.SetActive(false); // Don't destroy because we need the target available to open the bridge
        }

        protected override void OnUnload()
        {
            ItemChangerMod.Modules.Get<BridgeLeverModule>().OnHitBridgeLever -= OnHitLever;
            Events.RemoveSceneChangeEdit(sceneName, ReplaceOnSceneChange);
        }

        public GiveInfo GetGiveInfo()
        {
            GiveInfo info = new()
            {
                FlingType = flingType,
                Callback = null,
                Container = Container.Unknown,
                MessageType = MessageType.Corner,
            };

            if (GameManager.instance.sceneName == sceneName)
            {
                info.Transform = GameObject.Find(ObjectName).transform;
            }            
            return info;
        }

        private bool OnHitLever(int arg)
        {
            if (arg != bridgeNum) return false;

            Placement.GiveAll(GetGiveInfo());
            return true;
        }

        public override AbstractPlacement Wrap()
        {
            return new ExistingContainerPlacement(name)
            {
                Location = this,
            };
        }
    }
}
