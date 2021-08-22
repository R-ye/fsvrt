using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace fsvrt_new
{
    static class Program
    {


        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool createNew; using (System.Threading.Mutex mutex = new System.Threading.Mutex(true, Application.ProductName, out createNew))
            {
                if (createNew) { Application.Run(new MainForm()); }
                else
                {
                    MessageBox.Show("面签系统已在运行!","提示",MessageBoxButtons.OK,MessageBoxIcon.Information);

                 System.Threading.Thread.Sleep(1000);
                    System.Environment.Exit(1);
                }
            }
        }
    }
}
