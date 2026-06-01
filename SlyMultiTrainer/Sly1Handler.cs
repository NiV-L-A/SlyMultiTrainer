using System.Numerics;
using static SlyMultiTrainer.Util;

namespace SlyMultiTrainer
{
    public class Sly1Handler : GameBase_t
    {
        private Memory.Mem _m;
        private Form1 _form;

        public string GameStatePointer = "";
        public string ReloadAddress = "";
        public string ReloadValuesAddress = "";
        public int ReloadValuesStructSize; // 0x20 for each language + 0xC. Ntsc has 1, PAL has 5 (english, french, german, spanish, italian), jap has 2
        public string IsLoadingAddress = "";
        public string LanguageAddress = "";
        public string LivesAddress = "";
        public string LuckyCharmsAddress = "";
        public string CameraPointer = "";
        public string WorldIdAddress = "";
        public string SlyEntityPointer = "";
        public string ActiveCharacterVehiclePointer = "";
        public string ActiveCharacterPointer = "";
        public string EntranceRootNodePointer = "";
        public string TreasureInTheDepthsChestCountPointer = "";
        public string RaceLapsCountPointer = "";
        public string RaceNitrosCountPointer = "";
        public string PiranhaLakeFishCountPointer = "";
        public string PiranhaLakeTorchDownHomeCookingChickenCountPointer = "";
        public string BurningRubberFireSlugsComputerCountPointer = "";
        public string BurningRubberComputerCountPointer = "";
        public string BentleyComesThroughChipCountPointer = "";

        private string _offsetTransformation = "D0";
        private string _offsetVelocity = "";
        private string _offsetRadTarget = "638";
        private string _offsetCollision = "53F";
        private string _offsetSpeedMultiplier = "226C";
        private int _worldStateStructSize = 0x44C;
        private int _levelStateStructSize = 0x78;
        private int _offsetSafesOpened = 0x4;
        private int _offsetSprintsCompleted = 0x8;
        private int _offsetTimePlayed = 0xC;
        private int _offsetWorldFlag = 0x10;
        private const int _levelCount = 9;
        private string[] _tabWorldStateHeaderLabels = { "Levels", "Unlocked", "Key", "Safe", "Sprint" };

        public Sly1Handler(Form1 form, Memory.Mem m, Build_t build) : base(form, m, build)
        {
            _m = m;
            _form = form;

            if (build.Region == Util.BuildRegions[BUILD_NAME.NTSC]) 
            {
                // SCUS-97198 - C77AF2CA
                GameStatePointer = "2623C0";
                WorldIdAddress = $"{GameStatePointer},19D8";
                MapIdAddress = $"{GameStatePointer},19DC";
                LivesAddress = $"{GameStatePointer},19E0";
                LuckyCharmsAddress = $"{GameStatePointer},19E4";
                CoinsAddress = $"{GameStatePointer},19E8";
                GadgetAddress = $"{GameStatePointer},19F0";
                ReloadAddress = "275F84";
                ReloadValuesAddress = "247AF0";
                ReloadValuesStructSize = 0x2C;
                IsLoadingAddress = $"{ReloadAddress}";
                LanguageAddress = "";
                ClockAddress = "261854";
                CameraPointer = "261990";
                DrawDistanceAddress = $"{CameraPointer},B0";
                FOVAddress = $"{CameraPointer},1C8";
                ResetCameraAddress = $"{CameraPointer},220";
                CanCameraNoclipAddress = "275978";
                ControllerAddress = "262D18";
                DialoguePointer = "27051C";
                SlyEntityPointer = "262E10";
                ActiveCharacterVehiclePointer = "269C98";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                EntranceRootNodePointer = "275710,1A54";
                SkipFMVPointer = "269A50";
                TreasureInTheDepthsChestCountPointer = "26D32C";
                RaceLapsCountPointer = "26D82C";
                RaceNitrosCountPointer = "26DAAC";
                PiranhaLakeFishCountPointer = "26E744";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "26D5AC";
                BurningRubberFireSlugsComputerCountPointer = "272914";
                BurningRubberComputerCountPointer = "272694";
                BentleyComesThroughChipCountPointer = "26DFAC";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.PAL])
            {
                // SCES-50917 - DA3DD765
                GameStatePointer = "2636F0";
                WorldIdAddress = $"{GameStatePointer},19D8";
                MapIdAddress = $"{GameStatePointer},19DC";
                LivesAddress = $"{GameStatePointer},19E0";
                LuckyCharmsAddress = $"{GameStatePointer},19E4";
                CoinsAddress = $"{GameStatePointer},19E8";
                GadgetAddress = $"{GameStatePointer},19F0";
                ReloadAddress = "27F704";
                ReloadValuesAddress = "2490F0";
                ReloadValuesStructSize = 0xAC;
                IsLoadingAddress = $"{ReloadAddress}";
                LanguageAddress = "263710";
                ClockAddress = "262B54";
                CameraPointer = "262C90";
                DrawDistanceAddress = $"{CameraPointer},B0";
                FOVAddress = $"{CameraPointer},1C8";
                ResetCameraAddress = $"{CameraPointer},220";
                CanCameraNoclipAddress = "27F0E8";
                ControllerAddress = "263F80";
                DialoguePointer = "276448";
                SlyEntityPointer = "264070";
                ActiveCharacterVehiclePointer = "26AF08";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                EntranceRootNodePointer = "27EE80,1A54";
                SkipFMVPointer = "26ACB0";
                TreasureInTheDepthsChestCountPointer = "26F6AC";
                RaceLapsCountPointer = "2701CC";
                RaceNitrosCountPointer = "27075C";
                PiranhaLakeFishCountPointer = "272344";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "26FC3C";
                BurningRubberFireSlugsComputerCountPointer = "279EC4";
                BurningRubberComputerCountPointer = "279934";
                BentleyComesThroughChipCountPointer = "27127C";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCJ])
            {
                // SCPS-15036 - 15C88C7B
                GameStatePointer = "262964";
                WorldIdAddress = $"{GameStatePointer},19D8";
                MapIdAddress = $"{GameStatePointer},19DC";
                LivesAddress = $"{GameStatePointer},19E0";
                LuckyCharmsAddress = $"{GameStatePointer},19E4";
                CoinsAddress = $"{GameStatePointer},19E8";
                GadgetAddress = $"{GameStatePointer},19F0";
                ReloadAddress = "27E584";
                ReloadValuesAddress = "249330";
                ReloadValuesStructSize = 0x4C;
                IsLoadingAddress = $"{ReloadAddress}";
                LanguageAddress = "262984"; // You can't set it to english?
                ClockAddress = "261DD4";
                CameraPointer = "261F10";
                DrawDistanceAddress = $"{CameraPointer},B0";
                FOVAddress = $"{CameraPointer},1C8";
                ResetCameraAddress = $"{CameraPointer},220";
                CanCameraNoclipAddress = "27DF88";
                ControllerAddress = "263230";
                DialoguePointer = "275708";
                SlyEntityPointer = "263320";
                ActiveCharacterVehiclePointer = "26A1B8";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                EntranceRootNodePointer = "27DD20,1A54";
                SkipFMVPointer = "269F60";
                TreasureInTheDepthsChestCountPointer = "26E96C";
                RaceLapsCountPointer = "26F48C";
                RaceNitrosCountPointer = "26FA1C";
                PiranhaLakeFishCountPointer = "271604";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "26EEFC";
                BurningRubberFireSlugsComputerCountPointer = "278D64";
                BurningRubberComputerCountPointer = "2787D4";
                BentleyComesThroughChipCountPointer = "27053C";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCK])
            {
                // SCKA-20004 - 71017DE1
                GameStatePointer = "262C60";
                WorldIdAddress = $"{GameStatePointer},19D8";
                MapIdAddress = $"{GameStatePointer},19DC";
                LivesAddress = $"{GameStatePointer},19E0";
                LuckyCharmsAddress = $"{GameStatePointer},19E4";
                CoinsAddress = $"{GameStatePointer},19E8";
                GadgetAddress = $"{GameStatePointer},19F0";
                ReloadAddress = "27E884";
                ReloadValuesAddress = "249C70";
                ReloadValuesStructSize = 0x2C;
                IsLoadingAddress = $"{ReloadAddress}";
                LanguageAddress = "";
                ClockAddress = "2620D4";
                CameraPointer = "262210";
                DrawDistanceAddress = $"{CameraPointer},B0";
                FOVAddress = $"{CameraPointer},1C8";
                ResetCameraAddress = $"{CameraPointer},220";
                CanCameraNoclipAddress = "27E268";
                ControllerAddress = "263520";
                DialoguePointer = "2759E8";
                SlyEntityPointer = "263610";
                ActiveCharacterVehiclePointer = "26A4A8";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                EntranceRootNodePointer = "27E000,1A54";
                SkipFMVPointer = "26A250";
                TreasureInTheDepthsChestCountPointer = "26EC4C";
                RaceLapsCountPointer = "26F76C";
                RaceNitrosCountPointer = "26FCFC";
                PiranhaLakeFishCountPointer = "2718E4";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "26F1DC";
                BurningRubberFireSlugsComputerCountPointer = "279044";
                BurningRubberComputerCountPointer = "278AB4";
                BentleyComesThroughChipCountPointer = "27081C";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCDemo])
            {
                // SCUS-97210 - EF7F0CE6
                _offsetRadTarget = "5E8";
                _offsetCollision = "55F";
                _offsetSpeedMultiplier = "2044";
                _worldStateStructSize = 0x250;
                _levelStateStructSize = 0x40;
                _offsetSprintsCompleted = 0x4;
                _offsetTimePlayed = 0x8;
                _offsetWorldFlag = 0xC;

                GameStatePointer = "280C10";
                WorldIdAddress = $"{GameStatePointer},1040";
                MapIdAddress = $"{GameStatePointer},1044";
                LivesAddress = $"{GameStatePointer},1048";
                LuckyCharmsAddress = $"{GameStatePointer},104C";
                CoinsAddress = $"{GameStatePointer},1050";
                GadgetAddress = $"{GameStatePointer},1058";
                ReloadAddress = "2AA28C";
                ReloadValuesAddress = "289A08";
                ReloadValuesStructSize = 0; // Works a different way
                LanguageAddress = "";
                IsLoadingAddress = $"{ReloadAddress}";
                ClockAddress = "2386E4";
                CameraPointer = "2387BC";
                DrawDistanceAddress = $"{CameraPointer},B0";
                FOVAddress = $""; // probably 1C4 but it doesn't work
                ResetCameraAddress = $"{CameraPointer},208";
                //CanCameraNoclipAddress = "";
                ControllerAddress = "27AC60";
                DialoguePointer = "286A00";
                SlyEntityPointer = "27AD50";
                ActiveCharacterVehiclePointer = ""; // maybe 282038
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                EntranceRootNodePointer = "289A00,1A4C";
                SkipFMVPointer = "281E08";
                TreasureInTheDepthsChestCountPointer = "";
                RaceLapsCountPointer = "";
                RaceNitrosCountPointer = "";
                PiranhaLakeFishCountPointer = "";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "";
                BurningRubberFireSlugsComputerCountPointer = "";
                BurningRubberComputerCountPointer = "";
                BentleyComesThroughChipCountPointer = "";

                // stock_objects (skip)
                // splash
                // attract
                // a stealthy approach
                // prowling the grounds
                // high class heist
                // into the machine (skip)
                // a cunning disguise
                Maps.Skip(8).ToList().ForEach(m => m.IsVisible = false);
                Maps[6].IsVisible = false; // into the machine
                Maps.RemoveAt(2); // hideout
                Maps.RemoveAt(1); // paris
                Maps.Insert(1, new("attract", new(), true));

                Gadgets = new()
                {
                    new()
                    {
                        // 00000001 Dive
                        // 00000002 (in air) Slow
                        // 00000004 Ball
                        // 00000008 Binocucom Clues
                        // 00000010 Binocucom Coins
                        // 00000020 Binocucom Grab
                        // 00000040 Binocucom Hide
                        // 00000080 Binocucom Tertiary
                        // 00000100 Binocucom Breakables
                        // 00000200 Binocucom Short Cuts
                        // 00000400 Binocucom Scan
                        // 00000800 Invisibility after a while
                        // 00001000 Invisibility 2 (Invisibility immediately)
                        // 00002000 Invisibility sneak
                        // 00004000 Invisibility run
                        // 00020000 Fast
                        // 00800000 Jump and dive
                        new(_gadgetNames[GADGET_NAME.Dive], 0x0), // Dive
                        new(_gadgetNames[GADGET_NAME.Slow1], 0x1), // (in air) Slow
                        new(_gadgetNames[GADGET_NAME.Roll], 0x2), // Ball
                        new(_gadgetNames[GADGET_NAME.BinocucomClues], 0x3),
                        new(_gadgetNames[GADGET_NAME.BinocucomCoins], 0x4),
                        new(_gadgetNames[GADGET_NAME.BinocucomGrab], 0x5),
                        new(_gadgetNames[GADGET_NAME.BinocucomHide], 0x6),
                        new(_gadgetNames[GADGET_NAME.BinocucomTertiary], 0x7),
                        new(_gadgetNames[GADGET_NAME.BinocucomBreakables], 0x8),
                        new(_gadgetNames[GADGET_NAME.BinocucomShortCuts], 0x9),
                        new(_gadgetNames[GADGET_NAME.BinocucomScan], 0xA),
                        new(_gadgetNames[GADGET_NAME.InvisibilityAfterAWhile], 0xB),
                        new(_gadgetNames[GADGET_NAME.Invisibility], 0xC),
                        new(_gadgetNames[GADGET_NAME.InvisibilitySneak], 0xD),
                        new(_gadgetNames[GADGET_NAME.InvisibilityRun], 0xE),
                        new(_gadgetNames[GADGET_NAME.Fast], 0x11), // Fast
                        new(_gadgetNames[GADGET_NAME.SmashDive], 0x17), // Jump and dive
                    }
                };
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.PALDemo])
            {
                // SCED-51452 - F3FD8A14
                GameStatePointer = "25F1D8";
                WorldIdAddress = $"{GameStatePointer},19D8";
                MapIdAddress = $"{GameStatePointer},19DC";
                LivesAddress = $"{GameStatePointer},19E0";
                LuckyCharmsAddress = $"{GameStatePointer},19E4";
                CoinsAddress = $"{GameStatePointer},19E8";
                GadgetAddress = $"{GameStatePointer},19F0";
                ReloadAddress = "275A84";
                ReloadValuesAddress = "2451F0";
                ReloadValuesStructSize = 0xAC;
                IsLoadingAddress = $"{ReloadAddress}";
                LanguageAddress = "25F1F8";
                ClockAddress = "25E644";
                CameraPointer = "25E780";
                DrawDistanceAddress = $"{CameraPointer},B0";
                FOVAddress = $"{CameraPointer},1C8";
                ResetCameraAddress = $"{CameraPointer},220";
                CanCameraNoclipAddress = "275478";
                ControllerAddress = "25F878";
                DialoguePointer = "26C7D8";
                SlyEntityPointer = "25F970";
                ActiveCharacterVehiclePointer = "";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                EntranceRootNodePointer = "275210,1A54";
                SkipFMVPointer = "261610";
                TreasureInTheDepthsChestCountPointer = "";
                RaceLapsCountPointer = "";
                RaceNitrosCountPointer = "";
                PiranhaLakeFishCountPointer = "";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "";
                BurningRubberFireSlugsComputerCountPointer = "";
                BurningRubberComputerCountPointer = "";
                BentleyComesThroughChipCountPointer = "";

                Maps.Skip(8).ToList().ForEach(m => m.IsVisible = false);
                Maps[6].IsVisible = false; // into the machine
                Maps[2].IsVisible = false; // hideout
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCJDemo]
                  || build.Region == Util.BuildRegions[BUILD_NAME.NTSCKDemo])
            {
                // PAPX-90231 - 9C29F787
                // SCKA-90004 - 9CB33FB5
                if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCJDemo])
                {
                    GameStatePointer = "26704C";
                    ReloadValuesAddress = "24E0B0";
                    ReloadValuesStructSize = 0x4C;
                    LanguageAddress = "26706C"; // You can't set it to english?
                }
                else
                {
                    GameStatePointer = "267048";
                    ReloadValuesAddress = "24E670";
                    ReloadValuesStructSize = 0x2C;
                    LanguageAddress = "";
                }

                WorldIdAddress = $"{GameStatePointer},19D8";
                MapIdAddress = $"{GameStatePointer},19DC";
                LivesAddress = $"{GameStatePointer},19E0";
                LuckyCharmsAddress = $"{GameStatePointer},19E4";
                CoinsAddress = $"{GameStatePointer},19E8";
                GadgetAddress = $"{GameStatePointer},19F0";
                ReloadAddress = "27D504";
                IsLoadingAddress = $"{ReloadAddress}";
                ClockAddress = "2664C4";
                CameraPointer = "266600";
                DrawDistanceAddress = $"{CameraPointer},B0";
                FOVAddress = $"{CameraPointer},1C8";
                ResetCameraAddress = $"{CameraPointer},220";
                CanCameraNoclipAddress = "27CEF8";
                ControllerAddress = "267718";
                DialoguePointer = "274678";
                SlyEntityPointer = "267810";
                ActiveCharacterVehiclePointer = "2696F8";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                EntranceRootNodePointer = "27CC90,1A54";
                SkipFMVPointer = "2694B0";
                TreasureInTheDepthsChestCountPointer = "";
                RaceLapsCountPointer = "";
                RaceNitrosCountPointer = "";
                PiranhaLakeFishCountPointer = "";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "";
                BurningRubberFireSlugsComputerCountPointer = "";
                BurningRubberComputerCountPointer = "";
                BentleyComesThroughChipCountPointer = "";

                Maps.Skip(8).ToList().ForEach(m => m.IsVisible = false);
                Maps[6].IsVisible = false; // into the machine
                Maps[2].IsVisible = false; // hideout
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCDemoKiosk212Spring2004])
            {
                // SCUS-97383 - 7656425F
                GameStatePointer = "25DDC8";
                WorldIdAddress = $"{GameStatePointer},19D8";
                MapIdAddress = $"{GameStatePointer},19DC";
                LivesAddress = $"{GameStatePointer},19E0";
                LuckyCharmsAddress = $"{GameStatePointer},19E4";
                CoinsAddress = $"{GameStatePointer},19E8";
                GadgetAddress = $"{GameStatePointer},19F0";
                ReloadAddress = "274284";
                ReloadValuesAddress = "2453F0";
                ReloadValuesStructSize = 0x2C;
                IsLoadingAddress = $"{ReloadAddress}";
                LanguageAddress = "";
                ClockAddress = "25D244";
                CameraPointer = "25D380";
                DrawDistanceAddress = $"{CameraPointer},B0";
                FOVAddress = $"{CameraPointer},1C8";
                ResetCameraAddress = $"{CameraPointer},220";
                CanCameraNoclipAddress = "273C88";
                ControllerAddress = "25E498";
                DialoguePointer = "26B408";
                SlyEntityPointer = "25E590";
                ActiveCharacterVehiclePointer = "260478";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                EntranceRootNodePointer = "273A20,1A54";
                SkipFMVPointer = "260230";
                TreasureInTheDepthsChestCountPointer = "";
                RaceLapsCountPointer = "26518C";
                RaceNitrosCountPointer = "26571C";
                PiranhaLakeFishCountPointer = "";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "";
                BurningRubberFireSlugsComputerCountPointer = "";
                BurningRubberComputerCountPointer = "";
                BentleyComesThroughChipCountPointer = "";

                // 0 splash
                // 1 paris
                // 12 a rocky start
                // 13 muggshot turf
                // 14 boneyard
                // 16 at the dog track

                // From two to tango
                Maps.Skip(17).ToList().ForEach(m => m.IsVisible = false);
                Maps[15].IsVisible = false; // murray big gamble
                Maps.Skip(2).Take(10).ToList().ForEach(m => m.IsVisible = false); // hideout and world 1
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCDemoJune14])
            {
                _offsetRadTarget = "658";
                _offsetCollision = "55F";
                _offsetSpeedMultiplier = "2128";
                _worldStateStructSize = 0x2BC;
                _levelStateStructSize = 0x4C;
                _offsetSprintsCompleted = 0x4;
                _offsetTimePlayed = 0x8;
                _offsetWorldFlag = 0xC;

                GameStatePointer = "24FEB8";
                WorldIdAddress = $"{GameStatePointer},1078";
                MapIdAddress = $"{GameStatePointer},107C";
                LivesAddress = $"{GameStatePointer},1080";
                LuckyCharmsAddress = $"{GameStatePointer},1084";
                CoinsAddress = $"{GameStatePointer},1088";
                GadgetAddress = $"{GameStatePointer},1090";
                ReloadAddress = "2631C4";
                ReloadValuesAddress = "2628F8";
                ReloadValuesStructSize = 0; // Works a different way
                LanguageAddress = "";
                IsLoadingAddress = $"{ReloadAddress}";
                ClockAddress = "24F384";
                CameraPointer = "24F46C";
                DrawDistanceAddress = $"{CameraPointer},B0";
                FOVAddress = $"{CameraPointer},1C8";
                ResetCameraAddress = $"{CameraPointer},220";
                CanCameraNoclipAddress = "262B20";
                ControllerAddress = "2520C0";
                DialoguePointer = "25DF4C";
                SlyEntityPointer = "2521B0";
                ActiveCharacterVehiclePointer = "";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                EntranceRootNodePointer = "2628F0,1A90";
                SkipFMVPointer = "258638";
                TreasureInTheDepthsChestCountPointer = "";
                RaceLapsCountPointer = "";
                RaceNitrosCountPointer = "";
                PiranhaLakeFishCountPointer = "";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "";
                BurningRubberFireSlugsComputerCountPointer = "";
                BurningRubberComputerCountPointer = "";
                BentleyComesThroughChipCountPointer = "";

                // stock_objects (skip)
                // splash
                // attract (skip)
                // paris
                // a stealthy approach
                // prowling the grounds
                // high class heist
                // into the machine (skip)
                // a cunning disguise
                Maps.Skip(8).ToList().ForEach(m => m.IsVisible = false);
                Maps[6].IsVisible = false; // into the machine
                Maps.RemoveAt(2); // hideout
                Maps.Insert(1, new("attract", new(), false));

                Gadgets = new()
                {
                    new()
                    {
                        // 00000001 Dive
                        // 00000002 (in air) Slow
                        // 00000004 Ball
                        // 00000008 Invisibility
                        // 00000010 Jump and dive
                        // 00000020 MaxCharm3
                        // 00000040 Fast
                        // 00000080 Invisibility sneak
                        // 00000100 MaxCharm4
                        // 00000200 Binocucom Scan
                        // 00000400 Binocucom Tertiary
                        // 00200000 Binocucom "Clues - Breakables"
                        // 01000000 carmelita
                        new(_gadgetNames[GADGET_NAME.Dive], 0x0), // Dive
                        new(_gadgetNames[GADGET_NAME.Slow1], 0x1), // (in air) Slow
                        new(_gadgetNames[GADGET_NAME.Roll], 0x2), // Ball
                        new(_gadgetNames[GADGET_NAME.Invisibility], 0x3),
                        new(_gadgetNames[GADGET_NAME.SmashDive], 0x4), // Jump and dive
                        new(_gadgetNames[GADGET_NAME.MaxCharm3], 0x5),
                        new(_gadgetNames[GADGET_NAME.Fast], 0x6), // Fast
                        new(_gadgetNames[GADGET_NAME.InvisibilitySneak], 0x7),
                        new(_gadgetNames[GADGET_NAME.MaxCharm4], 0x8),
                        new(_gadgetNames[GADGET_NAME.BinocucomScan], 0x9),
                        new(_gadgetNames[GADGET_NAME.BinocucomTertiary], 0xA),
                        new(_gadgetNames[GADGET_NAME.BlueprintsRaleigh], 0x15),
                        new(_gadgetNames[GADGET_NAME.Carmelita], 0x18),
                    }
                };
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.PALDemoNovember18])
            {
                GameStatePointer = "25F158";
                WorldIdAddress = $"{GameStatePointer},19D8";
                MapIdAddress = $"{GameStatePointer},19DC";
                LivesAddress = $"{GameStatePointer},19E0";
                LuckyCharmsAddress = $"{GameStatePointer},19E4";
                CoinsAddress = $"{GameStatePointer},19E8";
                GadgetAddress = $"{GameStatePointer},19F0";
                ReloadAddress = "275A04";
                ReloadValuesAddress = "245170";
                ReloadValuesStructSize = 0xAC;
                IsLoadingAddress = $"{ReloadAddress}";
                LanguageAddress = "25F178";
                ClockAddress = "25E5C4";
                CameraPointer = "25E700";
                DrawDistanceAddress = $"{CameraPointer},B0";
                FOVAddress = $"{CameraPointer},1C8";
                ResetCameraAddress = $"{CameraPointer},220";
                CanCameraNoclipAddress = "2753F8";
                ControllerAddress = "25F7F8";
                DialoguePointer = "26C758";
                SlyEntityPointer = "25F8F0";
                ActiveCharacterVehiclePointer = "";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                EntranceRootNodePointer = "275190,1A54";
                SkipFMVPointer = "261590";
                TreasureInTheDepthsChestCountPointer = "";
                RaceLapsCountPointer = "";
                RaceNitrosCountPointer = "";
                PiranhaLakeFishCountPointer = "";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "";
                BurningRubberFireSlugsComputerCountPointer = "";
                BurningRubberComputerCountPointer = "";
                BentleyComesThroughChipCountPointer = "";

                Maps.Skip(8).ToList().ForEach(m => m.IsVisible = false);
                Maps[6].IsVisible = false; // into the machine
                Maps[2].IsVisible = false; // hideout
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.PALDemoPlayStationExperience])
            {
                // SCED-51148 - 53EBA5EB
                _offsetRadTarget = "5E8";
                _offsetCollision = "55F";
                _offsetSpeedMultiplier = "2044";
                _worldStateStructSize = 0x250;
                _levelStateStructSize = 0x40;
                _offsetSprintsCompleted = 0x4;
                _offsetTimePlayed = 0x8;
                _offsetWorldFlag = 0xC;

                GameStatePointer = "280B90";
                WorldIdAddress = $"{GameStatePointer},1040";
                MapIdAddress = $"{GameStatePointer},1044";
                LivesAddress = $"{GameStatePointer},1048";
                LuckyCharmsAddress = $"{GameStatePointer},104C";
                CoinsAddress = $"{GameStatePointer},1050";
                GadgetAddress = $"{GameStatePointer},1058";
                ReloadAddress = "2AA60C";
                ReloadValuesAddress = "289DA8";
                ReloadValuesStructSize = 0; // Works a different way
                LanguageAddress = "";
                IsLoadingAddress = $"{ReloadAddress}";
                ClockAddress = "238664";
                CameraPointer = "23873C";
                DrawDistanceAddress = $"{CameraPointer},B0";
                FOVAddress = $""; // probably 1C4 but it doesn't work
                ResetCameraAddress = $"{CameraPointer},208";
                //CanCameraNoclipAddress = "";
                ControllerAddress = "27ABE0";
                DialoguePointer = "286980";
                SlyEntityPointer = "27ACD0";
                ActiveCharacterVehiclePointer = "";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                EntranceRootNodePointer = "289DA0,1A4C";
                SkipFMVPointer = "281D88";
                TreasureInTheDepthsChestCountPointer = "";
                RaceLapsCountPointer = "";
                RaceNitrosCountPointer = "";
                PiranhaLakeFishCountPointer = "";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "";
                BurningRubberFireSlugsComputerCountPointer = "";
                BurningRubberComputerCountPointer = "";
                BentleyComesThroughChipCountPointer = "";

                // stock_objects (skip)
                // splash
                // attract
                // a stealthy approach
                // prowling the grounds
                // high class heist
                // into the machine (skip)
                // a cunning disguise
                Maps.Skip(8).ToList().ForEach(m => m.IsVisible = false);
                Maps[6].IsVisible = false; // into the machine
                Maps.RemoveAt(2); // hideout
                Maps.RemoveAt(1); // paris
                Maps.Insert(1, new("attract", new(), true));

                Gadgets = new()
                {
                    new()
                    {
                        // 00000001 Dive
                        // 00000002 (in air) Slow
                        // 00000004 Ball
                        // 00000008 Binocucom Clues
                        // 00000010 Binocucom Coins
                        // 00000020 Binocucom Grab
                        // 00000040 Binocucom Hide
                        // 00000080 Binocucom Tertiary
                        // 00000100 Binocucom Breakables
                        // 00000200 Binocucom Short Cuts
                        // 00000400 Binocucom Scan
                        // 00000800 Invisibility after a while
                        // 00001000 Invisibility 2 (Invisibility immediately)
                        // 00002000 Invisibility sneak
                        // 00004000 Invisibility run
                        // 00020000 Fast
                        // 00800000 Jump and dive
                        new(_gadgetNames[GADGET_NAME.Dive], 0x0), // Dive
                        new(_gadgetNames[GADGET_NAME.Slow1], 0x1), // (in air) Slow
                        new(_gadgetNames[GADGET_NAME.Roll], 0x2), // Ball
                        new(_gadgetNames[GADGET_NAME.BinocucomClues], 0x3),
                        new(_gadgetNames[GADGET_NAME.BinocucomCoins], 0x4),
                        new(_gadgetNames[GADGET_NAME.BinocucomGrab], 0x5),
                        new(_gadgetNames[GADGET_NAME.BinocucomHide], 0x6),
                        new(_gadgetNames[GADGET_NAME.BinocucomTertiary], 0x7),
                        new(_gadgetNames[GADGET_NAME.BinocucomBreakables], 0x8),
                        new(_gadgetNames[GADGET_NAME.BinocucomShortCuts], 0x9),
                        new(_gadgetNames[GADGET_NAME.BinocucomScan], 0xA),
                        new(_gadgetNames[GADGET_NAME.InvisibilityAfterAWhile], 0xB),
                        new(_gadgetNames[GADGET_NAME.Invisibility], 0xC),
                        new(_gadgetNames[GADGET_NAME.InvisibilitySneak], 0xD),
                        new(_gadgetNames[GADGET_NAME.InvisibilityRun], 0xE),
                        new(_gadgetNames[GADGET_NAME.Fast], 0x11), // Fast
                        new(_gadgetNames[GADGET_NAME.SmashDive], 0x17), // Jump and dive
                    }
                };
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCMay19])
            {
                // SCUS-97198 - 515E82DE
                _offsetTransformation = "E0";
                _offsetCollision = "56F";
                _offsetRadTarget = "5F8";
                _offsetSpeedMultiplier = "2068";
                _worldStateStructSize = 0x2BC;
                _levelStateStructSize = 0x4C;
                _offsetSprintsCompleted = 0x4;
                _offsetTimePlayed = 0x8;
                _offsetWorldFlag = 0xC;

                GameStatePointer = "276220";
                WorldIdAddress = $"{GameStatePointer},1078";
                MapIdAddress = $"{GameStatePointer},107C";
                LivesAddress = $"{GameStatePointer},1080";
                LuckyCharmsAddress = $"{GameStatePointer},1084";
                CoinsAddress = $"{GameStatePointer},1088";
                GadgetAddress = $"{GameStatePointer},1090";
                ReloadAddress = "28B40C";
                ReloadValuesAddress = "276228";
                ReloadValuesStructSize = 0; // Works a different way
                IsLoadingAddress = $"{ReloadAddress}";
                LanguageAddress = "";
                ClockAddress = "274B04";
                CameraPointer = "274C8C";
                DrawDistanceAddress = $"{CameraPointer},B0";
                FOVAddress = $""; // probably 1C4 but it doesn't work
                ResetCameraAddress = $"{CameraPointer},208";
                CanCameraNoclipAddress = "28A9C0";
                ControllerAddress = "278DF8";
                DialoguePointer = "285AF4";
                SlyEntityPointer = "278EF0";
                ActiveCharacterVehiclePointer = "280138";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                EntranceRootNodePointer = "28A780,1A88";
                SkipFMVPointer = "27FEE8";
                TreasureInTheDepthsChestCountPointer = "28379C";
                RaceLapsCountPointer = "283C9C";
                RaceNitrosCountPointer = "283F1C";
                PiranhaLakeFishCountPointer = "";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "";
                BurningRubberFireSlugsComputerCountPointer = "";
                BurningRubberComputerCountPointer = "";
                BentleyComesThroughChipCountPointer = "";

                // stock_objects (skip)
                // splash
                // attract (skip)
                // paris
                // a stealthy approach
                // prowling the grounds
                // high class heist
                // the fire down below
                // a cunning disguise
                // gunboat graveyard
                // treasure in the depths
                // into the machine
                // eye of the storm
                // a rocky start
                // boneyard casino
                // muggshot's turf
                // two to tango
                // straight to the top
                // back alley heist
                // murray big gamble
                // at the dog track
                // last call (skip)

                // invisible from "last call" to "a strange reunion"
                Maps.Skip(20).ToList().ForEach(m => m.IsVisible = false);

                // into the machine - fire down below
                (Maps[6], Maps[8]) = (Maps[8], Maps[6]);

                // into the machine - gunboat graveyard
                (Maps[8], Maps[10]) = (Maps[10], Maps[8]);

                // murray big gamble - two to tango
                (Maps[15], Maps[17]) = (Maps[17], Maps[15]);

                // murray big gamble - straight to the top
                (Maps[17], Maps[18]) = (Maps[18], Maps[17]);

                // murray big gamble - back alley heist
                (Maps[18], Maps[19]) = (Maps[19], Maps[18]);

                Maps.RemoveAt(2); // hideout

                // Move world 4 as if it was world 1
                var itemsToMove = Maps.GetRange(29, _levelCount);
                Maps.RemoveRange(29, _levelCount);
                Maps.InsertRange(2, itemsToMove);

                Gadgets = new()
                {
                    new()
                    {
                        // 00000001 Dive
                        // 00000002 (in air) Slow
                        // 00000004 Ball
                        // 00000008 Invisibility
                        // 00000010 Jump and dive
                        // 00000020 MaxCharm3
                        // 00000040 Fast
                        // 00000080 Invisibility sneak
                        // 00000100 MaxCharm4
                        // 00000200 Binocucom Scan
                        // 00000400 Binocucom Tertiary
                        // 00100000 SnowBlueprints
                        // 00200000 UnderwaterBlueprints
                        // 00400000 MuggshotBlueprints
                        // 00800000 VoodooBlueprints
                        // 01000000 Carmelita
                        new(_gadgetNames[GADGET_NAME.Dive], 0x0), // Dive
                        new(_gadgetNames[GADGET_NAME.Slow1], 0x1), // (in air) Slow
                        new(_gadgetNames[GADGET_NAME.Roll], 0x2), // Ball
                        new(_gadgetNames[GADGET_NAME.Invisibility], 0x3),
                        new(_gadgetNames[GADGET_NAME.SmashDive], 0x4), // Jump and dive
                        new(_gadgetNames[GADGET_NAME.MaxCharm3], 0x5),
                        new(_gadgetNames[GADGET_NAME.Fast], 0x6), // Fast
                        new(_gadgetNames[GADGET_NAME.InvisibilitySneak], 0x7),
                        new(_gadgetNames[GADGET_NAME.MaxCharm4], 0x8),
                        new(_gadgetNames[GADGET_NAME.BinocucomScan], 0x9),
                        new(_gadgetNames[GADGET_NAME.BinocucomTertiary], 0xA),
                        new(_gadgetNames[GADGET_NAME.BlueprintsPandaKing], 0x14),
                        new(_gadgetNames[GADGET_NAME.BlueprintsRaleigh], 0x15),
                        new(_gadgetNames[GADGET_NAME.BlueprintsMuggshot], 0x16),
                        new(_gadgetNames[GADGET_NAME.BlueprintsMzRuby], 0x17),
                        new(_gadgetNames[GADGET_NAME.Carmelita], 0x18),
                    }
                };
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCMay21])
            {
                // SCUS-97198 - E50A78F8
                _offsetTransformation = "E0";
                _offsetCollision = "56F";
                _offsetRadTarget = "5F8";
                _offsetSpeedMultiplier = "2068";
                _worldStateStructSize = 0x2BC;
                _levelStateStructSize = 0x4C;
                _offsetSprintsCompleted = 0x4;
                _offsetTimePlayed = 0x8;
                _offsetWorldFlag = 0xC;

                GameStatePointer = "2762A0";
                WorldIdAddress = $"{GameStatePointer},1078";
                MapIdAddress = $"{GameStatePointer},107C";
                LivesAddress = $"{GameStatePointer},1080";
                LuckyCharmsAddress = $"{GameStatePointer},1084";
                CoinsAddress = $"{GameStatePointer},1088";
                GadgetAddress = $"{GameStatePointer},1090";
                ReloadAddress = "28B48C";
                ReloadValuesAddress = "2762A8";
                ReloadValuesStructSize = 0; // Works a different way
                IsLoadingAddress = $"{ReloadAddress}";
                LanguageAddress = "";
                ClockAddress = "274B84";
                CameraPointer = "274D0C";
                DrawDistanceAddress = $"{CameraPointer},B0";
                FOVAddress = $""; // probably 1C4 but it doesn't work
                ResetCameraAddress = $"{CameraPointer},208";
                CanCameraNoclipAddress = "28AA40";
                ControllerAddress = "278E78";
                DialoguePointer = "285B74";
                SlyEntityPointer = "278F70";
                ActiveCharacterVehiclePointer = "2801B8";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                EntranceRootNodePointer = "28A800,1A88";
                SkipFMVPointer = "27FF68";
                TreasureInTheDepthsChestCountPointer = "28381C";
                RaceLapsCountPointer = "283D1C";
                RaceNitrosCountPointer = "283F9C";
                PiranhaLakeFishCountPointer = "";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "";
                BurningRubberFireSlugsComputerCountPointer = "";
                BurningRubberComputerCountPointer = "";
                BentleyComesThroughChipCountPointer = "";

                // Same as NTSC May 19
                // stock_objects (skip)
                // splash
                // attract (skip)
                // paris
                // a stealthy approach
                // prowling the grounds
                // high class heist
                // the fire down below
                // a cunning disguise
                // gunboat graveyard
                // treasure in the depths
                // into the machine
                // eye of the storm
                // a rocky start
                // boneyard casino
                // muggshot's turf
                // two to tango
                // straight to the top
                // back alley heist
                // murray big gamble
                // at the dog track
                // last call (skip)

                // invisible from "last call" to "a strange reunion"
                Maps.Skip(20).ToList().ForEach(m => m.IsVisible = false);

                // into the machine - fire down below
                (Maps[6], Maps[8]) = (Maps[8], Maps[6]);

                // into the machine - gunboat graveyard
                (Maps[8], Maps[10]) = (Maps[10], Maps[8]);

                // murray big gamble - two to tango
                (Maps[15], Maps[17]) = (Maps[17], Maps[15]);

                // murray big gamble - straight to the top
                (Maps[17], Maps[18]) = (Maps[18], Maps[17]);

                // murray big gamble - back alley heist
                (Maps[18], Maps[19]) = (Maps[19], Maps[18]);

                Maps.RemoveAt(2); // hideout

                // Move world 4 as if it was world 1
                var itemsToMove = Maps.GetRange(29, _levelCount);
                Maps.RemoveRange(29, _levelCount);
                Maps.InsertRange(2, itemsToMove);

                // Same as NTSC May 19
                Gadgets = new()
                {
                    new()
                    {
                        // 00000001 Dive
                        // 00000002 (in air) Slow
                        // 00000004 Ball
                        // 00000008 Invisibility
                        // 00000010 Jump and dive
                        // 00000020 MaxCharm3
                        // 00000040 Fast
                        // 00000080 Invisibility sneak
                        // 00000100 MaxCharm4
                        // 00000200 Binocucom Scan
                        // 00000400 Binocucom Tertiary
                        // 00100000 SnowBlueprints
                        // 00200000 UnderwaterBlueprints
                        // 00400000 MuggshotBlueprints
                        // 00800000 VoodooBlueprints
                        // 01000000 Carmelita
                        new(_gadgetNames[GADGET_NAME.Dive], 0x0), // Dive
                        new(_gadgetNames[GADGET_NAME.Slow1], 0x1), // (in air) Slow
                        new(_gadgetNames[GADGET_NAME.Roll], 0x2), // Ball
                        new(_gadgetNames[GADGET_NAME.Invisibility], 0x3),
                        new(_gadgetNames[GADGET_NAME.SmashDive], 0x4), // Jump and dive
                        new(_gadgetNames[GADGET_NAME.MaxCharm3], 0x5),
                        new(_gadgetNames[GADGET_NAME.Fast], 0x6), // Fast
                        new(_gadgetNames[GADGET_NAME.InvisibilitySneak], 0x7),
                        new(_gadgetNames[GADGET_NAME.MaxCharm4], 0x8),
                        new(_gadgetNames[GADGET_NAME.BinocucomScan], 0x9),
                        new(_gadgetNames[GADGET_NAME.BinocucomTertiary], 0xA),
                        new(_gadgetNames[GADGET_NAME.BlueprintsPandaKing], 0x14),
                        new(_gadgetNames[GADGET_NAME.BlueprintsRaleigh], 0x15),
                        new(_gadgetNames[GADGET_NAME.BlueprintsMuggshot], 0x16),
                        new(_gadgetNames[GADGET_NAME.BlueprintsMzRuby], 0x17),
                        new(_gadgetNames[GADGET_NAME.Carmelita], 0x18),
                    }
                };
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCAugust23])
            {
                // SCUS-97198 - 5E78A4F2
                GameStatePointer = "264F40";
                WorldIdAddress = $"{GameStatePointer},19D8";
                MapIdAddress = $"{GameStatePointer},19DC";
                LivesAddress = $"{GameStatePointer},19E0";
                LuckyCharmsAddress = $"{GameStatePointer},19E4";
                CoinsAddress = $"{GameStatePointer},19E8";
                GadgetAddress = $"{GameStatePointer},19F0";
                ReloadAddress = "278B04";
                ReloadValuesAddress = "248070";
                ReloadValuesStructSize = 0x2C;
                IsLoadingAddress = $"{ReloadAddress}";
                LanguageAddress = "";
                ClockAddress = "2643D4";
                CameraPointer = "264510";
                DrawDistanceAddress = $"{CameraPointer},B0";
                FOVAddress = $"{CameraPointer},1C8";
                ResetCameraAddress = $"{CameraPointer},220";
                CanCameraNoclipAddress = "278508";
                ControllerAddress = "265898";
                DialoguePointer = "2730AC";
                SlyEntityPointer = "265990";
                ActiveCharacterVehiclePointer = "26C828";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                EntranceRootNodePointer = "2782A0,1A54";
                SkipFMVPointer = "26C5E0";
                TreasureInTheDepthsChestCountPointer = "26FEBC";
                RaceLapsCountPointer = "2703BC";
                RaceNitrosCountPointer = "27063C";
                PiranhaLakeFishCountPointer = "2712D4";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "27013C";
                BurningRubberFireSlugsComputerCountPointer = "2754A4";
                BurningRubberComputerCountPointer = "275224";
                BentleyComesThroughChipCountPointer = "270B3C";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.PALNovember8])
            {
                // SCES-50917 - B9AB722F
                GameStatePointer = "2661E8";
                WorldIdAddress = $"{GameStatePointer},19D8";
                MapIdAddress = $"{GameStatePointer},19DC";
                LivesAddress = $"{GameStatePointer},19E0";
                LuckyCharmsAddress = $"{GameStatePointer},19E4";
                CoinsAddress = $"{GameStatePointer},19E8";
                GadgetAddress = $"{GameStatePointer},19F0";
                ReloadAddress = "282204";
                ReloadValuesAddress = "2495F0";
                ReloadValuesStructSize = 0xAC;
                IsLoadingAddress = $"{ReloadAddress}";
                LanguageAddress = "266208";
                ClockAddress = "265654";
                CameraPointer = "265790";
                DrawDistanceAddress = $"{CameraPointer},B0";
                FOVAddress = $"{CameraPointer},1C8";
                ResetCameraAddress = $"{CameraPointer},220";
                CanCameraNoclipAddress = "281BE8";
                ControllerAddress = "266A80";
                DialoguePointer = "278F48";
                SlyEntityPointer = "266B70";
                ActiveCharacterVehiclePointer = "26DA18";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                EntranceRootNodePointer = "281980,1A54";
                SkipFMVPointer = "26D7C0";
                TreasureInTheDepthsChestCountPointer = "2721AC";
                RaceLapsCountPointer = "272CCC";
                RaceNitrosCountPointer = "27325C";
                PiranhaLakeFishCountPointer = "274E44";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "27273C";
                BurningRubberFireSlugsComputerCountPointer = "27C9C4";
                BurningRubberComputerCountPointer = "27C434";
                BentleyComesThroughChipCountPointer = "273D7C";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCPS3PSN])
            {
                _offsetRadTarget = "620";
                _offsetSpeedMultiplier = "225C";
                
                GameStatePointer = "3A4FC0";
                WorldIdAddress = $"{GameStatePointer},19D8";
                MapIdAddress = $"{GameStatePointer},19DC";
                LivesAddress = $"{GameStatePointer},19E0";
                LuckyCharmsAddress = $"{GameStatePointer},19E4";
                CoinsAddress = $"{GameStatePointer},19E8";
                GadgetAddress = $"{GameStatePointer},19F4";
                ReloadAddress = "E62AC4";
                ReloadValuesAddress = "3B54FC";
                ReloadValuesStructSize = 0x6C;
                IsLoadingAddress = $"{ReloadAddress}";
                LanguageAddress = "3E8010"; // english, french and spanish
                ClockAddress = "39C314";
                CameraPointer = "3E6340";
                DrawDistanceAddress = $"{CameraPointer},B0";
                FOVAddress = $"{CameraPointer},1C8";
                ResetCameraAddress = $"{CameraPointer},220";
                CanCameraNoclipAddress = "E5E93C";
                ControllerAddress = "428BFC";
                DialoguePointer = "E418B0";
                SlyEntityPointer = "428D04";
                ActiveCharacterVehiclePointer = "E3F790";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                EntranceRootNodePointer = "E5E8D8,1A54";
                SkipFMVPointer = "F516CC";
                TreasureInTheDepthsChestCountPointer = "E44474";
                RaceLapsCountPointer = "E44F8C";
                RaceNitrosCountPointer = "E45518";
                PiranhaLakeFishCountPointer = "E470F0";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "E44A00";
                BurningRubberFireSlugsComputerCountPointer = "E4D25C";
                BurningRubberComputerCountPointer = "E4CCCC";
                BentleyComesThroughChipCountPointer = "E46030";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.PALPS3PSN])
            {
                _offsetRadTarget = "620";
                _offsetSpeedMultiplier = "225C";

                GameStatePointer = "3A4FE0";
                WorldIdAddress = $"{GameStatePointer},19D8";
                MapIdAddress = $"{GameStatePointer},19DC";
                LivesAddress = $"{GameStatePointer},19E0";
                LuckyCharmsAddress = $"{GameStatePointer},19E4";
                CoinsAddress = $"{GameStatePointer},19E8";
                GadgetAddress = $"{GameStatePointer},19F4";
                ReloadAddress = "E63B04";
                ReloadValuesAddress = "3B559C";
                ReloadValuesStructSize = 0xAC;
                IsLoadingAddress = $"{ReloadAddress}";
                LanguageAddress = "3E8C10";
                ClockAddress = "39C334";
                CameraPointer = "3E6F40";
                DrawDistanceAddress = $"{CameraPointer},B0";
                FOVAddress = $"{CameraPointer},1C8";
                ResetCameraAddress = $"{CameraPointer},220";
                CanCameraNoclipAddress = "E5F97C";
                ControllerAddress = "4297FC";
                DialoguePointer = "E424B0";
                SlyEntityPointer = "429904";
                ActiveCharacterVehiclePointer = "E40390";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                EntranceRootNodePointer = "E5F918,1A54";
                SkipFMVPointer = "F5270C";
                TreasureInTheDepthsChestCountPointer = "E45074";
                RaceLapsCountPointer = "E45B8C";
                RaceNitrosCountPointer = "E46118";
                PiranhaLakeFishCountPointer = "E47CF0";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "E45600";
                BurningRubberFireSlugsComputerCountPointer = "E4E27C";
                BurningRubberComputerCountPointer = "E4DCEC";
                BentleyComesThroughChipCountPointer = "E46C30";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCKPS3PSN])
            {
                _offsetRadTarget = "620";
                _offsetSpeedMultiplier = "225C";

                GameStatePointer = "3A4F90";
                WorldIdAddress = $"{GameStatePointer},19D8";
                MapIdAddress = $"{GameStatePointer},19DC";
                LivesAddress = $"{GameStatePointer},19E0";
                LuckyCharmsAddress = $"{GameStatePointer},19E4";
                CoinsAddress = $"{GameStatePointer},19E8";
                GadgetAddress = $"{GameStatePointer},19F4";
                ReloadAddress = "E61E44";
                ReloadValuesAddress = "3B545C";
                ReloadValuesStructSize = 0x2C;
                IsLoadingAddress = $"{ReloadAddress}";
                LanguageAddress = "";
                ClockAddress = "39C2F4";
                CameraPointer = "3E56C0";
                DrawDistanceAddress = $"{CameraPointer},B0";
                FOVAddress = $"{CameraPointer},1C8";
                ResetCameraAddress = $"{CameraPointer},220";
                CanCameraNoclipAddress = "E5DCBC";
                ControllerAddress = "427F7C";
                DialoguePointer = "E40C30";
                SlyEntityPointer = "428084";
                ActiveCharacterVehiclePointer = "E3EB10";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                EntranceRootNodePointer = "E5DC58,1A54";
                SkipFMVPointer = "F50A4C";
                TreasureInTheDepthsChestCountPointer = "E437F4";
                RaceLapsCountPointer = "E4430C";
                RaceNitrosCountPointer = "E44898";
                PiranhaLakeFishCountPointer = "E46470";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "E43D80";
                BurningRubberFireSlugsComputerCountPointer = "E4C5DC";
                BurningRubberComputerCountPointer = "E4C04C";
                BentleyComesThroughChipCountPointer = "E453B0";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCPS3])
            {
                _offsetRadTarget = "620";
                _offsetSpeedMultiplier = "225C";

                GameStatePointer = "3426E0";
                WorldIdAddress = $"{GameStatePointer},19D8";
                MapIdAddress = $"{GameStatePointer},19DC";
                LivesAddress = $"{GameStatePointer},19E0";
                LuckyCharmsAddress = $"{GameStatePointer},19E4";
                CoinsAddress = $"{GameStatePointer},19E8";
                GadgetAddress = $"{GameStatePointer},19F4";
                ReloadAddress = "E01D64";
                ReloadValuesAddress = "352BBC";
                ReloadValuesStructSize = 0x6C;
                IsLoadingAddress = $"{ReloadAddress}";
                LanguageAddress = "385690"; // english, french and spanish
                ClockAddress = "339A98";
                CameraPointer = "3839C0";
                DrawDistanceAddress = $"{CameraPointer},B0";
                FOVAddress = $"{CameraPointer},1C8";
                ResetCameraAddress = $"{CameraPointer},220";
                CanCameraNoclipAddress = "DFDBD0";
                ControllerAddress = "3C627C";
                DialoguePointer = "DE0B30";
                SlyEntityPointer = "3C6384";
                ActiveCharacterVehiclePointer = "DDEA10";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                EntranceRootNodePointer = "DFDB58,1A54";
                SkipFMVPointer = "EEF25C";
                TreasureInTheDepthsChestCountPointer = "DE36F4";
                RaceLapsCountPointer = "DE420C";
                RaceNitrosCountPointer = "DE4798";
                PiranhaLakeFishCountPointer = "DE6370";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "DE3C80";
                BurningRubberFireSlugsComputerCountPointer = "DEC4DC";
                BurningRubberComputerCountPointer = "DEBF4C";
                BentleyComesThroughChipCountPointer = "DE52B0";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.PALPS3])
            {
                _offsetRadTarget = "620";
                _offsetSpeedMultiplier = "225C";

                GameStatePointer = "342890";
                WorldIdAddress = $"{GameStatePointer},19D8";
                MapIdAddress = $"{GameStatePointer},19DC";
                LivesAddress = $"{GameStatePointer},19E0";
                LuckyCharmsAddress = $"{GameStatePointer},19E4";
                CoinsAddress = $"{GameStatePointer},19E8";
                GadgetAddress = $"{GameStatePointer},19F4";
                ReloadAddress = "E02FE4";
                ReloadValuesAddress = "352DDC";
                ReloadValuesStructSize = 0xAC;
                IsLoadingAddress = $"{ReloadAddress}";
                LanguageAddress = "386418";
                ClockAddress = "339C68";
                CameraPointer = "384740";
                DrawDistanceAddress = $"{CameraPointer},B0";
                FOVAddress = $"{CameraPointer},1C8";
                ResetCameraAddress = $"{CameraPointer},220";
                CanCameraNoclipAddress = "DFEE50";
                ControllerAddress = "3C701C";
                DialoguePointer = "DE1930";
                SlyEntityPointer = "3C7124";
                ActiveCharacterVehiclePointer = "DDF810";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                EntranceRootNodePointer = "DFEDD8,1A54";
                SkipFMVPointer = "EF052C";
                TreasureInTheDepthsChestCountPointer = "DE44F4";
                RaceLapsCountPointer = "DE500C";
                RaceNitrosCountPointer = "DE5598";
                PiranhaLakeFishCountPointer = "DE7170";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "DE4A80";
                BurningRubberFireSlugsComputerCountPointer = "DED6FC";
                BurningRubberComputerCountPointer = "DED16C";
                BentleyComesThroughChipCountPointer = "DE60B0";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.UKPS3])
            {
                _offsetRadTarget = "620";
                _offsetSpeedMultiplier = "225C";

                GameStatePointer = "3426A0";
                WorldIdAddress = $"{GameStatePointer},19D8";
                MapIdAddress = $"{GameStatePointer},19DC";
                LivesAddress = $"{GameStatePointer},19E0";
                LuckyCharmsAddress = $"{GameStatePointer},19E4";
                CoinsAddress = $"{GameStatePointer},19E8";
                GadgetAddress = $"{GameStatePointer},19F4";
                ReloadAddress = "E015A4";
                ReloadValuesAddress = "352AFC";
                ReloadValuesStructSize = 0x2C;
                IsLoadingAddress = $"{ReloadAddress}";
                LanguageAddress = "";
                ClockAddress = "339A88";
                CameraPointer = "382DC0";
                DrawDistanceAddress = $"{CameraPointer},B0";
                FOVAddress = $"{CameraPointer},1C8";
                ResetCameraAddress = $"{CameraPointer},220";
                CanCameraNoclipAddress = "DFD410";
                ControllerAddress = "3C567C";
                DialoguePointer = "DDFF30";
                SlyEntityPointer = "3C5784";
                ActiveCharacterVehiclePointer = "DDDE10";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                EntranceRootNodePointer = "DFD398,1A54";
                SkipFMVPointer = "EEEA9C";
                TreasureInTheDepthsChestCountPointer = "DE2AF4";
                RaceLapsCountPointer = "DE360C";
                RaceNitrosCountPointer = "DE3B98";
                PiranhaLakeFishCountPointer = "DE5770";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "DE3080";
                BurningRubberFireSlugsComputerCountPointer = "DEBCFC";
                BurningRubberComputerCountPointer = "DEB76C";
                BentleyComesThroughChipCountPointer = "DE46B0";
            }
            else if (build.Region == Util.BuildRegions[BUILD_NAME.NTSCJPS3]
                  || build.Region == Util.BuildRegions[BUILD_NAME.NTSCKPS3])
            {
                _offsetRadTarget = "620";
                _offsetSpeedMultiplier = "225C";

                GameStatePointer = "3426E0";
                WorldIdAddress = $"{GameStatePointer},19D8";
                MapIdAddress = $"{GameStatePointer},19DC";
                LivesAddress = $"{GameStatePointer},19E0";
                LuckyCharmsAddress = $"{GameStatePointer},19E4";
                CoinsAddress = $"{GameStatePointer},19E8";
                GadgetAddress = $"{GameStatePointer},19F4";
                ReloadAddress = "E01164";
                ReloadValuesAddress = "352B3C";
                ReloadValuesStructSize = 0x2C;
                IsLoadingAddress = $"{ReloadAddress}";
                LanguageAddress = "";
                ClockAddress = "339AA4";
                CameraPointer = "382DC0";
                DrawDistanceAddress = $"{CameraPointer},B0";
                FOVAddress = $"{CameraPointer},1C8";
                ResetCameraAddress = $"{CameraPointer},220";
                CanCameraNoclipAddress = "DFCFD0";
                ControllerAddress = "3C567C";
                DialoguePointer = "DDFF30";
                SlyEntityPointer = "3C5784";
                ActiveCharacterVehiclePointer = "DDDE10";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                EntranceRootNodePointer = "DFCF58,1A54";
                SkipFMVPointer = "EEE64C";
                TreasureInTheDepthsChestCountPointer = "DE2AF4";
                RaceLapsCountPointer = "DE360C";
                RaceNitrosCountPointer = "DE3B98";
                PiranhaLakeFishCountPointer = "DE5770";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "DE3080";
                BurningRubberFireSlugsComputerCountPointer = "DEB8DC";
                BurningRubberComputerCountPointer = "DEB34C";
                BentleyComesThroughChipCountPointer = "DE46B0";
            }

            _offsetVelocity = $"{_offsetTransformation}+80";
        }

        public override void CustomTick()
        {
            SetActiveCharacterPointer();

            _form.UpdateUI(() =>
            {
                if (!_form.cmbLuckyCharms.DroppedDown)
                {
                    _form.UpdateUI(_form.cmbLuckyCharms, ReadLuckyCharms());
                }
            });

            string tabName = "";
            _form.UpdateUI(() =>
            {
                tabName = _form.tabControlMain.SelectedTab!.Name;
            });

            if (tabName == "tabWorldStates")
            {
                UpdateWorldStates();
            }
        }

        private void UpdateWorldStates()
        {
            TabPage? tabPageWorld = null;
            string tabNameWorld = "";
            _form.UpdateUI(() =>
            {
                tabPageWorld = _form.tabControlWorldStates.SelectedTab!;
                tabNameWorld = tabPageWorld.Name;
            });

            // If it's the first time we switched to the world state tab, fill them with the right controls
            if (tabPageWorld!.Controls.Count == 0)
            {
                // Skip world 0, reserved for intro (splash and paris)
                for (int i = 1; i <= 5; i++)
                {
                    string internalWorldName = "";
                    if (Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCDemo]
                     || Build.Region == Util.BuildRegions[Util.BUILD_NAME.PALDemoPlayStationExperience])
                    {
                        // Has underwater as world 3
                        switch (i)
                        {
                            case 1: internalWorldName = "Voodoo"; break;
                            case 2: internalWorldName = "Muggshot"; break;
                            case 3: internalWorldName = "Underwater"; break;
                            case 4: internalWorldName = "Snow"; break;
                            case 5: internalWorldName = "Clockwerk"; break;
                        }
                    }
                    else if (Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCDemoJune14])
                    {
                        // Has underwater as world 2
                        switch (i)
                        {
                            case 1: internalWorldName = "Muggshot"; break;
                            case 2: internalWorldName = "Underwater"; break;
                            case 3: internalWorldName = "Voodoo"; break;
                            case 4: internalWorldName = "Snow"; break;
                            case 5: internalWorldName = "Clockwerk"; break;
                        }
                    }
                    else if (Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCMay19]
                          || Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCMay21])
                    {
                        // Has underwater as world 2 and muggshot as world 3
                        switch (i)
                        {
                            case 1: internalWorldName = "Snow"; break;
                            case 2: internalWorldName = "Underwater"; break;
                            case 3: internalWorldName = "Muggshot"; break;
                            case 4: internalWorldName = "Voodoo"; break;
                            case 5: internalWorldName = "Clockwerk"; break;
                        }
                    }
                    else
                    {
                        switch (i)
                        {
                            case 1: internalWorldName = "Underwater"; break;
                            case 2: internalWorldName = "Muggshot"; break;
                            case 3: internalWorldName = "Voodoo"; break;
                            case 4: internalWorldName = "Snow"; break;
                            case 5: internalWorldName = "Clockwerk"; break;
                        }
                    }

                    _form.UpdateUI(() =>
                    {
                        FillTabWorldState(i, internalWorldName);
                    });
                }
            }

            // Update the checkboxes for each level of the world currently selected
            int worldId = Convert.ToInt32(tabNameWorld.Last().ToString());
            for (int levelId = 0; levelId < _levelCount; levelId++)
            {
                if (worldId == 5)
                {
                    // Skip between peril and strange
                    if (levelId == 7)
                    {
                        levelId++;
                    }

                    // Skip between hazardous and rubber
                    if (levelId == 1)
                    {
                        levelId++;
                    }
                }

                // Update the checkboxes for this level
                UpdateWorldStateTabLevelFlags(worldId, levelId, tabPageWorld);
            }

            // Update the checkboxes for this world
            UpdateWorldStateTabFlags(worldId, tabPageWorld);
        }

        public override void OnFirstLoopAfterLoading(int mapId)
        {
            _form.UpdateUI(_form.grpGadgets, true, "Enabled");
        }

        public override bool IsLoading()
        {
            if (_m.ReadInt(IsLoadingAddress) != 0)
            {
                return true;
            }

            return false;
        }

        #region Gadgets
        public override long ReadGadgets()
        {
            return _m.ReadInt(GadgetAddress);
        }

        public override void WriteGadgets(long value)
        {
            int tmp = (int)value;
            _m.WriteMemory(GadgetAddress, "int", tmp.ToString());
        }

        public override void ToggleAllGadgets()
        {
            var gadgets = (int)ReadGadgets();
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
            throw new NotImplementedException();
        }

        public override void UnfreezeActCharGadgetPower()
        {
            throw new NotImplementedException();
        }

        public override void FreezeActCharGadgetPower(int value)
        {
            throw new NotImplementedException();
        }

        public void WriteActCharGadgetPower(int value)
        {
            throw new NotImplementedException();
        }

        public override int ReadActCharGadgetId(GADGET_BIND bind)
        {
            throw new NotImplementedException();
        }

        public override void WriteActCharGadgetId(GADGET_BIND bind, int value)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region World state
        private void FillTabWorldState(int worldId, string internalWorldName)
        {
            TabPage tabWorldState = _form.tabControlWorldStates.TabPages[$"tabWorldState{worldId}"]!;
            tabWorldState.Tag = internalWorldName;

            string mapNameStart = "";
            switch (internalWorldName)
            {
                case "Underwater": mapNameStart = "A Stealthy Approach"; break;
                case "Muggshot": mapNameStart = "A Rocky Start"; break;
                case "Voodoo": mapNameStart = "The Dread Swamp Path"; break;
                case "Snow": mapNameStart = "A Perilous Ascent"; break;
                case "Clockwerk": mapNameStart = "A Hazardous Path"; break;
            }

            int mapIdStart = Maps.FindIndex(x => x.Name == mapNameStart);

            TableLayoutPanel tableLevels = new()
            {
                Name = $"tabWorldState{worldId}LevelsTable",
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = _tabWorldStateHeaderLabels.Length,
                RowCount = 1,
                GrowStyle = TableLayoutPanelGrowStyle.AddRows,
                Anchor = AnchorStyles.None,
            };

            tableLevels.SuspendLayout();

            // Add header labels
            for (int column = 0; column < _tabWorldStateHeaderLabels.Length; column++)
            {
                Label lbl = new()
                {
                    Text = _tabWorldStateHeaderLabels[column],
                    AutoSize = true,
                    Font = _form.lblXCoord.Font,
                    Padding = new Padding(0, 0, 0, 14),
                };

                tableLevels.Controls.Add(lbl, column, 0);
            }

            // For each level, add the map name and the checkboxes
            CheckBox chk;
            for (int levelId = 0; levelId < _levelCount; levelId++)
            {
                string levelName = Maps[mapIdStart + levelId].Name.TrimStart();
                TabWorldStateAddLevel(tableLevels, worldId, levelId, levelName);
            }

            // Layout needs to be calculated first, so that we can add the buttons every 2 rows
            tabWorldState.Controls.Add(tableLevels);
            tableLevels.ResumeLayout();
            tableLevels.PerformLayout();

            // Add the "toggle to all" buttons
            tabWorldState.Controls.Add(TabWorldStateGetToggleButtonToAll(tableLevels, worldId, _tabWorldStateHeaderLabels[1], 0));
            tabWorldState.Controls.Add(TabWorldStateGetToggleButtonToAll(tableLevels, worldId, _tabWorldStateHeaderLabels[2], 2));
            tabWorldState.Controls.Add(TabWorldStateGetToggleButtonToAll(tableLevels, worldId, _tabWorldStateHeaderLabels[3], 4));
            tabWorldState.Controls.Add(TabWorldStateGetToggleButtonToAll(tableLevels, worldId, _tabWorldStateHeaderLabels[4], 6));

            // Flags
            TableLayoutPanel tableFlags = new()
            {
                Name = $"tabWorldState{worldId}FlagsTable",
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 5,
                RowCount = 1,
                GrowStyle = TableLayoutPanelGrowStyle.AddRows,
                Anchor = AnchorStyles.None,
                Location = new(0, tableLevels.Height + 16),
            };

            tableFlags.SuspendLayout();
            TabWorldStateAddWorldFlags(tableFlags, worldId, internalWorldName);
            tabWorldState.Controls.Add(tableFlags);
            tableFlags.ResumeLayout();
            tableFlags.PerformLayout();

            // Flags input
            TableLayoutPanel tableFlagsInputs = new()
            {
                Name = $"tabWorldState{worldId}FlagsInputsTable",
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                RowCount = 1,
                GrowStyle = TableLayoutPanelGrowStyle.AddRows,
                Anchor = AnchorStyles.None,
                Location = new(0, tableLevels.Height + 16 + tableFlags.Height),
            };

            tableFlagsInputs.SuspendLayout();
            TabWorldStateAddWorldFlagsInputs(tableFlagsInputs, worldId);
            tabWorldState.Controls.Add(tableFlagsInputs);
            tableFlagsInputs.ResumeLayout();
            tableFlagsInputs.PerformLayout();

            // Flags input specific for each world
            TableLayoutPanel tableWorldSpecificInputs = new()
            {
                Name = $"tabWorldState{worldId}SpecificFlagsInputsTable",
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                RowCount = 1,
                GrowStyle = TableLayoutPanelGrowStyle.AddRows,
                Anchor = AnchorStyles.None,
                Location = new(tableFlagsInputs.Width + 10, tableLevels.Height + 16 + tableFlags.Height),
            };

            tableWorldSpecificInputs.SuspendLayout();
            TabWorldStateAddWorldSpecificInputs(tableWorldSpecificInputs, internalWorldName, worldId);
            tabWorldState.Controls.Add(tableWorldSpecificInputs);
            tableWorldSpecificInputs.ResumeLayout();
            tableWorldSpecificInputs.PerformLayout();
        }

        private Button TabWorldStateGetToggleButtonToAll(TableLayoutPanel table, int worldId, string name, int rowIndex)
        {
            var btn = new Button
            {
                Name = $"btnWorld{worldId}Toggle{name}ToAll",
                Text = $"Toggle {name.ToLower()} to all",
                AutoSize = true,
                UseVisualStyleBackColor = true,
            };
            btn.Click += (s, e) => btnToggleToAll_Click(s, e, table, worldId, name);

            // Place the button every 2 rows, we need to calculate the vertical position by adding all the heights
            var heights = table.GetRowHeights();
            int y = table.Location.Y;
            for (int i = 0; i <= rowIndex; i++)
            {
                y += heights[i];
            }

            btn.Location = new Point( table.Location.X + table.PreferredSize.Width + 20, y);
            return btn;
        }

        private void TabWorldStateAddLevel(TableLayoutPanel tableLevels, int worldId, int levelId, string levelName)
        {
            if (levelName.StartsWith("_"))
            {
                // skip dummy levels in clockwerk
                return;
            }

            // The current row count is the index where we will insert the new row
            int row = tableLevels.RowCount;
            tableLevels.RowCount = row + 1;

            // Add level name
            Label lbl = new()
            {
                Text = levelName,
                AutoSize = true,
                Font = _form.lblXCoord.Font,
                Padding = new Padding(0)
            };
            tableLevels.Controls.Add(lbl, 0, row);

            // Add level checkboxes
            for (int col = 1; col < tableLevels.ColumnCount; col++)
            {
                string type = _tabWorldStateHeaderLabels[col];
                CheckBox chk = new()
                {
                    Name = $"chkWorld{worldId}Level{levelId}{type}",
                    Text = "",
                    Anchor = AnchorStyles.None, // center inside cell
                    AutoSize = true
                };

                chk.Click += (s, e) => chkWorldLevel_Click(s, e, type);
                tableLevels.Controls.Add(chk, col, row);
            }
        }

        private void TabWorldStateAddWorldFlags(TableLayoutPanel tableFlags, int worldId, string internalWorldName)
        {
            CheckBox chk = new()
            {
                Name = $"chkWorld{worldId}Started",
                Text = "Started",
                AutoSize = true,
            };
            chk.Click += (s, e) => chkWorldState_Click(s, e, worldId, internalWorldName, "Started");
            tableFlags.Controls.Add(chk);

            chk = new()
            {
                Name = $"chkWorld{worldId}Key1",
                Text = "1 key collected",
                AutoSize = true,
            };
            chk.Click += (s, e) => chkWorldState_Click(s, e, worldId, internalWorldName, "Key1");
            tableFlags.Controls.Add(chk);

            chk = new()
            {
                Name = $"chkWorld{worldId}Key3",
                Text = "3 keys collected",
                AutoSize = true,
            };
            chk.Click += (s, e) => chkWorldState_Click(s, e, worldId, internalWorldName, "Key3");
            tableFlags.Controls.Add(chk);

            chk = new()
            {
                Name = $"chkWorld{worldId}Key7",
                Text = "7 keys collected",
                AutoSize = true,
            };
            chk.Click += (s, e) => chkWorldState_Click(s, e, worldId, internalWorldName, "Key7");
            tableFlags.Controls.Add(chk);

            chk = new()
            {
                Name = $"chkWorld{worldId}BossDefeated",
                Text = "Boss defeated",
                AutoSize = true,
            };
            chk.Click += (s, e) => chkWorldState_Click(s, e, worldId, internalWorldName, "BossDefeated");
            tableFlags.Controls.Add(chk);
        }

        private void TabWorldStateAddWorldFlagsInputs(TableLayoutPanel table, int worldId)
        {
            Label lbl;
            TextBox txt;

            lbl = new()
            {
                Text = $"Keys collected",
                AutoSize = true,
                Font = _form.lblXCoord.Font,
                Padding = new Padding(0, 4, 0, 0),
            };
            table.Controls.Add(lbl, 0, 0);

            txt = new()
            {
                Name = $"txtWorld{worldId}KeysCollected",
            };
            txt.TextChanged += (s, e) => txtWorldFlag_TextChanged(s, e, worldId, "KeysCollected");
            table.Controls.Add(txt, 1, 0);

            if (Build.Region != Util.BuildRegions[Util.BUILD_NAME.NTSCDemo]
             && Build.Region != Util.BuildRegions[Util.BUILD_NAME.NTSCMay19]
             && Build.Region != Util.BuildRegions[Util.BUILD_NAME.NTSCMay21]
             && Build.Region != Util.BuildRegions[Util.BUILD_NAME.NTSCDemoJune14]
             && Build.Region != Util.BuildRegions[Util.BUILD_NAME.PALDemoPlayStationExperience])
            {
                // builds that don't have cvault field
                lbl = new()
                {
                    Text = $"Safes opened",
                    AutoSize = true,
                    Font = _form.lblXCoord.Font,
                    Padding = new Padding(0, 4, 0, 0)
                };
                table.Controls.Add(lbl, 0, 1);

                txt = new()
                {
                    Name = $"txtWorld{worldId}SafesOpened",
                };
                txt.TextChanged += (s, e) => txtWorldFlag_TextChanged(s, e, worldId, "SafesOpened");
                table.Controls.Add(txt, 1, 1);
            }

            lbl = new()
            {
                Text = $"Sprints completed",
                AutoSize = true,
                Font = _form.lblXCoord.Font,
                Padding = new Padding(0, 4, 0, 0)
            };
            table.Controls.Add(lbl, 0, 2);

            txt = new()
            {
                Name = $"txtWorld{worldId}SprintsCompleted",
            };
            txt.TextChanged += (s, e) => txtWorldFlag_TextChanged(s, e, worldId, "SprintsCompleted");
            table.Controls.Add(txt, 1, 2);
        }

        private void TabWorldStateAddWorldSpecificInputs(TableLayoutPanel table, string internalWorldName, int worldId)
        {
            if (internalWorldName == "Underwater")
            {
                Label lbl1 = new()
                {
                    Text = "Treasure in the Depths - Chests",
                    AutoSize = true,
                    Font = _form.lblXCoord.Font,
                    Padding = new Padding(0, 4, 0, 0),
                };
                table.Controls.Add(lbl1);

                TextBox txt1 = new()
                {
                    Name = $"txtWorld{worldId}DepthsChestCount",
                };
                txt1.TextChanged += (s, e) =>
                {
                    if (!int.TryParse(txt1.Text, out int value))
                    {
                        txt1.Text = "";
                    }
                    WriteTreasureInTheDepthsChestCount(value);
                };
                table.Controls.Add(txt1);
            }
            else if (internalWorldName == "Muggshot")
            {
                Label lbl1 = new()
                {
                    Text = "At the Dog Track - Nitros",
                    AutoSize = true,
                    Font = _form.lblXCoord.Font,
                    Padding = new Padding(0, 4, 0, 0),
                };
                table.Controls.Add(lbl1);

                TextBox txt1 = new()
                {
                    Name = $"txtWorld{worldId}DogTrackNitrosCount",
                };
                txt1.TextChanged += (s, e) =>
                {
                    if (!int.TryParse(txt1.Text, out int value))
                    {
                        txt1.Text = "";
                    }
                    WriteRaceNitrosCount(value);
                };
                table.Controls.Add(txt1);

                Label lbl2 = new()
                {
                    Text = "At the Dog Track - Laps",
                    AutoSize = true,
                    Font = _form.lblXCoord.Font,
                    Padding = new Padding(0, 4, 0, 0),
                };
                table.Controls.Add(lbl2);

                TextBox txt2 = new()
                {
                    Name = $"txtWorld{worldId}DogTrackLapsCount",
                };
                txt2.TextChanged += (s, e) =>
                {
                    if (!int.TryParse(txt2.Text, out int value))
                    {
                        txt2.Text = "";
                    }
                    WriteRaceLapsCount(value);
                };
                table.Controls.Add(txt2);
            }
            else if (internalWorldName == "Voodoo")
            {
                // Piranha Lake
                Label lbl1 = new()
                {
                    Text = "Piranha Lake - Fish",
                    AutoSize = true,
                    Font = _form.lblXCoord.Font,
                    Padding = new Padding(0, 4, 0, 0),
                };
                table.Controls.Add(lbl1);

                TextBox txt1 = new()
                {
                    Name = $"txtWorld{worldId}PiranhaLakeFishCount",
                };
                txt1.TextChanged += (s, e) =>
                {
                    if (!int.TryParse(txt1.Text, out int value))
                    {
                        txt1.Text = "";
                    }
                    WritePiranhaLakeFishCount(value);
                };
                table.Controls.Add(txt1);

                // Piranha Lake
                Label lbl2 = new()
                {
                    Text = "Piranha Lake - Torch",
                    AutoSize = true,
                    Font = _form.lblXCoord.Font,
                    Padding = new Padding(0, 4, 0, 0),
                };
                table.Controls.Add(lbl2);

                TextBox txt2 = new()
                {
                    Name = $"txtWorld{worldId}PiranhaLakeTorchCount",
                };
                txt2.TextChanged += (s, e) =>
                {
                    if (!int.TryParse(txt2.Text, out int value))
                    {
                        txt2.Text = "";
                    }
                    WritePiranhaLakeTorchDownHomeCookingChickenCount(value);
                };
                table.Controls.Add(txt2);

                // Down Home Cooking
                Label lbl3 = new()
                {
                    Text = "Down Home Cooking - Chicken",
                    AutoSize = true,
                    Font = _form.lblXCoord.Font,
                    Padding = new Padding(0, 4, 0, 0),
                };
                table.Controls.Add(lbl3);

                TextBox txt3 = new()
                {
                    Name = $"txtWorld{worldId}DownHomeCookingChickenCount",
                };
                txt3.TextChanged += (s, e) =>
                {
                    if (!int.TryParse(txt3.Text, out int value))
                    {
                        txt3.Text = "";
                    }
                    WritePiranhaLakeTorchDownHomeCookingChickenCount(value);
                };
                table.Controls.Add(txt3);
            }
            else if (internalWorldName == "Snow")
            {
                Label lbl1 = new()
                {
                    Text = "A Desperate Race - Nitros",
                    AutoSize = true,
                    Font = _form.lblXCoord.Font,
                    Padding = new Padding(0, 4, 0, 0),
                };
                table.Controls.Add(lbl1);

                TextBox txt1 = new()
                {
                    Name = $"txtWorld{worldId}ADesperateRaceNitrosCount",
                };
                txt1.TextChanged += (s, e) =>
                {
                    if (!int.TryParse(txt1.Text, out int value))
                    {
                        txt1.Text = "";
                    }
                    WriteRaceNitrosCount(value);
                };
                table.Controls.Add(txt1);

                Label lbl2 = new()
                {
                    Text = "A Desperate Race - Laps",
                    AutoSize = true,
                    Font = _form.lblXCoord.Font,
                    Padding = new Padding(0, 4, 0, 0),
                };
                table.Controls.Add(lbl2);

                TextBox txt2 = new()
                {
                    Name = $"txtWorld{worldId}ADesperateRaceLapsCount",
                };
                txt2.TextChanged += (s, e) =>
                {
                    if (!int.TryParse(txt2.Text, out int value))
                    {
                        txt2.Text = "";
                    }
                    WriteRaceLapsCount(value);
                };
                table.Controls.Add(txt2);
            }
            else if (internalWorldName == "Clockwerk")
            {
                // "Burning Rubber
                Label lbl1 = new()
                {
                    Text = "Burning Rubber - Fire slugs computer",
                    AutoSize = true,
                    Font = _form.lblXCoord.Font,
                    Padding = new Padding(0, 4, 0, 0),
                };
                table.Controls.Add(lbl1);

                TextBox txt1 = new()
                {
                    Name = $"txtWorld{worldId}BurningRubberSlugsComputerCount",
                };
                txt1.TextChanged += (s, e) =>
                {
                    if (!int.TryParse(txt1.Text, out int value))
                    {
                        txt1.Text = "";
                    }
                    WriteBurningRubberFireSlugsComputerCount(value);
                };
                table.Controls.Add(txt1);

                // "Burning Rubber
                Label lbl2 = new()
                {
                    Text = "Burning Rubber - Computer",
                    AutoSize = true,
                    Font = _form.lblXCoord.Font,
                    Padding = new Padding(0, 4, 0, 0),
                };
                table.Controls.Add(lbl2);

                TextBox txt2 = new()
                {
                    Name = $"txtWorld{worldId}BurningRubberComputerCount",
                };
                txt2.TextChanged += (s, e) =>
                {
                    if (!int.TryParse(txt2.Text, out int value))
                    {
                        txt2.Text = "";
                    }
                    WriteBurningRubberComputerCount(value);
                };
                table.Controls.Add(txt2);

                // Bentley Comes Through
                Label lbl3 = new()
                {
                    Text = "Bentley Comes Through - Chip",
                    AutoSize = true,
                    Font = _form.lblXCoord.Font,
                    Padding = new Padding(0, 4, 0, 0),
                };
                table.Controls.Add(lbl3);

                TextBox txt3 = new()
                {
                    Name = $"txtWorld{worldId}BentleyComesThroughChipCount",
                };
                txt3.TextChanged += (s, e) =>
                {
                    if (!int.TryParse(txt3.Text, out int value))
                    {
                        txt3.Text = "";
                    }
                    WriteBentleyComesThroughChipCount(value);
                };
                table.Controls.Add(txt3);
            }
        }

        private void UpdateWorldStateTabLevelFlags(int worldId, int levelId, TabPage tabPage)
        {
            CheckBox chk;
            TableLayoutPanel table = tabPage.Controls[$"tabWorldState{worldId}LevelsTable"] as TableLayoutPanel;
            int levelFlag = ReadLevelFlag(worldId, levelId);

            chk = (table.Controls[$"chkWorld{worldId}Level{levelId}Unlocked"] as CheckBox)!;
            _form.UpdateUI(chk, (levelFlag & 0x1) == 0x1, "Checked");

            chk = (table.Controls[$"chkWorld{worldId}Level{levelId}Key"] as CheckBox)!;
            _form.UpdateUI(chk, (levelFlag & 0x2) == 0x2, "Checked");

            chk = (table.Controls[$"chkWorld{worldId}Level{levelId}Safe"] as CheckBox)!;
            _form.UpdateUI(chk, (levelFlag & 0x4) == 0x4, "Checked");

            chk = (table.Controls[$"chkWorld{worldId}Level{levelId}Sprint"] as CheckBox)!;
            _form.UpdateUI(chk, (levelFlag & 0x8) == 0x8, "Checked");
        }

        private void UpdateWorldStateTabFlags(int worldId, TabPage tabPage)
        {
            CheckBox chk;
            TableLayoutPanel tableFlags = tabPage.Controls[$"tabWorldState{worldId}FlagsTable"] as TableLayoutPanel;
            string internalWorldName = tabPage.Tag as string;
            int worldFlag = ReadWorldFlag(worldId, internalWorldName);

            // Checkboxes
            chk = (tableFlags.Controls[$"chkWorld{worldId}Started"] as CheckBox)!;
            _form.UpdateUI(chk, (worldFlag & 0x1) == 0x1, "Checked");

            chk = (tableFlags.Controls[$"chkWorld{worldId}Key1"] as CheckBox)!;
            _form.UpdateUI(chk, (worldFlag & 0x2) == 0x2, "Checked");

            chk = (tableFlags.Controls[$"chkWorld{worldId}Key3"] as CheckBox)!;
            _form.UpdateUI(chk, (worldFlag & 0x4) == 0x4, "Checked");

            chk = (tableFlags.Controls[$"chkWorld{worldId}Key7"] as CheckBox)!;
            _form.UpdateUI(chk, (worldFlag & 0x8) == 0x8, "Checked");

            chk = (tableFlags.Controls[$"chkWorld{worldId}BossDefeated"] as CheckBox)!;
            _form.UpdateUI(chk, (worldFlag & 0x20) == 0x20, "Checked");

            // Textboxes
            TableLayoutPanel tableFlagsInputs = tabPage.Controls[$"tabWorldState{worldId}FlagsInputsTable"] as TableLayoutPanel;
            TextBox txt = (tableFlagsInputs.Controls[$"txtWorld{worldId}KeysCollected"] as TextBox)!;
            _form.UpdateUI(txt, ReadWorldKeysCollectedCount(worldId).ToString());

            txt = (tableFlagsInputs.Controls[$"txtWorld{worldId}SafesOpened"] as TextBox)!;
            _form.UpdateUI(txt, ReadWorldSafesOpenedCount(worldId).ToString());

            txt = (tableFlagsInputs.Controls[$"txtWorld{worldId}SprintsCompleted"] as TextBox)!;
            _form.UpdateUI(txt, ReadWorldSprintsCompletedCount(worldId).ToString());

            // Update specific world textboxes
            TableLayoutPanel tableWorldSpecificInputs = tabPage.Controls[$"tabWorldState{worldId}SpecificFlagsInputsTable"] as TableLayoutPanel;
            if (internalWorldName == "Underwater")
            {
                txt = (tableWorldSpecificInputs.Controls[$"txtWorld{worldId}DepthsChestCount"] as TextBox)!;
                _form.UpdateUI(txt, ReadTreasureInTheDepthsChestCount().ToString());
            }
            else if (internalWorldName == "Muggshot")
            {
                txt = (tableWorldSpecificInputs.Controls[$"txtWorld{worldId}DogTrackNitrosCount"] as TextBox)!;
                _form.UpdateUI(txt, ReadRaceNitrosCount().ToString());

                txt = (tableWorldSpecificInputs.Controls[$"txtWorld{worldId}DogTrackLapsCount"] as TextBox)!;
                _form.UpdateUI(txt, ReadRaceLapsCount().ToString());
            }
            else if (internalWorldName == "Voodoo")
            {
                txt = (tableWorldSpecificInputs.Controls[$"txtWorld{worldId}PiranhaLakeFishCount"] as TextBox)!;
                _form.UpdateUI(txt, ReadPiranhaLakeFishCount().ToString());

                txt = (tableWorldSpecificInputs.Controls[$"txtWorld{worldId}PiranhaLakeTorchCount"] as TextBox)!;
                _form.UpdateUI(txt, ReadPiranhaLakeTorchDownHomeCookingChickenCount().ToString());

                txt = (tableWorldSpecificInputs.Controls[$"txtWorld{worldId}DownHomeCookingChickenCount"] as TextBox)!;
                _form.UpdateUI(txt, ReadPiranhaLakeTorchDownHomeCookingChickenCount().ToString());
            }
            else if (internalWorldName == "Snow")
            {
                txt = (tableWorldSpecificInputs.Controls[$"txtWorld{worldId}ADesperateRaceNitrosCount"] as TextBox)!;
                _form.UpdateUI(txt, ReadRaceNitrosCount().ToString());

                txt = (tableWorldSpecificInputs.Controls[$"txtWorld{worldId}ADesperateRaceLapsCount"] as TextBox)!;
                _form.UpdateUI(txt, ReadRaceLapsCount().ToString());
            }
            else if (internalWorldName == "Clockwerk")
            {
                txt = (tableWorldSpecificInputs.Controls[$"txtWorld{worldId}BurningRubberSlugsComputerCount"] as TextBox)!;
                _form.UpdateUI(txt, ReadBurningRubberFireSlugsComputerCount().ToString());

                txt = (tableWorldSpecificInputs.Controls[$"txtWorld{worldId}BurningRubberComputerCount"] as TextBox)!;
                _form.UpdateUI(txt, ReadBurningRubberComputerCount().ToString());

                txt = (tableWorldSpecificInputs.Controls[$"txtWorld{worldId}BentleyComesThroughChipCount"] as TextBox)!;
                _form.UpdateUI(txt, ReadBentleyComesThroughChipCount().ToString());
            }
        }

        private void txtWorldFlag_TextChanged(object sender, EventArgs e, int worldId, string type)
        {
            TextBox txt = sender as TextBox;
            if (!int.TryParse(txt.Text, out int value))
            {
                txt.Text = "";
            }

            if (type == "KeysCollected")
            {
                WriteWorldKeysCollectedCount(worldId, value);
            }
            else if (type == "SafesOpened")
            {
                WriteWorldSafesOpenedCount(worldId, value);
            }
            else if (type == "SprintsCompleted")
            {
                WriteWorldSprintsCompletedCount(worldId, value);
            }
        }

        private void btnToggleToAll_Click(object sender, EventArgs e, TableLayoutPanel table, int worldId, string type)
        {
            Button btn = sender as Button;

            List<CheckBox> checkboxes = new();
            for (int i = 0; i < table.Controls.Count; i++)
            {
                if (table.Controls[i].Name.EndsWith(type))
                {
                    checkboxes.Add(table.Controls[i] as CheckBox);
                }
            }

            // true when at least 1 is unchecked
            // false when all are checked
            bool IsNotChecked = checkboxes.Any(item => !item.Checked);
            foreach (var chk in checkboxes)
            {
                if (chk.Checked != IsNotChecked)
                {
                    chkWorldLevel_Click(chk, e, type);
                }
            }
        }

        private void chkWorldState_Click(object sender, EventArgs e, int worldId, string internalWorldName, string type)
        {
            CheckBox chk = sender as CheckBox;
            int worldFlag = ReadWorldFlag(worldId, internalWorldName);

            if (type == "Started")
            {
                worldFlag = worldFlag ^ 0x1;
            }
            else if (type == "Key1")
            {
                worldFlag = worldFlag ^ 0x2;
            }
            else if (type == "Key3")
            {
                worldFlag = worldFlag ^ 0x4;
            }
            else if (type == "Key7")
            {
                worldFlag = worldFlag ^ 0x8;
            }
            else if (type == "BossDefeated")
            {
                worldFlag = worldFlag ^ 0x20;
            }

            WriteWorldFlag(worldId, internalWorldName, worldFlag);
        }

        private void chkWorldLevel_Click(object sender, EventArgs e, string type)
        {
            CheckBox chk = sender as CheckBox;
            int worldId = Convert.ToInt32(chk.Name[8].ToString());
            int levelId = Convert.ToInt32(chk.Name[14].ToString());
            int levelFlags = ReadLevelFlag(worldId, levelId);
            if (type == "Unlocked")
            {
                levelFlags = levelFlags ^ 0x1;
            }
            else if (type == "Key")
            {
                levelFlags = levelFlags ^ 0x2;
            }
            else if (type == "Safe")
            {
                levelFlags = levelFlags ^ 0x4;
            }
            else if (type == "Sprint")
            {
                levelFlags = levelFlags ^ 0x8;
            }

            WriteLevelFlag(worldId, levelId, levelFlags);
        }

        public int ReadLevelFlag(int worldId, int levelId)
        {
            return _m.ReadInt($"{GameStatePointer},{0x10 + (_worldStateStructSize * worldId) + (_levelStateStructSize * levelId):X8}");
        }

        public void WriteLevelFlag(int worldId, int levelId, int levelFlag)
        {
            _m.WriteMemory($"{GameStatePointer},{0x10 + (_worldStateStructSize * worldId) + (_levelStateStructSize * levelId):X8}", "int", levelFlag.ToString());
        }

        public int ReadWorldKeysCollectedCount(int worldId)
        {
            return _m.ReadInt($"{GameStatePointer},{0x10 + (_worldStateStructSize * worldId) + (_levelStateStructSize * _levelCount):X8}");
        }

        public void WriteWorldKeysCollectedCount(int worldId, int keysCollected)
        {
            _m.WriteMemory($"{GameStatePointer},{0x10 + (_worldStateStructSize * worldId) + (_levelStateStructSize * _levelCount):X8}", "int", keysCollected.ToString());
        }

        public int ReadWorldSafesOpenedCount(int worldId)
        {
            return _m.ReadInt($"{GameStatePointer},{0x10 + (_worldStateStructSize * worldId) + (_levelStateStructSize * _levelCount) + _offsetSafesOpened:X8}");
        }

        public void WriteWorldSafesOpenedCount(int worldId, int safesOpened)
        {
            _m.WriteMemory($"{GameStatePointer},{0x10 + (_worldStateStructSize * worldId) + (_levelStateStructSize * _levelCount) + _offsetSafesOpened:X8}", "int", safesOpened.ToString());
        }

        public int ReadWorldSprintsCompletedCount(int worldId)
        {
            return _m.ReadInt($"{GameStatePointer},{0x10 + (_worldStateStructSize * worldId) + (_levelStateStructSize * _levelCount) + _offsetSprintsCompleted:X8}");
        }

        public void WriteWorldSprintsCompletedCount(int worldId, int sprintsCompleted)
        {
            _m.WriteMemory($"{GameStatePointer},{0x10 + (_worldStateStructSize * worldId) + (_levelStateStructSize * _levelCount) + _offsetSprintsCompleted:X8}", "int", sprintsCompleted.ToString());
        }

        public float ReadWorldTimePlayed(int worldId)
        {
            return _m.ReadFloat($"{GameStatePointer},{0x10 + (_worldStateStructSize * worldId) + (_levelStateStructSize * _levelCount) + _offsetTimePlayed:X8}");
        }

        public void WriteWorldTimePlayed(int worldId, float timePlayed)
        {
            _m.WriteMemory($"{GameStatePointer},{0x10 + (_worldStateStructSize * worldId) + (_levelStateStructSize * _levelCount) + _offsetTimePlayed:X8}", "float", timePlayed.ToString());
        }

        public int ReadWorldFlag(int worldId, string internalWorldName)
        {
            int worldFlag = _m.ReadInt($"{GameStatePointer},{0x10 + (_worldStateStructSize * worldId) + (_levelStateStructSize * _levelCount) + _offsetWorldFlag:X8}");
            if (internalWorldName == "Underwater")
            {
                worldFlag = ConvertUnderwaterWorldFlagToNormal(worldFlag);
            }

            return worldFlag;
        }

        public void WriteWorldFlag(int worldId, string internalWorldName, int worldFlag)
        {
            if (internalWorldName == "Underwater")
            {
                worldFlag = ConvertNormalWorldFlagToUnderwater(worldFlag);
            }

            _m.WriteMemory($"{GameStatePointer},{0x10 + (_worldStateStructSize * worldId) + (_levelStateStructSize * _levelCount) + _offsetWorldFlag:X8}", "int", worldFlag.ToString());
        }

        // For all worlds, the order of bits goes like this:
        // 0x1 Started
        // 0x2 Key1
        // 0x4 Key3
        // 0x8 Key7
        // 0x20 BossDefeated
        // But for "Underwater", it goes like this:
        // 0x1 Started
        // 0x8 Key1
        // 0x2 Key3
        // 0x4 Key7
        // 0x20 BossDefeated
        // Key1, Key3 and Key7 are rotated
        // So let's change worldFlag to match the order of the other worlds
        private static int ConvertUnderwaterWorldFlagToNormal(int worldFlag)
        {
            return (worldFlag & 0x01) // Started
                 | ((worldFlag & 0x08) >> 2) // Key1
                 | ((worldFlag & 0x02) << 1) // Key3
                 | ((worldFlag & 0x04) << 1) // Key7
                 | (worldFlag & 0xF0);  // BossDefeated
        }

        private static int ConvertNormalWorldFlagToUnderwater(int worldFlag)
        {
            return (worldFlag & 0x01) // Started
                 | ((worldFlag & 0x02) << 2) // Key1
                 | ((worldFlag & 0x04) >> 1) // Key3
                 | ((worldFlag & 0x08) >> 1) // Key7
                 | (worldFlag & 0xF0); // BossDefeated
        }

        public int ReadTreasureInTheDepthsChestCount()
        {
            return _m.ReadInt($"{TreasureInTheDepthsChestCountPointer},0");
        }

        public void WriteTreasureInTheDepthsChestCount(int value)
        {
            _m.WriteMemory($"{TreasureInTheDepthsChestCountPointer},0", "int", value.ToString());
        }

        public int ReadRaceLapsCount()
        {
            return _m.ReadInt($"{RaceLapsCountPointer},0");
        }

        public void WriteRaceLapsCount(int value)
        {
            _m.WriteMemory($"{RaceLapsCountPointer},0", "int", value.ToString());
        }

        public int ReadRaceNitrosCount()
        {
            return _m.ReadInt($"{RaceNitrosCountPointer},0");
        }

        public void WriteRaceNitrosCount(int value)
        {
            _m.WriteMemory($"{RaceNitrosCountPointer},0", "int", value.ToString());
        }

        public int ReadPiranhaLakeFishCount()
        {
            return _m.ReadInt($"{PiranhaLakeFishCountPointer},0");
        }

        public void WritePiranhaLakeFishCount(int value)
        {
            _m.WriteMemory($"{PiranhaLakeFishCountPointer},0", "int", value.ToString());
        }

        public int ReadPiranhaLakeTorchDownHomeCookingChickenCount()
        {
            return _m.ReadInt($"{PiranhaLakeTorchDownHomeCookingChickenCountPointer},0");
        }

        public void WritePiranhaLakeTorchDownHomeCookingChickenCount(int value)
        {
            _m.WriteMemory($"{PiranhaLakeTorchDownHomeCookingChickenCountPointer},0", "int", value.ToString());
        }

        public int ReadBurningRubberFireSlugsComputerCount()
        {
            return _m.ReadInt($"{BurningRubberFireSlugsComputerCountPointer},0");
        }

        public void WriteBurningRubberFireSlugsComputerCount(int value)
        {
            _m.WriteMemory($"{BurningRubberFireSlugsComputerCountPointer},0", "int", value.ToString());
        }

        public int ReadBurningRubberComputerCount()
        {
            return _m.ReadInt($"{BurningRubberComputerCountPointer},0");
        }

        public void WriteBurningRubberComputerCount(int value)
        {
            _m.WriteMemory($"{BurningRubberComputerCountPointer},0", "int", value.ToString());
        }

        public int ReadBentleyComesThroughChipCount()
        {
            return _m.ReadInt($"{BentleyComesThroughChipCountPointer},0");
        }

        public void WriteBentleyComesThroughChipCount(int value)
        {
            _m.WriteMemory($"{BentleyComesThroughChipCountPointer},0", "int", value.ToString());
        }

        #endregion

        #region Entities
        public override bool EntityHasTransformation(string pointerToEntity)
        {
            return true;
        }

        public override Vector3 ReadEntityDeltaTranslation(string pointerToEntity)
        {
            return Vector3.Zero;
        }

        #region Origin
        public override Matrix4x4 ReadEntityOriginTransformation(string pointerToEntity)
        {
            return Matrix4x4.Identity;
        }

        public override Matrix4x4 ReadEntityOriginCombinedTransformation(string pointerToEntity)
        {
            return Matrix4x4.Identity;
        }

        public override void WriteEntityOriginTransformation(string pointerToEntity, Matrix4x4 transformation)
        {
            _m.WriteMemory($"{pointerToEntity},{_offsetTransformation}", "mat4", transformation.ToString());
        }
        #endregion

        #region Local
        public override Matrix4x4 ReadEntityLocalTransformation(string pointerToEntity)
        {
            return _m.ReadMatrix4($"{pointerToEntity},{_offsetTransformation}");
        }

        public override Matrix4x4 ReadEntityLocalCombinedTransformation(string pointerToEntity)
        {
            return ReadEntityLocalTransformation(pointerToEntity);
        }

        public override Vector3 ReadEntityLocalTranslation(string pointerToEntity)
        {
            return ReadEntityLocalTransformation(pointerToEntity).Translation;
        }

        public override void WriteEntityLocalTransformation(string pointerToEntity, Matrix4x4 trans)
        {
            if (!EntityHasTransformation(pointerToEntity))
            {
                return;
            }

            // Write to the world matrix and velocity too to prevent sly from warping back to his original position
            // Unfortunately, because we are an external process, we might be too slow for the game. Even if we do 1 write call all at once
            // So sometimes sly will still be teleported back to his original position
            // TODO: Find a consistent way to prevent sly from warping back to his original position
            float[] values =
            [
                trans.M11, trans.M12, trans.M13, trans.M14,
                trans.M21, trans.M22, trans.M23, trans.M24,
                trans.M31, trans.M32, trans.M33, trans.M34,
                trans.M41, trans.M42, trans.M43, trans.M44,

                trans.M11, trans.M12, trans.M13, trans.M14,
                trans.M21, trans.M22, trans.M23, trans.M24,
                trans.M31, trans.M32, trans.M33, trans.M34,
                trans.M41, trans.M42, trans.M43, trans.M44,

                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
            ];

            float radTarget = Convert.ToSingle(Math.Atan2(trans.M12, trans.M11));
            _m.WriteMemory($"{pointerToEntity},{_offsetTransformation}", "bytes", Memory.EndianBitConverter.ByteArrayToString(Memory.EndianBitConverter.ArrayToByteArray(values)));
            _m.WriteMemory($"{pointerToEntity},{_offsetRadTarget}", "float", radTarget.ToString());
        }

        public override void WriteEntityLocalTranslation(string pointerToEntity, Vector3 value)
        {
            var matrix = _m.ReadMatrix4($"{pointerToEntity},{_offsetTransformation}");
            matrix.Translation = value;
            WriteEntityLocalTransformation(pointerToEntity, matrix);
        }

        public override void FreezeEntityLocalTranslationX(string pointerToEntity, string value)
        {
            if (value == "")
            {
                Vector3 trans = ReadEntityLocalTranslation(pointerToEntity);
                value = trans.X.ToString();
            }

            _m.FreezeValue($"{pointerToEntity},{_offsetTransformation}+30", "float", value);
        }

        public override void FreezeEntityLocalTranslationY(string pointerToEntity, string value)
        {
            if (value == "")
            {
                Vector3 trans = ReadEntityLocalTranslation(pointerToEntity);
                value = trans.Y.ToString();
            }

            _m.FreezeValue($"{pointerToEntity},{_offsetTransformation}+34", "float", value);
        }

        public override void FreezeEntityLocalTranslationZ(string pointerToEntity, string value)
        {
            if (value == "")
            {
                Vector3 trans = ReadEntityLocalTranslation(pointerToEntity);
                value = trans.Z.ToString();
            }

            _m.FreezeValue($"{pointerToEntity},{_offsetTransformation}+38", "float", value);
        }

        public override void UnfreezeEntityLocalTranslationX(string pointerToEntity)
        {
            _m.UnfreezeValue($"{pointerToEntity},{_offsetTransformation}+30");
        }

        public override void UnfreezeEntityLocalTranslationY(string pointerToEntity)
        {
            _m.UnfreezeValue($"{pointerToEntity},{_offsetTransformation}+34");
        }

        public override void UnfreezeEntityLocalTranslationZ(string pointerToEntity)
        {
            _m.UnfreezeValue($"{pointerToEntity},{_offsetTransformation}+38");
        }

        public override float ReadEntityLocalScale(string pointerToEntity)
        {
            return _m.ReadFloat($"{pointerToEntity},{_offsetTransformation}");
        }

        public override void WriteEntityLocalScale(string pointerToEntity, float scale)
        {
            _m.WriteMemory($"{pointerToEntity},{_offsetTransformation}+0", "float", scale.ToString());
            _m.WriteMemory($"{pointerToEntity},{_offsetTransformation}+14", "float", scale.ToString());
            _m.WriteMemory($"{pointerToEntity},{_offsetTransformation}+28", "float", scale.ToString());
        }

        public override Vector3 ReadEntityLocalVelocity(string pointerToEntity)
        {
            return _m.ReadVector3($"{pointerToEntity},{_offsetVelocity}");
        }

        public override void WriteEntityLocalVelocity(string pointerToEntity, Vector3 value)
        {
            _m.WriteMemory($"{pointerToEntity},{_offsetVelocity}", "vec3", value.ToString());
        }
        #endregion

        #region World
        public override Matrix4x4 ReadEntityWorldTransformation(string pointerToEntity)
        {
            return _m.ReadMatrix4($"{pointerToEntity},{_offsetTransformation}+40");
        }

        public override Matrix4x4 ReadEntityWorldCombinedTransformation(string pointerToEntity)
        {
            return _m.ReadMatrix4($"{pointerToEntity},{_offsetTransformation}+40");
        }

        public override void WriteEntityWorldTransformation(string pointerToEntity, Matrix4x4 value)
        {
            _m.WriteMemory($"{pointerToEntity},{_offsetTransformation}+40", "mat4", value.ToString());
        }
        #endregion

        #region Final
        public override Vector3 ReadEntityFinalCombinedTranslation(string pointerToEntity)
        {
            return ReadEntityWorldTransformation(pointerToEntity).Translation;
        }

        public override Matrix4x4 ReadEntityFinalTransformation(string pointerToEntity)
        {
            return ReadEntityWorldTransformation(pointerToEntity);
        }

        public override Matrix4x4 ReadEntityFinalCombinedTransformation(string pointerToEntity)
        {
            return ReadEntityWorldCombinedTransformation(pointerToEntity);
        }

        public override void WriteEntityFinalTransformation(string pointerToEntity, Matrix4x4 value)
        {
            if (!EntityHasTransformation(pointerToEntity))
            {
                return;
            }

            _m.WriteMemory($"{pointerToEntity},{_offsetTransformation}+40", "mat4", value.ToString());
        }
        #endregion

        #endregion

        #region Active character
        public override bool IsActCharAvailable()
        {
            return _m.ReadInt(ActiveCharacterPointer) != 0;
        }

        private void SetActiveCharacterPointer()
        {
            var slyPointer = _m.ReadInt(SlyEntityPointer);
            if (slyPointer == 0)
            {
                // Treasure in the depths
                ActiveCharacterPointer = ActiveCharacterVehiclePointer;
                return;
            }

            var slyFlag = _m.ReadInt($"{SlyEntityPointer},1C");
            if (slyFlag == 0)
            {
                ActiveCharacterPointer = ActiveCharacterVehiclePointer;
                return;
            }

            ActiveCharacterPointer = SlyEntityPointer;
        }

        public override string GetActCharPointer()
        {
            return ActiveCharacterPointer;
        }

        public override int ReadActCharId()
        {
            return Characters.FirstOrDefault().Id;
        }

        public override void WriteActCharId(int id)
        {
            // Only sly is playable
            throw new NotImplementedException();
        }

        public override void FreezeActCharId(string value)
        {
            throw new NotImplementedException();
        }

        public override void UnfreezeActCharId()
        {
            throw new NotImplementedException();
        }

        public override int ReadActCharHealth()
        {
            return _m.ReadInt(LivesAddress);
        }

        public override void WriteActCharHealth(int value)
        {
            _m.WriteMemory(LivesAddress, "int", value.ToString());
        }

        public override void FreezeActCharHealth(int value)
        {
            if (value == 0)
            {
                value = ReadActCharHealth();
            }

            _m.FreezeValue(LivesAddress, "int", value.ToString());
        }

        public override void UnfreezeActCharHealth()
        {
            _m.UnfreezeValue(LivesAddress);
        }

        public int ReadLuckyCharms()
        {
            return _m.ReadInt(LuckyCharmsAddress);
        }

        public void WriteLuckyCharms(int value)
        {
            _m.WriteMemory(LuckyCharmsAddress, "int", value.ToString());
        }

        public void FreezeLuckyCharms(string value = "")
        {
            if (value == "")
            {
                value = ReadLuckyCharms().ToString();
            }

            _m.FreezeValue(LuckyCharmsAddress, "int", value);
        }

        public void UnfreezeLuckyCharms()
        {
            _m.UnfreezeValue(LuckyCharmsAddress);
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

        public override void FreezeActCharLocalTranslationY(string value = "")
        {
            FreezeEntityLocalTranslationY(ActiveCharacterPointer, value);
        }

        public override void FreezeActCharLocalTranslationZ(string value = "")
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

            _m.FreezeValue($"{ActiveCharacterPointer},{_offsetVelocity}+8", "float", value);
        }

        public override void UnfreezeActCharVelocityZ()
        {
            _m.UnfreezeValue($"{ActiveCharacterPointer},{_offsetVelocity}+8");
        }

        public override float ReadActCharSpeedMultiplier()
        {
            return _m.ReadFloat($"{ActiveCharacterPointer},{_offsetSpeedMultiplier}");
        }

        public override void WriteActCharSpeedMultiplier(float value)
        {
            value = value * 500;
            _m.WriteMemory($"{ActiveCharacterPointer},{_offsetSpeedMultiplier}", "float", value.ToString());
        }

        public override void FreezeActCharSpeedMultiplier(float value)
        {
            if (value == 0)
            {
                value = ReadActCharSpeedMultiplier();
            }

            value = value * 400;
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
            throw new NotImplementedException();
        }

        public override void ToggleInvulnerable(bool enableInvulnerable)
        {
            throw new NotImplementedException();
        }

        public override void ToggleInfiniteDbJump(bool enableInfDbJump)
        {
            string offset1 = "2228";
            string offset2 = "2498";
            if (Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCDemo]
             || Build.Region == Util.BuildRegions[Util.BUILD_NAME.PALDemoPlayStationExperience])
            {
                offset1 = "2008";
                offset2 = "2250";
            }
            else if (Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCDemoJune14])
            {
                offset1 = "20E8";
                offset2 = "22E4";
            }
            else if (Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCMay19]
                  || Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCMay21])
            {
                offset1 = "2028";
                offset2 = "2278";
            }
            else if (Build.Region.Contains("PS3"))
            {
                offset1 = "2218";
                offset2 = "2488";
            }

            if (enableInfDbJump)
            {
                _m.FreezeValue($"{ActiveCharacterPointer},{offset1}", "int", "-1");
                _m.FreezeValue($"{ActiveCharacterPointer},{offset2}", "int", "1");
            }
            else
            {
                _m.UnfreezeValue($"{ActiveCharacterPointer},{offset1}");
                _m.UnfreezeValue($"{ActiveCharacterPointer},{offset2}");
            }
        }

        public override void ActCharToggleNoclip(bool enableNoclip)
        {
            // Would be cool to disable zap kind pit and other zap damages

            string value = "9";
            if (Build.Region.Contains("PS3"))
            {
                // The bits are flipped too
                value = "0x90";
            }

            if (enableNoclip)
            {
                _m.WriteMemory($"{ActiveCharacterPointer},{_offsetCollision}", "byte", $"{value}");
            }
            else
            {
                _m.WriteMemory($"{ActiveCharacterPointer},{_offsetCollision}", "byte", "0");
            }
        }
        #endregion

        #region Maps
        public override void LoadMap(int mapId)
        {
            int mapAddress = 0;
            if (Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCDemo]
             || Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCDemoJune14]
             || Build.Region == Util.BuildRegions[Util.BUILD_NAME.PALDemoPlayStationExperience])
            {
                // +4 at the end to skip "stock_objects"
                mapAddress = _m.ReadInt($"{ReloadValuesAddress},{(mapId * 4) + 4:X}");
            }
            else if (Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCMay19]
                  || Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCMay21])
            {
                mapAddress = _m.ReadInt($"{ReloadValuesAddress}+{mapId * 0x10:X}");
            }
            else
            {
                if (mapId >= 41)
                {
                    if (mapId >= 47)
                    {
                        // skip between sinking peril and a strange reunion
                        mapId--;
                    }

                    // skip between a hazardous path and burning rubber
                    mapId--;
                }

                mapAddress = Convert.ToInt32(ReloadValuesAddress, 16);
                int languageId = _m.ReadInt(LanguageAddress);

                mapAddress += mapId * ReloadValuesStructSize; // Go to the map
                mapAddress += 0x20 * languageId; // Go to the language
            }

            _m.WriteMemory($"{ReloadAddress}+10", "int", $"0x{mapAddress:X}");
            ReloadMap();
        }

        public override void LoadMap(int mapId, int entranceValue)
        {
            _m.WriteMemory($"{ReloadAddress}+14", "int", $"{entranceValue}");
            LoadMap(mapId);
        }

        public override void LoadMap(int mapId, int entranceValue, int mode)
        {
            throw new NotImplementedException();
        }

        public override int ReadMapId()
        {
            int worldId = _m.ReadInt(WorldIdAddress);
            int mapId = _m.ReadInt(MapIdAddress);

            if (Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCDemo]
             || Build.Region == Util.BuildRegions[Util.BUILD_NAME.PALDemoPlayStationExperience])
            {
                if (worldId == 0)
                {
                    // 00 | 0 | 2 | splash |
                    // 01 | 0 | 3 | attract |
                    return mapId - 2;
                }
                else if (worldId == 3)
                {
                    // 02 | 3 | 0 | uw_exterior_approach | Outside Raleigh's
                    // 03 | 3 | 1 | uw_exterior_boat | Raleigh's Retreat
                    // 04 | 3 | 2 | uw_bonus_security | Moolah Museum
                    // into the machine (skip)
                    // 06 | 3 | 4 | uw_bonus_library | Readin' Room
                    return mapId + 2;
                }
            }
            else if (Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCDemoJune14])
            {
                if (worldId == 0)
                {
                    if (mapId == 2)
                    {
                        // 00 | 0 | 2 | splash |
                        return 0;
                    }
                    else if (mapId == 3)
                    {
                        // attract (skip)
                        // 02 | 0 | 3 | jb_intro | Paris, France
                        return 2;
                    }
                }
                else if (worldId == 2)
                {
                    // 03 | 2 | 0 | uw_exterior_approach | Outside Raleigh's
                    // 04 | 2 | 1 | uw_exterior_boat | Raleigh's Retreat
                    // 05 | 2 | 2 | uw_bonus_security | Moolah Museum
                    // into the machine (skip)
                    // 07 | 2 | 4 | uw_bonus_library | Readin' Room
                    return mapId + 3;
                }
            }
            else if (Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCMay19]
                  || Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCMay21])
            {
                if (worldId == 0)
                {
                    // 00 | 0 | 2 | splash |
                    // 01 | 0 | 3 | jb_intro |
                    return mapId - 2;
                }

                // Inaccessible world
                //if (worldId == 1)
                //{
                //    // 02 | 1 | 0 | s_approach | Snow Approach
                //    // 03 | 1 | 1 | s_hub | Snow Hub
                //    // 04 | 1 | 2 | s_security | Snow Security
                //    // 05 | 1 | 3 | s_barrel | Snow Barrel
                //    // 06 | 1 | 4 | s_sniper | Snow Sniper
                //    // 07 | 1 | 5 | s_inspector | Snow Inspector
                //    // 08 | 1 | 6 | s_tank | Snow Tank
                //    // 09 | 1 | 7 | s_suv | Snow SUV
                //    // 10 | 1 | 8 |  |
                //    return mapId + 2;
                //}

                if (worldId == 2)
                {
                    // 11 | 2 | 0 | uw_exterior_approach | Outside Raleigh's | a stealthy approach
                    // 12 | 2 | 1 | uw_exterior_boat | Raleigh's Retreat | prowling the grounds
                    // 13 | 2 | 2 | uw_bonus_security | Moolah Museum | high class heist
                    // 14 | 2 | 3 | uw_bonus_drivewheels_final | Lava Lair | the fire down below
                    // 15 | 2 | 4 | uw_bonus_library | Readin' Room | a cunning disguise
                    // 16 | 2 | 5 | uw_t3_final | Gunboat Graveyard | gunboat graveyard
                    // 17 | 2 | 6 | uw_rip_off | Crab Cave | treasure in the depths
                    // 18 | 2 | 7 | uw_c2_final | Crankshaft Cellar | into the machine
                    // 19 | 2 | 8 | uw_boss_blimp | The Blimp | eye of the storm
                    return mapId + 11;
                }

                if (worldId == 3)
                {
                    // 20 | 3 | 0 | ms_approach | Outside Mesa City | a rocky start
                    // 21 | 3 | 1 | ms_exterior | Mesa City | muggshot's turf
                    // 22 | 3 | 2 | ms_casino | Muggshot's Casino | boneyard casino
                    // 23 | 3 | 3 | ms_inspector | The Ruins | two to tango
                    // 24 | 3 | 4 | ms_suv | The Muggshot 300 | at the dog track
                    // 25 | 3 | 5 | ms_rooftop | Muggshot's Rooftop | straight to the top
                    // 26 | 3 | 6 | ms_vertigo | Vertigo Alley | back alley heist
                    // 27 | 3 | 7 | ms_sniper | Canine Canyon | murray big gamble
                    // 28 | 3 | 8 | ms_boss_temp_focus_test | Muggshot | last call (UNUSED)
                    return 20 + mapId;
                }
            }

            if (worldId == 0)
            {
                if (mapId < 2)
                {
                    return -1;
                }

                // 2 -> 0 for splash
                // 3 -> 1 for paris
                // 4 -> 2 for hideout
                return mapId - 2;
            }

            var tmp = (worldId - 1) * _levelCount;
            tmp = tmp + mapId; // from 0 to 8
            tmp = tmp + 3; // splash, paris and hideout
            return tmp;
        }

        public void ReloadMap()
        {
            _m.WriteMemory($"{ReloadAddress}+C", "int", "1");
            _m.WriteMemory($"{ReloadAddress}+20", "int", "1");
            _m.WriteMemory($"{ReloadAddress}", "int", "1");
        }
        #endregion

        public override void SkipCurrentDialogue()
        {
            int currentDialogue = _m.ReadInt(DialoguePointer);
            if (currentDialogue == 0)
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

            string offset = "2E8";
            if (Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCMay19]
             || Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCMay21])
            {
                offset = "2F8";
            }

            _m.WriteMemory($"{DialoguePointer},{offset}", "int", "0");
        }

        protected override List<Character_t> GetCharacters()
        {
            return new()
            {
                new("Sly", 1)
            };
        }

        protected override List<Warp_t> GetEntranceLocations()
        {
            List<(int id, Warp_t warp)> entrances = new();
            string entrance = _m.ReadInt(EntranceRootNodePointer).ToString("X");

            string splicePointerOffset = "2C";
            if (Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCMay19]
             || Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCMay21])
            {
                splicePointerOffset = "30";
            }

            while (entrance != "0")
            {
                int id = _m.ReadInt($"{entrance}+8");
                Matrix4x4 trans = _m.ReadMatrix3($"{entrance}+50");
                trans.Translation = _m.ReadVector3($"{entrance}+40");
                string str = $"Entrance {id:X} [{entrance:X}]";
                int splicePointer = _m.ReadInt($"{entrance}+{splicePointerOffset}");
                if (splicePointer != 0)
                {
                    str += " (Splice)";
                }

                entrances.Add(new(id, new(str, trans)));
                entrance = _m.ReadInt($"{entrance}+24").ToString("X");
            }

            return entrances.OrderBy(e => e.id).Select(e => e.warp).ToList();
        }

        private enum GADGET_NAME
        {
            Carmelita,
            Dive,
            Roll,
            Slow1,
            SmashDive,
            CoinMagnet,
            Mine,
            Fast,
            NoWaterDamage,
            Decoy,
            ElectricRoll,
            ComputerHacking,
            Slow2,
            InvisibilitySneak,
            NoPitDamage,
            Stun,
            Invisibility,
            BlueprintsPandaKing,
            BlueprintsRaleigh,
            BlueprintsMuggshot,
            BlueprintsMzRuby,

            BinocucomClues,
            BinocucomCoins,
            BinocucomGrab,
            BinocucomHide,
            BinocucomTertiary,
            BinocucomBreakables,
            BinocucomShortCuts,
            BinocucomScan,
            InvisibilityAfterAWhile,
            InvisibilityRun,
            MaxCharm3,
            MaxCharm4,
        }

        private static Dictionary<GADGET_NAME, string> _gadgetNames = new()
        {
            [GADGET_NAME.Carmelita] = "Carmelita Fox's important files",
            [GADGET_NAME.Dive] = "Dive (Fast Attack Dive Move)",
            [GADGET_NAME.Roll] = "Roll (Fast Getaway Raccoon Roll)",
            [GADGET_NAME.Slow1] = "Slow (Slow Motion Jumps)",
            [GADGET_NAME.SmashDive] = "Dive 2 (Dive Collection)",
            [GADGET_NAME.CoinMagnet] = "Coin Magnet",
            [GADGET_NAME.Mine] = "Mine (Explosive Hat)",
            [GADGET_NAME.Fast] = "Fast (Speed Up the Clock)",
            [GADGET_NAME.NoWaterDamage] = "No water damage (Water Safety)",
            [GADGET_NAME.Decoy] = "Decoy (Thief Replica)",
            [GADGET_NAME.ElectricRoll] = "Roll (Electric)",
            [GADGET_NAME.ComputerHacking] = "Computer Hacking",
            [GADGET_NAME.Slow2] = "Slow 2 (Perpetual Slow Motion)",
            [GADGET_NAME.InvisibilitySneak] = "Invisibility Sneak",
            [GADGET_NAME.NoPitDamage] = "No pit damage (Briefly Defy Gravity)",
            [GADGET_NAME.Stun] = "Stun (Time Stopper)",
            [GADGET_NAME.Invisibility] = "Invisibility",
            [GADGET_NAME.BlueprintsPandaKing] = "Blueprints (Panda King)",
            [GADGET_NAME.BlueprintsRaleigh] = "Blueprints (Raleigh)",
            [GADGET_NAME.BlueprintsMuggshot] = "Blueprints (Muggshot)",
            [GADGET_NAME.BlueprintsMzRuby] = "Blueprints (Mz. Ruby)",

            [GADGET_NAME.BinocucomClues] = "Binocucom - Clues",
            [GADGET_NAME.BinocucomCoins] = "Binocucom - Coins",
            [GADGET_NAME.BinocucomGrab] = "Binocucom - Grab",
            [GADGET_NAME.BinocucomHide] = "Binocucom - Hide",
            [GADGET_NAME.BinocucomTertiary] = "Binocucom - Tertiary",
            [GADGET_NAME.BinocucomBreakables] = "Binocucom - Breakables",
            [GADGET_NAME.BinocucomShortCuts] = "Binocucom - Short Cuts",
            [GADGET_NAME.BinocucomScan] = "Binocucom - Scan",
            [GADGET_NAME.InvisibilityAfterAWhile] = "Invisibility (after a while)",
            [GADGET_NAME.InvisibilityRun] = "Invisibility (run)",
            [GADGET_NAME.MaxCharm3] = "MaxCharm3",
            [GADGET_NAME.MaxCharm4] = "MaxCharm4",
        };

        protected override List<List<Gadget_t>> GetGadgets()
        {
            return new()
            {
                new()
                {
                    new(_gadgetNames[GADGET_NAME.Carmelita], 0x0),
                    new(_gadgetNames[GADGET_NAME.Dive], 0x1), // Dive
                    new(_gadgetNames[GADGET_NAME.Roll], 0x2), // Roll
                    new(_gadgetNames[GADGET_NAME.Slow1], 0x3), // Slow
                    new(_gadgetNames[GADGET_NAME.SmashDive], 0x4), // Smash Dive
                    new(_gadgetNames[GADGET_NAME.CoinMagnet], 0x5),
                    new(_gadgetNames[GADGET_NAME.Mine], 0x6), // Mine
                    new(_gadgetNames[GADGET_NAME.Fast], 0x7), // Fast
                    new(_gadgetNames[GADGET_NAME.NoWaterDamage], 0x8),
                    new(_gadgetNames[GADGET_NAME.Decoy], 0x9), // Decoy
                    new(_gadgetNames[GADGET_NAME.ElectricRoll], 0xA),
                    new(_gadgetNames[GADGET_NAME.ComputerHacking], 0xB),
                    new(_gadgetNames[GADGET_NAME.Slow2], 0xC), // Slow 2
                    new(_gadgetNames[GADGET_NAME.InvisibilitySneak], 0xD), // Move while staying invisible
                    new(_gadgetNames[GADGET_NAME.NoPitDamage], 0xE), // No Pit Damage
                    new(_gadgetNames[GADGET_NAME.Stun], 0xF), // Stun
                    new(_gadgetNames[GADGET_NAME.Invisibility], 0x10),
                    new(_gadgetNames[GADGET_NAME.BlueprintsPandaKing], 0x1C),
                    new(_gadgetNames[GADGET_NAME.BlueprintsRaleigh], 0x1D),
                    new(_gadgetNames[GADGET_NAME.BlueprintsMuggshot], 0x1E),
                    new(_gadgetNames[GADGET_NAME.BlueprintsMzRuby], 0x1F),
                }
            };
        }

        protected override List<Map_t> GetMaps()
        {
            return new()
            {
                new("Splash",
                    new()
                    {
                        new(),
                    }
                ),
                new("Paris",
                    new()
                    {
                        new("Start", -2660, 850, -500),
                        new("Vent", 1300, 850, -400),
                        new("Elevator", 1440, 1620, -500),
                        new("Window", 150, -600, -2200),
                        new("Safe", 1300, 0, -2300),
                        new("Le Exit", 3000, 600, -4400),
                        new("Van", 7600, 600, -4400),
                    }
                ),
                new("Hideout",
                    new()
                    {
                        new(),
                    }
                ),
                new("A Stealthy Approach",
                    new()
                    {
                        new("Start", 5588, -23622, 800),
                        new("Boat", -1570, -22000, 600),
                        new("Door", 2200, -18600, 700),
                        new("Waterfall", 1000, -11500, 1000),
                        new("Hook", -5700, -13000, 800),
                        new("Safe", -14700, -10000, 0),
                        new("Key", -15700, -8000, 0),
                    }
                ),
                new($"{SubMapNamePrefix}Prowling the Grounds",
                    new()
                    {
                        new("Start", -15654, -12032, 700),
                        new("Platform", -15800, -7850, 700),
                        new("Fountain", -14926, -3489, 0),
                        new("Tube", -11500, -3000, 200),
                        new("Cannon", -6800, -800, 200),
                        new("Submarine", -10643, 2477, 400),
                    }
                ),
                new($"{SubMapNamePrefix}High Class Heist",
                    new()
                    {
                        new("Start", -12384, 2114, -100),
                        new("Laser arena", -4800, 0, -200),
                        new("Safe", 0, -1500, -700),
                        new("Bridge", 0, 0, -400),
                        new("Spotlights", 6950, 1900, -700),
                        new("Key", 5800, -300, -400),
                    }
                ),
                new($"{SubMapNamePrefix}Into the Machine",
                    new()
                    {
                        new("Start", 2610, 6071, 400),
                        new("Tube", 2160, -200, 1000),
                        new("Spinning fans", 13200, -300, -1800),
                        new("Hook", 24000, -400, -800),
                        new("Machine", 36100, 3200, -3500),
                        new("Safe", 37800, 3700, -3200),
                        new("Key", 45300, 3800, -2000),
                    }
                ),
                new($"{SubMapNamePrefix}A Cunning Disguise",
                    new()
                    {
                        new("Start", -9660, 1020, -3500),
                        new("Safe", -1050, -5800, -3700),
                        new("Key", -2800, 0, -3600),
                    }
                ),
                new($"{SubMapNamePrefix}The Fire Down Below",
                    new()
                    {
                        new("Start", -4237, -5634, 0),
                        new("Safe", -5200, -4800, 0),
                        new("Wheel 1", -3200, 1000, 100),
                        new("Wheel 2", -3000, 8600, 600),
                        new("Key", -1710, 10850, 1300),
                    }
                ),
                new($"{SubMapNamePrefix}Treasure in the Depths",
                    new()
                    {
                        new("Start", -1873, 10, 349),
                    }
                ),
                new($"{SubMapNamePrefix}The Gunboat Graveyard",
                    new()
                    {
                        new("Start", 810, 3560, 150),
                        new("Plane", -2200, -3700, 1200),
                        new("Submarine", 850, -7600, 50),
                        new("Key", -5300, -7800, 900),
                    }
                ),
                new($"{SubMapNamePrefix}The Eye of the Storm",
                    new()
                    {
                        new("Start", -1200, 0, 100),
                    }
                ),
                new("A Rocky Start",
                    new()
                    {
                        new("Start", -3551, 3794, 319),
                        new("Cletus", 1000, 0, 100),
                        new("Bus", 5000, -2500, 800),
                        new("Hydraulic press", 11200, 2000, 1300),
                        new("Safe", 15800, 1800, 1900),
                        new("Tilting bus", 16000, -2500, 1600),
                        new("Key", 17200, -12500, 2000),
                    }
                ),
                new($"{SubMapNamePrefix}Muggshot's Turf",
                    new()
                    {
                        new("Start", -6245, 3136, 400),
                        new("Bridge", -3839, 19, 600),
                        new("Casino", 4700, 0, 600),
                    }
                ),
                new($"{SubMapNamePrefix}Boneyard Casino",
                    new()
                    {
                        new("Start", 3629, -1391, -400),
                        new("Laser", -900, -8500, -700),
                        new("Laser 2", 600, -7400, -1200),
                        new("Pool", -8600, -9000, -1300),
                        new("Safe", -17000, -4200, -300),
                        new("Key", -15000, -5000, 200),
                    }
                ),
                new($"{SubMapNamePrefix}Murray's Big Gamble",
                    new()
                    {
                        new("Start", 4685, 651, 1457),
                    }
                ),
                new($"{SubMapNamePrefix}At the Dog Track",
                    new()
                    {
                        new("Start", -10452, 5939, 178),
                        new("Key", -6088, 3785, 566),
                    }
                ),
                new($"{SubMapNamePrefix}Two to Tango",
                    new()
                    {
                        new("Start", -8700, -200, 1200),
                        new("Chase checkpoint", -1380, 10300, 1500),
                        new("Safe", -3800, 7700, 1400),
                        new("Key", -11300, 11500, 3100),
                    }
                ),
                new($"{SubMapNamePrefix}Straight to the Top",
                    new()
                    {
                        new("Start", 342, -331, -200),
                        new("Safe", 5600, -800, 2500),
                        new("Crane", 2900, 600, 2600),
                    }
                ),
                new($"{SubMapNamePrefix}Back Alley Heist",
                    new()
                    {
                        new("Start", -4589, -1750, -300),
                        new("Dog statues", 3900, 100, 2400),
                        new("Safe", -3000, -2900, 2000),
                        new("Key", -2200, -1600, 1400),
                    }
                ),
                new($"{SubMapNamePrefix}Last Call",
                    new()
                    {
                        new("Start", -132, -801, 200),
                        new("Second stage", 0, -1500, 1900),
                        new("Third stage", 0, -1374, 3300),
                    }
                ),
                new("The Dread Swamp Path",
                    new()
                    {
                        new("Start", 7261, -4509, 1500),
                        new("Tunnel", 1300, -6800, 200),
                        new("Tents", 0, -2400, 500),
                        new("Safe", 2300, -1200, 300),
                    }
                ),
                new($"{SubMapNamePrefix}The Swamp's Dark Centre",
                    new()
                    {
                        new("Start", -8313, 319, -1400),
                        new("Hub", -3641, -2404, -2500),
                        new("W3 boss fight trigger", 531, -358, 438),
                    }
                ),
                new($"{SubMapNamePrefix}The Lair of the Beast",
                    new()
                    {
                        new("Start", -832, -6506, 300),
                        new("Checkpoint 1", 3600, -4100, 1600),
                        new("Checkpoint 2", 3700, 4400, 500),
                        new("Key", 5000, -6400, 800),
                    }
                ),
                new($"{SubMapNamePrefix}A Grave Undertaking",
                    new()
                    {
                        new("Start", -6551, 1459, 1300),
                        new("Arena", 4000, -1600, 800),
                        new("Checkpoint", 6100, -1700, 1700),
                        new("Safe", -2000, 2000, 1700),
                        new("Key", -600, 3400, 1800),
                    }
                ),
                new($"{SubMapNamePrefix}Piranha Lake",
                    new()
                    {
                        new("Start", -700, 0, 0),
                        new("Key", 3100, 0, 400),
                    }
                ),
                new($"{SubMapNamePrefix}Descent into Danger",
                    new()
                    {
                        new("Start", -9772, 6418, -516),
                        new("Checkpoint 1", -600, 0, -1000),
                        new("Checkpoint 2", -4504, 5540, 1000),
                        new("Safe", -2400, 1000, 1000),
                        new("Key", -2300, 5200, -1100),
                    }
                ),
                new($"{SubMapNamePrefix}A Ghastly Voyage",
                    new()
                    {
                        new("Start", -8440, -9864, 600),
                        new("Checkpoint 1", 6500, -13300, 300),
                        new("Checkpoint 2", 9200, -6500, 1600),
                        new("Key", 5800, -13500, 1300),
                    }
                ),
                new($"{SubMapNamePrefix}Down Home Cooking",
                    new()
                    {
                        new("Start", 3, 1625, 200),
                    }
                ),
                new($"{SubMapNamePrefix}A Deadly Dance",
                    new()
                    {
                        new("Phase 1 start", -5533, 131, 100),
                        new("Phase 1 end", 250, 0, 100),
                        new("Phase 2 start", 1460, -2, 100),
                        new("Phase 2 end", 7574, -22, 100),
                        new("Phase 3 start", 7160, -1011, 300),
                        new("Phase 3 end", 2300, -3600, 1400),
                        new("Phase 4 start", 2792, -2742, 1622),
                        new("Phase 4 end", 3183, 4546, 1400),
                    }
                ),
                new("A Perilous Ascent",
                    new()
                    {
                        new("Start", -8164, -8403, -4575),
                        new("Checkpoint 1", -14400, -2800, -2300),
                        new("Checkpoint 2", -6400, -8000, -600),
                        new("Checkpoint 3", 450, -1500, 300),
                        new("Safe", -1400, 500, -300),
                        new("Checkpoint 4/Exit", 0, 3500, 0),
                        new("Key", 7500, 4900, 100),
                    }
                ),
                new($"{SubMapNamePrefix}Inside the Stronghold",
                    new()
                    {
                        new("Start", -3428, -556, -3300),
                        new("Hub", -1527, 476, -2101),
                    }
                ),
                new($"{SubMapNamePrefix}Flaming Temple of Flame",
                    new()
                    {
                        new("Start", -2159, 550, 100),
                        new("Checkpoint 1", 5200, -8200, -700),
                        new("Checkpoint 2", -500, -6000, -200),
                        new("Gong", 2600, -5000, 800),
                        new("Laser floor", 12500, 0, 2700),
                        new("Key", 10400, -2260, 2600),
                    }
                ),
                new($"{SubMapNamePrefix}The Unseen Foe",
                    new()
                    {
                        new("Start", -14526, -5006, -200),
                        new("Moving laser", -4600, -8900, 150),
                        new("Checkpoint 1", -6780, -4842, 700),
                        new("Checkpoint 2", -6908, -5808, 1700),
                        new("Safe", -8100, -9600, 3400),
                        new("Key", -8970, -9450, -100),
                    }
                ),
                new($"{SubMapNamePrefix}The King of the Hill",
                    new()
                    {
                        new("Start", -3124, 39, 737),
                    }
                ),
                new($"{SubMapNamePrefix}Rapid Fire Assault",
                    new()
                    {
                        new("Start", -1062, 3691, 100),
                        new("Door 1", 6500, 1600, -50),
                        new("Door 2", 7100, -1100, -900),
                        new("Key", 2900, 500, -1200),
                    }
                ),
                new($"{SubMapNamePrefix}Duel by the Dragon",
                    new()
                    {
                        new("Start", -10617, -3961, -2500),
                        new("Chase start", -5800, 1500, -2300),
                        new("Checkpoint 1", 1826, 4585, -2000),
                        new("Checkpoint 2", 5400, -2600, -1700),
                        new("Safe", 400, -3700, -1700),
                        new("Key", -1900, -2600, -800),
                    }
                ),
                new($"{SubMapNamePrefix}A Desperate Race",
                    new()
                    {
                        new("Start", 8306, -2161, 1100),
                        new("Key", -4033, 3810, 294),
                    }
                ),
                new($"{SubMapNamePrefix}Flame Fu!",
                    new()
                    {
                        new("Arena", -2687, 0, -100),
                    }
                ),
                new("A Hazardous Path",
                    new()
                    {
                        new("Start", 8570, -28669, 600),
                    }
                ),
                new("_dummy1",
                    new(),
                    false
                ),
                new($"{SubMapNamePrefix}Burning Rubber",
                    new()
                    {
                        new("Start", -5368, 1446, 300),
                    }
                ),
                new($"{SubMapNamePrefix}A Daring Rescue",
                    new()
                    {
                        new("Start", 429, 2111, 400),
                    }
                ),
                new($"{SubMapNamePrefix}Bentley Comes Through",
                    new()
                    {
                        new("Start", -1191, 0, 63),
                    }
                ),
                new($"{SubMapNamePrefix}A Temporary Truce",
                    new()
                    {
                        new("Start", 279, -201, 1025),
                    }
                ),
                new($"{SubMapNamePrefix}Sinking Peril",
                    new()
                    {
                        new("Start", -291, -880, 292),
                    }
                ),
                new("_dummy2",
                    new(),
                    false
                ),
                new($"{SubMapNamePrefix}A Strange Reunion",
                    new()
                    {
                        new("Start", -3728, -5855, 743),
                        new("To Clockwerk", 1988, 2489, 600),
                        new("Clockwerk", 825, -2700, 700),
                    }
                ),
            };
        }
    }
}
