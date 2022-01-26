using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WiresharkParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string defaultSaveFilePath = @".\" + DateTime.Now.Ticks + ".txt";
        SettingsWindow settings;
        public MainWindow()
        {
            InitializeComponent();
            SaveFilePath.Text = defaultSaveFilePath;
            SaveFilePath.IsEnabled = true;
            SaveAsButton.IsEnabled = true;
            StartButton.IsEnabled = true;
            settings = new SettingsWindow();
            settings.LoadSettings();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Файлы txt|*.txt";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == true)
                {
                    LoadFilePath.Text = openFileDialog.FileName;
                    //SaveFilePath.IsEnabled = true;
                    //SaveAsButton.IsEnabled = true;
                    //StartButton.IsEnabled = true;
                }
                else
                {
                    //SaveFilePath.IsEnabled = false;
                    //SaveAsButton.IsEnabled = false;
                    //StartButton.IsEnabled = false;
                }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //}
        }

        private void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog openFileDialog = new SaveFileDialog();
                openFileDialog.Filter = "Файлы txt|*.txt";
                if (openFileDialog.ShowDialog() == true)
                {
                    SaveFilePath.Text = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
                if (defaultSaveFilePath == SaveFilePath.Text)
                {
                    defaultSaveFilePath = @".\" + DateTime.Now.Ticks + ".txt";
                    SaveFilePath.Text = defaultSaveFilePath;
                }
                Parser parser = new Parser(this.LoadFilePath.Text, this.SaveFilePath.Text, settings);
                var arrayOfData = parser.arrayList;
                WindowVsData windowVsData = new WindowVsData(arrayOfData);
                windowVsData.ShowDialog();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //}
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            settings.Hide();
            settings.Show();     
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
            //settings.Close();
        }
    }
}
