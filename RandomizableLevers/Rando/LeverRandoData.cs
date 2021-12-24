using RandomizerCore.Logic;

namespace RandomizableLevers.Rando
{
    public static class LeverRandoData
    {
        // We'll only have logic for room rando. It's a *lot* easier to write, and can be used for item and area rando too.
        // TODO - investigate item rando with vanilla transitions
        private static LogicManagerBuilder _leverLMB = new();
    }
}
