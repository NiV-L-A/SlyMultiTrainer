using System.Numerics;
using System.Text;
using static SlyMultiTrainer.Sly2_3_Savefile;
using static SlyMultiTrainer.Util;

namespace SlyMultiTrainer
{
    public class Sly3Handler : GameBase_t
    {
        private Memory.Mem _m;
        private Form1 _form;
        private Encoding _encoding;

        public string ReloadAddress = "";
        public string ReloadValuesAddress = "";
        public string FKXListCountAddress = "";
        public string CameraPointer = "";
        public string ActiveCharacterPointer = "";
        public string ActiveCharacterIdAddress = "";
        public string StringTableCountAddress = "";
        public string IsLoadingAddress = "";
        public string EntranceRootNodePointer = "";
        public DAG_t DAG;
        public Sly2_3_Savefile Savefile;

        private string _offsetTransformationOrigin = "44";
        private string _offsetTransformationLocal = "";
        private string _offsetTransformationWorld = "";
        private string _offsetTransformationFinal = "";
        private string _offsetCollision = "DC";
        private string _offsetInvulnerable = "100";
        private string _offsetRadTarget = "1BC";
        private string _offsetHealth = "16C";
        private string _offsetGadgetPower = "174";
        private string _offsetDeltaTranslation = "250";
        private string _offsetInfiniteDbJump = "33C";
        private string _offsetSpeedMultiplier = "354";
        private string _offsetGadgetBinds = "1250";
        private string _offsetUndetectable = "1280";
        private string _offsetEntranceTransformation = "50";
        private string _offsetCurrentDialogue = "60";
        private string _offsetDialogueFlags = "5C";
        private int _lastInvulnerableValue = 0;

        public Sly3Handler(Form1 form, Memory.Mem m, Build_t build) : base(form, m, build)
        {
            _m = m;
            _form = form;
            DAG = new(m);
            DAG.SetVersion(DAG_VERSION.V3);
            Savefile = new(m);
            Savefile.SetVersion(SAVEFILE_VERSION.V1);
            _encoding = Encoding.Unicode;

            DAG.OffsetState = "44";
            DAG.OffsetGoalDescription = "4C";
            DAG.OffsetFocusCount = "54";
            DAG.OffsetCompleteCount = "58";
            DAG.OffsetMissionName = "60";
            DAG.OffsetMissionDescription = "64";
            DAG.OffsetClusterPointer = "6C";
            DAG.OffsetChildrenCount = "90";
            DAG.OffsetCheckpointEntranceValue = "A8";
            DAG.OffsetAttributes = "D0";
            DAG.OffsetAttributesForCluster = "D0";
            DAG.OffsetVerticalLayer = "108";
            DAG.GetStringFromId = GetStringFromId;
            DAG.LoadMap = LoadMap;
            DAG.WriteActCharId = WriteActCharId;

            if (build.Region == Util.BuildRegions[BUILD_NAME.NTSC])
            {
                // SCUS-97464 - 8BC95883
                ReloadAddress = "4797C4";
                ReloadValuesAddress = "2EDFD8";
                FKXListCountAddress = "479AAC";
                ClockAddress = "36BBA0";
                CoinsAddress = "468DDC";
                GadgetAddress = "468DCC";
                CameraPointer = "47933C";
                DrawDistanceAddress = $"{CameraPointer},114";
                FOVAddress = $"{CameraPointer},11C";
                ResetCameraAddress = $"{CameraPointer},2F4";
                CanCameraNoclipAddress = "349FA0";
                MapIdAddress = "47989C";
                GuardAIAddress = "370A8C";
                ActiveCharacterPointer = "36F84C";
                ActiveCharacterIdAddress = "36C710";
                StringTableCountAddress = "47A2D4";
                IsLoadingAddress = "467B00";
                EntranceRootNodePointer = "478680";
                DAG.RootNodePointer = "478C8C";
                DAG.CurrentCheckpointNodePointer = "4794CC";
                DAG.ClusterIdAddress = "36DB98";
                DAG.Sly3Time = "36BC20";
                DAG.Sly3Flag = "479754";
                Savefile.SavefileStartAddress = "468D30";
                Savefile.SavefileAddressTablePointer = "4793CC";
                Savefile.SavefileStringTablePointer = "4794A8";
                ControllerAddress = "36E758";
                DialoguePointer = "475200";
                SkipFMVPointer = "389C18";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.PAL])
            {
                // SCES-53409 - 8164C614
                ReloadAddress = "47AE44";
                ReloadValuesAddress = "2EE658";
                FKXListCountAddress = "47B12C";
                ClockAddress = "36C620";
                CoinsAddress = "46A45C";
                GadgetAddress = "46A44C";
                CameraPointer = "47A9BC";
                DrawDistanceAddress = $"{CameraPointer},114";
                FOVAddress = $"{CameraPointer},11C";
                ResetCameraAddress = $"{CameraPointer},2F4";
                CanCameraNoclipAddress = "34AA20";
                MapIdAddress = "47AF1C";
                GuardAIAddress = "37150C";
                ActiveCharacterPointer = "3702CC";
                ActiveCharacterIdAddress = "36D190";
                StringTableCountAddress = "47B954";
                IsLoadingAddress = "469180";
                EntranceRootNodePointer = "479D00";
                DAG.RootNodePointer = "47A30C";
                DAG.CurrentCheckpointNodePointer = "47AB4C";
                DAG.ClusterIdAddress = "36E618";
                DAG.Sly3Time = "36C6A0";
                DAG.Sly3Flag = "47ADD4";
                Savefile.SavefileStartAddress = "46A3B0";
                Savefile.SavefileAddressTablePointer = "47AA4C";
                Savefile.SavefileStringTablePointer = "47AB28";
                ControllerAddress = "36F1D8";
                DialoguePointer = "476880";
                SkipFMVPointer = "38AA98";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCK])
            {
                // SCKA-20063 - A8CC1583
                ReloadAddress = "47B8C4";
                ReloadValuesAddress = "2EEF58";
                FKXListCountAddress = "47BBAC";
                ClockAddress = "36D0A0";
                CoinsAddress = "46AEDC";
                GadgetAddress = "46AECC";
                CameraPointer = "47B43C";
                DrawDistanceAddress = $"{CameraPointer},114";
                FOVAddress = $"{CameraPointer},11C";
                ResetCameraAddress = $"{CameraPointer},2F4";
                CanCameraNoclipAddress = "34B4A0";
                MapIdAddress = "47B99C";
                GuardAIAddress = "371F8C";
                ActiveCharacterPointer = "370D4C";
                ActiveCharacterIdAddress = "36DC10";
                StringTableCountAddress = "47C3D4";
                IsLoadingAddress = "469C00";
                EntranceRootNodePointer = "47A780";
                DAG.RootNodePointer = "47AD8C";
                DAG.CurrentCheckpointNodePointer = "47B5CC";
                DAG.ClusterIdAddress = "36F098";
                DAG.Sly3Time = "36D120";
                DAG.Sly3Flag = "47B854";
                Savefile.SavefileStartAddress = "46AE30";
                Savefile.SavefileAddressTablePointer = "47B4CC";
                Savefile.SavefileStringTablePointer = "47B5A8";
                ControllerAddress = "36FC58";
                DialoguePointer = "477300";
                SkipFMVPointer = "38B518";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCDemoApril18])
            {
                DAG.SetVersion(DAG_VERSION.V2);
                _offsetDeltaTranslation = "240";
                _offsetSpeedMultiplier = "328";
                _offsetInfiniteDbJump = "318";
                _offsetGadgetBinds = "1240";
                _offsetUndetectable = "126C";
                DAG.Sly3Time = "36FA80";
                DAG.Sly3Flag = "460C00";
                DAG.OffsetCheckpointEntranceValue = "A4";
                DAG.OffsetMissionName = "";
                DAG.OffsetClusterPointer = "68";
                DAG.OffsetChildrenCount = "8C";
                DAG.OffsetAttributesForCluster = "C0";
                DAG.OffsetAttributes = "C4";
                DAG.OffsetVerticalLayer = "FC";

                ReloadAddress = "460C60";
                ReloadValuesAddress = "461900,0"; // pointer
                FKXListCountAddress = "460F6C";
                ClockAddress = "36FA00";
                CoinsAddress = "453F0C";
                GadgetAddress = "453F04";
                CameraPointer = "46080C";
                DrawDistanceAddress = $"{CameraPointer},234";
                FOVAddress = $"{CameraPointer},23C";
                ResetCameraAddress = $"{CameraPointer},444";
                CanCameraNoclipAddress = "325DD8";
                MapIdAddress = "453E28";
                GuardAIAddress = "37215C";
                ActiveCharacterPointer = "37211C";
                ActiveCharacterIdAddress = "453E2C";
                StringTableCountAddress = "461790";
                IsLoadingAddress = "452380";
                EntranceRootNodePointer = "45FBAC";
                DAG.RootNodePointer = "460158";
                DAG.CurrentCheckpointNodePointer = "460998";
                DAG.ClusterIdAddress = "370488";
                Savefile.SavefileStartAddress = "453E20";
                Savefile.SavefileAddressTablePointer = "46089C";
                Savefile.SavefileStringTablePointer = "460978";
                ControllerAddress = "371018";
                DialoguePointer = "45C6C0";
                SkipFMVPointer = "374550";

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

                Gadgets = new()
                {
                    new()
                    {
                        new(_gadgetNames[GADGET_NAME.SmokeBomb], 0x15),
                        new(_gadgetNames[GADGET_NAME.CombatDodge], 0x16),
                        new(_gadgetNames[GADGET_NAME.UnknownRocketBoots], 0x17),
                        new(_gadgetNames[GADGET_NAME.ThiefReflexes], 0x1B),
                        new(_gadgetNames[GADGET_NAME.FeralPounce], 0x1C),
                        new(_gadgetNames[GADGET_NAME.MegaJump], 0x1D),
                        new(_gadgetNames[GADGET_NAME.InsanityStrike], 0x20),
                        new(_gadgetNames[GADGET_NAME.VoltageAttack], 0x21),
                        new(_gadgetNames[GADGET_NAME.RageBomb], 0x23),
                        new(_gadgetNames[GADGET_NAME.MusicBox], 0x24),
                        new(_gadgetNames[GADGET_NAME.ShadowPowerLevel1], 0x26),
                        new(_gadgetNames[GADGET_NAME.TimeRush], 0x27),
                        new(_gadgetNames[GADGET_NAME.VeniceDisguise], 0x28),
                        new(_gadgetNames[GADGET_NAME.PopeDisguise], 0x29),
                        new(_gadgetNames[GADGET_NAME.RocketBoots], 0x2E),
                        new(_gadgetNames[GADGET_NAME.ShadowPowerLevel2], 0x2F),
                        new(_gadgetNames[GADGET_NAME.Shield], 0x31),
                    },

                    new()
                    {
                        new(_gadgetNames[GADGET_NAME.TriggerBomb], 0x6),
                        new(_gadgetNames[GADGET_NAME.FishingPole], 0x2A),
                        new(_gadgetNames[GADGET_NAME.Cube], 0x7),
                        new(_gadgetNames[GADGET_NAME.SnoozeBomb], 0x8),
                        new(_gadgetNames[GADGET_NAME.TemporalLock], 0xD),
                        new(_gadgetNames[GADGET_NAME.AdrenalineBurst], 0x9),
                        new(_gadgetNames[GADGET_NAME.HealthExtractor], 0xA),
                        new(_gadgetNames[GADGET_NAME.GrappleCam], 0x2D),
                        new(_gadgetNames[GADGET_NAME.ReductionBomb], 0xC),
                    },

                    new()
                    {
                        new(_gadgetNames[GADGET_NAME.BeTheBall], 0x2B),
                        new(_gadgetNames[GADGET_NAME.BerserkerCharge], 0x12),
                        new(_gadgetNames[GADGET_NAME.GutturalRoar], 0x13),
                        new(_gadgetNames[GADGET_NAME.FistsOfFlame], 0xE),
                        new(_gadgetNames[GADGET_NAME.RagingInfernoFlop], 0x14),
                        new(_gadgetNames[GADGET_NAME.DiabloFireSlam], 0x11),
                        new(_gadgetNames[GADGET_NAME.TurnbuckleLaunch], 0xF),
                        new(_gadgetNames[GADGET_NAME.ButterflyNet], 0x2C),
                    }
                };
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCDemoJuly7])
            {
                _offsetInfiniteDbJump = "348";
                _offsetSpeedMultiplier = "358";
                _offsetUndetectable = "1288";
                _offsetGadgetBinds = "1270";
                DAG.Sly3Time = "37A4E0";
                DAG.Sly3Flag = "46E918";
                DAG.OffsetMissionName = "58";
                DAG.OffsetAttributesForCluster = "C8";
                DAG.OffsetVerticalLayer = "110";

                ReloadAddress = "46E97C";
                ReloadValuesAddress = "2D83A0";
                FKXListCountAddress = "46EC60";
                ClockAddress = "37A460";
                CoinsAddress = "45DF34";
                GadgetAddress = "45DF28";
                CameraPointer = "46E42C";
                DrawDistanceAddress = $"{CameraPointer},114";
                FOVAddress = $"{CameraPointer},11C";
                ResetCameraAddress = $"{CameraPointer},324";
                CanCameraNoclipAddress = "330700";
                MapIdAddress = "46EA54";
                GuardAIAddress = "37CB5C";
                ActiveCharacterPointer = "37CB1C";
                ActiveCharacterIdAddress = "45DE50";
                StringTableCountAddress = "46F484";
                IsLoadingAddress = "45CC00";
                EntranceRootNodePointer = "46D78C";
                DAG.RootNodePointer = "46DD74";
                DAG.CurrentCheckpointNodePointer = "46E5B8";
                DAG.ClusterIdAddress = "37AE90";
                Savefile.SavefileStartAddress = "45DE40";
                Savefile.SavefileAddressTablePointer = "46E4BC";
                Savefile.SavefileStringTablePointer = "46E598";
                ControllerAddress = "37BA18";
                DialoguePointer = "46A310";
                SkipFMVPointer = "37EE98";

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

                Gadgets = new()
                {
                    new()
                    {
                        new(_gadgetNames[GADGET_NAME.SmokeBomb], 0x1A),
                        new(_gadgetNames[GADGET_NAME.CombatDodge], 0x1B),
                        new(_gadgetNames[GADGET_NAME.ThiefReflexes], 0x23),
                        new(_gadgetNames[GADGET_NAME.FeralPounce], 0x1E),
                        new(_gadgetNames[GADGET_NAME.MegaJump], 0x1F),
                        new(_gadgetNames[GADGET_NAME.ShadowPowerLevel1], 0x22),
                        new(_gadgetNames[GADGET_NAME.UnknownRocketBoots], 0x21),
                        new(_gadgetNames[GADGET_NAME.RocketBoots], 0x25),
                        new(_gadgetNames[GADGET_NAME.ShadowPowerLevel2], 0x24),
                        new(_gadgetNames[GADGET_NAME.Shield], 0x27),
                        new(_gadgetNames[GADGET_NAME.SpinAttackLevel1], 0x2B, false),
                        new(_gadgetNames[GADGET_NAME.SpinAttackLevel2], 0x2C, false),
                        new(_gadgetNames[GADGET_NAME.SpinAttackLevel3], 0x2D, false),
                        new(_gadgetNames[GADGET_NAME.JumpAttackLevel1], 0x2E, false),
                        new(_gadgetNames[GADGET_NAME.JumpAttackLevel2], 0x2F, false),
                        new(_gadgetNames[GADGET_NAME.JumpAttackLevel3], 0x30, false),
                        new(_gadgetNames[GADGET_NAME.PushAttackLevel1], 0x31, false),
                        new(_gadgetNames[GADGET_NAME.PushAttackLevel2], 0x32, false),
                        new(_gadgetNames[GADGET_NAME.PushAttackLevel3], 0x33, false),
                    },

                    new()
                    {
                        new(_gadgetNames[GADGET_NAME.TriggerBomb], 0x6),
                        new(_gadgetNames[GADGET_NAME.FishingPole], 0x7),
                        new(_gadgetNames[GADGET_NAME.AlarmClock], 0x8),
                        new(_gadgetNames[GADGET_NAME.AdrenalineBurst], 0x9),
                        new(_gadgetNames[GADGET_NAME.HealthExtractor], 0xA),
                        new(_gadgetNames[GADGET_NAME.InsanityStrike], 0xC),
                        new(_gadgetNames[GADGET_NAME.GrappleCam], 0xD),
                        new(_gadgetNames[GADGET_NAME.SizeDestabilizer], 0xE),
                        new(_gadgetNames[GADGET_NAME.RageBomb], 0xF),
                        new(_gadgetNames[GADGET_NAME.ReductionBomb], 0x10),
                    },

                    new()
                    {
                        new(_gadgetNames[GADGET_NAME.BeTheBall], 0x11),
                        new(_gadgetNames[GADGET_NAME.BerserkerCharge], 0x12),
                        new(_gadgetNames[GADGET_NAME.GutturalRoar], 0x14),
                        new(_gadgetNames[GADGET_NAME.FistsOfFlame], 0x15),
                        new(_gadgetNames[GADGET_NAME.TemporalLock], 0x16),
                        new(_gadgetNames[GADGET_NAME.RagingInfernoFlop], 0x17),
                        new(_gadgetNames[GADGET_NAME.DiabloFireSlam], 0x18),
                        new(_gadgetNames[GADGET_NAME.CutscenePuppet], 0x19),
                    }
                };
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.PALDemo])
            {
                // SCED-53802 - BAE3B5E9
                ReloadAddress = "485744";
                ReloadValuesAddress = "2E9DD8";
                FKXListCountAddress = "485A2C";
                ClockAddress = "38F660";
                CoinsAddress = "474D7C";
                GadgetAddress = "474D6C";
                CameraPointer = "4852BC";
                DrawDistanceAddress = $"{CameraPointer},114";
                FOVAddress = $"{CameraPointer},11C";
                ResetCameraAddress = $"{CameraPointer},2F4";
                CanCameraNoclipAddress = "345170";
                MapIdAddress = "474CD8";
                GuardAIAddress = "39338C";
                ActiveCharacterPointer = "39330C";
                ActiveCharacterIdAddress = "3901D0";
                StringTableCountAddress = "486254";
                IsLoadingAddress = "473A80";
                EntranceRootNodePointer = "484600";
                DAG.RootNodePointer = "484C0C";
                DAG.CurrentCheckpointNodePointer = "48544C";
                DAG.ClusterIdAddress = "391658";
                DAG.Sly3Time = "38F6E0";
                DAG.Sly3Flag = "4856D4";
                Savefile.SavefileStartAddress = "474CD0";
                Savefile.SavefileAddressTablePointer = "48534C";
                Savefile.SavefileStringTablePointer = "485428";
                ControllerAddress = "392218";
                DialoguePointer = "4811A0";
                SkipFMVPointer = "395708";

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
            else if (build.Region == Util.BuildRegions[BUILD_NAME.PALDemoSeptember2])
            {
                ReloadAddress = "4856C4";
                ReloadValuesAddress = "2E9D58";
                FKXListCountAddress = "4859AC";
                ClockAddress = "38F5E0";
                CoinsAddress = "474CFC";
                GadgetAddress = "474CEC";
                CameraPointer = "48523C";
                DrawDistanceAddress = $"{CameraPointer},114";
                FOVAddress = $"{CameraPointer},11C";
                ResetCameraAddress = $"{CameraPointer},2F4";
                CanCameraNoclipAddress = "3450F0";
                MapIdAddress = "474C58";
                GuardAIAddress = "39330C";
                ActiveCharacterPointer = "39328C";
                ActiveCharacterIdAddress = "390150";
                StringTableCountAddress = "4861D4";
                IsLoadingAddress = "473A00";
                EntranceRootNodePointer = "484580";
                DAG.RootNodePointer = "484B8C";
                DAG.CurrentCheckpointNodePointer = "4853CC";
                DAG.ClusterIdAddress = "3915D8";
                DAG.Sly3Time = "38F660";
                DAG.Sly3Flag = "485654";
                Savefile.SavefileStartAddress = "474C50";
                Savefile.SavefileAddressTablePointer = "4852CC";
                Savefile.SavefileStringTablePointer = "4853A8";
                ControllerAddress = "392198";
                DialoguePointer = "481120";
                SkipFMVPointer = "395688";

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
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCJuly16])
            {
                // SCUS-97464 - 0190CF8B
                _offsetInfiniteDbJump = "348";
                _offsetSpeedMultiplier = "358";
                _offsetUndetectable = "12A0";
                _offsetGadgetBinds = "1270";
                DAG.OffsetVerticalLayer = "110";

                ReloadAddress = "46BB24";
                ReloadValuesAddress = "2DEB08";
                FKXListCountAddress = "46BE0C";
                ClockAddress = "35F6A0";
                CoinsAddress = "45B0A8";
                GadgetAddress = "45B09C";
                CameraPointer = "46B5AC";
                DrawDistanceAddress = $"{CameraPointer},114";
                FOVAddress = $"{CameraPointer},11C";
                ResetCameraAddress = $"{CameraPointer},324";
                CanCameraNoclipAddress = "33E510";
                MapIdAddress = "46BBFC";
                GuardAIAddress = "362F1C";
                ActiveCharacterPointer = "361D5C";
                ActiveCharacterIdAddress = "45AFC4";
                StringTableCountAddress = "46C624";
                IsLoadingAddress = "459D80";
                EntranceRootNodePointer = "46A90C";
                DAG.RootNodePointer = "46AEF4";
                DAG.CurrentCheckpointNodePointer = "46B738";
                DAG.ClusterIdAddress = "3600D0";
                DAG.Sly3Time = "35F720";
                DAG.Sly3Flag = "46BAC8";
                Savefile.SavefileStartAddress = "45AFB0";
                Savefile.SavefileAddressTablePointer = "46B63C";
                Savefile.SavefileStringTablePointer = "46B718";
                ControllerAddress = "360C58";
                DialoguePointer = "467480";
                SkipFMVPointer = "37C068";

                for (int i = 23; i < 35; i++)
                {
                    Maps[i].IsVisible = false;
                }

                Maps.RemoveRange(36, 4);

                Gadgets = new()
                {
                    new()
                    {
                        new(_gadgetNames[GADGET_NAME.SmokeBomb], 0x1A),
                        new(_gadgetNames[GADGET_NAME.CombatDodge], 0x1B),
                        new(_gadgetNames[GADGET_NAME.FeralPounce], 0x1E),
                        new(_gadgetNames[GADGET_NAME.MegaJump], 0x1F),
                        new(_gadgetNames[GADGET_NAME.KnockoutDive], 0x20),
                        new(_gadgetNames[GADGET_NAME.UnknownRocketBoots], 0x21),
                        new(_gadgetNames[GADGET_NAME.ShadowPowerLevel1], 0x22),
                        new(_gadgetNames[GADGET_NAME.ThiefReflexes], 0x23),
                        new(_gadgetNames[GADGET_NAME.ShadowPowerLevel2], 0x24),
                        new(_gadgetNames[GADGET_NAME.RocketBoots], 0x25),
                        new(_gadgetNames[GADGET_NAME.TreasureMap], 0x26),
                        new(_gadgetNames[GADGET_NAME.Shield], 0x27),
                        new(_gadgetNames[GADGET_NAME.VeniceDisguise], 0x28),
                        new(_gadgetNames[GADGET_NAME.PhotographerDisguise], 0x29),
                        new(_gadgetNames[GADGET_NAME.PirateDisguise], 0x2A),
                    },

                    new()
                    {
                        new(_gadgetNames[GADGET_NAME.TriggerBomb], 0x6),
                        new(_gadgetNames[GADGET_NAME.FishingPole], 0x7),
                        new(_gadgetNames[GADGET_NAME.AlarmClock], 0x8),
                        new(_gadgetNames[GADGET_NAME.AdrenalineBurst], 0x9),
                        new(_gadgetNames[GADGET_NAME.HealthExtractor], 0xA),
                        new(_gadgetNames[GADGET_NAME.InsanityStrike], 0xC),
                        new(_gadgetNames[GADGET_NAME.GrappleCam], 0xD),
                        new(_gadgetNames[GADGET_NAME.SizeDestabilizer], 0xE),
                        new(_gadgetNames[GADGET_NAME.RageBomb], 0xF),
                        new(_gadgetNames[GADGET_NAME.ReductionBomb], 0x10),
                    },

                    new()
                    {
                        new(_gadgetNames[GADGET_NAME.BeTheBall], 0x11),
                        new(_gadgetNames[GADGET_NAME.BerserkerCharge], 0x12),
                        new(_gadgetNames[GADGET_NAME.GutturalRoar], 0x14),
                        new(_gadgetNames[GADGET_NAME.FistsOfFlame], 0x15),
                        new(_gadgetNames[GADGET_NAME.TemporalLock], 0x16),
                        new(_gadgetNames[GADGET_NAME.RagingInfernoFlop], 0x17),
                        new(_gadgetNames[GADGET_NAME.DiabloFireSlam], 0x18),
                        new(_gadgetNames[GADGET_NAME.CutscenePuppet], 0x19),
                    }
                };
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCAugust24])
            {
                // SCUS-97464 - 779B6999
                ReloadAddress = "4806C4";
                ReloadValuesAddress = "2EE858";
                FKXListCountAddress = "4809AC";
                ClockAddress = "372AA0";
                CoinsAddress = "46FCDC";
                GadgetAddress = "46FCCC";
                CameraPointer = "48023C";
                DrawDistanceAddress = $"{CameraPointer},114";
                FOVAddress = $"{CameraPointer},11C";
                ResetCameraAddress = $"{CameraPointer},2F4";
                CanCameraNoclipAddress = "350EA0";
                MapIdAddress = "48079C";
                GuardAIAddress = "37798C";
                ActiveCharacterPointer = "37674C";
                ActiveCharacterIdAddress = "373610";
                StringTableCountAddress = "4811D4";
                IsLoadingAddress = "46EA00";
                EntranceRootNodePointer = "47F580";
                DAG.RootNodePointer = "47FB8C";
                DAG.CurrentCheckpointNodePointer = "4803CC";
                DAG.ClusterIdAddress = "374A98";
                DAG.Sly3Time = "372B20";
                DAG.Sly3Flag = "480654";
                Savefile.SavefileStartAddress = "46FC30";
                Savefile.SavefileAddressTablePointer = "4802CC";
                Savefile.SavefileStringTablePointer = "4803A8";
                ControllerAddress = "375658";
                DialoguePointer = "47C100";
                SkipFMVPointer = "390B18";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.PALAugust2])
            {
                // SCES-52529 - 8C146034
                _offsetHealth = "170";
                _offsetGadgetPower = "178";
                _offsetRadTarget = "1CC";
                _offsetDeltaTranslation = "260";
                _offsetInfiniteDbJump = "34C";
                _offsetSpeedMultiplier = "368";
                _offsetUndetectable = "12B0";
                _offsetGadgetBinds = "1280";
                DAG.OffsetVerticalLayer = "110";

                ReloadAddress = "4AF7CC";
                ReloadValuesAddress = "2F8068";
                FKXListCountAddress = "4AFAB4";
                ClockAddress = "38B1A0";
                CoinsAddress = "49E750";
                GadgetAddress = "49E740";
                CameraPointer = "4AF33C";
                DrawDistanceAddress = $"{CameraPointer},114";
                FOVAddress = $"{CameraPointer},11C";
                ResetCameraAddress = $"{CameraPointer},324";
                CanCameraNoclipAddress = "385DC0";
                MapIdAddress = "4AF8A4";
                GuardAIAddress = "38EEA4";
                ActiveCharacterPointer = "38EE5C";
                ActiveCharacterIdAddress = "38BD40";
                StringTableCountAddress = "4B02C4";
                IsLoadingAddress = "49D480";
                EntranceRootNodePointer = "4AE680";
                DAG.RootNodePointer = "4AEC8C";
                DAG.CurrentCheckpointNodePointer = "4AF4C8";
                DAG.ClusterIdAddress = "38D1B8";
                DAG.Sly3Time = "38B220";
                DAG.Sly3Flag = "4AF770";
                Savefile.SavefileStartAddress = "49E6B0";
                Savefile.SavefileAddressTablePointer = "4AF3CC";
                Savefile.SavefileStringTablePointer = "4AF4A8";
                ControllerAddress = "38DD58";
                DialoguePointer = "4AB200";
                SkipFMVPointer = "3BF248";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.PALSeptember2])
            {
                // SCES-53409 - 3670B6F9
                _offsetHealth = "170";
                _offsetGadgetPower = "178";
                _offsetRadTarget = "1CC";
                _offsetDeltaTranslation = "260";
                _offsetInfiniteDbJump = "34C";
                _offsetSpeedMultiplier = "364";
                _offsetUndetectable = "1290";
                _offsetGadgetBinds = "1260";

                ReloadAddress = "4BE9C4";
                ReloadValuesAddress = "304248";
                FKXListCountAddress = "4BECAC";
                ClockAddress = "39A1A0";
                CoinsAddress = "4AD95C";
                GadgetAddress = "4AD94C";
                CameraPointer = "4BE53C";
                DrawDistanceAddress = $"{CameraPointer},114";
                FOVAddress = $"{CameraPointer},11C";
                ResetCameraAddress = $"{CameraPointer},2F4";
                CanCameraNoclipAddress = "394D20";
                MapIdAddress = "4BEA9C";
                GuardAIAddress = "39DF54";
                ActiveCharacterPointer = "39DECC";
                ActiveCharacterIdAddress = "39AD40";
                StringTableCountAddress = "4BF4D4";
                IsLoadingAddress = "4AC680";
                EntranceRootNodePointer = "4BD880";
                DAG.RootNodePointer = "4BDE8C";
                DAG.CurrentCheckpointNodePointer = "4BE6CC";
                DAG.ClusterIdAddress = "39C1C8";
                DAG.Sly3Time = "39A220";
                DAG.Sly3Flag = "4BE954";
                Savefile.SavefileStartAddress = "4AD8B0";
                Savefile.SavefileAddressTablePointer = "4BE5CC";
                Savefile.SavefileStringTablePointer = "4BE6A8";
                ControllerAddress = "39CDD8";
                DialoguePointer = "4BA410";
                SkipFMVPointer = "3CE288";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCPS3PSN]
                  || build.Region == Util.BuildRegions[BUILD_NAME.PALPS3PSN]
                  || build.Region == Util.BuildRegions[BUILD_NAME.NTSCKPS3PSN])
            {
                _encoding = Encoding.BigEndianUnicode;
                _offsetHealth = "168";
                _offsetGadgetPower = "170";
                _offsetRadTarget = "1AC";
                _offsetDeltaTranslation = "240";
                _offsetInfiniteDbJump = "32C";
                _offsetSpeedMultiplier = "344";
                _offsetUndetectable = "1270";
                _offsetGadgetBinds = "1240";
                _offsetDialogueFlags = "5D";

                ReloadAddress = "78D2C0";
                ReloadValuesAddress = "508630";
                if (build.Region == Util.BuildRegions[BUILD_NAME.PALPS3PSN])
                {
                    ReloadValuesAddress = "508650";
                }
                else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCKPS3PSN])
                {
                    ReloadValuesAddress = "508610";
                }

                FKXListCountAddress = "78D5A8";
                ClockAddress = "5898B8";
                CoinsAddress = "6CC808";
                GadgetAddress = "6CC7F8";
                CameraPointer = "78CE2C";
                DrawDistanceAddress = $"{CameraPointer},114";
                FOVAddress = $"{CameraPointer},11C";
                ResetCameraAddress = $"{CameraPointer},304";
                CanCameraNoclipAddress = "7D64D4";
                MapIdAddress = "78D398";
                GuardAIAddress = "5EC6CC";
                ActiveCharacterPointer = "5EC64C";
                ActiveCharacterIdAddress = "5EA000";
                StringTableCountAddress = "78DDD4";
                IsLoadingAddress = "6CB600";
                EntranceRootNodePointer = "78C170";
                DAG.RootNodePointer = "78C77C";
                DAG.CurrentCheckpointNodePointer = "78CFBC";
                DAG.ClusterIdAddress = "5EB488";
                DAG.Sly3Time = "589930";
                DAG.Sly3Flag = "78D250";
                Savefile.SavefileStartAddress = "6CC750";
                Savefile.SavefileAddressTablePointer = "78CEBC";
                Savefile.SavefileStringTablePointer = "78CF98";
                ControllerAddress = "5EC5AA";
                DialoguePointer = "788CB8";
                SkipFMVPointer = "83C8BC";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCPS3]
                  || build.Region == Util.BuildRegions[BUILD_NAME.PALPS3]
                  || build.Region == Util.BuildRegions[BUILD_NAME.UKPS3])
            {
                _encoding = Encoding.BigEndianUnicode;
                _offsetHealth = "168";
                _offsetGadgetPower = "170";
                _offsetRadTarget = "1AC";
                _offsetDeltaTranslation = "240";
                _offsetInfiniteDbJump = "32C";
                _offsetSpeedMultiplier = "344";
                _offsetUndetectable = "1270";
                _offsetGadgetBinds = "1240";
                _offsetDialogueFlags = "5D";

                ReloadAddress = "70FD3C";
                ReloadValuesAddress = "4930D0";
                FKXListCountAddress = "710024";
                ClockAddress = "514338";
                CoinsAddress = "657284";
                GadgetAddress = "657274";
                CameraPointer = "70F8AC";
                DrawDistanceAddress = $"{CameraPointer},114";
                FOVAddress = $"{CameraPointer},11C";
                ResetCameraAddress = $"{CameraPointer},304";
                CanCameraNoclipAddress = "758F68";
                MapIdAddress = "70FE14";
                GuardAIAddress = "57714C";
                ActiveCharacterPointer = "5770CC";
                ActiveCharacterIdAddress = "574A80";
                StringTableCountAddress = "710854";
                IsLoadingAddress = "656080";
                EntranceRootNodePointer = "70EBF0";
                DAG.RootNodePointer = "70F1FC";
                DAG.CurrentCheckpointNodePointer = "70FA3C";
                DAG.ClusterIdAddress = "575F08";
                DAG.Sly3Time = "5143B0";
                DAG.Sly3Flag = "70FCCC";
                Savefile.SavefileStartAddress = "6571D0";
                Savefile.SavefileAddressTablePointer = "70F93C";
                Savefile.SavefileStringTablePointer = "70FA18";
                ControllerAddress = "57702A";
                DialoguePointer = "70B738";
                SkipFMVPointer = "78451C";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCJPS3]
                  || build.Region == Util.BuildRegions[BUILD_NAME.NTSCKPS3])
            {
                _encoding = Encoding.BigEndianUnicode;
                _offsetHealth = "168";
                _offsetGadgetPower = "170";
                _offsetRadTarget = "1AC";
                _offsetDeltaTranslation = "240";
                _offsetInfiniteDbJump = "32C";
                _offsetSpeedMultiplier = "344";
                _offsetUndetectable = "1270";
                _offsetGadgetBinds = "1240";
                _offsetDialogueFlags = "5D";

                ReloadAddress = "717DBC";
                ReloadValuesAddress = "493110";
                if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCKPS3])
                {
                    ReloadValuesAddress = "4930F0";
                }

                FKXListCountAddress = "7180A4";
                ClockAddress = "5143B8";
                CoinsAddress = "657304";
                GadgetAddress = "6572F4";
                CameraPointer = "71792C";
                DrawDistanceAddress = $"{CameraPointer},114";
                FOVAddress = $"{CameraPointer},11C";
                ResetCameraAddress = $"{CameraPointer},304";
                CanCameraNoclipAddress = "760FE8";
                MapIdAddress = "717E94";
                GuardAIAddress = "5771CC";
                ActiveCharacterPointer = "57714C";
                ActiveCharacterIdAddress = "574B00";
                StringTableCountAddress = "7188D4";
                IsLoadingAddress = "656100";
                EntranceRootNodePointer = "716C70";
                DAG.RootNodePointer = "71727C";
                DAG.CurrentCheckpointNodePointer = "717ABC";
                DAG.ClusterIdAddress = "575F88";
                DAG.Sly3Time = "514430";
                DAG.Sly3Flag = "717D4C";
                Savefile.SavefileStartAddress = "657250";
                Savefile.SavefileAddressTablePointer = "7179BC";
                Savefile.SavefileStringTablePointer = "717A98";
                ControllerAddress = "5770AA";
                DialoguePointer = "7137B8";
                SkipFMVPointer = "78C59C";
            }

            _offsetTransformationLocal = $"{_offsetTransformationOrigin}+4";
            _offsetTransformationWorld = $"{_offsetTransformationOrigin}+8";
            _offsetTransformationFinal = $"{_offsetTransformationOrigin}+C";
        }

        public override void CustomTick()
        {

        }

        public override void OnFirstLoopAfterLoading(int mapId)
        {
            Savefile.Init();

            // In sly 3, not all characters are available in all maps (e.g. ep1 police station only has sly)
            // So, we need to filter the character list based on the entities list
            var list = GetFKXList();
            List<Character_t> newCharacters = new(Characters);
            for (int i = 0; i < newCharacters.Count; i++)
            {
                var character = newCharacters[i];
                var fkEntity = list.FirstOrDefault(x => x.Name == character.InternalName);
                if (fkEntity == null || fkEntity.SpawnRule == 0)
                {
                    // Remove if not found or its spawn rule is 0 (shaman in kaine island)
                    newCharacters.Remove(character);
                    i--;
                    continue;
                }

                // Sly 3 NTSC Demo April 18 doesn't have the same ids as retail
                // So let's read them on the fly
                int id = _m.ReadInt((fkEntity.EntityAddress[0] + 0x18).ToString("X"));
                newCharacters[i].Id = id;
            }

            if (newCharacters.Count != 0)
            {
                if (!newCharacters.SequenceEqual((List<Character_t>)_form.cmbActChar.DataSource))
                {
                    _form.UpdateUI(() =>
                    {
                        var last = _form.cmbActChar.SelectedItem;
                        _form.cmbActChar.DataSource = newCharacters;
                        if (newCharacters.Contains(last))
                        {
                            // If the new map contains the latest character, select it
                            _form.cmbActChar.SelectedItem = last;
                        }
                    });
                }
            }
        }

        public override bool IsLoading()
        {
            if (_m.ReadInt(IsLoadingAddress) == 3
             && _m.ReadInt(Savefile.SavefileAddressTablePointer) != 0)
            {
                return false;
            }

            return true;
        }

        #region Gadgets
        public override void ToggleAllGadgets()
        {
            long gadgets = ReadGadgets();
            if (gadgets == -1)
            {
                // Some of the "gadgets" are actually essential skillset
                // For example sly's square attack, binocucom, or bentley mines
                // The following value is the value set by the game when loading a new game
                // from NTSC at 349938 (array of 9 ints, each int is the bit index)
                gadgets = 0x00000200000200FE;
                if (Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCJuly16]
                 || Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCDemoJuly7])
                {
                    gadgets = 0x00000800000200FE;
                }
                else if (Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCDemoApril18])
                {
                    gadgets = 0;
                }
            }
            else
            {
                gadgets = -1;
            }

            WriteGadgets(gadgets);
        }

        public int ReadActCharGadgetPower()
        {
            return _m.ReadInt($"{ActiveCharacterPointer},{_offsetGadgetPower}");
        }

        public void WriteActCharGadgetPower(int value)
        {
            _m.WriteMemory($"{ActiveCharacterPointer},{_offsetGadgetPower}", "int", value.ToString());
        }

        public override void FreezeActCharGadgetPower(int value)
        {
            if (value == 0)
            {
                value = ReadActCharGadgetPower();
            }

            _m.FreezeValue($"{ActiveCharacterPointer},{_offsetGadgetPower}", "int", value.ToString());
        }

        public override void UnfreezeActCharGadgetPower()
        {
            _m.UnfreezeValue($"{ActiveCharacterPointer},{_offsetGadgetPower}");
        }

        public override int ReadActCharGadgetId(GADGET_BIND bind)
        {
            return _m.ReadInt($"{ActiveCharacterPointer},{_offsetGadgetBinds}+{(int)bind * 0xC:X}");
        }

        public override void WriteActCharGadgetId(GADGET_BIND bind, int value)
        {
            // Write to character's struct so that the change is immediate
            _m.WriteMemory($"{ActiveCharacterPointer},{_offsetGadgetBinds}+{(int)bind * 0xC:X}", "int", value.ToString());

            // When we use an invalid gadget, the game writes -1 to +4, which makes all the other gadgets unusable
            // We write 0 to +4 to prevent this
            _m.WriteMemory($"{ActiveCharacterPointer},{_offsetGadgetBinds}+{(int)bind * 0xC + 4:X}", "int", "0");

            // Write to savefile too so that it persists after reload
            var address = Savefile.GetSavefileAddress(ActiveCharacter.NameForSavefile, "apukCur");
            _m.WriteMemory($"{address:X}+{(int)bind * 0x4:X}", "int", value.ToString());
        }
        #endregion

        #region Entities
        public override bool EntityHasTransformation(string pointerToEntity)
        {
            if (_m.ReadInt($"{pointerToEntity},{_offsetTransformationOrigin}") == -1
             || _m.ReadInt($"{pointerToEntity},{_offsetTransformationOrigin}") == 0)
            {
                return false;
            }

            return true;
        }

        public override Vector3 ReadEntityDeltaTranslation(string pointerToEntity)
        {
            Vector3 delta = _m.ReadVector3($"{pointerToEntity},{_offsetDeltaTranslation}");
            delta = delta * -1;
            return delta;
        }

        #region Origin
        public override Matrix4x4 ReadEntityOriginTransformation(string pointerToEntity)
        {
            if (!EntityHasTransformation(pointerToEntity))
            {
                return Matrix4x4.Identity;
            }

            return _m.ReadMatrix4($"{pointerToEntity},{_offsetTransformationOrigin},0");
        }

        public override Matrix4x4 ReadEntityOriginCombinedTransformation(string pointerToEntity)
        {
            if (!EntityHasTransformation(pointerToEntity))
            {
                return Matrix4x4.Identity;
            }

            return _m.ReadMatrix4($"{pointerToEntity},{_offsetTransformationOrigin},40");
        }

        public override void WriteEntityOriginTransformation(string pointerToEntity, Matrix4x4 transformation)
        {
            if (!EntityHasTransformation(pointerToEntity))
            {
                return;
            }

            _m.WriteMemory($"{pointerToEntity},{_offsetTransformationOrigin},0", "mat4", transformation.ToString());
        }
        #endregion

        #region Local
        public override Matrix4x4 ReadEntityLocalTransformation(string pointerToEntity)
        {
            if (!EntityHasTransformation(pointerToEntity))
            {
                return Matrix4x4.Identity;
            }

            return _m.ReadMatrix4($"{pointerToEntity},{_offsetTransformationLocal},0");
        }

        public override Matrix4x4 ReadEntityLocalCombinedTransformation(string pointerToEntity)
        {
            if (!EntityHasTransformation(pointerToEntity))
            {
                return Matrix4x4.Identity;
            }

            return _m.ReadMatrix4($"{pointerToEntity},{_offsetTransformationLocal},40");
        }

        public override Vector3 ReadEntityLocalTranslation(string pointerToEntity)
        {
            return ReadEntityLocalTransformation(pointerToEntity).Translation;
        }

        public override void WriteEntityLocalTransformation(string pointerToEntity, Matrix4x4 transformation)
        {
            if (!EntityHasTransformation(pointerToEntity))
            {
                return;
            }

            _m.WriteMemory($"{pointerToEntity},{_offsetTransformationLocal},0", "mat4", transformation.ToString());
        }

        public override void WriteEntityLocalTranslation(string pointerToEntity, Vector3 value)
        {
            if (!EntityHasTransformation(pointerToEntity))
            {
                return;
            }

            _m.WriteMemory($"{pointerToEntity},{_offsetTransformationLocal},30", "vec3", value.ToString());
        }

        public override void FreezeEntityLocalTranslationX(string pointerToEntity, string value)
        {
            if (value == "")
            {
                Vector3 trans = ReadEntityLocalTranslation(pointerToEntity);
                value = trans.X.ToString();
            }

            _m.FreezeValue($"{pointerToEntity},{_offsetTransformationLocal},30", "float", value);
        }

        public override void FreezeEntityLocalTranslationY(string pointerToEntity, string value)
        {
            if (value == "")
            {
                Vector3 trans = ReadEntityLocalTranslation(pointerToEntity);
                value = trans.Y.ToString();
            }
            _m.FreezeValue($"{pointerToEntity},{_offsetTransformationLocal},34", "float", value);
        }

        public override void FreezeEntityLocalTranslationZ(string pointerToEntity, string value)
        {
            if (value == "")
            {
                Vector3 trans = ReadEntityLocalTranslation(pointerToEntity);
                value = trans.Z.ToString();
            }
            _m.FreezeValue($"{pointerToEntity},{_offsetTransformationLocal},38", "float", value);
        }

        public override void UnfreezeEntityLocalTranslationX(string pointerToEntity)
        {
            _m.UnfreezeValue($"{pointerToEntity},{_offsetTransformationLocal},30");
        }

        public override void UnfreezeEntityLocalTranslationY(string pointerToEntity)
        {
            _m.UnfreezeValue($"{pointerToEntity},{_offsetTransformationLocal},34");
        }

        public override void UnfreezeEntityLocalTranslationZ(string pointerToEntity)
        {
            _m.UnfreezeValue($"{pointerToEntity},{_offsetTransformationLocal},38");
        }

        public override float ReadEntityLocalScale(string pointerToEntity)
        {
            if (!EntityHasTransformation(pointerToEntity))
            {
                return 1f;
            }

            return _m.ReadFloat($"{pointerToEntity},{_offsetTransformationLocal},0");
        }

        public override void WriteEntityLocalScale(string pointerToEntity, float scale)
        {
            if (!EntityHasTransformation(pointerToEntity))
            {
                return;
            }

            _m.WriteMemory($"{pointerToEntity},{_offsetTransformationLocal},0", "float", scale.ToString());
            _m.WriteMemory($"{pointerToEntity},{_offsetTransformationLocal},14", "float", scale.ToString());
            _m.WriteMemory($"{pointerToEntity},{_offsetTransformationLocal},28", "float", scale.ToString());
        }

        public override Vector3 ReadEntityLocalVelocity(string pointerToEntity)
        {
            return _m.ReadVector3($"{pointerToEntity},{_offsetTransformationLocal},B0");
        }

        public override void WriteEntityLocalVelocity(string pointerToEntity, Vector3 value)
        {
            _m.WriteMemory($"{pointerToEntity},{_offsetTransformationLocal},B0", "vec3", value.ToString());
        }
        #endregion

        #region World
        public override Matrix4x4 ReadEntityWorldTransformation(string pointerToEntity)
        {
            if (!EntityHasTransformation(pointerToEntity))
            {
                return Matrix4x4.Identity;
            }

            return _m.ReadMatrix4($"{pointerToEntity},{_offsetTransformationWorld},0");
        }

        public override Matrix4x4 ReadEntityWorldCombinedTransformation(string pointerToEntity)
        {
            if (!EntityHasTransformation(pointerToEntity))
            {
                return Matrix4x4.Identity;
            }

            return _m.ReadMatrix4($"{pointerToEntity},{_offsetTransformationWorld},40");
        }

        public override void WriteEntityWorldTransformation(string pointerToEntity, Matrix4x4 trans)
        {
            if (!EntityHasTransformation(pointerToEntity))
            {
                return;
            }

            float radTarget = Convert.ToSingle(Math.Atan2(trans.M12, trans.M11));
            _m.WriteMemory($"{pointerToEntity},{_offsetTransformationWorld},0", "mat3", trans.ToString());
            _m.WriteMemory($"{pointerToEntity},{_offsetRadTarget}", "float", radTarget.ToString());
        }

        #region Final
        public override Vector3 ReadEntityFinalCombinedTranslation(string pointerToEntity)
        {
            if (!EntityHasTransformation(pointerToEntity))
            {
                return Vector3.Zero;
            }

            return _m.ReadVector3($"{pointerToEntity},{_offsetTransformationWorld},70");
        }

        public override Matrix4x4 ReadEntityFinalTransformation(string pointerToEntity)
        {
            if (!EntityHasTransformation(pointerToEntity))
            {
                return Matrix4x4.Identity;
            }

            return _m.ReadMatrix4($"{pointerToEntity},{_offsetTransformationFinal},0");
        }

        public override Matrix4x4 ReadEntityFinalCombinedTransformation(string pointerToEntity)
        {
            if (!EntityHasTransformation(pointerToEntity))
            {
                return Matrix4x4.Identity;
            }

            return _m.ReadMatrix4($"{pointerToEntity},{_offsetTransformationFinal},40");
        }

        public override void WriteEntityFinalTransformation(string pointerToEntity, Matrix4x4 value)
        {
            if (!EntityHasTransformation(pointerToEntity))
            {
                return;
            }

            _m.WriteMemory($"{pointerToEntity},{_offsetTransformationFinal},0", "mat4", value.ToString());
        }
        #endregion

        #endregion

        public List<FKXEntry_t> GetFKXList()
        {
            int fkxCount = _m.ReadInt(FKXListCountAddress);
            string fkxPointer = _m.ReadInt($"{FKXListCountAddress}+4").ToString("X");
            List<FKXEntry_t> fkxList = new(fkxCount);
            for (int i = 0; i < fkxCount; i++)
            {
                string address = (Convert.ToInt32(fkxPointer, 16) + i * 0x6C).ToString("X");
                var data = _m.ReadBytes(address, 0x5C);
                FKXEntry_t fkx = new(address, data);
                for (int j = 0; j < fkx.Count; j++)
                {
                    fkx.EntityAddress.Add(_m.ReadInt($"{fkx.PoolPointer:X}+{j * 4:X}"));
                }

                fkxList.Add(fkx);
            }

            fkxList = fkxList.OrderBy(x => x.Name).ToList();
            return fkxList;
        }

        #endregion

        #region Active character
        public override bool IsActCharAvailable()
        {
            return _m.ReadInt(ActiveCharacterPointer) != 0;
        }

        public override string GetActCharPointer()
        {
            return ActiveCharacterPointer;
        }

        public override int ReadActCharId()
        {
            return _m.ReadInt(ActiveCharacterIdAddress);
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

        public override void FreezeActCharId(string value)
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

        public override int ReadActCharHealth()
        {
            return _m.ReadInt($"{ActiveCharacterPointer},{_offsetHealth}");
        }

        public override void WriteActCharHealth(int value)
        {
            _m.WriteMemory($"{ActiveCharacterPointer},{_offsetHealth}", "int", value.ToString());
        }

        public override void FreezeActCharHealth(int value)
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

        public override Matrix4x4 ReadActCharOriginTransformation()
        {
            return ReadEntityOriginTransformation(ActiveCharacterPointer);
        }

        public override Vector3 ReadActCharLocalTranslation()
        {
            return ReadEntityLocalTranslation(ActiveCharacterPointer);
        }

        public override void WriteActCharLocalTranslation(Vector3 value)
        {
            WriteEntityLocalTranslation(ActiveCharacterPointer, value);
        }

        public override void FreezeActCharLocalTranslationX(string value)
        {
            FreezeEntityLocalTranslationX(ActiveCharacterPointer, value);
        }

        public override void FreezeActCharLocalTranslationY(string value)
        {
            FreezeEntityLocalTranslationY(ActiveCharacterPointer, value);
        }

        public override void FreezeActCharLocalTranslationZ(string value )
        {
            FreezeEntityLocalTranslationZ(ActiveCharacterPointer, value);
        }

        public override void UnfreezeActCharLocalTranslationX()
        {
            UnfreezeEntityLocalTranslationX(ActiveCharacterPointer);
        }

        public override void UnfreezeActCharLocalTranslationY()
        {
            UnfreezeEntityLocalTranslationY(ActiveCharacterPointer);
        }

        public override void UnfreezeActCharLocalTranslationZ()
        {
            UnfreezeEntityLocalTranslationZ(ActiveCharacterPointer);
        }

        public override Vector3 ReadActCharVelocity()
        {
            return ReadEntityLocalVelocity(ActiveCharacterPointer);
        }

        public override void WriteActCharVelocity(Vector3 value)
        {
            WriteEntityLocalVelocity(ActiveCharacterPointer, value);
        }

        public override void FreezeActCharVelocityZ(string value)
        {
            if (value == "")
            {
                Vector3 trans = ReadActCharVelocity();
                value = trans.Z.ToString();
            }

            _m.FreezeValue($"{ActiveCharacterPointer},{_offsetTransformationLocal},B8", "float", value);
        }

        public override void UnfreezeActCharVelocityZ()
        {
            _m.UnfreezeValue($"{ActiveCharacterPointer},{_offsetTransformationLocal},B8");
        }

        public override float ReadActCharSpeedMultiplier()
        {
            return _m.ReadFloat($"{ActiveCharacterPointer},{_offsetSpeedMultiplier}");
        }

        public override void WriteActCharSpeedMultiplier(float value)
        {
            _m.WriteMemory($"{ActiveCharacterPointer},{_offsetSpeedMultiplier}", "float", value.ToString());
        }

        public override void FreezeActCharSpeedMultiplier(float value)
        {
            if (value == 0)
            {
                value = ReadActCharSpeedMultiplier();
            }

            _m.FreezeValue($"{ActiveCharacterPointer},{_offsetSpeedMultiplier}", "float", value.ToString());
        }

        public override void UnfreezeActCharSpeedMultiplier()
        {
            _m.UnfreezeValue($"{ActiveCharacterPointer},{_offsetSpeedMultiplier}");
        }
        #endregion

        #region Toggles
        public override void ToggleUndetectable(bool enableUndetectable)
        {
            if (enableUndetectable)
            {
                _m.FreezeValue($"{ActiveCharacterPointer},{_offsetUndetectable}", "int", "1");
            }
            else
            {
                _m.WriteMemory($"{ActiveCharacterPointer},{_offsetUndetectable}", "int", "0");
                _m.UnfreezeValue($"{ActiveCharacterPointer},{_offsetUndetectable}");
            }
        }

        public override void ToggleInvulnerable(bool enableInvulnerable)
        {
            if (enableInvulnerable)
            {
                _lastInvulnerableValue = _m.ReadInt($"{ActiveCharacterPointer},{_offsetInvulnerable}");
                _m.FreezeValue($"{ActiveCharacterPointer},{_offsetInvulnerable}", "int", "0");
            }
            else
            {
                int tmp = _m.ReadInt($"{ActiveCharacterPointer},{_offsetInvulnerable}");
                if (tmp == 0)
                {
                    _m.WriteMemory($"{ActiveCharacterPointer},{_offsetInvulnerable}", "int", _lastInvulnerableValue.ToString());
                    _m.UnfreezeValue($"{ActiveCharacterPointer},{_offsetInvulnerable}");
                }
            }
        }

        public override void ToggleInfiniteDbJump(bool enableInfDbJump)
        {
            if (Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCJuly16]
             || Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCDemoApril18]
             || Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCDemoJuly7])
            {
                if (enableInfDbJump)
                {
                    _m.FreezeValue($"{ActiveCharacterPointer},{_offsetInfiniteDbJump}", "int", "1");
                }
                else
                {
                    _m.UnfreezeValue($"{ActiveCharacterPointer},{_offsetInfiniteDbJump}");
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

        public override void ActCharToggleNoclip(bool enableNoclip)
        {
            // Read the comment in Sly2Handler.cs on why this pointer chain is used for collision.
            if (enableNoclip)
            {
                string value = "9";
                if (Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCDemoApril18]
                 || Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCDemoJuly7]
                 || Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCJuly16]
                 || Build.Region == Util.BuildRegions[Util.BUILD_NAME.PALAugust2])
                {
                    // Prevents warp back
                    value = "5";
                }
                else if (Build.Region.Contains("PS3"))
                {
                    // The bits are flipped too
                    value = "0x90";
                }

                // Usually +34 and +38 have the same value, but for murray they are different and both writes are needed for him
                // Technically we should read the count at entity+D0, start from entity+D4, and for each entry write to +1A and increase the position by 0xD0
                _m.WriteMemory($"{ActiveCharacterPointer},{_offsetCollision},34,1A", "byte", $"{value}");
                _m.WriteMemory($"{ActiveCharacterPointer},{_offsetCollision},38,1A", "byte", $"{value}");
            }
            else
            {
                _m.WriteMemory($"{ActiveCharacterPointer},{_offsetCollision},34,1A", "byte", "0");
                _m.WriteMemory($"{ActiveCharacterPointer},{_offsetCollision},38,1A", "byte", "0");
            }
        }
        #endregion

        #region Maps
        public override void LoadMap(int mapId)
        {
            byte[] data = _m.ReadBytes($"{ReloadValuesAddress}+{mapId * 0x40:X8}", 0x40);
            _m.WriteMemory($"{ReloadAddress}+8", "bytes", Memory.EndianBitConverter.ByteArrayToString(data));
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
        #endregion

        public override void SkipCurrentDialogue()
        {
            // NOTE: This code is repeated in Sly 2 too
            int objVoiceover = _m.ReadInt(DialoguePointer);
            if (objVoiceover == 0)
            {
                // Skip FMV
                if (Build.Region.Contains("PS3"))
                {
                    _m.WriteMemory($"{SkipFMVPointer}", "int", "1");
                }
                else
                {
                    var IsFMVPlaying = _m.ReadInt($"{SkipFMVPointer},8");
                    if (IsFMVPlaying == 2)
                    {
                        _m.WriteMemory($"{SkipFMVPointer},0", "int", "1");
                    }
                }

                return;
            }

            // Don't do anything if the voice line was triggered from splice (e.g. sly 3 ep1 police hq when carmelita is talking)
            // NOTE: The sourceObject field might still be 0 even if the voice line was triggered from splice (e.g. sly 2 ep8 showdown with clock-la, while sly is in air)
            int sourceObject = _m.ReadInt($"{objVoiceover + 0x1C:X}");
            if (sourceObject != 0)
            {
                return;
            }

            // The following logic comes from 001E4120 in Sly 2 NTSC, which is a function responsible in the seq(uence) cutscenes
            // to check if the code should return to the main loop or execute the next seq instruction
            // The seq instruction 0x45 (play a voice line) will compare its string id argument with the current voice line id
            // If the current voice line id is -1, or is different than the seq instruction's argument, it will go to the next seq instruction
            int offsetStringId = 0x40;
            int stringIdAddress = _m.ReadInt($"{objVoiceover:X}+{_offsetCurrentDialogue}") + offsetStringId;
            int stringId = _m.ReadInt($"{stringIdAddress:X}");
            if (stringId == -1)
            {
                // Trying to skip an already skipped voice line
                // Or voice line is still playing but no dialogue is on the screen (the cutscene has ended, the player has control)
                //System.Diagnostics.Debug.WriteLine($"Trying to skip an already skipped voice line?");
                return;
            }

            //System.Diagnostics.Debug.WriteLine($"Skipping {stringId:X} at address {stringIdAddress:X} (start at {stringIdAddress - offsetStringId:X}) ({GetStringFromId(stringId)})");
            int write = -1;
            _m.WriteMemory($"{stringIdAddress:X}", "int", write.ToString());

            // While writing 8 (technically tmp = (tmp & ~0x1fff) | 0x8) to _offsetDialogueFlags
            // does skip the voice line, it's buggy and the game can get softlocked if we skip voice lines too quickly
            // We keep doing this write so that the character's lip animation gets skipped too
            _m.WriteMemory($"{objVoiceover:X}+{_offsetDialogueFlags}", "byte", "8");

            // Sly 3 has the ability to "soft" reload the map and make the same voice line play again
            // (especially because we can load a checkpoint with zero focus through the DAG)
            // so we need to restore the original string id
            // Restore the string id when the next voice line starts or when the cutscene ends
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(500);
                    //System.Diagnostics.Debug.WriteLine($"Trying to restore {stringId:X} at address {stringIdAddress:X} (start at {stringIdAddress - offsetStringId:X}) ({GetStringFromId(stringId)})");
                    int objVoiceoverNew = _m.ReadInt(DialoguePointer);
                    if (objVoiceoverNew != 0)
                    {
                        int stringIdAddressNew = _m.ReadInt($"{objVoiceoverNew:X}+{_offsetCurrentDialogue}") + offsetStringId;
                        if (stringIdAddress == stringIdAddressNew)
                        {
                            // Still in the same voice line (e.g. waiting for a camera transition)
                            continue;
                        }
                    }

                    // New voice line started or cutscene ended
                    // Only restore if the string id is still our value (it's not if we changed map or we loaded a savestate)
                    int stringIdNew = _m.ReadInt($"{stringIdAddress:X}");
                    if (stringIdNew == write)
                    {
                        //System.Diagnostics.Debug.WriteLine($"Restoring {stringId:X} at address {stringIdAddress:X} (start at {stringIdAddress - offsetStringId:X}) ({GetStringFromId(stringId)})");
                        _m.WriteMemory($"{stringIdAddress:X}", "int", stringId.ToString());
                    }

                    break;
                }
            });
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
                    return str;
                }
            }

            return "";
        }

        public List<(int id, string str)> GetStringTable(bool ordered)
        {
            string address = _m.ReadInt($"{StringTableCountAddress}+4").ToString("X");
            int count = _m.ReadInt($"{StringTableCountAddress}");
            List<(int id, string str)> table = new(count);
            for (int i = 0; i < count; i++)
            {
                int stringId = _m.ReadInt($"{address}+{i * 8:X}");
                int stringPointer = _m.ReadInt($"{address}+{i * 8 + 4:X}");
                string str = _m.ReadNullTerminatedString(stringPointer.ToString("X"), _encoding);
                str = str.Replace("\n", "\r\n");
                table.Add((stringId, str));
            }

            if (ordered)
            {
                table.Sort((a, b) => a.id.CompareTo(b.id));
            }

            return table;
        }

        protected override List<Character_t> GetCharacters()
        {
            return new()
            {
                new("Sly", 24, "jt", "sly"),
                new("Bentley", 25, "bentley", "bentley"),
                new("Murray", 26, "murray", "murray"),
                new("Guru", 29, "shaman"),
                new("Panda King", 30, "panda_king"),
                new("Penelope", 31, "penelope"),
                //new("Dimitri", 0x7443, "dimitri"),
                new("Dimitri swimmer", 0x3DDF, "dmitri_swimmer"), // 0x6C12 0x7440
                new("RC car", 0x9692, "c_rccar"), // 0x3AD7
            };
        }

        protected override List<Warp_t> GetEntranceLocations()
        {
            List<(int id, Warp_t warp)> entrances = new();
            string entrance = _m.ReadInt(EntranceRootNodePointer).ToString("X");

            string splicePointerOffset = "0";
            if (Build.Region.Contains("PS3"))
            {
                splicePointerOffset = "4";
            }

            while (entrance != "0")
            {
                int id = _m.ReadInt($"{entrance}+18");
                Matrix4x4 trans = _m.ReadMatrix4($"{entrance}+{_offsetEntranceTransformation}");
                string str = $"Entrance {id:X} [{entrance:X}]";
                int splicePointer = _m.ReadInt($"{entrance}+{splicePointerOffset}");
                if (splicePointer != 0)
                {
                    str += " (Splice)";
                }

                entrances.Add(new(id, new(str, trans)));
                entrance = _m.ReadInt($"{entrance}+20").ToString("X");
            }

            return entrances.OrderBy(e => e.id).Select(e => e.warp).ToList();
        }

        private enum GADGET_NAME
        {
            SmokeBomb,
            CombatDodge,
            Paraglide,
            SilentObliteration,
            FeralPounce,
            MegaJump,
            KnockoutDive,
            ShadowPowerLevel1,
            ThiefReflexes,
            ShadowPowerLevel2,
            RocketBoots,
            TreasureMap,
            Shield,
            VeniceDisguise,
            PhotographerDisguise,
            PirateDisguise,
            SpinAttackLevel1,
            SpinAttackLevel2,
            SpinAttackLevel3,
            JumpAttackLevel1,
            JumpAttackLevel2,
            JumpAttackLevel3,
            PushAttackLevel1,
            PushAttackLevel2,
            PushAttackLevel3,

            TriggerBomb,
            FishingPole,
            AlarmClock,
            AdrenalineBurst,
            HealthExtractor,
            HoverPack,
            InsanityStrike,
            GrappleCam,
            SizeDestabilizer,
            RageBomb,
            ReductionBomb,

            BeTheBall,
            BerserkerCharge,
            JuggernautThrow,
            GutturalRoar,
            FistsOfFlame,
            TemporalLock,
            RagingInfernoFlop,
            DiabloFireSlam,

            UnknownRocketBoots,
            VoltageAttack,
            MusicBox,
            TimeRush,
            PopeDisguise,
            Cube,
            SnoozeBomb,
            TurnbuckleLaunch,
            ButterflyNet,
            CutscenePuppet,
        }

        private static Dictionary<GADGET_NAME, string> _gadgetNames = new()
        {
            [GADGET_NAME.SmokeBomb] = "Smoke Bomb",
            [GADGET_NAME.CombatDodge] = "Combat Dodge",
            [GADGET_NAME.Paraglide] = "Paraglide",
            [GADGET_NAME.SilentObliteration] = "Silent Obliteration",
            [GADGET_NAME.FeralPounce] = "Feral Pounce",
            [GADGET_NAME.MegaJump] = "Mega Jump",
            [GADGET_NAME.KnockoutDive] = "Knockout Dive",
            [GADGET_NAME.ShadowPowerLevel1] = "Shadow Power Level 1",
            [GADGET_NAME.ThiefReflexes] = "Thief Reflexes",
            [GADGET_NAME.ShadowPowerLevel2] = "Shadow Power Level 2",
            [GADGET_NAME.RocketBoots] = "Rocket Boots",
            [GADGET_NAME.TreasureMap] = "Treasure Map",
            [GADGET_NAME.Shield] = "Shield",
            [GADGET_NAME.VeniceDisguise] = "Venice Disguise",
            [GADGET_NAME.PhotographerDisguise] = "Photographer Disguise",
            [GADGET_NAME.PirateDisguise] = "Pirate Disguise",
            [GADGET_NAME.SpinAttackLevel1] = "Spin Attack Level 1",
            [GADGET_NAME.SpinAttackLevel2] = "Spin Attack Level 2",
            [GADGET_NAME.SpinAttackLevel3] = "Spin Attack Level 3",
            [GADGET_NAME.JumpAttackLevel1] = "Jump Attack Level 1",
            [GADGET_NAME.JumpAttackLevel2] = "Jump Attack Level 2",
            [GADGET_NAME.JumpAttackLevel3] = "Jump Attack Level 3",
            [GADGET_NAME.PushAttackLevel1] = "Push Attack Level 1",
            [GADGET_NAME.PushAttackLevel2] = "Push Attack Level 2",
            [GADGET_NAME.PushAttackLevel3] = "Push Attack Level 3",

            [GADGET_NAME.TriggerBomb] = "Trigger Bomb",
            [GADGET_NAME.FishingPole] = "Fishing Pole",
            [GADGET_NAME.AlarmClock] = "Alarm Clock",
            [GADGET_NAME.AdrenalineBurst] = "Adrenaline Burst",
            [GADGET_NAME.HealthExtractor] = "Health Extractor",
            [GADGET_NAME.HoverPack] = "Hover Pack",
            [GADGET_NAME.InsanityStrike] = "Insanity Strike",
            [GADGET_NAME.GrappleCam] = "Grapple-Cam",
            [GADGET_NAME.SizeDestabilizer] = "Size Destabilizer",
            [GADGET_NAME.RageBomb] = "Rage Bomb",
            [GADGET_NAME.ReductionBomb] = "Reduction Bomb",

            [GADGET_NAME.BeTheBall] = "Be The Ball",
            [GADGET_NAME.BerserkerCharge] = "Berserker Charge",
            [GADGET_NAME.JuggernautThrow] = "Juggernaut Throw",
            [GADGET_NAME.GutturalRoar] = "Guttural Roar",
            [GADGET_NAME.FistsOfFlame] = "Fists of Flame",
            [GADGET_NAME.TemporalLock] = "Temporal Lock",
            [GADGET_NAME.RagingInfernoFlop] = "Raging Inferno Flop",
            [GADGET_NAME.DiabloFireSlam] = "Diablo Fire Slam",

            [GADGET_NAME.UnknownRocketBoots] = "Unknown Rocket Boots",
            [GADGET_NAME.VoltageAttack] = "Voltage Attack",
            [GADGET_NAME.MusicBox] = "Music Box",
            [GADGET_NAME.TimeRush] = "Time Rush",
            [GADGET_NAME.PopeDisguise] = "Pope Disguise",
            [GADGET_NAME.Cube] = "Cube",
            [GADGET_NAME.SnoozeBomb] = "Snooze Bomb",
            [GADGET_NAME.TurnbuckleLaunch] = "Turnbuckle Launch",
            [GADGET_NAME.ButterflyNet] = "Butterfly Net",
            [GADGET_NAME.CutscenePuppet] = "Cutscene Puppet",
        };

        protected override List<List<Gadget_t>> GetGadgets()
        {
            return new()
            {
                new()
                {
                    new(_gadgetNames[GADGET_NAME.SmokeBomb], 0x19),
                    new(_gadgetNames[GADGET_NAME.CombatDodge], 0x1A),
                    new(_gadgetNames[GADGET_NAME.Paraglide], 0x1B),
                    new(_gadgetNames[GADGET_NAME.SilentObliteration], 0x1C),
                    new(_gadgetNames[GADGET_NAME.FeralPounce], 0x1D),
                    new(_gadgetNames[GADGET_NAME.MegaJump], 0x1E),
                    new(_gadgetNames[GADGET_NAME.KnockoutDive], 0x1F),
                    new(_gadgetNames[GADGET_NAME.ShadowPowerLevel1], 0x20),
                    new(_gadgetNames[GADGET_NAME.ThiefReflexes], 0x21),
                    new(_gadgetNames[GADGET_NAME.ShadowPowerLevel2], 0x22),
                    new(_gadgetNames[GADGET_NAME.RocketBoots], 0x23),
                    new(_gadgetNames[GADGET_NAME.TreasureMap], 0x24),
                    new(_gadgetNames[GADGET_NAME.Shield], 0x25),
                    new(_gadgetNames[GADGET_NAME.VeniceDisguise], 0x26),
                    new(_gadgetNames[GADGET_NAME.PhotographerDisguise], 0x27),
                    new(_gadgetNames[GADGET_NAME.PirateDisguise], 0x28),
                    new(_gadgetNames[GADGET_NAME.SpinAttackLevel1], 0x29, false),
                    new(_gadgetNames[GADGET_NAME.SpinAttackLevel2], 0x2A, false),
                    new(_gadgetNames[GADGET_NAME.SpinAttackLevel3], 0x2B, false),
                    new(_gadgetNames[GADGET_NAME.JumpAttackLevel1], 0x2C, false),
                    new(_gadgetNames[GADGET_NAME.JumpAttackLevel2], 0x2D, false),
                    new(_gadgetNames[GADGET_NAME.JumpAttackLevel3], 0x2E, false),
                    new(_gadgetNames[GADGET_NAME.PushAttackLevel1], 0x2F, false),
                    new(_gadgetNames[GADGET_NAME.PushAttackLevel2], 0x30, false),
                    new(_gadgetNames[GADGET_NAME.PushAttackLevel3], 0x31, false),
                },

                new()
                {
                    new(_gadgetNames[GADGET_NAME.TriggerBomb], 0x6),
                    new(_gadgetNames[GADGET_NAME.FishingPole], 0x7),
                    new(_gadgetNames[GADGET_NAME.AlarmClock], 0x8),
                    new(_gadgetNames[GADGET_NAME.AdrenalineBurst], 0x9),
                    new(_gadgetNames[GADGET_NAME.HealthExtractor], 0xA),
                    new(_gadgetNames[GADGET_NAME.HoverPack], 0xB),
                    new(_gadgetNames[GADGET_NAME.InsanityStrike], 0xC),
                    new(_gadgetNames[GADGET_NAME.GrappleCam], 0xD),
                    new(_gadgetNames[GADGET_NAME.SizeDestabilizer], 0xE),
                    new(_gadgetNames[GADGET_NAME.RageBomb], 0xF),
                    new(_gadgetNames[GADGET_NAME.ReductionBomb], 0x10),
                },

                new()
                {
                    new(_gadgetNames[GADGET_NAME.BeTheBall], 0x11),
                    new(_gadgetNames[GADGET_NAME.BerserkerCharge], 0x12),
                    new(_gadgetNames[GADGET_NAME.JuggernautThrow], 0x13),
                    new(_gadgetNames[GADGET_NAME.GutturalRoar], 0x14),
                    new(_gadgetNames[GADGET_NAME.FistsOfFlame], 0x15),
                    new(_gadgetNames[GADGET_NAME.TemporalLock], 0x16),
                    new(_gadgetNames[GADGET_NAME.RagingInfernoFlop], 0x17),
                    new(_gadgetNames[GADGET_NAME.DiabloFireSlam], 0x18),
                },
            };
        }

        protected override List<Map_t> GetMaps()
        {
            return new()
            {
                new("DVD menu",
                    new()
                    {
                        new(),
                    }
                ),
                new("sampler_menu",
                    new()
                    {
                        new(),
                    },
                    false
                ),
                new("Hazard room",
                    new()
                    {
                        new("Center", 3550, 440, 150),
                        new("Top", 3580, 630, 3600),
                        new("Safehouse", 6640, 680, 150),
                    }
                ),
                new("Venice hub",
                    new()
                    {
                        new("Safehouse", 200, -2090, 273),
                        new("Safehouse (top)", 863, -1420, 1366),
                        new("Police HQ", -7570, 1670, 2062),
                        new("Ferris wheel", 6900, 1480, 260),
                        new("Stage", 6250, 8210, 360),
                        new("Fountain", -6670, 8550, 800),
                        new("Aquarium", 8040, -4365, 260),
                        new("Opera house (top)", 10100, 10100, 4500),
                    }
                ),
                new($"{SubMapNamePrefix}Canal chase",
                    new()
                    {
                        new("Boat", 0, 0, 230),
                        new("Intersection 1", 665, -12555, 240),
                        new("Intersection 2", 27250, 28580, 240),
                    }
                ),
                new($"{SubMapNamePrefix}Coffee houses",
                    new()
                    {
                        new("Door 1", 710, -5000, 225),
                        new("Door 2", 1070, 100, 225),
                        new("Door 3", 1160, 5000, 225),
                        new("Safe 1", -1710, -4990, 225),
                        new("Safe 2", -1750, 10, 225),
                        new("Safe 3", -3245, 4990, 225),
                        new("Roof", -1780, -4540, 1275),
                    }
                ),
                new($"{SubMapNamePrefix}Gauntlet / Opera house",
                    new()
                    {
                        new("Main door", -7130, -11340, 1130),
                        new("Basement door", 14440, -4000, 1115),
                        new("Pump room", -885, -2230, 280),
                        new("Worlitzer-700", -2100, 4890, 730),
                        new("Underground canal", 8720, -6490, 175),
                        new("Overlook", 8770, -5830, 1750),
                    }
                ),
                new($"{SubMapNamePrefix}Police station",
                    new()
                    {
                        new("Dimitri's cell", -60, 7600, 220),
                        new("Cell key", -685, 3250, 225),
                    }
                ),
                new("Outback hub",
                    new()
                    {
                        new("Safehouse", -4570, -7190, 1625),
                        new("Safehouse (top)", -4590, -7820, 2750),
                        new("Crane", -700, -1290, 4420),
                        new("Truck", 9820, -550, 1340),
                        new("Guru's hut", -8230, 4365, 2860),
                        new("Guru's cell", 8665, 5620, 2920),
                        new("Treeline", -8360, -3400, 5160),
                        new("Plateau", 6360, 7645, 7030),
                    }
                ),
                new($"{SubMapNamePrefix}Ayers Rock",
                    new()
                    {
                        new("Drill controls", 270, 160, 340),
                        new("Drill controls (top)", 420, 15, 2290),
                        new("Truck spawn", -16350, 8310, 4330),
                        new("Big door", 3830, 13920, 170),
                        new("Clifftop", 16260, 12890, 12760),
                    }
                ),
                new($"{SubMapNamePrefix}Oil field",
                    new()
                    {
                        new("The claw", 320, 10000, 170),
                        new("Catapult", 4820, -4470, 170),
                        new("Drill platform", -360, 620, 1335),
                    }
                ),
                new($"{SubMapNamePrefix}Cave 1 (Sly)",
                    new()
                    {
                        new("Door", -9345, 330, 120),
                        new("Safe", 6545, 125, 1211),
                        new("Drills", -780, -3420, 1220),
                    }
                ),
                new($"{SubMapNamePrefix}Cave 2 (Guru)",
                    new()
                    {
                        new("Door", -8945, 370, -1760),
                        new("Safe", -100, -4960, -510),
                        new("Hook conveyor belt", -5970, -1800, -1235),
                    }
                ),
                new($"{SubMapNamePrefix}Bar",
                    new()
                    {
                        new("Spawn", 0, 500, 200),
                    }
                ),
                new($"{SubMapNamePrefix}Cave 3 (Murray)",
                    new()
                    {
                        new("Door", -10230, -1445, -1040),
                        new("Piston", 3380, -1870, -920),
                        new("Triple piston", -2300, -8000, 250),
                    }
                ),
                new("Holland hub",
                    new()
                    {
                        new("Safehouse", 12180, -540, 1280),
                        new("Baron's hangar", -6015, 6880, 2855),
                        new("Forest", -2770, 3020, 530),
                        new("Ramp", -4645, -9100, 1780),
                        new("Barn", 3680, -6000, 700),
                    }
                ),
                new($"{SubMapNamePrefix}Hotel",
                    new()
                    {
                        new("Top floor", 2620, 280, 700),
                        new("Ham", -535, 420, 100),
                        new("Viking helmet", 830, 2950, 690),
                        new("Outside", 60, -6590, -445),
                    }
                ),
                new($"{SubMapNamePrefix}Hangar (team Belgium)",
                    new()
                    {
                        new("Spawn", 0, 0, 150),
                    }
                ),
                new($"{SubMapNamePrefix}Hangar (team Black Baron)",
                    new()
                    {
                        new("Spawn", 0, 600, 150),
                    }
                ),
                new($"{SubMapNamePrefix}Hangar (team Cooper)",
                    new()
                    {
                        new("Center", -180, -125, 175),
                        new("Control room", -1890, -130, 175),
                        new("Truck", -340, 2220, 1130),
                    }
                ),
                new($"{SubMapNamePrefix}Sewers",
                    new()
                    {
                        new("Ladder from hub", 20150, -9850, 310),
                        new("Path to hotel", 16490, 7280, 310),
                        new("Ladder to hotel", 7425, 9500, 310),
                        new("Ladder to hub", 7960, -12750, 310),
                        new("Platform", 200, 0, 200),
                    }
                ),
                new($"{SubMapNamePrefix}Biplane battlefield",
                    new()
                    {
                        new("Barn", -1890, 380, 970),
                        new("Crop squares", 17800, 3210, 1000),
                        new("Bridge 1", -140, -14670, 720),
                        new("Bridge 2", -4444, 16170, 550),
                        new("Bridge 3", 10460, 13260, 600),
                        new("Plane", 251764, -186, 100),
                    }
                ),
                new($"{SubMapNamePrefix}Two Player Hackathon",
                    new()
                    {
                        new(),
                    }
                ),
                new("China hub",
                    new()
                    {
                        new("Safehouse", -5440, -7500, 2120),
                        new("Turret tower", -5330, -8415, 3600),
                        new("Walk across the heavens", 7310, -8370, 5080),
                        new("Graveyard", 8570, 10150, 5940),
                        new("Statue", 795, -2980, 2015),
                        new("Palace", 940, 2255, 4890),
                    }
                ),
                new($"{SubMapNamePrefix}Intro",
                    new()
                    {
                        new("Passage", -2085, -54630, 950),
                        new("Panda King's perch", 400, -50485, 1988),
                        new("House", 3470, -51845, 920),
                        new("Clifftop", -2820, -57675, 5520),
                    }
                ),
                new($"{SubMapNamePrefix}Panda King's flashback",
                    new()
                    {
                        new("Spawn", -2513, 0, -200),
                    }
                ),
                new($"{SubMapNamePrefix}Tsao's battleground",
                    new()
                    {
                        new("Top", -50, 3060, 840),
                        new("Bottom", 130, 30410, 150),
                        new("Overlook", -4545, 35970, 4775),
                    }
                ),
                new($"{SubMapNamePrefix}Panda King's house",
                    new()
                    {
                        new("Yin", -1855, -100, 20095),
                        new("Yang", -240, -100, 20095),
                    }
                ),
                new($"{SubMapNamePrefix}Tsao's business center",
                    new()
                    {
                        new("Door", -3200, 0, 100),
                        new("Second floor", 1050, 1580, 800),
                        new("Computer", 1075, -1515, 800),
                        new("Outside", -4210, -140, 0),
                        new("Overlook", -10480, -4100, 2900),
                    }
                ),
                new($"{SubMapNamePrefix}Palace",
                    new()
                    {
                        new("Vases", -5270, -60, -50),
                        new("Computer", -2250, 1445, 150),
                        new("Jing King's room", 470, -1500, 150),
                        new("Drill site", 2075, 15000, 700),
                    }
                ),
                new($"{SubMapNamePrefix}Treasure temple",
                    new()
                    {
                        new("Door", -6300, -130, 500),
                        new("Treasure area", 1725, 730, -200),
                        new("Crawlspace", -560, 140, 1800),
                    }
                ),
                new("Pirate hub",
                    new()
                    {
                        new("Safehouse", 4900, 1345, 1225),
                        new("Safehouse (top)", 5590, 2310, 2780),
                        new("Skull keep (top)", -9600, -1880, 4510),
                        new("Waterfall (top)", 3625, 16360, 4535),
                        new("Fireplace", -530, 7030, 2070),
                        new("Monkeys?", -7415, 11565, 1620),
                        new("Cooper gang ship", 11390, -9290, 1650),
                        new("Archipelago", -26550, -19930, 2200),
                    }
                ),
                new($"{SubMapNamePrefix}Sailing map",
                    new()
                    {
                        new(),
                    }
                ),
                new($"{SubMapNamePrefix}Underwater shipwreck",
                    new()
                    {
                        new("Spawn", 28980, -100, 2800),
                        new("Ship (top)", 21020, 14180, 6030),
                        new("Shipwreck", 22460, 12380, -3280),
                        new("Depths", 21910, 8690, -7085),
                        new("Ocean current", 20860, 22410, -6920),

                    }
                ),
                new($"{SubMapNamePrefix}Dagger island",
                    new()
                    {
                        new("Cooper gang ship", -16760, 2940, 1000),
                        new("Palm tree circle", -8040, -970, 1200),
                        new("Flipped ship", 1620, -5250, 1240),
                        new("Pirate ship", 15680, 5290, 870),
                        new("Mountain peak", 2215, 10860, 8040),
                    }
                ),
                new("Kaine island",
                    new()
                    {
                        new("Spawn", -6715, -14380, -2800),
                        new("Wall sneak (top)", -6285, -2220, -2080),
                        new("Ventilation shaft", -1870, 4765, -3755),
                        new("Vault", -1085, -100, 2460),
                        new("Ship dock", 7855, -24360, -3670),
                        new("RC car track", -13650, -14110, -2090),
                        new("Random rope", -16480, 1520, -2730),
                        new("Rock formation", 13730, 20650, 2200),
                    }
                ),
                new($"{SubMapNamePrefix}Underwater",
                    new()
                    {
                        new("Spawn", 51570, 23850, -5745),
                        new("Water tube", 745, -34265, -720),
                        new("Boss area", 10200, -59780, 0),
                    }
                ),
                new($"{SubMapNamePrefix}Cooper vault (lobby)",
                    new()
                    {
                        new("Center", 0, 0, 140),
                        new("Vault door", 4350, -45, 560),
                    }
                ),
                new($"{SubMapNamePrefix}Cooper vault (gauntlet)",
                    new()
                    {
                        new("Slytunkhamen II", -28690, 21665, -2075),
                        new("Sir Galleth Cooper", -24715, 13995, -2160),
                        new("Salim Al-Kupar", -13760, 12485, -2100),
                        new("Slaigh MacCooper", -15325, 24680, -2090),
                        new("Rioichi Cooper", -21130, 19955, -80),
                        new("Henriette Cooper", -10530, 13200, 220),
                        new("Tennesee 'Kid' Cooper", 2010, 13275, -2180),
                        new("Thaddeus Winslow Cooper III", 9360, 1820, -2085),
                        new("Otto Van Cooper", -2050, 2740, 100),
                        new("Conner Cooper", 7515, 5030, 250),
                        new("Inner Sanctum door", 16645, -2260, 220),
                    }
                ),
                new($"{SubMapNamePrefix}Dr. M's arena",
                    new()
                    {
                        new("Center", 0, 0, 130),
                        new("Top", -3840, 1600, 2970),
                    }
                ),
            };
        }
    }
}
