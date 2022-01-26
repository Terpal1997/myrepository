using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;


namespace test
{
    public partial class AdressesEthernet : Form
    {
        public AdressesEthernet()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(Settings_Form_FormClosing);
        }
        void Settings_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;

            this.Hide();
        }
        static string f1;
        private void IPTextBox_Leave(object sender, EventArgs e)
        {
            TextBox f = sender as TextBox;
            IPAddress ip;
            if (!IPAddress.TryParse(f.Text, out ip))
            {
                f.Text = f1;
                MessageBox.Show("текст не соответствует формату ip-адреса");
            }
        }
        private void IPTextBox_Enter(object sender, EventArgs e)
        {
            TextBox f = sender as TextBox;
            f1 = f.Text;
        }
        private void MACTextBox_Leave(object sender, EventArgs e)
        {
            TextBox f = sender as TextBox;
            if (!Regex.IsMatch(f.Text, @"([0-9a-fA-F]{2}([:-]|$)){6}$|([0-9a-fA-F]{4}([.]|$)){3}"))
            {
                f.Text = f1;
                MessageBox.Show("текст не соответствует формату MAC-адреса");
            }
        }

        private void AdressesEthernet_SizeChanged(object sender, EventArgs e)
        {
            panel1.Width = this.Width - 34;
        }
    }
}
