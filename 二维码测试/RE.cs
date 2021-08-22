using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fsvrt_new
{
    class RE
    {
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetRegistPath(string name)
        {
            string registData;                                //注册数据
            RegistryKey hkml = Registry.CurrentUser;
            RegistryKey software = hkml.OpenSubKey("SOFTWARE", true);
            RegistryKey aimdir = software.OpenSubKey("GNU", true);
            RegistryKey install = aimdir.OpenSubKey("x264", true);
            registData = install.GetValue(name).ToString();
            return registData;
        }
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tovalue"></param>
        public static void WriteRegedit(string name, string tovalue,bool istwo)
        {
            RegistryKey hklm = Registry.CurrentUser;
            RegistryKey software = hklm.OpenSubKey("SOFTWARE", true);
            RegistryKey aimdir = software.CreateSubKey("GNU");
            RegistryKey install = aimdir.CreateSubKey("x264");
            if (!istwo)
            {
          
                install.SetValue(name, tovalue, RegistryValueKind.String);
            }
        }
        public static void WriteRegedit(string name, object tovalue)
        {
            RegistryKey hklm = Registry.CurrentUser;
            RegistryKey software = hklm.OpenSubKey("SOFTWARE", true);
            RegistryKey aimdir = software.CreateSubKey("GNU");
            RegistryKey install = aimdir.CreateSubKey("x264");
         
            {
                install.SetValue(name, tovalue, RegistryValueKind.DWord);
            }
           
        }
        public static bool IsRegeditItemExist1(string test)
        {
            try
            {
                string[] subkeyNames;
                RegistryKey hkml = Registry.CurrentUser;
                RegistryKey software = hkml.OpenSubKey("SOFTWARE");
                // RegistryKey software = hkml.OpenSubKey("SOFTWARE", true);
             //   RegistryKey aimdir = software.OpenSubKey("GNU", true);
                subkeyNames = software.GetSubKeyNames();
                //取得该项下所有子项的名称的序列，并传递给预定的数组中  
                foreach (string keyName in subkeyNames)
                {
                    //  MessageBox.Show(keyName);
                    if (keyName == test)
                    {
                        hkml.Close();
                        return true;
                    }
                }
                hkml.Close();
                return false;
            }
            catch
            {
                return false;
            }
        }
        public static bool IsRegeditItemExist2(string test)
        {
            try
            {
                string[] subkeyNames;
                RegistryKey hkml = Registry.CurrentUser;
                RegistryKey software = hkml.OpenSubKey("SOFTWARE");
                // RegistryKey software = hkml.OpenSubKey("SOFTWARE", true);
                RegistryKey aimdir = software.OpenSubKey("GNU", true);
                subkeyNames = aimdir.GetSubKeyNames();
                //取得该项下所有子项的名称的序列，并传递给预定的数组中  
                foreach (string keyName in subkeyNames)
                {
                    //  MessageBox.Show(keyName);
                    if (keyName == test)
                    {
                        hkml.Close();
                        return true;
                    }
                }
                hkml.Close();
                return false;
            }
            catch
            {
                return false;
            }
        }

    }
}
