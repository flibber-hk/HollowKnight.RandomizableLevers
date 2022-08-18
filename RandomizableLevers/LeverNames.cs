using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RandomizableLevers
{
    [RandoConstantGenerators.GenerateJsonConsts("$[*].name", "leverlocations.json")]
    public static partial class LeverNames
    {
        public static string[] ToArray()
        {
            return typeof(LeverNames).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.IsLiteral)
            .Select(f => (string)f.GetRawConstantValue())
            .ToArray();
        }

        // Generated consts
    }
}
