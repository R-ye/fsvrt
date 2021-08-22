namespace fsvrt_new
{
    partial class InputForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.Logolabel = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dsType = new System.Windows.Forms.RadioButton();
            this.dgType = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.orgcomboBox = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.dgpanel = new fsvrt_new.RoundPanel();
            this.label7 = new System.Windows.Forms.Label();
            this.lxfsTextBox = new fsvrt_new.NewTextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.zjhmTextBox = new fsvrt_new.NewTextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.zjlxcomboBox = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.zjxmTextBox = new fsvrt_new.NewTextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.hsrBTextBox = new fsvrt_new.NewTextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.hsrATextBox = new fsvrt_new.NewTextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.mqrq = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.zhxzcomboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.zhmcTextBox = new fsvrt_new.NewTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.khmcTextBox = new fsvrt_new.NewTextBox();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.dgpanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(246)))));
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.Controls.Add(this.Logolabel);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(761, 61);
            this.panel1.TabIndex = 26;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            // 
            // Logolabel
            // 
            this.Logolabel.BackColor = System.Drawing.Color.Transparent;
            this.Logolabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Logolabel.Font = new System.Drawing.Font("幼圆", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Logolabel.Image = ((System.Drawing.Image)(resources.GetObject("Logolabel.Image")));
            this.Logolabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Logolabel.Location = new System.Drawing.Point(14, 6);
            this.Logolabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Logolabel.Name = "Logolabel";
            this.Logolabel.Size = new System.Drawing.Size(50, 46);
            this.Logolabel.TabIndex = 23;
            this.Logolabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Logolabel.UseCompatibleTextRendering = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(69, 18);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(217, 21);
            this.label8.TabIndex = 22;
            this.label8.Text = "面签录音录像系统[视频参数]";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dsType);
            this.groupBox1.Controls.Add(this.dgType);
            this.groupBox1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(23, 68);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(714, 55);
            this.groupBox1.TabIndex = 27;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "录制类型";
            // 
            // dsType
            // 
            this.dsType.AutoSize = true;
            this.dsType.Location = new System.Drawing.Point(425, 22);
            this.dsType.Name = "dsType";
            this.dsType.Size = new System.Drawing.Size(83, 24);
            this.dsType.TabIndex = 1;
            this.dsType.TabStop = true;
            this.dsType.Text = "对私客户";
            this.dsType.UseVisualStyleBackColor = true;
            this.dsType.CheckedChanged += new System.EventHandler(this.dsType_CheckedChanged);
            // 
            // dgType
            // 
            this.dgType.AutoSize = true;
            this.dgType.Location = new System.Drawing.Point(208, 22);
            this.dgType.Name = "dgType";
            this.dgType.Size = new System.Drawing.Size(83, 24);
            this.dgType.TabIndex = 0;
            this.dgType.TabStop = true;
            this.dgType.Text = "对公客户";
            this.dgType.UseVisualStyleBackColor = true;
            this.dgType.CheckedChanged += new System.EventHandler(this.dgType_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(116, 139);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 20);
            this.label1.TabIndex = 29;
            this.label1.Text = "机构名称";
            // 
            // orgcomboBox
            // 
            this.orgcomboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.orgcomboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.orgcomboBox.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.orgcomboBox.ForeColor = System.Drawing.Color.Blue;
            this.orgcomboBox.FormattingEnabled = true;
            this.orgcomboBox.Location = new System.Drawing.Point(210, 136);
            this.orgcomboBox.Name = "orgcomboBox";
            this.orgcomboBox.Size = new System.Drawing.Size(383, 28);
            this.orgcomboBox.TabIndex = 30;
            this.orgcomboBox.SelectedIndexChanged += new System.EventHandler(this.orgcomboBox_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.Color.LightSeaGreen;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkTurquoise;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.MediumTurquoise;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(445, 633);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 39);
            this.button1.TabIndex = 153;
            this.button1.Text = "确认";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.BackColor = System.Drawing.Color.Red;
            this.button2.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(212, 633);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(104, 39);
            this.button2.TabIndex = 154;
            this.button2.Text = "取消";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(216)))), ((int)(((byte)(221)))));
            this.label4.Location = new System.Drawing.Point(35, 619);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(690, 1);
            this.label4.TabIndex = 155;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabel1.Location = new System.Drawing.Point(693, 680);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(56, 17);
            this.linkLabel1.TabIndex = 156;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "工程模式";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // dgpanel
            // 
            this.dgpanel.Back = System.Drawing.Color.Empty;
            this.dgpanel.Controls.Add(this.label7);
            this.dgpanel.Controls.Add(this.lxfsTextBox);
            this.dgpanel.Controls.Add(this.label14);
            this.dgpanel.Controls.Add(this.zjhmTextBox);
            this.dgpanel.Controls.Add(this.label13);
            this.dgpanel.Controls.Add(this.zjlxcomboBox);
            this.dgpanel.Controls.Add(this.label12);
            this.dgpanel.Controls.Add(this.zjxmTextBox);
            this.dgpanel.Controls.Add(this.label11);
            this.dgpanel.Controls.Add(this.hsrBTextBox);
            this.dgpanel.Controls.Add(this.label10);
            this.dgpanel.Controls.Add(this.hsrATextBox);
            this.dgpanel.Controls.Add(this.label9);
            this.dgpanel.Controls.Add(this.mqrq);
            this.dgpanel.Controls.Add(this.label6);
            this.dgpanel.Controls.Add(this.zhxzcomboBox);
            this.dgpanel.Controls.Add(this.label5);
            this.dgpanel.Controls.Add(this.checkBox1);
            this.dgpanel.Controls.Add(this.label3);
            this.dgpanel.Controls.Add(this.zhmcTextBox);
            this.dgpanel.Controls.Add(this.label2);
            this.dgpanel.Controls.Add(this.khmcTextBox);
            this.dgpanel.Location = new System.Drawing.Point(23, 168);
            this.dgpanel.MatrixRound = 8;
            this.dgpanel.Name = "dgpanel";
            this.dgpanel.Size = new System.Drawing.Size(714, 445);
            this.dgpanel.TabIndex = 28;
            // 
            // label7
            // 
            this.label7.Image = ((System.Drawing.Image)(resources.GetObject("label7.Image")));
            this.label7.Location = new System.Drawing.Point(591, 327);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 26);
            this.label7.TabIndex = 50;
            this.label7.Click += new System.EventHandler(this.label7_Click_1);
            // 
            // lxfsTextBox
            // 
            this.lxfsTextBox.BorderColor = System.Drawing.Color.LightGray;
            this.lxfsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lxfsTextBox.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lxfsTextBox.ForeColor = System.Drawing.Color.Blue;
            this.lxfsTextBox.HotColor = System.Drawing.Color.Blue;
            this.lxfsTextBox.Location = new System.Drawing.Point(187, 417);
            this.lxfsTextBox.MaxLength = 13;
            this.lxfsTextBox.Name = "lxfsTextBox";
            this.lxfsTextBox.Size = new System.Drawing.Size(383, 19);
            this.lxfsTextBox.TabIndex = 49;
            this.lxfsTextBox.WatermarkText = "请输入联系方式";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label14.Location = new System.Drawing.Point(95, 418);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(65, 20);
            this.label14.TabIndex = 48;
            this.label14.Text = "联系方式";
            // 
            // zjhmTextBox
            // 
            this.zjhmTextBox.BorderColor = System.Drawing.Color.LightGray;
            this.zjhmTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.zjhmTextBox.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.zjhmTextBox.ForeColor = System.Drawing.Color.Blue;
            this.zjhmTextBox.HotColor = System.Drawing.Color.Blue;
            this.zjhmTextBox.Location = new System.Drawing.Point(187, 376);
            this.zjhmTextBox.MaxLength = 32;
            this.zjhmTextBox.Name = "zjhmTextBox";
            this.zjhmTextBox.Size = new System.Drawing.Size(383, 19);
            this.zjhmTextBox.TabIndex = 47;
            this.zjhmTextBox.WatermarkText = "请输入证件号码";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label13.Location = new System.Drawing.Point(93, 374);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(65, 20);
            this.label13.TabIndex = 46;
            this.label13.Text = "证件号码";
            // 
            // zjlxcomboBox
            // 
            this.zjlxcomboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.zjlxcomboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.zjlxcomboBox.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.zjlxcomboBox.ForeColor = System.Drawing.Color.Blue;
            this.zjlxcomboBox.FormattingEnabled = true;
            this.zjlxcomboBox.Items.AddRange(new object[] {
            "居民身份证",
            "临时身份证",
            "港澳居民往来内地通行证",
            "台湾居民来往大陆通行证",
            "护照",
            "外国人永久居留证",
            "军人、武装警察证"});
            this.zjlxcomboBox.Location = new System.Drawing.Point(187, 326);
            this.zjlxcomboBox.Name = "zjlxcomboBox";
            this.zjlxcomboBox.Size = new System.Drawing.Size(383, 28);
            this.zjlxcomboBox.TabIndex = 45;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.Location = new System.Drawing.Point(93, 330);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 20);
            this.label12.TabIndex = 44;
            this.label12.Text = "证件类型";
            // 
            // zjxmTextBox
            // 
            this.zjxmTextBox.BorderColor = System.Drawing.Color.LightGray;
            this.zjxmTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.zjxmTextBox.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.zjxmTextBox.ForeColor = System.Drawing.Color.Blue;
            this.zjxmTextBox.HotColor = System.Drawing.Color.Blue;
            this.zjxmTextBox.Location = new System.Drawing.Point(187, 285);
            this.zjxmTextBox.MaxLength = 6;
            this.zjxmTextBox.Name = "zjxmTextBox";
            this.zjxmTextBox.Size = new System.Drawing.Size(383, 19);
            this.zjxmTextBox.TabIndex = 43;
            this.zjxmTextBox.WatermarkText = "请输入负责人或法人姓名";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.Location = new System.Drawing.Point(37, 286);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(121, 20);
            this.label11.TabIndex = 42;
            this.label11.Text = "负责人或法人姓名";
            // 
            // hsrBTextBox
            // 
            this.hsrBTextBox.BorderColor = System.Drawing.Color.LightGray;
            this.hsrBTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.hsrBTextBox.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.hsrBTextBox.ForeColor = System.Drawing.Color.Blue;
            this.hsrBTextBox.HotColor = System.Drawing.Color.Blue;
            this.hsrBTextBox.Location = new System.Drawing.Point(187, 244);
            this.hsrBTextBox.MaxLength = 8;
            this.hsrBTextBox.Name = "hsrBTextBox";
            this.hsrBTextBox.Size = new System.Drawing.Size(383, 19);
            this.hsrBTextBox.TabIndex = 41;
            this.hsrBTextBox.WatermarkText = "请输入核实人B工号";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(71, 242);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(88, 20);
            this.label10.TabIndex = 40;
            this.label10.Text = "核实人B工号";
            // 
            // hsrATextBox
            // 
            this.hsrATextBox.BorderColor = System.Drawing.Color.LightGray;
            this.hsrATextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.hsrATextBox.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.hsrATextBox.ForeColor = System.Drawing.Color.Blue;
            this.hsrATextBox.HotColor = System.Drawing.Color.Blue;
            this.hsrATextBox.Location = new System.Drawing.Point(187, 201);
            this.hsrATextBox.MaxLength = 8;
            this.hsrATextBox.Name = "hsrATextBox";
            this.hsrATextBox.Size = new System.Drawing.Size(383, 19);
            this.hsrATextBox.TabIndex = 39;
            this.hsrATextBox.WatermarkText = "请输入核实人A工号";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(71, 198);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(89, 20);
            this.label9.TabIndex = 38;
            this.label9.Text = "核实人A工号";
            // 
            // mqrq
            // 
            this.mqrq.CalendarForeColor = System.Drawing.Color.Blue;
            this.mqrq.CustomFormat = "yyyy年MM月dd日";
            this.mqrq.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.mqrq.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.mqrq.Location = new System.Drawing.Point(187, 154);
            this.mqrq.Name = "mqrq";
            this.mqrq.Size = new System.Drawing.Size(197, 26);
            this.mqrq.TabIndex = 37;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(93, 154);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 20);
            this.label6.TabIndex = 36;
            this.label6.Text = "面签日期";
            // 
            // zhxzcomboBox
            // 
            this.zhxzcomboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.zhxzcomboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.zhxzcomboBox.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.zhxzcomboBox.ForeColor = System.Drawing.Color.Blue;
            this.zhxzcomboBox.FormattingEnabled = true;
            this.zhxzcomboBox.Items.AddRange(new object[] {
            "基本户",
            "一般户",
            "专用户",
            "临时户"});
            this.zhxzcomboBox.Location = new System.Drawing.Point(187, 105);
            this.zhxzcomboBox.Name = "zhxzcomboBox";
            this.zhxzcomboBox.Size = new System.Drawing.Size(198, 28);
            this.zhxzcomboBox.TabIndex = 35;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(93, 110);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 20);
            this.label5.TabIndex = 34;
            this.label5.Text = "账户性质";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox1.ForeColor = System.Drawing.Color.Black;
            this.checkBox1.Location = new System.Drawing.Point(583, 62);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(111, 21);
            this.checkBox1.TabIndex = 33;
            this.checkBox1.Text = "与单位名称一致";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(93, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 20);
            this.label3.TabIndex = 32;
            this.label3.Text = "账户名称";
            // 
            // zhmcTextBox
            // 
            this.zhmcTextBox.BorderColor = System.Drawing.Color.LightGray;
            this.zhmcTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.zhmcTextBox.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.zhmcTextBox.ForeColor = System.Drawing.Color.Blue;
            this.zhmcTextBox.HotColor = System.Drawing.Color.Blue;
            this.zhmcTextBox.Location = new System.Drawing.Point(187, 64);
            this.zhmcTextBox.MaxLength = 32;
            this.zhmcTextBox.Name = "zhmcTextBox";
            this.zhmcTextBox.Size = new System.Drawing.Size(383, 19);
            this.zhmcTextBox.TabIndex = 31;
            this.zhmcTextBox.WatermarkText = "请输入账户名称";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(93, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 20);
            this.label2.TabIndex = 30;
            this.label2.Text = "单位名称";
            // 
            // khmcTextBox
            // 
            this.khmcTextBox.BorderColor = System.Drawing.Color.LightGray;
            this.khmcTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.khmcTextBox.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.khmcTextBox.ForeColor = System.Drawing.Color.Blue;
            this.khmcTextBox.HotColor = System.Drawing.Color.Blue;
            this.khmcTextBox.Location = new System.Drawing.Point(187, 23);
            this.khmcTextBox.MaxLength = 32;
            this.khmcTextBox.Name = "khmcTextBox";
            this.khmcTextBox.Size = new System.Drawing.Size(383, 19);
            this.khmcTextBox.TabIndex = 0;
            this.khmcTextBox.WatermarkText = "请输入单位名称";
            // 
            // InputForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(761, 706);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.orgcomboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgpanel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "InputForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "面签录音录像系统[视频参数]";
            this.Load += new System.EventHandler(this.InputForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.InputForm_Paint);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.dgpanel.ResumeLayout(false);
            this.dgpanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label Logolabel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton dsType;
        private System.Windows.Forms.RadioButton dgType;
        private RoundPanel dgpanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox orgcomboBox;
        private NewTextBox lxfsTextBox;
        private System.Windows.Forms.Label label14;
        private NewTextBox zjhmTextBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox zjlxcomboBox;
        private System.Windows.Forms.Label label12;
        private NewTextBox zjxmTextBox;
        private System.Windows.Forms.Label label11;
        private NewTextBox hsrBTextBox;
        private System.Windows.Forms.Label label10;
        private NewTextBox hsrATextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DateTimePicker mqrq;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox zhxzcomboBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label3;
        private NewTextBox zhmcTextBox;
        private System.Windows.Forms.Label label2;
        private NewTextBox khmcTextBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}