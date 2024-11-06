using BIL;
using Library;
using Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;

//Ricardo 20230816
//Junzhe 20230817
//Ziyun 20230911

namespace Base.UI.MyPanel
{
    public partial class MenuConnectPanel : UserControl
    {
        public Boolean isConnecting = false;//是否正在连接
        public TASKS meTask = TASKS.NULL;
        private XET actXET;//需操作的设备
        private String unit = "";//单位
        private String myCOM = "COM1";//串口初始化
        private JDBC jdbc = new JDBC();//数据库操作类
        private List<DevicePara> para = new List<DevicePara>();//存参数的数据库

        private string exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;//当前进程exe路径
        AutoFormSize autoSize = new AutoFormSize();
        public MenuConnectPanel()
        {
            InitializeComponent();
            autoSize.UIComponetForm(this);
        }

        //加载页面
        private void MenuConnectPanel_Load(object sender, EventArgs e)
        {
            //初始化界面
            ucRadioButton1.Checked = true;
            panel1.Visible = true;
            panel2.Visible = false;

            //自定义控件文本单独设置
            if (MyDevice.languageType == 1)
            {
                ucBtnExt1.BtnText = "Refresh";
                ucBtnExt2.BtnText = "Connect";
                ucBtnExt3.BtnText = "Connect";
                ucBtnExt4.BtnText = "Refresh";
                ucBtnExt5.BtnText = "Scan";
                ucBtnExt6.BtnText = "Unbind";
            }

            MyDevice.myUpdate += new freshHandler(receiveData);

            //
            if (MyDevice.protocol.IsOpen)
            {
                ucCombox3.Source = new List<KeyValuePair<string, string>>();
                String[] ports = SerialPort.GetPortNames();
                ucCombox3.Source.Add(new KeyValuePair<string, string>(0.ToString(), ports[0]));
                if (ucCombox3.SelectedIndex < 0)
                {
                    ucCombox3.SelectedIndex = 0;
                }
                textBox1.Text = (MyDevice.languageType == 0 ? "设备已通过串口连接" : "The device is connected through a serial port");
                ucBtnExt1.Enabled = true;
                ucBtnExt2.Enabled = false;
            }
            else
            {
                ucBtnExt1_BtnClick(null, null);
            }
        }

        //切换页面
        private void MenuConnectPanel_VisibleChanged(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            MyDevice.myUpdate -= new freshHandler(receiveData);
        }

        //获取存储数据
        private void GetSaveData()
        {
            //初始化设备数据
            actXET = MyDevice.actDev;

            //得到数据库存储的数据
            para = jdbc.QueryDeviceParaBybohrCode(actXET.bohrCode);

            //获取当前力矩单位
            switch (actXET.torqueUnit)
            {
                case UNIT.UNIT_nm: unit = "N·m"; break;
                case UNIT.UNIT_lbfin: unit = "lbf·in"; break;
                case UNIT.UNIT_lbfft: unit = "lbf·ft"; break;
                case UNIT.UNIT_kgcm: unit = "kgf·cm"; break;
            }

            //根据数据库更新数据
            if (para != null)
            {
                //拧紧数量
                actXET.mxTable.M1.screwTotal = Convert.ToByte(para[0].screw_total);
                actXET.mxTable.M2.screwTotal = Convert.ToByte(para[1].screw_total);
                actXET.mxTable.M3.screwTotal = Convert.ToByte(para[2].screw_total);
                actXET.mxTable.M4.screwTotal = Convert.ToByte(para[3].screw_total);
                actXET.mxTable.M5.screwTotal = Convert.ToByte(para[4].screw_total);
                actXET.mxTable.M6.screwTotal = Convert.ToByte(para[5].screw_total);

                //力矩上下限
                actXET.mxTable.M1.torqueLow = Convert.ToUInt32(para[0].torque_low);
                actXET.mxTable.M2.torqueLow = Convert.ToUInt32(para[1].torque_low);
                actXET.mxTable.M3.torqueLow = Convert.ToUInt32(para[2].torque_low);
                actXET.mxTable.M4.torqueLow = Convert.ToUInt32(para[3].torque_low);
                actXET.mxTable.M5.torqueLow = Convert.ToUInt32(para[4].torque_low);
                actXET.mxTable.M6.torqueLow = Convert.ToUInt32(para[5].torque_low);
                actXET.mxTable.M1.torqueHigh = Convert.ToUInt32(para[0].torque_up);
                actXET.mxTable.M2.torqueHigh = Convert.ToUInt32(para[1].torque_up);
                actXET.mxTable.M3.torqueHigh = Convert.ToUInt32(para[2].torque_up);
                actXET.mxTable.M4.torqueHigh = Convert.ToUInt32(para[3].torque_up);
                actXET.mxTable.M5.torqueHigh = Convert.ToUInt32(para[4].torque_up);
                actXET.mxTable.M6.torqueHigh = Convert.ToUInt32(para[5].torque_up);

                //角度上下限
                actXET.mxTable.M1.angleLow = Convert.ToUInt16(para[0].angle_low);
                actXET.mxTable.M2.angleLow = Convert.ToUInt16(para[1].angle_low);
                actXET.mxTable.M3.angleLow = Convert.ToUInt16(para[2].angle_low);
                actXET.mxTable.M4.angleLow = Convert.ToUInt16(para[3].angle_low);
                actXET.mxTable.M5.angleLow = Convert.ToUInt16(para[4].angle_low);
                actXET.mxTable.M6.angleLow = Convert.ToUInt16(para[5].angle_low);
                actXET.mxTable.M1.angleHigh = Convert.ToUInt16(para[0].angle_up);
                actXET.mxTable.M2.angleHigh = Convert.ToUInt16(para[1].angle_up);
                actXET.mxTable.M3.angleHigh = Convert.ToUInt16(para[2].angle_up);
                actXET.mxTable.M4.angleHigh = Convert.ToUInt16(para[3].angle_up);
                actXET.mxTable.M5.angleHigh = Convert.ToUInt16(para[4].angle_up);
                actXET.mxTable.M6.angleHigh = Convert.ToUInt16(para[5].angle_up);

                //
                actXET.workId = para[0].work_id;
                actXET.customId = para[0].custom_id;

                if (exePath.IndexOf("Koeorws.exe") != -1)
                {
                    KoeorwsShow.isCustomIdNull = actXET.customId == "" ? true : false;
                }
                else if (exePath.IndexOf("Torque.exe") != -1)
                {
                    TorqueShow.isCustomIdNull = actXET.customId == "" ? true : false;
                }
            }
        }

        //串口刷新
        private void ucBtnExt1_BtnClick(object sender, EventArgs e)
        {
            if (MyDevice.protocol.IsOpen)
            {
                if(MyDevice.devSum != 0)
                {
                    textBox1.Text = MyDevice.languageType == 0 ? "设备已连接" : "The device is connected";
                }
            }
            else
            {
                //刷串口
                ucCombox3.Source = new List<KeyValuePair<string, string>>();
                String[] ports = SerialPort.GetPortNames();
                for (int i = 0; i < ports.Length; i++)
                {
                    ucCombox3.Source.Add(new KeyValuePair<string, string>(i.ToString(), ports[i]));
                }

                //无串口
                if (ucCombox3.Source.Count == 0)
                {
                    ucCombox3.TextValue = null;
                    myCOM = null;
                }
                //有可用窗口
                else
                {
                    ucCombox3.TextValue = MyDevice.protocol.portName;
                    if (ucCombox3.SelectedIndex < 0)
                    {
                        ucCombox3.SelectedIndex = 0;
                    }
                    myCOM = ucCombox3.TextValue;
                }
                ucBtnExt2.Enabled = true;
            }
        }

        //串口连接
        private void ucBtnExt2_BtnClick(object sender, EventArgs e)
        {
            if (MyDevice.protocol.IsOpen)
            {
                if (MyDevice.devSum != 0)
                {
                    textBox1.Text = MyDevice.languageType == 0 ? "设备已连接" : "The device is connected";
                }
                else
                {
                    textBox1.Text = "";
                    textBox1.Text = MyDevice.languageType == 0 ? "适配器已打开\r\n搜索中 ." : "The adapter is turned on\r\nSearching .";
                    ucBtnExt2.BackColor = Color.OrangeRed;
                    MyDevice.protocol.Protocol_ClearState();
                    timer1.Enabled = true;
                }
            }
            else
            {
                if (myCOM != null)
                {
                    //切换自定义通讯
                    MyDevice.mePort_SetProtocol(COMP.SelfUART);

                    //打开串口
                    MyDevice.protocol.Protocol_PortOpen(ucCombox3.TextValue, 115200);

                    //串口发送
                    if (MyDevice.protocol.IsOpen)
                    {
                        textBox1.Text = MyDevice.languageType == 0 ? "适配器已打开\r\n搜索中 ." : "The adapter is turned on\r\nSearching .";
                        ucBtnExt2.BackColor = Color.OrangeRed;
                        MyDevice.protocol.Protocol_ClearState();
                        timer1.Enabled = true;
                    }
                }
            }
        }

        //有线连接
        private void label1_Click(object sender, EventArgs e)
        {
            ucRadioButton1.Checked = true;
            ucBtnExt6.Visible = false;
            panel1.Visible = true;
            panel2.Visible = false;
        }    
        private void ucRadioButton1_CheckedChangeEvent(object sender, EventArgs e)
        {
            ucBtnExt6.Visible = false;
            panel1.Visible = true;
            panel2.Visible = false;
        }

        //蓝牙连接
        private void label2_Click(object sender, EventArgs e)
        {
            ucRadioButton2.Checked = true;
            ucBtnExt6.Visible = true;
            panel1.Visible = true;
            panel2.Visible = false;
        }
        private void ucRadioButton2_CheckedChangeEvent(object sender, EventArgs e)
        {
            ucBtnExt6.Visible = true;
            panel1.Visible = true;
            panel2.Visible = false;
        }

        //射频连接
        private void label3_Click(object sender, EventArgs e)
        {
            ucRadioButton3.Checked = true;
            panel1.Visible = false;
            panel2.Visible = true;
            panel2.Location = new Point(panel1.Location.X, panel1.Location.Y);
        }
        private void ucRadioButton3_CheckedChangeEvent(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = true;
            panel2.Location = new Point(panel1.Location.X, panel1.Location.Y);
        }

        //WiFi连接
        private void label4_Click(object sender, EventArgs e)
        {
            ucRadioButton4.Checked = true;
            panel1.Visible = false;
            panel2.Visible = true;
            panel2.Location = new Point(panel1.Location.X, panel1.Location.Y);
        }
        private void ucRadioButton4_CheckedChangeEvent(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = true;
            panel2.Location = new Point(panel1.Location.X, panel1.Location.Y);
        }

        //解绑
        private void ucBtnExt6_BtnClick(object sender, EventArgs e)
        {
            if (MyDevice.protocol.IsOpen)
            {
                meTask = TASKS.SET_RECONFIG;
                ucBtnExt6.BackColor = Color.OrangeRed;
                MyDevice.protocol.Protocol_ClearState();
                MyDevice.protocol.Protocol_SetReconfig();
            }
            else
            {
                if (myCOM != null)
                {
                    //切换自定义通讯
                    MyDevice.mePort_SetProtocol(COMP.SelfUART);

                    //打开串口
                    MyDevice.protocol.Protocol_PortOpen(ucCombox3.TextValue, 115200);

                    //串口发送
                    if (MyDevice.protocol.IsOpen)
                    {
                        meTask = TASKS.SET_RECONFIG;
                        ucBtnExt6.BackColor = Color.OrangeRed;
                        MyDevice.protocol.Protocol_ClearState();
                        MyDevice.protocol.Protocol_SetReconfig();
                    }
                }
            }
        }

        //定时器触发
        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox1.Text += ".";
            isConnecting = true;
            MyDevice.protocol.Protocol_ReadTasks();
        }
        
        //连接组合框
        private void ucCombox3_SelectedChangedEvent(object sender, EventArgs e)
        {
            if (myCOM != ucCombox3.TextValue)
            {
                MyDevice.protocol.Protocol_PortClose();
                myCOM = ucCombox3.TextValue;
            }
        }

        //委托
        private void receiveData()
        {
            //其它线程的操作请求
            if (this.InvokeRequired)
            {
                try
                {
                    freshHandler meDelegate = new freshHandler(receiveData);
                    this.Invoke(meDelegate, new object[] { });
                }
                catch
                {
                    //MessageBox.Show("MenuConnectForm receiveData err 1");
                }
            }
            //本线程的操作请求
            else
            {
                string str = MyDevice.languageType == 0 ? "读取" : "Read ";

                switch (meTask)
                {
                    default:
                        timer1.Enabled = false;
                        //刷新界面
                        textBox1.Text += "\r\n" + str + MyDevice.protocol.trTASK.ToString();
                        MyDevice.protocol.Protocol_ReadTasks();
                        if (MyDevice.protocol.trTASK == TASKS.NULL)
                        {
                            MyDevice.mSUT.isActive = true;
                            ucBtnExt2.BackColor = Color.Green;
                            textBox1.Text += "\r\n" + str + (MyDevice.languageType == 0 ? "完成" : "finished");
                            isConnecting = false;
                            GetSaveData();
                        }
                        break;
                    case TASKS.SET_RECONFIG:
                        //刷新界面
                        textBox1.Text += (MyDevice.languageType == 0 ? "\r\n设置蓝牙解绑" : "\r\nSet up Bluetooth unbinding");
                        MyDevice.protocol.Protocol_SetReconfig();
                        if (MyDevice.protocol.trTASK == TASKS.NULL)
                        {
                            ucBtnExt6.BackColor = Color.Green;
                            if (MyDevice.mSUT.IsUnbind())
                            {
                                textBox1.Text += "\r\n" + (MyDevice.languageType == 0 ? "解绑成功" : "Successfully unbound");
                            }
                            else
                            {
                                textBox1.Text += "\r\n" + (MyDevice.languageType == 0 ? "解绑失败，请重试" : "Unbinding failed, please try again");
                            }
                        }
                        break;
                }
            }
        }

        private void MenuConnectPanel_Resize(object sender, EventArgs e)
        {
            autoSize.UIComponetForm_Resize(this);
            splitContainer1.SplitterDistance = this.Width / 8 * 1;
        }
    }
}
