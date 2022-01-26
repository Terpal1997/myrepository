using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WiresharkParser
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        IniFile INI = new IniFile("settings.ini");
        public TextBox[,] Configuration;
        static string[] parametresNames = new string[6] { "RsIp", "RsPort", "RsMac", "UUIp", "UUPort", "UUMac" };
        public SettingsWindow()
        {
            InitializeComponent();
            Configuration = new TextBox[32, 6]
            {{  RsIp1,RsPort1,RsMac1,UUIp1,UUPort1,UUMac1},
                {RsIp2,RsPort2,RsMac2,UUIp2,UUPort2,UUMac2},
                {RsIp3,RsPort3,RsMac3,UUIp3,UUPort3,UUMac3},
                {RsIp4,RsPort4,RsMac4,UUIp4,UUPort4,UUMac4},
                {RsIp5,RsPort5,RsMac5,UUIp5,UUPort5,UUMac5},
                {RsIp6,RsPort6,RsMac6,UUIp6,UUPort6,UUMac6},
                {RsIp7,RsPort7,RsMac7,UUIp7,UUPort7,UUMac7},
                {RsIp8,RsPort8,RsMac8,UUIp8,UUPort8,UUMac8},
                {RsIp9,RsPort9,RsMac9,UUIp9,UUPort9,UUMac9},
                {RsIp10,RsPort10,RsMac10,UUIp10,UUPort10,UUMac10},
                {RsIp11,RsPort11,RsMac11,UUIp11,UUPort11,UUMac11},
                {RsIp12,RsPort12,RsMac12,UUIp12,UUPort12,UUMac12},
                {RsIp13,RsPort13,RsMac13,UUIp13,UUPort13,UUMac13},
                {RsIp14,RsPort14,RsMac14,UUIp14,UUPort14,UUMac14},
                {RsIp15,RsPort15,RsMac15,UUIp15,UUPort15,UUMac15},
                {RsIp16,RsPort16,RsMac16,UUIp16,UUPort16,UUMac16},
                {RsIp17,RsPort17,RsMac17,UUIp17,UUPort17,UUMac17},
                {RsIp18,RsPort18,RsMac18,UUIp18,UUPort18,UUMac18},
                {RsIp19,RsPort19,RsMac19,UUIp19,UUPort19,UUMac19},
                {RsIp20,RsPort20,RsMac20,UUIp20,UUPort20,UUMac20},
                {RsIp21,RsPort21,RsMac21,UUIp21,UUPort21,UUMac21},
                {RsIp22,RsPort22,RsMac22,UUIp22,UUPort22,UUMac22},
                {RsIp23,RsPort23,RsMac23,UUIp23,UUPort23,UUMac23},
                {RsIp24,RsPort24,RsMac24,UUIp24,UUPort24,UUMac24},
                {RsIp25,RsPort25,RsMac25,UUIp25,UUPort25,UUMac25},
                {RsIp26,RsPort26,RsMac26,UUIp26,UUPort26,UUMac26},
                {RsIp27,RsPort27,RsMac27,UUIp27,UUPort27,UUMac27},
                {RsIp28,RsPort28,RsMac28,UUIp28,UUPort28,UUMac28},
                {RsIp29,RsPort29,RsMac29,UUIp29,UUPort29,UUMac29},
                {RsIp30,RsPort30,RsMac30,UUIp30,UUPort30,UUMac30},
                {RsIp31,RsPort31,RsMac31,UUIp31,UUPort31,UUMac31},
                {RsIp32,RsPort32,RsMac32,UUIp32,UUPort32,UUMac32},
        };
            TcpIpPrima.LostFocus += TextBox_LostFocus;
            TcpPortPrima.LostFocus += TextBox_LostFocus;
            UdpIpPrima.LostFocus += TextBox_LostFocus;
            UdpPortPrima.LostFocus += TextBox_LostFocus;
            TcpIpDevice.LostFocus += TextBox_LostFocus;
            TcpPortDevice.LostFocus += TextBox_LostFocus;
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    Configuration[i, j].LostFocus += TextBox_LostFocus;
                }
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            INI.Write("Settings", "TcpIpPrima", TcpIpPrima.Text);
            INI.Write("Settings", "TcpPortPrima", TcpPortPrima.Text);
            INI.Write("Settings", "UdpIpPrima", UdpIpPrima.Text);
            INI.Write("Settings", "UdpPortPrima", UdpPortPrima.Text);
            INI.Write("Settings", "TcpIpDevice", TcpIpDevice.Text);
            INI.Write("Settings", "TcpPortDevice", TcpPortDevice.Text);
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    INI.Write("Settings", parametresNames[i] + j.ToString(), Configuration[j, i].Text);
                }
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            LoadSettings();
        }
        public void LoadSettings()
        {
            if (INI.KeyExists("Settings", "TcpIpPrima"))
            {
                TcpIpPrima.Text = (INI.ReadINI("Settings", "TcpIpPrima"));
                TcpPortPrima.Text = (INI.ReadINI("Settings", "TcpPortPrima"));
                UdpIpPrima.Text = (INI.ReadINI("Settings", "UdpIpPrima"));
                UdpPortPrima.Text = (INI.ReadINI("Settings", "UdpPortPrima"));
                TcpIpDevice.Text = (INI.ReadINI("Settings", "TcpIpDevice"));
                TcpPortDevice.Text = (INI.ReadINI("Settings", "TcpPortDevice"));
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        Configuration[j, i].Text = (INI.ReadINI("Settings", parametresNames[i] + j));
                    }
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {

            //if (sender is TextBox textBox)
            //{

                TextBox textBox = sender as TextBox;

                textBox.Text = textBox.Text.Trim(' ');

            //}
        }
        
    }
}
