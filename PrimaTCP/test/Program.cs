using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace test
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [STAThread]
        static void Main()
        {
            /// Чтож, приступим  
            /// Данная программа по факту является моей первой серьезной работой. А по сему она вся корявая, даже не смотря на то,
            /// что я старался ее периодически чистить и улучшать
            /// Я, в целом, постараюсь объяснить как тут всё работает, если само поймы, ХЫ
            Thread thread = new Thread(() => Application.Run(new BCVK_Client_MainForm()));//Ествественно запускается рабочая форма в отдельном потоке.
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            // ShowWindow(GetConsoleWindow(), 0);
            Console.ReadKey();
        }      
    }
}