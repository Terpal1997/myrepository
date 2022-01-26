using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SodWinForms
{
    public partial class WritingToFile : Form
    {
        string pathOfCatalog = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\savedMessages\\";
        int numberOfChannel;
        string ipAdress;
        int ipPort;
        public MainWindow mainWindow;
        Corrupter corrupter;
        NetWorker netWorker;
        int ts;
        int speed;
        delegate void TakeAdress();
        delegate void emptyFunction();
        emptyFunction empty;
        private void MainWindowOnTop()
        {
            empty = mainWindow.OnTheTopScreen;
            Invoke(empty);
        }
        public WritingToFile(string ipAdressDevice, string portDevice)
        {
            InitializeComponent();
            ipAdress = ipAdressDevice;
            ipPort = Convert.ToInt32(portDevice);
            
            textBox1.Text = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\savedMessages\\" + DateTime.Now.ToString().Replace('.', '_').Replace(' ', '_').Replace(':', '_') + ".txt";
            DirectoryInfo dirinfo = new DirectoryInfo(pathOfCatalog);
            if(!dirinfo.Exists)
            {
                dirinfo.Create();
            }
        }
        private void ResetNetwork()
        {
            if (netWorker != null)
            {
                netWorker.TcpClose();
            }
            mainWindow.режимРаботыToolStripMenuItem.Enabled = true ;
            this.Close();        
        }
        private void Back_Click(object sender, EventArgs e)
        {
            ResetNetwork();
        }
        private void Start_Click(object sender, EventArgs e)
        {
            MainWindowOnTop();
            Start.Enabled = false;
            corrupter = new Corrupter(992, 0, 0);
            ts = TS_CB.SelectedIndex;
            speed = Speed_CB.SelectedIndex;
            if (netWorker != null)
            {
                netWorker.GenerateMessage10(numberOfChannel, speed, ts, 3);
            }
            else
            {
                numberOfChannel = (int)Math.Pow(2, Channel1_CB.SelectedIndex);
                netWorker = new NetWorker(ipAdress, ipPort, numberOfChannel, numberOfChannel, corrupter, (numberOfChannel & 1) + 1, (numberOfChannel & 1) + 1, new Corrupter(992, 0, 0), mainWindow);
                netWorker.channel1NoAnswer = true;
                netWorker.channel2NoAnswer = true;
                netWorker.channel1NoSave = false;
                netWorker.channel2NoSave = false;
                if (netWorker.TCPConnect(textBox1.Text))
                {
                    this.Text = this.Text + " (подключено)";
                    netWorker.GenerateMessage10(numberOfChannel, speed, ts, 3);
                }
                else
                {
                    netWorker = null;
                }
            }
        }
        private void ParametresOfCommand10Changed(object sender, EventArgs e)
        {
            if (netWorker != null)
            {
                numberOfChannel = (int)Math.Pow(2, Channel1_CB.SelectedIndex);
                ts = TS_CB.SelectedIndex;
                speed = Speed_CB.SelectedIndex;
                netWorker.GenerateMessage10(numberOfChannel, speed, ts, 3);
            }
        }
        private void Window_Closing(object sender, FormClosedEventArgs e)
        {
            ResetNetwork();
        }
        private void BrowseFile_Click(object sender, EventArgs e)
        {
            textBox1.Text= mainWindow.ChooseSaveFolder();
            this.Activate();
        }
    }
}
