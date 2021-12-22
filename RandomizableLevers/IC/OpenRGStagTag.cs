using System.Linq;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using UnityEngine;

namespace RandomizableLevers.IC
{
    /// <summary>
    /// Tag that opens RG stag. Copied from StagLocation.
    /// </summary>
    public class OpenRGStagTag : Tag
    {
        public override void Load(object parent)
        {
            Events.AddFsmEdit(SceneNames.RestingGrounds_09, new("Stag", "Stag Control"), EditStagControl);
            Events.AddFsmEdit(SceneNames.RestingGrounds_09, new("UI List Stag", "ui_list"), EditUIList);
            Events.AddFsmEdit(SceneNames.RestingGrounds_09, new("Station Bell Lever", "Stag Bell"), EditStationBell);
        }

        public override void Unload(object parent)
        {
            Events.RemoveFsmEdit(SceneNames.RestingGrounds_09, new("Stag", "Stag Control"), EditStagControl);
            Events.RemoveFsmEdit(SceneNames.RestingGrounds_09, new("UI List Stag", "ui_list"), EditUIList);
            Events.RemoveFsmEdit(SceneNames.RestingGrounds_09, new("Station Bell Lever", "Stag Bell"), EditStationBell);
        }

        private void EditStagControl(PlayMakerFSM fsm)
        {
            FsmState hsprompt = fsm.GetState("Hidden Station?");
            FsmState openGrate = fsm.GetState("Open Grate");
            FsmState currentLocationCheck = fsm.GetState("Current Location Check");
            FsmState checkResult = fsm.GetState("Check Result");
            FsmState hudreturn = fsm.GetState("HUD Return");

            openGrate.RemoveActionsOfType<SetPlayerDataBool>();
            openGrate.RemoveActionsOfType<SetBoolValue>();

            FsmBool cancelTravel = fsm.AddFsmBool("Cancel Travel", false);

            if (!PlayerData.instance.GetBool(fsm.FsmVariables.StringVariables.First(v => v.Name == "Station Opened Bool").Value))
            {
                fsm.FsmVariables.IntVariables.First(v => v.Name == "Station Position Number").Value = 0;
                currentLocationCheck.RemoveActionsOfType<IntCompare>();

                checkResult.AddFirstAction(new Lambda(() =>
                {
                    if (cancelTravel.Value)
                    {
                        fsm.SendEvent("CANCEL");
                    }
                }));
                checkResult.AddTransition("CANCEL", "HUD Return");
            }

            fsm.GetState("HUD Return").AddFirstAction(new SetBoolValue
            {
                boolVariable = cancelTravel,
                boolValue = false
            });
        }

        private void EditUIList(PlayMakerFSM fsm)
        {
            fsm.GetState("Selection Made Cancel").AddFirstAction(new Lambda(() =>
            {
                GameObject.Find("Stag").LocateMyFSM("Stag Control").FsmVariables
                    .BoolVariables.First(v => v.Name == "Cancel Travel").Value = true;
            }));
        }

        private void EditStationBell(PlayMakerFSM fsm)
        {
            FsmState init = fsm.GetState("Init");
            init.RemoveActionsOfType<PlayerDataBoolTest>();
            init.AddTransition("FINISHED", "Opened");
        }
    }
}