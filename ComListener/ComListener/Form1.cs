using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using System.Diagnostics;

namespace ComListener
{

    public partial class Form1 : Form
    {
        delegate void AddColorText(RichTextBox PrimaRTB, MessageForRTB messageForRTB);
        AddColorText addColorText;
        ComboBox[] COMs = new ComboBox[8];
        Thread listenThread;
        public Form1()
        {
            InitializeComponent();
            COMs = new ComboBox[8] { Com1, Com2, Com3, Com4, Com5, Com6, Com7, Com8 };
            string[] ports = SerialPort.GetPortNames();
            int i;
            for (i = 0; i < (ports.Count() <= 8 ? ports.Count() : 8); i++)
            {
                COMs[i].Items.AddRange(ports);
                COMs[i].SelectedIndex = i;
            }
            for (; i < 8; i++)
            {
                COMs[i].Enabled = false;
            }
        }
        SerialPort[] serialPorts;
        private void StartListener_Click(object sender, EventArgs e)
        {
            int countCom = 0;
            for (int i = 0; i < COMs.Length; i++)
            {
                if (COMs[i].Enabled == true)
                {
                    countCom++;
                }
            }
            serialPorts = new SerialPort[countCom];
            for (int i = 0; i < serialPorts.Length; i++)
            {
                try
                {
                    serialPorts[i] = new SerialPort(COMs[i].Text, 115200, Parity.None, 8, StopBits.One);
                    serialPorts[i].ReadTimeout = 20;
                    serialPorts[i].WriteTimeout = 20;
                    serialPorts[i].Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR: невозможно открыть порт:" + ex.ToString());
                    return;
                }
            }
            listenThread = new Thread(new ThreadStart(Listener));
        }
        bool onGoing;
        bool onScreen = true;
        private void Listener()
        {
            onGoing = true;
            Stopwatch listenStopwatch = new Stopwatch();
            listenStopwatch.Start();
            try
            {
                while (onGoing)
                {
                    while (onScreen)
                    {
                        for (int i = 0; i < serialPorts.Length; i++)
                        {
                            string r = serialPorts[i].ReadLine();
                            if (r.Length > 0)
                            {
                                recordToRTB(Color.Black, "\n[{0}] {1}:  {2}", Convert.ToString(listenStopwatch.Elapsed), serialPorts[i].PortName, r);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                for (int i = 0; i < serialPorts.Length; i++)
                {
                    serialPorts[i].Close();
                }
            }
        }
        public void recordToRTB(Color color, string control, string fstArg = "", string scdArg = "", string thrAdr = "", string fthArg = "",
            string FithArg = "", string sxArg = "", string sthArg = "", string eithArg = "", string nthArg = "", bool newline = true)//Несколько кривенькое, но сборное сообщение для RTB
        {
            string formated = String.Format(control, fstArg, scdArg, thrAdr, fthArg, FithArg, sxArg, sthArg, eithArg, nthArg);
            if (richTextBox1.IsHandleCreated)
            {
                BeginInvoke(addColorText, richTextBox1, new MessageForRTB(formated, color, newline));
            }
            else
            {
                BeginInvoke(addColorText, richTextBox1, new MessageForRTB(formated, color, newline));
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
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            onGoing = false;
        }

        private void ПрекратитьВыводToolStripMenuItem_Click(object sender, EventArgs e)
        {
            onScreen = !onScreen;
        }
    }
}
