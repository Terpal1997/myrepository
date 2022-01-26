using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SodWinForms
{
    class NetWorker
    {
        private int _channel1;
        private int _channel2;
        private int _channel1Send;//Канал на который будет отправлять полученные сообщения канал 1 
        private int _channel2Send;//Канал на который будет отправлять полученные сообщения канал 2 
        public bool channel1NoAnswer = false;
        public bool channel2NoAnswer = false;
        public bool channel1NoSave = true;
        public bool channel2NoSave = true;
        Corrupter corrupter1;
        Corrupter corrupter2;
        public XiskProcedure xisk;
        public bool channel2XiskProcedure = false;
        private System.Net.IPAddress ipAddress;
        private System.Net.IPEndPoint ipEndPoint;
        Socket socket;
        public bool connect;
        bool readyForNextMessage = false;
        bool accepted = false;
        Stopwatch stopwatch = new Stopwatch();
        byte[] message10parametres = new byte[3];
        MainWindow mainWindow;
        public NetWorker(string ipAddr, int Port, int Channel1, int Channel1Send, Corrupter Corrupter1, int Channel2, int Channel2Send, Corrupter Corrupter2, MainWindow Window)
        {
            this.ipAddress = IPAddress.Parse(ipAddr);
            this.ipEndPoint = new System.Net.IPEndPoint(ipAddress, Port);
            _channel1 = Channel1;
            _channel1Send = Channel1Send;
            corrupter1 = Corrupter1;
            _channel2 = Channel2;
            _channel2Send = Channel2Send;
            corrupter2 = Corrupter2;
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            accepted = false;
            mainWindow = Window;
            readyForNextMessage = true;
        }
        public void SendFromFile(string adress, int numberOfChannel)
        {
            byte[] data;
            int channel = 0;
            int channelSend = 0;
            Corrupter corrupter = null;
            if (numberOfChannel == 1)
            {
                channel = _channel1;
                channelSend = _channel1Send;
                corrupter = corrupter1;
            }
            else if (numberOfChannel == 2)
            {
                channel = _channel2;
                channelSend = _channel2Send;
                corrupter = corrupter2;
            }
            string[] str_s;
            try
            {
                using (StreamReader sr = new StreamReader(adress))
                {
                    str_s = sr.ReadToEnd().Split('\n');
                }
                if (!acceptedCheck(channel)) { return; }
                foreach (string str in str_s)
                {
                    if (str != "")
                    {
                        data = new byte[str.Length / 3];
                        for (int i = 0; i < (data.Length); i++)
                        {
                            string s = str.Substring(3 * i, 2);
                            data[i] = Convert.ToByte(Int32.Parse(str.Substring(3 * i, 2), System.Globalization.NumberStyles.HexNumber));
                        }
                        data = SendCorruptedMessage(data, corrupter, channelSend);
                        readyForNextMessage = false;
                        while (!readyForNextMessage)
                        {
                            accepted = false;
                            TCPSendData(data);
                            mainWindow.AddMessageToList("[Отправлено на " + channelSend + "-й канал] ", BitConverter.ToString(data), Color.Blue);
                            Thread.Sleep(1);
                            stopwatch.Restart();
                            if (!acceptedCheck(channel)) { return; }
                            stopwatch.Stop();
                            if (!readyForNextMessage)
                            {
                                Thread.Sleep(100);
                            }
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                mainWindow.AddDebuggInformation("Неправильный адрес файла/файл не существует");
                Thread.Sleep(1);
            }
            catch (IndexOutOfRangeException ex)
            {
                mainWindow.AddDebuggInformation(ex.Message);
                Thread.Sleep(1);
            }
        }
        private bool acceptedCheck(int channel)
        {
            stopwatch.Restart();
            while (!accepted)
            {
                Thread.Sleep(1);
                if (stopwatch.ElapsedMilliseconds > 5000)
                {
                    mainWindow.AddMessageToList("[Не получено ответа на " + channel + "-й канал] ", "", Color.Red);
                    stopwatch.Stop();
                    Thread.Sleep(1);

                    return false;
                }
            }
            return true;
        }
        public bool TCPConnect(string adress = null)
        {
            try
            {
                connect = true;
                this.socket.Connect(this.ipEndPoint);
                Task.Factory.StartNew(() => TCPReceiveData(adress));
                //Task.Factory.StartNew(() => ping());
            }
            catch (System.Net.Sockets.SocketException)
            {
                mainWindow.AddDebuggInformation("Попытка установить соединение была безуспешной, т.к. от другого компьютера за требуемое время не получен нужный отклик, или было разорвано уже установленное соединение из-за неверного отклика уже подключенного компьютера");
                Thread.Sleep(1);
                return false;
            }
            return true;
        }
        public void TCPReceiveData(string writepath)
        {
            byte[] buffer = new byte[16384];
            byte[] receiveMessage = new byte[1];
            UInt32 nomerbyte = 0;
            UInt32 startbyte = 0;
            while (connect)// (!stop)
            {
                try
                {
                    while (this.socket.Connected)
                    {
                        while (this.socket.Available > 0)
                        {
                            buffer = new byte[16384];
                            int packageLength = this.socket.Receive(buffer);
                            startbyte = 0;
                            while (startbyte < packageLength)
                            {
                                nomerbyte = 0;
                                if (buffer[startbyte] == 0x23)
                                {
                                    nomerbyte = nomerbyte + 1;
                                    UInt32 lengthOfReceiveData = BitConverter.ToUInt32(buffer, (int)(startbyte + nomerbyte));
                                    nomerbyte = nomerbyte + lengthOfReceiveData + 4;
                                    receiveMessage = new byte[lengthOfReceiveData + 5];
                                    for (int i = 0; i < 0 + nomerbyte; i++)
                                    {
                                        receiveMessage[i] = buffer[i + startbyte];
                                        continue;
                                    }
                                    startbyte = nomerbyte + startbyte;
                                    if (receiveMessage[5] == 0x0A)
                                    {
                                        message10parametres[0] = receiveMessage[6]; message10parametres[1] = receiveMessage[7]; message10parametres[2] = receiveMessage[11];
                                        mainWindow.AddMessageToList("[Получено 10е сообщение c " + receiveMessage[6].ToString() + "-ого канала ] ", BitConverter.ToString(receiveMessage), Color.Green);
                                        Thread.Sleep(1);
                                        continue;
                                    }
                                    if (receiveMessage.Length == 9 && receiveMessage[1] == 0x04 && receiveMessage[5] == 0x00)
                                    {
                                        if (receiveMessage[6] == 0x00)
                                        {
                                            readyForNextMessage = true;
                                        }
                                        mainWindow.AddMessageToList("[Получено 0е сообщение] ", BitConverter.ToString(receiveMessage), Color.Gray);
                                        accepted = true;
                                        Thread.Sleep(1);
                                        continue;
                                    }
                                    if (receiveMessage[5] == 0x01)
                                    {
                                        if (receiveMessage[15] == _channel1)
                                        {
                                            mainWindow.AddMessageToList("[Получено с " + _channel1 + "-ого канала] ", BitConverter.ToString(receiveMessage), Color.Red);
                                            Thread.Sleep(1);
                                            if (!channel1NoAnswer)
                                            {
                                                byte[] mas = SendCorruptedMessage(receiveMessage, corrupter1, _channel1Send);
                                                mainWindow.AddMessageToList("[Отправлено на " + _channel1Send + "-й канал] ", BitConverter.ToString(mas), Color.Blue);
                                                TCPSendData(mas);
                                                Thread.Sleep(1);
                                            }
                                            if (!channel1NoSave)
                                            {
                                                using (StreamWriter sw = new StreamWriter(writepath, true))
                                                {
                                                    sw.Write(BitConverter.ToString(receiveMessage) + Environment.NewLine);
                                                }
                                            }
                                            continue;
                                        }
                                        else if (receiveMessage[15] == _channel2)
                                        {
                                            mainWindow.AddMessageToList("[Получено с " + _channel2 + "-ого канала] ", BitConverter.ToString(receiveMessage), Color.Red);
                                            Thread.Sleep(1);
                                            if (!channel2NoAnswer)
                                            {
                                                byte[] mas = SendCorruptedMessage(receiveMessage, corrupter2, _channel2Send);
                                                mainWindow.AddMessageToList("[Отправлено на " + _channel2Send + "-й канал] ", BitConverter.ToString(mas), Color.Blue);
                                                TCPSendData(mas);
                                                Thread.Sleep(1);
                                            }
                                            if (!channel2NoSave)
                                            {
                                                using (StreamWriter sw = new StreamWriter(writepath, true))
                                                {
                                                    sw.Write(BitConverter.ToString(receiveMessage) + Environment.NewLine);
                                                }
                                            }
                                            if (channel2XiskProcedure)
                                            {
                                                if (xisk != null)
                                                {
                                                    xisk.Mainxisk(receiveMessage);
                                                }
                                            }
                                            //string s = "";
                                            //for (int i = 16; i < mas.Length; i++)
                                            //{
                                            //    s = s + (Convert.ToString(mas[i], 2).PadLeft(8, '0') + "-");
                                            //}
                                            //s = s + Environment.NewLine;
                                            //mainWindow.AddDebuggInformation(s);
                                            continue;
                                        }
                                        else
                                        {
                                            mainWindow.AddMessageToList("[Получено неопределенное сообщение] ", BitConverter.ToString(receiveMessage), Color.Red);
                                        }
                                    }
                                    Thread.Sleep(1);
                                }
                                else
                                {
                                    startbyte = startbyte + 1;
                                }
                            }
                        }
                        Thread.Sleep(1);
                    }
                    Thread.Sleep(1);
                }
                catch (Exception ex)
                {
                    mainWindow.AddDebuggInformation(Convert.ToString(ex));
                }
            }
        }
        public void GenerateMessage10(int channel1, int speed, int ts, int modetype, int channel2 = 0)
        //modetype - выбранный режим работы, был определен по договореннности, необходим для оповещения устройства в каком режиме работать 
        //1 - 1 канал, 
        //2 - квазидуплекс, 
        //3 - запись в файл, 
        //0x80 - передача из файла
        {
            byte[] message = new byte[14] { 0x23, 0x9, 0x0, 0x0, 0x0, 0x0A, Convert.ToByte(channel1), Convert.ToByte(speed), 0, Convert.ToByte(modetype), 0, Convert.ToByte(ts), 0, 1 };
            accepted = false;
            TCPSendData(message);
            mainWindow.AddMessageToList("[Отправлено 10е сообщение на " + channel1.ToString() + "-ой канала ] ", BitConverter.ToString(message), Color.Green);
            Thread.Sleep(1);
            if (!acceptedCheck(channel1)) { return; }
            if (channel2 != 0)
            {
                message = new byte[14] { 0x23, 0x9, 0x0, 0x0, 0x0, 0x0A, Convert.ToByte(channel2), Convert.ToByte(speed), 0, Convert.ToByte(modetype), 0, Convert.ToByte(ts), 0, 1 };
                accepted = false;
                TCPSendData(message);
                mainWindow.AddMessageToList("[Отправлено 10е сообщение на " + channel2.ToString() + "-ой канала ] ", BitConverter.ToString(message), Color.Green);
                Thread.Sleep(1);
                if (!acceptedCheck(channel2)) { return; }
            }
        }
        public void TCPSendData(byte[] DataToSend)
        {
            try
            {
                this.socket.Send(DataToSend);
            }
            catch (SocketException)
            {
                socket.Close();
            }
            catch (ObjectDisposedException)
            {
                socket.Close();
            }
        }
        private byte[] SendCorruptedMessage(byte[] mes, Corrupter corrupter, int channel)
        {
            int a;
            byte[] newmes = new byte[mes.Length];
            for (int i = 0; i < 14; i++)
            {
                newmes[i] = mes[i];
            }
            newmes[15] = Convert.ToByte(channel);
            for (int i = 16; i < mes.Length; i++)
            {
                a = mes[i];
                for (int j = 7; j >= 0; j--)
                {
                    if (corrupter.md5() == 1)
                    {
                        a = a ^ (1 << j);
                    }
                }
                newmes[i] = Convert.ToByte(a);
            }
            return newmes;
        }
        public void TcpClose()
        {
            connect = false;
            this.socket.Close();
        }
    }
}
