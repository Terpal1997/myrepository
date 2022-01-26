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
    public partial class ChannelSelector : Form
    {
        static public int _channel = -1;
        RadioButton[] chRB;
        public ChannelSelector()
        {
            InitializeComponent();
            chRB = new RadioButton[8] { radioButton1, radioButton2, radioButton3, radioButton4, radioButton5, radioButton6, radioButton7, radioButton8 };
            this.FormClosing += new FormClosingEventHandler(Settings_Form_FormClosing);
        }
        void Settings_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chRB.Length; i++)
            {
                if (chRB[i].Checked)
                {
                    _channel = i;
                    break;
                }
            }
            this.Hide();
        }
    }
}
