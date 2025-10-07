using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Reflection;

namespace SlyMultiTrainer
{
    public partial class Form1 : Form
    {
        Memory.Mem _m;
        Dictionary<string, TabPage> _hiddenTabs = new();
        Image? _iconFreezeEnabled = Util.GetEmbeddedImage($"SlyMultiTrainer.Img.icon_freeze_enabled.png");
        Image? _iconFreezeDisabled = Util.GetEmbeddedImage($"SlyMultiTrainer.Img.icon_freeze_disabled.png");
        string _formTitle = "";
        GameBase_t _game;
        bool _triggerReattach = false;

        public Form1()
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void Init()
        {
            if (!bgWorkerMain.IsBusy)
            {
                var version = Assembly.GetExecutingAssembly()
                              .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                              ?.InformationalVersion;
                _formTitle = $"Sly Multi Trainer (v{version})";
                bgWorkerMain.RunWorkerAsync();
            }
        }

        private void bgWorkerMain_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            // We use do-while (true) loops instead of while (condition) so that we
            // - don't have to check for the condition (true) if we need to exit in the first loop
            // - can have a Thread.Sleep only if we really need to loop again

            // Clear controls
            UpdateUI(this, _formTitle);
            UpdateUI(this, Util.GetEmbeddedIcon($"SlyMultiTrainer.Img.icon_1_256x256.ico")!, "Icon");
            UpdateUI(lblAboutTitle, _formTitle);
            UpdateUI(lblXCoord, Util.DefaultValueFloat);
            UpdateUI(lblYCoord, Util.DefaultValueFloat);
            UpdateUI(lblZCoord, Util.DefaultValueFloat);
            UpdateUI(txtAddresses, "");
            ClearFKXTab();
            UpdateUI(btnCopyBase, false);
            HideTab("Entities");
            HideTab("DAG");
            HideTab("WorldStates");
            UpdateUI(chkDisableDeathBarrier, false);
            UpdateUI(chkDisableGuardAI, false);
            UpdateUI(chkToggleInvulnerable, false);
            UpdateUI(chkToggleUndetectable, false);
            UpdateUI(lblLuckyCharms, false);
            UpdateUI(cmbLuckyCharms, false);
            UpdateUI(chkLuckyCharmsFreeze, false);
            UpdateUI(btnSkipCurrentDialogue, false);
            for (int i = 0; i < tabMain.Controls.Count; i++)
            {
                UpdateUI(tabMain.Controls[i], false, "Enabled");
            }

            // Find pcsx2
            UpdateUI(lblMain, "Not attached (Scanning for PCSX2 process...)");
            UpdateUI(lblMain, Color.Red);
            do
            {
                Process[] processes = Process.GetProcesses();
                for (int i = 0; i < processes.Length; i++)
                {
                    if (processes[i].ProcessName.StartsWith("pcsx2"))
                    {
                        // Open it and set ee base
                        UpdateUI(lblMain, "PCSX2 process found, but game not started");
                        UpdateUI(lblMain, Color.DarkOrange);
                        _m = new();
                        if (!_m.OpenProcess(processes[i].Id))
                        {
                            bgWorkerMain.CancelAsync();
                            return;
                        }

                        break;
                    }
                }

                if (_m != null && _m.mProc != null)
                {
                    break;
                }

                Thread.Sleep(1000);
            } while (true);

            // Detect game build
            Util.Build_t? build = null;
            do
            {
                build = Util.GetBuild(_m);
                if (build != null
                    || _m.mProc.Process == null
                    || _m.mProc.Process != null && _m.mProc.Process.HasExited
                    || _triggerReattach)
                {
                    // We exit if we found a matching build or we closed pcsx2
                    break;
                }

                Thread.Sleep(1000);
            } while (true);

            // If the process was closed and a game was not yet selected
            if (_m.mProc.Process == null
                || _m.mProc.Process.HasExited
                || _triggerReattach)
            {
                bgWorkerMain.CancelAsync();
                _triggerReattach = false;
                _m.CloseProcess();
                return;
            }

            _game = Util.GetGameFromBuild(build, _m, this);
            InitBuildUI(build);

            while (true)
            {
                try
                {
                    if (_m.mProc.Process.HasExited)
                    {
                        _triggerReattach = true;
                    }
                    else if (!Util.IsBuildCurrent(_m, build))
                    {
                        // In older pcsx2 versions, when loading a savestate
                        // sometimes the ee region would be set to 0
                        // This would cause the comparison for the build to return false.
                        // So let's wait a bit and check again if the user actually changed the game
                        if (_m.eeBaseAddress == 0x20000000)
                        {
                            Thread.Sleep(100);
                        }

                        if (!Util.IsBuildCurrent(_m, build))
                        {
                            _triggerReattach = true;
                        }
                    }

                    if (_triggerReattach)
                    {
                        _triggerReattach = false;
                        throw new Exception();
                    }

                    // _m.DumpFrozenAddresses();

                    _game.OnLoopTick();
                    Thread.Sleep(50);
                }
                catch (Exception ex)
                {
                    UpdateUI(() =>
                    {
                        if (tabControlMain.Controls.ContainsKey("tabDAG"))
                        {
                            tabControlMain.Controls["tabDAG"]?.Controls.Clear();
                        }
                        else if (tabControlMain.Controls.ContainsKey("tabWorldStates"))
                        {
                            var tabWorldState = tabControlMain.Controls["tabWorldStates"]?.Controls["tabControlWorldStates"]?.Controls;
                            for (int i = 1; i <= 5; i++)
                            {
                                tabWorldState?[$"tabWorldState{i}"]?.Controls.Clear();
                            }
                        }
                    });

                    _m.UnfreezeAll();
                    _m.CloseProcess();
                    break;
                }
            }
        }

        private void bgWorkerMain_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (!IsDisposed)
            {
                // Restart
                Init();
            }
        }

        void InitBuildUI(Util.Build_t build)
        {
            UpdateUI(this, $"{_formTitle} - {build}");
            UpdateUI(lblMain, $"Attached - Base at {_m.eeBaseAddress:X}");
            UpdateUI(lblMain, Color.Green);
            UncheckAllCheckboxes(Controls);
            for (int i = 0; i < tabMain.Controls.Count; i++)
            {
                UpdateUI(tabMain.Controls[i], true, "Enabled");
            }
            _m.UnfreezeAll();
            SetTxtAddresses();

            UpdateUI(grpFOV, true, "Visible");
            UpdateUI(trkFOV, true, "Visible");
            UpdateUI(btnFOVReset, true, "Visible");
            UpdateUI(chkFOVFreeze, true, "Visible");
            UpdateUI(chkToggleUndetectable, true, "Visible");
            UpdateUI(chkToggleInvulnerable, true, "Visible");
            UpdateUI(chkDisableGuardAI, true, "Visible");
            UpdateUI(this, Util.GetEmbeddedIcon($"SlyMultiTrainer.Img.icon_{build.Title.Last()}_256x256.ico")!, "Icon");

            if (build.Title == "Sly 1")
            {
                if (build.Region == "NTSC Demo")
                {
                    UpdateUI(grpFOV, false, "Visible");
                    UpdateUI(trkFOV, false, "Visible");
                    UpdateUI(btnFOVReset, false, "Visible");
                    UpdateUI(chkFOVFreeze, false, "Visible");
                }
                UpdateUI(btnGetGadgets, "Toggle all\r\nthief moves");
                UpdateUI(lblHealth, "Lives");
                UpdateUI(chkInfiniteGadgetPower, false);
                UpdateUI(lblLuckyCharms, true);
                UpdateUI(cmbLuckyCharms, true);
                UpdateUI(chkLuckyCharmsFreeze, true);
                UpdateUI(chkToggleInfDbJump, true);
                UpdateUI(btnSkipCurrentDialogue, true);
                UpdateUI(chkDisableDeathBarrier, false);
                UpdateUI(chkDisableGuardAI, false);
                UpdateUI(chkToggleInvulnerable, false);
                UpdateUI(chkToggleUndetectable, false);
                UpdateUI(btnLoadLevelFull, false);
                ShowTab("WorldStates");
            }
            else if (build.Title == "Sly 2")
            {
                if (build.Region == "NTSC E3 Demo")
                {
                    UpdateUI(chkToggleUndetectable, false);
                    UpdateUI(chkToggleInvulnerable, false);
                    UpdateUI(chkDisableGuardAI, false);
                }
                else if (build.Region == "NTSC March 17")
                {
                    UpdateUI(chkToggleUndetectable, false);
                    UpdateUI(chkToggleInvulnerable, false);
                    UpdateUI(chkDisableGuardAI, true);
                }
                else
                {
                    UpdateUI(chkToggleUndetectable, true);
                    UpdateUI(chkToggleInvulnerable, true);
                    UpdateUI(chkDisableGuardAI, true);
                }

                UpdateUI(btnGetGadgets, "Toggle all\r\ngadgets");
                UpdateUI(lblHealth, "Health");
                UpdateUI(chkInfiniteGadgetPower, true);
                UpdateUI(lblLuckyCharms, false);
                UpdateUI(cmbLuckyCharms, false);
                UpdateUI(chkLuckyCharmsFreeze, false);
                UpdateUI(btnLoadLevelFull, false);
                UpdateUI(chkDisableDeathBarrier, false);
                UpdateUI(btnSkipCurrentDialogue, false);
                UpdateUI(chkToggleInfDbJump, true);
                ShowTab("DAG");
                ShowTab("Entities");
            }
            else if (build.Title == "Sly 3")
            {
                if (build.Region == "NTSC E3 Demo")
                {
                    UpdateUI(chkDisableDeathBarrier, false);
                    UpdateUI(chkToggleInvulnerable, false);
                    UpdateUI(chkToggleUndetectable, false);
                }
                else
                {
                    UpdateUI(chkDisableDeathBarrier, true);
                    UpdateUI(chkToggleInvulnerable, true);
                    UpdateUI(chkToggleUndetectable, true);
                }
                UpdateUI(btnGetGadgets, "Toggle all\r\ngadgets");
                UpdateUI(lblHealth, "Health");
                UpdateUI(chkInfiniteGadgetPower, true);
                UpdateUI(lblLuckyCharms, false);
                UpdateUI(cmbLuckyCharms, false);
                UpdateUI(chkLuckyCharmsFreeze, false);
                UpdateUI(btnLoadLevelFull, true);
                UpdateUI(btnSkipCurrentDialogue, false);
                UpdateUI(chkDisableGuardAI, true);
                UpdateUI(chkToggleInfDbJump, true);
                ShowTab("DAG");
                ShowTab("Entities");
            }

            chkFOVFreeze.BackgroundImage = _iconFreezeDisabled;
            chkClockFreeze.BackgroundImage = _iconFreezeDisabled;
            chkDrawDistanceFreeze.BackgroundImage = _iconFreezeDisabled;
        }

        private void SetTxtAddresses()
        {
            string tmp = "";
            UpdateUI(txtAddresses, "");
            var fields = _game.GetType()
                              .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(string))
                {
                    string value = (string)field.GetValue(_game);
                    if (value == null || value == "")
                    {
                        continue;
                    }

                    tmp += $"{field.Name} = {value}{Environment.NewLine}";
                }
                else if (field.FieldType == typeof(DAG_t))
                {
                    DAG_t DAG = (DAG_t)field.GetValue(_game);
                    var dagFields = DAG.GetType()
                                       .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (var dagField in dagFields)
                    {
                        if (dagField.FieldType == typeof(string))
                        {
                            string value = (string)dagField.GetValue(DAG);
                            if (value == null || value == "")
                            {
                                continue;
                            }

                            tmp += $"DAG.{dagField.Name} = {value}{Environment.NewLine}";
                        }
                    }
                }
            }

            UpdateUI(txtAddresses, tmp);
        }

        private void HideTab(string tabName)
        {
            tabName = $"tab{tabName}";
            TabPage tab = tabControlMain.TabPages[tabName];
            if (tab != null && !_hiddenTabs.ContainsKey(tabName))
            {
                _hiddenTabs[tabName] = tab;
                UpdateUI(() =>
                {
                    tabControlMain.TabPages.Remove(tab);
                });
            }
        }

        private void ShowTab(string tabName)
        {
            tabName = $"tab{tabName}";
            if (_hiddenTabs.TryGetValue(tabName, out TabPage tab))
            {
                UpdateUI(() =>
                {
                    tabControlMain.TabPages.Insert(1, tab);
                });

                _hiddenTabs.Remove(tabName);
            }
        }

        // This should be more like "AccessUI"
        public void UpdateUI(Action action)
        {
            if (InvokeRequired)
            {
                Invoke(action);
            }
            else
            {
                action();
            }
        }

        // Default property
        public void UpdateUI(object sender, object value)
        {
            if (sender is Label label)
            {
                if (value is Color)
                {
                    UpdateUI(label, (Color)value, "ForeColor");
                }
                else if (value is string)
                {
                    UpdateUI(label, (string)value, "Text");
                }
                else if (value is float)
                {
                    UpdateUI(label, ((float)value).ToString("0"), "Text");
                }
                else if (value is Enum)
                {
                    UpdateUI(label, ((Enum)value).ToString(), "Text");
                }
                else if (value is bool)
                {
                    UpdateUI(label, (bool)value, "Visible");
                }
            }
            else if (sender is CheckBox checkBox)
            {
                if (value is int)
                {
                    UpdateUI(checkBox, ((int)value).ToString(), "Text");
                }
                else if (value is string)
                {
                    UpdateUI(checkBox, (string)value, "Text");
                }
                else if (value is bool)
                {
                    UpdateUI(checkBox, (bool)value, "Visible");
                }
            }
            else if (sender is ComboBox comboBox)
            {
                if (value is System.Collections.IList list && list.Count > 0)
                {
                    UpdateUI(comboBox, list, "DataSource");
                }
                else if (value is int)
                {
                    UpdateUI(comboBox, (int)value, "SelectedIndex");
                }
                else if (value is bool)
                {
                    UpdateUI(comboBox, (bool)value, "Visible");
                }
            }
            else if (sender is TextBox textBox)
            {
                if (value is int)
                {
                    UpdateUI(textBox, ((int)value).ToString(), "Text");
                }
                else if (value is string)
                {
                    UpdateUI(textBox, (string)value, "Text");
                }
            }
            else if (sender is TrackBar trackBar)
            {
                if (value is float)
                {
                    if ((float)value > trackBar.Maximum)
                    {
                        value = (float)trackBar.Maximum;
                    }

                    UpdateUI(trackBar, (int)(float)value, "Value");
                }
            }
            else if (sender is Button button)
            {
                if (value is bool)
                {
                    UpdateUI(button, (bool)value, "Visible");
                }
                else if (value is string)
                {
                    UpdateUI(button, (string)value, "Text");
                }
            }
            else if (sender is Form form)
            {
                if (value is string)
                {
                    UpdateUI(form, (string)value, "Text");
                }
            }
        }

        // Specific property
        public void UpdateUI(object sender, object value, string propertyName)
        {
            UpdateUI(() =>
            {
                var property = sender.GetType().GetProperty(propertyName);
                if (property != null && property.CanWrite)
                {
                    try
                    {
                        var targetType = property.PropertyType;
                        if (value != null && !targetType.IsAssignableFrom(value.GetType()))
                        {
                            if (targetType.IsEnum && value is string enumString)
                            {
                                value = Enum.Parse(targetType, enumString);
                            }
                            else
                            {
                                value = Convert.ChangeType(value, targetType);
                            }
                        }

                        property.SetValue(sender, value);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed setting property \"{propertyName}\" of \"{(sender as Control).Name}\" with value \"{value}\"");
                    }
                }
            });
        }

        private void UncheckAllCheckboxes(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control is CheckBox checkBox)
                {
                    UpdateUI(checkBox, false, "Checked");
                }

                if (control.HasChildren)
                {
                    UncheckAllCheckboxes(control.Controls);
                }
            }
        }

        private void trkDrawDistance_Scroll(object sender, EventArgs e)
        {
            float value = (float)trkDrawDistance.Value / 10;
            _game.WriteDrawDistance(value);
        }

        private void btnDrawDistanceReset_Click(object sender, EventArgs e)
        {
            trkDrawDistance.Value = 10;
            trkDrawDistance_Scroll(trkDrawDistance, EventArgs.Empty);
        }

        private void chkDrawDistanceFreeze_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDrawDistanceFreeze.Checked)
            {
                _game.FreezeDrawDistance();
                chkDrawDistanceFreeze.BackgroundImage = _iconFreezeEnabled;
            }
            else
            {
                _game.UnfreezeDrawDistance();
                chkDrawDistanceFreeze.BackgroundImage = _iconFreezeDisabled;
            }
        }

        private void btnLoadLevel_Click(object sender, EventArgs e)
        {
            var originalIdx = Util.GetOriginalMapId(cmbMaps);
            if (_game is Sly1Handler)
            {
                _game.LoadMap(originalIdx);
            }
            else if (_game is Sly2Handler)
            {
                int entranceValue = 0x189;
                if (_game.Region == "NTSC July 11")
                {
                    entranceValue = 0x193;
                }
                else if (_game.Region == "NTSC E3 Demo")
                {
                    entranceValue = 0x17A;
                }
                else if (_game.Region == "NTSC March 17")
                {
                    entranceValue = 0x171;
                }

                _game.LoadMap(originalIdx, entranceValue);
            }
            else if (_game is Sly3Handler)
            {
                int entranceValue = 0x1A8;
                if (_game.Region == "NTSC July 16")
                {
                    entranceValue = 0x1A2;
                }
                else if (_game.Region == "PAL August 2")
                {
                    entranceValue = 0x1A6;
                }

                _game.LoadMap(originalIdx, entranceValue);
            }
        }

        private void btnLoadLevelFull_Click(object sender, EventArgs e)
        {
            var originalIdx = Util.GetOriginalMapId(cmbMaps);
            (_game as Sly3Handler).LoadMapFull(originalIdx);
        }

        private void btnActCharXCoordMinus_Click(object sender, EventArgs e)
        {
            Vector3 value = _game.ReadActCharPosition();
            value.X -= Util.DefaultAmountToIncreaseOrDecreaseTranslationForActChar;
            _game.WriteActCharPosition(value);
        }

        private void btnActCharXCoordPlus_Click(object sender, EventArgs e)
        {
            Vector3 value = _game.ReadActCharPosition();
            value.X += Util.DefaultAmountToIncreaseOrDecreaseTranslationForActChar;
            _game.WriteActCharPosition(value);
        }

        private void chkActCharXCoordFreeze_CheckedChanged(object sender, EventArgs e)
        {
            if (chkActCharXCoordFreeze.Checked)
            {
                _game.FreezeActCharPositionX();
            }
            else
            {
                _game.UnfreezeActCharPositionX();
            }
        }

        private void btnActCharYCoordMinus_Click(object sender, EventArgs e)
        {
            Vector3 value = _game.ReadActCharPosition();
            value.Y -= Util.DefaultAmountToIncreaseOrDecreaseTranslationForActChar;
            _game.WriteActCharPosition(value);
        }

        private void btnActCharYCoordPlus_Click(object sender, EventArgs e)
        {
            Vector3 value = _game.ReadActCharPosition();
            value.Y += Util.DefaultAmountToIncreaseOrDecreaseTranslationForActChar;
            _game.WriteActCharPosition(value);
        }

        private void chkActCharYCoordFreeze_CheckedChanged(object sender, EventArgs e)
        {
            if (chkActCharYCoordFreeze.Checked)
            {
                _game.FreezeActCharPositionY();
            }
            else
            {
                _game.UnfreezeActCharPositionY();
            }
        }

        private void btnActCharZCoordMinus_Click(object sender, EventArgs e)
        {
            Vector3 value = _game.ReadActCharPosition();
            value.Z -= Util.DefaultAmountToIncreaseOrDecreaseTranslationForActChar;
            _game.WriteActCharPosition(value);
        }

        private void btnActCharZCoordPlus_Click(object sender, EventArgs e)
        {
            Vector3 value = _game.ReadActCharPosition();
            value.Z += Util.DefaultAmountToIncreaseOrDecreaseTranslationForActChar;
            _game.WriteActCharPosition(value);
        }

        private void chkActCharZCoordFreeze_CheckedChanged(object sender, EventArgs e)
        {
            if (chkActCharZCoordFreeze.Checked)
            {
                _game.FreezeActCharPositionZ();
            }
            else
            {
                _game.UnfreezeActCharPositionZ();
            }
        }

        private void btnActCharXCoordSet_Click(object sender, EventArgs e)
        {
            float.TryParse(txtActCharXCoordSet.Text, CultureInfo.InvariantCulture, out float value);
            Vector3 trans = _game.ReadActCharPosition();
            trans.X = value;
            _game.WriteActCharPosition(trans);
        }

        private void btnActCharYCoordSet_Click(object sender, EventArgs e)
        {
            float.TryParse(txtActCharYCoordSet.Text, CultureInfo.InvariantCulture, out float value);
            Vector3 trans = _game.ReadActCharPosition();
            trans.Y = value;
            _game.WriteActCharPosition(trans);
        }

        private void btnActCharZCoordSet_Click(object sender, EventArgs e)
        {
            float.TryParse(txtActCharZCoordSet.Text, CultureInfo.InvariantCulture, out float value);
            Vector3 trans = _game.ReadActCharPosition();
            trans.Z = value;
            _game.WriteActCharPosition(trans);
        }

        private void trkActCharCoord_Scroll(object sender, EventArgs e)
        {
            if (trkActCharCoord.Value == 0)
            {
                Util.DefaultAmountToIncreaseOrDecreaseTranslationForActChar = 10;
            }
            else
            {
                Util.DefaultAmountToIncreaseOrDecreaseTranslationForActChar = trkActCharCoord.Value * 50;
            }
        }

        private void btnActCharHealthMinus_Click(object sender, EventArgs e)
        {
            int health = _game.ReadActCharHealth();
            health -= Util.DefaultAmountToIncreaseOrDecreaseHealth;
            _game.WriteActCharHealth(health);
        }

        private void btnActCharHealthPlus_Click(object sender, EventArgs e)
        {
            int health = _game.ReadActCharHealth();
            health += Util.DefaultAmountToIncreaseOrDecreaseHealth;
            _game.WriteActCharHealth(health);
        }

        private void chkActCharHealthFreeze_CheckedChanged(object sender, EventArgs e)
        {
            if (chkActCharHealthFreeze.Checked)
            {
                _game.FreezeActCharHealth();
            }
            else
            {
                _game.UnfreezeActCharHealth();
            }
        }

        private void cmbActChar_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (_game is Sly1Handler)
            {
                return;
            }

            Util.Character_t selectedItem = cmbActChar.SelectedItem as Util.Character_t;
            _game.WriteActCharId(selectedItem.Id);
        }

        private void chkActCharFreeze_CheckedChanged(object sender, EventArgs e)
        {
            if (_game is Sly1Handler)
            {
                return;
            }

            if (chkActCharFreeze.Checked)
            {
                Util.Character_t selectedItem = cmbActChar.SelectedItem as Util.Character_t;
                _game.FreezeActCharId(selectedItem.Id.ToString());
            }
            else
            {
                _game.UnfreezeActCharId();
            }
        }

        private void chkActCharFly_CheckedChanged(object sender, EventArgs e)
        {
            if (chkActCharFly.Checked)
            {
                _game.FreezeActCharPositionZ();
            }
            else
            {
                if (!chkActCharZCoordFreeze.Checked)
                {
                    _game.UnfreezeActCharPositionZ();
                }
                _game.UnfreezeActCharVelocityZ();
            }
        }

        private void chkInfiniteGadgetPower_CheckedChanged(object sender, EventArgs e)
        {
            if (chkInfiniteGadgetPower.Checked)
            {
                _game.FreezeActCharGadgetPower(100);
            }
            else
            {
                _game.UnfreezeActCharGadgetPower();
            }
        }

        private void btnGetGadgets_Click(object sender, EventArgs e)
        {
            _game.ToggleAllGadgets();
        }

        private void trkFOV_Scroll(object sender, EventArgs e)
        {
            float value = (float)trkFOV.Value / 10;
            _game.WriteFOV(value);
        }

        private void btnFOVReset_Click(object sender, EventArgs e)
        {
            float value = 1f; // Sly 1 uses 1.0f
            if (_game is Sly2Handler || _game is Sly3Handler)
            {
                value = 1.1f;
            }

            trkFOV.Value = (int)(value * 10);
            trkFOV_Scroll(trkFOV, EventArgs.Empty);
        }

        private void chkFOVFreeze_CheckedChanged(object sender, EventArgs e)
        {
            if (chkFOVFreeze.Checked)
            {
                _game.FreezeFOV();
                chkFOVFreeze.BackgroundImage = _iconFreezeEnabled;
            }
            else
            {
                _game.UnfreezeFOV();
                chkFOVFreeze.BackgroundImage = _iconFreezeDisabled;
            }
        }

        private void trkClock_Scroll(object sender, EventArgs e)
        {
            float value = (float)trkClock.Value / 10;
            _game.WriteClock(value);
        }

        private void btnClockReset_Click(object sender, EventArgs e)
        {
            trkClock.Value = 10;
            trkClock_Scroll(trkClock, EventArgs.Empty);
        }

        private void chkClockFreeze_CheckedChanged(object sender, EventArgs e)
        {
            if (chkClockFreeze.Checked)
            {
                _game.FreezeClock();
                chkClockFreeze.BackgroundImage = _iconFreezeEnabled;
            }
            else
            {
                _game.UnfreezeClock();
                chkClockFreeze.BackgroundImage = _iconFreezeDisabled;
            }
        }

        private void btnResetCamera_Click(object sender, EventArgs e)
        {
            _game.ResetCamera();
        }

        private void btnCoinsSet_Click(object sender, EventArgs e)
        {
            int.TryParse(txtCoins.Text, CultureInfo.InvariantCulture, out int value);
            _game.SetCoins(value);
        }

        private void btnWarp_Click(object sender, EventArgs e)
        {
            Util.Warp_t warp = (Util.Warp_t)cmbWarps.SelectedItem;
            _game.WriteActCharPosition(warp.Position);
            _game.ResetCamera();
        }

        private void chkDisableDeathBarrier_CheckedChanged(object sender, EventArgs e)
        {
            if (_game is Sly3Handler)
            {
                (_game as Sly3Handler).ToggleDeathBarriers(chkDisableDeathBarrier.Checked);
            }
        }

        private void chkDisableGuardAI_CheckedChanged(object sender, EventArgs e)
        {
            _game.ToggleGuardAI(chkDisableGuardAI.Checked);
        }

        private void chkToggleInvulnerable_CheckedChanged(object sender, EventArgs e)
        {
            _game.ToggleInvulnerable(chkToggleInvulnerable.Checked);
        }

        private void chkToggleUndetectable_CheckedChanged(object sender, EventArgs e)
        {
            _game.ToggleUndetectable(chkToggleUndetectable.Checked);
        }

        private void chkToggleInfDbJump_CheckedChanged(object sender, EventArgs e)
        {
            _game.ToggleInfiniteDbJump(chkToggleInfDbJump.Checked);
        }

        private void chkLuckyCharmsFreeze_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLuckyCharmsFreeze.Checked)
            {
                (_game as Sly1Handler).FreezeLuckyCharms();
            }
            else
            {
                (_game as Sly1Handler).UnfreezeLuckyCharms();
            }
        }

        private void cmbLuckyCharms_SelectionChangeCommitted(object sender, EventArgs e)
        {
            (_game as Sly1Handler).WriteLuckyCharms(cmbLuckyCharms.SelectedIndex);
        }

        private void btnSkipCurrentDialogue_Click(object sender, EventArgs e)
        {
            (_game as Sly1Handler).SkipCurrentDialogue();
        }

        private void tabControlMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab.Name == "tabDAG"
                && tabControlMain.SelectedTab.Controls.Count == 0)
            {
                DAG_t DAG = null;
                if (_game is Sly2Handler)
                {
                    DAG = (_game as Sly2Handler).DAG;
                }
                else if (_game is Sly3Handler)
                {
                    DAG = (_game as Sly3Handler).DAG;
                }

                // https://www.microsoft.com/en-us/research/project/microsoft-automatic-graph-layout/code-samples/
                SuspendLayout();
                tabControlMain.SelectedTab.Controls.Add(DAG.Viewer);
                ResumeLayout();
            }
        }

        public void btnRefreshFKXList_Click(object sender, EventArgs e)
        {
            List<Util.FKXEntry_t> fkxList = new();
            if (_game is Sly2Handler)
            {
                fkxList = (_game as Sly2Handler).GetFKXList();
            }
            else if (_game is Sly3Handler)
            {
                fkxList = (_game as Sly3Handler).GetFKXList();
            }

            FillFKXTreeView(fkxList);
            ClearFKXTab();
        }

        private void FillFKXTreeView(List<Util.FKXEntry_t> fkxList)
        {
            trvFKXList.BeginUpdate();
            trvFKXList.Nodes.Clear();
            for (int i = 0; i < fkxList.Count; i++)
            {
                TreeNode node = new($"{fkxList[i].Name} ({fkxList[i].Count})");
                node.Tag = fkxList[i];
                trvFKXList.Nodes.Add(node);

                if (fkxList[i].PoolPointer == 0x0)
                {
                    continue;
                }

                for (int j = 0; j < fkxList[i].Count; j++)
                {
                    string entityPointer = fkxList[i].EntityPointer[j].ToString("X");
                    TreeNode childNode = new(entityPointer);
                    childNode.Tag = entityPointer;
                    node.Nodes.Add(childNode);
                }
            }

            trvFKXList.EndUpdate();
        }

        private void trvFKXList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is string)
            {
                // npc_boar_guard[0] = EF45B0
                UpdateUI(lblFKXEntityInfo, $"{(e.Node.Parent.Tag as Util.FKXEntry_t).Name}[{e.Node.Index}] = {e.Node.Tag}");
            }
            else if (e.Node.Tag is Util.FKXEntry_t)
            {
                ClearFKXTab();
                // npc_boar_guard = EF45B0
                UpdateUI(lblFKXEntityInfo, $"{(e.Node.Tag as Util.FKXEntry_t).Name} = {(e.Node.Tag as Util.FKXEntry_t).Address}");
                return;
            }
        }

        public int GetPointerToEntityFromSelectedFKXNode()
        {
            var node = trvFKXList.SelectedNode;
            if (node == null || node.Tag is not string)
            {
                return 0;
            }

            var fkx = node.Parent.Tag as Util.FKXEntry_t;
            int pointerToEntity = fkx.PoolPointer + node.Index * 4;
            return pointerToEntity;
        }

        private void btnCopyFKXEntityPointer_Click(object sender, EventArgs e)
        {
            if (trvFKXList.SelectedNode == null)
            {
                return;
            }

            if (trvFKXList.SelectedNode.Tag is string)
            {
                Clipboard.SetText(trvFKXList.SelectedNode.Tag.ToString());
            }
            else if (trvFKXList.SelectedNode.Tag is Util.FKXEntry_t)
            {
                Clipboard.SetText((trvFKXList.SelectedNode.Tag as Util.FKXEntry_t).Address);
            }
        }

        private Vector3 GetCurrentFKXEntityPosition(out string pointerToEntity)
        {
            int value = GetPointerToEntityFromSelectedFKXNode();
            pointerToEntity = value.ToString("X");
            if (value == 0)
            {
                return Vector3.Zero;
            }

            Vector3 trans = _game.ReadPositionFromPointerToEntity(pointerToEntity);
            return trans;
        }

        private void btnFKXEntityXCoordMinus_Click(object sender, EventArgs e)
        {
            Vector3 trans = GetCurrentFKXEntityPosition(out string pointerToEntity);
            trans.X -= Util.DefaultAmountToIncreaseOrDecreaseTranslationForFKXEntity;
            _game.WritePositionFromPointerToEntity(pointerToEntity, trans);
        }

        private void btnFKXEntityXCoordPlus_Click(object sender, EventArgs e)
        {
            Vector3 trans = GetCurrentFKXEntityPosition(out string pointerToEntity);
            trans.X += Util.DefaultAmountToIncreaseOrDecreaseTranslationForFKXEntity;
            _game.WritePositionFromPointerToEntity(pointerToEntity, trans);
        }

        private void chkFKXEntityXCoordFreeze_CheckedChanged(object sender, EventArgs e)
        {
            int pointerToEntity = GetPointerToEntityFromSelectedFKXNode();
            if (pointerToEntity == 0)
            {
                return;
            }

            if (chkFKXEntityXCoordFreeze.Checked)
            {
                _game.FreezePositionXFromPointerToEntity(pointerToEntity.ToString("X"));
            }
            else
            {
                _game.UnfreezePositionXFromPointerToEntity(pointerToEntity.ToString("X"));
            }
        }

        private void btnFKXEntityXCoordSet_Click(object sender, EventArgs e)
        {
            float.TryParse(txtFKXEntityXCoordSet.Text, CultureInfo.InvariantCulture, out float value);
            Vector3 trans = GetCurrentFKXEntityPosition(out string pointerToEntity);
            trans.X = value;
            _game.WritePositionFromPointerToEntity(pointerToEntity, trans);
        }

        private void btnFKXEntityYCoordMinus_Click(object sender, EventArgs e)
        {
            Vector3 trans = GetCurrentFKXEntityPosition(out string pointerToEntity);
            trans.Y -= Util.DefaultAmountToIncreaseOrDecreaseTranslationForFKXEntity;
            _game.WritePositionFromPointerToEntity(pointerToEntity, trans);
        }

        private void btnFKXEntityYCoordPlus_Click(object sender, EventArgs e)
        {
            Vector3 trans = GetCurrentFKXEntityPosition(out string pointerToEntity);
            trans.Y += Util.DefaultAmountToIncreaseOrDecreaseTranslationForFKXEntity;
            _game.WritePositionFromPointerToEntity(pointerToEntity, trans);
        }

        private void chkFKXEntityYCoordFreeze_CheckedChanged(object sender, EventArgs e)
        {
            int pointerToEntity = GetPointerToEntityFromSelectedFKXNode();
            if (pointerToEntity == 0)
            {
                return;
            }

            if (chkFKXEntityYCoordFreeze.Checked)
            {
                _game.FreezePositionYFromPointerToEntity(pointerToEntity.ToString("X"));
            }
            else
            {
                _game.UnfreezePositionYFromPointerToEntity(pointerToEntity.ToString("X"));
            }
        }

        private void btnFKXEntityYCoordSet_Click(object sender, EventArgs e)
        {
            float.TryParse(txtFKXEntityYCoordSet.Text, CultureInfo.InvariantCulture, out float value);
            Vector3 trans = GetCurrentFKXEntityPosition(out string pointerToEntity);
            trans.Y = value;
            _game.WritePositionFromPointerToEntity(pointerToEntity, trans);
        }

        private void btnFKXEntityZCoordMinus_Click(object sender, EventArgs e)
        {
            Vector3 trans = GetCurrentFKXEntityPosition(out string pointerToEntity);
            trans.Z -= Util.DefaultAmountToIncreaseOrDecreaseTranslationForFKXEntity;
            _game.WritePositionFromPointerToEntity(pointerToEntity, trans);
        }

        private void btnFKXEntityZCoordPlus_Click(object sender, EventArgs e)
        {
            Vector3 trans = GetCurrentFKXEntityPosition(out string pointerToEntity);
            trans.Z += Util.DefaultAmountToIncreaseOrDecreaseTranslationForFKXEntity;
            _game.WritePositionFromPointerToEntity(pointerToEntity, trans);
        }

        private void chkFKXEntityZCoordFreeze_CheckedChanged(object sender, EventArgs e)
        {
            int pointerToEntity = GetPointerToEntityFromSelectedFKXNode();
            if (pointerToEntity == 0)
            {
                return;
            }

            if (chkFKXEntityZCoordFreeze.Checked)
            {
                _game.FreezePositionZFromPointerToEntity(pointerToEntity.ToString("X"));
            }
            else
            {
                _game.UnfreezePositionZFromPointerToEntity(pointerToEntity.ToString("X"));
            }
        }

        private void btnFKXEntityZCoordSet_Click(object sender, EventArgs e)
        {
            float.TryParse(txtFKXEntityZCoordSet.Text, CultureInfo.InvariantCulture, out float value);
            Vector3 trans = GetCurrentFKXEntityPosition(out string pointerToEntity);
            trans.Z = value;
            _game.WritePositionFromPointerToEntity(pointerToEntity, trans);
        }

        private void trkFKXEntityCoord_Scroll(object sender, EventArgs e)
        {
            if (trkFKXEntityCoord.Value == 0)
            {
                Util.DefaultAmountToIncreaseOrDecreaseTranslationForFKXEntity = 10;
            }
            else
            {
                Util.DefaultAmountToIncreaseOrDecreaseTranslationForFKXEntity = trkFKXEntityCoord.Value * 50;
            }
        }

        private void trkFKXEntityScale_Scroll(object sender, EventArgs e)
        {
            float trkValue = (float)trkFKXEntityScale.Value / 10;
            int pointerToEntity = GetPointerToEntityFromSelectedFKXNode();
            _game.WriteScaleFromPointerToEntity(pointerToEntity.ToString("X"), trkValue);
        }

        private void btnFKXEntityScaleReset_Click(object sender, EventArgs e)
        {
            trkFKXEntityScale.Value = 10;
            trkFKXEntityScale_Scroll(trkFKXEntityScale, EventArgs.Empty);
        }

        private void chkFKXEntityEditRotation_CheckedChanged(object sender, EventArgs e)
        {
            trkFKXEntityRotationX.Enabled = chkFKXEntityEditRotation.Checked;
            trkFKXEntityRotationY.Enabled = chkFKXEntityEditRotation.Checked;
            trkFKXEntityRotationZ.Enabled = chkFKXEntityEditRotation.Checked;
        }

        private void btnFKXEntityWarpActChar_Click(object sender, EventArgs e)
        {
            int pointerToEntity = GetPointerToEntityFromSelectedFKXNode();
            if (pointerToEntity == 0)
            {
                return;
            }

            Vector3 trans = _game.ReadWorldPositionFromPointerToEntity(pointerToEntity.ToString("X"));
            _game.WriteActCharPosition(trans);
        }

        private void trkFKXEntityRotationX_Scroll(object sender, EventArgs e)
        {
            WriteRotationToSelectedFKXNode();
        }

        private void trkFKXEntityRotationY_Scroll(object sender, EventArgs e)
        {
            WriteRotationToSelectedFKXNode();
        }

        private void trkFKXEntityRotationZ_Scroll(object sender, EventArgs e)
        {
            WriteRotationToSelectedFKXNode();
        }

        private void WriteRotationToSelectedFKXNode()
        {
            int pointerToEntity = GetPointerToEntityFromSelectedFKXNode();

            // Degrees to radians
            float radX = MathF.PI / 180f * trkFKXEntityRotationX.Value;
            float radY = MathF.PI / 180f * trkFKXEntityRotationY.Value;
            float radZ = MathF.PI / 180f * trkFKXEntityRotationZ.Value;

            Matrix4x4 rotationMatrix = Matrix4x4.CreateRotationX(radX) * Matrix4x4.CreateRotationY(radY) * Matrix4x4.CreateRotationZ(radZ);

            _game.WriteWorldRotationFromPointerToEntity(pointerToEntity.ToString("X"), rotationMatrix);
        }

        private void ClearFKXTab()
        {
            UpdateUI(lblFKXEntityInfo, Util.DefaultValueString);
            UpdateUI(lblFKXEntityXCoord, Util.DefaultValueFloat);
            UpdateUI(lblFKXEntityYCoord, Util.DefaultValueFloat);
            UpdateUI(lblFKXEntityZCoord, Util.DefaultValueFloat);
            UpdateUI(lblFKXEntityXCoordWorld, Util.DefaultValueFloat);
            UpdateUI(lblFKXEntityYCoordWorld, Util.DefaultValueFloat);
            UpdateUI(lblFKXEntityZCoordWorld, Util.DefaultValueFloat);
            chkFKXEntityEditRotation.Checked = false;
            chkFKXEntityEditRotation_CheckedChanged(chkFKXEntityEditRotation, EventArgs.Empty);
        }

        private void ToolStripMenuItemActCharCoordsCopyOver_Click(object sender, EventArgs e)
        {
            txtActCharXCoordSet.Text = lblXCoord.Text;
            txtActCharYCoordSet.Text = lblYCoord.Text;
            txtActCharZCoordSet.Text = lblZCoord.Text;
        }

        private void ToolStripMenuItemActCharCoordsPasteXYZFromClipboard_Click(object sender, EventArgs e)
        {
            string clipboard = Clipboard.GetText();
            string[] coords = clipboard.Split(' ');
            if (coords.Length != 3)
            {
                return;
            }

            float.TryParse(coords[0], CultureInfo.InvariantCulture, out float value1);
            float.TryParse(coords[1], CultureInfo.InvariantCulture, out float value2);
            float.TryParse(coords[2], CultureInfo.InvariantCulture, out float value3);
            txtActCharXCoordSet.Text = value1.ToString();
            txtActCharYCoordSet.Text = value2.ToString();
            txtActCharZCoordSet.Text = value3.ToString();
        }

        private void ToolStripMenuItemActCharCoordsSetXYZ_Click(object sender, EventArgs e)
        {
            //btnActCharXCoordSet_Click(btnActCharXCoordSet, e);
            //btnActCharYCoordSet_Click(btnActCharYCoordSet, e);
            //btnActCharZCoordSet_Click(btnActCharZCoordSet, e);
            float.TryParse(txtActCharXCoordSet.Text, CultureInfo.InvariantCulture, out float value1);
            float.TryParse(txtActCharYCoordSet.Text, CultureInfo.InvariantCulture, out float value2);
            float.TryParse(txtActCharZCoordSet.Text, CultureInfo.InvariantCulture, out float value3);
            Vector3 trans = new(value1, value2, value3);
            _game.WriteActCharPosition(trans);
        }

        private void btnCopyBase_Click(object sender, EventArgs e)
        {
            if (_m.eeBaseAddress != 0)
            {
                Clipboard.SetText(_m.eeBaseAddress.ToString("X"));
            }
        }

        private void btnReattach_Click(object sender, EventArgs e)
        {
            _triggerReattach = true;
        }
    }
}
