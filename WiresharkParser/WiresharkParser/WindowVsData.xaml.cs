using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
    /// Interaction logic for WindowVsData.xaml
    /// </summary>
    public partial class WindowVsData : Window
    {
        private GridViewColumnHeader listViewSortCol = null;

        public ObservableCollection<Data> ListOfData { get; set; }
        public WindowVsData(ObservableCollection<Data> listOfData)
        {
            InitializeComponent();
            this.ListOfData = listOfData;
            ViewOfData.ItemsSource = this.ListOfData;


        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            // MainWindow mainWindow = new MainWindow();

        }

        private void TextBlock_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            //Здесь вставляется подсказка
            TextBlock textBlock = sender as TextBlock;
            var helpToolTip = new ToolTip();
            TextBlock helpTextBlock = new TextBlock();
            helpTextBlock.Width = 800;
            helpTextBlock.TextWrapping = TextWrapping.Wrap;
            helpTextBlock.Text = HelpParser(textBlock.Text); //Подсказка вставляется тут
            helpToolTip.Content = helpTextBlock;
            textBlock.ToolTip = helpToolTip;
        }
        private string HelpParser(string dataFromCell)
        {
            try
            {
                dataFromCell = dataFromCell.Trim(' ');

                string[] dataBytes = dataFromCell.Split(' ');
                byte[] ConvertedData = new byte[dataBytes.Length];

                for (int i = 0; i < dataBytes.Length; i++)
                {
                    ConvertedData[i] = byte.Parse(dataBytes[i], System.Globalization.NumberStyles.HexNumber);
                }
                try
                {

                    string helpForCell = "";
                    //Тут создавать подсказку
                    if (ConvertedData[0] == 35)
                    {
                        helpForCell = "Сообщение Примы: ";
                        helpForCell += ParseTCP(ConvertedData);
                        return helpForCell;
                    }
                    if (ConvertedData[0] == 0 || ConvertedData[0] == 1 || ConvertedData[0] == 2 || ConvertedData[0] == 3
                        || ConvertedData[0] == 4 || ConvertedData[0] == 5 || ConvertedData[0] == 6 || ConvertedData[0] == 7)
                    {
                        helpForCell = "Сообщение Радиостанции: ";
                        helpForCell += ParseUDP(ConvertedData);
                        return helpForCell;
                    }
                    return "Мусор";
                }
                catch
                { return "Error"; }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "Error";
            }
        }
        



        private string ParseTCP(byte[] Data)
        {
            string answer = "";
            switch (Data[0])
            {
                case 0: { Parse_00_TCP(ref answer, Data); break; };
                case 1: { Parse_01_TCP(ref answer, Data); break; };
                case 2: { Parse_02_TCP(ref answer, Data); break; };
                case 3: { Parse_03_TCP(ref answer, Data); break; };
                case 10: { Parse_0A_TCP(ref answer, Data); break; };
                case 11: { Parse_0B_TCP(ref answer, Data); break; };
                case 12: { Parse_0C_TCP(ref answer, Data); break; };
                case 13: { Parse_0D_TCP(ref answer, Data); break; };
                case 14: { Parse_0E_TCP(ref answer, Data); break; };
                case 15: { Parse_0F_TCP(ref answer, Data); break; };
                case 16: { Parse_10_TCP(ref answer, Data); break; };
                case 20: { Parse_14_TCP(ref answer, Data); break; };
                case 127: { Parse_7F_TCP(ref answer, Data); break; };
                case 255: { Parse_FF_TCP(ref answer, Data); break; };
                default: { answer = "Error"; break; }
            }
            return answer;
        }
        private string ParseUDP(byte[] Data)
        {
            string answer = "";
            switch (Data[0])
            {
                case 0: { Parse_00_UDP(ref answer, Data); break; };
                case 1: { Parse_01_UDP(ref answer, Data); break; };
                case 2: { Parse_02_UDP(ref answer, Data); break; };
                case 3: { Parse_03_UDP(ref answer, Data); break; };
                case 4: { Parse_04_UDP(ref answer, Data); break; };
                case 5: { Parse_05_UDP(ref answer, Data); break; };
                case 6: { Parse_06_UDP(ref answer, Data); break; };
                case 7: { Parse_07_UDP(ref answer, Data); break; };

                default: { answer = "Error"; break; }
            }
            return answer;
        }
        #region firstByteParse_TCP
        private void Parse_00_TCP(ref string answer, byte[] data)
        {

            answer = "Подтверждение команды";
        }
        private void Parse_01_TCP(ref string answer, byte[] data)
        {
            answer = "Данные одиночного сообщения";
        }
        private void Parse_02_TCP(ref string answer, byte[] data)
        {
            answer = "Результат передачи данных";
        }
        private void Parse_03_TCP(ref string answer, byte[] data)
        {
            answer = "Результат передачи данных";
        }
        private void Parse_0A_TCP(ref string answer, byte[] data)
        {
            answer = "Установление режима канала/Режим канала";
        }
        private void Parse_0B_TCP(ref string answer, byte[] data)
        {
            answer = "Установка времени";
        }
        private void Parse_0C_TCP(ref string answer, byte[] data)
        {
            answer = "Установка режимов ЗАС";
        }
        private void Parse_0D_TCP(ref string answer, byte[] data)
        {
            answer = "Сброс канала в исходное состояние";
        }
        private void Parse_0E_TCP(ref string answer, byte[] data)
        {
            answer = "Прекращение передачи данных";
        }
        private void Parse_0F_TCP(ref string answer, byte[] data)
        {
            answer = "Разрешение обработки составных сообщений на приёме";
        }
        private void Parse_10_TCP(ref string answer, byte[] data)
        {
            answer = "Установка режима взаимодействия с Ethernet радиостанцией/Режим взаимодействия с Ethernet радиостанцией";
        }
        private void Parse_14_TCP(ref string answer, byte[] data)
        {
            answer = "Разрешение выдачи в БЦВК донесений \"Состояние канала\"";
        }
        private void Parse_7F_TCP(ref string answer, byte[] data)
        {
            answer = "PrintF";
        }
        private void Parse_FF_TCP(ref string answer, byte[] data)
        {
            answer = "Контроль соединения";
        }
        #endregion
        #region firstByteParse_UDP
        private void Parse_00_UDP(ref string answer, byte[] data)
        {
            string numberOfMessage = Environment.NewLine + " Номер сообщения: " + Convert.ToString(BitConverter.ToInt16(data, 1));
            string numberOfPackege = Environment.NewLine + " Номер пакета: " + Convert.ToString(BitConverter.ToInt16(data, 3));
            string PSBI = Environment.NewLine + " Признак способа выдачи информации: " + Convert.ToString(data[5]);
            string modeOfSending = Environment.NewLine + " Режим выдачи: " + Convert.ToString(data[6]);
            string dataMessage = Environment.NewLine + " Данные: " + BitConverter.ToString(data, 7);
            answer = "Данные" + numberOfMessage + numberOfPackege + PSBI + modeOfSending + dataMessage;
        }
        private void Parse_01_UDP(ref string answer, byte[] data)
        {
            string numberOfMessage = Environment.NewLine + " Номер сообщения: " + Convert.ToString(BitConverter.ToInt16(data, 1));
            answer = "Сброс передачи данных" + numberOfMessage;
        }
        private void Parse_02_UDP(ref string answer, byte[] data)
        {
            string numberOfMessage = Environment.NewLine + " Номер сообщения: " + Convert.ToString(BitConverter.ToInt16(data, 1));
            string PSorSI = Environment.NewLine + " ПС/СИ: " + Convert.ToString(data[3]);

            answer = "Излучение" + numberOfMessage + PSorSI;
        }
        private void Parse_03_UDP(ref string answer, byte[] data)
        {
            string numberOfMessage = Environment.NewLine + " Номер сообщения: " + Convert.ToString(BitConverter.ToInt16(data, 1));
            string messageState = Environment.NewLine + " Статут сообщения: " + Convert.ToString(data[3]);

            answer = "Подтверждение команды/донесения" + numberOfMessage + messageState;
        }
        private void Parse_04_UDP(ref string answer, byte[] data)
        {
            string numberOfMessage = Environment.NewLine + " Номер сообщения: " + Convert.ToString(BitConverter.ToInt16(data, 1));
            string numberOfPackege = Environment.NewLine + " Номер пакета: " + Convert.ToString(BitConverter.ToInt16(data, 3));
            answer = "Начало передачи данных в радиоканал" + numberOfMessage + numberOfPackege;
        }
        private void Parse_05_UDP(ref string answer, byte[] data)
        {
            string numberOfMessage = Environment.NewLine + " Номер сообщения: " + Convert.ToString(BitConverter.ToInt16(data, 1));
            string numberOfPackege = Environment.NewLine + " Номер пакета: " + Convert.ToString(BitConverter.ToInt16(data, 3));
            string KO = Environment.NewLine + " Код ошибки: " + Convert.ToString(data[5]);
            answer = "Ошибка передачи данных" + numberOfMessage + numberOfPackege + KO;
        }
        private void Parse_06_UDP(ref string answer, byte[] data)
        {
            string numberOfMessage = Environment.NewLine + " Номер сообщения: " + Convert.ToString(BitConverter.ToInt16(data, 1));
            string numberOfPackege = Environment.NewLine + " Номер пакета: " + Convert.ToString(BitConverter.ToInt16(data, 3));
            answer = "Данные выданы в радиоканал" + numberOfMessage + numberOfPackege;
        }
        private void Parse_07_UDP(ref string answer, byte[] data)
        {
            string numberOfMessage = Environment.NewLine + " Номер сообщения: " + Convert.ToString(BitConverter.ToInt16(data, 1));
            answer = "Передача данных прекращена" + numberOfMessage;
        }

        #endregion

        //private void ViewOfData_Sorting(object sender, DataGridSortingEventArgs e)
        //{
        //    var dgSender = (DataGrid)sender;
        //    var cView = CollectionViewSource.GetDefaultView(dgSender.ItemsSource);
        //    ListSortDirection direction = ListSortDirection.Ascending;
        //    if (cView.SortDescriptions.FirstOrDefault().PropertyName == e.Column.SortMemberPath)
        //        direction = cView.SortDescriptions.FirstOrDefault().Direction == ListSortDirection.Descending ? ListSortDirection.Ascending : ListSortDirection.Descending;
        //    cView.SortDescriptions.Clear();
        //    AddSortColumn((DataGrid)sender, e.Column.SortMemberPath, direction);

        //    AddSortColumn((DataGrid)sender, "dateTime", ListSortDirection.Ascending);
        //    //CollectionViewSource view = (CollectionViewSource) this.FindResource(ViewOfData.ItemsSource);
        //    //ViewOfData.SortDescriptions.Add(new System.ComponentModel.SortDescription("dateTime", System.ComponentModel.ListSortDirection.Ascending));
        //    //ViewOfData.SortDescriptions.Add(new System.ComponentModel.SortDescription(view, System.ComponentModel.ListSortDirection.Ascending));
        //}
        //private void AddSortColumn(DataGrid sender, string sortColumn, ListSortDirection direction)

        //{
        //    var cView = CollectionViewSource.GetDefaultView(sender.ItemsSource);
        //    cView.SortDescriptions.Add(new SortDescription(sortColumn, direction));
        //    foreach (var col in sender.Columns.Where(x => x.SortMemberPath == sortColumn))
        //    {
        //        col.SortDirection = direction;
        //    }
        //}
    }
}
