using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SodWinForms
{
    class Corrupter
    {
        private const int c = 128;
        private const int c1 = 10;
        private const int c2 = 2048;
        private double[] pin = new double[c];
        private double[] dd = new double[c2];
        private double p10, p11;
        public double aa;//коэффициент группирования
        public double p7;//вероятность ошибки в канале
        public short n7; // длина заполнения bb2 чем больше, тем лучше
        private short s6; //длина равномерной части в хвосте для состояния 01
        private int s7;// длина равномерной части в хвосте для состояния 11
        private short s; // состояния цепи : 1 для 11 и 0 для 01
        private int s4;// длина  0^i  1, получаемая в md4, и равна i+1
        private double[] bb2 = new double[c2];//bb2[i] вероятно 0^i
        Random rand = new Random();
        public Corrupter(int a, double b, double c)
        {
            n7 = (short)a;// длина заполнения
            p7 = b;//вероятность ошибки в канале
            aa = c;//коэффициент группирования
            InitModel();
        }

        public void InitModel()
        {
            double y = 0, x = 0, x1 = 0, y1 = 0, x2=0, y2=0, t=0, x4=0, y4=0;
            try{ if (aa >= 0.05)
                {
                    bb2[0] = 1;
                    bb2[1] = 1 - p7;
                    for (int i = 2; i <= n7 + 2; i++)
                    {
                        y = (1 - aa) * Math.Log(i);
                        x = Math.Exp(y);
                        y = x * Math.Log(1 - p7);
                        y = Math.Exp(y);
                        bb2[i] = y;
                    }//веряотности 00...00
                    p10 = bb2[1] - bb2[2];
                    p11 = bb2[0] - 2 * bb2[1] + bb2[2];
                    //x2 это 0..1/11 [используется p(00..01) p(00..10)]
                    //y2 это 0..1/10 [используется p(00..01) p(00..10)]
                    y = 0; x = 0; x1 = 0; y1 = 0;
                    for (int i = 0; i <= n7 - 1; i++)
                    {
                        x2 = (bb2[i] - 3 * bb2[i + 1] + 3 * bb2[i + 2] - bb2[i + 3]) / p11;
                        x = x + x2;
                        x1 = x1 + (i + 1) * x2;//условная средняя длина 0^i 1/11
                        y2 = (bb2[i + 1] - 2 * bb2[i + 2] + bb2[i + 3]) / p10;
                        y = y + y2;
                        y1 = y1 + y2 * (i + 1);//условная средняя длина 0^i 1/10
                        if (double.IsNaN(x1)|| double.IsNaN(x) || double.IsNaN(x2) || double.IsNaN(y2) || double.IsNaN(y) || double.IsNaN(y1) || double.IsNaN(y2))
                        {
                            int li = 1;
                        }
                    }
                    if (y == 1)
                    {
                        y = (1 - Math.Pow(10.0, -13));
                    }
                    if (x == 1)
                    {
                        x = (1 - Math.Pow(10.0, -13));
                    }
                    x4 = ((bb2[1] / p10 - y1 - (1 - y) * (n7 + 0.5)) / (1 - y)) * 2;
                    if ((x4) % 1 == 0.5)
                    {
                        x4 += 0.5;
                    }
                    s6 = Convert.ToInt16(Math.Round(x4));
                    y4 = (((1 - bb2[1]) / p11 - x1 - (1 - x) * (n7 + 0.5)) / (1 - x)) * 2;
                    if ((y4) % 1 == 0.5)
                    {
                        y4 += 0.5;
                    }
                    s7 = Convert.ToInt32(Math.Round(y4));
                    t = p11 / (p11 + p10);//вероятность состояния 11
                    x = rand.NextDouble();
                    if (x < t)
                    {
                        s = 1;
                    }
                    else
                    {
                        s = 0;
                    }
                    s4 = 0;
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void md4()
        {
            double x, y, x1, y1, x2, y2, z, x3;
            int i;
            if (s4 == 0)
            {
               x = rand.NextDouble();
                i = 0;
                y = 1;
                y1 = 0;
                y2 = 0;
                x1 = 0;
                x2 = 0;
                while (y > 0)
                {
                    switch (s)
                    {
                        case 0:
                            {
                                i = i + 1;
                                if (i <= n7)
                                {
                                    y2 = (bb2[i] - 2 * bb2[i + 1] + bb2[i + 2]) / p10;
                                    y1 = y1 + y2;
                                    y = x - y1;
                                }
                                else
                                {
                                    y1 = 1;
                                    x3 = rand.NextDouble();
                                    z = s6 * x3;
                                    if (z % 1 == 0.5)
                                    {
                                        z += 0.5;
                                    }
                                    i = Convert.ToInt32(n7 + 1 + Math.Round(z));
                                    y = x - y1;
                                }
                                break;
                            }
                        case 1:
                            {
                                i = i + 1;
                                if (i <= n7)
                                {
                                    x2 = (bb2[i - 1] - 3 * bb2[i] + 3 * bb2[i + 1] - bb2[i + 2]) / p11;
                                    x1 = x1 + x2;
                                    y1 = x1;
                                    y = x - x1;
                                }
                                else
                                {
                                    y1 = 1;
                                    x3 = rand.NextDouble();
                                    z = s7 * x3;
                                    if (z % 1 == 0.5)
                                    {
                                        z += 0.5;
                                    }
                                    i = Convert.ToInt32(n7 + 1 + Math.Round(z));
                                    y = x - y1;
                                }
                                break;
                            }
                    }
                }
                if (i == 1)
                {
                    s = 1;
                }
                else
                {
                    s = 0;
                }
                s4 = i;
            }
        }
        public int md5()
        {
            if (aa >= 0.1) //0.05
            {
                md4();
                s4 = s4 - 1;
                if (s4 == 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                if (rand.NextDouble() < p7)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
