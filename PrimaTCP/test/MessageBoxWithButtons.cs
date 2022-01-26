using System;
using System.Windows.Forms;

namespace test
{
    public partial class MessageBoxWithButtons : Form
    {
        public bool _state;
        public MessageBoxWithButtons(string caption, string text, string leftButtonText, string rightButtonText)
        {
            InitializeComponent();
            this.Text = caption;
            this.text.Text = text;
            this.leftButton.Text = leftButtonText;
            this.rightButton.Text = rightButtonText;           
        }
        public MessageBoxWithButtons()
        {
            InitializeComponent();
        }
        private void leftButton_Click(object sender, EventArgs e)
        {
            _state = false;
            MessageBoxWithButtons.ActiveForm.Close();
        }
        private void rightButton_Click(object sender, EventArgs e)
        {
            _state = true;
            MessageBoxWithButtons.ActiveForm.Close();
        }
    }
}
