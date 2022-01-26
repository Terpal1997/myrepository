using System.Windows.Forms;

namespace test
{
    public partial class PerfomanceTestCounter : Form
    {
        public PerfomanceTestCounter()
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
