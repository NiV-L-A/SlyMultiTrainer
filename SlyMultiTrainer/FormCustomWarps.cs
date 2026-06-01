using System.Data;
using System.Numerics;
using static SlyMultiTrainer.Util;

namespace SlyMultiTrainer
{
    public partial class FormCustomWarps : Form
    {
        GameBase_t _game;
        Font _defaultNodeFont;
        Font _strikeoutNodeFont;

        public FormCustomWarps(GameBase_t game)
        {
            InitializeComponent();
            _game = game;
            _defaultNodeFont = new Font(trvWarps.Font, FontStyle.Regular);
            _strikeoutNodeFont = new Font(trvWarps.Font, FontStyle.Strikeout);
            this.Text = $"Sly Multi Trainer - Manage {_game.Build.Title} custom warps";
        }

        private void FormCustomWarps_Load(object sender, EventArgs e)
        {
            if (!GetRootFromCustomWarpsFile(out CustomWarpsFileRoot? _root))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
                return;
            }

            foreach (var game in _root.Data)
            {
                // Only consider the warps for the detected game
                if (game.Key != _game.Build.Title
                 || game.Value == null)
                {
                    continue;
                }

                TreeNode gameNode = new(game.Key);
                gameNode.Tag = game;
                foreach (var map in game.Value)
                {
                    TreeNode mapNode = new(map.Key);
                    mapNode.Tag = map;
                    foreach (var warp in map.Value)
                    {
                        TreeNode warpNode = new(warp.Name);
                        warpNode.Tag = warp;
                        if (warp.IsInvisible != null
                         && warp.IsInvisible == true)
                        {
                            warpNode.NodeFont = _strikeoutNodeFont;
                        }

                        mapNode.Nodes.Add(warpNode);
                    }

                    trvWarps.Nodes.Add(mapNode);
                }
            }

            trvWarps.ExpandAll();
            lblBuilds.Text = $"Visible only for the following {_game.Build.Title} builds:";
            clbBuilds.Items.AddRange(Util.Builds.Where(x => x.Title == _game.Build.Title).Select(x => x.Region).ToArray());
        }

        private void trvWarps_AfterSelect(object sender, TreeViewEventArgs e)
        {
            for (int i = 0; i < clbBuilds.Items.Count; i++)
            {
                clbBuilds.SetItemChecked(i, false);
            }

            btnSelectedDelete.Enabled = true;
            if (e.Node.Tag is not CustomWarpsFileWarpPoint)
            {
                btnSelectedWarpMoveUp.Enabled = false;
                btnSelectedWarpMoveDown.Enabled = false;
                btnSelectedWarpSave.Enabled = false;
                txtSelectedWarpName.Text = "";
                txtSelectedWarpPositionX.Text = "";
                txtSelectedWarpPositionY.Text = "";
                txtSelectedWarpPositionZ.Text = "";
                txtSelectedWarpRotation11.Text = "";
                txtSelectedWarpRotation12.Text = "";
                txtSelectedWarpRotation13.Text = "";
                txtSelectedWarpRotation21.Text = "";
                txtSelectedWarpRotation22.Text = "";
                txtSelectedWarpRotation23.Text = "";
                txtSelectedWarpRotation31.Text = "";
                txtSelectedWarpRotation32.Text = "";
                txtSelectedWarpRotation33.Text = "";
                txtSelectedWarpName.Enabled = false;
                txtSelectedWarpPositionX.Enabled = false;
                txtSelectedWarpPositionY.Enabled = false;
                txtSelectedWarpPositionZ.Enabled = false;
                txtSelectedWarpRotation11.Enabled = false;
                txtSelectedWarpRotation12.Enabled = false;
                txtSelectedWarpRotation13.Enabled = false;
                txtSelectedWarpRotation21.Enabled = false;
                txtSelectedWarpRotation22.Enabled = false;
                txtSelectedWarpRotation23.Enabled = false;
                txtSelectedWarpRotation31.Enabled = false;
                txtSelectedWarpRotation32.Enabled = false;
                txtSelectedWarpRotation33.Enabled = false;
                clbBuilds.Enabled = false;
                btnFillWithCurrentTransformation.Enabled = false;
                btnWarpToSelected.Enabled = false;
                chkIsInvisible.Checked = false;
                chkIsInvisible.Enabled = false;
                return;
            }

            if (e.Node == e.Node.Parent.FirstNode)
            {
                btnSelectedWarpMoveUp.Enabled = false;
            }
            else
            {
                btnSelectedWarpMoveUp.Enabled = true;
            }

            if (e.Node == e.Node.Parent.LastNode)
            {
                btnSelectedWarpMoveDown.Enabled = false;
            }
            else
            {
                btnSelectedWarpMoveDown.Enabled = true;
            }

            btnSelectedWarpSave.Enabled = true;
            txtSelectedWarpName.Enabled = true;
            txtSelectedWarpPositionX.Enabled = true;
            txtSelectedWarpPositionY.Enabled = true;
            txtSelectedWarpPositionZ.Enabled = true;
            txtSelectedWarpRotation11.Enabled = true;
            txtSelectedWarpRotation12.Enabled = true;
            txtSelectedWarpRotation13.Enabled = true;
            txtSelectedWarpRotation21.Enabled = true;
            txtSelectedWarpRotation22.Enabled = true;
            txtSelectedWarpRotation23.Enabled = true;
            txtSelectedWarpRotation31.Enabled = true;
            txtSelectedWarpRotation32.Enabled = true;
            txtSelectedWarpRotation33.Enabled = true;
            clbBuilds.Enabled = true;
            btnFillWithCurrentTransformation.Enabled = true;
            btnWarpToSelected.Enabled = true;
            chkIsInvisible.Enabled = true;

            CustomWarpsFileWarpPoint warp = (CustomWarpsFileWarpPoint)e.Node.Tag;
            txtSelectedWarpName.Text = warp.Name;
            txtSelectedWarpPositionX.Text = warp.Position[0].ToString();
            txtSelectedWarpPositionY.Text = warp.Position[1].ToString();
            txtSelectedWarpPositionZ.Text = warp.Position[2].ToString();
            if (warp.Rotation != null)
            {
                txtSelectedWarpRotation11.Text = warp.Rotation[0].ToString();
                txtSelectedWarpRotation12.Text = warp.Rotation[1].ToString();
                txtSelectedWarpRotation13.Text = warp.Rotation[2].ToString();
                txtSelectedWarpRotation21.Text = warp.Rotation[3].ToString();
                txtSelectedWarpRotation22.Text = warp.Rotation[4].ToString();
                txtSelectedWarpRotation23.Text = warp.Rotation[5].ToString();
                txtSelectedWarpRotation31.Text = warp.Rotation[6].ToString();
                txtSelectedWarpRotation32.Text = warp.Rotation[7].ToString();
                txtSelectedWarpRotation33.Text = warp.Rotation[8].ToString();
            }
            else
            {
                txtSelectedWarpRotation11.Text = "1";
                txtSelectedWarpRotation12.Text = "0";
                txtSelectedWarpRotation13.Text = "0";
                txtSelectedWarpRotation21.Text = "0";
                txtSelectedWarpRotation22.Text = "1";
                txtSelectedWarpRotation23.Text = "0";
                txtSelectedWarpRotation31.Text = "0";
                txtSelectedWarpRotation32.Text = "0";
                txtSelectedWarpRotation33.Text = "1";
            }

            if (warp.IsInvisible != null
             && warp.IsInvisible == true)
            {
                chkIsInvisible.Checked = true;
            }
            else
            {
                chkIsInvisible.Checked = false;
            }

            if (warp.IncludeBuilds != null)
            {
                for (int i = 0; i < warp.IncludeBuilds.Count; i++)
                {
                    for (int j = 0; j < clbBuilds.Items.Count; j++)
                    {
                        var region = clbBuilds.Items[j] as string;
                        if (string.Equals(warp.IncludeBuilds[i], region, StringComparison.OrdinalIgnoreCase))
                        {
                            clbBuilds.SetItemChecked(j, true);
                            break;
                        }
                    }
                }
            }
        }

        private void SelectNode(TreeNode node)
        {
            trvWarps.SelectedNode = node;
            trvWarps.Focus();
        }

        private void btnSelectedWarpMoveUp_Click(object sender, EventArgs e)
        {
            TreeNode node = trvWarps.SelectedNode;
            if (node.Tag is not CustomWarpsFileWarpPoint)
            {
                return;
            }

            TreeNodeCollection nodes = node.Parent.Nodes;
            if (node != node.Parent.FirstNode)
            {
                nodes.RemoveAt(node.Index);
                nodes.Insert(node.Index - 1, node);
            }

            SelectNode(node);
        }

        private void btnSelectedWarpMoveDown_Click(object sender, EventArgs e)
        {
            TreeNode node = trvWarps.SelectedNode;
            if (node.Tag is not CustomWarpsFileWarpPoint)
            {
                return;
            }

            TreeNodeCollection nodes = node.Parent.Nodes;
            if (node != node.Parent.LastNode)
            {
                nodes.RemoveAt(node.Index);
                nodes.Insert(node.Index + 1, node);
            }

            SelectNode(node);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormCustomWarps_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Make sure we are not coming from "save and exit"
            if (this.DialogResult != DialogResult.OK)
            {
                var result = MessageBox.Show("Are you sure you want to discard the changes made?", "Discard changes", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            _strikeoutNodeFont?.Dispose();
        }

        private bool CallbackSaveSelectedWarp()
        {
            if (!GetTransformationFromTextboxes(out Matrix4x4 trans))
            {
                return false;
            }

            TreeNode node = trvWarps.SelectedNode;
            CustomWarpsFileWarpPoint warp = (CustomWarpsFileWarpPoint)node.Tag;
            warp.Name = txtSelectedWarpName.Text;
            warp.Rotation = [trans.M11, trans.M12, trans.M13,
                             trans.M21, trans.M22, trans.M23,
                             trans.M31, trans.M32, trans.M33];
            warp.Position = [trans.M41, trans.M42, trans.M43];
            if (chkIsInvisible.Checked)
            {
                warp.IsInvisible = true;
                node.NodeFont = _strikeoutNodeFont;
            }
            else
            {
                warp.IsInvisible = null;
                node.NodeFont = _defaultNodeFont;
            }

            var includedBuilds = clbBuilds.CheckedItems;
            if (includedBuilds.Count == 0)
            {
                warp.IncludeBuilds = null;
            }
            else
            {
                // Having all builds checked makes no sense, so uncheck all
                if (clbBuilds.CheckedItems.Count == clbBuilds.Items.Count)
                {
                    warp.IncludeBuilds = null;
                    for (int i = 0; i < clbBuilds.Items.Count; i++)
                    {
                        clbBuilds.SetItemChecked(i, false);
                    }
                }
                else
                {
                    warp.IncludeBuilds = includedBuilds.Cast<string>().ToList();
                }
            }

            node.Text = warp.Name;
            return true;
        }

        private void btnSelectedWarpSave_Click(object sender, EventArgs e)
        {
            CallbackSaveSelectedWarp();
        }

        private void btnSelectedDelete_Click(object sender, EventArgs e)
        {
            TreeNode node = trvWarps.SelectedNode;
            if (node is null)
            {
                return;
            }

            if (node.Tag is CustomWarpsFileWarpPoint)
            {
                DialogResult result = MessageBox.Show($"Delete the selected custom warp with name \"{node.Text}\"?", "Delete custom warp", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    trvWarps.Nodes.Remove(node);
                }
            }
            else
            {
                DialogResult result = MessageBox.Show($"Delete the selected map \"{node.Text}\" and all its custom warps?", "Delete map with custom warps", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    trvWarps.Nodes.Remove(node);
                }
            }

            if (trvWarps.Nodes.Count == 0)
            {
                btnSelectedDelete.Enabled = false;
            }
        }

        private void btnAddWarp_Click(object sender, EventArgs e)
        {
            // Make sure the map node is present
            int mapId = _game.ReadMapId() + 1;
            string currentMapName = _game.Maps[mapId].Name.TrimStart();
            bool mapFoundInList = false;
            for (int i = 0; i < trvWarps.Nodes.Count; i++)
            {
                if (string.Equals(currentMapName, trvWarps.Nodes[i].Text, StringComparison.OrdinalIgnoreCase))
                {
                    mapFoundInList = true;
                    break;
                }
            }

            if (!mapFoundInList)
            {
                TreeNode newMapNode = new(currentMapName);
                newMapNode.Tag = new KeyValuePair<string, List<Util.CustomWarpsFileWarpPoint>>(currentMapName, []);
                trvWarps.Nodes.Add(newMapNode);
                SelectNode(newMapNode);
            }

            string warpGameName = $"{_game.Build.Title}";
            Matrix4x4 warpTrans = GetActCharTransformationRounded();

            int topMargin = 20;
            Form inputForm = new()
            {
                Text = $"Add custom warp for {warpGameName} map \"{currentMapName}\"",
                Width = 400,
                Height = 320,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false,
                AcceptButton = null,
                CancelButton = null
            };

            Label lblText = new()
            {
                Left = 10,
                Top = 10,
                Text = "Name",
                AutoSize = true
            };

            TextBox txtInput = new()
            {
                Left = 10,
                Top = lblText.Top + topMargin,
                Width = 360,
                Text = ""
            };

            CheckBox chkOnlyForThisBuild = new()
            {
                Left = 10,
                Top = txtInput.Top + topMargin + 10,
                Text = $"Visible only for the current {_game.Build.Title} build:{Environment.NewLine}\"{_game.Build.Region}\"",
                AutoSize = true,
            };

            Label lblPreviewText = new()
            {
                Left = 10,
                Top = chkOnlyForThisBuild.Top + topMargin + 20,
                AutoSize = true
            };

            lblPreviewText.Text = $"Current map: {currentMapName}";
            lblPreviewText.Text += $"{Environment.NewLine}{Environment.NewLine}Current transformation:";
            lblPreviewText.Text += $"{Environment.NewLine}{warpTrans.M11}, {warpTrans.M12}, {warpTrans.M13}";
            lblPreviewText.Text += $"{Environment.NewLine}{warpTrans.M21}, {warpTrans.M22}, {warpTrans.M23}";
            lblPreviewText.Text += $"{Environment.NewLine}{warpTrans.M31}, {warpTrans.M32}, {warpTrans.M33}";
            lblPreviewText.Text += $"{Environment.NewLine}{warpTrans.M41}, {warpTrans.M42}, {warpTrans.M43}";

            Button btnCancel = new()
            {
                Text = "Cancel",
                Left = inputForm.Width - 100,
                Width = 70,
                Top = inputForm.Height - 70,
                DialogResult = DialogResult.Cancel
            };

            Button btnAdd = new()
            {
                Text = "Add",
                Left = btnCancel.Left - 80,
                Width = 70,
                Top = inputForm.Height - 70,
                DialogResult = DialogResult.OK
            };

            inputForm.Controls.Add(lblText);
            inputForm.Controls.Add(txtInput);
            inputForm.Controls.Add(chkOnlyForThisBuild);
            inputForm.Controls.Add(lblPreviewText);
            inputForm.Controls.Add(btnAdd);
            inputForm.Controls.Add(btnCancel);
            inputForm.AcceptButton = btnAdd;
            inputForm.CancelButton = btnCancel;

            DialogResult result = inputForm.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            Util.CustomWarpsFileWarpPoint warpToAdd = new()
            {
                Name = txtInput.Text,
                Rotation = [warpTrans.M11, warpTrans.M12, warpTrans.M13,
                            warpTrans.M21, warpTrans.M22, warpTrans.M23,
                            warpTrans.M31, warpTrans.M32, warpTrans.M33],
                Position = [warpTrans.M41, warpTrans.M42, warpTrans.M43],
            };

            if (chkOnlyForThisBuild.Checked)
            {
                warpToAdd.IncludeBuilds = [_game.Build.Region];
            }

            // Insert the new warp node relative to the selected node only if the selected node is a warp of the current map
            // Else, always add to the end of the current map warps list
            TreeNode newNode = new(warpToAdd.Name);
            newNode.Tag = warpToAdd;
            TreeNode selectedNode = trvWarps.SelectedNode;
            TreeNodeCollection mapNodes = trvWarps.Nodes;
            for (int i = 0; i < mapNodes.Count; i++)
            {
                if (string.Equals(currentMapName, mapNodes[i].Text, StringComparison.OrdinalIgnoreCase))
                {
                    if (selectedNode.Tag is CustomWarpsFileWarpPoint
                     && selectedNode.Parent.Text == currentMapName)
                    {
                        // A warp of the same map was selected, so we add relative from that node
                        mapNodes[i].Nodes.Insert(selectedNode.Index + 1, newNode);
                    }
                    else
                    {
                        // A warp of another map was selected
                        // or a map was selected
                        // We add at the end of the current map (so not necessarily the one selected!)
                        mapNodes[i].Nodes.Add(newNode);
                    }

                    break;
                }
            }

            SelectNode(newNode);
        }

        private void FormCustomWarps_Shown(object sender, EventArgs e)
        {
            btnAddWarp_Click(btnAddWarp, e);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to save the changes made?", "Save changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
            {
                return;
            }

            // SelectedNode is null when the list is empty
            TreeNode node = trvWarps.SelectedNode;
            if (node != null
             && node.Tag is CustomWarpsFileWarpPoint)
            {
                // Save the currently selected warp, so that the user doesn't lose the changes they made without clicking the "Save selected warp" button
                if (!CallbackSaveSelectedWarp())
                {
                    return;
                }
            }

            if (!GetRootFromCustomWarpsFile(out CustomWarpsFileRoot? root))
            {
                return;
            }

            // Ensure game entry exists
            string warpGameName = _game.Build.Title;
            if (!root.Data.ContainsKey(warpGameName))
            {
                root.Data[warpGameName] = [];
            }

            // Clear existing maps for this game
            root.Data[warpGameName].Clear();

            foreach (TreeNode mapNode in trvWarps.Nodes)
            {
                string mapName = mapNode.Text;
                List<CustomWarpsFileWarpPoint> warps = new();
                foreach (TreeNode warpNode in mapNode.Nodes)
                {
                    if (warpNode.Tag is CustomWarpsFileWarpPoint warp)
                    {
                        warps.Add(warp);
                    }
                }

                root.Data[warpGameName][mapName] = warps;
            }

            try
            {
                string json = System.Text.Json.JsonSerializer.Serialize(root, CustomWarpsJsonOptions);
                File.WriteAllText(CustomWarpsJsonFilePath, json);
                MessageBox.Show("Custom warps saved successfully.", "Save successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh so the change in the UI is immediate
                _game.CustomWarps = Util.GetCustomWarps(_game.Build);
                int mapId = _game.ReadMapId() + 1; // + first item for current map
                _game.RefreshWarps(mapId);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save custom warps:\n\n{ex.Message}", "Save error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        Matrix4x4 GetActCharTransformationRounded()
        {
            string actChar = _game.GetActCharPointer();
            Matrix4x4 warpTrans = _game.ReadEntityFinalCombinedTransformation(actChar);
            Vector3 actCharDelta = _game.ReadEntityDeltaTranslation(actChar);
            warpTrans.Translation = warpTrans.Translation - actCharDelta;
            warpTrans.M11 = MathF.Round(warpTrans.M11, 3);
            warpTrans.M12 = MathF.Round(warpTrans.M12, 3);
            warpTrans.M13 = MathF.Round(warpTrans.M13, 3);
            warpTrans.M14 = MathF.Round(warpTrans.M14, 3);
            warpTrans.M21 = MathF.Round(warpTrans.M21, 3);
            warpTrans.M22 = MathF.Round(warpTrans.M22, 3);
            warpTrans.M23 = MathF.Round(warpTrans.M23, 3);
            warpTrans.M24 = MathF.Round(warpTrans.M24, 3);
            warpTrans.M31 = MathF.Round(warpTrans.M31, 3);
            warpTrans.M32 = MathF.Round(warpTrans.M32, 3);
            warpTrans.M33 = MathF.Round(warpTrans.M33, 3);
            warpTrans.M34 = MathF.Round(warpTrans.M34, 3);
            warpTrans.M41 = MathF.Round(warpTrans.M41, 3);
            warpTrans.M42 = MathF.Round(warpTrans.M42, 3);
            warpTrans.M43 = MathF.Round(warpTrans.M43, 3);
            warpTrans.M44 = MathF.Round(warpTrans.M44, 3);
            return warpTrans;
        }

        private void btnFillWithCurrentTransformation_Click(object sender, EventArgs e)
        {
            Matrix4x4 warpTrans = GetActCharTransformationRounded();
            txtSelectedWarpRotation11.Text = warpTrans.M11.ToString();
            txtSelectedWarpRotation12.Text = warpTrans.M12.ToString();
            txtSelectedWarpRotation13.Text = warpTrans.M13.ToString();
            txtSelectedWarpRotation21.Text = warpTrans.M21.ToString();
            txtSelectedWarpRotation22.Text = warpTrans.M22.ToString();
            txtSelectedWarpRotation23.Text = warpTrans.M23.ToString();
            txtSelectedWarpRotation31.Text = warpTrans.M31.ToString();
            txtSelectedWarpRotation32.Text = warpTrans.M32.ToString();
            txtSelectedWarpRotation33.Text = warpTrans.M33.ToString();
            txtSelectedWarpPositionX.Text = warpTrans.M41.ToString();
            txtSelectedWarpPositionY.Text = warpTrans.M42.ToString();
            txtSelectedWarpPositionZ.Text = warpTrans.M43.ToString();
        }

        private bool GetTransformationFromTextboxes(out Matrix4x4 trans)
        {
            trans = Matrix4x4.Identity;
            if (!float.TryParse(txtSelectedWarpPositionX.Text, out float coordX))
            {
                MessageBox.Show("Invalid value for X coordinate.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!float.TryParse(txtSelectedWarpPositionY.Text, out float coordY))
            {
                MessageBox.Show("Invalid value for Y coordinate.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!float.TryParse(txtSelectedWarpPositionZ.Text, out float coordZ))
            {
                MessageBox.Show("Invalid value for Z coordinate.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!float.TryParse(txtSelectedWarpRotation11.Text, out float rot11))
            {
                MessageBox.Show("Invalid value for rotation matrix [1,1].", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!float.TryParse(txtSelectedWarpRotation12.Text, out float rot12))
            {
                MessageBox.Show("Invalid value for rotation matrix [1,2].", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!float.TryParse(txtSelectedWarpRotation13.Text, out float rot13))
            {
                MessageBox.Show("Invalid value for rotation matrix [1,3].", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!float.TryParse(txtSelectedWarpRotation21.Text, out float rot21))
            {
                MessageBox.Show("Invalid value for rotation matrix [2,1].", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!float.TryParse(txtSelectedWarpRotation22.Text, out float rot22))
            {
                MessageBox.Show("Invalid value for rotation matrix [2,2].", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!float.TryParse(txtSelectedWarpRotation23.Text, out float rot23))
            {
                MessageBox.Show("Invalid value for rotation matrix [2,3].", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!float.TryParse(txtSelectedWarpRotation31.Text, out float rot31))
            {
                MessageBox.Show("Invalid value for rotation matrix [3,1].", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!float.TryParse(txtSelectedWarpRotation32.Text, out float rot32))
            {
                MessageBox.Show("Invalid value for rotation matrix [3,2].", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!float.TryParse(txtSelectedWarpRotation33.Text, out float rot33))
            {
                MessageBox.Show("Invalid value for rotation matrix [3,3].", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            trans = new(
                rot11, rot12, rot13, 0,
                rot21, rot22, rot23, 0,
                rot31, rot32, rot33, 0,
                coordX, coordY, coordZ, 1
            );

            return true;
        }

        private void btnWarpToSelected_Click(object sender, EventArgs e)
        {
            TreeNode node = trvWarps.SelectedNode;
            if (node.Tag is not CustomWarpsFileWarpPoint)
            {
                return;
            }

            if (!GetTransformationFromTextboxes(out Matrix4x4 trans))
            {
                return;
            }

            _game.WarpSourceEntityToPoint("", trans);
            _game.ResetCamera();
        }
    }
}
