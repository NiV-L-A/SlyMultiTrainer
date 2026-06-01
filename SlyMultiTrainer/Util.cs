using Memory;
using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace SlyMultiTrainer
{
    public static class Util
    {
        public static float AmountToIncreaseOrDecreaseTranslationForActChar = 100;
        public static float AmountToIncreaseOrDecreaseTranslationForFKXEntity = 100;
        public static int AmountToIncreaseOrDecreaseHealth = 10;
        public static string DefaultValueFloat = "0";
        public static string DefaultValueInt = "0";
        public static string DefaultValueString = "None";
        public static string SubMapNamePrefix = "   ";
        public static string CustomWarpsJsonFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SlyMultiTrainer", "SlyMultiTrainer_CustomWarps.json");
        public static System.Text.Json.JsonSerializerOptions CustomWarpsJsonOptions = new()
        {
            WriteIndented = true,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        [DebuggerDisplay("{Title,nq} {Region,nq} | {Address,nq} == {Value,nq}")]
        public class Build_t
        {
            public string Title = "";
            public string Region = "";
            public string Value = "";
            public string Address = "";

            public Build_t(string title, string region, string value, string address)
            {
                Title = title;
                Region = region;
                Value = value;
                Address = address;
            }

            public override string ToString()
            {
                return $"{Title} {Region}";
            }
        }

        public enum BUILD_NAME
        {
            NTSC,
            PAL,
            NTSCJ,
            NTSCK,
            NTSCDemo,
            PALDemo,
            NTSCJDemo,
            NTSCKDemo,
            NTSCDemoKiosk212Spring2004,
            NTSCDemoJune14,
            PALDemoNovember18,
            PALDemoPlayStationExperience,
            NTSCMay19,
            NTSCMay21,
            NTSCAugust23,
            PALNovember8,

            PALv100,
            PALv201,
            NTSCE3Demo,
            NTSCOfficialPlayStationMagazineDemoDisc089,
            PALDemoJuly27,
            NTSCDemoRatchetClankUpYourArsenal,
            PALDemoRatchetClank3,
            NTSCDemoRatchetClankUpYourArsenalAugust11,
            NTSCMarch17,
            NTSCJuly11,
            NTSCAugust9,
            PALAugust2,
            PALSeptember11,

            NTSCDemoApril18,
            NTSCDemoJuly7,
            PALDemoSeptember2,
            NTSCJuly16,
            NTSCAugust24,
            PALSeptember2,

            NTSCPS3PSN,
            PALPS3PSN,
            NTSCKPS3PSN,
            NTSCPS3,
            PALPS3,
            UKPS3,
            NTSCKPS3,
            NTSCJPS3,
        }

        public static Dictionary<BUILD_NAME, string> BuildRegions = new()
        {
            [BUILD_NAME.NTSC] = "NTSC",
            [BUILD_NAME.PAL] = "PAL",
            [BUILD_NAME.NTSCJ] = "NTSC-J",
            [BUILD_NAME.NTSCK] = "NTSC-K",
            [BUILD_NAME.NTSCDemo] = "NTSC Demo",
            [BUILD_NAME.PALDemo] = "PAL Demo",
            [BUILD_NAME.NTSCJDemo] = "NTSC-J Demo",
            [BUILD_NAME.NTSCKDemo] = "NTSC-K Demo",
            [BUILD_NAME.NTSCDemoKiosk212Spring2004] = "NTSC Demo Kiosk 2-12 Spring 2004",
            [BUILD_NAME.NTSCDemoJune14] = "NTSC Demo June 14",
            [BUILD_NAME.PALDemoNovember18] = "PAL Demo November 18",
            [BUILD_NAME.PALDemoPlayStationExperience] = "PAL Demo PlayStation Experience",
            [BUILD_NAME.NTSCMay19] = "NTSC May 19",
            [BUILD_NAME.NTSCMay21] = "NTSC May 21",
            [BUILD_NAME.NTSCAugust23] = "NTSC August 23",
            [BUILD_NAME.PALNovember8] = "PAL November 8",

            [BUILD_NAME.PALv100] = "PAL (v1.00)",
            [BUILD_NAME.PALv201] = "PAL (v2.01)",
            [BUILD_NAME.NTSCE3Demo] = "NTSC E3 Demo",
            [BUILD_NAME.NTSCOfficialPlayStationMagazineDemoDisc089] = "NTSC Official PlayStation Magazine Demo Disc 089",
            [BUILD_NAME.PALDemoJuly27] = "PAL Demo July 27",
            [BUILD_NAME.NTSCDemoRatchetClankUpYourArsenal] = "NTSC Demo (Ratchet & Clank: Up Your Arsenal)",
            [BUILD_NAME.PALDemoRatchetClank3] = "PAL Demo (Ratchet & Clank 3)",
            [BUILD_NAME.NTSCDemoRatchetClankUpYourArsenalAugust11] = "NTSC Demo (Ratchet and Clank: Up Your Arsenal August 11)",
            [BUILD_NAME.NTSCMarch17] = "NTSC March 17",
            [BUILD_NAME.NTSCJuly11] = "NTSC July 11",
            [BUILD_NAME.NTSCAugust9] = "NTSC August 9",
            [BUILD_NAME.PALAugust2] = "PAL August 2",
            [BUILD_NAME.PALSeptember11] = "PAL September 11",

            [BUILD_NAME.NTSCDemoApril18] = "NTSC Demo April 18",
            [BUILD_NAME.NTSCDemoJuly7] = "NTSC Demo July 7",
            [BUILD_NAME.PALDemoSeptember2] = "PAL Demo September 2",
            [BUILD_NAME.NTSCJuly16] = "NTSC July 16",
            [BUILD_NAME.NTSCAugust24] = "NTSC August 24",
            [BUILD_NAME.PALSeptember2] = "PAL September 2",

            [BUILD_NAME.NTSCPS3PSN] = "NTSC (PS3 PSN)",
            [BUILD_NAME.PALPS3PSN] = "PAL (PS3 PSN)",
            [BUILD_NAME.NTSCKPS3PSN] = "NTSC-K (PS3 PSN)",
            [BUILD_NAME.NTSCPS3] = "NTSC (PS3)",
            [BUILD_NAME.PALPS3] = "PAL (PS3)",
            [BUILD_NAME.UKPS3] = "UK (PS3)",
            [BUILD_NAME.NTSCJPS3] = "NTSC-J (PS3)",
            [BUILD_NAME.NTSCKPS3] = "NTSC-K (PS3)",
        };

        public static List<Build_t> Builds = new()
        {
            // Retail: final game version
            // Demo: early builds made publicly available, officially released by Sony
            // Prototypes: early builds that were not officially released by Sony

#region Sly 1 - Retail
            new("Sly 1", BuildRegions[BUILD_NAME.NTSC], "0824.2206", "276670"), // SCUS-97198 - C77AF2CA
            new("Sly 1", BuildRegions[BUILD_NAME.PAL], "1121.2105", "27FDD0"), // SCES-50917 - DA3DD765
            new("Sly 1", BuildRegions[BUILD_NAME.NTSCJ], "0131.1715", "27EC50"), // SCPS-15036 - 15C88C7B
            new("Sly 1", BuildRegions[BUILD_NAME.NTSCK], "1231.1308", "27EF50"), // SCKA-20004 - 71017DE1
#endregion

#region Sly 1 - Demo
            new("Sly 1", BuildRegions[BUILD_NAME.NTSCDemo], "0408.2044", "2AA470"), // SCUS-97210 - EF7F0CE6
            new("Sly 1", BuildRegions[BUILD_NAME.PALDemo], "1206.1234", "276150"), // SCED-51452 - F3FD8A14
            // both ntsc-j demo and ntsc-k demo have 1219.2129 at 27DBD0. Let's use the game serial instead
            new("Sly 1", BuildRegions[BUILD_NAME.NTSCJDemo], "PAPX_902.31;1", "15510"), // PAPX-90231 - 9C29F787
            new("Sly 1", BuildRegions[BUILD_NAME.NTSCKDemo], "SCKA_900.04;1", "15510"), // SCKA-90004 - 9CB33FB5

            new("Sly 1", BuildRegions[BUILD_NAME.NTSCDemoKiosk212Spring2004], "0131.1819", "274950"), // SCUS-97383 - 7656425F
            new("Sly 1", BuildRegions[BUILD_NAME.NTSCDemoJune14], "0614.2001", "2634C0"),
            // this build is in these discs:
            // Jampack Demo Disc - Winter 2002 SCUS-97235 - 7656425F
            // Official U.S. PlayStation Magazine Demo Disc 062 SCUS-97188 - 7656425F
            // Kiosk Demo Disc 2.7 SCUS-97227 - 7656425F
            // Kiosk Demo Disc 2.9 SCUS-97270 - 7656425F

            new("Sly 1", BuildRegions[BUILD_NAME.PALDemoNovember18], "1118.1459", "2760D0"),
            // this build is in these discs:
            // Official PlayStation 2 Magazine Demo 30 SCED-51457 - 90C0E5F1
            // Official PlayStation 2 Magazine Demo 30 (Germany) SCED-51483 90C0E5F1
            // Official PlayStation 2 Magazine Demo 30 (France) SCED-51489 90C0E5F1
            // Official PlayStation 2 Magazine Demo 30 (Australia) SCED-51485 90C0E5F1
            // Official PlayStation 2 Magazine Demo 30 (Spain) SCED-51552 90C0E5F1
            // Official PlayStation 2 Magazine Demo 37 SCED-51572 - 90C0E5F1
            // Official PlayStation 2 Magazine Demo 38 SCED-51573 - 90C0E5F1
            // Official PlayStation 2 Magazine Demo 83 SCED-54692 - EDCD7FA9
            // Tango - Game On Demo Disc 1 SCED-51454 - 9FA95865
            // Bonus Demo 4 SCED-51486 - B756418E

            new("Sly 1", BuildRegions[BUILD_NAME.PALDemoPlayStationExperience], "0415.1306", "2AA7F0"), // SCED-51148 - 53EBA5EB
#endregion

#region Sly 1 - Prototype
            new("Sly 1", BuildRegions[BUILD_NAME.NTSCMay19], "0519.1812", "28C190"), // SCUS-97198 - 515E82DE
            new("Sly 1", BuildRegions[BUILD_NAME.NTSCMay21], "0521.1452", "28C210"), // SCUS-97198 - E50A78F8
            new("Sly 1", BuildRegions[BUILD_NAME.NTSCAugust23], "0823.0119", "2791F0"), // SCUS-97198 - 5E78A4F2
            new("Sly 1", BuildRegions[BUILD_NAME.PALNovember8], "1108.1202", "2828D0"), // SCES-50917 - B9AB722F
#endregion

#region Sly 2 - Retail
            new("Sly 2", BuildRegions[BUILD_NAME.NTSC], "0813.0032", "2C46D8"), // SCUS-97316 - 07652DD9
            new("Sly 2", BuildRegions[BUILD_NAME.PALv100], "0914.1846", "2CBB08"), // SCES-52529 - FDA1CBF6
            new("Sly 2", BuildRegions[BUILD_NAME.PALv201], "1006.2123", "2CBB08"), // SCES-52529 - 15DD1F6F
            new("Sly 2", BuildRegions[BUILD_NAME.NTSCJ], "0121.1144", "2CD8E8"), // SCPS-15090 - 615EA2DB
            new("Sly 2", BuildRegions[BUILD_NAME.NTSCK], "1221.1745", "2CCF18"), // SCKA-20044 - 518DD841
#endregion

#region Sly 2 - Demo
            new("Sly 2", BuildRegions[BUILD_NAME.NTSCE3Demo], "0411.1757", "2A8F70"), // SCUS-97415 - 5B93397F
            new("Sly 2", BuildRegions[BUILD_NAME.NTSCOfficialPlayStationMagazineDemoDisc089], "0920.1827", "2CA218"), // SCUS-97342 - 7B564230
            new("Sly 2", BuildRegions[BUILD_NAME.PALDemoJuly27], "0727.2115", "2C8730"),
            // this build is in these discs:
            // Ratchet & Clank 3 + Sly 2 - Band of Thieves SCED-52848 - 4FB4FBA3
            // Official PlayStation 2 Magazine Demo 52 SCED-52167 - EDCD7FA9
            // Official PlayStation 2 Magazine Demo 61 SCED-53209 - 38174DD2
            // Official PlayStation 2 Magazine Demo 88 SCED-54413 - EDCD7FA9
            // Official PlayStation 2 Magazine Spécial Noël 2004 SCED-52996 - EDCD7FA9
            // Official PlayStation 2 Magazine Sonderausgabe 2004/3 SCED-52997 - EDCD7FA9
            // Magazine Ufficiale PlayStation 2 Italia 11/04 SCED-52981 - EDCD7FA9
            // SCEE Hits Demo SCED-52970 - 3CFE530D
            
            new("Sly 2", BuildRegions[BUILD_NAME.NTSCDemoRatchetClankUpYourArsenal], "0831.2225", "2CA018"), // SCUS-97353 - 45FE0CC4
            new("Sly 2", BuildRegions[BUILD_NAME.PALDemoRatchetClank3], "0831.2137", "2CA098"), // SCES-52456 - 17125698
            new("Sly 2", BuildRegions[BUILD_NAME.NTSCDemoRatchetClankUpYourArsenalAugust11], "0718.2021", "2C7680"), // SCUS-97353 - D8EB2C29
            
            // Ratchet & Clank 3 - Totsugeki! Galactic Rangers (Japan) SCPS-15084 - 64DC6000, has SLY2 folder in iso, but it's empty and pressing the combo doesn't do anything.
            // Ratchet & Clank - Gonggu Jeonsa Reloaded SCKA-20037 - 9FCC4BA4, has SLY2 folder in iso, but it's empty and pressing the combo doesn't do anything.
#endregion

#region Sly 2 - Prototype
            new("Sly 2", BuildRegions[BUILD_NAME.NTSCMarch17], "0317.1405", "2F91D8"), // SCUS-97198 - DD0B5E6C
            new("Sly 2", BuildRegions[BUILD_NAME.NTSCJuly11], "0711.1656", "2C6470"), // SCUS-97316 - A480549C
            new("Sly 2", BuildRegions[BUILD_NAME.NTSCAugust9], "0809.1956", "2CA0A8"), // SCUS-97316 - DB54798A (E732D915 with xdelta patch)
            new("Sly 2", BuildRegions[BUILD_NAME.PALAugust2], "0802.1031", "2D8208"), // SCES-52529 - 4BE9708A
            new("Sly 2", BuildRegions[BUILD_NAME.PALSeptember11], "0911.1830", "2CBB08"), // SCES-52529 - B89723F2
#endregion

#region Sly 3 - Retail
            new("Sly 3", BuildRegions[BUILD_NAME.NTSC], "0828.0212", "34A2F8"), // SCUS-97464 - 8BC95883
            new("Sly 3", BuildRegions[BUILD_NAME.PAL], "0921.1843", "34AD78"), // SCES-53409 - 8164C614
            new("Sly 3", BuildRegions[BUILD_NAME.NTSCK], "1112.1525", "34B7F8"), // SCKA-20063 - A8CC1583
#endregion

#region Sly 3 - Demo
            new("Sly 3", BuildRegions[BUILD_NAME.NTSCDemoApril18], "0418.1711", "3265E8"),
            // this build is in these discs:
            // NTSC E3 Demo SCUS-97484 - 3130A4D3
            // PlayStation Underground Demo Disc - Holiday 2005 [T-Rated] SCUS-97528 - 7B564230
            // Jampack Winter 2005 SCUS-97484 - 3130A4D3
            // Official U.S. PlayStation Magazine Demo Disc 107 SCUS-97535 - 7B564230

            new("Sly 3", BuildRegions[BUILD_NAME.NTSCDemoJuly7], "0707.2044", "330A28"),
            // this build is in these discs:
            // Sly 3 - Honor Among Thieves [Regular Demo] SCUS-97527 - 35CCFA60
            // Jampack Demo Disc Volume 13 (USA) (Mature) SCUS-97492 - 7B564230
            // Kiosk Demo Disc Q2-Q3 2006 SCUS-97557 - 7B564230
            // Kiosk Demo Disc Q3-Q4 2005 SCUS-97424 - 7B564230

            new("Sly 3", BuildRegions[BUILD_NAME.PALDemo], "0906.1452", "3454C8"), // SCED-53802 - BAE3B5E9
            
            new("Sly 3", BuildRegions[BUILD_NAME.PALDemoSeptember2], "0902.1844", "345448"),
            // this build is in these discs:
            // Official PlayStation 2 Magazine Demo 66 SCED-53169 - EDCD7FA9
            // Official PlayStation 2 Magazine Demo 67 SCED-53215 - 995D32A4
            // Official PlayStation 2 Magazine Demo 82 SCED-54642 - EDCD7FA9
            // Official PlayStation 2 Magazine Germany Special 3/2005 SCED-53938 - EDCD7FA9
            // Ufficiale PlayStation 2 Italia Kids Special 2005 Demo SCED-53798 - EDCD7FA9
            // Bonus Demo 10 SCED-53515 - DF77611A
            // Bonus Demo 10 (You) SCED-53513 - DF77611A
#endregion

#region Sly 3 - Prototype
            new("Sly 3", BuildRegions[BUILD_NAME.NTSCJuly16], "0716.1854", "33E838"), // SCUS-97464 - 0190CF8B
            new("Sly 3", BuildRegions[BUILD_NAME.NTSCAugust24], "0824.2234", "3511F8"), // SCUS-97464 - 779B6999 (31DB6999 with xdelta patch)
            new("Sly 3", BuildRegions[BUILD_NAME.PALAugust2], "0802.0136", "3860E8"), // SCES-52529 - 8C146034
            new("Sly 3", BuildRegions[BUILD_NAME.PALSeptember2], "0902.1747", "395078"), // SCES-53409 - 3670B6F9
#endregion

#region PS3 PSN
            new("Sly 1", BuildRegions[BUILD_NAME.NTSCPS3PSN], "0906.1415", "3A0928"), // NPUA80663
            new("Sly 1", BuildRegions[BUILD_NAME.PALPS3PSN], "1103.1309", "3A0948"), // NPEA00341
            new("Sly 1", BuildRegions[BUILD_NAME.NTSCKPS3PSN], "1129.1638", "3A08F8"), // NPHA80174

            new("Sly 2", BuildRegions[BUILD_NAME.NTSCPS3PSN], "0524.2241", "3FE780"), // NPUA80664
            new("Sly 2", BuildRegions[BUILD_NAME.PALPS3PSN], "0524.2241", "3FE7A0"), // NPEA00342
            new("Sly 2", BuildRegions[BUILD_NAME.NTSCKPS3PSN], "0524.2241", "3FE760"), // NPHA80175

            new("Sly 3", BuildRegions[BUILD_NAME.NTSCPS3PSN], "1222.1218", "4991B0"), // NPUA80665
            new("Sly 3", BuildRegions[BUILD_NAME.PALPS3PSN], "1222.1218", "4991C0"), // NPEA00343
            new("Sly 3", BuildRegions[BUILD_NAME.NTSCKPS3PSN], "1222.1218", "499180"), // NPHA80176
#endregion

#region PS3 physical discs
            // Sly 2 NTSC (PS3), Sly 2 PAL (PS3) and Sly 2 UK (PS3) have 0524.2241 at 38BB50
            // Sly 3 NTSC (PS3), Sly 3 PAL (PS3) and Sly 3 UK (PS3) have 1222.1218 at 426550
            // Let's use a different address instead

            // The Sly Collection (NTSC) - BCUS98246
            new("Sly 1", BuildRegions[BUILD_NAME.NTSCPS3], "1018.1857", "33E028"),
            new("Sly 2", BuildRegions[BUILD_NAME.NTSCPS3], "Sly 2", "301BCED4"),
            new("Sly 3", BuildRegions[BUILD_NAME.NTSCPS3], "Sly 3", "301BCED4"),
            
            // The Sly Trilogy (PAL) - BCES00968
            new("Sly 1", BuildRegions[BUILD_NAME.PALPS3], "1017.0103", "33E1F8"),
            new("Sly 2", BuildRegions[BUILD_NAME.PALPS3], "Sly 2", "301BD754"),
            new("Sly 3", BuildRegions[BUILD_NAME.PALPS3], "Sly 3", "301BD754"),

            // The Sly Trilogy (UK) - BCES00982
            new("Sly 1", BuildRegions[BUILD_NAME.UKPS3], "1017.2109", "33E008"),
            new("Sly 2", BuildRegions[BUILD_NAME.UKPS3], "Sly 2", "301BCDC4"),
            new("Sly 3", BuildRegions[BUILD_NAME.UKPS3], "Sly 3", "301BCDC4"),

            // Sly Cooper Collection (NTSC-J) - BCJS30061
            new("Sly 1", BuildRegions[BUILD_NAME.NTSCJPS3], "1129.2221", "33E028"),
            new("Sly 2", BuildRegions[BUILD_NAME.NTSCJPS3], "0524.2241", "38BBB0"),
            new("Sly 3", BuildRegions[BUILD_NAME.NTSCJPS3], "1222.1218", "426590"),

            // Sly Cooper Collection (NTSC-K) - BCKS10147
            new("Sly 1", BuildRegions[BUILD_NAME.NTSCKPS3], "1115.1236", "33E028"),
            new("Sly 2", BuildRegions[BUILD_NAME.NTSCKPS3], "0524.2241", "38BB60"),
            new("Sly 3", BuildRegions[BUILD_NAME.NTSCKPS3], "1222.1218", "426570"),
#endregion
        };

        public static Build_t? GetBuild(Memory.Mem m)
        {
            Build_t? build = null;
            for (int i = 0; i < Builds.Count; i++)
            {
                if (m.ReadString(Builds[i].Address) == Builds[i].Value)
                {
                    build = Builds[i];
                    break;
                }
            }

            return build;
        }

        public static bool IsBuildCurrent(Memory.Mem m, Build_t build)
        {
            if (m.ReadString(build.Address) == build.Value)
            {
                return true;
            }

            return false;
        }

        public static GameBase_t GetGameFromBuild(Form1 form, Memory.Mem m, Build_t? build)
        {
            GameBase_t game = null;
            if (build.Title == "Sly 1")
            {
                game = new Sly1Handler(form, m, build);
            }
            else if (build.Title == "Sly 2")
            {
                game = new Sly2Handler(form, m, build);
            }
            else if (build.Title == "Sly 3")
            {
                game = new Sly3Handler(form, m, build);
            }

            game.Maps.Insert(0, new("[Current map]", new() { new() }));

            // Sort and add a "None" gadget at the start of each gadget list
            foreach (var gadgetList in game.Gadgets)
            {
                gadgetList.Sort();
                gadgetList.Insert(0, new("None", -1));
            }

            return game;
        }

        [DebuggerDisplay("{Name,nq} {SpawnRule} {Count} {PoolPointer}")]
        public class FKXEntry_t
        {
            public string Address;
            public int SpawnRule;
            public int PoolPointer;
            public int Count;
            public int Count2;
            public string Name;
            public List<int> EntityAddress;

            public FKXEntry_t(string address, byte[] data)
            {
                Address = address;
                SpawnRule = EndianBitConverter.ToInt32(data, 0);
                PoolPointer = EndianBitConverter.ToInt32(data, 4);
                Count = EndianBitConverter.ToInt32(data, 8);
                Count2 = EndianBitConverter.ToInt32(data, 0xC);

                var t1 = Encoding.Default.GetString(data, 0x1C, 0x40);
                var t2 = t1.Split('\0');
                var t3 = t2[0];
                Name = t3.Substring(4);

                EntityAddress = new(Count);
            }
        }

        public static Vector3 ExtractEulerAngles(Matrix4x4 m)
        {
            float sy = MathF.Sqrt(m.M11 * m.M11 + m.M12 * m.M12);
            bool singular = sy < 1e-6;

            float x, y, z;
            if (!singular)
            {
                x = MathF.Atan2(m.M23, m.M33);
                y = MathF.Atan2(-m.M13, sy);
                z = MathF.Atan2(m.M12, m.M11);
            }
            else
            {
                x = MathF.Atan2(-m.M32, m.M22);
                y = MathF.Atan2(-m.M13, sy);
                z = 0;
            }

            return new Vector3(
                ToDegrees(x),
                ToDegrees(y),
                ToDegrees(z)
            );
        }

        public static float ToDegrees(float radians)
        {
            return radians * (180f / MathF.PI);
        }

        public static int GetOriginalMapId(ComboBox cmb)
        {
            // This logic was made to skip visually but internally consider some of the maps
            // For example in sly 2 july 11: splash, i_palace_heist, i_temple_hesit, p_prison_heist and p_castle_int
            int comboIdx = cmb.SelectedIndex;

            // Current map
            if (comboIdx == 0)
            {
                return -1;
            }

            List<Map_t>? comboItems = cmb.DataSource as List<Map_t>;
            List<Map_t>? originalItems = cmb.Tag as List<Map_t>;
            int originalIdx = originalItems.IndexOf(comboItems[comboIdx]);
            return originalIdx - 1;
        }

        public class Character_t
        {
            public string Name;
            public int Id;
            public string InternalName;
            public string NameForSavefile;

            public Character_t(string name, int id, string internalName, string nameForSavefile)
            {
                Name = name;
                Id = id;
                InternalName = internalName;
                NameForSavefile = nameForSavefile;
            }

            public Character_t(string name, int id, string internalName) : this(name, id, internalName, "")
            {

            }

            public Character_t(string name, int id) : this(name, id, "")
            {

            }

            public override string ToString()
            {
                return Name;
            }

            public override bool Equals(object? obj)
            {
                if (obj is not Character_t)
                {
                    return false;
                }

                var tmp = obj as Character_t;
                if (Id != tmp.Id)
                {
                    return false;
                }

                return true;
            }
        }

        public class Warp_t
        {
            public string Name;
            public Matrix4x4 Transformation;

            public Warp_t(string name, Matrix4x4 transformation)
            {
                Name = name;
                Transformation = transformation;
            }

            public Warp_t(string name, Vector3 position) : this(name, Matrix4x4.CreateTranslation(position))
            {

            }

            public Warp_t(string name, float x, float y, float z) : this(name, new Vector3(x, y, z))
            {

            }

            public Warp_t() : this("None", 0, 0, 0)
            {

            }

            public override string ToString()
            {
                return Name;
            }

            public override bool Equals(object? obj)
            {
                if (obj is not Warp_t)
                {
                    return false;
                }

                var tmp = obj as Warp_t;
                if (Name != tmp.Name
                 || Transformation != tmp.Transformation)
                {
                    return false;
                }

                return true;
            }
        }

        [DebuggerDisplay("{Name} | IsVisible: {IsVisible}")]
        public class Map_t
        {
            public string Name { get; set; }
            public List<Warp_t> Warps;
            /// <summary>
            /// Set to false if the map item should not appear in the combobox, but it should still be considered internally. Default is true.
            /// </summary>
            public bool IsVisible;

            public Map_t(string name, List<Warp_t> warps, bool isVisible = true)
            {
                Name = name;
                Warps = warps;
                IsVisible = isVisible;
            }

            public override string ToString()
            {
                return Name;
            }
        }

        public class Controller_t
        {
            public bool None { get; set; }
            public bool Select { get; set; }
            public bool L3 { get; set; }
            public bool R3 { get; set; }
            public bool Start { get; set; }
            public bool DPadUp { get; set; }
            public bool DPadRight { get; set; }
            public bool DPadDown { get; set; }
            public bool DPadLeft { get; set; }
            public bool L2 { get; set; }
            public bool R2 { get; set; }
            public bool L1 { get; set; }
            public bool R1 { get; set; }
            public bool Triangle { get; set; }
            public bool Circle { get; set; }
            public bool Cross { get; set; }
            public bool Square { get; set; }

            public Controller_t(Memory.Mem m, string address)
            {
                short value = m.ReadShort(address);
                ReadBinds(value);
            }

            public void ReadBinds(short value)
            {
                L2 = (value & (1 << 0)) != 0;
                R2 = (value & (1 << 1)) != 0;
                L1 = (value & (1 << 2)) != 0;
                R1 = (value & (1 << 3)) != 0;
                Triangle = (value & (1 << 4)) != 0;
                Circle = (value & (1 << 5)) != 0;
                Cross = (value & (1 << 6)) != 0;
                Square = (value & (1 << 7)) != 0;
                Select = (value & (1 << 8)) != 0;
                L3 = (value & (1 << 9)) != 0;
                R3 = (value & (1 << 10)) != 0;
                Start = (value & (1 << 11)) != 0;
                DPadUp = (value & (1 << 12)) != 0;
                DPadRight = (value & (1 << 13)) != 0;
                DPadDown = (value & (1 << 14)) != 0;
                DPadLeft = (value & (1 << 15)) != 0;
                None = (value == 0);
            }

            public bool IsButtonPressed(string buttonName)
            {
                if (buttonName == nameof(None))
                {
                    return false;
                }

                var property = typeof(Controller_t).GetProperty(buttonName);
                if (property != null && property.PropertyType == typeof(bool))
                {
                    return (bool)property.GetValue(this);
                }

                return false;
            }

            public bool IsNoButtonPressed()
            {
                return None;
            }

            public override string ToString()
            {
                var properties = GetType().GetProperties();
                List<string> pressedButtons = new();
                foreach (var prop in properties)
                {
                    if (prop.PropertyType == typeof(bool) && (bool)prop.GetValue(this)!)
                    {
                        pressedButtons.Add(prop.Name);
                    }
                }

                return string.Join(", ", pressedButtons);
            }
        }

        [DebuggerDisplay("{Id} - {Mask} - {Name}")]
        public class Gadget_t : IComparable<Gadget_t>
        {
            public string Name;
            public int Id;
            public long Mask;
            public bool IsBindable;

            public Gadget_t(string name, int id, bool isBindable = true)
            {
                Name = name;
                Id = id;
                Mask = 1L << id;
                IsBindable = isBindable;
            }

            public override string ToString()
            {
                return Name;
            }

            public int CompareTo(Gadget_t? other)
            {
                // Sort alphabetically
                if (other == null)
                {
                    return 1;
                }

                return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
            }
        }

        public static Image CreateSquare(Microsoft.Msagl.Drawing.Color color, int size = 16)
        {
            Bitmap bmp = new(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                using (Brush b = new SolidBrush(ConvertMsaglColorToSystemDrawingColor(color)))
                {
                    g.FillRectangle(b, 0, 0, size, size);
                }
            }

            return bmp;
        }

        public static Color ConvertMsaglColorToSystemDrawingColor(Microsoft.Msagl.Drawing.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static T? GetEmbeddedResource<T>(string resourceName) where T : class
        {
            using Stream? stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream($"SlyMultiTrainer.Img.{resourceName}");
            if (stream == null)
            {
                return null;
            }

            return Activator.CreateInstance(typeof(T), stream) as T;
        }

        public static Bitmap? GetEmbeddedImage(string resourceName)
        {
            return GetEmbeddedResource<Bitmap>(resourceName);
        }

        public static Icon? GetEmbeddedIcon(string resourceName)
        {
            return GetEmbeddedResource<Icon>(resourceName);
        }

        public enum GADGET_BIND
        {
            L1,
            L2,
            R2,
        }

        public class CustomWarpsFileRoot
        {
            public int Version { get; set; }
            public Dictionary<string, Dictionary<string, List<CustomWarpsFileWarpPoint>>> Data { get; set; }
        }

        public class CustomWarpsFileWarpPoint
        {
            public string Name { get; set; }
            public List<string>? IncludeBuilds { get; set; }
            public float[] Position { get; set; }
            public float[]? Rotation { get; set; }
            public bool? IsInvisible { get; set; }
        }

        public static bool GetRootFromCustomWarpsFile(out CustomWarpsFileRoot? root)
        {
            root = null;
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(CustomWarpsJsonFilePath));
                if (!File.Exists(CustomWarpsJsonFilePath))
                {
                    CustomWarpsFileRoot? newRoot = new()
                    {
                        Version = 1,
                        Data = []
                    };

                    string json = System.Text.Json.JsonSerializer.Serialize(newRoot, CustomWarpsJsonOptions);
                    File.WriteAllText(CustomWarpsJsonFilePath, json);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create the custom warps .json file at:\n\"{CustomWarpsJsonFilePath}\"\n\n{ex.Message}\n\nNo custom warps available.",
                    "Custom warps loader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            try
            {
                string json = File.ReadAllText(CustomWarpsJsonFilePath);
                root = System.Text.Json.JsonSerializer.Deserialize<CustomWarpsFileRoot>(json, CustomWarpsJsonOptions);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to parse the custom warps .json file at:\n\"{CustomWarpsJsonFilePath}\"\n\n{ex.Message}\n\nNo custom warps available.",
                    "Custom warps loader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (root == null
             || root.Data == null)
            {
                MessageBox.Show($"Failed to parse the custom warps .json file at:\n\"{CustomWarpsJsonFilePath}\"\n\nNo \"Data\" section found.\nNo custom warps available.",
                    "Custom warps loader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (root.Version != 1)
            {
                MessageBox.Show($"Unknown version \"{root.Version}\" in custom warps .json file at:\n\"{CustomWarpsJsonFilePath}\"\n\nNo custom warps available.",
                    "Custom warps loader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        public static Dictionary<string, List<Util.Warp_t>> GetCustomWarps(Util.Build_t build)
        {
            if (!Util.GetRootFromCustomWarpsFile(out CustomWarpsFileRoot? root))
            {
                return [];
            }

            Dictionary<string, List<Util.Warp_t>> customWarps = new();
            foreach (var game in root.Data)
            {
                // Only consider the warps for the detected build
                if (game.Key != build.Title
                 || game.Value == null)
                {
                    continue;
                }

                foreach (var map in game.Value)
                {
                    if (map.Value == null)
                    {
                        continue;
                    }

                    List<Util.Warp_t> mapWarps = new();
                    foreach (var warp in map.Value)
                    {
                        if (warp == null)
                        {
                            continue;
                        }

                        if (warp.Name == null)
                        {
                            var msgResult = MessageBox.Show($"A warp in \"{game.Key} -> {map.Key}\" does not have a valid \"Name\" field.\n\nThis warp will be ignored.\n\nContinue parsing the file?",
                                "Custom warps loader", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (msgResult == DialogResult.Yes)
                            {
                                continue;
                            }

                            return customWarps;
                        }

                        if (warp.IsInvisible != null
                         && warp.IsInvisible == true)
                        {
                            continue;
                        }

                        if (warp.IncludeBuilds?.Count > 0)
                        {
                            bool found = warp.IncludeBuilds.Any(b => string.Equals(b, build.Region, StringComparison.OrdinalIgnoreCase));
                            if (!found)
                            {
                                continue;
                            }
                        }

                        if (warp.Position == null
                         || warp.Position.Length != 3)
                        {
                            var msgResult = MessageBox.Show(
                                $"Warp \"{game.Key} -> {map.Key} -> {warp.Name}\" has an invalid \"Position\" field (must be an array of 3 floats).\n\nThis warp will be ignored.\n\nContinue parsing the file?",
                                "Custom warps loader", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (msgResult == DialogResult.Yes)
                            {
                                continue;
                            }

                            return customWarps;
                        }

                        Matrix4x4 trans = Matrix4x4.Identity;
                        if (warp.Rotation != null)
                        {
                            // Rotation defined
                            if (warp.Rotation.Length != 9)
                            {
                                var msgResult = MessageBox.Show($"Warp \"{game.Key} -> {map.Key} -> {warp.Name}\" has an invalid \"Rotation\" field (must be an array of 9 floats).\n\nThis warp will be ignored.\n\nContinue parsing the file?",
                                    "Custom warps loader", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (msgResult == DialogResult.Yes)
                                {
                                    continue;
                                }

                                return customWarps;
                            }

                            trans = new(
                                warp.Rotation[0], warp.Rotation[1], warp.Rotation[2], 0,
                                warp.Rotation[3], warp.Rotation[4], warp.Rotation[5], 0,
                                warp.Rotation[6], warp.Rotation[7], warp.Rotation[8], 0,
                                0, 0, 0, 1
                            );
                        }

                        trans.Translation = new Vector3(warp.Position[0], warp.Position[1], warp.Position[2]);
                        mapWarps.Add(new Util.Warp_t(warp.Name, trans));
                    }

                    if (mapWarps.Count > 0)
                    {
                        customWarps[map.Key.ToUpper()] = mapWarps;
                    }
                }
            }

            return customWarps;
        }
    }
}
