namespace SlyMultiTrainer
{
    partial class FormSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnSave = new Button();
            btnCancel = new Button();
            cmbFlyUp = new ComboBox();
            label1 = new Label();
            grpSettingsFly = new GroupBox();
            label3 = new Label();
            cmbFlyAccelerate = new ComboBox();
            label2 = new Label();
            cmbFlyDown = new ComboBox();
            btnResetSettings = new Button();
            tabControl1 = new TabControl();
            tabSettingsMain = new TabPage();
            label4 = new Label();
            cmbSkipCurrentDialogueHotkey = new ComboBox();
            grpSettingsEntities = new GroupBox();
            chkEntitiesSelectActChar = new CheckBox();
            grpSettingsWarps = new GroupBox();
            clbWarpsOrder = new CheckedListBox();
            btnWarpsOrderMoveDown = new Button();
            btnWarpsOrderMoveUp = new Button();
            grpSettingsFly.SuspendLayout();
            tabControl1.SuspendLayout();
            tabSettingsMain.SuspendLayout();
            grpSettingsEntities.SuspendLayout();
            grpSettingsWarps.SuspendLayout();
            SuspendLayout();
            // 
            // btnSave
            // 
            btnSave.AutoSize = true;
            btnSave.Location = new Point(455, 316);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(97, 25);
            btnSave.TabIndex = 2;
            btnSave.Text = "Save and exit";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnClose
            // 
            btnCancel.AutoSize = true;
            btnCancel.Location = new Point(558, 316);
            btnCancel.Name = "btnClose";
            btnCancel.Size = new Size(75, 25);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // cmbFlyUp
            // 
            cmbFlyUp.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFlyUp.Location = new Point(74, 25);
            cmbFlyUp.Name = "cmbFlyUp";
            cmbFlyUp.Size = new Size(86, 21);
            cmbFlyUp.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 28);
            label1.Name = "label1";
            label1.Size = new Size(21, 13);
            label1.TabIndex = 37;
            label1.Text = "Up";
            // 
            // grpSettingsFly
            // 
            grpSettingsFly.BackColor = Color.Transparent;
            grpSettingsFly.Controls.Add(label3);
            grpSettingsFly.Controls.Add(cmbFlyAccelerate);
            grpSettingsFly.Controls.Add(label2);
            grpSettingsFly.Controls.Add(cmbFlyDown);
            grpSettingsFly.Controls.Add(label1);
            grpSettingsFly.Controls.Add(cmbFlyUp);
            grpSettingsFly.Location = new Point(6, 6);
            grpSettingsFly.Name = "grpSettingsFly";
            grpSettingsFly.Size = new Size(169, 118);
            grpSettingsFly.TabIndex = 0;
            grpSettingsFly.TabStop = false;
            grpSettingsFly.Text = "Fly controller binds";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 86);
            label3.Name = "label3";
            label3.Size = new Size(58, 13);
            label3.TabIndex = 41;
            label3.Text = "Accelerate";
            // 
            // cmbFlyAccelerate
            // 
            cmbFlyAccelerate.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFlyAccelerate.Location = new Point(74, 83);
            cmbFlyAccelerate.Name = "cmbFlyAccelerate";
            cmbFlyAccelerate.Size = new Size(86, 21);
            cmbFlyAccelerate.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 57);
            label2.Name = "label2";
            label2.Size = new Size(35, 13);
            label2.TabIndex = 39;
            label2.Text = "Down";
            // 
            // cmbFlyDown
            // 
            cmbFlyDown.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFlyDown.Location = new Point(74, 54);
            cmbFlyDown.Name = "cmbFlyDown";
            cmbFlyDown.Size = new Size(86, 21);
            cmbFlyDown.TabIndex = 1;
            // 
            // btnResetSettings
            // 
            btnResetSettings.AutoSize = true;
            btnResetSettings.BackColor = SystemColors.Control;
            btnResetSettings.FlatAppearance.BorderSize = 0;
            btnResetSettings.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnResetSettings.ForeColor = SystemColors.ControlText;
            btnResetSettings.Location = new Point(12, 316);
            btnResetSettings.Name = "btnResetSettings";
            btnResetSettings.Size = new Size(89, 25);
            btnResetSettings.TabIndex = 1;
            btnResetSettings.Text = "Reset settings";
            btnResetSettings.UseVisualStyleBackColor = true;
            btnResetSettings.Click += btnResetSettings_Click;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabSettingsMain);
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(645, 310);
            tabControl1.TabIndex = 0;
            // 
            // tabSettingsMain
            // 
            tabSettingsMain.Controls.Add(label4);
            tabSettingsMain.Controls.Add(cmbSkipCurrentDialogueHotkey);
            tabSettingsMain.Controls.Add(grpSettingsEntities);
            tabSettingsMain.Controls.Add(grpSettingsWarps);
            tabSettingsMain.Controls.Add(grpSettingsFly);
            tabSettingsMain.Location = new Point(4, 22);
            tabSettingsMain.Name = "tabSettingsMain";
            tabSettingsMain.Padding = new Padding(3);
            tabSettingsMain.Size = new Size(637, 284);
            tabSettingsMain.TabIndex = 0;
            tabSettingsMain.Text = "Main";
            tabSettingsMain.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 189);
            label4.Name = "label4";
            label4.Size = new Size(142, 13);
            label4.TabIndex = 39;
            label4.Text = "Skip current dialogue hotkey";
            // 
            // cmbSkipCurrentDialogueHotkey
            // 
            cmbSkipCurrentDialogueHotkey.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSkipCurrentDialogueHotkey.Location = new Point(160, 186);
            cmbSkipCurrentDialogueHotkey.Name = "cmbSkipCurrentDialogueHotkey";
            cmbSkipCurrentDialogueHotkey.Size = new Size(86, 21);
            cmbSkipCurrentDialogueHotkey.TabIndex = 38;
            // 
            // grpSettingsEntities
            // 
            grpSettingsEntities.BackColor = Color.Transparent;
            grpSettingsEntities.Controls.Add(chkEntitiesSelectActChar);
            grpSettingsEntities.Location = new Point(6, 130);
            grpSettingsEntities.Name = "grpSettingsEntities";
            grpSettingsEntities.Size = new Size(360, 50);
            grpSettingsEntities.TabIndex = 2;
            grpSettingsEntities.TabStop = false;
            grpSettingsEntities.Text = "Entities";
            // 
            // chkEntitiesSelectActChar
            // 
            chkEntitiesSelectActChar.AutoSize = true;
            chkEntitiesSelectActChar.Location = new Point(6, 19);
            chkEntitiesSelectActChar.Name = "chkEntitiesSelectActChar";
            chkEntitiesSelectActChar.Size = new Size(333, 17);
            chkEntitiesSelectActChar.TabIndex = 0;
            chkEntitiesSelectActChar.Text = "Automatically select the current character on list refresh";
            chkEntitiesSelectActChar.UseVisualStyleBackColor = true;
            // 
            // grpSettingsWarps
            // 
            grpSettingsWarps.BackColor = Color.Transparent;
            grpSettingsWarps.Controls.Add(clbWarpsOrder);
            grpSettingsWarps.Controls.Add(btnWarpsOrderMoveDown);
            grpSettingsWarps.Controls.Add(btnWarpsOrderMoveUp);
            grpSettingsWarps.Location = new Point(181, 6);
            grpSettingsWarps.Name = "grpSettingsWarps";
            grpSettingsWarps.Size = new Size(185, 118);
            grpSettingsWarps.TabIndex = 1;
            grpSettingsWarps.TabStop = false;
            grpSettingsWarps.Text = "Order and visibility of warps";
            // 
            // clbWarpsOrder
            // 
            clbWarpsOrder.FormattingEnabled = true;
            clbWarpsOrder.Items.AddRange(new object[] { "Built-in", "Custom", "Entrance" });
            clbWarpsOrder.Location = new Point(6, 19);
            clbWarpsOrder.Name = "clbWarpsOrder";
            clbWarpsOrder.Size = new Size(133, 94);
            clbWarpsOrder.TabIndex = 0;
            clbWarpsOrder.SelectedIndexChanged += clbWarpsOrder_SelectedIndexChanged;
            // 
            // btnWarpsOrderMoveDown
            // 
            btnWarpsOrderMoveDown.Font = new Font("Microsoft Sans Serif", 13F);
            btnWarpsOrderMoveDown.Location = new Point(145, 54);
            btnWarpsOrderMoveDown.Name = "btnWarpsOrderMoveDown";
            btnWarpsOrderMoveDown.Size = new Size(30, 30);
            btnWarpsOrderMoveDown.TabIndex = 2;
            btnWarpsOrderMoveDown.Text = "▼";
            btnWarpsOrderMoveDown.UseVisualStyleBackColor = true;
            btnWarpsOrderMoveDown.Click += btnWarpsOrderMoveDown_Click;
            // 
            // btnWarpsOrderMoveUp
            // 
            btnWarpsOrderMoveUp.Font = new Font("Microsoft Sans Serif", 13F);
            btnWarpsOrderMoveUp.Location = new Point(145, 18);
            btnWarpsOrderMoveUp.Name = "btnWarpsOrderMoveUp";
            btnWarpsOrderMoveUp.Size = new Size(30, 30);
            btnWarpsOrderMoveUp.TabIndex = 1;
            btnWarpsOrderMoveUp.Text = "▲";
            btnWarpsOrderMoveUp.UseVisualStyleBackColor = true;
            btnWarpsOrderMoveUp.Click += btnWarpsOrderMoveUp_Click;
            // 
            // FormSettings
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = SystemColors.Control;
            ClientSize = new Size(645, 351);
            Controls.Add(tabControl1);
            Controls.Add(btnResetSettings);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Font = new Font("Microsoft Sans Serif", 8F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormSettings";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Sly Multi Trainer - Settings";
            FormClosing += FormSettings_FormClosing;
            Load += FormSettings_Load;
            grpSettingsFly.ResumeLayout(false);
            grpSettingsFly.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabSettingsMain.ResumeLayout(false);
            tabSettingsMain.PerformLayout();
            grpSettingsEntities.ResumeLayout(false);
            grpSettingsEntities.PerformLayout();
            grpSettingsWarps.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnSave;
        private Button btnCancel;
        public ComboBox cmbFlyUp;
        private Label label1;
        private GroupBox grpSettingsFly;
        private Label label3;
        public ComboBox cmbFlyAccelerate;
        private Label label2;
        public ComboBox cmbFlyDown;
        public Button btnResetSettings;
        private TabControl tabControl1;
        private TabPage tabSettingsMain;
        private CheckBox chkEntitiesSelectActChar;
        private CheckedListBox clbWarpsOrder;
        private Button btnWarpsOrderMoveDown;
        private Button btnWarpsOrderMoveUp;
        private GroupBox grpSettingsEntities;
        private GroupBox grpSettingsWarps;
        private Label label4;
        public ComboBox cmbSkipCurrentDialogueHotkey;
    }
}