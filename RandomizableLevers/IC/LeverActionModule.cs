using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ItemChanger;
using ItemChanger.Extensions;
using ItemChanger.Modules;

namespace RandomizableLevers.IC
{
    public enum LeverType
    {
        // e.g. City levers
        Ruins,
        // Dung defender acid lever
        WaterwaysCrank,
        // e.g. Mantis Village levers
        Mantis,
        // e.g. Peaks levers
        Mines,
        // Peaks Crushers levers
        MinesCrushers,
        // Palace levers
        Palace,
        // Palace orb levers
        PalaceOrb,
        // e.g. Dirtmouth Stag, Greenpath Stag
        Switch,
        // Crossroads_03 lever
        TollSwitch,
        // The levers in Fungus2_21
        BridgeLever
    }

    /// <summary>
    /// Module that opens any tolls that would normally be opened by the flipped lever's FSM
    /// </summary>
    public class LeverActionModule : Module
    {
        public Dictionary<string, HashSet<(string, LeverType)>> OpenedGatesByScene = new();

        public override void Initialize()
        {
            Events.OnSceneChange += OpenGatesOnSceneChange;
        }
        public override void Unload()
        {
            Events.OnSceneChange -= OpenGatesOnSceneChange;
        }

        public void OpenGate(string sceneName, string objectName, LeverType leverType)
        {
            if (!OpenedGatesByScene.TryGetValue(sceneName, out HashSet<(string, LeverType)> objectNames))
            {
                OpenedGatesByScene[sceneName] = objectNames = new();
            }
            objectNames.Add((objectName, leverType));

            if (GameManager.instance.sceneName == sceneName)
            {
                switch (leverType)
                {
                    case LeverType.PalaceOrb:
                        FSMUtility.SendEventToGameObject(GameObject.Find(objectName), "UP");
                        break;
                    case LeverType.Switch:
                        FSMUtility.SendEventToGameObject(GameObject.Find(objectName), "ACTIVATE");
                        break;
                    default:
                        FSMUtility.SendEventToGameObject(GameObject.Find(objectName), "OPEN");
                        break;
                }
            }
        }

        private void OpenGatesOnSceneChange(Scene scene)
        {
            // We have to wait a frame so animation stuff works
            GameManager.instance.StartCoroutine(OpenGatesOnSceneChangeAfterDelay(scene));
        }

        private IEnumerator OpenGatesOnSceneChangeAfterDelay(Scene scene)
        {
            yield return null;
            if (OpenedGatesByScene.TryGetValue(scene.name ?? string.Empty, out HashSet<(string, LeverType)> openGates))
            {
                foreach ((string objectName, LeverType leverType) in openGates)
                {
                    GameObject go = scene.FindGameObjectByName(objectName);
                    if (go == null) continue;

                    switch (leverType)
                    {
                        case LeverType.TollSwitch:
                            UnityEngine.Object.Destroy(go);
                            break;
                        case LeverType.PalaceOrb:
                            FSMUtility.SendEventToGameObject(go, "UP INSTANT");
                            break;
                        case LeverType.Switch:
                            FSMUtility.SendEventToGameObject(go, "ACTIVATE AGAIN");
                            FSMUtility.SendEventToGameObject(go, "ACTIVATE");
                            break;
                        default:
                            FSMUtility.SendEventToGameObject(go, "ACTIVATE");
                            break;
                    }
                }
            }
        }
    }
}
