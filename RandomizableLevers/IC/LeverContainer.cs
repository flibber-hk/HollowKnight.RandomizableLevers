using System;
using System.Collections.Generic;
using System.Linq;
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
            init.Actions[pos] = new DelegateBoolTest(() => info.placement.CheckVisitedAny(VisitState.Opened), test);

            fsm.GetState("Activated")?.RemoveActionsOfType<SendEventByName>();

            // Crossroads_03 switch
            fsm.GetState("Activate")?.RemoveActionsOfType<DestroyObject>();

            fsm.GetState("Player Data?")?.RemoveActionsOfType<FsmStateAction>();
            // Gate Switch
            fsm.GetState("PD On?")?.RemoveActionsOfType<FsmStateAction>();

            // WP orb levers use the Globe state; all others use the Open state
            FsmState open = fsm.GetState("Open") ?? fsm.GetState("Globe");
            open.RemoveActionsOfType<SendEventByName>();

            // Use the Opened visit state flag to mark that the lever has been hit
            open.AddFirstAction(new Lambda(() => info.placement.AddVisitFlag(VisitState.Opened)));
            
            // When hit, give all items immediately rather than spawning shinies.
            open.AddFirstAction(new AsyncLambda(callback => ItemUtility.GiveSequentially(info.items, info.placement, new GiveInfo()
            {
                FlingType = info.flingType,
                Container = LeverContainer.Lever,
                // Only support corner message type because the lever doesn't take control
                MessageType = MessageType.Corner,
                Transform = fsm.transform
            }, callback)));

            // Spawn shinies for respawned items in the target state of the BoolTest from Initiate
            FsmState activatedState = init.Transitions.First(x => x.FsmEvent == test.isTrue).ToFsmState;
            activatedState.AddFirstAction(new Lambda(() =>
            {
                foreach (AbstractItem item in info.items)
                {
                    if (!item.IsObtained())
                    {
                        GameObject shiny = ShinyUtility.MakeNewShiny(info.placement, item, info.flingType);
                        shiny.transform.SetPosition2D(fsm.transform.position.x, fsm.transform.position.y);
                        ShinyUtility.FlingShinyRandomly(shiny.LocateMyFSM("Shiny Control"));
                        shiny.SetActive(true);
                    }
                }
            }));

            // Speed things up, I guess
            FsmState hit = fsm.GetState("Hit");
            hit?.RemoveFirstActionOfType<Wait>();
        }
    }
}
