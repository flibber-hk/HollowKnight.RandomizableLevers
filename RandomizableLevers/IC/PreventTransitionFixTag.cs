using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.Extensions;
using ItemChanger.Modules;

namespace RandomizableLevers.IC
{
    /// <summary>
    /// Tag that prevents a regular transition fix from occurring (so it will be handled by the Proxy)
    /// </summary>
    public class PreventTransitionFixTag : Tag
    {
        public Transition excludedTransition;

        public override void Load(object parent)
        {
            if (ItemChangerMod.Modules.Get<TransitionFixes>() is TransitionFixes tf)
            {
                tf.ExcludedTransitionFixes.Add(excludedTransition);
                ItemChangerMod.Modules.GetOrAdd<TransitionFixesProxy>().IncludedTransitionFixes.Add(excludedTransition);
            }
        }
    }

    // Tag the Ruins1_31b lever so that if it's randomized the gate in Ruins1_31 works. I'm too lazy to make a derived class
    // of ExistingContainerLocation so here's a custom tag instead. :)
    public class ShadeSoulGatePatchTag : Tag
    {
        public override void Load(object parent)
        {
            Events.AddFsmEdit(SceneNames.Ruins1_31, new("Ruins Gate", "Toll Gate"), PatchShadeSoulGate);
        }
        public override void Unload(object parent)
        {
            Events.RemoveFsmEdit(SceneNames.Ruins1_31, new("Ruins Gate", "Toll Gate"), PatchShadeSoulGate);
        }

        // Patch the shade soul gate so it's not opened by the lever's persistent bool data.
        private void PatchShadeSoulGate(PlayMakerFSM fsm)
        {
            fsm.GetState("Idle").RemoveFirstActionOfType<BoolTest>();
        }
    }
}
