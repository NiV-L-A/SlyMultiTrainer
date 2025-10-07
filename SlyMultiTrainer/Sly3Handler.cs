using System.Numerics;
using static SlyMultiTrainer.Util;

namespace SlyMultiTrainer
{
    public class Sly3Handler : GameBase_t
    {
        public string ReloadAddress = "";
        public string ReloadValuesAddress = "";
        public string FKXListCount = "";
        public string CameraPointer = "";
        public string DeathBarriersAddress = "";
        public string ActiveCharacterPointer = "";
        public string ActiveCharacterIdAddress = "";
        public string StringTableCountAddress = "";
        public string IsLoadingAddress = "";
        public DAG_t DAG;

        private string _offsetTransformation1 = "44";
        private string _offsetTransformation2 = "48";
        private string _offsetTransformation3 = "4C";
        private string _offsetHealth = "16C";
        private string _offsetGadgetPower = "174";
        private string _offsetController = "134";
        private string _offsetInfiniteDbJump = "33C";
        private string _offsetSpeedMultiplier = "354";
        private string _offsetInvulnerable = "180";
        private string _offsetUndetectable = "1160";

        private Memory.Mem _m;
        private Form1 _form;

        public Sly3Handler(Memory.Mem m, Form1 form, string region) : base(m, form, region)
        {
            _m = m;
            _form = form;
            DAG = new(m);
            DAG.SetVersion(DAG_VERSION.V3);

            DAG.OffsetId = "18";
            DAG.OffsetNextNodePointer = "20";
            DAG.OffsetState = "44";
            DAG.OffsetGoalDescription = "4C";
            DAG.OffsetFocusCount1 = "54";
            DAG.OffsetFocusCount2 = "58";
            DAG.OffsetMissionName = "60";
            DAG.OffsetMissionDescription = "64";
            DAG.OffsetClusterPointer = "6C";
            DAG.OffsetChildrenCount = "90";
            DAG.OffsetCheckpointEntranceValue = "A8";
            DAG.OffsetSuckPointer = "AC";
            DAG.OffsetAttributes = "D0";
            DAG.OffsetAttributesForCluster = "D0";
            DAG.GetStringFromId = GetStringFromId;
            DAG.LoadMap = LoadMap;
            DAG.WriteActCharId = WriteActCharId;

            if (region == "NTSC")
            {
                ReloadAddress = "4797C4";
                ReloadValuesAddress = "2EDFD8";
                FKXListCount = "479AAC";
                ClockAddress = "36BBA0";
                CoinsAddress = "468DDC";
                GadgetAddress = "468DCC";
                CameraPointer = "47933C";
                DrawDistanceAddress = $"{CameraPointer},114";
                FOVAddress = $"{CameraPointer},11C";
                ResetCameraAddress = $"{CameraPointer},2F4";
                MapIdAddress = "47989C";
                DeathBarriersAddress = "370A80";
                GuardAIAddress = "370A8C";
                ActiveCharacterPointer = "36F84C";
                ActiveCharacterIdAddress = "36C710";
                DAG.RootNodePointer = "478C8C";
                DAG.CurrentCheckpointNodePointer = "4794CC";
                DAG.TaskStringTablePointer = "4794A8";
                DAG.ClusterIdAddress = "36DB98";
                DAG.SavefileStartAddress = "468D30";
                DAG.SavefileValuesOffsetsTablePointer = "4793CC";
                DAG.Sly3Time = "36BC20";
                DAG.Sly3Flag = "479754";
                StringTableCountAddress = "47A2D4";
                IsLoadingAddress = "467B00";
            }
            else if (region == "PAL")
            {
                ReloadAddress = "47AE44";
                ReloadValuesAddress = "2EE658";
                FKXListCount = "47B12C";
                ClockAddress = "36C620";
                CoinsAddress = "46A45C";
                GadgetAddress = "46A44C";
                CameraPointer = "47A9BC";
                DrawDistanceAddress = $"{CameraPointer},114";
                FOVAddress = $"{CameraPointer},11C";
                ResetCameraAddress = $"{CameraPointer},2F4";
                MapIdAddress = "47AF1C";
                DeathBarriersAddress = "371500";
                GuardAIAddress = "37150C";
                ActiveCharacterPointer = "3702CC";
                ActiveCharacterIdAddress = "36D190";
                DAG.RootNodePointer = "47A30C";
                DAG.CurrentCheckpointNodePointer = "47AB4C";
                DAG.TaskStringTablePointer = "47AB28";
                DAG.ClusterIdAddress = "36E618";
                DAG.SavefileStartAddress = "46A3B0";
                DAG.SavefileValuesOffsetsTablePointer = "47AA4C";
                DAG.Sly3Time = "36C6A0";
                DAG.Sly3Flag = "47ADD4";
                StringTableCountAddress = "47B954";
                IsLoadingAddress = "469180";
            }
            else if (region == "NTSC-K")
            {
                ReloadAddress = "47B8C4";
                ReloadValuesAddress = "2EEF58";
                FKXListCount = "47BBAC";
                ClockAddress = "36D0A0";
                CoinsAddress = "46AEDC";
                GadgetAddress = "46AECC";
                CameraPointer = "47B43C";
                DrawDistanceAddress = $"{CameraPointer},114";
                FOVAddress = $"{CameraPointer},11C";
                ResetCameraAddress = $"{CameraPointer},2F4";
                MapIdAddress = "47B99C";
                DeathBarriersAddress = "371F80";
                GuardAIAddress = "371F8C";
                ActiveCharacterPointer = "370D4C";
                ActiveCharacterIdAddress = "36DC10";
                DAG.RootNodePointer = "47AD8C";
                DAG.CurrentCheckpointNodePointer = "47B5CC";
                DAG.TaskStringTablePointer = "47B5A8";
                DAG.ClusterIdAddress = "36F098";
                DAG.SavefileStartAddress = "46AE30";
                DAG.SavefileValuesOffsetsTablePointer = "47B4CC";
                DAG.Sly3Time = "36D120";
                DAG.Sly3Flag = "47B854";
                StringTableCountAddress = "47C3D4";
                IsLoadingAddress = "469C00";
            }
            else if (region == "NTSC July 16")
            {
                // undetectability
                _offsetSpeedMultiplier = "358";
                ReloadAddress = "46BB24";
                ReloadValuesAddress = "2DEB08";
                FKXListCount = "46BE0C";
                ClockAddress = "35F6A0";
                CoinsAddress = "45B0A8";
                GadgetAddress = "45B09C";
                CameraPointer = "46B5AC";
                DrawDistanceAddress = $"{CameraPointer},114";
                FOVAddress = $"{CameraPointer},11C";
                ResetCameraAddress = $"{CameraPointer},324";
                MapIdAddress = "46BBFC";
                DeathBarriersAddress = "361D90";
                GuardAIAddress = "362F1C";
                ActiveCharacterPointer = "361D5C";
                ActiveCharacterIdAddress = "45AFC4";
                DAG.RootNodePointer = "46AEF4";
                DAG.CurrentCheckpointNodePointer = "46B738";
                DAG.TaskStringTablePointer = "46B718";
                DAG.ClusterIdAddress = "3600D0";
                DAG.SavefileStartAddress = "45AFB0";
                DAG.SavefileValuesOffsetsTablePointer = "46B63C";
                DAG.Sly3Time = "35F720";
                DAG.Sly3Flag = "46BAC8";
                StringTableCountAddress = "46C624";
                IsLoadingAddress = "459D80";

                for (int i = 23; i < 35; i++)
                {
                    Maps[i].IsVisible = false;
                }

                Maps.RemoveRange(36, 4);
            }
            else if (region == "NTSC E3 Demo")
            {
                _offsetSpeedMultiplier = "328";
                _offsetInfiniteDbJump = "30C";
                DAG.SetVersion(DAG_VERSION.V2);
                ReloadAddress = "460C60";
                ReloadValuesAddress = "461900,0"; // pointer
                FKXListCount = "460F6C";
                ClockAddress = "36FA00";
                CoinsAddress = "453F0C";
                GadgetAddress = "453F04";
                CameraPointer = "46080C";
                DrawDistanceAddress = $"{CameraPointer},234";
                FOVAddress = $"{CameraPointer},23C";
                ResetCameraAddress = $"{CameraPointer},444";
                MapIdAddress = "453E28";
                DeathBarriersAddress = "";
                GuardAIAddress = "37215C";
                ActiveCharacterPointer = "37211C";
                ActiveCharacterIdAddress = "453E2C";
                StringTableCountAddress = "461790";
                IsLoadingAddress = "452380";

                DAG.RootNodePointer = "460158";
                DAG.CurrentCheckpointNodePointer = "460998";
                DAG.TaskStringTablePointer = "460978";
                DAG.ClusterIdAddress = "370488";
                DAG.SavefileStartAddress = "453E20";
                DAG.SavefileValuesOffsetsTablePointer = "46089C";
                DAG.Sly3Time = "36FA80";
                DAG.Sly3Flag = "460C00";
                DAG.OffsetCheckpointEntranceValue = "A4";
                DAG.OffsetMissionName = "";
                DAG.OffsetClusterPointer = "68";
                DAG.OffsetChildrenCount = "8C";
                DAG.OffsetAttributesForCluster = "C0";
                DAG.OffsetAttributes = "C4";

                Maps[0].IsVisible = false;
                Maps[1].Name = "dvd_menu";
                Maps[1].IsVisible = true;
                Maps.RemoveAt(2);
                Maps.RemoveAt(6);
                Maps[3].IsVisible = false;
                Maps[4].IsVisible = false;
                Maps[5].IsVisible = false;
                Maps[7].IsVisible = false;
                Maps[8].IsVisible = false;
                Maps[9].IsVisible = false;
                Maps[10].IsVisible = false;
                Maps[12].IsVisible = false;
                Maps[13].IsVisible = false;
                Maps[14].IsVisible = false;
                Maps[15].IsVisible = false;
                Maps[16].IsVisible = false;
                Maps[17].IsVisible = false;
                Maps[18].IsVisible = false;
                Maps.RemoveRange(20, 18);
            }
            else if (region == "PAL Demo")
            {
                ReloadAddress = "485744";
                ReloadValuesAddress = "2E9DD8";
                FKXListCount = "485A2C";
                ClockAddress = "38F660";
                CoinsAddress = "474D7C";
                GadgetAddress = "474D74";
                CameraPointer = "4852BC";
                DrawDistanceAddress = $"{CameraPointer},114";
                FOVAddress = $"{CameraPointer},11C";
                ResetCameraAddress = $"{CameraPointer},2F4";
                MapIdAddress = "474CD8";
                DeathBarriersAddress = "393380";
                GuardAIAddress = "39338C";
                ActiveCharacterPointer = "39330C";
                ActiveCharacterIdAddress = "3901D0";
                DAG.RootNodePointer = "484C0C";
                DAG.CurrentCheckpointNodePointer = "48544C";
                DAG.TaskStringTablePointer = "485428";
                DAG.ClusterIdAddress = "391658";
                DAG.SavefileStartAddress = "474CD0";
                DAG.SavefileValuesOffsetsTablePointer = "48534C";
                DAG.Sly3Time = "38F6E0";
                DAG.Sly3Flag = "4856D4";
                StringTableCountAddress = "486254";
                IsLoadingAddress = "473A80";

                Maps[0].IsVisible = false;
                Maps[1].IsVisible = true;
                Maps[2].IsVisible = false;
                Maps[4].IsVisible = false;
                Maps[5].IsVisible = false;
                Maps[7].IsVisible = false;
                Maps[9].IsVisible = false;
                Maps[10].IsVisible = false;
                Maps[11].IsVisible = false;
                Maps[12].IsVisible = false;
                Maps[14].IsVisible = false;
                Maps[15].IsVisible = false;
                Maps[16].IsVisible = false;
                Maps[17].IsVisible = false;
                Maps[18].IsVisible = false;
                Maps[19].IsVisible = false;
                Maps[20].IsVisible = false;
                Maps.RemoveRange(22, 18);
            }
            else if (region == "PAL August 2")
            {
                _offsetHealth = "170";
                _offsetGadgetPower = "178";
                _offsetInfiniteDbJump = "34C";
                _offsetInvulnerable = "184";
                _offsetUndetectable = "1190";
                _offsetSpeedMultiplier = "368";

                ReloadAddress = "4AF7CC";
                ReloadValuesAddress = "2F8068";
                FKXListCount = "4AFAB4";
                ClockAddress = "38B1A0";
                CoinsAddress = "49E750";
                GadgetAddress = "49E740";
                CameraPointer = "4AF33C";
                DrawDistanceAddress = $"{CameraPointer},114";
                FOVAddress = $"{CameraPointer},11C";
                ResetCameraAddress = $"{CameraPointer},324";
                MapIdAddress = "4AF8A4";
                DeathBarriersAddress = "38EE98";
                GuardAIAddress = "38EEA4";
                ActiveCharacterPointer = "38EE5C";
                ActiveCharacterIdAddress = "38BD40";
                DAG.RootNodePointer = "4AEC8C";
                DAG.CurrentCheckpointNodePointer = "4AF4C8";
                DAG.TaskStringTablePointer = "4AF4A8";
                DAG.ClusterIdAddress = "38D1B8";
                DAG.SavefileStartAddress = "49E6B0";
                DAG.SavefileValuesOffsetsTablePointer = "4AF3CC";
                DAG.Sly3Time = "38B220";
                DAG.Sly3Flag = "4AF770";
                StringTableCountAddress = "4B02C4";
                IsLoadingAddress = "49D480";
            }
            else if (region == "PAL September 2")
            {
                _offsetHealth = "170";
                _offsetGadgetPower = "178";
                _offsetInfiniteDbJump = "34C";
                _offsetInvulnerable = "184";
                _offsetUndetectable = "1170";
                _offsetSpeedMultiplier = "364";

                ReloadAddress = "4BE9C4";
                ReloadValuesAddress = "304248";
                FKXListCount = "4BECAC";
                ClockAddress = "39A1A0";
                CoinsAddress = "4AD95C";
                GadgetAddress = "4AD94C";
                CameraPointer = "4BE53C";
                DrawDistanceAddress = $"{CameraPointer},114";
                FOVAddress = $"{CameraPointer},11C";
                ResetCameraAddress = $"{CameraPointer},2F4";
                MapIdAddress = "4BEA9C";
                DeathBarriersAddress = "39DF48";
                GuardAIAddress = "39DF54";
                ActiveCharacterPointer = "39DECC";
                ActiveCharacterIdAddress = "39AD40";
                DAG.RootNodePointer = "4BDE8C";
                DAG.CurrentCheckpointNodePointer = "4BE6CC";
                DAG.TaskStringTablePointer = "4BE6A8";
                DAG.ClusterIdAddress = "39C1C8";
                DAG.SavefileStartAddress = "4AD8B0";
                DAG.SavefileValuesOffsetsTablePointer = "4BE5CC";
                DAG.Sly3Time = "39A220";
                DAG.Sly3Flag = "4BE954";
                StringTableCountAddress = "4BF4D4";
                IsLoadingAddress = "4AC680";
            }

            _form.UpdateUI(_form.cmbMaps, Maps.Where(x => x.IsVisible).ToList());
            _form.UpdateUI(_form.cmbMaps, Maps, "Tag");
            _form.UpdateUI(_form.cmbActChar, Characters);
        }

        public override void CustomTick()
        {

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

        public override bool IsActCharAvailable()
        {
            return _m.ReadInt(ActiveCharacterPointer) != 0;
        }

        public override Vector3 ReadActCharPosition()
        {
            return ReadPositionFromPointerToEntity(ActiveCharacterPointer);
        }

        public override void WriteActCharPosition(Vector3 value)
        {
            int q = _m.ReadInt($"{ActiveCharacterPointer},D4");
            _m.WriteMemory($"{ActiveCharacterPointer},D4", "int", "0");
            WritePositionFromPointerToEntity(ActiveCharacterPointer, value);
            Thread.Sleep(10);
            _m.WriteMemory($"{ActiveCharacterPointer},D4", "int", q.ToString());
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

        public override int ReadActCharHealth()
        {
            int health = _m.ReadInt($"{ActiveCharacterPointer},{_offsetHealth}");
            return health;
        }

        public override void WriteActCharHealth(int value)
        {
            _m.WriteMemory($"{ActiveCharacterPointer},{_offsetHealth}", "int", value.ToString());
        }

        public override void FreezeActCharHealth(int value = 0)
        {
            if (value == 0)
            {
                value = ReadActCharHealth();
            }
            _m.FreezeValue($"{ActiveCharacterPointer},{_offsetHealth}", "int", value.ToString());
        }

        public override void UnfreezeActCharHealth()
        {
            _m.UnfreezeValue($"{ActiveCharacterPointer},{_offsetHealth}");
        }

        public override Controller_t GetController()
        {
            return new(_m, $"{ActiveCharacterPointer},{_offsetController},18");
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
            string address = $"{ActiveCharacterPointer},{_offsetUndetectable},1C";
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
            if (Region == "NTSC July 16" || Region == "NTSC E3 Demo")
            {
                if (enableInfDbJump)
                {
                    _m.FreezeValue($"{ActiveCharacterPointer},{_offsetInfiniteDbJump}+C", "int", "1");
                }
                else
                {
                    _m.UnfreezeValue($"{ActiveCharacterPointer},{_offsetInfiniteDbJump}+C");
                }
                return;
            }

            if (enableInfDbJump)
            {
                _m.FreezeValue($"{ActiveCharacterPointer},{_offsetInfiniteDbJump}", "int", "0");
                _m.FreezeValue($"{ActiveCharacterPointer},{_offsetInfiniteDbJump}+C", "int", "0");
            }
            else
            {
                _m.UnfreezeValue($"{ActiveCharacterPointer},{_offsetInfiniteDbJump}");
                _m.UnfreezeValue($"{ActiveCharacterPointer},{_offsetInfiniteDbJump}+C");
            }
        }

        public override bool IsLoading()
        {
            if (_m.ReadInt(IsLoadingAddress) != 3)
            {
                return true;
            }
            return false;
        }

        public override void UnfreezeActCharGadgetPower()
        {
            _m.UnfreezeValue($"{ActiveCharacterPointer},{_offsetGadgetPower}");
        }

        public override void FreezeActCharGadgetPower(int value = 0)
        {
            if (value == 0)
            {
                value = ReadActCharGadgetPower();
            }
            _m.FreezeValue($"{ActiveCharacterPointer},{_offsetGadgetPower}", "int", value.ToString());
        }

        public int ReadActCharGadgetPower()
        {
            int GadgetPower = _m.ReadInt($"{ActiveCharacterPointer},{_offsetGadgetPower}");
            return GadgetPower;
        }

        public void WriteActCharGadgetPower(int value)
        {
            _m.WriteMemory($"{ActiveCharacterPointer},{_offsetGadgetPower}", "int", value.ToString());
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
            _m.WriteMemory($"{ActiveCharacterIdAddress}+4", "int", id2.ToString());
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

        public override void LoadMap(int mapId)
        {
            // current map
            if (mapId == -1)
            {
                mapId = GetMapId();
            }

            byte[] data = _m.ReadBytes($"{ReloadValuesAddress}+{mapId * 0x40:X8}", 0x40);
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

        public void LoadMapFull(int mapId)
        {
            _m.WriteMemory($"{ReloadAddress}+A0", "int", "-1");
            LoadMap(mapId);
        }

        public void ReloadMap()
        {
            _m.WriteMemory(ReloadAddress, "int", "1");
        }

        public void ToggleDeathBarriers(bool removeDeathBarriers)
        {
            if (removeDeathBarriers)
            {
                _m.FreezeValue(DeathBarriersAddress, "int", "1");
            }
            else
            {
                _m.UnfreezeValue(DeathBarriersAddress);
                _m.WriteMemory(DeathBarriersAddress, "int", "0");
            }
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
                    string descriptionName = _m.ReadNullTerminatedString(stringPointer.ToString("X"), System.Text.Encoding.Unicode);
                    // "Carmelita's Gunner"
                    descriptionName = descriptionName.Replace('\ufffd', '\'');
                    return descriptionName;
                }
            }

            return "";
        }

        protected override List<Character_t> GetCharacters()
        {
            return new List<Character_t>
            {
                new("Sly", 24),
                new("Bentley", 25),
                new("Murray", 26),
            };
        }

        protected override List<Map_t> GetMaps()
        {
            return new List<Map_t>
            {
                new("DVD Menu",
                    new()
                    {
                        new("None", new(0, 0, 0)),
                    }
                ),
                new("sampler_menu",
                    new(),
                    false
                ),
                new("Hazard Room",
                    new()
                    {
                        new("Center", new(3550, 440, 100)),
                        new("Top", new(3580, 630, 3440)),
                        new("Safehouse", new(6640, 680, 90)),
                    }
                ),
                new("Venice Hub",
                    new()
                    {
                        new("Safehouse", new(200, -2090, 173)),
                        new("Safehouse (Top)", new(863, -1420, 1266)),
                        new("Police HQ", new(-7570, 1670, 1962)),
                        new("Ferris Wheel", new(6900, 1480, 160)),
                        new("Stage", new(6250, 8210, 260)),
                        new("Fountain", new(-6670, 8550, 700)),
                        new("Aquarium", new(8040, -4365, 160)),
                    }
                ),
                new($"{StringBeforeSubMapName}Canal Chase",
                    new()
                    {
                        new("Boat", new(0, 0, 130)),
                        new("Intersection 1", new(665, -12555, 140)),
                        new("Intersection 2", new(27250, 28580, 140)),
                    }
                ),
                new($"{StringBeforeSubMapName}Coffeehouses",
                    new()
                    {
                        new("Entrance 1", new(710, -5000, 125)),
                        new("Entrance 2", new(1070, 100, 125)),
                        new("Entrance 3", new(1160, 5000, 125)),
                        new("Safe 1", new(-1710, -4990, 125)),
                        new("Safe 2", new(-1750, 10, 125)),
                        new("Safe 3", new(-3245, 4990, 125)),
                        new("Roof", new(-1780, -4540, 1175)),
                    }
                ),
                new($"{StringBeforeSubMapName}Gauntlet / Opera House",
                    new()
                    {
                        new("Main Entrance", new(-7130, -11340, 1030)),
                        new("Basement Entrance", new(14440, -4000, 1015)),
                        new("Pump Room", new(-885, -2230, 180)),
                        new("Worlitzer-700", new(-2100, 4890, 630)),
                        new("Underground Canal", new(8720, -6490, 75)),
                        new("Overlook", new(8770, -5830, 1650)),
                    }
                ),
                new($"{StringBeforeSubMapName}Police Station",
                    new()
                    {
                        new("Dimitri's Cell", new(-60, 7600, 120)),
                        new("Cell Key", new(-685, 3250, 125)),
                    }
                ),
                new("Outback Hub",
                    new()
                    {
                        new("Safehouse", new(-4570, -7190, 1525)),
                        new("Safehouse (Top)", new(-4590, -7820, 2650)),
                        new("Crane", new(-700, -1290, 4320)),
                        new("Truck", new(9820, -550, 1240)),
                        new("Guru's Hut", new(-8230, 4365, 2760)),
                        new("Guru's Cell", new(8665, 5620, 2820)),
                        new("Treeline", new(-8360, -3400, 5060)),
                        new("Plateau", new(6360, 7645, 6930)),
                    }
                ),
                new($"{StringBeforeSubMapName}Quarry / Ayers Rock",
                    new()
                    {
                        new("Drill Controls", new(270, 160, 240)),
                        new("Drill Controls (Top)", new(420, 15, 2190)),
                        new("Truck Spawn", new(-16350, 8310, 4230)),
                        new("Mine Entrance", new(3830, 13920, 70)),
                        new("Clifftop", new(16260, 12890, 12660)),
                    }
                ),
                new($"{StringBeforeSubMapName}Oil Field",
                    new()
                    {
                        new("The Claw", new(320, 10000, 70)),
                        new("Catapult", new(4820, -4470, 70)),
                        new("Drill Platform", new(-360, 620, 1235)),
                    }
                ),
                new($"{StringBeforeSubMapName}Cave 1 (Sly)",
                    new()
                    {
                        new("Entrance", new(-9345, 330, 20)),
                        new("Safe", new(6545, 125, 1111)),
                        new("Drills", new(-780, -3420, 1120)),
                    }
                ),
                new($"{StringBeforeSubMapName}Cave 2 (Guru)",
                    new()
                    {
                        new("Entrance", new(-8945, 370, -1860)),
                        new("Safe", new(-100, -4960, -610)),
                        new("Hook Conveyor Belt", new(-5970, -1800, -1335)),
                    }
                ),
                new($"{StringBeforeSubMapName}Bar",
                    new()
                    {
                        new("None", new(0, 0, 0)),
                    }
                ),
                new($"{StringBeforeSubMapName}Cave 3 (Murray)",
                    new()
                    {
                        new("Entrance", new(-10230, -1445, -1140)),
                        new("Piston", new(3380, -1870, -1020)),
                        new("Triple Piston", new(-2300, -8000, 150)),
                        new("Drilling Area", new(200, -9200, 320)),
                    }
                ),
                new("Holland Hub",
                    new()
                    {
                        new("Safehouse", new(12180, -540, 1180)),
                        new("Baron's Hangar", new(-6075, 6950, 2855)),
                        new("Forest", new(-2770, 3020, 430)),
                        new("Ramp", new(-4645, -9100, 1680)),
                        new("Barn", new(3680, -6000, 600)),
                        new("Well", new(4330, 3490, 120)),
                    }
                ),
                new($"{StringBeforeSubMapName}Hotel",
                    new()
                    {
                        new("Safehouse Entrance", new(2620, 280, 600)),
                        new("Ham", new(-535, 420, 0)),
                        new("Viking Helmet", new(830, 2950, 590)),
                        new("Outside", new(60, -6590, -545)),
                    }
                ),
                new($"{StringBeforeSubMapName}Hangar (team Belgium)",
                    new()
                    {
                        new("None", new(0, 0, 0)),
                    }
                ),
                new($"{StringBeforeSubMapName}Hangar (team Black Baron)",
                    new()
                    {
                        new("None", new(0, 0, 0)),
                    }
                ),
                new($"{StringBeforeSubMapName}Hangar (team Cooper)",
                    new()
                    {
                        new("Center", new(-180, -125, 75)),
                        new("Control Room", new(-1890, -130, 75)),
                        new("Truck", new(-340, 2220, 1030)),
                    }
                ),
                new($"{StringBeforeSubMapName}Sewers",
                    new()
                    {
                        new("Entrance", new(20150, -9850, 210)),
                        new("Iceland Hotel Path", new(16490, 7280, 210)),
                        new("Exit to Surface", new(7960, -12750, 210)),
                        new("Iceland Hotel Entrance", new(7425, 9500, 210)),
                        new("Platform", new(200, 0, 200)),
                    }
                ),
                new($"{StringBeforeSubMapName}Dogfight / Biplane Battlefield",
                    new()
                    {
                        new("Barn", new(-1890, 380, 870)),
                        new("Crop Squares", new(17800, 3210, 900)),
                        new("Bridge 1", new(-140, -14670, 620)),
                        new("Bridge 2", new(-4444, 16170, 450)),
                        new("Bridge 3", new(10460, 13260, 500)),
                        new("Plane", new(251764, -186, 0)),
                    }
                ),
                new("Two Player Hackathon",
                    new()
                    {
                        new("None", new(0, 0, 0)),
                    }
                ),
                new("China Hub",
                    new()
                    {
                        new("Safehouse", new(-5440, -7500, 2020)),
                        new("Turret Tower", new(-5330, -8415, 3500)),
                        new("Walk Across the Heavens", new(7310, -8370, 4980)),
                        new("Graveyard", new(8570, 10150, 5840)),
                        new("Statue", new(795, -2980, 1915)),
                        new("Palace", new(940, 2255, 4790)),
                    }
                ),
                new($"{StringBeforeSubMapName}Intro",
                    new()
                    {
                        new("Entrance", new(-2085, -54630, 850)),
                        new("Panda King's Perch", new(400, -50485, 1888)),
                        new("House", new(3470, -51845, 820)),
                        new("Clifftop", new(-2820, -57675, 5420)),
                    }
                ),
                new($"{StringBeforeSubMapName}Panda King's Flashback",
                    new()
                    {
                        new("None", new(0, 0, 0)),
                    }
                ),
                new($"{StringBeforeSubMapName}Tsao's Battleground",
                    new()
                    {
                        new("Top", new(-50, 3060, 840)),
                        new("Bottom", new(130, 30410, -50)),
                        new("Overlook", new(-4545, 35970, 4675)),
                    }
                ),
                new($"{StringBeforeSubMapName}Panda King's House",
                    new()
                    {
                        new("Yang", new(-240, -100, 19995)),
                        new("Yin", new(-1855, -100, 19995)),
                    }
                ),
                new($"{StringBeforeSubMapName}Tsao's Business Center",
                    new()
                    {
                        new("Laser Wall", new(-2930, 0, -50)),
                        new("Second Floor", new(1050, 1580, 645)),
                        new("Computer", new(1075, -1515, 645)),
                        new("Outside", new(-4210, -140, -185)),
                        new("Overlook", new(-10480, -4100, 2760)),
                    }
                ),
                new($"{StringBeforeSubMapName}Palace",
                    new()
                    {
                        new("Vases", new(-5270, -60, -190)),
                        new("Computer", new(-2250, 1445, 0)),
                        new("Jing King's Room", new(470, -1500, 0)),
                        new("Drill Site", new(2075, 15000, 545)),
                    }
                ),
                new($"{StringBeforeSubMapName}Treasure Temple",
                    new()
                    {
                        new("Entrance", new(-6300, -130, 330)),
                        new("Treasure Area", new(1725, 730, -330)),
                        new("Crawlspace", new(-560, 140, 1800)),
                    }
                ),
                new("Pirate Hub",
                    new()
                    {
                        new("Safehouse", new(4900, 1345, 1125)),
                        new("Safehouse (Top)", new(5590, 2310, 2680)),
                        new("Skull Keep (Top)", new(-9600, -1880, 4410)),
                        new("Waterfall (Top)", new(3625, 16360, 4435)),
                        new("Fireplace", new(-530, 7030, 1970)),
                        new("Monkeys?", new(-7415, 11565, 1520)),
                        new("Cooper Gang Ship", new(11390, -9290, 1550)),
                        new("Archipelago", new(-26550, -19930, 2100)),
                    }
                ),
                new($"{StringBeforeSubMapName}Sailing Map",
                    new()
                    {
                        new("None", new(0, 0, 0)),
                    }
                ),
                new($"{StringBeforeSubMapName}Underwater Shipwreck",
                    new()
                    {
                        new("Spawn", new(28980, -100, 2600)),
                        new("Ship (Top)", new(21020, 14180, 6030)),
                        new("Shipwreck", new(22460, 12380, -3480)),
                        new("Depths", new(21910, 8690, -7285)),
                        new("Ocean Current", new(20860, 22410, -7120)),

                    }
                ),
                new($"{StringBeforeSubMapName}Dagger Island",
                    new()
                    {
                        new("Cooper Gang Ship", new(-16760, 2940, 1000)),
                        new("Palm Tree Circle", new(-8040, -970, 1100)),
                        new("Flipped Ship", new(1620, -5250, 1140)),
                        new("Pirate Ship", new(15680, 5290, 770)),
                        new("Mountain Peak", new(2215, 10860, 7940)),
                    }
                ),
                new("Kaine Island",
                    new()
                    {
                        new("Spawn", new(-6715, -14380, -2900)),
                        new("Wall Sneak (Top)", new(-6285, -2220, -2180)),
                        new("Ventilation Shaft", new(-1870, 4765, -3855)),
                        new("Vault Entrance", new(-1085, -100, 2360)),
                        new("Ship Dock", new(7855, -24360, -3770)),
                        new("RC Car Track", new(-13650, -14110, -2190)),
                        new("Random Rope", new(-16480, 1520, -2830)),
                        new("Rock Formation", new(13730, 20650, 2100)),
                    }
                ),
                new($"{StringBeforeSubMapName}Underwater",
                    new()
                    {
                        new("Spawn", new(51570, 23850, -5745)),
                        new("Water Tube", new(745, -34265, -820)),
                        new("Boss Area", new(10200, -59780, 0)),
                    }
                ),
                new($"{StringBeforeSubMapName}Cooper Vault (entrance)",
                    new()
                    {
                        new("Center", new(0, 0, 40)),
                        new("Entrance Door", new(4350, -45, 460)),
                    }
                ),
                new($"{StringBeforeSubMapName}Cooper Vault (gauntlet)",
                    new()
                    {
                        new("Slytunkhamen II", new(-28690, 21665, -2175)),
                        new("Sir Galleth Cooper", new(-24715, 13995, -2260)),
                        new("Salim Al-Kupar", new(-13760, 12485, -2200)),
                        new("Slaigh MacCooper", new(-15325, 24680, -2190)),
                        new("Rioichi Cooper", new(-21130, 19955, -180)),
                        new("Henriette Cooper", new(-10530, 13200, 120)),
                        new("Tennesee 'Kid' Cooper", new(2010, 13275, -2280)),
                        new("Thaddeus Winslow Cooper III", new(9360, 1820, -2185)),
                        new("Otto Van Cooper", new(-2050, 2740, 0)),
                        new("Conner Cooper", new(7515, 5030, 150)),
                        new("Inner Sanctum Entrance", new(16645, -2260, 120)),
                    }
                ),
                new($"{StringBeforeSubMapName}Dr. M's Arena",
                    new()
                    {
                        new("Center", new(0, 0, 30)),
                        new("Top Platform", new(-3840, 1600, 2970)),
                    }
                ),
            };
        }
    }
}
