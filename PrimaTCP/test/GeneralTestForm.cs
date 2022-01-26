using System;
using System.Windows.Forms;

namespace test
{
    public partial class GeneralTestForm : Form
    {
        public bool TG;
        public bool FL;
        public bool WithoutAnswer;
        public bool WithAnswer;
        public bool Start = false;
        public GeneralTestForm()
        {
            InitializeComponent();

            this.FormClosing += new FormClosingEventHandler(Settings_Form_FormClosing);
        }
        void Settings_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            TG = PerevalTG_CB.Checked;
            FL = PerevalFL_CB.Checked;
            WithoutAnswer = RS_withoutAnswer_CB.Checked;
            WithAnswer = RS_withAnswer_CB.Checked;
        }
        private void CloseButton_Click(object sender, EventArgs e)
        {
            Start = true;
            this.Close();
        }
    }
}
