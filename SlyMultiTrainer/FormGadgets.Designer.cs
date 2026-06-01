namespace SlyMultiTrainer
{
    partial class FormGadgets
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
            grpSly = new GroupBox();
            clbSly = new CheckedListBox();
            grpBentley = new GroupBox();
            clbBentley = new CheckedListBox();
            grpMurray = new GroupBox();
            clbMurray = new CheckedListBox();
            btnClose = new Button();
            btnToggleAllSly = new Button();
            btnToggleAllBentley = new Button();
            btnToggleAllMurray = new Button();
            grpSly.SuspendLayout();
            grpBentley.SuspendLayout();
            grpMurray.SuspendLayout();
            SuspendLayout();
            // 
            // grpSly
            // 
            grpSly.BackColor = SystemColors.Control;
            grpSly.Controls.Add(clbSly);
            grpSly.Location = new Point(12, 12);
            grpSly.Name = "grpSly";
            grpSly.Size = new Size(215, 317);
            grpSly.TabIndex = 0;
            grpSly.TabStop = false;
            grpSly.Text = "Sly";
            // 
            // clbSly
            // 
            clbSly.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            clbSly.CheckOnClick = true;
            clbSly.FormattingEnabled = true;
            clbSly.Location = new Point(6, 19);
            clbSly.Name = "clbSly";
            clbSly.Size = new Size(203, 289);
            clbSly.TabIndex = 1;
            clbSly.ItemCheck += clbSly_ItemCheck;
            // 
            // grpBentley
            // 
            grpBentley.BackColor = SystemColors.Control;
            grpBentley.Controls.Add(clbBentley);
            grpBentley.Location = new Point(236, 12);
            grpBentley.Name = "grpBentley";
            grpBentley.Size = new Size(215, 317);
            grpBentley.TabIndex = 1;
            grpBentley.TabStop = false;
            grpBentley.Text = "Bentley";
            // 
            // clbBentley
            // 
            clbBentley.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            clbBentley.CheckOnClick = true;
            clbBentley.FormattingEnabled = true;
            clbBentley.Location = new Point(6, 19);
            clbBentley.Name = "clbBentley";
            clbBentley.Size = new Size(203, 289);
            clbBentley.TabIndex = 1;
            clbBentley.ItemCheck += clbBentley_ItemCheck;
            // 
            // grpMurray
            // 
            grpMurray.BackColor = SystemColors.Control;
            grpMurray.Controls.Add(clbMurray);
            grpMurray.Location = new Point(460, 12);
            grpMurray.Name = "grpMurray";
            grpMurray.Size = new Size(215, 317);
            grpMurray.TabIndex = 2;
            grpMurray.TabStop = false;
            grpMurray.Text = "Murray";
            // 
            // clbMurray
            // 
            clbMurray.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            clbMurray.CheckOnClick = true;
            clbMurray.FormattingEnabled = true;
            clbMurray.Location = new Point(6, 19);
            clbMurray.Name = "clbMurray";
            clbMurray.Size = new Size(203, 289);
            clbMurray.TabIndex = 1;
            clbMurray.ItemCheck += clbMurray_ItemCheck;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.Location = new Point(601, 335);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(75, 25);
            btnClose.TabIndex = 4;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // btnToggleAllSly
            // 
            btnToggleAllSly.Location = new Point(125, 6);
            btnToggleAllSly.Name = "btnToggleAllSly";
            btnToggleAllSly.Size = new Size(97, 25);
            btnToggleAllSly.TabIndex = 5;
            btnToggleAllSly.Text = "Toggle all";
            btnToggleAllSly.UseVisualStyleBackColor = true;
            btnToggleAllSly.Click += btnToggleAllSly_Click;
            // 
            // btnToggleAllBentley
            // 
            btnToggleAllBentley.Location = new Point(349, 6);
            btnToggleAllBentley.Name = "btnToggleAllBentley";
            btnToggleAllBentley.Size = new Size(97, 25);
            btnToggleAllBentley.TabIndex = 6;
            btnToggleAllBentley.Text = "Toggle all";
            btnToggleAllBentley.UseVisualStyleBackColor = true;
            btnToggleAllBentley.Click += btnToggleAllBentley_Click;
            // 
            // btnToggleAllMurray
            // 
            btnToggleAllMurray.Location = new Point(573, 6);
            btnToggleAllMurray.Name = "btnToggleAllMurray";
            btnToggleAllMurray.Size = new Size(97, 25);
            btnToggleAllMurray.TabIndex = 7;
            btnToggleAllMurray.Text = "Toggle all";
            btnToggleAllMurray.UseVisualStyleBackColor = true;
            btnToggleAllMurray.Click += btnToggleAllMurray_Click;
            // 
            // FormGadgets
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(687, 372);
            Controls.Add(btnToggleAllMurray);
            Controls.Add(btnToggleAllBentley);
            Controls.Add(btnToggleAllSly);
            Controls.Add(btnClose);
            Controls.Add(grpMurray);
            Controls.Add(grpBentley);
            Controls.Add(grpSly);
            Font = new Font("Microsoft Sans Serif", 8F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormGadgets";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Gadgets";
            Load += FormGadgets_Load;
            grpSly.ResumeLayout(false);
            grpBentley.ResumeLayout(false);
            grpMurray.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox grpSly;
        private CheckedListBox clbSly;
        private GroupBox grpBentley;
        private CheckedListBox clbBentley;
        private GroupBox grpMurray;
        private CheckedListBox clbMurray;
        private Button btnClose;
        private Button btnToggleAllSly;
        private Button btnToggleAllBentley;
        private Button btnToggleAllMurray;
    }
}