using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SodWinForms
{
    public partial class MainWindow : Form
    {
        public string writePath;
        object[] ChannelsList;
        object[] TSList;
        object[] SpeedList;
        public static Thread WorkMode;
        bool autoScroll = true;
        delegate void AppendText(MessageForRTB messageForRTB);
        delegate void AppendDebugText(string debug);
        delegate void TakeAdress();
        WritingToFile writingToFile;
        TwoDevices twoDevice;
        OneDevice oneDevice;
        SendFromFile sendFromFile;
        static ConcurrentBag<MessageForRTB> messageQUEUE = new ConcurrentBag<MessageForRTB>();
        static List<MessageForRTB> messageList = new List<MessageForRTB>();
        IniFile INI = new IniFile("config.ini");
        public MainWindow()
        {
            
            SFD = new SaveFileDialog();
            SFD.Filter = "Text files(*.txt)|*.txt|ALL files(*.*)|*.*";
            OFD = new OpenFileDialog();
            OFD.Filter = "Text files(*.txt)|*.txt|ALL files(*.*)|*.*";
            InitializeComponent();
            ChannelsList = new object[2] { "1", "2" };
            TSList = new object[2] { "ТГ", "ФЛ" };
            SpeedList = new object[8] { "75", "150", "300", "1200", "2400", "4800", "9600", "16000" };
            Task.Factory.StartNew(() => LogShower());
            LoadParametres();
        }
        private void LoadParametres()
        {
            if (INI.KeyExists("Connect_Parametres", "Ip_Adress"))
            {
                ipAdressDevice.Text = INI.ReadINI("Connect_Parametres", "Ip_Adress");
                portDevice.Text = INI.ReadINI("Connect_Parametres", "Ip_Port");

            }
        }
        private void SaveParametres()
        {
            INI.Write("Connect_Parametres", "Ip_Adress",  ipAdressDevice.Text);
            INI.Write("Connect_Parametres", "Ip_Port",  portDevice.Text);
        }
        private void PortDevice_KeyPress(object sender, KeyPressEventArgs e)
        {
            OnlynumbersCheck(sender, e);
        }

        public void OnlynumbersCheck(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && (!char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
            }
        }
        private void IpAdressDevice_KeyPress(object sender, KeyPressEventArgs e)
        {
            //OnlynumbersAndDotesCheck(sender, e);
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar!='.')&&(!char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
            }
        }
        public void OnlynumbersAndDotesCheck(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != ',') && (e.KeyChar != '.') && (!char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
            }
        }
        private void ИскажениеИнформацииToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FullReset();
            string ipAdress = ipAdressDevice.Text;
            string portAdress = portDevice.Text;
            if (WorkMode == null || !WorkMode.IsAlive)
            {
                WorkMode = new Thread(() => OneDevice_Start(ipAdress, portAdress, ChannelsList, SpeedList, TSList));
                //WorkMode.SetApartmentState(ApartmentState.STA);
                WorkMode.Start();
            }
        }
        private void FullReset()
        {
            LogRTB.Clear();
            режимРаботыToolStripMenuItem.Enabled = false;
            writingToFile = null ;
             twoDevice = null;
             oneDevice = null;
             sendFromFile = null;
        }

        private void OneDevice_Start(string ipAdress, string portAdress, object[] ChannelsList, object[] SpeedList, object[] TSList)
        {
            oneDevice = new OneDevice(ipAdress, portAdress);
            try
            {
                oneDevice.Channel1_CB.Items.AddRange   (ChannelsList);
                oneDevice.Channel1_CB.SelectedIndex = 0;
                oneDevice.Speed_CB.Items.AddRange( SpeedList);
                oneDevice.Speed_CB.SelectedIndex = 5;
                oneDevice.TS_CB.Items.AddRange(TSList);
                oneDevice.TS_CB.SelectedIndex = 0;
                oneDevice.mainWindow = this;
                oneDevice.ShowDialog();
            }
            catch (InvalidOperationException ex)
            {
                //MessageBox.Show(ex.Message);
            }
            catch (Exception) {}
        }
        private void ИскажениеИнформацииВКвазидуплексномРежимеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FullReset();
            string ipAdress = ipAdressDevice.Text;
            string portAdress = portDevice.Text;
            if (WorkMode == null || !WorkMode.IsAlive)
            {
                WorkMode = new Thread(() => TwoDevices_Start(ipAdress, portAdress, ChannelsList, SpeedList, TSList));
                //WorkMode.SetApartmentState(ApartmentState.STA);
                WorkMode.Start();
            }
        }
        private void TwoDevices_Start(string ipAdress, string portAdress, object[] ChannelsList, object[] SpeedList, object[] TSList)
        {
             twoDevice = new TwoDevices(ipAdress, portAdress);
            try
            {
                twoDevice.Channel1_CB.Items.AddRange(ChannelsList);
                twoDevice.Channel1_CB.SelectedIndex = 0;
                twoDevice.Channel2_CB.Items.AddRange(ChannelsList);
                twoDevice.Channel2_CB.SelectedIndex = 1;
                twoDevice.Speed_CB.Items.AddRange(SpeedList);
                twoDevice.Speed_CB.SelectedIndex = 5;
                twoDevice.TS_CB.Items.AddRange(TSList);
                twoDevice.TS_CB.SelectedIndex = 0;
                twoDevice.mainWindow = this;
                twoDevice.ShowDialog();
            }
            catch (InvalidOperationException ex)
            {
                //MessageBox.Show(ex.Message);
            }
            catch (Exception) { }
        }
        private void WritingToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FullReset();
            string ipAdress = ipAdressDevice.Text;
            string portAdress = portDevice.Text;
           // SavingMessagesmode
            if (WorkMode == null || !WorkMode.IsAlive)
            {
                WorkMode = new Thread(() => WritingToFileStart(ipAdress, portAdress, ChannelsList, SpeedList, TSList));
                //WorkMode.SetApartmentState(ApartmentState.STA);
                WorkMode.Start();
            }
        }
        private void WritingToFileStart(string ipAdress, string portAdress, object[] ChannelsList, object[] SpeedList, object[] TSList)
        {
            
            writingToFile = new WritingToFile(ipAdress, portAdress);
            try
            {
                writingToFile.Channel1_CB.Items.AddRange(ChannelsList);
                writingToFile.Channel1_CB.SelectedIndex = 0;
                writingToFile.Speed_CB.Items.AddRange(SpeedList);
                writingToFile.Speed_CB.SelectedIndex = 5;
                writingToFile.TS_CB.Items.AddRange(TSList);
                writingToFile.TS_CB.SelectedIndex = 0;
                writingToFile.mainWindow = this;
                writingToFile.ShowDialog();
            }
            catch (InvalidOperationException ex)
            {
                //MessageBox.Show(ex.Message);
            }
            catch (Exception) {          }
        }
        private void SendFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FullReset();
            string ipAdress = ipAdressDevice.Text;
            string portAdress = portDevice.Text;
            // SavingMessagesmode
            if (WorkMode == null || !WorkMode.IsAlive)
            {
                WorkMode = new Thread(() => SendFromFileStart(ipAdress, portAdress, ChannelsList, SpeedList, TSList));
                //WorkMode.SetApartmentState(ApartmentState.STA);
                WorkMode.Start();
            }
        }
        private void SendFromFileStart(string ipAdress, string portAdress, object[] ChannelsList, object[] SpeedList, object[] TSList)
        {
            sendFromFile = new SendFromFile(ipAdress, portAdress);
            try
            {
                sendFromFile.Channel1_CB.Items.AddRange(ChannelsList);
                sendFromFile.Channel1_CB.SelectedIndex = 0;
                sendFromFile.Speed_CB.Items.AddRange(SpeedList);
                sendFromFile.Speed_CB.SelectedIndex = 5;
                sendFromFile.TS_CB.Items.AddRange(TSList);
                sendFromFile.TS_CB.SelectedIndex = 0;
                sendFromFile.mainWindow = this;
                sendFromFile.ShowDialog();
            }
            catch (InvalidOperationException ex)
            {
                //MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
             //   MessageBox.Show(ex.Message);
            }
        }
        public struct MessageForRTB
        {
            public string dialogInformation;
            //public int Channel;
            public string message;
            public Color color;
            public MessageForRTB(string dialogInformation, string message, Color color)
            {
                this.dialogInformation = dialogInformation;
                //this.Channel = channel;
                this.message = message;
                this.color = color;
                messageQUEUE.Add(this);
            }

        }
        private void LogShower()
        {
            MessageForRTB defaultmessage = new MessageForRTB();
            MessageForRTB meshub;
            while (true)
            {
                try
                {
                    if (messageQUEUE.TryTake(out meshub))
                    {
                       
                        messageList.Add(meshub);

                        if (autoScroll)
                        {
                            BeginInvoke(new AppendText(PrintToRTB), meshub);

                            //BeginInvoke(new AppengText(PrintToRTB), meshub);
                            if (messageList.Count >= 1000)
                            {
                                messageList.RemoveRange(0, messageList.Count-500);
                                LogRTB.Clear();

                                for (int i = 0; i < messageList.Count; i++)
                                {
                                    BeginInvoke(new AppendText(PrintToRTB), messageList[i]);
                                    //  BeginInvoke(new AppengText(PrintToRTB), messageList[i]);
                                }

                            }
                        }
                    }
                    else
                    {
                        if (messageQUEUE.Count > 0 && messageQUEUE.First().Equals(defaultmessage))
                        {
                            messageQUEUE.TryTake(out meshub);
                        }
                       
                    }
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    Thread.Sleep(1);
                }
            }
        }
        public void AddDebuggInformation(string message)
        {
            BeginInvoke(new AppendDebugText(PrintToRTB), message);
        }
        void PrintToRTB(MessageForRTB messageForRTB)
        {
       
            if (messageForRTB.dialogInformation != null)
            {
                    LogRTB.SelectionStart = LogRTB.TextLength;
                   // LogRTB.SelectionLength = 0;
                
                LogRTB.SelectionColor = messageForRTB.color;
                LogRTB.AppendText(Environment.NewLine+messageForRTB.dialogInformation);
                LogRTB.SelectionColor = LogRTB.ForeColor;
                LogRTB.AppendText(messageForRTB.message );
            }
        }
        void PrintToRTB(string message)
        {    
                LogRTB.AppendText(message +Environment.NewLine);           
        }

        public void AddMessageToList(string dialogInformation, string message, Color color)
        {
            new MessageForRTB(dialogInformation, message, color);
        }

        private void АвтопрокруткаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!autoScroll)
            {
                for (int i = 0; i < messageList.Count; i++)
                {
                    BeginInvoke(new AppendText(PrintToRTB), messageList[i]);
                    //  BeginInvoke(new AppengText(PrintToRTB), messageList[i]);
                }
            }
            autoScroll = !autoScroll;
            if(!autoScroll)
            {
                messageList.Clear();
            }
            автопрокруткаToolStripMenuItem.Checked = !автопрокруткаToolStripMenuItem.Checked;
        }
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (WorkMode != null && WorkMode.IsAlive)
                {
                    WorkMode.Abort();
                }
            }
            catch
            {
            }
            SaveParametres();
        }
        private void ClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogRTB.Clear();
        }

        public string ChooseSaveFolder()
        {
                BeginInvoke(new TakeAdress(TakeFolder));

            IsbrowserClosed = false;
            while (!IsbrowserClosed)
            {
                Thread.Sleep(10);
            }
            return adressOfFile;
        }
        private bool IsbrowserClosed= true;
        public string adressOfFile="";
        public void TakeFolder()
        {
            if (writingToFile != null)
            {
                SFD.Filter = "Text files(*.txt)|*.txt|ALL files(*.*)|*.*";
                if (SFD.ShowDialog() == DialogResult.OK)
                {
                    adressOfFile = SFD.FileName;
                }
                else
                {
                    adressOfFile = Environment.CurrentDirectory + "\\savedMessages\\" + DateTime.Now.ToString().Replace('.', '_').Replace(' ', '_').Replace(':', '_') + ".txt";

                }
            }
            else if (sendFromFile != null)
            {
                OFD.Filter = "Text files(*.txt)|*.txt|ALL files(*.*)|*.*";
                if (OFD.ShowDialog() == DialogResult.OK)
                {
                    adressOfFile = OFD.FileName;

                }
                else
                {
                    adressOfFile = "";
                }
            }
            

            IsbrowserClosed = true;
        }
        public void OnTheTopScreen()
        {
            this.BringToFront();
            //this.Hide();
            //this.Show();
        }

       
    }
}
