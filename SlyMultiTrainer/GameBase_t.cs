using System.Numerics;
using static SlyMultiTrainer.Util;

namespace SlyMultiTrainer
{
    public abstract class GameBase_t
    {
        private Form1 _form;
        private Memory.Mem _m;
        public string Region;

        protected string FOVAddress;
        protected string ClockAddress;
        protected string DrawDistanceAddress;
        protected string ResetCameraAddress;
        protected string CoinsAddress;
        protected string MapIdAddress;
        protected string GadgetAddress;
        protected string GuardAIAddress;

        protected List<Map_t> Maps;
        protected List<Character_t> Characters;

        protected GameBase_t(Memory.Mem m, Form1 form, string region)
        {
            _form = form;
            _m = m;
            Region = region;
            Maps = GetMaps();
            Characters = GetCharacters();
        }

        // Methods that are that and nothing else:
        //     public [type] [name]() { [implementation] }
        // Methods that must be implemented by the game (they are game specific):
        //     public abstract [type] [name]();
        // Methods that have default implementation but can be overridden by the game:
        //     public virtual [type] [name]() { [default implementation] }

        public void OnLoopTick()
        {
            // Shared logic
            _form.UpdateUI(_form.trkFOV, ReadFOV() * 10);
            _form.UpdateUI(_form.trkClock, ReadClock() * 10);
            _form.UpdateUI(_form.trkDrawDistance, ReadDrawDistance() * 10);

            // Set the warps for the current map
            var mapId = GetMapId();
            if (mapId != -1
                && _form.cmbWarps.DataSource as List<Warp_t> != Maps[mapId].Warps)
            {
                _form.UpdateUI(_form.cmbWarps, Maps[mapId].Warps);
                OnMapChange(mapId);
            }

            // Active character in the dropdown
            _form.UpdateUI(() =>
            {
                if (!_form.cmbActChar.DroppedDown)
                {
                    var currentId = ReadActCharId();
                    Character_t currentCharacter = Characters.Find(x => x.Id == currentId);
                    if (currentCharacter == null)
                    {
                        return;
                    }

                    if (currentCharacter != _form.cmbActChar.SelectedItem)
                    {
                        _form.UpdateUI(_form.cmbActChar, currentCharacter.Id - Characters.FirstOrDefault().Id);
                    }
                }
            });

            if (IsActCharAvailable())
            {
                UpdateActChar();
            }
            else
            {
                _form.UpdateUI(_form.lblXCoord, DefaultValueFloat);
                _form.UpdateUI(_form.lblYCoord, DefaultValueFloat);
                _form.UpdateUI(_form.lblZCoord, DefaultValueFloat);
                _form.UpdateUI(_form.chkActCharHealthFreeze, "0");
            }

            string tabName = "";
            _form.UpdateUI(() =>
            {
                tabName = _form.tabControlMain.SelectedTab.Name;
            });

            if (!IsLoading())
            {
                if (tabName == "tabEntities")
                {
                    UpdateEntities();
                }
                else if (tabName == "tabDAG")
                {
                    UpdateDAG();
                }
            }

            // Game specific logic
            CustomTick();
        }

        void UpdateActChar()
        {
            // Position and health
            Vector3 position = ReadActCharPosition();
            _form.UpdateUI(_form.lblXCoord, position.X);
            _form.UpdateUI(_form.lblYCoord, position.Y);
            _form.UpdateUI(_form.lblZCoord, position.Z);
            _form.UpdateUI(_form.chkActCharHealthFreeze, ReadActCharHealth());

            // Fly logic
            if (_form.chkActCharFly.Checked)
            {
                Controller_t controller = GetController();
                string FlyButtonUp = Properties.Settings.Default.FlyButtonUp;
                string FlyButtonDown = Properties.Settings.Default.FlyButtonDown;
                string FlyButtonAccelerate = Properties.Settings.Default.FlyButtonAccelerate;

                if (controller.IsButtonPressed(FlyButtonAccelerate))
                {
                    FreezeSpeedMultiplier(DefaultAmountToIncreaseOrDecreaseTranslationForActChar / 50);
                }
                else
                {
                    UnfreezeSpeedMultiplier();
                    WriteSpeedMultiplier(1);
                }

                if (controller.IsButtonPressed(FlyButtonUp))
                {
                    // up
                    //    unfreeze Z
                    //    set velocity Z to 500, keep freeze
                    UnfreezeActCharPositionZ();

                    if (controller.IsButtonPressed(FlyButtonAccelerate))
                    {
                        FreezeActCharVelocityZ((DefaultAmountToIncreaseOrDecreaseTranslationForActChar * 7).ToString());
                    }
                    else
                    {
                        FreezeActCharVelocityZ((DefaultAmountToIncreaseOrDecreaseTranslationForActChar * 3).ToString());
                    }

                }
                else if (controller.IsButtonPressed(FlyButtonDown))
                {
                    // down
                    //    unfreeze Z
                    //    set velocity Z to -500, keep freeze
                    UnfreezeActCharPositionZ();

                    if (controller.IsButtonPressed(FlyButtonAccelerate))
                    {
                        FreezeActCharVelocityZ((-DefaultAmountToIncreaseOrDecreaseTranslationForActChar * 7).ToString());
                    }
                    else
                    {
                        FreezeActCharVelocityZ((-DefaultAmountToIncreaseOrDecreaseTranslationForActChar * 3).ToString());
                    }
                }
                else
                {
                    // idle
                    //    freeze Z to current
                    //    freeze velocity Z to 0

                    // Using position.Z which is read a bit earlier makes the character stutter,
                    // so let's get the latest value possible by reading the position again
                    FreezeActCharPositionZ(ReadActCharPosition().Z.ToString());
                    FreezeActCharVelocityZ("0");

                    // It is possible that while in this if scope, the user disabled the fly function
                    // Let's check it again to see if we should keep the position and velocity frozen
                    if (!_form.chkActCharFly.Checked)
                    {
                        UnfreezeActCharVelocityZ();

                        // But only unfreeze the Z position if the checkbox for the z coordinate of the active character is not frozen
                        if (!_form.chkActCharZCoordFreeze.Checked)
                        {
                            UnfreezeActCharPositionZ();
                        }
                    }
                }
            }
        }

        void UpdateEntities()
        {
            if (_form.trvFKXList.Nodes.Count == 0)
            {
                _form.UpdateUI(() =>
                {
                    _form.btnRefreshFKXList_Click(_form.btnRefreshFKXList, EventArgs.Empty);
                });
            }

            int pointerToEntity = 0;
            _form.UpdateUI(() =>
            {
                pointerToEntity = _form.GetPointerToEntityFromSelectedFKXNode();
            });

            if (pointerToEntity == 0)
            {
                return;
            }

            Vector3 localTrans = ReadPositionFromPointerToEntity(pointerToEntity.ToString("X"));
            _form.UpdateUI(_form.lblFKXEntityXCoord, localTrans.X);
            _form.UpdateUI(_form.lblFKXEntityYCoord, localTrans.Y);
            _form.UpdateUI(_form.lblFKXEntityZCoord, localTrans.Z);

            Vector3 worldTrans = ReadWorldPositionFromPointerToEntity(pointerToEntity.ToString("X"));
            _form.UpdateUI(_form.lblFKXEntityXCoordWorld, worldTrans.X);
            _form.UpdateUI(_form.lblFKXEntityYCoordWorld, worldTrans.Y);
            _form.UpdateUI(_form.lblFKXEntityZCoordWorld, worldTrans.Z);

            float scale = ReadScaleFromPointerToEntity(pointerToEntity.ToString("X"));
            _form.UpdateUI(_form.trkFKXEntityScale, scale * 10);

            // Read rotation only if the edit checkbox is not checked
            if (!_form.chkFKXEntityEditRotation.Checked)
            {
                Matrix4x4 rotationMatrix = ReadWorldRotationFromPointerToEntity(pointerToEntity.ToString("X"));
                var euler = ExtractEulerAngles(rotationMatrix);
                _form.UpdateUI(_form.trkFKXEntityRotationX, euler.X);
                _form.UpdateUI(_form.trkFKXEntityRotationY, euler.Y);
                _form.UpdateUI(_form.trkFKXEntityRotationZ, euler.Z);
            }
        }

        void UpdateDAG()
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

            if (DAG.Graph == null)
            {
                DAG.Init();
                DAG.GetDAG();
                DAG.SetGraph();
                _form.UpdateUI(DAG.Viewer, true, "Enabled");
            }

            // We check some fields in every node in the loaded dag
            // to see if there is a mismatch between what we have on the dag and the value in-game
            // If there is a mismatch, either the in-game dag was changed by the game
            // or the user forcefully changed a field of the node

            // This variable is used to trigger a redraw
            bool redrawGraph = false;

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
                    DAG.Graph = null;
                    break;
                }

                // If there is a mismatch between what is in game and what we have on the trainer
                // = something changed
                bool areTasksEqual = DAG.IsTaskEqualToTask(task, taskInGame);
                if (!areTasksEqual)
                {
                    redrawGraph = true;
                    if (task.IsStateChangedByUser)
                    {
                        // If it was changed by the user (right click on the node, set state to)
                        // Then we also need to update the in-game dag
                        task.IsStateChangedByUser = false;
                        DAG.WriteTaskState(task);
                        DAG.WriteTaskFocusCount1(task);
                        DAG.WriteTaskFocusCount2(task);
                    }
                    else
                    {
                        task.State = taskInGame.State;
                        task.FocusCount1 = taskInGame.FocusCount1;
                        task.FocusCount2 = taskInGame.FocusCount2;
                    }

                    // Update the node's color
                    task.MsaglNode.Attr.FillColor = DAG.GetNodeColorFromState(task.State);

                    // Update the cluster's color only if this is the first node of the cluster
                    if (task.Cluster.Tasks.FirstOrDefault() == task)
                    {
                        task.Cluster.Subgraph.Attr.FillColor = DAG.GetClusterColorFromState(task.State);
                    }
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
                Cluster_t clusterInGame = DAG.ReadCluster(cluster.Address, false);

                bool areClustersEqual = DAG.IsClusterEqualToCluster(cluster, clusterInGame);
                if (!areClustersEqual)
                {
                    if (cluster.IsStateChangedByUser)
                    {
                        // user requested the suck change
                        DAG.WriteClusterSuck(cluster);
                    }
                    else
                    {
                        cluster.Suck = clusterInGame.Suck;
                    }
                }
            }

            if (redrawGraph)
            {
                DAG.Viewer.Invalidate();
            }
        }

        public abstract bool IsLoading();
        public abstract bool IsActCharAvailable();
        public abstract int ReadActCharHealth();
        public abstract void WriteActCharHealth(int value);
        public abstract void FreezeActCharHealth(int value = 0);
        public abstract void UnfreezeActCharHealth();
        public abstract Vector3 ReadPositionFromPointerToEntity(string pointerToEntity);
        public abstract void WritePositionFromPointerToEntity(string pointerToEntity, Vector3 value);
        public abstract void FreezePositionXFromPointerToEntity(string pointerToEntity, string value = "");
        public abstract void FreezePositionYFromPointerToEntity(string pointerToEntity, string value = "");
        public abstract void FreezePositionZFromPointerToEntity(string pointerToEntity, string value = "");
        public abstract void UnfreezePositionXFromPointerToEntity(string pointerToEntity);
        public abstract void UnfreezePositionYFromPointerToEntity(string pointerToEntity);
        public abstract void UnfreezePositionZFromPointerToEntity(string pointerToEntity);
        public abstract Vector3 ReadWorldPositionFromPointerToEntity(string pointerToEntity);
        public abstract Matrix4x4 ReadWorldRotationFromPointerToEntity(string pointerToEntity);
        public abstract void WriteWorldRotationFromPointerToEntity(string pointerToEntity, Matrix4x4 value);
        public abstract float ReadScaleFromPointerToEntity(string pointerToEntity);
        public abstract void WriteScaleFromPointerToEntity(string pointerToEntity, float value);

        public abstract Vector3 ReadActCharPosition();
        public abstract void WriteActCharPosition(Vector3 value);
        public abstract void FreezeActCharPositionX(string value = "");
        public abstract void FreezeActCharPositionY(string value = "");
        public abstract void FreezeActCharPositionZ(string value = "");
        public abstract void UnfreezeActCharPositionX();
        public abstract void UnfreezeActCharPositionY();
        public abstract void UnfreezeActCharPositionZ();
        public abstract void FreezeActCharVelocityZ(string value = "");
        public abstract void UnfreezeActCharVelocityZ();
        public abstract float ReadSpeedMultiplier();
        public abstract void WriteSpeedMultiplier(float value);
        public abstract void FreezeSpeedMultiplier(float value);
        public abstract void UnfreezeSpeedMultiplier();

        public abstract Controller_t GetController();

        public abstract int ReadActCharId();
        public abstract void WriteActCharId(int id);
        public abstract void FreezeActCharId(string value = "");
        public abstract void UnfreezeActCharId();

        public abstract void OnMapChange(int mapId);

        public abstract void ToggleUndetectable(bool enableUndetectable);

        public abstract void ToggleInvulnerable(bool enableInvulnerable);

        public abstract void ToggleInfiniteDbJump(bool enableInfDbJump);

        public virtual void ToggleGuardAI(bool disableGuardAI)
        {
            if (disableGuardAI)
            {
                _m.FreezeValue(GuardAIAddress, "int", "1");
            }
            else
            {
                _m.UnfreezeValue(GuardAIAddress);
                _m.WriteMemory(GuardAIAddress, "int", "0");
            }
        }

        public virtual long ReadGadgets()
        {
            long gadgets = _m.ReadLong(GadgetAddress);
            return gadgets;
        }

        public void ToggleAllGadgets()
        {
            if (this is Sly1Handler)
            {
                int gadgets = (int)(this as Sly1Handler).ReadGadgets();

                if (gadgets == -1)
                {
                    _m.WriteMemory(GadgetAddress, "int", "0");
                }
                else
                {
                    _m.WriteMemory(GadgetAddress, "int", (-1).ToString());
                }
            }
            else
            {
                long gadgets = ReadGadgets();
                if (gadgets == -1)
                {
                    _m.WriteMemory(GadgetAddress, "long", "0");
                }
                else
                {
                    _m.WriteMemory(GadgetAddress, "long", (-1).ToString());
                }
            }
        }

        public abstract void UnfreezeActCharGadgetPower();

        public abstract void FreezeActCharGadgetPower(int value = 0);

        public float ReadFOV()
        {
            float value = _m.ReadFloat($"{FOVAddress}");
            return value;
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

        public float ReadClock()
        {
            float clock = _m.ReadFloat($"{ClockAddress}");
            return clock;
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

        public float ReadDrawDistance()
        {
            float drawDistance = _m.ReadFloat($"{DrawDistanceAddress}");
            return drawDistance;
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

        public void ResetCamera()
        {
            _m.WriteMemory(ResetCameraAddress, "int", "1");
        }

        public void SetCoins(int value)
        {
            _m.WriteMemory(CoinsAddress, "int", value.ToString());
        }

        protected abstract List<Map_t> GetMaps();
        protected abstract List<Character_t> GetCharacters();

        public virtual int GetMapId()
        {
            if (Region == "NTSC E3 Demo")
            {
                byte tmp = _m.ReadByte(MapIdAddress);
                return tmp;
            }
            else if (Region == "NTSC March 17")
            {
                byte tmp = _m.ReadByte(MapIdAddress);

                if (tmp != 0)
                {
                    if (tmp <= 4)
                    {
                        tmp = (byte)(tmp - 1);
                    }
                    else if (tmp == 5)
                    {
                        tmp = (byte)(tmp + 1);
                    }
                    else if (tmp <= 7)
                    {
                        tmp = (byte)(tmp - 2);
                    }
                    else if (tmp <= 11)
                    {
                        tmp = (byte)(tmp + 1);
                    }
                    else if (tmp <= 14)
                    {
                        tmp = (byte)(tmp - 6);
                    }
                }

                return tmp;
            }

            int mapid = _m.ReadInt(MapIdAddress);
            return mapid;
        }

        public abstract void LoadMap(int mapId);
        public abstract void LoadMap(int mapId, int entranceValue);
        public abstract void LoadMap(int mapId, int entranceValue, int mode);

        public abstract void CustomTick();
    }
}
