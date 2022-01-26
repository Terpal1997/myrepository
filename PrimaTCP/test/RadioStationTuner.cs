using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace test
{
    class RadioStationTuner 
    {
        byte _zas;
        byte _shl;
        byte _kp;
        byte _kpp;
        byte _rd1;
        byte _rd2;
        byte _kv;
        string _adressRS;
        int _portRS;
        byte _sc;
        private byte _mode;
        UdpClient udpSocket;
        public bool stateOfRasdioStation = false;
        public RadioStationTuner(string adressRS, int portRs,int myport,byte shl, byte zas, byte kp, byte kpp, byte rd1,byte rd2, byte kv,byte sc, byte mode)
        {
            udpSocket = new UdpClient(myport);
            udpSocket.Client.Blocking = true;
            _zas = zas;_shl = shl;_kp = kp;
            _kpp = kpp;_rd1 = rd1; _rd2 = rd2; _kv = kv;
            _adressRS = adressRS;_portRS = portRs;
            _sc = sc;
            _mode = mode;
            Task.Factory.StartNew(() =>sendTuneMessage( generatingTuneMessage()));
           //sendTuneMessage(generatingTuneMessage());
        }
        private void sendTuneMessage(byte[] message)
        {//IPEndPoint endPoint = new
            Task.Factory.StartNew(() => Listener(message)); 
            try
            {
                udpSocket.Send(message, message.Length, _adressRS, _portRS);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        IPEndPoint remoteIp = null;
        private void Listener(byte[] message)
        {
            Stopwatch timeForAnswer = new Stopwatch();
            timeForAnswer.Start();
            bool trueanswer = true;
            while (timeForAnswer.ElapsedMilliseconds < 5000)
            {
                try
                {
                    if (udpSocket.Available > 0)
                    {
                        byte[] data = udpSocket.Receive(ref remoteIp);
                        Console.WriteLine(BitConverter.ToString(data));
                        for (int i = 0; i < data.Length; i++)
                        {
                            if (message[i] != data[i])
                            {
                                trueanswer = false;
                                break;
                            }
                        }
                        if (trueanswer)
                        {
                            timeForAnswer.Stop();
                            stateOfRasdioStation = true;
                            udpSocket.Close();
                            return;
                        }                        
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    
                }
                Thread.Sleep(5);
            }
            timeForAnswer.Stop();
            stateOfRasdioStation = false;
            udpSocket.Close();
        }
        private byte[] generatingTuneMessage()
        {
            byte[] tuningMessage = new byte[10];
            tuningMessage[0] = 16;
            tuningMessage[1] =  _shl;
            tuningMessage[2] = _zas;
            tuningMessage[3] = _kp;
            tuningMessage[4] = _kpp;
            tuningMessage[5] = _rd1;
            tuningMessage[6] = _rd2;
            tuningMessage[7] = _kv;
            tuningMessage[8] = _sc;
            tuningMessage[9] = _mode;
            return tuningMessage;
        }
    }
}
