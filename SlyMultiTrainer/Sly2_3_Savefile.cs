using Memory;
using System.Diagnostics;

namespace SlyMultiTrainer
{
    public class Sly2_3_Savefile
    {
        public SAVEFILE_VERSION Version;
        public string SavefileStartAddress = "";
        public string SavefileAddressTablePointer = "";
        public string SavefileStringTablePointer = "";
        public Dictionary<string, int> SavefileAddressTable; // Ids, absolute address
        public Dictionary<short, string> SavefileStringTable; // id, string

        private Memory.Mem _m;
        private int _tableStart;
        private int _savefileStart;
        private char _pathSeparator = '/';

        public Sly2_3_Savefile(Memory.Mem m)
        {
            _m = m;
        }

        public void SetVersion(SAVEFILE_VERSION version)
        {
            Version = version;
        }

        public void Init()
        {
            if (SavefileStringTable != null && SavefileStringTable.Count != 0
            && SavefileAddressTable != null && SavefileAddressTable.Count != 0)
            {
                // Savefile already parsed
                return;
            }

            SavefileStringTable = ReadSavefileStringTable();
            List<SavefileEntry_t> entries = ParseSavefile();
            SavefileAddressTable = new();
            _savefileStart = Convert.ToInt32(SavefileStartAddress, 16);
            foreach (var entry in entries)
            {
                BuildAddressLookup(entry, new List<short>());
            }
        }

        private List<SavefileEntry_t> ParseSavefile()
        {
            List<SavefileEntry_t> entries = new();
            _tableStart = _m.ReadInt($"{SavefileAddressTablePointer}");
            if (_tableStart == 0)
            {
                return entries;
            }

            // IMPORTANT: Skip node 0, we start from +0xA
            int currentIndex = 1;
            while (currentIndex != 0)
            {
                var current = ReadNode(currentIndex);
                entries.Add(current);
                if (current.NextChildIndex != 0)
                {
                    ParseChildren(current);
                }

                currentIndex = current.NextSiblingIndex;
            }

            // For version 0, the SavefileOffset field is actually the length in bytes
            // Let's go through the entries and update the field so that it represents the offset in the savefile
            if (Version == SAVEFILE_VERSION.V0)
            {
                short offset = 0;
                foreach (var entry in entries)
                {
                    UpdateSavefileOffsetForVersion0(entry, ref offset);
                }
            }

            return entries;
        }

        private void UpdateSavefileOffsetForVersion0(SavefileEntry_t entry, ref short offset)
        {
            short length = entry.SavefileOffset;
            entry.SavefileOffset = offset;

            // We need to update offset AFTER parsing the children
            if (entry.Children.Count == 0)
            {
                offset += length;
            }

            foreach (var child in entry.Children)
            {
                UpdateSavefileOffsetForVersion0(child, ref offset);
            }
        }

        private void ParseChildren(SavefileEntry_t parent)
        {
            int currentIndex = parent.NextChildIndex;
            while (currentIndex != 0)
            {
                var current = ReadNode(currentIndex);
                parent.Children.Add(current);
                ParseChildren(current);
                currentIndex = current.NextSiblingIndex;
            }
        }

        private SavefileEntry_t ReadNode(int index)
        {
            int address = _tableStart + (index * 0xA);
            byte[] data = _m.ReadBytes(address.ToString("X"), 0xA);
            SavefileEntry_t node = new(data);
            node.Name = SavefileStringTable.GetValueOrDefault(node.Id, "");
            return node;
        }

        public int GetSavefileAddress(params short[] ids)
        {
            string key = GetIdPath(ids);
            if (SavefileAddressTable.TryGetValue(key, out int address))
            {
                return address;
            }

            return 0;
        }

        public int GetSavefileAddress(params string[] names)
        {
            short[] ids = new short[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                ids[i] = SavefileStringTable.FirstOrDefault(x => x.Value == names[i]).Key;
            }

            return GetSavefileAddress(ids);
        }

        public T ReadSavefileValue<T>(params string[] names)
        {
            int addr = GetSavefileAddress(names);
            return _m.ReadMemory<T>(addr.ToString("X"));
        }

        public void WriteSavefileValue(string dataType, string value, params string[] names)
        {
            int addr = GetSavefileAddress(names);
            _m.WriteMemory(addr.ToString("X"), dataType, value);
        }

        public List<string> DumpSavefileAddressTable(bool ordered = true)
        {
            IEnumerable<KeyValuePair<string, int>> entries = SavefileAddressTable;
            if (ordered)
            {
                entries = entries.OrderBy(x => x.Value);
            }

            List<string> list = new(entries.Count());
            foreach (var entry in entries)
            {
                string[] ids = entry.Key.Split(_pathSeparator);
                List<string> names = new(ids.Length);
                foreach (string idString in ids)
                {
                    short.TryParse(idString, out short id);
                    SavefileStringTable.TryGetValue(id, out string? name);
                    names.Add($"{name}");
                }

                string path = string.Join($" {_pathSeparator} ", names);
                list.Add($"{entry.Value:X} - {path}");
            }

            return list;
        }

        private void BuildAddressLookup(SavefileEntry_t node, List<short> currentIdPath)
        {
            currentIdPath.Add(node.Id);

            // Only add actual fields, not groups
            if (node.DataType != 0xFF)
            {
                string key = GetIdPath(currentIdPath);
                SavefileAddressTable[key] = _savefileStart + node.SavefileOffset;
            }

            foreach (var child in node.Children)
            {
                BuildAddressLookup(child, new List<short>(currentIdPath));
            }
        }

        private string GetIdPath(IEnumerable<short> ids)
        {
            return string.Join(_pathSeparator, ids);
        }

        private Dictionary<short, string> ReadSavefileStringTable()
        {
            int i = 0;
            short stringId = 0;
            Dictionary<short, string> table = new();
            while (stringId != 0x100)
            {
                stringId = (short)_m.ReadInt($"{SavefileStringTablePointer},{i * 0x10 + 8:X}");
                int stringPointer = _m.ReadInt($"{SavefileStringTablePointer},{i * 0x10 + 4:X}");
                string name = _m.ReadNullTerminatedString(stringPointer.ToString("X"));
                table.TryAdd(stringId, name);
                i++;
            }

            return table;
        }

        [DebuggerDisplay("Offset: {SavefileOffset,h} | Children: {Children.Count} | {Name,nq}")]
        public class SavefileEntry_t
        {
            public short Id;
            public byte DataType;
            public byte Unk1;
            public short SavefileOffset;
            public short NextChildIndex;
            public short NextSiblingIndex;
            public List<SavefileEntry_t> Children;
            public string? Name;

            public SavefileEntry_t(byte[] data)
            {
                Children = new();

                Id = EndianBitConverter.ToInt16(data, 0);
                DataType = data[2];
                Unk1 = data[3];
                SavefileOffset = EndianBitConverter.ToInt16(data, 4);
                NextChildIndex = EndianBitConverter.ToInt16(data, 6);
                NextSiblingIndex = EndianBitConverter.ToInt16(data, 8);
            }
        }

        public enum SAVEFILE_VERSION
        {
            V0 = 0, // Sly 2 NTSC E3 Demo, Sly 2 NTSC March 17
            V1 = 1 // All other builds
        }
    }
}
