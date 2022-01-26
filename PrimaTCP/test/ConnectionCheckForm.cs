using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace test
{
   
    public partial class ConnectionCheckForm : Form
    {
        public int timeOutLength;

        public bool Start = false;
        public ConnectionCheckForm()
        {
            InitializeComponent();

            this.FormClosing += new FormClosingEventHandler(Settings_Form_FormClosing);
        }
        void Settings_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            timeOutLength = Convert.ToInt32(timeoutMessage.Text);
        }
        private void CloseButton_Click(object sender, EventArgs e)
        {
            Start = true;
            this.Close();
        }
    }
}
