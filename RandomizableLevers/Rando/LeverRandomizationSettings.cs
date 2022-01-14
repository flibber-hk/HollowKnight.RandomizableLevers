namespace RandomizableLevers.Rando
{
    public class LeverRandomizationSettings
    {
        public bool DefineRefs;

        public bool RandomizeLevers;

        [Newtonsoft.Json.JsonIgnore]
        public bool Any => DefineRefs || RandomizeLevers;

        [MenuChanger.Attributes.MenuRange(-1, 99)]
        public int LeverGroup = -1;
    }
}
