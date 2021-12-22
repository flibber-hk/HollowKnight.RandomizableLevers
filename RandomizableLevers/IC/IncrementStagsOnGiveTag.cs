using ItemChanger;

namespace RandomizableLevers.IC
{
    /// <summary>
    /// Increment stags unlocked when given.
    /// </summary>
    public class IncrementStagsOnGiveTag : Tag
    {
        public override void Load(object parent)
        {
            AbstractItem item = (AbstractItem)parent;
            item.OnGive += OnGive;
        }

        public override void Unload(object parent)
        {
            AbstractItem item = (AbstractItem)parent;
            item.OnGive -= OnGive;
        }

        public void OnGive(ReadOnlyGiveEventArgs args)
        {
            PlayerData.instance.IncrementInt(nameof(PlayerData.stationsOpened));
        }
    }
}
