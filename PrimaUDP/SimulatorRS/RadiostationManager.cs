using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Collections.Concurrent;

namespace SimulatorRS
{
    class RadiostationManager
    {
        //int lengthOfmessage;
        //static object locker = new object();
        static Stopwatch stopwatch = new Stopwatch();
        public functionUDPPrima function = new functionUDPPrima();
        Stopwatch BytePerSecondStopwatch = new Stopwatch();
        UdpClient udpSocket;
        UURadiostation UU;
        ConcurrentBag<byte[]> RecievedMessageList = new ConcurrentBag<byte[]>();
        static CancellationTokenSource StopRsWork = new CancellationTokenSource();
        static CancellationToken StopRsWorkToken = StopRsWork.Token;
        //Thread noiseGenerator;
        // List<byte[]> RecievedKvitList = new List<byte[]>();
    
        List<short> RepeatedIns = new List<short>();
        List<short> RepeatedNp = new List<short>();
        SemaphoreSlim MessageSendingToRS = new SemaphoreSlim(1, 1);
        SemaphoreSlim MessageSendingToChannel = new SemaphoreSlim(1, 1);
        SemaphoreSlim Return0MessageToPrima = new SemaphoreSlim(1, 1);
        SemaphoreSlim CheckFor0MessageFromPrima = new SemaphoreSlim(1, 1);
        SemaphoreSlim GeneratorOfNoise = new SemaphoreSlim(0, 1);
        int kpInner = 0;
        ConcurrentQueue<byte[]> DataMessagesForRs = new ConcurrentQueue<byte[]>();
        private bool otkrivaskaDliaStartingStateOfBuffer;
        private bool UUSeparated = false;
        Stopwatch stopwatchNoMessage = new Stopwatch();
        bool ListenerType = false;
        public static bool stop = false;
        
        bool emittingOn = false;
        static public string _ipAdressPrima = "";
        static public int _portPrima;
        int _RSnumber;

        IPEndPoint remoteIp = null;
  
        Int16[] recievedmessage = new Int16[1472];
        MainForm _mainForm;
        int _timeOfSleep;
        int _port;
        bool moreThan2PackagesInList = true;
        bool isDotesAvailable = false;
        delegate void TextToChat(string message);
        LogManager _logManager;

        double dotesPerMilliseconds;
        
        public int maxCountOfZeroMessages = 2;//необходимое количество нулевых комманд (Данных) в радиостанции для начала ее работы
        bool kvitrecieved = false;
        byte[][] masOfMessagesForNoise = new byte[3][];
        int[] timeOnMessageForNoise = new int[3];
        #region параметры радиостанции
        bool shl;
        bool nz;
        int kp = 1;
        int kpp = 1;
        int rd = 1432;
        bool kv;
        #endregion
        int kpCurrent = 0;
        Stopwatch Broadcast = new Stopwatch();
        private short lastNumberOfPackage = 0;
        int timeForBroadcast = 0;
        int timer = 200;
        public RadiostationManager(int port, int portUU, int RsNumber,  MainForm mainForm, LogManager logManager)
        {
            udpSocket = new UdpClient(port);
            udpSocket.Ttl = 50;
            if (port != portUU)
            {
                UU = new UURadiostation(portUU, this, function);
                UUSeparated = true;
            }
            _port = port;
            _mainForm = mainForm;
            _logManager = logManager;
            
            _RSnumber = RsNumber;
            _timeOfSleep = Convert.ToInt32(mainForm.sleepTime.Text);
            
        }
       static public void reset()
        {
            stop = false;
            StopRsWork=new CancellationTokenSource();
            StopRsWorkToken = StopRsWork.Token;
        }
        private void StartingStateOfBuffer()
        {
            //Stopwatch startingStateOfBufferSW = new Stopwatch();
            //startingStateOfBufferSW.Start();


                while (RecievedMessageList.Count < maxCountOfZeroMessages && !stop)
                {
                    Thread.Sleep(1);
                    //if (startingStateOfBufferSW.ElapsedMilliseconds > 500)
                    //{
                    //    Console.WriteLine(startingStateOfBufferSW.ElapsedMilliseconds.ToString());
                    //}
                }
                moreThan2PackagesInList = true;
                otkrivaskaDliaStartingStateOfBuffer = true;
                
                //startingStateOfBufferSW.Reset();
            

        }

        public void SendOneMessage(byte[] mas = null)
        {
            //if (mas == null)
            //{
            //    byte[] sendedMessage = function.generatingMessage4(0);

            //    // SendMessage(sendedMessage);
            //    StringBuilder sb = new StringBuilder("Точки, длинной в " + Convert.ToString(sendedMessage.Length));
            //    _logManager.ToChatRTB(Convert.ToString(stopwatch.Elapsed), _RSnumber + 1, Convert.ToString(sb));
            //}
            //else
            {
                byte[] sendedMessage = function.generatingMessage0(1, 1, mas);
                // new Socket_Message(this.udpSocket, sendedMessage);
                SendMessage(sendedMessage);
                //function.messagenumber++;
                _logManager.ToChatRTB(Convert.ToString(stopwatch.Elapsed), _RSnumber + 1, BitConverter.ToString(sendedMessage));
            }
        }
        public void SendOneMessage(int numberOfCommand, short options)
        {
            switch (numberOfCommand)
            {
                case 5:
                    Task.Factory.StartNew(() => generatingNextKvitMessage(5, lastNumberOfPackage, options));
                    break;
                default:
                    return;
            }
        }

        public void Sender()
        {///Функция отвечает за отправку сообщений
         ///Разделена на две части 
          
            byte[] currentMessage;
            byte[] savedMessage0 = new byte[0];
            byte[] data;
            bool noiseActive = false;
            Stopwatch timeForResetNumbers = new Stopwatch();
            //short numberOfPackage = 0;
            
                otkrivaskaDliaStartingStateOfBuffer = true;
            Stopwatch noiseSW = new Stopwatch();
            try
            {
                while (!stop)
                {
                    StopRsWorkToken.ThrowIfCancellationRequested();
                    try
                    {
                        if (RecievedMessageList.Count == 0 && Broadcast.ElapsedMilliseconds == 0)///первая часть отвечает за генерацию шума ( при отсутствии сообщений в канале)
                        {

                            timeForResetNumbers.Start();
                            if (noise && !kvitrecieved && MessageSendingToRS.CurrentCount == 1 && Return0MessageToPrima.CurrentCount == 1 && !emittingOn && noiseActive)
                            {
                                try
                                {
                                    if (noiseWithNulls)
                                    {

                                        data = function.generatingMessage0(1, rd, true);
                                    }
                                    else
                                    {
                                        data = function.generatingMessage0(1, rd);
                                    }
                                    //NoiseIsSilenced = false;
                                    for (int i = 0; i < kpp; i++)
                                    {
                                        //new Socket_Message(this.udpSocket, data);
                                        SendMessage(data);
                                        noiseSW.Restart();
                                        MessageSendingToChannel.Wait();
                                        _logManager.ToChatRTB(Convert.ToString(stopwatch.Elapsed), _RSnumber + 1, "Ушел шум:  " + BitConverter.ToString(data));
                                        //Thread.Sleep(timer);
                                        if (!checkforkvit(timer))
                                        {
                                            noiseSW.Reset();
                                            TimeOutToChatRTB("Нет ответа на шум  " + BitConverter.ToString(data));
                                            if (i == kpp - 1)
                                            {
                                                CreateError();

                                                break;
                                            }
                                            continue;
                                        }
                                        while (noiseSW.ElapsedMilliseconds < timer)
                                        {
                                            Thread.Sleep(1);
                                        }
                                        break;
                                        // CreateError();
                                    }
                                    if (timeForResetNumbers.ElapsedMilliseconds >= 5000)
                                    {
                                        //function.messagenumber = 0;
                                        noiseActive = false;
                                        timeForResetNumbers.Reset();
                                        ResetCheckState();
                                    }

                                    //NoiseIsSilenced = true;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                            else
                            {

                                if (otkrivaskaDliaStartingStateOfBuffer)
                                {
                                    moreThan2PackagesInList = false;
                                    otkrivaskaDliaStartingStateOfBuffer = false;
                                    Task.Factory.StartNew(() => StartingStateOfBuffer(), StopRsWorkToken);
                                }
                                //if (!moreThan2PackagesInList)
                                //{
                                //    //moreThan2PackagesInList = false;

                                //    Task.Factory.StartNew(() => StartingStateOfBuffer(), StopRsWorkToken);
                                //}
                                else if (timeForResetNumbers.ElapsedMilliseconds >= 5000)
                                {
                                    //function.messagenumber = 0;
                                    noiseActive = false;
                                    timeForResetNumbers.Reset();
                                    ResetCheckState();
                                }
                                else
                                {
                                    Thread.Sleep(1);
                                }
                            }

                        }
                        else if (RecievedMessageList.Count > 0 && moreThan2PackagesInList)///Вторая часть отвечает за отлов полученных сообщений и запуск задачи иммитациия сообщений (4-6-0)
                        {
                            byte[] kvitMessage3 = function.generatingMessage3();
                            // new Socket_Message(this.udpSocket, kvitMessage3);
                            SendMessage(kvitMessage3);
                            _logManager.ToChatRTB(Convert.ToString(stopwatch.Elapsed), _RSnumber + 1, BitConverter.ToString(kvitMessage3));

                            timeForResetNumbers.Reset();
                            noiseActive = true;
                            RecievedMessageList.TryTake(out currentMessage);
                            if (currentMessage != null)
                            {
                                MessageSendingToRS.Wait();
                                byte[] TempBuf = new byte[currentMessage.Length];//создаю временный буфер для отправление сообщения в таск
                                                                                 //в противном случае может случится подмена сообщения (0-вая команда)
                                                                                 //на другую (в следствие адресации памяти)
                                Array.Copy(currentMessage, TempBuf, currentMessage.Length);
                                //byte[] messageUnderWorking = new byte[];

                                Task.Factory.StartNew(() => MessageSendImmitator(TempBuf), StopRsWorkToken);
                            }
                            //}

                        }
                        else
                        {
                            Thread.Sleep(1);
                        }
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                ResetCheckState();
                timeForResetNumbers.Reset();
            }

        }
        private void ResetCheckState()
        {
            RepeatedIns.Clear();
            RepeatedNp.Clear();

        }
        Stopwatch stopwatchOnkvit = new Stopwatch();
        internal static bool noise = true;
        public bool noiseWithNulls { get; internal set; }
        bool checkforkvit(int timeOut = 50)
        {
            stopwatchOnkvit.Restart();
            //Thread.Sleep(timer)
            while (stopwatchOnkvit.ElapsedMilliseconds < timeOut) 
            {
                if (kvitrecieved)
                {
                    kvitrecieved = false;
                    //stopwatchOnkvit.Stop();

                    MessageSendingToChannel.Release();
                    return true;

                }
                Thread.Sleep(1);
            }
            stopwatchOnkvit.Stop();
            //kvitrecieved = false;
            _mainForm.ChangeNumberOfRepeats(_RSnumber);
            _logManager.ToChatRTB(Convert.ToString(stopwatch.Elapsed), _RSnumber + 1, "timeout kvit:" + stopwatchOnkvit.ElapsedMilliseconds.ToString());
            MessageSendingToChannel.Release();
            return false;

        }
        private void CreateError()
        {
            //stop = true;
            StopRsWork.Cancel();
            _mainForm.ModeLogManagerCBHandChangerMaster(0);
            _mainForm.ModeLogManagerCBHandChangerMaster(_RSnumber + 1);
        }


        private void MessageSendImmitator(byte[] message)
        {
            short numberOfPackage;

            byte[] savedMessage0 = new byte[message.Length];
            Array.Copy(message, savedMessage0, message.Length);
            //savedMessage0 = function.generatingMessage0(savedMessage0);
            byte[] sendedMessage;
            try
            {
                numberOfPackage = BitConverter.ToInt16(savedMessage0, 3);
                lastNumberOfPackage = numberOfPackage;
                timeForBroadcast = Convert.ToInt32(savedMessage0.Length / dotesPerMilliseconds);
                if (ListenerType)
                {
                    while ((/*!NoiseIsSilenced || */MessageSendingToChannel.CurrentCount == 0 /*|| kvitrecieved*/) && !stop)
                    {
                        Thread.Sleep(1);
                    }
                    Broadcast.Restart();
                    generatingNextKvitMessage(4, numberOfPackage);
                    while (Broadcast.ElapsedMilliseconds < timeForBroadcast)
                    {
                        Thread.Sleep(1);
                    }
                    // Thread.Sleep(timeForBroadcast);
                    Broadcast.Reset();
                    //Thread.Sleep(0);//это команда означает, что я могу перейти к другому потоку, если необходимо
                    generatingNextKvitMessage(6, numberOfPackage);
                    Return0MessageToPrima.Wait();
                    MessageSendingToRS.Release();
                    if (!stop)
                    {
                        generatingNextKvitMessage(0, numberOfPackage, 0, savedMessage0);
                    }
                    Return0MessageToPrima.Release();
                }
                else
                {
                    Broadcast.Start();
                    sendedMessage = function.generatingMessage4(numberOfPackage);
                    sendRepeate_NoListenerTypeMessage(ref sendedMessage);
                    while (Broadcast.ElapsedMilliseconds < timeForBroadcast)
                    {
                        Thread.Sleep(1);
                    }
                    Broadcast.Reset();
                    sendedMessage = function.generatingMessage6(numberOfPackage);
                    sendRepeate_NoListenerTypeMessage(ref sendedMessage);
                    MessageSendingToRS.Release();
                    Return0MessageToPrima.Wait();
                    sendedMessage = function.generatingMessage0(savedMessage0);
                    sendRepeate_NoListenerTypeMessage(ref sendedMessage);
                    Return0MessageToPrima.Release();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void sendRepeate_NoListenerTypeMessage(ref byte[] sendedMessage)
        {
            SendMessage(sendedMessage);
            for (int i = 0; i < kp - 1; i++)
            {
                sendedMessage = function.setMessageNumber(sendedMessage);
                function.messagenumber++;
                SendMessage(sendedMessage);
                _logManager.ToChatRTB(Convert.ToString(stopwatch.Elapsed), _RSnumber + 1, BitConverter.ToString(sendedMessage));
            }
        }

        private void generatingNextKvitMessage(int numberOfCommand, short numberOfPackage, short options = 0, byte[] data = null)
        {
            //byte[] savedData = new byte[data.Length];
            //Array.Copy(data, savedData, data.Length);
            byte[] sendedMessage = new byte[0];
            switch (numberOfCommand)
            {
                case 4: sendedMessage = function.generatingMessage4(numberOfPackage); break;
                case 6: sendedMessage = function.generatingMessage6(numberOfPackage); break;
                case 0: sendedMessage = function.generatingMessage0(data); function.packagenumber++; break;
                case 5: sendedMessage = function.generatingMessage5(numberOfPackage, options); break;
            }
            MessageSendingToChannel.Wait();
            //new Socket_Message(this.udpSocket, sendedMessage);
            SendMessage(sendedMessage);            
            _logManager.ToChatRTB(Convert.ToString(stopwatch.Elapsed), _RSnumber + 1, BitConverter.ToString(sendedMessage));
            if (checkforkvit())
            {
                return;
            }
            TimeOutToChatRTB("Нет ответа на команду " + numberOfCommand + "  " + BitConverter.ToString(sendedMessage));
            _mainForm.ChangeNumberOfRepeats(_RSnumber);
            for (int i = 0; i < kpp-1; i++)
            {
                sendedMessage = function.setMessageNumber(sendedMessage);
                function.messagenumber++;
                MessageSendingToChannel.Wait();
                SendMessage(sendedMessage);
                
                _logManager.ToChatRTB(Convert.ToString(stopwatch.Elapsed), _RSnumber + 1, BitConverter.ToString(sendedMessage));

                if (checkforkvit())
                {
                    return;
                }
                TimeOutToChatRTB("Нет ответа на команду " + numberOfCommand + "  " + BitConverter.ToString(sendedMessage));
                _mainForm.ChangeNumberOfRepeats(_RSnumber);
                // CreateError();
            }
            CreateError();
        }
        public void Listen()
        {
            StringBuilder sb;
            ListenerType = _mainForm.ListenerTypeFullAnswer.Checked;
            string writePath = MainForm.FolderPath + "\\savedMessages\\" + _RSnumber + 1.ToString() + ".txt";
            byte[] kvitMessage3 = new byte[0];
            byte[] data = new byte[1472];

            Thread sender = new Thread(() => Sender());
            //sender.Priority = ThreadPriority.AboveNormal; 
            sender.Start();
            int x = 0;
            while (!stop)
            {
                try
                {
                    if (udpSocket.Available > 0)
                    {
                        data = udpSocket.Receive(ref remoteIp); // получаем данные

                        if (data.Length > 0)
                        {
                            if (!(_mainForm.messageForRecord && data[0] == 0))
                            {

                                _logManager.ReturnFromNetToChatRTB(Convert.ToString(stopwatch.Elapsed), _RSnumber + 1, BitConverter.ToString(data));

                                if (data[0] == 16)
                                {
                                    shl = Convert.ToBoolean(data[1]);
                                    nz = Convert.ToBoolean(data[2]);
                                    kp = Convert.ToInt32(data[3]);
                                    kpp = Convert.ToInt32(data[4]);
                                    rd = Convert.ToInt32(data[5]) + Convert.ToInt32(data[6]) * 256;
                                    
                                    ListenerType = kv = Convert.ToBoolean(data[7]);
                                    SpeedSet(data[8]);
                                    _mainForm.offScreen = Convert.ToBoolean(data[9]);
                                    _logManager.ToChatRTB(Convert.ToString(stopwatch.Elapsed), _RSnumber + 1, BitConverter.ToString(data));
                                    udpSocket.Send(data, data.Length, remoteIp);
                                    RepeatedIns.Clear();
                                    RepeatedNp.Clear();
                                    stopwatch.Restart();
                                }
                                else if (!isInsRepeat(BitConverter.ToInt16(data, 1)))
                                {
                                    if (kp > 1 && data[0] == 0)
                                    {
                                        //_logManager.timesofRepeats(_RSnumber + 1, Convert.ToString(kpCurrent + 1));
                                    }
                                    if (data[0] == 3 && ListenerType)
                                    {
                                        //    while (kvitrecieved)
                                        //    {

                                        //        Thread.Sleep(1);
                                        //    }
                                        stopwatchOnkvit.Stop();
                                        kvitrecieved = true;
                                    }
                                    else if (data[0] == 1)
                                    {
                                        StopRsWork.Cancel();
                                        emittingOn = false;
                                        RecievedMessageList = new ConcurrentBag<byte[]>();
                                        data = function.generatingMessage7();
                                        //new Socket_Message(this.udpSocket, data);
                                        SendMessage(data);

                                        _logManager.ToChatRTB(Convert.ToString(stopwatch.Elapsed), _RSnumber + 1, BitConverter.ToString(data));
                                        StopRsWork = new CancellationTokenSource();
                                        StopRsWorkToken = StopRsWork.Token;
                                    }
                                    else if (!emittingOn)
                                    {
                                        if (data[0] == 2 && !UUSeparated)
                                        {
                                            data = function.generatingMessage2(data[3]);
                                            //new Socket_Message(this.udpSocket, data);
                                            SendMessage(data);

                                            _logManager.ToChatRTB(Convert.ToString(stopwatch.Elapsed), _RSnumber + 1, BitConverter.ToString(data));
                                            if (data[3] == 1)
                                            {
                                                emittingOn = true;
                                            }
                                            else if (data[3] == 2)
                                            {
                                                emittingOn = false;
                                            }
                                        }
                                        else
                                        {
                                            errorMessageWithoutEmitting();
                                            CreateError();
                                        }
                                    }
                                    else
                                    {
                                        if (data[0] == 0)
                                        {
                                            if (ListenerType)
                                            {
                                            }
                                            if (!isNpRepeat(BitConverter.ToInt16(data, 3)))
                                            {
                                                if (!(data.Length > rd + 7))//7 - шапка
                                                {

                                                    RecievedMessageList.Add(data);
                                                }
                                                else
                                                {
                                                    errorMessageOversizedData();
                                                }
                                                //using (FileStream fstream = new FileStream(writePath, FileMode.OpenOrCreate))
                                                //{
                                                //    fstream.Write(data, 7, data.Length - 7);
                                                //}
                                            }
                                        }
                                        else if (data[0] == 2 && !UUSeparated)
                                        {
                                            data = function.generatingMessage2(data[3]);
                                            if (data[3] == 2)
                                            {
                                                emittingOn = false;
                                            }
                                            //new Socket_Message(this.udpSocket, data);
                                            SendMessage(data);
                                            _logManager.ToChatRTB(Convert.ToString(stopwatch.Elapsed), _RSnumber + 1, BitConverter.ToString(data));
                                        }

                                    }
                                }
                                else if (ListenerType)
                                {
                                    CreateError();
                                    errorRepeateMessage();
                                }
                            }
                            else
                            {
                                sb = new StringBuilder("Записано сообщение:   " + BitConverter.ToString(data));
                                _logManager.ReturnFromNetToChatRTB(Convert.ToString(stopwatch.Elapsed), _RSnumber + 1, Convert.ToString(sb));
                                using (FileStream fstream = new FileStream(writePath, FileMode.OpenOrCreate))
                                {
                                    fstream.Write(data, 7, data.Length - 7);
                                }
                            }
                        }
                        else
                        {
                            _logManager.ReturnFromNetToChatRTB(Convert.ToString(stopwatch.Elapsed), _RSnumber + 1, BitConverter.ToString(data));
                        }
                        //Thread.Sleep(0);
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }
                catch (NullReferenceException Exception)
                {
                    /// Console.WriteLine(Exception.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString() + BitConverter.ToString(data));
                }
                finally
                {
                    //Thread.Sleep(1);
                }

            }
        }

        private void SpeedSet(byte timeMode)
        {
            int speed = 0; ;
            switch(timeMode)
            {
                case 0:
                    speed = 75;
                    break;
                case 1:
                    speed = 150;
                    break;
                case 2:
                    speed = 300;
                    break;
                case 3:
                    speed = 1200;
                    break;
                case 4:
                    speed = 2400;
                    break;
                case 5:
                    speed = 4800;
                    break;
                case 6:
                    speed = 9600;
                    break;
                case 7:
                default:
                    speed = 16000;
                    break;
                case 8:
                    speed = 50;
                    break;
                case 9:
                    speed = 100;
                    break;
            }
            dotesPerMilliseconds = speed / 8000.0;//отправка байт в секунду
            timer = (int)Math.Round((rd + 7) / dotesPerMilliseconds, MidpointRounding.AwayFromZero);
        }

        public void SendMessage(byte[] data)
        {
            udpSocket.Send(data, data.Length, _ipAdressPrima, _portPrima);

        }
        public void TimeOutToChatRTB(string message)
        {
            _logManager.TimeOutToChatRTB(Convert.ToString(stopwatch.Elapsed), _RSnumber + 1, message);
        }
        private void errorRepeateMessage()
        {
            _logManager.ReturnFromNetToChatRTB(Convert.ToString(stopwatch.Elapsed), _RSnumber + 1, "!!!!!!!!!!ПОВТОР!!!!!!!!!!");
        }
        private void errorRepeatePackage()
        {
            _logManager.ReturnFromNetToChatRTB(Convert.ToString(stopwatch.Elapsed), _RSnumber + 1, "!!!!!!!!!!ПОВТОРНЫЙ ПАКЕТ!!!!!!!!!!");
        }
        private void errorUnexpectedMessage()
        {
            _logManager.ReturnFromNetToChatRTB(Convert.ToString(stopwatch.Elapsed), _RSnumber + 1, "!!!!!!!!!!СООБЩЕНИЕ НЕВПОПАД!!!!!!!!!!");
        }
        private void errorMessageWithoutEmitting()
        {
            _logManager.ReturnFromNetToChatRTB(Convert.ToString(stopwatch.Elapsed), _RSnumber + 1, "!!!!!!!!!!СООБЩЕНИЕ ДО ВКЛЮЧЕНИЯ ИЗЛУЧЕНИЯ!!!!!!!!!!");
        }
        private void errorMessageOversizedData()
        {
            _logManager.ReturnFromNetToChatRTB(Convert.ToString(stopwatch.Elapsed), _RSnumber + 1, "!!!!!!!!!!ПРЕВЫШЕН РАЗМЕР ПАКЕТА!!!!!!!!!!");
        }
        public void ChangeListenerType(bool type)
        {
            ListenerType = type;

        }
        public void StopListener()
        {
            SemaphoreSlim[] SSMas = new SemaphoreSlim[4] { MessageSendingToRS, MessageSendingToChannel, Return0MessageToPrima, GeneratorOfNoise };
            Thread.Sleep(40);
            CloseUdpSocket();
            foreach (SemaphoreSlim ss in SSMas)
            {
                try
                {
                    ss.Release();
                    ss.Dispose();
                }
                catch { }
            }
        }
        public void DotesState(bool state)
        {
            isDotesAvailable = state;
        }
        private void CloseUdpSocket()
        {
            if (UU != null)
            {
                UU.CloseSocket();
            }
            if (udpSocket != null)
            {
                udpSocket.Close();
            }
        }
        public bool isInsRepeat(short INS)
        {
            if (RepeatedIns.Count > 0)//Если в списке номеров сообщений есть значения
            {
                foreach (short repIns in RepeatedIns)//То новое значение сравнивается со всем списком
                {
                    if (INS.Equals(repIns))//Если оно совпадает с каким либо - значит повтор
                    {
                        kpInner++;
                        if (kpInner > kp)//Если повторов больше чем установенное допустимое КП
                        {
                            errorRepeateMessage();
                        }
                        return true;
                    }
                }
                RepeatedIns.Insert(0, INS);//Если повторов нет - то значение добавляется в список
                if (RepeatedIns.Count > 10)//и список обрезается при предельном количестве значений
                {
                    RepeatedIns.RemoveAt(10);
                }
                kpCurrent = kpInner;
                kpInner = 0;
                return false;
            }
            else
            {
                RepeatedIns.Add(INS);//Если список пустой - просто добавляется
                kpInner++;

                return false;
            }
        }
        public void emittingSet(byte[] recievedMessage, byte[] SendedMessage, bool state)
        {
            _logManager.ReturnFromNetToChatRTB(Convert.ToString(stopwatch.Elapsed), _RSnumber + 1, "УУ " + BitConverter.ToString(recievedMessage));
            emittingOn = state;
            _logManager.ToChatRTB(Convert.ToString(stopwatch.Elapsed), _RSnumber + 1, "УУ " + BitConverter.ToString(SendedMessage));
        }
        public bool isNpRepeat(short RP)
        {
            if (RepeatedNp.Count > 0)//Если в списке пакетов сообщений есть значения
            {
                foreach (short repRp in RepeatedNp)//То новое значение сравнивается со всем списком
                {
                    if (RP.Equals(repRp))//Если оно совпадает с каким либо - значит повтор
                    {
                        if (ListenerType)
                        {
                            errorRepeatePackage();
                            //CreateError();
                        }

                        return true;
                    }
                }
                RepeatedNp.Insert(0, RP);//Если повторов нет - то значение добавляется в список
                if (RepeatedNp.Count > 5)//и список обрезается при предельном количестве значений
                {
                    RepeatedNp.RemoveAt(5);
                }
                return false;
            }
            else
            {
                RepeatedNp.Add(RP);//Если список пустой - просто добавляется
                return false;
            }
        }
    }
}

