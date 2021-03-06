
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SodWinForms
{
    class IniFile
    {
        string Path;
        [DllImport("kernel32")]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);
        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);
        public IniFile(string IniPath)
        {
            Path = new FileInfo(IniPath).FullName.ToString();
        }
        public string ReadINI(string Section, string Key)
        {
            var RetVal = new StringBuilder(65535);
            GetPrivateProfileString(Section, Key, "", RetVal, 65535, Path);
            return RetVal.ToString();
        }
        public void Write(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, Path);
        }     
        public bool KeyExists(string Section, string Key = null)
        {
            return ReadINI(Section, Key).Length > 0;
        }
    }
}
