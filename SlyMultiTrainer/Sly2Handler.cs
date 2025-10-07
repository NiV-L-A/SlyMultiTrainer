using System.Numerics;
using System.Text;
using static SlyMultiTrainer.Util;

namespace SlyMultiTrainer
{
    public class Sly2Handler : GameBase_t
    {
        public string ReloadAddress = "";
        public string ReloadValuesPointer = "";
        public string FKXListCount = "";
        public string ActiveCharacterPointer = "";
        public string ActiveCharacterIdAddress = "";
        public string ActiveCharacterHealthAddress = "";
        public string StringTableCountAddress = "";
        public string IsLoadingAddress = "";
        public DAG_t DAG;

        private string _offsetTransformation1 = "54";
        private string _offsetTransformation2 = "58";
        private string _offsetTransformation3 = "5C";
        private string _offsetHealthAndGadgetPower = "E00";
        private string _offsetController = "150";
        private string _offsetInfiniteDbJump = "2E8";
        private string _offsetSpeedMultiplier = "2F8"; // 2c0 + 38
        private string _offsetInvulnerable = "298";
        private string _offsetUndetectable = "11AC";

        private Memory.Mem _m;
        private Form1 _form;
        private Encoding _encoding;

        public Sly2Handler(Memory.Mem m, Form1 form, string region) : base(m, form, region)
        {
            _m = m;
            _form = form;
            DAG = new(m);
            DAG.SetVersion(DAG_VERSION.V1);
            _encoding = Encoding.UTF8;

            DAG.OffsetId = "18";
            DAG.OffsetNextNodePointer = "20";
            DAG.OffsetState = "54";
            DAG.OffsetGoalDescription = "5C";
            DAG.OffsetFocusCount1 = "64";
            DAG.OffsetFocusCount2 = "68";
            DAG.OffsetMissionName = "6C";
            DAG.OffsetMissionDescription = "70";
            DAG.OffsetClusterPointer = "7C";
            DAG.OffsetChildrenCount = "A0";
            DAG.OffsetSuckPointer = "A8";
            DAG.OffsetCheckpointEntranceValue = "B8";
            DAG.OffsetAttributes = "C8";
            DAG.OffsetAttributesForCluster = "D0";
            DAG.GetStringFromId = GetStringFromId;
            DAG.LoadMap = LoadMap;
            DAG.WriteActCharId = WriteActCharId;

            // Listing the bases and then calculating the addresses from those + offsets was tried,
            // but was not pursued because the offsets change based on the region
            if (region == "NTSC")
            {
                ReloadAddress = "3E1080";
                ReloadValuesPointer = "3E1C40";
                FKXListCount = "3E1394";
                ClockAddress = "2DDED8";
                CoinsAddress = "3D4B00";
                GadgetAddress = "3D4AF8";
                DrawDistanceAddress = "2DDF5C";
                FOVAddress = "2DDF64";
                ResetCameraAddress = "2DE240";
                MapIdAddress = "3E1110";
                GuardAIAddress = "3E1214";
                ActiveCharacterPointer = "2DE2F0";
                ActiveCharacterIdAddress = "3D4A6C";
                ActiveCharacterHealthAddress = $"{ActiveCharacterPointer},{_offsetHealthAndGadgetPower},0";
                DAG.RootNodePointer = "3E0B04";
                DAG.CurrentCheckpointNodePointer = "3E0FA4";
                DAG.TaskStringTablePointer = "3E0F88";
                DAG.ClusterIdAddress = "2DEB40";
                DAG.SavefileStartAddress = "3D4A60";
                DAG.SavefileValuesOffsetsTablePointer = "3E0EAC";
                StringTableCountAddress = "3E1AD0";
                IsLoadingAddress = "3D3980";
            }
            else if (region == "PAL (v1.00)"
                  || region == "PAL (v2.01)"
                  || region == "PAL September 11")
            {
                ReloadAddress = "3E8880";
                ReloadValuesPointer = "3E9430";
                FKXListCount = "3E8B94";
                ClockAddress = "2E52D8";
                CoinsAddress = "3DC300";
                GadgetAddress = "3DC2F8";
                DrawDistanceAddress = "2E535C";
                FOVAddress = "2E5364";
                ResetCameraAddress = "2E5640";
                MapIdAddress = "3E8910";
                GuardAIAddress = "3E8A14";
                ActiveCharacterPointer = "2E55A0";
                ActiveCharacterIdAddress = "3DC26C";
                ActiveCharacterHealthAddress = $"{ActiveCharacterPointer},{_offsetHealthAndGadgetPower},0";
                DAG.RootNodePointer = "3E8304";
                DAG.CurrentCheckpointNodePointer = "3E87A4";
                DAG.TaskStringTablePointer = "3E8788";
                DAG.ClusterIdAddress = "2E5F40";
                DAG.SavefileStartAddress = "3DC260";
                DAG.SavefileValuesOffsetsTablePointer = "3E86AC";
                StringTableCountAddress = "3E92D0";
                IsLoadingAddress = "3DB180";
            }
            else if (region == "NTSC-J")
            {
                _encoding = Encoding.Unicode;
                _offsetTransformation1 = "44";
                _offsetTransformation2 = "48";
                _offsetHealthAndGadgetPower = "DF0";
                _offsetController = "140";
                _offsetInfiniteDbJump = "2D8";
                _offsetSpeedMultiplier = "2E8";
                _offsetInvulnerable = "288";
                _offsetUndetectable = "119C";

                ReloadAddress = "3EAA80";
                ReloadValuesPointer = "3EB630";
                FKXListCount = "3EAD94";
                ClockAddress = "2E7158";
                CoinsAddress = "3DE300";
                GadgetAddress = "3DE2F8";
                DrawDistanceAddress = "2E71DC";
                FOVAddress = "2E71E4";
                ResetCameraAddress = "2E74C0";
                MapIdAddress = "3EAB10";
                GuardAIAddress = "3EAC14";
                ActiveCharacterPointer = "2E7420";
                ActiveCharacterIdAddress = "3DE26C";
                ActiveCharacterHealthAddress = $"{ActiveCharacterPointer},{_offsetHealthAndGadgetPower},0";
                DAG.RootNodePointer = "3EA304";
                DAG.CurrentCheckpointNodePointer = "3EA9A4";
                DAG.TaskStringTablePointer = "3EA988";
                DAG.ClusterIdAddress = "2E7DC0";
                DAG.SavefileStartAddress = "3DE260";
                DAG.SavefileValuesOffsetsTablePointer = "3EA8AC";
                StringTableCountAddress = "3EB4D0";
                IsLoadingAddress = "3DD180";

                DAG.OffsetState = "44";
                DAG.OffsetGoalDescription = "4C";
                DAG.OffsetFocusCount1 = "54";
                DAG.OffsetFocusCount2 = "58";
                DAG.OffsetMissionName = "5C";
                DAG.OffsetMissionDescription = "60";
                DAG.OffsetClusterPointer = "6C";
                DAG.OffsetChildrenCount = "90";
                DAG.OffsetSuckPointer = "98";
                DAG.OffsetCheckpointEntranceValue = "A8";
                DAG.OffsetAttributes = "B8";
                DAG.OffsetAttributesForCluster = "C0";
            }
            else if (region == "NTSC-K")
            {
                _encoding = Encoding.Unicode;
                _offsetTransformation1 = "44";
                _offsetTransformation2 = "48";
                _offsetHealthAndGadgetPower = "DF0";
                _offsetController = "140";
                _offsetInfiniteDbJump = "2D8";
                _offsetSpeedMultiplier = "2E8";
                _offsetInvulnerable = "288";
                _offsetUndetectable = "119C";

                ReloadAddress = "3EA100";
                ReloadValuesPointer = "3EACB0";
                FKXListCount = "3EA414";
                ClockAddress = "2E6758";
                CoinsAddress = "3DD980";
                GadgetAddress = "3DD978";
                DrawDistanceAddress = "2E67DC";
                FOVAddress = "2E67E4";
                ResetCameraAddress = "2E6AC0";
                MapIdAddress = "3EA190";
                GuardAIAddress = "3EA294";
                ActiveCharacterPointer = "2E6A20";
                ActiveCharacterIdAddress = "3DD8EC";
                ActiveCharacterHealthAddress = $"{ActiveCharacterPointer},{_offsetHealthAndGadgetPower},0";
                DAG.RootNodePointer = "3E9984";
                DAG.CurrentCheckpointNodePointer = "3EA024";
                DAG.TaskStringTablePointer = "3EA008";
                DAG.ClusterIdAddress = "2E73C0";
                DAG.SavefileStartAddress = "3DD8E0";
                DAG.SavefileValuesOffsetsTablePointer = "3E9F2C";
                StringTableCountAddress = "3EAB50";
                IsLoadingAddress = "3DC800";

                DAG.OffsetState = "44";
                DAG.OffsetGoalDescription = "4C";
                DAG.OffsetFocusCount1 = "54";
                DAG.OffsetFocusCount2 = "58";
                DAG.OffsetMissionName = "5C";
                DAG.OffsetMissionDescription = "60";
                DAG.OffsetClusterPointer = "6C";
                DAG.OffsetChildrenCount = "90";
                DAG.OffsetSuckPointer = "98";
                DAG.OffsetCheckpointEntranceValue = "A8";
                DAG.OffsetAttributes = "B8";
                DAG.OffsetAttributesForCluster = "C0";
            }
            else if (region == "NTSC E3 Demo")
            {
                DAG.SetVersion(DAG_VERSION.V0);
                _offsetHealthAndGadgetPower = "FD0";
                _offsetController = "140";
                _offsetInfiniteDbJump = "338";
                _offsetSpeedMultiplier = "344";

                ReloadAddress = "39A860";
                ReloadValuesPointer = "39CEC4";
                FKXListCount = "39AB34";
                ClockAddress = "2D1F58";
                CoinsAddress = "2D2B08";
                GadgetAddress = "2D2B00"; // confirm
                DrawDistanceAddress = "2D1FDC";
                FOVAddress = "2D1FE4";
                ResetCameraAddress = "2D2324";
                MapIdAddress = "3CA7C1"; // confirm 39A8F0
                GuardAIAddress = "";
                ActiveCharacterPointer = "2D2300";
                ActiveCharacterIdAddress = "3CA7C2";
                ActiveCharacterHealthAddress = $"{ActiveCharacterPointer},{_offsetHealthAndGadgetPower}";
                DAG.RootNodePointer = "39A3E8";
                DAG.CurrentCheckpointNodePointer = "39A854"; // confirm
                DAG.TaskStringTablePointer = "39A838";
                DAG.ClusterIdAddress = "2D2B20";
                DAG.SavefileStartAddress = "3CA7C0";
                DAG.SavefileValuesOffsetsTablePointer = "39A75C";
                StringTableCountAddress = "39CE64";
                IsLoadingAddress = "393700";

                var tmp = Maps[1];
                Maps.RemoveAt(1);
                Maps.Insert(0, tmp);
                Maps[0].IsVisible = false;
                Maps[3].IsVisible = false;
                Maps[5].IsVisible = false;
                Maps.Skip(7).ToList().ForEach(m => m.IsVisible = false);

                // nightclub door entrance
                Maps[4].Warps[0].Position = new(-4200, 5400, -200);

                // DAG.OffsetState = "54";
                // DAG.OffsetGoalDescription = "5C";
                // DAG.OffsetFocusCount1 = "64";
                // DAG.OffsetFocusCount2 = "68";
                // DAG.OffsetMissionName = "6C";
                // DAG.OffsetMissionDescription = "70";
                DAG.OffsetClusterPointer = "74";
                DAG.OffsetChildrenCount = "94";
                // DAG.OffsetSuckPointer = "A8";
                DAG.OffsetCheckpointEntranceValue = "AC";
                DAG.OffsetAttributes = "B8";
                DAG.OffsetAttributesForCluster = "BC";
            }
            else if (region == "NTSC July 11")
            {
                _offsetTransformation1 = "54";
                _offsetTransformation2 = "58";
                _offsetHealthAndGadgetPower = "1040";
                _offsetController = "160";
                _offsetInfiniteDbJump = "348";
                _offsetSpeedMultiplier = "354";
                //_offsetInvulnerable = "298"; // TO FIND
                //_offsetUndetectable = "11AC"; // TO FIND
                ReloadAddress = "3FBF60";
                ReloadValuesPointer = "3FE6B0";
                FKXListCount = "3FC244";
                ClockAddress = "2F9A28";
                CoinsAddress = "3EF6C0";
                GadgetAddress = "3EF6B8";
                DrawDistanceAddress = "2F9AAC";
                FOVAddress = "2F9AB4";
                ResetCameraAddress = "2F9D90";
                MapIdAddress = "3EF628";
                GuardAIAddress = "3FC0F0";
                ActiveCharacterPointer = "2F9CF0";
                ActiveCharacterIdAddress = "3EF62C";
                ActiveCharacterHealthAddress = $"{ActiveCharacterPointer},{_offsetHealthAndGadgetPower},0";
                DAG.RootNodePointer = "3FBAB4";
                DAG.CurrentCheckpointNodePointer = "3FBF34";
                DAG.TaskStringTablePointer = "3FBF18";
                DAG.ClusterIdAddress = "2FA688";
                DAG.SavefileStartAddress = "3EF620";
                DAG.SavefileValuesOffsetsTablePointer = "3FBE3C";
                StringTableCountAddress = "3FE560";
                IsLoadingAddress = "3EF600";

                DAG.OffsetAttributes = "C4";
                DAG.OffsetAttributesForCluster = "AC";

                // Remove ep8, vault room and dvd menu
                Maps.RemoveRange(38, 6);
                Maps.RemoveRange(16, 1);
                Maps.RemoveRange(1, 1);

                Maps.Insert(0, new("Splash", new(), false));
                Maps.Insert(12, new($"{StringBeforeSubMapName}i_palace_heist", new(), false));
                Maps.Insert(15, new($"{StringBeforeSubMapName}i_temple_hesit", new(), false));
                Maps.Insert(18, new($"{StringBeforeSubMapName}p_prison_heist", new(), false));
            }
            else if (region == "NTSC March 17")
            {
                DAG.SetVersion(DAG_VERSION.V0);
                _offsetTransformation1 = "54";
                _offsetTransformation2 = "58";
                _offsetTransformation3 = "5C";
                _offsetInfiniteDbJump = "328";
                _offsetHealthAndGadgetPower = "ED0";
                _offsetController = "140";
                _offsetSpeedMultiplier = "334";
                //_offsetInvulnerable = "298";
                //_offsetUndetectable = "11AC";
                ReloadAddress = "3EE978";
                ReloadValuesPointer = "325FF8";
                FKXListCount = "3EEBF8";
                ClockAddress = "303E18";
                CoinsAddress = "304A04";
                GadgetAddress = "";
                DrawDistanceAddress = "303E9C";
                FOVAddress = "303EA4";
                ResetCameraAddress = "3041F4";
                MapIdAddress = "41FC81";
                GuardAIAddress = "3EEB00";
                ActiveCharacterPointer = "304070";
                ActiveCharacterIdAddress = "41FC82";
                ActiveCharacterHealthAddress = $"{ActiveCharacterPointer},{_offsetHealthAndGadgetPower}";
                DAG.RootNodePointer = "3EE52C";
                DAG.CurrentCheckpointNodePointer = "3EE974";
                DAG.TaskStringTablePointer = "3EE958";
                DAG.ClusterIdAddress = "304A1C";
                DAG.SavefileStartAddress = "";
                DAG.SavefileValuesOffsetsTablePointer = "3EE87C";
                StringTableCountAddress = "3F0F14";
                IsLoadingAddress = "3E7380";

                DAG.OffsetId = "18";
                DAG.OffsetNextNodePointer = "20";
                DAG.OffsetState = "54";
                DAG.OffsetGoalDescription = "5C";
                DAG.OffsetMissionName = "BC";
                DAG.OffsetMissionDescription = "70";
                DAG.OffsetClusterPointer = "74";
                DAG.OffsetChildrenCount = "94";
                DAG.OffsetCheckpointEntranceValue = "AC";
                DAG.OffsetAttributes = "B8";
                DAG.OffsetAttributesForCluster = "B8";

                // Remove ep8, vault room and dvd menu
                Maps.RemoveRange(14, 30);
                Maps.RemoveRange(0, 2);

                Maps.Insert(0, new("Splash", new()));

                Map_t temp = Maps[4]; // get print room
                Maps[4] = Maps[5];
                Maps[5] = Maps[6];
                Maps[6] = temp;

                var ep2 = Maps.Take(new Range(7, 11)).ToList();
                Maps.RemoveRange(7, 4);
                Maps.AddRange(ep2);

                // nightclub door entrance
                Maps[3].Warps[0].Position = new(-4200, 5400, -200);
                //Maps.Insert(12, new($"{StringBeforeSubMapName}i_palace_heist", new(), false));
                //Maps.Insert(15, new($"{StringBeforeSubMapName}i_temple_hesit", new(), false));
                //Maps.Insert(18, new($"{StringBeforeSubMapName}p_prison_heist", new(), false));
            }

            _form.UpdateUI(_form.cmbMaps, Maps.Where(x => x.IsVisible).ToList());
            _form.UpdateUI(_form.cmbMaps, Maps, "Tag");
            _form.UpdateUI(_form.cmbActChar, Characters);
        }

        public override void CustomTick()
        {

        }

        public override bool IsLoading()
        {
            if (_m.ReadInt(IsLoadingAddress) != 3)
            {
                return true;
            }
            return false;
        }

        public override bool IsActCharAvailable()
        {
            return _m.ReadInt(ActiveCharacterPointer) != 0;
        }

        public override void OnMapChange(int mapId)
        {
            _form.UpdateUI(() =>
            {
                _form.trvFKXList.Nodes.Clear();
                if (DAG != null)
                {
                    DAG.Viewer.Enabled = false;
                }
            });

            if (DAG != null)
            {
                DAG.Graph = null;
            }
        }

        public override Vector3 ReadActCharPosition()
        {
            return ReadPositionFromPointerToEntity(ActiveCharacterPointer);
        }

        public override void WriteActCharPosition(Vector3 value)
        {
            WritePositionFromPointerToEntity(ActiveCharacterPointer, value);
        }

        public override void FreezeActCharPositionX(string value = "")
        {
            FreezePositionXFromPointerToEntity(ActiveCharacterPointer, value);
        }

        public override void FreezeActCharPositionY(string value = "")
        {
            FreezePositionYFromPointerToEntity(ActiveCharacterPointer, value);
        }

        public override void FreezeActCharPositionZ(string value = "")
        {
            FreezePositionZFromPointerToEntity(ActiveCharacterPointer, value);
        }

        public override void UnfreezeActCharPositionX()
        {
            UnfreezePositionXFromPointerToEntity(ActiveCharacterPointer);
        }

        public override void UnfreezeActCharPositionY()
        {
            UnfreezePositionYFromPointerToEntity(ActiveCharacterPointer);
        }

        public override void UnfreezeActCharPositionZ()
        {
            UnfreezePositionZFromPointerToEntity(ActiveCharacterPointer);
        }

        public override void FreezeActCharVelocityZ(string value = "")
        {
            if (value == "")
            {
                Vector3 trans = ReadActCharVelocity();
                value = trans.Z.ToString();
            }

            _m.FreezeValue($"{ActiveCharacterPointer},{_offsetTransformation2},B8", "float", value);
        }

        public override void UnfreezeActCharVelocityZ()
        {
            _m.UnfreezeValue($"{ActiveCharacterPointer},{_offsetTransformation2},B8");
        }

        public override float ReadSpeedMultiplier()
        {
            var value = _m.ReadFloat($"{ActiveCharacterPointer},{_offsetSpeedMultiplier}");
            return value;
        }

        public override void WriteSpeedMultiplier(float value)
        {
            _m.WriteMemory($"{ActiveCharacterPointer},{_offsetSpeedMultiplier}", "float", value.ToString());
        }

        public override void FreezeSpeedMultiplier(float value)
        {
            if (value == 0)
            {
                value = ReadSpeedMultiplier();
            }

            _m.FreezeValue($"{ActiveCharacterPointer},{_offsetSpeedMultiplier}", "float", value.ToString());
        }

        public override void UnfreezeSpeedMultiplier()
        {
            _m.UnfreezeValue($"{ActiveCharacterPointer},{_offsetSpeedMultiplier}");
        }

        private string GetActCharHealthAddress()
        {
            string address = ActiveCharacterHealthAddress;
            if (_m.ReadInt($"{ActiveCharacterPointer},{_offsetHealthAndGadgetPower}") == 0)
            {
                address = $"{ActiveCharacterPointer},184";
            }

            return address;
        }

        public override int ReadActCharHealth()
        {
            int health = _m.ReadInt(GetActCharHealthAddress());
            return health;
        }

        public override void WriteActCharHealth(int value)
        {
            _m.WriteMemory(GetActCharHealthAddress(), "int", value.ToString());
        }

        public override void FreezeActCharHealth(int value = 0)
        {
            if (value == 0)
            {
                value = ReadActCharHealth();
            }

            _m.FreezeValue(GetActCharHealthAddress(), "int", value.ToString());
        }

        public override void UnfreezeActCharHealth()
        {
            _m.UnfreezeValue(GetActCharHealthAddress());
        }

        public override Controller_t GetController()
        {
            return new(_m, $"{ActiveCharacterPointer},{_offsetController},30");
        }

        public override void ToggleInvulnerable(bool enableInvulnerable)
        {
            string address = $"{ActiveCharacterPointer},{_offsetInvulnerable}";
            if (enableInvulnerable)
            {
                _m.FreezeValue(address, "int", "1");
            }
            else
            {
                _m.UnfreezeValue(address);
                _m.WriteMemory(address, "int", "0");
            }
        }

        public override void ToggleUndetectable(bool enableUndetectable)
        {
            string address = $"{ActiveCharacterPointer},{_offsetUndetectable}";
            if (enableUndetectable)
            {
                _m.FreezeValue(address, "int", "1");
            }
            else
            {
                _m.UnfreezeValue(address);
                _m.WriteMemory(address, "int", "0");
            }
        }

        public override void ToggleInfiniteDbJump(bool enableInfDbJump)
        {
            if (enableInfDbJump)
            {
                _m.FreezeValue($"{ActiveCharacterPointer},{_offsetInfiniteDbJump}", "int", "1");
            }
            else
            {
                _m.UnfreezeValue($"{ActiveCharacterPointer},{_offsetInfiniteDbJump}");
            }
        }

        public override int ReadActCharId()
        {
            int Id = _m.ReadInt(ActiveCharacterIdAddress);
            return Id;
        }

        public override void WriteActCharId(int id)
        {
            _m.WriteMemory($"{ActiveCharacterIdAddress}", "int", id.ToString());
        }

        public void WriteActCharId(int id, int id2 = -1)
        {
            _m.WriteMemory($"{ActiveCharacterIdAddress}", "int", id.ToString());
        }

        public override void FreezeActCharId(string value = "")
        {
            if (value == "")
            {
                value = ReadActCharId().ToString();
            }

            _m.FreezeValue($"{ActiveCharacterIdAddress}", "int", value.ToString());
        }

        public override void UnfreezeActCharId()
        {
            _m.UnfreezeValue($"{ActiveCharacterIdAddress}");
        }

        public override void UnfreezeActCharGadgetPower()
        {
            _m.UnfreezeValue($"{ActiveCharacterPointer},{_offsetHealthAndGadgetPower},4");
        }

        public override void FreezeActCharGadgetPower(int value = 0)
        {
            if (value == 0)
            {
                value = ReadActCharGadgetPower();
            }
            _m.FreezeValue($"{ActiveCharacterPointer},{_offsetHealthAndGadgetPower},4", "int", value.ToString());
        }

        public int ReadActCharGadgetPower()
        {
            int GadgetPower = _m.ReadInt($"{ActiveCharacterPointer},{_offsetHealthAndGadgetPower},4");
            return GadgetPower;
        }

        public void WriteActCharGadgetPower(int value)
        {
            _m.WriteMemory($"{ActiveCharacterPointer},{_offsetHealthAndGadgetPower},4", "int", value.ToString());
        }

        public override Matrix4x4 ReadWorldRotationFromPointerToEntity(string pointerToEntity)
        {
            if (_m.ReadInt($"{pointerToEntity},{_offsetTransformation1}") == -1)
            {
                return Matrix4x4.Identity;
            }

            Matrix4x4 trans = _m.ReadMatrix4($"{pointerToEntity},{_offsetTransformation3},0");
            return trans;
        }

        public override void WriteWorldRotationFromPointerToEntity(string pointerToEntity, Matrix4x4 rotationMatrix)
        {
            if (_m.ReadInt($"{pointerToEntity},{_offsetTransformation1}") == -1)
            {
                return;
            }

            _m.WriteMemory($"{pointerToEntity},{_offsetTransformation3},0", "mat4", rotationMatrix.ToString());
        }

        public override void WriteScaleFromPointerToEntity(string pointerToEntity, float scale)
        {
            if (_m.ReadInt($"{pointerToEntity},{_offsetTransformation1}") == -1)
            {
                return;
            }

            _m.WriteMemory($"{pointerToEntity},{_offsetTransformation2},0", "float", scale.ToString());
            _m.WriteMemory($"{pointerToEntity},{_offsetTransformation2},14", "float", scale.ToString());
            _m.WriteMemory($"{pointerToEntity},{_offsetTransformation2},28", "float", scale.ToString());
        }

        public override float ReadScaleFromPointerToEntity(string pointerToEntity)
        {
            if (_m.ReadInt($"{pointerToEntity},{_offsetTransformation1}") == -1
                || _m.ReadInt($"{pointerToEntity},{_offsetTransformation1}") == 0)
            {
                return 1f;
            }

            float trans = _m.ReadFloat($"{pointerToEntity},{_offsetTransformation2},0");
            return trans;
        }

        public override Vector3 ReadPositionFromPointerToEntity(string pointerToEntity)
        {
            if (_m.ReadInt($"{pointerToEntity},{_offsetTransformation1}") == -1)
            {
                return Vector3.Zero;
            }

            Vector3 trans = _m.ReadVector3($"{pointerToEntity},{_offsetTransformation2},30");
            return trans;
        }

        public override Vector3 ReadWorldPositionFromPointerToEntity(string pointerToEntity)
        {
            if (_m.ReadInt($"{pointerToEntity},{_offsetTransformation1}") == -1)
            {
                return Vector3.Zero;
            }

            Vector3 trans = _m.ReadVector3($"{pointerToEntity},{_offsetTransformation2},70");
            return trans;
        }

        public override void WritePositionFromPointerToEntity(string pointerToEntity, Vector3 value)
        {
            if (_m.ReadInt($"{pointerToEntity},{_offsetTransformation1}") == -1)
            {
                return;
            }

            _m.WriteMemory($"{pointerToEntity},{_offsetTransformation2},30", "vec3", value.ToString());
        }

        public override void FreezePositionXFromPointerToEntity(string pointerToEntity, string value = "")
        {
            if (value == "")
            {
                Vector3 trans = ReadPositionFromPointerToEntity(pointerToEntity);
                value = trans.X.ToString();
            }
            _m.FreezeValue($"{pointerToEntity},{_offsetTransformation2},30", "float", value);
        }

        public override void FreezePositionYFromPointerToEntity(string pointerToEntity, string value = "")
        {
            if (value == "")
            {
                Vector3 trans = ReadPositionFromPointerToEntity(pointerToEntity);
                value = trans.Y.ToString();
            }
            _m.FreezeValue($"{pointerToEntity},{_offsetTransformation2},34", "float", value);
        }

        public override void FreezePositionZFromPointerToEntity(string pointerToEntity, string value = "")
        {
            if (value == "")
            {
                Vector3 trans = ReadPositionFromPointerToEntity(pointerToEntity);
                value = trans.Z.ToString();
            }
            _m.FreezeValue($"{pointerToEntity},{_offsetTransformation2},38", "float", value);
        }

        public override void UnfreezePositionXFromPointerToEntity(string pointerToEntity)
        {
            _m.UnfreezeValue($"{pointerToEntity},{_offsetTransformation2},30");
        }

        public override void UnfreezePositionYFromPointerToEntity(string pointerToEntity)
        {
            _m.UnfreezeValue($"{pointerToEntity},{_offsetTransformation2},34");
        }

        public override void UnfreezePositionZFromPointerToEntity(string pointerToEntity)
        {
            _m.UnfreezeValue($"{pointerToEntity},{_offsetTransformation2},38");
        }

        public Vector3 ReadActCharVelocity()
        {
            Vector3 trans = _m.ReadVector3($"{ActiveCharacterPointer},{_offsetTransformation2},B0");
            return trans;
        }

        public void WriteActCharVelocity(Vector3 value)
        {
            _m.WriteMemory($"{ActiveCharacterPointer},{_offsetTransformation2},B0", "vec3", value.ToString());
        }

        public override void LoadMap(int mapId)
        {
            // current map
            if (mapId == -1)
            {
                mapId = GetMapId();
            }

            //int extra = 0;
            //if (Region == "NTSC E3 Demo")
            //{
            //    extra = 0x40;
            //}

            byte[] data = _m.ReadBytes($"{ReloadValuesPointer},{mapId * 0x40:X8}", 0x40);
            _m.WriteBytes($"{ReloadAddress}+8", data);
            ReloadMap();
        }

        public override void LoadMap(int mapId, int entranceValue)
        {
            _m.WriteMemory($"{ReloadAddress}+48", "int", $"{entranceValue}");
            LoadMap(mapId);
        }

        public override void LoadMap(int mapId, int entranceValue, int mode)
        {
            _m.WriteMemory($"{ReloadAddress}+4", "int", $"{mode}");
            LoadMap(mapId, entranceValue);
        }

        public void ReloadMap()
        {
            _m.WriteMemory($"{ReloadAddress}+4", "int", "0"); // mode
            _m.WriteMemory(ReloadAddress, "int", "1");
        }

        public List<FKXEntry_t> GetFKXList()
        {
            int fkxCount = _m.ReadInt(FKXListCount);
            string fkxPointer = _m.ReadInt($"{FKXListCount}+4").ToString("X");
            List<FKXEntry_t> fkxList = new(fkxCount);
            for (int i = 0; i < fkxCount; i++)
            {
                string address = (Convert.ToInt32(fkxPointer, 16) + i * 0x6C).ToString("X");
                var data = _m.ReadBytes(address, 0x5C);
                FKXEntry_t fkx = new(address, data);
                for (int j = 0; j < fkx.Count; j++)
                {
                    fkx.EntityPointer.Add(_m.ReadInt($"{fkx.PoolPointer:X}+{j * 4:X}"));
                }

                fkxList.Add(fkx);
            }

            fkxList = fkxList.OrderBy(x => x.Name).ToList();
            return fkxList;
        }

        public string GetStringFromId(int id)
        {
            if (id == -1)
            {
                return "";
            }

            int count = _m.ReadInt($"{StringTableCountAddress}");
            string address = _m.ReadInt($"{StringTableCountAddress}+4").ToString("X");
            for (int i = 0; i < count; i++)
            {
                int stringId = _m.ReadInt($"{address}+{i * 8:X}");
                if (id == stringId)
                {
                    int stringPointer = _m.ReadInt($"{address}+{i * 8 + 4:X}");
                    string str = _m.ReadNullTerminatedString(stringPointer.ToString("X"), _encoding);
                    // "Carmelita's Gunner"
                    str = str.Replace('\ufffd', '\'');
                    return str;
                }
            }

            return "";
        }

        protected override List<Character_t> GetCharacters()
        {
            return new List<Character_t>
            {
                new("Sly", 7),
                new("Bentley", 8),
                new("Murray", 9),
            };
        }

        protected override List<Map_t> GetMaps()
        {
            return new List<Map_t>
            {
                new("Cairo",
                    new()
                    {
                        new("Museum", new(4910, -5210, 580)),
                        new("Computer", new(5400, -700, 1100)),
                        new("Balcony 1", new(11560, -1050, 1110)),
                        new("Balcony 2", new(-12870, 15500, 1370)),
                        new("Murray rendezvous", new(18790, 80, 1500)),
                        new("Warehouse", new(12500, 5790, 1600)),
                        new("Chase start", new(10000, 8150, 1860)),
                        new("Pickup point", new(-26400, 4350, 80)),
                    }
                ),
                new("DVD menu",
                    new()
                    {
                        new("None", new(0, 0, 0)),
                    }
                ),
                new("Paris hub",
                    new()
                    {
                        new("Safehouse", new(-1800, -4100, 535)),
                        new("Safehouse (top)", new(-2665, -3640, 1256)),
                        new("Dimitri's boat", new(-7090, -6320, -30)),
                        new("Satellite dish 1", new(-6000, 4700, 1100)),
                        new("Satellite dish 2", new(-5000, -4800, 1300)),
                        new("Satellite dish 3", new(6100, -7500, 1300)),
                        new("Nightclub (door)", new(-1645, 5655, 60)),
                        new("Nightclub (window)", new(-3455, 5510, 1100)),
                        new("Nightclub (top)", new(0, 0, 5000)),
                        new("Courtyard", new(1860, 5630, -50)),
                        new("Clock tower", new(8600, 2080, 2070)),
                        new("Hotel", new(4430, -8000, 2420)),
                    }
                ),
                new($"{StringBeforeSubMapName}Wine cellar",
                    new()
                    {
                        new("Entrance", new(14500, -6700, 470)),
                        new("Lasers", new(10340, -3900, 60)),
                        new("Office", new(5800, -2900, -150)),
                        new("Music room", new(95, 2850, 125)),
                    }
                ),
                new($"{StringBeforeSubMapName}Nightclub",
                    new()
                    {
                        new("Entrance (door)", new(-2670, 5880, -540)),
                        new("Entrance (window)", new(-7780, 1440, 810)),
                        new("Dancefloor", new(-7030, 8845, -1000)),
                        new("Dimitri's office", new(-3800, 6460, 200)),

                    }
                ),
                new($"{StringBeforeSubMapName}Print room",
                    new()
                    {
                        new("Entrance (recon)", new(-1100, 4180, 1470)),
                        new("Bottom floor", new(0, 0, -50)),
                        new("Money printer", new(0, 900, 740)),
                        new("Top floor", new(0, 1800, 1570)),
                    }
                ),
                new($"{StringBeforeSubMapName}Theater",
                    new()
                    {
                        new("Entrance", new(-40, 4220, 910)),
                        new("Fan control", new(7000, 5110, 730)),
                        new("TV", new(3800, 8490, 895)),
                        new("Spotlight control", new(2560, 5820, 1560)),
                    }
                ),
                new($"{StringBeforeSubMapName}Water pump room",
                    new()
                    {
                        new("Entrance", new(-13060, 6580, -170)),
                        new("Fireplace", new(-9670, 2470, -540)),
                        new("Water pump", new(-5560, 3850, 130)),
                    }
                ),
                new("India 1 hub",
                    new()
                    {
                        new("Safehouse", new(-4600, 2180, 460)),
                        new("Safehouse (inside)", new(-6670, -1470, 2270)),
                        new("Palace (door)", new(10000, 2100, 1690)),
                        new("Guesthouse (top)", new(3160, -10420, 1770)),
                        new("Cobra statue", new(6950, 8680, 1770)),
                        new("Drain pipe (basement entrance)", new(14150, 2160, 780)),
                        new("Drawbridge control", new(200, 1980, 960)),
                    }
                ),
                new($"{StringBeforeSubMapName}Hotel",
                    new()
                    {
                        new("Entrance", new(-80, 1800, -780)),
                        new("Room 101", new(-6400, -1700, 100)),
                        new("Room 102", new(-3300, 120, -170)),
                        new("Room 103", new(-110, -1540, -480)),
                        new("Room 104", new(3320, 50, -170)),
                        new("Room 105", new(6540, -1880, 110)),
                    }
                ),
                new($"{StringBeforeSubMapName}Basement",
                    new()
                    {
                        new("Entrance", new(2470, -1850, 40)),
                        new("Vault", new(2640, 0, 390)),
                        new("Boardroom", new(1360, 0, 1170)),
                    }
                ),
                new($"{StringBeforeSubMapName}Ballroom",
                    new()
                    {
                        new("Entrance", new(1160, 5380, 910)),
                        new("Dance floor", new(1460, 2600, 70)),
                        new("Guests (left)", new(3200, -1000, 1400)),
                        new("Guests (right)", new(-500, -800, 1400)),
                    }
                ),
                new("India 2 hub",
                    new()
                    {
                        new("Safehouse", new(-2930, -5540, 1980)),
                        new("Safehouse (top)", new(-2700, -5600, 3070)),
                        new("Watermill ", new(2500, -700, 1300)),
                        new("Tilting temple", new(-9100, -1600, 2500)),
                        new("Temple entrance", new(0, 3400, 2040)),
                        new("Waterfall", new(-9040, 7430, 0)),
                        new("Dam", new(1390, 8200, 8010)),
                    }
                ),
                new($"{StringBeforeSubMapName}Spice factory",
                    new()
                    {
                        new("Factory entrance", new(-10200, 9100, -30)),
                        new("Factory entrance (top)", new(-10000, 9880, 1600)),
                        new("Factory recon area", new(-14160, 12000, 2200)),
                        new("Spice grinder entrance", new(-5700, 1760, -1290)),
                        new("Spice grinder", new(-8200, -2560, -850)),
                        new("Rajan's office", new(5100, 11260, 2200)),
                    }
                ),
                new("Prague 1 hub",
                    new()
                    {
                        new("Safehouse", new(-7100, -10500, 235)),
                        new("Safehouse (top)", new(-7490, -11010, 1440)),
                        new("Bridge", new(6120, -5800, 730)),
                        new("Prison (center)", new(-1430, 1120, 3690)),
                        new("Prison (sly)", new(1930, -4480, 1960)),
                        new("Rooftop 1", new(-9060, 310, 1295)),
                        new("Rooftop 2", new(2370, -12450, 850)),
                        new("The Contessa's house", new(6840, -590, 1850)),
                    }
                ),
                new($"{StringBeforeSubMapName}Prison",
                    new()
                    {
                        new("Entrance", new(540, -4040, 500)),
                        new("Murray's cell", new(7700, -2100, -1130)),
                        new("Hypno arena", new(-3660, 1950, -320)),
                        new("Control room", new(700, -1300, 250)),
                    }
                ),
                new($"{StringBeforeSubMapName}Vault room",
                    new()
                    {
                        new("Entrance", new(-280, 390, 20)),
                        new("Behind the wall", new(1330, -1000, 490)),
                    }
                ),
                new("Prague 2 hub",
                    new()
                    {
                        new("Safehouse", new(11560, 2350, 800)),
                        new("Sewer", new(7980, 2270, -610)),
                        new("Graveyard", new(-2150, -7300, 420)),
                        new("Castle main entrance", new(0, 0, 150)),
                        new("Castle back entrance", new(-4700, -400, -300)),
                        new("Castle top 1", new(-6690, -2720, 1300)),
                        new("Castle top 2", new(-800, 4680, 2150)),
                        new("Castle top 3", new(-3980, 1920, 4530)),
                        new("Guillotine", new(-6445, 4333, 180)),
                        new("Re-education tower (entrance)", new(-1025, -4400, 3330)),
                        new("Re-education tower (balcony)", new(-175, -4485, 4900)),
                    }
                ),
                new("p_castle_int", // used to skip map id 18
                    new(),
                    false
                ),
                new($"{StringBeforeSubMapName}Crypt 3 (Stealing Voices)",
                    new()
                    {
                        new("Entrance", new(-25200, -180, 75)),
                        new("End", new(-25220, -9320, 190)),
                    }
                ),
                new($"{StringBeforeSubMapName}Crypts 1 & 2 (Stealing Voices)",
                    new()
                    {
                        new("Entrance (crypt 1)", new(-16570, -11490, -380)),
                        new("Vault (crypt 1)", new(-12270, -11550, -210)),
                        new("Entrance (crypt 2)", new(-14750, 560, 540)),
                        new("End (crypt 2)", new(-19480, 5450, 1180)),
                    }
                ),
                new($"{StringBeforeSubMapName}Crypt 4 (Ghost Capture)",
                    new()
                    {
                        new("Entrance", new(3530, -11640, -380)),
                        new("Tomb", new(3040, -6300, -1180)),
                    }
                ),
                new($"{StringBeforeSubMapName}Re-education tower & Hacking Crypt",
                    new()
                    {
                        new("Entrance (Re-education Tower)", new(-16020, -11490, -40)),
                        new("Re-education cell", new(-14300, -11720, -340)),
                        new("Entrance (hack)", new(-6690, -4700, 470)),
                        new("End (hack)", new(-1000, 8300, -380)),
                        new("Unused area 1", new(9050, -2630, 100)),
                        new("Unused area 2", new(10200, -7430, -400)),
                        new("Unused area 3", new(13330, -7390, -380)),
                    }
                ),
                new($"{StringBeforeSubMapName}Crypt 1 (Mojo Trap Action)",
                    new()
                    {
                        new("Entrance", new(-16197, -11483, -307)),
                    }
                ),
                new($"{StringBeforeSubMapName}Crypt 3 (Mojo Trap Action)",
                    new()
                    {
                        new("Entrance", new(4852, -6280, -1303)),
                    }
                ),
                new($"{StringBeforeSubMapName}Crypt 2 (Mojo Trap Action)",
                    new()
                    {
                        new("Entrance", new(9069, -2627, 118)),
                    }
                ),
                new($"{StringBeforeSubMapName}Crypt 4 (Mojo Trap Action)",
                    new()
                    {
                        new("Entrance", new(-25210, -5428, 133)),
                    }
                ),
                new("Canada hub",
                    new()
                    {
                        new("Safehouse", new(-1060, -11040, 30)),
                        new("Safehouse (top)", new(-50, -10560, 1440)),
                        new("Cabin 1 (Jean Bison)", new(-3940, 6870, 2030)),
                        new("Cabin 2", new(-12220, -3630, 1960)),
                        new("Cabin 3", new(5960, 6275, 890)),
                        new("Satellite dish", new(660, 5330, 4740)),
                        new("Plane", new(-1840, 9260, 20)),
                    }
                ),
                new($"{StringBeforeSubMapName}Cabins",
                    new()
                    {
                        new("Cabin 1", new(-8820, -8470, 130)),
                        new("Cabin 2", new(8370, -8500, 130)),
                        new("Cabin 3", new(8370, 6260, 130)),
                    }
                ),
                new($"{StringBeforeSubMapName}Train (Aerial Assault / Theft on Rails)",
                    new()
                    {
                        new("Back", new(0, -8400, 120)),
                        new("Front", new(40, 21300, 120)),
                    }
                ),
                new($"{StringBeforeSubMapName}Train (Operation)",
                    new()
                    {
                        new("Back", new(0, -8400, 120)),
                        new("Front", new(0, 25100, 120)),
                        new("Jean Bison", new(0, 2380, 120)),
                    }
                ),
                new($"{StringBeforeSubMapName}Train (Ride the Iron Horse)",
                    new()
                    {
                        new("Back", new(0, -8400, 120)),
                        new("Front", new(0, 25100, 120)),
                    }
                ),
                new("Canada 2 hub",
                    new()
                    {
                        new("Safehouse", new(2290, -3380, 560)),
                        new("Van", new(8420, -970, -810)),
                        new("Sawmill 1", new(520, 7270, 1825)),
                        new("Sawmill 2 / Laser Redirection", new(-5140, 7200, 1470)),
                        new("Sawmill 3 / RC Combat Club", new(-5700, -6390, 1480)),
                        new("Bomb fishing spot", new(-4670, -1190, 920)),
                        new("Battery silo", new(-9480, -3050, 1490)),
                        new("Lighthouse", new(-12750, -3500, -330)),
                        new("Lighthouse (top)", new(-13700, -3850, 4060)),
                    }
                ),
                new($"{StringBeforeSubMapName}RC Combat Club",
                    new()
                    {
                        new("Moose head", new(19390, -1080, 1720)),
                        new("Sawblade crawl", new(22040, -2520, 1470)),
                    }
                ),
                new($"{StringBeforeSubMapName}Sawmill",
                    new()
                    {
                        new("Entrance", new(-1400, 22123, 1200)),
                        new("Vault", new(760, 20930, 880)),
                        new("Lasers", new(-268, 20348, 1828)),
                        new("Lever", new(690, 22320, 1190)),
                    }
                ),
                new($"{StringBeforeSubMapName}Lighthouse",
                    new()
                    {
                        new("Top entrance", new(-290, 375, 5845)),
                        new("Bottom", new(0, 0, 960)),
                        new("Recon", new(680, 1060, 1015)),
                    }
                ),
                new($"{StringBeforeSubMapName}Bear cave",
                    new()
                    {
                        new("Entrance", new(-2525, 2960, 30)),
                        new("Large ice wall", new(1000, 5255, 165)),
                    }
                ),
                new($"{StringBeforeSubMapName}Sawmill (Boss)",
                    new()
                    {
                        new("Arena", new(1140, -520, 75)),
                        new("Control room", new(620, 840, 1800)),
                    }
                ),
                new("Blimp hub",
                    new()
                    {
                        new("Safehouse", new(11400, -25, 960)),
                        new("Safehouse (top)", new(10840, -485, 3100)),
                        new("Balloon 1", new(6540, -2710, 2360)),
                        new("Balloon 2", new(6480, 2690, 2360)),
                        new("Engine 1", new(-10220, 4055, 3960)),
                        new("Engine 2", new(-10270, -4060, 3960)),
                        new("Center", new(0, 0, 2810)),
                    }
                ),
                new($"{StringBeforeSubMapName}Blimp HQ",
                    new()
                    {
                        new("Entrance", new(-3890, 0, 730)),
                        new("Clockwerk", new(135, 0, 360)),
                        new("Neyla", new(5160, 0, 100)),
                        new("Center", new(0, 0, -670)),
                    }
                ),
                new($"{StringBeforeSubMapName}Engine room 1 (Bentley & Murray)",
                    new()
                    {
                        new("Entrance", new(-2190, 130, 10)),
                        new("Control room", new(-2250, -20, 710)),
                    }
                ),
                new($"{StringBeforeSubMapName}Engine room 2 (Sly & Bentley)",
                    new()
                    {
                        new("Entrance", new(-2140, 140, 10)),
                        new("Control room", new(-2250, -20, 710)),
                    }
                ),
                new($"{StringBeforeSubMapName}Engine room 3 (Murray & Sly)",
                    new()
                    {
                        new("Entrance", new(-2110, 430, 10)),
                        new("Control room", new(-2250, -85, 710)),
                    }
                ),
                new($"{StringBeforeSubMapName}Paris (Clock-la)",
                    new()
                    {
                        new("Spawn (sky)", new(-12560, 580, 86720)),
                        new("Clock-la (sky)", new(22803, -1340, 76170)),
                        new("Ground level", new(-100, -666, -130)),
                        new("Destroyed walkway", new(920, -7480, 1580)),
                    }
                )
            };
        }
    }
}
