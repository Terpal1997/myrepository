using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using static SimulatorRS.MainForm;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SimulatorRS
{
    class LogManager : Form
    {
        //public List<Task> messageQUEUE = new List<Task>();

        SemaphoreSlim MessageInRow = new SemaphoreSlim(1, 1);
        delegate void AppengText(string dialogInformation, string message, Color color);
        delegate void AddColor(RichTextBox RTB, string message, Color color);
        RichTextBox _RTB;
        MainForm _main;
        public LogManager(RichTextBox RTB, MainForm main)
        {
            _RTB = RTB;
            _main = main;
           // Task.Factory.StartNew(() => LogQueue());
        }

       

        public void timesofRepeats(int channel, string message)
        {
           
            MessageForRTB mes = new MessageForRTB("Количество повторов:", channel, message, Color.Black);
            //MessageInRow.Release();
        }
        public void ToChatRTB(string time,int channel,string message)
        {

            MessageForRTB mes = new MessageForRTB("["+time+"] "+"Ушло:", channel, message, Color.Blue);
            //MessageInRow.Release();
        }
        public void ReToChatRTB(string time, int channel, string message)
        {

            MessageForRTB mes = new MessageForRTB("[" + time + "] " + "Ушло повторно:", channel, message, Color.Blue);
           // MessageInRow.Release();
        }
        public void ErrorToChatRTB(string time, int channel, string message)
        {

            MessageForRTB mes = new MessageForRTB("[" + time + "] " + "Error:", channel, message, Color.Red);
            //MessageInRow.Release();
        }
        public void TimeOutToChatRTB(string time, int channel, string message)
        {

            MessageForRTB mes = new MessageForRTB("[" + time + "] " + "timeout:", channel, message, Color.Blue);
           // MessageInRow.Release();
        }
        public void ReturnFromNetToChatRTB(string time, int channel, string message)
        {

            MessageForRTB mes = new MessageForRTB("[" + time + "] " + "Пришло:",channel, message, Color.Green);
            //MessageInRow.Release();
        }
        private void AppendingText(string message)
        {

            _RTB.AppendText(message);
           // MessageInRow.Release();
        }
    }
}
