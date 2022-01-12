namespace RandomizableLevers.Rando
{
    public class LeverRandomizationSettings
    {
        public bool RandomizeLevers = true;

        [MenuChanger.Attributes.MenuRange(-1, 99)]
        public int LeverGroup = -1;
    }
}
