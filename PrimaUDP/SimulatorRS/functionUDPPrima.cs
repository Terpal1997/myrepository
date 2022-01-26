using System;

namespace SimulatorRS
{
    class functionUDPPrima
    {
        public functionUDPPrima()
        { }
        Random rand = new Random();
        public UInt16 messagenumber = 0;
        public UInt16 packagenumber = 0;
        public byte[] Combine(byte[] mas1, byte[] mas2)
        {
            byte[] massaved = new byte[1];
            massaved = mas1;
            mas1 = new byte[mas1.Length + mas2.Length];
            for (int i = 0; i < massaved.Length; i++)
            {
                mas1[i] = massaved[i];
            }
            mas2.CopyTo(mas1, mas1.Length - mas2.Length);
            return mas1;
        }
        public byte[] setMessageNumber(byte[] data)//функция сделана, чтобы обновлять номер у повторно отправляемых сообщений без полного ресоздания сообщения и инкреминтирования номера пакета
        {
            byte[] MessageN = BitConverter.GetBytes(messagenumber);
            if (data.Length>=3)
            {
                for (int i = 0; i < 2; i++)
                {
                    data[i + 1] = MessageN[i];
                }
            }            
            return data;
        }
        public byte[] generatingMessage0(int sendMode, int data, bool nullsData = false)
        {
            byte[] cmd = new byte[data + 7];
            
            setMessageNumber(cmd);
            messagenumber++;
            byte[] PackageN = BitConverter.GetBytes(packagenumber);
            packagenumber++;
           // cmd[3] = 0x1;
            cmd[5] = 0x1;
            for (int i = 0; i < 2; i++)
            {
                cmd[i + 3] = PackageN[i];
            }
            cmd[6] = Convert.ToByte(sendMode);
            if (!nullsData)
            {
                for (int i = 7; i < data + 7; i++)
                {
                    cmd[i] = Convert.ToByte(rand.Next(0x00, 0x100));
                }
            }
            return cmd;
        }
        
        public byte[] generatingMessage0(int numberOfPackage, int sendMode, byte[] data)
        {
            byte[] cmd = new byte[7] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x1, 0x0 };
            setMessageNumber(cmd);
            messagenumber++;
            byte[] PackageN = BitConverter.GetBytes(numberOfPackage);
            packagenumber++;

            for (int i = 0; i < 2; i++)
            {
                cmd[i + 3] = PackageN[i];
            }
            cmd[6] = Convert.ToByte(sendMode);
            cmd = Combine(cmd, data);
            return cmd;
        }
        public byte[] generatingMessage0(byte[] data)
        {
            setMessageNumber(data);
            messagenumber++;
            byte[] PackageN = BitConverter.GetBytes(packagenumber);
            packagenumber++;
            for (int i = 0; i < 2; i++)
            {
                data[i + 3] = PackageN[i];
            }
            return data;
        }
        public byte[] generatingMessage7()
        {
            byte[] cmd = new byte[3] { 0x07, 0x0, 0x0 };
            setMessageNumber(cmd);
            messagenumber++;
            return cmd;
        }
        public byte[] generatingMessage4(Int16 PackageNumber)
        {
            byte[] cmd = new byte[5] { 0x04, 0x0, 0x0, 0x0, 0x0 };
            //if (messagenumber == 65535)
            //{
            //    messagenumber = 0;
            //}
            setMessageNumber(cmd);
            messagenumber++;
            byte[] PackageN = BitConverter.GetBytes(PackageNumber);
            for (int i = 0; i < 2; i++)
            {
                cmd[i + 3] = PackageN[i];
            }
            return cmd;
        }
        public byte[] generatingMessage5(Int16 PackageNumber, Int16 KO)
        {
            byte[] cmd = new byte[6] { 0x05, 0x0, 0x0, 0x0, 0x0, 0x0 };
            setMessageNumber(cmd);
            messagenumber++;
            byte[] PackageN = BitConverter.GetBytes(PackageNumber);
            for (int i = 0; i < 2; i++)
            {
                cmd[i + 3] = PackageN[i];
            }
            cmd[5] = Convert.ToByte(KO);
            return cmd;
        }
        public byte[] generatingMessage6(Int16 PackageNumber)
        {
            byte[] cmd = new byte[5] { 0x06, 0x0, 0x0, 0x0, 0x0 };
            setMessageNumber(cmd);
            messagenumber++;
            byte[] PackageN = BitConverter.GetBytes(PackageNumber);
            for (int i = 0; i < 2; i++)
            {
                cmd[i + 3] = PackageN[i];
            }
            return cmd;
        }
        public byte[] generatingMessage2(byte state)
        {
            byte[] cmd = new byte[4] { 0x02, 0x0, 0x0, 0x0 };
            setMessageNumber(cmd);
            messagenumber++;
            cmd[3] = state;
            return cmd;
        }
        public byte[] generatingDotes(int length)
        {
            if (length > 1472)
                length = 1472;
            byte[] cmd = new byte[length];
            for (int i = 0; i < cmd.Length; i++)
            {
                cmd[i] = 255;
            }
            return cmd;
        }
        public byte[] generatingMessage3()
        {
            byte[] cmd = new byte[4] { 0x03, 0x0, 0x0, 0x0 };
            setMessageNumber(cmd);
            messagenumber++;
            return cmd;
        }
    }
}
