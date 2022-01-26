using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SodWinForms
{
    public partial class TwoDevices : Form
    {
        int numberOfChannel1;
        int numberOfChannel2;
        string ipAdress;
        int ipPort;
        Corrupter corrupter1;
        Corrupter corrupter2;
        int dlinaOshibok;
        double chanceOfError;
        double koefGrupp;
        NetWorker netWorker;
        public MainWindow mainWindow;
        int ts;
        int speed;
        delegate void emptyFunction();
        emptyFunction empty;
        private void MainWindowOnTop()
        {
            empty = mainWindow.OnTheTopScreen;
            Invoke(empty);
        }
        public TwoDevices(string ipAdressDevice, string portDevice)
        {
            InitializeComponent();
            ipAdress = ipAdressDevice;
            ipPort = Convert.ToInt32(portDevice);
            mainWindow = (MainWindow)this.Owner;
        }
       
        private void ResetNetwork()
        {
            if (netWorker != null)
            {
                netWorker.connect = false;
                netWorker.TcpClose();
            }
            mainWindow.режимРаботыToolStripMenuItem.Enabled = true;
            this.Close();
        }
        private void OnlyNumbers_KeyPress(object sender, KeyPressEventArgs e)
        {
            mainWindow.OnlynumbersCheck(sender, e);
        }
        private void OnlyNumbersDrobes_KeyPress(object sender, KeyPressEventArgs e)
        {
            mainWindow.OnlynumbersAndDotesCheck(sender, e);
        }
        private int DS_Count(string s)
        {
            string substr = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0].ToString();
            int count = (s.Length - s.Replace(substr, "").Length) / substr.Length;
            return count;
        }
        private void Start_Click(object sender, EventArgs e)
        {
            MainWindowOnTop();
            dlinaOshibok = Convert.ToInt32(errorThreadLength.Text);
            chanceOfError = Convert.ToDouble(averageChanceError.Text.Replace('.', ','));
            koefGrupp = Convert.ToDouble(KoefficientGroup.Text.Replace('.', ','));
            corrupter1 = new Corrupter(dlinaOshibok, chanceOfError, koefGrupp);
            corrupter2 = new Corrupter(dlinaOshibok, chanceOfError, koefGrupp);
            numberOfChannel1 = (int)Math.Pow(2, Channel1_CB.SelectedIndex);
            numberOfChannel2 = (int)Math.Pow(2, Channel1_CB.SelectedIndex);
            netWorker = new NetWorker(ipAdress, ipPort, numberOfChannel1, (numberOfChannel1&1)+1, corrupter1, numberOfChannel2, (numberOfChannel2 & 1) + 1, corrupter2, mainWindow);
            if (netWorker != null)
            {
                netWorker.GenerateMessage10(numberOfChannel1, speed, ts, 2, numberOfChannel2);
                corrupter1.n7 = (short)dlinaOshibok;
                corrupter1.p7 = chanceOfError;
                corrupter1.aa = koefGrupp;
                corrupter2.n7 = (short)dlinaOshibok;
                corrupter2.p7 = chanceOfError;
                corrupter2.aa = koefGrupp;
                corrupter1.InitModel();
                corrupter2.InitModel();
            }
            else
            {
                if (netWorker.TCPConnect())
                {
                    ts = TS_CB.SelectedIndex;
                    speed = Speed_CB.SelectedIndex;
                    this.Text = this.Text + " (подключено)";
                    netWorker.GenerateMessage10(numberOfChannel1, speed, ts, 2, numberOfChannel2);
                }
                else
                {
                    netWorker = null;
                }
            }
        }
        private void Back_Click(object sender, EventArgs e)
        {
            ResetNetwork();
        }
        private void TwoDevices_FormClosed(object sender, FormClosedEventArgs e)
        {
            ResetNetwork();
        }        
    }
}
