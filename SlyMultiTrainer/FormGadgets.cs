using static SlyMultiTrainer.Util;

namespace SlyMultiTrainer
{
    public partial class FormGadgets : Form
    {
        GameBase_t _game;
        CheckedListBox[] _clbGadgets;
        // The event ItemCheck is fired multiple times for various reasons (e.g. updating the items with SetItemChecked)
        // We only allow the ItemCheck event to pass when it's the user that triggers it
        bool _suppressItemCheck;

        public FormGadgets(GameBase_t game)
        {
            InitializeComponent();
            _game = game;
            this.Text = $"Sly Multi Trainer - Manage {_game.Build.Title} gadgets";
            if (_game is Sly1Handler)
            {
                _clbGadgets = [clbSly];
                grpBentley.Visible = false;
                grpMurray.Visible = false;
                btnToggleAllBentley.Visible = false;
                btnToggleAllMurray.Visible = false;
            }
            else
            {
                _clbGadgets = [clbSly, clbBentley, clbMurray];
                grpBentley.Visible = true;
                grpMurray.Visible = true;
                btnToggleAllBentley.Visible = true;
                btnToggleAllMurray.Visible = true;
            }
        }

        private async void FormGadgets_Load(object sender, EventArgs e)
        {
            _suppressItemCheck = true;
            for (int i = 0; i < _clbGadgets.Length; i++)
            {
                _clbGadgets[i].BeginUpdate();
                foreach (var gadget in _game.Gadgets[i])
                {
                    if (gadget.Id == -1)
                    {
                        continue;
                    }

                    _clbGadgets[i].Items.Add(gadget);
                }

                _clbGadgets[i].EndUpdate();
            }

            _suppressItemCheck = false;
            while (!this.IsDisposed)
            {
                long currentGadgets = _game.ReadGadgets();
                for (int i = 0; i < _clbGadgets.Length; i++)
                {
                    foreach (var gadget in _game.Gadgets[i])
                    {
                        if (gadget.Id == -1)
                        {
                            continue;
                        }

                        bool isEarned = _game.IsGadgetEarned(currentGadgets, gadget);
                        var index = _clbGadgets[i].Items.IndexOf(gadget);
                        var current = _clbGadgets[i].GetItemChecked(index);
                        if (current != isEarned)
                        {
                            _suppressItemCheck = true;
                            _clbGadgets[i].SetItemChecked(index, isEarned);
                            _suppressItemCheck = false;
                        }
                    }
                }

                await Task.Delay(100);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnToggleAllSly_Click(object sender, EventArgs e)
        {
            HandleCheckedListBoxToggleAll(clbSly);
        }

        private void btnToggleAllBentley_Click(object sender, EventArgs e)
        {
            HandleCheckedListBoxToggleAll(clbBentley);
        }

        private void btnToggleAllMurray_Click(object sender, EventArgs e)
        {
            HandleCheckedListBoxToggleAll(clbMurray);
        }

        private void HandleCheckedListBoxToggleAll(CheckedListBox clb)
        {
            _suppressItemCheck = true;

            // true when at least 1 is unchecked
            // false when all are checked
            bool IsNotChecked = clb.CheckedItems.Count < clb.Items.Count;
            long currentGadgets = _game.ReadGadgets();
            for (int i = 0; i < clb.Items.Count; i++)
            {
                var gadget = clb.Items[i] as Gadget_t;
                currentGadgets = _game.ToggleEarnedGadget(currentGadgets, gadget, IsNotChecked);
                clb.SetItemChecked(i, IsNotChecked);
            }

            _game.WriteGadgets(currentGadgets);
            _suppressItemCheck = false;
        }

        private void clbSly_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            HandleCheckedListBoxItemCheck(clbSly, e);
        }

        private void clbBentley_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            HandleCheckedListBoxItemCheck(clbBentley, e);
        }

        private void clbMurray_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            HandleCheckedListBoxItemCheck(clbMurray, e);
        }

        void HandleCheckedListBoxItemCheck(CheckedListBox clb, ItemCheckEventArgs e)
        {
            if (_suppressItemCheck)
            {
                return;
            }

            var gadget = clb.Items[e.Index] as Gadget_t;
            if (gadget == null)
            {
                return;
            }

            long currentGadgets = _game.ReadGadgets();
            bool IsChecked = e.NewValue == CheckState.Checked;
            currentGadgets = _game.ToggleEarnedGadget(currentGadgets, gadget, IsChecked);
            _game.WriteGadgets(currentGadgets);
        }
    }
}
