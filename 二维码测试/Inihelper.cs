using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace fsvrt_new
{
    class IniHelper
    {



        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, StringBuilder retVal, int size, string filePath);
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        private string sPath = null;
        public void Init(string path)
        {
            sPath = path;

        }

        public void Write(string section, string key, string value)
        {

            // section=配置节，key=键名，value=键值，path=路径  

            WritePrivateProfileString(section, key, value, sPath);

        }
        public string Read(string section, string key)
        {

            // 每次从ini中读取多少字节  

            System.Text.StringBuilder temp = new System.Text.StringBuilder(255);

            // section=配置节，key=键名，temp=上面，path=路径  

            GetPrivateProfileString(section, key, "", temp, 255, sPath);

            return temp.ToString();

        }




    }
}
