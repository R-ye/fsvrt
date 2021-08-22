using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fsvrt_new
{
    public partial class PrintForm : Form
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
        int dytype = 0;
        public PrintForm(int type=0)//0表示凭证打印，1表示客户图片
        {
            InitializeComponent();
            dytype = type;
            
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

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            try
            {if (Class1.lastFilename != "")
                {
                    Class1.lastFilename = Class1.lastFilename.Replace(".avi", ".mp4");
                }
                if (dytype == 1)
                {
                    if (Class1.khimg != null)
                    {
                        Bitmap ret = (Bitmap)Class1.khimg.Clone();
                        //   e.Graphics.DrawString("客户区截图", DrawFont, brush, 600, 600);
                        e.Graphics.DrawImage(ret, (e.PageBounds.Width - 640) / 2, 20, 640, 480);  //img大小

                        // e.Graphics.DrawString(TicCode, DrawFont, brush, 600, 600); //绘制字符串
                        e.HasMorePages = false;
                    }
                }
                else
                {
                    Font ft = new Font("宋体", 12, FontStyle.Regular);

                    string tag = "";
                    Bitmap img = new Bitmap(e.PageBounds.Width, e.PageBounds.Height);
                    Graphics e1 = Graphics.FromImage(img);
                    e1.DrawImage(Logolabel.Image, 25, 10);
                    e1.DrawString("开户面签录音录像凭证", ft, new SolidBrush(Color.Black), new PointF((e.PageBounds.Width - 160) / 2, 25));
                    e1.DrawString("开户机构：" + Class1.org, ft, new SolidBrush(Color.Black), new PointF(20, 75));
                    if (Class1.khlx == 0)//单位开户
                    {
                        e1.DrawString("==========================================开户单位信息=======================================", ft, new SolidBrush(Color.Black), new PointF(20, 100));
                        e1.DrawString("单位名称：" + Class1.khmc, ft, new SolidBrush(Color.Black), new PointF(20, 135));
                        e1.DrawString("账户名称：" + Class1.zhmc, ft, new SolidBrush(Color.Black), new PointF(20, 170));
                        e1.DrawString("开户方式：柜面开户", ft, new SolidBrush(Color.Black), new PointF(20, 205));
                        e1.DrawString("账户性质：" + Class1.zhxz, ft, new SolidBrush(Color.Black), new PointF(e.PageBounds.Width - 300, 205));
                        e1.DrawString("========================================法人或负责人信息=====================================", ft, new SolidBrush(Color.Black), new PointF(20, 240));
                        e1.DrawString("法人或负责人姓名：" + Class1.fzrxm, ft, new SolidBrush(Color.Black), new PointF(20, 275));
                        e1.DrawString("证件类型：" + Class1.zjlx, ft, new SolidBrush(Color.Black), new PointF(20, 305));
                        e1.DrawString("证件号码：" + Class1.zjhm, ft, new SolidBrush(Color.Black), new PointF(20, 340));
                        e1.DrawString("联系方式：" + Class1.lxfs, ft, new SolidBrush(Color.Black), new PointF(20, 375));
                        tag = "法人或负责人签字：";
                    }
                    else
                    {
                        e1.DrawString("==========================================开户账户信息=======================================", ft, new SolidBrush(Color.Black), new PointF(20, 100));
                        e1.DrawString("客户名称：" + Class1.khmc, ft, new SolidBrush(Color.Black), new PointF(20, 135));
                        e1.DrawString("账户名称：" + Class1.zhmc, ft, new SolidBrush(Color.Black), new PointF(20, 170));
                        e1.DrawString("开户方式：柜面开户", ft, new SolidBrush(Color.Black), new PointF(20, 205));
                        e1.DrawString("账户性质：" + Class1.zhxz, ft, new SolidBrush(Color.Black), new PointF(e.PageBounds.Width - 300, 205));
                        e1.DrawString("==========================================开户客户信息======================================", ft, new SolidBrush(Color.Black), new PointF(20, 240));
                        e1.DrawString("客户姓名：" + Class1.khmc, ft, new SolidBrush(Color.Black), new PointF(20, 275));
                        e1.DrawString("证件类型：" + Class1.zjlx, ft, new SolidBrush(Color.Black), new PointF(20, 305));
                        e1.DrawString("证件号码：" + Class1.zjhm, ft, new SolidBrush(Color.Black), new PointF(20, 340));
                        e1.DrawString("联系方式：" + Class1.lxfs, ft, new SolidBrush(Color.Black), new PointF(20, 375));
                        tag = "客户签字：";
                    }
                    e1.DrawString("=========================================视频文件信息========================================", ft, new SolidBrush(Color.Black), new PointF(20, 410));
                    if (Path.GetFileNameWithoutExtension(Class1.lastFilename).Length < 40)
                    {
                        e1.DrawString("视频文件名称：" + Path.GetFileNameWithoutExtension(Class1.lastFilename).Replace(".avi", ".mp4"), ft, new SolidBrush(Color.Black), new PointF(20, 445));
                        e1.DrawString("视频文件创建时间：" + Class1.filecrateTime, ft,
                           new SolidBrush(Color.Black), new PointF(20, 480));

                        e1.DrawString("视频文件时长：" + string.Format(
                            "{0:00}:{1:00}",
                            Math.Floor((decimal)Class1.sc / 60),
                             Class1.sc % 60),
                        ft, new SolidBrush(Color.Black), new PointF(20, 515));
                        e1.DrawString("============================================签字区===========================================", ft, new SolidBrush(Color.Black), new PointF(20, 550));
                        e1.DrawString("银行经办人（双人）签字：", ft, new SolidBrush(Color.Black), new PointF(20, 610));
                        e1.DrawString(tag, ft, new SolidBrush(Color.Black), new PointF(20, 670));
                        e1.DrawString("日期：" + DateTime.Now.ToString("yyyy年MM月dd日"), ft, new SolidBrush(Color.Black), new PointF(e.PageBounds.Width - 300, 740));
                        //e1.DrawImage(img, (new PointF(e.PageBounds.Width - 250, 605)));
                    }
                    else
                    {
                        e1.DrawString("视频名称：" + Path.GetFileNameWithoutExtension(Class1.lastFilename).Substring(0, 39),
                            ft, new SolidBrush(Color.Black), new PointF(20, 445));
                        e1.DrawString(Path.GetFileNameWithoutExtension(Class1.lastFilename).Substring(39, Path.GetFileNameWithoutExtension(Class1.lastFilename).Length - 39).Replace(".avi", ".mp4"),
                             ft, new SolidBrush(Color.Black), new PointF(20, 470));

                        e1.DrawString("视频文件创建时间：" + Class1.filecrateTime, ft,
                           new SolidBrush(Color.Black), new PointF(20, 505));
                        e1.DrawString("视频文件时长：" + string.Format(
                            "{0:00}:{1:00}",
                            Math.Floor((decimal)Class1.sc / 60),
                             Class1.sc % 60),
                        ft, new SolidBrush(Color.Black), new PointF(20, 540));
                        e1.DrawString("============================================签字区===========================================", ft, new SolidBrush(Color.Black), new PointF(20, 575));
                        e1.DrawString("银行经办人（双人）签字：", ft, new SolidBrush(Color.Black), new PointF(20, 635));
                        e1.DrawString(tag, ft, new SolidBrush(Color.Black), new PointF(20, 695));
                        e1.DrawString("日期：" + DateTime.Now.ToString("yyyy年MM月dd日"), ft, new SolidBrush(Color.Black), new PointF(e.PageBounds.Width - 300, 765));

                    }
                    if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "PngData"))
                    {
                        Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "PngData");
                    }
                    string name = Class1.org + "_" + Class1.khmc + "_" + Class1.zhxz + "_" + DateTime.Now.ToString("yyyyMMdd") + "_面签凭证" + ".png";
                    img.Save(AppDomain.CurrentDomain.BaseDirectory + "PngData\\" + name);
                    e.Graphics.DrawImage(img, 0, 0);
                    e.HasMorePages = false;
                }
            }
            catch
            { }
        }
        public void SavePz()
        {
            try
            {
                if (Class1.lastFilename != "")
                {
                    Class1.lastFilename = Class1.lastFilename.Replace(".avi", ".mp4");
                }
                Font ft = new Font("宋体", 12, FontStyle.Regular);

                string tag = "";
                Bitmap img = new Bitmap(zw, zh);
                Graphics e1 = Graphics.FromImage(img);
                e1.DrawImage(Logolabel.Image, 25, 10);
                e1.DrawString("开户面签录音录像凭证", ft, new SolidBrush(Color.Black), new PointF((zw - 160) / 2, 25));
                e1.DrawString("开户机构：" + Class1.org, ft, new SolidBrush(Color.Black), new PointF(20, 75));
                if (Class1.khlx == 0)//单位开户
                {
                    e1.DrawString("==========================================开户单位信息=======================================", ft, new SolidBrush(Color.Black), new PointF(20, 100));
                    e1.DrawString("单位名称：" + Class1.khmc, ft, new SolidBrush(Color.Black), new PointF(20, 135));
                    e1.DrawString("账户名称：" + Class1.zhmc, ft, new SolidBrush(Color.Black), new PointF(20, 170));
                    e1.DrawString("开户方式：柜面开户", ft, new SolidBrush(Color.Black), new PointF(20, 205));
                    e1.DrawString("账户性质：" + Class1.zhxz, ft, new SolidBrush(Color.Black), new PointF(zw - 300, 205));
                    e1.DrawString("========================================法人或负责人信息=====================================", ft, new SolidBrush(Color.Black), new PointF(20, 240));
                    e1.DrawString("法人或负责人姓名：" + Class1.fzrxm, ft, new SolidBrush(Color.Black), new PointF(20, 275));
                    e1.DrawString("证件类型：" + Class1.zjlx, ft, new SolidBrush(Color.Black), new PointF(20, 305));
                    e1.DrawString("证件号码：" + Class1.zjhm, ft, new SolidBrush(Color.Black), new PointF(20, 340));
                    e1.DrawString("联系方式：" + Class1.lxfs, ft, new SolidBrush(Color.Black), new PointF(20, 375));
                    tag = "单位法人或负责人签字：";
                }
                else
                {
                    e1.DrawString("==========================================开户账户信息=======================================", ft, new SolidBrush(Color.Black), new PointF(20, 100));
                    e1.DrawString("客户名称：" + Class1.khmc, ft, new SolidBrush(Color.Black), new PointF(20, 135));
                    e1.DrawString("账户名称：" + Class1.zhmc, ft, new SolidBrush(Color.Black), new PointF(20, 170));
                    e1.DrawString("开户方式：柜面开户", ft, new SolidBrush(Color.Black), new PointF(20, 205));
                    e1.DrawString("账户性质：" + Class1.zhxz, ft, new SolidBrush(Color.Black), new PointF(zw - 300, 205));
                    e1.DrawString("==========================================开户客户信息======================================", ft, new SolidBrush(Color.Black), new PointF(20, 240));
                    e1.DrawString("客户姓名：" + Class1.khmc, ft, new SolidBrush(Color.Black), new PointF(20, 275));
                    e1.DrawString("证件类型：" + Class1.zjlx, ft, new SolidBrush(Color.Black), new PointF(20, 305));
                    e1.DrawString("证件号码：" + Class1.zjhm, ft, new SolidBrush(Color.Black), new PointF(20, 340));
                    e1.DrawString("联系方式：" + Class1.lxfs, ft, new SolidBrush(Color.Black), new PointF(20, 375));
                    tag = "客户签字：";
                }
                e1.DrawString("=========================================视频文件信息========================================", ft, new SolidBrush(Color.Black), new PointF(20, 410));
                if (Path.GetFileNameWithoutExtension(Class1.lastFilename).Length < 40)
                {
                    e1.DrawString("视频文件名称：" + Path.GetFileNameWithoutExtension(Class1.lastFilename.Replace(".avi", ".mp4")), ft, new SolidBrush(Color.Black), new PointF(20, 445));
                    e1.DrawString("视频文件创建时间：" + Class1.filecrateTime, ft,
                       new SolidBrush(Color.Black), new PointF(20, 480));

                    e1.DrawString("视频文件时长：" + string.Format(
                        "{0:00}:{1:00}",
                        Math.Floor((decimal)Class1.sc / 60),
                         Class1.sc % 60),
                    ft, new SolidBrush(Color.Black), new PointF(20, 515));
                    e1.DrawString("============================================签字区===========================================", ft, new SolidBrush(Color.Black), new PointF(20, 550));
                    e1.DrawString("银行经办人（双人）签字：", ft, new SolidBrush(Color.Black), new PointF(20, 610));
                    e1.DrawString(tag, ft, new SolidBrush(Color.Black), new PointF(20, 670));
                    e1.DrawString("日期：" + DateTime.Now.ToString("yyyy年MM月dd日"), ft, new SolidBrush(Color.Black), new PointF(zw - 300, 740));
                    //e1.DrawImage(img, (new PointF(e.PageBounds.Width - 250, 605)));
                }
                else
                {
                    e1.DrawString("视频名称：" + Path.GetFileNameWithoutExtension(Class1.lastFilename).Substring(0, 39),
                        ft, new SolidBrush(Color.Black), new PointF(20, 445));
                    e1.DrawString(Path.GetFileNameWithoutExtension(Class1.lastFilename).Substring(39, Path.GetFileNameWithoutExtension(Class1.lastFilename).Length - 39).Replace(".avi", ".mp4"),
                         ft, new SolidBrush(Color.Black), new PointF(20, 470));

                    e1.DrawString("视频文件创建时间：" + Class1.filecrateTime, ft,
                       new SolidBrush(Color.Black), new PointF(20, 505));
                    e1.DrawString("视频文件时长：" + string.Format(
                        "{0:00}:{1:00}",
                        Math.Floor((decimal)Class1.sc / 60),
                         Class1.sc % 60),
                    ft, new SolidBrush(Color.Black), new PointF(20, 540));
                    e1.DrawString("============================================签字区===========================================", ft, new SolidBrush(Color.Black), new PointF(20, 575));
                    e1.DrawString("银行经办人（双人）签字：", ft, new SolidBrush(Color.Black), new PointF(20, 635));
                    e1.DrawString(tag, ft, new SolidBrush(Color.Black), new PointF(20, 695));
                    e1.DrawString("日期：" + DateTime.Now.ToString("yyyy年MM月dd日"), ft, new SolidBrush(Color.Black), new PointF(zw - 300, 765));

                }
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "PngData"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "PngData");
                }
                string name = Class1.org + "_" + Class1.khmc + "_" + Class1.zhxz + "_" + DateTime.Now.ToString("yyyyMMdd") + "_面签凭证" + ".png";
                img.Save(AppDomain.CurrentDomain.BaseDirectory + "PngData\\" + name);
            }
            catch
            {
                MessageBox.Show("初始化凭证信息发生错误，可能无法打印凭证!","严重错误",MessageBoxButtons.OK,MessageBoxIcon.Stop);
            }

        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (dycomboBox.Text != "")
                {
                  
                    printDocument1.Print();
                 
                    MessageBox.Show("打印任务已传输至打印机，请插入A4纸!","提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("请先选择打印机，然后点击打印按钮！","提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
           catch
            {
                MessageBox.Show("出错啦，大概是打印机错误！","错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        int zw = 828;
        int zh = 1169;
        IniHelper ini = new IniHelper();
        public void init()
        {
           try
            {
                
                  
                for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)       //获取当前打印机
                {
                    dycomboBox.Items.Add(PrinterSettings.InstalledPrinters[i]);
                }
                    if (ini.Read("Print", "PrinterName") != "" && dycomboBox.Items.Contains(ini.Read("Print", "PrinterName")))
                    {
                        dycomboBox.Text = ini.Read("Print", "PrinterName");
                    }
                    else
                    {
                        if (PrinterSettings.InstalledPrinters.Count > 0)
                        {
                            dycomboBox.Text = PrinterSettings.InstalledPrinters[0];

                        }
                    }
                this.printDocument1.DocumentName = "面签视频录制系统凭证打印任务-" + DateTime.Now.ToString("yyyy年MM月dd日");
                for (int i = 0; i < printDocument1.PrinterSettings.PaperSizes.Count; i++)
                {
                    if (printDocument1.PrinterSettings.PaperSizes[i].PaperName.ToUpper() == "A4")
                    {
                        this.printDocument1.DefaultPageSettings.PaperSize = printDocument1.PrinterSettings.PaperSizes[i];
                        zw = this.printDocument1.DefaultPageSettings.PaperSize.Width;
                        zh = this.printDocument1.DefaultPageSettings.PaperSize.Height;
                        try
                        {
                            ini.Write("Import", "width", zw.ToString());
                            ini.Write("Import", "height", zh.ToString());
                        }
                        catch
                        { }
                        break;
                    }
                }
                if (zw == 828 && zh == 1169)
                {
                    MessageBox.Show("系统无法获取到制定页面的配置信息，生成的页面可能与实际不符!","警告",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                }
                SavePz();
               
            }
           catch
            { }
        }
        private void DyForm_Load(object sender, EventArgs e)
        {
            try
            {
                ini.Init(System.AppDomain.CurrentDomain.BaseDirectory + "\\Config\\配置.ini");
                Control.CheckForIllegalCrossThreadCalls = false;
                Thread thread = new Thread(init);
                thread.Start();
            }
            catch
            { }
        }

        private void dycomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                printDocument1.PrinterSettings.PrinterName = dycomboBox.Text;
                ini.Write("Print", "PrinterName", dycomboBox.Text);
            }
            catch
            { }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                PrintPreviewDialog cppd = new PrintPreviewDialog();
                cppd.Document = printDocument1;
                cppd.ShowDialog();
            }
            catch
            { }
        }

        private void DyForm_Paint(object sender, PaintEventArgs e)
        {
            Pen p = new Pen(Color.FromArgb(222, 223, 225), 2);
            Graphics g = e.Graphics;
            g.DrawRectangle(p, 0, 0, this.Width, this.Height);
        }
    }
}
