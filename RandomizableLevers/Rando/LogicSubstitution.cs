namespace RandomizableLevers.Rando
{
    /// <summary>
    /// Represents a logic substitution. Replace the old term with the infix in replacement in the existing logic for name.
    /// </summary>
    public struct LogicSubstitution
    {
        public string name;
        public string old;
        public string replacement;
    }
}
