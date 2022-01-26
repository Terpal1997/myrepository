using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
namespace test
{
    public class functions
    {
        static Random rand = new Random();
        public static byte[] objectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        public static byte[] PPRCh(int pprch)
        {
            string pprchMessage = "";
            switch (pprch)
            {
                case 0:
                    pprchMessage = "Отключен";
                    break;
                case 1:
                    pprchMessage = "Включен";
                    break;
            }
            System.Console.WriteLine(" Режим ППРЧ -- {0}", pprchMessage);
            Byte[] byteArr1 = new Byte[1];
            byteArr1[0] = Convert.ToByte(pprch);
            return byteArr1;
        }
        public static void BiNuSy(int num1) //КОСЯК с возвращением
        {
            string str1;
            string str = Convert.ToString(num1, 2);
            while (str.Length < 8)
            {
                str1 = "0" + str;
                str = str1;
            }
            System.Console.WriteLine("используемые порты {0}", str);
        }
        public static byte[] PortAdress(int mode, int special = 7) //КОСЯК с возвращением
        {
            if (mode == 0)
            {
                byte[] port = new byte[1];
                if (special <= 6)
                {
                    port[0] = Convert.ToByte(Math.Pow(2, special) + Math.Pow(2, special + 1));//на случай чайки
                }
                else
                {
                    port[0] = Convert.ToByte(rand.Next(0x00, 0x100));
                }
                System.Console.WriteLine(" используемые каналы {0}", BitConverter.ToString(port));// спросить у Сергея
                return port;
            }
            else if (mode == 1)
            {
                int NumberOfAdresses = rand.Next(1, 8);
                Byte[] byteArr = new Byte[2 + NumberOfAdresses * 6];
                byteArr[1] = Convert.ToByte(NumberOfAdresses);
                for (int j = 2; j < byteArr.Length; j++)
                {
                    byteArr[j] = (Byte)(rand.Next(0x00, 0x100));//d1...dn
                }
                System.Console.WriteLine(" количество портов + ip {0}", BitConverter.ToString(byteArr));
                return byteArr;
            }
            Byte[] byteArr1 = new Byte[1];
            return byteArr1;/////////////////////////////////////////////////////////////////////////// пустое возвращение
        }
        public static byte[] DataMessage(int MessageSize, int mode = 0, bool onlyNulls = false, bool onlyNumberOfChannels = false, int numberOfChannels = 0)
        {
            int FilledMessageSize = 0;

            if (mode == 0)
            {
                if (MessageSize <= 17)
                    FilledMessageSize = 17;
                else if (MessageSize <= 48)
                    FilledMessageSize = 48;
                else if (MessageSize <= 80)
                    FilledMessageSize = 80;
                else if (MessageSize <= 112)
                    FilledMessageSize = 112;
                /*
Для одиночной кодограммы «Перевал»: n = 17, 48, 80 или 112, что соответствует 
1, 2, 3 или 4-блочному сообщению. Если n не равно 17, 48, 80, 112, то «Прима-БМ» дополняет массив нулевыми байтами до ближайшего значения.
Младший разряд кодограммы размещается в младшем разряде байта Д1. При использовании УКВС, САЧ входит в массив Д1…Дn;
младший разряд байта Д1 занимает младший разряд номера формуляра. 
                 */
                System.Console.WriteLine("Перевал одиночный");
                Byte[] byteArr = new Byte[FilledMessageSize]; //длина сообщения 17,48,80, 112
                if (!onlyNulls)
                {
                    if (!onlyNumberOfChannels)
                    {
                        for (int j = 0; j < byteArr.Length; j++)//определить САЧ и младший разряд номера формуляра
                        {
                            byteArr[j] = (Byte)(rand.Next(0x00, 0xff));
                        }
                    }
                    else
                    {
                        for (int j = 0; j < byteArr.Length; j++)//определить САЧ и младший разряд номера формуляра
                        {
                            byteArr[j] = (Byte)numberOfChannels;
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < byteArr.Length; j++)//определить САЧ и младший разряд номера формуляра
                    {
                        byteArr[j] = 0;
                    }
                }
                return byteArr;
            }
            if (mode == 1)  //Чайка
            {
                if (MessageSize <= 15)
                    FilledMessageSize = 15;
                else if (MessageSize <= 62)
                    FilledMessageSize = 62;
                Byte[] byteArr = new Byte[FilledMessageSize]; //длина сообщения 15, 62, первые 9 разрядов адресная часть - заполнить
                if (!onlyNulls)
                {
                    if (!onlyNumberOfChannels)
                    {
                        for (int j = 9; j < byteArr.Length; j++)
                        {
                            byteArr[j] = (Byte)(rand.Next(0x00, 0x20));//d1...dn
                        }
                    }
                    else
                    {
                        for (int j = 0; j < byteArr.Length; j++)//определить САЧ и младший разряд номера формуляра
                        {
                            byteArr[j] = (Byte)numberOfChannels;
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < byteArr.Length; j++)//определить САЧ и младший разряд номера формуляра
                    {
                        byteArr[j] = 0;
                    }
                }
                return byteArr;
            }
            if (mode == 2)  //непрерывный
            {
                Byte[] byteArr = new Byte[MessageSize]; //длина сообщения 15, 62, первые 9 разрядов адресная часть - заполнить

                if (!onlyNulls)
                {
                    if (!onlyNumberOfChannels)
                    {
                        for (int j = 0; j < byteArr.Length; j++) //генератор сообщения перевал
                        {
                            byteArr[j] = (Byte)(rand.Next(0x00, 0xff));//d1...dn
                        }
                    }
                    else
                    {
                        for (int j = 0; j < byteArr.Length; j++)//определить САЧ и младший разряд номера формуляра
                        {
                            byteArr[j] = (Byte)numberOfChannels;
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < byteArr.Length; j++)//определить САЧ и младший разряд номера формуляра
                    {
                        byteArr[j] = 0;
                    }
                }
                return byteArr;
            }
            Byte[] byteArr1 = new Byte[1];
            return byteArr1;
        }
        public static int ArrayLength(byte[] messageToSend)//считает длину команды 5 - начальный байт + адрес, остальные 4 значения - длина всего остального сообщения
        {
            return 5 + messageToSend[1] + messageToSend[2] * 256 + messageToSend[3] * 256 * 256 + messageToSend[4] * 256 * 256 * 256;
        }
        public static byte[] DB(int db)
        {
            System.Console.WriteLine("Максимальный размер полезного блока данных пакета Ethernet -- {0}", db);
            Byte[] byteArr1 = new Byte[2];
            byteArr1 = (BitConverter.GetBytes(db));
            return byteArr1;
        }
        public static byte[] ZAS(int zas)// зависит ли от номера зас его настройка?
        {
            Byte[] byteArr1 = new Byte[1];
            byteArr1[0] = Convert.ToByte(zas);
            return byteArr1;
        }
        public static byte[] KV(int kv)// зависит ли от номера зас его настройка?
        {
            string kvMessage = "";
            switch (kv)
            {
                case 0:
                    kvMessage = "отключена";
                    break;
                case 1:
                    kvMessage = "Включена";
                    break;

            }
            System.Console.WriteLine("квитирование сообщений от радиостанций -- {0}", kvMessage);
            Byte[] byteArr1 = new Byte[1];
            byteArr1[0] = Convert.ToByte(kv);
            return byteArr1;
        }
        public static byte[] Kvit(int kvit)
        {
            string kvitMessage = "";
            switch (kvit)
            {
                case 0:
                    kvitMessage = "Отключен";
                    break;
                case 1:
                    kvitMessage = "Включен";
                    break;
            }
            System.Console.WriteLine(" Режим квитирования-- {0}", kvitMessage);
            Byte[] byteArr1 = new Byte[1];
            byteArr1[0] = Convert.ToByte(kvit);
            return byteArr1;
        }
        public static byte[] PRG(int prg)
        {
            System.Console.WriteLine(" номер прораммы Т-821-06 -- {0}", prg);
            Byte[] byteArr1 = new Byte[1];
            byteArr1[0] = (byte)Convert.ToByte(prg);
            return byteArr1;
        }
        public static byte[] RP(int rp)
        {
            string rpMessage = "";
            Byte[] PA = new Byte[1];
            PA[0] = (byte)Convert.ToByte(0);
            switch (rp)
            {
                case 0:
                    rpMessage = "Режим не задан";
                    break;
                case 1:
                    rpMessage = "Чайка однократный";

                    break;
                case 2:
                    rpMessage = "Чайка двукратный";
                    break;
                case 3:
                    {
                        rpMessage = "Чайка 'Аккорд'";////////////////////////////
                        int channel = rand.Next(0x00, 0x07);
                        PA = PortAdress(0, channel);
                    }
                    break;
                case 4:
                    {
                        rpMessage = "Чайка 'Мелодия'";///////////////
                        int channel = rand.Next(0x00, 0x07);
                        PA = PortAdress(0, channel);
                    }
                    break;
                case 5:
                    {
                        rpMessage = "Перевал короткий код";
                    }
                    break;
                case 6:
                    rpMessage = "Перевал длинный код";
                    break;
                case 7:
                    rpMessage = "Безызботочные сообщения";
                    break;
            }
            System.Console.WriteLine(" Режим  передачи 'Перевал'-- {0}", rpMessage);
            Byte[] byteArr1 = new Byte[2];
            byteArr1[0] = (byte)Convert.ToByte(rp);
            byteArr1[1] = PA[0]; // в случае чайки аккорд
            return byteArr1;
        }
        public static byte[] NP(int np)
        {
            string npMessage = "";
            switch (np)
            {
                case 0:
                    npMessage = "отключать излучение по окончании передачи в радиоканал";
                    break;
                case 1:
                    npMessage = "не отключать излучение по окончании передачи в радиоканал";
                    break;
            }
            System.Console.WriteLine(" Непрерывный режим работы передатчика -- {0}", npMessage);
            Byte[] byteArr1 = new Byte[1];
            byteArr1[0] = (byte)Convert.ToByte(np);
            return byteArr1;
        }
        public static byte[] DD(int dd)
        {
            Byte[] byteArr1 = (BitConverter.GetBytes(dd));
            return byteArr1;
        }
        static int insBCVK = 0;
        public static byte[] INS()
        {
            Byte[] byteArr1 = (BitConverter.GetBytes(insBCVK));
            insBCVK++;
            return byteArr1;
        }
        public static byte[] INSDlyaPrima(int ins)/////////////////////////////доделать
        {
            string insMessage = "";
            switch (ins)
            {
                case 100:
                    insMessage = "сообщение было получено полностью";
                    break;
                default:
                    insMessage = "сообщение было получено на " + ins + "процентов";
                    break;
            }
            System.Console.WriteLine("  {0}", insMessage);
            Byte[] byteArr1 = new Byte[1];
            byteArr1[0] = (byte)Convert.ToByte(ins);
            return byteArr1;/////////////////////////////////////////////////// пока на 100 потом исправлять
        }
       
        public static byte[] NF(int nf)
        {
            System.Console.WriteLine("Имя файла");
            Byte[] byteArr = new Byte[nf];
            for (int j = 0; j < nf - 1; j++)
            {
                byteArr[j] = (Byte)(rand.Next(0x00, 0x100));
            }
            byteArr[nf - 1] = (Byte)Convert.ToByte(0x00);
            System.Console.WriteLine(" {0}", BitConverter.ToString(byteArr));
            return byteArr;
        }
        public static byte[] KOD(int kod)
        {
            string kodMessage = "";
            switch (kod)
            {
                case 0:
                    kodMessage = "команда выполнена или принята к выполнению";
                    break;
                case 1:
                    kodMessage = "ошибка формата";
                    break;
                case 2:
                    kodMessage = "невозможно выполнить";
                    break;
                default:
                    kodMessage = "отказ канала";
                    break;
            }
            System.Console.WriteLine("{0}", kodMessage);
            Byte[] byteArr1 = new Byte[1];
            byteArr1[0] = Convert.ToByte(kod);
            return byteArr1;
        }
        public static byte[] DT(int dt)
        {
            string datetime = DateTime.Now.ToString();
            string[] datetimeS = new string[2];
            datetimeS = (datetime.Split(new char[] { }));
            switch (dt)
            {
                case 0:
                    {
                        string[] dateS = new string[3];
                        byte[] dateB = new byte[3];
                        dateS = (datetimeS[0].Split(new char[] { '.' }));
                        dateS[2] = dateS[2].Substring(2);
                        for (int i = 0; i < 3; i++)
                        {
                            dateB[i] = (byte)Convert.ToByte(Int16.Parse(dateS[i]));
                        }
                        Console.WriteLine("{0} : {1}", "дата", BitConverter.ToString(dateB));
                        Byte[] byteArr1 = dateB;
                        return byteArr1;
                    }
                case 1:
                    {
                        string[] timeS = new string[3];
                        byte[] timeB = new byte[3];
                        timeS = (datetimeS[1].Split(new char[] { ':' }));
                        for (int i = 0; i < 3; i++)
                        {
                            timeB[i] = (byte)Convert.ToByte(Int16.Parse(timeS[i]));
                        }
                        Console.WriteLine("{0} : {1}", "время", BitConverter.ToString(timeB));
                        Byte[] byteArr1 = timeB;
                        return byteArr1;
                    }
                case 2:
                    {
                        string[] dateS = new string[3];
                        byte[] dateB = new byte[3];
                        string[] timeS = new string[3];
                        byte[] timeB = new byte[3];
                        dateS = (datetimeS[0].Split(new char[] { '.' }));
                        timeS = (datetimeS[1].Split(new char[] { ':' }));
                        dateS[2] = dateS[2].Substring(2);
                        for (int i = 0; i < 3; i++)
                        {
                            dateB[i] = (byte)Convert.ToByte(Int16.Parse(dateS[i]));
                            timeB[i] = (byte)Convert.ToByte(Int16.Parse(timeS[i]));
                        }
                        Byte[] byteArr1 = new Byte[dateB.Length + timeB.Length];
                        for (int i = 0; i < 3; i++)
                        {
                            byteArr1[i] = dateB[i];
                            byteArr1[i + 3] = timeB[i];
                        }
                        return byteArr1;
                    }
                case 3:
                    {
                        byte[] byteArr1 = new byte[6];
                        Console.WriteLine("Так как Имя не задано - значения Даты и времени = 0");
                        for (int i = 0; i < 6; i++)
                        {
                            byteArr1[i] = (byte)(0);
                        }
                        return byteArr1;
                    }
            }
            Byte[] byteArr11 = new Byte[1];
            return byteArr11;
        }
        public static byte[] DP(int dp) // не используется пока
        {
            byte[] Dp = new byte[1];
            System.Console.WriteLine(" Длина преамбулы в 10 мс {0}", BitConverter.ToString(Dp));
            Byte[] byteArr1 = new Byte[1];
            byteArr1[0] = (byte)Convert.ToByte(dp);
            return byteArr1;
        }
        public static byte[] State(int st1, int st2) // не используется пока
        {
            string st11; string st12; string st13; string st14; string st15; string st16; string st17; string st18; string st21; string st22; string st23; string st24; string st25; string st26;
            st11 = (st1 % 2 == 1) ? "Исправен" : "Отказ";
            st12 = (st1 % 4 >= 2) ? "Исправен" : "Отказ";
            st13 = (st1 % 8 >= 4) ? "Исправен" : "Отказ";
            st14 = (st1 % 16 >= 8) ? "Исправен" : "Отказ";
            st15 = (st1 % 32 >= 16) ? "Исправен" : "Отказ";
            st16 = (st1 % 64 >= 32) ? "Исправен" : "Отказ";
            st17 = (st1 % 128 >= 64) ? "Исправен" : "Отказ";
            st18 = (st1 > 127) ? "Исправен" : "Отказ";
            st21 = (st2 % 2 == 1) ? "Исправен" : "Отказ";
            st22 = (st2 % 4 >= 2) ? "Исправен" : "Отказ";
            st23 = (st2 % 8 >= 4) ? "Исправен" : "Отказ";
            st24 = (st2 % 16 >= 8) ? "Исправен" : "Отказ";
            st25 = (st2 % 32 >= 16) ? "Исправен" : "Отказ";
            st26 = (st2 % 64 >= 32) ? "Исправен" : "Отказ";
            System.Console.WriteLine(@"Состояние: \n 1ый Канал {0} \n 2ой Канал {1} \n 3ий Канал {2} \n 4ый Канал {3} \n 5ый Канал {4} \n 6ой Канал {5} 7ой Канал {6} \n 8ой Канал {7} \n
Устройство управления {8} \n Контроллер Ethernet радиостанции {9} \n Контроллер ЗАС1 {10} \n Контроллер ЗАС2 {11} \n Состояние ЗАС1 {12} \n Состояние ЗАС2 {13}",
                st11, st12, st13, st14, st15, st16, st17, st18, st21, st22, st23, st24, st25, st26);
            Byte[] byteArr1 = new Byte[2];
            byteArr1[0] = (byte)Convert.ToByte(st1);
            byteArr1[1] = (byte)Convert.ToByte(st2);
            return byteArr1;
        }
        public static byte[] NSKS(int ns, int ks)
        {
            ns = ns + (ks * 65536);
            Byte[] byteArr = (BitConverter.GetBytes(ns));
            System.Console.WriteLine("Номер сегмента и Количество сегментов{0} \n ", BitConverter.ToString(byteArr));
            return byteArr;
        }
        public static byte[] ChChMMSS(int chch)
        {
            int[] timeI = new int[] { 24, 60, 60 };
            bool NormalForm = false;
            System.Console.WriteLine("Напишите время в формате ЧЧ.ММ.СС: ");
            do
            {
                string time = System.Console.ReadLine();
                string[] timeS = time.Split(new char[] { '.' });
                if (timeS.Length != 3)
                {
                    System.Console.WriteLine("неверный формат");
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        timeI[i] = Convert.ToInt16(timeS[i]);
                    }
                    if (timeI[0] < 24 && timeI[1] < 60 && timeI[2] < 60)
                    {
                        NormalForm = true;
                    }
                    else
                    {
                        System.Console.WriteLine("неверно задано время");
                    }
                }
            }
            while (NormalForm == false);
            Byte[] byteArr = new Byte[3];
            for (int t = 0; t < 3; t++)
            {
                byteArr[t] = Convert.ToByte(timeI[t]);
            }
            System.Console.WriteLine("переведенное время \n ", BitConverter.ToString(byteArr));
            return byteArr;
        }
        public static byte[] KSP(int ksp)
        {
            string KSPMessage = "";
            switch (ksp)
            {
                case 0:
                    KSPMessage = "Передача выполнена";
                    break;
                case 1:
                    KSPMessage = "Таймаут ожидания сигнала 'Излучение выключено'";
                    break;
                case 2:
                    KSPMessage = "Преждевеременное отключение сигнала сигнала 'Излучение выключено'";
                    break;
                case 3:
                    KSPMessage = "Передача выполнена, но не отключение сигнала 'Излучение включено' в ответ на отключение сигнала 'Включить излучение'";
                    break;
                case 4:
                    KSPMessage = "Передаач прекращена по команде'Сбросить канал' или 'Прекратить передачу'";
                    break;
                case 5:
                    KSPMessage = "Ошибка формата сообщения данных";
                    break;
                case 6:
                    KSPMessage = "Неисправность Т-821-06 (Аппарат неисправен или отсутствует требуемая программа)";
                    break;
                case 7:
                    KSPMessage = "Ошибка передачи данных (от радиостанции); 0х0F = не задан режим канала ( исходное состояние)";
                    break;
                case 8:
                    KSPMessage = "Сообщение не доведено ( при КВТ=1)"; //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
                    break;
                case 9:
                    KSPMessage = "Отказ Модуля МПДИ-01"; //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
                    break;
                case 10:
                    KSPMessage = "Отказ Модуля МПДИ-02"; //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
                    break;
                case 15:
                    KSPMessage = "Не задан режим канала (Исходное состояние)";////ПРИМА
                    break;
            }
            System.Console.WriteLine(" Состояние передачи {0}", KSPMessage);
            Byte[] byteArr1 = new Byte[1];
            byteArr1[0] = Convert.ToByte(ksp);
            return byteArr1;//////////////////////////////////////////////////////////// пустое возвращение
        }
        public static byte[] Combine(byte[] mas1, byte[] mas2)
        {
            byte[] massaved = new byte[1];
            massaved = mas1;
            mas1 = new byte[mas1.Length + mas2.Length];
            massaved.CopyTo(mas1, 0);
            mas2.CopyTo(mas1, mas1.Length - mas2.Length);
            return mas1;
        }
    }
}
