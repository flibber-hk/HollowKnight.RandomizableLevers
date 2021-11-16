using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemChanger;
using ItemChanger.Items;
using ItemChanger.Locations;
using ItemChanger.UIDefs;
using RandomizableLevers.IC;
using RandomizableLevers.IC.BridgeLevers;

namespace RandomizableLevers
{
    internal static class LeverDB
    {
        private static bool ModifiedAnything = false;
        // Modify the internal dictionary - return true to reserialize. Just used for debugging atm
        internal static bool ModifyLeverDefinitions()
        {


            return ModifiedAnything;
        }

        private static void CreateItem(AbstractItem item)
        {
            Serializer.Items[item.name] = item;
            Finder.DefineCustomItem(item);
            ModifiedAnything = true;
        }
        private static void CreateLocation(AbstractLocation loc)
        {
            Serializer.Locations[loc.name] = loc;
            Finder.DefineCustomLocation(loc);
            ModifiedAnything = true;
        }
        private static void ModifyItem(AbstractItem item)
        {
            Serializer.Items[item.name] = item;
            Finder.UndefineCustomItem(item.name);
            Finder.DefineCustomItem(item);
            ModifiedAnything = true;
        }
        private static void ModifyLocation(AbstractLocation loc)
        {
            Serializer.Locations[loc.name] = loc;
            Finder.UndefineCustomLocation(loc.name);
            Finder.DefineCustomLocation(loc);
            ModifiedAnything = true;
        }
    }
}
