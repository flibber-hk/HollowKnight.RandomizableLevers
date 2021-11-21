using System.Linq;
using System.Reflection;

namespace RandomizableLevers
{
    public static class LeverNames
    {
        public static string[] ToArray()
        {
            return typeof(LeverNames).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.IsLiteral)
            .Select(f => (string)f.GetRawConstantValue())
            .ToArray();
        }

        // Switches
        public const string Switch_Dirtmouth_Stag = "Switch-Dirtmouth_Stag";
        public const string Switch_Outside_Ancestral_Mound = "Switch-Outside_Ancestral_Mound";
        public const string Switch_Greenpath_Stag = "Switch-Greenpath_Stag";
        public const string Switch_Lower_Resting_Grounds = "Switch-Lower_Resting_Grounds";
        public const string Switch_Petra_Arena = "Switch-Petra_Arena";
        public const string Switch_Queens_Gardens_Stag = "Switch-Queen's_Gardens_Stag";

        public const string Switch_Crossroads_East = "Switch-Crossroads_East";

        // Ruins Levers
        public const string Lever_Dung_Defender = "Lever-Dung_Defender";

        // The item for RG stag is currently a bit bugged, because it doesn't increase unlocked stags count.
        // Best to use the StagItem for now.
        public const string Lever_Resting_Grounds_Stag = "Lever-Resting_Grounds_Stag";

        public const string Lever_Waterways_Hwurmp_Arena = "Lever-Waterways_Hwurmp_Arena";
        public const string Lever_Queens_Station_Mask_Shard = "Lever-Queen's_Station_Mask_Shard";
        public const string Lever_Queens_Gardens_Ground_Block = "Lever-Queen's_Gardens_Ground_Block";
        public const string Lever_Below_Overgrown_Mound = "Lever-Below_Overgrown_Mound";
        public const string Lever_Tower_of_Love = "Lever-Tower_of_Love";
        public const string Lever_Abyss_Lighthouse = "Lever-Abyss_Lighthouse";
        public const string Lever_Failed_Tramway_Right = "Lever-Failed_Tramway_Right";
        public const string Lever_Failed_Tramway_Left = "Lever-Failed_Tramway_Left";

        public const string Lever_Below_Spell_Twister = "Lever-Below_Spell_Twister";
        public const string Lever_Sanctum_East = "Lever-Sanctum_East";
        public const string Lever_Sanctum_Soul_Warrior = "Lever-Sanctum_Soul_Warrior";
        public const string Lever_Sanctum_Bottom = "Lever-Sanctum_Bottom";
        public const string Lever_Sanctum_West_Upper = "Lever-Sanctum_West_Upper";
        public const string Lever_Sanctum_West_Lower = "Lever-Sanctum_West_Lower";

        public const string Lever_City_Fountain = "Lever-City_Fountain";
        public const string Lever_City_Spire_Sentry_Lower = "Lever-City_Spire_Sentry_Lower";
        public const string Lever_City_Spire_Sentry_Upper = "Lever-City_Spire_Sentry_Upper";
        public const string Lever_City_Bridge_Above_Fountain = "Lever-City_Bridge_Above_Fountain";
        public const string Lever_City_Storerooms = "Lever-City_Storerooms";
        public const string Lever_City_Lemm = "Lever-City_Lemm";
        public const string Lever_City_Above_Lemm_Right = "Lever-City_Above_Lemm_Right";
        public const string Lever_City_Above_Lemm_Left = "Lever-City_Above_Lemm_Left";
        public const string Lever_City_Above_Lemm_Upper = "Lever-City_Above_Lemm_Upper";
        public const string Lever_Shade_Soul_Exit = "Lever-Shade_Soul_Exit";
        public const string Lever_Emilitia = "Lever-Emilitia";

        // Mantis Levers
        public const string Lever_Mantis_Lords_Top_Left = "Lever-Mantis_Lords_Top_Left";
        public const string Lever_Mantis_Lords_Middle_Left = "Lever-Mantis_Lords_Middle_Left";
        public const string Lever_Mantis_Lords_Bottom_Left = "Lever-Mantis_Lords_Bottom_Left";
        public const string Lever_Mantis_Lords_Middle_Right = "Lever-Mantis_Lords_Middle_Right";
        public const string Lever_Mantis_Lords_Bottom_Right = "Lever-Mantis_Lords_Bottom_Right";
        public const string Lever_Mantis_Claw = "Lever-Mantis_Claw";
        public const string Lever_Mantis_Lords_Access = "Lever-Mantis_Lords_Access";
        public const string Lever_Fungal_Wastes_Thorns_Gauntlet = "Lever-Fungal_Wastes_Thorns_Gauntlet";
        public const string Lever_Fungal_Wastes_Below_Shrumal_Ogres = "Lever-Fungal_Wastes_Below_Shrumal_Ogres";
        public const string Lever_Fungal_Wastes_Bouncy_Grub = "Lever-Fungal_Wastes_Bouncy_Grub";

        // Mines Levers
        public const string Lever_Dirtmouth_Elevator = "Lever-Dirtmouth_Elevator";
        public const string Lever_Crystal_Peak_Tall_Room_Upper = "Lever-Crystal_Peak_Tall_Room_Upper";
        public const string Lever_Crystal_Peak_Tall_Room_Middle = "Lever-Crystal_Peak_Tall_Room_Middle";
        public const string Lever_Crystal_Peak_Tall_Room_Lower = "Lever-Crystal_Peak_Tall_Room_Lower";
        public const string Lever_Crystal_Peak_Spike_Grub = "Lever-Crystal_Peak_Spike_Grub";
        public const string Lever_Crystal_Peak_Below_Chest = "Lever-Crystal_Peak_Below_Chest";
        public const string Lever_Crystal_Peak_Above_Chest = "Lever-Crystal_Peak_Above_Chest";

        public const string Lever_Crystal_Peak_Crushers_Grub = "Lever-Crystal_Peak_Crushers_Grub";
        public const string Lever_Crystal_Peak_Crushers_Chest = "Lever-Crystal_Peak_Crushers_Chest";

        // Palace Levers
        public const string Lever_Palace_Atrium = "Lever-Palace_Atrium";
        public const string Lever_Palace_Right = "Lever-Palace_Right";
        public const string Lever_Palace_Final = "Lever-Palace_Final";
        public const string Lever_Path_of_Pain = "Lever-Path_of_Pain";

        public const string Lever_Palace_Entrance_Orb = "Lever-Palace_Entrance_Orb";
        public const string Lever_Palace_Left_Orb = "Lever-Palace_Left_Orb";
        public const string Lever_Palace_Right_Orb = "Lever-Palace_Right_Orb";

        // Bridge Levers
        public const string Lever_Pilgrims_Way_Left = "Lever-Pilgrim's_Way_Left";
        public const string Lever_Pilgrims_Way_Right = "Lever-Pilgrim's_Way_Right";
    }
}
