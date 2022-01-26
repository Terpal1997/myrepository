using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Drawing;

namespace test
{
    class NetWorker
    {
        delegate void pingFunction(Label c);
        delegate void backrecieve(byte[] message);
        private System.Net.IPAddress ipAddress;
        private System.Net.IPEndPoint ipEndPoint;
        Stopwatch stopwatchHandTest = new Stopwatch();
        BCVK_Client_MainForm BCVK;
        Socket socket;
        bool connect;
        public NetWorker(string ipAddr, string Port, BCVK_Client_MainForm bcvk)
        {
            Thread.Sleep(20);
            this.BCVK = bcvk;
            this.ipAddress = IPAddress.Parse(ipAddr);
            this.ipEndPoint = new System.Net.IPEndPoint(ipAddress, Convert.ToInt32(Port));
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public void ping()
        {
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();
            options.DontFragment = true;
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = Convert.ToInt32(this.BCVK.timeOutTB.Text);
            Task.Factory.StartNew(() => TCPReConnect());
            while (true)
            {
                try
                {
                    PingReply reply = pingSender.Send(ipAddress, timeout, buffer, options);
                    if (reply.Status == IPStatus.Success)
                    {
                        this.BCVK.StateOfConnection.ForeColor = Color.DarkGreen;
                        this.BCVK.StateOfConnection.Text = "Прима подключена";
                        this.BCVK.StateOfConnection.Update();
                        this.BCVK.StateOfConnection.Refresh();
                        this.BCVK.StateOfConnection.Hide();
                    }
                }
                catch
                {
                    this.BCVK.StateOfConnection.ForeColor = Color.Red;
                    this.BCVK.StateOfConnection.Text = "Прима отключена";
                    this.BCVK.StateOfConnection.Refresh();
                    BCVK_Client_MainForm.connectionstate = true;
                }
                finally
                {
                    Thread.Sleep(999);
                }
            }
        }
        public bool TCPConnect()
        {
            try
            {
                connect = true;
                this.socket.Connect(this.ipEndPoint);
                Thread RecieveThread = new Thread(() => TCPReceiveData());



                RecieveThread.Priority = ThreadPriority.Highest;
                RecieveThread.Start();
                //Task.Factory.StartNew(() => TCPReceiveData());
            }
            catch (System.Net.Sockets.SocketException)
            {
                MessageBox.Show(Convert.ToString("Попытка установить соединение была безуспешной, т.к. от другого компьютера за требуемое время не получен нужный отклик, или было разорвано уже установленное соединение из-за неверного отклика уже подключенного компьютера"));
                return false;
            }     
            return true;
        }
        public void TCPReConnect()
        {
            while (true)
            {
                try
                {
                    if (BCVK_Client_MainForm.connectionstate == true)
                    {
                        connect = false;
                        Thread.Sleep(1);
                        this.socket.Close();
                        this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        this.socket.Connect(this.ipEndPoint);
                        connect = true;
                        BCVK_Client_MainForm.connectionstate = false;
                    }
                    else
                    {
                        return;
                    }
                }
                catch
                {
                    BCVK_Client_MainForm.connectionstate = true;
                }
                finally
                {
                    Thread.Sleep(999);
                }
            }
        }
        public void TCPSendData(byte[] DataToSend)
        {
            try
            {
                this.socket.Send(DataToSend);
                stopwatchHandTest.Restart();
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
        public bool dividedMessageMode = false;
        public void TCPReceiveData()
        {
            byte[] buffer = new byte[2048];
            byte[] receiveMessage = new byte[1];
            int nomerbyte = 0;
            int startbyte = 0;
            while (connect)
            {
                try
                {
                    while (this.socket.Connected)
                    {
                        if (this.socket.Available > 0)
                        {
                            buffer = new byte[2048];
                            int packageLength = this.socket.Receive(buffer);
                            if (!dividedMessageMode)
                            {
                                nomerbyte = 0;
                                startbyte = 0;
                                while (startbyte < packageLength)
                                {
                                    nomerbyte = 0;
                                    if (buffer[startbyte] == 0x23)
                                    {//всё что не 23 и 00
                                        nomerbyte = nomerbyte + 1;
                                        int lengthOfReceiveData = Convert.ToInt32(BitConverter.ToUInt32(buffer, startbyte + nomerbyte));
                                        nomerbyte = nomerbyte + lengthOfReceiveData + 4;
                                        receiveMessage = new byte[lengthOfReceiveData + 5];
                                        for (int i = 0; i < 0 + nomerbyte; i++)
                                        { receiveMessage[i] = buffer[i + startbyte]; }
                                        startbyte = nomerbyte + startbyte;
                                        if (receiveMessage != null)
                                        {
                                            BCVK.TCPClientReceiveData(receiveMessage);
                                        }
                                        //Thread.Sleep(1);
                                    }
                                    else if (buffer[startbyte] != 0x0)
                                    {
                                        BCVK.UnexpectedDataInPackage(buffer, startbyte);
                                        break;

                                    }
                                    else
                                    {
                                        startbyte = startbyte + 1;
                                    }
                                }
                            }
                            else
                            {
                                BCVK.CheckInCompleteLongMessage(receiveMessage);
                            }
                        }
                      else
                        {
                            Thread.Sleep(1);
                        }
                        //if (startbyte > 0)
                        //{
                        //    byte[] bufferforConsole = new byte[startbyte];
                        //    Array.Copy(buffer, bufferforConsole, startbyte);
                        //    startbyte = 0;
                        //}
                        
                    }
                }
                catch(ArgumentOutOfRangeException ex)
                {
                    MessageBox.Show(ex.Message+Environment.NewLine+ BitConverter.ToString( buffer));
                }
                catch (MessageException ex)
                {
                  //  MessageBox.Show(ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(Convert.ToString(ex));
                //}
            }

        }
        private void labelChanging(Label c, string state, Color color)
        {
            c.Text = state;
            c.ForeColor = color;
        }
        public void TcpClose()
        {
            connect = false;
            this.socket.Close();
        }
    }
}
