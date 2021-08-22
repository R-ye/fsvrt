using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fsvrt_new
{
    public partial class CompressForm : Form
    {
        public static int CS_DropSHADOW = 0x20000;
        public static int GCL_STYLE = (-26);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SetClassLong(IntPtr hwnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassLong(IntPtr hwnd, int nIndex);
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
              (
                  int nLeftRect, // x-coordinate of upper-left corner
                  int nTopRect, // y-coordinate of upper-left corner
                  int nRightRect, // x-coordinate of lower-right corner
                  int nBottomRect, // y-coordinate of lower-right corner
                  int nWidthEllipse, // height of ellipse
                  int nHeightEllipse // width of ellipse
               );
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);
        #region 内存回收
        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        private static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
        /// <summary>
        /// 释放内存
        /// </summary>
        public static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }
        #endregion
        private bool m_aeroEnabled;                     // variables for box shadow
        private const int CS_DROPSHADOW = 0x00020000;
        private const int WM_NCPAINT = 0x0085;
        private const int WM_ACTIVATEAPP = 0x001C;

        public struct MARGINS                           // struct for box shadow
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }

        private const int WM_NCHITTEST = 0x84;          // variables for dragging the form
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;

        protected override CreateParams CreateParams
        {
            get
            {
                m_aeroEnabled = CheckAeroEnabled();

                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                if (!m_aeroEnabled)
                    cp.ClassStyle |= CS_DROPSHADOW;

                return cp;
            }
        }

        private bool CheckAeroEnabled()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                int enabled = 0;
                DwmIsCompositionEnabled(ref enabled);
                return (enabled == 1) ? true : false;
            }
            return false;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCPAINT:                        // box shadow
                    if (m_aeroEnabled)
                    {
                        var v = 2;
                        DwmSetWindowAttribute(this.Handle, 2, ref v, 4);
                        MARGINS margins = new MARGINS()
                        {
                            bottomHeight = 1,
                            leftWidth = 1,
                            rightWidth = 1,
                            topHeight = 1
                        };
                        DwmExtendFrameIntoClientArea(this.Handle, ref margins);

                    }
                    break;
                default:
                    break;
            }
            base.WndProc(ref m);



        }
        public CompressForm()
        {
            InitializeComponent();
        }
        private void Output(object sendProcess, DataReceivedEventArgs output)
        {
            if (!String.IsNullOrEmpty(output.Data))
            {
                //处理方法...
                if (output.Data.IndexOf("time=") != -1)
                {
                    string timel = output.Data.Substring(output.Data.IndexOf("time=") + 5, 8);
                   int t= int.Parse(timel.Split(':')[0]) * 3600 + int.Parse(timel.Split(':')[1]) * 60 + int.Parse(timel.Split(':')[2]) * 1;
                    if ((t * 100 / (int)Class1.sc) >= 100)
                    {
                        progressBar1.Value = 100;
                    }
                    else
                    {
                        progressBar1.Value = (t * 100 / (int)Class1.sc);
                    }
                    if (progressBar1.Value == 100)
                    {
                      
                        label1.Text = "最后的处理工作...";
                        Thread.Sleep(3000);

                        label1.Text = "压缩成功";
                        File.Delete(Class1.lastFilename);
                    }
                }

            }
        }
        private void AddFapiaoBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(Class1.lastFilename))
                {
                    label1.Text = "开始准备工作...";
                    Process p = new Process();//建立外部调用线程
                    p.StartInfo.FileName = System.Environment.CurrentDirectory + "\\ffmpeg.exe";//要调用外部程序的绝对路径
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    label1.Text = "正在压缩 " + Path.GetFileName(Class1.lastFilename);
                    string strArg = " -i " + Class1.lastFilename + " -vcodec libx264  -y  " + Class1.lastFilename.Replace(".avi", ".mp4");
                    p.StartInfo.Arguments = strArg;
                    p.StartInfo.UseShellExecute = false;//不使用操作系统外壳程序启动线程(一定为FALSE,详细的请看MSDN)
                    p.StartInfo.RedirectStandardError = true;//把外部程序错误输出写到StandardError流中(这个一定要注意,FFMPEG的所有输出信息,都为错误输出流,用StandardOutput是捕获不到任何消息的...这是我耗费了2个多月得出来的经验...mencoder就是用standardOutput来捕获的)
                    p.StartInfo.CreateNoWindow = true;//不创建进程窗口
                    p.ErrorDataReceived += new DataReceivedEventHandler(Output);//外部程序(这里是FFMPEG)输出流时候产生的事件,这里是把流的处理过程转移到下面的方法中,详细请查阅MSDN
                    p.Start();//启动线程
                    p.BeginErrorReadLine();//开始异步读取
                    p.WaitForExit();//阻塞等待进程结束
                    p.Close();//关闭进程
                    p.Dispose();//释放资源
                }
                else
                {
                    label1.Text = "原始文件不存在";
                }
            }
            catch
            { }
        }

        private void label7_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            int WM_SYSCOMMAND = 0x0112;

            //窗体移动
            int SC_MOVE = 0xF010;
            int HTCAPTION = 0x0002;

            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }
        [DllImport("shell32.dll", ExactSpelling = true)]
        private static extern void ILFree(IntPtr pidlList);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern IntPtr ILCreateFromPathW(string pszPath);

        [DllImport("shell32.dll", ExactSpelling = true)]
        private static extern int SHOpenFolderAndSelectItems(IntPtr pidlList, uint cild, IntPtr children, uint dwFlags);
        public void ExplorerFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath) && !System.IO.Directory.Exists(filePath))
                return;

            if (System.IO.Directory.Exists(filePath))
                System.Diagnostics.Process.Start(@"explorer.exe", "/select,\"" + filePath + "\"");
            else
            {
                IntPtr pidlList = ILCreateFromPathW(filePath);
                if (pidlList != IntPtr.Zero)
                {
                    try
                    {
                        Marshal.ThrowExceptionForHR(SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0));
                    }
                    finally
                    {
                        ILFree(pidlList);
                    }
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                ExplorerFile(Class1.lastFilename.Replace(".avi", ".mp4"));
            }
            catch
            { }
        }

        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            Pen p = new Pen(Color.FromArgb(222, 223, 225), 2);
            Graphics g = e.Graphics;
            g.DrawRectangle(p, 0, 0, this.Width, this.Height);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.Title = "请选择需要压缩的视频文件";
            fileDialog.Filter = "avi文件(*.avi)|*.avi";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {

                Class1.lastFilename = fileDialog.FileName;
                Class1.sc = 1000;
            }


        }
    }
}
