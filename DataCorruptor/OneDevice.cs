using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace SodWinForms
{
    public partial class OneDevice : Form
    {
        int numberOfChannel;
        string ipAdress;
        int ipPort;
        public MainWindow mainWindow;
        Corrupter corrupter;
        int dlinaOshibok;
        double chanceOfError;
        double koefGrupp;
        NetWorker netWorker;
        int ts;
        int speed;
        delegate void emptyFunction();
        emptyFunction empty;
        public OneDevice(string ipAdressDevice, string portDevice)
        {
            InitializeComponent();
            ipAdress = ipAdressDevice;
            ipPort = Convert.ToInt32(portDevice);
            //mainWindow = (MainWindow)this.Owner;
        }
        private void ResetNetwork()
        {
            if (netWorker != null)
            {
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
        private void Back_Click(object sender, EventArgs e)
        {
            ResetNetwork();
        }
        private void MainWindowOnTop()
        {
            empty = mainWindow.OnTheTopScreen;
            Invoke(empty);    
        }
        private void Start_Click(object sender, EventArgs e)
        {
            try
            {
                dlinaOshibok = Convert.ToInt32(errorThreadLength.Text);
                chanceOfError = Convert.ToDouble(averageChanceError.Text.Replace('.', ','));
                koefGrupp = Convert.ToDouble(KoefficientGroup.Text.Replace('.', ','));
                numberOfChannel = (int)Math.Pow(2, Channel1_CB.SelectedIndex);
                ts = TS_CB.SelectedIndex;
                speed = Speed_CB.SelectedIndex;
                if (netWorker != null)
                {
                    if (Speed_CB.SelectedIndex > 5)
                    {
                        testMessageTothe2ndChannel.Enabled = false;
                    }
                    else 
                    {
                        testMessageTothe2ndChannel.Enabled = true;
                    }
                    netWorker.channel2NoAnswer = true;
                    netWorker.GenerateMessage10(numberOfChannel, speed, ts, 1);
                    corrupter.n7 = (short)dlinaOshibok;
                    corrupter.p7 = chanceOfError;
                    corrupter.aa = koefGrupp;
                    corrupter.InitModel();
                }
                else
                {
                    corrupter = new Corrupter(dlinaOshibok, chanceOfError, koefGrupp);
                    netWorker = new NetWorker(ipAdress, ipPort, numberOfChannel, numberOfChannel, corrupter, (numberOfChannel & 1) + 1, (numberOfChannel & 1) + 1, new Corrupter(992, 0, 0), mainWindow);
                    netWorker.channel2NoAnswer = true;
                    if (netWorker.TCPConnect())
                    {
                        if (Speed_CB.SelectedIndex <= 5)
                        {
                            testMessageTothe2ndChannel.Enabled = true;
                        }
                        else
                        {
                            testMessageTothe2ndChannel.Enabled = false;
                        }
                        this.Text = this.Text + " (подключено)";
                        netWorker.GenerateMessage10(numberOfChannel, speed, ts, 1);
                    }
                    else
                    {
                        netWorker = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void OneDevice_FormClosed(object sender, FormClosedEventArgs e)
        {
            ResetNetwork();
        }
        private void TestMessageTothe2ndChannel_Click(object sender, EventArgs e)
        {
            MainWindowOnTop();
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            testMessageTothe2ndChannel.Enabled = false;
            string adressOfStandart = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\savedMessages\\Эталон.txt";
            dlinaOshibok = Convert.ToInt32(errorThreadLength.Text);
            chanceOfError = Convert.ToDouble(averageChanceError.Text.Replace('.', ','));
            koefGrupp = Convert.ToDouble(KoefficientGroup.Text.Replace('.', ','));
            if (File.Exists(adressOfStandart))
            {
                netWorker.channel2XiskProcedure = true;
                netWorker.xisk = new XiskProcedure(dlinaOshibok, chanceOfError, koefGrupp, mainWindow); ;
                netWorker.GenerateMessage10((numberOfChannel & 1) + 1, Speed_CB.SelectedIndex, TS_CB.SelectedIndex, 0x80);
                netWorker.SendFromFile(adressOfStandart, (numberOfChannel & 1) + 1);
                mainWindow.Focus();
            }
            else
            {
                MessageBox.Show("Файл эталона не существует");
            }
            while (stopwatch.ElapsedMilliseconds<2000)
            {
                System.Threading.Thread.Sleep(1);
            }
            stopwatch.Reset();
            testMessageTothe2ndChannel.Enabled = true;

        }
    }
}
