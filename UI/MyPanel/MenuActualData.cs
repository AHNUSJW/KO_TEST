using BIL;
using Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

//Lumi 20230914

namespace Base.UI.MyPanel
{
    public partial class MenuActualData : Form
    {
        private XET actXET;              //需操作的设备
        private TASKS nextTask;          //按键指令

        private Byte infoTick = 0;       //控制info显示时间
        private Int32 infoErr = 0;       //控制info显示时间
        private Int32 infoLevel = 0;     //控制info显示时间

        private string torqueUnit;       //扭矩单位
        private string torqueTarget = "";     //扭力值设定
        private string angleTarget = "";      //角度设定
        private Double torquePercent;    //实际扭矩与目标扭矩百分比
        private Double anglePercent;     //实际角度与目标角度百分比

        private float anglePeak = 0f;    //记录扭矩峰值
        private float torquePeak = 0f;   //记录角度峰值
        private string res = "NG";       //结果

        private string startTime = "";        //本次操作的起始时间戳

        List<DataInfo> dataInfos = new List<DataInfo>();       //数据表
        DevicePara myDevicePara = new DevicePara();            //符合M值的bohrcode的参数数据

        private byte mxscrew;                 //M1-M6模式下拧紧螺丝数量

        JDBC jdbc = new JDBC();
        private string exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;//获取当前运行的exe完整路径

        public List<DataInfo> DataInfos
        {
            get { return dataInfos; }
        }

        public byte Mxscrew
        {
            set { mxscrew = value; }
            get { return mxscrew; }
        }

        public MenuActualData()
        {
            InitializeComponent();
        }

        private void MenuActualData_Load(object sender, EventArgs e)
        {
            //初始化设备数据
            actXET = MyDevice.actDev;

            //自定义控件文本单独设置
            if (MyDevice.languageType == 1)
            {
                ucBtnExt1.BtnText = "Exit";
            }

            //获取参数信息
            myDevicePara = new DevicePara()
            {
                bohr_code = actXET.bohrCode,
                work_id = actXET.workId,
                custom_id = actXET.customId,
                m_value = "M" + (actXET.modeMx + 1).ToString(),
            };

            switch (actXET.modeMx)
            {
                case 0:
                    mxscrew = actXET.mxTable.M1.screw;
                    myDevicePara.screw_total = actXET.mxTable.M1.screwTotal.ToString();
                    angleTarget = (actXET.mxTable.M1.angleTarget / 10.0f).ToString();
                    torqueTarget = (actXET.mxTable.M1.torqueTarget / 100.0f).ToString();
                    myDevicePara.angle_low = (actXET.mxTable.M1.angleLow / 10.0f).ToString();
                    myDevicePara.angle_up = (actXET.mxTable.M1.angleHigh / 10.0f).ToString();
                    myDevicePara.torque_low = (actXET.mxTable.M1.torqueLow / 100.0f).ToString();
                    myDevicePara.torque_up = (actXET.mxTable.M1.torqueHigh / 100.0f).ToString();
                    break;
                case 1:
                    mxscrew = actXET.mxTable.M2.screw;
                    myDevicePara.screw_total = actXET.mxTable.M2.screwTotal.ToString();
                    angleTarget = (actXET.mxTable.M2.angleTarget / 10.0f).ToString();
                    torqueTarget = (actXET.mxTable.M2.torqueTarget / 100.0f).ToString();
                    myDevicePara.angle_low = (actXET.mxTable.M2.angleLow / 10.0f).ToString();
                    myDevicePara.angle_up = (actXET.mxTable.M2.angleHigh / 10.0f).ToString();
                    myDevicePara.torque_low = (actXET.mxTable.M2.torqueLow / 100.0f).ToString();
                    myDevicePara.torque_up = (actXET.mxTable.M2.torqueHigh / 100.0f).ToString();
                    break;
                case 2:
                    mxscrew = actXET.mxTable.M3.screw;
                    myDevicePara.screw_total = actXET.mxTable.M3.screwTotal.ToString();
                    angleTarget = (actXET.mxTable.M3.angleTarget / 10.0f).ToString();
                    torqueTarget = (actXET.mxTable.M3.torqueTarget / 100.0f).ToString();
                    myDevicePara.angle_low = (actXET.mxTable.M3.angleLow / 10.0f).ToString();
                    myDevicePara.angle_up = (actXET.mxTable.M3.angleHigh / 10.0f).ToString();
                    myDevicePara.torque_low = (actXET.mxTable.M3.torqueLow / 100.0f).ToString();
                    myDevicePara.torque_up = (actXET.mxTable.M3.torqueHigh / 100.0f).ToString();
                    break;
                case 3:
                    mxscrew = actXET.mxTable.M4.screw;
                    myDevicePara.screw_total = actXET.mxTable.M4.screwTotal.ToString();
                    angleTarget = (actXET.mxTable.M4.angleTarget / 10.0f).ToString();
                    torqueTarget = (actXET.mxTable.M4.torqueTarget / 100.0f).ToString();
                    myDevicePara.angle_low = (actXET.mxTable.M4.angleLow / 10.0f).ToString();
                    myDevicePara.angle_up = (actXET.mxTable.M4.angleHigh / 10.0f).ToString();
                    myDevicePara.torque_low = (actXET.mxTable.M4.torqueLow / 100.0f).ToString();
                    myDevicePara.torque_up = (actXET.mxTable.M4.torqueHigh / 100.0f).ToString();
                    break;
                case 4:
                    mxscrew = actXET.mxTable.M5.screw;
                    myDevicePara.screw_total = actXET.mxTable.M5.screwTotal.ToString();
                    angleTarget = (actXET.mxTable.M5.angleTarget / 10.0f).ToString();
                    torqueTarget = (actXET.mxTable.M5.torqueTarget / 100.0f).ToString();
                    myDevicePara.angle_low = (actXET.mxTable.M5.angleLow / 10.0f).ToString();
                    myDevicePara.angle_up = (actXET.mxTable.M5.angleHigh / 10.0f).ToString();
                    myDevicePara.torque_low = (actXET.mxTable.M5.torqueLow / 100.0f).ToString();
                    myDevicePara.torque_up = (actXET.mxTable.M5.torqueHigh / 100.0f).ToString();
                    break;
                case 5:
                    mxscrew = actXET.mxTable.M6.screw;
                    myDevicePara.screw_total = actXET.mxTable.M6.screwTotal.ToString();
                    angleTarget = (actXET.mxTable.M6.angleTarget / 10.0f).ToString();
                    torqueTarget = (actXET.mxTable.M6.torqueTarget / 100.0f).ToString();
                    myDevicePara.angle_low = (actXET.mxTable.M6.angleLow / 10.0f).ToString();
                    myDevicePara.angle_up = (actXET.mxTable.M6.angleHigh / 10.0f).ToString();
                    myDevicePara.torque_low = (actXET.mxTable.M6.torqueLow / 100.0f).ToString();
                    myDevicePara.torque_up = (actXET.mxTable.M6.torqueHigh / 100.0f).ToString();
                    break;
                default:
                    break;
            }

            //事件委托
            MyDevice.myUpdate += new freshHandler(receiveData);

            //预设值更新
            switch (actXET.torqueUnit)
            {
                case UNIT.UNIT_nm: torqueUnit = "N·m"; break;
                case UNIT.UNIT_lbfin: torqueUnit = "lbf·in"; break;
                case UNIT.UNIT_lbfft: torqueUnit = "lbf·ft"; break;
                case UNIT.UNIT_kgcm: torqueUnit = "kgf·cm"; break;
                default: break;
            }

            //单位换算
            torqueTarget = (MyDevice.mSUT.Torque_nmTrans(Convert.ToUInt32(Convert.ToDouble(torqueTarget) * 100.0f + 0.5f), torqueUnit) / 100.0f).ToString();
            myDevicePara.torque_up = (MyDevice.mSUT.Torque_nmTrans(Convert.ToUInt32(Convert.ToDouble(myDevicePara.torque_up) * 100.0f + 0.5f), torqueUnit) / 100.0f).ToString();
            myDevicePara.torque_low = (MyDevice.mSUT.Torque_nmTrans(Convert.ToUInt32(Convert.ToDouble(myDevicePara.torque_low) * 100.0f + 0.5f), torqueUnit) / 100.0f).ToString();
            label2.Text = (float.Parse(torqueTarget)).ToString() + torqueUnit + "/" + (float.Parse(angleTarget)).ToString() + "°";

            //工位螺丝数量
            if (myDevicePara.screw_total.Equals("0"))
            {
                label3.Text = "0";
                mxscrew = 0;
            }
            else
            {
                label3.Text = mxscrew.ToString() + "/" + myDevicePara.screw_total.ToString();
            }

            //peak峰值
            if (actXET.modePt == 0)
            {
                label5.Text = (actXET.anglePeak / 10.0f).ToString() + "°";
                label7.Text = (actXET.torquePeak / 100.0f).ToString() + torqueUnit;
            }
            //track实时
            else if (actXET.modePt == 1)
            {
                label5.Text = (actXET.angle / 10.0f).ToString() + "°";
                label7.Text = (actXET.torque / 100.0f).ToString() + torqueUnit;
            }

            //触发定时器读心跳
            timer1.Interval = 100;
            timer1.Enabled = true;

            startTime = MyDevice.GetMilTimeStamp();
        }

        //退出
        private void ucBtnExt1_BtnClick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            MyDevice.myUpdate -= new freshHandler(receiveData);

            //存dataInfos到数据库
            saveDataToDb();

            this.DialogResult = DialogResult.OK;//这里的DialogResult是Form2类对象的属性

            this.Close();
        }

        //定时器
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (exePath.IndexOf("Koeorws.exe") != -1)
            {
                if (KoeorwsShow.activeForm != "实时数据") return;
            }
            else if (exePath.IndexOf("Torque.exe") != -1)
            {
                if (TorqueShow.activeForm != "实时数据") return;
            }

            //提示信息10秒, torqueErr屏蔽angleLevel信息
            if (infoErr != actXET.torqueErr)
            {
                infoTick++;
                if (infoTick > 100)
                {
                    infoTick = 0;
                    infoErr = actXET.torqueErr;
                    infoLevel = actXET.angleLevel;
                }
            }
            //提示信息5秒
            else if (infoLevel != actXET.angleLevel)
            {
                infoTick++;
                if (infoTick > 50)
                {
                    infoTick = 0;
                    infoLevel = actXET.angleLevel;
                }
            }
            else
            {
                infoTick = 0;
            }

            //通讯监控
            if (actXET.isActive)
            {
                nextTask = TASKS.READ_HEART;
                MyDevice.protocol.Protocol_Read_SendCOM(TASKS.READ_HEART);
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
                    //MessageBox.Show("MenuRunForm receiveData err 3");
                }
            }
            //本线程的操作请求
            else
            {
                if (actXET.isActive)
                {
                    if (exePath.IndexOf("Koeorws.exe") != -1)
                    {
                        //关闭弹窗
                        if (KoeorwsShow.activeForm != "实时数据" || KoeorwsShow.isQuitPopup)
                        {
                            KoeorwsShow.isQuitPopup = false;

                            MyDevice.myUpdate -= new freshHandler(receiveData);

                            //存dataInfos到数据库
                            saveDataToDb();

                            this.DialogResult = DialogResult.OK;//这里的DialogResult是Form2类对象的属性

                            this.Close();
                        }
                    }
                    else if (exePath.IndexOf("Torque.exe") != -1)
                    {
                        //关闭弹窗
                        if (TorqueShow.activeForm != "实时数据" || TorqueShow.isQuitPopup)
                        {
                            TorqueShow.isQuitPopup = false;

                            MyDevice.myUpdate -= new freshHandler(receiveData);

                            //存dataInfos到数据库
                            saveDataToDb();

                            this.DialogResult = DialogResult.OK;//这里的DialogResult是Form2类对象的属性

                            this.Close();
                        }
                    }

                    //更新表格
                    updateTableFromHeart();

                    switch (nextTask)
                    {
                        case TASKS.READ_HEART:
                            //按c
                            if (actXET.isKeyZero == true)
                            {
                                timer1.Enabled = false;

                                anglePeak = 0f;     //记录扭矩峰值
                                torquePeak = 0f;    //记录角度峰值

                                Thread.Sleep(50);
                                nextTask = TASKS.READ_PARA;
                                MyDevice.protocol.Protocol_Read_SendCOM(TASKS.READ_PARA);

                                //关闭弹窗
                                if (myDevicePara.screw_total.Equals("0") || res.Equals("ok"))
                                {
                                    MyDevice.myUpdate -= new freshHandler(receiveData);

                                    //存dataInfos到数据库
                                    saveDataToDb();

                                    this.DialogResult = DialogResult.OK;//这里的DialogResult是Form2类对象的属性

                                    this.Close();
                                }
                            }
                            break;

                        case TASKS.READ_PARA:
                            nextTask = TASKS.READ_HEART;
                            timer1.Enabled = true;
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        //更新mTable表格
        private void updateTableFromHeart()
        {
            for (int i = 0; i < actXET.REC.Count; i++)
            {
                switch (actXET.REC[i].torqueUnit)
                {
                    case UNIT.UNIT_nm: torqueUnit = "N·m"; break;
                    case UNIT.UNIT_lbfin: torqueUnit = "lbf·in"; break;
                    case UNIT.UNIT_lbfft: torqueUnit = "lbf·ft"; break;
                    case UNIT.UNIT_kgcm: torqueUnit = "kgf·cm"; break;
                    default: break;
                }

                //角度数值
                //peak峰值
                if (actXET.modePt == 0)
                {
                    label5.Text = (actXET.anglePeak / 10.0f).ToString() + "°";
                    if (actXET.torquePeak > (actXET.torqueCapacity * 1.2))
                    {
                        label7.Text = "   Er0";
                    }
                    else
                    {
                        label7.Text = (actXET.torquePeak / 100.0f).ToString() + torqueUnit;
                    }
                    torquePercent = 1.0 * (actXET.torquePeak / 100.0f) / Convert.ToDouble(torqueTarget);
                    anglePercent = 1.0 * (actXET.anglePeak / 10.0f) / Convert.ToDouble(angleTarget);
                }
                //track实时
                else if (actXET.modePt == 1)
                {
                    label5.Text = (actXET.REC[i].angle / 10.0f).ToString() + "°";
                    if (actXET.REC[i].torque > (actXET.torqueCapacity * 1.2))
                    {
                        label7.Text = "   Er0";
                    }
                    else
                    {
                        label7.Text = (actXET.REC[i].torque / 100.0f).ToString() + torqueUnit;
                    }

                    torquePercent = 1.0 * (actXET.REC[i].torque / 100.0f) / Convert.ToDouble(torqueTarget);
                    anglePercent = 1.0 * (actXET.REC[i].angle / 10.0f) / Convert.ToDouble(angleTarget);
                }

                //颜色变化
                //纯扭矩
                if (angleTarget.Equals("0"))
                {
                    if (torquePercent >= 1)
                    {
                        panel5.BackColor = Color.Red;
                    }
                    else if (torquePercent >= 0.90 && torquePercent < 1)
                    {
                        panel5.BackColor = (panel5.BackColor == Color.Gold) ? Color.White : Color.Gold;
                    }
                    else if (torquePercent >= 0.85 && torquePercent < 0.90)
                    {
                        panel5.BackColor = Color.Gold;
                    }
                    else
                    {
                        panel5.BackColor = Color.White;

                    }
                    panel6.BackColor = Color.White;
                }
                //纯角度
                else if (torqueTarget.Equals("0"))
                {
                    if (anglePercent >= 1)
                    {
                        panel6.BackColor = Color.Red;
                    }
                    else if (anglePercent >= 0.90 && anglePercent < 1)
                    {
                        panel6.BackColor = (panel6.BackColor == Color.Gold) ? Color.White : Color.Gold;
                    }
                    else if (anglePercent >= 0.85 && anglePercent < 0.90)
                    {
                        panel6.BackColor = Color.Gold;
                    }
                    else
                    {
                        panel6.BackColor = Color.White;
                    }
                    panel5.BackColor = Color.White;
                }
                //扭矩+角度
                else
                {
                    if (anglePercent >= 1)
                    {
                        panel5.BackColor = Color.Red;
                        panel6.BackColor = Color.Red;
                    }
                    else if (anglePercent >= 0.90 && anglePercent < 1)
                    {
                        panel5.BackColor = (panel5.BackColor == Color.Gold) ? Color.White : Color.Gold;
                        panel6.BackColor = (panel6.BackColor == Color.Gold) ? Color.White : Color.Gold;
                    }
                    else if (anglePercent >= 0.85 && anglePercent < 0.90)
                    {
                        panel5.BackColor = Color.Gold;
                        panel6.BackColor = Color.Gold;
                    }
                    else
                    {
                        panel5.BackColor = Color.White;
                        panel6.BackColor = Color.White;
                    }
                }

                //peak模式峰值
                if (actXET.modePt == 0)
                {
                    if ((actXET.REC[i].torquePeak / 100.0f) > torquePeak)
                    {
                        torquePeak = actXET.REC[i].torquePeak / 100.0f;
                    }
                    if ((actXET.REC[i].anglePeak / 10.0f) > anglePeak)
                    {
                        anglePeak = actXET.REC[i].anglePeak / 10.0f;
                    }
                }
                //track模式峰值
                else if (actXET.modePt == 1)
                {
                    if ((actXET.REC[i].torque / 100.0f) > torquePeak)
                    {
                        torquePeak = actXET.REC[i].torque / 100.0f;
                    }
                    if ((actXET.REC[i].angle / 10.0f) > anglePeak)
                    {
                        anglePeak = actXET.REC[i].angle / 10.0f;
                    }
                }

                //结果判断
                //纯扭矩
                if (angleTarget.Equals("0"))
                {
                    if (torquePeak > float.Parse(myDevicePara.torque_up) || label7.Text == "   Er0")
                    {
                        res = "NG";
                    }
                    else if (torquePeak >= float.Parse(myDevicePara.torque_low) && torquePeak <= float.Parse(myDevicePara.torque_up))
                    {
                        res = "ok";
                    }
                }
                //纯角度或扭矩+角度
                else
                {
                    if (anglePeak > float.Parse(myDevicePara.angle_up) || label7.Text == "   Er0")
                    {
                        res = "NG";
                    }
                    else if (anglePeak >= float.Parse(myDevicePara.angle_low) && anglePeak <= float.Parse(myDevicePara.angle_up))
                    {
                        res = "ok";
                    }
                }

                //表格数据
                DataInfo di = new DataInfo()
                {
                    work_id = myDevicePara.work_id,
                    custom_id = myDevicePara.custom_id,
                    torque_limit = (actXET.REC[i].torqueCapacity / 100.0f).ToString() + torqueUnit,
                    m_value = myDevicePara.m_value,
                    screw_num = mxscrew.ToString() + "/" + myDevicePara.screw_total,
                    torque_target = torqueTarget + torqueUnit,
                    angle_target = angleTarget + "°",
                    torque_low = myDevicePara.torque_low + torqueUnit,
                    torque_up = myDevicePara.torque_up + torqueUnit,
                    angle_low = myDevicePara.angle_low + "°",
                    angle_up = myDevicePara.angle_up + "°",
                    torque_actual = (actXET.REC[i].torque / 100.0f).ToString() + torqueUnit,
                    angle_actual = (actXET.REC[i].angle / 10.0f).ToString() + "°",
                    torque_over = torquePeak.ToString() + torqueUnit,
                    angle_over = anglePeak.ToString() + "°",
                    result = res,
                    time = startTime + "_" + MyDevice.GetMilTimeStamp(),
                };
                if (myDevicePara.screw_total.Equals("0"))
                {
                    di.screw_num = "0";
                }

                dataInfos.Add(di);
            }
            actXET.REC.Clear();
        }

        //存数据库
        private void saveDataToDb()
        {
            if (dataInfos.Count > 0)
            {
                string datatime = dataInfos[0].time.Split('_')[0];
                string dataInfoTableName = myDevicePara.work_id + "_" + MyDevice.GetMilTime(datatime).ToString("yyyy-MM-dd");     //数据表表名
                List<string> datasInfoName = new List<string>();        //数据统计表名称列
                bool isTableNameExist = false;                          //是否存在对应数据表

                datasInfoName = jdbc.GetListDatasName();
                jdbc.TableName = dataInfoTableName;
                if (datasInfoName == null)
                {
                    isTableNameExist = false;
                }
                else if (datasInfoName.Count > 0)
                {
                    if (datasInfoName.FindIndex(item => item == dataInfoTableName) < 0)
                    {
                        isTableNameExist = false;
                    }
                    else
                    {
                        isTableNameExist = true;
                    }
                }

                //查找该对应的数据表是否存在
                if (!isTableNameExist)
                {
                    //不存在
                    //先在数据统计表增加记录
                    DatasInfo di = new DatasInfo()
                    {
                        time = datatime,
                        name = dataInfoTableName
                    };
                    jdbc.AddDatasInfoByName(di);

                    //再建新表
                    jdbc.CreateDataTable();
                }

                //保存数据
                jdbc.AddDataInfoByID(dataInfos);
            }
        }
    }
}
