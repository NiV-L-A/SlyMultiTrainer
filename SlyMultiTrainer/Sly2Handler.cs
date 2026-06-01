using System.Numerics;
using System.Text;
using static SlyMultiTrainer.Sly2_3_Savefile;
using static SlyMultiTrainer.Util;

namespace SlyMultiTrainer
{
    public class Sly2Handler : GameBase_t
    {
        private Memory.Mem _m;
        private Form1 _form;
        private Encoding _encoding;

        public string ReloadAddress = "";
        public string ReloadValuesPointer = "";
        public string FKXListCountAddress = "";
        public string ActiveCharacterPointer = "";
        public string ActiveCharacterIdAddress = "";
        public string ActiveCharacterHealthAddress = "";
        public string StringTableCountAddress = "";
        public string IsLoadingAddress = "";
        public string EntranceRootNodePointer = "";
        public DAG_t DAG;
        public Sly2_3_Savefile Savefile;

        private string _offsetTransformationOrigin = "54";
        private string _offsetTransformationLocal = "";
        private string _offsetTransformationWorld = "";
        private string _offsetTransformationFinal = "";
        private string _offsetCollision = "EC";
        private string _offsetRadTarget = "1A8";
        private string _offsetDeltaTranslation = "240";
        private string _offsetInvulnerable = "298";
        private string _offsetInfiniteDbJump = "2E8";
        private string _offsetSpeedMultiplier = "2F8";
        private string _offsetSavefileHealth = "E00";
        private string _offsetSavefileGadgetPower = "";
        private string _offsetGadgetBinds = "1180";
        private string _offsetUndetectable = "11AC";
        private string _offsetEntranceTransformation = "60";
        private string _offsetCurrentDialogue = "60";
        private string _offsetDialogueFlags = "50";

        public Sly2Handler(Form1 form, Memory.Mem m, Build_t build) : base(form, m, build)
        {
            _m = m;
            _form = form;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _encoding = Encoding.GetEncoding(1252);
            DAG = new(m);
            DAG.SetVersion(DAG_VERSION.V1);
            Savefile = new(m);
            Savefile.SetVersion(SAVEFILE_VERSION.V1);

            DAG.OffsetState = "54";
            DAG.OffsetGoalDescription = "5C";
            DAG.OffsetFocusCount = "64";
            DAG.OffsetCompleteCount = "68";
            DAG.OffsetMissionName = "6C";
            DAG.OffsetMissionDescription = "70";
            DAG.OffsetClusterPointer = "7C";
            DAG.OffsetChildrenCount = "A0";
            DAG.OffsetCheckpointEntranceValue = "B8";
            DAG.OffsetAttributes = "C8";
            DAG.OffsetAttributesForCluster = "D0";
            DAG.OffsetVerticalLayer = "100";
            DAG.GetStringFromId = GetStringFromId;
            DAG.LoadMap = LoadMap;
            DAG.WriteActCharId = WriteActCharId;

            if (build.Region == Util.BuildRegions[BUILD_NAME.NTSC])
            {
                // SCUS-97316 - 07652DD9
                ReloadAddress = "3E1080";
                ReloadValuesPointer = "3E1C40";
                FKXListCountAddress = "3E1394";
                ClockAddress = "2DDED8";
                CoinsAddress = "3D4B00";
                GadgetAddress = "3D4AF8";
                DrawDistanceAddress = "2DDF5C";
                FOVAddress = "2DDF64";
                ResetCameraAddress = "2DE240";
                CanCameraNoclipAddress = "2C4118";
                MapIdAddress = "3E1110";
                GuardAIAddress = "3E1214";
                ActiveCharacterPointer = "3E138C";
                ActiveCharacterIdAddress = "3D4A6C";
                ActiveCharacterHealthAddress = $"{ActiveCharacterPointer},{_offsetSavefileHealth},0";
                StringTableCountAddress = "3E1AD0";
                IsLoadingAddress = "3D3980";
                EntranceRootNodePointer = "3E06C0";
                DAG.RootNodePointer = "3E0B04";
                DAG.CurrentCheckpointNodePointer = "3E0FA4";
                DAG.ClusterIdAddress = "2DEB40";
                Savefile.SavefileStartAddress = "3D4A60";
                Savefile.SavefileAddressTablePointer = "3E0EAC";
                Savefile.SavefileStringTablePointer = "3E0F88";
                ControllerAddress = "2E0CB4";
                DialoguePointer = "3DD240";
                SkipFMVPointer = "2F6808";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.PALv100]
                  || build.Region == Util.BuildRegions[BUILD_NAME.PALv201]
                  || build.Region == Util.BuildRegions[BUILD_NAME.PALSeptember11])
            {
                // SCES-52529 - FDA1CBF6
                // SCES-52529 - 15DD1F6F
                // SCES-52529 - B89723F2
                ReloadAddress = "3E8880";
                ReloadValuesPointer = "3E9430";
                FKXListCountAddress = "3E8B94";
                ClockAddress = "2E52D8";
                CoinsAddress = "3DC300";
                GadgetAddress = "3DC2F8";
                DrawDistanceAddress = "2E535C";
                FOVAddress = "2E5364";
                ResetCameraAddress = "2E5640";
                CanCameraNoclipAddress = "2CB548";
                MapIdAddress = "3E8910";
                GuardAIAddress = "3E8A14";
                ActiveCharacterPointer = "3E8B8C";
                ActiveCharacterIdAddress = "3DC26C";
                ActiveCharacterHealthAddress = $"{ActiveCharacterPointer},{_offsetSavefileHealth},0";
                StringTableCountAddress = "3E92D0";
                IsLoadingAddress = "3DB180";
                EntranceRootNodePointer = "3E7EC0";
                DAG.RootNodePointer = "3E8304";
                DAG.CurrentCheckpointNodePointer = "3E87A4";
                DAG.ClusterIdAddress = "2E5F40";
                Savefile.SavefileStartAddress = "3DC260";
                Savefile.SavefileAddressTablePointer = "3E86AC";
                Savefile.SavefileStringTablePointer = "3E8788";
                ControllerAddress = "2E80B4";
                DialoguePointer = "3E4A70";
                SkipFMVPointer = "2FDC18";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCJ])
            {
                // SCPS-15090 - 615EA2DB
                _encoding = Encoding.Unicode;
                _offsetTransformationOrigin = "44";
                _offsetCollision = "DC";
                _offsetRadTarget = "198";
                _offsetDeltaTranslation = "230";
                _offsetInfiniteDbJump = "2D8";
                _offsetSpeedMultiplier = "2E8";
                _offsetInvulnerable = "288";
                _offsetSavefileHealth = "DF0";
                _offsetGadgetBinds = "1170";
                _offsetUndetectable = "119C";
                _offsetEntranceTransformation = "50";
                DAG.OffsetState = "44";
                DAG.OffsetGoalDescription = "4C";
                DAG.OffsetFocusCount = "54";
                DAG.OffsetCompleteCount = "58";
                DAG.OffsetMissionName = "5C";
                DAG.OffsetMissionDescription = "60";
                DAG.OffsetClusterPointer = "6C";
                DAG.OffsetChildrenCount = "90";
                DAG.OffsetCheckpointEntranceValue = "A8";
                DAG.OffsetAttributes = "B8";
                DAG.OffsetAttributesForCluster = "C0";
                DAG.OffsetVerticalLayer = "F0";

                ReloadAddress = "3EAA80";
                ReloadValuesPointer = "3EB630";
                FKXListCountAddress = "3EAD94";
                ClockAddress = "2E7158";
                CoinsAddress = "3DE300";
                GadgetAddress = "3DE2F8";
                DrawDistanceAddress = "2E71DC";
                FOVAddress = "2E71E4";
                ResetCameraAddress = "2E74C0";
                CanCameraNoclipAddress = "2CD328";
                MapIdAddress = "3EAB10";
                GuardAIAddress = "3EAC14";
                ActiveCharacterPointer = "3EAD8C";
                ActiveCharacterIdAddress = "3DE26C";
                ActiveCharacterHealthAddress = $"{ActiveCharacterPointer},{_offsetSavefileHealth},0";
                StringTableCountAddress = "3EB4D0";
                IsLoadingAddress = "3DD180";
                EntranceRootNodePointer = "3E9EC0";
                DAG.RootNodePointer = "3EA304";
                DAG.CurrentCheckpointNodePointer = "3EA9A4";
                DAG.ClusterIdAddress = "2E7DC0";
                Savefile.SavefileStartAddress = "3DE260";
                Savefile.SavefileAddressTablePointer = "3EA8AC";
                Savefile.SavefileStringTablePointer = "3EA988";
                ControllerAddress = "2E9F34";
                DialoguePointer = "3E6A70";
                SkipFMVPointer = "2FFA78";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCK])
            {
                // SCKA-20044 - 518DD841
                _encoding = Encoding.Unicode;
                _offsetTransformationOrigin = "44";
                _offsetCollision = "DC";
                _offsetRadTarget = "198";
                _offsetDeltaTranslation = "230";
                _offsetInfiniteDbJump = "2D8";
                _offsetSpeedMultiplier = "2E8";
                _offsetInvulnerable = "288";
                _offsetSavefileHealth = "DF0";
                _offsetGadgetBinds = "1170";
                _offsetUndetectable = "119C";
                _offsetEntranceTransformation = "50";
                DAG.OffsetState = "44";
                DAG.OffsetGoalDescription = "4C";
                DAG.OffsetFocusCount = "54";
                DAG.OffsetCompleteCount = "58";
                DAG.OffsetMissionName = "5C";
                DAG.OffsetMissionDescription = "60";
                DAG.OffsetClusterPointer = "6C";
                DAG.OffsetChildrenCount = "90";
                DAG.OffsetCheckpointEntranceValue = "A8";
                DAG.OffsetAttributes = "B8";
                DAG.OffsetAttributesForCluster = "C0";
                DAG.OffsetVerticalLayer = "F0";

                ReloadAddress = "3EA100";
                ReloadValuesPointer = "3EACB0";
                FKXListCountAddress = "3EA414";
                ClockAddress = "2E6758";
                CoinsAddress = "3DD980";
                GadgetAddress = "3DD978";
                DrawDistanceAddress = "2E67DC";
                FOVAddress = "2E67E4";
                ResetCameraAddress = "2E6AC0";
                CanCameraNoclipAddress = "2CC958";
                MapIdAddress = "3EA190";
                GuardAIAddress = "3EA294";
                ActiveCharacterPointer = "3EA40C";
                ActiveCharacterIdAddress = "3DD8EC";
                ActiveCharacterHealthAddress = $"{ActiveCharacterPointer},{_offsetSavefileHealth},0";
                StringTableCountAddress = "3EAB50";
                IsLoadingAddress = "3DC800";
                EntranceRootNodePointer = "3E9540";
                DAG.RootNodePointer = "3E9984";
                DAG.CurrentCheckpointNodePointer = "3EA024";
                DAG.ClusterIdAddress = "2E73C0";
                Savefile.SavefileStartAddress = "3DD8E0";
                Savefile.SavefileAddressTablePointer = "3E9F2C";
                Savefile.SavefileStringTablePointer = "3EA008";
                ControllerAddress = "2E9534";
                DialoguePointer = "3E60F0";
                SkipFMVPointer = "2FF098";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCE3Demo])
            {
                // SCUS-97415 - 5B93397F
                DAG.SetVersion(DAG_VERSION.V0);
                Savefile.SetVersion(SAVEFILE_VERSION.V0);
                _offsetRadTarget = "198";
                _offsetDeltaTranslation = "230";
                _offsetInfiniteDbJump = "338";
                _offsetSpeedMultiplier = "344";
                _offsetSavefileHealth = "FD0";
                _offsetUndetectable = "13B8";
                DAG.OffsetClusterPointer = "74";
                DAG.OffsetChildrenCount = "94";
                DAG.OffsetCheckpointEntranceValue = "AC";
                DAG.OffsetAttributes = "B8";
                DAG.OffsetAttributesForCluster = "BC";
                DAG.OffsetVerticalLayer = "E0";

                ReloadAddress = "39A860";
                ReloadValuesPointer = "39CEC4";
                FKXListCountAddress = "39AB34";
                ClockAddress = "2D1F58";
                CoinsAddress = "2D2B08";
                DrawDistanceAddress = "2D1FDC";
                FOVAddress = "2D1FE4";
                ResetCameraAddress = "2D2324";
                CanCameraNoclipAddress = "2A8908";
                MapIdAddress = "39A8F0";
                GuardAIAddress = "39A9E8";
                ActiveCharacterPointer = "39AB2C";
                ActiveCharacterIdAddress = "3CA7C2";
                ActiveCharacterHealthAddress = $"{ActiveCharacterPointer},{_offsetSavefileHealth}";
                StringTableCountAddress = "39CE64";
                IsLoadingAddress = "393700";
                EntranceRootNodePointer = "399F44";
                DAG.RootNodePointer = "39A3E8";
                DAG.CurrentCheckpointNodePointer = "39A854";
                DAG.ClusterIdAddress = "2D2B20";
                Savefile.SavefileStartAddress = "3CA7C0";
                Savefile.SavefileAddressTablePointer = "39A75C";
                Savefile.SavefileStringTablePointer = "39A838";
                ControllerAddress = "2D4E30";
                DialoguePointer = "396B00";
                SkipFMVPointer = "2D6510";

                var tmp = Maps[1];
                Maps.RemoveAt(1);
                Maps.Insert(0, tmp);
                Maps[0].IsVisible = false;
                Maps[3].IsVisible = false;
                Maps[5].IsVisible = false;
                Maps.Skip(7).ToList().ForEach(m => m.IsVisible = false);

                // nightclub door entrance
                Maps[4].Warps[0].Transformation = Matrix4x4.CreateTranslation(-4200, 5400, -200);
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCOfficialPlayStationMagazineDemoDisc089])
            {
                // SCUS-97342 - 7B564230
                _offsetRadTarget = "1B8";
                _offsetDeltaTranslation = "250";
                _offsetInfiniteDbJump = "348";
                _offsetSpeedMultiplier = "354";
                //_offsetInvulnerable = "298"; // TO FIND
                _offsetSavefileHealth = "E60";
                _offsetGadgetBinds = "11E0";
                _offsetUndetectable = "1210";

                ReloadAddress = "3F0EC8";
                ReloadValuesPointer = "3F1A80";
                FKXListCountAddress = "3F11B4";
                ClockAddress = "302ED8";
                CoinsAddress = "3E494C";
                GadgetAddress = "3E4944";
                DrawDistanceAddress = "302F5C";
                FOVAddress = "302F64";
                ResetCameraAddress = "303240";
                CanCameraNoclipAddress = "2C9A98";
                MapIdAddress = "3F0F58";
                GuardAIAddress = "3F105C";
                ActiveCharacterPointer = "3F11AC";
                ActiveCharacterIdAddress = "3E48BC";
                ActiveCharacterHealthAddress = $"{ActiveCharacterPointer},{_offsetSavefileHealth},0";
                StringTableCountAddress = "3F1920";
                IsLoadingAddress = "3E4800";
                EntranceRootNodePointer = "3F04C0";
                DAG.RootNodePointer = "3F0958";
                DAG.CurrentCheckpointNodePointer = "3F0E04";
                DAG.ClusterIdAddress = "303B2C";
                Savefile.SavefileStartAddress = "3E48B0";
                Savefile.SavefileAddressTablePointer = "3F0D0C";
                Savefile.SavefileStringTablePointer = "3F0DE8";
                ControllerAddress = "305E74";
                DialoguePointer = "3ED060";
                SkipFMVPointer = "307648";

                Maps[1].IsVisible = false; // dvd_menu
                Maps[3].IsVisible = false; // wine cellar
                Maps[5].IsVisible = false; // print room
                Maps.Skip(7).ToList().ForEach(m => m.IsVisible = false);

                //Gadgets[0].RemoveRange(18, 2); // remove tom and timerush
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.PALDemoJuly27])
            {
                _offsetRadTarget = "1B8";
                _offsetDeltaTranslation = "250";
                _offsetInfiniteDbJump = "348";
                _offsetSpeedMultiplier = "354";
                //_offsetInvulnerable = "298"; // TO FIND
                _offsetSavefileHealth = "E60";
                _offsetGadgetBinds = "11E0";
                _offsetUndetectable = "1210";

                ReloadAddress = "3EF2C8";
                ReloadValuesPointer = "3EFE80";
                FKXListCountAddress = "3EF5B4";
                ClockAddress = "3013A0";
                CoinsAddress = "3E2D4C";
                GadgetAddress = "3E2D44";
                DrawDistanceAddress = "30142C";
                FOVAddress = "301434";
                ResetCameraAddress = "301710";
                CanCameraNoclipAddress = "2C7FB8";
                MapIdAddress = "3EF358";
                GuardAIAddress = "3EF45C";
                ActiveCharacterPointer = "3EF5AC";
                ActiveCharacterIdAddress = "3E2CBC";
                ActiveCharacterHealthAddress = $"{ActiveCharacterPointer},{_offsetSavefileHealth},0";
                StringTableCountAddress = "3EFD20";
                IsLoadingAddress = "3E2C80";
                EntranceRootNodePointer = "3EE8C0";
                DAG.RootNodePointer = "3EED58";
                DAG.CurrentCheckpointNodePointer = "3EF204";
                DAG.ClusterIdAddress = "301FF4";
                Savefile.SavefileStartAddress = "3E2CB0";
                Savefile.SavefileAddressTablePointer = "3EF10C";
                Savefile.SavefileStringTablePointer = "3EF1E8";
                ControllerAddress = "304334";
                DialoguePointer = "3EB460";
                SkipFMVPointer = "305AC8";

                Maps[1].IsVisible = false; // dvd_menu
                Maps[3].IsVisible = false; // wine cellar
                Maps[5].IsVisible = false; // print room
                Maps.Skip(7).ToList().ForEach(m => m.IsVisible = false);
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCDemoRatchetClankUpYourArsenal])
            {
                // SCUS-97353 - 45FE0CC4
                _offsetRadTarget = "1B8";
                _offsetDeltaTranslation = "250";
                _offsetInfiniteDbJump = "348";
                _offsetSpeedMultiplier = "354";
                _offsetSavefileHealth = "E60";
                //_offsetInvulnerable = "298"; // TO FIND
                _offsetGadgetBinds = "11E0";
                _offsetUndetectable = "1210";

                ReloadAddress = "3F0CC8";
                ReloadValuesPointer = "3F1880";
                FKXListCountAddress = "3F0FB4";
                ClockAddress = "302CD8";
                CoinsAddress = "3E474C";
                GadgetAddress = "3E4744";
                DrawDistanceAddress = "302D5C";
                FOVAddress = "302D64";
                ResetCameraAddress = "303040";
                CanCameraNoclipAddress = "2C9898";
                MapIdAddress = "3F0D58";
                GuardAIAddress = "3F0E5C";
                ActiveCharacterPointer = "3F0FAC";
                ActiveCharacterIdAddress = "3E46BC";
                ActiveCharacterHealthAddress = $"{ActiveCharacterPointer},{_offsetSavefileHealth},0";
                StringTableCountAddress = "3F1720";
                IsLoadingAddress = "3E4600";
                EntranceRootNodePointer = "3F02C0";
                DAG.RootNodePointer = "3F0758";
                DAG.CurrentCheckpointNodePointer = "3F0C04";
                DAG.ClusterIdAddress = "30392C";
                Savefile.SavefileStartAddress = "3E46B0";
                Savefile.SavefileAddressTablePointer = "3F0B0C";
                Savefile.SavefileStringTablePointer = "3F0BE8";
                ControllerAddress = "305C74";
                DialoguePointer = "3ECE60";
                SkipFMVPointer = "307448";

                Maps[1].IsVisible = false; // dvd_menu
                Maps[3].IsVisible = false; // wine cellar
                Maps[5].IsVisible = false; // print room
                Maps.Skip(7).ToList().ForEach(m => m.IsVisible = false);

                //Gadgets[0].RemoveRange(18, 2); // remove tom and timerush
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.PALDemoRatchetClank3])
            {
                // SCES-52456 - 17125698
                _offsetRadTarget = "1B8";
                _offsetDeltaTranslation = "250";
                _offsetInfiniteDbJump = "348";
                _offsetSpeedMultiplier = "354";
                _offsetSavefileHealth = "E60";
                //_offsetInvulnerable = "298"; // TO FIND
                _offsetGadgetBinds = "11E0";
                _offsetUndetectable = "1210";

                ReloadAddress = "3F0D48";
                ReloadValuesPointer = "3F1900";
                FKXListCountAddress = "3F1034";
                ClockAddress = "302D58";
                CoinsAddress = "3E47CC";
                GadgetAddress = "3E47C4";
                DrawDistanceAddress = "302DDC";
                FOVAddress = "302DE4";
                ResetCameraAddress = "3030C0";
                CanCameraNoclipAddress = "2C9918";
                MapIdAddress = "3F0DD8";
                GuardAIAddress = "3F0EDC";
                ActiveCharacterPointer = "3F102C";
                ActiveCharacterIdAddress = "3E473C";
                ActiveCharacterHealthAddress = $"{ActiveCharacterPointer},{_offsetSavefileHealth},0";
                StringTableCountAddress = "3F17A0";
                IsLoadingAddress = "3E4680";
                EntranceRootNodePointer = "3F0340";
                DAG.RootNodePointer = "3F07D8";
                DAG.CurrentCheckpointNodePointer = "3F0C84";
                DAG.ClusterIdAddress = "3039AC";
                Savefile.SavefileStartAddress = "3E4730";
                Savefile.SavefileAddressTablePointer = "3F0B8C";
                Savefile.SavefileStringTablePointer = "3F0C68";
                ControllerAddress = "305CF4";
                DialoguePointer = "3ECEE0";
                SkipFMVPointer = "3074C8";

                Maps[1].IsVisible = false; // dvd_menu
                Maps[3].IsVisible = false; // wine cellar
                Maps[5].IsVisible = false; // print room
                Maps.Skip(7).ToList().ForEach(m => m.IsVisible = false);

                //Gadgets[0].RemoveRange(18, 2); // remove tom and timerush
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCDemoRatchetClankUpYourArsenalAugust11])
            {
                // SCUS-97353 - D8EB2C29
                _offsetRadTarget = "1B8";
                _offsetDeltaTranslation = "250";
                _offsetInfiniteDbJump = "348";
                _offsetSpeedMultiplier = "354";
                //_offsetInvulnerable = "298"; // TO FIND
                _offsetSavefileHealth = "1040";
                _offsetGadgetBinds = "1390";
                _offsetUndetectable = "13C0";

                ReloadAddress = "3E91D8";
                ReloadValuesPointer = "3E9D80";
                FKXListCountAddress = "3E94C4";
                ClockAddress = "2FB2A0";
                CoinsAddress = "3DCC4C";
                GadgetAddress = "3DCC44";
                DrawDistanceAddress = "2FB32C";
                FOVAddress = "2FB334";
                ResetCameraAddress = "2FB610";
                CanCameraNoclipAddress = "2C6F08";
                MapIdAddress = "3E9268";
                GuardAIAddress = "3E936C";
                ActiveCharacterPointer = "3E94BC";
                ActiveCharacterIdAddress = "3DCBBC";
                ActiveCharacterHealthAddress = $"{ActiveCharacterPointer},{_offsetSavefileHealth},0";
                StringTableCountAddress = "3E9C30";
                IsLoadingAddress = "3DCB80";
                EntranceRootNodePointer = "3E87C0";
                DAG.RootNodePointer = "3E8C58";
                DAG.CurrentCheckpointNodePointer = "3E9114";
                DAG.ClusterIdAddress = "2FBEF4";
                Savefile.SavefileStartAddress = "3DCBB0";
                Savefile.SavefileAddressTablePointer = "3E901C";
                Savefile.SavefileStringTablePointer = "3E90F8";
                ControllerAddress = "2FE234";
                DialoguePointer = "3E5370";
                SkipFMVPointer = "2FF9D0";

                Maps[1].IsVisible = false; // dvd_menu
                Maps[3].IsVisible = false; // wine cellar
                Maps[5].IsVisible = false; // print room
                Maps.Skip(7).ToList().ForEach(m => m.IsVisible = false);

                Gadgets = new()
                {
                    new()
                    {
                        new(_gadgetNames[GADGET_NAME.SmokeBomb], 0x18),
                        new(_gadgetNames[GADGET_NAME.CombatDodge], 0x19),
                        new(_gadgetNames[GADGET_NAME.StealthSlide], 0x1A),
                        new(_gadgetNames[GADGET_NAME.AlarmClock], 0x1B),
                        new(_gadgetNames[GADGET_NAME.Paraglide], 0x1C),
                        new(_gadgetNames[GADGET_NAME.SilentObliteration], 0x1D),
                        new(_gadgetNames[GADGET_NAME.ThiefReflexes], 0x1E),
                        new(_gadgetNames[GADGET_NAME.FeralPounce], 0x1F),
                        new(_gadgetNames[GADGET_NAME.MegaJump], 0x20),
                        new(_gadgetNames[GADGET_NAME.TornadoStrike], 0x21),
                        new(_gadgetNames[GADGET_NAME.KnockoutDive], 0x22),
                        new(_gadgetNames[GADGET_NAME.InsanityStrike], 0x23),
                        new(_gadgetNames[GADGET_NAME.VoltageAttack], 0x24),
                        new(_gadgetNames[GADGET_NAME.LongToss], 0x25),
                        new(_gadgetNames[GADGET_NAME.RageBomb], 0x26),
                        new(_gadgetNames[GADGET_NAME.MusicBox], 0x27),
                        new(_gadgetNames[GADGET_NAME.LightningSpin], 0x28),
                        new(_gadgetNames[GADGET_NAME.ShadowPower], 0x29),
                    },

                    new()
                    {
                        new(_gadgetNames[GADGET_NAME.TriggerBomb], 0x7),
                        new(_gadgetNames[GADGET_NAME.SizeDestabilizer], 0x8),
                        new(_gadgetNames[GADGET_NAME.SnoozeBomb], 0x9),
                        new(_gadgetNames[GADGET_NAME.AdrenalineBurst], 0xA),
                        new(_gadgetNames[GADGET_NAME.HealthExtractor], 0xB),
                        new(_gadgetNames[GADGET_NAME.HoverPack], 0xC),
                        new(_gadgetNames[GADGET_NAME.ReductionBomb], 0xD),
                        new(_gadgetNames[GADGET_NAME.TemporalLock], 0xE),
                    },

                    new()
                    {
                        new(_gadgetNames[GADGET_NAME.Uppercut], 0xF),
                        new(_gadgetNames[GADGET_NAME.FistsOfFlame], 0x10),
                        new(_gadgetNames[GADGET_NAME.TurnbuckleLaunch], 0x11),
                        new(_gadgetNames[GADGET_NAME.JuggernautThrow], 0x12),
                        new(_gadgetNames[GADGET_NAME.AtlasStrength], 0x13),
                        new(_gadgetNames[GADGET_NAME.DiabloFireSlam], 0x14),
                        new(_gadgetNames[GADGET_NAME.BerserkerCharge], 0x15),
                        new(_gadgetNames[GADGET_NAME.GutturalRoar], 0x16),
                        new(_gadgetNames[GADGET_NAME.RagingInfernoFlop], 0x17),
                    },
                };
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCMarch17])
            {
                // SCUS-97198 - DD0B5E6C
                DAG.SetVersion(DAG_VERSION.V0);
                Savefile.SetVersion(SAVEFILE_VERSION.V0);
                _offsetRadTarget = "198";
                _offsetDeltaTranslation = "230";
                _offsetInfiniteDbJump = "328";
                _offsetSpeedMultiplier = "334";
                //_offsetInvulnerable = "298";
                _offsetSavefileHealth = "ED0";
                _offsetUndetectable = "12A8";
                DAG.OffsetState = "54";
                DAG.OffsetGoalDescription = "5C";
                DAG.OffsetMissionName = "BC";
                DAG.OffsetMissionDescription = "70";
                DAG.OffsetClusterPointer = "74";
                DAG.OffsetChildrenCount = "94";
                DAG.OffsetCheckpointEntranceValue = "AC";
                DAG.OffsetAttributes = "B8";
                DAG.OffsetAttributesForCluster = "B8";
                DAG.OffsetVerticalLayer = "E0";

                ReloadAddress = "3EE978";
                ReloadValuesPointer = "3F0F34";
                FKXListCountAddress = "3EEBF8";
                ClockAddress = "303E18";
                CoinsAddress = "304A04";
                GadgetAddress = "";
                DrawDistanceAddress = "303E9C";
                FOVAddress = "303EA4";
                ResetCameraAddress = "3041F4";
                CanCameraNoclipAddress = "2F8BA8";
                MapIdAddress = "3EEA08";
                GuardAIAddress = "3EEB00";
                ActiveCharacterPointer = "3EEBF0";
                ActiveCharacterIdAddress = "41FC82";
                ActiveCharacterHealthAddress = $"{ActiveCharacterPointer},{_offsetSavefileHealth}";
                StringTableCountAddress = "3F0F14";
                IsLoadingAddress = "3E7380";
                EntranceRootNodePointer = "3EE0D0";
                DAG.RootNodePointer = "3EE52C";
                DAG.CurrentCheckpointNodePointer = "3EE974";
                DAG.ClusterIdAddress = "304A1C";
                Savefile.SavefileStartAddress = "41FC80";
                Savefile.SavefileAddressTablePointer = "3EE87C";
                Savefile.SavefileStringTablePointer = "3EE958";
                ControllerAddress = "3072F0";
                DialoguePointer = "3EACCC";
                //SkipFMVPointer = ""; // you can already skip fmv in-game

                // Remove ep8, vault room and dvd menu
                Maps.RemoveRange(14, 30);
                Maps.RemoveAt(1);
                Maps[0].IsVisible = false; // cairo

                Maps.Insert(0, new("Splash", new() { new() }));
                Maps.Insert(12, new($"{SubMapNamePrefix}i_palace_heist", new(), false));

                // nightclub door entrance
                Maps[4].Warps[0].Transformation = Matrix4x4.CreateTranslation(-4200, 5400, -200);
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCJuly11])
            {
                // SCUS-97316 - A480549C
                _offsetRadTarget = "1B8";
                _offsetDeltaTranslation = "250";
                _offsetInfiniteDbJump = "348";
                _offsetSpeedMultiplier = "354";
                //_offsetInvulnerable = "298"; // TO FIND
                _offsetSavefileHealth = "1040";
                _offsetGadgetBinds = "1390";
                _offsetUndetectable = "13C0";
                DAG.OffsetAttributes = "C4";
                DAG.OffsetAttributesForCluster = "AC";
                DAG.OffsetVerticalLayer = "FC";

                ReloadAddress = "3FBF60";
                ReloadValuesPointer = "3FE6B0";
                FKXListCountAddress = "3FC244";
                ClockAddress = "2F9A28";
                CoinsAddress = "3EF6C0";
                GadgetAddress = "3EF6B8";
                DrawDistanceAddress = "2F9AAC";
                FOVAddress = "2F9AB4";
                ResetCameraAddress = "2F9D90";
                CanCameraNoclipAddress = "2C5CE8";
                MapIdAddress = "3EF628";
                GuardAIAddress = "3FC0F0";
                ActiveCharacterPointer = "3FC23C";
                ActiveCharacterIdAddress = "3EF62C";
                ActiveCharacterHealthAddress = $"{ActiveCharacterPointer},{_offsetSavefileHealth},0";
                StringTableCountAddress = "3FE560";
                IsLoadingAddress = "3EF600";
                EntranceRootNodePointer = "3FB634";
                DAG.RootNodePointer = "3FBAB4";
                DAG.CurrentCheckpointNodePointer = "3FBF34";
                DAG.ClusterIdAddress = "2FA688";
                Savefile.SavefileStartAddress = "3EF620";
                Savefile.SavefileAddressTablePointer = "3FBE3C";
                Savefile.SavefileStringTablePointer = "3FBF18";
                ControllerAddress = "2FC9F4";
                DialoguePointer = "3F81F0";
                SkipFMVPointer = "312410";

                // Remove ep8, vault room and dvd menu
                Maps.RemoveRange(38, 6);
                Maps.RemoveRange(16, 1);
                Maps.RemoveRange(1, 1);

                Maps.Insert(0, new("Splash", new(), false));
                Maps.Insert(12, new($"{SubMapNamePrefix}i_palace_heist", new(), false));
                Maps.Insert(15, new($"{SubMapNamePrefix}i_temple_heist", new(), false));
                Maps.Insert(18, new($"{SubMapNamePrefix}p_prison_heist", new(), false));

                for (int i = 0; i < 3; i++)
                {
                    Gadgets[i].Insert(1, new(_gadgetNames[GADGET_NAME.ClueFinder], 0x6));
                }
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCAugust9])
            {
                // SCUS-97316 - DB54798A
                ReloadAddress = "3E6A80";
                ReloadValuesPointer = "3E7640";
                FKXListCountAddress = "3E6D94";
                ClockAddress = "2E38D8";
                CoinsAddress = "3DA500";
                GadgetAddress = "3DA4F8";
                DrawDistanceAddress = "2E395C";
                FOVAddress = "2E3964";
                ResetCameraAddress = "2E3C40";
                CanCameraNoclipAddress = "2C9AE8";
                MapIdAddress = "3E6B10";
                GuardAIAddress = "3E6C14";
                ActiveCharacterPointer = "3E6D8C";
                ActiveCharacterIdAddress = "3DA46C";
                ActiveCharacterHealthAddress = $"{ActiveCharacterPointer},{_offsetSavefileHealth},0";
                StringTableCountAddress = "3E74D0";
                IsLoadingAddress = "3D9380";
                EntranceRootNodePointer = "3E60C0";
                DAG.RootNodePointer = "3E6504";
                DAG.CurrentCheckpointNodePointer = "3E69A4";
                DAG.ClusterIdAddress = "2E4540";
                Savefile.SavefileStartAddress = "3DA460";
                Savefile.SavefileAddressTablePointer = "3E68AC";
                Savefile.SavefileStringTablePointer = "3E6988";
                ControllerAddress = "2E66B4";
                DialoguePointer = "3E2C40";
                SkipFMVPointer = "2FC208";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.PALAugust2])
            {
                // SCES-52529 - 4BE9708A
                _offsetRadTarget = "1B8";
                _offsetDeltaTranslation = "250";
                _offsetInvulnerable = "2A8";
                _offsetInfiniteDbJump = "2F8";
                _offsetSpeedMultiplier = "308";
                _offsetSavefileHealth = "E10";
                _offsetGadgetBinds = "1190";
                _offsetUndetectable = "11BC";

                ReloadAddress = "3F53E8";
                ReloadValuesPointer = "3F5F90";
                FKXListCountAddress = "3F56D4";
                ClockAddress = "2F3258";
                CoinsAddress = "3E8E2C";
                GadgetAddress = "3E8E24";
                DrawDistanceAddress = "2F32DC";
                FOVAddress = "2F32E4";
                ResetCameraAddress = "2F35C0";
                CanCameraNoclipAddress = "2D7A88";
                MapIdAddress = "3F5478";
                GuardAIAddress = "3F557C";
                ActiveCharacterPointer = "3F56CC";
                ActiveCharacterIdAddress = "3E8D9C";
                ActiveCharacterHealthAddress = $"{ActiveCharacterPointer},{_offsetSavefileHealth},0";
                StringTableCountAddress = "3F5E40";
                IsLoadingAddress = "3E8D00";
                EntranceRootNodePointer = "3F49C0";
                DAG.RootNodePointer = "3F4E7C";
                DAG.CurrentCheckpointNodePointer = "3F5324";
                DAG.ClusterIdAddress = "2F3EB0";
                Savefile.SavefileStartAddress = "3E8D90";
                Savefile.SavefileAddressTablePointer = "3F522C";
                Savefile.SavefileStringTablePointer = "3F5308";
                ControllerAddress = "2F5FF4";
                DialoguePointer = "3F1570";
                SkipFMVPointer = "30BB28";

                // Remove ep8
                Maps.RemoveRange(38, 6);

                Maps.Insert(12, new($"{SubMapNamePrefix}i_palace_heist", new(), false));
                Maps.Insert(15, new($"{SubMapNamePrefix}i_temple_heist", new(), false));
                Maps.Insert(18, new($"{SubMapNamePrefix}p_prison_heist", new(), false));
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCPS3PSN]
                  || build.Region == Util.BuildRegions[BUILD_NAME.NTSCKPS3PSN])
            {
                _encoding = Encoding.BigEndianUnicode;
                _offsetTransformationOrigin = "44";
                _offsetCollision = "DC";
                _offsetRadTarget = "188";
                _offsetDeltaTranslation = "220";
                _offsetInvulnerable = "278";
                _offsetInfiniteDbJump = "2C8";
                _offsetSpeedMultiplier = "2D8";
                _offsetSavefileHealth = "DE0";
                _offsetGadgetBinds = "1160";
                _offsetUndetectable = "118C";
                _offsetCurrentDialogue = "54";
                _offsetDialogueFlags = "51";
                _offsetEntranceTransformation = "50";
                DAG.OffsetState = "44";
                DAG.OffsetGoalDescription = "4C";
                DAG.OffsetFocusCount = "54";
                DAG.OffsetCompleteCount = "58";
                DAG.OffsetMissionName = "5C";
                DAG.OffsetMissionDescription = "60";
                DAG.OffsetClusterPointer = "6C";
                DAG.OffsetChildrenCount = "90";
                DAG.OffsetCheckpointEntranceValue = "A8";
                DAG.OffsetAttributes = "B8";
                DAG.OffsetAttributesForCluster = "C0";
                DAG.OffsetVerticalLayer = "F0";

                ReloadAddress = "7B4C50";
                ReloadValuesPointer = "7B57F0";
                FKXListCountAddress = "7B4F64";
                ClockAddress = "49DF80";
                CoinsAddress = "7A83B0";
                GadgetAddress = "7A83A8";
                DrawDistanceAddress = "49E04C";
                FOVAddress = "49E054";
                ResetCameraAddress = "49E330";
                CanCameraNoclipAddress = "815D18";
                MapIdAddress = "7B4CE0";
                GuardAIAddress = "7B4DE4";
                ActiveCharacterPointer = "7B4F5C";
                ActiveCharacterIdAddress = "7A830C";
                ActiveCharacterHealthAddress = $"{ActiveCharacterPointer},{_offsetSavefileHealth},0";
                StringTableCountAddress = "7B56A0";
                IsLoadingAddress = "7A7200";
                EntranceRootNodePointer = "7B4080";
                DAG.RootNodePointer = "7B44C4";
                DAG.CurrentCheckpointNodePointer = "7B4B64";
                DAG.ClusterIdAddress = "4FEBF4";
                Savefile.SavefileStartAddress = "7A8300";
                Savefile.SavefileAddressTablePointer = "7B4A6C";
                Savefile.SavefileStringTablePointer = "7B4B48";
                ControllerAddress = "500F76";
                DialoguePointer = "7B0BD0";
                SkipFMVPointer = "9066FC";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.PALPS3PSN])
            {
                _encoding = Encoding.BigEndianUnicode;
                _offsetTransformationOrigin = "44";
                _offsetCollision = "DC";
                _offsetRadTarget = "188";
                _offsetDeltaTranslation = "220";
                _offsetInvulnerable = "278";
                _offsetInfiniteDbJump = "2C8";
                _offsetSpeedMultiplier = "2D8";
                _offsetSavefileHealth = "DE0";
                _offsetGadgetBinds = "1160";
                _offsetUndetectable = "118C";
                _offsetCurrentDialogue = "54";
                _offsetDialogueFlags = "51";
                _offsetEntranceTransformation = "50";
                DAG.OffsetState = "44";
                DAG.OffsetGoalDescription = "4C";
                DAG.OffsetFocusCount = "54";
                DAG.OffsetCompleteCount = "58";
                DAG.OffsetMissionName = "5C";
                DAG.OffsetMissionDescription = "60";
                DAG.OffsetClusterPointer = "6C";
                DAG.OffsetChildrenCount = "90";
                DAG.OffsetCheckpointEntranceValue = "A8";
                DAG.OffsetAttributes = "B8";
                DAG.OffsetAttributesForCluster = "C0";
                DAG.OffsetVerticalLayer = "F0";

                ReloadAddress = "7B4BD0";
                ReloadValuesPointer = "7B5770";
                FKXListCountAddress = "7B4EE4";
                ClockAddress = "49DF00";
                CoinsAddress = "7A8330";
                GadgetAddress = "7A8328";
                DrawDistanceAddress = "49DFCC";
                FOVAddress = "49DFD4";
                ResetCameraAddress = "49E2B0";
                CanCameraNoclipAddress = "815C98";
                MapIdAddress = "7B4C60";
                GuardAIAddress = "7B4D64";
                ActiveCharacterPointer = "7B4EDC";
                ActiveCharacterIdAddress = "7A828C";
                ActiveCharacterHealthAddress = $"{ActiveCharacterPointer},{_offsetSavefileHealth},0";
                StringTableCountAddress = "7B5620";
                IsLoadingAddress = "7A7180";
                EntranceRootNodePointer = "7B4000";
                DAG.RootNodePointer = "7B4444";
                DAG.CurrentCheckpointNodePointer = "7B4AE4";
                DAG.ClusterIdAddress = "4FEB74";
                Savefile.SavefileStartAddress = "7A8280";
                Savefile.SavefileAddressTablePointer = "7B49EC";
                Savefile.SavefileStringTablePointer = "7B4AC8";
                ControllerAddress = "500EF6";
                DialoguePointer = "7B0B50";
                SkipFMVPointer = "90667C";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCPS3]
                  || build.Region == Util.BuildRegions[BUILD_NAME.NTSCKPS3])
            {
                _encoding = Encoding.BigEndianUnicode;
                _offsetTransformationOrigin = "44";
                _offsetCollision = "DC";
                _offsetRadTarget = "188";
                _offsetDeltaTranslation = "220";
                _offsetInvulnerable = "278";
                _offsetInfiniteDbJump = "2C8";
                _offsetSpeedMultiplier = "2D8";
                _offsetSavefileHealth = "DE0";
                _offsetGadgetBinds = "1160";
                _offsetUndetectable = "118C";
                _offsetCurrentDialogue = "54";
                _offsetDialogueFlags = "51";
                _offsetEntranceTransformation = "50";
                DAG.OffsetState = "44";
                DAG.OffsetGoalDescription = "4C";
                DAG.OffsetFocusCount = "54";
                DAG.OffsetCompleteCount = "58";
                DAG.OffsetMissionName = "5C";
                DAG.OffsetMissionDescription = "60";
                DAG.OffsetClusterPointer = "6C";
                DAG.OffsetChildrenCount = "90";
                DAG.OffsetCheckpointEntranceValue = "A8";
                DAG.OffsetAttributes = "B8";
                DAG.OffsetAttributesForCluster = "C0";
                DAG.OffsetVerticalLayer = "F0";

                ReloadAddress = "74134C";
                ReloadValuesPointer = "741EE0";
                FKXListCountAddress = "741654";
                ClockAddress = "428A80";
                CoinsAddress = "734AAC";
                GadgetAddress = "734AA4";
                DrawDistanceAddress = "428B4C";
                FOVAddress = "428B54";
                ResetCameraAddress = "428E30";
                CanCameraNoclipAddress = "7A2418";
                MapIdAddress = "7413DC";
                GuardAIAddress = "7414E0";
                ActiveCharacterPointer = "74164C";
                ActiveCharacterIdAddress = "734A0C";
                ActiveCharacterHealthAddress = $"{ActiveCharacterPointer},{_offsetSavefileHealth},0";
                StringTableCountAddress = "741D90";
                IsLoadingAddress = "733900";
                EntranceRootNodePointer = "740780";
                DAG.RootNodePointer = "740BC4";
                DAG.CurrentCheckpointNodePointer = "741264";
                DAG.ClusterIdAddress = "4896F4";
                Savefile.SavefileStartAddress = "734A00";
                Savefile.SavefileAddressTablePointer = "74116C";
                Savefile.SavefileStringTablePointer = "741248";
                ControllerAddress = "48BA76";
                DialoguePointer = "73D2D0";
                SkipFMVPointer = "8563DC";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.PALPS3]
                  || build.Region == Util.BuildRegions[BUILD_NAME.UKPS3])
            {
                _encoding = Encoding.BigEndianUnicode;
                _offsetTransformationOrigin = "44";
                _offsetCollision = "DC";
                _offsetRadTarget = "188";
                _offsetDeltaTranslation = "220";
                _offsetInvulnerable = "278";
                _offsetInfiniteDbJump = "2C8";
                _offsetSpeedMultiplier = "2D8";
                _offsetSavefileHealth = "DE0";
                _offsetGadgetBinds = "1160";
                _offsetUndetectable = "118C";
                _offsetCurrentDialogue = "54";
                _offsetDialogueFlags = "51";
                _offsetEntranceTransformation = "50";
                DAG.OffsetState = "44";
                DAG.OffsetGoalDescription = "4C";
                DAG.OffsetFocusCount = "54";
                DAG.OffsetCompleteCount = "58";
                DAG.OffsetMissionName = "5C";
                DAG.OffsetMissionDescription = "60";
                DAG.OffsetClusterPointer = "6C";
                DAG.OffsetChildrenCount = "90";
                DAG.OffsetCheckpointEntranceValue = "A8";
                DAG.OffsetAttributes = "B8";
                DAG.OffsetAttributesForCluster = "C0";
                DAG.OffsetVerticalLayer = "F0";

                ReloadAddress = "74124C";
                ReloadValuesPointer = "741DE0";
                FKXListCountAddress = "741554";
                ClockAddress = "428980";
                CoinsAddress = "7349AC";
                GadgetAddress = "7349A4";
                DrawDistanceAddress = "428A4C";
                FOVAddress = "428A54";
                ResetCameraAddress = "428D30";
                CanCameraNoclipAddress = "7A2318";
                MapIdAddress = "7412DC";
                GuardAIAddress = "7413E0";
                ActiveCharacterPointer = "74154C";
                ActiveCharacterIdAddress = "73490C";
                ActiveCharacterHealthAddress = $"{ActiveCharacterPointer},{_offsetSavefileHealth},0";
                StringTableCountAddress = "741C90";
                IsLoadingAddress = "733800";
                EntranceRootNodePointer = "740680";
                DAG.RootNodePointer = "740AC4";
                DAG.CurrentCheckpointNodePointer = "741164";
                DAG.ClusterIdAddress = "4895F4";
                Savefile.SavefileStartAddress = "734900";
                Savefile.SavefileAddressTablePointer = "74106C";
                Savefile.SavefileStringTablePointer = "741148";
                ControllerAddress = "48B976";
                DialoguePointer = "73D1D0";
                SkipFMVPointer = "8562DC";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCJPS3])
            {
                _encoding = Encoding.BigEndianUnicode;
                _offsetTransformationOrigin = "44";
                _offsetCollision = "DC";
                _offsetRadTarget = "188";
                _offsetDeltaTranslation = "220";
                _offsetInvulnerable = "278";
                _offsetInfiniteDbJump = "2C8";
                _offsetSpeedMultiplier = "2D8";
                _offsetSavefileHealth = "DE0";
                _offsetGadgetBinds = "1160";
                _offsetUndetectable = "118C";
                _offsetCurrentDialogue = "54";
                _offsetDialogueFlags = "51";
                _offsetEntranceTransformation = "50";
                DAG.OffsetState = "44";
                DAG.OffsetGoalDescription = "4C";
                DAG.OffsetFocusCount = "54";
                DAG.OffsetCompleteCount = "58";
                DAG.OffsetMissionName = "5C";
                DAG.OffsetMissionDescription = "60";
                DAG.OffsetClusterPointer = "6C";
                DAG.OffsetChildrenCount = "90";
                DAG.OffsetCheckpointEntranceValue = "A8";
                DAG.OffsetAttributes = "B8";
                DAG.OffsetAttributesForCluster = "C0";
                DAG.OffsetVerticalLayer = "F0";

                ReloadAddress = "7412CC";
                ReloadValuesPointer = "741E60";
                FKXListCountAddress = "7415D4";
                ClockAddress = "428A00";
                CoinsAddress = "734A2C";
                GadgetAddress = "734A24";
                DrawDistanceAddress = "428ACC";
                FOVAddress = "428AD4";
                ResetCameraAddress = "428DB0";
                CanCameraNoclipAddress = "7A2398";
                MapIdAddress = "74135C";
                GuardAIAddress = "741460";
                ActiveCharacterPointer = "7415CC";
                ActiveCharacterIdAddress = "73498C";
                ActiveCharacterHealthAddress = $"{ActiveCharacterPointer},{_offsetSavefileHealth},0";
                StringTableCountAddress = "741D10";
                IsLoadingAddress = "733880";
                EntranceRootNodePointer = "740700";
                DAG.RootNodePointer = "740B44";
                DAG.CurrentCheckpointNodePointer = "7411E4";
                DAG.ClusterIdAddress = "489674";
                Savefile.SavefileStartAddress = "734980";
                Savefile.SavefileAddressTablePointer = "7410EC";
                Savefile.SavefileStringTablePointer = "7411A0";
                ControllerAddress = "48B9F6";
                DialoguePointer = "73D250";
                SkipFMVPointer = "85635C";
            }

            _offsetTransformationLocal = $"{_offsetTransformationOrigin}+4";
            _offsetTransformationWorld = $"{_offsetTransformationOrigin}+8";
            _offsetTransformationFinal = $"{_offsetTransformationOrigin}+C";
            _offsetSavefileGadgetPower = $"{_offsetSavefileHealth},4";
        }

        public override void CustomTick()
        {

        }

        public override void OnFirstLoopAfterLoading(int mapId)
        {
            Savefile.Init();
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
                gadgets = 0;
            }
            else
            {
                gadgets = -1;
            }

            WriteGadgets(gadgets);
        }

        public int ReadActCharGadgetPower()
        {
            return _m.ReadInt($"{ActiveCharacterPointer},{_offsetSavefileGadgetPower}");
        }

        public void WriteActCharGadgetPower(int value)
        {
            _m.WriteMemory($"{ActiveCharacterPointer},{_offsetSavefileGadgetPower}", "int", value.ToString());
        }

        public override void FreezeActCharGadgetPower(int value)
        {
            if (value == 0)
            {
                value = ReadActCharGadgetPower();
            }

            _m.FreezeValue($"{ActiveCharacterPointer},{_offsetSavefileGadgetPower}", "int", value.ToString());
        }

        public override void UnfreezeActCharGadgetPower()
        {
            _m.UnfreezeValue($"{ActiveCharacterPointer},{_offsetSavefileGadgetPower}");
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
            var transPointer = _m.ReadInt($"{pointerToEntity},{_offsetTransformationOrigin}");
            if (transPointer == -1
             || transPointer == 0)
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

        // This method is used for loading jobs in the dag to make it compatible with the sly 3 version
        // Sly 2 has only 1 player
        public void WriteActCharId(int id, int id2)
        {
            WriteActCharId(id);
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
            return _m.ReadInt(GetActCharHealthAddress());
        }

        public override void WriteActCharHealth(int value)
        {
            _m.WriteMemory(GetActCharHealthAddress(), "int", value.ToString());
        }

        public override void FreezeActCharHealth(int value)
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

        public override void FreezeActCharLocalTranslationZ(string value)
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

        private string GetActCharHealthAddress()
        {
            string address = ActiveCharacterHealthAddress;
            if (_m.ReadInt($"{ActiveCharacterPointer},{_offsetSavefileHealth}") == 0)
            {
                address = $"{ActiveCharacterPointer},184";
            }

            return address;
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
                _m.FreezeValue($"{ActiveCharacterPointer},{_offsetInvulnerable}", "int", "1");
            }
            else
            {
                _m.WriteMemory($"{ActiveCharacterPointer},{_offsetInvulnerable}", "int", "0");
                _m.UnfreezeValue($"{ActiveCharacterPointer},{_offsetInvulnerable}");
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

        public override void ActCharToggleNoclip(bool enableNoclip)
        {
            // The following pointer chain was chosen because it is the most stable and consistent way to remove AND regain collision.
            // - Nulling the pointer at +E4 works, but was not used because:
            //   - it makes the character warp back to their last position if the game detects they are stuck
            //   - the value is a pointer, we need to store its value in a variable to be able to restore it
            // - Nulling the pointer at +E8 was not used because:
            //   - it makes you not be able to move
            //   - it sometimes crashes the game
            //   - the value is a pointer, we need to store its value in a variable to be able to restore it
            // - Nulling the pointer at +EC was not used because:
            //   - it sometimes crashes the game
            //   - the value is a pointer, we need to store its value in a variable to be able to restore it
            // - Setting to 3 ("CT_Locked" from sly 1) the value at +E8,C0 works, but was not used because:
            //   - it makes the character warp back to their last position if the game detects they are stuck
            //   - after a while, you can't regain collision by setting it back to 0
            // - Setting to 4 the value at +EC,70 works, but was not used because:
            //   - we can't freeze it; we need the game to be able to modify that value even while we are noclipping
            //   - it sometimes disables movement
            //   - it is possible that the game is in the middle of a collision check when we modify that value back to 0, which would make the game go into an infinite loop (not a crash: the game can be recovered by setting that value to 1)
            // - Fake floor (e.g. sly 2 ntsc at 3E11AC) was not used because:
            //   - the value is a pointer, we need to store its value in a variable to be able to restore it
            //   - the value changes based on the character's position, so we can't really restore collision
            //   - it makes the character warp back to their last position if the game detects they are stuck
            //   - we only want noclip for the player

            if (enableNoclip)
            {
                string value = "4";
                if (Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCMarch17]
                 || Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCJuly11]
                 || Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCDemoRatchetClankUpYourArsenal]
                 || Build.Region == Util.BuildRegions[Util.BUILD_NAME.PALDemoRatchetClank3]
                 || Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCDemoRatchetClankUpYourArsenalAugust11]
                 || Build.Region == Util.BuildRegions[Util.BUILD_NAME.PALDemoJuly27])
                {
                    // Prevents warp back
                    value = "5";
                }
                else if (Build.Region.Contains("PS3"))
                {
                    // The bits are flipped too
                    value = "0x20";
                }

                _m.WriteMemory($"{ActiveCharacterPointer},{_offsetCollision},34,16", "byte", $"{value}");
            }
            else
            {
                _m.WriteMemory($"{ActiveCharacterPointer},{_offsetCollision},34,16", "byte", "0");
            }
        }
        #endregion

        #region Maps
        public override void LoadMap(int mapId)
        {
            if (mapId == -1)
            {
                // This is in here too (other than in the method used when clicking on the load map button)
                // because we might be coming from the DAG loading a checkpoint in NTSC March 17.
                // In NTSC March 17, the following tasks have their map id field set to -1:
                // t1_follow_intro
                // t2_bomb_bridge_intro
                // t2_bomb_bridge_lower_complete
                // t3_steal_ruby_trainer
                // t3_boss_sly_capture
                // t3_turret_intro
                // t3_free_elephant_intro
                // So, as a failsafe, we load the current map and hope that we are in the correct one (e.g. loading t1_follow_intro and we are in Paris Hub)
                // The map id field set to -1 also affects the debug menu:
                // If the player is, say, in the Wine Cellar map and loads Follow Dimitri through the debug menu, the game will load the Wine Cellar map again instead of loading Paris Hub

                // Current map
                mapId = ReadMapId();
            }

            byte[] data = _m.ReadBytes($"{ReloadValuesPointer},{mapId * 0x40:X8}", 0x40);
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

        public void ReloadMap()
        {
            _m.WriteMemory($"{ReloadAddress}+4", "int", "0"); // mode
            _m.WriteMemory(ReloadAddress, "int", "1");
        }
        #endregion

        public override void SkipCurrentDialogue()
        {
            // NOTE: This code is repeated in Sly 3 too
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

            if (Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCMarch17])
            {
                // Handled at 001FD578
                // The code returns 0 if DialoguePointer has a value. If we want to skip the current voice line we need the code to return 1
                // To make the code return 1, we do the following:
                _m.WriteMemory($"{DialoguePointer}", "int", "0");
                _m.WriteMemory($"410FB0", "int", "-1"); // Current voice line id
                _m.WriteMemory($"{objVoiceover:X}+50", "byte", "8");
                return;
            }

            // Don't do anything if the voice line was triggered from splice (e.g. sly 2 ep1 follow dimitri, while dimitri is walking)
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
            // This might not be needed for Sly 2, but let's do it anyway
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
            int address = _m.ReadInt($"{StringTableCountAddress}+4");
            int count = _m.ReadInt($"{StringTableCountAddress}");
            List<(int id, string str)> table = new(count);
            for (int i = 0; i < count; i++)
            {
                int stringId = _m.ReadInt($"{address + i * 8:X}");
                int stringPointer = _m.ReadInt($"{address + i * 8 + 4:X}");
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
                new("Sly", 7, "jt", "sly"),
                new("Bentley", 8, "bentley", "bentley"),
                new("Murray", 9, "murray", "murray"),
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
            StealthSlide,
            AlarmClock,
            Paraglide,
            SilentObliteration,
            ThiefReflexes,
            FeralPounce,
            MegaJump,
            TornadoStrike,
            KnockoutDive,
            InsanityStrike,
            VoltageAttack,
            RageBomb,
            MusicBox,
            LightningSpin,
            ShadowPower,
            TOM,
            TimeRush,

            TriggerBomb,
            SizeDestabilizer,
            SnoozeBomb,
            AdrenalineBurst,
            HealthExtractor,
            HoverPack,
            ReductionBomb,
            TemporalLock,
            LongToss,

            FistsOfFlame,
            TurnbuckleLaunch,
            JuggernautThrow,
            AtlasStrength,
            DiabloFireSlam,
            BerserkerCharge,
            GutturalRoar,
            RagingInfernoFlop,

            Uppercut,
            ClueFinder,
        }

        private static Dictionary<GADGET_NAME, string> _gadgetNames = new()
        {
            [GADGET_NAME.SmokeBomb] = "Smoke Bomb",
            [GADGET_NAME.CombatDodge] = "Combat Dodge",
            [GADGET_NAME.StealthSlide] = "Stealth Slide",
            [GADGET_NAME.AlarmClock] = "Alarm Clock",
            [GADGET_NAME.Paraglide] = "Paraglide",
            [GADGET_NAME.SilentObliteration] = "Silent Obliteration",
            [GADGET_NAME.ThiefReflexes] = "Thief Reflexes",
            [GADGET_NAME.FeralPounce] = "Feral Pounce",
            [GADGET_NAME.MegaJump] = "Mega Jump",
            [GADGET_NAME.TornadoStrike] = "Tornado Strike",
            [GADGET_NAME.KnockoutDive] = "Knockout Dive",
            [GADGET_NAME.InsanityStrike] = "Insanity Strike",
            [GADGET_NAME.VoltageAttack] = "Voltage Attack",
            [GADGET_NAME.RageBomb] = "Rage Bomb",
            [GADGET_NAME.MusicBox] = "Music Box",
            [GADGET_NAME.LightningSpin] = "Lightning Spin",
            [GADGET_NAME.ShadowPower] = "Shadow Power",
            [GADGET_NAME.TOM] = "TOM",
            [GADGET_NAME.TimeRush] = "Time Rush",

            [GADGET_NAME.TriggerBomb] = "Trigger Bomb",
            [GADGET_NAME.SizeDestabilizer] = "Size Destabilizer",
            [GADGET_NAME.SnoozeBomb] = "Snooze Bomb",
            [GADGET_NAME.AdrenalineBurst] = "Adrenaline Burst",
            [GADGET_NAME.HealthExtractor] = "Health Extractor",
            [GADGET_NAME.HoverPack] = "Hover Pack",
            [GADGET_NAME.ReductionBomb] = "Reduction Bomb",
            [GADGET_NAME.TemporalLock] = "Temporal Lock",
            [GADGET_NAME.LongToss] = "Long Toss",

            [GADGET_NAME.FistsOfFlame] = "Fists of Flame",
            [GADGET_NAME.TurnbuckleLaunch] = "Turnbuckle Launch",
            [GADGET_NAME.JuggernautThrow] = "Juggernaut Throw",
            [GADGET_NAME.AtlasStrength] = "Atlas Strength",
            [GADGET_NAME.DiabloFireSlam] = "Diablo Fire Slam",
            [GADGET_NAME.BerserkerCharge] = "Berserker Charge",
            [GADGET_NAME.GutturalRoar] = "Guttural Roar",
            [GADGET_NAME.RagingInfernoFlop] = "Raging Inferno Flop",

            [GADGET_NAME.Uppercut] = "Uppercut",
            [GADGET_NAME.ClueFinder] = "Clue Finder",
        };

        protected override List<List<Gadget_t>> GetGadgets()
        {
            return new()
            {
                new()
                {
                    new(_gadgetNames[GADGET_NAME.SmokeBomb], 0x17),
                    new(_gadgetNames[GADGET_NAME.CombatDodge], 0x18),
                    new(_gadgetNames[GADGET_NAME.StealthSlide], 0x19),
                    new(_gadgetNames[GADGET_NAME.AlarmClock], 0x1A),
                    new(_gadgetNames[GADGET_NAME.Paraglide], 0x1B),
                    new(_gadgetNames[GADGET_NAME.SilentObliteration], 0x1C),
                    new(_gadgetNames[GADGET_NAME.ThiefReflexes], 0x1D),
                    new(_gadgetNames[GADGET_NAME.FeralPounce], 0x1E),
                    new(_gadgetNames[GADGET_NAME.MegaJump], 0x1F),
                    new(_gadgetNames[GADGET_NAME.TornadoStrike], 0x20),
                    new(_gadgetNames[GADGET_NAME.KnockoutDive], 0x21),
                    new(_gadgetNames[GADGET_NAME.InsanityStrike], 0x22),
                    new(_gadgetNames[GADGET_NAME.VoltageAttack], 0x23),
                    new(_gadgetNames[GADGET_NAME.RageBomb], 0x25),
                    new(_gadgetNames[GADGET_NAME.MusicBox], 0x26),
                    new(_gadgetNames[GADGET_NAME.LightningSpin], 0x27),
                    new(_gadgetNames[GADGET_NAME.ShadowPower], 0x28),
                    new(_gadgetNames[GADGET_NAME.TOM], 0x29),
                    new(_gadgetNames[GADGET_NAME.TimeRush], 0x2A),
                },

                new()
                {
                    new(_gadgetNames[GADGET_NAME.TriggerBomb], 0x7),
                    new(_gadgetNames[GADGET_NAME.SizeDestabilizer], 0x8),
                    new(_gadgetNames[GADGET_NAME.SnoozeBomb], 0x9),
                    new(_gadgetNames[GADGET_NAME.AdrenalineBurst], 0xA),
                    new(_gadgetNames[GADGET_NAME.HealthExtractor], 0xB),
                    new(_gadgetNames[GADGET_NAME.HoverPack], 0xC),
                    new(_gadgetNames[GADGET_NAME.ReductionBomb], 0xD),
                    new(_gadgetNames[GADGET_NAME.TemporalLock], 0xE),
                    new(_gadgetNames[GADGET_NAME.LongToss], 0x24),
                },

                new()
                {
                    new(_gadgetNames[GADGET_NAME.FistsOfFlame], 0xF),
                    new(_gadgetNames[GADGET_NAME.TurnbuckleLaunch], 0x10),
                    new(_gadgetNames[GADGET_NAME.JuggernautThrow], 0x11),
                    new(_gadgetNames[GADGET_NAME.AtlasStrength], 0x12),
                    new(_gadgetNames[GADGET_NAME.DiabloFireSlam], 0x13),
                    new(_gadgetNames[GADGET_NAME.BerserkerCharge], 0x14),
                    new(_gadgetNames[GADGET_NAME.GutturalRoar], 0x15),
                    new(_gadgetNames[GADGET_NAME.RagingInfernoFlop], 0x16),
                },
            };
        }

        protected override List<Map_t> GetMaps()
        {
            return new()
            {
                new("Cairo",
                    new()
                    {
                        new("Museum", 4910, -5210, 580),
                        new("Computer", 5400, -700, 1100),
                        new("Balcony 1", 11560, -1050, 1110),
                        new("Balcony 2", -12870, 15500, 1370),
                        new("Murray rendezvous", 18790, 80, 1500),
                        new("Warehouse", 12500, 5790, 1600),
                        new("Chase start", 10000, 8150, 1860),
                        new("Pickup point", -26400, 4350, 80),
                        new("End trigger", -27331, -49, 924),
                    }
                ),
                new("DVD menu",
                    new()
                    {
                        new(),
                    }
                ),
                new("Paris hub",
                    new()
                    {
                        new("Safehouse", -1800, -4100, 535),
                        new("Safehouse (top)", -2740, -3640, 1245),
                        new("Dimitri's boat", -7090, -6320, -30),
                        new("Satellite dish 1", -6000, 4700, 1100),
                        new("Satellite dish 2", -5000, -4800, 1300),
                        new("Satellite dish 3", 6100, -7500, 1300),
                        new("Nightclub (door)", -1645, 5655, 60),
                        new("Nightclub (window)", -3455, 5510, 1100),
                        new("Nightclub (top)", 0, 0, 5000),
                        new("Courtyard", 1860, 5630, -50),
                        new("Clock tower", 8600, 2080, 2070),
                        new("Hotel", 4430, -8000, 2420),
                        new("Water tower (inside)", 0, 0, 20100),
                    }
                ),
                new($"{SubMapNamePrefix}Wine cellar",
                    new()
                    {
                        new("Door", 14500, -6700, 470),
                        new("Lasers", 10340, -3900, 60),
                        new("Office", 5800, -2900, -150),
                        new("Music room", 95, 2850, 125),
                    }
                ),
                new($"{SubMapNamePrefix}Nightclub",
                    new()
                    {
                        new("Door", -2670, 5880, -540),
                        new("Window", -7780, 1440, 810),
                        new("Dance floor", -7030, 8845, -1000),
                        new("Dimitri's office", -3800, 6460, 200),

                    }
                ),
                new($"{SubMapNamePrefix}Print room",
                    new()
                    {
                        new("Recon", -1100, 4180, 1470),
                        new("Bottom floor", 0, 0, -50),
                        new("Money printer", 0, 900, 740),
                        new("Top floor", 0, 1800, 1570),
                    }
                ),
                new($"{SubMapNamePrefix}Theater",
                    new()
                    {
                        new("Door", -40, 4220, 910),
                        new("Fan control", 7000, 5110, 730),
                        new("TV", 3800, 8490, 895),
                        new("Spotlight control", 2560, 5820, 1560),
                    }
                ),
                new($"{SubMapNamePrefix}Water pump room",
                    new()
                    {
                        new("Door", -13060, 6580, -170),
                        new("Fireplace", -9670, 2470, -540),
                        new("Water pump", -5560, 3850, 130),
                    }
                ),
                new("India 1 hub",
                    new()
                    {
                        new("Safehouse", -4600, 2180, 460),
                        new("Safehouse (inside)", -6670, -1470, 2270),
                        new("Palace (door)", 10000, 2100, 1690),
                        new("Guesthouse (top)", 3160, -10420, 1770),
                        new("Cobra statue", 6950, 8680, 1770),
                        new("Drain pipe", 14000, 2600, 837),
                        new("Drawbridge control", 200, 1980, 960),
                    }
                ),
                new($"{SubMapNamePrefix}Hotel",
                    new()
                    {
                        new("Door", -80, 1800, -780),
                        new("Room 101", -6400, -1700, 100),
                        new("Room 102", -3300, 120, -170),
                        new("Room 103", -110, -1540, -480),
                        new("Room 104", 3320, 50, -170),
                        new("Room 105", 6540, -1880, 110),
                    }
                ),
                new($"{SubMapNamePrefix}Basement",
                    new()
                    {
                        new("Tube", 2470, -1850, 40),
                        new("Vault", 2640, 0, 390),
                        new("Boardroom", 1360, 0, 1170),
                    }
                ),
                new($"{SubMapNamePrefix}Ballroom",
                    new()
                    {
                        new("Door", 1160, 5380, 910),
                        new("Dance floor", 1460, 2600, 70),
                        new("Guests (left)", 3200, -1000, 1400),
                        new("Guests (right)", -500, -800, 1400),
                    }
                ),
                new("India 2 hub",
                    new()
                    {
                        new("Safehouse", -2930, -5540, 1980),
                        new("Safehouse (top)", -2700, -5600, 3070),
                        new("Watermill ", 2500, -700, 1300),
                        new("Tilting temple", -9100, -1600, 2500),
                        new("Main temple", 0, 3400, 2040),
                        new("Waterfall", -9040, 7430, 0),
                        new("Dam", 1390, 8200, 8010),
                    }
                ),
                new($"{SubMapNamePrefix}Spice factory",
                    new()
                    {
                        new("Bottom floor", -10200, 9100, -30),
                        new("Top floor", -10000, 9880, 1600),
                        new("Recon area", -14160, 12000, 2200),
                        new("Spice grinder hole", -5700, 1760, -1290),
                        new("Spice grinder", -8200, -2560, -850),
                        new("Rajan's office", 5100, 11260, 2200),
                    }
                ),
                new("Prague 1 hub",
                    new()
                    {
                        new("Safehouse", -7100, -10500, 235),
                        new("Safehouse (top)", -7490, -11010, 1600),
                        new("Bridge", 6120, -5800, 730),
                        new("Prison (center)", -1430, 1120, 3690),
                        new("Prison (Sly)", 1930, -4480, 1960),
                        new("Rooftop 1", -9060, 310, 1295),
                        new("Rooftop 2", 2370, -12450, 850),
                        new("The Contessa's house", 6840, -590, 1850),
                    }
                ),
                new($"{SubMapNamePrefix}Prison",
                    new()
                    {
                        new("Door", 540, -4040, 500),
                        new("Murray's cell", 7700, -2100, -1130),
                        new("Hypno arena", -3660, 1950, -320),
                        new("Control room", 900, -1800, 300),
                    }
                ),
                new($"{SubMapNamePrefix}Vault room",
                    new()
                    {
                        new("Crawlspace", -280, 390, 20),
                        new("Behind the wall", 1330, -1000, 490),
                    }
                ),
                new("Prague 2 hub",
                    new()
                    {
                        new("Safehouse", 11560, 2350, 800),
                        new("Sewer", 7980, 2270, -610),
                        new("Graveyard", -2150, -7300, 420),
                        new("Castle main door", 0, 0, 150),
                        new("Castle back door", -4700, -400, -300),
                        new("Castle top 1", -6690, -2720, 1300),
                        new("Castle top 2", -800, 4680, 2150),
                        new("Castle top 3", -3980, 1920, 4530),
                        new("Guillotine", -6445, 4333, 180),
                        new("Re-education tower (door)", -1025, -4400, 3330),
                        new("Re-education tower (balcony)", -175, -4485, 4900),
                    }
                ),
                new("p_castle_int", // used to skip map id 18
                    new(),
                    false
                ),
                new($"{SubMapNamePrefix}Crypt 3 (Stealing Voices)",
                    new()
                    {
                        new("Door", -25200, -180, 75),
                        new("End", -25220, -9320, 190),
                    }
                ),
                new($"{SubMapNamePrefix}Crypts 1 & 2 (Stealing Voices)",
                    new()
                    {
                        new("Door (crypt 1)", -16570, -11490, -380),
                        new("Vault (crypt 1)", -12270, -11550, -210),
                        new("Door (crypt 2)", -14750, 560, 540),
                        new("End (crypt 2)", -19480, 5450, 1180),
                    }
                ),
                new($"{SubMapNamePrefix}Crypt 4 (Ghost Capture)",
                    new()
                    {
                        new("Door", 3530, -11640, -380),
                        new("Tomb", 3040, -6300, -1180),
                    }
                ),
                new($"{SubMapNamePrefix}Re-education tower & Hacking Crypt",
                    new()
                    {
                        new("Door (Re-education Tower)", -16020, -11490, -40),
                        new("Re-education cell", -14300, -11720, -340),
                        new("Door (hack)", -6690, -4700, 470),
                        new("End (hack)", -1000, 8300, -380),
                        new("Unused area 1", 9050, -2630, 100),
                        new("Unused area 2", 10200, -7430, -400),
                        new("Unused area 3", 13330, -7390, -380),
                    }
                ),
                new($"{SubMapNamePrefix}Crypt 1 (Mojo Trap Action)",
                    new()
                    {
                        new("Door", -16197, -11483, -307),
                    }
                ),
                new($"{SubMapNamePrefix}Crypt 3 (Mojo Trap Action)",
                    new()
                    {
                        new("Door", 4852, -6280, -1303),
                    }
                ),
                new($"{SubMapNamePrefix}Crypt 2 (Mojo Trap Action)",
                    new()
                    {
                        new("Door", 9069, -2627, 118),
                    }
                ),
                new($"{SubMapNamePrefix}Crypt 4 (Mojo Trap Action)",
                    new()
                    {
                        new("Door", -25210, -5428, 133),
                    }
                ),
                new("Canada hub",
                    new()
                    {
                        new("Safehouse", -1060, -11040, 30),
                        new("Safehouse (top)", -50, -10560, 1440),
                        new("Cabin 1 (Jean Bison)", -3940, 6870, 2030),
                        new("Cabin 2", -12220, -3630, 1960),
                        new("Cabin 3", 5960, 6275, 890),
                        new("Satellite dish", 660, 5330, 4740),
                        new("Plane", -1840, 9260, 20),
                    }
                ),
                new($"{SubMapNamePrefix}Cabins",
                    new()
                    {
                        new("Cabin 1", -8820, -8470, 130),
                        new("Cabin 2", 8370, -8500, 130),
                        new("Cabin 3", 8370, 6260, 130),
                    }
                ),
                new($"{SubMapNamePrefix}Train (Aerial Assault / Theft on the Rails)",
                    new()
                    {
                        new("Back", 0, -8400, 120),
                        new("Front", 40, 21300, 120),
                    }
                ),
                new($"{SubMapNamePrefix}Train (Operation)",
                    new()
                    {
                        new("Back", 0, -8400, 120),
                        new("Front", 0, 25100, 120),
                        new("Jean Bison", 0, 2380, 120),
                    }
                ),
                new($"{SubMapNamePrefix}Train (Ride the Iron Horse)",
                    new()
                    {
                        new("Back", 0, -8400, 120),
                        new("Front", 0, 25100, 120),
                    }
                ),
                new("Canada 2 hub",
                    new()
                    {
                        new("Safehouse", 2290, -3380, 560),
                        new("Van", 8420, -970, -810),
                        new("Sawmill 1", 520, 7270, 1825),
                        new("Sawmill 2 (Laser Redirection)", -5140, 7200, 1470),
                        new("Sawmill 3 (RC Combat Club)", -5700, -6390, 1480),
                        new("Bomb fishing spot", -4670, -1190, 920),
                        new("Battery silo", -9480, -3050, 1490),
                        new("Lighthouse", -12750, -3500, -330),
                        new("Lighthouse (top)", -13700, -3850, 4060),
                    }
                ),
                new($"{SubMapNamePrefix}Mulch mill",
                    new()
                    {
                        new("Moose head", 19390, -1080, 1720),
                        new("Sawblade crawl", 22040, -2520, 1470),
                    }
                ),
                new($"{SubMapNamePrefix}Sawmill",
                    new()
                    {
                        new("Ladder", -1400, 22123, 1200),
                        new("Vault", 760, 20930, 880),
                        new("Lasers", -268, 20348, 1828),
                        new("Lever", 690, 22320, 1190),
                    }
                ),
                new($"{SubMapNamePrefix}Lighthouse",
                    new()
                    {
                        new("Ladder", -290, 375, 5845),
                        new("Bottom", 0, 0, 960),
                        new("Recon", 680, 1060, 1015),
                    }
                ),
                new($"{SubMapNamePrefix}Bear cave",
                    new()
                    {
                        new("Crawlspace", -2525, 2960, 30),
                        new("Large ice wall", 1000, 5255, 165),
                    }
                ),
                new($"{SubMapNamePrefix}Sawmill (boss)",
                    new()
                    {
                        new("Arena", 1140, -520, 75),
                        new("Control room", 620, 840, 1800),
                    }
                ),
                new("Blimp hub",
                    new()
                    {
                        new("Safehouse", 11400, -25, 960),
                        new("Safehouse (top)", 10840, -485, 3100),
                        new("Balloon 1", 6540, -2710, 2360),
                        new("Balloon 2", 6480, 2690, 2360),
                        new("Engine 1", -10220, 4055, 3960),
                        new("Engine 2", -10270, -4060, 3960),
                        new("Center", 0, 0, 2810),
                    }
                ),
                new($"{SubMapNamePrefix}Blimp HQ",
                    new()
                    {
                        new("Crawlspace", -3890, 0, 730),
                        new("Clockwerk", 135, 0, 360),
                        new("Neyla", 5160, 0, 100),
                        new("Center", 0, 0, -670),
                    }
                ),
                new($"{SubMapNamePrefix}Engine room 1 (Bentley/Murray)",
                    new()
                    {
                        new("Tube", -2190, 130, 10),
                        new("Half", 2300, 0, 100),
                        new("Control room", -2250, -20, 710),
                    }
                ),
                new($"{SubMapNamePrefix}Engine room 2 (Sly/Bentley)",
                    new()
                    {
                        new("Tube", -2140, 140, 10),
                        new("Half", 2300, 0, 100),
                        new("Control room", -2250, -20, 710),
                    }
                ),
                new($"{SubMapNamePrefix}Engine room 3 (Murray/Sly)",
                    new()
                    {
                        new("Tube", -2110, 430, 10),
                        new("Half", 2300, 0, 100),
                        new("Control room", -2250, -85, 710),
                    }
                ),
                new($"{SubMapNamePrefix}Paris (Clock-La)",
                    new()
                    {
                        new("Spawn (sky)", -12560, 580, 86720),
                        new("Clock-La (sky)", 22803, -1340, 76170),
                        new("Ground level", -100, -666, -130),
                        new("Destroyed walkway", 920, -7480, 1580),
                    }
                )
            };
        }
    }
}
