using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using ItemChanger;

namespace RandomizableLevers
{
    public static class Serializer
    {
        internal static Dictionary<string, AbstractItem> Items;
        internal static Dictionary<string, AbstractLocation> Locations;

        public static void DeserializeLevers()
        {
            JsonSerializer js = new()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
            };

            js.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            using (Stream s = ItemJson)
            using (StreamReader sr = new(s))
            using (JsonTextReader jtr = new(sr))
            {
                Items = js.Deserialize<Dictionary<string, AbstractItem>>(jtr);
            }

            using (Stream s = LocationJson)
            using (StreamReader sr = new(s))
            using (JsonTextReader jtr = new(sr))
            {
                Locations = js.Deserialize<Dictionary<string, AbstractLocation>>(jtr);
            }
        }
        private static Stream ItemJson => typeof(Serializer).Assembly.GetManifestResourceStream("RandomizableLevers.Resources.leveritems.json");
        private static Stream LocationJson => typeof(Serializer).Assembly.GetManifestResourceStream("RandomizableLevers.Resources.leverlocations.json");

        public static void SerializeLevers()
        {
            Finder.Serialize("leverlocations.json", Locations);
            Finder.Serialize("leveritems.json", Items);
        }
    }
}
