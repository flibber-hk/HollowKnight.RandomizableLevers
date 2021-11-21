using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomizableLevers
{
    public static class Vanilla
    {
        // Produces a random pairing of lever items <--> lever locations, excluding the two stag levers
        public static Dictionary<string, string> GetRandomPairs(Random rng)
        {
            Dictionary<string, string> ret = null;
            while (ret is null)
            {
                ret = TryGetRandomPairs(rng);
            }
            return ret;
        }

        public static Dictionary<string, string> TryGetRandomPairs(Random rng)
        {
            List<string> items = LeverNames.ToArray().Where(x => x != LeverNames.Switch_Dirtmouth_Stag && x != LeverNames.Lever_Resting_Grounds_Stag).ToList();
            List<string> locations = LeverNames.ToArray().Where(x => x != LeverNames.Switch_Dirtmouth_Stag && x != LeverNames.Lever_Resting_Grounds_Stag).ToList();

            Dictionary<string, string> pairs = new();

            while (locations.Count > 0)
            {
                List<string> reachable = locations.Where(x => Reachable(x, new HashSet<string>(pairs.Values))).ToList();
                if (reachable.Count == 0) return null;

                int locInd = rng.Next(reachable.Count);
                int itmInd = rng.Next(items.Count);

                string loc = reachable[locInd];
                string itm = items[itmInd];

                locations.Remove(loc);
                items.RemoveAt(itmInd);

                pairs[loc] = itm;
            }

            return pairs;
        }

        private static bool Reachable(string loc, HashSet<string> items)
        {
            if (!Requirements.TryGetValue(loc, out List<List<string>> reqs)) return true;

            foreach (List<string> req in reqs)
            {
                if (!req.Any(x => items.Contains(x))) return false;
            }

            return true;
        }


        public static readonly Dictionary<string, List<List<string>>> Requirements = new()
        {
            [LeverNames.Lever_Failed_Tramway_Left] = new() { new() { LeverNames.Lever_Failed_Tramway_Left, LeverNames.Lever_Failed_Tramway_Right } },

            [LeverNames.Lever_Below_Spell_Twister] = new() { new() { LeverNames.Lever_Sanctum_Soul_Warrior, LeverNames.Lever_Sanctum_Bottom } },
            [LeverNames.Lever_Sanctum_East] = new() { new() { LeverNames.Lever_Sanctum_Soul_Warrior, LeverNames.Lever_Sanctum_Bottom } },
            [LeverNames.Lever_Sanctum_Bottom] = new() { new() { LeverNames.Lever_Sanctum_Soul_Warrior, LeverNames.Lever_Sanctum_Bottom } },
            [LeverNames.Lever_Sanctum_West_Upper] = new() { new() { LeverNames.Lever_Sanctum_Soul_Warrior, LeverNames.Lever_Sanctum_Bottom } },
            [LeverNames.Lever_Sanctum_West_Lower] = new() { new() { LeverNames.Lever_Sanctum_Soul_Warrior, LeverNames.Lever_Sanctum_Bottom } },

            [LeverNames.Lever_City_Spire_Sentry_Upper] = new() { new() { LeverNames.Lever_City_Spire_Sentry_Upper, LeverNames.Lever_City_Spire_Sentry_Lower, LeverNames.Lever_City_Bridge_Above_Fountain } },
            [LeverNames.Lever_City_Bridge_Above_Fountain] = new() { new() { LeverNames.Lever_City_Spire_Sentry_Upper, LeverNames.Lever_City_Spire_Sentry_Lower, LeverNames.Lever_City_Bridge_Above_Fountain } },

            [LeverNames.Lever_Mantis_Lords_Top_Left] = new() { new() { LeverNames.Lever_Mantis_Lords_Access } },
            [LeverNames.Lever_Mantis_Lords_Middle_Left] = new() { new() { LeverNames.Lever_Mantis_Lords_Access } },
            [LeverNames.Lever_Mantis_Lords_Bottom_Left] = new() { new() { LeverNames.Lever_Mantis_Lords_Access } },
            [LeverNames.Lever_Mantis_Lords_Middle_Right] = new() { new() { LeverNames.Lever_Mantis_Lords_Access } },
            [LeverNames.Lever_Mantis_Lords_Bottom_Right] = new() { new() { LeverNames.Lever_Mantis_Lords_Access } },
            
            [LeverNames.Lever_Palace_Final] = new() { new() { LeverNames.Lever_Palace_Entrance_Orb }, new() { LeverNames.Lever_Palace_Left_Orb }, new() { LeverNames.Lever_Palace_Right_Orb } },
            [LeverNames.Lever_Path_of_Pain] = new() { new() { LeverNames.Lever_Palace_Entrance_Orb }, new() { LeverNames.Lever_Palace_Left_Orb }, new() { LeverNames.Lever_Palace_Right_Orb } },
            
            [LeverNames.Lever_Palace_Right_Orb] = new() { new() { LeverNames.Lever_Palace_Entrance_Orb }, new() { LeverNames.Lever_Palace_Right } },
            [LeverNames.Lever_Palace_Left_Orb] = new() { new() { LeverNames.Lever_Palace_Entrance_Orb } },
            [LeverNames.Lever_Palace_Right] = new() { new() { LeverNames.Lever_Palace_Entrance_Orb } },
            [LeverNames.Lever_Palace_Atrium] = new() { new() { LeverNames.Lever_Palace_Entrance_Orb } },
        };
    }
}
