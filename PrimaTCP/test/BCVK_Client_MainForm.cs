using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test
{
    public partial class BCVK_Client_MainForm : Form
    {
        AdressesEthernet AdrEth = new AdressesEthernet();
        PerfomanceTestCounter pTC = new PerfomanceTestCounter();
        ChannelSelector chS = new ChannelSelector();
        MessageBoxWithButtons mes;
        IniFile INI = new IniFile("config.ini");
        Thread startTest;
        Encoding ascii = Encoding.Default;
        static CancellationTokenSource testStop = new CancellationTokenSource();
        static CancellationTokenSource controlConnectStop = new CancellationTokenSource();
        static CancellationToken controlConnectStopToken = controlConnectStop.Token;
        static CancellationToken tokenTestStop;
        static string writePath = Environment.CurrentDirectory + "\\file.txt";//@"\test.txt";
        static NetWorker net;
        static public Queue<byte[]> FIFO = new Queue<byte[]>();
        static public List<MessageForRTB> MessageForRTBList = new List<MessageForRTB>();
        List<int> RepeatedIns = new List<int>();
        static ConcurrentQueue<MessageForRTB> MessagesForRTBQueue = new ConcurrentQueue<MessageForRTB>();
        bool _driverType = false;
        bool _accepted16command = true;
        bool _primaConnectCheck;
        bool _connectControlToPrimaStarted;
        Random rand = new Random();
        static Stopwatch stopwatchConnectionCheck = new Stopwatch();
        static Stopwatch stopwatchHandTest = new Stopwatch();
        static Stopwatch stopwatchFullTest = new Stopwatch();
        static Stopwatch stopwatchAccept = new Stopwatch();
        static double[] amountOfDataOnChannel = new double[32] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
        static Stopwatch[] stopwatchMas = new Stopwatch[32] {
            new Stopwatch(),  new Stopwatch(),  new Stopwatch(),  new Stopwatch(),  new Stopwatch(),  new Stopwatch(),  new Stopwatch(),  new Stopwatch(),
             new Stopwatch(),  new Stopwatch(),  new Stopwatch(),  new Stopwatch(),  new Stopwatch(),  new Stopwatch(),  new Stopwatch(),  new Stopwatch(),
             new Stopwatch(),  new Stopwatch(),  new Stopwatch(),  new Stopwatch(),  new Stopwatch(),  new Stopwatch(),  new Stopwatch(),  new Stopwatch(),
             new Stopwatch(),  new Stopwatch(),  new Stopwatch(),  new Stopwatch(),  new Stopwatch(),  new Stopwatch(),  new Stopwatch(),  new Stopwatch()  };
        static int _numberOFStopwatch;


        delegate void ToTextBox(TextBox tb, string message);
        delegate void LabelChanging(int numberCh, int numberMessage);
        LabelChanging labelChanging;
        delegate void LabelChangingpTC(int number, int count);
        LabelChangingpTC labelChangingpTC;
        delegate string BringTextFromFormComponent(Object obj);
        BringTextFromFormComponent DelegateTextReturn;
        delegate int BringIndexFromFormComponent(Object obj);
        delegate void LabelFunction(Label label, string text);
        LabelFunction labelFunction;
        //delegate void BoolFunction(bool state);
        //BoolFunction boolFunction;
        delegate void NullFunction();
        delegate void AppendText(MessageForRTB messageForRTB);
        delegate void PingFunction(Label c, string state, Color color);
        public static bool connectionstate;
        private bool accepted;
        SemaphoreSlim acceptedState = new SemaphoreSlim(1, 1);
        static bool ShowOnRtb = true;
        static bool handMessageReadyToSend = true;
        public static bool connected = false;
        public static bool rightMessage0 = false;
        static bool commandIsCompleted = true;
        int timeOut;
        int timeOutAnswer = 1000;
        int startPointOfFindInPrimaRTB;
        //int[] correspondenceTable;
        public bool[] messageReady;
        bool mode = false;
        bool perfomanceTestMode = false;
        bool settingTest = false;
        bool probabilityOfBringingTestMode = false;
        bool perevalMode = false, ethernetMode = false, perfomanceMode = false;
        //byte[] adresses;
        byte[] EthernetAdresses;
        Label[] errorlabels;
        Label[] Counters;
        TextBox[,] Ethernetadresses = new TextBox[64, 3];
        RadioButton[,] EthernetCustomisationRBs = new RadioButton[32, 3];
        TextBox[,] EthernetCustomisationTBs = new TextBox[32, 3];
        ComboBox[] EthernetCustomisationComboBoxes = new ComboBox[32];
        CheckBox[] RadioStationcheckboxes = new CheckBox[32];
        byte[,] send10MessageMas = new byte[8, 14];
        byte[,] send16MessageMas = new byte[32, 50];
        byte[] income12 = new byte[8];
        public byte[,] incomeData = new byte[32, 1700];
        private bool[] answerRecieved = new bool[32] { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true,
        true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true};
        private bool[] kvitRecieved = new bool[32] {true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true,
        true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true };
        byte[,] longPrintFMessages = new byte[12, 2048];
        int[,] flagsOfLongPrintF = new int[12, 3];//Первый набор флагов указывает текущий ожидаемый номер пакета
                                                  //Второй - указатель на последний заполненный байт в общем массиве
                                                  //Третий - длина последнего сообщения (каждого из составного) на последний заполненный байт в общем массиве
        private bool[] access = new bool[32] { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true,
        true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true};
        private int[] SkMas = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        private int[] TimeOutMas = new int[8] { 7000, 7000, 7000, 7000, 7000, 7000, 7000, 7000 };
        private bool[] ipPaCheckedMas = new bool[8];
        private bool[] labelConnectedBlock = new bool[32];
        ChannelWorker[] Channels;
        private void MainForm_Load(object sender, EventArgs e)
        {
            Invoke(new NullFunction(this.ToolStripMenuItemLoadConfiguration.PerformClick));
        }
        public BCVK_Client_MainForm()
        {
            ///Отрицать нет смысла, класс формы сильно перегружен, но несмотря, что я старался всячески разбить его, лучшего выхода чем собрать всё тут - я не нашел
            ///Один из вариантов был также разобрать класс на partial, но как выяснилось - это делает работу еще более неудобной (разные окна сильно путают), или может быть я такой кривой?)
            /// in_it по факту просто начальная инициализация формы: забивание данных в списки, создание необходимых массивов и тп
            #region _in_it_
            InitializeComponent();
            this.MaximumSize = new System.Drawing.Size(3000, 3000);
            DirectoryInfo dirInfo = new DirectoryInfo(Environment.CurrentDirectory + "\\SavedMessages");
            OFD.Filter = "Text files(*.txt)|*.txt|ALL files(*.*)|*.*";
            SFD.Filter = "Text files(*.txt)|*.txt|ALL files(*.*)|*.*";
            ProtocoltypeChannel1All.Items.AddRange(new string[] {"Режим не задан","Чайка однократный","Чайка двукратный","Чайка 'Аккорд'",
            "Чайка 'Мелодия'", "Перевал короткий код","Перевал длинный код","Безызботочные сообщения"});
            ProtocoltypeChannel1All.SelectedIndex = 5;
            perevalLengthOfmessage.Items.AddRange(new string[] { "17", "48", "80", "112" });
            perevalLengthOfmessage.SelectedIndex = 0;
            LengthOfChaikaMes1.Items.AddRange(new string[] { "15", "62" });
            LengthOfChaikaMes1.SelectedIndex = 0;
            ProtocoltypeChannel2All.Items.AddRange(new string[] {"Режим не задан","Чайка однократный","Чайка двукратный","Чайка 'Аккорд'",
            "Чайка 'Мелодия'", "Перевал короткий код","Перевал длинный код","Безызботочные сообщения"});
            ProtocoltypeChannel2All.SelectedIndex = 6;
            numberOfHandCommand.Items.AddRange(new string[] { "1", "2", "10", "11", "12", "13", "14", "16", "255", "серия команд" });
            numberOfHandCommand.SelectedIndex = 1;
            numberOfHandCommand.DropDownHeight = numberOfHandCommand.Items.Count * numberOfHandCommand.ItemHeight;
            сomboBoxSpeedCode10.Items.AddRange(new string[] { "75", "150", "300", "1200", "2400", "4800", "9600", "16000", "50", "100" });
            сomboBoxSpeedCode10.SelectedIndex = 5;
            сomboBoxSpeedCode10.DropDownHeight = сomboBoxSpeedCode10.Items.Count * сomboBoxSpeedCode10.ItemHeight;
            comboBoxPlume10.Items.AddRange(new string[] { "отключить шлейф", "На уровне контроллера Ethernet" });
            comboBoxPlume10.SelectedIndex = 0;
            interfacetype10.Items.AddRange(new string[] { "С1-ТГ", "С1-ФЛ-БИ", "Ethernet" });
            interfacetype10.SelectedIndex = 0;
            sach2ComboBox.Items.AddRange(new string[] { "CАЧ не используется", "8", "10", "12", "16" });
            sach2ComboBox.SelectedIndex = 1;
            Counters = new Label[32]
             {
                 pTC.Counter11,pTC.Counter12,pTC.Counter13,pTC.Counter14,
                 pTC.Counter21,pTC.Counter22,pTC.Counter23,pTC.Counter24,
                 pTC.Counter31,pTC.Counter32,pTC.Counter33,pTC.Counter34,
                 pTC.Counter41,pTC.Counter42,pTC.Counter43,pTC.Counter44,
                 pTC.Counter51,pTC.Counter52,pTC.Counter53,pTC.Counter54,
                 pTC.Counter61,pTC.Counter62,pTC.Counter63,pTC.Counter64,
                 pTC.Counter71,pTC.Counter72,pTC.Counter73,pTC.Counter74,
                 pTC.Counter81,pTC.Counter82,pTC.Counter83,pTC.Counter84
             };
            Ethernetadresses = new TextBox[64, 3]
                                      {
                                          {AdrEth.Rad_IP_1, AdrEth.Rad_P_1,AdrEth.Rad_MAC_1},{AdrEth.UU_IP_1, AdrEth.UU_P_1,AdrEth.UU_MAC_1},
                                          {AdrEth.Rad_IP_2, AdrEth.Rad_P_2,AdrEth.Rad_MAC_2},{AdrEth.UU_IP_2, AdrEth.UU_P_2,AdrEth.UU_MAC_2},
                                          {AdrEth.Rad_IP_3, AdrEth.Rad_P_3,AdrEth.Rad_MAC_3},{AdrEth.UU_IP_3, AdrEth.UU_P_3,AdrEth.UU_MAC_3},
                                          {AdrEth.Rad_IP_4, AdrEth.Rad_P_4,AdrEth.Rad_MAC_4},{AdrEth.UU_IP_4, AdrEth.UU_P_4,AdrEth.UU_MAC_4},
                                          {AdrEth.Rad_IP_5, AdrEth.Rad_P_5,AdrEth.Rad_MAC_5},{AdrEth.UU_IP_5, AdrEth.UU_P_5,AdrEth.UU_MAC_5},
                                          {AdrEth.Rad_IP_6, AdrEth.Rad_P_6,AdrEth.Rad_MAC_6},{AdrEth.UU_IP_6, AdrEth.UU_P_6,AdrEth.UU_MAC_6},
                                          {AdrEth.Rad_IP_7, AdrEth.Rad_P_7,AdrEth.Rad_MAC_7},{AdrEth.UU_IP_7, AdrEth.UU_P_7,AdrEth.UU_MAC_7},
                                          {AdrEth.Rad_IP_8, AdrEth.Rad_P_8,AdrEth.Rad_MAC_8},{AdrEth.UU_IP_8, AdrEth.UU_P_8,AdrEth.UU_MAC_8},
                                          {AdrEth.Rad_IP_9, AdrEth.Rad_P_9,AdrEth.Rad_MAC_9},{AdrEth.UU_IP_9, AdrEth.UU_P_9,AdrEth.UU_MAC_9},
                                          {AdrEth.Rad_IP_10, AdrEth.Rad_P_10,AdrEth.Rad_MAC_10},{AdrEth.UU_IP_10, AdrEth.UU_P_10,AdrEth.UU_MAC_10},
                                          {AdrEth.Rad_IP_11, AdrEth.Rad_P_11,AdrEth.Rad_MAC_11},{AdrEth.UU_IP_11, AdrEth.UU_P_11,AdrEth.UU_MAC_11},
                                          {AdrEth.Rad_IP_12, AdrEth.Rad_P_12,AdrEth.Rad_MAC_12},{AdrEth.UU_IP_12, AdrEth.UU_P_12,AdrEth.UU_MAC_12},
                                          {AdrEth.Rad_IP_13, AdrEth.Rad_P_13,AdrEth.Rad_MAC_13},{AdrEth.UU_IP_13, AdrEth.UU_P_13,AdrEth.UU_MAC_13},
                                          {AdrEth.Rad_IP_14, AdrEth.Rad_P_14,AdrEth.Rad_MAC_14},{AdrEth.UU_IP_14, AdrEth.UU_P_14,AdrEth.UU_MAC_14},
                                          {AdrEth.Rad_IP_15, AdrEth.Rad_P_15,AdrEth.Rad_MAC_15},{AdrEth.UU_IP_15, AdrEth.UU_P_15,AdrEth.UU_MAC_15},
                                          {AdrEth.Rad_IP_16, AdrEth.Rad_P_16,AdrEth.Rad_MAC_16},{AdrEth.UU_IP_16, AdrEth.UU_P_16,AdrEth.UU_MAC_16},
                                          {AdrEth.Rad_IP_17, AdrEth.Rad_P_17,AdrEth.Rad_MAC_17},{AdrEth.UU_IP_17, AdrEth.UU_P_17,AdrEth.UU_MAC_17},
                                          {AdrEth.Rad_IP_18, AdrEth.Rad_P_18,AdrEth.Rad_MAC_18},{AdrEth.UU_IP_18, AdrEth.UU_P_18,AdrEth.UU_MAC_18},
                                          {AdrEth.Rad_IP_19, AdrEth.Rad_P_19,AdrEth.Rad_MAC_19},{AdrEth.UU_IP_19, AdrEth.UU_P_19,AdrEth.UU_MAC_19},
                                          {AdrEth.Rad_IP_20, AdrEth.Rad_P_20,AdrEth.Rad_MAC_20},{AdrEth.UU_IP_20, AdrEth.UU_P_20,AdrEth.UU_MAC_20},
                                          {AdrEth.Rad_IP_21, AdrEth.Rad_P_21,AdrEth.Rad_MAC_21},{AdrEth.UU_IP_21, AdrEth.UU_P_21,AdrEth.UU_MAC_21},
                                          {AdrEth.Rad_IP_22, AdrEth.Rad_P_22,AdrEth.Rad_MAC_22},{AdrEth.UU_IP_22, AdrEth.UU_P_22,AdrEth.UU_MAC_22},
                                          {AdrEth.Rad_IP_23, AdrEth.Rad_P_23,AdrEth.Rad_MAC_23},{AdrEth.UU_IP_23, AdrEth.UU_P_23,AdrEth.UU_MAC_23},
                                          {AdrEth.Rad_IP_24, AdrEth.Rad_P_24,AdrEth.Rad_MAC_24},{AdrEth.UU_IP_24, AdrEth.UU_P_24,AdrEth.UU_MAC_24},
                                          {AdrEth.Rad_IP_25, AdrEth.Rad_P_25,AdrEth.Rad_MAC_25},{AdrEth.UU_IP_25, AdrEth.UU_P_25,AdrEth.UU_MAC_25},
                                          {AdrEth.Rad_IP_26, AdrEth.Rad_P_26,AdrEth.Rad_MAC_26},{AdrEth.UU_IP_26, AdrEth.UU_P_26,AdrEth.UU_MAC_26},
                                          {AdrEth.Rad_IP_27, AdrEth.Rad_P_27,AdrEth.Rad_MAC_27},{AdrEth.UU_IP_27, AdrEth.UU_P_27,AdrEth.UU_MAC_27},
                                          {AdrEth.Rad_IP_28, AdrEth.Rad_P_28,AdrEth.Rad_MAC_28},{AdrEth.UU_IP_28, AdrEth.UU_P_28,AdrEth.UU_MAC_28},
                                          {AdrEth.Rad_IP_29, AdrEth.Rad_P_29,AdrEth.Rad_MAC_29},{AdrEth.UU_IP_29, AdrEth.UU_P_29,AdrEth.UU_MAC_29},
                                          {AdrEth.Rad_IP_30, AdrEth.Rad_P_30,AdrEth.Rad_MAC_30},{AdrEth.UU_IP_30, AdrEth.UU_P_30,AdrEth.UU_MAC_30},
                                          {AdrEth.Rad_IP_31, AdrEth.Rad_P_31,AdrEth.Rad_MAC_31},{AdrEth.UU_IP_31, AdrEth.UU_P_31,AdrEth.UU_MAC_31},
                                          {AdrEth.Rad_IP_32, AdrEth.Rad_P_32,AdrEth.Rad_MAC_32},{AdrEth.UU_IP_32, AdrEth.UU_P_32,AdrEth.UU_MAC_32},
                                      };
            EthernetCustomisationRBs = new RadioButton[32, 9] {
                { AdrEth.Rad_1_Shl_Off,AdrEth.Rad_1_Shl_On,AdrEth.Rad_1_Zas_1,AdrEth.Rad_1_Zas_2,AdrEth.Rad_1_Zas_FF,AdrEth.Rad_1_Kv_Off,AdrEth.Rad_1_Kv_On,AdrEth.Rad_1_Kvt_Off,AdrEth.Rad_1_Kvt_On},
                { AdrEth.Rad_2_Shl_Off,AdrEth.Rad_2_Shl_On,AdrEth.Rad_2_Zas_1,AdrEth.Rad_2_Zas_2,AdrEth.Rad_2_Zas_FF,AdrEth.Rad_2_Kv_Off,AdrEth.Rad_2_Kv_On,AdrEth.Rad_2_Kvt_Off,AdrEth.Rad_2_Kvt_On},
                { AdrEth.Rad_3_Shl_Off,AdrEth.Rad_3_Shl_On,AdrEth.Rad_3_Zas_1,AdrEth.Rad_3_Zas_2,AdrEth.Rad_3_Zas_FF,AdrEth.Rad_3_Kv_Off,AdrEth.Rad_3_Kv_On,AdrEth.Rad_3_Kvt_Off,AdrEth.Rad_3_Kvt_On},
                { AdrEth.Rad_4_Shl_Off,AdrEth.Rad_4_Shl_On,AdrEth.Rad_4_Zas_1,AdrEth.Rad_4_Zas_2,AdrEth.Rad_4_Zas_FF,AdrEth.Rad_4_Kv_Off,AdrEth.Rad_4_Kv_On,AdrEth.Rad_4_Kvt_Off,AdrEth.Rad_4_Kvt_On},
                { AdrEth.Rad_5_Shl_Off,AdrEth.Rad_5_Shl_On,AdrEth.Rad_5_Zas_1,AdrEth.Rad_5_Zas_2,AdrEth.Rad_5_Zas_FF,AdrEth.Rad_5_Kv_Off,AdrEth.Rad_5_Kv_On,AdrEth.Rad_5_Kvt_Off,AdrEth.Rad_5_Kvt_On},
                { AdrEth.Rad_6_Shl_Off,AdrEth.Rad_6_Shl_On,AdrEth.Rad_6_Zas_1,AdrEth.Rad_6_Zas_2,AdrEth.Rad_6_Zas_FF,AdrEth.Rad_6_Kv_Off,AdrEth.Rad_6_Kv_On,AdrEth.Rad_6_Kvt_Off,AdrEth.Rad_6_Kvt_On},
                { AdrEth.Rad_7_Shl_Off,AdrEth.Rad_7_Shl_On,AdrEth.Rad_7_Zas_1,AdrEth.Rad_7_Zas_2,AdrEth.Rad_7_Zas_FF,AdrEth.Rad_7_Kv_Off,AdrEth.Rad_7_Kv_On,AdrEth.Rad_7_Kvt_Off,AdrEth.Rad_7_Kvt_On},
                { AdrEth.Rad_8_Shl_Off,AdrEth.Rad_8_Shl_On,AdrEth.Rad_8_Zas_1,AdrEth.Rad_8_Zas_2,AdrEth.Rad_8_Zas_FF,AdrEth.Rad_8_Kv_Off,AdrEth.Rad_8_Kv_On,AdrEth.Rad_8_Kvt_Off,AdrEth.Rad_8_Kvt_On},
                { AdrEth.Rad_9_Shl_Off,AdrEth.Rad_9_Shl_On,AdrEth.Rad_9_Zas_1,AdrEth.Rad_9_Zas_2,AdrEth.Rad_9_Zas_FF,AdrEth.Rad_9_Kv_Off,AdrEth.Rad_9_Kv_On,AdrEth.Rad_9_Kvt_Off,AdrEth.Rad_9_Kvt_On},
                { AdrEth.Rad_10_Shl_Off,AdrEth.Rad_10_Shl_On,AdrEth.Rad_10_Zas_1,AdrEth.Rad_10_Zas_2,AdrEth.Rad_10_Zas_FF,AdrEth.Rad_10_Kv_Off,AdrEth.Rad_10_Kv_On,AdrEth.Rad_10_Kvt_Off,AdrEth.Rad_10_Kvt_On},
                { AdrEth.Rad_11_Shl_Off,AdrEth.Rad_11_Shl_On,AdrEth.Rad_11_Zas_1,AdrEth.Rad_11_Zas_2,AdrEth.Rad_11_Zas_FF,AdrEth.Rad_11_Kv_Off,AdrEth.Rad_11_Kv_On,AdrEth.Rad_11_Kvt_Off,AdrEth.Rad_11_Kvt_On},
                { AdrEth.Rad_12_Shl_Off,AdrEth.Rad_12_Shl_On,AdrEth.Rad_12_Zas_1,AdrEth.Rad_12_Zas_2,AdrEth.Rad_12_Zas_FF,AdrEth.Rad_12_Kv_Off,AdrEth.Rad_12_Kv_On,AdrEth.Rad_12_Kvt_Off,AdrEth.Rad_12_Kvt_On},
                { AdrEth.Rad_13_Shl_Off,AdrEth.Rad_13_Shl_On,AdrEth.Rad_13_Zas_1,AdrEth.Rad_13_Zas_2,AdrEth.Rad_13_Zas_FF,AdrEth.Rad_13_Kv_Off,AdrEth.Rad_13_Kv_On,AdrEth.Rad_13_Kvt_Off,AdrEth.Rad_13_Kvt_On},
                { AdrEth.Rad_14_Shl_Off,AdrEth.Rad_14_Shl_On,AdrEth.Rad_14_Zas_1,AdrEth.Rad_14_Zas_2,AdrEth.Rad_14_Zas_FF,AdrEth.Rad_14_Kv_Off,AdrEth.Rad_14_Kv_On,AdrEth.Rad_14_Kvt_Off,AdrEth.Rad_14_Kvt_On},
                { AdrEth.Rad_15_Shl_Off,AdrEth.Rad_15_Shl_On,AdrEth.Rad_15_Zas_1,AdrEth.Rad_15_Zas_2,AdrEth.Rad_15_Zas_FF,AdrEth.Rad_15_Kv_Off,AdrEth.Rad_15_Kv_On,AdrEth.Rad_15_Kvt_Off,AdrEth.Rad_15_Kvt_On},
                { AdrEth.Rad_16_Shl_Off,AdrEth.Rad_16_Shl_On,AdrEth.Rad_16_Zas_1,AdrEth.Rad_16_Zas_2,AdrEth.Rad_16_Zas_FF,AdrEth.Rad_16_Kv_Off,AdrEth.Rad_16_Kv_On,AdrEth.Rad_16_Kvt_Off,AdrEth.Rad_16_Kvt_On},
                { AdrEth.Rad_17_Shl_Off,AdrEth.Rad_17_Shl_On,AdrEth.Rad_17_Zas_1,AdrEth.Rad_17_Zas_2,AdrEth.Rad_17_Zas_FF,AdrEth.Rad_17_Kv_Off,AdrEth.Rad_17_Kv_On,AdrEth.Rad_17_Kvt_Off,AdrEth.Rad_17_Kvt_On},
                { AdrEth.Rad_18_Shl_Off,AdrEth.Rad_18_Shl_On,AdrEth.Rad_18_Zas_1,AdrEth.Rad_18_Zas_2,AdrEth.Rad_18_Zas_FF,AdrEth.Rad_18_Kv_Off,AdrEth.Rad_18_Kv_On,AdrEth.Rad_18_Kvt_Off,AdrEth.Rad_18_Kvt_On},
                { AdrEth.Rad_19_Shl_Off,AdrEth.Rad_19_Shl_On,AdrEth.Rad_19_Zas_1,AdrEth.Rad_19_Zas_2,AdrEth.Rad_19_Zas_FF,AdrEth.Rad_19_Kv_Off,AdrEth.Rad_19_Kv_On,AdrEth.Rad_19_Kvt_Off,AdrEth.Rad_19_Kvt_On},
                { AdrEth.Rad_20_Shl_Off,AdrEth.Rad_20_Shl_On,AdrEth.Rad_20_Zas_1,AdrEth.Rad_20_Zas_2,AdrEth.Rad_20_Zas_FF,AdrEth.Rad_20_Kv_Off,AdrEth.Rad_20_Kv_On,AdrEth.Rad_20_Kvt_Off,AdrEth.Rad_20_Kvt_On},
                { AdrEth.Rad_21_Shl_Off,AdrEth.Rad_21_Shl_On,AdrEth.Rad_21_Zas_1,AdrEth.Rad_21_Zas_2,AdrEth.Rad_21_Zas_FF,AdrEth.Rad_21_Kv_Off,AdrEth.Rad_21_Kv_On,AdrEth.Rad_21_Kvt_Off,AdrEth.Rad_21_Kvt_On},
                { AdrEth.Rad_22_Shl_Off,AdrEth.Rad_22_Shl_On,AdrEth.Rad_22_Zas_1,AdrEth.Rad_22_Zas_2,AdrEth.Rad_22_Zas_FF,AdrEth.Rad_22_Kv_Off,AdrEth.Rad_22_Kv_On,AdrEth.Rad_22_Kvt_Off,AdrEth.Rad_22_Kvt_On},
                { AdrEth.Rad_23_Shl_Off,AdrEth.Rad_23_Shl_On,AdrEth.Rad_23_Zas_1,AdrEth.Rad_23_Zas_2,AdrEth.Rad_23_Zas_FF,AdrEth.Rad_23_Kv_Off,AdrEth.Rad_23_Kv_On,AdrEth.Rad_23_Kvt_Off,AdrEth.Rad_23_Kvt_On},
                { AdrEth.Rad_24_Shl_Off,AdrEth.Rad_24_Shl_On,AdrEth.Rad_24_Zas_1,AdrEth.Rad_24_Zas_2,AdrEth.Rad_24_Zas_FF,AdrEth.Rad_24_Kv_Off,AdrEth.Rad_24_Kv_On,AdrEth.Rad_24_Kvt_Off,AdrEth.Rad_24_Kvt_On},
                { AdrEth.Rad_25_Shl_Off,AdrEth.Rad_25_Shl_On,AdrEth.Rad_25_Zas_1,AdrEth.Rad_25_Zas_2,AdrEth.Rad_25_Zas_FF,AdrEth.Rad_25_Kv_Off,AdrEth.Rad_25_Kv_On,AdrEth.Rad_25_Kvt_Off,AdrEth.Rad_25_Kvt_On},
                { AdrEth.Rad_26_Shl_Off,AdrEth.Rad_26_Shl_On,AdrEth.Rad_26_Zas_1,AdrEth.Rad_26_Zas_2,AdrEth.Rad_26_Zas_FF,AdrEth.Rad_26_Kv_Off,AdrEth.Rad_26_Kv_On,AdrEth.Rad_26_Kvt_Off,AdrEth.Rad_26_Kvt_On},
                { AdrEth.Rad_27_Shl_Off,AdrEth.Rad_27_Shl_On,AdrEth.Rad_27_Zas_1,AdrEth.Rad_27_Zas_2,AdrEth.Rad_27_Zas_FF,AdrEth.Rad_27_Kv_Off,AdrEth.Rad_27_Kv_On,AdrEth.Rad_27_Kvt_Off,AdrEth.Rad_27_Kvt_On},
                { AdrEth.Rad_28_Shl_Off,AdrEth.Rad_28_Shl_On,AdrEth.Rad_28_Zas_1,AdrEth.Rad_28_Zas_2,AdrEth.Rad_28_Zas_FF,AdrEth.Rad_28_Kv_Off,AdrEth.Rad_28_Kv_On,AdrEth.Rad_28_Kvt_Off,AdrEth.Rad_28_Kvt_On},
                { AdrEth.Rad_29_Shl_Off,AdrEth.Rad_29_Shl_On,AdrEth.Rad_29_Zas_1,AdrEth.Rad_29_Zas_2,AdrEth.Rad_29_Zas_FF,AdrEth.Rad_29_Kv_Off,AdrEth.Rad_29_Kv_On,AdrEth.Rad_29_Kvt_Off,AdrEth.Rad_29_Kvt_On},
                { AdrEth.Rad_30_Shl_Off,AdrEth.Rad_30_Shl_On,AdrEth.Rad_30_Zas_1,AdrEth.Rad_30_Zas_2,AdrEth.Rad_30_Zas_FF,AdrEth.Rad_30_Kv_Off,AdrEth.Rad_30_Kv_On,AdrEth.Rad_30_Kvt_Off,AdrEth.Rad_30_Kvt_On},
                { AdrEth.Rad_31_Shl_Off,AdrEth.Rad_31_Shl_On,AdrEth.Rad_31_Zas_1,AdrEth.Rad_31_Zas_2,AdrEth.Rad_31_Zas_FF,AdrEth.Rad_31_Kv_Off,AdrEth.Rad_31_Kv_On,AdrEth.Rad_31_Kvt_Off,AdrEth.Rad_31_Kvt_On},
                { AdrEth.Rad_32_Shl_Off,AdrEth.Rad_32_Shl_On,AdrEth.Rad_32_Zas_1,AdrEth.Rad_32_Zas_2,AdrEth.Rad_32_Zas_FF,AdrEth.Rad_32_Kv_Off,AdrEth.Rad_32_Kv_On,AdrEth.Rad_32_Kvt_Off,AdrEth.Rad_32_Kvt_On},
            };
            EthernetCustomisationTBs = new TextBox[32, 4] {
                { AdrEth.Rad_1_KP,AdrEth.Rad_1_KPP,AdrEth.Rad_1_RD,AdrEth.Rad_1_Tiv}, { AdrEth.Rad_2_KP,AdrEth.Rad_2_KPP,AdrEth.Rad_2_RD,AdrEth.Rad_2_Tiv },
                { AdrEth.Rad_3_KP,AdrEth.Rad_3_KPP,AdrEth.Rad_3_RD,AdrEth.Rad_3_Tiv}, { AdrEth.Rad_4_KP,AdrEth.Rad_4_KPP,AdrEth.Rad_4_RD,AdrEth.Rad_4_Tiv },
                { AdrEth.Rad_5_KP,AdrEth.Rad_5_KPP,AdrEth.Rad_5_RD,AdrEth.Rad_5_Tiv}, { AdrEth.Rad_6_KP,AdrEth.Rad_6_KPP,AdrEth.Rad_6_RD,AdrEth.Rad_6_Tiv },
                { AdrEth.Rad_7_KP,AdrEth.Rad_7_KPP,AdrEth.Rad_7_RD,AdrEth.Rad_7_Tiv}, { AdrEth.Rad_8_KP,AdrEth.Rad_8_KPP,AdrEth.Rad_8_RD,AdrEth.Rad_8_Tiv },
                { AdrEth.Rad_9_KP,AdrEth.Rad_9_KPP,AdrEth.Rad_9_RD,AdrEth.Rad_9_Tiv}, { AdrEth.Rad_10_KP,AdrEth.Rad_10_KPP,AdrEth.Rad_10_RD,AdrEth.Rad_10_Tiv },
                { AdrEth.Rad_11_KP,AdrEth.Rad_11_KPP,AdrEth.Rad_11_RD,AdrEth.Rad_11_Tiv}, { AdrEth.Rad_12_KP,AdrEth.Rad_12_KPP,AdrEth.Rad_12_RD,AdrEth.Rad_12_Tiv },
                { AdrEth.Rad_13_KP,AdrEth.Rad_13_KPP,AdrEth.Rad_13_RD,AdrEth.Rad_13_Tiv}, { AdrEth.Rad_14_KP,AdrEth.Rad_14_KPP,AdrEth.Rad_14_RD,AdrEth.Rad_14_Tiv },
                { AdrEth.Rad_15_KP,AdrEth.Rad_15_KPP,AdrEth.Rad_15_RD,AdrEth.Rad_15_Tiv}, { AdrEth.Rad_16_KP,AdrEth.Rad_16_KPP,AdrEth.Rad_16_RD,AdrEth.Rad_16_Tiv },
                { AdrEth.Rad_17_KP,AdrEth.Rad_17_KPP,AdrEth.Rad_17_RD,AdrEth.Rad_17_Tiv}, { AdrEth.Rad_18_KP,AdrEth.Rad_18_KPP,AdrEth.Rad_18_RD,AdrEth.Rad_18_Tiv },
                { AdrEth.Rad_19_KP,AdrEth.Rad_19_KPP,AdrEth.Rad_19_RD,AdrEth.Rad_19_Tiv}, { AdrEth.Rad_20_KP,AdrEth.Rad_20_KPP,AdrEth.Rad_20_RD,AdrEth.Rad_20_Tiv },
                { AdrEth.Rad_21_KP,AdrEth.Rad_21_KPP,AdrEth.Rad_21_RD,AdrEth.Rad_21_Tiv}, { AdrEth.Rad_22_KP,AdrEth.Rad_22_KPP,AdrEth.Rad_22_RD,AdrEth.Rad_22_Tiv },
                { AdrEth.Rad_23_KP,AdrEth.Rad_23_KPP,AdrEth.Rad_23_RD,AdrEth.Rad_23_Tiv}, { AdrEth.Rad_24_KP,AdrEth.Rad_24_KPP,AdrEth.Rad_24_RD,AdrEth.Rad_24_Tiv },
                { AdrEth.Rad_25_KP,AdrEth.Rad_25_KPP,AdrEth.Rad_25_RD,AdrEth.Rad_25_Tiv}, { AdrEth.Rad_26_KP,AdrEth.Rad_26_KPP,AdrEth.Rad_26_RD,AdrEth.Rad_26_Tiv },
                { AdrEth.Rad_27_KP,AdrEth.Rad_27_KPP,AdrEth.Rad_27_RD,AdrEth.Rad_27_Tiv}, { AdrEth.Rad_28_KP,AdrEth.Rad_28_KPP,AdrEth.Rad_28_RD,AdrEth.Rad_28_Tiv },
                { AdrEth.Rad_29_KP,AdrEth.Rad_29_KPP,AdrEth.Rad_29_RD,AdrEth.Rad_29_Tiv}, { AdrEth.Rad_30_KP,AdrEth.Rad_30_KPP,AdrEth.Rad_30_RD,AdrEth.Rad_30_Tiv },
                { AdrEth.Rad_31_KP,AdrEth.Rad_31_KPP,AdrEth.Rad_31_RD,AdrEth.Rad_31_Tiv}, { AdrEth.Rad_32_KP,AdrEth.Rad_32_KPP,AdrEth.Rad_32_RD,AdrEth.Rad_32_Tiv },
            };
            EthernetCustomisationComboBoxes = new ComboBox[32]
            {
                AdrEth.Rad_1_SC,AdrEth.Rad_2_SC,AdrEth.Rad_3_SC,AdrEth.Rad_4_SC,AdrEth.Rad_5_SC,AdrEth.Rad_6_SC,AdrEth.Rad_7_SC,AdrEth.Rad_8_SC,
                AdrEth.Rad_9_SC,AdrEth.Rad_10_SC,AdrEth.Rad_11_SC,AdrEth.Rad_12_SC,AdrEth.Rad_13_SC,AdrEth.Rad_14_SC,AdrEth.Rad_15_SC,AdrEth.Rad_16_SC,
                AdrEth.Rad_17_SC,AdrEth.Rad_18_SC,AdrEth.Rad_19_SC,AdrEth.Rad_20_SC,AdrEth.Rad_21_SC,AdrEth.Rad_22_SC,AdrEth.Rad_23_SC,AdrEth.Rad_24_SC,
                AdrEth.Rad_25_SC,AdrEth.Rad_26_SC,AdrEth.Rad_27_SC,AdrEth.Rad_28_SC,AdrEth.Rad_29_SC,AdrEth.Rad_30_SC,AdrEth.Rad_31_SC,AdrEth.Rad_32_SC
            };
            foreach (ComboBox cb in EthernetCustomisationComboBoxes)
            {
                cb.SelectedIndex = 0;
            }
            PrimaRTB.ContextMenuStrip = contextMenuPrimaRTB;
            RadioStationcheckboxes = new CheckBox[32]
            {RadioStation1CB,RadioStation2CB,RadioStation3CB,RadioStation4CB,RadioStation5CB,RadioStation6CB,RadioStation7CB,RadioStation8CB,
RadioStation9CB,RadioStation10CB,RadioStation11CB,RadioStation12CB,RadioStation13CB,RadioStation14CB,RadioStation15CB,RadioStation16CB,
RadioStation17CB,RadioStation18CB,RadioStation19CB,RadioStation20CB,RadioStation21CB,RadioStation22CB,RadioStation23CB,RadioStation24CB,
RadioStation25CB,RadioStation26CB,RadioStation27CB,RadioStation28CB,RadioStation29CB,RadioStation30CB,RadioStation31CB,RadioStation32CB};
            DelegateTextReturn = ReturnSelectedText;
            labelChanging = ErrorLabelChanging;
            labelFunction = ChangeLabelText;
            labelChangingpTC = CounterPTCChanging;
            Thread ShowingOnscreenThread = new Thread(() => LogShower());
            ShowingOnscreenThread.Start();
            #endregion
        }


        #region workpart
        private bool CreationConnect(string addressPrima, string portPrima) //как можно догадаться просто создание tcp соединения с примой
        {
            net = new NetWorker(addressPrima, portPrima, this);
            connected = true;//Указывает на состояние подключения, на случай если нужно пересоздать его или стоит ли пытаться послать что-то если его нет
            var t = net.TCPConnect();
            if (!t)
            {
                connected = false;
            }
            return t;
        }
        private void StartTest()
        {
            if (Init())//Сброс состояний, если соединение не было создано, то тест не будет начат
            {
                try
                {

                    BringIndexFromFormComponent DelegateIndexReturn = ReturnSelectedIndex;//делегат, чтобы получать данные из элементов формы
                                                                                          ///В зависимости от выбранного режима будет
                    if (!mode)//ручная генерация сообщений (с помощью вкладок с параметрами)
                    {//либо выбирается какая то определенная команда, либо "макрос" ручной тест, который позволяет сразу выдать несколько комманд, все комманды берут данные из соответствующих для них форм
                        #region HandTest
                        int prg;
                        byte[] rp;
                        int np;
                        int dp;
                        int channel;
                        int lengthOfMessage;
                        int numberOfCommand = numberOfHandCommand.SelectedIndex;
                        switch (numberOfCommand)
                        {
                            case 0:
                                byte nz;
                                if (ZasFFRB1.Checked)
                                {
                                    nz = 0xFF;
                                }
                                else
                                {
                                    nz = Convert.ToByte(ZasNomer2RB1.Checked);
                                }
                                prg = Convert.ToInt32(Invoke(DelegateTextReturn, PRGtextbox1));
                                rp = functions.RP((int)Invoke(DelegateIndexReturn, ProtocoltypeChannel1All));
                                np = Convert.ToInt16(npOnButton1.Checked);
                                dp = Convert.ToInt32(Invoke(DelegateTextReturn, DPtextbox1));
                                channel = (1 * Convert.ToInt16(checkBoxChannel1.Checked) + 2 * Convert.ToInt16(checkBoxChannel2.Checked) + 4 * Convert.ToInt16(checkBoxChannel3.Checked)
                                               + 8 * Convert.ToInt16(checkBoxChannel4.Checked) + 16 * Convert.ToInt16(checkBoxChannel5.Checked) + 32 * Convert.ToInt16(checkBoxChannel6.Checked)
                                               + 64 * Convert.ToInt16(checkBoxChannel7.Checked) + 128 * Convert.ToInt16(checkBoxChannel8.Checked));
                                lengthOfMessage = (int)Invoke(DelegateIndexReturn, perevalLengthOfmessage);
                                RecordToRTB(Color.Blue, "отправка сообщения");
                                NewgeneratingMessage1(nz, prg, Convert.ToInt32(rp[0]), np, dp, channel, lengthOfMessage, 16);
                                break;
                            case 1:
                                //if (ZasNomer2RB2.Checked)
                                //{
                                //    nz = 0x1;
                                //}
                                //else
                                //{
                                //    nz = Convert.ToByte(ZasNomer2RB1.Checked);
                                //}
                                //prg = Convert.ToInt32(Invoke(DelegateTextReturn, prgTextBox2));
                                //rp = functions.RP((int)Invoke(DelegateIndexReturn, ProtocoltypeChannel2All));
                                //np = Convert.ToInt16(npOnButton2.Checked);
                                //dp = Convert.ToInt32(Invoke(DelegateTextReturn, DPtextbox2));
                                //channel = (1 * Convert.ToInt16(checkBoxChannel1.Checked) + 2 * Convert.ToInt16(checkBoxChannel2.Checked) + 4 * Convert.ToInt16(checkBoxChannel3.Checked) + 8 * Convert.ToInt16(checkBoxChannel4.Checked) + 16 * Convert.ToInt16(checkBoxChannel5.Checked)
                                //    + 32 * Convert.ToInt16(checkBoxChannel6.Checked) + 64 * Convert.ToInt16(checkBoxChannel7.Checked) + 128 * Convert.ToInt16(checkBoxChannel8.Checked));
                                //lengthOfMessage = Convert.ToInt32(Invoke(DelegateTextReturn, LengthcompositeMes));
                                //RecordToRTB(Color.Blue, "отправка сообщения");
                                //NewgeneratingMessage2(nz, prg, Convert.ToInt32(rp[0]), np, dp, channel, (int)Invoke(DelegateIndexReturn, sach2ComboBox), Convert.ToInt32(Invoke(DelegateTextReturn, nfTextBox2)), lengthOfMessage);

                                break;
                            case 2:
                                channel = (1 * Convert.ToInt16(checkBoxChannel1.Checked) + 2 * Convert.ToInt16(checkBoxChannel2.Checked) + 4 * Convert.ToInt16(checkBoxChannel3.Checked)
                                           + 8 * Convert.ToInt16(checkBoxChannel4.Checked) + 16 * Convert.ToInt16(checkBoxChannel5.Checked) + 32 * Convert.ToInt16(checkBoxChannel6.Checked) + 64 * Convert.ToInt16(checkBoxChannel7.Checked)
                                           + 128 * Convert.ToInt16(checkBoxChannel8.Checked));
                                int sk = сomboBoxSpeedCode10.SelectedIndex;
                                int shl = (int)Invoke(DelegateIndexReturn, comboBoxPlume10);
                                //if (shl == 1 || shl == 2)
                                //{
                                //    shl++;
                                //}
                                int zas;
                                if (ZASFFradiobutton10.Checked)
                                {
                                    zas = 0xff;
                                }
                                else
                                {
                                    zas = Convert.ToInt32(ZAS2radiobutton10.Checked);
                                }
                                int PPRCh = Convert.ToInt32(pprchOn10.Checked);
                                int TS = Convert.ToInt32(interfacetype10.SelectedIndex);
                                int KVT = Convert.ToInt16(kvitOn10.Checked);
                                int Tiv = Convert.ToInt32(Invoke(DelegateTextReturn, TB_Tiv10));
                                NewgeneratingMessage10(channel, sk, shl, zas, PPRCh, TS, KVT, Tiv);
                                break;
                            case 3:
                                int chch = Convert.ToInt32(Invoke(DelegateTextReturn, сhchtextbox11));
                                int mm = Convert.ToInt32(Invoke(DelegateTextReturn, mmtextbox11));
                                int ss = Convert.ToInt32(Invoke(DelegateTextReturn, sstextbox11));
                                NewgeneratingMessage11(chch, mm, ss);
                                break;
                            case 4:
                                byte[] zas1 = functions.ZAS(Convert.ToInt16(zas1Chaika12.Checked));
                                byte[] zas2 = functions.ZAS(Convert.ToInt16(zas2Chaika12.Checked));
                                NewgeneratingMessage12(Convert.ToInt32(zas1[0]), Convert.ToInt32(zas2[0]));
                                break;
                            case 5:
                                NewgeneratingMessage13();
                                break;
                            case 6:
                                NewgeneratingMessage14();
                                break;
                            case 7:
                                byte[,] generategCustomizationMas = GenerateCustomizationForRD();
                                TuningRadiostations(generategCustomizationMas);

                                Task.Factory.StartNew(() => NewgeneratingMessage16(generategCustomizationMas));
                                break;
                            case 8:
                                NewgeneratingMessage255();
                                break;
                            case 9:
                                Task.Factory.StartNew(() => PerevalHandTest());
                                break;
                        }


                        #endregion
                    }
                    else if (mode)//Заранее подготовленный тест, согласно выбранному типу(Воообще это скорее отладочные тесты, основные тесты запускается из панели меню)
                    {
                        #region AutoTest
                        //StreamWriter ws = new StreamWriter(writePath, false);
                        //ws.Close();
                        int channelFirst = Convert.ToInt32(Invoke(DelegateTextReturn, numberOfChannelFirstTB));
                        int channelLast = Convert.ToInt32(Invoke(DelegateTextReturn, numberOfChannelLastTB));
                        if (typeOfAutoTestPerevalRB.Checked)//Канальный тест
                        {
                            perevalMode = true;
                            if (!IndependentMode.Checked)
                            {
                                startTest = new Thread(() => PerevalStandartTest(channelFirst, channelLast, true));
                            }
                            else
                            {
                                startTest = new Thread(() => PerevalMasterMixedTest());
                                //PerevalStandartTest(channelFirst, channelLast, true));
                            }

                            startTest.Start();
                        }
                        else if (typeOfAutoTestEthernetRB.Checked)// Ethernet тест, с радиостанциями
                        {
                            ethernetMode = true;
                            if (!IndependentMode.Checked)
                            {
                                startTest = new Thread(() => EthernetStandartTest());
                            }
                            else
                            {
                                startTest = new Thread(() => EthernetMasterMixedTest());

                            }
                            startTest.Start();
                        }
                        else if (typeOfAutoTestPerfomanceTestRB.Checked)//Тест производительности 
                        {
                            perfomanceMode = true;
                            startTest = new Thread(() => PerfomanceTest());
                            startTest.Start();
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public void PerevalHandTest()//Это собстно говоря и есть ручной "Макрос"
        {
            BringTextFromFormComponent SetDelegateText = ReturnSelectedText;
            BringIndexFromFormComponent SetDelegate = ReturnSelectedIndex;
            ;
            string commandString = (string)Invoke(SetDelegateText, MakrosTextBox);
            string[] commands = commandString.Split(',');
            UInt16 timeOut = Convert.ToUInt16(timeOutTB.Text);
            foreach (string str in commands)
            {
                try
                {
                    tokenTestStop.ThrowIfCancellationRequested();
                    handMessageReadyToSend = false;
                    switch (str)
                    {
                        case "1":
                            {
                                RecordToRTB(Color.Blue, "Команда номер 1");
                                byte nz;
                                if (ZasNomer2RB1.Checked)
                                {
                                    nz = 0x01;
                                }
                                else if (ZasFFRB1.Checked)
                                {
                                    nz = 0xFF;
                                }
                                else
                                {
                                    nz = 0x00;
                                }
                                int prg = Convert.ToInt32(Invoke(SetDelegateText, PRGtextbox1));
                                byte[] rp = functions.RP((int)Invoke(SetDelegate, ProtocoltypeChannel1All));
                                int np = Convert.ToInt16(npOnButton1.Checked);
                                int dp = Convert.ToInt32(Invoke(SetDelegateText, DPtextbox1));
                                int channel = (1 * Convert.ToInt16(checkBoxChannel1.Checked) + 2 * Convert.ToInt16(checkBoxChannel2.Checked) + 4 * Convert.ToInt16(checkBoxChannel3.Checked)
                                               + 8 * Convert.ToInt16(checkBoxChannel4.Checked) + 16 * Convert.ToInt16(checkBoxChannel5.Checked) + 32 * Convert.ToInt16(checkBoxChannel6.Checked)
                                               + 64 * Convert.ToInt16(checkBoxChannel7.Checked) + 128 * Convert.ToInt16(checkBoxChannel8.Checked));
                                int lengthOfMessage = (int)Invoke(SetDelegate, perevalLengthOfmessage);
                                RecordToRTB(Color.Blue, "отправка сообщения");
                                NewgeneratingMessage1(nz, prg, Convert.ToInt32(rp[0]), np, dp, channel, lengthOfMessage, 16);
                                break;
                            }
                        case "10":
                            {
                                RecordToRTB(Color.Blue, "Команда номер 10");
                                int channel = (1 * Convert.ToInt16(checkBoxChannel1.Checked) + 2 * Convert.ToInt16(checkBoxChannel2.Checked) + 4 * Convert.ToInt16(checkBoxChannel3.Checked)
           + 8 * Convert.ToInt16(checkBoxChannel4.Checked) + 16 * Convert.ToInt16(checkBoxChannel5.Checked) + 32 * Convert.ToInt16(checkBoxChannel6.Checked) + 64 * Convert.ToInt16(checkBoxChannel7.Checked)
           + 128 * Convert.ToInt16(checkBoxChannel8.Checked));
                                int sk = (int)Invoke(SetDelegate, сomboBoxSpeedCode10);
                                int shl = (int)Invoke(SetDelegate, comboBoxPlume10);
                                //if (shl == 1 || shl == 2)
                                //{
                                //    shl++;
                                //}
                                int zas;
                                if (ZASFFradiobutton10.Checked)
                                {
                                    zas = 255;
                                }
                                else
                                {
                                    zas = Convert.ToInt32(ZAS2radiobutton10.Checked);
                                }
                                int PPRCh = Convert.ToInt32(pprchOn10.Checked);
                                int TS = (int)Invoke(SetDelegate, interfacetype10);
                                int KVT = Convert.ToInt16(kvitOn10.Checked);
                                int Tiv = Convert.ToInt32(Invoke(DelegateTextReturn, TB_Tiv10));
                                NewgeneratingMessage10(channel, sk, shl, zas, PPRCh, TS, KVT, Tiv);
                                break;
                            }
                        case "11":
                            {
                                RecordToRTB(Color.Blue, "Команда номер 11");
                                int chch = Convert.ToInt32(Invoke(SetDelegateText, сhchtextbox11));
                                int mm = Convert.ToInt32(Invoke(SetDelegateText, mmtextbox11));
                                int ss = Convert.ToInt32(Invoke(SetDelegateText, sstextbox11));
                                NewgeneratingMessage11(chch, mm, ss);
                                break;
                            }
                        case "12":
                            {
                                RecordToRTB(Color.Blue, "Команда номер 12");
                                byte[] zas1 = functions.ZAS(Convert.ToInt16(zas1Chaika12.Checked));
                                byte[] zas2 = functions.ZAS(Convert.ToInt16(zas2Chaika12.Checked));
                                NewgeneratingMessage12(Convert.ToInt32(zas1[0]), Convert.ToInt32(zas2[0]));
                                break;
                            }
                        case "13":
                            {
                                RecordToRTB(Color.Blue, "Команда номер 13");
                                NewgeneratingMessage13();
                                break;
                            }
                        case "14":
                            {
                                RecordToRTB(Color.Blue, "Команда номер 13");
                                NewgeneratingMessage14();
                                break;
                            }
                        case "16":
                            {
                                RecordToRTB(Color.Blue, "Команда номер 16");

                                NewgeneratingMessage16(GenerateCustomizationForRD());
                                break;
                            }
                        default:
                            break;
                    }
                    while (!handMessageReadyToSend)
                    {
                        tokenTestStop.ThrowIfCancellationRequested();
                        if (stopwatchHandTest.ElapsedMilliseconds > timeOut)
                        {
                            stopwatchHandTest.Stop();
                            RecordToRTB(Color.Red, "{0}: {1}", "timeOut of message", Convert.ToString(stopwatchHandTest.ElapsedMilliseconds));
                            handMessageReadyToSend = true;
                        }
                        Thread.Sleep(1);
                    }
                    //Thread.Sleep(20);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Convert.ToString(ex));
                }
            }
        }
        //функции для делегатов, для получение данных из элементов формы
        private int ReturnSelectedIndex(Object obj)
        {
            if (obj is ComboBox)
            {
                return ((ComboBox)obj).SelectedIndex;
            }
            return -1;
        }
        private string ReturnSelectedText(Object obj)
        {
            if (obj is TextBox)
            {
                return ((TextBox)obj).Text;
            }
            else if (obj is RichTextBox)
            {
                return ((RichTextBox)obj).Text;
            }
            else if (obj is DomainUpDown)
            {
                return ((DomainUpDown)obj).Text;
            }
            else if (obj is CheckBox)
            {
                return ((CheckBox)obj).Checked.ToString();
            }
            else if (obj is RadioButton)
            {
                return ((RadioButton)obj).Checked.ToString();
            }
            else if (obj is ComboBox)
            {
                return ((ComboBox)obj).SelectedIndex.ToString();
            }
            else if (obj is Label)
            {
                return ((Label)obj).Text;
            }
            else
            {
                MessageBox.Show("Незнакомый тип объекта для функции делегата");
                return null;
            }
        }
        private bool Init()
        {
            startPointOfFindInPrimaRTB = 0; //для поиска в логах
            _connectControlToPrimaStarted = false; // пока не используется, но будет для проверки соединения спримой
            controlConnectStop.Cancel();
            stopwatchConnectionCheck.Reset();
            timeOut = Convert.ToUInt16(timeOutTB.Text);
            stopwatchAccept.Reset();
            longPrintFMessages = new byte[12, 2048];
            SkMas = new int[32] { 0, 0, 0, 0, 0, 0, 0, 0,
             0, 0, 0, 0, 0, 0, 0, 0,
             0, 0, 0, 0, 0, 0, 0, 0,
             0, 0, 0, 0, 0, 0, 0, 0};
            TimeOutMas = new int[32] { 7000, 7000, 7000, 7000, 7000, 7000, 7000, 7000,
            7000, 7000, 7000, 7000, 7000, 7000, 7000, 7000,
            7000, 7000, 7000, 7000, 7000, 7000, 7000, 7000,
            7000, 7000, 7000, 7000, 7000, 7000, 7000, 7000};
            for (int i = 0; i < 12; i++)
            {
                flagsOfLongPrintF[i, 0] = flagsOfLongPrintF[i, 1] = 0;
                flagsOfLongPrintF[i, 2] = 16;
            }
            for (int i = 0; i < 32; i++)
            {
                access[i] = true;
                incomeData[i, 0] = 0x0;
                stopwatchMas[i].Reset();
                answerRecieved[i] = true;
                kvitRecieved[i] = true;
                labelConnectedBlock[i] = false;
                amountOfDataOnChannel[i] = 0.0;
            }
            stopwatchFullTest.Restart();
            errorlabels = new Label[32] { errorLabel11, errorLabel12, errorLabel13, errorLabel14,  errorLabel21, errorLabel22, errorLabel23, errorLabel24 ,
                errorLabel31, errorLabel32, errorLabel33, errorLabel34,errorLabel41, errorLabel42, errorLabel43, errorLabel44,
                errorLabel51, errorLabel52, errorLabel53, errorLabel54,errorLabel61, errorLabel62, errorLabel63, errorLabel64,
                errorLabel71, errorLabel72, errorLabel73, errorLabel74,errorLabel81, errorLabel82, errorLabel83, errorLabel84};
            foreach (Label label in errorlabels)
            {
                label.Text = "off";
            }
            {
                testStop.Dispose();
                testStop = new CancellationTokenSource();
                tokenTestStop = testStop.Token;
                controlConnectStop.Dispose();
                controlConnectStop = new CancellationTokenSource();
                controlConnectStopToken = controlConnectStop.Token;
            }
            perevalMode = false;
            ethernetMode = false;
            perfomanceMode = false;
            accepted = true;
            _primaConnectCheck = true;
            if (perfomanceTestMode)
                for (int i = 0; i < 32; i++)
                    BeginInvoke(labelChangingpTC, i, 0);
            perfomanceTestMode = false;
            mode = ModeTestRB2.Checked;
            RepeatedIns.Clear();
            var t = true;
            if (startTest != null)
            {
                startTest.Abort();
            }
            startedRepeat = false;
            startTest = null;
            if (!connected)
            {
                t = CreationConnect(addressPrima.Text, portPrima.Text);
                if (t)
                {
                    Task.Factory.StartNew(() => net.TCPReConnect());
                }
            }
            RTBRedrawList.Clear();
            net.dividedMessageMode = false;
            numberOfDividingByte = 0;
            ChannelOfDivideMessage = 0;
            //**************************************
            settingTest = false;//******************
            probabilityOfBringingTestMode = false;//
            Invoke(labelFunction, probabilityOfBringingCountOfSendedMessagesLabel, "0");
            Invoke(labelFunction, probabilityOfBringingCountOfRecievedMessagesLabel, "0");
            //**************************************
            return t;
        }
        #region Tests
        public void PerevalStandartTest(int numberOfFirstChannel, int numberOfLastChannel, bool cycle)//Стандартный тест для канального режима из данных берутся только ЗАСы и начальный и конечный канал
        {// остальное всё заранее подготовленное
            int j = 0;
            int firstZas10 = 0;
            int secondZas10 = 0;
            try
            {
                int zasCom1;
                if (zasFFCom1Auto.Checked)
                {
                    zasCom1 = 255;
                }
                else if (zas2Com1Auto.Checked && zas1Com1Auto.Checked)
                {
                    zasCom1 = 254;
                }
                else
                {
                    zasCom1 = Convert.ToInt16(zas2Com1Auto.Checked);
                }
                if (zasFFCom10Auto.Checked)
                {
                    firstZas10 = secondZas10 = 255;
                }
                else if (zas2Com10Auto.Checked && zas1Com10Auto.Checked)
                {
                    secondZas10 = 1;
                }
                else
                {
                    firstZas10 = secondZas10 = Convert.ToInt16(zas2Com10Auto.Checked);
                }
                RecordToRTB(Color.DarkOrange, "[{0}] Начало Перевал теста: ", Convert.ToString(stopwatchFullTest.Elapsed));
                do
                {
                    j++;
                    RecordToRTB(Color.DarkOrange, "[{0}] Начало цикла:{1} ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(j));
                    Invoke(labelFunction, CyrcleLabel, "Цикл: " + (j));
                    NewgeneratingMessage12(0, 0);
                    /////////////////////////с1-тг
                    NewgeneratingMessage10(-1, 7, 0, firstZas10, 0, 0, 0, 1, numberOfFirstChannel, numberOfLastChannel);
                    NewgeneratingMessage1(zasCom1, 0, 5, 0, 0, -1, 17, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 6, 0, 0, -1, 17, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 5, 0, 0, -1, 48, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 6, 0, 0, -1, 48, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 5, 0, 0, -1, 80, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 6, 0, 0, -1, 80, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 5, 0, 0, -1, 112, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 6, 0, 0, -1, 112, numberOfFirstChannel, numberOfLastChannel, 8);
                    CheckAllChannelsAnswer();//перед десятой командой желательно дожидаться все ответы, иначе при перенстройке происходит сброс Примы
                    NewgeneratingMessage10(-1, 7, 0, secondZas10, 0, 0, 0, 1, numberOfFirstChannel, numberOfLastChannel);
                    NewgeneratingMessage1(zasCom1, 0, 5, 0, 0, -1, 17, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 6, 0, 0, -1, 17, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 5, 0, 0, -1, 48, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 6, 0, 0, -1, 48, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 5, 0, 0, -1, 80, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 6, 0, 0, -1, 80, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 5, 0, 0, -1, 112, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 6, 0, 0, -1, 112, numberOfFirstChannel, numberOfLastChannel, 8);
                    CheckAllChannelsAnswer();
                    for (int i = 0; i < 8; i++)
                    {
                        NewgeneratingMessage10(-1, i, 0, firstZas10, 0, 0, 0, 1, numberOfFirstChannel, numberOfLastChannel);
                        NewgeneratingMessage1(zasCom1, 0, 5, 0, 0, -1, 17, numberOfFirstChannel, numberOfLastChannel, 1);
                        NewgeneratingMessage1(zasCom1, 0, 6, 0, 0, -1, 17, numberOfFirstChannel, numberOfLastChannel, 1);
                        CheckAllChannelsAnswer();
                    }
                    /////////////////////////с1-фл-би
                    NewgeneratingMessage10(-1, 7, 0, firstZas10, 0, 1, 0, 1, numberOfFirstChannel, numberOfLastChannel);
                    NewgeneratingMessage1(zasCom1, 0, 5, 0, 0, -1, 17, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 6, 0, 0, -1, 17, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 5, 0, 0, -1, 48, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 6, 0, 0, -1, 48, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 5, 0, 0, -1, 80, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 6, 0, 0, -1, 80, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 5, 0, 0, -1, 112, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 6, 0, 0, -1, 112, numberOfFirstChannel, numberOfLastChannel, 8);
                    CheckAllChannelsAnswer();
                    NewgeneratingMessage10(-1, 7, 0, secondZas10, 0, 1, 0, 1, numberOfFirstChannel, numberOfLastChannel);
                    NewgeneratingMessage1(zasCom1, 0, 5, 0, 0, -1, 17, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 6, 0, 0, -1, 17, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 5, 0, 0, -1, 48, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 6, 0, 0, -1, 48, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 5, 0, 0, -1, 80, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 6, 0, 0, -1, 80, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 5, 0, 0, -1, 112, numberOfFirstChannel, numberOfLastChannel, 8);
                    NewgeneratingMessage1(zasCom1, 0, 6, 0, 0, -1, 112, numberOfFirstChannel, numberOfLastChannel, 8);
                    CheckAllChannelsAnswer();
                    for (int i = 3; i < 8; i++)
                    {
                        NewgeneratingMessage10(-1, i, 0, firstZas10, 0, 1, 0, 1, numberOfFirstChannel, numberOfLastChannel);
                        NewgeneratingMessage1(zasCom1, 0, 5, 0, 0, -1, 17, numberOfFirstChannel, numberOfLastChannel, 1);
                        NewgeneratingMessage1(zasCom1, 0, 6, 0, 0, -1, 17, numberOfFirstChannel, numberOfLastChannel, 1);
                        CheckAllChannelsAnswer();
                    }
                    #region 2 комманда
                    //NewgeneratingMessage10(-1, 7, 0, 1, 0, 0, 0, numberOfChannels);

                    //newgeneratingMessage2(0, 0, 5, 0, 0, -1, 16, 81, 10240, numberOfChannels);

                    //NewgeneratingMessage10(-1, 7, 0, 1, 0, 1, 0, numberOfChannels);

                    //newgeneratingMessage2(0, 0, 5, 0, 0, -1, 16, 81, 10240, numberOfChannels);

                    //Thread.Sleep(30);
                    #endregion
                    Thread.Sleep(100);
                    CheckAllChannelsAnswer();
                    new MessageForRTB("[" + Convert.ToString(stopwatchFullTest.Elapsed) + "] ЦИКЛ №" + j + " УСПЕШНО ЗАВЕРШЕН", Color.DarkBlue);
                    for (int acces = 0; acces < 8; acces++)
                    {
                        incomeData[acces, 0] = 0x0;
                        stopwatchMas[acces].Reset();
                    }
                    stopwatchAccept.Reset();
                    accepted = true;
                    Invoke(new NullFunction(RecordToFile));
                    Thread.Sleep(7000);
                    tokenTestStop.ThrowIfCancellationRequested();
                    Invoke(new NullFunction(toolStripCLearPrimaRTB.PerformClick));
                } while (cycle == true);
            }
            catch (MessageException)
            {
                Console.WriteLine("MessageException");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("stop");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //MessageBox.Show(Convert.ToString(ex));
            }
            finally
            {
                for (int acces = 0; acces < 8; acces++)
                {
                    incomeData[acces, 0] = 0x0;
                    stopwatchMas[acces].Reset();
                }
                stopwatchAccept.Reset();
                accepted = true;
                Thread.Sleep(1000);
                Invoke(new NullFunction(RecordToFile));

                //   Invoke(boolFunction, false);
            }
        }
        public void PerevalMasterMixedTest()
        {
            BringIndexFromFormComponent DelegateIndexReturn = ReturnSelectedIndex;
            int firstZas10 = 0;
            int secondZas10 = 0;
            bool randmode = randMode.Checked;
            try
            {
                int zasCom1;
                if (zasFFCom1Auto.Checked)
                {
                    zasCom1 = 255;
                }
                else if (zas2Com1Auto.Checked && zas1Com1Auto.Checked)
                {
                    zasCom1 = 254;
                }
                else
                {
                    zasCom1 = Convert.ToInt16(zas2Com1Auto.Checked);
                }
                if (zasFFCom10Auto.Checked)
                {
                    firstZas10 = secondZas10 = 255;
                }
                else if (zas2Com10Auto.Checked && zas1Com10Auto.Checked)
                {
                    secondZas10 = 1;
                }
                else
                {
                    firstZas10 = secondZas10 = Convert.ToInt16(zas2Com10Auto.Checked);
                }
                int shl = (int)Invoke(DelegateIndexReturn, comboBoxPlume10);

                int TS = (int)Invoke(DelegateIndexReturn, interfacetype10);

                int pprch = Convert.ToInt32(pprchOn10.Checked);
                int kvit = Convert.ToInt32(kvitOn10.Checked);
                int Tiv = Convert.ToInt32(TB_Tiv10.Text);

                timeOutAnswer = Convert.ToUInt16(timeOutTB.Text);
                Task[] tasks = new Task[8];
                //NullFunction Stop_Click = StopTestButton.PerformClick;
                //int n = 0;
                int[] channels = new int[8];
                RecordToRTB(Color.DarkOrange, "[{0}] Начало независимого Перевал теста: ", Convert.ToString(stopwatchFullTest.Elapsed));
                NewgeneratingMessage12(0, 0);

                for (int i = 0; i < 8; i++)
                {
                    if (access[i])
                    {

                        int k = new int();
                        k = i;
                        channels[k] = i;
                        //n = new int();
                        //n = i;
                        tasks[k] = new Task(() => PerevalMixedTest(channels[k], zasCom1, firstZas10, secondZas10, shl, TS, pprch, kvit, Tiv, randmode), tokenTestStop);
                        //tasks[k].Start();
                    }
                }


                for (int i = 0; i < 8; i++)
                {
                    if (access[i])
                    {
                        tasks[i].Start();
                    }
                }

            }
            catch (MessageException)
            {
                Console.WriteLine("MessageException");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("stop");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // MessageBox.Show(Convert.ToString(ex));
            }
            finally
            {
                //for (int acces = 0; acces < 8; acces++)
                //{
                //    incomeData[acces, 0] = 0x0;
                //    stopwatchMas[acces].Reset();
                //}
                //stopwatchAccept.Reset();
                //accepted = true;
                //Thread.Sleep(1000);
                //Invoke(new NullFunction(recordToFile));
            }
        }

        private void PerevalMixedTest(int channel, int zasCom1, int firstZas10, int secondZas10, int shl, int TS, int pprch, int kvit, int Tiv, bool randmode = false)
        {
            int _channel = channel;
            int _zasCom1 = zasCom1;
            int _firstZas10 = firstZas10;
            int _secondZas10 = secondZas10;
            //timeOutAnswer = 4000;
            try
            {
                RecordToRTB(Color.DarkOrange, "[{0}] Начало Теста: Канал {1}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(_channel + 1));
                if (!randmode)
                {

                    while (true)
                    {
                        /////////////////////////с1-тг
                        NewgeneratingMessage10SCh(_channel, 7, shl, _firstZas10, pprch, TS, kvit, Tiv);
                        NewgeneratingMessage1PerevalMixed(_zasCom1, 1, 5, 0, 0, _channel, 112, true, 64);
                        NewgeneratingMessage10SCh(_channel, 7, shl, _secondZas10, pprch, TS, kvit, Tiv);
                        NewgeneratingMessage1PerevalMixed(_zasCom1, 1, 5, 0, 0, _channel, 112, true, 64);
                        //CheckAllChannelsAnswer();
                        //for (int i = 0; i < 4; i++)
                        //{
                        //    newgeneratingMessage10SCh(_channel, i, 0, 0, 0, 0, 0, 1);
                        //newgeneratingMessage1PerevalMixed(_zasCom1, 1, 5, 0, 0, _channel, 112, true, 1);
                        //newgeneratingMessage1PerevalMixed(_zasCom1, 1, 6, 0, 0, _channel, 112, true, 1);
                        //}
                        /////////////////////////с1-фл-би
                        NewgeneratingMessage10SCh(_channel, 7, shl, _firstZas10, pprch, TS, kvit, Tiv);
                        NewgeneratingMessage1PerevalMixed(_zasCom1, 1, 5, 0, 0, _channel, 112, true, 64);
                        NewgeneratingMessage10SCh(_channel, 7, shl, _secondZas10, pprch, TS, kvit, Tiv);
                        NewgeneratingMessage1PerevalMixed(_zasCom1, 1, 5, 0, 0, _channel, 112, true, 64);
                        //}
                        #region 2 комманда
                        //NewgeneratingMessage10(-1, 7, 0, 1, 0, 0, 0, numberOfChannels);
                        //newgeneratingMessage2(0, 0, 5, 0, 0, -1, 16, 81, 10240, numberOfChannels);
                        //NewgeneratingMessage10(-1, 7, 0, 1, 0, 1, 0, numberOfChannels);
                        //newgeneratingMessage2(0, 0, 5, 0, 0, -1, 16, 81, 10240, numberOfChannels);
                        //Thread.Sleep(30);
                        #endregion
                        //Thread.Sleep(500);
                    }
                }
                else if (randmode)
                {
                    NewgeneratingMessage10SCh(_channel, 7, shl, _firstZas10, pprch, TS, kvit, Tiv);
                    while (true)
                    {
                        NewgeneratingMessage1PerevalMixed(_zasCom1, 1, 0, 0, 0, _channel, 0, true, 256);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                //MessageBox.Show(Convert.ToString(ex));
                Console.WriteLine("stop");
                Console.ReadKey();
            }
            catch (MessageException)
            {
                for (int acces = 0; acces < 8; acces++)
                {
                    incomeData[acces, 0] = 0x0;
                    stopwatchMas[acces].Reset();
                }
                stopwatchAccept.Reset();
                accepted = true;
                // StartTestButton.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(Convert.ToString(ex));
                StartTestButton.Enabled = true;
            }
            finally
            {
                //Thread.Sleep(1000);
                //Invoke(new NullFunction(RecordToFile));
                //Invoke(boolFunction, false);
            }
        }
        public void EthernetMasterMixedTest()
        {
            EthernetAdresses = Ethernetadressesgeneration();
            try
            {
                int zasCom1;
                bool randmode = randMode.Checked;
                if (zasFFCom1Auto.Checked)
                {
                    zasCom1 = 255;
                }
                else if (zas2Com1Auto.Checked && zas1Com1Auto.Checked)
                {
                    zasCom1 = 254;
                }
                else
                {
                    zasCom1 = Convert.ToInt16(zas2Com1Auto.Checked);
                }
                byte[,] generategCustomizationMas = GenerateCustomizationForRD();
                int numberOfRadiostations = EthernetAdresses.Length / 24;
                RecordToRTB(Color.DarkOrange, "[{0}] Начало независимого Ethetnet теста: ", Convert.ToString(stopwatchFullTest.Elapsed));
                TuningRadiostations(generategCustomizationMas);
                timeOutAnswer = Convert.ToUInt16(timeOutTB.Text);
                Task[] tasks = new Task[numberOfRadiostations];
                //byte[] temporaryMas = new byte[12 * 8];
                int[] m = new int[numberOfRadiostations];
                //NullFunction Stop_Click = StopTestButton.PerformClick;
                int[] numberOfEthernetAdress = new int[numberOfRadiostations];
                int j = 0;
                for (int i = 0; i < numberOfRadiostations; i++)
                {
                    for (; j < RadioStationcheckboxes.Length; j++)
                    {
                        if (RadioStationcheckboxes[j].Checked)
                        {

                            int k = new int();
                            k = i;
                            m[k] = j;
                            numberOfEthernetAdress[k] = i;
                            //tasks[i] = new Task(() => EthernetMixedTest(temporaryMas[i], m[i], zasCom1));
                            tasks[k] = new Task(() => EthernetMixedTest(numberOfEthernetAdress[k], m[k], zasCom1, randmode), tokenTestStop);
                            j++;
                            break;
                        }
                    }
                }
                for (int i = 0; i < 8; i++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        if (RadioStationcheckboxes[i * 4 + k].Checked)
                        {
                            NewgeneratingMessage10(-1, 0, 0, 0, 0, 2, 0, 1, i + 1, i + 1, true);
                            break;
                        }
                    }
                }
                NewgeneratingMessage16(generategCustomizationMas);
                NewgeneratingMessage12(0, 0);
                for (int channel = 0; channel < numberOfRadiostations; channel++)
                {
                    if (access[channel])
                    {
                        tasks[channel].Start();
                    }
                }
                #endregion
            }
            catch (MessageException ex)
            {
                Console.WriteLine(ex.Message);
                //   StartTestButton.Enabled = true;
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine("stop");
                Console.ReadKey();
                //  StartTestButton.Enabled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //MessageBox.Show(Convert.ToString(ex));
            }
        }
        private void EthernetMixedTest(int numberOfEthernetAdress, int numberOfRadiostations, int zasCom1, bool randmode)
        {
            int _numberOfRadiostations = numberOfRadiostations;
            int _zasCom1 = zasCom1;
            byte[] currentMas = new byte[12];
            Buffer.BlockCopy(EthernetAdresses, numberOfEthernetAdress * 24, currentMas, 0, 12);
            timeOutAnswer = 7000;
            try
            {
                RecordToRTB(Color.DarkOrange, "[{0}] Начало Теста: Радиостанция {1}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(_numberOfRadiostations + 1));
                if (randmode)
                {
                    while (true)
                    {
                        NewgeneratingMessage1EthernetMixed(currentMas, _zasCom1, 1, 5, 0, 0, _numberOfRadiostations, 112, true, 64);
                        #region 2 комманда
                        //NewgeneratingMessage10(-1, 7, 0, 1, 0, 0, 0, numberOfChannels);
                        //newgeneratingMessage2(0, 0, 5, 0, 0, -1, 16, 81, 10240, numberOfChannels);
                        //NewgeneratingMessage10(-1, 7, 0, 1, 0, 1, 0, numberOfChannels);
                        //newgeneratingMessage2(0, 0, 5, 0, 0, -1, 16, 81, 10240, numberOfChannels);
                        //Thread.Sleep(30);
                        #endregion
                    }
                }
                else
                {
                    while (true)
                    {
                        NewgeneratingMessage1EthernetMixed(currentMas, _zasCom1, 1, 5, 0, 0, _numberOfRadiostations, 17, false, 16);
                        NewgeneratingMessage1EthernetMixed(currentMas, _zasCom1, 1, 5, 0, 0, _numberOfRadiostations, 48, false, 16);
                        NewgeneratingMessage1EthernetMixed(currentMas, _zasCom1, 1, 5, 0, 0, _numberOfRadiostations, 80, false, 16);
                        NewgeneratingMessage1EthernetMixed(currentMas, _zasCom1, 1, 5, 0, 0, _numberOfRadiostations, 112, false, 16);
                        #region 2 комманда
                        //NewgeneratingMessage10(-1, 7, 0, 1, 0, 0, 0, numberOfChannels);
                        //newgeneratingMessage2(0, 0, 5, 0, 0, -1, 16, 81, 10240, numberOfChannels);
                        //NewgeneratingMessage10(-1, 7, 0, 1, 0, 1, 0, numberOfChannels);
                        //newgeneratingMessage2(0, 0, 5, 0, 0, -1, 16, 81, 10240, numberOfChannels);
                        //Thread.Sleep(30);
                        #endregion
                    }
                }
            }
            catch (OperationCanceledException)
            {
                //MessageBox.Show(Convert.ToString(ex));
                Console.WriteLine("stop");
                Console.ReadKey();
            }
            catch (MessageException ex)
            {
                Console.WriteLine(ex.Message + "РС" + (numberOfRadiostations + 1));
                for (int acces = 0; acces < 32; acces++)
                {
                    incomeData[acces, 0] = 0x0;
                    stopwatchMas[acces].Reset();
                }
                stopwatchAccept.Reset();
                accepted = true;
                // StartTestButton.Enabled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //MessageBox.Show(Convert.ToString(ex));
                StartTestButton.Enabled = true;
            }
            finally
            {
                //Thread.Sleep(1000);
                //Invoke(new NullFunction(RecordToFile));
                //Invoke(boolFunction, false);
            }
        }

        public void EthernetRSTest()//Тест радиостанций, существует лишь для того, чтобы проверить что они хоть работают 
        {
            EthernetAdresses = Ethernetadressesgeneration();
            try
            {
                int zasCom1;
                if (zasFFCom1Auto.Checked)
                {
                    zasCom1 = 255;
                }
                else if (zas2Com1Auto.Checked && zas1Com1Auto.Checked)
                {
                    zasCom1 = 254;
                }
                else
                {
                    zasCom1 = Convert.ToInt16(zas2Com1Auto.Checked);
                }
                int zasCom10;
                if (zasFFCom10Auto.Checked)
                {
                    zasCom10 = 255;
                }
                else
                {
                    zasCom10 = Convert.ToInt16(zas2Com10Auto.Checked);
                }
                int j = 0;

                RecordToRTB(Color.DarkOrange, "[{0}] Начало теста радиостанций: ", Convert.ToString(stopwatchFullTest.Elapsed));
                Invoke(labelFunction, CyrcleLabel, "Ethernet ");
                j++;
                RecordToRTB(Color.DarkOrange, "[{0}] Начало цикла:{1} ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(j));
                Invoke(labelFunction, CyrcleLabel, "Цикл: " + (j));
                NewgeneratingMessage10(-1, 0, 0, zasCom10, 0, 2, 0, 1, 1, 8);
                NewgeneratingMessage12(0, 0);
                NewgeneratingMessage16RSTest(GenerateCustomizationForRD());
                Thread.Sleep(1000);
                NewgeneratingMessage1EthRSTest(zasCom1, 0, 5, 0, 0, 17, 1);
                NewgeneratingMessage1EthRSTest(zasCom1, 0, 5, 0, 0, 48, 1);
                NewgeneratingMessage1EthRSTest(zasCom1, 0, 5, 0, 0, 80, 1);
                NewgeneratingMessage1EthRSTest(zasCom1, 0, 5, 0, 0, 112, 1);
                CheckAllChannelsAnswer(true);
                for (int acces = 0; acces < 32; acces++)
                {
                    stopwatchMas[acces].Reset();
                }
                stopwatchAccept.Reset();
                accepted = true;
                Thread.Sleep(2000);
                new MessageForRTB("[" + Convert.ToString(stopwatchFullTest.Elapsed) + "] Тест " + j + " УСПЕШНО ЗАВЕРШЕН", Color.DarkBlue);
            }
            catch (MessageException)
            {
                for (int acces = 0; acces < 8; acces++)
                {
                    incomeData[acces, 0] = 0x0;
                    stopwatchMas[acces].Reset();
                }
                stopwatchAccept.Reset();
                accepted = true;
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine("stop");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(Convert.ToString(ex));
            }
            finally
            {
                Thread.Sleep(1000);
                Invoke(new NullFunction(RecordToFile));
                //   Invoke(boolFunction, false);
            }
        }
        public void EthernetStandartTest()//Тест для отладки радиостанций по факту берет тоже ЗАСы, но также поддерживает randmode, который позволяет генерировать сообщения случайной блочности и Режима передачи
        {
            EthernetAdresses = Ethernetadressesgeneration();
            try
            {
                int zasCom1;
                int firstZas10 = 0;
                int secondZas10 = 0;
                bool randmode = randMode.Checked;
                if (zasFFCom1Auto.Checked)
                {
                    zasCom1 = 255;
                }
                else if (zas2Com1Auto.Checked && zas1Com1Auto.Checked)
                {
                    zasCom1 = 254;
                }
                else
                {
                    zasCom1 = Convert.ToInt16(zas2Com1Auto.Checked);
                }
                if (zasFFCom10Auto.Checked)
                {
                    firstZas10 = secondZas10 = 255;
                }
                else if (zas2Com10Auto.Checked && zas1Com10Auto.Checked)
                {
                    secondZas10 = 1;
                }
                else
                {
                    firstZas10 = secondZas10 = Convert.ToInt16(zas2Com10Auto.Checked);
                }
                int j = 0;
                byte[,] generategCustomizationMas = GenerateCustomizationForRD();

                RecordToRTB(Color.DarkOrange, "[{0}] Начало Ethetnet теста: ", Convert.ToString(stopwatchFullTest.Elapsed));
                Invoke(labelFunction, CyrcleLabel, "Ethernet ");
                TuningRadiostations(generategCustomizationMas);
                while (true)
                {
                    j++;
                    RecordToRTB(Color.DarkOrange, "[{0}] Начало цикла:{1} ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(j));
                    Invoke(labelFunction, CyrcleLabel, "Цикл: " + (j));
                    for (int i = 0; i < 8; i++)
                    {
                        for (int k = 0; k < 4; k++)
                        {
                            if (RadioStationcheckboxes[i * 4 + k].Checked)
                            {
                                NewgeneratingMessage10(-1, 0, 0, firstZas10, 0, 2, 0, 1, i + 1, i + 1, true);
                                break;
                            }
                        }
                    }
                    NewgeneratingMessage16(generategCustomizationMas);
                    NewgeneratingMessage12(0, 0);
                    if (!randmode)
                    {
                        NewgeneratingMessage1Eth(zasCom1, 0, 5, 0, 0, 17, 8);
                        NewgeneratingMessage1Eth(zasCom1, 0, 6, 0, 0, 17, 8);
                        NewgeneratingMessage1Eth(zasCom1, 0, 5, 0, 0, 48, 8);
                        NewgeneratingMessage1Eth(zasCom1, 0, 6, 0, 0, 48, 8);
                        NewgeneratingMessage1Eth(zasCom1, 0, 5, 0, 0, 80, 8);
                        NewgeneratingMessage1Eth(zasCom1, 0, 6, 0, 0, 80, 8);
                        NewgeneratingMessage1Eth(zasCom1, 0, 5, 0, 0, 112, 8);
                        NewgeneratingMessage1Eth(zasCom1, 0, 6, 0, 0, 112, 8);
                    }
                    else
                    {
                        NewgeneratingMessage1Eth(zasCom1, 0, 0, 0, 0, 0, 128, true);
                    }
                    CheckAllChannelsAnswer(true);
                    for (int i = 0; i < 8; i++)
                    {
                        for (int k = 0; k < 4; k++)
                        {
                            if (RadioStationcheckboxes[i * 4 + k].Checked)
                            {
                                NewgeneratingMessage10(-1, 0, 0, secondZas10, 0, 2, 0, 1, i + 1, i + 1, true);
                                break;
                            }
                        }
                    }
                    NewgeneratingMessage16(generategCustomizationMas);
                    NewgeneratingMessage12(0, 0);
                    if (!randmode)
                    {
                        NewgeneratingMessage1Eth(zasCom1, 0, 5, 0, 0, 17, 8);
                        NewgeneratingMessage1Eth(zasCom1, 0, 6, 0, 0, 17, 8);
                        NewgeneratingMessage1Eth(zasCom1, 0, 5, 0, 0, 48, 8);
                        NewgeneratingMessage1Eth(zasCom1, 0, 6, 0, 0, 48, 8);
                        NewgeneratingMessage1Eth(zasCom1, 0, 5, 0, 0, 80, 8);
                        NewgeneratingMessage1Eth(zasCom1, 0, 6, 0, 0, 80, 8);
                        NewgeneratingMessage1Eth(zasCom1, 0, 5, 0, 0, 112, 8);
                        NewgeneratingMessage1Eth(zasCom1, 0, 6, 0, 0, 112, 8);
                    }
                    else
                    {
                        NewgeneratingMessage1Eth(zasCom1, 0, 0, 0, 0, 0, 128, true);
                    }
                    CheckAllChannelsAnswer(true);
                    new MessageForRTB("[" + Convert.ToString(stopwatchFullTest.Elapsed) + "] ЦИКЛ №" + j + " УСПЕШНО ЗАВЕРШЕН", Color.DarkBlue);
                    Invoke(new NullFunction(RecordToFile));
                    //Thread.Sleep(2000);
                    Invoke(new NullFunction(toolStripCLearPrimaRTB.PerformClick));
                }
                #region 2 комманда
                //NewgeneratingMessage10(-1, 7, 0, 1, 0, 0, 0, numberOfChannels);
                //newgeneratingMessage2(0, 0, 5, 0, 0, -1, 16, 81, 10240, numberOfChannels);
                //NewgeneratingMessage10(-1, 7, 0, 1, 0, 1, 0, numberOfChannels);
                //newgeneratingMessage2(0, 0, 5, 0, 0, -1, 16, 81, 10240, numberOfChannels);
                //Thread.Sleep(30);
                #endregion
            }
            catch (MessageException)
            {
                //   StartTestButton.Enabled = true;
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine("stop");
                Console.ReadKey();
                //  StartTestButton.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(Convert.ToString(ex));
            }
            finally
            {
                for (int acces = 0; acces < 8; acces++)
                {
                    stopwatchMas[acces].Reset();
                }
                stopwatchAccept.Reset();
                accepted = true;
                Invoke(new NullFunction(RecordToFile));
                // Invoke(boolFunction, false);
            }
        }
        public void SettingTest(bool cycle, int sk, int ts, int zas1, int zas10, int zas12)//а это вообще странное чудо, "настроечный тест", который лишь единожды дает настройки, а потом гоняет сообщения до посинения
        {
            int j = 0;
            settingTest = true;
            try
            {
                RecordToRTB(Color.DarkOrange, "[{0}] Начало установки режима: ", Convert.ToString(stopwatchFullTest.Elapsed));
                NewgeneratingMessage10(-1, sk, 0, zas10, 0, ts, 0, 1);
                NewgeneratingMessage12(zas12, zas12);
                Thread.Sleep(1000);
                mode = false;
                while (cycle == true)
                {
                    j++;
                    RecordToRTB(Color.DarkOrange, "[{0}] Начало цикла:{1} ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(j));
                    Invoke(labelFunction, CyrcleLabel, "Цикл: " + (j));
                    NewgeneratingMessage1(zas1, 0, 6, 0, 10, -1, 112, 1, 8, 1);
                    CheckAllChannelsAnswer();
                    stopwatchAccept.Reset();
                    new MessageForRTB("[" + Convert.ToString(stopwatchFullTest.Elapsed) + "] ЦИКЛ №" + j + " УСПЕШНО ЗАВЕРШЕН", Color.DarkBlue);
                    Thread.Sleep(1);
                    Invoke(new NullFunction(toolStripCLearPrimaRTB.PerformClick));
                }
            }
            catch (MessageException)
            {
            }
            catch (OperationCanceledException)
            {
                //MessageBox.Show(Convert.ToString(ex));
                Console.WriteLine("stop");
            }
            catch (Exception ex)
            {
                MessageBox.Show(Convert.ToString(ex));
            }
            finally
            {
                Thread.Sleep(1000);
                Invoke(new NullFunction(RecordToFile));
                //   Invoke(boolFunction, false);
            }
        }
        public void ProbabilityOfBringingTest(int numberOfChannel, int rp)//Этот тест отображает кпд примы, иначе говоря, количество отработанных сообщений
        {
            probabilityOfBringingTestMode = true;
            mode = true;
            try
            {
                RecordToRTB(Color.DarkOrange, "[{0}] Начало Перевал теста: ", Convert.ToString(stopwatchFullTest.Elapsed));
                NewgeneratingMessage12(0, 0);
                NewgeneratingMessage10(-1, 5, 0, 0, 0, 0, 0, 1, numberOfChannel + 1, numberOfChannel + 1);
                NewgeneratingMessage1probabilityOfBringing(rp, numberOfChannel);
                Thread.Sleep(1000);
                while (incomeData[numberOfChannel, 0] != 0x0)
                    Thread.Sleep(1);
                incomeData[numberOfChannel, 0] = 0x0;
                stopwatchMas[numberOfChannel].Reset();
                stopwatchAccept.Reset();
                accepted = true;
                new MessageForRTB("[" + Convert.ToString(stopwatchFullTest.Elapsed) + "] Тест Завершен", Color.DarkBlue);
                MessageBox.Show("Вероятность доведения равна " + Convert.ToString((float)((Convert.ToDouble(probabilityOfBringingCountOfRecievedMessagesLabel.Text) / Convert.ToDouble(probabilityOfBringingCountOfSendedMessagesLabel.Text)) * 100)) + "%", "Результат теста");
                //вероятно поведения
            }
            catch (MessageException)
            {
            }
            catch (OperationCanceledException)
            {
                //MessageBox.Show(Convert.ToString(ex));
                Console.WriteLine("stop");
            }
            catch (Exception ex)
            {
                MessageBox.Show(Convert.ToString(ex));
            }
            finally
            {
                Thread.Sleep(1000);
                Invoke(new NullFunction(RecordToFile));
                //  Invoke(boolFunction, false);
            }
        }
        public void GeneralTest(int numberOfFirstChannel, int numberOfLastChannel, bool TG, bool FL, bool WithoutAnswer, bool WithAnswer)
        {
            ///А ЭТО основной тест, именно для полного теста всей Примы, в окошке, которое возникнет, можно выбрать, какие тесты НЕ нужны
            int j = 0;
            int firstZas10 = 0;
            int secondZas10 = 1;
            byte[,] generategCustomizationMas = GenerateCustomizationForRD();
            try
            {
                int zasCom1 = 255;
                RecordToRTB(Color.DarkOrange, "[{0}] Начало Общего теста: ", Convert.ToString(stopwatchFullTest.Elapsed));
                do
                {
                    j++;
                    RecordToRTB(Color.DarkOrange, "[{0}] Начало цикла:{1} ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(j));
                    if (TG)
                    {
                        Invoke(labelFunction, CyrcleLabel, "Цикл: " + (j) + " ТГ");
                        NewgeneratingMessage12(0, 0);
                        /////////////////////////с1-тг
                        NewgeneratingMessage10(-1, 7, 0, firstZas10, 0, 0, 0, 1, numberOfFirstChannel, numberOfLastChannel);
                        NewgeneratingMessage1(zasCom1, 0, 0, 0, 0, -1, 0, numberOfFirstChannel, numberOfLastChannel, 64, true);
                        CheckAllChannelsAnswer();
                        NewgeneratingMessage10(-1, 7, 0, secondZas10, 0, 0, 0, 1, numberOfFirstChannel, numberOfLastChannel);
                        NewgeneratingMessage1(zasCom1, 0, 0, 0, 0, -1, 0, numberOfFirstChannel, numberOfLastChannel, 64, true);
                        CheckAllChannelsAnswer();
                        for (int i = 0; i < 8; i++)
                        {
                            NewgeneratingMessage10(-1, i, 0, firstZas10 ^= 1, 0, 0, 0, 1, numberOfFirstChannel, numberOfLastChannel);
                            NewgeneratingMessage1(zasCom1, 0, 5, 0, 0, -1, 17, numberOfFirstChannel, numberOfLastChannel, 1);
                            NewgeneratingMessage1(zasCom1, 0, 6, 0, 0, -1, 17, numberOfFirstChannel, numberOfLastChannel, 1);
                            CheckAllChannelsAnswer();
                        }
                        Thread.Sleep(3000);
                        Invoke(new NullFunction(toolStripCLearPrimaRTB.PerformClick));
                    }
                    //////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///
                    //////////////////////////////////////////////////////////////////////////////////////////////////////
                    if (FL)
                    {
                        Invoke(labelFunction, CyrcleLabel, "Цикл: " + (j) + " ФЛ");
                        /////////////////////////с1-фл-би
                        NewgeneratingMessage12(0, 0);
                        NewgeneratingMessage10(-1, 7, 0, firstZas10, 0, 1, 0, 1, numberOfFirstChannel, numberOfLastChannel);
                        NewgeneratingMessage1(zasCom1, 0, 0, 0, 0, -1, 0, numberOfFirstChannel, numberOfLastChannel, 64, true);
                        CheckAllChannelsAnswer();
                        NewgeneratingMessage10(-1, 7, 0, secondZas10, 0, 1, 0, 1, numberOfFirstChannel, numberOfLastChannel);
                        NewgeneratingMessage1(zasCom1, 0, 0, 0, 0, -1, 0, numberOfFirstChannel, numberOfLastChannel, 64, true);
                        CheckAllChannelsAnswer();
                        for (int i = 3; i < 8; i++)
                        {
                            NewgeneratingMessage10(-1, i, 0, firstZas10 ^= 1, 0, 1, 0, 1, numberOfFirstChannel, numberOfLastChannel);
                            NewgeneratingMessage1(zasCom1, 0, 5, 0, 0, -1, 17, numberOfFirstChannel, numberOfLastChannel, 1);
                            NewgeneratingMessage1(zasCom1, 0, 6, 0, 0, -1, 17, numberOfFirstChannel, numberOfLastChannel, 1);
                            CheckAllChannelsAnswer();
                        }
                        Thread.Sleep(3000);
                        Invoke(new NullFunction(toolStripCLearPrimaRTB.PerformClick));
                    }
                    //////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///
                    //////////////////////////////////////////////////////////////////////////////////////////////////////
                    if (WithoutAnswer)
                    {
                        for (int i = 0; i < 32; i++)
                        {
                            generategCustomizationMas[i, 6] = 0;
                        }
                        Invoke(labelFunction, CyrcleLabel, "Цикл: " + (j) + " Eth без ответа");
                        TuningRadiostations(generategCustomizationMas);
                        for (int i = 0; i < 8; i++)
                        {
                            for (int k = 0; k < 4; k++)
                            {

                                if (RadioStationcheckboxes[i * 4 + k].Checked)
                                {
                                    NewgeneratingMessage10(-1, 0, 0, firstZas10, 0, 2, 0, 1, i + 1, i + 1, true);
                                    break;
                                }
                            }
                        }
                        NewgeneratingMessage16(generategCustomizationMas);
                        NewgeneratingMessage12(0, 0);
                        NewgeneratingMessage1Eth(zasCom1, 0, 0, 0, 0, 0, 128, true);
                        CheckAllChannelsAnswer(true);
                        for (int i = 0; i < 8; i++)
                        {
                            for (int k = 0; k < 4; k++)
                            {
                                if (RadioStationcheckboxes[i * 4 + k].Checked)
                                {
                                    NewgeneratingMessage10(-1, 0, 0, secondZas10, 0, 2, 0, 1, i + 1, i + 1, true);
                                    break;
                                }
                            }
                        }
                        NewgeneratingMessage1Eth(zasCom1, 0, 0, 0, 0, 0, 128, true);
                        CheckAllChannelsAnswer(true);
                        Thread.Sleep(3000);
                        Invoke(new NullFunction(toolStripCLearPrimaRTB.PerformClick));
                    }
                    //////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///
                    //////////////////////////////////////////////////////////////////////////////////////////////////////
                    if (WithAnswer)
                    {
                        Invoke(labelFunction, CyrcleLabel, "Цикл: " + (j) + " Eth с ответом");
                        for (int i = 0; i < 32; i++)
                        {
                            generategCustomizationMas[i, 6] = 1;
                        }
                        TuningRadiostations(generategCustomizationMas);
                        for (int i = 0; i < 8; i++)
                        {
                            for (int k = 0; k < 4; k++)
                            {
                                if (RadioStationcheckboxes[i * 4 + k].Checked)
                                {
                                    NewgeneratingMessage10(-1, 0, 0, firstZas10, 0, 2, 0, 1, i + 1, i + 1, true);
                                    break;
                                }
                            }
                        }
                        NewgeneratingMessage16(generategCustomizationMas);
                        NewgeneratingMessage12(0, 0);
                        NewgeneratingMessage1Eth(zasCom1, 0, 0, 0, 0, 0, 128, true);
                        CheckAllChannelsAnswer(true);
                        for (int i = 0; i < 8; i++)
                        {
                            for (int k = 0; k < 4; k++)
                            {
                                if (RadioStationcheckboxes[i * 4 + k].Checked)
                                {
                                    NewgeneratingMessage10(-1, 0, 0, secondZas10, 0, 2, 0, 1, i + 1, i + 1, true);
                                    break;
                                }
                            }
                        }
                        NewgeneratingMessage1Eth(zasCom1, 0, 0, 0, 0, 0, 128, true);
                        CheckAllChannelsAnswer(true);
                        Thread.Sleep(3000);
                        Invoke(new NullFunction(toolStripCLearPrimaRTB.PerformClick));
                    }
                    CheckAllChannelsAnswer(true);
                    for (int acces = 0; acces < 32; acces++)
                    {
                        stopwatchMas[acces].Reset();
                    }
                    stopwatchAccept.Reset();
                    accepted = true;
                    new MessageForRTB("[" + Convert.ToString(stopwatchFullTest.Elapsed) + "] ЦИКЛ №" + j + " УСПЕШНО ЗАВЕРШЕН", Color.DarkBlue);
                    Invoke(new NullFunction(RecordToFile));
                    Invoke(new NullFunction(toolStripCLearPrimaRTB.PerformClick));
                } while (true);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }

        public void PerfomanceTest()//Еще тест "производительности" тут по факту проверяется, могут ли настроиться радиостанции
        {
            EthernetAdresses = Ethernetadressesgeneration();
            try
            {
                Invoke(labelFunction, CyrcleLabel, "Perfomance");
                perfomanceTestMode = true;
                RecordToRTB(Color.DarkOrange, "[{0}] Начало теста производительности: ", Convert.ToString(stopwatchFullTest.Elapsed));
                int sk = Convert.ToInt32(Invoke(DelegateTextReturn, сomboBoxSpeedCode10));
                int ts = Convert.ToInt32(Invoke(DelegateTextReturn, interfacetype10));
                int plume = Convert.ToInt32(Invoke(DelegateTextReturn, comboBoxPlume10));
                int zas;
                if (ZAS2radiobutton10.Checked)
                {
                    zas = 1;
                }
                else if (ZASFFradiobutton10.Checked)
                {
                    zas = 255;
                }
                else
                {
                    zas = 0;
                }
                int pprch = Convert.ToInt32(pprchOn10.Checked);
                int kv = Convert.ToInt32(kvitOn10.Checked);
                int Tiv = Convert.ToInt32(Invoke(DelegateTextReturn, TB_Tiv10));
                NewgeneratingMessage10(-1, sk, plume, zas, pprch, ts, kv, Tiv);
                NewgeneratingMessage12(0, 0);
                NewgeneratingMessage16(GenerateCustomizationForRD());
                pTC.Show();
            }
            catch (MessageException)
            {
                for (int acces = 0; acces < 8; acces++)
                {
                    incomeData[acces, 0] = 0x0;
                    stopwatchMas[acces].Reset();
                }
                stopwatchAccept.Reset();
                accepted = true;
                // StartTestButton.Enabled = true;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("stop");
                Console.ReadKey();
                StartTestButton.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(Convert.ToString(ex));
            }
            finally
            {
                Thread.Sleep(1000);
                Invoke(new NullFunction(RecordToFile));
                //  Invoke(boolFunction, false);
            }
        }
        public void FullMasterMixedTest(int[] mas)//Микс-тест, разные каналы настраиваются на разные режимы передачи (На "чайку" выделяется по 2 канала сразу)
        {
            try
            {
                int len = 0;
                for (int ch = 0; ch < mas.Length; ch++)
                {
                    switch (mas[ch])
                    {
                        case 0:
                            len += 2;
                            break;
                        case 1:
                            len += 1;
                            break;
                        default:
                            break;
                    }
                }
                //for (int i = 0; i < len;)
                //{
                //    if (mas[j] == 0)
                //    {
                //        masForTasks[0, i] = k;
                //        masForTasks[1, i] = 0;
                //        i++;
                //        masForTasks[0, i] = k + 1;
                //        masForTasks[1, i] = 0;
                //        i++;
                //    }
                //    else if (mas[j] == 1)
                //    {
                //        masForTasks[0, i] = k;
                //        masForTasks[1, i] = 1;
                //        i++;
                //    }
                //    j++;
                //    k = 2 * j;
                //}
                //newgeneratingMessage12(0, 1);
                //timeOutAnswer = Convert.ToUInt16(timeOutTB.Text);
                //Task[] tasks = new Task[len];
                //for (int i = 0; i < len; i++)
                //{
                //    int a = masForTasks[0, i];
                //    int b = masForTasks[1, i];
                //    tasks[i] = new Task(() => PerevalMixedTest(a, b));
                //}
                ///////////////////////////////////////////
                ///ЕСЛИ ВСЕ СДОХЛО ВОЗВРАЩАЙ ЭТОТ КОД
                //////////////////////////////////////////

                NewgeneratingMessage12(0, 1);
                timeOutAnswer = Convert.ToUInt16(timeOutTB.Text);
                Task[] tasks = new Task[len];
                NullFunction Stop_Click = StopTestButton.PerformClick;
                int numberch = 0;
                int n = 0; int m = 0;
                for (int i = 0; i < len; i++)
                {
                    switch (mas[numberch])
                    {
                        case 0:
                            n = new int(); m = new int();
                            n = i; m = mas[numberch];
                            tasks[i] = new Task(() => PerevalChaikaMixedTest(n, m));
                            i++;
                            n = new int();
                            n = i;
                            tasks[i] = new Task(() => PerevalChaikaMixedTest(n, m));
                            break;
                        case 1:
                            n = new int(); m = new int();
                            n = i; m = mas[numberch];
                            tasks[i] = new Task(() => PerevalChaikaMixedTest(n, m));
                            break;
                        default:
                            break;
                    }
                }
                foreach (var task in tasks)
                {
                    task.Start();
                }
                Stopwatch MixedTestStopwatch = new Stopwatch();
                MixedTestStopwatch.Start();
                while (MixedTestStopwatch.ElapsedMilliseconds < 297000)
                {
                    Thread.Sleep(100);
                }
                BeginInvoke(Stop_Click);
                Thread.Sleep(3000);
                RecordToRTB(Color.DarkOrange, "[{0}] Тест завершён", Convert.ToString(stopwatchFullTest.Elapsed));
            }
            catch
            {
            }
        }
        public void PerevalChaikaMixedTest(int channel, int rp)
        {
            int ch = channel;
            int zasCom10 = rp;
            int zasCom1 = rp;
            timeOutAnswer = 7000;
            try
            {
                RecordToRTB(Color.DarkOrange, "[{0}] Начало Теста: Канал {1}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(ch + 1));
                while (true)
                {
                    if (rp == 0)
                    {
                        /////////////////////////Перевал
                        NewgeneratingMessage10SCh(ch, 3, 0, 0, 0, 0, 0, 1);
                        NewgeneratingMessage1SCh(0, 1, 5, 0, 0, ch, 17, 16);
                        NewgeneratingMessage1SCh(0, 1, 6, 0, 0, ch, 17, 16);
                        NewgeneratingMessage1SCh(0, 1, 5, 0, 0, ch, 48, 16);
                        NewgeneratingMessage1SCh(0, 1, 6, 0, 0, ch, 48, 16);
                        NewgeneratingMessage1SCh(0, 1, 5, 0, 0, ch, 80, 16);
                        NewgeneratingMessage1SCh(0, 1, 6, 0, 0, ch, 80, 16);
                        NewgeneratingMessage1SCh(0, 1, 5, 0, 0, ch, 112, 16);
                        NewgeneratingMessage1SCh(0, 1, 6, 0, 0, ch, 112, 16);
                        CheckAllChannelsAnswer();
                        for (int i = 0; i < 4; i++)
                        {
                            NewgeneratingMessage10SCh(ch, i, 0, 0, 0, 0, 0, 1);
                            NewgeneratingMessage1SCh(0, 1, 5, 0, 0, ch, 17, 1);
                            NewgeneratingMessage1SCh(0, 1, 6, 0, 0, ch, 17, 1);
                            CheckAllChannelsAnswer();
                        }
                        /////////////////////////с1-фл-би
                        NewgeneratingMessage10SCh(ch, 3, 0, 0, 0, 1, 0, 1);
                        NewgeneratingMessage1SCh(0, 1, 5, 0, 0, ch, 17, 16);
                        NewgeneratingMessage1SCh(0, 1, 6, 0, 0, ch, 17, 16);
                        NewgeneratingMessage1SCh(0, 1, 5, 0, 0, ch, 48, 16);
                        NewgeneratingMessage1SCh(0, 1, 6, 0, 0, ch, 48, 16);
                        NewgeneratingMessage1SCh(0, 1, 5, 0, 0, ch, 80, 16);
                        NewgeneratingMessage1SCh(0, 1, 6, 0, 0, ch, 80, 16);
                        NewgeneratingMessage1SCh(0, 1, 5, 0, 0, ch, 112, 16);
                        NewgeneratingMessage1SCh(0, 1, 6, 0, 0, ch, 112, 16);
                        //for (int i = 3; i < 8; i++)
                        //{
                        //    newgeneratingMessage10SCh(channel, i, 0, zasCom10, 0, 1, 0);
                        //    newgeneratingMessage1SCh(zasCom1, 0, 5, 0, 0, channel, 17, 1);
                        //    newgeneratingMessage1SCh(zasCom1, 0, 6, 0, 0, channel, 17, 1);
                        //}
                        #region 2 комманда
                        //NewgeneratingMessage10(-1, 7, 0, 1, 0, 0, 0, numberOfChannels);

                        //newgeneratingMessage2(0, 0, 5, 0, 0, -1, 16, 81, 10240, numberOfChannels);

                        //NewgeneratingMessage10(-1, 7, 0, 1, 0, 1, 0, numberOfChannels);

                        //newgeneratingMessage2(0, 0, 5, 0, 0, -1, 16, 81, 10240, numberOfChannels);

                        //Thread.Sleep(30);
                        #endregion
                        Thread.Sleep(500);
                    }
                    else if (rp == 1)
                    {
                        /////////////////////////Чайка
                        //newgeneratingMessage10SCh(channel, 0, 0, 1, 0, 0, 0);
                        //newgeneratingMessage1SCh(1, 0, 1, 0, 0, channel, 15, 1);
                        ////newgeneratingMessage1SCh(1, 0, 1, 0, 0, channel, 62, 1);

                        //newgeneratingMessage10SCh(channel, 1, 0, 1, 0, 0, 0);
                        //newgeneratingMessage1SCh(1, 0, 1, 0, 0, channel, 15, 1);
                        ////newgeneratingMessage1SCh(1, 0, 1, 0, 0, channel, 62, 1);

                        //newgeneratingMessage10SCh(channel, 2, 0, 1, 0, 0, 0);
                        //newgeneratingMessage1SCh(1, 0, 1, 0, 0, channel, 15, 1);
                        ////newgeneratingMessage1SCh(1, 0, 1, 0, 0, channel, 62, 1);

                        //newgeneratingMessage10SCh(channel, 3, 0, 1, 0, 0, 0);
                        //newgeneratingMessage1SCh(1, 0, 1, 0, 0, channel, 15, 1);
                        ////newgeneratingMessage1SCh(1, 0, 1, 0, 0, channel, 62, 1);
                        CheckAllChannelsAnswer();
                        NewgeneratingMessage10SCh(ch, 3, 0, 1, 0, 1, 0, 1);
                        NewgeneratingMessage1SCh(1, 1, 1, 0, 0, ch, 15, 1);
                        //newgeneratingMessage1SCh(1, 0, 1, 0, 0, channel, 62, 1);
                        #region 2 комманда
                        //NewgeneratingMessage10(-1, 7, 0, 1, 0, 0, 0, numberOfChannels);
                        //newgeneratingMessage2(0, 0, 5, 0, 0, -1, 16, 81, 10240, numberOfChannels);
                        //NewgeneratingMessage10(-1, 7, 0, 1, 0, 1, 0, numberOfChannels);
                        //newgeneratingMessage2(0, 0, 5, 0, 0, -1, 16, 81, 10240, numberOfChannels);
                        //Thread.Sleep(30);
                        #endregion
                        Thread.Sleep(100);
                    }
                    //incomeData[channel, 0] = 0x0;
                    //stopwatchMas[channel].Reset();
                    //stopwatchAccept.Reset();   
                }
            }
            catch (OperationCanceledException)
            {
                //MessageBox.Show(Convert.ToString(ex));
                Console.WriteLine("stop");
                Console.ReadKey();
            }
            catch (MessageException)
            {
                for (int acces = 0; acces < 8; acces++)
                {
                    incomeData[acces, 0] = 0x0;
                    stopwatchMas[acces].Reset();
                }
                stopwatchAccept.Reset();
                accepted = true;
                // StartTestButton.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(Convert.ToString(ex));
                StartTestButton.Enabled = true;
            }
            finally
            {
                Thread.Sleep(1000);
                Invoke(new NullFunction(RecordToFile));
                //Invoke(boolFunction, false);
            }
        }
        #endregion
        private byte[,] GenerateCustomizationForRD()//генерация списков данных для настройки РС иммитатора
        {
            byte[,] resultetMas = new byte[32, 10];
            for (int i = 0; i < 32; i++)
            {
                resultetMas[i, 0] = Convert.ToByte(EthernetCustomisationRBs[i, 1].Checked);
                if (EthernetCustomisationRBs[i, 4].Checked)
                {
                    resultetMas[i, 1] = Convert.ToByte(0xff);
                }
                else
                {
                    resultetMas[i, 1] = Convert.ToByte(EthernetCustomisationRBs[i, 3].Checked);
                }
                resultetMas[i, 2] = Convert.ToByte(Invoke(DelegateTextReturn, EthernetCustomisationTBs[i, 0]));
                resultetMas[i, 3] = Convert.ToByte(Invoke(DelegateTextReturn, EthernetCustomisationTBs[i, 1]));
                resultetMas[i, 4] = Convert.ToByte(Convert.ToInt32(Invoke(DelegateTextReturn, EthernetCustomisationTBs[i, 2])) % 256);
                resultetMas[i, 5] = Convert.ToByte(Convert.ToInt32(Invoke(DelegateTextReturn, EthernetCustomisationTBs[i, 2])) / 256);
                resultetMas[i, 6] = Convert.ToByte(EthernetCustomisationRBs[i, 6].Checked);
                resultetMas[i, 7] = Convert.ToByte(EthernetCustomisationRBs[i, 8].Checked);
                resultetMas[i, 8] = Convert.ToByte(Convert.ToInt32(Invoke(DelegateTextReturn, EthernetCustomisationTBs[i, 3])));
                resultetMas[i, 9] = Convert.ToByte(Convert.ToInt32(Invoke(DelegateTextReturn, EthernetCustomisationComboBoxes[i])));
            }
            return resultetMas;
        }
        private void TuningRadiostations(byte[,] customizationMas)//Функция для настройки Рс иммитатора, засылается до передачи данных в приму 
        {
            RadioStationTuner[] RadioStationTunerMas = new RadioStationTuner[32];
            bool[] isRadiostationAnswered = new bool[32] { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true,
                true, true, true, true, true, true, true, true, true, true, true, true, true, true };
            try
            {
                for (int i = 0; i < 32; i++)
                {
                    if (RadioStationcheckboxes[i].Checked)
                    {
                        //Task.Factory.StartNew
                        RadioStationTunerMas[i] = new RadioStationTuner(Ethernetadresses[i * 2, 0].Text, Convert.ToInt32(Ethernetadresses[i * 2, 1].Text), Convert.ToInt32(Ethernetadresses[i * 2, 1].Text) - 5000,
                            customizationMas[i, 0], customizationMas[i, 1], customizationMas[i, 2], customizationMas[i, 3], customizationMas[i, 4], customizationMas[i, 5], customizationMas[i, 6], customizationMas[i, 9], Convert.ToByte(mode));
                        Thread.Sleep(20);
                        isRadiostationAnswered[i] = false;
                    }
                }
                Stopwatch timeForTuning = new Stopwatch();
                timeForTuning.Start();
                while (timeForTuning.ElapsedMilliseconds < 5000)
                {
                    for (int i = 0; i < 32; i++)
                    {
                        if (!isRadiostationAnswered[i])
                        {
                            if (RadioStationTunerMas[i].stateOfRasdioStation == true)
                            {
                                isRadiostationAnswered[i] = true;
                                RecordToRTB(Color.Green, "\n Радиостанция номер {0} настроена ", Convert.ToString(i + 1));
                                Thread.Sleep(1);
                            }
                        }
                    }
                    int notAnsweredCounter = 0;
                    for (int i = 0; i < 32; i++)
                    {
                        if (!isRadiostationAnswered[i])
                        {
                            notAnsweredCounter++;
                            break;
                        }
                    }
                    if (notAnsweredCounter == 0)
                    {
                        return;
                    }
                }
                for (int i = 0; i < 32; i++)
                {
                    if (!isRadiostationAnswered[i])
                    {
                        if (RadioStationTunerMas[i].stateOfRasdioStation == true)
                        {
                            RecordToRTB(Color.Red, "\n Радиостанция номер {0} не настроена ", Convert.ToString(i + 1));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public void Run(Task task)
        {
            task.Start();
        }
        private void OverwriteIncomeData(byte[] data, int numberMas, int lengthofMas)//перезапись последнего сообщения для данного канала\радиостанции
        {//Необходимо для повторной отправки сообщения, и сравнения полученных данных на предмет искажения
            try
            {
                int j;
                for (j = 0; j < data.Length; j++)
                {
                    incomeData[numberMas, j] = data[j];
                }
                for (; j < lengthofMas; j++)
                {
                    incomeData[numberMas, j] = 0xff;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #region Генерация комманд/сообщений
        public void NewgeneratingMessage0()//Квитанция-подтверждение о получения сообщения
        {
            byte[] cmd = new byte[10] { 0x23, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            TCPClientSendData(cmd);
            RecordToRTB(Color.Green, "\n[{0}] Отправляю подтверждение {1}", Convert.ToString(stopwatchFullTest.Elapsed), BitConverter.ToString(cmd));

        }

        public void NewgeneratingMessage1(int nz, int prg, int rp, int np, int dp, int channel, int lengthOfMessage, int numberOfFirstChannel = 1, int numberOfLastChannel = 8, int cycles = 16, bool randMode = false)
        {//Основная функция по генерации Данных, здесь есть как генерация в ручном режиме, так и в автоматическом, с поддержкой randmode.
            tokenTestStop.ThrowIfCancellationRequested();
            string[] rpmas = new string[8] {"Режим не задан","Чайка однократный","Чайка двукратный","Чайка 'Аккорд'",
            "Чайка 'Мелодия'", "Перевал короткий код","Перевал длинный код","Безызботочные сообщения"};
            Invoke(labelFunction, CommandStateLabel, "Ком:" + 1 + ", НЗ:" + nz + ", ПРГ:" + prg + ", РП:" + rpmas[rp] + ", НП:" + np + ", ДП:" + dp + ", Длина информации:" + lengthOfMessage);
            //Thread.Sleep(1);
            byte[] cmd = new byte[1] { 0 };
            if (!mode)
            {
                if (numPA.Checked)
                {
                    cmd = new byte[16] { 0x23, 0x0, 0x0, 0x0, 0x0, 0x01, 0, 0, 0, 0, Convert.ToByte(nz), Convert.ToByte(prg), Convert.ToByte(rp), Convert.ToByte(np), Convert.ToByte(dp), Convert.ToByte(channel) };//Convert.ToByte(Math.Pow(2, channel)) };
                }
                else
                {
                    cmd = new byte[16] { 0x23, 0x0, 0x0, 0x0, 0x0, 0x01, 0, 0, 0, 0, Convert.ToByte(nz), Convert.ToByte(prg), Convert.ToByte(rp), Convert.ToByte(np), Convert.ToByte(dp), 0x0 };//Convert.ToByte(Math.Pow(2, channel)) };
                    cmd = functions.Combine(cmd, Com1adressesgeneration());
                    CheckBox[] checkboxes = new CheckBox[32] { Com1CB_RS1_1, Com1CB_RS1_2, Com1CB_RS1_3, Com1CB_RS1_4, Com1CB_RS2_1, Com1CB_RS2_2, Com1CB_RS2_3, Com1CB_RS2_4,
                                                       Com1CB_RS3_1, Com1CB_RS3_2, Com1CB_RS3_3, Com1CB_RS3_4, Com1CB_RS4_1, Com1CB_RS4_2, Com1CB_RS4_3, Com1CB_RS4_4,
                                                       Com1CB_RS5_1, Com1CB_RS5_2, Com1CB_RS5_3, Com1CB_RS5_4, Com1CB_RS6_1, Com1CB_RS6_2, Com1CB_RS6_3, Com1CB_RS6_4,
                                                       Com1CB_RS7_1, Com1CB_RS7_2, Com1CB_RS7_3, Com1CB_RS7_4, Com1CB_RS8_1, Com1CB_RS8_2, Com1CB_RS8_3, Com1CB_RS8_4,};
                    channel = 0;
                    for (int i = 0; i < checkboxes.Length; i++)
                    {
                        if (checkboxes[i].Checked)
                        {
                            channel = channel + i + 1;

                        }
                    }
                }
                byte[] cmdNewPart = functions.INS();
                for (int j = 0; j < 4; j++)
                    cmd[j + 6] = cmdNewPart[j];
                if (!DataHandButton1.Checked)//выбор режима передачи
                {
                    if (rp == 7)
                    {
                        cmdNewPart = functions.DataMessage(Convert.ToInt32(LengthOfbreakevenMes1TB.Text), 2, onlyNulls, numberRadiostations, channel);//безызбыточное сообщение
                    }
                    else if (rp < 7 && rp > 4)
                    {
                        cmdNewPart = functions.DataMessage(Convert.ToInt32(perevalLengthOfmessage.Text), 0, onlyNulls, numberRadiostations, channel);//формат перевала
                    }
                    else if (rp <= 4 && rp > 0)
                    {
                        cmdNewPart = functions.DataMessage(Convert.ToInt32(LengthOfChaikaMes1.Text), 1, onlyNulls, numberRadiostations, channel);//формат чайки
                    }
                }
                else if (DataHandButton1.Checked)
                {
                    bool notInt = false;
                    int i;
                    switch (rp)
                    {

                        case 1:
                        case 2:
                        case 3:
                        case 4:
                            cmdNewPart = new byte[Convert.ToInt32(LengthOfChaikaMes1.Text)];
                            break;
                        case 5:
                        case 6:
                            cmdNewPart = new byte[Convert.ToInt32(perevalLengthOfmessage.Text)];
                            break;
                        case 7:
                            cmdNewPart = new byte[Convert.ToInt32(LengthOfbreakevenMes1TB.Text)];
                            break;
                    }
                    string data = Convert.ToString(Message1.Text);
                    data = data.Replace("–", "");
                    data = data.Replace("-", "");
                    data = data.Replace(" ", "");
                    data = data.Replace("\r", "");
                    data = data.Replace("\n", "");
                    if (data.Length >= 2 * cmdNewPart.Length)
                    {
                        data = data.Substring(0, 2 * cmdNewPart.Length);
                    }
                    else
                    {
                        if (data.Length % 2 != 0)
                        {
                            notInt = true;
                        }
                    }
                    int n = 0;
                    for (i = 0; i < (data.Length / 2); i++)
                    {
                        n = Int32.Parse(data.Substring(2 * i, 2), System.Globalization.NumberStyles.HexNumber);
                        if (rp <= 4 && rp > 0)
                        {
                            if (n > 31)
                            {
                                MessageBox.Show("Запрещенная комбинация в " + (i + 1) + " позиции");
                                return;
                            }
                        }
                        cmdNewPart[i] = Convert.ToByte(n);
                    }
                    i--;
                    if (notInt)
                    {
                        cmdNewPart[i + 1] = Convert.ToByte(16 * Int32.Parse(data.Substring(2 * i + 2, 1), System.Globalization.NumberStyles.HexNumber));

                    }
                }
                cmd = functions.Combine(cmd, cmdNewPart);
                cmdNewPart = functions.DD(cmd.Length - 5);
                for (int j = 0; j < 4; j++)
                    cmd[j + 1] = cmdNewPart[j];
                TCPClientSendData(cmd);
            }
            else
            {
                int prgNumber = 0;
                bool randnz = false;
                Random nzrand = new Random();
                if (nz == 254)
                {
                    randnz = true;
                }
                for (int numberOfMes = 0; numberOfMes < cycles; numberOfMes++)
                {

                    for (int i = numberOfFirstChannel - 1; i < numberOfLastChannel; i++)
                    {
                        byte[] cmdNewPart;
                        tokenTestStop.ThrowIfCancellationRequested();
                        if (access[i])
                        {
                            if (randnz == true)
                            {
                                nz = nzrand.Next(0, 2);
                            }
                            if (!randMode)
                            {
                                cmd = new byte[16] { 0x23, 0x0, 0x0, 0x0, 0x0, 0x01, 0, 0, 0, 0, Convert.ToByte(nz), Convert.ToByte(prgNumber), Convert.ToByte(rp), Convert.ToByte(np),
                               Convert.ToByte(dp), Convert.ToByte( Math.Pow(2,i)) };
                                cmdNewPart = new byte[lengthOfMessage];
                                if (!onlyNulls)//Если не выбран режим нулей
                                {
                                    if (lengthOfMessage == 15 || lengthOfMessage == 62)//Длины Чайки
                                    {
                                        if (!numberRadiostations)
                                        {
                                            for (int j = 0; j < lengthOfMessage; j++)
                                            {
                                                cmdNewPart[j] = (byte)Convert.ToByte(rand.Next(0x00, 0x20));
                                            }
                                        }
                                        else
                                        {
                                            for (int j = 0; j < lengthOfMessage; j++)
                                            {
                                                cmdNewPart[j] = (byte)Convert.ToByte(i + 1);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!numberRadiostations)
                                        {

                                            (new Random()).NextBytes(cmdNewPart);

                                        }
                                        else
                                        {
                                            for (int j = 0; j < lengthOfMessage; j++)
                                            {
                                                cmdNewPart[j] = (byte)Convert.ToByte(i + 1);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                rp = rand.Next(5, 7);//8 безизбыточные вернуть
                                switch (rp)
                                {
                                    case 0:
                                    case 1:
                                    case 2:
                                    case 3:
                                    case 4:
                                        break;
                                    case 5:
                                    case 6:
                                        {
                                            switch (rand.Next(0, 4))
                                            {
                                                case 0:
                                                    lengthOfMessage = 17;
                                                    break;
                                                case 1:
                                                    lengthOfMessage = 48;
                                                    break;
                                                case 2:
                                                    lengthOfMessage = 80;
                                                    break;
                                                case 3:
                                                    lengthOfMessage = 112;
                                                    break;
                                            }
                                        }
                                        break;
                                    case 7:
                                        {
                                            lengthOfMessage = rand.Next(1, 1432);
                                        }
                                        break;
                                }
                                cmd = new byte[16] { 0x23, 0x0, 0x0, 0x0, 0x0, 0x01, 0, 0, 0, 0, Convert.ToByte(nz), Convert.ToByte(prgNumber), Convert.ToByte(rp), Convert.ToByte(np),
                               Convert.ToByte(dp), Convert.ToByte( Math.Pow(2,i)) };
                                cmdNewPart = new byte[lengthOfMessage];
                                if (!onlyNulls)
                                {
                                    if (!numberRadiostations)
                                    {
                                        for (int j = 0; j < lengthOfMessage; j++)
                                        {
                                            cmdNewPart[j] = Convert.ToByte(rand.Next(0x00, 0x100));
                                        }
                                    }
                                    else
                                    {
                                        for (int j = 0; j < lengthOfMessage; j++)
                                        {
                                            cmdNewPart[j] = Convert.ToByte(i + 1);
                                        }
                                    }
                                }
                            }
                            cmd = functions.Combine(cmd, cmdNewPart);
                            cmdNewPart = functions.INS();
                            for (int j = 0; j < 4; j++)
                            {
                                cmd[j + 6] = cmdNewPart[j];
                            }
                            cmdNewPart = functions.DD(cmd.Length - 5);
                            for (int j = 0; j < 4; j++)
                            {
                                cmd[j + 1] = cmdNewPart[j];
                            }
                            CheckAnswerAndKvitation(i);
                            if (access[i])
                            {
                                OverwriteIncomeData(cmd, i, cmd.Length);
                                ActivatingLabel(4 * i);
                                _numberOFStopwatch = i;
                                RecordToRTB(Color.Red, "\n[{0}] Отправляю сообщение {1} по Кан {2}", Convert.ToString(stopwatchFullTest.Elapsed), BitConverter.ToString(cmd), Convert.ToString(i + 1));
                                answerRecieved[i] = false;
                                kvitRecieved[i] = false;
                                accepted = false;
                                //private int CalculationTimeOut(int prg, int speed, int length)
                                TimeOutMas[i] = CalculationTimeOut(rp, SkMas[i], lengthOfMessage);
                                TCPClientSendData(cmd);
                            }
                        }
                    }
                    //timeOutAnswer = 4000;
                    prgNumber = ((prgNumber + 1) % 8);
                }
            }
        }
        private int CalculationTimeOut(int prg, int speed, int length)
        {
            double x, y, z;
            switch (prg)
            {
                case 5: { x = 18.0; break; }
                case 6: { x = 32.0; break; }
                case 7: { x = length; break; }
                default:
                    return 7000;
            }
            switch (speed)
            {
                case 0: { y = 75.0; break; }
                case 1: { y = 150.0; break; }
                case 2: { y = 300.0; break; }
                case 3: { y = 1200.0; break; }
                case 4: { y = 2400.0; break; }
                case 5: { y = 4800.0; break; }
                case 6: { y = 9600.0; break; }
                case 7: { y = 16000.0; break; }
                case 8: { y = 50.0; break; }
                case 9: { y = 100.0; break; }
                default:
                    return 7000;
            }
            switch (length)
            {
                case 17: z = 1.0; break;
                case 48: z = 2.0; break;
                case 80: z = 3.0; break;
                case 112: z = 4.0; break;
                default: z = 8.0; break;
            }
            switch (prg)
            {
                case 5:
                case 6:
                    {
                        int n = Convert.ToInt32(1000 * ((31 * x * z / y) * 1.30 + 2.0));
                        return Convert.ToInt32(1000 * ((31 * x * z / y) * 1.30 + 2));
                    } //время таймаута для короткого\длинного кода
                case 7:
                    {

                        return Convert.ToInt32(1000 * (((x * z + 118) / y) * 1.30 + 2));
                    } //время таймаута для безизбыточного сообщения 
                default:
                    { return 7000; }

            }


        }
        private void ActivatingLabel(int numberlabel)
        {
            if (!labelConnectedBlock[numberlabel])
            {
                BeginInvoke(labelChanging, numberlabel, 0);
                labelConnectedBlock[numberlabel] = true;
            }
        }
        public void NewgeneratingMessage1Eth(int nz, int prg, int rp, int np, int dp, int lengthOfMessage, int cycles = 16, bool randMode = false)//Стандартные данные для Etherneta
        {
            tokenTestStop.ThrowIfCancellationRequested();
            string[] rpmas = new string[8] {"Режим не задан","Чайка однократный","Чайка двукратный","Чайка 'Аккорд'",
            "Чайка 'Мелодия'", "Перевал короткий код","Перевал длинный код","Безызботочные сообщения"};
            Invoke(labelFunction, CommandStateLabel, "Ком:" + 1 + ", НЗ:" + nz + ", ПРГ:" + prg + ", РП:" + rpmas[rp] + ", НП:" + np + ", ДП:" + dp + ", Длина информации:" + lengthOfMessage);
            EthernetAdresses = Ethernetadressesgeneration();
            byte[] cmd = new byte[1] { 0 };
            byte[] cmdNewPart = new byte[1] { 0 };
            bool randnz = false;
            Random rand = new Random();
            int prgNumber = 0;
            if (nz == 254)
            {
                randnz = true;
            }
            for (int numberOfMes = 0; numberOfMes < cycles; numberOfMes++)
            {
                int numberOfEthernetAdress = 0;
                for (int i = 0; i < 32; i++)
                {
                    tokenTestStop.ThrowIfCancellationRequested();
                    if (RadioStationcheckboxes[i].Checked)
                    {
                        if (access[i])
                        {
                            if (randnz == true)
                            {
                                nz = rand.Next(0, 2);
                            }
                            if (!randMode)
                            {
                                cmd = new byte[29] { 0x23, 0x0, 0x0, 0x0, 0x0, 0x01, 0, 0, 0, 0, Convert.ToByte(nz), Convert.ToByte(prgNumber), Convert.ToByte(rp), Convert.ToByte(np),
                               Convert.ToByte(dp), 0, 0x1, EthernetAdresses[numberOfEthernetAdress*24],EthernetAdresses[numberOfEthernetAdress*24+1],EthernetAdresses[numberOfEthernetAdress*24+2],EthernetAdresses[numberOfEthernetAdress*24+3],
                               EthernetAdresses[numberOfEthernetAdress*24+4],EthernetAdresses[numberOfEthernetAdress*24+5],
                               EthernetAdresses[numberOfEthernetAdress*24+6],EthernetAdresses[numberOfEthernetAdress*24+7],EthernetAdresses[numberOfEthernetAdress*24+8],EthernetAdresses[numberOfEthernetAdress*24+9],
                               EthernetAdresses[numberOfEthernetAdress*24+10],EthernetAdresses[numberOfEthernetAdress*24+11]};
                                numberOfEthernetAdress++;
                                cmdNewPart = functions.INS();
                                for (int j = 0; j < 4; j++)
                                {
                                    cmd[j + 6] = cmdNewPart[j];
                                }
                                cmdNewPart = new byte[lengthOfMessage];
                                if (!onlyNulls)
                                {
                                    if (numberRadiostations)
                                    {
                                        for (int j = 0; j < lengthOfMessage; j++)
                                        {
                                            cmdNewPart[j] = (byte)Convert.ToByte(i + 1);
                                        }
                                    }
                                    else
                                    {
                                        for (int j = 0; j < lengthOfMessage; j++)
                                        {

                                            cmdNewPart[j] = (byte)Convert.ToByte(rand.Next(0x00, 0x100));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                rp = rand.Next(5, 7);//8 безизбыточные вернуть
                                switch (rp)
                                {
                                    case 0:
                                    case 1:
                                    case 2:
                                    case 3:
                                    case 4:
                                        break;
                                    case 5:
                                    case 6:
                                        {
                                            switch (rand.Next(0, 4))
                                            {
                                                case 0:
                                                    lengthOfMessage = 17;
                                                    break;
                                                case 1:
                                                    lengthOfMessage = 48;
                                                    break;
                                                case 2:
                                                    lengthOfMessage = 80;
                                                    break;
                                                case 3:
                                                    lengthOfMessage = 112;
                                                    break;
                                            }
                                        }
                                        break;
                                    case 7:
                                        {
                                            lengthOfMessage = rand.Next(1, 1432);
                                        }
                                        break;
                                }
                                cmd = new byte[29] { 0x23, 0x0, 0x0, 0x0, 0x0, 0x01, 0, 0, 0, 0, Convert.ToByte(nz), Convert.ToByte(prgNumber), Convert.ToByte(rp), Convert.ToByte(np),
                               Convert.ToByte(dp), 0, 0x1,
                                    EthernetAdresses[numberOfEthernetAdress*24],EthernetAdresses[numberOfEthernetAdress*24+1],EthernetAdresses[numberOfEthernetAdress*24+2],
                                    EthernetAdresses[numberOfEthernetAdress*24+3],EthernetAdresses[numberOfEthernetAdress*24+4],EthernetAdresses[numberOfEthernetAdress*24+5],
                                    EthernetAdresses[numberOfEthernetAdress*24+6],EthernetAdresses[numberOfEthernetAdress*24+7],EthernetAdresses[numberOfEthernetAdress*24+8],
                                    EthernetAdresses[numberOfEthernetAdress*24+9],EthernetAdresses[numberOfEthernetAdress*24+10],EthernetAdresses[numberOfEthernetAdress*24+11]};
                                numberOfEthernetAdress++;
                                cmdNewPart = functions.INS();
                                for (int j = 0; j < 4; j++)
                                {
                                    cmd[j + 6] = cmdNewPart[j];
                                }
                                cmdNewPart = new byte[lengthOfMessage];
                                if (!onlyNulls)
                                {
                                    if (numberRadiostations)
                                    {
                                        for (int j = 0; j < lengthOfMessage; j++)
                                        {
                                            cmdNewPart[j] = (byte)Convert.ToByte(i + 1);
                                        }
                                    }
                                    else
                                    {
                                        for (int j = 0; j < lengthOfMessage; j++)
                                        {
                                            cmdNewPart[j] = Convert.ToByte(rand.Next(0x00, 0x100));
                                        }
                                    }
                                }
                            }
                            cmd = functions.Combine(cmd, cmdNewPart);
                            cmdNewPart = functions.DD(cmd.Length - 5);
                            for (int j = 0; j < 4; j++)
                            {
                                cmd[j + 1] = cmdNewPart[j];
                            }
                            CheckAnswerAndKvitation(i, true, true);
                            if (access[i])
                            {
                                OverwriteIncomeData(cmd, i, cmd.Length);

                                _numberOFStopwatch = i;
                                RecordToRTB(Color.Red, "\n[{0}] Отправляю сообщение по РС {1}:  {2}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(i + 1), BitConverter.ToString(cmd));
                                answerRecieved[i] = false;
                                kvitRecieved[i] = false;
                                TimeOutMas[i] = CalculationTimeOut(rp, SkMas[i], lengthOfMessage);
                                accepted = false;
                                TCPClientSendData(cmd);
                            }
                        }
                    }
                    timeOutAnswer = 7000;
                }
                prgNumber = ((prgNumber + 1) % 8);
            }
        }
        public void NewgeneratingMessage1EthRSTest(int nz, int prg, int rp, int np, int dp, int lengthOfMessage, int cycles = 16)//Данные для RSTest
        {
            tokenTestStop.ThrowIfCancellationRequested();
            string[] rpmas = new string[8] {"Режим не задан","Чайка однократный","Чайка двукратный","Чайка 'Аккорд'",
            "Чайка 'Мелодия'", "Перевал короткий код","Перевал длинный код","Безызботочные сообщения"};
            Invoke(labelFunction, CommandStateLabel, "Ком:" + 1 + ", НЗ:" + nz + ", ПРГ:" + prg + ", РП:" + rpmas[rp] + ", НП:" + np + ", ДП:" + dp + ", Длина информации:" + lengthOfMessage);
            //Thread.Sleep(1);
            byte[] cmd = new byte[1] { 0 };
            int l = 0;
            if (mode)
            {
                bool randnz = false;
                Random nzrand = new Random();
                if (nz == 254)
                {
                    randnz = true;
                }
                for (int numberOfMes = 0; numberOfMes < cycles; numberOfMes++)
                {
                    int numberOfEthernetAdress = 0;
                    for (int i = 0; i < 32; i++)
                    {
                        tokenTestStop.ThrowIfCancellationRequested();
                        if (RadioStationcheckboxes[i].Checked)
                        {
                            if (access[i])
                            {
                                if (randnz == true)
                                {
                                    nz = nzrand.Next(0, 2);
                                }
                                cmd = new byte[29] { 0x23, 0x0, 0x0, 0x0, 0x0, 0x01, 0, 0, 0, 0, Convert.ToByte(nz), Convert.ToByte(prg), Convert.ToByte(rp), Convert.ToByte(np),
                               Convert.ToByte(dp), 0, 0x1, EthernetAdresses[numberOfEthernetAdress*24],EthernetAdresses[numberOfEthernetAdress*24+1],EthernetAdresses[numberOfEthernetAdress*24+2],EthernetAdresses[numberOfEthernetAdress*24+3],
                               EthernetAdresses[numberOfEthernetAdress*24+4],EthernetAdresses[numberOfEthernetAdress*24+5],
                               EthernetAdresses[numberOfEthernetAdress*24+6],EthernetAdresses[numberOfEthernetAdress*24+7],EthernetAdresses[numberOfEthernetAdress*24+8],EthernetAdresses[numberOfEthernetAdress*24+9],
                               EthernetAdresses[numberOfEthernetAdress*24+10],EthernetAdresses[numberOfEthernetAdress*24+11]};
                                numberOfEthernetAdress++;
                                byte[] cmdNewPart = functions.INS();
                                for (int j = 0; j < 4; j++)
                                {
                                    cmd[j + 6] = cmdNewPart[j];
                                }
                                cmdNewPart = new byte[lengthOfMessage];
                                if (!onlyNulls)
                                {
                                    if (numberRadiostations)
                                    {
                                        for (int j = 0; j < lengthOfMessage; j++)
                                        {
                                            cmdNewPart[j] = (byte)Convert.ToByte(i);
                                        }
                                    }
                                    else
                                    {
                                        for (int j = 0; j < lengthOfMessage; j++)
                                        {

                                            cmdNewPart[j] = (byte)Convert.ToByte(rand.Next(0x00, 0x100));
                                        }
                                    }
                                }
                                cmd = functions.Combine(cmd, cmdNewPart);
                                cmdNewPart = functions.DD(cmd.Length - 5);
                                for (int j = 0; j < 4; j++)
                                {
                                    cmd[j + 1] = cmdNewPart[j];
                                }
                                if (i > 0) { l = i - 1; }
                                else { l = 31; }
                                while (accepted == false || incomeData[i, 0] != 0x0 || incomeData[l, 0] != 0x0)
                                {
                                    tokenTestStop.ThrowIfCancellationRequested();
                                    Thread.Sleep(1);
                                    if (stopwatchAccept.ElapsedMilliseconds > timeOut)
                                    {
                                        Command0NoAnswer(_numberOFStopwatch, true);
                                    }
                                    if (stopwatchMas[i].ElapsedMilliseconds > TimeOutMas[i])
                                    {
                                        CommandNokvitation(i, true, true);
                                    }
                                }
                                if (access[i])
                                {
                                    OverwriteIncomeData(cmd, i, cmd.Length);
                                    // Thread.Sleep(1);

                                    _numberOFStopwatch = i;
                                    RecordToRTB(Color.Red, "\n[{0}] Отправляю сообщение  по РС {1}:  {2}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(i + 1), BitConverter.ToString(cmd));
                                    answerRecieved[i] = false;
                                    kvitRecieved[i] = false;
                                    TimeOutMas[i] = CalculationTimeOut(rp, SkMas[i], lengthOfMessage);
                                    accepted = false;
                                    TCPClientSendData(cmd);
                                }
                            }
                        }
                    }
                    timeOutAnswer = 7000;
                }
            }
        }
        public void NewgeneratingMessage1SCh(int nz, int prg, int rp, int np, int dp, int channel, int lengthOfMessage, int cycles = 16)//Генерация Данных для каждого канала в Mixed test
        {
            tokenTestStop.ThrowIfCancellationRequested();
            string[] rpmas = new string[8] {"Режим не задан","Чайка однократный","Чайка двукратный","Чайка 'Аккорд'",
            "Чайка 'Мелодия'", "Перевал короткий код","Перевал длинный код","Безызботочные сообщения"};
            Invoke(labelFunction, CommandStateLabel, "Ком:" + 1 + ", ПРГ:" + prg + ", РП:" + rpmas[rp] + ", НП:" + np + ", ДП:" + dp + ", Длина информации:" + lengthOfMessage);
            //Thread.Sleep(1);
            byte[] cmd = new byte[1] { 0 };
            NullFunction Stop_Click = StopTestButton.PerformClick;
            for (int numberOfMes = 0; numberOfMes < cycles; numberOfMes++)
            {
                tokenTestStop.ThrowIfCancellationRequested();
                if (access[channel])
                {
                    cmd = new byte[16] { 0x23, 0x0, 0x0, 0x0, 0x0, 0x01, 0, 0, 0, 0, Convert.ToByte(nz), Convert.ToByte(prg), Convert.ToByte(rp), Convert.ToByte(np), Convert.ToByte(dp), Convert.ToByte(Math.Pow(2, channel)) };
                    byte[] cmdNewPart = new byte[lengthOfMessage];
                    if (!onlyNulls)
                    {
                        if (lengthOfMessage == 15 || lengthOfMessage == 62)
                        {
                            for (int j = 0; j < lengthOfMessage; j++)
                            {
                                cmdNewPart[j] = (byte)Convert.ToByte(rand.Next(0x00, 0x20));
                            }
                        }
                        else
                        {
                            for (int j = 0; j < lengthOfMessage; j++)
                            {
                                cmdNewPart[j] = (byte)Convert.ToByte(rand.Next(0x00, 0x100));
                            }
                        }
                    }
                    cmd = functions.Combine(cmd, cmdNewPart);
                    cmdNewPart = functions.DD(cmd.Length - 5);
                    for (int j = 0; j < 4; j++)
                    {
                        cmd[j + 1] = cmdNewPart[j];
                    }
                    while (accepted == false || incomeData[channel, 0] != 0x0)
                    {
                        tokenTestStop.ThrowIfCancellationRequested();
                        Thread.Sleep(1);
                        if (stopwatchAccept.ElapsedMilliseconds > timeOut)
                        {
                            Command0NoAnswer(_numberOFStopwatch);
                        }
                        if (stopwatchMas[channel].ElapsedMilliseconds > timeOutAnswer)
                        {
                            incomeData[channel, 0] = 0x0;
                            access[channel] = false;
                            stopwatchMas[channel].Stop();
                            RecordToRTB(Color.Red, "\n[{0}] ERROR! Превышено время ожидания команды {1}, время [{2}] по Кан {3} ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(incomeData[channel, 5]), Convert.ToString(Convert.ToInt32(stopwatchMas[channel].ElapsedMilliseconds)), (Convert.ToString(channel + 1)));
                            stopwatchAccept.Reset();
                            BeginInvoke(labelChanging, channel * 4, 2);
                            BeginInvoke(Stop_Click);
                            return;
                        }
                    }
                    accepted = false;
                    if (access[channel])
                    {
                        cmdNewPart = functions.INS();
                        for (int j = 0; j < 4; j++)
                        {
                            cmd[j + 6] = cmdNewPart[j];
                        }
                        OverwriteIncomeData(cmd, channel, cmd.Length);
                        ActivatingLabel(4 * channel);

                        //Thread.Sleep(1);
                        _numberOFStopwatch = channel;
                        RecordToRTB(Color.Red, "\n[{0}] Отправляю сообщение по Кан {1}:  {2}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(channel + 1), BitConverter.ToString(cmd));
                        answerRecieved[channel] = false;
                        kvitRecieved[channel] = false;
                        TCPClientSendData(cmd);
                    }
                }
            }
        }
        public void NewgeneratingMessage1PerevalMixed(int nz, int prg, int rp, int np, int dp, int channel, int lengthOfMessage, bool randmode = false, int cycles = 16)
        {
            bool randnz = false;
            tokenTestStop.ThrowIfCancellationRequested();
            if (nz == 254)
            {
                randnz = true;
            }
            //Thread.Sleep(1);
            byte[] cmd = new byte[1] { 0 };
            NullFunction Stop_Click = StopTestButton.PerformClick;

            for (int numberOfMes = 0; numberOfMes < cycles; numberOfMes++)
            {
                tokenTestStop.ThrowIfCancellationRequested();
                if (access[channel])
                {

                    if (randmode)
                    {
                        rp = rand.Next(5, 7);//8 безизбыточные в режиме не Ethernet  не используются
                        switch (rp)
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                break;
                            case 5:
                            case 6:
                                {
                                    switch (rand.Next(0, 4))
                                    {
                                        case 0:
                                            lengthOfMessage = 17;
                                            break;
                                        case 1:
                                            lengthOfMessage = 48;
                                            break;
                                        case 2:
                                            lengthOfMessage = 80;
                                            break;
                                        case 3:
                                            lengthOfMessage = 112;
                                            break;
                                    }
                                }
                                break;
                            case 7:
                                {
                                    lengthOfMessage = rand.Next(1, 1433);
                                }
                                break;
                        }
                    }
                    byte[] cmdNewPart = new byte[lengthOfMessage];
                    if (!onlyNulls)
                    {
                        for (int j = 0; j < lengthOfMessage; j++)
                        {
                            cmdNewPart[j] = Convert.ToByte(rand.Next(0x00, 0x100));
                        }
                    }
                    else
                    {
                        for (int j = 0; j < lengthOfMessage; j++)
                        {
                            cmdNewPart[j] = 0x00;
                        }
                    }
                    if (randnz)
                    {
                        nz = rand.Next(0, 2);
                    }
                    cmd = new byte[16] { 0x23, 0x0, 0x0, 0x0, 0x0, 0x01, 0, 0, 0, 0, Convert.ToByte(nz), Convert.ToByte(prg), Convert.ToByte(rp), Convert.ToByte(np), Convert.ToByte(dp), Convert.ToByte(Math.Pow(2, channel)) };
                    cmd = functions.Combine(cmd, cmdNewPart);
                    cmdNewPart = functions.DD(cmd.Length - 5);
                    for (int j = 0; j < 4; j++)
                    {
                        cmd[j + 1] = cmdNewPart[j];
                    }
                    while (accepted == false || incomeData[channel, 0] != 0x0)
                    {
                        tokenTestStop.ThrowIfCancellationRequested();
                        Thread.Sleep(1);
                        if (stopwatchAccept.ElapsedMilliseconds > timeOut)
                        {
                            Command0NoAnswer(_numberOFStopwatch);
                        }
                        if (stopwatchMas[channel].ElapsedMilliseconds > TimeOutMas[channel])
                        {
                            incomeData[channel, 0] = 0x0;
                            access[channel] = false;
                            stopwatchMas[channel].Stop();
                            RecordToRTB(Color.Red, "\n[{0}] ERROR! Превышено время ожидания команды {1}, время [{2}] по Кан {3} ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(incomeData[channel, 5]), Convert.ToString(Convert.ToInt32(stopwatchMas[channel].ElapsedMilliseconds)), (Convert.ToString(channel + 1)));
                            stopwatchAccept.Reset();
                            BeginInvoke(labelChanging, channel * 4, 2);
                            BeginInvoke(Stop_Click);
                            return;
                        }
                    }
                    acceptedState.Wait();
                    accepted = false;
                    if (access[channel])
                    {
                        cmdNewPart = functions.INS();
                        for (int j = 0; j < 4; j++)
                        {
                            cmd[j + 6] = cmdNewPart[j];
                        }
                        OverwriteIncomeData(cmd, channel, cmd.Length);
                        ActivatingLabel(4 * channel);

                        //Thread.Sleep(1);
                        _numberOFStopwatch = channel;
                        tokenTestStop.ThrowIfCancellationRequested();
                        RecordToRTB(Color.Red, "\n[{0}] Отправляю сообщение по Кан {1}:  {2}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(channel + 1), BitConverter.ToString(cmd));
                        answerRecieved[channel] = false;
                        kvitRecieved[channel] = false;
                        TimeOutMas[channel] = CalculationTimeOut(rp, SkMas[channel], lengthOfMessage);
                        amountOfDataOnChannel[channel] += lengthOfMessage;
                        TCPClientSendData(cmd);
                    }
                }
            }
        }
        public void NewgeneratingMessage1EthernetMixed(byte[] EthernetAdress, int nz, int prg, int rp, int np, int dp, int radiostationnumber, int lengthOfMessage, bool randmode = false, int cycles = 16)
        {
            bool randnz = false;
            tokenTestStop.ThrowIfCancellationRequested();
            if (nz == 254)
            {
                randnz = true;
            }
            //Thread.Sleep(1);
            byte[] cmd = new byte[1] { 0 };
            NullFunction Stop_Click = StopTestButton.PerformClick;
            int rd = Convert.ToInt32(EthernetCustomisationTBs[radiostationnumber, 2].Text) > 16 ? Convert.ToInt32(EthernetCustomisationTBs[radiostationnumber, 2].Text) - 16 : 0;

            for (int numberOfMes = 0; numberOfMes < cycles; numberOfMes++)
            {
                tokenTestStop.ThrowIfCancellationRequested();
                //tokenTestStop.ThrowIfCancellationRequested();
                if (access[radiostationnumber])
                {
                    if (randmode)
                    {
                        rp = rand.Next(5, 8);
                        switch (rp)
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                break;
                            case 5:
                            case 6:
                                {
                                    switch (rand.Next(0, 4))
                                    {
                                        case 0:
                                            lengthOfMessage = 17;
                                            break;
                                        case 1:
                                            lengthOfMessage = 48;
                                            break;
                                        case 2:
                                            lengthOfMessage = 80;
                                            break;
                                        case 3:
                                            lengthOfMessage = 112;
                                            break;
                                    }
                                }
                                break;
                            case 7:
                                {
                                    lengthOfMessage = rand.Next(1, rd < 1417 ? rd : 1417);//1417=1433 - 16 байт, которые докидывает ЗАС
                                }
                                break;
                        }
                    }
                    byte[] cmdNewPart = new byte[lengthOfMessage];
                    if (onlyNulls)
                    {
                        for (int j = 0; j < lengthOfMessage; j++)
                        {
                            cmdNewPart[j] = 0x00;

                        }
                    }
                    else if (numberRadiostations)
                    {
                        for (int j = 0; j < lengthOfMessage; j++)
                        {
                            cmdNewPart[j] = Convert.ToByte(radiostationnumber);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < lengthOfMessage; j++)
                        {
                            cmdNewPart[j] = Convert.ToByte(rand.Next(0x00, 0x100));
                        }
                    }
                    if (randnz)
                    {
                        nz = rand.Next(0, 2);
                    }
                    cmd = new byte[29] { 0x23, 0x0, 0x0, 0x0, 0x0, 0x01, 0, 0, 0, 0, Convert.ToByte(nz), Convert.ToByte(prg), Convert.ToByte(rp), Convert.ToByte(np),
                               Convert.ToByte(dp), 0, 0x1, EthernetAdress[0],EthernetAdress[1],EthernetAdress[2],EthernetAdress[3],EthernetAdress[4],EthernetAdress[5],EthernetAdress[6],EthernetAdress[7],EthernetAdress[8],EthernetAdress[9],EthernetAdress[10],EthernetAdress[11]};
                    cmd = functions.Combine(cmd, cmdNewPart);
                    cmdNewPart = functions.DD(cmd.Length - 5);
                    for (int j = 0; j < 4; j++)
                    {
                        cmd[j + 1] = cmdNewPart[j];
                    }
                    while (accepted == false || incomeData[radiostationnumber, 0] != 0x0)
                    {
                        tokenTestStop.ThrowIfCancellationRequested();
                        Thread.Sleep(1);
                        if (stopwatchAccept.ElapsedMilliseconds > timeOut)
                        {
                            Command0NoAnswer(_numberOFStopwatch);
                        }
                        if (stopwatchMas[radiostationnumber].ElapsedMilliseconds > TimeOutMas[radiostationnumber])
                        {
                            int n = (int)stopwatchMas[radiostationnumber].ElapsedMilliseconds;
                            incomeData[radiostationnumber, 0] = 0x0;
                            access[radiostationnumber] = false;
                            stopwatchMas[radiostationnumber].Stop();
                            RecordToRTB(Color.Red, "\n[{0}] ERROR! Превышено время ожидания команды {1}, время [{2}] по РС {3}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(incomeData[radiostationnumber, 5]), Convert.ToString(Convert.ToInt32(stopwatchMas[radiostationnumber].ElapsedMilliseconds)), (Convert.ToString(radiostationnumber + 1)));
                            stopwatchAccept.Reset();
                            BeginInvoke(labelChanging, radiostationnumber, 2);
                            BeginInvoke(Stop_Click);
                            return;
                        }
                    }
                    acceptedState.Wait();
                    accepted = false;
                    if (access[radiostationnumber])
                    {
                        cmdNewPart = functions.INS();
                        for (int j = 0; j < 4; j++)
                        {
                            cmd[j + 6] = cmdNewPart[j];
                        }
                        OverwriteIncomeData(cmd, radiostationnumber, cmd.Length);
                        ActivatingLabel(radiostationnumber);

                        //Thread.Sleep(1);
                        _numberOFStopwatch = radiostationnumber;
                        tokenTestStop.ThrowIfCancellationRequested();

                        RecordToRTB(Color.Red, "\n[{0}] Отправляю сообщение по РС {1}:  {2}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(radiostationnumber + 1), BitConverter.ToString(cmd));
                        answerRecieved[radiostationnumber] = false;
                        kvitRecieved[radiostationnumber] = false;
                        TimeOutMas[radiostationnumber] = CalculationTimeOut(rp, SkMas[radiostationnumber], lengthOfMessage);
                        TCPClientSendData(cmd);
                        amountOfDataOnChannel[radiostationnumber] += lengthOfMessage;
                    }
                }
            }
        }
        public void NewgeneratingMessage1probabilityOfBringing(int rp, int channel, int lengthOfMessage = 17)//Функция для теста по КПД Примы
        {
            tokenTestStop.ThrowIfCancellationRequested();
            string[] rpmas = new string[8] {"Режим не задан","Чайка однократный","Чайка двукратный","Чайка 'Аккорд'",
            "Чайка 'Мелодия'", "Перевал короткий код","Перевал длинный код","Безызботочные сообщения"};
            Invoke(labelFunction, CommandStateLabel, "Ком:" + 1 + ", НЗ:" + 0 + ", ПРГ:" + 0 + ", РП:" + rpmas[rp] + ", НП:" + 0 + ", ДП:" + 0 + ", Длина информации:" + 17);
            //Thread.Sleep(1);
            byte[] cmd = new byte[1] { 0 };
            if (mode)
            {
                for (int numberOfMes = 0; numberOfMes < 1000; numberOfMes++)
                {
                    tokenTestStop.ThrowIfCancellationRequested();
                    if (access[channel])
                    {
                        cmd = new byte[16] { 0x23, 0x0, 0x0, 0x0, 0x0, 0x01, 0, 0, 0, 0, Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(rp), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(Math.Pow(2, channel)) };
                        byte[] cmdNewPart = functions.INS();
                        for (int j = 0; j < 4; j++)
                        {
                            cmd[j + 6] = cmdNewPart[j];
                        }
                        cmdNewPart = new byte[lengthOfMessage];
                        if (!onlyNulls)
                        {
                            for (int j = 0; j < lengthOfMessage; j++)
                            {
                                cmdNewPart[j] = (byte)Convert.ToByte(rand.Next(0x00, 0x100));
                            }
                        }
                        cmd = functions.Combine(cmd, cmdNewPart);
                        cmdNewPart = functions.DD(cmd.Length - 5);
                        for (int j = 0; j < 4; j++)
                        {
                            cmd[j + 1] = cmdNewPart[j];
                        }
                        while (accepted == false || incomeData[channel, 0] != 0x0)
                        {
                            tokenTestStop.ThrowIfCancellationRequested();
                            Thread.Sleep(1);
                            if (stopwatchAccept.ElapsedMilliseconds > timeOut)
                            {
                                Command0NoAnswer(_numberOFStopwatch);
                            }
                            if (stopwatchMas[channel].ElapsedMilliseconds > 1000)
                            {
                                CommandNokvitation(channel, false);
                                stopwatchMas[channel].Reset();
                            }
                        }
                        if (access[channel])
                        {
                            OverwriteIncomeData(cmd, channel, cmd.Length);
                            //Thread.Sleep(1);

                            _numberOFStopwatch = channel;
                            RecordToRTB(Color.Red, "\n[{0}] Отправляю сообщение по Кан {1}:  {2}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(channel + 1), BitConverter.ToString(cmd));
                            answerRecieved[channel] = false;
                            kvitRecieved[channel] = false;
                            accepted = false;
                            TCPClientSendData(cmd);
                            Invoke(labelFunction, probabilityOfBringingCountOfSendedMessagesLabel, (numberOfMes + 1).ToString());
                        }
                    }
                }
            }
        }
        public void NewgeneratingMessage2(int nz, int prg, int rp, int np, int dp, int channel, int sach, int Name, int lengthOfMessage, int numberOfFirstChannel = 1, int numberOfLastChannel = 8, int cycles = 1)
        {//Генерация составный сообщений,  пока еще не использовалось
            byte[] cmd = new byte[1] { 0 };
            tokenTestStop.ThrowIfCancellationRequested();
            if (!mode)
            {
                cmd = new byte[16] { 0x23, 0x0, 0x0, 0x0, 0x0, 0x02,0x0,0x0,0x0,0x0, Convert.ToByte(nz), Convert.ToByte(prg), Convert.ToByte(rp),
                    Convert.ToByte(np), Convert.ToByte(dp), Convert.ToByte(channel) };
                byte[] cmdNewPart = functions.INS();
                for (int j = 0; j < 4; j++)
                    cmd[j + 6] = cmdNewPart[j];
                string sachData = Convert.ToString(sach2TextBox.Text);
                switch (sach)
                {
                    case 0:
                        cmdNewPart = new byte[1] { 0 };
                        break;
                    case 1:
                        cmdNewPart = ConvertTextToByteArray(sachData, 8);
                        break;
                    case 2:
                        cmdNewPart = ConvertTextToByteArray(sachData, 10);
                        break;
                    case 3:
                        cmdNewPart = ConvertTextToByteArray(sachData, 12);
                        break;
                    case 4:
                        cmdNewPart = ConvertTextToByteArray(sachData, 16);
                        break;
                }
                cmd = functions.Combine(cmd, cmdNewPart);
                string fileName = Convert.ToString(nfTextBox2.Text);
                cmdNewPart = ConvertTextToByteArray(fileName);
                cmd = functions.Combine(cmd, cmdNewPart);
                cmd = functions.Combine(cmd, new byte[1] { 0 });
                if (fileName.Length > 0)
                {
                    cmdNewPart = functions.DT(2);
                }
                else
                {
                    cmdNewPart = functions.DT(3);
                }
                cmd = functions.Combine(cmd, cmdNewPart);
                cmdNewPart = new byte[lengthOfMessage];
                for (int mes = 0; mes < lengthOfMessage; mes++)
                { cmdNewPart[mes] = (byte)Convert.ToByte(rand.Next(0x00, 0x100)); }
                cmd = functions.Combine(cmd, cmdNewPart);
                cmdNewPart = functions.DD(cmd.Length - 5);
                for (int j = 0; j < 4; j++)
                {
                    cmd[j + 1] = cmdNewPart[j];
                }
                TCPClientSendData(cmd);
            }
            else
            {
                for (int numberOfMes = 0; numberOfMes < cycles; numberOfMes++)
                {
                    if (numberOfMes == 1)
                    {
                        timeOutAnswer = 7000;
                    }
                    for (int i = numberOfFirstChannel - 1; i < numberOfLastChannel; i++)
                    {
                        if (access[i])
                        {
                            cmd = new byte[16] { 0x23, 0x0, 0x0, 0x0, 0x0, 0x02,0x0,0x0,0x0,0x0, Convert.ToByte(nz), Convert.ToByte(prg), Convert.ToByte(rp),
                    Convert.ToByte(np), Convert.ToByte(dp), Convert.ToByte(Math.Pow( 2,i)) };
                            byte[] cmdNewPart = functions.INS();
                            for (int j = 0; j < 4; j++)
                                cmd[j + 6] = cmdNewPart[j];
                            cmdNewPart = new byte[sach];
                            for (int mes = 0; mes < sach; mes++)
                            { cmdNewPart[mes] = (byte)Convert.ToByte(rand.Next(0x00, 0x100)); }
                            cmd = functions.Combine(cmd, cmdNewPart);
                            cmdNewPart = new byte[Name];
                            for (int mes = 0; mes < sach - 1; mes++)
                            { cmdNewPart[mes] = (byte)Convert.ToByte(rand.Next(0x00, 0x100)); }
                            cmdNewPart[sach - 1] = 0x00;
                            cmd = functions.Combine(cmd, cmdNewPart);
                            cmdNewPart = functions.DT(2);
                            cmd = functions.Combine(cmd, cmdNewPart);
                            cmdNewPart = new byte[lengthOfMessage];
                            for (int mes = 0; mes < lengthOfMessage; mes++)
                            { cmdNewPart[mes] = (byte)Convert.ToByte(rand.Next(0x00, 0x100)); }
                            cmd = functions.Combine(cmd, cmdNewPart);
                            cmdNewPart = functions.DD(cmd.Length - 5);
                            for (int j = 0; j < 4; j++)
                            {
                                cmd[j + 1] = cmdNewPart[j];
                            }
                            while (accepted == false || incomeData[i, 0] != 0x0)
                            {
                                Thread.Sleep(1);
                                if (stopwatchAccept.ElapsedMilliseconds > 4000)
                                {
                                    Command0NoAnswer(_numberOFStopwatch);
                                }
                                if (stopwatchMas[i].ElapsedMilliseconds > TimeOutMas[i])
                                {
                                    CommandNokvitation(i);
                                }
                            }
                            if (access[i])
                            {
                                OverwriteIncomeData(cmd, i, cmd.Length);
                                ActivatingLabel(4 * i);
                                //Thread.Sleep(1);
                                _numberOFStopwatch = i;
                                RecordToRTB(Color.Red, "\n[{0}] Отправляю cоставное сообщение по Кан {1}:  {2}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(i + 1), BitConverter.ToString(cmd));
                                TCPClientSendData(cmd);
                                accepted = false;
                            }
                        }
                    }
                }
            }
        }
        byte[] ConvertTextToByteArray(string str)
        {
            str = str.Replace("–", "");
            str = str.Replace("-", "");
            str = str.Replace(" ", "");
            str = str.Replace("\r", "");
            str = str.Replace("\n", "");
            return ConvertTextToByteArray(str, ((str.Length + 1) / 2) + 1);
        }
        byte[] ConvertTextToByteArray(string str, int arrayLength)
        {
            byte[] convertedMas = new byte[arrayLength];
            int n = 0;
            bool notInt = false;

            str = str.Replace("–", "");
            str = str.Replace("-", "");
            str = str.Replace(" ", "");
            str = str.Replace("\r", "");
            str = str.Replace("\n", "");
            //convertedMas = new byte[((str.Length + 1) / 2) + 1];
            if (str.Length > arrayLength * 2)
            {
                str = str.Substring(0, arrayLength * 2);
            }
            else
            {
                if (str.Length % 2 != 0)
                {
                    notInt = true;
                }
            }
            int i;
            for (i = 0; i < (str.Length / 2); i++)
            {
                n = Int32.Parse(str.Substring(2 * i, 2), System.Globalization.NumberStyles.HexNumber);
                convertedMas[i] = Convert.ToByte(n);
            }
            i--;
            if (notInt)
            {
                convertedMas[i + 1] = Convert.ToByte(16 * Int32.Parse(str.Substring(2 * i + 2, 1), System.Globalization.NumberStyles.HexNumber));

            }
            return convertedMas;
        }
        public void NewgeneratingMessage10(int channel, int sk, int shl, int zas, int PPRCh, int TS, int KVT, int Tiv, int numberOfFirstChannel = 1, int numberOfLastChannel = 8, bool ethernetTest = false)
        {//Генерация настроечных команд для каналов
            int[] skmas = new int[10] { 75, 150, 300, 1200, 2400, 4800, 9600, 16000, 50, 100 };
            string[] TSmas = new string[3] { "С1-ТГ", "С1-ФЛ-БИ", "Ethernet" };
            Invoke(labelFunction, CommandStateLabel, "Ком:" + 10 + ", Скорость:" + skmas[sk] + ", ШЛ:" + shl + ", НЗ:" + zas + ", ППРЧ:" + PPRCh + ", ТС:" + TSmas[TS] + ", КВТ:" + KVT + ", Тив:" + Tiv);
            Invoke(labelFunction, SkorostLabel, "Скорость:" + skmas[sk]);
            if (!mode)
            {
                byte[] cmd;
                byte[] cmdNewPart;
                int i = channel;
                cmd = new byte[14] { 0x23, 0x0, 0x0, 0x0, 0x0, 0x0A,Convert.ToByte(i),
                            Convert.ToByte(sk), Convert.ToByte(shl), Convert.ToByte(zas),Convert.ToByte(PPRCh), Convert.ToByte(TS), Convert.ToByte(KVT), Convert.ToByte(Tiv) }; //Convert.ToByte(DP), Convert.ToByte(PPRCh), Convert.ToByte(TS), Convert.ToByte(KVT) };
                cmdNewPart = functions.DD(cmd.Length - 5);
                for (int j = 0; j < 4; j++)
                    cmd[j + 1] = cmdNewPart[j];
                TCPClientSendData(cmd);
            }
            else
            {

                tokenTestStop.ThrowIfCancellationRequested();
                for (int i = numberOfFirstChannel - 1; i < numberOfLastChannel; i++)
                {
                    if (access[i])
                    {
                        byte[] cmd = new byte[14] { 0x23, 0x0, 0x0, 0x0, 0x0, 0x0A,Convert.ToByte(Math.Pow( 2,i)),
                            Convert.ToByte(sk), Convert.ToByte(shl), Convert.ToByte(zas),Convert.ToByte(PPRCh), Convert.ToByte(TS), Convert.ToByte(KVT), Convert.ToByte(Tiv) }; //Convert.ToByte(DP), Convert.ToByte(PPRCh), Convert.ToByte(TS), Convert.ToByte(KVT) };
                        byte[] cmdNewPart = functions.DD(cmd.Length - 5);
                        SkMas[i] = sk;
                        for (int j = 0; j < 4; j++)
                            cmd[j + 1] = cmdNewPart[j];
                        if (ethernetTest)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                CheckAnswerAndKvitation(i * 4 + j, true, true);
                            }
                        }
                        else
                        {
                            CheckAnswerAndKvitation(i);
                        }
                        if (access[i])
                        {
                            OverwriteIncomeData(cmd, i, cmd.Length);
                            // ActivatingLabel(i, 1);
                            //Thread.Sleep(1);
                            _numberOFStopwatch = i;
                            RecordToRTB(Color.Red, "\n[{0}] Отправляю сообщение по Кан {1}:  {2}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(i + 1), BitConverter.ToString(cmd));
                            accepted = false;
                            TCPClientSendData(cmd);

                            for (int k = 0; k < cmd.Length; k++)
                            {
                                send10MessageMas[i, k] = cmd[k];
                            }
                        }
                    }
                }
                timeOutAnswer = Convert.ToUInt16(timeOutTB.Text);
            }
        }
        public void NewgeneratingMessage10SCh(int channel, int sk, int shl, int zas, int PPRCh, int TS, int KVT, int Tiv)//настройка канала в Mixed test
        {
            NullFunction Stop_Click = StopTestButton.PerformClick;
            tokenTestStop.ThrowIfCancellationRequested();
            if (access[channel])
            {


                byte[] cmd = new byte[14] { 0x23, 0x0, 0x0, 0x0, 0x0, 0x0A,Convert.ToByte(Math.Pow( 2,channel)),
                            Convert.ToByte(sk), Convert.ToByte(shl), Convert.ToByte(zas),Convert.ToByte(PPRCh), Convert.ToByte(TS), Convert.ToByte(KVT), Convert.ToByte(Tiv) }; //Convert.ToByte(DP), Convert.ToByte(PPRCh), Convert.ToByte(TS), Convert.ToByte(KVT) };


                SkMas[channel] = sk;

                byte[] cmdNewPart = functions.DD(cmd.Length - 5);
                for (int j = 0; j < 4; j++)
                    cmd[j + 1] = cmdNewPart[j];
                while (accepted == false || incomeData[channel, 0] != 0x0)
                {
                    tokenTestStop.ThrowIfCancellationRequested();
                    Thread.Sleep(1);
                    if (stopwatchAccept.ElapsedMilliseconds > timeOut)
                    {
                        Command0NoAnswer(_numberOFStopwatch);
                    }
                    if (stopwatchMas[channel].ElapsedMilliseconds > timeOutAnswer)
                    {
                        incomeData[channel, 0] = 0x0;
                        access[channel] = false;
                        stopwatchMas[channel].Stop();
                        RecordToRTB(Color.Red, "\n[{0}] ERROR! Превышено время ожидания команды {1}, время [{2}] по Кан {3} ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(incomeData[channel, 5]), Convert.ToString(Convert.ToInt32(stopwatchMas[channel].ElapsedMilliseconds)), (Convert.ToString(channel + 1)));
                        stopwatchAccept.Reset();
                        BeginInvoke(labelChanging, 4 * channel, 2);
                        BeginInvoke(Stop_Click);
                        return;
                    }
                }
                acceptedState.Wait();
                accepted = false;
                if (access[channel])
                {
                    OverwriteIncomeData(cmd, channel, cmd.Length);
                    // ActivatingLabel(channel, 1);
                    _numberOFStopwatch = channel;
                    RecordToRTB(Color.Red, "\n[{0}] Отправляю сообщение по Кан {1}:  {2}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(channel + 1), BitConverter.ToString(cmd));
                    TCPClientSendData(cmd);
                }
            }
        }
        public void NewgeneratingMessage11(int ch, int m, int s)//Настройка времени для примы
        {
            byte[] cmd = new byte[1] { 0 };
            cmd = new byte[9] { 0x23, 0x04, 0x0, 0x0, 0x0, 0x0B, Convert.ToByte(ch), Convert.ToByte(m), Convert.ToByte(s) };
            TCPClientSendData(cmd);
        }
        public void NewgeneratingMessage12(int zas1 = 1, int zas2 = 0)//настройка засов для примы
        {
            if (!mode)
            {
                byte[] cmd = new byte[8] { 0x23, 0x03, 0x0, 0x0, 0x0, 0x0C, Convert.ToByte(zas1), Convert.ToByte(zas2) };
                TCPClientSendData(cmd);
            }
            else if (mode)
            {
                timeOutAnswer = 7000;
                while (accepted == false)
                {
                    Thread.Sleep(1);
                    if (stopwatchAccept.ElapsedMilliseconds > timeOut)
                    {
                        accepted = true;
                        RecordToRTB(Color.Red, "\n[{0}] ERROR! Превышено время ожидания  ПОДТВЕРЖДЕНИЯ {1}  ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(Convert.ToInt32(stopwatchHandTest.ElapsedMilliseconds)));
                        throw new MessageException();
                    }
                }
                int countOfResendedMessages = 0;
                for (int i = 0; i < 8; i++)
                {
                    while (incomeData[i, 0] != 0x0)
                    {

                        if (stopwatchMas[i].ElapsedMilliseconds > timeOutAnswer)
                        {
                            CommandNokvitation(i);
                        }
                        Thread.Sleep(1);
                    }
                }
                byte[] cmd = new byte[8] { 0x23, 0x03, 0x0, 0x0, 0x0, 0x0C, Convert.ToByte(zas1), Convert.ToByte(zas2) };
                for (int j = 0; j < cmd.Length; j++)
                {
                    income12[j] = cmd[j];
                }
                RecordToRTB(Color.Red, "\n Отправляю сообщение Номер 12 {0}  ", BitConverter.ToString(cmd));
                _numberOFStopwatch = 0;
                accepted = false;
                TCPClientSendData(cmd);
                while (accepted == false || income12[0] != 0x00)
                {
                    Thread.Sleep(1);
                    tokenTestStop.ThrowIfCancellationRequested();

                    if (stopwatchAccept.ElapsedMilliseconds > timeOut)
                    {
                        Command0NoAnswer(_numberOFStopwatch);
                    }
                    if (stopwatchMas[_numberOFStopwatch].ElapsedMilliseconds > timeOutAnswer)
                    {
                        income12[0] = 0x0;
                        access[_numberOFStopwatch] = false;
                        stopwatchMas[_numberOFStopwatch].Stop();
                        RecordToRTB(Color.Red, "\n[{0}] ERROR! Превышено время ожидания команды 12, время [{1}] ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(Convert.ToInt32(stopwatchMas[_numberOFStopwatch].ElapsedMilliseconds)));
                        stopwatchAccept.Reset();
                        BeginInvoke(labelChanging, _numberOFStopwatch, 2);
                        throw new MessageException();
                    }
                }
                while (!commandIsCompleted)
                {
                    if (countOfResendedMessages >= 3)
                    {
                        Command0NoAnswer(_numberOFStopwatch);
                    }
                    RecordToRTB(Color.Red, "\n[{0}] Отправляю повторное 12 сообщение {1} ", Convert.ToString(stopwatchFullTest.Elapsed), BitConverter.ToString(income12));
                    accepted = false;
                    TCPClientSendData(income12);
                    countOfResendedMessages++;
                    Thread.Sleep(100);
                }
            }
        }
        public void NewgeneratingMessage13()//Сбросить канал в исходное состоянеи
        {
            byte[] cmd = new byte[1] { 0 };

            if (numPA.Checked)
            {
                int channel = (1 * Convert.ToInt16(checkBoxChannel1.Checked) + 2 * Convert.ToInt16(checkBoxChannel2.Checked) + 4 * Convert.ToInt16(checkBoxChannel3.Checked)
                                                           + 8 * Convert.ToInt16(checkBoxChannel4.Checked) + 16 * Convert.ToInt16(checkBoxChannel5.Checked) + 32 * Convert.ToInt16(checkBoxChannel6.Checked)
                                                           + 64 * Convert.ToInt16(checkBoxChannel7.Checked) + 128 * Convert.ToInt16(checkBoxChannel8.Checked));
                cmd = new byte[7] { 0x23, 0x02, 0x0, 0x0, 0x0, 0x0D, Convert.ToByte(channel) };//Convert.ToByte(Math.Pow(2, channel)) };
            }
            else
            {
                cmd = new byte[7] { 0x23, 0x02, 0x0, 0x0, 0x0, 0x0D, 0x0 };//Convert.ToByte(Math.Pow(2, channel)) };
                cmd = functions.Combine(cmd, Com1adressesgeneration());
            }
            byte[] cmdNewPart = functions.DD(cmd.Length - 5);
            for (int j = 0; j < 4; j++)
                cmd[j + 1] = cmdNewPart[j];
            RecordToRTB(Color.Red, "\n Отправляю сообщение Номер 13 {0}  ", BitConverter.ToString(cmd));
            TCPClientSendData(cmd);
        }
        public void NewgeneratingMessage14()//Сбросить канал в исходное состоянеи
        {
            byte[] cmd = new byte[1] { 0 };
            if (numPA.Checked)
            {
                int channel = (1 * Convert.ToInt16(checkBoxChannel1.Checked) + 2 * Convert.ToInt16(checkBoxChannel2.Checked) + 4 * Convert.ToInt16(checkBoxChannel3.Checked)
                                           + 8 * Convert.ToInt16(checkBoxChannel4.Checked) + 16 * Convert.ToInt16(checkBoxChannel5.Checked) + 32 * Convert.ToInt16(checkBoxChannel6.Checked)
                                           + 64 * Convert.ToInt16(checkBoxChannel7.Checked) + 128 * Convert.ToInt16(checkBoxChannel8.Checked));
                cmd = new byte[7] { 0x23, 0x02, 0x0, 0x0, 0x0, 0x0E, Convert.ToByte(channel) };//Convert.ToByte(Math.Pow(2, channel)) };
            }
            else
            {
                cmd = new byte[7] { 0x23, 0x02, 0x0, 0x0, 0x0, 0x0E, 0x0 };//Convert.ToByte(Math.Pow(2, channel)) };
                cmd = functions.Combine(cmd, Com1adressesgeneration());
            }
            byte[] cmdNewPart = functions.DD(cmd.Length - 5);
            for (int j = 0; j < 4; j++)
                cmd[j + 1] = cmdNewPart[j];
            RecordToRTB(Color.Red, "\n Отправляю сообщение Номер 14 {0}  ", BitConverter.ToString(cmd));
            TCPClientSendData(cmd);
        }
        public void NewgeneratingMessage255(bool mode = false, int timeout = 500)//Контроль соединения
        {
            if (!mode)
            {
                byte[] cmd = new byte[1] { 0 };
                cmd = new byte[6] { 0x23, 0x01, 0x0, 0x0, 0x0, 0xFF };
                TCPClientSendData(cmd);
            }
            else if (mode)
            {
                while (!accepted)
                {
                    if (stopwatchAccept.ElapsedMilliseconds > timeOut)
                    {
                        RecordToRTB(Color.Red, "\n[{0}] ERROR! Превышено время ожидания  подтверждения контроля соединения, время  [{1}]  ", Convert.ToString(stopwatchFullTest.Elapsed),
                        Convert.ToString(Convert.ToInt32(stopwatchAccept.ElapsedMilliseconds)));
                        throw new MessageException();
                    }
                }
                if (accepted && stopwatchAccept.ElapsedMilliseconds < timeout)
                {
                    byte[] cmd = new byte[6] { 0x23, 0x01, 0x0, 0x0, 0x0, 0xFF };
                    stopwatchAccept.Restart();
                    accepted = false;
                    TCPClientSendData(cmd);
                    RecordToRTB(Color.Red, "\n Отправляю контроль соединения {0}  ", BitConverter.ToString(cmd));
                }
            }
        }

        public void NewgeneratingMessage16RSTest(byte[,] customizationMas, int numberOfFirstChannel = 1, int numberOfLastChannel = 8)//Команда настройки радиостанция для РС-Теста
        {
            byte[] cmd = new byte[1];
            byte[] cmdNewPart;
            byte[] adresses = Ethernetadressesgeneration();
            int numberOfAdress = 0;
            int l = 0;
            int r = 0;
            if (mode)
            {
                for (int i = numberOfFirstChannel - 1; i < numberOfLastChannel; i++)
                {
                    // if (access[i])
                    // {
                    for (int j = 0; j < 4; j++)
                    {


                        //if (access[i])
                        //{

                        r = i * 4 + j;
                        tokenTestStop.ThrowIfCancellationRequested();
                        if (RadioStationcheckboxes[r].Checked && access[r])
                        {
                            cmd = new byte[6] { 0x23, 0x23, 0x0, 0x0, 0x0, 0x10 };
                            cmdNewPart = new byte[24];
                            for (int q = 0; q < 24; q++)
                            { cmdNewPart[q] = adresses[numberOfAdress * 24 + q]; }
                            numberOfAdress++;
                            cmd = functions.Combine(cmd, cmdNewPart);
                            BeginInvoke(labelFunction, CommandStateLabel, "Ком:" + 16 + ", РС " + Convert.ToString(r));// + ", Шл:" + shl + ", НЗ:" + nz + ", КП:" + kp + ", КПП:" + kpp + ", РД:" + rd + ", КВТ:" + KVT);
                            cmdNewPart = new byte[11] { Convert.ToByte(Math.Pow(2, i)), customizationMas[r,0], customizationMas[r, 1], customizationMas[r, 2],
                                customizationMas[r,3], customizationMas[r,4], customizationMas[r,5], customizationMas[r,6], customizationMas[r,7], customizationMas[r,8],customizationMas[r,9]   };
                            SkMas[r] = customizationMas[r, 9];
                            cmd = functions.Combine(cmd, cmdNewPart);
                            cmdNewPart = functions.DD(cmd.Length - 5);
                            for (int k = 0; k < 4; k++)
                                cmd[k + 1] = cmdNewPart[k];
                            if (i > 0) { l = i - 1; } else { l = 31; } //проверка на результат с прошлой радиостанции
                            while (accepted == false || incomeData[i, 0] != 0x0 || incomeData[l, 0] != 0x0)
                            {
                                tokenTestStop.ThrowIfCancellationRequested();
                                Thread.Sleep(1);
                                if (stopwatchAccept.ElapsedMilliseconds > timeOut)
                                {
                                    Command0NoAnswer(_numberOFStopwatch);
                                }
                                if (stopwatchMas[r].ElapsedMilliseconds > timeOutAnswer)
                                {
                                    CommandNokvitation(i);
                                }
                            }
                            if (access[r])
                            {
                                OverwriteIncomeData(cmd, r, cmd.Length);
                                ActivatingLabel(r);
                                //Thread.Sleep(1);
                                _numberOFStopwatch = r;
                                RecordToRTB(Color.Red, "\n[{0}] Отправляю сообщение по РС {1}:  {2}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(r + 1), BitConverter.ToString(cmd));
                                accepted = false;
                                TCPClientSendData(cmd);
                                //     }
                            }
                        }
                    }
                    timeOutAnswer = 7000;
                }
            }
        }
        public void NewgeneratingMessage16(byte[,] customizationMas)//Команда для настройки радиостанций
        {
            byte[] cmd = new byte[1];
            byte[] cmdNewPart;
            byte[] adresses = Ethernetadressesgeneration();
            int numberOfAdress = 0;
            try
            {
                if (!mode)
                {
                    int r = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        //if (access[i])
                        //{
                        for (int j = 0; j < 4; j++)
                        {
                            r = i * 4 + j;
                            if (RadioStationcheckboxes[r].Checked)
                            {
                                cmd = new byte[6] { 0x23, 0x00, 0x0, 0x0, 0x0, 0x10 };
                                cmdNewPart = new byte[24];
                                for (int q = 0; q < 24; q++)
                                { cmdNewPart[q] = adresses[numberOfAdress * 24 + q]; }
                                numberOfAdress++;
                                cmd = functions.Combine(cmd, cmdNewPart);
                                cmdNewPart = new byte[11] { Convert.ToByte(Math.Pow(2, i)), customizationMas[r,0], customizationMas[r, 1], customizationMas[r, 2],
                                customizationMas[r,3], customizationMas[r,4], customizationMas[r,5], customizationMas[r,6],customizationMas[r,7],customizationMas[r,8],customizationMas[r,9]   };
                                SkMas[r] = customizationMas[r, 9];
                                cmd = functions.Combine(cmd, cmdNewPart);
                                cmdNewPart = functions.DD(cmd.Length - 5);
                                for (int k = 0; k < 4; k++)
                                    cmd[k + 1] = cmdNewPart[k];
                                while (accepted == false)
                                {
                                    tokenTestStop.ThrowIfCancellationRequested();
                                    Thread.Sleep(1);
                                    if (stopwatchAccept.ElapsedMilliseconds > timeOut)
                                    {
                                        accepted = true;
                                        RecordToRTB(Color.Red, "\n[{0}] ERROR! Превышено время ожидания  ПОДТВЕРЖДЕНИЯ {1}  ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(Convert.ToInt32(stopwatchHandTest.ElapsedMilliseconds)));
                                        stopwatchAccept.Reset();
                                        break;
                                    }
                                }
                                _accepted16command = false;
                                stopwatchAccept.Restart();
                                _numberOFStopwatch = r;
                                OverwriteIncomeData(cmd, r, cmd.Length);
                                TCPClientSendData(cmd);
                                if (perfomanceTestMode)
                                {

                                    RecordToRTB(Color.Red, "\n[{0}] Отправляю сообщение по РС {1}:  {2}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(r + 1), BitConverter.ToString(cmd));
                                }
                                accepted = false;
                                //Thread.Sleep(1);
                                while (_accepted16command == false || accepted == false)
                                {
                                    tokenTestStop.ThrowIfCancellationRequested();
                                    Thread.Sleep(1);
                                    if (stopwatchAccept.ElapsedMilliseconds > timeOut)
                                    {
                                        Command0NoAnswer(_numberOFStopwatch);
                                    }
                                    if (stopwatchHandTest.ElapsedMilliseconds > timeOut)
                                    {
                                        RecordToRTB(Color.Red, "{0}   - ANSWER TIMEOUT ERROR", ("РС " + Convert.ToString(r + 1)));
                                        stopwatchHandTest.Reset();
                                        _accepted16command = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    NullFunction Stop_Click = StopTestButton.PerformClick;
                    int r = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        //if (access[i])
                        //{
                        for (int j = 0; j < 4; j++)
                        {
                            r = i * 4 + j;
                            tokenTestStop.ThrowIfCancellationRequested();
                            if (RadioStationcheckboxes[r].Checked && access[r])
                            {
                                cmd = new byte[6] { 0x23, 0x23, 0x0, 0x0, 0x0, 0x10 };
                                cmdNewPart = new byte[24];
                                for (int q = 0; q < 24; q++)
                                { cmdNewPart[q] = adresses[numberOfAdress * 24 + q]; }
                                numberOfAdress++;
                                cmd = functions.Combine(cmd, cmdNewPart);
                                BeginInvoke(labelFunction, CommandStateLabel, "Ком:" + 16 + ", РС " + Convert.ToString(r)); //+ ", Шл:" + shl + ", НЗ:" + nz + ", КП:" + kp + ", КПП:" + kpp + ", РД:" + rd + ", КВТ:" + KVT);
                                cmdNewPart = new byte[11] { Convert.ToByte(Math.Pow(2, i)), customizationMas[r,0], customizationMas[r, 1], customizationMas[r, 2],
                                customizationMas[r,3], customizationMas[r,4], customizationMas[r,5], customizationMas[r,6], customizationMas[r,7], customizationMas[r,8],customizationMas[r,9] };
                                SkMas[r] = customizationMas[r, 9];
                                cmd = functions.Combine(cmd, cmdNewPart);
                                cmdNewPart = functions.DD(cmd.Length - 5);
                                for (int k = 0; k < 4; k++)
                                    cmd[k + 1] = cmdNewPart[k];
                                CheckAnswerAndKvitation(r);
                                if (access[r])
                                {
                                    OverwriteIncomeData(cmd, r, cmd.Length);
                                    ActivatingLabel(r);
                                    //Thread.Sleep(1);
                                    _numberOFStopwatch = r;
                                    RecordToRTB(Color.Red, "\n[{0}] Отправляю сообщение по РС {1}:  {2}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(r + 1), BitConverter.ToString(cmd));
                                    accepted = false;
                                    TCPClientSendData(cmd);
                                    for (int k = 0; k < cmd.Length; k++)
                                    {
                                        send16MessageMas[r, k] = cmd[k];
                                    }
                                }
                            }
                            //}
                        }
                        timeOutAnswer = 7000;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
        private void Command0NoAnswer(int numberOFChannel, bool radiostationType = false)//выведение ошибки об отсутствии подтверждения
        {
            if (!radiostationType)
            {
                RecordToRTB(Color.Red, "\n[{0}] ERROR! Превышено время ожидания правильной КОМАНДЫ 0, время  [{1}] по Кан {2}", Convert.ToString(stopwatchFullTest.Elapsed),
                    Convert.ToString(Convert.ToInt32(stopwatchAccept.ElapsedMilliseconds)), (Convert.ToString(numberOFChannel + 1)));
                BeginInvoke(labelChanging, 4 * numberOFChannel, 1);
                RecordToRTB(Color.Red, "\n[{0}] Состояние 10ой команды Кан {1}: Ск:{2}, Шл:{3}, НЗ:{4}, ППРЧ:{5}, ТС:{6}, КВТ:{7},Тив:{8}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(numberOFChannel), Convert.ToString(send10MessageMas[numberOFChannel, 7]),
    Convert.ToString(send10MessageMas[numberOFChannel, 8]), Convert.ToString(send10MessageMas[numberOFChannel, 9]), Convert.ToString(send10MessageMas[numberOFChannel, 10]),
    Convert.ToString(send10MessageMas[numberOFChannel, 11]), Convert.ToString(send10MessageMas[numberOFChannel, 12]), Convert.ToString(send10MessageMas[numberOFChannel, 13]));
            }
            else
            {
                RecordToRTB(Color.Red, "\n[{0}] ERROR! Превышено время ожидания правильной КОМАНДЫ 0, время  [{1}] по РС {2}", Convert.ToString(stopwatchFullTest.Elapsed),
                    Convert.ToString(Convert.ToInt32(stopwatchAccept.ElapsedMilliseconds)), (Convert.ToString(numberOFChannel + 1)));
                BeginInvoke(labelChanging, numberOFChannel, 1);
                RecordToRTB(Color.Red, "\n[{0}] Состояние 16ой команды РС {1}: НЗ:{2}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(numberOFChannel + 1), Convert.ToString(send16MessageMas[numberOFChannel, 32]));
            }
            stopwatchAccept.Reset();
            incomeData[numberOFChannel, 0] = 0x0;
            access[numberOFChannel] = false;
            stopwatchMas[numberOFChannel].Stop();
            throw new MessageException();
        }
        private void CommandNokvitation(int numberOFChannel, bool stopNeeded = true, bool radiostationType = false)//выведение ошибки об отсутствии квитанции
        {

            incomeData[numberOFChannel, 0] = 0x0;
            stopwatchMas[numberOFChannel].Stop();
            if (!radiostationType)
            {
                RecordToRTB(Color.Red, "\n[{0}] ERROR! Превышено время ожидания ответа на команду {1}, время [{2}] по Кан {3}", Convert.ToString(stopwatchFullTest.Elapsed),
                    Convert.ToString(incomeData[numberOFChannel, 5]), Convert.ToString(Convert.ToInt32(stopwatchMas[numberOFChannel].ElapsedMilliseconds)), (Convert.ToString(numberOFChannel + 1)));
                RecordToRTB(Color.Red, "\n[{0}] Состояние 10ой команды Кан {1}: Ск:{2}, Шл:{3}, НЗ:{4}, ППРЧ:{5}, ТС:{6}, КВТ:{7},Тив:{8}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(numberOFChannel), Convert.ToString(send10MessageMas[numberOFChannel, 7]),
Convert.ToString(send10MessageMas[numberOFChannel, 8]), Convert.ToString(send10MessageMas[numberOFChannel, 9]), Convert.ToString(send10MessageMas[numberOFChannel, 10]),
Convert.ToString(send10MessageMas[numberOFChannel, 11]), Convert.ToString(send10MessageMas[numberOFChannel, 12]), Convert.ToString(send10MessageMas[numberOFChannel, 13]));
            }
            else
            {
                RecordToRTB(Color.Red, "\n[{0}] ERROR! Превышено время ожидания ответа на РС {1}, время [{2}] по Кан {3}", Convert.ToString(stopwatchFullTest.Elapsed),
               Convert.ToString(incomeData[numberOFChannel, 5]), Convert.ToString(Convert.ToInt32(stopwatchMas[numberOFChannel].ElapsedMilliseconds)), (Convert.ToString(numberOFChannel + 1)));
                RecordToRTB(Color.Red, "\n[{0}] Состояние 16ой команды РС {1} : НЗ:{2}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(numberOFChannel + 1), Convert.ToString(send16MessageMas[numberOFChannel, 32]));
            }
            if (stopNeeded)
            {
                access[numberOFChannel] = false;
                stopwatchAccept.Reset();
                if (!radiostationType)
                {
                    BeginInvoke(labelChanging, 4 * numberOFChannel, 2);
                }
                else
                {
                    BeginInvoke(labelChanging, numberOFChannel, 2);
                }
                throw new MessageException();
            }
        }
        public void TCPClientSendData(byte[] DataToSend)//Данные пересылаются через класс NetWorker
        {
            try
            {
                net.TCPSendData(DataToSend);
                if (mode)
                {
                    stopwatchMas[_numberOFStopwatch].Restart();
                    stopwatchAccept.Restart();
                }
                if (!mode)
                {
                    RecordToRTB(Color.Red, BitConverter.ToString(DataToSend));
                    stopwatchHandTest.Restart();
                }
            }
            catch (SocketException)
            {
                net.TcpClose();
                Invoke(new NullFunction(StartTestButton.PerformClick));
            }
            catch (ObjectDisposedException)
            {
                net.TcpClose();
                StartTestButton.PerformClick();
            }
        }
        public void UnexpectedDataInPackage(byte[] buffer, int errorbyte)
        {
            byte[] firstpart = new byte[errorbyte];
            byte[] lastpart = new byte[buffer.Length - errorbyte - 1];
            Array.Copy(buffer, firstpart, firstpart.Length);
            Array.Copy(buffer, errorbyte + 1, lastpart, 0, lastpart.Length);
            RecordToRTB(Color.Red, "\n ERROR! Неожиданная информация в {0} байте пакета  {1}", Convert.ToString(errorbyte + 1), BitConverter.ToString(firstpart) + "-", "", "", "", "", "", "", "", false);
            RecordToRTB(Color.Black, "{0}", Convert.ToString(buffer[errorbyte], 16).ToUpper(), "", "", "", "", "", "", "", "", false);
            RecordToRTB(Color.Red, "{0}", "-" + BitConverter.ToString(lastpart));
            //  throw new MessageException();
        }
        public void TCPClientReceiveData(byte[] receiveMessage)//Принятые данные сразу же проверяются на правильность
        {
            if (!mode)//Проверка сообщения в ручном режиме
            {
                CheckHandMessages(receiveMessage);
            }
            else if (mode)//Проверка сообщения в автоматическом режиме
            {
                CheckAutoMessages(receiveMessage);
            }
        }
        #region Проверка полученных данных
        public void CheckHandMessages(byte[] receiveMessage)
        {
            if (receiveMessage.Length == 9 && receiveMessage[5] == 0)
            {
                CheckMessage0(receiveMessage);
            }
            else if (receiveMessage[5] == 0x7f)
            {
                CheckMessagePrintF(receiveMessage);
            }
            else if (receiveMessage[5] == 0x7e)
            {
                CheckMessageLongPrintF(receiveMessage);
            }
            else if (!_accepted16command && receiveMessage[5] == 0x10)
            {

                handMessageReadyToSend = true;
                stopwatchHandTest.Stop();
                RecordToRTB(Color.Blue, "Ответ");
                RecordToRTB(Color.Black, BitConverter.ToString(receiveMessage));
                _accepted16command = true;

                //recordToRTB(Color.Black, "[{0}] Приём:  [{1}] {2}", Convert.ToString(stopwatchFullTest.Elapsed),
                //    Convert.ToString(Convert.ToInt32(stopwatchAccept.Elapsed.Milliseconds)), BitConverter.ToString(receiveMessage)); 
                ///Для теста макроса?
            }
            else
            {
                handMessageReadyToSend = true;
                stopwatchHandTest.Stop();
                stopwatchAccept.Stop();
                RecordToRTB(Color.Blue, "Ответ");
                RecordToRTB(Color.Black, BitConverter.ToString(receiveMessage));
            }
        }
        public void CheckAutoMessages(byte[] receiveMessage)// проверка полученных данных осуществлена в иерархии
        {
            switch (receiveMessage[5])
            {
                case (0x00):
                    CheckMessage0(receiveMessage);
                    break;
                case (0x7e):
                    CheckMessageLongPrintF(receiveMessage);
                    break;
                case (0x7f):
                    CheckMessagePrintF(receiveMessage);//Отдельного внимания заслуживают ПринтФы, они были сделаны также в целях отладки и просто выдают отладочную информацию в закодированном виде
                    break;
                case (0x01):
                    CheckAutoMessages1(receiveMessage);
                    break;
                case (0x0A):
                    CheckAutoMessages10(receiveMessage);
                    break;
                case (0x0C):
                    CheckAutoMessages12(receiveMessage);
                    break;
                case (0x03):
                    CheckAutoMessages3(receiveMessage);
                    break;
                case (0x10):
                    CheckAutoMessages16(receiveMessage);
                    break;
                default:
                    RecordToRTB(Color.Black, "[{0}] Мусор:{1}    ", Convert.ToString(stopwatchFullTest.Elapsed), BitConverter.ToString(receiveMessage));
                    break;
            }
        }

        private void CheckMessageLongPrintF(byte[] receiveMessage)
        {
            int lengthOfPartOfMessage = receiveMessage.Length - 8;
            bool IsFinalPackage = Convert.ToBoolean(receiveMessage[7] / 128);
            int numberOfPackage = receiveMessage[7] % 128;
            int numberOfChannel = receiveMessage[6];
            byte[] resultedMessage = new byte[0];
            if (numberOfPackage == flagsOfLongPrintF[numberOfChannel, 0])
            {
                for (int i = 0; i < lengthOfPartOfMessage; i++)
                {
                    longPrintFMessages[numberOfChannel, i + flagsOfLongPrintF[numberOfChannel, 1]] = receiveMessage[i + 8];
                }
                flagsOfLongPrintF[numberOfChannel, 0]++;
                flagsOfLongPrintF[numberOfChannel, 1] += lengthOfPartOfMessage;
                flagsOfLongPrintF[numberOfChannel, 2] = lengthOfPartOfMessage;
            }
            else if (numberOfPackage > flagsOfLongPrintF[numberOfChannel, 0])
            {
                flagsOfLongPrintF[numberOfChannel, 1] += ((numberOfPackage - flagsOfLongPrintF[numberOfChannel, 0]) * flagsOfLongPrintF[numberOfChannel, 2]);
                for (int i = 0; i < lengthOfPartOfMessage; i++)
                {
                    longPrintFMessages[numberOfChannel, i + flagsOfLongPrintF[numberOfChannel, 1]] = receiveMessage[i + 8];
                }
                flagsOfLongPrintF[numberOfChannel, 0] = numberOfPackage + 1;
                flagsOfLongPrintF[numberOfChannel, 1] += lengthOfPartOfMessage;
                flagsOfLongPrintF[numberOfChannel, 2] = lengthOfPartOfMessage;
            }
            else if (numberOfPackage < flagsOfLongPrintF[numberOfChannel, 0])
            {
                RecordToRTB(Color.Red, " ERROR пришел пакет составного PrintF с номером - ниже ожидаемого ({0}) - {1}", Convert.ToString(flagsOfLongPrintF[numberOfChannel, 0]), BitConverter.ToString(receiveMessage));
            }
            if (IsFinalPackage)
            {
                resultedMessage = new byte[flagsOfLongPrintF[numberOfChannel, 1]];
                for (int i = 0; i < flagsOfLongPrintF[numberOfChannel, 1]; i++)
                {
                    resultedMessage[i] = longPrintFMessages[numberOfChannel, i];
                    longPrintFMessages[numberOfChannel, i] = 0; ;
                }
                RecordToRTB(Color.DeepPink, "Составной PrintF по Кан {0} - {1}", numberOfChannel.ToString(), BitConverter.ToString(resultedMessage));
                flagsOfLongPrintF[numberOfChannel, 0] = flagsOfLongPrintF[numberOfChannel, 1] = 0;
            }
        }
        bool startedRepeat = false;
        public void CheckMessage0(byte[] receiveMessage)
        {
            if (receiveMessage.Length == 9 && receiveMessage[0] == 0x23)
            {
                if (!accepted || !mode)
                {
                    while (stopwatchAccept.IsRunning)
                    {
                        stopwatchAccept.Stop();
                        handMessageReadyToSend = true;
                        stopwatchHandTest.Stop();
                        if (receiveMessage[6] == 0x02)
                        {
                            commandIsCompleted = false;
                        }
                        else
                        {
                            commandIsCompleted = true;
                        }
                    }
                    if (Convert.ToInt32(stopwatchAccept.Elapsed.Milliseconds) > timeOut)
                    {
                        RecordToRTB(Color.Red, "[{0}] Приём: Команда 0  [{1}] - TIMEOUT ERROR {2}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(Convert.ToInt32(stopwatchAccept.Elapsed.Milliseconds)), BitConverter.ToString(receiveMessage));
                    }
                    else
                    {
                        RecordToRTB(Color.Black, "[{0}] Приём: Команда 0  [{1}] {2}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(Convert.ToInt32(stopwatchAccept.Elapsed.Milliseconds)), BitConverter.ToString(receiveMessage));
                        if (commandIsCompleted)
                        {
                            accepted = true;
                            if (acceptedState.CurrentCount <= 0)
                            {
                                acceptedState.Release();
                            }
                        }
                        else
                        {
                            if (!startedRepeat)
                            {
                                startedRepeat = true;
                                Task.Factory.StartNew(() => CheckAccomplismentCommand());
                            }
                            //if (!CheckAccomplismentCommand())
                            //{
                            //    Command0NoAnswer(_numberOFStopwatch);
                            //}
                        }
                    }
                    if (BitConverter.ToInt16(receiveMessage, 7) > 0)
                    {
                        CheckMessage0State(BitConverter.ToInt16(receiveMessage, 7));
                    }
                    else
                    {
                        rightMessage0 = true;
                    }
                }
                else
                {
                    RecordToRTB(Color.Red, "[{0}] ERROR пришла повторная Команда 0  до отправки сообщения {1}", Convert.ToString(stopwatchFullTest.Elapsed), BitConverter.ToString(receiveMessage));
                }
            }
        }
        void CheckAnswerAndKvitation(int numberOfChannel, bool stopNeeded = true, bool radiostationType = false)
        {
            while (accepted == false || incomeData[numberOfChannel, 0] != 0x0)
            {
                tokenTestStop.ThrowIfCancellationRequested();
                Thread.Sleep(1);
                if (stopwatchAccept.ElapsedMilliseconds > timeOut)
                {
                    Command0NoAnswer(_numberOFStopwatch, radiostationType);
                }
                if (stopwatchMas[numberOfChannel].ElapsedMilliseconds > (TimeOutMas[numberOfChannel]))
                {
                    CommandNokvitation(numberOfChannel, stopNeeded, radiostationType);
                }
            }

        }
        void CheckAccomplismentCommand()
        {

            Thread.Sleep(99);
            int countOfResendedMessages = 0;
            byte[] messageLength = new byte[4];
            for (int i = 0; i < messageLength.Length; i++)
            {
                messageLength[i] = incomeData[_numberOFStopwatch, i + 1];
            }
            int tempMesLength = BitConverter.ToInt32(messageLength, 0) + 5;
            byte[] tempMes = new byte[tempMesLength];
            for (int t = 0; t < tempMesLength; t++)
            {
                tempMes[t] = incomeData[_numberOFStopwatch, t];
            }
            while (!commandIsCompleted)
            {
                tokenTestStop.ThrowIfCancellationRequested();
                if (countOfResendedMessages >= 3)
                {
                    try
                    {
                        Command0NoAnswer(_numberOFStopwatch, ethernetMode);
                    }
                    catch
                    {
                        startedRepeat = false;
                        return;
                    }
                }
                //по Кан { 3} по РС

                RecordToRTB(Color.Red, "[{0}] Отправляю повторное сообщение " + (ethernetMode ? "по РС " : "по Кан ") + "{1}:  {2}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(_numberOFStopwatch + 1), BitConverter.ToString(tempMes)/*, Convert.ToString(_numberOFStopwatch + 1)*/);
                TCPClientSendData(tempMes);
                stopwatchAccept.Restart();
                stopwatchMas[_numberOFStopwatch].Restart();
                //accepted = false;
                countOfResendedMessages++;
                Thread.Sleep(100);

            }
            startedRepeat = false;
        }
        public void CheckMessage0State(int statebytes)
        {
            string[] errorbyte = new string[16]{" ОТКАЗ канала 1"," ОТКАЗ канала 2"," ОТКАЗ канала 3",
                " ОТКАЗ канала 4"," ОТКАЗ канала 5"," ОТКАЗ канала 6"," ОТКАЗ канала 7"," ОТКАЗ канала 8"," ОТКАЗ комплекса устройства управления",
                " ОТКАЗ контроллера Ethernet радиостанций"," ОТКАЗ контроллера ЗАС1"," ОТКАЗ контроллера ЗАС2"," ОТКАЗ ЗАС1"," ОТКАЗ ЗАС2","", ""};
            string errorstring = "ERROR";
            bool isAnyErrors = false;
            for (int i = 0; i < 16; i++)
            {
                if (Convert.ToInt16(statebytes & 1) == 1)
                {
                    isAnyErrors = true;
                    errorstring += errorbyte[i];
                }
                statebytes >>= 1;
            }
            if (isAnyErrors)
            {
                RecordToRTB(Color.Red, "[{0}]{1}", Convert.ToString(stopwatchFullTest.Elapsed), errorstring);
            }
        }
        public void CheckMessagePrintF(byte[] receiveMessage)
        {
            byte[] printf = new byte[receiveMessage.Length - 6];
            for (int i = 0; i < printf.Length; i++)
            { printf[i] = receiveMessage[i + 6]; }
            string decodedstring = ascii.GetString(printf);
            RecordToRTB(Color.DarkGreen, "{0} - {1}", decodedstring);
            if (decodedstring.Length >= 7 && decodedstring[5] == 'с' && decodedstring[6] == 'т')
            {
                RecordToRTB(Color.Red, "[{0}] ERROR Пришел ПринтФ со значением Стерт   ", Convert.ToString(stopwatchFullTest.Elapsed));
                if (!probabilityOfBringingTestMode)
                {
                    NullFunction Stop_Click = StopTestButton.PerformClick;
                    BeginInvoke(Stop_Click);
                }
            }
        }
        public void CheckAutoMessages1(byte[] receiveMessage)
        {
            if (!perfomanceTestMode)
            {
                CheckAutoMessages1Normal(receiveMessage);
            }
            else if (perfomanceTestMode)
            {
                CheckAutoMessages1PerfomanceTest(receiveMessage);
            }
        }
        public void CheckAutoMessages1Normal(byte[] receiveMessage)
        {
            if (receiveMessage[15] != 0)
            {
                CheckAutoMessages1NormalOnChannel(receiveMessage);
            }
            else
            {
                CheckAutoMessages1NormalOnFullAddress(receiveMessage);
            }
        }
        int numberOfDividingByte = 0;
        int ChannelOfDivideMessage = 0;
        public void CheckAutoMessages1NormalOnChannel(byte[] receiveMessage)
        {
            NullFunction Stop_Click = StopTestButton.PerformClick;
            int numberOfCommand = Convert.ToUInt16(Math.Log(receiveMessage[15], 2));
            bool bigMessageNotFullData = false;
            if (!_driverType)
            {
                if (incomeData[numberOfCommand, 1] + (incomeData[numberOfCommand, 2] + (incomeData[numberOfCommand, 3] +
                        incomeData[numberOfCommand, 4] * 256) * 256) * 256 > BitConverter.ToInt32(receiveMessage, 1))
                {
                    bigMessageNotFullData = true;
                }
                else if (incomeData[numberOfCommand, 1] + (incomeData[numberOfCommand, 2] + (incomeData[numberOfCommand, 3] +
                        incomeData[numberOfCommand, 4] * 256) * 256) * 256 < BitConverter.ToInt32(receiveMessage, 1))
                {
                    RecordToRTB(Color.Red, "[{0}] ERROR команда 1 с временем [{1}] по Кан {2}, но длины не совпадают:  {3} ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(stopwatchMas[numberOfCommand].Elapsed.Milliseconds), Convert.ToString(numberOfCommand + 1), BitConverter.ToString(receiveMessage));
                    if (!probabilityOfBringingTestMode)
                    {
                        BeginInvoke(Stop_Click);
                    }
                    return;
                }
                for (int j = 16; j < receiveMessage.Length; j++)
                {
                    if (receiveMessage[j] != incomeData[numberOfCommand, j])
                    {
                        RecordToRTB(Color.Red, "[{0}] ERROR команда 1 с временем [{1}] по Кан {2}, но данные не совпадают:  {3} ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(stopwatchMas[numberOfCommand].Elapsed.Milliseconds), Convert.ToString(numberOfCommand + 1), BitConverter.ToString(receiveMessage));
                        if (!probabilityOfBringingTestMode)
                        {
                            BeginInvoke(Stop_Click);
                        }
                        return;

                    }
                }
                if (!bigMessageNotFullData)
                {
                    RecordToRTB(Color.Black, "[{0}] Приём по Кан {1}: команда 1 с временем [{2}]    {3}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(numberOfCommand + 1), Convert.ToString(stopwatchMas[numberOfCommand].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage));
                    answerRecieved[numberOfCommand] = true;
                    MessageFullyAnswered(numberOfCommand);
                }
                else
                {
                    RecordToRTB(Color.Black, "[{0}] Приём по Кан {1}: НЕПОЛНАЯ команда 1 с временем [{2}]   {3}", Convert.ToString(stopwatchFullTest.Elapsed),
                        Convert.ToString(numberOfCommand + 1), Convert.ToString(stopwatchMas[numberOfCommand].Elapsed.Milliseconds),
                        BitConverter.ToString(receiveMessage));
                    net.dividedMessageMode = true;
                    numberOfDividingByte = receiveMessage.Length;
                    ChannelOfDivideMessage = numberOfCommand;
                    stopwatchMas[numberOfCommand].Restart();
                }
            }
            else
            {
                for (int i = 6; i < 10; i++)
                {
                    receiveMessage[i] = 0x0;
                }
                Channels[numberOfCommand].Message1recieved(receiveMessage);
            }
        }
        public void CheckInCompleteLongMessage(byte[] receiveMessage)
        {
            NullFunction Stop_Click = StopTestButton.PerformClick;
            if (numberOfDividingByte + receiveMessage.Length == (5 + incomeData[ChannelOfDivideMessage, 1] + (incomeData[ChannelOfDivideMessage, 2] +
                (incomeData[ChannelOfDivideMessage, 3] + incomeData[ChannelOfDivideMessage, 4] * 256) * 256) * 256))
            {
                for (int j = numberOfDividingByte; j < receiveMessage.Length + numberOfDividingByte; j++)
                {
                    if (receiveMessage[j] != incomeData[ChannelOfDivideMessage, j])
                    {
                        RecordToRTB(Color.Red, "[{0}] ERROR команда 1 с временем [{1}] по Кан {2}, но данные не совпадают:  {3} ",
                            Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(stopwatchMas[ChannelOfDivideMessage].Elapsed.Milliseconds),
                            Convert.ToString(ChannelOfDivideMessage + 1), BitConverter.ToString(receiveMessage));

                        BeginInvoke(Stop_Click);
                        return;
                    }
                }
                RecordToRTB(Color.Black, "[{0}] Приём по Кан {1}: команда 1 с временем [{2}]    {3}", Convert.ToString(stopwatchFullTest.Elapsed),
                    Convert.ToString(ChannelOfDivideMessage + 1), Convert.ToString(stopwatchMas[ChannelOfDivideMessage].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage));
                net.dividedMessageMode = false;
                answerRecieved[ChannelOfDivideMessage] = true;

            }
            else if (numberOfDividingByte + receiveMessage.Length > (5 + incomeData[ChannelOfDivideMessage, 1] + (incomeData[ChannelOfDivideMessage, 2] +
                (incomeData[ChannelOfDivideMessage, 3] + incomeData[ChannelOfDivideMessage, 4] * 256) * 256) * 256))
            {
                RecordToRTB(Color.Red, "[{0}] ERROR команда 1 с временем [{1}] по Кан {2}, но суммарная длина превышает не совпадают:  {3} ", Convert.ToString(stopwatchFullTest.Elapsed),
                    Convert.ToString(stopwatchMas[ChannelOfDivideMessage].Elapsed.Milliseconds), Convert.ToString(ChannelOfDivideMessage + 1), BitConverter.ToString(receiveMessage));

                BeginInvoke(Stop_Click);
                return;
            }
            else
            {
                for (int j = numberOfDividingByte; j < receiveMessage.Length + numberOfDividingByte; j++)
                {
                    if (receiveMessage[j] != incomeData[ChannelOfDivideMessage, j])
                    {
                        RecordToRTB(Color.Red, "[{0}] ERROR НЕПОЛНАЯ команда 1 с временем [{1}] по Кан {2}, но данные не совпадают:  {3} ",
                            Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(stopwatchMas[ChannelOfDivideMessage].Elapsed.Milliseconds), Convert.ToString(ChannelOfDivideMessage + 1), BitConverter.ToString(receiveMessage));

                        BeginInvoke(Stop_Click);
                        return;
                    }
                }
                RecordToRTB(Color.Black, "[{0}] Приём по Кан {1}: НЕПОЛНАЯ команда 1 с временем [{2}]   {3}",
                    Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(ChannelOfDivideMessage + 1), Convert.ToString(stopwatchMas[ChannelOfDivideMessage].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage));
                numberOfDividingByte = receiveMessage.Length + numberOfDividingByte;
                stopwatchMas[ChannelOfDivideMessage].Restart();
            }
        }
        private void CheckAllChannelsAnswer(bool radistationtype = false)
        {
            if (!radistationtype)
            {
                while (incomeData[0, 0] != 0x0 || incomeData[1, 0] != 0x0 || incomeData[2, 0] != 0x0 || incomeData[3, 0] != 0x0 || incomeData[4, 0] != 0x0 || incomeData[5, 0] != 0x0
                            || incomeData[6, 0] != 0x0 || incomeData[7, 0] != 0x0)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        while (accepted == false || incomeData[i, 0] != 0x0)
                        {
                            tokenTestStop.ThrowIfCancellationRequested();
                            Thread.Sleep(1);
                            if (stopwatchAccept.ElapsedMilliseconds > 4000)
                            {
                                Command0NoAnswer(_numberOFStopwatch, false);
                            }
                            if (stopwatchMas[i].ElapsedMilliseconds > TimeOutMas[i])
                            {
                                CommandNokvitation(i, true, false);
                            }
                        }
                    }
                    Thread.Sleep(10);
                }
            }
            else
            {
                while (incomeData[0, 0] != 0x0 || incomeData[1, 0] != 0x0 || incomeData[2, 0] != 0x0 || incomeData[3, 0] != 0x0 || incomeData[4, 0] != 0x0 || incomeData[5, 0] != 0x0
                            || incomeData[6, 0] != 0x0 || incomeData[7, 0] != 0x0 || incomeData[8, 0] != 0x0 || incomeData[9, 0] != 0x0 || incomeData[10, 0] != 0x0 || incomeData[11, 0] != 0x0 || incomeData[12, 0] != 0x0 || incomeData[13, 0] != 0x0
                            || incomeData[14, 0] != 0x0 || incomeData[15, 0] != 0x0 || incomeData[16, 0] != 0x0 || incomeData[17, 0] != 0x0 || incomeData[18, 0] != 0x0 || incomeData[19, 0] != 0x0 || incomeData[20, 0] != 0x0 || incomeData[21, 0] != 0x0
                            || incomeData[22, 0] != 0x0 || incomeData[23, 0] != 0x0 || incomeData[24, 0] != 0x0 || incomeData[25, 0] != 0x0 || incomeData[26, 0] != 0x0 || incomeData[27, 0] != 0x0 || incomeData[28, 0] != 0x0 || incomeData[29, 0] != 0x0
                            || incomeData[30, 0] != 0x0 || incomeData[31, 0] != 0x0 || !accepted)
                {
                    for (int i = 0; i < 32; i++)
                    {
                        while (accepted == false || incomeData[i, 0] != 0x0)
                        {
                            tokenTestStop.ThrowIfCancellationRequested();
                            Thread.Sleep(1);
                            if (stopwatchAccept.ElapsedMilliseconds > 4000)
                            {
                                Command0NoAnswer(_numberOFStopwatch, true);
                            }
                            if (stopwatchMas[i].ElapsedMilliseconds > 7000)
                            {
                                CommandNokvitation(i, true, true);
                            }
                        }
                    }
                    Thread.Sleep(10);
                }
            }
        }
        public void CheckAutoMessages1NormalOnFullAddress(byte[] receiveMessage)
        {
            NullFunction Stop_Click = StopTestButton.PerformClick;
            bool bigMessageNotFullData = false;
            int numberOfCommand = 0;
            for (int i = 0; i < 32; i++)
            {
                if (incomeData[i, 0] != 0x00)
                {
                    if (receiveMessage[15] == 0 && receiveMessage[16] == 1 && receiveMessage[17] == incomeData[i, 17] && receiveMessage[18] == incomeData[i, 18] && receiveMessage[19] == incomeData[i, 19]
                         && receiveMessage[20] == incomeData[i, 20] && receiveMessage[21] == incomeData[i, 21] && receiveMessage[22] == incomeData[i, 22] && receiveMessage[23] == incomeData[i, 23]
                         && receiveMessage[24] == incomeData[i, 24] && receiveMessage[25] == incomeData[i, 25] && receiveMessage[26] == incomeData[i, 26] && receiveMessage[27] == incomeData[i, 27]
                         && receiveMessage[28] == incomeData[i, 28])
                    {
                        numberOfCommand = i;
                        if (incomeData[numberOfCommand, 1] + (incomeData[numberOfCommand, 2] + (incomeData[numberOfCommand, 3] +
                       incomeData[numberOfCommand, 4] * 256) * 256) * 256 > BitConverter.ToInt32(receiveMessage, 1))
                        {
                            bigMessageNotFullData = true;
                        }
                        else if (incomeData[numberOfCommand, 1] + (incomeData[numberOfCommand, 2] + (incomeData[numberOfCommand, 3] +
                                incomeData[numberOfCommand, 4] * 256) * 256) * 256 < BitConverter.ToInt32(receiveMessage, 1))
                        {
                            RecordToRTB(Color.Red, "[{0}] ERROR команда 1 с временем [{1}] по РС {2}, но длины не совпадают:  {3} ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(stopwatchMas[numberOfCommand].Elapsed.Milliseconds), Convert.ToString(numberOfCommand + 1), BitConverter.ToString(receiveMessage));

                            BeginInvoke(Stop_Click);
                            return;
                        }
                        for (int j = 29; j < receiveMessage.Length; j++)
                        {
                            if (receiveMessage[j] != incomeData[numberOfCommand, j])
                            {
                                RecordToRTB(Color.Red, "[{0}] ERROR команда 1 с временем [{1}] по РС {2}, но данные не совпадают  :{3} ", Convert.ToString(stopwatchFullTest.Elapsed),
                                    Convert.ToString(stopwatchMas[numberOfCommand].Elapsed.Milliseconds), Convert.ToString(i + 1), BitConverter.ToString(receiveMessage));
                                byte[] data = new byte[receiveMessage.Length - 29];
                                for (int k = 29; k < receiveMessage.Length; k++)
                                {
                                    data[k - 29] = incomeData[numberOfCommand, k];
                                }
                                RecordToTheMessageBox(data);

                                BeginInvoke(Stop_Click);
                                return;
                            }
                        }
                        if (!bigMessageNotFullData)
                        {
                            RecordToRTB(Color.Black, "[{0}] Приём по РС {1}: команда 1 с временем [{2}]    {3}", Convert.ToString(stopwatchFullTest.Elapsed),
                                Convert.ToString(numberOfCommand + 1), Convert.ToString(stopwatchMas[numberOfCommand].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage));
                            answerRecieved[numberOfCommand] = true;
                            MessageFullyAnswered(numberOfCommand);
                        }
                        else
                        {
                            RecordToRTB(Color.Black, "[{0}] Приём по РС {1}: НЕПОЛНАЯ команда 1 с временем [{2}]   {3}", Convert.ToString(stopwatchFullTest.Elapsed),
                                Convert.ToString(numberOfCommand + 1), Convert.ToString(stopwatchMas[numberOfCommand].Elapsed.Milliseconds),
                                BitConverter.ToString(receiveMessage));
                            net.dividedMessageMode = true;
                            numberOfDividingByte = receiveMessage.Length;
                            ChannelOfDivideMessage = numberOfCommand;
                            stopwatchMas[numberOfCommand].Restart();
                        }
                        return;
                    }
                }
            }
            for (int i = 0; i < 32; i++)
            {
                if (receiveMessage[15] == 0 && receiveMessage[16] == 1 && receiveMessage[17] == incomeData[i, 17] && receiveMessage[18] == incomeData[i, 18] && receiveMessage[19] == incomeData[i, 19]
                          && receiveMessage[20] == incomeData[i, 20] && receiveMessage[21] == incomeData[i, 21] && receiveMessage[22] == incomeData[i, 22] && receiveMessage[23] == incomeData[i, 23]
                          && receiveMessage[24] == incomeData[i, 24] && receiveMessage[25] == incomeData[i, 25] && receiveMessage[26] == incomeData[i, 26] && receiveMessage[27] == incomeData[i, 27]
                          && receiveMessage[28] == incomeData[i, 28])
                {
                    RecordToRTB(Color.Red, "[{0}] ERROR команда 1, адрес не известен, возможно РС {1}:   {2} ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(i + 1), BitConverter.ToString(receiveMessage));
                    throw new MessageException();
                }
            }
            RecordToRTB(Color.Red, "[{0}] ERROR команда 1, но адрес не известен:{1} ", Convert.ToString(stopwatchFullTest.Elapsed), BitConverter.ToString(receiveMessage));
            throw new MessageException();
        }
        public void CheckAutoMessages1PerfomanceTest(byte[] receiveMessage)
        {
            for (int i = 0; i < EthernetAdresses.Length / 24; i++)
            {
                if (receiveMessage[15] == 0 && receiveMessage[16] == 1 && receiveMessage[17] == EthernetAdresses[i * 24] && receiveMessage[18] == EthernetAdresses[i * 24 + 1] && receiveMessage[19] == EthernetAdresses[i * 24 + 2]
                                 && receiveMessage[20] == EthernetAdresses[i * 24 + 3] && receiveMessage[21] == EthernetAdresses[i * 24 + 4] && receiveMessage[22] == EthernetAdresses[i * 24 + 5] && receiveMessage[23] == EthernetAdresses[i * 24 + 6]
                                 && receiveMessage[24] == EthernetAdresses[i * 24 + 7] && receiveMessage[25] == EthernetAdresses[i * 24 + 8] && receiveMessage[26] == EthernetAdresses[i * 24 + 9] && receiveMessage[27] == EthernetAdresses[i * 24 + 10]
                                 && receiveMessage[28] == EthernetAdresses[i * 24 + 11])
                {
                    RecordToRTB(Color.Black, "[{0}] Приём команда 1    {1}", Convert.ToString(stopwatchFullTest.Elapsed), BitConverter.ToString(receiveMessage));
                    BeginInvoke(new LabelChangingpTC(CounterPTCChanging), i, Convert.ToString(Convert.ToInt32(i + 1)));
                    //BeginInvoke(labelChangingpTC, correspondenceTable[i], Convert.ToInt32(Invoke(DelegateTextReturn, Counters[corres/pondenceTable[i]])) + 1);
                    break;
                }
            }
        }
        public void CheckAutoMessages10(byte[] receiveMessage)
        {
            int numberOfCommand = Convert.ToUInt16(Math.Log(receiveMessage[6], 2));
            if (incomeData[numberOfCommand, 5] == receiveMessage[5])
            {
                stopwatchMas[numberOfCommand].Stop();
                incomeData[numberOfCommand, 0] = 0x0;
                RecordToRTB(Color.Black, "[{0}] Приём по Кан {1}: команда 10 с временем [{2}]    {3}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(numberOfCommand + 1), Convert.ToString(stopwatchMas[numberOfCommand].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage));
            }
        }
        public void CheckAutoMessages12(byte[] receiveMessage)
        {
            bool truth = true;
            stopwatchMas[0].Stop();
            for (int i = 0; i < receiveMessage.Length; i++)
            {
                if (income12[i] != receiveMessage[i])
                {
                    truth = false;
                    break;
                }
            }
            if (truth)
            {
                RecordToRTB(Color.Black, "[{0}] Приём: команда 12 с временем [{1}]  {2}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(stopwatchMas[0].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage));
                income12[0] = 0x0;
            }
            else
            {
                RecordToRTB(Color.Red, "[{0}] ERROR команда 12 с временем [{1}], но данные не совпадают  :{2} ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(stopwatchMas[0].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage));
                income12[0] = 0x0;
            }
        }
        public void CheckAutoMessages16(byte[] receiveMessage)
        {
            bool RsFound = false;

            for (int i = 0; i < 32; i++)
            {
                if (incomeData[i, 0] != 0x00)
                {
                    if (
                        receiveMessage[6] == incomeData[i, 6] && receiveMessage[7] == incomeData[i, 7] && receiveMessage[8] == incomeData[i, 8]
                         && receiveMessage[9] == incomeData[i, 9] && receiveMessage[10] == incomeData[i, 10] && receiveMessage[11] == incomeData[i, 11] && receiveMessage[12] == incomeData[i, 12]
                         && receiveMessage[13] == incomeData[i, 13] && receiveMessage[14] == incomeData[i, 14] && receiveMessage[15] == incomeData[i, 15] && receiveMessage[16] == incomeData[i, 16] &&
                        receiveMessage[17] == incomeData[i, 17] && receiveMessage[18] == incomeData[i, 18] && receiveMessage[19] == incomeData[i, 19]
                         && receiveMessage[20] == incomeData[i, 20] && receiveMessage[21] == incomeData[i, 21] && receiveMessage[22] == incomeData[i, 22] && receiveMessage[23] == incomeData[i, 23]
                         && receiveMessage[24] == incomeData[i, 24] && receiveMessage[25] == incomeData[i, 25] && receiveMessage[26] == incomeData[i, 26] && receiveMessage[27] == incomeData[i, 27]
                         && receiveMessage[28] == incomeData[i, 28] && receiveMessage[29] == incomeData[i, 29])
                    {
                        RsFound = true;
                        if (incomeData[i, 30] == receiveMessage[30])
                        {
                            if (incomeData[i, 31] == receiveMessage[31])
                            {
                                if (incomeData[i, 32] == receiveMessage[32])
                                {
                                    if (incomeData[i, 33] == receiveMessage[33])
                                    {
                                        if (incomeData[i, 34] == receiveMessage[34])
                                        {
                                            if ((Convert.ToInt16(incomeData[i, 35]) + Convert.ToInt16(incomeData[i, 36]) * 256) == BitConverter.ToInt16(receiveMessage, 35))
                                            {
                                                if (incomeData[i, 37] == receiveMessage[37])
                                                {
                                                    if (incomeData[i, 38] == receiveMessage[38])
                                                    {
                                                        if (incomeData[i, 39] == receiveMessage[39])
                                                        {
                                                            if (incomeData[i, 40] == receiveMessage[40])
                                                            {
                                                                stopwatchMas[i].Stop();
                                                                incomeData[i, 0] = 0x0;
                                                                RecordToRTB(Color.Black, "[{0}] Приём по РС {1}: команда 16 с временем [{2}]     {3}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(i + 1), Convert.ToString(stopwatchMas[i].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage));
                                                            }
                                                            else RecordToRTB(Color.Red, "[{0}] ERROR! Приём по РС {1}: Команда 16 с временем [{2}]    {3} , НО СК = {4} ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(i + 1), Convert.ToString(stopwatchMas[i].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage), Convert.ToString(receiveMessage[40]));
                                                        }
                                                        else RecordToRTB(Color.Red, "[{0}] ERROR! Приём по РС {1}: Команда 16 с временем [{2}]    {3} , НО Тив = {4} ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(i + 1), Convert.ToString(stopwatchMas[i].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage), Convert.ToString(receiveMessage[39]));
                                                    }
                                                    else RecordToRTB(Color.Red, "[{0}] ERROR! Приём по РС {1}: Команда 16 с временем [{2}]    {3} , НО КВТ = {4} ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(i + 1), Convert.ToString(stopwatchMas[i].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage), Convert.ToString(receiveMessage[38]));
                                                }
                                                else RecordToRTB(Color.Red, "[{0}] ERROR! Приём по РС {1}: Команда 16 с временем [{2}]    {3} , НО КВ = {4} ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(i + 1), Convert.ToString(stopwatchMas[i].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage), Convert.ToString(receiveMessage[37]));
                                            }
                                            else RecordToRTB(Color.Red, "[{0}] ERROR! Приём по РС {1}: Команда 16 с временем [{2}]    {3} , НО РД = {4} ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(i + 1), Convert.ToString(stopwatchMas[i].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage), Convert.ToString(BitConverter.ToInt16(receiveMessage, 35)));
                                        }
                                        else RecordToRTB(Color.Red, "[{0}] ERROR! Приём по РС {1}: Команда 16 с временем [{2}]    {3} , НО КПП = {4} ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(i + 1), Convert.ToString(stopwatchMas[i].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage), Convert.ToString(receiveMessage[34]));
                                    }
                                    else RecordToRTB(Color.Red, "[{0}] ERROR! Приём по РС {1} Команда 16 с временем [{2}]    {3} , НО КП = {4} ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(i + 1), Convert.ToString(stopwatchMas[i].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage), Convert.ToString(receiveMessage[33]));
                                }
                                else RecordToRTB(Color.Red, "[{0}] ERROR! Приём по РС {1}: Команда 16 с временем [{2}]    {3} , НО НЗ = {4} ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(i + 1), Convert.ToString(stopwatchMas[i].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage), Convert.ToString(receiveMessage[32]));
                            }
                            else RecordToRTB(Color.Red, "[{0}] ERROR! Приём по РС {1}: Команда 16 с временем [{2}]    {3} , НО ШЛ = {4} ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(i + 1), Convert.ToString(stopwatchMas[i].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage), Convert.ToString(receiveMessage[31]));

                        }
                        else RecordToRTB(Color.Red, "[{0}] ERROR! Приём по РС {1}: Команда 16 с временем [{2}]    {3} , НО КАН = {4} ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(i + 1), Convert.ToString(stopwatchMas[i].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage), Convert.ToString(receiveMessage[30]));
                    }
                    //else RecordToRTB(Color.Red, "[{0}] ERROR! Приём по Кан {1}: Команда 16 с временем [{2}]    {3} , НО АДР = {4} ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(i + 1), Convert.ToString(stopwatchMas[i].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage), BitConverter.ToString(mas2));
                }
            }
            if (!RsFound)
            {
                RecordToRTB(Color.Black, "[{0}] Пришло что-то пока неопознанное {1} ", Convert.ToString(stopwatchFullTest.Elapsed), BitConverter.ToString(receiveMessage));
            }
        }
        public void CheckAutoMessages3(byte[] receiveMessage)
        {
            if (receiveMessage.Length == 16)
            {
                CheckAutoMessages3NormalOnChannel(receiveMessage);
            }
            else if (receiveMessage.Length == 29)
            {
                CheckAutoMessages3NormalOnFullAddress(receiveMessage);
            }
        }
        public void CheckAutoMessages3NormalOnChannel(byte[] receiveMessage)
        {
            int numberOfCommand = Convert.ToUInt16(Math.Log(receiveMessage[15], 2));
            if (!_driverType)
            {
                if (receiveMessage[14] == 0x00)
                {
                    if (!IsInsRepeat(BitConverter.ToInt32(receiveMessage, 6)))
                    {
                        if (receiveMessage[10] == 0x01 && receiveMessage[10] == receiveMessage[12])
                        {
                            if (receiveMessage[14] == 0x00 || receiveMessage[14] == 0x09 || receiveMessage[14] == 0x0A || receiveMessage[14] == 0x0B || receiveMessage[14] == 0x0C)
                            {
                                if (!settingTest)
                                {
                                    //kvitRecieved[numberOfCommand] = true;

                                    //messageFullyAnswered(numberOfCommand);

                                }
                                else if (settingTest)
                                {
                                    stopwatchMas[numberOfCommand].Stop();
                                    incomeData[numberOfCommand, 0] = 0x0;
                                }
                                RecordToRTB(Color.Black, "[{0}] Приём по Кан {1}: Команда 3 с временем [{2}]    {3}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(numberOfCommand + 1), Convert.ToString(stopwatchMas[numberOfCommand].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage));
                            }
                            else
                            {
                                RecordToRTB(Color.Red, "[{0}] ERROR! Приём по Кан {1}: Команда 3 с временем [{2}]    {3} , НО КСП = {4} ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(numberOfCommand + 1), Convert.ToString(stopwatchMas[numberOfCommand].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage), Convert.ToString(receiveMessage[14]));
                            }
                        }
                        else
                        {
                            RecordToRTB(Color.Red, "[{0}] ERROR! Приём по Кан {1}: Команда 3 с временем [{2}]    {3} , НО НС = {4}, КС = {5} ",
                                Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(numberOfCommand + 1),
                                Convert.ToString(stopwatchMas[numberOfCommand].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage),
                                Convert.ToString(BitConverter.ToInt16(receiveMessage, 10)), Convert.ToString(BitConverter.ToInt16(receiveMessage, 12)));
                        }
                    }

                }
                else
                {
                    if (receiveMessage[14] == 0x09 || receiveMessage[14] == 0x0A || receiveMessage[14] == 0x0B || receiveMessage[14] == 0x0C)
                    {
                        RecordToRTB(Color.Black, "[{0}] Приём по Кан {1}: Команда 3 с временем [{2}]    {3}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(numberOfCommand + 1), Convert.ToString(stopwatchMas[numberOfCommand].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage));

                    }
                    else
                    {
                        RecordToRTB(Color.Red, "[{0}] ERROR! Приём по Кан {1}: Команда 3 с временем [{2}]    {3} , НО ИНС = {4}, ",
                                Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(numberOfCommand + 1),
                                Convert.ToString(stopwatchMas[numberOfCommand].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage),
                                Convert.ToString(BitConverter.ToInt32(receiveMessage, 6)));
                    }
                }
            }
            else
            {
                Channels[numberOfCommand].Message3recieved();
            }
        }
        public void CheckAutoMessages3NormalOnFullAddress(byte[] receiveMessage)
        {
            int numberOfCommand = 0;
            bool Command1found = false;
            for (int i = 0; i < 32; i++)
            {
                //if (incomeData[i, 0] != 0x00)
                //{
                if (receiveMessage[15] == 0 && receiveMessage[16] == 1 && receiveMessage[17] == incomeData[i, 17] && receiveMessage[18] == incomeData[i, 18] && receiveMessage[19] == incomeData[i, 19]
                     && receiveMessage[20] == incomeData[i, 20] && receiveMessage[21] == incomeData[i, 21] && receiveMessage[22] == incomeData[i, 22] && receiveMessage[23] == incomeData[i, 23]
                     && receiveMessage[24] == incomeData[i, 24] && receiveMessage[25] == incomeData[i, 25] && receiveMessage[26] == incomeData[i, 26] && receiveMessage[27] == incomeData[i, 27]
                     && receiveMessage[28] == incomeData[i, 28])
                {
                    numberOfCommand = i;
                    Command1found = true;
                    break;
                }
                //}
            }
            if (Command1found)
            {
                if (receiveMessage[14] != 0x00 || !IsInsRepeat(BitConverter.ToInt32(receiveMessage, 6)))
                {
                    if (receiveMessage[10] == 0x01 && receiveMessage[10] == receiveMessage[12])
                    {
                        if (receiveMessage[14] == 0x00 || receiveMessage[14] == 0x09 || receiveMessage[14] == 0x0A)
                        {
                            if (!settingTest)
                            {
                                //kvitRecieved[numberOfCommand] = true;

                                //messageFullyAnswered(numberOfCommand);
                            }
                            else if (settingTest)
                            {
                                stopwatchMas[numberOfCommand].Stop();
                                incomeData[numberOfCommand, 0] = 0x0;
                            }
                            RecordToRTB(Color.Black, "[{0}] Приём по РС {1}: Команда 3 с временем [{2}]    {3}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(numberOfCommand + 1), Convert.ToString(stopwatchMas[numberOfCommand].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage));
                        }
                        else
                        {
                            RecordToRTB(Color.Red, "[{0}] ERROR! Приём по РС {1}: Команда 3 с временем [{2}]    {3} , НО КСП = {4} ", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(numberOfCommand + 1), Convert.ToString(stopwatchMas[numberOfCommand].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage), Convert.ToString(receiveMessage[14]));
                        }
                    }
                    else
                    {
                        RecordToRTB(Color.Red, "[{0}] ERROR! Приём по РС {1}: Команда 3 с временем [{2}]    {3} , НО НС = {4}, КС = {5} ",
                            Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(numberOfCommand + 1),
                            Convert.ToString(stopwatchMas[numberOfCommand].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage),
                            Convert.ToString(BitConverter.ToInt16(receiveMessage, 10)), Convert.ToString(BitConverter.ToInt16(receiveMessage, 12)));
                    }
                }
                else
                {
                    if (receiveMessage[14] == 0x09 || receiveMessage[14] == 0x00 || receiveMessage[14] == 0x0A || receiveMessage[14] == 0x0C)
                    {
                        RecordToRTB(Color.Black, "[{0}] Приём по РС {1}: Команда 3 с временем [{2}]    {3}", Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(numberOfCommand + 1), Convert.ToString(stopwatchMas[numberOfCommand].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage));
                    }
                    else
                    {
                        RecordToRTB(Color.Red, "[{0}] ERROR! Приём по РС {1}: Команда 3 с временем [{2}]    {3} , НО ИНС = {4}, ",
                                    Convert.ToString(stopwatchFullTest.Elapsed), Convert.ToString(numberOfCommand + 1),
                                    Convert.ToString(stopwatchMas[numberOfCommand].Elapsed.Milliseconds), BitConverter.ToString(receiveMessage),
                                    Convert.ToString(BitConverter.ToInt32(receiveMessage, 6)));
                    }
                }
            }
            else
            {
                RecordToRTB(Color.Red, "[{0}] ERROR! Приём: Команда 3:  {1}", Convert.ToString(stopwatchFullTest.Elapsed), BitConverter.ToString(receiveMessage));
                //recordToRTB(Color.Red, "[{0}] Приём: ERROR! неизвестная 3ья Команда 3 (возможно квитанция пришла после возвращения сообщения):  {1}", Convert.ToString(stopwatchFullTest.Elapsed), BitConverter.ToString(receiveMessage));
                //throw new MessageException();
            }
        }
        #endregion

        private int GetMessageLength(byte[,] mas, int numberOfChannel)
        {
            byte[] lengthBuffer = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                lengthBuffer[i] = mas[numberOfChannel, 1 + i];
            }
            return BitConverter.ToInt32(lengthBuffer, 0);
        }
        public void RecordToTheMessageBox(byte[] mas)
        {
            string str = BitConverter.ToString(mas);
            str = str.Replace('-', ' ');
            int amount = new Regex(" 00").Matches(str).Count;
            if (amount >= mas.Length - 17)
            {
                str = str.Substring(0, 17 * 2 + 16);
                try
                {
                    perevalLengthOfmessage.SelectedItem = 0;
                }
                catch { }
            }
            else if (amount >= mas.Length - 48)
            {
                str = str.Substring(0, 48 * 2 + 47);
                try
                {
                    perevalLengthOfmessage.SelectedItem = 1;
                }
                catch { }
            }
            else if (amount >= mas.Length - 80)
            {
                str = str.Substring(0, 80 * 2 + 79);
                try
                {
                    perevalLengthOfmessage.SelectedItem = 2;
                }
                catch { }
            }
            else if (amount >= mas.Length - 112)
            {
                str = str.Substring(0, 112 * 2 + 111);
                try
                {
                    perevalLengthOfmessage.SelectedItem = 3;
                }
                catch { }
            }
            BeginInvoke(new ToTextBox(Totextbox1), Message1, str);
        }
        public bool IsInsRepeat(int INS)
        {
            if (RepeatedIns.Count > 0)//Если в списке номеров сообщений есть значения
            {
                foreach (int repIns in RepeatedIns)//То новое значение сравнивается со всем списком
                {
                    if (INS.Equals(repIns))//Если оно совпадает с каким либо - значит повтор
                    {
                        return true;
                    }
                }
                RepeatedIns.Insert(0, INS);//Если повторов нет - то значение добавляется в список
                if (RepeatedIns.Count > 10)//и список обрезается при предельном количестве значений
                {
                    RepeatedIns.RemoveAt(10);
                }
                return false;
            }
            else
            {
                RepeatedIns.Add(INS);//Если список пустой - просто добавляется
                return false;
            }
        }
        private void MessageFullyAnswered(int i)//на данный момент проверка полной обработки сообщения(данных) это 3 ответа:0-команда (собстно для всех), 3 и 1 
        {
            if (answerRecieved[i])//&& kvitRecieved[i])
            {
                stopwatchMas[i].Stop();
                incomeData[i, 0] = 0x0;
                if (probabilityOfBringingTestMode)
                {
                    Invoke(labelFunction, probabilityOfBringingCountOfRecievedMessagesLabel, (Convert.ToInt32(probabilityOfBringingCountOfRecievedMessagesLabel.Text) + 1).ToString());
                }
            }
        }
        public void RecordToFile()//запись данных в файл
        {
            StreamWriter ws;
            string[] lines = PrimaRTB.Text.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            ws = new StreamWriter(writePath, true);
            foreach (string line in lines)
            {
                ws.WriteLine(line);
            }
            ws.Close();
        }
        private void ConnectControlToPrima()//контроль соединения , пока не используется
        {
            _primaConnectCheck = true;
            _connectControlToPrimaStarted = true;
            stopwatchConnectionCheck.Restart();
            while (_connectControlToPrimaStarted)
            {
                if (stopwatchConnectionCheck.ElapsedMilliseconds >= 5000)
                {
                    RecordToRTB(Color.Black, "ERROR Нет квитанции от примы");
                    controlConnectStop.Cancel();
                }
                try
                {
                    if (_primaConnectCheck)
                    {
                        stopwatchConnectionCheck.Restart();
                        byte[] CCMessage = new byte[6] { 0x23, 0x01, 0x00, 0x00, 0x00, 0xff };
                        _primaConnectCheck = false;
                        Thread.Sleep(1000);
                    }
                    controlConnectStopToken.ThrowIfCancellationRequested();
                }
                catch
                {
                    RecordToRTB(Color.Black, "Контроль соединения прерван");
                    break;
                }
                Thread.Sleep(1);
            }
            stopwatchConnectionCheck.Reset();
        }
        #region formpart
        private void PingLabelChanging(Label c, string state, Color color)
        {
            c.Text = state;
            c.ForeColor = color;
        }
        public void RecordToRTB(Color color, string control, string fstArg = "", string scdArg = "", string thrAdr = "", string fthArg = "",
            string FithArg = "", string sxArg = "", string sthArg = "", string eithArg = "", string nthArg = "", bool newline = true)//Несколько кривенькое, но сборное сообщение для RTB
        {
            string formated = String.Format(control, fstArg, scdArg, thrAdr, fthArg, FithArg, sxArg, sthArg, eithArg, nthArg);

            if (PrimaRTB.IsHandleCreated)
            {
                new MessageForRTB(formated, color, newline);
            }
            else
            {
                new MessageForRTB(formated, color, newline);
            }
        }

        private void Totextbox1(TextBox tb, string message)
        {
            try
            {
                tb.Text = message;
            }
            catch //(//Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        public void ChangeLabelText(Label label, string text)
        {
            label.Text = text;
        }
        static List<MessageForRTB> RTBRedrawList = new List<MessageForRTB>();
        private void PrintToRTB(MessageForRTB messageForRTB)
        {
            PrimaRTB.SelectionStart = PrimaRTB.TextLength;
            PrimaRTB.SelectionLength = 0;
            PrimaRTB.SelectionColor = messageForRTB.color;
            PrimaRTB.AppendText(messageForRTB.message);
            PrimaRTB.SelectionColor = PrimaRTB.ForeColor;
            if (messageForRTB.newline)
            {
                PrimaRTB.AppendText(Environment.NewLine);
            }
        }
        private void LogShower()
        {
            MessageForRTB messageForRTB;
            Stopwatch logStopwatch = new Stopwatch();
            while (true)
            {
                if (MessagesForRTBQueue.TryDequeue(out messageForRTB))
                {
                    if (RTBRedrawList.Count > 2000)
                    {
                        RTBRedrawList.RemoveRange(0, 1000);
                        BeginInvoke(new NullFunction(PrimaRTB.Clear));
                    }
                    RTBRedrawList.Add(messageForRTB);
                    logStopwatch.Restart();
                    if (ShowOnRtb)
                    {
                        //if (MessageForRTBList.Count > 0)
                        //{
                        //    foreach (MessageForRTB mes in MessageForRTBList)
                        //    {
                        //        BeginInvoke(new AppendText(PrintToRTB), mes);
                        //    }

                        //    MessageForRTBList.Clear();
                        //}

                        BeginInvoke(new AppendText(PrintToRTB), messageForRTB);

                    }


                }
                else
                {
                    Thread.Sleep(1);
                    if (logStopwatch.ElapsedMilliseconds > 20000)
                    {
                        logStopwatch.Reset();
                        if (mode)
                        {
                            BeginInvoke(new NullFunction(PrimaRTB.Clear));
                            foreach (MessageForRTB mes in RTBRedrawList)
                            {
                                BeginInvoke(new AppendText(PrintToRTB), mes);
                            }
                        }
                    }
                }
            }
        }
        public struct MessageForRTB
        {
            public string message;
            public Color color;
            public bool newline;
            public MessageForRTB(string message, Color color, bool newline = true)
            {
                this.message = message;
                this.color = color;
                this.newline = newline;
                MessagesForRTBQueue.Enqueue(this);
            }
        }
        private void BCVK_Client_MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (net != null)
            {
                net.TcpClose();
            }
            Environment.Exit(0);
        }
        public void Clear()
        {
            GroupBox[] groupboxes = { /*groupMainBox0,*/ groupMainBox1, groupMainBox2, groupMainBox10, groupMainBox11,
                                        groupMainBox12, groupMainBox13, groupMainBox14, groupMainBox15, groupMainBox16,
                                        groupMainBox20/*groupMainBox255*/ };
            foreach (GroupBox groupbox in groupboxes)
            {
                groupbox.Visible = false;
            }
        }
        #region KeyPress
        private void Digit_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8)
            {
                e.Handled = true;
            }
        }
        private void Message1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (number == 32 || number == 8 || number == 65 || number == 66 || number == 67 || number == 68 || number == 69 || number == 70
                    || number == 97 || number == 98 || number == 99 || number == 100 || number == 101 || number == 102 || Char.IsDigit(number))
            {
                e.KeyChar = Convert.ToChar(e.KeyChar.ToString().ToUpper());
                e.Handled = false;
            }
            else
            { e.Handled = true; }
        }
        private void Sach1Chaika_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            TextBox tb = sender as TextBox;
            if (!Char.IsDigit(number) && number != 8)
            {
                e.Handled = true;
            }
            else if (number == 65 || number == 66 || number == 67 || number == 68 || number == 69 || number == 70
                    || number == 97 || number == 98 || number == 99 || number == 100 || number == 101 || number == 102)
            {
                var t = tb.SelectionStart;
                string part1 = tb.Text.Substring(0, tb.SelectionStart);
                string part2 = tb.Text.Substring(tb.SelectionStart, tb.TextLength - tb.SelectionStart);
                Char.ToUpper(number);
                tb.Text = part1 + e.KeyChar.ToString().ToUpper() + part2;
                tb.SelectionStart = t + 1;
                e.Handled = true;
            }
        }
        private void Message1Chaika_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;
            char number = e.KeyChar;//77
            string mastrue = "()-?:.,'=/+";
            var t = 0;
            if ((Char.IsSeparator(number) && number != 12) || Char.IsDigit(number) || number == 8)//&& number != 8
            {
                e.Handled = false;
            }
            else if (Char.IsLetter(number))
            {
                if (number != 0x451 && number != 0x401 && number != 0x427 && number != 0x447 && number != 0x42A && number != 0x44A)
                {
                    t = tb.SelectionStart;
                    string part1 = tb.Text.Substring(0, tb.SelectionStart);
                    string part2 = tb.Text.Substring(tb.SelectionStart, tb.TextLength - tb.SelectionStart);
                    Char.ToUpper(number);
                    tb.Text = part1 + e.KeyChar.ToString().ToUpper() + part2;
                    tb.SelectionStart = t + 1;
                }
                e.Handled = true;
            }
            else
            {
                for (int i = 0; i < mastrue.Length; i++)
                    if (Char.Equals(number, mastrue[i]))
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;
                    }
            }
        }
        #endregion
        #region TextChanged или ограничение на вводимые значения
        private void Сhchtextbox11_TextChanged(object sender, EventArgs e)
        {
            TextBox f = sender as TextBox;
            if (f.Text.Length == 0)
            {
                f.Text = "0";
            }
            if (Convert.ToInt16(сhchtextbox11.Text) > 23)
            {
                сhchtextbox11.Text = "23";
            }
        }
        private void Sstextbox11_TextChanged(object sender, EventArgs e)
        {
            TextBox f = sender as TextBox;
            if (f.Text.Length == 0)
            {
                f.Text = "0";
            }
            if (Convert.ToInt16(f.Text) > 59)
            {
                f.Text = "59";
            }
        }
        private void Byte_TextChanged(object sender, EventArgs e)
        {
            TextBox f = sender as TextBox;
            if (f.Text.Length == 0)
            {
                f.Text = "0";
            }
            if (Convert.ToInt16(f.Text) > 255)
            {
                f.Text = "255";
            }
        }
        private void DoubleByte_TextChanged(object sender, EventArgs e)
        {
            TextBox f = sender as TextBox;
            if (f.Text.Length == 0)
            {
                f.Text = "0";
            }
            if (Convert.ToInt32(f.Text) > 65535)
            {
                f.Text = "65535";
            }
        }
        private void Db_TextChanged(object sender, EventArgs e)
        {
            TextBox f = sender as TextBox;
            if (f.Text.Length == 0)
            {
                f.Text = "0";
            }
            //if (Convert.ToInt32(f.Text) > 1492)
            //{
            //    f.Text = "1492";
            //}
        }
        #endregion
        #region elementValueChanging
        private void NumberOfHandCommand_SelectedIndexChanged(object sender, EventArgs e)
        {
            Clear();
            MakrosGB.Visible = false;
            switch (numberOfHandCommand.SelectedIndex)
            {
                //************************************************************************
                //ПРИ ДОБАВЛЕНИИ МЕНЯТЬ ПРОВЕРКИ В ГЕНЕРАЦИИ СООБЩЕНИЙ
                //************************************************************************
                case 0:
                    {
                        //numPA.Checked = !(ipPaCheckedMas[0]);
                        //ipPA.Checked = (ipPaCheckedMas[0]);
                        groupMainBox1.Visible = true;
                        //numPA.Checked = !ipstate;
                        //ipPA.Checked = ipstate;
                        ipPA.Enabled = true;
                        break;
                    }
                case 1:
                    {
                        //numPA.Checked = !(ipPaCheckedMas[0]);
                        //ipPA.Checked = (ipPaCheckedMas[0]);
                        groupMainBox2.Visible = true;
                        //numPA.Checked = !ipstate;
                        //ipPA.Checked = ipstate;
                        ipPA.Enabled = true;
                        break;
                    }
                case 2:
                    {
                        groupMainBox10.Visible = true;
                        numPA.Checked = true;
                        //ipPA.Checked = false;
                        //ipPA.Enabled = false;
                        break;
                    }
                case 3:
                    {
                        groupMainBox11.Visible = true;
                        break;
                    }
                case 4:
                    {
                        groupMainBox12.Visible = true;
                        break;
                    }
                case 5:
                    {
                        groupMainBox13.Visible = true;
                        numPA.Checked = true;
                        //ipPA.Checked = false;
                        ipPA.Enabled = true;
                        break;
                    }
                case 6:
                    {
                        groupMainBox14.Visible = true;
                        ipPA.Enabled = true;
                        break;
                    }
                case 7:
                    {
                        groupMainBox16.Visible = true;
                        numPA.Checked = true;
                        ipPA.Checked = false;
                        ipPA.Enabled = false;
                        break;
                    }
                case 8:
                    {
                        break;
                    }
                case 9:
                    {
                        MakrosGB.Visible = true;
                        break;
                    }
            }
        }
        private void ModeTestRB1_CheckedChanged(object sender, EventArgs e)
        {
            if (ModeTestRB1.Checked)
            {
                BringIndexFromFormComponent DelegateIndexReturn = ReturnSelectedIndex;
                // mode = false;
                numberofHandCommandGB.Visible = true;
                numberOfChannelsGB.Visible = false;
                zasCom10Auto.Visible = false;
                zasCom1Auto.Visible = false;
                typeOfAutoTestGB.Visible = false;
                AddressTypeGB.Visible = true;
                if (numPA.Checked == true)
                {
                    numBox.Visible = true;
                }
                else if (ipPA.Checked == true)
                {
                    adrBox.Visible = true;
                }
                if (numberOfHandCommand.SelectedIndex == 8)
                { MakrosGB.Visible = true; }
                else
                {
                    int n = (int)Invoke(DelegateIndexReturn, numberOfHandCommand);
                    numberOfHandCommand.SelectedIndex = 7;
                    numberOfHandCommand.SelectedIndex = n;
                }
            }
            else if (ModeTestRB2.Checked)
            {
                // mode = true;
                numberofHandCommandGB.Visible = false;
                numberOfChannelsGB.Visible = true;
                zasCom10Auto.Visible = true;
                zasCom1Auto.Visible = true;
                MakrosGB.Visible = false;
                typeOfAutoTestGB.Visible = true;

                RadioButton[] autoTests = new RadioButton[3] { typeOfAutoTestPerevalRB, typeOfAutoTestEthernetRB, typeOfAutoTestPerfomanceTestRB };
                for (int i = 0; i < autoTests.Length; i++)
                {
                    if (autoTests[i].Checked)
                    {
                        autoTests[i].Checked = false;
                        autoTests[i].Checked = true;
                    }

                }
            }
        }
        private void ZasFFCom1Auto_CheckedChanged(object sender, EventArgs e)
        {
            zas2Com1Auto.Enabled = zas1Com1Auto.Enabled = !zasFFCom1Auto.Checked;
        }
        private void ZasFFCom10Auto_CheckedChanged(object sender, EventArgs e)
        {
            zas2Com10Auto.Enabled = zas1Com10Auto.Enabled = !zasFFCom10Auto.Checked;
        }
        private void NumberOfChannelFirstTB_TextChanged(object sender, EventArgs e)
        {
            TextBox f = sender as TextBox;
            if (Convert.ToInt16(f.Text) < 1)
            {
                f.Text = "1";
            }
            if (Convert.ToInt16(f.Text) > 8)
            {
                сhchtextbox11.Text = "8";
            }
        }
        private void NumberOfChannelFirstTB_SelectedItemChanged(object sender, EventArgs e)
        {
            CheckBox[] checkboxes = { checkBoxChannel1, checkBoxChannel2, checkBoxChannel3, checkBoxChannel4, checkBoxChannel5, checkBoxChannel6, checkBoxChannel7, checkBoxChannel8 };
            foreach (CheckBox checkbox in checkboxes)
                if (Convert.ToInt16(numberOfChannelFirstTB.Text) > Convert.ToInt16(numberOfChannelLastTB.Text))
                {
                    numberOfChannelLastTB.SelectedIndex = numberOfChannelFirstTB.SelectedIndex;
                }
            for (int i = 0; i < checkboxes.Length; i++)
            {
                checkboxes[i].Checked = false;
            }
            for (int i = 7 - numberOfChannelFirstTB.SelectedIndex; i < 7 - numberOfChannelLastTB.SelectedIndex + 1; i++)
            {
                checkboxes[i].Checked = true;
            }
        }
        private void NumberOfChannelLastTB_SelectedItemChanged(object sender, EventArgs e)
        {
            CheckBox[] checkboxes = { checkBoxChannel1, checkBoxChannel2, checkBoxChannel3, checkBoxChannel4, checkBoxChannel5, checkBoxChannel6, checkBoxChannel7, checkBoxChannel8 };
            foreach (CheckBox checkbox in checkboxes)
                if (Convert.ToInt16(numberOfChannelFirstTB.Text) > Convert.ToInt16(numberOfChannelLastTB.Text))
                {
                    numberOfChannelFirstTB.SelectedIndex = numberOfChannelLastTB.SelectedIndex;
                }
            for (int i = 0; i < checkboxes.Length; i++)
            {
                checkboxes[i].Checked = false;
            }
            for (int i = 7 - numberOfChannelFirstTB.SelectedIndex; i < 7 - numberOfChannelLastTB.SelectedIndex + 1; i++)
            {
                checkboxes[i].Checked = true;
            }
        }
        private void CheckBoxChannelAll_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox[] checkboxes = { checkBoxChannel1, checkBoxChannel2, checkBoxChannel3, checkBoxChannel4, checkBoxChannel5, checkBoxChannel6, checkBoxChannel7, checkBoxChannel8 };
            foreach (CheckBox checkbox in checkboxes)
            {
                checkbox.Checked = checkBoxChannelAll.Checked;
                checkbox.Enabled = !checkBoxChannelAll.Checked;
            }
        }
        static bool numberRadiostations = false;
        static bool onlyNulls = false;
        private void OnlyNullsCB_CheckedChanged(object sender, EventArgs e)
        {
            onlyNulls = onlyNullsCB.Checked;
        }
        private void OnlyNumbersOfchannels_CheckedChanged(object sender, EventArgs e)
        {
            numberRadiostations = onlyNumbersOfchannels.Checked;
        }
        private void SachComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sach2ComboBox.SelectedIndex == 0)
            {
                sach2TextBox.Enabled = false;
            }
            else
            {
                sach2TextBox.MaxLength = Convert.ToInt16(sach2ComboBox.Text);
                sach2TextBox.Text = "";
                sach2TextBox.Enabled = true;
            }
        }
        private void Interfacetype10_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (interfacetype10.SelectedIndex == 2)
            {
                сomboBoxSpeedCode10.Enabled = false;
            }
            else
            {
                сomboBoxSpeedCode10.Enabled = true;
            }
        }
        private void ProtocoltypeChannel1All_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ProtocoltypeChannel1All.SelectedIndex == 7)
            {
                LengthOfPerevalMes1GB.Visible = false;
                LengthOfbreakevenMes1GB.Visible = true;
                LengthOfChaikaMes1GB.Visible = false;
            }
            else if (ProtocoltypeChannel1All.SelectedIndex > 0 && ProtocoltypeChannel1All.SelectedIndex <= 4)
            {
                LengthOfPerevalMes1GB.Visible = false;
                LengthOfbreakevenMes1GB.Visible = false;
                LengthOfChaikaMes1GB.Visible = true;
            }
            else
            {
                LengthOfPerevalMes1GB.Visible = true;
                LengthOfbreakevenMes1GB.Visible = false;
                LengthOfChaikaMes1GB.Visible = false;
            }
        }
        private void ErrorLabelChanging(int numberlabel, int numberError)
        {
            try
            {
                switch (numberError)
                {
                    case 0:
                        {
                            errorlabels[numberlabel].Text = "On";
                            break;
                        }
                    case 1:
                        {

                            errorlabels[numberlabel].Text = "Tout 0";
                            break;
                        }
                    case 2:
                        {
                            if (!answerRecieved[numberlabel] && !kvitRecieved[numberlabel])
                            {
                                errorlabels[numberlabel].Text = "Tout 1&3";
                            }
                            else if (!answerRecieved[numberlabel])
                            {
                                errorlabels[numberlabel].Text = "Tout 1";
                            }
                            else if (!kvitRecieved[numberlabel])
                            {
                                errorlabels[numberlabel].Text = "Tout 3";
                            }
                            else
                            {
                                errorlabels[numberlabel].Text = "Err";
                            }
                            break;
                        }
                }
            }
            catch //(Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }
        }
        private void CounterPTCChanging(int number, int count)
        {
            try
            {
                Counters[number].Text = Convert.ToString(count);
                //pTC.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void IpPA_CheckedChanged(object sender, EventArgs e)
        {
            if (numberOfHandCommand.SelectedIndex == 0)
            {
                ipPaCheckedMas[0] = ipPA.Checked;
            }
            else if (numberOfHandCommand.SelectedIndex == 1)
            {
                ipPaCheckedMas[1] = ipPA.Checked;
            }
            else if (numberOfHandCommand.SelectedIndex == 2)
            {
                ipPaCheckedMas[2] = ipPA.Checked;
            }
            else if (numberOfHandCommand.SelectedIndex == 3)
            {
                ipPaCheckedMas[3] = ipPA.Checked;
            }
            else if (numberOfHandCommand.SelectedIndex == 4)
            {
                ipPaCheckedMas[4] = ipPA.Checked;
            }
            else if (numberOfHandCommand.SelectedIndex == 5)
            {
                ipPaCheckedMas[5] = ipPA.Checked;
            }
            else if (numberOfHandCommand.SelectedIndex == 6)
            {
                ipPaCheckedMas[6] = ipPA.Checked;
            }
            else if (numberOfHandCommand.SelectedIndex == 7)
            {
                ipPaCheckedMas[7] = ipPA.Checked;
            }
            else if (numberOfHandCommand.SelectedIndex == 8)
            {
                ipPaCheckedMas[8] = ipPA.Checked;
            }
            if (ipPA.Checked)
            {
                adrBox.Visible = true;
                numBox.Visible = false;
            }
            else
            {
                adrBox.Visible = false;
                numBox.Visible = true;
            }
        }
        private void DataHandButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (DataHandButton1.Checked)
            {
                groupBox1Mes1.Visible = true;
            }
            else
            {
                groupBox1Mes1.Visible = false;
            }
        }
        private void Zas2Com1Auto_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox f = sender as CheckBox;
            if (zas1Com1Auto.Checked == false && zas2Com1Auto.Checked == false)
            {
                MessageBox.Show("Хотя бы один Зас должен быть выбран");
                f.Checked = true;
            }
        }
        private void TypeOfAutoTestEthernetRB_CheckedChanged(object sender, EventArgs e)
        {

            RadioButton ch = sender as RadioButton;
            Clear();
            MakrosGB.Visible = false;
            if (ch.Checked == true)
            {
                AddressTypeGB.Visible = false;
                adrBox.Visible = false;
                numBox.Visible = false;
                numberOfChannelsGB.Visible = false;
                MakrosGB.Visible = false;
            }
            groupMainBox16.Visible = true;
        }
        private void TypeOfAutoTestPerevalRB_CheckedChanged(object sender, EventArgs e)
        {
            Clear();
            MakrosGB.Visible = false;
            if (typeOfAutoTestPerevalRB.Checked == true)
            {
                AddressTypeGB.Visible = true;
                numPA.Enabled = true;
                ipPA.Enabled = true;
                groupMainBox1.Visible = true;
                if (ipPA.Checked)
                {
                    adrBox.Visible = true;
                }
                else
                {
                    numBox.Visible = true;
                }
                numberOfChannelsGB.Visible = true;
                zasCom10Auto.Visible = true;
                zasCom1Auto.Visible = true;
            }
        }
        #endregion
        #region Click
        private void StartTestButton_Click(object sender, EventArgs e)
        {
            //_typeofExchange = ModeTestRB2.Checked;
            mode = ModeTestRB2.Checked;

            if (PrimaRTB.CanFocus) { PrimaRTB.Focus(); }
            if (ModeTestRB2.Checked) { StartTestButton.Enabled = false; }
            StartTest();
        }
        int lastSelectionStart = 0;



        private void FindBtn_Click(object sender, EventArgs e)
        {
            if (findTB.Text.Length > 0)
            {
                PrimaRTB.Focus();

                int nextPointOfFindInPrimaRTB = PrimaRTB.Find(findTB.Text, startPointOfFindInPrimaRTB, RichTextBoxFinds.None);

                if (nextPointOfFindInPrimaRTB >= 0)
                {
                    if (PrimaRTB.SelectionStart == lastSelectionStart)
                    {
                        startPointOfFindInPrimaRTB = nextPointOfFindInPrimaRTB + 1;
                        nextPointOfFindInPrimaRTB = PrimaRTB.Find(findTB.Text, startPointOfFindInPrimaRTB, RichTextBoxFinds.None);
                    }
                    startPointOfFindInPrimaRTB = nextPointOfFindInPrimaRTB + 1;
                }
                else if (startPointOfFindInPrimaRTB == 0)
                {
                    MessageBox.Show("Совпадений не найдено");
                }
                else
                {
                    nextPointOfFindInPrimaRTB = PrimaRTB.Find(findTB.Text, 0, RichTextBoxFinds.None);
                    startPointOfFindInPrimaRTB = nextPointOfFindInPrimaRTB + 1;
                }
                lastSelectionStart = PrimaRTB.SelectionStart;
                //PrimaRTB.
            }
        }
        private void FindUpBtn_Click(object sender, EventArgs e)
        {
            if (findTB.Text.Length > 0)
            {
                PrimaRTB.Focus();
                int nextPointOfFindInPrimaRTB = PrimaRTB.Find(findTB.Text, 0, startPointOfFindInPrimaRTB, RichTextBoxFinds.Reverse);
                if (nextPointOfFindInPrimaRTB >= 0)
                {
                    if (PrimaRTB.SelectionStart == lastSelectionStart)
                    {
                        startPointOfFindInPrimaRTB = nextPointOfFindInPrimaRTB + 1;
                        nextPointOfFindInPrimaRTB = PrimaRTB.Find(findTB.Text, 0, startPointOfFindInPrimaRTB, RichTextBoxFinds.Reverse);
                    }
                    startPointOfFindInPrimaRTB = nextPointOfFindInPrimaRTB + 1;
                }
                else if (startPointOfFindInPrimaRTB == 0)
                {
                    MessageBox.Show("Совпадений не найдено");
                }
                else
                {
                    nextPointOfFindInPrimaRTB = PrimaRTB.Find(findTB.Text, PrimaRTB.Text.Length, RichTextBoxFinds.Reverse);
                    startPointOfFindInPrimaRTB = nextPointOfFindInPrimaRTB + 1;
                }
                lastSelectionStart = PrimaRTB.SelectionStart;
            }

        }
        private void StopTestButton_Click(object sender, EventArgs e)
        {
            if (!tokenTestStop.IsCancellationRequested)
            {
                testStop.Cancel();
                if (Channels != null)
                {
                    foreach (ChannelWorker channel in Channels)
                    {
                        channel.StopChannel();
                    }
                }
                StartTestButton.Enabled = true;
                perfomanceTestMode = false;

                ShowThroughputOfChannels();
            }
            //Invoke(new NullFunction(RecordToFile));
        }
        private void ShowThroughputOfChannels()
        {
            double testTime = stopwatchFullTest.ElapsedMilliseconds;
            Thread.Sleep(1000);
            for (int i = 0; i < amountOfDataOnChannel.Length; i++)
            {
                if (amountOfDataOnChannel[i] > 0)
                {
                    RecordToRTB(Color.Red, "{0} Rs: общее количество данных = {1} бит, среднее количество данных в секунду = {2}", Convert.ToString(i + 1),
                    Convert.ToString(amountOfDataOnChannel[i] * 8), Convert.ToString(amountOfDataOnChannel[i] * 8 * 1000 / testTime));
                }
            }

        }

        private void PauseTestButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (startTest.ThreadState == System.Threading.ThreadState.WaitSleepJoin)
                {
                    startTest.Suspend();
                }
                else
                {
                    startTest.Resume();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void PlacingParametresStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Init())
            {
                mode = true;
                // Task.Factory.StartNew(() => SettingTest(false, 5, 0, 0, 0, 0));
                startTest = new Thread(() => SettingTest(false, 5, 0, 0, 0, 0));
                startTest.Start();
            }
        }
        private void ProbabilityOfBringingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Init())
            {
                chS = new ChannelSelector();
                Thread chSThread = new Thread(() => chS.ShowDialog());
                chSThread.Start();
                while (chSThread.ThreadState != System.Threading.ThreadState.Stopped)
                    Thread.Sleep(1);
                int channel = ChannelSelector._channel;
                if (channel == -1)
                {
                    return;
                }
                else if (channel >= 0)
                {
                    ChannelSelector._channel = -1;
                    mes = new MessageBoxWithButtons("Тип кода", "Выберите тип кода", "Короткий", "Длинный");
                    Thread mesBox = new Thread(() => mes.ShowDialog());
                    mesBox.Start();
                    while (mesBox.ThreadState != System.Threading.ThreadState.Stopped)
                        Thread.Sleep(1);
                    bool rpbool = mes._state;
                    int rp = 0;
                    if (!rpbool)
                    { rp = 5; }
                    else if (rpbool)
                    {
                        { rp = 6; }
                    }
                    groupBox1Mes1.Hide();
                    startTest = new Thread(() => ProbabilityOfBringingTest(channel, rp));
                    startTest.Start();
                    //probabilityOfBringingTest(channel,rp);
                }
            }
        }
        private void RestartBtn_Click(object sender, EventArgs e)
        {
            if (net != null)
            {
                net.TcpClose();
                connected = false;
                Thread.Sleep(50);
                connectionstate = true;
                PrimaRTB.Clear();
                CreationConnect(addressPrima.Text, portPrima.Text);
                return;
            }
            CreationConnect(addressPrima.Text, portPrima.Text);
            Task.Factory.StartNew(() => net.TCPReConnect());
        }
        private void EthernetChannelsSettings_BTN_Click(object sender, EventArgs e)
        {
            AdrEth.Hide();
            AdrEth.Show();
        }
        private void UdpChatStart_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog
            {
                Filter = "exe files(*.exe)|*.exe|ALL files(*.*)|*.*"
            };
            if (OFD.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            ProcessStartInfo procInfo = new ProcessStartInfo
            {
                FileName = OFD.FileName
            };
            EthernetAdresses = Ethernetadressesgeneration();
            int numberOfRadiostations = EthernetAdresses.Length / 24;
            string arguments = Convert.ToString(numberOfRadiostations);
            for (int i = 0; i < numberOfRadiostations; i++)
            {
                arguments = arguments + " " + Convert.ToString(EthernetAdresses[i * 24 + 4] * 256 + EthernetAdresses[i * 24 + 5]);
            }
            procInfo.Arguments = arguments;
            Process.Start(procInfo);
            return;
        }
        private void ПроверкаВремениГотовностиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startTest = new Thread(() => ПроверкаВремениГотовности());
            startTest.Start();
        }
        private void ПроверкаВремениГотовности()
        {
            Stopwatch stopwatchTestConnection = new Stopwatch();
            rightMessage0 = false;
            if (connected)
            {
                net.TcpClose();
            }
            MessageBox.Show("Включите комплекс, по нажатию кнопки начнется проверка готовности");
            RecordToRTB(Color.Black, "Начата проверка готовности");
            stopwatchTestConnection.Start();
            if (net != null)
            {
                connectionstate = true;
                while (connectionstate)
                {
                    Thread.Sleep(1);
                    if (stopwatchTestConnection.ElapsedMilliseconds > 25000)
                    {
                        stopwatchTestConnection.Stop();
                        RecordToRTB(Color.Red, "ERROR за время {0} с не было установлено соединение", Convert.ToString(stopwatchTestConnection.ElapsedMilliseconds / 1000));
                        return;
                    }
                }
                while (stopwatchTestConnection.ElapsedMilliseconds < 60000)
                {
                    NewgeneratingMessage12(0, 1);
                    while (accepted == false)
                    {
                        Thread.Sleep(1);
                        if (stopwatchAccept.ElapsedMilliseconds < 60000)
                        {
                            RecordToRTB(Color.Red, "ERROR за время {0} с не было получено подтверждение о состоянии", Convert.ToString(stopwatchAccept.ElapsedMilliseconds / 1000));
                        }
                    }
                    if (rightMessage0 == true)
                    {
                        RecordToRTB(Color.Black, "за время {0} с комплекс был приведен в состояние готовности", Convert.ToString(stopwatchTestConnection.ElapsedMilliseconds / 1000));
                        stopwatchTestConnection.Stop();
                        return;
                    }
                    Thread.Sleep(1000);
                }
                RecordToRTB(Color.Red, "ERROR за время {0} с комплекс не был приведен в состояние готовности", Convert.ToString(stopwatchAccept.ElapsedMilliseconds / 1000));
            }
            else
            {
                net = new NetWorker(addressPrima.Text, portPrima.Text, this);
                connected = true;
                var t = net.TCPConnect();
                if (!t)
                {
                    stopwatchTestConnection.Stop();
                    RecordToRTB(Color.Red, "ERROR за время {0} с не было установлено соединение", Convert.ToString(stopwatchTestConnection.ElapsedMilliseconds / 1000));
                    connected = false;
                    return;
                }
                else
                {
                    while (stopwatchTestConnection.ElapsedMilliseconds < 60000)
                    {
                        NewgeneratingMessage12(0, 1);
                        while (accepted == false)
                        {
                            Thread.Sleep(1);
                            if (stopwatchAccept.ElapsedMilliseconds < 60000)
                            {
                                RecordToRTB(Color.Red, "ERROR за время {0} с не было получено подтверждение о состоянии", Convert.ToString(stopwatchAccept.ElapsedMilliseconds / 1000));
                            }
                        }
                        if (rightMessage0 == true)
                        {
                            RecordToRTB(Color.Black, "за время {0} с комплекс был приведен в состояние готовности", Convert.ToString(stopwatchTestConnection.ElapsedMilliseconds / 1000));
                            stopwatchTestConnection.Stop();
                            Task.Factory.StartNew(() => net.TCPReConnect());
                            return;
                        }
                        Thread.Sleep(1000);
                    }
                    RecordToRTB(Color.Red, "ERROR за время {0} с комплекс не был приведен в состояние готовности", Convert.ToString(stopwatchAccept.ElapsedMilliseconds / 1000));
                    Task.Factory.StartNew(() => net.TCPReConnect());
                }
            }
        }
        private void ПрекратитьВыводToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowOnRtb = (!ShowOnRtb);
            прекратитьВыводToolStripMenuItem.Checked = (!прекратитьВыводToolStripMenuItem.Checked);
        }
        private void OpenMessage_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog
            {
                InitialDirectory = (Environment.CurrentDirectory + "\\SavedMessages"),
                RestoreDirectory = true,
                Filter = "Text files(*.txt)|*.txt|ALL files(*.*)|*.*"
            };
            if (OFD.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            Stream filestream = OFD.OpenFile();
            StreamReader sw = new StreamReader(filestream);
            string str = sw.ReadToEnd();
            sw.Close();
            BeginInvoke(new ToTextBox(Totextbox1), Message1, str);
            return;
        }
        private void SaveMessage_Click(object sender, EventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog
            {
                InitialDirectory = (Environment.CurrentDirectory + "\\SavedMessages"),
                FileName = "*.txt",
                DefaultExt = "txt",
                Filter = "Text files(*.txt)|*.txt|ALL files(*.*)|*.*"
            };
            if (SFD.ShowDialog() == DialogResult.OK)
            {
                using (FileStream fstream = new FileStream(SFD.FileName, FileMode.Create))
                {
                    StreamWriter sw = new StreamWriter(fstream);
                    sw.Write(Message1.Text);
                    sw.Close();
                }
                return;
            }
        }
        private void АссинхронныйТестToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Init())
            {
                mode = true;
                int[] rpStates = new int[4] { 0, 0, 1, 1 };
                startTest = new Thread(() => FullMasterMixedTest(rpStates));
                startTest.Start();
            }
        }
        private void ТестРадиостанцийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Init())
            {
                mode = true;
                startTest = new Thread(() => EthernetRSTest());
                startTest.Start();
            }
        }
        private void GeneralTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrimaRTB.Clear();
            if (Init())
            {
                bool TG;
                bool FL;
                bool WithoutAnswer;
                bool WithAnswer;
                GeneralTestForm GTF = new GeneralTestForm();
                Thread GTFBox = new Thread(() => GTF.ShowDialog());
                GTFBox.Start();
                while (GTFBox.ThreadState != System.Threading.ThreadState.Stopped)
                    Thread.Sleep(1);
                if (GTF.Start)
                {
                    TG = GTF.TG;
                    FL = GTF.FL;
                    WithoutAnswer = GTF.WithoutAnswer;
                    WithAnswer = GTF.WithAnswer;
                    if (!(!TG && !FL && !WithoutAnswer && !WithAnswer))
                    {
                        mode = true;
                        int channelFirst = Convert.ToInt32(Invoke(DelegateTextReturn, numberOfChannelFirstTB));
                        int channelLast = Convert.ToInt32(Invoke(DelegateTextReturn, numberOfChannelLastTB));
                        startTest = new Thread(() => GeneralTest(channelFirst, channelLast, TG, FL, WithoutAnswer, WithAnswer));
                        startTest.Start();
                    }
                }
            }
        }
        private void ClearRTBButton_Click(object sender, EventArgs e)
        {
            PrimaRTB.Clear();
        }
        private void TestingParametresStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Init())
            {
                bool ts;
                mes = new MessageBoxWithButtons("Тип стыка", "Выберите тип стыка", "С1-ТГ", "С1-ФЛ-БИ");
                Thread mesBox = new Thread(() => mes.ShowDialog());
                mesBox.Start();
                while (mesBox.ThreadState != System.Threading.ThreadState.Stopped)
                    Thread.Sleep(1);
                ts = mes._state;
                mode = true;
                startTest = new Thread(() => SettingTest(true, 5, Convert.ToInt32(ts), 0, 0, 0));
                startTest.Start();
            }
        }
        private void Форма_Click(object sender, EventArgs e)
        {
            pTC.Show();
        }
        #endregion
        #region KeyDown
        private void Message1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                e.SuppressKeyPress = true;
            }
        }
        #endregion
        #region Запись\Загрузка\Выгрузка параметров формы
        private byte[] Ethernetadressesgeneration()
        {
            int numberOfCheckedCB = 0;
            for (int i = 0; i < RadioStationcheckboxes.Length; i++)
            {
                if (RadioStationcheckboxes[i].Checked)
                {
                    numberOfCheckedCB++;
                }
            }
            // correspondenceTable = new int[numberOfCheckedCB];
            int numberOfColOfTable = 0;
            byte[] EthernetIPAdresses = new byte[numberOfCheckedCB * 24];
            int numberofEthernetIPAdresses = 0;
            string[] buf;
            for (int i = 0; i < 32; i++)
            {
                if (RadioStationcheckboxes[i].Checked)
                {
                    // correspondenceTable[numberOfColOfTable] = i;
                    numberOfColOfTable++;
                    buf = Ethernetadresses[2 * i, 0].Text.Split('.');
                    for (int j = 0; j < 4; j++)
                    {
                        EthernetIPAdresses[numberofEthernetIPAdresses] = Convert.ToByte(buf[j]);
                        numberofEthernetIPAdresses++;
                    }
                    EthernetIPAdresses[numberofEthernetIPAdresses] = Convert.ToByte(Convert.ToInt32(Ethernetadresses[2 * i, 1].Text) / 256);
                    numberofEthernetIPAdresses++;
                    EthernetIPAdresses[numberofEthernetIPAdresses] = Convert.ToByte(Convert.ToInt32(Ethernetadresses[2 * i, 1].Text) % 256);
                    numberofEthernetIPAdresses++;
                    buf = Ethernetadresses[2 * i, 2].Text.Split(':');
                    for (int j = 0; j < 6; j++)
                    {
                        EthernetIPAdresses[numberofEthernetIPAdresses] = Convert.ToByte(Convert.ToInt32(buf[j], 16));
                        numberofEthernetIPAdresses++;
                    }
                    buf = Ethernetadresses[2 * i + 1, 0].Text.Split('.');
                    for (int j = 0; j < 4; j++)
                    {
                        EthernetIPAdresses[numberofEthernetIPAdresses] = Convert.ToByte(buf[j]);
                        numberofEthernetIPAdresses++;
                    }
                    EthernetIPAdresses[numberofEthernetIPAdresses] = Convert.ToByte(Convert.ToInt32(Ethernetadresses[2 * i + 1, 1].Text) / 256);
                    numberofEthernetIPAdresses++;
                    EthernetIPAdresses[numberofEthernetIPAdresses] = Convert.ToByte(Convert.ToInt32(Ethernetadresses[2 * i + 1, 1].Text) % 256);
                    numberofEthernetIPAdresses++;
                    buf = Ethernetadresses[2 * i + 1, 2].Text.Split(':');
                    for (int j = 0; j < 6; j++)
                    {
                        EthernetIPAdresses[numberofEthernetIPAdresses] = Convert.ToByte(Convert.ToInt32(buf[j], 16));
                        numberofEthernetIPAdresses++;
                    }
                }
            }
            return EthernetIPAdresses;
        }
        private byte[] Com1adressesgeneration()
        {
            int numberOfCheckedCB = 0;
            CheckBox[] checkboxes = new CheckBox[32] { Com1CB_RS1_1, Com1CB_RS1_2, Com1CB_RS1_3, Com1CB_RS1_4, Com1CB_RS2_1, Com1CB_RS2_2, Com1CB_RS2_3, Com1CB_RS2_4,
                                                       Com1CB_RS3_1, Com1CB_RS3_2, Com1CB_RS3_3, Com1CB_RS3_4, Com1CB_RS4_1, Com1CB_RS4_2, Com1CB_RS4_3, Com1CB_RS4_4,
                                                       Com1CB_RS5_1, Com1CB_RS5_2, Com1CB_RS5_3, Com1CB_RS5_4, Com1CB_RS6_1, Com1CB_RS6_2, Com1CB_RS6_3, Com1CB_RS6_4,
                                                       Com1CB_RS7_1, Com1CB_RS7_2, Com1CB_RS7_3, Com1CB_RS7_4, Com1CB_RS8_1, Com1CB_RS8_2, Com1CB_RS8_3, Com1CB_RS8_4,};
            for (int i = 0; i < checkboxes.Length; i++)
            {
                if (checkboxes[i].Checked)
                {
                    numberOfCheckedCB++;
                }
            }
            //if (numberOfCheckedCB > 8)
            //{
            //    MessageBox.Show("Невозможно выбрать более 8 радиостанций");
            //    throw new MessageException();
            //}
            string[] buf;
            byte[] EthernetIPAdresses = new byte[numberOfCheckedCB * 12 + 1];
            EthernetIPAdresses[0] = Convert.ToByte(numberOfCheckedCB);
            int numberofEthernetIPAdresses = 1;
            for (int i = 0; i < 32; i++)
            {
                if (checkboxes[i].Checked)
                {
                    buf = Ethernetadresses[2 * i, 0].Text.Split('.');
                    for (int j = 0; j < 4; j++)
                    {
                        EthernetIPAdresses[numberofEthernetIPAdresses] = Convert.ToByte(buf[j]);
                        numberofEthernetIPAdresses++;
                    }
                    EthernetIPAdresses[numberofEthernetIPAdresses] = Convert.ToByte(Convert.ToInt32(Ethernetadresses[2 * i, 1].Text) / 256);
                    numberofEthernetIPAdresses++;
                    EthernetIPAdresses[numberofEthernetIPAdresses] = Convert.ToByte(Convert.ToInt32(Ethernetadresses[2 * i, 1].Text) % 256);
                    numberofEthernetIPAdresses++;
                    buf = Ethernetadresses[2 * i, 2].Text.Split(':');
                    for (int j = 0; j < 6; j++)
                    {
                        EthernetIPAdresses[numberofEthernetIPAdresses] = Convert.ToByte(Convert.ToInt32(buf[j], 16));
                        numberofEthernetIPAdresses++;
                    }
                }
            }
            return EthernetIPAdresses;
        }
        private void LoadParametres(object sender, EventArgs e)
        {
            if (INI.KeyExists("Connect_Parametres", "Prima_Adress"))
            {
                addressPrima.Text = INI.ReadINI("Connect_Parametres", "Prima_Adress");
                portPrima.Text = INI.ReadINI("Connect_Parametres", "Prima_Port");
                timeOutTB.Text = INI.ReadINI("Connect_Parametres", "Critical_Time_Of_Answer");
            }
            if (INI.KeyExists("Hand_Test", "MakrosMessages"))
            {
                MakrosTextBox.Text = INI.ReadINI("Hand_Test", "MakrosMessages");
                //numberOfHandCommand.SelectedIndex = int.Parse(INI.ReadINI("Hand_Test", "Number_Of_Test_Message"));
                //operatingComboBox.SelectedIndex = int.Parse(INI.ReadINI("Hand_Test", "Number_Of_Window_Of_Test_Message"));
                for (int i = 0; i < 8; i++)
                {
                    ipPaCheckedMas[i] = bool.Parse(INI.ReadINI("Hand_Test", "ipPaMas" + (i + 1)));
                }
            }
            if (INI.KeyExists("ChannelsSettings", "Type_Zadachi_Kanala"))
            {
                numberOfChannelLastTB.SelectedIndex = 8 - Convert.ToInt32(INI.ReadINI("ChannelsSettings", "numberOfChannelLastTB_Index"));
                numberOfChannelFirstTB.SelectedIndex = 8 - Convert.ToInt32(INI.ReadINI("ChannelsSettings", "numberOfChannelFirstTB_Index"));
                //ipPA.Checked = bool.Parse(INI.ReadINI("ChannelsSettings", "Type_Zadachi_Kanala"));
                //numPA.Checked = !(bool.Parse(INI.ReadINI("ChannelsSettings", "Type_Zadachi_Kanala")));
                zas1Com1Auto.Checked = bool.Parse(INI.ReadINI("ChannelsSettings", "zas1Com1Auto "));
                zas2Com1Auto.Checked = bool.Parse(INI.ReadINI("ChannelsSettings", "zas2Com1Auto "));
                if (INI.KeyExists("ChannelsSettings", "zasFFCom1Auto"))
                {
                    zasFFCom1Auto.Checked = bool.Parse(INI.ReadINI("ChannelsSettings", "zasFFCom1Auto "));

                }
                zas1Com10Auto.Checked = bool.Parse(INI.ReadINI("ChannelsSettings", "zas1Com10Auto "));
                zas2Com10Auto.Checked = bool.Parse(INI.ReadINI("ChannelsSettings", "zas2Com10Auto "));
                if (INI.KeyExists("ChannelsSettings", "zasFFCom10Auto"))
                {
                    zasFFCom10Auto.Checked = bool.Parse(INI.ReadINI("ChannelsSettings", "zasFFCom10Auto "));

                }
                CheckBox[] checkboxes = { checkBoxChannel1, checkBoxChannel2, checkBoxChannel3, checkBoxChannel4, checkBoxChannel5, checkBoxChannel6, checkBoxChannel7, checkBoxChannel8 };
                checkBoxChannelAll.Checked = bool.Parse(INI.ReadINI("ChannelsSettings", "All_ActiveChannel "));
                for (int i = 0; i < 8; i++)
                {
                    checkboxes[i].Checked = bool.Parse(INI.ReadINI("ChannelsSettings", "ActiveChannel " + i));
                }
            }
            if (INI.KeyExists("COMMAND_1", "Regim_Peregachi"))
            {
                ProtocoltypeChannel1All.SelectedIndex = int.Parse(INI.ReadINI("COMMAND_1", "Regim_Peregachi"));
                ipPA.Checked = bool.Parse(INI.ReadINI("COMMAND_1", "Type_Zadachi_Kanala"));
                numPA.Checked = !(bool.Parse(INI.ReadINI("COMMAND_1", "Type_Zadachi_Kanala")));
                CheckBox[] checkboxesadr1 = new CheckBox[32] { Com1CB_RS1_1, Com1CB_RS1_2, Com1CB_RS1_3, Com1CB_RS1_4, Com1CB_RS2_1, Com1CB_RS2_2, Com1CB_RS2_3, Com1CB_RS2_4,
                                                       Com1CB_RS3_1, Com1CB_RS3_2, Com1CB_RS3_3, Com1CB_RS3_4, Com1CB_RS4_1, Com1CB_RS4_2, Com1CB_RS4_3, Com1CB_RS4_4,
                                                       Com1CB_RS5_1, Com1CB_RS5_2, Com1CB_RS5_3, Com1CB_RS5_4, Com1CB_RS6_1, Com1CB_RS6_2, Com1CB_RS6_3, Com1CB_RS6_4,
                                                       Com1CB_RS7_1, Com1CB_RS7_2, Com1CB_RS7_3, Com1CB_RS7_4, Com1CB_RS8_1, Com1CB_RS8_2, Com1CB_RS8_3, Com1CB_RS8_4,};
                for (int i = 0; i < checkboxesadr1.Length / 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        checkboxesadr1[i * 4 + j].Checked = bool.Parse(INI.ReadINI("COMMAND_1", "Com1CB_RS" + i + "_" + j));
                    }
                }
                perevalLengthOfmessage.SelectedIndex = int.Parse(INI.ReadINI("COMMAND_1", "Kolichestvo_blokov"));
                if (INI.KeyExists("COMMAND_1", "Nomer_Vibrannogo_Zasa_1"))
                {
                    ZasNomer1RB1.Checked = bool.Parse(INI.ReadINI("COMMAND_1", "Nomer_Vibrannogo_Zasa_1"));
                    ZasNomer2RB1.Checked = !(bool.Parse(INI.ReadINI("COMMAND_1", "Nomer_Vibrannogo_Zasa_2")));
                    ZasFFRB1.Checked = bool.Parse(INI.ReadINI("COMMAND_1", "Nomer_Vibrannogo_Zasa_FF"));
                }
                npOnButton1.Checked = bool.Parse(INI.ReadINI("COMMAND_1", "Neprerivnaya_peredacha"));
                npOffButton1.Checked = !(bool.Parse(INI.ReadINI("COMMAND_1", "Neprerivnaya_peredacha")));
                DPtextbox1.Text = INI.ReadINI("COMMAND_1", "Dlina_Preambuli");
                PRGtextbox1.Text = INI.ReadINI("COMMAND_1", "Nomer_Programmy");
                DataAutoButton1.Checked = bool.Parse(INI.ReadINI("COMMAND_1", "AutoDataCheckBox"));
                DataHandButton1.Checked = !bool.Parse(INI.ReadINI("COMMAND_1", "AutoDataCheckBox"));
                Message1.Text = INI.ReadINI("COMMAND_1", "Message");
                if (INI.KeyExists("COMMAND_2", "Regim_Peregachi"))
                {
                    ZasNomer2RB2.Checked = bool.Parse(INI.ReadINI("COMMAND_2", "Nomer_Vibrannogo_Zasa"));
                    ZasNomer1RB2.Checked = !(bool.Parse(INI.ReadINI("COMMAND_2", "Nomer_Vibrannogo_Zasa")));
                    npOnButton2.Checked = bool.Parse(INI.ReadINI("COMMAND_2", "Neprerivnaya_peredacha"));
                    npOffButton2.Checked = !(bool.Parse(INI.ReadINI("COMMAND_2", "Neprerivnaya_peredacha")));
                    ProtocoltypeChannel2All.SelectedIndex = int.Parse(INI.ReadINI("COMMAND_2", "Regim_Peregachi"));
                    sach2ComboBox.SelectedIndex = int.Parse(INI.ReadINI("COMMAND_2", "SachMaxDlina"));
                    sach2TextBox.Text = INI.ReadINI("COMMAND_2", "Sach");
                    DPtextbox2.Text = INI.ReadINI("COMMAND_2", "Dlina_Preambuli");
                    nfTextBox2.Text = INI.ReadINI("COMMAND_2", "FileName");
                    prgTextBox2.Text = INI.ReadINI("COMMAND_2", "NomerProgrammi");
                }
                if (INI.KeyExists("COMMAND_10", "Skorost_Coda"))
                {
                    // INI.Write("COMMAND_10", "Regim_Peregachi", ProtocoltypeChannel10All.SelectedIndex.ToString());
                    сomboBoxSpeedCode10.SelectedIndex = int.Parse(INI.ReadINI("COMMAND_10", "Skorost_Coda"));
                    interfacetype10.SelectedIndex = int.Parse(INI.ReadINI("COMMAND_10", "Interface_Type"));
                    comboBoxPlume10.SelectedIndex = int.Parse(INI.ReadINI("COMMAND_10", "Plume"));
                    ZAS1radiobutton10.Checked = bool.Parse(INI.ReadINI("COMMAND_10", "Nomer_Zas1"));
                    ZAS2radiobutton10.Checked = bool.Parse(INI.ReadINI("COMMAND_10", "Nomer_Zas2"));
                    ZASFFradiobutton10.Checked = bool.Parse(INI.ReadINI("COMMAND_10", "Nomer_ZasFF"));
                    if (INI.KeyExists("COMMAND_10", "Tiv"))
                    {
                        TB_Tiv10.Text = INI.ReadINI("COMMAND_10", "Tiv");
                    }
                    pprchOn10.Checked = bool.Parse(INI.ReadINI("COMMAND_10", "PPRCh"));
                    pprchOff10.Checked = !pprchOn10.Checked;
                    kvitOn10.Checked = bool.Parse(INI.ReadINI("COMMAND_10", "Kvitirovanie"));
                    kvitOff10.Checked = !kvitOn10.Checked;
                }
                if (INI.KeyExists("COMMAND_11", "Hours"))
                {
                    сhchtextbox11.Text = INI.ReadINI("COMMAND_11", "Hours");
                    mmtextbox11.Text = INI.ReadINI("COMMAND_11", "Minutes");
                    sstextbox11.Text = INI.ReadINI("COMMAND_11", "Seconds");
                }
            }
            if (INI.KeyExists("COMMAND_12", "zas1"))
            {
                zas1Chaika12.Checked = bool.Parse(INI.ReadINI("COMMAND_12", "zas1"));
                zas1Pereval12.Checked = !(bool.Parse(INI.ReadINI("COMMAND_12", "zas1")));
                zas2Chaika12.Checked = bool.Parse(INI.ReadINI("COMMAND_12", "zas2"));
                zas2Pereval12.Checked = !(bool.Parse(INI.ReadINI("COMMAND_12", "zas2")));
            }
            if (INI.KeyExists("COMMAND_13", "0_Channel"))
            {
                RadioButton[] Channels13 = { channel1RadioButton13, channel2RadioButton13, channel3RadioButton13, channel4RadioButton13, channel5RadioButton13, channel6RadioButton13, channel7RadioButton13, channel8RadioButton13 };
                for (int i = 0; i < 8; i++)
                {
                    Channels13[i].Checked = bool.Parse(INI.ReadINI("COMMAND_13", i + "_Channel"));
                }
            }
            if (INI.KeyExists("COMMAND_13", "0_Channel"))
            {
                RadioButton[] Channels14 = { channel1RadioButton14, channel2RadioButton14, channel3RadioButton14, channel4RadioButton14, channel5RadioButton14, channel6RadioButton14, channel7RadioButton14, channel8RadioButton14 };
                for (int i = 0; i < 8; i++)
                {
                    Channels14[i].Checked = bool.Parse(INI.ReadINI("COMMAND_14", i + "_Channel"));
                }
            }
            if (INI.KeyExists("COMMAND_15", "OSP"))
            {
                ospOnButton15.Checked = bool.Parse(INI.ReadINI("COMMAND_15", "OSP"));
                ospOffButton15.Checked = !(bool.Parse(INI.ReadINI("COMMAND_15", "OSP")));
            }
            if (INI.KeyExists("COMMAND_16", "Accept_Adresses_1"))
            {
                CheckBox[] Accept_Adresses = new CheckBox[32] {RadioStation1CB, RadioStation2CB, RadioStation3CB, RadioStation4CB, RadioStation5CB, RadioStation6CB,RadioStation7CB, RadioStation8CB,
                RadioStation9CB, RadioStation10CB, RadioStation11CB,RadioStation12CB, RadioStation13CB, RadioStation14CB,RadioStation15CB, RadioStation16CB,
                RadioStation17CB, RadioStation18CB, RadioStation19CB,RadioStation20CB, RadioStation21CB, RadioStation22CB,RadioStation23CB, RadioStation24CB,
                RadioStation25CB, RadioStation26CB, RadioStation27CB,RadioStation28CB, RadioStation29CB, RadioStation30CB,RadioStation31CB, RadioStation32CB};
                for (int i = 0; i < 32; i++)
                {
                    Accept_Adresses[i].Checked = bool.Parse(INI.ReadINI("COMMAND_16", "Accept_Adresses_" + Convert.ToString(i + 1)));
                }
                for (int i = 0; i < 32; i++)
                {
                    Ethernetadresses[2 * i, 0].Text = INI.ReadINI("COMMAND_16", "Rad_IP_" + Convert.ToString(i + 1));
                    Ethernetadresses[2 * i, 1].Text = INI.ReadINI("COMMAND_16", "Rad_P_" + Convert.ToString(i + 1));
                    Ethernetadresses[2 * i, 2].Text = INI.ReadINI("COMMAND_16", "Rad_MAC_" + Convert.ToString(i + 1));
                    Ethernetadresses[2 * i + 1, 0].Text = INI.ReadINI("COMMAND_16", "UU_IP_" + Convert.ToString(i + 1));
                    Ethernetadresses[2 * i + 1, 1].Text = INI.ReadINI("COMMAND_16", "UU_P_" + Convert.ToString(i + 1));
                    Ethernetadresses[2 * i + 1, 2].Text = INI.ReadINI("COMMAND_16", "UU_MAC_" + Convert.ToString(i + 1));


                }
            }
            if (INI.KeyExists("COMMAND_16", "Rad_1_Shl_On"))
            {
                for (int i = 0; i < 32; i++)
                {
                    if (INI.KeyExists("COMMAND_16", "Rad_1_Shl_Off"))
                    {
                        EthernetCustomisationRBs[i, 0].Checked = !bool.Parse(INI.ReadINI("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_Shl_Off"));
                        EthernetCustomisationRBs[i, 5].Checked = !bool.Parse(INI.ReadINI("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_Kv_Off"));
                    }
                    EthernetCustomisationRBs[i, 1].Checked = bool.Parse(INI.ReadINI("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_Shl_On"));
                    EthernetCustomisationRBs[i, 6].Checked = bool.Parse(INI.ReadINI("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_Kv_On"));
                    if (INI.KeyExists("COMMAND_16", "Rad_1_Kvt_Off"))
                    {
                        EthernetCustomisationRBs[i, 7].Checked = bool.Parse(INI.ReadINI("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_Kvt_Off"));
                        EthernetCustomisationRBs[i, 8].Checked = bool.Parse(INI.ReadINI("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_Kvt_On"));
                    }

                    EthernetCustomisationRBs[i, 4].Checked = bool.Parse(INI.ReadINI("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_Zas_FF"));
                    EthernetCustomisationRBs[i, 2].Checked = bool.Parse(INI.ReadINI("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_Zas_1"));
                    EthernetCustomisationRBs[i, 3].Checked = bool.Parse(INI.ReadINI("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_Zas_2"));
                }
            }
            for (int i = 0; i < 32; i++)
            {
                EthernetCustomisationTBs[i, 0].Text = INI.ReadINI("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_KP");
                EthernetCustomisationTBs[i, 1].Text = INI.ReadINI("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_KPP");
                EthernetCustomisationTBs[i, 2].Text = INI.ReadINI("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_RD");
                if (INI.KeyExists("COMMAND_16", "Rad_1_Tiv"))
                {
                    EthernetCustomisationTBs[i, 3].Text = INI.ReadINI("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_Tiv");
                }
            }

            if (INI.KeyExists("COMMAND_16", "Rad_1_SC"))
            {
                for (int i = 0; i < 32; i++)
                {
                    EthernetCustomisationComboBoxes[i].SelectedIndex = int.Parse(INI.ReadINI("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_SC"));
                }
            }
            if (INI.KeyExists("COMMAND_20", "VDS"))
            {
                vdsinradiobutton20.Checked = bool.Parse(INI.ReadINI("COMMAND_20", "VDS"));
                vdsinradiobutton20.Checked = !(bool.Parse(INI.ReadINI("COMMAND_20", "VDS")));
            }
            if (INI.KeyExists("Mainsettings", "ModeTestRB1"))
            {
                typeOfAutoTestPerevalRB.Checked = bool.Parse(INI.ReadINI("Mainsettings", "typeOfAutoTestPerevalRB"));
                typeOfAutoTestEthernetRB.Checked = bool.Parse(INI.ReadINI("Mainsettings", "typeOfAutoTestEthernetRB"));
                typeOfAutoTestPerfomanceTestRB.Checked = bool.Parse(INI.ReadINI("Mainsettings", "typeOfAutoTestPerfomanceTestRB"));
                ModeTestRB1.Checked = bool.Parse(INI.ReadINI("Mainsettings", "ModeTestRB1"));
                ModeTestRB2.Checked = bool.Parse(INI.ReadINI("Mainsettings", "ModeTestRB2"));
                randMode.Checked = bool.Parse(INI.ReadINI("Mainsettings", "RandModeCB"));
                if (INI.KeyExists("Mainsettings", "IndependentModeCB"))
                {
                    IndependentMode.Checked = bool.Parse(INI.ReadINI("Mainsettings", "IndependentModeCB"));
                }
            }
        }
        private void SavedParametres(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => SaveConfig());
        }
        private void SaveConfig()
        {
            RecordToRTB(Color.SteelBlue, "\nСохранение конфигураций. Подождите...");

            INI.Write("Mainsettings", "ModeTestRB1", (string)Invoke(DelegateTextReturn, ModeTestRB1));
            INI.Write("Mainsettings", "ModeTestRB2", (string)Invoke(DelegateTextReturn, ModeTestRB2));
            INI.Write("Mainsettings", "typeOfAutoTestPerevalRB", (string)Invoke(DelegateTextReturn, typeOfAutoTestPerevalRB));
            INI.Write("Mainsettings", "typeOfAutoTestEthernetRB", (string)Invoke(DelegateTextReturn, typeOfAutoTestEthernetRB));
            INI.Write("Mainsettings", "typeOfAutoTestPerfomanceTestRB", (string)Invoke(DelegateTextReturn, typeOfAutoTestPerfomanceTestRB));
            INI.Write("Mainsettings", "RandModeCB", (string)Invoke(DelegateTextReturn, randMode));
            INI.Write("Mainsettings", "IndependentModeCB", (string)Invoke(DelegateTextReturn, IndependentMode));
            INI.Write("Connect_Parametres", "Prima_Adress", (string)Invoke(DelegateTextReturn, addressPrima));
            INI.Write("Connect_Parametres", "Prima_Port", (string)Invoke(DelegateTextReturn, portPrima));
            INI.Write("Connect_Parametres", "Critical_Time_Of_Answer", (string)Invoke(DelegateTextReturn, timeOutTB));
            INI.Write("Hand_Test", "MakrosMessages", (string)Invoke(DelegateTextReturn, MakrosTextBox));
            for (int i = 0; i < 8; i++)
            {
                INI.Write("Hand_Test", "ipPaMas" + (i + 1), Convert.ToString(ipPaCheckedMas[i]));
            }
            INI.Write("ChannelsSettings", "Type_Zadachi_Kanala", (string)Invoke(DelegateTextReturn, ipPA));
            INI.Write("ChannelsSettings", "numberOfChannelLastTB_Index", (string)Invoke(DelegateTextReturn, numberOfChannelLastTB));
            INI.Write("ChannelsSettings", "numberOfChannelFirstTB_Index", (string)Invoke(DelegateTextReturn, numberOfChannelFirstTB));
            INI.Write("ChannelsSettings", "zas1Com1Auto", (string)Invoke(DelegateTextReturn, zas1Com1Auto));
            INI.Write("ChannelsSettings", "zas2Com1Auto", (string)Invoke(DelegateTextReturn, zas2Com1Auto));
            INI.Write("ChannelsSettings", "zasFFCom1Auto", (string)Invoke(DelegateTextReturn, zasFFCom1Auto));
            INI.Write("ChannelsSettings", "zas1Com10Auto", (string)Invoke(DelegateTextReturn, zas1Com10Auto));
            INI.Write("ChannelsSettings", "zas2Com10Auto", (string)Invoke(DelegateTextReturn, zas2Com10Auto));
            INI.Write("ChannelsSettings", "zasFFCom10Auto", (string)Invoke(DelegateTextReturn, zasFFCom10Auto));
            CheckBox[] checkboxes = { checkBoxChannel1, checkBoxChannel2, checkBoxChannel3, checkBoxChannel4, checkBoxChannel5, checkBoxChannel6, checkBoxChannel7, checkBoxChannel8 };
            for (int i = 0; i < 8; i++)
            {
                INI.Write("ChannelsSettings", "ActiveChannel " + i, (string)Invoke(DelegateTextReturn, checkboxes[i]));
            }
            INI.Write("ChannelsSettings", "All_ActiveChannel ", (string)Invoke(DelegateTextReturn, checkBoxChannelAll));
            INI.Write("COMMAND_1", "Regim_Peregachi", (string)Invoke(DelegateTextReturn, ProtocoltypeChannel1All));
            INI.Write("COMMAND_1", "Type_Zadachi_Kanala", (string)Invoke(DelegateTextReturn, ipPA));
            RadioButton[] Channels14 = { channel1RadioButton14, channel2RadioButton14, channel3RadioButton14, channel4RadioButton14, channel5RadioButton14, channel6RadioButton14, channel7RadioButton14, channel8RadioButton14 };
            RadioButton[] Channels13 = { channel1RadioButton13, channel2RadioButton13, channel3RadioButton13, channel4RadioButton13, channel5RadioButton13, channel6RadioButton13, channel7RadioButton13, channel8RadioButton13 };
            CheckBox[] checkboxesadr1 = new CheckBox[32] { Com1CB_RS1_1, Com1CB_RS1_2, Com1CB_RS1_3, Com1CB_RS1_4, Com1CB_RS2_1, Com1CB_RS2_2, Com1CB_RS2_3, Com1CB_RS2_4,
                                                       Com1CB_RS3_1, Com1CB_RS3_2, Com1CB_RS3_3, Com1CB_RS3_4, Com1CB_RS4_1, Com1CB_RS4_2, Com1CB_RS4_3, Com1CB_RS4_4,
                                                       Com1CB_RS5_1, Com1CB_RS5_2, Com1CB_RS5_3, Com1CB_RS5_4, Com1CB_RS6_1, Com1CB_RS6_2, Com1CB_RS6_3, Com1CB_RS6_4,
                                                       Com1CB_RS7_1, Com1CB_RS7_2, Com1CB_RS7_3, Com1CB_RS7_4, Com1CB_RS8_1, Com1CB_RS8_2, Com1CB_RS8_3, Com1CB_RS8_4,};
            for (int i = 0; i < checkboxesadr1.Length / 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    INI.Write("COMMAND_1", "Com1CB_RS" + i + "_" + j, (string)Invoke(DelegateTextReturn, checkboxesadr1[i * 4 + j]));
                }
            }
            INI.Write("COMMAND_1", "Kolichestvo_blokov", (string)Invoke(DelegateTextReturn, perevalLengthOfmessage));
            INI.Write("COMMAND_1", "Chaika_Dlina", (string)Invoke(DelegateTextReturn, LengthOfChaikaMes1));
            INI.Write("COMMAND_1", "breakeven_Dlina", (string)Invoke(DelegateTextReturn, LengthOfbreakevenMes1TB));
            INI.Write("COMMAND_1", "Nomer_Vibrannogo_Zasa_1", (string)Invoke(DelegateTextReturn, ZasNomer1RB1));
            INI.Write("COMMAND_1", "Nomer_Vibrannogo_Zasa_2", (string)Invoke(DelegateTextReturn, ZasNomer2RB1));
            INI.Write("COMMAND_1", "Nomer_Vibrannogo_Zasa_FF", (string)Invoke(DelegateTextReturn, ZasFFRB1));
            INI.Write("COMMAND_1", "Neprerivnaya_peredacha", (string)Invoke(DelegateTextReturn, npOnButton1));
            INI.Write("COMMAND_1", "Dlina_Preambuli", (string)Invoke(DelegateTextReturn, DPtextbox1));
            INI.Write("COMMAND_1", "Nomer_Programmy", (string)Invoke(DelegateTextReturn, PRGtextbox1));
            INI.Write("COMMAND_1", "AutoDataCheckBox", (string)Invoke(DelegateTextReturn, DataAutoButton1));
            INI.Write("COMMAND_1", "Message", (string)Invoke(DelegateTextReturn, Message1));
            INI.Write("COMMAND_2", "Nomer_Vibrannogo_Zasa", (string)Invoke(DelegateTextReturn, ZasNomer2RB2));
            INI.Write("COMMAND_2", "Neprerivnaya_peredacha", (string)Invoke(DelegateTextReturn, npOnButton2));
            INI.Write("COMMAND_2", "Regim_Peregachi", (string)Invoke(DelegateTextReturn, ProtocoltypeChannel2All));
            INI.Write("COMMAND_2", "SachMaxDlina", (string)Invoke(DelegateTextReturn, sach2ComboBox));
            INI.Write("COMMAND_2", "LengthcompositeMessage", (string)Invoke(DelegateTextReturn, LengthcompositeMes));
            INI.Write("COMMAND_2", "Sach", (string)Invoke(DelegateTextReturn, sach2TextBox));
            INI.Write("COMMAND_2", "Dlina_Preambuli", (string)Invoke(DelegateTextReturn, DPtextbox2));
            INI.Write("COMMAND_2", "FileName", (string)Invoke(DelegateTextReturn, nfTextBox2));
            INI.Write("COMMAND_2", "NomerProgrammi", (string)Invoke(DelegateTextReturn, prgTextBox2));
            INI.Write("COMMAND_10", "Skorost_Coda", (string)Invoke(DelegateTextReturn, сomboBoxSpeedCode10));
            INI.Write("COMMAND_10", "Interface_Type", (string)Invoke(DelegateTextReturn, interfacetype10));
            INI.Write("COMMAND_10", "Plume", (string)Invoke(DelegateTextReturn, comboBoxPlume10));
            INI.Write("COMMAND_10", "Nomer_Zas1", (string)Invoke(DelegateTextReturn, ZAS1radiobutton10));
            INI.Write("COMMAND_10", "Nomer_Zas2", (string)Invoke(DelegateTextReturn, ZAS2radiobutton10));
            INI.Write("COMMAND_10", "Nomer_ZasFF", (string)Invoke(DelegateTextReturn, ZASFFradiobutton10));
            INI.Write("COMMAND_10", "PPRCh", (string)Invoke(DelegateTextReturn, pprchOn10));
            INI.Write("COMMAND_10", "Tiv", (string)Invoke(DelegateTextReturn, TB_Tiv10));
            INI.Write("COMMAND_10", "Kvitirovanie", (string)Invoke(DelegateTextReturn, kvitOn10));
            INI.Write("COMMAND_11", "Hours", (string)Invoke(DelegateTextReturn, сhchtextbox11));
            INI.Write("COMMAND_11", "Minutes", (string)Invoke(DelegateTextReturn, mmtextbox11));
            INI.Write("COMMAND_11", "Seconds", (string)Invoke(DelegateTextReturn, sstextbox11));
            INI.Write("COMMAND_12", "zas1", (string)Invoke(DelegateTextReturn, zas1Chaika12));
            INI.Write("COMMAND_12", "zas2", (string)Invoke(DelegateTextReturn, zas2Chaika12));
            for (int i = 0; i < 8; i++)
            {
                INI.Write("COMMAND_13", i + "_Channel", (string)Invoke(DelegateTextReturn, Channels13[i]));
            }
            for (int i = 0; i < 8; i++)
            {
                INI.Write("COMMAND_14", i + "_Channel", (string)Invoke(DelegateTextReturn, Channels14[i]));
            }
            INI.Write("COMMAND_15", "OSP", (string)Invoke(DelegateTextReturn, ospOnButton15));
            CheckBox[] Accept_Adresses = new CheckBox[32] {RadioStation1CB, RadioStation2CB, RadioStation3CB, RadioStation4CB, RadioStation5CB, RadioStation6CB,RadioStation7CB, RadioStation8CB,
                RadioStation9CB, RadioStation10CB, RadioStation11CB,RadioStation12CB, RadioStation13CB, RadioStation14CB,RadioStation15CB, RadioStation16CB,
                RadioStation17CB, RadioStation18CB, RadioStation19CB,RadioStation20CB, RadioStation21CB, RadioStation22CB,RadioStation23CB, RadioStation24CB,
                RadioStation25CB, RadioStation26CB, RadioStation27CB,RadioStation28CB, RadioStation29CB, RadioStation30CB,RadioStation31CB, RadioStation32CB};
            for (int i = 0; i < 32; i++)
            {
                INI.Write("COMMAND_16", "Accept_Adresses_" + Convert.ToString(i + 1), (string)Invoke(DelegateTextReturn, Accept_Adresses[i]));
            }
            for (int i = 0; i < 32; i++)
            {
                INI.Write("COMMAND_16", "Rad_IP_" + Convert.ToString(i + 1), (string)Invoke(DelegateTextReturn, Ethernetadresses[2 * i, 0]));
                INI.Write("COMMAND_16", "Rad_P_" + Convert.ToString(i + 1), (string)Invoke(DelegateTextReturn, Ethernetadresses[2 * i, 1]));
                INI.Write("COMMAND_16", "Rad_MAC_" + Convert.ToString(i + 1), (string)Invoke(DelegateTextReturn, Ethernetadresses[2 * i, 2]));
                INI.Write("COMMAND_16", "UU_IP_" + Convert.ToString(i + 1), (string)Invoke(DelegateTextReturn, Ethernetadresses[2 * i + 1, 0]));
                INI.Write("COMMAND_16", "UU_P_" + Convert.ToString(i + 1), (string)Invoke(DelegateTextReturn, Ethernetadresses[2 * i + 1, 1]));
                INI.Write("COMMAND_16", "UU_MAC_" + Convert.ToString(i + 1), (string)Invoke(DelegateTextReturn, Ethernetadresses[2 * i + 1, 2]));
                //Thread.Sleep(1);
            }
            for (int i = 0; i < 32; i++)
            {
                INI.Write("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_Shl_On", (string)Invoke(DelegateTextReturn, EthernetCustomisationRBs[i, 1]));
                INI.Write("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_Zas_1", (string)Invoke(DelegateTextReturn, EthernetCustomisationRBs[i, 2]));
                INI.Write("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_Zas_2", (string)Invoke(DelegateTextReturn, EthernetCustomisationRBs[i, 3]));
                INI.Write("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_Zas_FF", (string)Invoke(DelegateTextReturn, EthernetCustomisationRBs[i, 4]));
                INI.Write("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_Kv_Off", (string)Invoke(DelegateTextReturn, EthernetCustomisationRBs[i, 5]));
                INI.Write("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_Kv_On", (string)Invoke(DelegateTextReturn, EthernetCustomisationRBs[i, 6]));
                INI.Write("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_Kvt_Off", (string)Invoke(DelegateTextReturn, EthernetCustomisationRBs[i, 7]));
                INI.Write("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_Kvt_On", (string)Invoke(DelegateTextReturn, EthernetCustomisationRBs[i, 8]));
                //Thread.Sleep(1);
            }
            for (int i = 0; i < 32; i++)
            {
                INI.Write("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_KP", (string)Invoke(DelegateTextReturn, EthernetCustomisationTBs[i, 0])); ;
                INI.Write("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_KPP", (string)Invoke(DelegateTextReturn, EthernetCustomisationTBs[i, 1]));
                INI.Write("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_RD", (string)Invoke(DelegateTextReturn, EthernetCustomisationTBs[i, 2]));
                INI.Write("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_Tiv", (string)Invoke(DelegateTextReturn, EthernetCustomisationTBs[i, 3]));
                //Thread.Sleep(1);
            }
            for (int i = 0; i < 32; i++)
            {
                INI.Write("COMMAND_16", "Rad_" + Convert.ToString(i + 1) + "_SC", (string)Invoke(DelegateTextReturn, EthernetCustomisationComboBoxes[i]));

            }


            INI.Write("COMMAND_20", "VDS", (string)Invoke(DelegateTextReturn, vdsinradiobutton20));
            RecordToRTB(Color.SteelBlue, "\nКонфигурации сохранены.");
        }
        #endregion
        #endregion
        private void ТестКонтроляСоединенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrimaRTB.Clear();
            if (Init())
            {
                int timeoutForConnect = 100;
                ConnectionCheckForm CCF = new ConnectionCheckForm();
                Thread CCFBox = new Thread(() => CCF.ShowDialog());
                CCFBox.Start();
                while (CCFBox.ThreadState != System.Threading.ThreadState.Stopped)
                    Thread.Sleep(1);
                if (CCF.Start)
                {
                    timeoutForConnect = CCF.timeOutLength;
                }
                startTest = new Thread(() => ConnectionCheckTest(timeoutForConnect));
                startTest.Start();
            }
        }
        private void ConnectionCheckTest(int timeout)
        {
            //mode = true;
            Stopwatch timer = new Stopwatch();
            timer.Restart();
            try
            {
                while (true)
                {
                    if (timer.ElapsedMilliseconds >= timeout)
                    {
                        NewgeneratingMessage255(true, timeout);
                        timer.Restart();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //MessageBox.Show(ex.ToString());
            }
        }
        private void PrimaRTB_MouseUp(object sender, MouseEventArgs e)
        {
            startPointOfFindInPrimaRTB = PrimaRTB.SelectionStart;
        }

        private void MaxNumberOfLinesTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8)
            {
                e.Handled = true;
            }
        }


    }
}