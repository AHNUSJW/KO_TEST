using BIL;
using Library;
using Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

//Ricardo 20230824
//Ricardo 20230830
//Ricardo 20230908
//Ziyun 20230911
//Ricardo 20230913

namespace Base.UI
{
    public partial class MenuCurveDataPanel : UserControl
    {
        private XET actXET;                   //需操作的设备
        private string torqueUnit;            //扭矩单位
        private byte oldMx;                   //更改M模式前的旧MX
        private byte mxscrew;                 //M1-M6模式下拧紧螺丝数量
        private byte mxscrewTotal;            //M1-M6模式下拧紧螺丝总数
        private Double torquePercent;         //实际扭矩与目标扭矩百分比
        private Double anglePercent;          //实际角度与目标角度百分比

        private int xAngle;         //角度横坐标 
        private int xToruqe;        //扭矩横坐标

        private Boolean isLoad;

        private volatile TASKS nextTask;      //下一个指令

        private Int32 lines = 1;                               //table表行数
        List<DataInfo> dataInfos = new List<DataInfo>();       //数据表
        List<DataInfo> dataInfosToDb = new List<DataInfo>();   //实际存储到数据库的数据表
        DevicePara myDevicePara = new DevicePara();            //符合M值的bohrcode的参数数据

        private double anglePeak = 0d;     //记录角度峰值
        private double torquePeak = 0d;    //记录扭矩峰值
        private double angleX = 0d;        //根据模式判定是实时角度/峰值角度
        private double torqueX = 0d;       //根据模式判定是实时扭矩/峰值扭矩

        private string res = "no";         //结果
        private string angleTarget = "";   //角度预设值
        private string torqueTarget = "";  //扭矩预设值

        private JDBC jdbc = new JDBC();    //数据库

        private int table_index = 0;       //读取表格下标
        private string startTime = "";     //本次加载的起始时间戳

        AutoFormSize autoSize = new AutoFormSize();

        public MenuCurveDataPanel()
        {
            InitializeComponent();
            autoSize.UIComponetForm(this);
        }

        //定时器
        private void timer1_Tick(object sender, EventArgs e)
        {
            updateDataInfosToDb();

            //通讯监控
            if (actXET.isActive)
            {
                switch(nextTask)
                {
                    case TASKS.READ_HEART:
                        MyDevice.protocol.Protocol_Read_SendCOM(TASKS.READ_HEART);
                        break;

                    case TASKS.READ_PARA:
                        //MyDevice.protocol.Protocol_Read_SendCOM(TASKS.READ_PARA);
                        break;

                    case TASKS.READ_M12DAT:
                        nextTask = TASKS.READ_M34DAT;
                        MyDevice.protocol.trTASK = TASKS.NULL;
                        MyDevice.protocol.Protocol_UpdateMx();
                        break;
                    default: 
                        break;
                }
            }
        }

        //更新实时数据
        private void updateDataFromHeart()
        {
            double angle;           //实时角度
            double torque;          //实时扭矩
            double limit;           //扳手量程

            //获取实时数据
            for (int i = 0; i < actXET.REC.Count; i++)
            {
                angle = actXET.REC[i].angle / 10.0f;
                torque = actXET.REC[i].torque / 100.0f;
                limit = actXET.REC[i].torqueCapacity / 100.0f;

                switch (actXET.REC[i].torqueUnit)
                {
                    case UNIT.UNIT_nm: torqueUnit = "N·m"; break;
                    case UNIT.UNIT_lbfin: torqueUnit = "lbf·in"; break;
                    case UNIT.UNIT_lbfft: torqueUnit = "lbf·ft"; break;
                    case UNIT.UNIT_kgcm: torqueUnit = "kgf·cm"; break;
                    default: break;
                }

                //表格数据
                DataInfo di = new DataInfo()
                {
                    id = lines,
                    work_id = myDevicePara.work_id,
                    custom_id = myDevicePara.custom_id,
                    torque_limit = limit.ToString(),
                    m_value = myDevicePara.m_value,
                    screw_num = (mxscrewTotal == 0) ? "0" : mxscrew.ToString() + "/" + mxscrewTotal.ToString(),
                    torque_target = torqueTarget.ToString(),
                    angle_target = angleTarget.ToString(),
                    torque_low = myDevicePara.torque_low.ToString(),
                    torque_up = myDevicePara.torque_up.ToString(),
                    angle_low = myDevicePara.angle_low.ToString(),
                    angle_up = myDevicePara.angle_up.ToString(),
                    torque_actual = torque.ToString(),
                    angle_actual = angle.ToString(),
                    torque_over = "",
                    angle_over = "",
                    result = "",
                    time = startTime + "_" + MyDevice.GetMilTimeStamp()
                };

                dataInfos.Add(di);

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

                    label5.Text = anglePeak.ToString() + "°";
                    label7.Text = torquePeak.ToString() + torqueUnit;

                    angleX = anglePeak;
                    torqueX = torquePeak;
                }
                //track模式峰值
                else if (actXET.modePt == 1)
                {
                    if (torque> torquePeak)
                    {
                        torquePeak = torque;
                    }
                    if (angle > anglePeak)
                    {
                        anglePeak = angle;
                    }

                    label5.Text = angle.ToString() + "°";
                    label7.Text = torque.ToString() + torqueUnit;

                    angleX = angle;
                    torqueX = torque;
                }

                //纯扭矩
                if (angleTarget.Equals("0"))
                {
                    if (torquePeak < float.Parse(myDevicePara.torque_low) || torquePeak > float.Parse(myDevicePara.torque_up) || label7.Text == "Er0")
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
                    if (anglePeak < float.Parse(myDevicePara.angle_low) || anglePeak > float.Parse(myDevicePara.angle_up) || label7.Text == "Er0")
                    {
                        res = "NG";
                    }
                    if (anglePeak >= float.Parse(myDevicePara.angle_low) && anglePeak <= float.Parse(myDevicePara.angle_up))
                    {
                        res = "ok";
                    }
                }
                Console.WriteLine(res);
                lines++;

                //根据模式进行报警和错误提示

                double torqueTargetMx = Convert.ToDouble(torqueTarget);  //扭矩预设值
                double angleTargetMx = Convert.ToDouble(angleTarget);    //角度预设值

                //纯扭矩模式
                if (angleTargetMx == 0 && torqueTargetMx != 0) 
                {
                    torquePercent =  torqueX / torqueTargetMx;
                    if (torquePercent >= 0.85 && torquePercent < 0.9)
                    {
                        panel5.BackColor = Color.Gold;
                    }
                    else if (torquePercent >= 0.9 && torquePercent < 1)
                    {
                        panel5.BackColor = (panel5.BackColor == Color.Gold) ? Color.White : Color.Gold;
                    }
                    else if (torquePercent >= 1)
                    {
                        panel5.BackColor = Color.Red;
                    }
                    else
                    {
                        panel5.BackColor = Color.White;
                    }
                }
                //纯角度模式
                else if (torqueTargetMx == 0 && angleTargetMx != 0)
                {
                    anglePercent = angleX / angleTargetMx;
                    if (anglePercent >= 0.85 && anglePercent < 0.9)
                    {
                        panel6.BackColor = Color.Gold;
                    }
                    else if (anglePercent >= 0.9 && anglePercent < 1)
                    {
                        panel6.BackColor = (panel6.BackColor == Color.Gold) ? Color.White : Color.Gold;
                    }
                    else if (anglePercent >= 1)
                    {
                        panel6.BackColor = Color.Red;
                    }
                    else
                    {
                        panel6.BackColor = Color.White;
                    }
                }
                //先达标扭矩预设值再判断角度
                else
                {
                    torquePercent = torqueX / torqueTargetMx;
                    anglePercent = angleX / angleTargetMx;

                    if (torquePercent >= 1)
                    {
                        if (anglePercent >= 0.85 && anglePercent < 0.9)
                        {
                            panel5.BackColor = Color.Gold;
                            panel6.BackColor = Color.Gold;
                        }
                        else if (anglePercent >= 0.9 && anglePercent < 1)
                        {
                            panel5.BackColor = (panel5.BackColor == Color.Gold) ? Color.White : Color.Gold;
                            panel6.BackColor = (panel6.BackColor == Color.Gold) ? Color.White : Color.Gold;
                        }
                        else if (anglePercent >= 1)
                        {
                            panel5.BackColor = Color.Red;
                            panel6.BackColor = Color.Red;
                        }
                        else
                        {
                            panel5.BackColor = Color.White;
                            panel6.BackColor = Color.White;
                        }
                    }
                }
                //超出量程1.2倍提示报错
                if (torqueX > limit * 1.2)
                {
                    label7.Text = "ER0";
                    panel5.BackColor = Color.Red;
                }

                //更新实时曲线
                chart1.Series[0].Points.AddXY(xToruqe, torque);
                chart2.Series[0].Points.AddXY(xAngle, angle);
                xToruqe++;
                xAngle++;
            }

            actXET.REC.Clear();
        }

        //更新数据库
        private void updateDataInfosToDb()
        {
            Int32 idx = 0;

            if (actXET.isActive == false)
            {
                return;
            }

            //加行
            while (table_index < dataInfos.Count)
            {
                if (actXET.modePt == 1)
                {
                    //行数
                    idx++;

                    //添加数据库数据
                    DataInfo diTodb = new DataInfo()
                    {
                        id = idx,
                        work_id = dataInfos[table_index].work_id,
                        custom_id = dataInfos[table_index].custom_id,
                        torque_limit = dataInfos[table_index].torque_limit.ToString() + torqueUnit,
                        m_value = dataInfos[table_index].m_value,
                        screw_num = dataInfos[table_index].screw_num,
                        torque_target = dataInfos[table_index].torque_target.ToString() + torqueUnit,
                        angle_target = dataInfos[table_index].angle_target.ToString() + "°",
                        torque_low = dataInfos[table_index].torque_low.ToString() + torqueUnit,
                        torque_up = dataInfos[table_index].torque_up.ToString() + torqueUnit,
                        angle_low = dataInfos[table_index].angle_low.ToString() + "°",
                        angle_up = dataInfos[table_index].angle_up.ToString() + "°",
                        torque_actual = dataInfos[table_index].torque_actual+ torqueUnit,
                        angle_actual = dataInfos[table_index].angle_actual + "°",
                        torque_over = torquePeak.ToString() + torqueUnit,
                        angle_over = anglePeak.ToString() + "°",
                        result = res,
                        time = dataInfos[table_index].time
                    };

                    dataInfosToDb.Add(diTodb);
                    table_index++;
                }
                else
                {
                    //行数
                    idx++;

                    //添加数据库数据
                    DataInfo diTodb = new DataInfo()
                    {
                        id = idx,
                        work_id = dataInfos[table_index].work_id,
                        custom_id = dataInfos[table_index].custom_id,
                        torque_limit = dataInfos[table_index].torque_limit.ToString() + torqueUnit,
                        m_value = dataInfos[table_index].m_value,
                        screw_num = dataInfos[table_index].screw_num,
                        torque_target = dataInfos[table_index].torque_target.ToString() + torqueUnit,
                        angle_target = dataInfos[table_index].angle_target.ToString() + "°",
                        torque_low = dataInfos[table_index].torque_low.ToString() + torqueUnit,
                        torque_up = dataInfos[table_index].torque_up.ToString() + torqueUnit,
                        angle_low = dataInfos[table_index].angle_low.ToString() + "°",
                        angle_up = dataInfos[table_index].angle_up.ToString() + "°",
                        torque_actual = torquePeak.ToString() + torqueUnit,
                        angle_actual = anglePeak.ToString() + "°",
                        torque_over = torquePeak.ToString() + torqueUnit,
                        angle_over = anglePeak.ToString() + "°",
                        result = res,
                        time = dataInfos[table_index].time
                    };
                    dataInfosToDb.Add(diTodb);
                    table_index++;
                }
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
                    updateDataFromHeart();

                    if(nextTask == TASKS.READ_M34DAT)
                    {
                        MyDevice.protocol.Protocol_UpdateMx();

                        if(MyDevice.protocol.trTASK == TASKS.NULL)
                        {
                            nextTask = TASKS.READ_HEART;

                            //切换M值更换显示预设值
                            mxUnit(actXET.modeMx);
                            torqueTarget = (MyDevice.mSUT.Torque_nmTrans(Convert.ToUInt32(Convert.ToDouble(torqueTarget) * 100.0f + 0.5f), torqueUnit) / 100.0f).ToString();
                            label2.Text = (float.Parse(torqueTarget)).ToString() + torqueUnit + "/" + (float.Parse(angleTarget)).ToString() + "°";
                            label2.Refresh();

                            if (mxscrewTotal == 0)
                            {
                                label3.Text = "0";
                            }
                            else
                            {
                                label3.Text = mxscrew.ToString() + "/" + mxscrewTotal.ToString();
                                label3.Refresh();
                            }
                        }
                        return;
                    }
                    
                    if (actXET.isKeyZero)
                    {
                        //清除曲线数据
                        this.chart1.Series[0].Points.Clear();
                        this.chart2.Series[0].Points.Clear();

                        //绘图初始化
                        xAngle = 0;
                        xToruqe = 0;
                        picture_Draw();

                        //归零清数据
                        anglePeak = 0;
                        torquePeak = 0;

                        if (res == "ok")
                        {
                            res = "false";

                            startTime = MyDevice.GetMilTimeStamp();

                            mxscrew++;

                            if (mxscrew > mxscrewTotal)
                            {
                                mxscrew = 1;
                            }
                        }
                        nextTask = TASKS.READ_PARA;
                    }

                    //预设值更改
                    if (actXET.isUpdate)
                    {
                        nextTask = TASKS.READ_M12DAT;
                        return;
                    }

                    //读完PARA开始读心跳
                    if (MyDevice.protocol.trTASK == TASKS.READ_PARA)
                    {
                        nextTask = TASKS.READ_HEART;
                    }

                    //M值更改，拧紧数量从1开始
                    if (actXET.modeMx != oldMx)
                    {
                        mxscrew = 1;
                    }

                    //更新拧紧数量
                    switch (actXET.modeMx)
                    {
                        default:
                            break;
                        case 0:
                            actXET.mxTable.M1.screw = mxscrew;
                            break;
                        case 1:
                            actXET.mxTable.M2.screw = mxscrew;
                            break;
                        case 2:
                            actXET.mxTable.M3.screw = mxscrew;
                            break;
                        case 3:
                            actXET.mxTable.M4.screw = mxscrew;
                            break;
                        case 4:
                            actXET.mxTable.M5.screw = mxscrew;
                            break;
                        case 5:
                            actXET.mxTable.M6.screw = mxscrew;
                            break;
                    }

                    //更新参数
                    mxUnit(actXET.modeMx);

                    //更新预设扭矩/角度
                    torqueTarget = (MyDevice.mSUT.Torque_nmTrans(Convert.ToUInt32(Convert.ToDouble(torqueTarget) * 100.0f + 0.5f), torqueUnit) / 100.0f).ToString();
                    myDevicePara.torque_up = (MyDevice.mSUT.Torque_nmTrans(Convert.ToUInt32(Convert.ToDouble(myDevicePara.torque_up) * 100.0f + 0.5f), torqueUnit) / 100.0f).ToString();
                    myDevicePara.torque_low = (MyDevice.mSUT.Torque_nmTrans(Convert.ToUInt32(Convert.ToDouble(myDevicePara.torque_low) * 100.0f + 0.5f), torqueUnit) / 100.0f).ToString();
                    label2.Text = (float.Parse(torqueTarget)).ToString() + torqueUnit + "/" + (float.Parse(angleTarget)).ToString() + "°";
                    label2.Refresh();

                    //显示拧紧数量
                    if (mxscrewTotal == 0)
                    {
                        label3.Text = "0";
                    }
                    else
                    {
                        label3.Text = mxscrew.ToString() + "/" + mxscrewTotal.ToString();
                        label3.Refresh();
                    }

                    //更改前的M值
                    oldMx = actXET.modeMx;

                    if(nextTask == TASKS.READ_PARA)
                    {
                        MyDevice.protocol.Protocol_Read_SendCOM(TASKS.READ_PARA);
                    }
                }
            }
        }

        //页面切换
        private void MenuCurveDataPanel_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                //初始化设备数据
                actXET = MyDevice.actDev;

                //每次加载记录操作开始时间
                startTime = MyDevice.GetMilTimeStamp();

                //获取参数信息
                myDevicePara = new DevicePara()
                {
                    bohr_code = actXET.bohrCode,
                    work_id = actXET.workId,
                    custom_id = actXET.customId,
                    screw_total = actXET.mxTable.M1.screwTotal.ToString(),
                    m_value = "M" + (actXET.modeMx + 1).ToString(),
                };

                //更新参数
                mxUnit(actXET.modeMx);

                //记录更新前的Mx模式
                oldMx = actXET.modeMx;

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
                
                if (mxscrewTotal == 0)
                {
                    label3.Text = "0";
                }
                else
                {
                    label3.Text = mxscrew.ToString() + "/" + mxscrewTotal.ToString();
                }

                panel5.BackColor = Color.White;
                panel6.BackColor = Color.White;


                //清除曲线数据
                this.chart1.Series[0].Points.Clear();
                this.chart2.Series[0].Points.Clear();

                anglePeak = 0d;
                torquePeak = 0d;
                xAngle = 0;
                xToruqe = 0;
                picture_Draw();

                //触发定时器读心跳
                isLoad = true;
                timer1.Interval = 100;
                timer1.Enabled = true;
                nextTask = TASKS.READ_HEART;
            }
            else
            {
                MyDevice.myUpdate -= new freshHandler(receiveData);
                timer1.Enabled = false;

                //清除曲线数据
                this.chart1.Series[0].Points.Clear();
                this.chart2.Series[0].Points.Clear();

                //保存数据list到数据库
                saveTodb(dataInfosToDb);
            }        
        }

        //保存到数据库
        private void saveTodb(List<DataInfo> dataInfosToDb)
        {
            //保存数据list到数据库
            if (dataInfosToDb.Count > 0)
            {
                string datatime = dataInfosToDb[0].time.Split('_')[0];
                string dataInfoTableName = actXET.workId + "_" + MyDevice.GetMilTime(datatime).ToString("yyyy-MM-dd");     //数据表表名
                jdbc.TableName = dataInfoTableName;
                List<string> datasInfoName = new List<string>();             //数据统计表名称列
                bool isTableNameExist = false;                               //是否存在对应数据表

                // datasInfo = jdbc.GetListDatas();
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
                jdbc.AddDataInfoByID(dataInfosToDb);
            }

            dataInfosToDb.Clear();
        }

        //M值初始化
        private void mxUnit(Byte modeMx)
        {
            switch (modeMx)
            {
                case 0:
                    mxscrew = actXET.mxTable.M1.screw;
                    mxscrewTotal = actXET.mxTable.M1.screwTotal;
                    angleTarget = (actXET.mxTable.M1.angleTarget / 10.0f).ToString();
                    torqueTarget = (actXET.mxTable.M1.torqueTarget / 100.0f).ToString();
                    myDevicePara.angle_low = (actXET.mxTable.M1.angleLow / 10.0f).ToString();
                    myDevicePara.angle_up = (actXET.mxTable.M1.angleHigh / 10.0f).ToString();
                    myDevicePara.torque_low = (actXET.mxTable.M1.torqueLow / 100.0f).ToString();
                    myDevicePara.torque_up = (actXET.mxTable.M1.torqueHigh / 100.0f).ToString();
                    break;
                case 1:
                    mxscrew = actXET.mxTable.M2.screw;
                    mxscrewTotal = actXET.mxTable.M2.screwTotal;
                    angleTarget = (actXET.mxTable.M2.angleTarget / 10.0f).ToString();
                    torqueTarget = (actXET.mxTable.M2.torqueTarget / 100.0f).ToString();
                    myDevicePara.angle_low = (actXET.mxTable.M2.angleLow / 10.0f).ToString();
                    myDevicePara.angle_up = (actXET.mxTable.M2.angleHigh / 10.0f).ToString();
                    myDevicePara.torque_low = (actXET.mxTable.M2.torqueLow / 100.0f).ToString();
                    myDevicePara.torque_up = (actXET.mxTable.M2.torqueHigh / 100.0f).ToString();
                    break;
                case 2:
                    mxscrew = actXET.mxTable.M3.screw;
                    mxscrewTotal = actXET.mxTable.M3.screwTotal;
                    angleTarget = (actXET.mxTable.M3.angleTarget / 10.0f).ToString();
                    torqueTarget = (actXET.mxTable.M3.torqueTarget / 100.0f).ToString();
                    myDevicePara.angle_low = (actXET.mxTable.M3.angleLow / 10.0f).ToString();
                    myDevicePara.angle_up = (actXET.mxTable.M3.angleHigh / 10.0f).ToString();
                    myDevicePara.torque_low = (actXET.mxTable.M3.torqueLow / 100.0f).ToString();
                    myDevicePara.torque_up = (actXET.mxTable.M3.torqueHigh / 100.0f).ToString();
                    break;
                case 3:
                    mxscrew = actXET.mxTable.M4.screw;
                    mxscrewTotal = actXET.mxTable.M4.screwTotal;
                    angleTarget = (actXET.mxTable.M4.angleTarget / 10.0f).ToString();
                    torqueTarget = (actXET.mxTable.M4.torqueTarget / 100.0f).ToString();
                    myDevicePara.angle_low = (actXET.mxTable.M4.angleLow / 10.0f).ToString();
                    myDevicePara.angle_up = (actXET.mxTable.M4.angleHigh / 10.0f).ToString();
                    myDevicePara.torque_low = (actXET.mxTable.M4.torqueLow / 100.0f).ToString();
                    myDevicePara.torque_up = (actXET.mxTable.M4.torqueHigh / 100.0f).ToString();
                    break;
                case 4:
                    mxscrew = actXET.mxTable.M5.screw;
                    mxscrewTotal = actXET.mxTable.M5.screwTotal;
                    angleTarget = (actXET.mxTable.M5.angleTarget / 10.0f).ToString();
                    torqueTarget = (actXET.mxTable.M5.torqueTarget / 100.0f).ToString();
                    myDevicePara.angle_low = (actXET.mxTable.M5.angleLow / 10.0f).ToString();
                    myDevicePara.angle_up = (actXET.mxTable.M5.angleHigh / 10.0f).ToString();
                    myDevicePara.torque_low = (actXET.mxTable.M5.torqueLow / 100.0f).ToString();
                    myDevicePara.torque_up = (actXET.mxTable.M5.torqueHigh / 100.0f).ToString();
                    break;
                case 5:
                    mxscrew = actXET.mxTable.M6.screw;
                    mxscrewTotal = actXET.mxTable.M6.screwTotal;
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
        }

        //绘图初始化
        private void picture_Draw()
        {
            // chart属性
            Series series1 = chart1.Series[0];
            series1.ChartType = SeriesChartType.FastLine;
            series1.BorderWidth = 3;
            series1.Color = System.Drawing.Color.Tomato;  //曲线颜色
            series1.LegendText = MyDevice.languageType == 0 ? "扭力值" : "Torque";

            // chart属性
            Series series2 = chart2.Series[0];
            series2.ChartType = SeriesChartType.FastLine;
            series2.BorderWidth = 3;
            series2.Color = series2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));   //曲线颜色
            series2.LegendText = MyDevice.languageType == 0 ? "角度值" : "Angle";

            // 设置坐标轴和背景
            chart1.BackColor = System.Drawing.Color.Honeydew;
            ChartArea chartAreaTorque = chart2.ChartAreas[0];
            chartAreaTorque.BackColor = System.Drawing.Color.Ivory;
            chartAreaTorque.AxisX.Minimum = 0;
            chartAreaTorque.AxisX.Maximum = System.Double.NaN;
            chartAreaTorque.AxisY.Minimum = 0d;
            chartAreaTorque.AxisY.Maximum = System.Double.NaN;
            chartAreaTorque.AxisX.MajorGrid.Enabled = false;
            chartAreaTorque.AxisY.MajorGrid.Enabled = false;
            chartAreaTorque.AxisX.LabelStyle.Enabled = false;
            chartAreaTorque.AxisY.LineColor = System.Drawing.Color.Gray;
            chartAreaTorque.AxisX.MajorGrid.LineColor = System.Drawing.Color.Gray;
            chartAreaTorque.AxisY.MajorGrid.LineColor = System.Drawing.Color.Gray;
            chartAreaTorque.AxisX.MajorTickMark.Size = 0;

            chart2.BackColor = System.Drawing.Color.Ivory;
            ChartArea chartAreaAngle = chart1.ChartAreas[0];
            chartAreaAngle.BackColor = System.Drawing.Color.Honeydew;
            chartAreaAngle.AxisX.Minimum = 0;
            chartAreaAngle.AxisX.Maximum = System.Double.NaN;
            chartAreaAngle.AxisY.Minimum = 0d;
            chartAreaAngle.AxisY.Maximum = System.Double.NaN;
            chartAreaAngle.AxisX.MajorGrid.Enabled = false;
            chartAreaAngle.AxisY.MajorGrid.Enabled = false;
            chartAreaAngle.AxisX.LabelStyle.Enabled = false;
            chartAreaAngle.AxisY.LineColor = System.Drawing.Color.Gray;
            chartAreaAngle.AxisX.MajorGrid.LineColor = System.Drawing.Color.Gray;
            chartAreaAngle.AxisY.MajorGrid.LineColor = System.Drawing.Color.Gray;
            chartAreaAngle.AxisX.MajorTickMark.Size = 0;
        }


        private void MenuCurveDataPanel_Resize(object sender, EventArgs e)
        {
            autoSize.UIComponetForm_Resize(this);
            splitContainer1.SplitterDistance = this.Width / 5 * 3;
        }
    }
}
