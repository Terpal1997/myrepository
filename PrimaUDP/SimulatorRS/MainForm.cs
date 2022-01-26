using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace SimulatorRS
{
    public partial class MainForm : Form
    {
        #region Stacks
        #region variable
        Stack<byte> stackFirstsBytes = new Stack<byte>();
        byte[][] stackREceiveBytes;
        #endregion
        #region WorkWithStack
        private void StackErrorSort()
        {
        }
        #endregion
        #endregion
        static public string FolderPath = Environment.CurrentDirectory;
        static string writePath = Environment.CurrentDirectory;  //+ filename;//@"\test.txt";
        WritingRecord wr = new WritingRecord();

        Settings_Form set = new Settings_Form();
        static RadiostationManager[] radistationManagerMas;
        delegate void AppendText(MessageForRTB messageForRTB);
        delegate string BringTextFromFormComponent(Object obj);
        delegate void LabelChanging(int channel);
        delegate void CutlinesFromRTB(int length);
        delegate void ChangeCBSelectedIndex(int newmode);
        ChangeCBSelectedIndex changeCBSelectedIndex;
        LabelChanging labelChanging;
        BringTextFromFormComponent DelegateTextReturn;
        functionUDPPrima function;//= new functionUDPPrima();
        CutlinesFromRTB cutlines;
        LogManager logManager;
        IniFile INI;//= new IniFile("config.ini");
        static CancellationTokenSource testStop = new CancellationTokenSource();
        #region variable
        public bool messageForRecord = false;
        byte firstByteOfreceiveMessage; //первый байт от каждого сообщения, если 0 , то ошибка
        bool stop = false;
        public bool messageOnScreen = true;
        delegate void NullFunction();
        int sendMessage = 0;
        int[] statesOfFullAnswers = new int[32];
        int[] numberOf0Messages = new int[32];
        Int16[] numberOfLastMessages = new Int16[32];

        byte[][] datasLegal = new byte[1000][];
        //int numberOfLostPackages = 0;
        int maxWeight;
        int maximumBytePerSecond;
        //Int16 NumberOfOnlinePorts = 0;
        bool sendOnce = false;
        bool ongoing = true;
        static List<MessageForRTB>[] messagesChannelsList;
        int[] tableOfRS;
        int mode = 0;
        //byte[] data, dataListener;
        bool[] Acces;
        //bool[] accepted;
        //Thread[] radiostationThreads;
        Thread[] radiostationThreads;
        List<int> numberOfPorts = new List<int>();
        List<int> numberOfPortsUU = new List<int>();
        List<int> numberOfRadistations = new List<int>();
        Int16[,] sendedmessageMas = new Int16[32, 1472];
        Int16[,] recievedmessageMas = new Int16[32, 1472];
        int[] lengthOfmessage = new int[32];
        int timeOfSleep;
        Label[] PovtorsChetchik;
        #endregion
        #region SocketsAndTask
        static CheckBox[] AccestoOurPorts;
        static TextBox[] OurPorts;
        static TextBox[] SpeedOfOurSocketRS;

        IPEndPoint remoteIp = null, remoteListenerIp = null;
        //Task listeningTask, listenerTask;
        #region WorkWithSockets
        public MainForm(string[] args)
        {
            InitializeComponent();
            labelChanging = insRepeats;
            changeCBSelectedIndex = ModeLogManagerCBHandChanger;
            this.MaximumSize = new System.Drawing.Size(3000, 3000);
            InitMas();
            modeLogManagerCB.Items.AddRange(new string[] { "Все радиостанции" });
            modeLogManagerCB.SelectedIndex = 0;
            toolStripErrCodeCB.SelectedIndex = 0;
            //LoadParametres();

            if (args.Length > 0)
            {
                for (int i = 0; i < AccestoOurPorts.Length; i++)
                {
                    AccestoOurPorts[i].Checked = false;
                }
                for (int i = 0; i < Convert.ToInt32(args[0]); i++)
                {
                    // MessageBox.Show(args[i + 1]);
                    OurPorts[2 * i].Text = args[i + 1];
                    AccestoOurPorts[i].Checked = true;
                }
            }

            AcceptButton = sendBtn;
            //myPortTB.Select();
            DirectoryInfo dirInfo = new DirectoryInfo(writePath);
            if (!dirInfo.Exists)//!Directory.Exists(Environment.CurrentDirectory + "\\SavedMessages"))
            {
                dirInfo.Create();
                //Directory.CreateDirectory(Environment.CurrentDirectory + "\\SavedMessages");
            }
            Task.Factory.StartNew(() => LogShower());
            //LogShower();
        }
        private void InitMas()
        {
            OurPorts = new TextBox[64] {
                        set.Channel1PortTB,set.Channel1UUTB, set.Channel2PortTB,set.Channel2UUTB, set.Channel3PortTB,set.Channel3UUTB, set.Channel4PortTB,set.Channel4UUTB,
                        set.Channel5PortTB,set.Channel5UUTB, set.Channel6PortTB,set.Channel6UUTB, set.Channel7PortTB,set.Channel7UUTB, set.Channel8PortTB,set.Channel8UUTB,
                    set.Channel9PortTB,set.Channel9UUTB, set.Channel10PortTB,set.Channel10UUTB, set.Channel11PortTB,set.Channel11UUTB, set.Channel12PortTB,set.Channel12UUTB,
                    set.Channel13PortTB,set.Channel13UUTB, set.Channel14PortTB,set.Channel14UUTB, set.Channel15PortTB,set.Channel15UUTB, set.Channel16PortTB,set.Channel16UUTB,
                    set.Channel17PortTB,set.Channel17UUTB, set.Channel18PortTB,set.Channel18UUTB, set.Channel19PortTB,set.Channel19UUTB, set.Channel20PortTB,set.Channel20UUTB,
                    set.Channel21PortTB,set.Channel21UUTB, set.Channel22PortTB,set.Channel22UUTB, set.Channel23PortTB,set.Channel23UUTB, set.Channel24PortTB,set.Channel24UUTB,
                    set.Channel25PortTB,set.Channel25UUTB, set.Channel26PortTB,set.Channel26UUTB, set.Channel27PortTB,set.Channel27UUTB, set.Channel28PortTB,set.Channel28UUTB,
                    set.Channel29PortTB,set.Channel29UUTB, set.Channel30PortTB,set.Channel30UUTB, set.Channel31PortTB,set.Channel31UUTB, set.Channel32PortTB,set.Channel32UUTB };
            AccestoOurPorts = new CheckBox[32] { Channel1PortCB, Channel2PortCB, Channel3PortCB, Channel4PortCB, Channel5PortCB, Channel6PortCB, Channel7PortCB, Channel8PortCB,
                                    Channel9PortCB, Channel10PortCB, Channel11PortCB, Channel12PortCB, Channel13PortCB, Channel14PortCB, Channel15PortCB,Channel16PortCB,
                    Channel17PortCB, Channel18PortCB, Channel19PortCB, Channel20PortCB, Channel21PortCB, Channel22PortCB, Channel23PortCB, Channel24PortCB,
                    Channel25PortCB, Channel26PortCB, Channel27PortCB, Channel28PortCB, Channel29PortCB, Channel30PortCB, Channel31PortCB, Channel32PortCB};
            PovtorsChetchik = new Label[32]{Povtor_1,Povtor_2,Povtor_3,Povtor_4,Povtor_5,Povtor_6,Povtor_7,Povtor_8,Povtor_9,Povtor_10,Povtor_11,Povtor_12,
                Povtor_13,Povtor_14,Povtor_15,Povtor_16,Povtor_17,Povtor_18,Povtor_19,Povtor_20,Povtor_21,Povtor_22,Povtor_23,Povtor_24,Povtor_25,Povtor_26,Povtor_27,Povtor_28,
                Povtor_29,Povtor_30,Povtor_31,Povtor_32};
            SpeedOfOurSocketRS = new TextBox[32] {
                set.Channel1Speed,set.Channel2Speed,set.Channel3Speed,set.Channel4Speed,
                set.Channel5Speed,set.Channel6Speed,set.Channel7Speed,set.Channel8Speed,
                set.Channel9Speed,set.Channel10Speed,set.Channel11Speed,set.Channel12Speed,
                set.Channel13Speed,set.Channel14Speed,set.Channel15Speed,set.Channel16Speed,
                set.Channel17Speed,set.Channel18Speed,set.Channel19Speed,set.Channel20Speed,
                set.Channel21Speed,set.Channel22Speed,set.Channel23Speed,set.Channel24Speed,
                set.Channel25Speed,set.Channel26Speed,set.Channel27Speed,set.Channel28Speed,
                set.Channel29Speed,set.Channel30Speed,set.Channel31Speed,set.Channel32Speed};
            ///
            /////////////////////////////////////////////////////////////////////////////////////
            foreach(TextBox tb in SpeedOfOurSocketRS)
            {
                tb.Enabled = false;
            }
        }
        private void LoadConfig()
        {

            if (INI.KeyExists("COMMAND_16", "Accept_Adresses_1"))
            {
                for (int i = 0; i < 32; i++)
                {
                    AccestoOurPorts[i].Checked = bool.Parse(INI.ReadINI("COMMAND_16", "Accept_Adresses_" + Convert.ToString(i + 1)));
                    OurPorts[2 * i].Text = INI.ReadINI("COMMAND_16", "Rad_P_" + Convert.ToString(i + 1));
                    OurPorts[2 * i + 1].Text = INI.ReadINI("COMMAND_16", "UU_P_" + Convert.ToString(i + 1));
                   // SpeedOfOurSocketRS[i].Text = INI.ReadINI("COMMAND_16", "Speed_" + Convert.ToString(i + 1));

                }
            }
            if (INI.KeyExists("UDP_Settings", "PrimaAdress"))
            {
                addressTB.Text = INI.ReadINI("UDP_Settings", "PrimaAdress");
                portTB.Text = INI.ReadINI("UDP_Settings", "PrimaPort");
                maximumOfPackage.Text = INI.ReadINI("UDP_Settings", "maximumOfPackage");
                weightOfPackage.Text = INI.ReadINI("UDP_Settings", "weightOfPackage");
                sleepTime.Text = INI.ReadINI("UDP_Settings", "sleepTime");
                ListenerTypeFullAnswer.Checked = bool.Parse(INI.ReadINI("UDP_Settings", "ListenerTypeFullAnswer"));
                ListenerTypeNoAnswer.Checked = bool.Parse(INI.ReadINI("UDP_Settings", "ListenerTypeNoAnswer"));
            }
        }
        private void testBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addressTB.Text = "192.168.1.87";
            portTB.Text = "8001";
        }
        private void comp2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addressTB.Text = "127.0.0.1";
            portTB.Text = "8888";
        }
        private void iPSwitchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addressTB.Text = "192.168.1.225";
        }
        private void iPcomp1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addressTB.Text = "192.168.1.90";
        }
        #endregion
        #endregion
        #region WorkWithForm
        private void очиститьОкноЧатаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ongoing = false;
            СhatRTB.Clear();
            foreach (List<MessageForRTB> list in messagesChannelsList)
            {
                list.Clear();
            }
            messageList.Clear();
            ongoing = true;
        }
        private void закрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        //private void fileTransmitBtn_Click(object sender, EventArgs e)
        //{
        //    //FileTransmit fileTransmit = new FileTransmit();
        //    //fileTransmit.Show();
        //}
        #region PrintOnForm

        public void PrintToRTBWithDelegate(string dialogInformation, string message, Color color)
        {
            //if (_main.IsHandleCreated)
            //      BeginInvoke(new AppengText(PrintToRTB), dialogInformation, message, color);
        }

        public void ChangeNumberOfRepeats(int channel)
        {
            BeginInvoke(labelChanging, channel);
            //PovtorsChetchik[channel].Text = Convert.ToString(Convert.ToInt16(PovtorsChetchik[channel].Text) + 1);
        }
        public void insRepeats(int channel)
        {
            PovtorsChetchik[channel].Text = Convert.ToString(Convert.ToInt16(PovtorsChetchik[channel].Text) + 1);
        }
        private void CleansRepeats()
        {
            foreach (Label label in PovtorsChetchik)
            {
                label.Text = "0";
            }
        }

        #endregion
        private void setPortBtn_Click(object sender, EventArgs e)
        {

            //try
            //{
            CleansRepeats();
            if ((startExchanging != null) && ((startExchanging.ThreadState != System.Threading.ThreadState.Unstarted) || (startExchanging.ThreadState != System.Threading.ThreadState.Stopped) || (startExchanging.ThreadState != System.Threading.ThreadState.AbortRequested)))
            {
                startExchanging.Abort();
            }
            numberOfPorts.Clear();
            numberOfPortsUU.Clear();
            numberOfRadistations.Clear();
            int[] tableOfcorrespondece = new int[32];
            int j = 0;
            for (int i = 0; i < AccestoOurPorts.Length; i++)
            {
                if (AccestoOurPorts[i].Checked)
                {
                    numberOfPorts.Add(Convert.ToInt16(OurPorts[2 * i].Text));
                    numberOfPortsUU.Add(Convert.ToInt16(OurPorts[2 * i+1].Text));
                    numberOfRadistations.Add(i);
                    tableOfcorrespondece[j] = i;
                    j++;
                }
            }
            //проверка на повторяющиеся порты
            for (int i = 0; i < numberOfPorts.Count; i++)
            {
                for (int k = i+1; k < numberOfPortsUU.Count; k++)
                {
                    if (numberOfPorts[i]==numberOfPortsUU[k])
                    {
                        MessageBox.Show("Обнаружены совпадающиеся порты независимых радиостанции и УУ");
                        startListener.Enabled = false;
                        return;
                    }
                    if (numberOfPorts[i] == numberOfPorts[k])
                    {
                        MessageBox.Show("Обнаружены совпадающиеся порты радиостанций");
                        startListener.Enabled = false;
                        return;
                    }
                    if (numberOfPortsUU[i] == numberOfPortsUU[k])
                    {
                        MessageBox.Show("Обнаружены совпадающиеся порты УУ");
                        startListener.Enabled = false;
                        return;
                    }
                }
            }
            while (modeLogManagerCB.Items.Count > 1)
            {
                modeLogManagerCB.Items.RemoveAt(1);
            }
            while ( toolStripSelectedChannelCB.Items.Count > 0)
            {
                toolStripSelectedChannelCB.Items.RemoveAt(0);
            }
            //radiostationThreads = new Thread[numberOfPorts.Count];
            radiostationThreads = new Thread[numberOfPorts.Count];
            radistationManagerMas = new RadiostationManager[numberOfPorts.Count];
            logManager = new LogManager(СhatRTB, this);
            int rsLength = radiostationThreads.Length;
            messagesChannelsList = new List<MessageForRTB>[rsLength];
            tableOfRS = new int[rsLength];
            RadiostationManager._ipAdressPrima = addressTB.Text;
            RadiostationManager._portPrima = int.Parse(portTB.Text);
            for (int i = 0; i < rsLength; i++)
            {

                radistationManagerMas[i] = new RadiostationManager(numberOfPorts[i], numberOfPortsUU[i],
                 numberOfRadistations[i],  this, logManager);
                modeLogManagerCB.Items.Add(numberOfRadistations[i] + 1);
                toolStripSelectedChannelCB.Items.Add(numberOfRadistations[i] + 1);
                tableOfRS[i] = numberOfRadistations[i] + 1;
                messagesChannelsList[i] = new List<MessageForRTB>();

            }

            for (int i = 0; i < rsLength; i++)
            {
                radiostationThreads[i] = new Thread(() => radistationManagerMas[i].Listen());
                radiostationThreads[i].Priority = ThreadPriority.Highest;
                //Thread.Sleep(20);
            }
            messageQUEUE = new ConcurrentQueue<MessageForRTB>();
            toolStripSelectedChannelCB.SelectedIndex = 0;
            //RadiostationManager.sendingQueue = new  ConcurrentQueue <RadiostationManager.Socket_Message>();
            //Thread sendingThread = new Thread(() => RadiostationManager.SendMessageThread());
            RadiostationManager.reset();
            //sendingThread.Start();
            СhatRTB.Clear();

            Acces = new bool[AccestoOurPorts.Length];

            for (int i = 0; i < AccestoOurPorts.Length; i++)
            {
                //Acces[i] = Convert.ToBoolean(AccestoOurPorts[i]);
                Acces[i] = Convert.ToBoolean(AccestoOurPorts[i].CheckState);
            }
            if (radistationManagerMas != null)
            {
                if (RazmerActivaciiRaven1.Checked)
                {
                    for (int i = 0; i < radistationManagerMas.Length/* radiostationThreads.Length*/; i++)
                    {
                        radistationManagerMas[i].maxCountOfZeroMessages = 1;
                    }
                }
                else
                {
                    for (int i = 0; i < radistationManagerMas.Length/* radiostationThreads.Length*/; i++)
                    {
                        radistationManagerMas[i].maxCountOfZeroMessages = 2;
                    }
                }
            }
            toolStripMenuItemSendMessage.Enabled = false;
            sendBtn.Enabled = true;
            sendOnceBtn.Enabled = true;
            startListener.Enabled = true;
            EnabledElement(true);
            setBtn.Enabled = false;
            //}
            //catch (Exception ex)
            //{

            //          MessageBox.Show(ex.Message);
            //}
        }
        private void sendBtn_Click(object sender, EventArgs e)
        {
            RadiostationManager.stop = false;
            sendOnce = false;
            lostPackagesBox.Text = "0";
            startExchanging = new Thread(startExchange);
            startExchanging.Start();
            setBtn.Enabled = false;
            sendBtn.Enabled = false;
            waitButton.Enabled = true;
            stopBtn.Enabled = true;
            startListener.Enabled = false;
            // startExchanging.
            //EnabledElement(true);
        }
        private void EnabledElement(bool state)
        {
            //Включение/выключение настроек 
            ListenerTypeGB.Enabled = state;
            //addressTB.Enabled = state;
            //portTB.Enabled = state;
            //maximumOfPackage.Enabled = state;
            //weightOfPackage.Enabled = state;
            //sleepTime.Enabled = state;
        }
        private void EnabledButton()
        {
            //Включение/выключение настроек 
            setBtn.Enabled = true;
            sendBtn.Enabled = true;
            waitButton.Enabled = false;
            stopBtn.Enabled = false;
        }
        private void stopBtn_Click(object sender, EventArgs e)
        {
            setBtn.Enabled = true;
            //sendBtn.Enabled = true;
            waitButton.Enabled = false;
            stopBtn.Enabled = false;
            EnabledElement(false);
            RadiostationManager.stop = true;
            for (int i = 0; i < radistationManagerMas.Length; i++)
            {
                radistationManagerMas[i].StopListener();
                 }

            startListener.Enabled = true;
            toolStripMenuItemSendMessage.Enabled = false;

        }
        private void WaitButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (startExchanging.ThreadState == System.Threading.ThreadState.WaitSleepJoin)
                {
                    startExchanging.Suspend();
                }
                else
                {
                    startExchanging.Resume();
                }
            }
            catch
            {
                //               MessageBox.Show(ex.Message);
            }
        }

        //}
        private void startListenerTB_Click(object sender, EventArgs e)
        {
            {
                infoBox.Text = "Принято сообщений";
                RadiostationManager.noise = Noise.Checked;
                
                int radiostationThreadsLength = radiostationThreads.Length;
                for (int i = 0; i < radiostationThreadsLength/* radiostationThreads.Length*/; i++)
                {

                    radistationManagerMas[i].noiseWithNulls = NoisesWithNulls.Checked;
                    radiostationThreads[i] = new Thread(() => radistationManagerMas[i].Listen());
                    radiostationThreads[i].Priority = ThreadPriority.Highest;
                    radiostationThreads[i].Start();
                    Thread.Sleep(20);
                    if (i == radiostationThreadsLength - 1)
                    {
                        break;
                    }

                }

                ushort sdvigNomerov = Convert.ToUInt16((65536 - 1) / radistationManagerMas.Length);
                ushort startNomerov = 0;
                for (int i = 0; i < radiostationThreadsLength/* radiostationThreads.Length*/; i++)
                {

                    radistationManagerMas[i].function.messagenumber = startNomerov;
                    startNomerov = Convert.ToUInt16(Convert.ToInt32(startNomerov) + sdvigNomerov);
                }
                setBtn.Enabled = false;
                startListener.Enabled = false;
                sendOnceBtn.Enabled = false;
                sendBtn.Enabled = false;
                setBtn.Enabled = false;
                stopBtn.Enabled = true;
                toolStripMenuItemSendMessage.Enabled = true;
            }
            // 


            //startListenerTB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;


        }

        #endregion
        #region delegate
        delegate void NumberToRepeat(int j);

        delegate void EnableButton(Button btn);
        delegate void EnableElement(bool state);
        delegate void increaseValueOfBox<T>(T box);
        increaseValueOfBox<Label> IVoB = IncreaseValueOfBox;
        //    delegate void Refresh();

        #endregion
        #region Threads
        Thread startExchanging;
        #endregion
        #region exchanging
        private void Init()
        {
            Region regionrefresh = new Region(new Rectangle(0, 0, 666, 548));
            stackREceiveBytes = null;
            firstByteOfreceiveMessage = 0;
            maximumBytePerSecond = Convert.ToInt32(maximumOfPackage.Text) / 8; //максимальное количество пакетов
            maxWeight = Convert.ToInt32(weightOfPackage.Text); //максимальное количество данных в пакете
            timeOfSleep = Convert.ToInt32(sleepTime.Text); //задержка
            stackREceiveBytes = new byte[256][];
            sendMessage = 0;

            //else if (listeningTask.Status == TaskStatus.Running)
            //{
            //    listeningTask.

            //}
            Task[] tasks = new Task[8];
            //  RadiostationManager(string ipAdressPrima,string portPrima ,int port, int suspendTime , MainForm mainForm , LogManager logManager)
            // addressTB
            RadiostationManager[] radiostationManager = new RadiostationManager[8];
            //int[] ports = new int[8] { 8001, 8002, 8003, 8004, 8005, 8006, 8007, 8008 };
            //for (int i = 0; i < tasks.Length; i++)
            //{

            //    radiostationManager[i] = new RadiostationManager(addressTB.Text, portTB.Text, ports[i],numberOfRadistations[i], 500, this, logManager);
            //}
            //for (int i = 0; i < tasks.Length; i++)
            //{

            //    tasks[i] = new Task(() => radiostationManager[i].Listen());
            //}

            stackFirstsBytes.Clear();
        }

        private void startExchange()
        {

            Init();
            //StringBuilder sb;
            //infoBox.Text = "Повторы";
            Int32[] currentBytePerSecond = new Int32[Acces.Length];
            Int16[] chetchikpovtorov = new Int16[Acces.Length];
            bool[] sendBlock = new bool[Acces.Length];
            //byte[] SendedMessage;
            //bool push = true;
            BeginInvoke(new EnableElement(EnabledElement), true);

        }
        private void startExchange(byte[] message, int lengthOfPart)
        {
            Init();
            //StringBuilder sb;
            //infoBox.Text = "Повторы";
            Int32[] currentBytePerSecond = new Int32[Acces.Length];
            Int16[] chetchikpovtorov = new Int16[Acces.Length];
            bool[] sendBlock = new bool[Acces.Length];
            //byte[] SendedMessage;
            //bool push = true;
            BeginInvoke(new EnableElement(EnabledElement), true);
            int parts = 0;
            if (message.Length % (lengthOfPart - 7) > 0)
            {
                parts = (message.Length / (lengthOfPart - 7)) + 1;
            }
            else if (message.Length % (lengthOfPart - 7) == 0)
            {
                parts = message.Length / (lengthOfPart - 7);
            }
            int lengthofmessage = lengthOfPart - 7;
            byte[] newMessage;
            byte[][] dividedmessage = new byte[parts][];
            int m = 0;
            for (int j = 0; j < parts; j++)
            {

                if (m + lengthofmessage <= message.Length)
                {
                    newMessage = new byte[lengthofmessage];
                }
                else
                {
                    lengthofmessage = message.Length - m;
                    newMessage = new byte[lengthofmessage];
                }
                for (int k = 0; k < lengthofmessage; k++)
                {
                    newMessage[k] = message[m];
                    m++;
                }
                newMessage = function.generatingMessage0(j, 1, newMessage);
                dividedmessage[j] = newMessage;
            }
        }

        //private void izluchenie(bool state)
        //{
        //    byte[] izluchenieData = functionUDPPrima.generatingMessage2(state);
        //    while (true)
        //    {
        //        tokenTestStop.ThrowIfCancellationRequested();
        //        udpSocket1.Send(data, data.Length, addressTB.Text, Int32.Parse(portTB.Text));
        //        BeginInvoke(new TextToChat(ReturnFromNetToListenerRTB), "Отправлены сведения об излучении");
        //        Thread.Sleep(1000);
        //    }
        //}
        #endregion
        public void PerformClickBTN(Button btn)
        {
            btn.PerformClick();
        }
        public static void IncreaseValueOfBox<T>(T box) where T : Label
        {
            int num;
            bool success = Int32.TryParse(box.Text, out num);
            if (success)
            {
                box.Text = Convert.ToString(num + 1);
            }
            else
            {
                MessageBox.Show("Не удалось инкриментировать значение {0} ", box.ToString());
            }
        }
        private string returnSelectedText(Object obj)
        {
            if (obj is TextBox)
            {
                return ((TextBox)obj).Text;
            }
            if (obj is RichTextBox)
            {
                return ((RichTextBox)obj).Text;
            }
            if (obj is DomainUpDown)
            {
                return ((DomainUpDown)obj).Text;
            }
            if (obj is CheckBox)
            {
                return ((CheckBox)obj).Checked.ToString();
            }
            if (obj is RadioButton)
            {
                return ((RadioButton)obj).Checked.ToString();
            }
            if (obj is ComboBox)
            {
                return ((ComboBox)obj).SelectedIndex.ToString();
            }
            if (obj is Label)
            {
                return ((Label)obj).Text;
            }
            MessageBox.Show("Незнакомый тип объекта для функции делегата");
            return null;
        }
        public void recordToFile(string str)
        {
            StreamWriter ws;
            Console.WriteLine(writePath);
            ws = new StreamWriter(writePath, false);
            ws.WriteLine(str);
            ws.Close();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            INI = new IniFile(Environment.CurrentDirectory + "\\config.ini");
            LoadConfig();
        }
        private void SendToFile(string s)
        {
            File.AppendAllText(@"C:\Users\user\Desktop\log.txt", s + Environment.NewLine);
        }
        private void PortsShowBtn_Click(object sender, EventArgs e)
        {
            set.Show();
        }
        private void SavedParametres(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => SaveConfig());
        }
        private void SaveConfig()
        {
            DelegateTextReturn = returnSelectedText;
            for (int i = 0; i < 32; i++)
            {
                INI.Write("COMMAND_16", "Accept_Adresses_" + Convert.ToString(i + 1), (string)Invoke(DelegateTextReturn, AccestoOurPorts[i]));
            }
            for (int i = 0; i < 32; i++)
            {
                INI.Write("COMMAND_16", "Rad_P_" + Convert.ToString(i + 1), (string)Invoke(DelegateTextReturn, OurPorts[2 * i]));
            }
            for (int i = 0; i < 32; i++)
            {
                INI.Write("COMMAND_16", "UU_P_" + Convert.ToString(i + 1), (string)Invoke(DelegateTextReturn, OurPorts[2 * i + 1]));
            }
            //for (int i = 0; i < 32; i++)
            //{
            //    INI.Write("COMMAND_16", "Speed_" + Convert.ToString(i + 1), (string)Invoke(DelegateTextReturn, SpeedOfOurSocketRS[i]));
            //}


            INI.Write("UDP_Settings", "PrimaAdress", (string)Invoke(DelegateTextReturn, addressTB));
            INI.Write("UDP_Settings", "PrimaPort", (string)Invoke(DelegateTextReturn, portTB));
            INI.Write("UDP_Settings", "maximumOfPackage", (string)Invoke(DelegateTextReturn, maximumOfPackage));
            INI.Write("UDP_Settings", "weightOfPackage", (string)Invoke(DelegateTextReturn, weightOfPackage));
            INI.Write("UDP_Settings", "sleepTime", (string)Invoke(DelegateTextReturn, sleepTime));

            INI.Write("UDP_Settings", "ListenerTypeFullAnswer", (string)Invoke(DelegateTextReturn, ListenerTypeFullAnswer));
            INI.Write("UDP_Settings", "ListenerTypeNoAnswer", (string)Invoke(DelegateTextReturn, ListenerTypeNoAnswer));
        }
        private void MessageToChatBtn_CheckedChanged(object sender, EventArgs e)
        {
            messageOnScreen = MessageToChatBtn.Checked;
        }
        private void sendOnceBtn_Click(object sender, EventArgs e)
        {
            RadiostationManager.stop = false;
            for (int i = 0; i < radiostationThreads.Length/* radiostationThreads.Length*/; i++)
            {
                radiostationThreads[i] = new Thread(() => radistationManagerMas[i].Listen());
                radiostationThreads[i].Priority = ThreadPriority.Highest;
                radiostationThreads[i].Start();


                Thread.Sleep(20);

            }
            Thread.Sleep(100);
            for (int i = 0; i < radiostationThreads.Length/* radiostationThreads.Length*/; i++)

            {
                radistationManagerMas[i].SendOneMessage();

            }
        }
        private void RecordNextMessageBtn_Click(object sender, EventArgs e)
        {
            if (RecordNextMessageBtn.FlatStyle == System.Windows.Forms.FlatStyle.Flat)
            {
                RecordNextMessageBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
                RecordMessageGB.Visible = false;
                messageForRecord = false;
            }
            else
            {
                RecordNextMessageBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                messageForRecord = true;
                RecordMessageGB.Visible = true;
            }
        }
        private void OpenFileButton_Click(object sender, EventArgs e)
        {
            if (OFD.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            FilePath.Text = writePath = OFD.FileName;

        }
        private void ChooseFolderButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FDB = new FolderBrowserDialog();
            FDB.SelectedPath = Environment.CurrentDirectory;
            if (FDB.ShowDialog() == DialogResult.OK)
            {
                FolderPath = FDB.SelectedPath;
                FilePath.Text = FDB.SelectedPath;
            }
        }
        private void SendFileButton_Click(object sender, EventArgs e)
        {
            lostPackagesBox.Text = "0";
            sendOnce = true;
            byte[] ReadMessage;
            using (FileStream fstream = new FileStream(writePath, FileMode.Open))
            {
                ReadMessage = new byte[fstream.Length];
                fstream.Read(ReadMessage, 0, ReadMessage.Length);
            }
            for (int i = 0; i < radiostationThreads.Length/* radiostationThreads.Length*/; i++)

            {
                radistationManagerMas[i].SendOneMessage(ReadMessage);

            }
        }
        private void записьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RecordNextMessageBtn.Visible = true;
            ChooseFolderButton.Visible = true;
            RecordMessageGB.Visible = true;
            SendFileButton.Visible = false;
            OpenFileButton.Visible = false;
            LengthofsendPackage.Visible = false;
            FilePath.Visible = true;
        }
        private void отправитьФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OFD.Filter = "Text files(*.txt)|*.txt|ALL files(*.*)|*.*";
            RecordNextMessageBtn.Visible = false;
            ChooseFolderButton.Visible = false;
            RecordMessageGB.Visible = false;
            SendFileButton.Visible = true;
            OpenFileButton.Visible = true;
            LengthofsendPackage.Visible = true;
            FilePath.Visible = true;
        }
        private void Sach1Chaika_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8)
            {
                e.Handled = true;
            }
        }
        private void ToolStripMenuItemLoadConfiguration_Click(object sender, EventArgs e)
        {
            OFD.Filter = "INI files(*.ini)|*.ini|ALL files(*.*)|*.*";
            if (OFD.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            INI = new IniFile(OFD.FileName);
            LoadConfig();
        }
        private void ToolStripMenuItemSaveConfiguration_Click(object sender, EventArgs e)
        {
            SFD.Filter = "INI files(*.ini)|*.ini|ALL files(*.*)|*.*";
            SFD.OverwritePrompt = false;
            if (SFD.ShowDialog() == DialogResult.Cancel)
            {
                return;

            }
            INI = new IniFile(SFD.FileName);
            SaveConfig();
        }
        private void ListenerTypeFullAnswer_CheckedChanged(object sender, EventArgs e)
        {
            foreach (RadiostationManager radiostationManager in radistationManagerMas)
            {
                radiostationManager.ChangeListenerType(ListenerTypeFullAnswer.Checked);
            }
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            stop = true;
            RadiostationManager.stop = true;
            try
            {
                foreach (RadiostationManager radiostation in radistationManagerMas)
                {
                    radiostation.StopListener();

                }
            }
            catch
            {

            }
            
            
        }

        private void DotesEnableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chckbox = sender as CheckBox;
            if (radistationManagerMas != null)
                foreach (RadiostationManager radiostationManager in radistationManagerMas)
                {
                    radiostationManager.DotesState(chckbox.Checked);
                }
        }
        public struct MessageForRTB
        {
            public string dialogInformation;
            public int channel;
            public string message;
            public Color color;
            public MessageForRTB(string dialogInformation, int channel, string message, Color color)
            {
                this.dialogInformation = dialogInformation;
                this.channel = channel;
                this.message = message;
                this.color = color;

                messageQUEUE.Enqueue(this);
            }

        }

        private void RazmerActivaciiRaven1_CheckedChanged(object sender, EventArgs e)
        {
            if (radistationManagerMas != null)
            {
                if (RazmerActivaciiRaven1.Checked)
                {

                    for (int i = 0; i < radistationManagerMas.Length/* radiostationThreads.Length*/; i++)
                    {

                        radistationManagerMas[i].maxCountOfZeroMessages = 1;

                    }

                }
                else
                {
                    for (int i = 0; i < radistationManagerMas.Length/* radiostationThreads.Length*/; i++)
                    {

                        radistationManagerMas[i].maxCountOfZeroMessages = 2;

                    }
                }
            }
        }
        static ConcurrentQueue<MessageForRTB> messageQUEUE = new ConcurrentQueue<MessageForRTB>();
        static List<MessageForRTB> messageList = new List<MessageForRTB>();
        private void ResizeRTB(int size)
        {

                if (СhatRTB.Lines.Length > size)
                {
                    int countOfChars = 0;
                    for (int i = 0; i < СhatRTB.Lines.Length - size; i++)
                    {
                        countOfChars += СhatRTB.Lines[i].Length;
                    }
                    СhatRTB.Text = СhatRTB.Text.Remove(СhatRTB.GetFirstCharIndexFromLine(0), countOfChars + 1);
                }
                
            
            
        }
        private void TaskForResizeRTB(int size)
        {
            cutlines = ResizeRTB;
            while (!stop)
        {
            BeginInvoke(cutlines, MaxNumberOfLines);
            Thread.Sleep(1);
        }
        }
        public bool offScreen = true;
        private void LogShower()
        {
           
             MessageForRTB defaultmessage = new MessageForRTB();
            MessageForRTB meshub;

           //Task.Factory.StartNew(()=> TaskForResizeRTB( MaxNumberOfLines));
            while (!stop)
            {
                if (ongoing)
                {
                    
                    try
                    {

                        if (messageQUEUE.TryDequeue(out meshub))
                        {
                            messageList.Add(meshub);
                            for (int j = 0; j < tableOfRS.Length; j++)
                            {
                                if (tableOfRS[j] == meshub.channel)
                                {
                                    messagesChannelsList[j].Add(meshub);
                                    if (mode - 1 == j && !offScreen)
                                    {
                                        BeginInvoke(new AppendText(PrintToRTB), meshub);
                                    }
                                    if (messagesChannelsList[j].Count >= 80)
                                    {
                                        messagesChannelsList[j].RemoveRange(0, 40);
                                        //if (mode - 1 == j)
                                        //{
                                        //    Invoke(new NullFunction(ClearRTBToolStripMenuItem.PerformClick));
                                        //    for (int i = 0; i < messagesChannelsList[j].Count; i++)
                                        //    {
                                        //        BeginInvoke(new AppendText(PrintToRTB), messagesChannelsList[j][i]);
                                        //    }
                                        //}
                                    }
                                    break;
                                }
                                
                            }
                            if (mode == 0 && !offScreen)
                            {
                                BeginInvoke(new AppendText(PrintToRTB), meshub);
                            }
                            if (messageList.Count >= radistationManagerMas.Length * 80)
                            {
                                messageList.RemoveRange(0, radistationManagerMas.Length * 40);
                                //if (mode == 0)
                                //{
                                //    Invoke(new NullFunction(ClearRTBToolStripMenuItem.PerformClick));
                                //    for (int i = 0; i < messageList.Count; i++)
                                //    {
                                //        BeginInvoke(new AppendText(PrintToRTB), messageList[i]);
                                //    }
                                //}
                            }
                        }
                        else
                        {
                            if (messageQUEUE.Count > 0 && messageQUEUE.First().Equals(defaultmessage))
                            {
                                messageQUEUE.TryDequeue(out meshub);
                            }
                            else
                            {
                                Thread.Sleep(1);
                                continue;
                            }
                        }


                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.ToString());
                    }
                }
                //Thread.Sleep(1);
            }
        }
        public void ModeLogManagerCBHandChanger(int newmode)
        {
            modeLogManagerCB.SelectedIndex = newmode;
        }
        public void ModeLogManagerCBHandChangerMaster(int newmode)
        {
            if (newmode != 0)
            {
                for (int i = 0; i < tableOfRS.Length; i++)
                {
                    if (tableOfRS[i] == newmode)
                    {
                        BeginInvoke(changeCBSelectedIndex, i+1);
                    }
                }
            }
            else
            {
                BeginInvoke(changeCBSelectedIndex, newmode);
            }
        }
        private void ModeLogManagerCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            ongoing = false;
            Thread.Sleep(20);
            СhatRTB.Clear();
            if (modeLogManagerCB.SelectedIndex != 0)
            {
                foreach (MessageForRTB mes in messagesChannelsList[modeLogManagerCB.SelectedIndex - 1])
                {
                    BeginInvoke(new AppendText(PrintToRTB), mes);
                }
            }
            else
            {
                foreach (MessageForRTB mes in messageList)
                {
                    BeginInvoke(new AppendText(PrintToRTB), mes);
                }
            }
            mode = modeLogManagerCB.SelectedIndex;
            ongoing = true;
        }
        private void NoisesWithNulls_CheckedChanged(object sender, EventArgs e)
        {
            if (radistationManagerMas != null)
            {
                for (int i = 0; i < radistationManagerMas.Length; i++)
                {
                    radistationManagerMas[i].noiseWithNulls = NoisesWithNulls.Checked;
                }
            }
        }
        private void Noise_CheckedChanged(object sender, EventArgs e)
        {
            RadiostationManager.noise = Noise.Checked;
        }
        Semaphore rtbCleaner = new Semaphore(1, 1);
        int countOfChars = 0;
        void PrintToRTB(MessageForRTB messageForRTB)
        {
            if (messageForRTB.dialogInformation != null)
            {

                СhatRTB.SelectionStart = СhatRTB.TextLength;
                СhatRTB.SelectionColor = messageForRTB.color;
                СhatRTB.AppendText(messageForRTB.dialogInformation);
                СhatRTB.SelectionColor = СhatRTB.ForeColor;
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("Ch: " + messageForRTB.channel.ToString() + " " + messageForRTB.message);
                СhatRTB.AppendText(sb.ToString());

            }
        }

        private void MaxNumberOfLinesTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8)
            {
                e.Handled = true;
            }
        }
        private void MaxNumberOfLinesTB_TextChanged(object sender, EventArgs e)
        {
            try
            {
                MaxNumberOfLines = Convert.ToInt32(MaxNumberOfLinesTB.Text);
            }
            catch { }
        }

        private void ToolStripMenuItem_5thComSend_Click(object sender, EventArgs e)
        {
            radistationManagerMas[toolStripSelectedChannelCB.SelectedIndex].SendOneMessage(5, Convert.ToInt16(toolStripErrCodeCB.SelectedIndex + 1));
        }

        int MaxNumberOfLines = 200;


    }

}
