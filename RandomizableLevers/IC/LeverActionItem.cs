using System.Collections.Generic;
using System.Linq;
using ItemChanger;

namespace RandomizableLevers.IC
{
    /// <summary>
    /// Item that represents the action of hitting a lever, and activates the relevant doors if in the same scene.
    /// (All done through the module)
    /// </summary>
    public class LeverActionItem : AbstractItem
    {
        /// <summary>
        /// The PDBool set by hitting the lever, and any SD bools associated with the lever's targets
        /// </summary>
        public List<IWritableBool> SaveDataOnHit;
        /// <summary>
        /// The names of the gate objects opened by the lever (that are in the same scene as the lever)
        /// </summary>
        public List<string> targets;
        public string sceneName;
        public LeverType leverType;

        public override string GetPreferredContainer() => LeverContainer.Lever;
        public override void GiveImmediate(GiveInfo info)
        {
            foreach (IWritableBool @bool in SaveDataOnHit ?? Enumerable.Empty<IWritableBool>())
            {
                @bool.Value = true;
            }

            foreach (string gate in targets ?? Enumerable.Empty<string>())
            {
                ItemChangerMod.Modules.GetOrAdd<LeverActionModule>().OpenGate(sceneName, gate, leverType);
            }

            if (sceneName == SceneNames.Ruins1_31b)
            {
                ItemChangerMod.Modules.GetOrAdd<LeverActionModule>().OpenGate(SceneNames.Ruins1_31, "Ruins Gate", leverType);
            }
        }
    }
}
