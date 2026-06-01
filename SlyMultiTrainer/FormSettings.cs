namespace SlyMultiTrainer
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            var controllerBinds = typeof(Util.Controller_t).GetProperties().Select(p => p.Name).ToArray();
            cmbFlyUp.Items.AddRange(controllerBinds);
            cmbFlyDown.Items.AddRange(controllerBinds);
            cmbFlyAccelerate.Items.AddRange(controllerBinds);
            cmbSkipCurrentDialogueHotkey.Items.AddRange(controllerBinds);
            btnWarpsOrderMoveUp.Enabled = false;
            btnWarpsOrderMoveDown.Enabled = false;
            ReadSettings();
        }

        private void ReadSettings()
        {
            cmbFlyUp.SelectedItem = Properties.Settings.Default.FlyButtonUp;
            cmbFlyDown.SelectedItem = Properties.Settings.Default.FlyButtonDown;
            cmbFlyAccelerate.SelectedItem = Properties.Settings.Default.FlyButtonAccelerate;
            cmbSkipCurrentDialogueHotkey.SelectedItem = Properties.Settings.Default.SkipCurrentDialogueBind;
            chkEntitiesSelectActChar.Checked = Properties.Settings.Default.EntitiesSelectActChar;

            var items = Properties.Settings.Default.WarpsList.Split('|');
            clbWarpsOrder.Items.Clear();
            foreach (var item in items)
            {
                var parts = item.Split(';');
                if (parts.Length != 2)
                {
                    continue;
                }

                string name = parts[0];
                bool isChecked = bool.Parse(parts[1]);
                clbWarpsOrder.Items.Add(name, isChecked);
            }
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.FlyButtonUp = cmbFlyUp.SelectedItem?.ToString();
            Properties.Settings.Default.FlyButtonDown = cmbFlyDown.SelectedItem?.ToString();
            Properties.Settings.Default.FlyButtonAccelerate = cmbFlyAccelerate.SelectedItem?.ToString();
            Properties.Settings.Default.SkipCurrentDialogueBind = cmbSkipCurrentDialogueHotkey.SelectedItem?.ToString();
            Properties.Settings.Default.EntitiesSelectActChar = chkEntitiesSelectActChar.Checked;

            List<string> warpsList = new(3);
            for (int i = 0; i < clbWarpsOrder.Items.Count; i++)
            {
                string name = clbWarpsOrder.Items[i].ToString();
                CheckState checkState = clbWarpsOrder.GetItemCheckState(i);
                if (checkState == CheckState.Checked)
                {
                    warpsList.Add($"{name};true");
                }
                else
                {
                    warpsList.Add($"{name};false");
                }
            }
            Properties.Settings.Default.WarpsList = string.Join("|", warpsList);
            Properties.Settings.Default.Save();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveSettings();
            MessageBox.Show("The settings were successfully saved!", "Sly Multi Trainer - Settings saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK)
            {
                // From save and exit
                return;
            }

            var result = MessageBox.Show("Are you sure you want to discard the changes made?", "Discard changes", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void btnResetSettings_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to reset the settings to their default values?", "Sly Multi Trainer - Reset Settings", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Properties.Settings.Default.Reset();
                ReadSettings();
                MessageBox.Show("The settings have been reset to their default values.", "Sly Multi Trainer - Reset Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnWarpsOrderMoveUp_Click(object sender, EventArgs e)
        {
            var item = clbWarpsOrder.SelectedItem;
            if (item != null && clbWarpsOrder.SelectedIndex > 0)
            {
                int index = clbWarpsOrder.SelectedIndex;
                CheckState checkState = clbWarpsOrder.GetItemCheckState(index);
                clbWarpsOrder.Items.RemoveAt(index);
                clbWarpsOrder.Items.Insert(index - 1, item);
                clbWarpsOrder.SetSelected(index - 1, true);
                clbWarpsOrder.SetItemCheckState(index - 1, checkState);
            }
        }

        private void btnWarpsOrderMoveDown_Click(object sender, EventArgs e)
        {
            var item = clbWarpsOrder.SelectedItem;
            if (item != null && clbWarpsOrder.SelectedIndex < clbWarpsOrder.Items.Count - 1)
            {
                int index = clbWarpsOrder.SelectedIndex;
                CheckState checkState = clbWarpsOrder.GetItemCheckState(index);
                clbWarpsOrder.Items.RemoveAt(index);
                clbWarpsOrder.Items.Insert(index + 1, item);
                clbWarpsOrder.SetSelected(index + 1, true);
                clbWarpsOrder.SetItemCheckState(index + 1, checkState);
            }
        }

        private void clbWarpsOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (clbWarpsOrder.SelectedIndex == -1)
            {
                return;
            }

            if (clbWarpsOrder.SelectedIndex > 0)
            {
                btnWarpsOrderMoveUp.Enabled = true;
            }
            else
            {
                btnWarpsOrderMoveUp.Enabled = false;
            }

            if (clbWarpsOrder.SelectedIndex < clbWarpsOrder.Items.Count - 1)
            {
                btnWarpsOrderMoveDown.Enabled = true;
            }
            else
            {
                btnWarpsOrderMoveDown.Enabled = false;
            }
        }
    }
}
