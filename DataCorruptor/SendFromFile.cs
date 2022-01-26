using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SodWinForms
{
    public partial class SendFromFile : Form
    {
        int numberOfChannel;
        string ipAdress;
        int ipPort;
        public MainWindow mainWindow;
        Corrupter corrupter;
        NetWorker netWorker;
        int ts;
        int speed;
        delegate void TakeAdress();
        RadioButton[] RBs;
        GroupBox[] RB_GBs;
        object[] contents;
        delegate void emptyFunction();
        emptyFunction empty;
        private void MainWindowOnTop()
        {
            empty = mainWindow.OnTheTopScreen;
            Invoke(empty);
        }
        public SendFromFile(string ipAdressDevice, string portDevice)
        {
            InitializeComponent();
            ipAdress = ipAdressDevice;
            ipPort = Convert.ToInt32(portDevice);
            DirectoryInfo dirinfo = new DirectoryInfo(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\savedMessages\\");
            
            if (!dirinfo.Exists)
            {
                dirinfo.Create();
            }
            textBox1.Text= System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)+  "\\savedMessages\\Эталон.txt";
            RBs = new RadioButton[6] { radioButton6, radioButton2, radioButton5, radioButton3, radioButton4, radioButton1 };
            RB_GBs = new GroupBox[6] { RBgroupBox6, RBgroupBox2, RBgroupBox5, RBgroupBox3, RBgroupBox4, RBgroupBox1 };
            contents = new object[6] { RBcomboBox6, RBlabel2, RBcomboBox5, RBcomboBox3, RBcomboBox4, RBlabel1 };
            foreach (object item in contents)
            {
                if (item is ComboBox)
                {
                    ((ComboBox)item).SelectedIndex = 0;
                }
            }

        }
        private void Start_Click(object sender, EventArgs e)
        {
            numberOfChannel = (int)Math.Pow(2, Channel1_CB.SelectedIndex);
            MainWindowOnTop();
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            Start.Enabled = false;
            if (netWorker != null)
            {
                generationmessage10();
                netWorker.SendFromFile(textBox1.Text, numberOfChannel);
            }
            else
            {
                corrupter = new Corrupter(992, 0, 0);
                netWorker = new NetWorker(ipAdress, ipPort, numberOfChannel, numberOfChannel, corrupter, (numberOfChannel & 1) + 1, (numberOfChannel & 1) + 1, new Corrupter(992, 0, 0), mainWindow);
                if (netWorker.TCPConnect())
                {
                    ts = TS_CB.SelectedIndex;
                    speed = Speed_CB.SelectedIndex;
                    Text = Text + " (подключено)";
                    generationmessage10();
                    netWorker.SendFromFile(textBox1.Text, numberOfChannel);
                }
                else
                {
                    netWorker = null;
                }            
            }
            while (stopwatch.ElapsedMilliseconds < 2000)
            {
                System.Threading.Thread.Sleep(1);
            }
            stopwatch.Reset();
            Start.Enabled = true;
        }
        private void generationmessage10()
        {
            numberOfChannel = (int)Math.Pow(2, Channel1_CB.SelectedIndex);
            ts = TS_CB.SelectedIndex;
            speed = Speed_CB.SelectedIndex;
            int mode = 0x80;
            for (int i = 0; i < RBs.Length; i++)
            {
                if (RBs[i].Checked)
                {
                    mode += (i * 16);
                    if (contents[i] is ComboBox)
                    {
                        mode += ((ComboBox)contents[i]).SelectedIndex;
                    }
                }
            }
            netWorker.GenerateMessage10(numberOfChannel, speed, ts, mode);
        }
        private void BrowseFile_Click(object sender, EventArgs e)
        {
            textBox1.Text = mainWindow.ChooseSaveFolder();
            this.Activate();
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
        private void Back_Click(object sender, EventArgs e)
        {
            ResetNetwork();
        }
        private void Window_Closing(object sender, FormClosedEventArgs e)
        {
            ResetNetwork();
        }
        private void CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < RBs.Length; i++)
            {
                if (RBs[i].Checked)
                {
                    StateChanged(i, true);
                }
                else { StateChanged(i, false); }
            }
        }
        private void StateChanged(int numberOfElement, bool state)
        {
            if (contents[numberOfElement] is Label)
            {
                if (state == false) { ((Label)contents[numberOfElement]).Text = "Выключено"; }
                else { ((Label)contents[numberOfElement]).Text = "Включено"; }
            }
            RB_GBs[numberOfElement].Enabled = state;
        }
    }
}
