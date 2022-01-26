using System;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Text;
using static System.Math;
namespace SodWinForms
{
    class XiskProcedure
    {
        BitArray standartBitArray;
        readonly byte[] standart =/* new byte[]*/ { 0xA9, 0x47, 0xC3, 0x42, 0x23, 0x0E, 0x8F, 0x8C, 0x31, 0x72, 0x97, 0xF2, 0x0A, 0x30, 0x76, 0x91, 0x88, 0xE2, 0x5D, 0x52, 0xA7, 0x45, 0x0D, 0x1B, 0xDE, 0x63, 0x44, 0x2B, 0x39, 0x42, 0x71, 0x90, 0xEA, 0x15, 0xF3, 0xE6, 0x73, 0x40, 0x9A, 0xFB, 0xF9, 0x42, 0x61, 0x6E, 0xF1, 0xCF, 0x78, 0x7E, 0xB2, 0xCA, 0xF6, 0xD6, 0xB0, 0x84, 0x62, 0xA3, 0xC1, 0x7D, 0x84, 0xAB, 0xF8, 0xBE, 0x88, 0x29, 0xBE, 0xF1, 0x3E, 0x1B, 0x59, 0x7B, 0x5C, 0x68, 0x88, 0x9C, 0x9A, 0xB1, 0x24, 0xA7, 0x69, 0xFC, 0xEB, 0xA9, 0xF4, 0xBE, 0x1D, 0x7C, 0x52, 0x13, 0x14, 0x3F, 0x29, 0xB8, 0xCF, 0x79, 0xEA, 0x9E, 0xEC, 0xCC, 0x3C, 0x17, 0xA4, 0x39, 0x80, 0x40, 0xEF, 0x28, 0x75, 0x65, 0x5C, 0x07, 0xFC, 0x6F, 0x6A, 0x1F, 0xE6, 0xB2, 0x05, 0x1D, 0xFB, 0xF7, 0x02, 0x99, 0x4B, 0x73, 0x2C, 0xEE, 0x78, 0xF1, 0x57, 0xBC, 0x47, 0x7F, 0x26, 0xF9, 0x5C, 0xF2, 0x19, 0x85, 0x38, 0x60, 0xD3, 0xCB, 0x25, 0x1B, 0xEB, 0x8E, 0x5F, 0x6B, 0x51, 0xBD, 0x5C, 0x06, 0x37, 0x18, 0xD7, 0xBF, 0x0D, 0xBA, 0x33, 0xC6, 0x61, 0xB3, 0x69, 0x58, 0xA3, 0xC5, 0x95, 0xD9, 0xF9, 0x16, 0x2A, 0x9B, 0x7A, 0x72, 0x53, 0xF5, 0xC8, 0xB7, 0x1C, 0xDF, 0xAC, 0x1B, 0x46, 0x85, 0xB9, 0x73, 0x88, 0xD4, 0xAA, 0x4A, 0x5B, 0x19, 0x4E, 0xD9, 0x72, 0x4A, 0x03, 0xAE, 0xAF, 0xD7, 0x82, 0x92, 0xE8, 0xAC, 0xB7, 0xC2, 0xBB, 0xBF, 0xA3, 0xBD, 0x3E, 0xCF, 0xCD, 0xE2, 0x18, 0x61, 0xAB, 0x7D, 0xA3, 0x1A, 0xF3, 0xF4, 0xAD, 0x96, 0xC5, 0x1C, 0xFC, 0xE2, 0xEC, 0x64, 0x0C, 0xD0, 0xC6, 0xA6, 0xBE, 0x8A, 0x02, 0x44, 0x08, 0xFD, 0xBA, 0xFA, 0xA6, 0x11, 0x9B, 0x16, 0x8C, 0x05, 0x88, 0xBC, 0x0E, 0x8F, 0x16, 0x14, 0x94, 0x33, 0xA8, 0xD8, 0xA5, 0x31, 0xC1, 0x0A, 0x5F, 0xA6, 0x5F, 0x79, 0x9A, 0x46, 0x24, 0xE1, 0x72, 0x87, 0x71, 0x91, 0x2B, 0x3F, 0xE0, 0x63, 0x58 };//, 0x6C, 0xFA };последние пару байт обрубаем чтобы было кратно 31
        MainWindow mainWindow;
        int n7;
        double p7;
        double aa;
        UInt16 k, kk;
        double n;
        double[] p1 = new double[32];
        Int16 xj;
        int ni;
        int jj = 0;
        public XiskProcedure(int _n7, double _p7, double _aa, MainWindow window)
        {
            n7 = _n7;
            p7 = _p7;
            aa = _aa;
            mainWindow = window;
        }
        void Initxisk()
        {
            jj = 0;
            for (int i = 0; i < 32; i++)
            {
                p1[i] = 0;
            }
            xj = 0;
        }
        void Xisk(short i)
        {
            int n = 31;
            jj++;
            xj += i;
            if (jj % n == 0)
            {
                p1[xj] += 1;
                xj = 0;
            }
        }
        void Endxisk()
        {
            double n = 31;
            Int16 i;
            double[] q3 = new double[(int)n + 1];
            double[] h3 = new double[(int)n + 1];
            double[] v3 = new double[(int)n + 1];
            double[] r3 = new double[(int)n + 1];
            double[] r4 = new double[(int)n + 1];
            double y, x, x1;
            if (aa < 0.1)
            {
                x = n * Log(1 - p7);
                y = Math.Exp(x);
                r3[0] = y;
                for (i = 1; i < 10; i++)//в выполнении операции с операндом i он увеличен на 1, чтобы соблюсти размерность массивов (в pascal часть массивов начинается с 1)
                {
                    r3[i] = (r3[i - 1] * (n - i + 2) * p7) / (i * (1 - p7));
                }
            }
            else
            {
                q3[0] = 1;
                y = (1 - aa) * Log(n);
                x = Exp(y);
                y = x * Log(1 - p7);
                y = Exp(y);
                q3[1] = 1 - y;
                y = (1 - aa + (aa / n)) * Log(n);
                h3[1] = Exp(y);
                y = h3[1] * Log(1 - p7);
                y = 1 - Exp(y);
                v3[1] = p7 / y;
                for (i = 1; i < n; i++)
                {
                    y = (1 - aa + (aa * (i + 1) / n)) * Log(n / (i + 1));
                    h3[i + 1] = Exp(y);
                    y = h3[i + 1] * Log(1 - p7);
                    v3[i + 1] = p7 / (1 - Exp(y));
                    x = v3[i] - ((double)i) / n;
                    x1 = v3[i + 1] - ((double)i) / n;
                    q3[i + 1] = q3[i] * x / x1;
                }
                r3[(int)n] = q3[(int)n];//n не инкрементируется по той же причине
                for (i = 0; i < n; i++)
                {
                    r3[i] = q3[i] - q3[i + 1];
                }
                for (i = 0; i < n; i++)
                {
                    r4[i + 1] = q3[i + 1];
                }
            }
            for (i = 0; i < n; i++)
            {
                double k = ni / n;
                p1[i] = p1[i] / (ni / n);
            }
            x = 0;
            y = (double)ni / n;
            for (i = 0; i < 3; i++)
            {
                if (r3[i] == p1[i])
                {
                    continue;
                }
                if (r3[i] == 0)
                {
                    x += (y * (r3[i] - p1[i]) * (r3[i] - p1[i])) / 0.001;
                    continue;
                }
                x += (y * (r3[i] - p1[i]) * (r3[i] - p1[i])) / r3[i];
            }
            mainWindow.AddMessageToList("Критерий xi^2 c m=3 степенями свободы = " + x.ToString(), "", Color.DarkBlue);
            if (x < 11.3)
            {
                mainWindow.AddMessageToList("  Экспериментальные данные соответствуют модели", "", Color.DarkBlue);
            }
            else
            {
                mainWindow.AddMessageToList("  ___Экспериментальные данные не соответствуют модели___", "", Color.DarkBlue);
            }
        }
        private BitArray reverseBitMas(BitArray bitMas)
        {
            for (int i = 0; i < bitMas.Length / 8; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    bool x = bitMas[i * 8 + j];
                    bool y = bitMas[i * 8 + 7 - j];
                    bitMas[i * 8 + j] = y;
                    bitMas[i * 8 + 7 - j] = x;
                }
            }
            return bitMas;
        }
        public void Mainxisk(byte[] mes)
        {
            standartBitArray = new BitArray(standart);
            standartBitArray = reverseBitMas(standartBitArray);
            ni = standartBitArray.Length;
            BitArray mesBitArray = new BitArray(mes);
            mesBitArray = reverseBitMas(mesBitArray);
            if (mesBitArray.Length < standartBitArray.Length)
            {
                mainWindow.AddMessageToList("  ___Размер блока меньше чем размер Эталона___", "", Color.DarkBlue);
                return;
            }
            BitArray temporaryBitArray;
            n = 0; k = 50;
            k = (ushort)(k * Round(n / 1000, MidpointRounding.AwayFromZero));
            kk = (ushort)Round((double)k, MidpointRounding.AwayFromZero);
            double errorCounter = 0.0;
            Initxisk();
            for (int i = 0; i < mesBitArray.Length - standartBitArray.Length; i++)
            {
                temporaryBitArray = new BitArray(mesBitArray);
                for (int j = 0; j < standartBitArray.Length; j++)
                {
                    temporaryBitArray.Set(j, mesBitArray[j + i]);
                }
                temporaryBitArray.Length = standartBitArray.Length;
                ///
                //BitArray forMonitoring = new BitArray(temporaryBitArray);
                ///
                temporaryBitArray = temporaryBitArray.Xor(standartBitArray);
                errorCounter = 0;
                for (int j = 0; j < standartBitArray.Length; j++)
                {
                    if (temporaryBitArray[j])
                    {
                        errorCounter++;
                    }
                }
                if (errorCounter / temporaryBitArray.Length < 0.3)
                {
                    mainWindow.AddDebuggInformation("");
                    string str = "";
                    foreach (object obj in temporaryBitArray)
                    {
                        str += Convert.ToInt16(obj);
                    }
                    mainWindow.AddDebuggInformation("Результат, количество ошибок = " + errorCounter);
                    mainWindow.AddDebuggInformation(str);
                    for (int j = 0; j < temporaryBitArray.Length; j++)
                    {
                        Xisk(Convert.ToInt16(temporaryBitArray[j]));
                    }
                    Endxisk();
                    return;
                }
            }
            mainWindow.AddMessageToList("  ___Не обнаружена модель___", "", Color.DarkBlue);
            return;
        }
    }
}
