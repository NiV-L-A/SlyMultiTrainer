using System.Numerics;
using static SlyMultiTrainer.Util;

namespace SlyMultiTrainer
{
    public abstract class GameBase_t
    {
        private Form1 _form;
        private Memory.Mem _m;
        private int _lastMapId;
        private int _lastActCharId;
        private bool _isFirstLoopAfterLoading;
        private bool _canSkipDialogueWithBind;

        protected string FOVAddress;
        protected string ClockAddress;
        protected string DrawDistanceAddress;
        protected string ResetCameraAddress;
        protected string CoinsAddress;
        protected string MapIdAddress;
        protected string GadgetAddress;
        protected string GuardAIAddress;
        protected string CanCameraNoclipAddress;
        protected string ControllerAddress;
        protected string DialoguePointer;
        protected string SkipFMVPointer;

        public Build_t Build;
        public Character_t ActiveCharacter;
        public Controller_t Controller;
        public List<Character_t> Characters;
        public List<List<Gadget_t>> Gadgets;
        public List<Map_t> Maps;
        public Dictionary<string, List<Warp_t>> CustomWarps;

        protected GameBase_t(Form1 form, Memory.Mem m, Build_t build)
        {
            _form = form;
            _m = m;
            Build = build;
            _isFirstLoopAfterLoading = true;
            _canSkipDialogueWithBind = true;
            Characters = GetCharacters();
            Gadgets = GetGadgets();
            Maps = GetMaps();
        }

        // Methods that are that and nothing else:
        //     public [type] [name]() { [implementation] }
        // Methods that must be implemented by the game (they are game specific):
        //     public abstract [type] [name]();
        // Methods that have default implementation but can be overridden by the game:
        //     public virtual [type] [name]() { [default implementation] }

        // Main loop tick for all games
        public void OnLoopTick()
        {
            int mapId = ReadMapId() + 1; // + first item for current map
            if (mapId != 0 && mapId != _lastMapId)
            {
                OnMapChange(mapId);
                
                // If we load a savestate, pcsx2 needs time to restore the memory
                // We return here so that pcsx2 has enough time to finish restoring the memory
                // E.g. for pcsx2 to restore the bytes for all "Entrance" warp points
                return;
            }

            Controller = GetController();
            if (IsLoading())
            {
                // While the map is loading
                _isFirstLoopAfterLoading = true;
                _form.UpdateUI(_form.grpGadgets, false, "Enabled");
                _lastActCharId = 0;
                // Done here too for FMVs
                HandleDialogueSkipBind();
                return;
            }

            if (_isFirstLoopAfterLoading)
            {
                // Only run once after map loading
                if (mapId == 0)
                {
                    // For sly 1 no map (before splash appears)
                    return;
                }

                _isFirstLoopAfterLoading = false;
                RefreshWarps(mapId);
                OnFirstLoopAfterLoading(mapId);
                if (this is Sly1Handler)
                {
                    _form.UpdateUI(_form.grpGadgets, true, "Enabled");
                }

                _form.UpdateUI(() =>
                {
                    var maps = (List<Map_t>)_form.cmbMaps.DataSource!;
                    maps[0].Name = $"[Current map: {Maps[mapId].Name.TrimStart()}]";
                    ((CurrencyManager)_form.cmbMaps.BindingContext![maps]).Refresh();
                });
            }

            _form.UpdateUI(_form.trkFOV, ReadFOV() * 10);
            _form.UpdateUI(_form.trkClock, ReadClock() * 10);
            _form.UpdateUI(_form.trkDrawDistance, ReadDrawDistance() * 10);

            UpdateActChar();
            HandleDialogueSkipBind();

            string tabName = "";
            _form.UpdateUI(() =>
            {
                tabName = _form.tabControlMain.SelectedTab.Name;
            });

            if (tabName == "tabEntities")
            {
                UpdateEntities();
            }
            else if (tabName == "tabDAG")
            {
                UpdateDAG();
            }
            else if (tabName == "tabStrings")
            {
                UpdateStrings();
            }

            // Game specific logic
            CustomTick();
        }

        void UpdateActChar()
        {
            // Update active character in the dropdown
            // Sly 1 only has sly as playable
            if (this is not Sly1Handler)
            {
                _form.UpdateUI(() =>
                {
                    if (!_form.cmbActChar.DroppedDown)
                    {
                        var actCharId = _m.ReadInt($"{GetActCharPointer()},18");
                        if (_lastActCharId != actCharId
                         || _isFirstLoopAfterLoading)
                        {
                            _lastActCharId = actCharId;
                            var characters = _form.cmbActChar.DataSource as List<Character_t>;
                            int currentCharacter = characters.FindIndex(x => x.Id == actCharId);
                            _form.grpGadgets.Enabled = false;
                            _form.cmbGadgetL1.SelectedIndex = 0;
                            _form.cmbGadgetL2.SelectedIndex = 0;
                            _form.cmbGadgetR2.SelectedIndex = 0;
                            if (currentCharacter != -1)
                            {
                                // A character might be playable but don't have gadgets (e.g. sly 3 guru)
                                _form.cmbActChar.SelectedIndex = currentCharacter;
                                ActiveCharacter = characters[currentCharacter];
                                if (characters[currentCharacter].NameForSavefile != "")
                                {
                                    // Character is playable and has gadgets
                                    _form.grpGadgets.Enabled = true;
                                    List<Gadget_t> characterGadgetsL1 = new(Gadgets[currentCharacter].Where(x => x.IsBindable));
                                    List<Gadget_t> characterGadgetsR2 = new(Gadgets[currentCharacter].Where(x => x.IsBindable));
                                    List<Gadget_t> characterGadgetsL2 = new(Gadgets[currentCharacter].Where(x => x.IsBindable));
                                    _form.cmbGadgetL1.DataSource = characterGadgetsL1;
                                    _form.cmbGadgetL2.DataSource = characterGadgetsL2;
                                    _form.cmbGadgetR2.DataSource = characterGadgetsR2;
                                }
                            }
                        }
                    }
                });
            }

            if (!IsActCharAvailable())
            {
                _form.UpdateUI(_form.lblXCoord, DefaultValueFloat);
                _form.UpdateUI(_form.lblYCoord, DefaultValueFloat);
                _form.UpdateUI(_form.lblZCoord, DefaultValueFloat);
                _form.UpdateUI(_form.chkActCharHealthFreeze, DefaultValueInt);
                _form.UpdateUI(_form.lblSpeed, DefaultValueFloat);
                return;
            }

            Vector3 position = ReadActCharLocalTranslation();
            _form.UpdateUI(_form.lblXCoord, position.X);
            _form.UpdateUI(_form.lblYCoord, position.Y);
            _form.UpdateUI(_form.lblZCoord, position.Z);
            _form.UpdateUI(_form.chkActCharHealthFreeze, ReadActCharHealth());
            _form.UpdateUI(_form.lblSpeed, ReadActCharVelocity().Length());

            // Fly logic
            if (_form.chkActCharFly.Checked)
            {
                string FlyButtonUp = Properties.Settings.Default.FlyButtonUp;
                string FlyButtonDown = Properties.Settings.Default.FlyButtonDown;
                string FlyButtonAccelerate = Properties.Settings.Default.FlyButtonAccelerate;

                if (Controller.IsButtonPressed(FlyButtonAccelerate))
                {
                    FreezeActCharSpeedMultiplier(AmountToIncreaseOrDecreaseTranslationForActChar / 50);
                }
                else
                {
                    UnfreezeActCharSpeedMultiplier();
                    WriteActCharSpeedMultiplier(1);
                }

                if (Controller.IsButtonPressed(FlyButtonUp))
                {
                    // up
                    //    unfreeze Z
                    //    set velocity Z to 500, keep freeze
                    UnfreezeActCharLocalTranslationZ();

                    if (Controller.IsButtonPressed(FlyButtonAccelerate))
                    {
                        FreezeActCharVelocityZ((AmountToIncreaseOrDecreaseTranslationForActChar * 7).ToString());
                    }
                    else
                    {
                        FreezeActCharVelocityZ((AmountToIncreaseOrDecreaseTranslationForActChar * 3).ToString());
                    }

                }
                else if (Controller.IsButtonPressed(FlyButtonDown))
                {
                    // down
                    //    unfreeze Z
                    //    set velocity Z to -500, keep freeze
                    UnfreezeActCharLocalTranslationZ();

                    if (Controller.IsButtonPressed(FlyButtonAccelerate))
                    {
                        FreezeActCharVelocityZ((-AmountToIncreaseOrDecreaseTranslationForActChar * 7).ToString());
                    }
                    else
                    {
                        FreezeActCharVelocityZ((-AmountToIncreaseOrDecreaseTranslationForActChar * 3).ToString());
                    }
                }
                else
                {
                    // idle
                    //    freeze Z to current
                    //    freeze velocity Z to 0

                    // Using position.Z which is read a bit earlier makes the character stutter,
                    // so let's get the latest value possible by reading the position again
                    FreezeActCharLocalTranslationZ(ReadActCharLocalTranslation().Z.ToString());
                    FreezeActCharVelocityZ("0");

                    // It is possible that while in this if scope, the user disabled the fly function
                    // Let's check it again to see if we should keep the position and velocity frozen
                    if (!_form.chkActCharFly.Checked)
                    {
                        UnfreezeActCharVelocityZ();

                        // But only unfreeze the Z position if the checkbox for the z coordinate of the active character is not frozen
                        if (!_form.chkActCharZCoordFreeze.Checked)
                        {
                            UnfreezeActCharLocalTranslationZ();
                        }
                    }
                }
            }

            // Read gadget binds
            if (this is not Sly1Handler)
            {
                _form.UpdateUI(() =>
                {
                    if (!_form.grpGadgets.Visible
                     || !_form.grpGadgets.Enabled)
                    {
                        // For builds or characters that don't have gadgets
                        return;
                    }

                    UpdateActCharGadgetBind(_form.cmbGadgetL1, GADGET_BIND.L1);
                    UpdateActCharGadgetBind(_form.cmbGadgetL2, GADGET_BIND.L2);
                    UpdateActCharGadgetBind(_form.cmbGadgetR2, GADGET_BIND.R2);
                });
            }
        }

        void HandleDialogueSkipBind()
        {
            string SkipCurrentDialogueBind = Properties.Settings.Default.SkipCurrentDialogueBind;
            if (_canSkipDialogueWithBind && Controller.IsButtonPressed(SkipCurrentDialogueBind))
            {
                _canSkipDialogueWithBind = false;
                SkipCurrentDialogue();
            }
            else if (Controller.IsNoButtonPressed())
            {
                _canSkipDialogueWithBind = true;
            }
        }

        void UpdateActCharGadgetBind(ComboBox cmbGadget, GADGET_BIND bind)
        {
            if (cmbGadget.DroppedDown
             || cmbGadget.ContainsFocus
             || cmbGadget.DataSource is not List<Gadget_t> gadgets)
            {
                // Do not update if the combobox is opened
                return;
            }

            int gadgetId = ReadActCharGadgetId(bind);

            // Default to none
            int selectedIndex = 0;

            // Find the index of the gadget in the list only if a gadget is binded
            if (gadgetId != 0 && gadgetId != -1)
            {
                selectedIndex = gadgets.FindIndex(x => x.Id == gadgetId);
            }

            cmbGadget.SelectedIndex = selectedIndex;
        }

        void UpdateEntities()
        {
            if (_form.trvEntitiesList.Nodes.Count == 0)
            {
                _form.UpdateUI(() =>
                {
                    if (!_form.txtEntitiesSearch.Focused)
                    {
                        _form.btnEntitiesRefreshList_Click(_form.btnEntitiesRefreshList, EventArgs.Empty);
                    }
                });
            }

            // Sly 3, for when the day hasn't changed but the time of day did
            var fkList = _form.trvEntitiesList.Tag as List<FKXEntry_t>;
            for (int i = 0; i < fkList.Count; i++)
            {
                var poolPointer = fkList[i].PoolPointer;
                var poolPointerInGame = _m.ReadInt($"{fkList[i].Address}+4");
                if (poolPointer != poolPointerInGame)
                {
                    _form.UpdateUI(() =>
                    {
                        _form.trvEntitiesList.Nodes.Clear();
                    });

                    return;
                }
            }

            int pointerToEntity = 0;
            _form.UpdateUI(() =>
            {
                pointerToEntity = _form.GetPointerToEntityFromSelectedEntitiesNode();
            });

            if (pointerToEntity == 0)
            {
                return;
            }

            Vector3 localTrans = ReadEntityLocalTranslation(pointerToEntity.ToString("X"));
            _form.UpdateUI(_form.lblEntitiesXCoord, localTrans.X);
            _form.UpdateUI(_form.lblEntitiesYCoord, localTrans.Y);
            _form.UpdateUI(_form.lblEntitiesZCoord, localTrans.Z);

            Vector3 localVelocity = ReadEntityLocalVelocity(pointerToEntity.ToString("X"));
            _form.UpdateUI(_form.lblEntitiesSpeed, localVelocity.Length());

            Vector3 worldTrans = ReadEntityFinalCombinedTranslation(pointerToEntity.ToString("X"));
            _form.UpdateUI(_form.lblEntitiesXCoordWorld, worldTrans.X);
            _form.UpdateUI(_form.lblEntitiesYCoordWorld, worldTrans.Y);
            _form.UpdateUI(_form.lblEntitiesZCoordWorld, worldTrans.Z);

            float scale = ReadEntityLocalScale(pointerToEntity.ToString("X"));
            _form.UpdateUI(_form.trkEntitiesScale, scale * 10);

            // Read rotation only if the edit checkbox is not checked
            if (!_form.chkEntitiesEditRotation.Checked)
            {
                Matrix4x4 rotationMatrix = ReadEntityWorldTransformation(pointerToEntity.ToString("X"));
                var euler = ExtractEulerAngles(rotationMatrix);
                _form.UpdateUI(_form.trkEntitiesRotationX, euler.X);
                _form.UpdateUI(_form.trkEntitiesRotationY, euler.Y);
                _form.UpdateUI(_form.trkEntitiesRotationZ, euler.Z);
            }
        }

        void UpdateDAG()
        {
            DAG_t DAG;
            Sly2_3_Savefile savefile;
            if (this is Sly2Handler)
            {
                DAG = (this as Sly2Handler).DAG;
                savefile = (this as Sly2Handler).Savefile;
            }
            else
            {
                DAG = (this as Sly3Handler).DAG;
                savefile = (this as Sly3Handler).Savefile;
            }

            if (DAG.Graph == null)
            {
                DAG.Init(savefile);
                DAG.GetDAG();
                if (!DAG.SetGraph())
                {
                    return;
                }

                _form.UpdateUI(DAG.Viewer, true, "Enabled");
            }

            // We check some fields in every node in the loaded dag
            // to see if there is a mismatch between what we have on the dag and the value in-game
            // If there is a mismatch, either the in-game dag was changed by the game
            // or the user forcefully changed a field of the node

            // This variable is used to trigger a redraw
            // UNUSED FOR NOW
            bool redrawGraph = true;

            string currentCheckpointAddress = DAG.GetCurrentCheckpointAddress();
            for (int i = 0; i < DAG.Tasks.Count; i++)
            {
                Task_t task = DAG.Tasks[i];
                Task_t taskInGame = DAG.ReadTask(task.Address, false);

                // When loading a save state or loading a save file, it is possible to have a mismatch between what we have on the dag and the value in-game
                // So, we make sure the task we are reading is the task we are looking for
                // If it's not, then we trigger a refresh of the entire dag
                if (task.Id != taskInGame.Id)
                {
                    _form.UpdateUI(() =>
                    {
                        DAG.TriggerRefresh();
                    });

                    return;
                }

                // If there is a mismatch between what is in game and what we have on the trainer
                // = something changed
                bool areTasksEqual = DAG.IsTaskEqualToTask(task, taskInGame);
                if (!areTasksEqual)
                {
                    redrawGraph = true;
                    task.State = taskInGame.State;
                    task.FocusCount = taskInGame.FocusCount;
                    task.CompleteCount = taskInGame.CompleteCount;
                }

                // Update the node's color
                task.MsaglNode.Attr.FillColor = DAG.GetNodeColorFromState(task.State);

                // Update the cluster's color only if this is the first node of the cluster
                if (task.Cluster.Tasks.FirstOrDefault() == task)
                {
                    task.Cluster.Subgraph.Attr.FillColor = DAG.GetClusterColorFromState(task.State);
                }

                // Check if it's the current checkpoint
                if (task.Address == currentCheckpointAddress)
                {
                    if (!task.MsaglNode.Attr.Styles.Contains(Microsoft.Msagl.Drawing.Style.Dashed))
                    {
                        redrawGraph = true;
                        task.MsaglNode.Attr.AddStyle(Microsoft.Msagl.Drawing.Style.Dashed);
                    }

                    if (!DAG.IsNodeSelected(task.MsaglNode))
                    {
                        task.MsaglNode.Attr.LineWidth = DAG.NodeCurrentCheckpointDefaultLineWidth;
                    }
                }
                else if (task.MsaglNode.Attr.Styles.Any())
                {
                    // previous checkpoint
                    if (task.MsaglNode.Attr.Styles.Contains(Microsoft.Msagl.Drawing.Style.Dashed))
                    {
                        if (DAG.IsNodeSelected(task.MsaglNode))
                        {
                            task.MsaglNode.Attr.LineWidth = DAG.SelectedNodeDefaultLineWidth;
                        }
                        else
                        {
                            task.MsaglNode.Attr.LineWidth = DAG.NodeDefaultLineWidth;
                        }
                    }

                    task.MsaglNode.Attr.ClearStyles();
                }
            }

            for (int i = 0; i < DAG.Clusters.Count; i++)
            {
                Cluster_t cluster = DAG.Clusters[i];
                // To true because we want to read the new suck value
                Cluster_t clusterInGame = DAG.ReadCluster(cluster.Address, true);

                if (cluster.Id != clusterInGame.Id)
                {
                    _form.UpdateUI(() =>
                    {
                        DAG.TriggerRefresh();
                    });

                    return;
                }

                bool areClustersEqual = DAG.IsClusterEqualToCluster(cluster, clusterInGame);
                if (!areClustersEqual)
                {
                    cluster.Suck = clusterInGame.Suck;
                }
            }

            if (redrawGraph)
            {
                DAG.Viewer.Invalidate();
            }
        }

        void UpdateStrings()
        {
            string tabName = "";
            _form.UpdateUI(() =>
            {
                tabName = _form.tabControlStrings.SelectedTab.Name;
            });

            if (tabName == "tabPageLocalized")
            {
                if (!string.IsNullOrEmpty(_form.txtStringsLocalized.Text))
                {
                    return;
                }

                List<(int id, string str)> list;
                if (this is Sly2Handler)
                {
                    list = (this as Sly2Handler).GetStringTable(true);
                }
                else
                {
                    list = (this as Sly3Handler).GetStringTable(true);
                }

                var output = $"Id - String{Environment.NewLine}";
                output += string.Join(Environment.NewLine, list.Select(i => $"{i.id:X} - {i.str}"));
                _form.UpdateUI(_form.txtStringsLocalized, output);
            }
            else if (tabName == "tabPageSavefile")
            {
                if (!string.IsNullOrEmpty(_form.txtStringsSavefile.Text))
                {
                    return;
                }

                Sly2_3_Savefile savefile;
                if (this is Sly2Handler)
                {
                    savefile = (this as Sly2Handler).Savefile;
                }
                else
                {
                    savefile = (this as Sly3Handler).Savefile;
                }
                
                List<string> list = savefile.DumpSavefileAddressTable(true);
                _form.UpdateUI(_form.txtStringsSavefile, $"Address - Field path{Environment.NewLine}{string.Join(Environment.NewLine, list)}");
            }
        }

        public abstract void CustomTick();
        public abstract void OnFirstLoopAfterLoading(int mapId);
        public void OnMapChange(int mapId)
        {
            _isFirstLoopAfterLoading = true;
            _lastMapId = mapId;

            // Reset entities, dag and the localized strings which are all map dependent
            if (this is Sly2Handler || this is Sly3Handler)
            {
                DAG_t DAG;
                if (this is Sly2Handler)
                {
                    DAG = (this as Sly2Handler).DAG;
                }
                else
                {
                    DAG = (this as Sly3Handler).DAG;
                }

                _form.UpdateUI(() =>
                {
                    _form.trvEntitiesList.Nodes.Clear();
                    DAG.TriggerRefresh();
                    _form.txtStringsLocalized.Text = "";
                });
            }
        }

        public void RefreshWarps(int mapId)
        {
            List<Warp_t> warpsToAdd = new();

            // Check settings for which warps to add
            string[] items = Properties.Settings.Default.WarpsList.Split('|');
            foreach (var item in items)
            {
                string[] parts = item.Split(';');
                string name = parts[0];
                bool.TryParse(parts[1], out bool isChecked);
                if (isChecked)
                {
                    switch (name)
                    {
                        case "Built-in":
                            warpsToAdd.AddRange(Maps[mapId].Warps);
                            break;
                        case "Custom":
                            // TrimStart to remove the sub map name prefix
                            if (CustomWarps.TryGetValue(Maps[mapId].Name.TrimStart().ToUpper(), out List<Warp_t> customWarps))
                            {
                                warpsToAdd.AddRange(customWarps);
                            }
                            break;
                        case "Entrance":
                            List<Warp_t> mapEntrances = GetEntranceLocations();
                            warpsToAdd.AddRange(mapEntrances);
                            break;
                    }
                }
            }

            // Only update if this is the first time or we changed map
            // So if the game reloaded to the same map, we should not update the warps list
            if (_form.cmbWarps.DataSource == null
             || !warpsToAdd.SequenceEqual((List<Warp_t>)_form.cmbWarps.DataSource))
            {
                _form.UpdateUI(_form.cmbWarps, warpsToAdd);

                if (this is not Sly1Handler)
                {
                    _form.UpdateUI(_form.cmbEntitiesWarps, new List<Warp_t>(warpsToAdd));
                }
            }
        }

        public abstract bool IsLoading();

        #region Gadgets
        // Read and write "bitfield64" because the value is not reversed on ps3
        public virtual long ReadGadgets()
        {
            return _m.ReadBitfield64(GadgetAddress);
        }

        public virtual void WriteGadgets(long value)
        {
            _m.WriteMemory(GadgetAddress, "bitfield64", value.ToString());
        }

        public virtual bool IsGadgetEarned(long gadgets, Gadget_t gadget)
        {
            if (gadget.Id == -1)
            {
                return false;
            }

            bool isEarned = (gadgets & gadget.Mask) != 0;
            return isEarned;
        }

        public virtual long ToggleEarnedGadget(long gadgets, Gadget_t gadget, bool isEarned)
        {
            if (isEarned)
            {
                gadgets |= gadget.Mask;
            }
            else
            {
                gadgets &= ~gadget.Mask;
            }

            return gadgets;
        }

        public abstract void ToggleAllGadgets();
        public abstract void FreezeActCharGadgetPower(int value = 0);
        public abstract void UnfreezeActCharGadgetPower();
        public abstract int ReadActCharGadgetId(GADGET_BIND bind);
        public abstract void WriteActCharGadgetId(GADGET_BIND bind, int value);
        #endregion

        #region Coins
        public void SetCoins(int value)
        {
            _m.WriteMemory(CoinsAddress, "int", value.ToString());
        }
        #endregion

        #region Entities
        // Sly 1 only has 1 transformation component (2 4x4 matrices)

        // Sly 2 and 3 have 4 transformation components (2 4x4 matrices per transformation component; 8 4x4 matrices in total)
        // Sometimes the last 2 transformation components are the same (e.g. sly in sly 2 ep1)
        // Sometimes the last 2 transformation components are different (e.g. carmelita in sly 3 ep1)
        // These are the names given to the 4 transformation components: Origin, Local, World, Final
        // Each transformation component has 2 4x4 transformation matrices. One at +0x0 and one at +0x40
        // The one at +0x0 is write-able and it's the delta (relative) transformation from the previous transformation component
        // The one at +0x40 is not write-able and it's the multiplication (combined) of the matrix at +0x0 of "this" transformation component and the matrix at +0x40 of the previous transformation component

        public abstract bool EntityHasTransformation(string pointerToEntity);

        // Used for sly 2 and 3 for warping to entrance locations (sly, bentley and murray have different height)
        public abstract Vector3 ReadEntityDeltaTranslation(string pointerToEntity);
        #region Origin
        /*
            Origin
            OriginTransformation
            OriginCombinedTransformation
        */
        public abstract Matrix4x4 ReadEntityOriginTransformation(string pointerToEntity);
        public abstract Matrix4x4 ReadEntityOriginCombinedTransformation(string pointerToEntity);
        public abstract void WriteEntityOriginTransformation(string pointerToEntity, Matrix4x4 transformation);

        #endregion

        #region Local
        /*
            Local
            LocalTransformation
            LocalCombinedTransformation
        */
        public abstract Matrix4x4 ReadEntityLocalTransformation(string pointerToEntity);
        public abstract Matrix4x4 ReadEntityLocalCombinedTransformation(string pointerToEntity);
        public abstract void WriteEntityLocalTransformation(string pointerToEntity, Matrix4x4 transformation);
        public abstract Vector3 ReadEntityLocalTranslation(string pointerToEntity);
        public abstract void WriteEntityLocalTranslation(string pointerToEntity, Vector3 value);
        public abstract void FreezeEntityLocalTranslationX(string pointerToEntity, string value = "");
        public abstract void FreezeEntityLocalTranslationY(string pointerToEntity, string value = "");
        public abstract void FreezeEntityLocalTranslationZ(string pointerToEntity, string value = "");
        public abstract void UnfreezeEntityLocalTranslationX(string pointerToEntity);
        public abstract void UnfreezeEntityLocalTranslationY(string pointerToEntity);
        public abstract void UnfreezeEntityLocalTranslationZ(string pointerToEntity);
        public abstract float ReadEntityLocalScale(string pointerToEntity);
        public abstract void WriteEntityLocalScale(string pointerToEntity, float value);
        public abstract Vector3 ReadEntityLocalVelocity(string pointerToEntity);
        public abstract void WriteEntityLocalVelocity(string pointerToEntity, Vector3 value);
        #endregion

        #region World
        /*
            World
            WorldTransformation
            WorldCombinedTransformation
        */
        public abstract Matrix4x4 ReadEntityWorldTransformation(string pointerToEntity);
        public abstract Matrix4x4 ReadEntityWorldCombinedTransformation(string pointerToEntity);
        public abstract void WriteEntityWorldTransformation(string pointerToEntity, Matrix4x4 value);
        #endregion

        #region Final
        /*
            Final
            FinalTransformation
            FinalCombinedTransformation
        */
        public abstract Matrix4x4 ReadEntityFinalTransformation(string pointerToEntity);
        public abstract Matrix4x4 ReadEntityFinalCombinedTransformation(string pointerToEntity);
        public abstract Vector3 ReadEntityFinalCombinedTranslation(string pointerToEntity);
        public abstract void WriteEntityFinalTransformation(string pointerToEntity, Matrix4x4 value);
        #endregion

        public void WarpSourceEntityToPoint(string pointerToSourceEntity, Matrix4x4 point)
        {
            if (pointerToSourceEntity == "")
            {
                pointerToSourceEntity = GetActCharPointer();
            }

            if (this is Sly1Handler)
            {
                WriteEntityLocalTransformation(pointerToSourceEntity, point);
                return;
            }

            // Convert warp position to local space (sly 2 ep1 npc_boar_guard, sly 3 carmelita)
            Matrix4x4 originMatrix = ReadEntityOriginTransformation(pointerToSourceEntity);
            Matrix4x4.Invert(originMatrix, out Matrix4x4 originInverse);
            point = point * originInverse;

            // Normalize point because characters have different height
            Vector3 delta = ReadEntityDeltaTranslation(pointerToSourceEntity);
            point.Translation = point.Translation + delta;
            WriteEntityLocalTranslation(pointerToSourceEntity, point.Translation);
            WriteEntityWorldTransformation(pointerToSourceEntity, point);
        }

        public void WarpSourceEntityToPoint(string pointerToSourceEntity, Vector3 point)
        {
            WarpSourceEntityToPoint(pointerToSourceEntity, Matrix4x4.CreateTranslation(point));
        }

        public void WarpSourceEntityToDestEntity(string pointerToSourceEntity, string pointerToDestEntity)
        {
            if (pointerToDestEntity == "")
            {
                pointerToDestEntity = GetActCharPointer();
            }

            Matrix4x4 trans = ReadEntityFinalCombinedTransformation(pointerToDestEntity);
            WarpSourceEntityToPoint(pointerToSourceEntity, trans);
        }

        #endregion

        #region Active character
        public abstract bool IsActCharAvailable();
        public abstract string GetActCharPointer();
        public abstract int ReadActCharId();
        public abstract void WriteActCharId(int id);
        public abstract void FreezeActCharId(string value = "");
        public abstract void UnfreezeActCharId();
        public abstract int ReadActCharHealth();
        public abstract void WriteActCharHealth(int value);
        public abstract void FreezeActCharHealth(int value = 0);
        public abstract void UnfreezeActCharHealth();
        public abstract Matrix4x4 ReadActCharOriginTransformation();
        public abstract Vector3 ReadActCharLocalTranslation();
        public abstract void WriteActCharLocalTranslation(Vector3 value);
        public abstract void FreezeActCharLocalTranslationX(string value = "");
        public abstract void FreezeActCharLocalTranslationY(string value = "");
        public abstract void FreezeActCharLocalTranslationZ(string value = "");
        public abstract void UnfreezeActCharLocalTranslationX();
        public abstract void UnfreezeActCharLocalTranslationY();
        public abstract void UnfreezeActCharLocalTranslationZ();
        public abstract Vector3 ReadActCharVelocity();
        public abstract void WriteActCharVelocity(Vector3 value);
        public abstract void FreezeActCharVelocityZ(string value = "");
        public abstract void UnfreezeActCharVelocityZ();
        public abstract float ReadActCharSpeedMultiplier();
        public abstract void WriteActCharSpeedMultiplier(float value);
        public abstract void FreezeActCharSpeedMultiplier(float value);
        public abstract void UnfreezeActCharSpeedMultiplier();
        #endregion

        #region Toggles
        public abstract void ToggleUndetectable(bool enableUndetectable);
        public abstract void ToggleInvulnerable(bool enableInvulnerable);
        public abstract void ToggleInfiniteDbJump(bool enableInfDbJump);
        public abstract void ActCharToggleNoclip(bool enableNoclip);

        public virtual void ToggleNoclip(bool enableNoclip)
        {
            if (enableNoclip)
            {
                _m.WriteMemory($"{CanCameraNoclipAddress}", "int", "1");
            }
            else
            {
                _m.WriteMemory($"{CanCameraNoclipAddress}", "int", "0");
            }

            ActCharToggleNoclip(enableNoclip);
        }

        public virtual void ToggleGuardAI(bool disableGuardAI)
        {
            if (disableGuardAI)
            {
                _m.FreezeValue(GuardAIAddress, "int", "1");
            }
            else
            {
                _m.WriteMemory(GuardAIAddress, "int", "0");
                _m.UnfreezeValue(GuardAIAddress);
            }
        }
        #endregion

        #region Camera
        public void ResetCamera()
        {
            _m.WriteMemory(ResetCameraAddress, "int", "1");
        }

        public float ReadFOV()
        {
            return _m.ReadFloat($"{FOVAddress}");
        }

        public void WriteFOV(float value)
        {
            _m.WriteMemory($"{FOVAddress}", "float", value.ToString());
        }

        public void FreezeFOV(float value = 0)
        {
            if (value == 0)
            {
                value = ReadFOV();
            }

            _m.FreezeValue($"{FOVAddress}", "float", value.ToString());
        }

        public void UnfreezeFOV()
        {
            _m.UnfreezeValue($"{FOVAddress}");
        }

        public float ReadDrawDistance()
        {
            return _m.ReadFloat($"{DrawDistanceAddress}");
        }

        public void WriteDrawDistance(float value)
        {
            _m.WriteMemory($"{DrawDistanceAddress}", "float", value.ToString());
        }

        public void FreezeDrawDistance(float value = 0)
        {
            if (value == 0)
            {
                value = ReadDrawDistance();
            }

            _m.FreezeValue($"{DrawDistanceAddress}", "float", value.ToString());
        }

        public void UnfreezeDrawDistance()
        {
            _m.UnfreezeValue($"{DrawDistanceAddress}");
        }
        #endregion

        #region Clock
        public float ReadClock()
        {
            return _m.ReadFloat($"{ClockAddress}");
        }

        public void WriteClock(float value)
        {
            _m.WriteMemory($"{ClockAddress}", "float", value.ToString());
        }

        public void FreezeClock(float value = 0)
        {
            if (value == 0)
            {
                value = ReadClock();
            }

            _m.FreezeValue($"{ClockAddress}", "float", value.ToString());
        }

        public void UnfreezeClock()
        {
            _m.UnfreezeValue($"{ClockAddress}");
        }
        #endregion

        #region Maps
        public virtual int ReadMapId()
        {
            return _m.ReadInt(MapIdAddress);
        }
        public abstract void LoadMap(int mapId);
        public abstract void LoadMap(int mapId, int entranceValue);
        public abstract void LoadMap(int mapId, int entranceValue, int mode);
        #endregion

        public abstract void SkipCurrentDialogue();
        public Controller_t GetController()
        {
            return new(_m, ControllerAddress);
        }
        protected abstract List<Character_t> GetCharacters();
        protected abstract List<Warp_t> GetEntranceLocations();
        protected abstract List<List<Gadget_t>> GetGadgets();
        protected abstract List<Map_t> GetMaps();
    }
}
