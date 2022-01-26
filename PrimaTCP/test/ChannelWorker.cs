using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test
{
    class ChannelWorker : Form
    {
        bool readyForNextMessage = true;
        readonly Random rand = new Random();
        readonly int _numberOfMyPort;//добавил модификаторы для чтения
        readonly int _myRp;
        readonly int _NumberOfMyChannel;
        readonly BCVK_Client_MainForm _mainForm;
        IPEndPoint ipPoint;
        Socket listenSocket;
        bool Stop = false;
        List<byte[]> ConvertedMessagesList = new List<byte[]>();
        bool[] check1MessageMas;
        int countOf3rdkvit = 0;
        public ChannelWorker(int numberOfPort, int rp, int numberofchannel, BCVK_Client_MainForm mainForm)
        {
            _numberOfMyPort = numberOfPort;
            _myRp = rp;
            _NumberOfMyChannel = numberofchannel;
            _mainForm = mainForm;
            Task.Factory.StartNew(() => TcpListener());
        }
        private void TcpListener()
        {
            ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _numberOfMyPort);
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listenSocket.Bind(ipPoint);
                listenSocket.Listen(10);
                while (!Stop)
                {
                    Socket handler = listenSocket.Accept();
                    byte[] message;
                    int bytes = 0;
                    byte[] data = new byte[4096];
                    do
                    {
                        bytes = handler.Receive(data);
                        message = new byte[bytes];
                        for (int i = 0; i < bytes; i++)
                        {
                            message[i] = data[i];
                        }
                    } while (handler.Available > 0);
                    if (message.Length > 0)
                    {
                        Task.Factory.StartNew(() => TcpDivider(message));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private void TcpDivider(byte[] message)
        {
            int numberOfMessages = 0;
            byte[] submessage = new byte[1];
            if (_myRp == 5 || _myRp == 6)
            {
                numberOfMessages = message.Length / 112;
                if (message.Length % 112 > 0)
                {
                    numberOfMessages++;
                }
            }
            for (int i = 0; i < numberOfMessages - 1; i++)
            {
                submessage = new byte[112];
                for (int j = 0; j < 112; j++)
                {
                    submessage[j] = message[i * 112 + j];
                }
                ConvertedMessagesList.Add(NewgeneratingMessage1(submessage));
            }
            submessage = new byte[message.Length - ((numberOfMessages - 1) * 112)];
            for (int j = 0; j < message.Length - ((numberOfMessages - 1) * 112); j++)
            {
                submessage[j] = message[((numberOfMessages - 1) * 112) + j];
            }
            ConvertedMessagesList.Add(NewgeneratingMessage1(submessage));
            countOf3rdkvit = numberOfMessages;
            check1MessageMas = new bool[numberOfMessages];
            TcpReSender();
            if (CheckingForFullAnswer())
            {
                TcpAnswerBack(message);
            }
        }
        private bool CheckingForFullAnswer()
        {
            Stopwatch timeForChecking = new Stopwatch();
            timeForChecking.Start();
            bool ready = false;
            while (timeForChecking.ElapsedMilliseconds > 4000)
            {
                if (countOf3rdkvit == 0)
                {
                    ready = true;
                    foreach (bool checkMes in check1MessageMas)
                    {
                        if (!checkMes)
                        {
                            ready = false;
                            break;
                        }
                    }
                    if (ready)
                    {
                        return true;
                    }
                }
            }
            MessageBox.Show("По каналу:" + _NumberOfMyChannel + "не пришли все квитанции или подтверждения");
            return false;
        }
        public void Message3recieved()
        {
            countOf3rdkvit--;
        }
        public void Message1recieved(byte[] message)
        {
            for (int i = 0; i < ConvertedMessagesList.Count; i++)
            {
                if (ConvertedMessagesList[i].Equals(message))
                {
                    check1MessageMas[i] = true;
                    return;
                }
            }
        }
        private void TcpAnswerBack(byte[] message)
        {
            listenSocket.Send(message);
        }
        private void TcpReSender()
        {
            foreach (byte[] mes in ConvertedMessagesList)
            {
                mes.CopyTo(_mainForm.incomeData, 0);
                readyForNextMessage = false;
                _mainForm.messageReady[_NumberOfMyChannel] = true;
                while (!readyForNextMessage && !Stop)
                {
                    Thread.Sleep(10);
                }
            }
        }
        public byte[] NewgeneratingMessage1( byte[] data)
        {
            byte[] cmd = new byte[16] { 0x23, 0x0, 0x0, 0x0, 0x0, 0x01, 0, 0, 0, 0, Convert.ToByte(rand.Next(0,2)), Convert.ToByte(rand.Next(0, 8)),
                Convert.ToByte(rand.Next(5, 7)), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(Math.Pow(2, _NumberOfMyChannel)) };
            cmd = Combine(cmd, data);
            BitConverter.GetBytes(cmd.Length - 5).CopyTo(cmd, 1);
            return cmd;
        }
        private byte[] Combine(byte[] mas1, byte[] mas2)
        {
            byte[] Resultmas = new byte[mas1.Length + mas2.Length];
            mas1.CopyTo(Resultmas, 0);
            mas2.CopyTo(Resultmas, mas1.Length);
            return Resultmas;
        }
        internal void StopChannel()
        {
            Stop = true;
        }
        internal void ReadyForNextMessage()
        {
            readyForNextMessage = true;
        }
    }
}


