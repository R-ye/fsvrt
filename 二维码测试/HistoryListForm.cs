using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fsvrt_new
{
    public partial class HistoryListForm : Form
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
        public HistoryListForm()
        {
            InitializeComponent();
        }
        public void AddData()
        {
            this.listView1.BeginUpdate();
            if (File.Exists("List\\history.txt"))
            {
                string[] data = File.ReadAllLines("List\\history.txt");
                data.Reverse();
                int count = data.Count();
                for (int i = 0; i < count; i++)
                {
                    ListViewItem lvi = new ListViewItem();



                    lvi.Text = (i+1).ToString();
                    string[] d = data[i].Split(',');
                    foreach (string s in d)
                    {
                        lvi.SubItems.Add(s);
                    }
                    this.listView1.Items.Add(lvi);
                }
            }
            this.listView1.EndUpdate();
        }
        private void label7_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            AddData();
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

        private void Form3_Paint(object sender, PaintEventArgs e)
        {
            Pen p = new Pen(Color.FromArgb(222, 223, 225), 2);
            Graphics g = e.Graphics;
            g.DrawRectangle(p, 0, 0, this.Width, this.Height);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count > 0)
                {if (File.Exists(listView1.SelectedItems[0].SubItems[3].Text))
                    {
                        System.Diagnostics.Process.Start("explorer.exe", listView1.SelectedItems[0].SubItems[3].Text);
                        return;
                    }
                    if (File.Exists(listView1.SelectedItems[0].SubItems[3].Text.Replace(".avi",".mp4")))
                    {
                        System.Diagnostics.Process.Start("explorer.exe", listView1.SelectedItems[0].SubItems[3].Text.Replace(".avi", ".mp4"));
                        return;
                    }
                    MessageBox.Show("视频文件丢失，无法打开!","错误",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            catch
            { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    if (File.Exists(listView1.SelectedItems[0].SubItems[4].Text))
                    {
                        System.Diagnostics.Process.Start("explorer.exe", listView1.SelectedItems[0].SubItems[4].Text);
                        return;
                    }
                  
                    MessageBox.Show("凭证文件丢失，无法打开!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (DialogResult.Yes == MessageBox.Show("确认删除历史文件记录吗？删除后将不再索引，但不影响文件本身。", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    File.Delete("List\\history.txt");
                    listView1.Items.Clear();
                }
            }
            catch { }
        }

        private void 删除凭证文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    if (File.Exists(listView1.SelectedItems[0].SubItems[4].Text))
                    {
                        File.Delete(listView1.SelectedItems[0].SubItems[4].Text);
                        return;
                    }
                   
                    MessageBox.Show("凭证文件丢失，无法删除!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            { }
        }

        private void 删除视频文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    if (File.Exists(listView1.SelectedItems[0].SubItems[3].Text))
                    {
                        File.Delete(listView1.SelectedItems[0].SubItems[3].Text);
                        return;
                    }
                    if (File.Exists(listView1.SelectedItems[0].SubItems[3].Text.Replace(".avi", ".mp4")))
                    {
                        File.Delete(listView1.SelectedItems[0].SubItems[3].Text.Replace(".avi", ".mp4"));
                        return;
                    }
                    MessageBox.Show("视频文件丢失，无法删除!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            { }
        }
    }
}
