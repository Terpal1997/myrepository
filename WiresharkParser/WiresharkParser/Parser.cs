using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Documents;
using System.Collections.ObjectModel;
namespace WiresharkParser
{
    public class Parser
    {
        public ObservableCollection<Data> arrayList
        {
            get;
            set;
        }

        private string fileLoadPath, fileSavePath = @".\" + DateTime.Now.Ticks + ".txt";
        SettingsWindow settings;
        Encoding ascii = Encoding.Default;
        //SettingsWindow settings;
        //public Parser(string fileLoadPath, string fileSavePath, SettingsWindow settingsForm)
        public Parser(string fileLoadPath, string fileSavePath, SettingsWindow settingsForm)
        {
            arrayList = new ObservableCollection<Data>();
            this.fileLoadPath = fileLoadPath;
            if (fileSavePath != null && fileSavePath != "")
            {
                this.fileSavePath = fileSavePath;
            }
            settings = settingsForm;
            FileReader();
            //  FileWriter();
        }

        private void AddItemToList(string dateTime, string itemToAdd)
        {
            string protokol = " undefined ";
            string srcIP = IpAdressConverter(itemToAdd.Substring(84, 11).ToUpper().Replace('|', '.'));
            string srcPort = Convert.ToString(Int32.Parse(itemToAdd.Substring(108, 2), System.Globalization.NumberStyles.HexNumber) * 256
                + Int32.Parse(itemToAdd.Substring(111, 2), System.Globalization.NumberStyles.HexNumber));
            string dstIP = IpAdressConverter(itemToAdd.Substring(96, 11).ToUpper().Replace('|', '.'));
            string dstPort = Convert.ToString(Int32.Parse(itemToAdd.Substring(114, 2), System.Globalization.NumberStyles.HexNumber) * 256
                + Int32.Parse(itemToAdd.Substring(117, 2), System.Globalization.NumberStyles.HexNumber));




            if (itemToAdd.Substring(75, 2).Equals("06"))
                protokol = " TCP ";
            else if (itemToAdd.Substring(75, 2).Equals("11"))
                protokol = " UDP ";
            if (protokol == " UDP ")
            {
                int dataLength = 0;
                switch (itemToAdd.Substring(132, 2))
                {
                    case "01":
                    case "03":
                        dataLength = 3; break;
                    case "02":
                        if (itemToAdd.Length > 138)
                        {
                            if (itemToAdd.Substring(141, 2) != "00")
                            {
                                dataLength = 4; break;
                            }
                        }
                        dataLength = 2; break;
                    case "04":
                    case "06":
                    case "07":
                        dataLength = 5; break;
                    case "05":
                        dataLength = 6; break;
                    default:
                        break;
                }

                int trashdataLength = 0;
                if (itemToAdd.Substring(132, 2) != "00")
                {
                    trashdataLength = itemToAdd.Length - 132 - dataLength * 3;
                }

                arrayList.Add(new Data(
                dateTime, //Время
                srcIP,
                srcPort,
                dstIP,
                dstPort,
                protokol,  //Протокол
                itemToAdd.Substring(132, itemToAdd.Length - 132 - trashdataLength).Replace('|', ' '), //Данные UDP
                definitionOfMessage(srcIP, srcPort, dstIP, dstPort, protokol, itemToAdd.Substring(132).Replace('|', ' ')))
                );
            }
            if (protokol == " TCP " && itemToAdd.Length > 198)
            {
                List<string> messages = (dataAnalyzator(itemToAdd.Substring(168))) ?? (new List<string>());

                while (messages.Count > 0)
                {
                    arrayList.Add(new Data(
                    dateTime, //Время
                    srcIP,
                    srcPort,
                    dstIP,
                    dstPort,
                    protokol,  //Протокол
                    messages.First(),
                    definitionOfMessage(srcIP, srcPort, dstIP, dstPort, protokol, messages.First()))

                //itemToAdd.Substring(168, itemToAdd.Length - 168).Replace('|', ' ')) //Данные TCP


                );
                    messages.RemoveAt(0);
                }
            }
            if (protokol == " undefined ")
            {
                int lenght = itemToAdd.Length;
                lenght = 0;
            }
        }

        private string definitionOfMessage(string srcIP, string srcPort, string dstIP, string dstPort, string protokol, string data)
        {

            string resultString = "";
            if (protokol == " TCP ")
            {
                if (srcIP.Equals(settings.TcpIpPrima.Text) && srcPort.Equals(settings.TcpPortPrima.Text))
                {
                    resultString += "Сообщение от Примы ";
                }
                else if (srcIP.Equals(settings.TcpIpDevice.Text))
                {
                    resultString += "Сообщение от ЭВМ ";
                }
                else
                {
                    resultString += "Сообщение от НЕИЗВЕСТНО ";
                }
                if (dstIP.Equals(settings.TcpIpPrima.Text) && dstPort.Equals(settings.TcpPortPrima.Text))
                {
                    resultString += "к Приме ";
                }
                else if (dstIP.Equals(settings.TcpIpDevice.Text))
                {
                    resultString += "к ЭВМ ";
                }
                else
                {
                    resultString += "к НЕИЗВЕСТНО ";
                }
                resultString += ParseTCP(data);
            }
            else if (protokol == " UDP ")
            {
                bool messageUndefined = true;
                if (srcIP.Equals(settings.UdpIpPrima.Text) && srcPort.Equals(settings.UdpPortPrima.Text))
                {
                    resultString += "Сообщение от Примы ";
                }
                else
                {
                    for (int i = 0; i < 32; i++)
                    {
                        if (srcIP.Equals(settings.Configuration[i, 0].Text)
                        && srcPort.Equals(settings.Configuration[i, 1].Text))
                        {
                            resultString += "Сообщение от ЭВМ c радиоканала " + (i + 1).ToString() + " ";
                            messageUndefined = false;
                            break;
                        }
                    }
                    if (messageUndefined)
                    {
                        resultString += "Сообщение от НЕИЗВЕСТНО ";
                    }
                }
                messageUndefined = true;
                if (dstIP.Equals(settings.UdpIpPrima.Text) && dstPort.Equals(settings.UdpPortPrima.Text))
                {
                    resultString += "к Приме ";
                }
                else
                {
                    for (int i = 0; i < 32; i++)
                    {
                        if (dstIP.Equals(settings.Configuration[i, 0].Text)
                        && dstPort.Equals(settings.Configuration[i, 1].Text))
                        {
                            resultString += "к ЭВМ c радиоканала " + (i + 1).ToString() + " ";
                            messageUndefined = false;
                            break;
                        }
                    }
                    if (messageUndefined)
                    {
                        resultString += "к НЕИЗВЕСТНО";
                    }
                }
                resultString += ParseUDP(data);

            }
            return resultString;
            //  throw new NotImplementedException();
        }

        private string IpAdressConverter(string ipadress)
        {
            string[] resmas = ipadress.Split('.');
            for (int i = 0; i < resmas.Length; i++)
            {
                resmas[i] = Convert.ToString(Int32.Parse(resmas[i], System.Globalization.NumberStyles.HexNumber));
            }

            return resmas[0] + '.' + resmas[1] + '.' + resmas[2] + '.' + resmas[3];

        }
        private List<string> dataAnalyzator(string data)
        {
            data = data.Trim(' ');
            data = data.Trim('|');
            data = data.Trim('-');
            string[] dataBytes;
            //if ( data.Split('-').GetLength(0) != 1)
            //{
            //     dataBytes = data.Split('-');
            //}
            //else
            //{
            //    dataBytes = data.Split('|');
            //}
            dataBytes = data.Split('|');
            byte[] ConvertedData = new byte[dataBytes.Length];
            for (int i = 0; i < dataBytes.Length; i++)
            {
                ConvertedData[i] = byte.Parse(dataBytes[i], System.Globalization.NumberStyles.HexNumber);
            }
            if (ConvertedData.Length == 1460 && ConvertedData[1459] == 100) //HTTP filter
            { return null; }
            int nomerbyte = 0;
            int startbyte = 0;
            List<string> resList = new List<string>();
            while (startbyte < ConvertedData.Length)
            {
                nomerbyte = 0;
                if (ConvertedData[startbyte] == 0x23)
                {

                    //breakbyte = 0;
                    nomerbyte = nomerbyte + 1;
                    int lengthOfReceiveData = Convert.ToInt32(BitConverter.ToUInt32(ConvertedData, startbyte + nomerbyte));
                    nomerbyte = nomerbyte + lengthOfReceiveData + 4;
                    byte[] bufMessage = new byte[lengthOfReceiveData + 5];
                    for (int i = 0; i < 0 + nomerbyte; i++)
                    { bufMessage[i] = ConvertedData[i + startbyte]; }
                    startbyte = nomerbyte + startbyte;
                    if (bufMessage != null)
                    {
                        resList.Add((BitConverter.ToString(bufMessage)).Replace('-', ' '));
                    }

                }
                else
                {
                    startbyte = startbyte + 1;
                }

            }
            return resList;
        }
        private string IpConverter(string str)
        {
            string[] NumberMas;
            NumberMas = str.ToUpper().Split(' ');
            return (Convert.ToString(Int32.Parse(NumberMas[0], System.Globalization.NumberStyles.HexNumber))
                + "." + Convert.ToString(Int32.Parse(NumberMas[1], System.Globalization.NumberStyles.HexNumber))
                 + "." + Convert.ToString(Int32.Parse(NumberMas[2], System.Globalization.NumberStyles.HexNumber))
                  + "." + Convert.ToString(Int32.Parse(NumberMas[3], System.Globalization.NumberStyles.HexNumber)));
        }
        private string PortConverter(string str)
        {
            string[] NumberMas;
            NumberMas = str.ToUpper().Split(' ');
            return Convert.ToString(Int32.Parse(NumberMas[0], System.Globalization.NumberStyles.HexNumber) + Int32.Parse(NumberMas[1], System.Globalization.NumberStyles.HexNumber) * 256);
        }
        private string MacConverter(string str)
        {
            return str.Replace(' ', ':');
        }
        private void FileReader()
        {
            string dateTime = "";
            using (StreamReader streamReader = new StreamReader(fileLoadPath))
            {
                string lineFromFile;
                while ((lineFromFile = streamReader.ReadLine()) != null)
                {
                    if (!lineFromFile.Equals("+---------+---------------+----------+") && !lineFromFile.Equals(""))
                    {
                        if (lineFromFile.Length > 162)
                        {
                            AddItemToList(dateTime, lineFromFile);
                        }
                        else if (lineFromFile.Length <= 30)
                        {
                            dateTime = lineFromFile.Substring(0, 16);
                        }
                    }

                }
            }
        }
        private void FileWriter()
        {
            FileStream filestream = new FileStream(fileSavePath, FileMode.OpenOrCreate);
            using (StreamWriter streamWriter = new StreamWriter(filestream))
            {
                foreach (Data data in arrayList.OrderBy(u => u.source))
                {
                    streamWriter.WriteLine("DateTime  {0,20} From {1,20} Port {2,20} To {3,20} Port {4,20} Protocol {5,10} Data {6} Information {7}", data.dateTime, data.source, data.sourcePort, data.destination, data.destinationPort, data.protocol, data.data, data.information);
                }
            }
        }
        private string ParseTCP(string Data)
        {
            string answer = "";
            switch (UInt32.Parse(Data.Substring(15, 2), System.Globalization.NumberStyles.HexNumber))
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
        private string ParseUDP(string Data)
        {
            string answer = "";
            switch (UInt32.Parse(Data.Substring(0, 2), System.Globalization.NumberStyles.HexNumber))
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
        private void Parse_00_TCP(ref string answer, string data)
        {
            string[] errorbyte = new string[16]{" ОТКАЗ канала 1"," ОТКАЗ канала 2"," ОТКАЗ канала 3",
                " ОТКАЗ канала 4"," ОТКАЗ канала 5"," ОТКАЗ канала 6"," ОТКАЗ канала 7"," ОТКАЗ канала 8"," ОТКАЗ комплекса устройства управления",
                " ОТКАЗ контроллера Ethernet радиостанций"," ОТКАЗ контроллера ЗАС1"," ОТКАЗ контроллера ЗАС2"," ОТКАЗ ЗАС1"," ОТКАЗ ЗАС2","", ""};
            answer = "Подтверждение команды";
            string[] statebyte = new string[16]{" ОТКАЗ канала 1"," ОТКАЗ канала 2"," ОТКАЗ канала 3",
                " ОТКАЗ канала 4"," ОТКАЗ канала 5"," ОТКАЗ канала 6"," ОТКАЗ канала 7"," ОТКАЗ канала 8"," ОТКАЗ комплекса устройства управления",
                " ОТКАЗ контроллера Ethernet радиостанций"," ОТКАЗ контроллера ЗАС1"," ОТКАЗ контроллера ЗАС2"," ОТКАЗ ЗАС1"," ОТКАЗ ЗАС2","", ""};
            answer = "Подтверждение команды ";
            switch (UInt32.Parse(data.Substring(18, 2), System.Globalization.NumberStyles.HexNumber))
            {
                case 0:
                    answer += "команда выполнена или принята к выполнению ";
                    break;
                case 1:
                    answer += "ошибка формата ";
                    break;
                case 2:
                    answer += "невозможно выполнить ";
                    break;
            }
            byte[] errorbytes = new byte[2];
            errorbytes[0] = byte.Parse(data.Substring(21, 2), System.Globalization.NumberStyles.HexNumber);
            errorbytes[1] = byte.Parse(data.Substring(24, 2), System.Globalization.NumberStyles.HexNumber);
            int ConvertedBytes = BitConverter.ToInt16(errorbytes, 0);
            for (int i = 0; i < 16; i++)
            {
                if (Convert.ToInt16(ConvertedBytes & 1) == 1)
                {

                    answer += errorbyte[i];
                }
                ConvertedBytes = ConvertedBytes >> 1;
            }

        }
        private string InsConverter(string INS)
        {
            string bufString;
            bufString = INS;
            string[] bufStringMas = bufString.Split(' ');

            return Convert.ToString(UInt32.Parse(bufStringMas[0], System.Globalization.NumberStyles.HexNumber) + UInt32.Parse(bufStringMas[1], System.Globalization.NumberStyles.HexNumber) * 256
                + UInt32.Parse(bufStringMas[2], System.Globalization.NumberStyles.HexNumber) * 65536 + UInt32.Parse(bufStringMas[3], System.Globalization.NumberStyles.HexNumber) * 16777216) + " ";


        }
        private void Parse_01_TCP(ref string answer, string data)
        {
            //UInt32.Parse(data.Substring(18, 2), System.Globalization.NumberStyles.HexNumber)
            answer = "ИНС: " + InsConverter(data.Substring(18, 11));
            answer += " Данные одиночного сообщения ";
            answer += "НЗпрд: " + Convert.ToString(UInt32.Parse(data.Substring(30, 2), System.Globalization.NumberStyles.HexNumber));
            answer += " ПРГ: " + Convert.ToString(UInt32.Parse(data.Substring(33, 2), System.Globalization.NumberStyles.HexNumber));
            answer += " РП: ";
            switch (UInt32.Parse(data.Substring(36, 2), System.Globalization.NumberStyles.HexNumber))
            {
                case 0: answer += "режим не задан "; break;
                case 1: answer += "Чайка однократный "; break;
                case 2: answer += "Чайка двукратный "; break;
                case 3: answer += "Чайка «Аккорд» "; break;
                case 4: answer += "Чайка «Мелодия» "; break;
                case 5: answer += "Перевал короткий код "; break;
                case 6: answer += "Перевал длинный код "; break;
                case 7: answer += "безызбыточные сообщения "; break;
            }
            answer += " НП: " + Convert.ToString(UInt32.Parse(data.Substring(39, 2), System.Globalization.NumberStyles.HexNumber));
            answer += " ДП: " + Convert.ToString(UInt32.Parse(data.Substring(42, 2), System.Globalization.NumberStyles.HexNumber));
            int channel = Int32.Parse(data.Substring(45, 2), System.Globalization.NumberStyles.HexNumber);
            answer += " КАН: ";
            if (channel != 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (channel >= Math.Pow(2, i))
                    {
                        channel -= (int)Math.Pow(2, i);
                        answer += Convert.ToString(i+1) + " ";

                    }

                }
            }
            else
            {
                channel = Int32.Parse(data.Substring(48, 2), System.Globalization.NumberStyles.HexNumber);
                for (int i = 0; i < channel; i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        if (IpConverter(data.Substring(51 + (i * 36), 11)).Equals(settings.Configuration[j, 0].Text)
                            && PortConverter(data.Substring(63 + (i * 36), 5)).Equals(settings.Configuration[j, 1].Text)
                            && MacConverter(data.Substring(69 + (i * 36), 17)).Equals(settings.Configuration[j, 2].Text))
                        {
                            answer += Convert.ToString(j + 1) + " ";
                            break;
                        }
                        
                    }
                    answer += "Неизвестный ";
                }
                answer += "Радиоканалы ";
            }
        }
        private void Parse_02_TCP(ref string answer, string data)
        {
            //UInt32.Parse(data.Substring(18, 2), System.Globalization.NumberStyles.HexNumber)
            answer = "ИНС: " + InsConverter(data.Substring(18, 11));
            answer += " Данные составного сообщения ";
            answer += "НЗпрд: " + Convert.ToString(UInt32.Parse(data.Substring(30, 2), System.Globalization.NumberStyles.HexNumber));
            answer += " ПРГ: " + Convert.ToString(UInt32.Parse(data.Substring(33, 2), System.Globalization.NumberStyles.HexNumber));
            answer += " РП: ";
            switch (UInt32.Parse(data.Substring(36, 2), System.Globalization.NumberStyles.HexNumber))
            {
                case 0: answer += "режим не задан "; break;
                case 1: answer += "Чайка однократный "; break;
                case 2: answer += "Чайка двукратный "; break;
                case 3: answer += "Чайка «Аккорд» "; break;
                case 4: answer += "Чайка «Мелодия» "; break;
                case 5: answer += "Перевал короткий код "; break;
                case 6: answer += "Перевал длинный код "; break;
                case 7: answer += "безызбыточные сообщения "; break;
            }
            answer += " НП: " + Convert.ToString(UInt32.Parse(data.Substring(39, 2), System.Globalization.NumberStyles.HexNumber));
            answer += " ДП: " + Convert.ToString(UInt32.Parse(data.Substring(42, 2), System.Globalization.NumberStyles.HexNumber));
            int channel = Int32.Parse(data.Substring(45, 2), System.Globalization.NumberStyles.HexNumber);
            if (channel != 0)
            {
                answer += " КАН: ";

                for (int i = 0; i < 8; i++)
                {
                    if (channel >= Math.Pow(2, i))
                    {
                        channel -= (int)Math.Pow(2, i);
                        answer += Convert.ToString(i) + " ";

                    }

                }
            }
            else
            {
                channel = Int32.Parse(data.Substring(48, 2), System.Globalization.NumberStyles.HexNumber);
                for (int i = 0; i < channel; i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        if (IpConverter(data.Substring(51 + (i * 36), 11)).Equals(settings.Configuration[j, 0].Text)
                            && PortConverter(data.Substring(63 + (i * 36), 5)).Equals(settings.Configuration[j, 1].Text)
                            && MacConverter(data.Substring(69 + (i * 36), 17)).Equals(settings.Configuration[j, 2].Text))
                        {
                            answer += Convert.ToString(j + 1) + " ";
                            break;
                        }
                    }
                    answer += "Неизвестный ";
                }
                answer += "Радиоканалы ";
            }
        }
        private void Parse_03_TCP(ref string answer, string data)
        {
            byte[] twobytes = new byte[2];
            answer = "ИНС: " + InsConverter(data.Substring(18, 11));
            answer += "Результат передачи данных ";
            twobytes[0] = byte.Parse(data.Substring(30, 2), System.Globalization.NumberStyles.HexNumber);
            twobytes[1] = byte.Parse(data.Substring(33, 2), System.Globalization.NumberStyles.HexNumber);
            answer += "НС: " + Convert.ToString(BitConverter.ToInt16(twobytes, 0));
            twobytes[0] = byte.Parse(data.Substring(36, 2), System.Globalization.NumberStyles.HexNumber);
            twobytes[1] = byte.Parse(data.Substring(39, 2), System.Globalization.NumberStyles.HexNumber);
            answer += " КС: " + Convert.ToString(BitConverter.ToInt16(twobytes, 0));
            answer += " КСП: ";
            switch (Int32.Parse(data.Substring(42, 2), System.Globalization.NumberStyles.HexNumber))
            {
                case 0: answer += "передача выполнена "; break;
                case 1: answer += "таймаут ожидания сигнала «Излучение включено» "; break;
                case 2: answer += "преждевременное отключение сигнала «Излучение включено» "; break;
                case 3: answer += "передача выполнена, но нет отключения сигнала «Излучение включено» в ответ на отключение сигнала «Включить излучение»;  "; break;
                case 4: answer += "передача прекращена по команде «Сбросить канал» или «Прекратить передачу» "; break;
                case 5: answer += "ошибка формата сообщения данных "; break;
                case 6: answer += "неисправность Т-821-06 "; break;
                case 7: answer += "Ошибка передачи данных (от радиостанции) "; break;
                case 8: answer += "сообщение не доведено "; break;
                case 9: answer += "отказ модуля МПДИ-01 "; break;
                case 10: answer += "отказ модуля МПДИ-02 "; break;
                case 15: answer += "не задан режим канала  "; break;
            }
            int channel = Int32.Parse(data.Substring(45, 2), System.Globalization.NumberStyles.HexNumber);
            if (channel != 0)
            {
                answer += " КАН: ";

                for (int i = 0; i < 8; i++)
                {
                    if (channel >= Math.Pow(2, i))
                    {
                        channel -= (int)Math.Pow(2, i);
                        answer += Convert.ToString(i) + " ";

                    }

                }
            }
            else
            {
                channel = Int32.Parse(data.Substring(48, 2), System.Globalization.NumberStyles.HexNumber);
                for (int i = 0; i < channel; i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        if (IpConverter(data.Substring(51 + (i * 36), 11)).Equals(settings.Configuration[j, 0].Text)
                            && PortConverter(data.Substring(63 + (i * 36), 5)).Equals(settings.Configuration[j, 1].Text)
                            && MacConverter(data.Substring(69 + (i * 36), 17)).Equals(settings.Configuration[j, 2].Text))
                        {
                            answer += Convert.ToString(j + 1) + " ";
                            break;
                        }
                    }
                    answer += "Неизвестный ";
                }
                answer += "Радиоканалы ";
            }
        }
        private void Parse_0A_TCP(ref string answer, string data)
        {

            answer = "Установление режима канала/Режим канала";
            answer += " КАН: " + Convert.ToString(Math.Log(int.Parse(data.Substring(18, 2), System.Globalization.NumberStyles.HexNumber), 2));
            answer += " СК: " + Convert.ToString(int.Parse(data.Substring(21, 2), System.Globalization.NumberStyles.HexNumber));
            answer += " ШЛ: " + Convert.ToString(int.Parse(data.Substring(24, 2), System.Globalization.NumberStyles.HexNumber));
            answer += " НЗпрм: " + Convert.ToString(int.Parse(data.Substring(27, 2), System.Globalization.NumberStyles.HexNumber));
            answer += " ППРЧ: " + Convert.ToString(int.Parse(data.Substring(30, 2), System.Globalization.NumberStyles.HexNumber));
            answer += " ТС: " + Convert.ToString(int.Parse(data.Substring(33, 2), System.Globalization.NumberStyles.HexNumber));
            answer += " КВТ: " + Convert.ToString(int.Parse(data.Substring(36, 2), System.Globalization.NumberStyles.HexNumber));

        }
        private void Parse_0B_TCP(ref string answer, string data)
        {
            answer = "Установка времени";
            answer += " ЧЧ: " + Convert.ToString(int.Parse(data.Substring(18, 2), System.Globalization.NumberStyles.HexNumber));
            answer += " ММ: " + Convert.ToString(int.Parse(data.Substring(21, 2), System.Globalization.NumberStyles.HexNumber));
            answer += " СС: " + Convert.ToString(int.Parse(data.Substring(24, 2), System.Globalization.NumberStyles.HexNumber));

        }
        private void Parse_0C_TCP(ref string answer, string data)
        {
            answer = " Установка режимов ЗАС";
            answer += " ЗАС1: " + Convert.ToString(int.Parse(data.Substring(18, 2), System.Globalization.NumberStyles.HexNumber));
            answer += " ЗАС2: " + Convert.ToString(int.Parse(data.Substring(21, 2), System.Globalization.NumberStyles.HexNumber));

        }
        private void Parse_0D_TCP(ref string answer, string data)
        {
            answer = "Сброс канала в исходное состояние";
            int channel = Int32.Parse(data.Substring(18, 2), System.Globalization.NumberStyles.HexNumber);
            if (channel != 0)
            {
                answer += " КАН: ";

                for (int i = 0; i < 8; i++)
                {
                    if (channel >= Math.Pow(2, i))
                    {
                        channel -= (int)Math.Pow(2, i);
                        answer += Convert.ToString(i) + " ";

                    }

                }
            }
            else
            {
                channel = Int32.Parse(data.Substring(48, 2), System.Globalization.NumberStyles.HexNumber);
                for (int i = 0; i < channel; i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        if (IpConverter(data.Substring(51 + (i * 36), 11)).Equals(settings.Configuration[j, 0].Text)
                            && PortConverter(data.Substring(63 + (i * 36), 5)).Equals(settings.Configuration[j, 1].Text)
                            && MacConverter(data.Substring(69 + (i * 36), 17)).Equals(settings.Configuration[j, 2].Text))
                        {
                            answer += Convert.ToString(j + 1) + " ";
                            break;
                        }
                    }
                    answer += "Неизвестный ";
                }
                answer += "Радиоканалы ";
            }
        }
        private void Parse_0E_TCP(ref string answer, string data)
        {
            answer = "Прекращение передачи данных";
            int channel = Int32.Parse(data.Substring(18, 2), System.Globalization.NumberStyles.HexNumber);
            if (channel != 0)
            {
                answer += " КАН: ";

                for (int i = 0; i < 8; i++)
                {
                    if (channel >= Math.Pow(2, i))
                    {
                        channel -= (int)Math.Pow(2, i);
                        answer += Convert.ToString(i) + " ";

                    }

                }
            }
            else
            {
                channel = Int32.Parse(data.Substring(48, 2), System.Globalization.NumberStyles.HexNumber);
                for (int i = 0; i < channel; i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        if (IpConverter(data.Substring(51 + (i * 36), 11)).Equals(settings.Configuration[j, 0].Text)
                            && PortConverter(data.Substring(63 + (i * 36), 5)).Equals(settings.Configuration[j, 1].Text)
                            && MacConverter(data.Substring(69 + (i * 36), 17)).Equals(settings.Configuration[j, 2].Text))
                        {
                            answer += Convert.ToString(j + 1) + " ";
                            break;
                        }
                    }
                    answer += "Неизвестный ";
                }
                answer += "Радиоканалы ";
            }
        }
        private void Parse_0F_TCP(ref string answer, string data)
        {
            answer = "Разрешение обработки составных сообщений на приёме";
            answer += " ОСП: " + Convert.ToString(Math.Log(int.Parse(data.Substring(18, 2), System.Globalization.NumberStyles.HexNumber), 2));

        }
        private void Parse_10_TCP(ref string answer, string data)
        {
            answer = "Установка режима взаимодействия с Ethernet радиостанцией/Режим взаимодействия с Ethernet радиостанцией";
            answer += " Адр: ";
            for (int j = 0; j < 32; j++)
            {
                if (IpConverter(data.Substring(18, 11)).Equals(settings.Configuration[j, 0].Text)
                    && PortConverter(data.Substring(30, 5)).Equals(settings.Configuration[j, 1].Text)
                    && MacConverter(data.Substring(36, 17)).Equals(settings.Configuration[j, 2].Text)
                    && IpConverter(data.Substring(54, 11)).Equals(settings.Configuration[j, 3].Text)
                    && PortConverter(data.Substring(66, 5)).Equals(settings.Configuration[j, 4].Text)
                    && MacConverter(data.Substring(72, 17)).Equals(settings.Configuration[j, 5].Text))
                {
                    answer += Convert.ToString(j + 1) + " радиоканал ";
                    break;
                }
                answer += "Неизвестный радиоканал ";
            }
            answer += " КАН: ";
            int channel = Int32.Parse(data.Substring(90, 2), System.Globalization.NumberStyles.HexNumber);
            for (int i = 0; i < 8; i++)
            {
                if (channel >= Math.Pow(2, i))
                {
                    channel -= (int)Math.Pow(2, i);
                    answer += Convert.ToString(i) + " ";

                }
            }
            answer += " ШЛ: " + Convert.ToString(UInt32.Parse(data.Substring(93, 2), System.Globalization.NumberStyles.HexNumber));
            answer += " НЗпрм: " + Convert.ToString(UInt32.Parse(data.Substring(96, 2), System.Globalization.NumberStyles.HexNumber));
            answer += " КП: " + Convert.ToString(UInt32.Parse(data.Substring(99, 2), System.Globalization.NumberStyles.HexNumber));
            answer += " КПП: " + Convert.ToString(UInt32.Parse(data.Substring(102, 2), System.Globalization.NumberStyles.HexNumber));
            byte[] twobytes = new byte[2];
            twobytes[0] = byte.Parse(data.Substring(105, 2), System.Globalization.NumberStyles.HexNumber);
            twobytes[1] = byte.Parse(data.Substring(108, 2), System.Globalization.NumberStyles.HexNumber);
            answer += " РД: " + Convert.ToString(BitConverter.ToInt16(twobytes, 0));
            answer += " КВ: " + Convert.ToString(UInt32.Parse(data.Substring(111, 2), System.Globalization.NumberStyles.HexNumber));
        }
        private void Parse_14_TCP(ref string answer, string data)
        {
            answer = "Разрешение выдачи в БЦВК донесений \"Состояние канала\"";
            answer += " КВ: " + Convert.ToString(UInt32.Parse(data.Substring(111, 2), System.Globalization.NumberStyles.HexNumber));
        }
        private void Parse_7F_TCP(ref string answer, string data)
        {
            answer = "PrintF: ";
            string[] bytes = data.Split(' ');
            byte[] printf = new byte[bytes.Length];
            for (int i = 0; i < printf.Length; i++)
            { printf[i] = byte.Parse(bytes[i], System.Globalization.NumberStyles.HexNumber); }
            answer += ascii.GetString(printf);


        }
        private void Parse_FF_TCP(ref string answer, string data)
        {
            answer = "Контроль соединения";
        }
        #endregion
        #region firstByteParse_UDP
        private void Parse_00_UDP(ref string answer, string data)
        {
            byte[] twobytes = new byte[2];
            answer += " Команда/донесение <Передать данные> ";
            twobytes[0] = byte.Parse(data.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
            twobytes[1] = byte.Parse(data.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            answer += " Номер сообщения: " + Convert.ToString(BitConverter.ToInt16(twobytes, 0));
            twobytes[0] = byte.Parse(data.Substring(9, 2), System.Globalization.NumberStyles.HexNumber);
            twobytes[1] = byte.Parse(data.Substring(12, 2), System.Globalization.NumberStyles.HexNumber);
            answer += " Номер пакета: " + Convert.ToString(BitConverter.ToInt16(twobytes, 0));
            answer += " ПСВИ: " + Convert.ToString(UInt32.Parse(data.Substring(15, 2), System.Globalization.NumberStyles.HexNumber));
            answer += " Режим выдачи: " + Convert.ToString(UInt32.Parse(data.Substring(18, 2), System.Globalization.NumberStyles.HexNumber));
        }
        private void Parse_01_UDP(ref string answer, string data)
        {
            byte[] twobytes = new byte[2];
            answer += " Сбросить передачу данных";
            twobytes[0] = byte.Parse(data.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
            twobytes[1] = byte.Parse(data.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            answer += " Номер сообщения: " + Convert.ToString(BitConverter.ToInt16(twobytes, 0));
        }
        private void Parse_02_UDP(ref string answer, string data)
        {
            byte[] twobytes = new byte[2];
            answer += " Состояние излучения";
            twobytes[0] = byte.Parse(data.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
            twobytes[1] = byte.Parse(data.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            answer += " Номер сообщения: " + Convert.ToString(BitConverter.ToInt16(twobytes, 0));
            answer += " ПС: " + Convert.ToString(UInt32.Parse(data.Substring(9, 2), System.Globalization.NumberStyles.HexNumber));
        }
        private void Parse_03_UDP(ref string answer, string data)
        {
            byte[] twobytes = new byte[2];
            answer += " Квитанция о получении";
            twobytes[0] = byte.Parse(data.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
            twobytes[1] = byte.Parse(data.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            answer += " Номер сообщения: " + Convert.ToString(BitConverter.ToInt16(twobytes, 0));
            answer += " Статус сообщения: " + Convert.ToString(UInt32.Parse(data.Substring(9, 2), System.Globalization.NumberStyles.HexNumber));
        }
        private void Parse_04_UDP(ref string answer, string data)
        {
            byte[] twobytes = new byte[2];
            answer += " начало передачи данных в радиоканал ";
            twobytes[0] = byte.Parse(data.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
            twobytes[1] = byte.Parse(data.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            answer += " Номер сообщения: " + Convert.ToString(BitConverter.ToInt16(twobytes, 0));
            twobytes[0] = byte.Parse(data.Substring(9, 2), System.Globalization.NumberStyles.HexNumber);
            twobytes[1] = byte.Parse(data.Substring(12, 2), System.Globalization.NumberStyles.HexNumber);
            answer += " Номер пакета: " + Convert.ToString(BitConverter.ToInt16(twobytes, 0));
        }
        private void Parse_05_UDP(ref string answer, string data)
        {
            byte[] twobytes = new byte[2];
            answer += " Ошибка передачи данных ";
            twobytes[0] = byte.Parse(data.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
            twobytes[1] = byte.Parse(data.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            answer += " Номер сообщения: " + Convert.ToString(BitConverter.ToInt16(twobytes, 0));
            twobytes[0] = byte.Parse(data.Substring(9, 2), System.Globalization.NumberStyles.HexNumber);
            twobytes[1] = byte.Parse(data.Substring(12, 2), System.Globalization.NumberStyles.HexNumber);
            answer += " Номер пакета: " + Convert.ToString(BitConverter.ToInt16(twobytes, 0));
        }
        private void Parse_06_UDP(ref string answer, string data)
        {
            byte[] twobytes = new byte[2];
            answer += " Данные выданы в радиоканал ";
            twobytes[0] = byte.Parse(data.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
            twobytes[1] = byte.Parse(data.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            answer += " Номер сообщения: " + Convert.ToString(BitConverter.ToInt16(twobytes, 0));
            twobytes[0] = byte.Parse(data.Substring(9, 2), System.Globalization.NumberStyles.HexNumber);
            twobytes[1] = byte.Parse(data.Substring(12, 2), System.Globalization.NumberStyles.HexNumber);
            answer += " Номер пакета: " + Convert.ToString(BitConverter.ToInt16(twobytes, 0));
        }
        private void Parse_07_UDP(ref string answer, string data)
        {
            byte[] twobytes = new byte[2];
            answer += " Передача данных прекращена";
            twobytes[0] = byte.Parse(data.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
            twobytes[1] = byte.Parse(data.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            answer += " Номер сообщения: " + Convert.ToString(BitConverter.ToInt16(twobytes, 0));
        }
        #endregion 
    }
}
