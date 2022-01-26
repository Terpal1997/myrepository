using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SimulatorRS
{
    class UURadiostation//данный класс выполняет задачу УУ (устройства управления) радиостанции, принимает и оповещает
    {
        UdpClient UUudpSocket;
        RadiostationManager _myRS;
        int _myPort;
        bool stop = false;
        Thread Listen;
        IPEndPoint remoteIp = null;
        functionUDPPrima function;
        public UURadiostation (int port, RadiostationManager rs,functionUDPPrima function)
        {
            _myRS = rs;
            _myPort = port;
            UUudpSocket = new UdpClient(_myPort);
            this.function = function;
            Listen = new Thread(() => ListenEmitting());
            Listen.Start();
        }

        private void ListenEmitting()
        {
            byte[] dataRecieved;
            byte[] dataForSend;
            while(!stop)
            {
                if (UUudpSocket.Available > 0)
                {
                    dataRecieved = UUudpSocket.Receive(ref remoteIp);
                    if (dataRecieved[0] == 2)
                    {
                        dataForSend = function.generatingMessage2(dataRecieved[3]);
                        switch(dataRecieved[3])
                        {
                            case 1:
                                _myRS.emittingSet(dataRecieved, dataForSend, true);
                                break;
                            case 2:
                            default:
                                _myRS.emittingSet(dataRecieved, dataForSend, false);
                                break;
                               
                        }
                        //new Socket_Message(this.udpSocket, data);
                        UUudpSocket.Send(dataForSend, dataForSend.Length, remoteIp);
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
            }

        }
        public void CloseSocket()
        {
            stop = true;
            Thread.Sleep(10);
            UUudpSocket.Close();
        }

    }
}
