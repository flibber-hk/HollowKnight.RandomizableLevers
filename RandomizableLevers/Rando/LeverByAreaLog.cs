using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RandomizerMod.Logging;
using RandomizerMod.RC;
using RandomizerCore;
using RandomizerCore.Logic;
using ItemChanger;
using CondensedSpoilerLogger;

namespace RandomizableLevers.Rando
{
    public class LeverByAreaLog : RandoLogger
    {
        public override void Log(LogArguments args)
        {
            if (!RandoInterop.Settings.RandomizeLevers) return;

            StringBuilder sb = new();
            SpoilerReader sr = new(args);
            sr.Indent = 2;

            sb.AppendLine($"Lever spoiler for seed: {args.gs.Seed}");
            sb.AppendLine();

            sb.AppendLine("Dirtmouth:");
            sr.AddItemToStringBuilder(sb, LeverNames.Switch_Dirtmouth_Stag);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Dirtmouth_Elevator);
            sb.AppendLine();

            sb.AppendLine("Forgotten Crossroads:");
            sr.AddItemToStringBuilder(sb, LeverNames.Switch_Outside_Ancestral_Mound);
            sr.AddItemToStringBuilder(sb, LeverNames.Switch_Crossroads_East);
            sb.AppendLine();

            sb.AppendLine("Greenpath:");
            sr.AddItemToStringBuilder(sb, LeverNames.Switch_Greenpath_Stag);
            sb.AppendLine();

            sb.AppendLine("Fog Canyon:");
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Below_Overgrown_Mound);
            sb.AppendLine();

            sb.AppendLine("Queen's Gardens:");
            sr.AddItemToStringBuilder(sb, LeverNames.Switch_Petra_Arena);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Queens_Gardens_Ground_Block);
            sr.AddItemToStringBuilder(sb, LeverNames.Switch_Queens_Gardens_Stag);
            sb.AppendLine();

            sb.AppendLine("Fungal Wastes:");
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Queens_Station_Mask_Shard);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Fungal_Wastes_Below_Shrumal_Ogres);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Fungal_Wastes_Thorns_Gauntlet);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Fungal_Wastes_Bouncy_Grub);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Pilgrims_Way_Left);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Pilgrims_Way_Right);
            sb.AppendLine();

            sb.AppendLine("Mantis Village:");
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Mantis_Claw);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Mantis_Lords_Access);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Mantis_Lords_Top_Left);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Mantis_Lords_Middle_Left);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Mantis_Lords_Bottom_Left);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Mantis_Lords_Middle_Right);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Mantis_Lords_Bottom_Right);
            sb.AppendLine();

            sb.AppendLine("City of Tears:");
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_City_Above_Lemm_Left);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_City_Above_Lemm_Right);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_City_Above_Lemm_Upper);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_City_Lemm);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_City_Fountain);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Shade_Soul_Exit);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_City_Storerooms);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_City_Spire_Sentry_Lower);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_City_Spire_Sentry_Upper);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_City_Bridge_Above_Fountain);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Emilitia);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Tower_of_Love);
            sb.AppendLine();

            sb.AppendLine("Soul Sanctum:");
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Sanctum_Soul_Warrior);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Sanctum_Bottom);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Below_Spell_Twister);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Sanctum_East);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Sanctum_West_Upper);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Sanctum_West_Lower);
            sb.AppendLine();

            sb.AppendLine("Royal Waterways:");
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Dung_Defender);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Waterways_Hwurmp_Arena);
            sb.AppendLine();

            sb.AppendLine("Crystal Peak:");
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Crystal_Peak_Spike_Grub);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Crystal_Peak_Tall_Room_Upper);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Crystal_Peak_Tall_Room_Middle);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Crystal_Peak_Tall_Room_Lower);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Crystal_Peak_Above_Chest);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Crystal_Peak_Below_Chest);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Crystal_Peak_Crushers_Grub);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Crystal_Peak_Crushers_Chest);
            sb.AppendLine();

            sb.AppendLine("Resting Grounds:");
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Resting_Grounds_Stag);
            sr.AddItemToStringBuilder(sb, LeverNames.Switch_Lower_Resting_Grounds);
            sb.AppendLine();

            sb.AppendLine("Abyss:");
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Abyss_Lighthouse);
            sb.AppendLine();

            sb.AppendLine("Failed Tramway:");
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Failed_Tramway_Right);
            sr.AddItemToStringBuilder(sb, LeverNames.Lever_Failed_Tramway_Left);
            sb.AppendLine();

            if (args.gs.LongLocationSettings.WhitePalaceRando != RandomizerMod.Settings.LongLocationSettings.WPSetting.ExcludeWhitePalace)
            {
                sb.AppendLine("White Palace:");
                sr.AddItemToStringBuilder(sb, LeverNames.Lever_Palace_Entrance_Orb);
                sr.AddItemToStringBuilder(sb, LeverNames.Lever_Palace_Left_Orb);
                sr.AddItemToStringBuilder(sb, LeverNames.Lever_Palace_Right_Orb);
                sr.AddItemToStringBuilder(sb, LeverNames.Lever_Palace_Atrium);
                sr.AddItemToStringBuilder(sb, LeverNames.Lever_Palace_Right);
                sr.AddItemToStringBuilder(sb, LeverNames.Lever_Palace_Final);
                sr.AddItemToStringBuilder(sb, LeverNames.Lever_Path_of_Pain);
                sb.AppendLine();
            }

            LogManager.Write(sb.ToString(), "LeversByAreaSpoiler.txt");
        }
    }
}
