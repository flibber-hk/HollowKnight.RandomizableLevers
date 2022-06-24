using System.Collections.Generic;
using System.IO;
using ItemChanger;
using Modding;
using Newtonsoft.Json;

namespace RandomizableLevers
{
    public static class LanguageData
    {
        // Implementing this as a struct purely so the Json file is nicer
        private struct LanguageEntryData
        {
            public string key;
            public string sheet;
            public string text;

            public LanguageEntryData(string key, string sheet, string value)
            {
                this.key = key;
                this.sheet = sheet;
                this.text = value;
            }
        }

        private static Dictionary<LanguageKey, string> LanguageStrings = new();
        private static List<LanguageEntryData> RawLanguageEntries = new();

        private static Stream LanguageJson => typeof(LanguageData).Assembly.GetManifestResourceStream("RandomizableLevers.Resources.languagedata.json");

        internal static void Load()
        {
            JsonSerializer js = new()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
            };

            using (Stream s = LanguageJson)
            using (StreamReader sr = new(s))
            using (JsonTextReader jtr = new(sr))
            {
                RawLanguageEntries = js.Deserialize<List<LanguageEntryData>>(jtr);
            }

            foreach (LanguageEntryData entry in RawLanguageEntries)
            {
                LanguageKey key = new(entry.sheet, entry.key);

                LanguageStrings[key] = entry.text;
            }
        }

        internal static void Hook() => ModHooks.LanguageGetHook += OverrideLanguageString;
        internal static void Unhook() => ModHooks.LanguageGetHook -= OverrideLanguageString;

        private static string OverrideLanguageString(string key, string sheetTitle, string orig)
        {
            if (key == null || sheetTitle == null) return orig;
            if (string.IsNullOrEmpty(orig) || orig.StartsWith("#!"))
            {
                LanguageKey obj = new(sheetTitle, key);
                return LanguageStrings.TryGetValue(obj, out string value) ? value : orig;
            }
            return orig; // If orig has already been overridden, then it was probably an ItemChanger language override
        }
    }
}
