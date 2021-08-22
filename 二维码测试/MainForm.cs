using AForge.Controls;
using AForge.Video.DirectShow;
using Microsoft.Win32;
using NAudio.Wave;
using SharpAvi;
using SharpAvi.Codecs;
using SharpAvi.Output;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Video.Config;
namespace fsvrt_new
{
    public partial class MainForm : Form
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
               // cp.ExStyle |= 0x02000000;
                if (!m_aeroEnabled)
                    cp.ClassStyle |= CS_DROPSHADOW;

                return cp;
            }
        }
        private  void DeleteLogs(string dir)
        {
            int days = 3;
            //日志保留时长 单位：天
            string logsDay = "3";
            if (!string.IsNullOrEmpty(logsDay))
            {
                days = int.Parse(logsDay);
            }

            try
            {
                if (!Directory.Exists(dir))
                {
                    return;
                }
                var now = DateTime.Now;
                foreach (var f in Directory.GetFileSystemEntries(dir).Where(f => File.Exists(f)))
                {
                    var t = File.GetCreationTime(f);
                    var elapsedTicks = now.Ticks - t.Ticks;
                    var elaspsedSpan = new TimeSpan(elapsedTicks);
                    if (elaspsedSpan.TotalDays > days)
                    {
                        File.Delete(f);
                    }
                }
            }
            catch (Exception ex)
            {

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
        //定义
        public VideoSourcePlayer VideoCam1 = new VideoSourcePlayer();
        public VideoSourcePlayer VideoCam2 = new VideoSourcePlayer();
        private int camCount = 0;
        private Bitmap latestImageFromCamera1;
        private Bitmap latestImageFromCamera2;
        private Font font = new Font("宋体", 14f);
        private static Color fontColor = Color.Red ;
        private SolidBrush solidBrush = new SolidBrush(fontColor);
        public delegate void ShowMergedImage1(Bitmap mergedImage);
        private List<CodecInfo> AvailableCodecs;
        private WaveInEvent audioSource;
        private string outputFolder ="";
        private FourCC encoder;
        private int encodingQuality;
        private int audioSourceIndex;
        private int videoencodeIndex;
        private SupportedWaveFormat audioWaveFormat;
        private bool encodeAudio;
        private int audioQuality;
        private string lastFileName;
        private readonly int screenWidth = 640;
        private readonly int screenHeight = 480;
        private AviWriter writer;
        private IAviVideoStream videoStream;
        private IAviAudioStream audioStream;
        private Thread screenThread;
        private bool isCancel = true;
        private readonly SupportedWaveFormat[] audioFormats = new[]
      {
            SupportedWaveFormat.WAVE_FORMAT_44M16,
            SupportedWaveFormat.WAVE_FORMAT_44S16
        };
        public Dictionary<int, string> AvailableAudioSources { get; private set; }
        public int SelectedAudioSourceIndex { get; private set; }
        public bool HasLastScreencast { get; private set; }
        public bool IsRecording { get; private set; }

        private Stopwatch recordingStopwatch = new Stopwatch();
        private ManualResetEvent stopThread = new ManualResetEvent(false);
        private AutoResetEvent videoFrameWritten = new AutoResetEvent(false);
        private AutoResetEvent audioBlockWritten = new AutoResetEvent(false);
        public MainForm()
        {
            InitializeComponent();
            try
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
                if (ini.Read("VideoCam", "VideoCam1Anglet") != "")
                {
                    khzxz = int.Parse(ini.Read("VideoCam", "VideoCam1Anglet"));
                }
                if (ini.Read("VideoCam", "VideoCam2Anglet") != "")
                {
                    yhzxz = int.Parse(ini.Read("VideoCam", "VideoCam2Anglet"));
                }

                InitAvailableCodecs();
                InitAvailableAudioSources();
                Control.CheckForIllegalCrossThreadCalls = false;
                SetStyle(ControlStyles.DoubleBuffer |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint, true);
                UpdateStyles();
                this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
                SetClassLong(this.Handle, GCL_STYLE, GetClassLong(this.Handle, GCL_STYLE) | CS_DropSHADOW); //API函数加载，实现窗体边框阴影效果  
                camCount = getCameraCount();
                this.VideoCam1.NewFrame += VideoCam1_NewFrame;
                this.VideoCam2.NewFrame += VideoCam2_NewFrame;
                costomcomboBox.Items.Clear();
                BankcomboBox.Items.Clear();
                if (getCameraName() != null)
                {
                    costomcomboBox.Items.AddRange(getCameraName());
                    BankcomboBox.Items.AddRange(getCameraName());
                }
               
               

                var asmDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var is64BitProcess = IntPtr.Size * 8 == 64;
                var dllName = string.Format("lameenc{0}.dll", is64BitProcess ? "64" : "32");
                Mp3AudioEncoderLame.SetLameDllLocation(Path.Combine(asmDir, dllName));
                //("运行环境dll动态加载，返回" + (is64BitProcess ? "64" : "32"));

                switch (camCount)
                {
                    case 0:
                        break;
                    case 1:
                        costomcomboBox.Enabled = BankcomboBox.Enabled = false;
                        costomcomboBox.Text = costomcomboBox.Items[0].ToString();
                        VideoID = 0;
                        
                       VideoCaptureDevice videoSource = setVideoCamera(VideoID);
                        this.VideoCam1.VideoSource = videoSource;
                        this.VideoCam1.Visible = false;
                        this.VideoCam1.Start();
                        button2.Text = "关闭摄像头";
                        timer1.Enabled = true;
                        break;
                    default:
                        costomcomboBox.Enabled = BankcomboBox.Enabled = false;
                        try
                        {
                            if (ini.Read("VideoCam", "VideoCam1Source") != "" && int.Parse(ini.Read("VideoCam", "VideoCam1Source")) <= 1)
                            {
                                costomcomboBox.Text = costomcomboBox.Items[int.Parse(ini.Read("VideoCam", "VideoCam1Source"))].ToString();
                                VideoID = int.Parse(ini.Read("VideoCam", "VideoCam1Source"));
                            }
                            else
                            {
                                costomcomboBox.Text = costomcomboBox.Items[0].ToString();
                                VideoID = 0;
                            }
                        }
                        catch
                        {
                            costomcomboBox.Text = costomcomboBox.Items[0].ToString();
                            VideoID = 0;
                        }
                       
                       VideoCaptureDevice videoSource1 = setVideoCamera(VideoID);
                        this.VideoCam1.VideoSource = videoSource1;
                        this.VideoCam1.Visible = false;
                        this.VideoCam1.Start();
                        // timer1.Enabled = true;
                        try
                        {
                            if (ini.Read("VideoCam", "VideoCam2Source") != "" && int.Parse(ini.Read("VideoCam", "VideoCam2Source")) <= 1)
                            {
                                BankcomboBox.Text = BankcomboBox.Items[int.Parse(ini.Read("VideoCam", "VideoCam2Source"))].ToString();
                                VideoID2 = int.Parse(ini.Read("VideoCam", "VideoCam2Source"));
                            }
                            else
                            {
                                BankcomboBox.Text = BankcomboBox.Items[1].ToString();
                                VideoID2 = 1;
                            }
                        }
                        catch
                        {
                            BankcomboBox.Text = BankcomboBox.Items[1].ToString();
                            VideoID2 = 1;
                        }
                     
                       VideoCaptureDevice videoSource2 = setVideoCamera(VideoID2);
                        this.VideoCam2.VideoSource = videoSource2;
                        this.VideoCam2.Visible = false;
                        this.VideoCam2.Start();
                        timer1.Enabled = true;
                        button2.Text = "关闭摄像头";
                        break;

                }
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
        }
        Bitmap imgRet;
        /// <summary>
        /// 初始化视频设置
        /// </summary>
        private void InitDefaultSettings()
        {
           // var exePath = new Uri(System.Reflection.Assembly.GetEntryAssembly().Location).LocalPath;
          //  outputFolder = System.IO.Path.GetDirectoryName(exePath);//视频文件输出文件夹
            encoder = codecs[videoencodeIndex].Codec;//视频压缩格式
            encodingQuality = trackBar1.Value*10;//视频质量
            audioWaveFormat = SupportedWaveFormat.WAVE_FORMAT_44S16;//声音格式
            if (SelectedAudioSourceIndex != -1)
            {
                audioSourceIndex = SelectedAudioSourceIndex;//音频设备ID
              
                encodeAudio = true;//允许录制声音
                audioQuality = (Mp3AudioEncoderLame.SupportedBitRates.Length + 1) / 2;//音频质量
            }
            else
            {
                audioQuality = (Mp3AudioEncoderLame.SupportedBitRates.Length + 1) / 2;//音频质量
                audioSourceIndex = SelectedAudioSourceIndex;
                  encodeAudio = false;
            }
           

            // minimizeOnStart = true;
        }
        private void InitAvailableAudioSources()
        {
            try
            {
                var deviceList = new Dictionary<int, string>
            {
                { -1, "(no sound)" }
            };
                for (var i = 0; i < WaveInEvent.DeviceCount; i++)
                {
                    var caps = WaveInEvent.GetCapabilities(i);
                    if (audioFormats.All(caps.SupportsWaveFormat))
                    {
                        deviceList.Add(i, caps.ProductName);
                        AudiocomboBox.Items.Add(caps.ProductName);
                    }
                }
                if (AudiocomboBox.Items.Count > 0)
                {
                    try
                    {
                        if (ini.Read("AudioDevice", "DeviceID") != "" && int.Parse(ini.Read("AudioDevice", "DeviceID")) < AudiocomboBox.Items.Count)
                        {
                            AudiocomboBox.Text = AudiocomboBox.Items[int.Parse(ini.Read("AudioDevice", "DeviceID"))].ToString();
                            SelectedAudioSourceIndex = int.Parse(ini.Read("AudioDevice", "DeviceID"));
                        }
                        else
                        {
                            AudiocomboBox.Text = AudiocomboBox.Items[0].ToString();
                            SelectedAudioSourceIndex = 0;
                        }
                    }
                    catch
                    {
                        AudiocomboBox.Text = AudiocomboBox.Items[0].ToString();
                        SelectedAudioSourceIndex = 0;
                    }

                }
                else
                {
                    SelectedAudioSourceIndex = -1;
                }
                AvailableAudioSources = deviceList;
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
        }
        List<CodecInfo> codecs = null;
        /// <summary>
        /// 初始化视频压缩编码
        /// </summary>
        private void InitAvailableCodecs()
        {
            try
            {
              codecs = new List<CodecInfo>
            {
                new CodecInfo(KnownFourCCs.Codecs.Uncompressed, "(none)"),
                // VideoEncoderCombo.Items.Add("None");
                new CodecInfo(KnownFourCCs.Codecs.MotionJpeg, "Motion JPEG")
            };
                // VideoEncoderCombo.Items.Add("Motion JPEG");
                codecs.AddRange(Mpeg4VideoEncoderVcm.GetAvailableCodecs());
                AvailableCodecs = codecs;
                foreach (CodecInfo inf in codecs)
                {
                    VideoEncoderCombo.Items.Add(inf.Name);
                }
                if (VideoEncoderCombo.Items.Count >0)
                {
                    try
                    {

                        if (ini.Read("Video", "VideoEncoderID") != "" && int.Parse(ini.Read("Video", "VideoEncoderID")) < VideoEncoderCombo.Items.Count)
                        {
                           
                            VideoEncoderCombo.Text = VideoEncoderCombo.Items[int.Parse(ini.Read("Video", "VideoEncoderID"))].ToString();
                            videoencodeIndex = int.Parse(ini.Read("Video", "VideoEncoderID"));
                        }
                        else
                        {
                           // MessageBox.Show("d"+ ini.Read("Video", "VideoEncoderID"));
                            VideoEncoderCombo.Text = "Motion JPEG";
                            videoencodeIndex = 1;
                        }
                    }
                    catch
                    {
                       
                        VideoEncoderCombo.Text = "Motion JPEG";
                        videoencodeIndex = 1;
                    }
                   
                }
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
        }
        public void StopRecord()
        {

        }
        public void StartRecord()
        {
            try
            {
                InitDefaultSettings();

                HasLastScreencast = false;
                IsRecording = true;
                recordingStopwatch.Reset();
                string name = Class1.org + "_" + Class1.khmc + "_" + Class1.zhxz + "_" + DateTime.Now.ToString("yyyyMMdd") + ".avi";
                Class1.filecrateTime = DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒");
                //  recordingTimer.Start();
                lastFileName = System.IO.Path.Combine(outputFolder + "\\", name);
                //MessageBox.Show(lastFileName);
                Class1.lastFilename = lastFileName;
                label2.Text = "    " + name;
                var bitRate = Mp3AudioEncoderLame.SupportedBitRates.OrderBy(br => br).ElementAt(audioQuality);
                Recorder(lastFileName,
                    encoder, encodingQuality,
                    audioSourceIndex, audioWaveFormat, encodeAudio, bitRate);
                recordingStopwatch.Start();
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
        }
        public void Recorder(string fileName,
      FourCC codec, int quality,
      int audioSourceIndex, SupportedWaveFormat audioWaveFormat, bool encodeAudio, int audioBitRate)
        {

            {
                writer = new AviWriter(fileName)
                {
                    FramesPerSecond = frame,
                    EmitIndex1 = true,
                };
                videoStream = CreateVideoStream(codec, quality);
                videoStream.Name = "Screencast";
                if (audioSourceIndex >= 0)
                {
                    //("录音");
                    var waveFormat = ToWaveFormat(audioWaveFormat);
                    audioStream = CreateAudioStream(waveFormat, encodeAudio, audioBitRate);
                    audioStream.Name = "Voice";
                    audioSource = new WaveInEvent
                    {
                        DeviceNumber = audioSourceIndex,
                        WaveFormat = waveFormat,
                        BufferMilliseconds = (int)Math.Ceiling(1000 / writer.FramesPerSecond),
                        NumberOfBuffers = 3,
                    };
                    audioSource.DataAvailable += audioSource_DataAvailable;
                }
                screenThread = new Thread(RecordScreen)
                {
                    Name = "RecordScreen",
                    IsBackground = true
                };
                if (audioSourceIndex >= 0 && audioSource != null)
                {
                    videoFrameWritten.Set();
                    audioBlockWritten.Reset();
                    audioSource.StartRecording();
                }
                screenThread.Start();
            }

        }
        public static Bitmap AddText2Image(Bitmap img, string text, System.Drawing.Point p, Font font, Color fontColor, int angle)
        {
            GC.Collect();
            using (var g = Graphics.FromImage(img))
            using (var brush = new SolidBrush(fontColor))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                var sizeF = g.MeasureString(text, font);
                g.ResetTransform();
                g.TranslateTransform(p.X, p.Y);
                g.RotateTransform(angle);
                g.DrawString(text, font, brush, new PointF(0, 0));
            }
            return img;
        }


        private void GetScreenshot(byte[] buffer)
        {
            try
            {

                {
                    var bitmap = new Bitmap(640, 480);
                    {

                        Graphics g = Graphics.FromImage(bitmap);
                        // g.FillRectangle(Brushes.White, new Rectangle(0, 0, 640, 480));
                        if (((Bitmap)imgRet.Clone() == null))
                            {
                            buffer = null;
                        }
                        g.DrawImage(((Bitmap)imgRet.Clone()), 0, 0, 640, 480);


                        var bits = bitmap.LockBits(new Rectangle(0, 0, screenWidth, screenHeight), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                        Marshal.Copy(bits.Scan0, buffer, 0, buffer.Length);
                        bitmap.UnlockBits(bits);
                    }
                }
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
        }
        long shotsTaken = 0;
        long ts = 0;

        private void RecordScreen()
        {
            var stopwatch = new Stopwatch();
            var buffer = new byte[screenWidth * screenHeight * 4];
            Task videoWriteTask = null;


            ts = 0;
            shotsTaken = 0;
            var isFirstFrame = true;

            var timeTillNextFrame = TimeSpan.Zero;
            stopwatch.Start();

            while (!stopThread.WaitOne(timeTillNextFrame))
            {
                // Thread.Sleep(1);
                // GC.Collect();
                GetScreenshot(buffer);
                if (buffer == null)
                {
                    isCancel = true;
                    goto c;
                }
                shotsTaken++;
                ts = shotsTaken / frame;


                // Wait for the previous frame is written
                if (!isFirstFrame)
                {

                    videoWriteTask.Wait();

                    videoFrameWritten.Set();
                }

                if (audioStream != null)
                {

                    var signalled = WaitHandle.WaitAny(new WaitHandle[] { audioBlockWritten, stopThread });
                    if (signalled == 1)
                    {

                        break;
                    }
                }


                videoWriteTask = videoStream.WriteFrameAsync(true, buffer, 0, buffer.Length);


                timeTillNextFrame = TimeSpan.FromSeconds(shotsTaken / (double)writer.FramesPerSecond - stopwatch.Elapsed.TotalSeconds);
                if (timeTillNextFrame < TimeSpan.Zero)
                    timeTillNextFrame = TimeSpan.Zero;
                c:
                isFirstFrame = false;
                
                if (isCancel)
                {
                    timer1.Stop();
                    Class1.sc = ts;
                    Timelabel.Text = "录制结束 "+string.Format(
                   "{0:00}:{1:00}",
                   Math.Floor((decimal)ts / 60),
                    ts % 60);
                    IsRecording = false;
                    HasLastScreencast = true;
                  
                    writer.Close();
                    stopwatch.Stop();

                    screenThread.Abort();
                    screenThread = null;
                    stopThread.Close();
                    stopThread.Set();
                    // screenThread.Join();
                    if (audioSource != null)
                    {
                        audioSource.StopRecording();
                        audioSource.DataAvailable -= audioSource_DataAvailable;
                        audioSource = null;
                        audioSource.Dispose();
                    }


                    return;

                }
            }
            writer.Close();
            stopwatch.Stop();

            // Wait for the last frame is written
            if (!isFirstFrame)
            {

                videoWriteTask.Wait();

            }
        }

        /// <summary>
        /// 音频流到达方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void audioSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            try
            {
                var signalled = WaitHandle.WaitAny(new WaitHandle[] { videoFrameWritten, stopThread });
                if (signalled == 0)
                {
                    //  //("录音返回");
                    audioStream.WriteBlock(e.Buffer, 0, e.BytesRecorded);
                    audioBlockWritten.Set();
                }
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
        }
        /// <summary>
        /// 创建视频流
        /// </summary>
        /// <param name="codec"></param>
        /// <param name="quality"></param>
        /// <returns></returns>
        private IAviVideoStream CreateVideoStream(SharpAvi.FourCC codec, int quality)
        {

            if (codec == KnownFourCCs.Codecs.Uncompressed)
            {
                return writer.AddUncompressedVideoStream(screenWidth, screenHeight);
            }
            else if (codec == KnownFourCCs.Codecs.MotionJpeg)
            {
                return writer.AddMotionJpegVideoStream(screenWidth, screenHeight, quality);
            }
            else
            {
                return writer.AddMpeg4VideoStream(screenWidth, screenHeight, (double)writer.FramesPerSecond,
                     quality: quality,
                     codec: codec,
                    forceSingleThreadedAccess: true);
            }
        }
        /// <summary>
        /// 创建音频流
        /// </summary>
        /// <param name="waveFormat"></param>
        /// <param name="encode"></param>
        /// <param name="bitRate"></param>
        /// <returns></returns>
        private IAviAudioStream CreateAudioStream(WaveFormat waveFormat, bool encode, int bitRate)
        {
            // Create encoding or simple stream based on settings
            if (encode)
            {
                // LAME DLL path is set in App.OnStartup()
                return writer.AddMp3AudioStream(waveFormat.Channels, waveFormat.SampleRate, bitRate);
            }
            else
            {
                return writer.AddAudioStream(
                    channelCount: waveFormat.Channels,
                    samplesPerSecond: waveFormat.SampleRate,
                    bitsPerSample: waveFormat.BitsPerSample);
            }
        }
        /// <summary>
        /// 音频格式转换
        /// </summary>
        /// <param name="waveFormat"></param>
        /// <returns></returns>
        private static WaveFormat ToWaveFormat(SupportedWaveFormat waveFormat)
        {
            switch (waveFormat)
            {
                case SupportedWaveFormat.WAVE_FORMAT_44M16:
                    return new WaveFormat(44100, 16, 1);
                case SupportedWaveFormat.WAVE_FORMAT_44S16:
                    return new WaveFormat(44100, 16, 2);
                default:
                   
                    return null;
            }
        }



        private void VideoCam2_NewFrame(object sender, ref Bitmap image)
        {
            try
            {
                latestImageFromCamera2 = (Bitmap)image.Clone();

                GC.Collect();
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
            //  ClearMemory();
        }
        public Bitmap Span(string text, string text2, string text3)
        {
            Font font = new Font("宋体", 18f);
            Color red = Color.Red;
            SolidBrush brush = new SolidBrush(red);
            Bitmap bitmap = new Bitmap(640, 480);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, 640, 480));
                graphics.InterpolationMode = InterpolationMode.High;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                Rectangle r = new Rectangle(0, 180, 640, 100);
                Rectangle r2 = new Rectangle(0, 220, 640, 100);
                Rectangle r3 = new Rectangle(0, 260, 640, 100);
                graphics.DrawString(text, font, brush, r, stringFormat);
                graphics.DrawString(text2, font, brush, r2, stringFormat);
                graphics.DrawString(text3, font, brush, r3, stringFormat);
            }
            return bitmap;
        }
        public void GenImg()
        {
            try
            {
               // bool flag = this.latestImageFromCamera1 == null && this.latestImageFromCamera2 == null;
               // if (!flag)
                {
                    Bitmap bitmap = new Bitmap(640, 480);
                    
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, 640, 480));
                        bool flag2 = this.latestImageFromCamera1 != null;
                        if (flag2)
                        {
                            Bitmap a1 = (Bitmap)this.latestImageFromCamera1.Clone();
                            switch (khzxz)
                            {

                                case 90:
                                    a1.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                    break;
                                case 180:
                                    a1.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                    break;
                                case 270:
                                   a1.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                    break;
                                case 360:
                                   a1.RotateFlip(RotateFlipType.RotateNoneFlipNone);
                                    break;
                            }
                            graphics.DrawImage(a1, 0, 0, 640, 480);
                        }
                        bool flag3 = this.latestImageFromCamera2 != null;
                        if (flag3)
                        {
                            Bitmap a2 = (Bitmap)this.latestImageFromCamera2.Clone();
                            switch (yhzxz)
                            {

                                case 90:
                                   a2.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                    break;
                                case 180:
                                   a2.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                    break;
                                case 270:
                                   a2.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                    break;
                                case 360:
                                    a2.RotateFlip(RotateFlipType.RotateNoneFlipNone);
                                    break;
                            }
                            graphics.DrawImage(a2, 480, 360, 160, 120);
                        }
                        graphics.InterpolationMode = InterpolationMode.High;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        SolidBrush solidBrush = new SolidBrush(Color.Red);
                        Font font = new Font("宋体", 1f, FontStyle.Regular, GraphicsUnit.Millimeter);
                        if (!isCancel)
                        {
                            if (Class1.khlx == 0)
                            {
                                string sy = "单位名称：" + Class1.khmc + "\r\n核实人：" + Class1.hsra + "  " + Class1.hsrb;

                                graphics.DrawString(sy, this.font, this.solidBrush, new PointF(15f, 10f));
                            }
                            else
                            {
                                string sy = "客户姓名：" + Class1.khmc + "\r\n核实人：" + Class1.hsra + "  " + Class1.hsrb;

                                graphics.DrawString(sy, this.font, this.solidBrush, new PointF(15f, 10f));
                            }
                        }
                    }
              
                    {
                        
                    }
                    // this.pictureBox1.Image = bitmap;
                    imgRet = (Bitmap)bitmap.Clone();
                    this.ShowMergedImage(bitmap);
                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
        }
        private void ShowMergedImage(Bitmap mergedImage)
        {
            try
            {
                GC.Collect();
                bool invokeRequired = base.InvokeRequired;
                if (invokeRequired)
                {
                    ShowMergedImage1 showMergedImage = new ShowMergedImage1(this.ShowMergedImage);
                    showMergedImage.BeginInvoke(mergedImage, null, null);
                }
                else
                {
                    Bitmap bitmap = (Bitmap)this.pictureBox1.Image;
                    this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    this.pictureBox1.Image = mergedImage;
                    bool flag = bitmap != null;
                    if (flag)
                    {
                       bitmap.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
        }
        private void VideoCam1_NewFrame(object sender, ref Bitmap image)
        {
            try
            {
                latestImageFromCamera1 = (Bitmap)image.Clone();

                // khzxz = 0;
                //pictureBox1.Image = latestImageFromCamera1;
                //  this.imgRet= (Bitmap)image.Clone();
                //ClearMemory();
                GC.Collect();
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
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

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Pen p = new Pen(Color.FromArgb(222, 223, 225), 2);
            Graphics g = e.Graphics;
            g.DrawRectangle(p, 0, 0, this.Width, this.Height);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.VideoCam1 != null)
                {
                    this.VideoCam1.Stop();
                }
                if (this.VideoCam1 != null)
                {
                    this.VideoCam2.Stop();
                }
                if (!File.Exists(Class1.lastFilename.Replace(".avi", ".mp4")) && File.Exists(Class1.lastFilename) && Class1.isneedys)
                {
                    DialogResult result = MessageBox.Show("存在视频没有压缩，如直接退出，将无法压缩，是否压缩？", "视频压缩确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        CompressForm MsgboxInf = new CompressForm()
                        {
                            StartPosition = FormStartPosition.CenterParent
                        };
                        MsgboxInf.ShowDialog();
                        try
                        {
                            string programPath = System.Environment.CurrentDirectory;
                            string volume = programPath.Substring(0, System.Windows.Forms.Application.StartupPath.IndexOf(":"));
                            //    MessageBox.Show(volume);
                            List<HardDiskPartition> listInfo = GetDiskListInfo();
                            if (listInfo != null && listInfo.Count > 0)
                            {
                                //listBox1.Items.Clear();
                                foreach (HardDiskPartition disk in listInfo)
                                {
                                    if (disk.PartitionName.Trim().ToUpper().Replace(":", "") == volume.ToUpper())
                                    {
                                        //listBox1.Items.Add(string.Format("{0}  总空间：{1} GB,剩余:{2} GB", disk.PartitionName, ManagerDoubleValue(disk.SumSpace, 1), ManagerDoubleValue(disk.FreeSpace, 1)));
                                        label1.Text = "磁盘容量：" + ManagerDoubleValue(disk.FreeSpace, 1) + "GB/" + ManagerDoubleValue(disk.SumSpace, 1) + "GB";
                                    }
                                }
                            }
                        }
                        catch
                        {
                            if (Class1.isgc)
                            {
                                File.AppendAllText("PromgramLog.txt", "[" + DateTime.Now.ToString() + "]" + "获取磁盘容量发生错误\r\n");
                            }
                        }
                    }
                    else
                    {
                        this.Close();
                    }
                }
                this.Close();
            }
            catch (Exception ex)
            {
               
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
                this.Close();
            }
        }


        bool isstop = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!isCancel && isstop == false)
                {
                    Timelabel.Text = "正在录制 " + string.Format(
                            "{0:00}:{1:00}",
                            Math.Floor((decimal)ts / 60),
                             ts % 60);
                    //   label8.Text = GetFileVersionInfo(Class1.lastFilename).ToString()+"MB";
                }
                if (!isCancel && isstop)
                {
                    Timelabel.Text = "录制暂停 " + string.Format(
                            "{0:00}:{1:00}",
                            Math.Floor((decimal)ts / 60),
                             ts % 60);
                }
                GenImg();
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
        }

        private void jtBtn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                CostompictureBox.Image = (Image)latestImageFromCamera1.Clone();
                Class1.khimg = (Image)latestImageFromCamera1.Clone();
                CostompictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
        }

        public void MehotdForRe()
        {
            try
            {
                GC.Collect();
                if (checkHasInstalledSoftWare())
                {
                    if (RE.IsRegeditItemExist1("GNU") && RE.IsRegeditItemExist2("x264"))
                    {
                        RE.WriteRegedit("preset", "medium", false);
                        RE.WriteRegedit("tuning", "", false);
                        RE.WriteRegedit("profile", "", false);
                        RE.WriteRegedit("avc_level", 0x00000000);
                        RE.WriteRegedit("fastdecode", 0x00000001);
                        RE.WriteRegedit("zerolatency", 0x00000001);
                        // RE.WriteRegedit("preset", "medium", false);
                     //   RE.WriteRegedit("zerolatency", 0x00000000);
                        RE.WriteRegedit("keep_input_csp", 0x00000000);
                        RE.WriteRegedit("encoding_type", 0x00000004);
                        RE.WriteRegedit("quantizer", 0x00000017);
                        RE.WriteRegedit("ratefactor", 0x000000E6);
                        RE.WriteRegedit("passbitrate", 0x00000320);
                        RE.WriteRegedit("pass_number", 0x00000001);
                        RE.WriteRegedit("fast1pass", 0x00000000);
                        RE.WriteRegedit("createstats", 0x00000000);
                        RE.WriteRegedit("updatestats", 0x00000001);
                        RE.WriteRegedit("output_mode", 0x00000000);
                        RE.WriteRegedit("fourcc_num", 0x00000000);
                        RE.WriteRegedit("vd_hack", 0x00000001);
                        RE.WriteRegedit("sar_width", 0x00000001);
                        RE.WriteRegedit("sar_height", 0x00000001);
                        RE.WriteRegedit("log_level", 0x00000000);
                        RE.WriteRegedit("psnr", 0x00000001);
                        RE.WriteRegedit("ssim", 0x00000001);
                        RE.WriteRegedit("no_asm", 0x00000000);
                        RE.WriteRegedit("disable_decoder", 0x00000000);
                        RE.WriteRegedit("statsfile", ".\x264.stats", false);
                        RE.WriteRegedit("output_file", "", false);
                        RE.WriteRegedit("extra_cmdline", "", false);
                    }
                    if (RE.IsRegeditItemExist1("GNU"))
                    {
                        RegistryKey key = Registry.CurrentUser;
                        RegistryKey software = key.CreateSubKey("software\\GNU\\x264");
                        key.Close();
                    }
                    if (!RE.IsRegeditItemExist1("GNU"))
                    {
                        RegistryKey key = Registry.CurrentUser;
                        RegistryKey software = key.CreateSubKey("software\\GNU\\x264");
                        key.Close();
                    }
                }
                if (!checkHasInstalledSoftWare())
                {
                    MessageBox.Show("系统检测到本机未安装x264编码器，将自动为你安装，请在弹出的窗口中依次点击[Next]即可，安装完成后，重启本软件!","提示",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    Process.Start("x264vfw.exe").WaitForExit();
                    System.Environment.Exit(0);
                }
                    GC.Collect();
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
        }
        private bool checkHasInstalledSoftWare()
        {
            try
            {
                Microsoft.Win32.RegistryKey uninstallNode = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
                foreach (string subKeyName in uninstallNode.GetSubKeyNames())
                {
                    Microsoft.Win32.RegistryKey subKey = uninstallNode.OpenSubKey(subKeyName);
                    object disName = subKey.GetValue("DisplayName");
                    if (disName != null)
                    {
                        //  MessageBox.Show(displayName.ToString());
                        if (disName.ToString().IndexOf("x264vfw") != -1)
                        {
                            // MessageBox.Show(disName.ToString());
                            return true;
                            // MessageBox.Show(displayName.ToString()); 


                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (DialogResult.Yes == MessageBox.Show("录制前，请确认银行区及客户区的摄像头是否正确，包括位置和角度。其中，银行区摄像头对内拍摄，客户区摄像头拍摄客户（较大画面）。并确认麦克风选择是否正确？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                { if (IsRecording == false && pictureBox1.Image != null)
                {
                    VideoEncoderCombo.Enabled = costomcomboBox.Enabled = BankcomboBox.Enabled = AudiocomboBox.Enabled = trackBar1.Enabled = false;
                    InputForm MsgboxInf = new InputForm()
                    {
                        StartPosition = FormStartPosition.CenterParent
                    };
                    MsgboxInf.ShowDialog();
                    ini.Write("AudioDevice", "DeviceID", SelectedAudioSourceIndex.ToString());
                    if (Class1.ixqx == false)
                    {
                        timer1.Start();
                        isCancel = false;

                        StartRecord();
                        kslzBtn.Enabled = false;
                        ztlzBtn.Enabled = true;
                        jxlzBtn.Enabled = false;
                        jslzBtn.Enabled = true;
                    }
                    else
                    {
                        VideoEncoderCombo.Enabled = AudiocomboBox.Enabled = trackBar1.Enabled = true;
                    }
                }
                else
                {
                    MessageBox.Show("请先开启摄像头，确认有画面后重试!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }

        }
        public void SavePz()
        {
            try
            {
                int zw = 828;
                int zh = 1169;
                try
                {
                    if (ini.Read("Import", "width") != "")
                    {
                        zw = int.Parse(ini.Read("Import", "width"));
                    }
                    if (ini.Read("Import", "height") != "")
                    {
                        zh = int.Parse(ini.Read("Import", "height"));
                    }
                }
                catch
                {
                }
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
                    tag = "法人或负责人签字：";
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
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }

        }
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                VideoEncoderCombo.Enabled = AudiocomboBox.Enabled = trackBar1.Enabled = true;
                isCancel = true;
                kslzBtn.Enabled = true;
                ztlzBtn.Enabled = false;
                jxlzBtn.Enabled = false;
                jslzBtn.Enabled = false;
                if (!Directory.Exists("List"))
                {
                    Directory.CreateDirectory("List");
                }
                string name = Class1.org + "_" + Class1.khmc + "_" + Class1.zhxz + "_" + DateTime.Now.ToString("yyyyMMdd") + "_面签凭证" + ".png";
                try
                {
                    SavePz();
                }
                catch
                { }
                //  img.Save(AppDomain.CurrentDomain.BaseDirectory + "Data\\" + name);
                File.AppendAllText("List\\history.txt", Class1.khmc + "," + Class1.mqrq + "," + lastFileName + "," + AppDomain.CurrentDomain.BaseDirectory + "PngData\\" + name + "\r\n");
                if (Class1.isneedys == true)
                {
                    if (VideoEncoderCombo.Text.IndexOf("x264") != -1)
                    {

                        if (DialogResult.Yes == MessageBox.Show("系统检测到当前选择的视频编码器生成的视频，可无需压缩，压缩可能导致视频画质降低,你确定要压缩吗?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                        {
                            CompressForm MsgboxInf1 = new CompressForm()
                            {
                                StartPosition = FormStartPosition.CenterParent
                            };
                            MsgboxInf1.ShowDialog();
                        }
                    }
                    CompressForm MsgboxInf = new CompressForm()
                    {
                        StartPosition = FormStartPosition.CenterParent
                    };
                    MsgboxInf.ShowDialog();
                }

                try
                {
                    PrintForm dy = new PrintForm(0);
                    dy.StartPosition = FormStartPosition.CenterParent;
                    dy.ShowDialog();

                }
                catch (Exception ex)
                {
                    if (Class1.isgc)
                    {
                        WriteErorrLog(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
        }

        private void AudiocomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedAudioSourceIndex = AudiocomboBox.SelectedIndex;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsRecording && screenThread.IsAlive)
                {
                    screenThread.Suspend();
                    isstop = true;
                    kslzBtn.Enabled = false;
                    ztlzBtn.Enabled = false;
                    jxlzBtn.Enabled = true;
                    jslzBtn.Enabled = false;
                }
            }

            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsRecording && screenThread.IsAlive)
                {
                    isstop = false;
                    screenThread.Resume();
                    kslzBtn.Enabled = false;
                    ztlzBtn.Enabled = true;
                    jxlzBtn.Enabled = false;
                    jslzBtn.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
        }
        private PrintDocument pd = new PrintDocument();
        private void PicturePrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
        }
        private void bcBtn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "图片文件(*.jpg)|*.jpg|All Files|*.*"
                };
                bool flag = saveFileDialog.ShowDialog() == DialogResult.OK;
                if (flag)
                {
                    string fileName = saveFileDialog.FileName;
                    
                    CostompictureBox.Image.Save(fileName);

                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
        }

        private void dyBtn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                PrintForm dy = new PrintForm(1);
                dy.StartPosition = FormStartPosition.CenterParent;
                dy.ShowDialog();
             
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
           
        }

        private void label4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
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
        private void label2_Click(object sender, EventArgs e)
        {
            try
            {
                if (isCancel && IsRecording == false)
                {

                    if (lastFileName != null && lastFileName != ""  && File.Exists(lastFileName))
                    {
                        System.Diagnostics.Process.Start("explorer.exe", lastFileName);
                        return;
                    }
                    if (lastFileName != null && lastFileName != ""  && File.Exists(lastFileName.Replace(".avi", ".mp4")))
                    {
                        System.Diagnostics.Process.Start("explorer.exe", lastFileName.Replace(".avi", ".mp4"));
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("系统正在录制视频，请在录制结束后播放视频文件!","警告",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
            // ExplorerFile(lastFileName);
        }

        private void AddFapiaoBtn_Click(object sender, EventArgs e)
        {
            try
            {
               // MessageBox.Show(lastFileName);
                if (lastFileName!=null && lastFileName != "" && File.Exists(lastFileName))
                {
                    ExplorerFile(lastFileName);
                    return;
                }
                if (lastFileName != null &&  lastFileName !="" && File.Exists(lastFileName.Replace(".avi", ".mp4")))
                {
                    ExplorerFile(lastFileName.Replace(".avi", ".mp4"));
                    return;
                }
               // if (lastFileName == "")
                {
                    System.Diagnostics.Process.Start("explorer.exe", AppDomain.CurrentDomain.BaseDirectory+"File");
                }
            }
            catch(Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
        }
        public  void WriteErorrLog(Exception ex)
        {
            if (ex == null)
                return;                         //ex = null 返回
            StreamWriter write = null;
            DateTime dt = DateTime.Now;       // 设置日志时间
            string time = dt.ToString("yyyy-mm-dd HH:mm:ss"); //年-月-日 时：分：秒
            string LogName = dt.ToString("yyyy-mm-dd")+"_Error.log";       //日志名称
            string LogPath = AppDomain.CurrentDomain.BaseDirectory+"Log\\";   //日志存放路径
           
            string Log = LogPath + LogName;   //路径 + 名称
            if (!File.Exists(Log))             //是否存在
            {
                Directory.CreateDirectory(LogPath);   //创建文件夹
                write = File.CreateText(LogName);     // 创建日志
            }
            else
            {
                write = File.AppendText(Log);         //追加，添加错误信息；
            }
            write.WriteLine(time);
            write.WriteLine(ex.Message);
            write.WriteLine("异常信息：" + ex.ToString());
            write.WriteLine("异常堆栈：" + ex.StackTrace.ToString());
            write.WriteLine("/r/n-----------------");
            write.Flush();
            write.Dispose();

        }
        private void button3_Click_1(object sender, EventArgs e)
        {
            if (isCancel && IsRecording == false)
            {if (VideoEncoderCombo.Text.IndexOf("x264") != -1)
                {
                    if (DialogResult.Yes == MessageBox.Show("系统检测到当前选择的视频编码器生成的视频，可无需压缩，压缩可能导致视频画质降低,你确定要压缩吗?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        CompressForm MsgboxInf1 = new CompressForm()
                        {
                            StartPosition = FormStartPosition.CenterParent
                        };
                        MsgboxInf1.ShowDialog();
                    }
                }
               
                
            }
            else
            {
                MessageBox.Show("系统正在录制视频，请在录制结束后压缩视频文件!","警告",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ckBtn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {if (CostompictureBox.Image != null)
            {if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Picture"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Picture");
                }
                CostompictureBox.Image.Save(AppDomain.CurrentDomain.BaseDirectory + "Picture\\客户区截图.bmp");
            }
            Process.Start(new ProcessStartInfo("mspaint.exe", AppDomain.CurrentDomain.BaseDirectory + "Picture\\客户区截图.bmp"));
        }
        int khzxz = 0;
        int yhzxz = 0;
        private void khxzBtn_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show(khxzBtn, 20, 20);
        }

        private void 旋转90ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            khzxz = 90;
            ini.Write("VideoCam", "VideoCam1Anglet", khzxz.ToString());
        }

        private void 旋转180ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            khzxz = 180;
            ini.Write("VideoCam", "VideoCam1Anglet", khzxz.ToString());
        }

        private void 旋转270ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            khzxz = 270;
            ini.Write("VideoCam", "VideoCam1Anglet", khzxz.ToString());
        }

        private void 旋转360ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            khzxz = 360;
            ini.Write("VideoCam", "VideoCam1Anglet", "0");
        }

        private void yhxzBtn_Click(object sender, EventArgs e)
        {
            contextMenuStrip2.Show(yhxzBtn, 20, 20);
        }

        private void 旋转90ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            yhzxz = 90;
            ini.Write("VideoCam", "VideoCam2Anglet", yhzxz.ToString());
        }

        private void 旋转180ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            yhzxz = 180;
            ini.Write("VideoCam", "VideoCam2Anglet", yhzxz.ToString());
        }

        private void 旋转270ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            yhzxz = 270;
            ini.Write("VideoCam", "VideoCam2Anglet", yhzxz.ToString());
        }

        private void 旋转360ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            yhzxz = 360;
            ini.Write("VideoCam", "VideoCam2Anglet","0");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (button2.Text == "开启摄像头")
                {
                    if (camCount == 0)
                    {
                        MessageBox.Show("摄像头数为0，无法开启摄像头！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (costomcomboBox.Text == BankcomboBox.Text && camCount > 0)
                    {
                        MessageBox.Show("摄像头设置错误，不能设置为同一个摄像头！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (!this.VideoCam1.IsRunning)
                    {
                        this.VideoCam1.Start();
                    }
                    if (!this.VideoCam2.IsRunning)
                    {
                        this.VideoCam2.Start();
                    }
                    if (getCameraCount() == 0)
                    {
                        button2.Text = "开启摄像头";
                    }
                    else
                    {
                        ini.Write("VideoCam", "VideoCam1Source", costomcomboBox.SelectedIndex.ToString());
                        ini.Write("VideoCam", "VideoCam2Source", BankcomboBox.SelectedIndex.ToString());
                        timer1.Enabled = true;
                        button2.Text = "关闭摄像头";
                        costomcomboBox.Enabled = BankcomboBox.Enabled = false;
                    }
                }
                else
                {

                    if (isCancel && IsRecording == false)
                    {
                        costomcomboBox.Enabled = BankcomboBox.Enabled = true;
                        timer1.Enabled = false;
                        if (this.VideoCam1.IsRunning)
                        {
                            this.VideoCam1.Stop();
                        }
                        if (this.VideoCam2.IsRunning)
                        {
                            this.VideoCam2.Stop();
                        }
                        pictureBox1.Image = null;
                        {
                            button2.Text = "开启摄像头";
                        }

                    }
                    else
                    {
                        MessageBox.Show("系统正在录制视频，请勿关闭摄像头!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
        }

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("mmsys.cpl");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (isCancel && IsRecording == false)
                {
                    PrintForm dy = new PrintForm(0);
                    dy.StartPosition = FormStartPosition.CenterParent;
                    dy.ShowDialog();
                }
                else
                {
                    MessageBox.Show("系统正在录制视频，请在结束录制后打印凭证!","警告",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
        }
        int VideoID = -1;
        int VideoID2 = -1;
        private void costomcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
          
            {
                VideoCaptureDevice videoSource = setVideoCamera(costomcomboBox.SelectedIndex);
                this.VideoCam1.VideoSource = videoSource;
                VideoID = costomcomboBox.SelectedIndex;


            }
        }

        private void BankcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            {
                VideoCaptureDevice videoSource = setVideoCamera(BankcomboBox.SelectedIndex);
                this.VideoCam2.VideoSource = videoSource;
                VideoID2 = BankcomboBox.SelectedIndex;
            }
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                AboutForm dy = new AboutForm();
                dy.StartPosition = FormStartPosition.CenterParent;
                dy.ShowDialog();

            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
        }

        private void label7_MouseMove(object sender, MouseEventArgs e)
        {
            label7.ForeColor = Color.White;
            label7.BackColor = Color.Red;
        }

        private void label7_MouseLeave(object sender, EventArgs e)
        {
            label7.ForeColor = Color.White;
            label7.BackColor = Color.Transparent;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                HistoryListForm dy = new HistoryListForm();
                dy.StartPosition = FormStartPosition.CenterParent;
                dy.ShowDialog();

            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
        }
        private double ManagerDoubleValue(double _value, int Length)
        {
            if (Length < 0)
            {
                Length = 0;
            }
            return System.Math.Round(_value, Length);
        }


        private List<HardDiskPartition> GetDiskListInfo()
        {
            List<HardDiskPartition> list = null;
            //指定分区的容量信息
            try
            {
                SelectQuery selectQuery = new SelectQuery("select * from win32_logicaldisk");

                ManagementObjectSearcher searcher = new ManagementObjectSearcher(selectQuery);

                ManagementObjectCollection diskcollection = searcher.Get();
                if (diskcollection != null && diskcollection.Count > 0)
                {
                    list = new List<HardDiskPartition>();
                    HardDiskPartition harddisk = null;
                    foreach (ManagementObject disk in searcher.Get())
                    {
                        int nType = Convert.ToInt32(disk["DriveType"]);
                        if (nType != Convert.ToInt32(DriveType.Fixed))
                        {
                            continue;
                        }
                        else
                        {
                            harddisk = new HardDiskPartition();
                            harddisk.FreeSpace = Convert.ToDouble(disk["FreeSpace"]) / (1024 * 1024 * 1024);
                            harddisk.SumSpace = Convert.ToDouble(disk["Size"]) / (1024 * 1024 * 1024);
                            harddisk.PartitionName = disk["DeviceID"].ToString();
                            list.Add(harddisk);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
            return list;
        }


        private void button5_Click(object sender, EventArgs e)
        {if (File.Exists("Config\\话术.txt"))
            {
                System.Diagnostics.Process.Start("notepad.exe", "Config\\话术.txt");
            }
        }
      public double GetFileVersionInfo(string path)
        {
          //  double ret = 0.0f;
            System.IO.FileInfo fileInfo = null;
            try
            {
                fileInfo = new System.IO.FileInfo(path);
            }
            catch (Exception e)
            {
                return 0.0f;
                // 其他处理异常的代码
            }
            // 如果文件存在
            if (fileInfo != null && fileInfo.Exists)
            {
                System.Diagnostics.FileVersionInfo info = System.Diagnostics.FileVersionInfo.GetVersionInfo(path);

                return System.Math.Ceiling(fileInfo.Length / 1024.0) / 1024.0;
              
            }
            else
            {
                return 0.0f;
            }
            // 末尾空一行
          
        }
        IniHelper ini = new IniHelper();
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
             

                string programPath = System.Environment.CurrentDirectory;
                string volume = programPath.Substring(0, System.Windows.Forms.Application.StartupPath.IndexOf(":"));
            //    MessageBox.Show(volume);
                List<HardDiskPartition> listInfo = GetDiskListInfo();
                if (listInfo != null && listInfo.Count > 0)
                {
                    //listBox1.Items.Clear();
                    foreach (HardDiskPartition disk in listInfo)
                    {
                        if (disk.PartitionName.Trim().ToUpper().Replace(":","") == volume.ToUpper())
                        {
                            //listBox1.Items.Add(string.Format("{0}  总空间：{1} GB,剩余:{2} GB", disk.PartitionName, ManagerDoubleValue(disk.SumSpace, 1), ManagerDoubleValue(disk.FreeSpace, 1)));
                          label1.Text= "磁盘容量："  + ManagerDoubleValue(disk.FreeSpace, 1) + "GB/"+ ManagerDoubleValue(disk.SumSpace, 1)+"GB";
                            if (ManagerDoubleValue(disk.FreeSpace, 1)<=0.2)
                            {
                                MessageBox.Show("磁盘容量不足，请清空后重试!","警告",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                                System.Environment.Exit(0);
                                return;
                            }
                            if (videoencodeIndex <= 1)
                            {
                                if (ManagerDoubleValue(disk.FreeSpace, 1) <= 0.5 && ManagerDoubleValue(disk.FreeSpace, 1) >= 0.3)
                                {
                                    trackBar1.Value = 5;
                                }
                                if (ManagerDoubleValue(disk.FreeSpace, 1) >= 0.8)
                                {
                                    trackBar1.Value = 7;
                                }
                                if (ManagerDoubleValue(disk.FreeSpace, 1) >= 1.1)
                                {
                                    trackBar1.Value = 8;
                                }
                                if (ManagerDoubleValue(disk.FreeSpace, 1) >= 2)
                                {
                                    trackBar1.Value = 9;
                                }
                                if (ManagerDoubleValue(disk.FreeSpace, 1) >= 3)
                                {
                                    trackBar1.Value = 10;
                                }
                            }
                        }
                       }
                }
                try
                {
                    if (ini.Read("Debug", "Switch") == "1" || ini.Read("Debug", "Switch").ToUpper() == "TRUE")
                    {
                        checkBox1.Checked = true;
                    }
                    if (ini.Read("Debug", "Switch") == "0" || ini.Read("Debug", "Switch").ToUpper() == "FALSE")
                    {
                        checkBox1.Checked = false;
                    }
                    try
                    {

                        //生成VBS代码
                        string vbs = this.CreateVBS();
                        //以文件形式写入临时文件夹
                        this.WriteToTemp(vbs);
                        //调用Process执行
                        this.RunProcess();
                    }
                    catch (Exception ex)
                    {
                        if (Class1.isgc)
                        {
                            WriteErorrLog(ex);
                        }
                    }

                }
                catch (Exception ex)
                {
                    if (Class1.isgc)
                    {
                        WriteErorrLog(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }


            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "File"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "File");
            }
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Log"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Log");
            }
            try
            {
                DeleteLogs((AppDomain.CurrentDomain.BaseDirectory + "Log"));
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
            outputFolder = AppDomain.CurrentDomain.BaseDirectory + "File";
            if (ini.Read("Tag", "Read") == "" || ini.Read("Tag", "Read") == "0")
            {
                DialogResult result = MessageBox.Show("欢迎使用面签录音录像系统，在使用本系统前，我需要确认你是否阅读了使用手册，部分操作在阅读使用手册前提下才能正常操作。如你已经阅读，请点击是；如未阅读，请点击否，系统将指引你打开使用手册。请问你是否已经阅读使用手册？","系统提示",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {if (File.Exists("面签录音录像系统使用手册.pdf"))
                    {
                        Process.Start("面签录音录像系统使用手册.pdf");
                    }
                    else
                    {
                        MessageBox.Show("使用手册文件丢失，系统无法完成后续指引操作，已自动跳过!","提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    }
                }
                ini.Write("Tag","Read","1");
                
            }
            MehotdForRe();
            if (ini.Read("Video", "Frame") != "")
            {
                frame = int.Parse(ini.Read("Video", "Frame"));
            }
        }
        int frame = 16;
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            double ret = 0f;
            try
            {
                string programPath = System.Environment.CurrentDirectory;
                string volume = programPath.Substring(0, System.Windows.Forms.Application.StartupPath.IndexOf(":"));
                //    MessageBox.Show(volume);
                List<HardDiskPartition> listInfo = GetDiskListInfo();
                if (listInfo != null && listInfo.Count > 0)
                {
                    //listBox1.Items.Clear();
                    foreach (HardDiskPartition disk in listInfo)
                    {
                        if (disk.PartitionName.Trim().ToUpper().Replace(":", "") == volume.ToUpper())
                        {
                            //listBox1.Items.Add(string.Format("{0}  总空间：{1} GB,剩余:{2} GB", disk.PartitionName, ManagerDoubleValue(disk.SumSpace, 1), ManagerDoubleValue(disk.FreeSpace, 1)));
                           ret= ManagerDoubleValue(disk.FreeSpace, 1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
            if (videoencodeIndex <= 1)
            {
                if (trackBar1.Value >= 7 && ret < 0.7)
                {
                    MessageBox.Show("你设置视频画质质量超过70，这将占用更多的内存和空间。当前磁盘可用空间不满足条件！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    trackBar1.Value = 6;
                }
                if (trackBar1.Value >= 7 && ret >= 1)
                {
                    MessageBox.Show("你设置视频画质质量超过70，这将占用更多的内存和空间！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        public class HardDiskPartition
        {
            private string _PartitionName;//分区名称
            private double _FreeSpace;//空余大小
            private double _SumSpace;//总空间
            public double FreeSpace
            {
                get { return _FreeSpace; }
                set { this._FreeSpace = value; }
            }
            /// 
            /// 使用空间
            /// 
            public double UseSpace
            {
                get { return _SumSpace - _FreeSpace; }
            }
            /// 
            /// 总空间
            /// 
            public double SumSpace
            {
                get { return _SumSpace; }
                set { this._SumSpace = value; }
            }
            /// 
            /// 分区名称
            /// 
            public string PartitionName
            {
                get { return _PartitionName; }
                set { this._PartitionName = value; }
            }
            /// 
            /// 是否主分区
            /// 
            public bool IsPrimary
            {
                get
                {
                    //判断是否为系统安装分区
                    if (System.Environment.GetEnvironmentVariable("windir").Remove(2) == this._PartitionName)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        private void VideoEncoderCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                bool ishavex264 = false;
                foreach (string s in VideoEncoderCombo.Items)
                {
                    if (s.IndexOf("x264") != -1)
                    {
                        ishavex264 = true;
                        break;
                    }
                }
                if (ishavex264 && VideoEncoderCombo.Text.IndexOf("x264")==-1)
                {
                    MessageBox.Show("系统检测到你已安装x264视频编码器，推荐使用该编码器，你也可以使用其他的编码器，但可能存在生成视频体积过大等问题，除非当前编码器无法使用，否则不建议使用其他编码器！","提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
                if (VideoEncoderCombo.SelectedIndex <= 1)
                {
                    Class1.isneedys = true;
                }
                else
                {
                    Class1.isneedys = false;
                }
                ini.Write("Video", "VideoEncoderID", VideoEncoderCombo.SelectedIndex.ToString());
                videoencodeIndex = VideoEncoderCombo.SelectedIndex;
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }
        private string CreateVBS()
        {
            string vbs = string.Empty;
            vbs += ("set WshShell = WScript.CreateObject(\"WScript.Shell\")\r\n");
            vbs += ("strDesktop = WshShell.SpecialFolders(\"Desktop\")\r\n");
            vbs += ("set oShellLink = WshShell.CreateShortcut(strDesktop & \"\\面签录音录像系统.lnk\")\r\n");
            vbs += ("oShellLink.TargetPath = \"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\"\r\n");
            vbs += ("oShellLink.WindowStyle = 1\r\n");
            vbs += ("oShellLink.Description = \"fsvrt\"\r\n");
            vbs += ("oShellLink.WorkingDirectory = \"" + System.Environment.CurrentDirectory + "\"\r\n");
            vbs += ("oShellLink.Save");
            return vbs;
        }
        ///
        /// 写入临时文件
        ///
        ///
        private void WriteToTemp(string vbs)
        {
            if (!string.IsNullOrEmpty(vbs))
            {
                //临时文件
                string tempFile = Environment.GetFolderPath(Environment.SpecialFolder.Templates) + "\\temp.vbs";
                //写入文件
                FileStream fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write);
                try
                {
                    //这里必须用UnicodeEncoding. 因为用UTF-8或ASCII会造成VBS乱码
                    System.Text.UnicodeEncoding uni = new UnicodeEncoding();
                    byte[] b = uni.GetBytes(vbs);
                    fs.Write(b, 0, b.Length);
                    fs.Flush();
                    fs.Close();
                }
                catch (Exception ex)
                {
                    if (Class1.isgc)
                    {
                        WriteErorrLog(ex);
                    }
                }
                finally
                {
                    //释放资源
                    fs.Dispose();
                }
            }
        }
        ///
        /// 执行VBS中的代码
        ///
        private void RunProcess()
        {
            string tempFile = Environment.GetFolderPath(Environment.SpecialFolder.Templates) + "\\temp.vbs";
            if (File.Exists(tempFile))
            {
                //执行VBS
                Process.Start(tempFile);
            }
        }
        private void linkLabel2_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("x264cmd.bat");
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            try
            {
                {
                    System.Diagnostics.Process.Start("explorer.exe", AppDomain.CurrentDomain.BaseDirectory + "PngData");
                }
            }
            catch (Exception ex)
            {
                if (Class1.isgc)
                {
                    WriteErorrLog(ex);
                }
            }
        }

        private void costomcomboBox_Click(object sender, EventArgs e)
        {
            MessageBox.Show("请先关闭摄像头，然后切换摄像头!","提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //Class1.isgc = checkBox1.Checked;
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            Class1.isgc = checkBox1.Checked;
            ini.Write("Debug","Switch",checkBox1.Checked.ToString());
        }
    }
}
