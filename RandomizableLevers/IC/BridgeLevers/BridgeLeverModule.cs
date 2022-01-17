using System;
using System.Collections;
using System.Linq;
using Modding;
using UnityEngine;
using UnityEngine.SceneManagement;
using ItemChanger;
using ItemChanger.Extensions;
using ItemChanger.Internal;
using ItemChanger.Modules;

namespace RandomizableLevers.IC.BridgeLevers
{
    /// <summary>
    /// Module which manages the levers and bridges in Fungus2_21.
    /// </summary>
    public class BridgeLeverModule : Module
    {
        public static readonly Vector2 BridgeLever1Position = new Vector2(69.1f, 12.7f);
        public static readonly Vector2 BridgeLever2Position = new Vector2(111.1f, 12.8f);
        
        // Bools to mark that the bridges have been opened - set by the relevant Items
        public bool OpenedBridge1 { get; set; } = false;
        public bool OpenedBridge2 { get; set; } = false;

        // Event raised when a bridge lever is hit. Return true to override the original behaviour.
        public event Func<int, bool> OnHitBridgeLever;

        public override void Initialize()
        {
            Events.AddSceneChangeEdit(SceneNames.Fungus2_21, OpenBridgesOnSceneEntry);
            On.BridgeLever.Start += PatchBridgeLeverOnSceneEntry;
            On.BridgeLever.OpenBridge += PatchBridgeLeverOnHit;
        }

        public override void Unload()
        {
            Events.RemoveSceneChangeEdit(SceneNames.Fungus2_21, OpenBridgesOnSceneEntry);
            On.BridgeLever.Start -= PatchBridgeLeverOnSceneEntry;
            On.BridgeLever.OpenBridge -= PatchBridgeLeverOnHit;
        }

        #region Levers
        public bool CheckOpenedLever(int bridgeNum)
        {
            if (bridgeNum == 1)
            {
                if (Ref.Settings?.Placements?.TryGetValue(LeverNames.Lever_Pilgrims_Way_Left, out AbstractPlacement pmt) ?? false)
                {
                    return pmt.CheckVisitedAny(VisitState.Opened);
                }
                else
                {
                    return PlayerData.instance.GetBool(nameof(PlayerData.cityBridge1));
                }
            }
            else
            {
                if (Ref.Settings?.Placements?.TryGetValue(LeverNames.Lever_Pilgrims_Way_Right, out AbstractPlacement pmt) ?? false)
                {
                    return pmt.CheckVisitedAny(VisitState.Opened);
                }
                else
                {
                    return PlayerData.instance.GetBool(nameof(PlayerData.cityBridge2));
                }
            }
        }

        // Rewriting the function in an on hook omegamaggotprime but it makes no sense to use an IL hook to cut away most of the function
        // Note that rewriting is necessary even if the placement does not exist so that the item can exist too.
        private void PatchBridgeLeverOnSceneEntry(On.BridgeLever.orig_Start orig, BridgeLever self)
        {
            int leverNum = self.playerDataBool.EndsWith("1") ? 1 : 2;
            bool activated = CheckOpenedLever(leverNum);

            ReflectionHelper.SetField(self, "activated", activated);
            if (activated)
            {
                self.GetComponent<tk2dSpriteAnimator>().Play("Lever Activated");
            }
        }

        private IEnumerator PatchBridgeLeverOnHit(On.BridgeLever.orig_OpenBridge orig, BridgeLever self)
        {
            int leverNum = self.playerDataBool.EndsWith("1") ? 1 : 2;
            // Always call orig(self) so subscribers to the on hook get to execute
            IEnumerator originalEnumerator = orig(self);

            bool modified = false;
            if (OnHitBridgeLever != null)
            {
                foreach (Func<int, bool> toInvoke in OnHitBridgeLever.GetInvocationList())
                {
                    modified |= toInvoke(leverNum);
                }
            }
            if (!modified)
            {
                if (leverNum == 1)
                {
                    OpenedBridge1 = true;
                }
                else 
                { 
                    OpenedBridge2 = true; 
                }
                return originalEnumerator;
            }

            IEnumerator flipLever()
            {
                // Set the pd bool just in case they deactivate this mod, I guess
                PlayerData.instance.SetBool(self.playerDataBool, true);
                self.switchSound.SpawnAndPlayOneShot(self.audioPlayerPrefab, self.transform.position);
                GameManager.instance.FreezeMoment(1);
                GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
                if (self.strikeNailPrefab)
                {
                    self.strikeNailPrefab.Spawn(self.hitOrigin.position);
                }
                self.GetComponent<tk2dSpriteAnimator>().Play("Lever Hit");
                yield return null;
            }

            return flipLever();
        }
        #endregion

        #region Bridges
        public void OpenBridge(int bridgeNum)
        {
            if (bridgeNum == 1) OpenedBridge1 = true;
            else OpenedBridge2 = true;

            if (GameManager.instance.sceneName == SceneNames.Fungus2_21)
            {
                OpenBridgeInScene(bridgeNum);
            }
        }

        private void OpenBridgeInScene(int bridgeNum)
        {
            GameObject.Find($"Bridge Collider {bridgeNum}").GetComponent<Collider2D>().enabled = true;

            BridgeLever lever = UnityEngine.Object.FindObjectsOfType<BridgeLever>(true).First(x => x.name == $"Bridge Lever {bridgeNum}");

            foreach (BridgeSection section in UnityEngine.Object.FindObjectsOfType<BridgeSection>())
            {
                if (bridgeNum == 1 && section.transform.position.x < BridgeLever1Position.x
                    || bridgeNum == 2 && section.transform.position.x > BridgeLever1Position.x)
                {
                    // The BridgeLever parameter is only used for its position
                    section.Open(lever, true);
                }
            }
        }

        private void OpenBridgesOnSceneEntry(Scene scene)
        {
            scene.FindGameObjectByName("Bridge Collider 1").GetComponent<Collider2D>().enabled = OpenedBridge1;
            scene.FindGameObjectByName("Bridge Collider 2").GetComponent<Collider2D>().enabled = OpenedBridge2;

            foreach (BridgeSection section in UnityEngine.Object.FindObjectsOfType<BridgeSection>())
            {
                if ((OpenedBridge1 && section.transform.position.x < BridgeLever1Position.x) 
                    || (OpenedBridge2 && section.transform.position.x > BridgeLever1Position.x))
                {
                    try
                    {
                        section.Open(null, false);
                    }
                    catch (Exception ex)
                    {
                        RandomizableLevers.instance.LogError($"Error opening bridge section {section.name}" + ex);
                    }
                }
            }
        }
        #endregion
    }
}
