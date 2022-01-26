using System;

using System.Windows.Forms;

namespace SimulatorRS
{
    public partial class WritingRecord : Form
    {
        public WritingRecord()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(Settings_Form_FormClosing);
        }
        void Settings_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void ChooseFolderButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FDB = new FolderBrowserDialog();
            if (FDB.ShowDialog() == DialogResult.OK)
            {
                string str = FDB.SelectedPath;
                //FolderPath = FDB.SelectedPath;
                FilePath.Text = FDB.SelectedPath;
            }
        }
    }
}
