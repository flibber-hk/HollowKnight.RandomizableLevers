using ItemChanger;
using ItemChanger.Extensions;
using ItemChanger.Locations;
using ItemChanger.Placements;
using ItemChanger.Util;
using System;
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
            if (WillBeReplaced())
            {
                Events.AddSceneChangeEdit(sceneName, ReplaceOnSceneChange);
            }
            else
            {
                ItemChangerMod.Modules.GetOrAdd<BridgeLeverModule>().OnHitBridgeLever += OnHitLever;
                Events.AddSceneChangeEdit(sceneName, SpawnShinies);
            }
        }

        private void SpawnShinies(Scene scene)
        {
            if (!Placement.CheckVisitedAny(VisitState.Opened)) return;

            GameObject target = scene.FindGameObject(ObjectName);

            foreach (AbstractItem item in Placement.Items)
            {
                if (!item.IsObtained())
                {
                    GameObject shiny = ShinyUtility.MakeNewShiny(Placement, item, flingType);
                    shiny.transform.SetPosition2D(target.transform.position);
                    ShinyUtility.FlingShinyRandomly(shiny.LocateFSM("Shiny Control"));
                }
            }
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
            ItemChangerMod.Modules.GetOrAdd<BridgeLeverModule>().OnHitBridgeLever -= OnHitLever;
            Events.RemoveSceneChangeEdit(sceneName, ReplaceOnSceneChange);
            Events.RemoveSceneChangeEdit(sceneName, SpawnShinies);
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

            Placement.AddVisitFlag(VisitState.Opened);
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
