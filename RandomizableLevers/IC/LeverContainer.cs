using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.Components;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using ItemChanger.Util;
using UnityEngine;

namespace RandomizableLevers.IC
{
    /// <summary>
    /// Container that supports giving items immediately when a lever is hit.
    /// </summary>
    public class LeverContainer : Container
    {
        public const string Lever = "Lever";

        /* There are 60 existing levers in-game which are controlled by fsm.
         * 26 Ruins Levers (all in city, plus a few others)
         * 10 Mantis Levers (all in fungal, except for the QS mask shard lever and the city crest bridge levers)
         * 9 Mines Levers (all in Peak, including the crushers levers, plus the dirtmouth lift lever)
         * 6 Gate Switches (including Dirtmouth Stag, below Xero, outside GP stag but *not* the one in Crossroads_03)
         * 4 Palace Levers
         * 3 Palace Orb Levers
         * 1 Waterways Crank Lever (past Dung Defender; fsm is the same as the ruins lever fsm)
         * 1 Toll Switch (in Crossroads_03; has a different fsm to the others)
         *
         * The City Crest bridge levers are not controlled by FSM but rather a component; changing them isn't implemented through the container.
         */
        public override string Name => LeverContainer.Lever;
        public override GameObject GetNewContainer(AbstractPlacement placement, IEnumerable<AbstractItem> items, FlingType flingType, Cost cost = null, Transition? changeSceneTo = null)
        {
            throw new NotImplementedException();
        }

        public override void AddGiveEffectToFsm(PlayMakerFSM fsm, ContainerGiveInfo info)
        {
            // Provide our own check to see if the lever should be activated on scene entry
            FsmState init = fsm.GetState("Initiate");
            BoolTest test = init.GetFirstActionOfType<BoolTest>();
            int pos = init.Actions.IndexOf(test);
            init.Actions[pos] = new DelegateBoolTest(() => info.placement.AllObtained(), test);

            fsm.GetState("Activated")?.RemoveActionsOfType<SendEventByName>();

            // Crossroads_03 switch
            fsm.GetState("Activate")?.RemoveActionsOfType<DestroyObject>();

            fsm.GetState("Player Data?")?.RemoveActionsOfType<FsmStateAction>();
            // Gate Switch
            fsm.GetState("PD On?")?.RemoveActionsOfType<FsmStateAction>();

            // WP orb levers use the Globe state
            FsmState open = fsm.GetState("Open") ?? fsm.GetState("Globe");
            open.RemoveActionsOfType<SendEventByName>();

            // Correctly add the visit flag even though we're not using it to determine lever hit state
            open.AddFirstAction(new Lambda(() => info.placement.AddVisitFlag(VisitState.Opened)));
            
            open.AddFirstAction(new AsyncLambda(callback => ItemUtility.GiveSequentially(info.items, info.placement, new GiveInfo()
            {
                FlingType = info.flingType,
                Container = "Lever",
                // Only support corner message type because the lever doesn't take control
                MessageType = MessageType.Corner,
                Transform = fsm.transform
            }, callback)));

            // Speed things up, I guess
            FsmState hit = fsm.GetState("Hit");
            hit?.RemoveFirstActionOfType<Wait>();
        }
    }
}
