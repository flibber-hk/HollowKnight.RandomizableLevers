using System.Collections.Generic;
using ItemChanger;
using ItemChanger.Modules;

namespace RandomizableLevers.IC
{
    /// <summary>
    /// Module which replaces the transition fixes with versions which don't set the levers' states
    /// </summary>
    public class TransitionFixesProxy : Module
    {
        public HashSet<Transition> IncludedTransitionFixes = new();

        public override void Initialize()
        {
            Events.OnBeginSceneTransition += OnBeginSceneTransition;
        }

        public override void Unload()
        {
            Events.OnBeginSceneTransition -= OnBeginSceneTransition;
        }

        private void OnBeginSceneTransition(Transition t)
        {
            if (IncludedTransitionFixes.Contains(t))
            {
                switch (t.SceneName)
                {
                    case SceneNames.Crossroads_06 when t.GateName == "left1":
                    case SceneNames.Crossroads_33 when t.GateName == "right1":
                        Finder.GetItem(LeverNames.Switch_Outside_Ancestral_Mound).GiveImmediate(null);
                        break;
                    case SceneNames.Fungus3_13 when t.GateName == "left2":
                    case SceneNames.Fungus3_40 when t.GateName == "right1":
                        Finder.GetItem(LeverNames.Switch_Queens_Gardens_Stag).GiveImmediate(null);
                        break;
                    case SceneNames.RestingGrounds_02 when t.GateName == "bot1":
                    case SceneNames.RestingGrounds_06 when t.GateName == "top1":
                        Finder.GetItem(LeverNames.Switch_Lower_Resting_Grounds).GiveImmediate(null);
                        break;
                    case SceneNames.Ruins2_04 when t.GateName == "door_Ruin_House_03":
                    case SceneNames.Ruins_House_03 when t.GateName == "left1":
                        Finder.GetItem(LeverNames.Lever_Emilitia).GiveImmediate(null);
                        break;
                    case SceneNames.Waterways_07 when t.GateName == "right1":
                        Finder.GetItem(LeverNames.Lever_Dung_Defender).GiveImmediate(null);
                        break;
                    case SceneNames.Ruins1_31 when t.GateName == "left2":
                    case SceneNames.Ruins1_31b when t.GateName == "right1":
                        Finder.GetItem(LeverNames.Lever_Shade_Soul_Exit).GiveImmediate(null);
                        break;
                }
            }
        }
    }
}
