using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Reflection;

namespace SlyMultiTrainer
{
    public partial class Form1 : Form
    {
        Memory.Mem _m;
        GameBase_t? _game;
        Dictionary<string, TabPage> _hiddenTabs = new();
        Image? _iconFreezeEnabled = Util.GetEmbeddedImage($"icon_freeze_enabled.png");
        Image? _iconFreezeDisabled = Util.GetEmbeddedImage($"icon_freeze_disabled.png");
        string _formTitle = "";
        bool _triggerReattach = false;
        Font _monospaceFont;

        public Form1()
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            _monospaceFont = new Font("Courier New", 8F);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Run once to fill the process list immediately
            FillcmbProcesses();
            Init();
            Task.Run(async () =>
            {
                while (true)
                {
                    FillcmbProcesses();
                    await Task.Delay(1000);
                }
            });
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
                if (version!.Contains('+'))
                {
                    version = version!.Split('+')[0];
                }

                _formTitle = $"Sly Multi Trainer (v{version})";
                bgWorkerMain.RunWorkerAsync();
            }
        }

        private void bgWorkerMain_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            // We use do-while (true) loops instead of while (condition) so that we
            // - don't have to check for the condition (true) if we need to exit in the first loop
            // - can have a Thread.Sleep only if we really need to loop again
            // We shouldn't ever call Thread.Sleep if the emulator is already running with a game open

            // Clear controls
            UpdateUI(this, _formTitle);
            UpdateUI(this, Util.GetEmbeddedIcon($"icon_1_256x256.ico")!, "Icon");
            UpdateUI(lblAboutTitle, _formTitle);
            UpdateUI(lblXCoord, Util.DefaultValueFloat);
            UpdateUI(lblYCoord, Util.DefaultValueFloat);
            UpdateUI(lblZCoord, Util.DefaultValueFloat);
            UpdateUI(chkActCharHealthFreeze, Util.DefaultValueInt);
            UpdateUI(lblSpeed, Util.DefaultValueFloat);
            UpdateUI(txtAddresses, "");
            UpdateUI(txtStringsLocalized, "");
            UpdateUI(txtStringsSavefile, "");
            UpdateUI(chkStringsMonospaceFont, false, "Checked");
            ClearEntitiesTab();
            HideTab("Entities");
            HideTab("DAG");
            HideTab("Strings");
            HideTab("WorldStates");
            for (int i = 0; i < tabMain.Controls.Count; i++)
            {
                UpdateUI(tabMain.Controls[i], false, "Enabled");
            }

            // Find the emulator
            UpdateUI(lblProcessStatus, "Not attached (Scanning for PCSX2/RPCS3 process...)");
            UpdateUI(lblProcessStatus, Color.Red);
            do
            {
                UpdateUI(() =>
                {
                    var process = cmbProcesses.SelectedItem as Memory.Proc;
                    if (process == null)
                    {
                        return;
                    }

                    // Open it and set base
                    _m = new();
                    if (!_m.OpenProcess(process.Process.Id))
                    {
                        _m.CloseProcess();
                        return;
                    }

                    UpdateUI(lblProcessStatus, $"{_m.displayName} process found, but game build not detected");
                    UpdateUI(lblProcessStatus, Color.DarkOrange);
                });

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
                    // We exit if we found a matching build or we closed the emulator
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

            _game = Util.GetGameFromBuild(this, _m, build);
            UpdateUI(cmbMaps, _game.Maps.Where(x => x.IsVisible).ToList());
            UpdateUI(cmbMaps, _game.Maps, "Tag");
            UpdateUI(cmbActChar, _game.Characters);
            InitBuildUI(build);
            _game.CustomWarps = Util.GetCustomWarps(build);

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
                        // When loading a savestate in older pcsx2 versions sometimes the ee region would be set to 0
                        // This would cause the comparison for the build to return false.
                        // So let's wait a bit and check again if the user actually changed the game
                        if (_m.baseAddress == 0x20000000)
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

                    //_m.DumpFrozenAddresses();

                    _game.OnLoopTick();
                    Thread.Sleep(50);
                }
                catch (Exception ex)
                {
                    // close sub forms
                    FormCollection forms = Application.OpenForms;
                    for (int i = 0; i < forms.Count; i++)
                    {
                        if (forms[i] != this)
                        {
                            UpdateUI(() =>
                            {
                                forms[i].DialogResult = DialogResult.OK;
                                forms[i].Close();
                            });
                        }
                    }

                    // Clear controls added during runtime
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
                                var world = tabWorldState?[$"tabWorldState{i}"];
                                if (world != null)
                                {
                                    world.Controls.Clear();
                                }
                            }
                        }
                    });

                    _m.CloseProcess();
                    _game = null;
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
            UpdateUI(() =>
            {
                this.Text = $"{_formTitle} - {build}";
                lblProcessStatus.Text = $"Attached - Base at {_m.baseAddress:X}";
                lblProcessStatus.ForeColor = Color.Green;
                this.Icon = Util.GetEmbeddedIcon($"icon_{build.Title.Last()}_256x256.ico")!;
                ToolStripMenuItemActCharManageCustomWarps.Text = $"Manage {build.Title} custom warps...";
                SetTxtAddresses();
                SetControlsToDefault(tabMain.Controls);
            });

            chkFOVFreeze.BackgroundImage = _iconFreezeDisabled;
            chkClockFreeze.BackgroundImage = _iconFreezeDisabled;
            chkDrawDistanceFreeze.BackgroundImage = _iconFreezeDisabled;

            // All controls here have visible and enabled set to true
            // We change the visible property to false based on the game and build
            if (build.Title == "Sly 1")
            {
                ShowTab("WorldStates");
                UpdateUI(lblHealth, "Lives");
                UpdateUI(lblGadgetL1, false);
                UpdateUI(lblGadgetL2, false);
                UpdateUI(lblGadgetR2, false);
                UpdateUI(cmbGadgetL1, false);
                UpdateUI(cmbGadgetL2, false);
                UpdateUI(cmbGadgetR2, false);
                UpdateUI(chkGadgetInfinitePower, false);
                UpdateUI(chkDisableGuardAI, false);
                UpdateUI(chkToggleInvulnerable, false);
                UpdateUI(chkToggleUndetectable, false);
                UpdateUI(btnLoadMapFull, false);

                if (build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCDemo])
                {
                    UpdateUI(grpFOV, false);
                    UpdateUI(btnFOVReset, false);
                    UpdateUI(chkFOVFreeze, false);
                    UpdateUI(chkToggleNoclip, false);
                }
                else if (build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCDemoJune14])
                {
                    UpdateUI(chkToggleNoclip, false);
                }
                else if (build.Region == Util.BuildRegions[Util.BUILD_NAME.PALDemoPlayStationExperience])
                {
                    UpdateUI(grpFOV, false);
                    UpdateUI(btnFOVReset, false);
                    UpdateUI(chkFOVFreeze, false);
                    UpdateUI(chkToggleNoclip, false);
                }
                else if (build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCMay19])
                {
                    UpdateUI(grpFOV, false);
                    UpdateUI(btnFOVReset, false);
                    UpdateUI(chkFOVFreeze, false);
                    UpdateUI(chkToggleNoclip, false);
                }
                else if (build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCMay21])
                {
                    UpdateUI(grpFOV, false);
                    UpdateUI(btnFOVReset, false);
                    UpdateUI(chkFOVFreeze, false);
                    UpdateUI(chkToggleNoclip, false);
                }
            }
            else if (build.Title == "Sly 2")
            {
                ShowTab("Strings");
                ShowTab("DAG");
                ShowTab("Entities");
                UpdateUI(lblHealth, "Health");
                UpdateUI(lblLuckyCharms, false);
                UpdateUI(cmbLuckyCharms, false);
                UpdateUI(chkLuckyCharmsFreeze, false);
                UpdateUI(btnLoadMapFull, false);

                if (build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCE3Demo])
                {
                    UpdateUI(chkToggleInvulnerable, false);
                    UpdateUI(grpGadgets, false);
                }
                else if (build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCOfficialPlayStationMagazineDemoDisc089])
                {
                    UpdateUI(chkToggleInvulnerable, false);
                }
                else if (build.Region == Util.BuildRegions[Util.BUILD_NAME.PALDemoJuly27])
                {
                    UpdateUI(chkToggleInvulnerable, false);
                }
                else if (build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCDemoRatchetClankUpYourArsenal])
                {
                    UpdateUI(chkToggleInvulnerable, false);
                }
                else if (build.Region == Util.BuildRegions[Util.BUILD_NAME.PALDemoRatchetClank3])
                {
                    UpdateUI(chkToggleInvulnerable, false);
                }
                else if (build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCDemoRatchetClankUpYourArsenalAugust11])
                {
                    UpdateUI(chkToggleInvulnerable, false);
                }
                else if (build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCMarch17])
                {
                    UpdateUI(chkToggleInvulnerable, false);
                    UpdateUI(grpGadgets, false);
                    UpdateUI(chkGadgetInfinitePower, false);
                    UpdateUI(btnGadgetToggleAll, false);
                }
                else if (build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCJuly11])
                {
                    UpdateUI(chkToggleInvulnerable, false);
                }
            }
            else if (build.Title == "Sly 3")
            {
                ShowTab("Strings");
                ShowTab("DAG");
                ShowTab("Entities");
                UpdateUI(lblHealth, "Health");
                UpdateUI(lblLuckyCharms, false);
                UpdateUI(cmbLuckyCharms, false);
                UpdateUI(chkLuckyCharmsFreeze, false);
            }
        }

        private void SetTxtAddresses()
        {
            List<string> result = new();
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

                    result.Add($"{field.Name} = {value}");
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

                            result.Add($"DAG.{dagField.Name} = {value}");
                        }
                    }
                }
                else if (field.FieldType == typeof(Sly2_3_Savefile))
                {
                    Sly2_3_Savefile savefile = (Sly2_3_Savefile)field.GetValue(_game);
                    var savefileFields = savefile.GetType()
                                                 .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (var savefileField in savefileFields)
                    {
                        if (savefileField.FieldType == typeof(string))
                        {
                            string value = (string)savefileField.GetValue(savefile);
                            if (value == null || value == "")
                            {
                                continue;
                            }

                            result.Add($"Savefile.{savefileField.Name} = {value}");
                        }
                    }
                }
            }

            result.Sort();
            var tmp = string.Join(Environment.NewLine, result.OrderBy(x => x.StartsWith("_")));
            UpdateUI(txtAddresses, tmp);
        }

        private void FillcmbProcesses()
        {
            List<Memory.Proc> procs = new();
            Process[] processes = Process.GetProcesses();
            for (int i = 0; i < processes.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(processes[i].MainWindowTitle))
                {
                    continue;
                }

                if (processes[i].ProcessName.StartsWith("pcsx2")
                 || processes[i].ProcessName.StartsWith("rpcs3"))
                {
                    Memory.Proc proc = new()
                    {
                        Process = processes[i],
                    };

                    procs.Add(proc);
                }
            }

            Memory.Proc prevSelected;
            UpdateUI(() =>
            {
                if (cmbProcesses.DroppedDown)
                {
                    return;
                }

                prevSelected = cmbProcesses.SelectedItem as Memory.Proc;
                cmbProcesses.DataSource = procs;

                if (prevSelected == null)
                {
                    return;
                }

                var x = procs.FirstOrDefault(x => x.Process.Id == prevSelected.Process.Id);
                if (x != null)
                {
                    cmbProcesses.SelectedItem = x;
                }
            });
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
                if (value is System.Collections.IList list /*&& list.Count > 0*/)
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
            else if (sender is GroupBox groupBox)
            {
                if (value is bool)
                {
                    UpdateUI(groupBox, (bool)value, "Visible");
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

        private void SetControlsToDefault(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                control.Visible = true;
                control.Enabled = true;
                if (control is CheckBox checkBox)
                {
                    checkBox.Checked = false;
                }

                if (control.HasChildren)
                {
                    SetControlsToDefault(control.Controls);
                }
            }
        }

        #region Gadgets
        private void btnGadgetToggleAll_Click(object sender, EventArgs e)
        {
            _game.ToggleAllGadgets();
        }

        private void chkGadgetInfinitePower_CheckedChanged(object sender, EventArgs e)
        {
            if (chkGadgetInfinitePower.Checked)
            {
                _game.FreezeActCharGadgetPower(100);
            }
            else
            {
                _game.UnfreezeActCharGadgetPower();
            }
        }

        private void btnGadgetManage_Click(object sender, EventArgs e)
        {
            using FormGadgets f2 = new(_game);
            f2.Icon = this.Icon;
            f2.ShowDialog();
        }

        private void cmbGadgetL1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _game.WriteActCharGadgetId(Util.GADGET_BIND.L1, (cmbGadgetL1.SelectedItem as Util.Gadget_t).Id);
        }

        private void cmbGadgetL2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _game.WriteActCharGadgetId(Util.GADGET_BIND.L2, (cmbGadgetL2.SelectedItem as Util.Gadget_t).Id);
        }

        private void cmbGadgetR2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _game.WriteActCharGadgetId(Util.GADGET_BIND.R2, (cmbGadgetR2.SelectedItem as Util.Gadget_t).Id);
        }

        // When the user selects the item through autocompletion, the SelectionChangeCommitted event is not fired
        // We can detect this and run it manually
        private void cmbGadgetL1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbGadgetL1.ContainsFocus)
            {
                cmbGadgetL1_SelectionChangeCommitted(sender, e);
            }
        }

        private void cmbGadgetL2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbGadgetL2.ContainsFocus)
            {
                cmbGadgetL2_SelectionChangeCommitted(sender, e);
            }
        }

        private void cmbGadgetR2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbGadgetR2.ContainsFocus)
            {
                cmbGadgetR2_SelectionChangeCommitted(sender, e);
            }
        }
        #endregion

        #region Coins
        private void btnCoinsSet_Click(object sender, EventArgs e)
        {
            int.TryParse(txtCoins.Text, CultureInfo.InvariantCulture, out int value);
            _game.SetCoins(value);
        }

        #endregion

        #region Entities
        private void ClearEntitiesTab()
        {
            UpdateUI(lblEntitiesInfo, Util.DefaultValueString);
            UpdateUI(lblEntitiesXCoord, Util.DefaultValueFloat);
            UpdateUI(lblEntitiesYCoord, Util.DefaultValueFloat);
            UpdateUI(lblEntitiesZCoord, Util.DefaultValueFloat);
            UpdateUI(lblEntitiesZCoord, Util.DefaultValueFloat);
            UpdateUI(lblEntitiesXCoordWorld, Util.DefaultValueFloat);
            UpdateUI(lblEntitiesYCoordWorld, Util.DefaultValueFloat);
            UpdateUI(lblEntitiesZCoordWorld, Util.DefaultValueFloat);
            UpdateUI(chkEntitiesEditRotation, false, "Checked");
            chkEntitiesEditRotation_CheckedChanged(chkEntitiesEditRotation, EventArgs.Empty);
        }

        private void FillEntitiesTreeView(List<Util.FKXEntry_t> fkxList, string filter = "")
        {
            if (filter != "")
            {
                // We filter based on the name
                // Or the entity's address
                List<Util.FKXEntry_t> filtered = new();
                for (int i = 0; i < fkxList.Count; i++)
                {
                    var item = fkxList[i];
                    if (item.Name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                    {
                        filtered.Add(item);
                    }
                    else if (item.Count > 0)
                    {
                        // Add the node if ANY of the entity addresses match
                        string addressFilter = filter.TrimStart('0');
                        for (int j = 0; j < item.Count; j++)
                        {
                            if (item.EntityAddress[j].ToString("X").StartsWith(addressFilter, StringComparison.OrdinalIgnoreCase))
                            {
                                filtered.Add(item);
                                break;
                            }
                        }
                    }
                }

                fkxList = filtered;
            }

            trvEntitiesList.BeginUpdate();
            trvEntitiesList.Nodes.Clear();
            for (int i = 0; i < fkxList.Count; i++)
            {
                TreeNode node = new($"{fkxList[i].Name} ({fkxList[i].Count})");
                node.Tag = fkxList[i];
                node.Name = fkxList[i].Name;
                trvEntitiesList.Nodes.Add(node);

                if (fkxList[i].PoolPointer == 0x0)
                {
                    continue;
                }

                for (int j = 0; j < fkxList[i].Count; j++)
                {
                    string entityPointer = fkxList[i].EntityAddress[j].ToString("X");
                    TreeNode childNode = new(entityPointer);
                    childNode.Tag = entityPointer;
                    node.Nodes.Add(childNode);
                }
            }

            trvEntitiesList.EndUpdate();
        }

        public void btnEntitiesRefreshList_Click(object sender, EventArgs e)
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

            trvEntitiesList.Tag = fkxList;
            FillEntitiesTreeView(fkxList);
            ClearEntitiesTab();
            txtEntitiesSearch.Text = "";
            txtEntitiesSearch.PlaceholderText = $"Search through {fkxList.Count} entities";

            // Automatically select active character
            bool selectActiveCharacter = Properties.Settings.Default.EntitiesSelectActChar;
            if (selectActiveCharacter && _game.ActiveCharacter != null)
            {
                var actChar = trvEntitiesList.Nodes[_game.ActiveCharacter.InternalName];
                if (actChar != null)
                {
                    trvEntitiesList.SelectedNode = actChar.FirstNode;

                    // Bring the active character node to the center if possible
                    // (e.g. not the case for sly 2 ep1 hub for bentley as he appears too early in the list)
                    int visibleCount = trvEntitiesList.Height / trvEntitiesList.ItemHeight;
                    int half = visibleCount / 2;
                    TreeNode topNode = trvEntitiesList.SelectedNode;
                    for (int i = 0; i < half; i++)
                    {
                        if (topNode.PrevVisibleNode == null)
                        {
                            break;
                        }

                        topNode = topNode.PrevVisibleNode;
                    }

                    trvEntitiesList.TopNode = topNode;
                }
            }

            txtEntitiesSearch.Focus();
        }

        private void txtEntitiesSearch_TextChanged(object sender, EventArgs e)
        {
            List<Util.FKXEntry_t> fkxList = trvEntitiesList.Tag as List<Util.FKXEntry_t>;
            if (fkxList is null)
            {
                return;
            }

            FillEntitiesTreeView(fkxList, txtEntitiesSearch.Text);
            ClearEntitiesTab();
        }

        private void trvEntitiesList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is string)
            {
                // npc_boar_guard[0] = EF45B0
                UpdateUI(lblEntitiesInfo, $"{(e.Node.Parent.Tag as Util.FKXEntry_t).Name}[{e.Node.Index}] = {e.Node.Tag}");
                return;
            }

            if (e.Node.Tag is Util.FKXEntry_t)
            {
                ClearEntitiesTab();

                // npc_boar_guard = 48D110
                UpdateUI(lblEntitiesInfo, $"{(e.Node.Tag as Util.FKXEntry_t).Name} = {(e.Node.Tag as Util.FKXEntry_t).Address}");
            }
        }

        public int GetPointerToEntityFromSelectedEntitiesNode()
        {
            var node = trvEntitiesList.SelectedNode;
            if (node == null || node.Tag is not string)
            {
                return 0;
            }

            var fkx = node.Parent.Tag as Util.FKXEntry_t;
            int pointerToEntity = fkx.PoolPointer + node.Index * 4;
            return pointerToEntity;
        }

        private void btnEntitiesCopyAddress_Click(object sender, EventArgs e)
        {
            if (trvEntitiesList.SelectedNode == null)
            {
                return;
            }

            if (trvEntitiesList.SelectedNode.Tag is string)
            {
                Clipboard.SetText(trvEntitiesList.SelectedNode.Tag.ToString());
            }
            else if (trvEntitiesList.SelectedNode.Tag is Util.FKXEntry_t)
            {
                Clipboard.SetText((trvEntitiesList.SelectedNode.Tag as Util.FKXEntry_t).Address);
            }
        }

        private Vector3 GetTranslationFromSelectedEntitiesNode(out string pointerToEntity)
        {
            int value = GetPointerToEntityFromSelectedEntitiesNode();
            pointerToEntity = value.ToString("X");
            if (value == 0)
            {
                return Vector3.Zero;
            }

            Vector3 trans = _game.ReadEntityLocalTranslation(pointerToEntity);
            return trans;
        }

        private void btnEntitiesXCoordMinus_Click(object sender, EventArgs e)
        {
            Vector3 trans = GetTranslationFromSelectedEntitiesNode(out string pointerToEntity);
            trans.X -= Util.AmountToIncreaseOrDecreaseTranslationForFKXEntity;
            _game.WriteEntityLocalTranslation(pointerToEntity, trans);
        }

        private void btnEntitiesXCoordPlus_Click(object sender, EventArgs e)
        {
            Vector3 trans = GetTranslationFromSelectedEntitiesNode(out string pointerToEntity);
            trans.X += Util.AmountToIncreaseOrDecreaseTranslationForFKXEntity;
            _game.WriteEntityLocalTranslation(pointerToEntity, trans);
        }

        private void chkEntitiesXCoordFreeze_CheckedChanged(object sender, EventArgs e)
        {
            int pointerToEntity = GetPointerToEntityFromSelectedEntitiesNode();
            if (pointerToEntity == 0)
            {
                return;
            }

            if (chkEntitiesXCoordFreeze.Checked)
            {
                _game.FreezeEntityLocalTranslationX(pointerToEntity.ToString("X"));
            }
            else
            {
                _game.UnfreezeEntityLocalTranslationX(pointerToEntity.ToString("X"));
            }
        }

        private void btnEntitiesXCoordSet_Click(object sender, EventArgs e)
        {
            float.TryParse(txtEntitiesXCoordSet.Text, CultureInfo.InvariantCulture, out float value);
            Vector3 trans = GetTranslationFromSelectedEntitiesNode(out string pointerToEntity);
            trans.X = value;
            _game.WriteEntityLocalTranslation(pointerToEntity, trans);
        }

        private void btnEntitiesYCoordMinus_Click(object sender, EventArgs e)
        {
            Vector3 trans = GetTranslationFromSelectedEntitiesNode(out string pointerToEntity);
            trans.Y -= Util.AmountToIncreaseOrDecreaseTranslationForFKXEntity;
            _game.WriteEntityLocalTranslation(pointerToEntity, trans);
        }

        private void btnEntitiesYCoordPlus_Click(object sender, EventArgs e)
        {
            Vector3 trans = GetTranslationFromSelectedEntitiesNode(out string pointerToEntity);
            trans.Y += Util.AmountToIncreaseOrDecreaseTranslationForFKXEntity;
            _game.WriteEntityLocalTranslation(pointerToEntity, trans);
        }

        private void chkEntitiesYCoordFreeze_CheckedChanged(object sender, EventArgs e)
        {
            int pointerToEntity = GetPointerToEntityFromSelectedEntitiesNode();
            if (pointerToEntity == 0)
            {
                return;
            }

            if (chkEntitiesYCoordFreeze.Checked)
            {
                _game.FreezeEntityLocalTranslationY(pointerToEntity.ToString("X"));
            }
            else
            {
                _game.UnfreezeEntityLocalTranslationY(pointerToEntity.ToString("X"));
            }
        }

        private void btnEntitiesYCoordSet_Click(object sender, EventArgs e)
        {
            float.TryParse(txtEntitiesYCoordSet.Text, CultureInfo.InvariantCulture, out float value);
            Vector3 trans = GetTranslationFromSelectedEntitiesNode(out string pointerToEntity);
            trans.Y = value;
            _game.WriteEntityLocalTranslation(pointerToEntity, trans);
        }

        private void btnEntitiesZCoordMinus_Click(object sender, EventArgs e)
        {
            Vector3 trans = GetTranslationFromSelectedEntitiesNode(out string pointerToEntity);
            trans.Z -= Util.AmountToIncreaseOrDecreaseTranslationForFKXEntity;
            _game.WriteEntityLocalTranslation(pointerToEntity, trans);
        }

        private void btnEntitiesZCoordPlus_Click(object sender, EventArgs e)
        {
            Vector3 trans = GetTranslationFromSelectedEntitiesNode(out string pointerToEntity);
            trans.Z += Util.AmountToIncreaseOrDecreaseTranslationForFKXEntity;
            _game.WriteEntityLocalTranslation(pointerToEntity, trans);
        }

        private void chkEntitiesZCoordFreeze_CheckedChanged(object sender, EventArgs e)
        {
            int pointerToEntity = GetPointerToEntityFromSelectedEntitiesNode();
            if (pointerToEntity == 0)
            {
                return;
            }

            if (chkEntitiesZCoordFreeze.Checked)
            {
                _game.FreezeEntityLocalTranslationZ(pointerToEntity.ToString("X"));
            }
            else
            {
                _game.UnfreezeEntityLocalTranslationZ(pointerToEntity.ToString("X"));
            }
        }

        private void btnEntitiesZCoordSet_Click(object sender, EventArgs e)
        {
            float.TryParse(txtEntitiesZCoordSet.Text, CultureInfo.InvariantCulture, out float value);
            Vector3 trans = GetTranslationFromSelectedEntitiesNode(out string pointerToEntity);
            trans.Z = value;
            _game.WriteEntityLocalTranslation(pointerToEntity, trans);
        }

        private void trkEntitiesCoord_Scroll(object sender, EventArgs e)
        {
            if (trkEntitiesCoord.Value == 0)
            {
                Util.AmountToIncreaseOrDecreaseTranslationForFKXEntity = 10;
            }
            else
            {
                Util.AmountToIncreaseOrDecreaseTranslationForFKXEntity = trkEntitiesCoord.Value * 50;
            }
        }

        private void trkEntitiesScale_Scroll(object sender, EventArgs e)
        {
            float trkValue = (float)trkEntitiesScale.Value / 10;
            int pointerToEntity = GetPointerToEntityFromSelectedEntitiesNode();
            _game.WriteEntityLocalScale(pointerToEntity.ToString("X"), trkValue);
        }

        private void btnEntitiesScaleReset_Click(object sender, EventArgs e)
        {
            trkEntitiesScale.Value = 10;
            trkEntitiesScale_Scroll(trkEntitiesScale, EventArgs.Empty);
        }

        private void chkEntitiesEditRotation_CheckedChanged(object sender, EventArgs e)
        {
            trkEntitiesRotationX.Enabled = chkEntitiesEditRotation.Checked;
            trkEntitiesRotationY.Enabled = chkEntitiesEditRotation.Checked;
            trkEntitiesRotationZ.Enabled = chkEntitiesEditRotation.Checked;
        }

        private void trkEntitiesRotationX_Scroll(object sender, EventArgs e)
        {
            WriteRotationToSelectedEntitiesNode();
        }

        private void trkEntitiesRotationY_Scroll(object sender, EventArgs e)
        {
            WriteRotationToSelectedEntitiesNode();
        }

        private void trkEntitiesRotationZ_Scroll(object sender, EventArgs e)
        {
            WriteRotationToSelectedEntitiesNode();
        }

        private void WriteRotationToSelectedEntitiesNode()
        {
            string pointerToEntity = GetPointerToEntityFromSelectedEntitiesNode().ToString("X");

            // Degrees to radians
            float radX = MathF.PI / 180f * trkEntitiesRotationX.Value;
            float radY = MathF.PI / 180f * trkEntitiesRotationY.Value;
            float radZ = MathF.PI / 180f * trkEntitiesRotationZ.Value;

            Matrix4x4 rotationMatrix = Matrix4x4.CreateRotationX(radX) * Matrix4x4.CreateRotationY(radY) * Matrix4x4.CreateRotationZ(radZ);
            rotationMatrix.Translation = _game.ReadEntityWorldTransformation(pointerToEntity).Translation;
            _game.WriteEntityWorldTransformation(pointerToEntity, rotationMatrix);
        }

        private void btnEntitiesWarpActCharToEntity_Click(object sender, EventArgs e)
        {
            // Act char to entity
            int pointerToEntity = GetPointerToEntityFromSelectedEntitiesNode();
            if (pointerToEntity == 0)
            {
                return;
            }

            _game.WarpSourceEntityToDestEntity("", pointerToEntity.ToString("X"));
            _game.ResetCamera();
        }

        private void btnEntitiesWarpEntityToActChar_Click(object sender, EventArgs e)
        {
            // Entity to act char
            int pointerToEntity = GetPointerToEntityFromSelectedEntitiesNode();
            if (pointerToEntity == 0)
            {
                return;
            }

            _game.WarpSourceEntityToDestEntity(pointerToEntity.ToString("X"), "");
        }

        private void btnEntitiesWarp_Click(object sender, EventArgs e)
        {
            int pointerToEntity = GetPointerToEntityFromSelectedEntitiesNode();
            if (pointerToEntity == 0)
            {
                return;
            }

            Util.Warp_t warp = (Util.Warp_t)cmbEntitiesWarps.SelectedItem;
            if (warp == null)
            {
                return;
            }

            _game.WarpSourceEntityToPoint(pointerToEntity.ToString("X"), warp.Transformation);
        }
        #endregion

        #region Active character
        private void btnActCharHealthMinus_Click(object sender, EventArgs e)
        {
            int health = _game.ReadActCharHealth();
            health -= Util.AmountToIncreaseOrDecreaseHealth;
            _game.WriteActCharHealth(health);
        }

        private void btnActCharHealthPlus_Click(object sender, EventArgs e)
        {
            int health = _game.ReadActCharHealth();
            health += Util.AmountToIncreaseOrDecreaseHealth;
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

        private void btnActCharHealthSet_Click(object sender, EventArgs e)
        {
            int.TryParse(txtActCharHealthSet.Text, CultureInfo.InvariantCulture, out int value);
            _game.WriteActCharHealth(value);
        }

        private void chkLuckyCharmsFreeze_CheckedChanged(object sender, EventArgs e)
        {
            if (_game is not Sly1Handler)
            {
                return;
            }

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

        private void btnActCharXCoordMinus_Click(object sender, EventArgs e)
        {
            Vector3 value = _game.ReadActCharLocalTranslation();
            value.X -= Util.AmountToIncreaseOrDecreaseTranslationForActChar;
            _game.WriteActCharLocalTranslation(value);
        }

        private void btnActCharXCoordPlus_Click(object sender, EventArgs e)
        {
            Vector3 value = _game.ReadActCharLocalTranslation();
            value.X += Util.AmountToIncreaseOrDecreaseTranslationForActChar;
            _game.WriteActCharLocalTranslation(value);
        }

        private void chkActCharXCoordFreeze_CheckedChanged(object sender, EventArgs e)
        {
            if (chkActCharXCoordFreeze.Checked)
            {
                _game.FreezeActCharLocalTranslationX();
            }
            else
            {
                _game.UnfreezeActCharLocalTranslationX();
            }
        }

        private void btnActCharYCoordMinus_Click(object sender, EventArgs e)
        {
            Vector3 value = _game.ReadActCharLocalTranslation();
            value.Y -= Util.AmountToIncreaseOrDecreaseTranslationForActChar;
            _game.WriteActCharLocalTranslation(value);
        }

        private void btnActCharYCoordPlus_Click(object sender, EventArgs e)
        {
            Vector3 value = _game.ReadActCharLocalTranslation();
            value.Y += Util.AmountToIncreaseOrDecreaseTranslationForActChar;
            _game.WriteActCharLocalTranslation(value);
        }

        private void chkActCharYCoordFreeze_CheckedChanged(object sender, EventArgs e)
        {
            if (chkActCharYCoordFreeze.Checked)
            {
                _game.FreezeActCharLocalTranslationY();
            }
            else
            {
                _game.UnfreezeActCharLocalTranslationY();
            }
        }

        private void btnActCharZCoordMinus_Click(object sender, EventArgs e)
        {
            Vector3 value = _game.ReadActCharLocalTranslation();
            value.Z -= Util.AmountToIncreaseOrDecreaseTranslationForActChar;
            _game.WriteActCharLocalTranslation(value);
        }

        private void btnActCharZCoordPlus_Click(object sender, EventArgs e)
        {
            Vector3 value = _game.ReadActCharLocalTranslation();
            value.Z += Util.AmountToIncreaseOrDecreaseTranslationForActChar;
            _game.WriteActCharLocalTranslation(value);
        }

        private void chkActCharZCoordFreeze_CheckedChanged(object sender, EventArgs e)
        {
            if (chkActCharZCoordFreeze.Checked)
            {
                _game.FreezeActCharLocalTranslationZ();
            }
            else
            {
                _game.UnfreezeActCharLocalTranslationZ();
            }
        }

        private void btnActCharXCoordSet_Click(object sender, EventArgs e)
        {
            float.TryParse(txtActCharXCoordSet.Text, CultureInfo.InvariantCulture, out float value);
            Vector3 trans = _game.ReadActCharLocalTranslation();
            trans.X = value;
            _game.WriteActCharLocalTranslation(trans);
        }

        private void btnActCharYCoordSet_Click(object sender, EventArgs e)
        {
            float.TryParse(txtActCharYCoordSet.Text, CultureInfo.InvariantCulture, out float value);
            Vector3 trans = _game.ReadActCharLocalTranslation();
            trans.Y = value;
            _game.WriteActCharLocalTranslation(trans);
        }

        private void btnActCharZCoordSet_Click(object sender, EventArgs e)
        {
            float.TryParse(txtActCharZCoordSet.Text, CultureInfo.InvariantCulture, out float value);
            Vector3 trans = _game.ReadActCharLocalTranslation();
            trans.Z = value;
            _game.WriteActCharLocalTranslation(trans);
        }

        private void trkActCharCoord_Scroll(object sender, EventArgs e)
        {
            if (trkActCharCoord.Value == 0)
            {
                Util.AmountToIncreaseOrDecreaseTranslationForActChar = 10;
            }
            else
            {
                Util.AmountToIncreaseOrDecreaseTranslationForActChar = trkActCharCoord.Value * 50;
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

        private void ToolStripMenuItemActCharCoordsCopyXYZToTextboxes_Click(object sender, EventArgs e)
        {
            txtActCharXCoordSet.Text = lblXCoord.Text;
            txtActCharYCoordSet.Text = lblYCoord.Text;
            txtActCharZCoordSet.Text = lblZCoord.Text;
        }

        private void ToolStripMenuItemActCharCoordsCopyXYZToClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetText($"{txtActCharXCoordSet.Text} {txtActCharYCoordSet.Text} {txtActCharZCoordSet.Text}");
        }

        private void ToolStripMenuItemActCharCoordsPasteXYZFromClipboard_Click(object sender, EventArgs e)
        {
            string clipboard = Clipboard.GetText();
            string[] coords = clipboard.Split(' ');
            if (coords.Length != 3)
            {
                return;
            }

            // Sometimes when you copy the 3 floats from cheat engine's memory viewer, it has commas instead of periods
            for (int i = 0; i < 3; i++)
            {
                string input = coords[i].Trim();
                if (input.Contains(',') && !input.Contains('.'))
                {
                    coords[i] = input.Replace(',', '.');
                }
            }

            float.TryParse(coords[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float value1);
            float.TryParse(coords[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float value2);
            float.TryParse(coords[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float value3);
            txtActCharXCoordSet.Text = value1.ToString();
            txtActCharYCoordSet.Text = value2.ToString();
            txtActCharZCoordSet.Text = value3.ToString();
        }

        private void ToolStripMenuItemActCharCoordsSetXYZ_Click(object sender, EventArgs e)
        {
            float.TryParse(txtActCharXCoordSet.Text, CultureInfo.InvariantCulture, out float value1);
            float.TryParse(txtActCharYCoordSet.Text, CultureInfo.InvariantCulture, out float value2);
            float.TryParse(txtActCharZCoordSet.Text, CultureInfo.InvariantCulture, out float value3);
            Vector3 trans = new(value1, value2, value3);
            _game.WriteActCharLocalTranslation(trans);
        }

        private void ToolStripMenuItemActCharAddCustomWarp_Click(object sender, EventArgs e)
        {
            using FormCustomWarps f2 = new(_game);
            f2.Icon = this.Icon;
            f2.ShowDialog();
        }

        private void chkActCharFly_CheckedChanged(object sender, EventArgs e)
        {
            if (chkActCharFly.Checked)
            {
                _game.FreezeActCharLocalTranslationZ();
            }
            else
            {
                if (!chkActCharZCoordFreeze.Checked)
                {
                    _game.UnfreezeActCharLocalTranslationZ();
                }

                _game.UnfreezeActCharVelocityZ();
            }
        }

        private void btnWarp_Click(object sender, EventArgs e)
        {
            Util.Warp_t warp = (Util.Warp_t)cmbWarps.SelectedItem;
            if (warp == null)
            {
                return;
            }

            _game.WarpSourceEntityToPoint("", warp.Transformation);
            _game.ResetCamera();
        }
        #endregion

        #region Toggles
        private void chkToggleUndetectable_CheckedChanged(object sender, EventArgs e)
        {
            _game.ToggleUndetectable(chkToggleUndetectable.Checked);
        }

        private void chkToggleInvulnerable_CheckedChanged(object sender, EventArgs e)
        {
            _game.ToggleInvulnerable(chkToggleInvulnerable.Checked);
        }

        private void chkToggleInfDbJump_CheckedChanged(object sender, EventArgs e)
        {
            _game.ToggleInfiniteDbJump(chkToggleInfDbJump.Checked);
        }

        private void chkToggleNoclip_CheckedChanged(object sender, EventArgs e)
        {
            _game.ToggleNoclip(chkToggleNoclip.Checked);
        }

        private void chkDisableGuardAI_CheckedChanged(object sender, EventArgs e)
        {
            _game.ToggleGuardAI(chkDisableGuardAI.Checked);
        }
        #endregion

        #region Camera
        private void btnResetCamera_Click(object sender, EventArgs e)
        {
            _game.ResetCamera();
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
        #endregion

        #region Clock
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
        #endregion

        #region Maps
        private void btnLoadMap_Click(object sender, EventArgs e)
        {
            var mapId = Util.GetOriginalMapId(cmbMaps);

            // Current map
            if (mapId == -1)
            {
                mapId = _game.ReadMapId();
            }

            if (_game is Sly1Handler)
            {
                int entranceValue = 0x1BC;
                if (_game.Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCDemo])
                {
                    entranceValue = 0x196;
                }
                else if (_game.Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCDemoJune14])
                {
                    entranceValue = 0x1B3;
                }
                else if (_game.Build.Region == Util.BuildRegions[Util.BUILD_NAME.PALDemoPlayStationExperience])
                {
                    entranceValue = 0x196;
                }
                else if (_game.Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCMay19])
                {
                    entranceValue = 0x19D;
                }
                else if (_game.Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCMay21])
                {
                    entranceValue = 0x19D;
                }

                _game.LoadMap(mapId, entranceValue);
            }
            else if (_game is Sly2Handler)
            {
                int entranceValue = 0x189;
                if (_game.Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCJuly11])
                {
                    entranceValue = 0x193;
                }
                else if (_game.Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCE3Demo])
                {
                    entranceValue = 0x17A;
                }
                else if (_game.Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCMarch17])
                {
                    entranceValue = 0x171;
                }
                else if (_game.Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCDemoRatchetClankUpYourArsenalAugust11])
                {
                    entranceValue = 0x194;
                }

                _game.LoadMap(mapId, entranceValue);
            }
            else if (_game is Sly3Handler)
            {
                int entranceValue = 0x1A8;
                if (_game.Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCJuly16])
                {
                    entranceValue = 0x1A2;
                }
                else if (_game.Build.Region == Util.BuildRegions[Util.BUILD_NAME.PALAugust2])
                {
                    entranceValue = 0x1A6;
                }
                else if (_game.Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCDemoApril18])
                {
                    entranceValue = 0x19B;
                }
                else if (_game.Build.Region == Util.BuildRegions[Util.BUILD_NAME.NTSCDemoJuly7])
                {
                    entranceValue = 0x1A2;
                }

                _game.LoadMap(mapId, entranceValue, 0);
            }
        }

        private void btnLoadMapFull_Click(object sender, EventArgs e)
        {
            var mapId = Util.GetOriginalMapId(cmbMaps);

            // Current map
            if (mapId == -1)
            {
                mapId = _game.ReadMapId();
            }

            (_game as Sly3Handler).LoadMapFull(mapId);
        }
        #endregion

        private void btnSkipCurrentDialogue_Click(object sender, EventArgs e)
        {
            _game.SkipCurrentDialogue();
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

        private void chkStringsMonospaceFont_CheckedChanged(object sender, EventArgs e)
        {
            if (chkStringsMonospaceFont.Checked)
            {
                txtStringsLocalized.Font = _monospaceFont;
                txtStringsSavefile.Font = _monospaceFont;
            }
            else
            {
                txtStringsLocalized.Font = txtStringsLocalized.Parent.Font;
                txtStringsSavefile.Font = txtStringsSavefile.Parent.Font;
            }
        }

        private void btnReattach_Click(object sender, EventArgs e)
        {
            _triggerReattach = true;
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            using FormSettings f2 = new();
            f2.Icon = this.Icon;
            if (f2.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            // Don't refresh warps if no game is attached
            if (_game is not null)
            {
                int currentMapId = _game.ReadMapId() + 1;
                _game.RefreshWarps(currentMapId);
            }
        }

        private void cmbProcesses_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (_m.mProc == null)
            {
                return;
            }

            var proc = (cmbProcesses.SelectedItem as Memory.Proc);
            if (proc.Process.Id == _m.mProc.Process.Id)
            {
                // If the process selected is the current one, don't reattach
                return;
            }

            _triggerReattach = true;
        }
    }
}
