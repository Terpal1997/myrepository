using System.Windows.Forms;

namespace SimulatorRS
{
    public partial class Settings_Form : Form
    {
        public Settings_Form()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(Settings_Form_FormClosing);
        }
        void Settings_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
