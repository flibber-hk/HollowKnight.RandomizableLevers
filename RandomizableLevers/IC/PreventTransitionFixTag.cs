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
}
