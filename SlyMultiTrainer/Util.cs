using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace SlyMultiTrainer
{
    public static class Util
    {
        public static float DefaultAmountToIncreaseOrDecreaseTranslationForActChar = 100;
        public static float DefaultAmountToIncreaseOrDecreaseTranslationForFKXEntity = 100;
        public static int DefaultAmountToIncreaseOrDecreaseHealth = 10;
        public static string DefaultValueFloat = "0";
        public static string DefaultValueString = "None";
        public static string StringBeforeSubMapName = "   ";

        [DebuggerDisplay("{Title,nq} {Region,nq} | {BuildIdAddress,nq} == {BuildId,nq}")]
        public class Build_t
        {
            public string Title = "";
            public string Region = "";
            public string BuildId = "";
            public string BuildIdAddress = "";

            public Build_t(string title, string region, string buildId, string buildIdAddress)
            {
                Title = title;
                Region = region;
                BuildId = buildId;
                BuildIdAddress = buildIdAddress;
            }

            public override string ToString()
            {
                string tmp = $"{Title} {Region}";
                return tmp;
            }
        }

        public static List<Build_t> Builds = new()
        {
            new("Sly 1", "NTSC", "0824.2206", "276670"),
            new("Sly 1", "PAL", "1121.2105", "27FDD0"),
            new("Sly 1", "NTSC-J", "0131.1715", "27EC50"),
            new("Sly 1", "NTSC-K", "1231.1308", "27EF50"),
            //new("Sly 1", "NTSC May 19", "0519.1812", "28C190"),
            new("Sly 1", "NTSC Demo", "0408.2044", "2AA470"),
            new("Sly 1", "PAL Demo", "1206.1234", "276150"),
            // both ntsc-j demo and ntsc-k demo have 1219.2129 at 27DBD0
            // Let's use the game serial instead
            new("Sly 1", "NTSC-J Demo", "PAPX_902.31;1", "15510"),
            new("Sly 1", "NTSC-K Demo", "SCKA_900.04;1", "15510"),

            new("Sly 2", "NTSC", "0813.0032", "2C46D8"),
            new("Sly 2", "PAL (v1.00)", "0914.1846", "2CBB08"),
            new("Sly 2", "PAL (v2.01)", "1006.2123", "2CBB08"),
            new("Sly 2", "NTSC-J", "0121.1144", "2CD8E8"),
            new("Sly 2", "NTSC-K", "1221.1745", "2CCF18"),
            new("Sly 2", "NTSC E3 Demo", "0411.1757", "2A8F70"),
            // new("Sly 2", "NTSC PlayStation Magazine Demo Disc 089", "0920.1827", "2CA218"),
            new("Sly 2", "PAL September 11", "0911.1830", "2CBB08"),
            new("Sly 2", "NTSC March 17", "0317.1405", "2F91D8"),
            new("Sly 2", "NTSC July 11", "0711.1656", "2C6470"),

            new("Sly 3", "NTSC", "0828.0212", "34A2F8"),
            new("Sly 3", "PAL", "0921.1843", "34AD78"),
            new("Sly 3", "NTSC-K", "1112.1525", "34B7F8"),
            new("Sly 3", "NTSC E3 Demo", "0418.1711", "3265E8"),
            //new("Sly 3", "NTSC Jampack Volume 13 Demo", "0707.2044", "330A28"),
            new("Sly 3", "PAL Demo", "0906.1452", "3454C8"),
            new("Sly 3", "NTSC July 16", "0716.1854", "33E838"),
            new("Sly 3", "PAL August 2", "0802.0136", "3860E8"),
            new("Sly 3", "PAL September 2", "0902.1747", "395078"),
        };

        public static Build_t? GetBuild(Memory.Mem m)
        {
            Build_t? build = null;
            for (int i = 0; i < Builds.Count; i++)
            {
                if (m.ReadString(Builds[i].BuildIdAddress) == Builds[i].BuildId)
                {
                    build = Builds[i];
                    break;
                }
            }

            return build;
        }

        public static bool IsBuildCurrent(Memory.Mem m, Build_t build)
        {
            if (m.ReadString(build.BuildIdAddress) == build.BuildId)
            {
                return true;
            }

            return false;
        }

        public static GameBase_t GetGameFromBuild(Build_t? build, Memory.Mem m, Form1 form)
        {
            GameBase_t game = null;
            if (build.Title == "Sly 1")
            {
                game = new Sly1Handler(m, form, build.Region);
            }
            else if (build.Title == "Sly 2")
            {
                game = new Sly2Handler(m, form, build.Region);
            }
            else if (build.Title == "Sly 3")
            {
                game = new Sly3Handler(m, form, build.Region);
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
            public List<int> EntityPointer;

            public FKXEntry_t(string address, byte[] data)
            {
                Address = address;
                SpawnRule = BitConverter.ToInt32(data, 0);
                PoolPointer = BitConverter.ToInt32(data, 4);
                Count = BitConverter.ToInt32(data, 8);
                Count2 = BitConverter.ToInt32(data, 0xC);
                Name = Encoding.Default.GetString(data, 0x1C, 0x40).Split('\0')[0].Substring(4);

                EntityPointer = new(Count);
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
            var tmp = radians * (180f / MathF.PI);
            return tmp;
        }

        public static int GetOriginalMapId(ComboBox cmb)
        {
            // This logic was made to skip visually but internally consider some of the maps
            // For example in sly 2 july 11: splash, i_palace_heist, i_temple_hesit, p_prison_heist and p_castle_int
            int comboIdx = cmb.SelectedIndex;
            List<Map_t>? comboItems = cmb.DataSource as List<Map_t>;
            List<Map_t>? originalItems = cmb.Tag as List<Map_t>;
            int originalIdx = originalItems.IndexOf(comboItems[comboIdx]);
            return originalIdx;
        }

        public class Character_t
        {
            public string Name;
            public int Id;

            public Character_t(string name, int id)
            {
                Name = name;
                Id = id;
            }

            public override string ToString()
            {
                return Name;
            }
        }

        public class Warp_t
        {
            public string Name;
            public Vector3 Position;

            public Warp_t(string name, Vector3 position)
            {
                Name = name;
                Position = position;
            }

            public override string ToString()
            {
                return Name;
            }
        }

        [DebuggerDisplay("{Name} | IsVisible: {IsVisible}")]
        public class Map_t
        {
            public string Name;
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
                ushort value = (ushort)m.ReadShort(address);
                ReadBinds(value);
            }

            public void ReadBinds(ushort value)
            {
                value = (ushort)((value >> 8) | (value << 8));
                Select = (value & (1 << 0)) != 0;
                L3 = (value & (1 << 1)) != 0;
                R3 = (value & (1 << 2)) != 0;
                Start = (value & (1 << 3)) != 0;
                DPadUp = (value & (1 << 4)) != 0;
                DPadRight = (value & (1 << 5)) != 0;
                DPadDown = (value & (1 << 6)) != 0;
                DPadLeft = (value & (1 << 7)) != 0;
                L2 = (value & (1 << 8)) != 0;
                R2 = (value & (1 << 9)) != 0;
                L1 = (value & (1 << 10)) != 0;
                R1 = (value & (1 << 11)) != 0;
                Triangle = (value & (1 << 12)) != 0;
                Circle = (value & (1 << 13)) != 0;
                Cross = (value & (1 << 14)) != 0;
                Square = (value & (1 << 15)) != 0;
            }

            public bool IsButtonPressed(string buttonName)
            {
                var property = typeof(Controller_t).GetProperty(buttonName);
                if (property != null && property.PropertyType == typeof(bool))
                {
                    return (bool)property.GetValue(this);
                }

                return false;
            }
        }

        public static byte[] IntArrayToByteArray(int[] array)
        {
            byte[] byteArray = new byte[array.Length * sizeof(int)];
            Buffer.BlockCopy(array, 0, byteArray, 0, byteArray.Length);
            return byteArray;
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

        public static Bitmap? GetEmbeddedImage(string resourceName)
        {
            using Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                return null;
            }

            return new Bitmap(stream);
        }

        public static Icon? GetEmbeddedIcon(string resourceName)
        {
            using Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                return null;
            }

            return new Icon(stream);
        }
    }
}
