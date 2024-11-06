using Base.UI;
using Base.UI.MyPanel;
using Library;
using Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

//Junzhe 20230817
//Ricardo 20230829
//Ziyun 20230911
//Ricardo 20231010

namespace Base
{
    public partial class Main : Form
    {
        public static Boolean isCustomIdNull = true;//扳手编码状态
        public static Boolean isQuitPopup = false;
        public static String activeForm = "";//激活的窗口
        private static String picPath = Application.StartupPath + @"\logo";//加载页面图片所在路径

        MenuCalPanel myMenuCalPanel = new MenuCalPanel();
        MenuConnectPanel myMenuConnectPanel = new MenuConnectPanel();
        MenuCurveDataPanel myCurveDataPanel = new MenuCurveDataPanel();
        MenuActualDataPanel myMenuActualDataPanel = new MenuActualDataPanel();

        public double percent;
        public double initWidth;
        public double newWidth;

        //获取"激活的窗口"字段
        public static new String ActiveForm
        {
            set
            {
                activeForm = value;
            }
            get
            {
                return activeForm;
            }
        }

        public Main()
        {
            InitializeComponent();
        }

        //加载
        private void Koeorws_Load(object sender, EventArgs e)
        {
            //先显示加载界面
            this.Hide();
            MenuLoadForm myMenuLoadForm = new MenuLoadForm();
            myMenuLoadForm.ShowDialog();

            //
            panel5.Visible = false;
            panel6.Visible = false;
            panel8.Visible = false;

            //
            ucRadioButton1.Visible = true;
            ucRadioButton2.Visible = true;

            //
            myMenuCalPanel.Visible = false;
            myCurveDataPanel.Visible = false;
            myMenuActualDataPanel.Visible = false;
            myMenuConnectPanel.Visible = false;

            //
            myMenuConnectPanel.Visible = true;
            myMenuConnectPanel.Width = splitContainer1.Panel1.Width;
            myMenuConnectPanel.Height = splitContainer1.Panel1.Height;
            splitContainer1.Panel1.Controls.Add(myMenuConnectPanel);
            myMenuConnectPanel.BringToFront();

            //
            if (!Directory.Exists(picPath)) Directory.CreateDirectory(picPath);
            else
            {
                List<string> fileNames = GetPictureFiles(picPath);

                foreach (string fileName in fileNames)
                {
                    if (Path.GetFileName(fileName).Split('.')[0] == "logo")
                    {
                        pictureBox8.Image = Image.FromFile(Application.StartupPath + @"\logo\" + fileName);
                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;//加载的图片充满picrurebox大小
                    }
                }
            }
        }

        //设备连接
        private void panel1_Click(object sender, EventArgs e)
        {
            //
            if (activeForm == "设备连接") return;

            //
            myMenuCalPanel.Visible = false;
            myCurveDataPanel.Visible = false;
            myMenuConnectPanel.Visible = true;
            myMenuActualDataPanel.Visible = false;

            //
            myMenuConnectPanel.Width = splitContainer1.Panel1.Width;
            myMenuConnectPanel.Height = splitContainer1.Panel1.Height;
            splitContainer1.Panel1.Controls.Add(myMenuConnectPanel);
            myMenuConnectPanel.BringToFront();
            activeForm = "设备连接";

            //
            panel5.Visible = false;
            panel6.Visible = false;
            panel8.Visible = false;

            //
            ucRadioButton1.Visible = true;
            ucRadioButton2.Visible = true;
        }

        //设备参数
        private void panel2_Click(object sender, EventArgs e)
        {
            //提示
            if (MyDevice.mSUT.isActive == false)
            {
                if (MyDevice.languageType == 0)
                {
                    MessageBox.Show("请先连接设备！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    MessageBoxEX.Show("Please connect the device first！", "Hint", MessageBoxButtons.OK, new string[] { "OK" });
                }
                return;
            }

            //
            if (activeForm == "设备参数") return;

            //
            myMenuCalPanel.Visible = true;
            myCurveDataPanel.Visible = false;
            myMenuConnectPanel.Visible = false;
            myMenuActualDataPanel.Visible = false;

            //
            myMenuCalPanel.Width = splitContainer1.Panel1.Width;
            myMenuCalPanel.Height = splitContainer1.Panel1.Height;
            splitContainer1.Panel1.Controls.Add(myMenuCalPanel);
            myMenuCalPanel.BringToFront();
            activeForm = "设备参数";

            //
            panel5.Visible = false;
            panel6.Visible = false;
            panel8.Visible = false;

            //
            ucRadioButton1.Visible = false;
            ucRadioButton2.Visible = false;
        }

        //实时数据
        private void panel3_Click(object sender, EventArgs e)
        {
            //提示
            if (MyDevice.mSUT.isActive == false)
            {
                if (MyDevice.languageType == 0)
                {
                    MessageBox.Show("请先连接设备！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    MessageBoxEX.Show("Please connect the device first！", "Hint", MessageBoxButtons.OK, new string[] { "OK" });
                }

                return;
            }
            if (isCustomIdNull)
            {
                if (MyDevice.languageType == 0)
                {
                    MessageBox.Show("请输入扳手编码并确认！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    MessageBoxEX.Show("Please enter the wrench code and confirm！", "Hint", MessageBoxButtons.OK, new string[] { "OK" });
                }
                return;
            }

            //
            if (activeForm == "实时数据") return;

            //
            myMenuCalPanel.Visible = false;
            myCurveDataPanel.Visible = false;
            myMenuConnectPanel.Visible = false;
            myMenuActualDataPanel.Visible = true;

            //
            myMenuActualDataPanel.Width = splitContainer1.Panel1.Width;
            myMenuActualDataPanel.Height = splitContainer1.Panel1.Height;
            splitContainer1.Panel1.Controls.Add(myMenuActualDataPanel);
            myMenuActualDataPanel.BringToFront();
            activeForm = "实时数据";

            //
            panel5.Visible = true;
            panel6.Visible = true;
            panel8.Visible = true;

            //
            ucRadioButton1.Visible = false;
            ucRadioButton2.Visible = false;
        }

        //曲线数据
        private void panel4_Click(object sender, EventArgs e)
        {
            //提示
            if (MyDevice.mSUT.isActive == false)
            {
                if (MyDevice.languageType == 0)
                {
                    MessageBox.Show("请先连接设备！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    MessageBoxEX.Show("Please connect the device first！", "Hint", MessageBoxButtons.OK, new string[] { "OK" });
                }
                return;
            }
            if (isCustomIdNull)
            {
                if (MyDevice.languageType == 0)
                {
                    MessageBox.Show("请输入扳手编码并确认！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    MessageBoxEX.Show("Please enter the wrench code and confirm！", "Hint", MessageBoxButtons.OK, new string[] { "OK" });
                }
                return;
            }

            //
            if (activeForm == "曲线数据") return;

            //
            myMenuCalPanel.Visible = false;
            myCurveDataPanel.Visible = true;
            myMenuConnectPanel.Visible = false;
            myMenuActualDataPanel.Visible = false;

            //
            myCurveDataPanel.Width = splitContainer1.Panel1.Width;
            myCurveDataPanel.Height = splitContainer1.Panel1.Height;
            splitContainer1.Panel1.Controls.Add(myCurveDataPanel);
            myCurveDataPanel.BringToFront();
            activeForm = "曲线数据";

            //
            panel5.Visible = false;
            panel6.Visible = false;
            panel8.Visible = false;

            //
            ucRadioButton1.Visible = false;
            ucRadioButton2.Visible = false;
        }

        //数据导入(导入缓存数据)
        private void panel5_Click(object sender, EventArgs e)
        {
            if (!activeForm.Equals("实时数据")) return;

            isQuitPopup = true;

            myMenuActualDataPanel.Visible = false;
            myMenuActualDataPanel.Visible = true;
        }

        //数据导出
        private void panel6_Click(object sender, EventArgs e)
        {
            isQuitPopup = true;

            if (!activeForm.Equals("实时数据")) return;

            string datFolderPath = Application.StartupPath + @"\dat"; //导出目录

            if (!Directory.Exists(datFolderPath))
            {
                Directory.CreateDirectory(datFolderPath);
            }

            string myExcel = datFolderPath + "\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";  //导出文件名
            if (myMenuActualDataPanel.saveActualDataToExcel(myExcel))
            {
                if (MyDevice.languageType == 0)
                {
                    MessageBox.Show("导出EXCEL成功！");
                }
                else
                {
                    MessageBoxEX.Show("Export to Excel successfully！", "Hint", MessageBoxButtons.OK, new string[] { "OK" });
                }
                Process.Start(datFolderPath);
            }
        }

        //退出系统
        private void panel7_Click(object sender, EventArgs e)
        {
            isQuitPopup = true;

            //提示
            if ((int)(MyDevice.languageType == 0 ? 
                MessageBox.Show("是否确定退出系统？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) :
                MessageBoxEX.Show("Are you sure to exit the system？", "Hint", MessageBoxButtons.OKCancel, new string[] { "OK", "Cancel" })) != 1) 
            { 
                return;
            }

            //退出所有窗口
            System.Environment.Exit(0);
        }

        //获取多种指定图片格式文件
        private List<string> GetPictureFiles(string filePath)
        {
            DirectoryInfo di = new DirectoryInfo(filePath);
            FileInfo[] files = di.GetFiles("*.*");
            string fileName;
            List<string> list = new List<string>();

            for (int i = 0; i < files.Length; i++)
            {
                fileName = files[i].Name.ToLower();
                if (fileName.EndsWith(".bmp")
                    || fileName.EndsWith(".png")
                    || fileName.EndsWith(".jpg")
                    || fileName.EndsWith("gif")
                    || fileName.EndsWith("svg")
                    || fileName.EndsWith("tif")
                    || fileName.EndsWith("webp"))
                {
                    list.Add(fileName);
                }
            }
            return list;
        }

        //窗口大小调整
        private void Koeorws_SizeChanged(object sender, EventArgs e)
        {
            myMenuCalPanel.Width = splitContainer1.Panel1.Width;
            myMenuCalPanel.Height = splitContainer1.Panel1.Height;
            myCurveDataPanel.Width = splitContainer1.Panel1.Width;
            myCurveDataPanel.Height = splitContainer1.Panel1.Height;
            myMenuConnectPanel.Width = splitContainer1.Panel1.Width;
            myMenuConnectPanel.Height = splitContainer1.Panel1.Height;
            myMenuActualDataPanel.Width = splitContainer1.Panel1.Width;
            myMenuActualDataPanel.Height = splitContainer1.Panel1.Height;
        }

        //切换语言（中文）
        private void ucRadioButton1_CheckedChangeEvent(object sender, EventArgs e)
        {
            //zh-CN 为中文，更多的关于 Culture 的字符串请查 MSDN
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN");
            //语言设置为中文
            MyDevice.languageType = 0;
            //提示
            MessageBox.Show("请重新启动软件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            //保存选择的语言
            RecordLanguage(0);
            //重启软件
            Application.Restart();
        }

        //切换语言（英文）
        private void ucRadioButton2_CheckedChangeEvent(object sender, EventArgs e)
        {
            //en 为英文，更多的关于 Culture 的字符串请查 MSDN
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
            //语言设置为英文
            MyDevice.languageType = 1;
            MessageBoxEX.Show("Please restart the software.", "Hint", MessageBoxButtons.OK, new string[] { "OK" });
            //保存选择的语言
            RecordLanguage(1);
            //重启软件
            Application.Restart();
        }

        //保存选择的语言
        public void RecordLanguage(int language)
        {
            //空
            if (MyDevice.userDAT == null)
            {
                return;
            }
            //创建新路径
            else if (!Directory.Exists(MyDevice.userDAT))
            {
                Directory.CreateDirectory(MyDevice.userDAT);
            }

            //写入
            try
            {
                string mePath = MyDevice.userDAT + @"\Language.txt";//设置文件路径
                if (File.Exists(mePath))
                {
                    System.IO.File.SetAttributes(mePath, FileAttributes.Normal);
                }
                File.WriteAllText(mePath, language.ToString());
                System.IO.File.SetAttributes(mePath, FileAttributes.ReadOnly);
            }
            catch
            {
            }
        }

        private void Main_Resize(object sender, EventArgs e)
        {
        }
    }
}
