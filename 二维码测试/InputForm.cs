
using service;
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
    public partial class InputForm : Form
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
        public InputForm()
        {
            InitializeComponent();
            toolTip1.SetToolTip(label7,"使用设备读取身份证件信息");
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

        private void InputForm_Load(object sender, EventArgs e)
        {
            if (Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "\\Config"))
            {
                ini.Init(System.AppDomain.CurrentDomain.BaseDirectory + "\\Config\\配置.ini");
            }
            else
            {
                Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + "\\Config");
                ini.Init(System.AppDomain.CurrentDomain.BaseDirectory + "\\Config\\配置.ini");
            }
            if (File.Exists("Config//机构列表.txt"))
            {
                string[] orglist = File.ReadAllLines("Config//机构列表.txt");
                orgcomboBox.Items.Clear();
                orgcomboBox.Items.AddRange(orglist);
                if (ini.Read("Org", "Name") != "" && orgcomboBox.Items.Contains(ini.Read("Org", "Name")))
                {
                    orgcomboBox.Text = ini.Read("Org", "Name");
                }
            }
            else
            {
                if (Class1.isgc)
                {
                    File.AppendAllText("PromgramLog.txt", "[" + DateTime.Now.ToString() + "]" + "机构文件丢失，请在运行目录新建org.txt，并逐行填写机构名称\r\n");
                }
                MessageBox.Show("机构文件丢失，请在运行目录新建org.txt，并逐行填写机构名称!","数据丢失错误");
            }
        }
        string[] dglx = { "基本存款账户", "一般存款账户", "专用存款账户", "临时存款账户", "其他"};
        string[] dslx = { "个人结算账户","个人非结算账户","个人储蓄账户","其他"};
        private void dgType_CheckedChanged(object sender, EventArgs e)
        {
            if (dgType.Checked)
            {
                label2.Text = "单位名称";
                khmcTextBox.WatermarkText = "请输入单位名称";
                checkBox1.Text = "与单位名称一致";
                label11.Text = "负责人或法人姓名";
                label11.Visible = true;
                zjxmTextBox.WatermarkText = "请输入负责人或法人姓名";
                zjxmTextBox.Enabled = true;
                zhxzcomboBox.Items.Clear();
                zhxzcomboBox.Items.AddRange(dglx);
            }
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Enter)  // 按下的是回车键
            {
                foreach (Control c in this.Controls)
                {
                    if (c is NewTextBox)  // 当前控件是文本框控件
                    {
                        keyData = Keys.Tab;
                    }
                    if (c is System.Windows.Forms.Button)  // 当前控件是文本框控件
                    {
                        //button1_Click(null, null);
                    }
                }
                keyData = Keys.Tab;
            }
            return base.ProcessDialogKey(keyData);
        }
        private void dsType_CheckedChanged(object sender, EventArgs e)
        {
            if (dsType.Checked)
            {
                label2.Text = "客户姓名";
                khmcTextBox.WatermarkText = "请输入客户姓名";
                checkBox1.Text = "与客户姓名一致";
              //  label11.Text = "         ";
                label11.Visible=true;
                zjxmTextBox.Enabled= false;
                zjxmTextBox.BackColor = Color.White;
                zjxmTextBox.WatermarkText = "";
                zhxzcomboBox.Items.Clear();
                zhxzcomboBox.Items.AddRange(dslx);
            }
        }
        IniHelper ini = new IniHelper();
        private void button2_Click(object sender, EventArgs e)
        {
            Class1.ixqx = true;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {//||khmcTextBox.Text.Length < 2 || zhmcTextBox.Text.Length < 2 || zhxzcomboBox.Text == "" || hsrATextBox.Text.Length <= 5 || hsrBTextBox.Text.Length <= 5 || zjlxcomboBox.Text == "" || zjhmTextBox.Text.Length <= 4 || lxfsTextBox.Text.Length <= 4
            if (orgcomboBox.Text.Length<=2 )
            {
                MessageBox.Show("机构名称不合法，请正确选择开户机构名称!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (khmcTextBox.Text.Length <2)
            {
                MessageBox.Show("客户或单位名称不合法，请检查后重试!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (zhmcTextBox.Text.Length <2)
            {
                MessageBox.Show("账户名称不合法，请检查后重试!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (zhxzcomboBox.Text=="")
            {
                MessageBox.Show("账户性质不合法，请检查后重试!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
           
            if (hsrATextBox.Text.Trim().Length<6 || hsrBTextBox.Text.Length < 6)
            {
                MessageBox.Show("核实人A和核实人B不合法!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (hsrATextBox.Text.Trim() == hsrBTextBox.Text.Trim())
            {
                MessageBox.Show("核实人A和核实人B不能为同一人!","错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (dgType.Checked && zjxmTextBox.Text.Length < 2)
            {
                MessageBox.Show("单位负责人姓名不合法，请检查后重试!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (zjlxcomboBox.Text == "")
            {
                MessageBox.Show("证件类型不合法，请检查后重试!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (zjhmTextBox.Text == "")
            {
                MessageBox.Show("证件号码不合法，请检查后重试!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (zjlxcomboBox.Text == "居民身份证" && zjhmTextBox.Text.Length >= 15)
            {
                if (CheckIDCard18(zjhmTextBox.Text) == false)
                {
                    MessageBox.Show("证件号码校验失败，请检查后重试!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            if (lxfsTextBox.Text.Length<=4)
            {
                MessageBox.Show("联系方式不合法，请检查后重试!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult result =MessageBox.Show("请确认录入的信息是否完全正确？一经确认，无法修改。", "录入确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Class1.ixqx = false;
                Class1.org = orgcomboBox.Text;
                Class1.khmc = khmcTextBox.Text;
                Class1.zhmc = zhmcTextBox.Text;
                Class1.zhxz = zhxzcomboBox.Text;
                Class1.mqrq = mqrq.Text;
                Class1.hsra = hsrATextBox.Text;
                Class1.hsrb = hsrBTextBox.Text;
                Class1.fzrxm = zjxmTextBox.Text;
                Class1.zjhm = zjhmTextBox.Text;
                Class1.zjlx = zjlxcomboBox.Text;
                Class1.lxfs = lxfsTextBox.Text;
                if (dsType.Checked)
                {
                    Class1.khlx = 1;
                }
                else
                {
                    Class1.khlx = 0;
                }
                this.Close();
            }
            else {
                Class1.ixqx = true;
            }
        }
        private  bool CheckIDCard18(string Id)
        {
            try
            {
                long n = 0;
                if (long.TryParse(Id.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out n) == false)
                {
                    return false;
                }
                string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
                if (address.IndexOf(Id.Remove(2)) == -1)
                {
                    return false;
                }
                string birth = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
                DateTime time = new DateTime();
                if (DateTime.TryParse(birth, out time) == false)
                {
                    return false;
                }
                string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
                string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
                char[] Ai = Id.Remove(17).ToCharArray();
                int sum = 0;
                for (int i = 0; i < 17; i++)
                {
                    sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
                }
                int y = -1;
                Math.DivRem(sum, 11, out y);
                if (arrVarifyCode[y] != Id.Substring(17, 1).ToLower())
                {
                    return false;
                }
                return true;//正确
            }
            catch
            {
                return false;
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                zhmcTextBox.Text = khmcTextBox.Text ;
            }
        }
        
        private void InputForm_Paint(object sender, PaintEventArgs e)
        {
            Pen p = new Pen(Color.FromArgb(222, 223, 225), 2);
            Graphics g = e.Graphics;
            g.DrawRectangle(p, 0, 0, this.Width, this.Height);
        }

        private void label7_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (dgType.Checked)
                {
                    ServiceMethod sc = new ServiceMethod();
                    if (sc.GetServiceStatus() == true)
                    {
                        if (sc.GetName() == "fail" || sc.GetName() == "null")
                        { MessageBox.Show("读取证件失败，请手动输入!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        else
                        {
                            zjxmTextBox.Text = sc.GetName();
                            zjlxcomboBox.Text = "居民身份证";
                            zjhmTextBox.Text = sc.GetID();
                        }
                    }
                    else
                    {
                        MessageBox.Show("未检测到外设服务程序或无法连接身份证读取设备!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                if (!dgType.Checked)
                {
                    ServiceMethod sc = new ServiceMethod();
                    if (sc.GetServiceStatus() == true)
                    {
                        if (sc.GetName() == "fail" || sc.GetName() == "null")
                        { MessageBox.Show("读取证件失败，请手动输入!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        else
                        {
                            zhmcTextBox.Text = khmcTextBox.Text = sc.GetName();
                            zjlxcomboBox.Text = "居民身份证";
                            zjhmTextBox.Text = sc.GetID();
                        }
                    }
                    else
                    { MessageBox.Show("未检测到外设服务程序或无法连接身份证读取设备!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
            catch
            {
                if (Class1.isgc)
                {
                    File.AppendAllText("PromgramLog.txt", "[" + DateTime.Now.ToString() + "]" + "身份证读取模块加载失败\r\n");
                }
                MessageBox.Show("身份证读取模块加载失败!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                label7.Visible = false;
            }
        }

        private void orgcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ini.Write("Org","Name",orgcomboBox.Text);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                MsgForm dy = new MsgForm();
                dy.StartPosition = FormStartPosition.CenterParent;
                dy.ShowDialog();

            }
            catch
            { }
            Class1.ixqx = false;
            Class1.org = "工程模式测试机构";
            Class1.khmc = "工程模式测试客户名称";
            Class1.zhmc = "工程模式测试账户名称";
            Class1.zhxz ="工程模式测试账户性质";
            Class1.mqrq =DateTime.Now.ToShortDateString();
            Class1.hsra ="R01234";
            Class1.hsrb = "R02345";
            Class1.fzrxm = "工程模式负责人姓名";
            Class1.zjhm = "工程模式证件号码0x0102030405060708";
            Class1.zjlx ="工程模式证件类型";
            Class1.lxfs = "13333333333";
            if (dsType.Checked)
            {
                Class1.khlx = 1;
            }
            else
            {
                Class1.khlx = 0;
            }
            Class1.ixqx = false;
           
            this.Close();
        }
    }
}
