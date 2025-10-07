using System.Numerics;
using static SlyMultiTrainer.Util;

namespace SlyMultiTrainer
{
    public class Sly1Handler : GameBase_t
    {
        public string GameStatePointer = "";
        public string ReloadAddress = "";
        public string ReloadValuesAddress = "";
        public int ReloadValuesStructSize; // 0x20 for each language + 0xC. Ntsc has 1, PAL has 5, jap has 2
        public string IsLoadingAddress = "";
        public string LanguageAddress = "";
        public string LivesAddress = "";
        public string LuckyCharmsAddress = "";
        public string CameraPointer = "";
        public string WorldIdAddress = "";
        public string ControllerAddress = "";
        public string DialoguePointer = "";
        public string SlyEntityPointer = "";
        public string ActiveCharacterVehiclePointer = "";
        public string ActiveCharacterPointer = "";
        public string TreasureInTheDepthsChestCountPointer = "";
        public string RaceLapsCountPointer = "";
        public string RaceNitrosCountPointer = "";
        public string PiranhaLakeFishCountPointer = "";
        public string PiranhaLakeTorchDownHomeCookingChickenCountPointer = "";
        //public string TimerAddress = "";
        public string BurningRubberFireSlugsComputerCountPointer = "";
        public string BurningRubberComputerCountPointer = "";
        public string BentleyComesThroughChipCountPointer = "";

        private string _offsetTransformationPosition = "10"; // D0 + 30 = 100. We add the last digit later
        private string _offsetTransformationVelocityZ = "15";
        private string _offsetCollider = "3F8";
        private string _offsetSpeedMultiplier = "700";

        private Memory.Mem _m;
        private Form1 _form;

        public Sly1Handler(Memory.Mem m, Form1 form, string region) : base(m, form, region)
        {
            _m = m;
            _form = form;

            if (region == "NTSC")
            {
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
                LanguageAddress = "";
                IsLoadingAddress = $"{ReloadAddress}";
                ClockAddress = "261854";
                CameraPointer = "261990";
                DrawDistanceAddress = $"{CameraPointer},B0";
                FOVAddress = $"{CameraPointer},1C8";
                ResetCameraAddress = $"{CameraPointer},220";
                ControllerAddress = "262D18";
                DialoguePointer = "27051C";
                SlyEntityPointer = "262E10";
                ActiveCharacterVehiclePointer = "269C98";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                TreasureInTheDepthsChestCountPointer = "26D32C";
                RaceLapsCountPointer = "26D82C";
                RaceNitrosCountPointer = "26DAAC";
                PiranhaLakeFishCountPointer = "26E744";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "26D5AC";
                //TimerAddress = "26E9C8";
                BurningRubberFireSlugsComputerCountPointer = "272914";
                BurningRubberComputerCountPointer = "272694";
                BentleyComesThroughChipCountPointer = "26DFAC";
            }
            else if (region == "PAL")
            {
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
                ControllerAddress = "263F80";
                DialoguePointer = "276448";
                SlyEntityPointer = "264070";
                ActiveCharacterVehiclePointer = "26AF08";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                TreasureInTheDepthsChestCountPointer = "26F6AC";
                RaceLapsCountPointer = "2701CC";
                RaceNitrosCountPointer = "27075C";
                PiranhaLakeFishCountPointer = "272344";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "26FC3C";
                //TimerAddress = "";
                BurningRubberFireSlugsComputerCountPointer = "279EC4";
                BurningRubberComputerCountPointer = "279934";
                BentleyComesThroughChipCountPointer = "27127C";
            }
            else if (region == "NTSC-J")
            {
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
                ControllerAddress = "263230";
                DialoguePointer = "275708";
                SlyEntityPointer = "263320";
                ActiveCharacterVehiclePointer = "26A1B8";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                TreasureInTheDepthsChestCountPointer = "26E96C";
                RaceLapsCountPointer = "26F48C";
                RaceNitrosCountPointer = "26FA1C";
                PiranhaLakeFishCountPointer = "271604";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "26EEFC";
                //TimerAddress = "";
                BurningRubberFireSlugsComputerCountPointer = "278D64";
                BurningRubberComputerCountPointer = "2787D4";
                BentleyComesThroughChipCountPointer = "27053C";
            }
            else if (region == "NTSC-K")
            {
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
                ControllerAddress = "263520";
                DialoguePointer = "2759E8";
                SlyEntityPointer = "263610";
                ActiveCharacterVehiclePointer = "26A4A8";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                TreasureInTheDepthsChestCountPointer = "26EC4C";
                RaceLapsCountPointer = "26F76C";
                RaceNitrosCountPointer = "26FCFC";
                PiranhaLakeFishCountPointer = "2718E4";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "26F1DC";
                //TimerAddress = "";
                BurningRubberFireSlugsComputerCountPointer = "279044";
                BurningRubberComputerCountPointer = "278AB4";
                BentleyComesThroughChipCountPointer = "27081C";
            }
            //else if (region == "NTSC May 19")
            //{
            //    GameStatePointer = "276220";
            //    WorldIdAddress = $"{GameStatePointer},1078";
            //    MapIdAddress = $"{GameStatePointer},107C";
            //    LivesAddress = $"{GameStatePointer},1080";
            //    LuckyCharmsAddress = $"{GameStatePointer},1084";
            //    CoinsAddress = $"{GameStatePointer},1088";
            //    GadgetAddress = $"{GameStatePointer},108C";
            //    ReloadAddress = "28B40C";
            //    ReloadValuesAddress = "";
            //    ReloadValuesStructSize = 0x2C;
            //    LanguageAddress = "";
            //    IsLoadingAddress = $"{ReloadAddress}";
            //    ClockAddress = "";
            //    CameraPointer = "";
            //    DrawDistanceAddress = $"{CameraPointer},B0";
            //    FOVAddress = $"{CameraPointer},1C8";
            //    ResetCameraAddress = $"{CameraPointer},220";
            //    ControllerAddress = "";
            //    DialoguePointer = "";
            //    SlyEntityPointer = "";
            //    ActiveCharacterVehiclePointer = "";
            //    ActiveCharacterPointer = $"{SlyEntityPointer}";
            //    TreasureInTheDepthsChestCountPointer = "";
            //    RaceLapsCountPointer = "";
            //    RaceNitrosCountPointer = "";
            //    PiranhaLakeFishCountPointer = "";
            //    PiranhaLakeTorchDownHomeCookingChickenCountPointer = "";
            //    //TimerAddress = "";
            //    BurningRubberFireSlugsComputerCountPointer = "";
            //    BurningRubberComputerCountPointer = "";
            //    BentleyComesThroughChipCountPointer = "";
            //}
            else if (region == "NTSC Demo")
            {
                _offsetCollider = "408";
                GameStatePointer = "280C10";
                WorldIdAddress = $"{GameStatePointer},1040";
                MapIdAddress = $"{GameStatePointer},1044";
                LivesAddress = $"{GameStatePointer},1048";
                LuckyCharmsAddress = $"{GameStatePointer},104C";
                CoinsAddress = $"{GameStatePointer},1050";
                GadgetAddress = $"{GameStatePointer},1058";
                ReloadAddress = "2AA28C";
                ReloadValuesAddress = ""; // Works a different way
                ReloadValuesStructSize = 0;
                LanguageAddress = "";
                IsLoadingAddress = $"{ReloadAddress}";
                ClockAddress = "2386E4";
                CameraPointer = "2387BC";
                DrawDistanceAddress = $"{CameraPointer},B0";
                FOVAddress = $""; // not here?
                ResetCameraAddress = $"{CameraPointer},208";
                ControllerAddress = "27AC60";
                DialoguePointer = "286A00";
                SlyEntityPointer = "27AD50";
                ActiveCharacterVehiclePointer = "";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                TreasureInTheDepthsChestCountPointer = "";
                RaceLapsCountPointer = "";
                RaceNitrosCountPointer = "";
                PiranhaLakeFishCountPointer = "";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "";
                //TimerAddress = "";
                BurningRubberFireSlugsComputerCountPointer = "";
                BurningRubberComputerCountPointer = "";
                BentleyComesThroughChipCountPointer = "";

                // Splash
                // Attract
                // a stealthy approach
                // prowling
                // hch
                // SKIP
                // cunning
                Maps[1].Name = "Attract";
                Maps[1].Warps = Maps[0].Warps;
                Maps.RemoveAt(2); // hideout
                Maps[5].IsVisible = false;
                Maps.Skip(7).ToList().ForEach(m => m.IsVisible = false);
            }
            else if (region == "PAL Demo")
            {
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
                ControllerAddress = "25F878";
                DialoguePointer = "26C7D8";
                SlyEntityPointer = "25F970";
                ActiveCharacterVehiclePointer = "";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                TreasureInTheDepthsChestCountPointer = "";
                RaceLapsCountPointer = "";
                RaceNitrosCountPointer = "";
                PiranhaLakeFishCountPointer = "";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "";
                //TimerAddress = "";
                BurningRubberFireSlugsComputerCountPointer = "";
                BurningRubberComputerCountPointer = "";
                BentleyComesThroughChipCountPointer = "";

                Maps[2].IsVisible = false; // hideout
                Maps[6].IsVisible = false; // itm
                Maps.Skip(8).ToList().ForEach(m => m.IsVisible = false);
            }
            else if (region == "NTSC-J Demo" || region == "NTSC-K Demo")
            {
                if (region == "NTSC-J Demo")
                {
                    GameStatePointer = "26704C";
                    ReloadValuesAddress = "24E0B0";
                    ReloadValuesStructSize = 0x4C;
                }
                else
                {
                    GameStatePointer = "267048";
                    ReloadValuesAddress = "24E670";
                    ReloadValuesStructSize = 0x2C;
                }
                WorldIdAddress = $"{GameStatePointer},19D8";
                MapIdAddress = $"{GameStatePointer},19DC";
                LivesAddress = $"{GameStatePointer},19E0";
                LuckyCharmsAddress = $"{GameStatePointer},19E4";
                CoinsAddress = $"{GameStatePointer},19E8";
                GadgetAddress = $"{GameStatePointer},19F0";

                ReloadAddress = "27D504";
                IsLoadingAddress = $"{ReloadAddress}";
                LanguageAddress = ""; // You can't set it to english? 26706C
                ClockAddress = "2664C4";
                CameraPointer = "266600";
                DrawDistanceAddress = $"{CameraPointer},B0";
                FOVAddress = $"{CameraPointer},1C8";
                ResetCameraAddress = $"{CameraPointer},220";
                ControllerAddress = "267718";
                DialoguePointer = "274678";
                SlyEntityPointer = "267810";
                ActiveCharacterVehiclePointer = "2696F8";
                ActiveCharacterPointer = $"{SlyEntityPointer}";
                TreasureInTheDepthsChestCountPointer = "";
                RaceLapsCountPointer = "";
                RaceNitrosCountPointer = "";
                PiranhaLakeFishCountPointer = "";
                PiranhaLakeTorchDownHomeCookingChickenCountPointer = "";
                //TimerAddress = "";
                BurningRubberFireSlugsComputerCountPointer = "";
                BurningRubberComputerCountPointer = "";
                BentleyComesThroughChipCountPointer = "";

                Maps[2].IsVisible = false;
                Maps[6].IsVisible = false;

                Maps.Skip(8).ToList().ForEach(m => m.IsVisible = false);
            }

            _form.UpdateUI(_form.cmbMaps, Maps.Where(x => x.IsVisible).ToList());
            _form.UpdateUI(_form.cmbMaps, Maps, "Tag");
            _form.UpdateUI(_form.cmbActChar, Characters);
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
                TabPage tabPage = null;
                string tabName2 = "";
                _form.UpdateUI(() =>
                {
                    tabPage = _form.tabControlWorldStates.SelectedTab!;
                    tabName2 = tabPage.Name;
                });
                int worldId = Convert.ToInt32(tabName2.Last().ToString());

                // If it's the first time we switched to this world state tab, fill it with the right controls
                if (tabPage!.Controls.Count == 0)
                {
                    string mapNameStart = "";
                    switch (worldId)
                    {
                        case 1:
                            mapNameStart = "A Stealthy Approach";
                            break;
                        case 2:
                            mapNameStart = "A Rocky Start";
                            break;
                        case 3:
                            mapNameStart = "The Dread Swamp Path";
                            break;
                        case 4:
                            mapNameStart = "A Perilous Ascent";
                            break;
                        case 5:
                            mapNameStart = "A Hazardous Path";
                            break;
                    }

                    _form.UpdateUI(() =>
                    {
                        FillTabWorldState(worldId, Maps.FindIndex(x => x.Name == mapNameStart));
                    });
                }

                // Update the checkboxes for each level
                int levelCount = GetLevelCount(worldId);
                for (int i = 0; i < levelCount; i++)
                {
                    int levelId = i;
                    if (worldId == 5)
                    {
                        // Skip between peril and strange
                        if (i >= 6)
                        {
                            levelId++;
                        }

                        // Skip between hazardous and rubber
                        if (i >= 1)
                        {
                            levelId++;
                        }
                    }

                    int levelFlag = ReadLevelFlag(worldId, levelId);
                    UpdateWorldStateTabLevelFlags(worldId, i, levelFlag, tabPage);
                }

                // Update the checkboxes for this world
                int worldFlag = ReadWorldFlag(worldId);
                UpdateWorldStateTabFlags(worldId, worldFlag, tabPage);
            }
        }

        public override void OnMapChange(int mapId)
        {

        }

        private void FillTabWorldState(int worldId, int mapIdStart)
        {
            TabPage tabWorldState = _form.tabControlWorldStates.TabPages[$"tabWorldState{worldId}"]!;
            TabWorldStateAddLabels(tabWorldState);
            TabWorldStateAddButtons(tabWorldState, worldId);
            TabWorldStateAddCheckboxes(tabWorldState, worldId);

            int levelCount = GetLevelCount(worldId);
            for (int i = 0; i < levelCount; i++)
            {
                // Map name
                Label lbl = new()
                {
                    Text = Maps[mapIdStart + i].Name.TrimStart(' '),
                    AutoSize = true,
                    Font = _form.lblXCoord.Font,
                    Location = new Point(0, 10 + 20 * (i + 1)),
                };
                tabWorldState.Controls.Add(lbl);

                TabWorldStateAddCheckboxesForLevel(tabWorldState, worldId, i);
            }

            TabWorldStateAddWorldFlagsInputs(tabWorldState, worldId);
        }

        private void TabWorldStateAddLabels(TabPage tabWorldState)
        {
            Label lbl = new()
            {
                Text = "Levels",
                AutoSize = true,
                Font = _form.lblXCoord.Font,
                Location = new Point(0, 0),
            };
            tabWorldState.Controls.Add(lbl);

            lbl = new()
            {
                Text = "Unlocked",
                AutoSize = true,
                Font = _form.lblXCoord.Font,
                Location = new Point(175, 0),
            };
            tabWorldState.Controls.Add(lbl);

            lbl = new()
            {
                Text = "Key",
                AutoSize = true,
                Font = _form.lblXCoord.Font,
                Location = new Point(260, 0),
            };
            tabWorldState.Controls.Add(lbl);

            lbl = new()
            {
                Text = "Safe",
                AutoSize = true,
                Font = _form.lblXCoord.Font,
                Location = new Point(330, 0),
            };
            tabWorldState.Controls.Add(lbl);

            lbl = new()
            {
                Text = "Sprint",
                AutoSize = true,
                Font = _form.lblXCoord.Font,
                Location = new Point(390, 0),
            };
            tabWorldState.Controls.Add(lbl);
        }

        private void TabWorldStateAddButtons(TabPage tabWorldState, int worldId)
        {
            Button btn = new()
            {
                Name = $"btnWorld{worldId}ToggleUnlockedToAll",
                Text = "Toggle unlocked to all",
                Location = new Point(450, 30),
                AutoSize = true
            };
            btn.Click += (s, e) => btnToggleToAll_Click(s, e, worldId, "Unlocked");
            tabWorldState.Controls.Add(btn);

            btn = new()
            {
                Name = $"btnWorld{worldId}ToggleKeyToAll",
                Text = "Toggle key to all",
                Location = new Point(450, 60),
                AutoSize = true
            };
            btn.Click += (s, e) => btnToggleToAll_Click(s, e, worldId, "Key");
            tabWorldState.Controls.Add(btn);

            btn = new()
            {
                Name = $"btnWorld{worldId}ToggleSafeToAll",
                Text = "Toggle safe to all",
                Location = new Point(450, 90),
                AutoSize = true
            };
            btn.Click += (s, e) => btnToggleToAll_Click(s, e, worldId, "Safe");
            tabWorldState.Controls.Add(btn);

            btn = new()
            {
                Name = $"btnWorld{worldId}ToggleSprintToAll",
                Text = "Toggle sprint to all",
                Location = new Point(450, 120),
                AutoSize = true
            };
            btn.Click += (s, e) => btnToggleToAll_Click(s, e, worldId, "Sprint");
            tabWorldState.Controls.Add(btn);
        }

        private void TabWorldStateAddCheckboxes(TabPage tabWorldState, int worldId)
        {
            CheckBox chk = new()
            {
                Name = $"chkWorld{worldId}Started",
                Text = "Started",
                Location = new Point(5, 230),
                AutoSize = true,
            };
            chk.Click += (s, e) => chkWorldState_Click(s, e, worldId, "Started");
            tabWorldState.Controls.Add(chk);

            chk = new()
            {
                Name = $"chkWorld{worldId}Key1",
                Text = "1 key collected",
                Location = new Point(80, 230),
                AutoSize = true,
            };
            chk.Click += (s, e) => chkWorldState_Click(s, e, worldId, "Key1");
            tabWorldState.Controls.Add(chk);

            chk = new()
            {
                Name = $"chkWorld{worldId}Key3",
                Text = "3 keys collected",
                Location = new Point(195, 230),
                AutoSize = true,
            };
            chk.Click += (s, e) => chkWorldState_Click(s, e, worldId, "Key3");
            tabWorldState.Controls.Add(chk);

            chk = new()
            {
                Name = $"chkWorld{worldId}Key7",
                Text = "7 keys collected",
                Location = new Point(310, 230),
                AutoSize = true,
            };
            chk.Click += (s, e) => chkWorldState_Click(s, e, worldId, "Key7");
            tabWorldState.Controls.Add(chk);
        }

        private void TabWorldStateAddCheckboxesForLevel(TabPage tabWorldState, int worldId, int levelId)
        {
            CheckBox chk = new()
            {
                Name = $"chkWorld{worldId}Level{levelId}Unlocked",
                Text = "",
                Size = new Size(20, 23),
                Location = new Point(200, 10 + 20 * (levelId + 1)),
            };
            chk.Click += (s, e) => chkWorldLevel_Click(s, e, "Unlocked");
            tabWorldState.Controls.Add(chk);

            chk = new()
            {
                Name = $"chkWorld{worldId}Level{levelId}Key",
                Text = "",
                Size = new Size(20, 23),
                Location = new Point(270, 10 + 20 * (levelId + 1)),
            };
            chk.Click += (s, e) => chkWorldLevel_Click(s, e, "Key");
            tabWorldState.Controls.Add(chk);

            chk = new()
            {
                Name = $"chkWorld{worldId}Level{levelId}Safe",
                Text = "",
                Size = new Size(20, 23),
                Location = new Point(340, 10 + 20 * (levelId + 1)),
            };
            chk.Click += (s, e) => chkWorldLevel_Click(s, e, "Safe");
            tabWorldState.Controls.Add(chk);

            chk = new()
            {
                Name = $"chkWorld{worldId}Level{levelId}Sprint",
                Text = "",
                Size = new Size(20, 23),
                Location = new Point(410, 10 + 20 * (levelId + 1)),
            };
            chk.Click += (s, e) => chkWorldLevel_Click(s, e, "Sprint");
            tabWorldState.Controls.Add(chk);
        }

        private void TabWorldStateAddWorldFlagsInputs(TabPage tabWorldState, int worldId)
        {
            Label lbl = new()
            {
                Text = "Keys collected",
                AutoSize = true,
                Font = _form.lblXCoord.Font,
                Location = new Point(0, 260),
            };
            tabWorldState.Controls.Add(lbl);

            TextBox txt = new()
            {
                Name = $"txtWorld{worldId}KeysCollected",
                Location = new Point(130, 260),
            };
            txt.TextChanged += (s, e) => txtWorldFlag_TextChanged(s, e, worldId, "KeysCollected");
            tabWorldState.Controls.Add(txt);

            lbl = new()
            {
                Text = "Safes opened",
                AutoSize = true,
                Font = _form.lblXCoord.Font,
                Location = new Point(0, 285),
            };
            tabWorldState.Controls.Add(lbl);

            txt = new()
            {
                Name = $"txtWorld{worldId}SafesOpened",
                Location = new Point(130, 285),
            };
            txt.TextChanged += (s, e) => txtWorldFlag_TextChanged(s, e, worldId, "SafesOpened");
            tabWorldState.Controls.Add(txt);

            lbl = new()
            {
                Text = "Sprints completed",
                AutoSize = true,
                Font = _form.lblXCoord.Font,
                Location = new Point(0, 310),
            };
            tabWorldState.Controls.Add(lbl);

            txt = new()
            {
                Name = $"txtWorld{worldId}SprintsCompleted",
                Location = new Point(130, 310),
            };
            txt.TextChanged += (s, e) => txtWorldFlag_TextChanged(s, e, worldId, "SprintsCompleted");
            tabWorldState.Controls.Add(txt);

            if (worldId == 1)
            {
                lbl = new()
                {
                    Text = "Treasure in the Depths - Chests",
                    AutoSize = true,
                    Font = _form.lblXCoord.Font,
                    Location = new Point(235, 260),
                };
                tabWorldState.Controls.Add(lbl);

                txt = new()
                {
                    Name = $"txtWorld{worldId}DepthsChestCount",
                    Location = new Point(450, 260),
                };
                txt.TextChanged += (s, e) =>
                {
                    if (!int.TryParse(txt.Text, out int value))
                    {
                        txt.Text = "";
                    }
                    WriteTreasureInTheDepthsChestCount(value);
                };
                tabWorldState.Controls.Add(txt);
            }
            else if (worldId == 2)
            {
                Label lbl1 = new()
                {
                    Text = "At the Dog Track - Nitros",
                    AutoSize = true,
                    Font = _form.lblXCoord.Font,
                    Location = new Point(235, 260),
                };
                tabWorldState.Controls.Add(lbl1);

                TextBox txt1 = new()
                {
                    Name = $"txtWorld{worldId}DogTrackNitrosCount",
                    Location = new Point(450, 260),
                };
                txt1.TextChanged += (s, e) =>
                {
                    if (!int.TryParse(txt1.Text, out int value))
                    {
                        txt1.Text = "";
                    }
                    WriteRaceNitrosCount(value);
                };
                tabWorldState.Controls.Add(txt1);

                Label lbl2 = new()
                {
                    Text = "At the Dog Track - Laps",
                    AutoSize = true,
                    Font = _form.lblXCoord.Font,
                    Location = new Point(235, 285),
                };
                tabWorldState.Controls.Add(lbl2);

                TextBox txt2 = new()
                {
                    Name = $"txtWorld{worldId}DogTrackLapsCount",
                    Location = new Point(450, 285),
                };
                txt2.TextChanged += (s, e) =>
                {
                    if (!int.TryParse(txt2.Text, out int value))
                    {
                        txt2.Text = "";
                    }
                    WriteRaceLapsCount(value);
                };
                tabWorldState.Controls.Add(txt2);
            }
            else if (worldId == 3)
            {
                // Piranha Lake
                Label lbl1 = new()
                {
                    Text = "Piranha Lake - Fish",
                    AutoSize = true,
                    Font = _form.lblXCoord.Font,
                    Location = new Point(235, 260),
                };
                tabWorldState.Controls.Add(lbl1);

                TextBox txt1 = new()
                {
                    Name = $"txtWorld{worldId}PiranhaLakeFishCount",
                    Location = new Point(450, 260),
                };
                txt1.TextChanged += (s, e) =>
                {
                    if (!int.TryParse(txt1.Text, out int value))
                    {
                        txt1.Text = "";
                    }
                    WritePiranhaLakeFishCount(value);
                };
                tabWorldState.Controls.Add(txt1);

                // Piranha Lake
                Label lbl2 = new()
                {
                    Text = "Piranha Lake - Torch",
                    AutoSize = true,
                    Font = _form.lblXCoord.Font,
                    Location = new Point(235, 285),
                };
                tabWorldState.Controls.Add(lbl2);

                TextBox txt2 = new()
                {
                    Name = $"txtWorld{worldId}PiranhaLakeTorchCount",
                    Location = new Point(450, 285),
                };
                txt2.TextChanged += (s, e) =>
                {
                    if (!int.TryParse(txt2.Text, out int value))
                    {
                        txt2.Text = "";
                    }
                    WritePiranhaLakeTorchDownHomeCookingChickenCount(value);
                };
                tabWorldState.Controls.Add(txt2);

                // Down Home Cooking
                Label lbl3 = new()
                {
                    Text = "Down Home Cooking - Chicken",
                    AutoSize = true,
                    Font = _form.lblXCoord.Font,
                    Location = new Point(235, 310),
                };
                tabWorldState.Controls.Add(lbl3);

                TextBox txt3 = new()
                {
                    Name = $"txtWorld{worldId}DownHomeCookingChickenCount",
                    Location = new Point(450, 310),
                };
                txt3.TextChanged += (s, e) =>
                {
                    if (!int.TryParse(txt3.Text, out int value))
                    {
                        txt3.Text = "";
                    }
                    WritePiranhaLakeTorchDownHomeCookingChickenCount(value);
                };
                tabWorldState.Controls.Add(txt3);
            }
            else if (worldId == 4)
            {
                Label lbl1 = new()
                {
                    Text = "A Desperate Race - Nitros",
                    AutoSize = true,
                    Font = _form.lblXCoord.Font,
                    Location = new Point(235, 260),
                };
                tabWorldState.Controls.Add(lbl1);

                TextBox txt1 = new()
                {
                    Name = $"txtWorld{worldId}ADesperateRaceNitrosCount",
                    Location = new Point(450, 260),
                };
                txt1.TextChanged += (s, e) =>
                {
                    if (!int.TryParse(txt1.Text, out int value))
                    {
                        txt1.Text = "";
                    }
                    WriteRaceNitrosCount(value);
                };
                tabWorldState.Controls.Add(txt1);

                Label lbl2 = new()
                {
                    Text = "A Desperate Race - Laps",
                    AutoSize = true,
                    Font = _form.lblXCoord.Font,
                    Location = new Point(235, 285),
                };
                tabWorldState.Controls.Add(lbl2);

                TextBox txt2 = new()
                {
                    Name = $"txtWorld{worldId}ADesperateRaceLapsCount",
                    Location = new Point(450, 285),
                };
                txt2.TextChanged += (s, e) =>
                {
                    if (!int.TryParse(txt2.Text, out int value))
                    {
                        txt2.Text = "";
                    }
                    WriteRaceLapsCount(value);
                };
                tabWorldState.Controls.Add(txt2);
            }
            else if (worldId == 5)
            {
                // "Burning Rubber
                Label lbl1 = new()
                {
                    Text = "Burning Rubber - Fire slugs computer",
                    AutoSize = true,
                    Font = _form.lblXCoord.Font,
                    Location = new Point(235, 260),
                };
                tabWorldState.Controls.Add(lbl1);

                TextBox txt1 = new()
                {
                    Name = $"txtWorld{worldId}BurningRubberSlugsComputerCount",
                    Location = new Point(480, 260),
                };
                txt1.TextChanged += (s, e) =>
                {
                    if (!int.TryParse(txt1.Text, out int value))
                    {
                        txt1.Text = "";
                    }
                    WriteBurningRubberFireSlugsComputerCount(value);
                };
                tabWorldState.Controls.Add(txt1);

                // "Burning Rubber
                Label lbl2 = new()
                {
                    Text = "Burning Rubber - Computer",
                    AutoSize = true,
                    Font = _form.lblXCoord.Font,
                    Location = new Point(235, 285),
                };
                tabWorldState.Controls.Add(lbl2);

                TextBox txt2 = new()
                {
                    Name = $"txtWorld{worldId}BurningRubberComputerCount",
                    Location = new Point(480, 285),
                };
                txt2.TextChanged += (s, e) =>
                {
                    if (!int.TryParse(txt2.Text, out int value))
                    {
                        txt2.Text = "";
                    }
                    WriteBurningRubberComputerCount(value);
                };
                tabWorldState.Controls.Add(txt2);

                // Bentley Comes Through
                Label lbl3 = new()
                {
                    Text = "Bentley Comes Through - Chip",
                    AutoSize = true,
                    Font = _form.lblXCoord.Font,
                    Location = new Point(235, 310),
                };
                tabWorldState.Controls.Add(lbl3);

                TextBox txt3 = new()
                {
                    Name = $"txtWorld{worldId}BentleyComesThroughChipCount",
                    Location = new Point(480, 310),
                };
                txt3.TextChanged += (s, e) =>
                {
                    if (!int.TryParse(txt3.Text, out int value))
                    {
                        txt3.Text = "";
                    }
                    WriteBentleyComesThroughChipCount(value);
                };
                tabWorldState.Controls.Add(txt3);
            }
        }

        private void UpdateWorldStateTabLevelFlags(int worldId, int levelId, int levelFlag, TabPage tabPage)
        {
            CheckBox chk;
            chk = (tabPage.Controls[$"chkWorld{worldId}Level{levelId}Unlocked"] as CheckBox)!;
            _form.UpdateUI(chk, (levelFlag & 0x1) == 0x1, "Checked");

            chk = (tabPage.Controls[$"chkWorld{worldId}Level{levelId}Key"] as CheckBox)!;
            _form.UpdateUI(chk, (levelFlag & 0x2) == 0x2, "Checked");

            chk = (tabPage.Controls[$"chkWorld{worldId}Level{levelId}Safe"] as CheckBox)!;
            _form.UpdateUI(chk, (levelFlag & 0x4) == 0x4, "Checked");

            chk = (tabPage.Controls[$"chkWorld{worldId}Level{levelId}Sprint"] as CheckBox)!;
            _form.UpdateUI(chk, (levelFlag & 0x8) == 0x8, "Checked");
        }

        private void UpdateWorldStateTabFlags(int worldId, int worldFlag, TabPage tabPage)
        {
            CheckBox chk;
            chk = (tabPage.Controls[$"chkWorld{worldId}Started"] as CheckBox)!;
            _form.UpdateUI(chk, (worldFlag & 0x1) == 0x1, "Checked");

            chk = (tabPage.Controls[$"chkWorld{worldId}Key1"] as CheckBox)!;
            _form.UpdateUI(chk, (worldFlag & 0x2) == 0x2, "Checked");

            chk = (tabPage.Controls[$"chkWorld{worldId}Key3"] as CheckBox)!;
            _form.UpdateUI(chk, (worldFlag & 0x4) == 0x4, "Checked");

            chk = (tabPage.Controls[$"chkWorld{worldId}Key7"] as CheckBox)!;
            _form.UpdateUI(chk, (worldFlag & 0x8) == 0x8, "Checked");

            TextBox txt = (tabPage.Controls[$"txtWorld{worldId}KeysCollected"] as TextBox)!;
            _form.UpdateUI(txt, ReadWorldKeysCollectedCount(worldId).ToString());

            txt = (tabPage.Controls[$"txtWorld{worldId}SafesOpened"] as TextBox)!;
            _form.UpdateUI(txt, ReadWorldSafesOpenedCount(worldId).ToString());

            txt = (tabPage.Controls[$"txtWorld{worldId}SprintsCompleted"] as TextBox)!;
            _form.UpdateUI(txt, ReadWorldSprintsCompletedCount(worldId).ToString());

            // Only read the values for the current map id
            var mapId = GetMapId();
            UpdateWorldStateTabForCurrentLevel(worldId, mapId, tabPage);
        }

        private void UpdateWorldStateTabForCurrentLevel(int worldId, int mapId, TabPage tabPage)
        {
            TextBox txt;
            if (mapId == 9)
            {
                txt = (tabPage.Controls[$"txtWorld{worldId}DepthsChestCount"] as TextBox)!;
                _form.UpdateUI(txt, ReadTreasureInTheDepthsChestCount().ToString());
            }
            else if (mapId == 16)
            {
                txt = (tabPage.Controls[$"txtWorld{worldId}DogTrackNitrosCount"] as TextBox)!;
                _form.UpdateUI(txt, ReadRaceNitrosCount().ToString());

                txt = (tabPage.Controls[$"txtWorld{worldId}DogTrackLapsCount"] as TextBox)!;
                _form.UpdateUI(txt, ReadRaceLapsCount().ToString());
            }
            else if (mapId == 25)
            {
                txt = (tabPage.Controls[$"txtWorld{worldId}PiranhaLakeFishCount"] as TextBox)!;
                _form.UpdateUI(txt, ReadPiranhaLakeFishCount().ToString());

                txt = (tabPage.Controls[$"txtWorld{worldId}PiranhaLakeTorchCount"] as TextBox)!;
                _form.UpdateUI(txt, ReadPiranhaLakeTorchDownHomeCookingChickenCount().ToString());
            }
            else if (mapId == 28)
            {
                txt = (tabPage.Controls[$"txtWorld{worldId}DownHomeCookingChickenCount"] as TextBox)!;
                _form.UpdateUI(txt, ReadPiranhaLakeTorchDownHomeCookingChickenCount().ToString());
            }
            else if (mapId == 37)
            {
                txt = (tabPage.Controls[$"txtWorld{worldId}ADesperateRaceNitrosCount"] as TextBox)!;
                _form.UpdateUI(txt, ReadRaceNitrosCount().ToString());

                txt = (tabPage.Controls[$"txtWorld{worldId}ADesperateRaceLapsCount"] as TextBox)!;
                _form.UpdateUI(txt, ReadRaceLapsCount().ToString());
            }
            else if (mapId == 40)
            {
                txt = (tabPage.Controls[$"txtWorld{worldId}BurningRubberSlugsComputerCount"] as TextBox)!;
                _form.UpdateUI(txt, ReadBurningRubberFireSlugsComputerCount().ToString());

                txt = (tabPage.Controls[$"txtWorld{worldId}BurningRubberComputerCount"] as TextBox)!;
                _form.UpdateUI(txt, ReadBurningRubberComputerCount().ToString());
            }
            else if (mapId == 42)
            {
                txt = (tabPage.Controls[$"txtWorld{worldId}BentleyComesThroughChipCount"] as TextBox)!;
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

        private void btnToggleToAll_Click(object sender, EventArgs e, int worldId, string type)
        {
            Button btn = sender as Button;
            TabPage tabWorldState = _form.tabControlWorldStates.SelectedTab;

            List<CheckBox> checkboxes = new();
            for (int i = 0; i < tabWorldState.Controls.Count; i++)
            {
                if (tabWorldState.Controls[i].Name.EndsWith(type))
                {
                    checkboxes.Add(tabWorldState.Controls[i] as CheckBox);
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

        private void chkWorldState_Click(object sender, EventArgs e, int worldId, string type)
        {
            CheckBox chk = sender as CheckBox;
            int worldFlags = ReadWorldFlag(worldId);

            if (type == "Started")
            {
                worldFlags = worldFlags ^ 0x1;
            }
            else if (type == "Key1")
            {
                worldFlags = worldFlags ^ 0x2;
            }
            else if (type == "Key3")
            {
                worldFlags = worldFlags ^ 0x4;
            }
            else if (type == "Key7")
            {
                worldFlags = worldFlags ^ 0x8;
            }

            WriteWorldFlag(worldId, worldFlags);
        }

        private void chkWorldLevel_Click(object sender, EventArgs e, string type)
        {
            CheckBox chk = sender as CheckBox;
            int worldId = Convert.ToInt32(chk.Name[8].ToString());
            int levelId = Convert.ToInt32(chk.Name[14].ToString());

            if (worldId == 5)
            {
                // Skip between peril and strange
                if (levelId >= 6)
                {
                    levelId++;
                }

                // Skip between hazardous and rubber
                if (levelId >= 1)
                {
                    levelId++;
                }
            }

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
            return _m.ReadInt($"{GameStatePointer},{0x10 + (0x44C * worldId) + (0x78 * levelId):X8}");
        }

        public void WriteLevelFlag(int worldId, int levelId, int levelFlag)
        {
            _m.WriteMemory($"{GameStatePointer},{0x10 + (0x44C * worldId) + (0x78 * levelId):X8}", "int", levelFlag.ToString());
        }

        public int ReadWorldKeysCollectedCount(int worldId)
        {
            return _m.ReadInt($"{GameStatePointer},{0x10 + (0x44C * worldId) + 0x438:X8}");
        }

        public void WriteWorldKeysCollectedCount(int worldId, int keysCollected)
        {
            _m.WriteMemory($"{GameStatePointer},{0x10 + (0x44C * worldId) + 0x438:X8}", "int", keysCollected.ToString());
        }

        public int ReadWorldSafesOpenedCount(int worldId)
        {
            return _m.ReadInt($"{GameStatePointer},{0x10 + (0x44C * worldId) + 0x43C:X8}");
        }

        public void WriteWorldSafesOpenedCount(int worldId, int safesOpened)
        {
            _m.WriteMemory($"{GameStatePointer},{0x10 + (0x44C * worldId) + 0x43C:X8}", "int", safesOpened.ToString());
        }

        public int ReadWorldSprintsCompletedCount(int worldId)
        {
            return _m.ReadInt($"{GameStatePointer},{0x10 + (0x44C * worldId) + 0x440:X8}");
        }

        public void WriteWorldSprintsCompletedCount(int worldId, int sprintsCompleted)
        {
            _m.WriteMemory($"{GameStatePointer},{0x10 + (0x44C * worldId) + 0x440:X8}", "int", sprintsCompleted.ToString());
        }

        public float ReadWorldTimePlayed(int worldId)
        {
            return _m.ReadFloat($"{GameStatePointer},{0x10 + (0x44C * worldId) + 0x444:X8}");
        }

        public int ReadWorldFlag(int worldId)
        {
            return _m.ReadInt($"{GameStatePointer},{0x10 + (0x44C * worldId) + 0x448:X8}");
        }

        public void WriteWorldFlag(int worldId, int worldFlag)
        {
            _m.WriteMemory($"{GameStatePointer},{0x10 + (0x44C * worldId) + 0x448:X8}", "int", worldFlag.ToString());
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
            var tmp = _m.ReadInt($"{RaceLapsCountPointer},0");
            return tmp;
        }

        public void WriteRaceLapsCount(int value)
        {
            _m.WriteMemory($"{RaceLapsCountPointer},0", "int", value.ToString());
        }

        public int ReadRaceNitrosCount()
        {
            var tmp = _m.ReadInt($"{RaceNitrosCountPointer},0");
            return tmp;
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

        //public float ReadTimer()
        //{
        //    return _m.ReadFloat($"{TimerAddress}");
        //}

        //public void WriteTimer(float value)
        //{
        //    _m.WriteMemory($"{TimerAddress}", "float", value.ToString());
        //}

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

        int GetLevelCount(int worldId)
        {
            // World 5 has 7 levels
            if (worldId == 5)
            {
                return 7;
            }

            return 9;
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

        public override Vector3 ReadActCharPosition()
        {
            return _m.ReadVector3($"{ActiveCharacterPointer},{_offsetTransformationPosition}0");
        }

        public override void WriteActCharPosition(Vector3 value)
        {
            int tmp = _m.ReadInt($"{ActiveCharacterPointer},{_offsetCollider}");
            _m.WriteMemory($"{ActiveCharacterPointer},{_offsetCollider}", "int", "0");
            _m.WriteMemory($"{ActiveCharacterPointer},{_offsetTransformationPosition}0", "vec3", value.ToString());
            Thread.Sleep(10);
            _m.WriteMemory($"{ActiveCharacterPointer},{_offsetCollider}", "int", tmp.ToString());
        }

        public override void FreezeActCharPositionX(string value = "")
        {
            if (value == "")
            {
                Vector3 trans = ReadActCharPosition();
                value = trans.X.ToString();
            }
            _m.FreezeValue($"{ActiveCharacterPointer},{_offsetTransformationPosition}0", "float", value);
        }

        public override void FreezeActCharPositionY(string value = "")
        {
            if (value == "")
            {
                Vector3 trans = ReadActCharPosition();
                value = trans.Y.ToString();
            }
            _m.FreezeValue($"{ActiveCharacterPointer},{_offsetTransformationPosition}4", "float", value);
        }

        public override void FreezeActCharPositionZ(string value = "")
        {
            if (value == "")
            {
                Vector3 trans = ReadActCharPosition();
                value = trans.Z.ToString();
            }
            _m.FreezeValue($"{ActiveCharacterPointer},{_offsetTransformationPosition}8", "float", value);
        }

        public override void UnfreezeActCharPositionX()
        {
            _m.UnfreezeValue($"{ActiveCharacterPointer},{_offsetTransformationPosition}0");
        }

        public override void UnfreezeActCharPositionY()
        {
            _m.UnfreezeValue($"{ActiveCharacterPointer},{_offsetTransformationPosition}4");
        }

        public override void UnfreezeActCharPositionZ()
        {
            _m.UnfreezeValue($"{ActiveCharacterPointer},{_offsetTransformationPosition}8");
        }

        public override void FreezeActCharVelocityZ(string value = "")
        {
            if (value == "")
            {
                Vector3 trans = ReadActCharVelocity();
                value = trans.Z.ToString();
            }

            _m.FreezeValue($"{ActiveCharacterPointer},{_offsetTransformationVelocityZ}8", "float", value);
        }

        public override void UnfreezeActCharVelocityZ()
        {
            _m.UnfreezeValue($"{ActiveCharacterPointer},{_offsetTransformationVelocityZ}8");
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
            int health = _m.ReadInt(LivesAddress);
            return health;
        }

        public override void WriteActCharHealth(int value)
        {
            _m.WriteMemory(LivesAddress, "int", value.ToString());
        }

        public override void FreezeActCharHealth(int value = 0)
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

        public override Controller_t GetController()
        {
            return new(_m, ControllerAddress);
        }

        public override bool IsActCharAvailable()
        {
            return _m.ReadInt(ActiveCharacterPointer) != 0;
        }

        public override bool IsLoading()
        {
            if (_m.ReadInt(IsLoadingAddress) != 0)
            {
                return true;
            }
            return false;
        }

        public void SkipCurrentDialogue()
        {
            _m.WriteMemory($"{DialoguePointer},2E8", "int", "0");
        }

        public int ReadLuckyCharms()
        {
            int tmp = _m.ReadInt(LuckyCharmsAddress);
            return tmp;
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

        public Vector3 ReadActCharVelocity()
        {
            Vector3 trans = _m.ReadVector3($"{ActiveCharacterPointer},{_offsetTransformationVelocityZ}0");
            return trans;
        }

        public void WriteActCharVelocity(Vector3 value)
        {
            _m.WriteMemory($"{ActiveCharacterPointer},{_offsetTransformationVelocityZ}0", "vec3", value.ToString());
        }

        public int ReadMapId()
        {
            return _m.ReadInt(MapIdAddress);
        }

        public int ReadWorldId()
        {
            return _m.ReadInt(WorldIdAddress);
        }

        public override int GetMapId()
        {
            int worldId = ReadWorldId();
            int mapId = ReadMapId();

            if (Region == "NTSC Demo")
            {
                if (worldId == 0)
                {
                    if (mapId == 2)
                    {
                        // splash
                        return 0;
                    }
                    else if (mapId == 3)
                    {
                        // attract
                        return 1;
                    }
                }
                else if (worldId == 3)
                {
                    if (mapId == 0)
                    {
                        // asa
                        return 2;
                    }
                    else if (mapId == 1)
                    {
                        // ptg
                        return 3;
                    }
                    else if (mapId == 2)
                    {
                        // hch
                        return 4;
                    }
                    else if (mapId == 4)
                    {
                        // cunning
                        return 6;
                    }
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

            var tmp = (worldId - 1) * 9; // 9 levels in the world
            tmp = tmp + mapId; // from 0 to 8
            tmp = tmp + 3; // splash, paris and hideout

            if (worldId == 5)
            {
                // Skip mapId 1
                if (mapId > 0)
                {
                    tmp--;
                }

                // Skip mapId 7
                if (mapId == 8)
                {
                    tmp--;
                }
            }

            return tmp;
        }

        public override void LoadMap(int mapId)
        {
            string write = "";
            if (Region == "NTSC Demo")
            {
                int stringPointer = _m.ReadInt($"289A08,{mapId * 4:X}+4");
                write = stringPointer.ToString("X");
            }
            else
            {
                int tmp = Convert.ToInt32(ReloadValuesAddress, 16);
                int languageId = _m.ReadInt(LanguageAddress);
                tmp += mapId * ReloadValuesStructSize + 0x20 * languageId;
                write = tmp.ToString("X");
            }

            _m.WriteMemory($"{ReloadAddress}+10", "int", $"0x{write}");
            ReloadMap();
        }

        public void ReloadMap()
        {
            _m.WriteMemory($"{ReloadAddress}+C", "int", "1");
            _m.WriteMemory($"{ReloadAddress}+20", "int", "1");
            _m.WriteMemory($"{ReloadAddress}", "int", "1");
        }

        public override void ToggleInfiniteDbJump(bool enableInfDbJump)
        {
            string offset1 = "2228";
            string offset2 = "2498";
            if (Region == "NTSC Demo")
            {
                offset1 = "2008";
                offset2 = "2250";
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

        public override long ReadGadgets()
        {
            int gadgets = _m.ReadInt(GadgetAddress);
            return gadgets;
        }

        protected override List<Character_t> GetCharacters()
        {
            return new List<Character_t>
            {
                new("Sly", 1)
            };
        }

        protected override List<Map_t> GetMaps()
        {
            return new List<Map_t>
            {
                new Map_t("Splash",
                    new()
                    {
                        new("None", new(0, 0, 0)),
                    }
                ),
                new Map_t("Paris",
                    new()
                    {
                        new("Start", new(-2660, 850, -500)),
                        new("Vent", new(1300, 850, -400)),
                        new("Elevator", new(1440, 1620, -500)),
                        new("Window", new(150, -600, -2200)),
                        new("Safe", new(1800, 0, -2300)),
                        new("Le Exit", new(3000, 600, -4400)),
                        new("Van", new(7600, 600, -4400)),
                    }
                ),
                new Map_t("Hideout",
                    new()
                    {
                        new("None", new(0, 0, 0)),
                    }
                ),
                new Map_t("A Stealthy Approach",
                    new()
                    {
                        new("Start", new(5588, -23622, 800)),
                        new("Boat", new(-1570, -22000, 600)),
                        new("Door", new(2200, -18600, 700)),
                        new("Waterfall", new(1000, -11500, 1000)),
                        new("Hook", new(-5700, -13000, 800)),
                        new("Safe", new(-14700, -10000, 0)),
                        new("Exit", new(-15700, -8000, 0)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}Prowling The Grounds",
                    new()
                    {
                        new("Start", new(-15654, -12032, 700)),
                        new("Platform", new(-15800, -7850, 700)),
                        new("Fountain", new(-14926, -3489, 0)),
                        new("Cannon", new(-6800, -800, 100)),
                        new("Submarine", new(-10643, 2477, 400)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}High Class Heist",
                    new()
                    {
                        new("Start", new(-12384, 2114, -100)),
                        new("Laser arena", new(-4800, 0, -200)),
                        new("Safe", new(0, -1500, -700)),
                        new("Bridge", new(0, 0, -400)),
                        new("Spotlights", new(6950, 1900, -700)),
                        new("Exit", new(5800, -300, -400)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}Into the Machine",
                    new()
                    {
                        new("Start", new(2610, 6071, 400)),
                        new("Tube", new(2160, -200, 1000)),
                        new("Spinning fans", new(13200, -300, -1800)),
                        //new("Spinning fans 2", new(19800, 1700, -1000)),
                        new("Hook", new(24000, -400, -800)),
                        //new("Machine", new(32200, 3850, -3200)),
                        new("Machine", new(36100, 3200, -3500)),
                        new("Safe", new(37800, 3700, -3200)),
                        //new("Hook2", new(37500, 3000, -1900)),
                        new("Exit", new(45300, 3800, -2000)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}A Cunning Disguise",
                    new()
                    {
                        new("Start", new(-9660, 1020, -3500)),
                        new("Safe", new(-1050, -5800, -3700)),
                        new("Exit", new(-1550, -500, -3700)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}The Fire Down Below",
                    new()
                    {
                        new("Start", new(-4237, -5634, 0)),
                        new("Safe", new(-5200, -4800, 0)),
                        new("Wheel 1", new(-3200, 1000, 100)),
                        new("Wheel 2", new(-3000, 8600, 600)),
                        new("Exit", new(-1710, 10850, 1300)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}Treasure in the Depths",
                    new()
                    {
                        new("Start", new(-1873, 10, 349)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}The Gunboat Graveyard",
                    new()
                    {
                        new("Start", new(810, 3560, 150)),
                        new("Plane", new(-2200, -3700, 1200)),
                        new("Submarine", new(850, -7600, 50)),
                        new("Exit", new(-4400, -8300, 400)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}The Eye of the Storm",
                    new()
                    {
                        new("Start", new(-1200, 0, 100)),
                    }
                ),
                new Map_t("A Rocky Start",
                    new()
                    {
                        new("Start", new(-3551, 3794, 319)),
                        new("Cletus", new(1000, 0, 100)),
                        new("Bus", new(5000, -2500, 800)),
                        new("Hydraulic press", new(11200, 2000, 1300)),
                        new("Safe", new(15800, 1800, 1900)),
                        new("Tilting bus", new(16000, -2500, 1600)),
                        new("Exit", new(18000, -11700, 2000)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}Muggshot's Turf",
                    new()
                    {
                        new("Start", new(-6245, 3136, 400)),
                        new("Bridge", new(-3839, 19, 600)),
                        new("Casino", new(4700, 0, 600)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}Boneyard Casino",
                    new()
                    {
                        new("Start", new(3629, -1391, -400)),
                        new("Laser", new(-900, -8500, -700)),
                        new("Laser 2", new(600, -7400, -1200)),
                        new("Pool", new(-8600, -9000, -1300)),
                        new("Safe", new(-17000, -4200, -300)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}Murray's Big Gamble",
                    new()
                    {
                        new("Start", new(4685, 651, 1457)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}At the Dog Track",
                    new()
                    {
                        new("Start", new(-10452, 5939, 178)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}Two to Tango",
                    new()
                    {
                        new("Start", new(-8700, -200, 1200)),
                        new("Chase checkpoint", new(-1380, 10300, 1500)),
                        new("Safe", new(-3800, 7700, 1400)),
                        new("Chase end", new(-11300, 11500, 3100)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}Straight to the Top",
                    new()
                    {
                        new("Start", new(342, -331, -200)),
                        new("Safe", new(6100, -500, 2200)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}Back Alley Heist",
                    new()
                    {
                        new("Start", new(-4589, -1750, -300)),
                        new("Dog statues", new(3900, 100, 2400)),
                        new("Safe", new(-3000, -2900, 2000)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}Last Call",
                    new()
                    {
                        new("Start", new(-132, -801, 200)),
                        new("Second stage", new(0, -1500, 1900)),
                        new("Third stage", new(0, -1374, 3300)),
                    }
                ),
                new Map_t("The Dread Swamp Path",
                    new()
                    {
                        new("Start", new(7261, -4509, 1500)),
                        new("Tunnel", new(1300, -6800, 200)),
                        new("Tents", new(0, -2400, 500)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}The Swamp's Dark Centre",
                    new()
                    {
                        new("Start", new(-8313, 319, -1400)),
                        new("Hub", new(-3641, -2404, -2500)),
                        new("W3 boss fight trigger", new(531, -358, 438)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}The Lair of the Beast",
                    new()
                    {
                        new("Start", new(-832, -6506, 300)),
                        new("Checkpoint 1", new(3600, -4100, 1600)),
                        new("Checkpoint 2", new(3700, 4400, 500)),
                        new("Exit", new(5000, -6400, 800)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}A Grave Undertaking",
                    new()
                    {
                        new("Start", new(-6551, 1459, 1300)),
                        new("Arena", new(4000, -1600, 800)),
                        new("Checkpoint", new(6100, -1700, 1700)),
                        new("Exit", new(-600, 3400, 1800)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}Piranha Lake",
                    new()
                    {
                        new("Start", new(-700, 0, 0)),
                        new("Exit", new(3100, 0, 400)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}Descent into Danger",
                    new()
                    {
                        new("Start", new(-9772, 6418, -516)),
                        new("Checkpoint 1", new(-600, 0, -1000)),
                        new("Checkpoint 2", new(-4504, 5540, 1000)),
                        new("Safe", new(-2400, 1000, 1000)),
                        new("Exit", new(-2300, 5200, -1100)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}A Ghastly Voyage",
                    new()
                    {
                        new("Start", new(-8440, -9864, 600)),
                        new("Checkpoint 1", new(6500, -13300, 300)),
                        new("Checkpoint 2", new(9200, -6500, 1600)),
                        new("Exit", new(5800, -13500, 1300)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}Down Home Cooking",
                    new()
                    {
                        new("Start", new(3, 1625, 200)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}A Deadly Dance",
                    new()
                    {
                        new("Phase 1 start", new(-5533, 131, 100)),
                        new("Phase 1 end", new(250, 0, 100)),
                        new("Phase 2 start", new(1460, -2, 100)),
                        new("Phase 2 end", new(7574, -22, 100)),
                        new("Phase 3 start", new(7160, -1011, 300)),
                        new("Phase 3 end", new(2300, -3600, 1400)),
                        new("Phase 4 start", new(2792, -2742, 1622)),
                        new("Phase 4 end", new(3183, 4546, 1400)),
                    }
                ),
                new Map_t("A Perilous Ascent",
                    new()
                    {
                        new("Start", new(-8164, -8403, -4575)),
                        new("Checkpoint 1", new(-14400, -2800, -2300)),
                        new("Checkpoint 2", new(-6400, -8000, -600)),
                        new("Checkpoint 3", new(450, -1500, 300)),
                        new("Safe", new(-1400, 500, -300)),
                        new("Checkpoint 4/Exit", new(0, 3500, 0)),
                        new("Key", new(7500, 4900, 100)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}Inside the Stronghold",
                    new()
                    {
                        new("Start", new(-3428, -556, -3300)),
                        new("Hub", new(-1527, 476, -2101)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}Flaming Temple of Flame",
                    new()
                    {
                        new("Start", new(-2159, 550, 100)),
                        new("Checkpoint 1", new(5200, -8200, -700)),
                        new("Checkpoint 2", new(-500, -6000, -200)),
                        new("Gong", new(2600, -5000, 800)),
                        new("Laser floor", new(12500, 0, 2700)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}The Unseen Foe",
                    new()
                    {
                        new("Start", new(-14526, -5006, -200)),
                        new("Moving laser", new(-4600, -8900, 150)),
                        new("Checkpoint 1", new(-6780, -4842, 700)),
                        new("Checkpoint 2", new(-6908, -5808, 1700)),
                        new("Safe", new(-8100, -9600, 3400)),
                        new("Exit", new(-8970, -9450, -100)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}The King of the Hill",
                    new()
                    {
                        new("Start", new(-3124, 39, 737)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}Rapid Fire Assault",
                    new()
                    {
                        new("Start", new(-1062, 3691, 100)),
                        new("Door 1", new(6500, 1600, -50)),
                        new("Door 2", new(7100, -1100, -900)),
                        new("Exit", new(2900, 500, -1200)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}Duel by the Dragon",
                    new()
                    {
                        new("Start", new(-10617, -3961, -2500)),
                        new("Chase start", new(-5800, 1500, -2300)),
                        new("Checkpoint 1", new(1826, 4585, -2000)),
                        new("Checkpoint 2", new(5400, -2600, -1700)),
                        new("Safe", new(400, -3700, -1700)),
                        new("Exit", new(-1900, -2600, -800)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}A Desperate Race",
                    new()
                    {
                        new("Start", new(8306, -2161, 1100)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}Flame Fu!",
                    new()
                    {
                        new("Arena", new(-2687, 0, -100)),
                    }
                ),
                new Map_t("A Hazardous Path",
                    new()
                    {
                        new("Start", new(8570, -28669, 600)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}Burning Rubber",
                    new()
                    {
                        new("Start", new(-5368, 1446, 300)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}A Daring Rescue",
                    new()
                    {
                        new("Start", new(429, 2111, 400)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}Bentley Comes Through",
                    new()
                    {
                        new("Start", new(-1191, 0, 63)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}A Temporary Truce",
                    new()
                    {
                        new("Start", new(279, -201, 1025)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}Sinking Peril",
                    new()
                    {
                        new("Start", new(-291, -880, 292)),
                    }
                ),
                new Map_t($"{StringBeforeSubMapName}A Strange Reunion",
                    new()
                    {
                        new("Start", new(-3728, -5855, 743)),
                        new("To Clockwerk", new(1988, 2489, 600)),
                        new("Clockwerk", new(825, -2700, 700)),
                    }
                ),
            };
        }

        public override int ReadActCharId()
        {
            return Characters.FirstOrDefault().Id;
        }

        public override void WriteActCharId(int id)
        {
            throw new NotImplementedException();
        }

        public override void FreezeActCharId(string value = "")
        {
            throw new NotImplementedException();
        }

        public override void UnfreezeActCharId()
        {
            throw new NotImplementedException();
        }

        public int ReadActCharGadgetPower()
        {
            throw new NotImplementedException();
        }

        public override void UnfreezeActCharGadgetPower()
        {
            throw new NotImplementedException();
        }

        public override void FreezeActCharGadgetPower(int value = 0)
        {
            throw new NotImplementedException();
        }

        public override void ToggleInvulnerable(bool enableInvulnerable)
        {
            throw new NotImplementedException();
        }

        public override void ToggleUndetectable(bool enableUndetectable)
        {
            throw new NotImplementedException();
        }

        public void WriteActCharGadgetPower(int value)
        {
            throw new NotImplementedException();
        }

        public override Vector3 ReadPositionFromPointerToEntity(string pointerToEntity)
        {
            throw new NotImplementedException();
        }

        public override Vector3 ReadWorldPositionFromPointerToEntity(string pointerToEntity)
        {
            throw new NotImplementedException();
        }

        public override void WritePositionFromPointerToEntity(string pointerToEntity, Vector3 value)
        {
            throw new NotImplementedException();
        }

        public override void FreezePositionXFromPointerToEntity(string pointerToEntity, string value = "")
        {
            throw new NotImplementedException();
        }

        public override void FreezePositionYFromPointerToEntity(string pointerToEntity, string value = "")
        {
            throw new NotImplementedException();
        }

        public override void FreezePositionZFromPointerToEntity(string pointerToEntity, string value = "")
        {
            throw new NotImplementedException();
        }

        public override void UnfreezePositionXFromPointerToEntity(string pointerToEntity)
        {
            throw new NotImplementedException();
        }

        public override void UnfreezePositionYFromPointerToEntity(string pointerToEntity)
        {
            throw new NotImplementedException();
        }

        public override void UnfreezePositionZFromPointerToEntity(string pointerToEntity)
        {
            throw new NotImplementedException();
        }

        public override Matrix4x4 ReadWorldRotationFromPointerToEntity(string pointerToEntity)
        {
            throw new NotImplementedException();
        }

        public override void WriteWorldRotationFromPointerToEntity(string pointerToEntity, Matrix4x4 rotationMatrix)
        {
            throw new NotImplementedException();
        }

        public override void WriteScaleFromPointerToEntity(string pointerToEntity, float scale)
        {
            throw new NotImplementedException();
        }

        public override float ReadScaleFromPointerToEntity(string pointerToEntity)
        {
            throw new NotImplementedException();
        }

        public override void LoadMap(int mapId, int entranceValue)
        {
            throw new NotImplementedException();
        }

        public override void LoadMap(int mapId, int entranceValue, int mode)
        {
            throw new NotImplementedException();
        }
    }
}
