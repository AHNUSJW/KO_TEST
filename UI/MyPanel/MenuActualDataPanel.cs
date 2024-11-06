using BIL;
using Base.UI.MyPanel;
using Library;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

//Lumi 20230914
//Ricardo 20231009

namespace Base.UI
{
    public partial class MenuActualDataPanel : UserControl
    {
        private XET actXET;                //需操作的设备
        private TASKS nextTask;            //按键指令

        private int table_index = 0;       //读取表格下标
        private Int32 recCount = 0;        //缓存数量
        private Boolean askEPT = false;    //是否可以读出缓存
        List<string> delIndexList = new List<string>();  //存储被删除项的序号

        List<DataInfo> dataInfosUI = new List<DataInfo>();                 //界面上的数据表
        List<DataInfo> dataInfosFromOperation = new List<DataInfo>();      //来自弹窗的数据
        List<DataInfo> dataInfoCurve = new List<DataInfo>();               //画曲线的数据

        private bool isOperating = false;   //是否正在操作扳手
        private System.Timers.Timer menuActiveTime = new System.Timers.Timer();
        private System.Timers.Timer timer1 = new System.Timers.Timer();
        public Boolean menuActive = false;

        JDBC jdbc = new JDBC();
        AutoFormSize autoSize = new AutoFormSize();

        private byte oldMx;                 //更改M模式前的旧MX
        private int oldDGVCount;            //更改前的表格行数

        private string exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

        //页面初始化
        public MenuActualDataPanel()
        {
            InitializeComponent();
            autoSize.UIComponetForm(this);

            //设置窗体的双缓冲
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            //利用反射设置DataGridView的双缓冲
            Type myType = this.dataGridView1.GetType();
            PropertyInfo pi = myType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(this.dataGridView1, true, null);
        }

        //页面加载
        private void MenuActualDataPanel_Load(object sender, EventArgs e)
        {
            menuActiveTime.Elapsed += new System.Timers.ElapsedEventHandler(OnTmrTrg);
            menuActiveTime.Interval = 100;
            menuActiveTime.AutoReset = false; //true-一直循环 ，false-循环一次

            timer1.Elapsed += new System.Timers.ElapsedEventHandler(timer1_Tick);
            timer1.Interval = 100;
            timer1.AutoReset = true; //true-一直循环 ，false-循环一次
        }

        //切换页面
        private void MenuActualDataPanel_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                //初始化设备数据
                actXET = MyDevice.actDev;
                nextTask = TASKS.NULL;
                isOperating = false;
                oldMx = actXET.modeMx;

                dataInfosUI.Clear();
                delIndexList.Clear();
                table_index = 0;

                //自定义控件文本单独设置
                if (MyDevice.languageType == 1)
                {
                    ucBtnExt1.BtnText = "Clear";
                    this.Column17.Text = "curve";
                    this.Column18.Text = "delete";
                }

                //数据库
                dataGridView1.Rows.Clear();
                //数据库操作
                List<DevicePara> para = new List<DevicePara>();           //存参数的数据库表
                para = jdbc.QueryDeviceParaBybohrCode(actXET.bohrCode);   //得到数据库存储的数据
                string myWorkID = para[para.Count - 1].work_id;
                string myCustomId = para[para.Count - 1].custom_id;
                List<string> datasInfoNameFromDb = new List<string>();    //数据统计表名称列
                datasInfoNameFromDb = jdbc.GetListDatasName();

                //显示数据库已有数据
                if (datasInfoNameFromDb != null)
                {
                    List<string> myTableNameList = datasInfoNameFromDb.Where(item => item.StartsWith(myWorkID)).ToList();   //所有与当前连接设备的bohrcode和work_id相同的数据库表表名
                    List<DataInfo> dataInfosLoad = new List<DataInfo>();      //加载时从数据库拿到的数据
                    foreach (string tb in myTableNameList)
                    {
                        jdbc.TableName = tb;
                        dataInfosLoad = jdbc.GetListDataByWorkIDAndCustomId(myWorkID, myCustomId);

                        if (dataInfosLoad == null) continue;

                        var filteredItems = dataInfosLoad.GroupBy(d => d.time.Split('_')[0])  // 按起始时间戳分组
                                                         .Where(g => !g.All(d => RemoveUnit(d.angle_over) == "0" && RemoveUnit(d.torque_over) == "0"))  // 去除所有 angle_over 和 torque_over 都等于 0 的分组
                                                         .SelectMany(g => g)  // 展开分组
                                                         .ToList();

                        filteredItems.RemoveAll(d => (d.result == "false"));

                        //找到该数据表里需要填入到datagridview的所有项
                        filteredItems = filteredItems.Where(item => item.time.Contains("_"))
                                                     .GroupBy(item => item.time.Split('_')[0])
                                                     .Select(group => group.OrderByDescending(item => item.time.Split('_')[1]).First())
                                                     .ToList();

                        //填入到datagridview
                        foreach (var item in filteredItems)
                        {
                            dataInfosUI.Add(item);
                            table_index++;

                            var picRes = Properties.Resources.red;
                            if (item.result.Equals("ok"))
                            {
                                picRes = Properties.Resources.green;
                            }
                            else if (item.result.Equals(""))
                            {
                                picRes = Properties.Resources.white;
                            }

                            dataGridView1.Rows.Add(table_index, item.work_id, item.custom_id, item.m_value, item.screw_num,
                                                   item.torque_target, item.angle_target, item.torque_low, item.torque_up, item.angle_low, item.angle_up,
                                                   item.torque_over, item.angle_over, picRes, ConvertTimestampToStr(item.time.Split('_')[0]));
                        }
                    }
                }

                //表格初始化
                dataGridView1.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Font = new System.Drawing.Font("Arial", 11);
                //
                dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(161)))), ((int)(((byte)(103)))));
                dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dataGridView1.EnableHeadersVisualStyles = false;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AllowUserToDeleteRows = false;
                dataGridView1.AllowUserToOrderColumns = false;
                dataGridView1.AllowUserToResizeColumns = false;
                dataGridView1.AllowUserToResizeRows = false;
                dataGridView1.RowHeadersVisible = false;

                //事件委托
                MyDevice.myUpdate += new freshHandler(receiveData);

                //读心跳发生器
                timer1.Enabled = true;

                //可以读取缓存（只有一次）
                askEPT = true;

                //显示到最后一行数据
                if (dataGridView1.RowCount > 2)
                {
                    dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
                }
            }
            else
            {
                MyDevice.myUpdate -= new freshHandler(receiveData);

                timer1.Enabled = false;

                //存储缓存数据
                saveRecDataToDb();
            }
        }

        //重绘结果列、曲线和删除按键单元格
        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0 && e.ColumnIndex == 13)
            {
                // 取消默认绘制
                e.Handled = true;

                // 按钮单元格样式
                Color borderColor = SystemColors.ControlDark;   // 设置边框颜色

                // 绘制按钮背景
                e.Graphics.FillRectangle(new SolidBrush(Color.White), e.CellBounds);

                // 绘制按钮边框
                ControlPaint.DrawBorder(e.Graphics, e.CellBounds, borderColor, 0, ButtonBorderStyle.Solid, borderColor, 0, ButtonBorderStyle.Solid, borderColor, 1, ButtonBorderStyle.Solid, borderColor, 1, ButtonBorderStyle.Solid);

                // 绘制按钮文本
                e.PaintContent(e.CellBounds);
            }
            else if (e.ColumnIndex >= 0 && e.RowIndex >= 0 && (e.ColumnIndex == 15 || e.ColumnIndex == 16))
            {
                // 取消默认绘制
                e.Handled = true;

                // 绘制单元格的背景和边框，但不包括内容前景
                e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~(DataGridViewPaintParts.ContentForeground));

                // 绘制单元格的内容区域
                var r = e.CellBounds;
                r.Inflate(-2, -2);

                // 单元格的背景色
                e.Graphics.FillRectangle(Brushes.White, r);

                // 绘制单元格的内容前景
                e.Paint(e.CellBounds, DataGridViewPaintParts.ContentForeground);
            }
        }

        //曲线、删除
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // 曲线
            if (e.ColumnIndex == dataGridView1.Columns["Column17"].Index && e.RowIndex >= 0 && dataGridView1.Rows[e.RowIndex].Cells[5].Value != "")
            {
                //获取该段数据
                string startTime = dataInfosUI[e.RowIndex].time.Split('_')[0];    //数据段的起始时间
                string workID = dataInfosUI[e.RowIndex].work_id;
                jdbc.TableName = workID + "_" + MyDevice.GetMilTime(startTime).ToString("yyyy-MM-dd");
                dataInfoCurve = jdbc.GetListDataPeriod(startTime);

                MenuActualDataCurve menuActualDataCurve = new MenuActualDataCurve(dataInfoCurve);
                menuActualDataCurve.ShowDialog();
                menuActualDataCurve.StartPosition = FormStartPosition.CenterScreen;
            }

            // 删除对应行的数据
            if (e.ColumnIndex == dataGridView1.Columns["Column18"].Index && e.RowIndex >= 0)
            {
                string dataID = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                DialogResult result = MyDevice.languageType == 0 ? 
                    MessageBox.Show($"是否删除数据{dataID}?", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question) : 
                    MessageBoxEX.Show($"Whether to delete data{dataID}?", "Confirm the deletion", MessageBoxButtons.YesNo, new string[] { "Yes", "NO" });

                if (result == DialogResult.No)
                {
                    return;
                }

                //数据库操作
                string startTime = dataInfosUI[e.RowIndex].time.Split('_')[0];    //要删除的数据段的起始时间
                string workID = dataInfosUI[e.RowIndex].work_id;
                jdbc.TableName = workID + "_" + MyDevice.GetMilTime(startTime).ToString("yyyy-MM-dd");
                jdbc.DelDataInfoByStartTime(startTime);

                delIndexList.Add(dataID);

                dataInfosUI.RemoveAt(e.RowIndex);
                dataGridView1.Rows.RemoveAt(e.RowIndex);    //表格移除指定的行
                if (MyDevice.languageType == 0)
                {
                    MessageBox.Show("数据" + dataID + "删除成功", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBoxEX.Show("Data" + dataID + "was successfully deleted", "Hint", MessageBoxButtons.OK, new string[] { "OK" });
                }
                //必须数据库先删除后再刷新Ui界面的数据，因为CurrentCell会因为UI界面刷新删除数据后发生单元格的变动，
                //跳到下一行数据，从而导致数据库删除失败。
            }
        }

        //清除全部数据
        private void ucBtnExt1_BtnClick(object sender, EventArgs e)
        {
            DialogResult result = MyDevice.languageType == 0 ? 
                MessageBox.Show("确认清除全部数据？", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) :
                MessageBoxEX.Show("Confirm that all data is cleared？", "Hint", MessageBoxButtons.YesNo, new string[] { "Yes", "NO" });

            if (result == DialogResult.No) return;

            //数据库操作
            List<DevicePara> para = new List<DevicePara>();           //存参数的数据库表
            para = jdbc.QueryDeviceParaBybohrCode(actXET.bohrCode);   //得到数据库存储的数据
            string myWorkID = para[para.Count - 1].work_id;
            string myCustomId = para[para.Count - 1].custom_id;
            List<string> datasInfoNameFromDb = new List<string>();    //数据统计表名称列
            datasInfoNameFromDb = jdbc.GetListDatasName();
            List<string> myTableNameList = datasInfoNameFromDb.Where(item => item.StartsWith(myWorkID)).ToList();   //所有与当前连接设备的bohrcode和work_id相同的数据库表表名

            foreach (string tb in myTableNameList)
            {
                jdbc.TableName = tb;
                jdbc.DelDataInfoByWorkIDAndCustomId(myWorkID, myCustomId);
            }

            dataInfosUI.Clear();
            dataGridView1.Rows.Clear();
            delIndexList.Clear();
            table_index = 0;


            if (MyDevice.languageType == 0)
            {
                MessageBox.Show("清除数据成功", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBoxEX.Show("Data cleared successfully", "Hint", MessageBoxButtons.OK, new string[] { "OK" });
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
                }
            }
            //本线程的操作请求
            else
            {
                if (actXET.isActive)
                {
                    switch (nextTask)
                    {

                        case TASKS.READ_HEART:
                            if (actXET.isKeyZero == true)
                            {
                                timer1.Enabled = false;

                                Thread.Sleep(50);
                                nextTask = TASKS.READ_PARA;
                                MyDevice.protocol.Protocol_Read_SendCOM(TASKS.READ_PARA);
                                break;
                            }

                            if (actXET.isUpdate == true)
                            {
                                timer1.Enabled = false;

                                nextTask = TASKS.READ_M12DAT;
                                MyDevice.protocol.Protocol_Read_SendCOM(TASKS.READ_M12DAT);
                                break;
                            }

                            if ((actXET.isEmpty == false)  && askEPT)
                            {
                                if (DialogResult.Yes ==
                                    (MyDevice.languageType == 0 ?
                                    MessageBox.Show("扭矩扳手设备有缓存数据, [确定]读出设备缓存", "读出设备缓存?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) :
                                    MessageBoxEX.Show("Torque wrench devices have cache data, [OK] reads out the device cache?", "reads out the device cache?", MessageBoxButtons.YesNo, new string[] { "Yes", "NO" }))
                                    )
                                {
                                    //弹窗关闭之后开始读size
                                    timer1.Enabled = false;
                                    nextTask = TASKS.WRITE_RECSIZE;
                                    MyDevice.protocol.Protocol_Read_SendCOM(TASKS.WRITE_RECSIZE);
                                }
                            }

                            askEPT = false;//不再读取缓存（保证读取缓存弹窗只弹一次）

                            isOperating = false;

                            //获取实时数据
                            for (int i = 0; i < actXET.REC.Count; i++)
                            {
                                if ((actXET.REC[i].torque / 100.0f) > 0)
                                {
                                    actXET.REC.Clear();
                                    isOperating = true;
                                    break;
                                }
                                actXET.REC.Clear();
                            }

                            if (isOperating && !menuActive)
                            {
                                timer1.Stop();
                                menuActive = true;
                                menuActiveTime.Enabled = true;
                            }

                            //更新dataGridView1表格
                            dataGridView_update();
                            break;

                        case TASKS.READ_PARA:
                            nextTask = TASKS.READ_HEART;
                            timer1.Enabled = true;
                            break;

                        case TASKS.READ_M12DAT:
                            nextTask = TASKS.READ_M34DAT;
                            MyDevice.protocol.Protocol_Read_SendCOM(TASKS.READ_M34DAT);
                            break;

                        case TASKS.READ_M34DAT:
                            nextTask = TASKS.READ_M56DAT;
                            MyDevice.protocol.Protocol_Read_SendCOM(TASKS.READ_M56DAT);
                            break;

                        case TASKS.READ_M56DAT:
                            nextTask = TASKS.READ_HEART;
                            timer1.Enabled = true;
                            break;

                        case TASKS.WRITE_RECSIZE:
                            //开始读
                            if (actXET.queueSize > 0)
                            {
                                nextTask = TASKS.WRITE_RECDAT;
                                MyDevice.protocol.Protocol_Write_SendCOM(TASKS.WRITE_RECDAT);
                            }
                            break;

                        case TASKS.WRITE_RECDAT:
                            //检测是否读完
                            if (actXET.queueArray[0].index == 0)
                            {
                                //检测完整性
                                for (UInt16 i = 1; i < actXET.queueSize; i++)
                                {
                                    if (actXET.queueArray[i].index == 0)
                                    {
                                        MyDevice.protocol.Protocol_Write_SendCOM(TASKS.WRITE_RECDAT, i);
                                        return;
                                    }
                                }

                                //更新缓存表格
                                updateTableFromRecord();

                                //完成
                                MyDevice.protocol.Protocol_Write_SendCOM(TASKS.WRITE_RECSIZE);

                                //读完设备缓存之后读心跳
                                nextTask = TASKS.READ_HEART;
                                timer1.Enabled = true;
                            }
                            break;

                        default:
                            break;
                    }

                    if (!oldMx.Equals(actXET.modeMx))
                    {
                        oldMx = actXET.modeMx;
                        switch (actXET.modeMx)
                        {
                            default:
                                break;
                            case 0:
                                actXET.mxTable.M1.screw = 1;
                                break;
                            case 1:
                                actXET.mxTable.M2.screw = 1;
                                break;
                            case 2:
                                actXET.mxTable.M3.screw = 1;
                                break;
                            case 3:
                                actXET.mxTable.M4.screw = 1;
                                break;
                            case 4:
                                actXET.mxTable.M5.screw = 1;
                                break;
                            case 5:
                                actXET.mxTable.M6.screw = 1;
                                break;
                        }
                    }
                }
            }
        }

        //更新缓存数据
        private void updateTableFromRecord()
        {
            int index;//当前显示的表格行数
            string now = MyDevice.GetMilTimeStamp();//当前时间的时间戳
            oldDGVCount = dataGridView1.RowCount;//未更新前的表格行数
            int j = 0;

            for (UInt16 i = actXET.queueSize; i > 0;)
            {
                //
                i--;

                //缓存数量
                recCount++;

                DataInfo di = new DataInfo()
                {
                    work_id = actXET.workId,
                    custom_id = actXET.customId,
                    m_value = "",
                    torque_limit = "",
                    screw_num = "",
                    torque_target = "",
                    angle_target = "",
                    torque_low = "",
                    torque_up = "",
                    angle_low = "",
                    angle_up = "",
                    torque_actual = (actXET.queueArray[i].torque / 100.0f).ToString() + " N·m",
                    angle_actual = (actXET.queueArray[i].angle / 10.0f).ToString() + " °",
                    torque_over = (actXET.queueArray[i].torque / 100.0f).ToString() + " N·m",
                    angle_over = (actXET.queueArray[i].angle / 10.0f).ToString() + " °",
                    result = "",//缓存结果为空
                    time = (now.Substring(0, now.Length - recCount.ToString().Length) + recCount) + "_" + (now.Substring(0, now.Length - recCount.ToString().Length) + recCount) //+recCount  和 +100 是为了便于区分每一条缓存
                };

                dataInfosUI.Add(di);

            }

            //加行
            while (table_index < dataInfosUI.Count)
            {
                //行数
                index = dataGridView1.Rows.Add();
                //
                j++;

                table_index++;
                dataGridView1.Rows[index].Cells[0].Value = table_index;                       //序号
                dataGridView1.Rows[index].Cells[1].Value = dataInfosUI[oldDGVCount + j - 1].work_id;        //工位号
                dataGridView1.Rows[index].Cells[2].Value = dataInfosUI[oldDGVCount + j - 1].custom_id;      //产品编码
                dataGridView1.Rows[index].Cells[3].Value = dataInfosUI[oldDGVCount + j - 1].m_value;        //m值
                dataGridView1.Rows[index].Cells[4].Value = dataInfosUI[oldDGVCount + j - 1].screw_num;      //拧紧螺丝数量
                dataGridView1.Rows[index].Cells[5].Value = dataInfosUI[oldDGVCount + j - 1].torque_target;  //扭力值设定
                dataGridView1.Rows[index].Cells[6].Value = dataInfosUI[oldDGVCount + j - 1].angle_target;   //角度设定
                dataGridView1.Rows[index].Cells[7].Value = dataInfosUI[oldDGVCount + j - 1].torque_low;     //扭力值下限
                dataGridView1.Rows[index].Cells[8].Value = dataInfosUI[oldDGVCount + j - 1].torque_up;      //扭力值上限
                dataGridView1.Rows[index].Cells[9].Value = dataInfosUI[oldDGVCount + j - 1].angle_low;      //角度值下限
                dataGridView1.Rows[index].Cells[10].Value = dataInfosUI[oldDGVCount + j - 1].angle_up;      //角度值上限
                dataGridView1.Rows[index].Cells[11].Value = dataInfosUI[oldDGVCount + j - 1].torque_over;   //最终扭力值
                dataGridView1.Rows[index].Cells[12].Value = dataInfosUI[oldDGVCount + j - 1].angle_over;    //最终角度值
                dataGridView1.Rows[index].Cells[13].Value = Properties.Resources.white;       //结果
                dataGridView1.Rows[index].Cells[14].Value = ConvertTimestampToStr(dataInfosUI[oldDGVCount + j - 1].time.Split('_')[0]);      //时间

                //显示到最后一行数据
                if (dataGridView1.RowCount > 2)
                {
                    dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
                }
            }
        }

        //更新dataGridView1表格
        private void dataGridView_update()
        {
            Int32 idx;

            if (actXET.isActive == false)
            {
                return;
            }

            //加行
            while (table_index < (dataInfosUI.Count + delIndexList.Count))
            {
                //行数
                idx = dataGridView1.Rows.Add();
                //数据
                var picRes = Properties.Resources.red;
                if (dataInfosUI[dataInfosUI.Count - 1].result.Equals("ok"))
                {
                    picRes = Properties.Resources.green;
                }

                table_index++;
                dataGridView1.Rows[idx].Cells[0].Value = table_index;    //序号
                dataGridView1.Rows[idx].Cells[1].Value = dataInfosUI[dataInfosUI.Count - 1].work_id;        //工位号
                dataGridView1.Rows[idx].Cells[2].Value = dataInfosUI[dataInfosUI.Count - 1].custom_id;      //产品编码
                dataGridView1.Rows[idx].Cells[3].Value = dataInfosUI[dataInfosUI.Count - 1].m_value;        //m值
                dataGridView1.Rows[idx].Cells[4].Value = dataInfosUI[dataInfosUI.Count - 1].screw_num;      //拧紧螺丝数量
                dataGridView1.Rows[idx].Cells[5].Value = dataInfosUI[dataInfosUI.Count - 1].torque_target;  //扭力值设定
                dataGridView1.Rows[idx].Cells[6].Value = dataInfosUI[dataInfosUI.Count - 1].angle_target;   //角度设定
                dataGridView1.Rows[idx].Cells[7].Value = dataInfosUI[dataInfosUI.Count - 1].torque_low;     //扭力值下限
                dataGridView1.Rows[idx].Cells[8].Value = dataInfosUI[dataInfosUI.Count - 1].torque_up;      //扭力值上限
                dataGridView1.Rows[idx].Cells[9].Value = dataInfosUI[dataInfosUI.Count - 1].angle_low;      //角度值下限
                dataGridView1.Rows[idx].Cells[10].Value = dataInfosUI[dataInfosUI.Count - 1].angle_up;      //角度值上限
                dataGridView1.Rows[idx].Cells[11].Value = dataInfosUI[dataInfosUI.Count - 1].torque_over;   //最终扭力值
                dataGridView1.Rows[idx].Cells[12].Value = dataInfosUI[dataInfosUI.Count - 1].angle_over;    //最终角度值
                dataGridView1.Rows[idx].Cells[13].Value = picRes;    //结果
                dataGridView1.Rows[idx].Cells[14].Value = ConvertTimestampToStr(dataInfosUI[dataInfosUI.Count - 1].time.Split('_')[1]);          //时间

                //显示到最后一行数据
                if (dataGridView1.RowCount > 2)
                {
                    dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
                }
            }
        }

        //存缓存数据数据库
        private void saveRecDataToDb()
        {
            if (dataGridView1.RowCount > oldDGVCount)
            {
                string datatime = dataInfosUI[0].time.Split('_')[0];
                string dataInfoTableName = actXET.workId + "_" + MyDevice.GetMilTime(datatime).ToString("yyyy-MM-dd");     //数据表表名
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
                jdbc.AddDataInfoByID(dataInfosUI);
            }

            //保存之后更新表格旧值
            oldDGVCount = dataGridView1.RowCount;
        }

        //触发弹窗的定时器
        private void OnTmrTrg(object sender, System.Timers.ElapsedEventArgs e)
        {
            //根据不同exe环境来运行不同功能
            if (exePath.IndexOf("Koeorws.exe") != -1)
            {
                if (KoeorwsShow.activeForm != "实时数据") return;
            }
            else if (exePath.IndexOf("Torque.exe") != -1)
            {
                if (TorqueShow.activeForm != "实时数据") return;
            }

            //事件委托
            MyDevice.myUpdate -= new freshHandler(receiveData);

            //操作弹窗
            MenuActualData menuActualData = new MenuActualData();
            menuActualData.StartPosition = FormStartPosition.CenterScreen;

            if (menuActualData.ShowDialog() == DialogResult.OK)
            {
                dataInfosFromOperation.Clear();

                dataInfosFromOperation = menuActualData.DataInfos;
                if (dataInfosFromOperation.Count < 1) return;
                dataInfosUI.Add(dataInfosFromOperation[dataInfosFromOperation.Count - 1]);

                isOperating = false;
                menuActive = false;

                //根据不同exe环境来运行不同功能
                if (exePath.IndexOf("Koeorws.exe") != -1)
                {
                    if (KoeorwsShow.activeForm != "实时数据") return;
                }
                else if (exePath.IndexOf("Torque.exe") != -1)
                {
                    if (TorqueShow.activeForm != "实时数据") return;
                }

                //事件委托
                MyDevice.myUpdate += new freshHandler(receiveData);
                timer1.Enabled = true;

                //切换下一螺丝
                byte mxscrewTotal = 0;            //M1-M6模式下拧紧螺丝总数
                switch (actXET.modeMx)
                {
                    default:
                        break;
                    case 0:
                        mxscrewTotal = actXET.mxTable.M1.screwTotal;
                        break;
                    case 1:
                        mxscrewTotal = actXET.mxTable.M2.screwTotal;
                        break;
                    case 2:
                        mxscrewTotal = actXET.mxTable.M3.screwTotal;
                        break;
                    case 3:
                        mxscrewTotal = actXET.mxTable.M4.screwTotal;
                        break;
                    case 4:
                        mxscrewTotal = actXET.mxTable.M5.screwTotal;
                        break;
                    case 5:
                        mxscrewTotal = actXET.mxTable.M6.screwTotal;
                        break;
                }
                if ((mxscrewTotal != 0) && dataInfosFromOperation[dataInfosFromOperation.Count - 1].result.Equals("ok"))
                {
                    if (menuActualData.Mxscrew == mxscrewTotal)
                    {
                        menuActualData.Mxscrew = 1;
                    }
                    else
                    {
                        menuActualData.Mxscrew++;
                    }
                    switch (actXET.modeMx)
                    {
                        default:
                            break;
                        case 0:
                            actXET.mxTable.M1.screw = menuActualData.Mxscrew;
                            break;
                        case 1:
                            actXET.mxTable.M2.screw = menuActualData.Mxscrew;
                            break;
                        case 2:
                            actXET.mxTable.M3.screw = menuActualData.Mxscrew;
                            break;
                        case 3:
                            actXET.mxTable.M4.screw = menuActualData.Mxscrew;
                            break;
                        case 4:
                            actXET.mxTable.M5.screw = menuActualData.Mxscrew;
                            break;
                        case 5:
                            actXET.mxTable.M6.screw = menuActualData.Mxscrew;
                            break;
                    }
                }
            }
        }

        //定时器-页面加载触发
        private void timer1_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            //通讯监控
            if (actXET.isActive)
            {
                nextTask = TASKS.READ_HEART;
                MyDevice.protocol.Protocol_Read_SendCOM(TASKS.READ_HEART);
            }
        }

        //将时间戳转换为时间（ms）
        private string ConvertTimestampToStr(string timestamp)
        {
            DateTime dateTime = MyDevice.GetMilTime(timestamp);
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        //导出csv
        public bool saveActualDataToExcel(string mePath)
        {
            //空
            if (mePath == null)
            {
                return false;
            }

            //写入
            try
            {
                //excel的每一行
                var lines = new List<string>();

                lines.Add("序号,工位号,产品编码,M值,拧紧数量,扭力值设定,角度设定,扭力值下限,扭力值上限,角度值下限,角度值上限,最终扭力值,最终角度值,结果,时间");
                DataInfo dataInfoToExcel = new DataInfo();

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    dataInfoToExcel = new DataInfo();

                    dataInfoToExcel.work_id = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    dataInfoToExcel.custom_id = dataGridView1.Rows[i].Cells[2].Value.ToString();
                    dataInfoToExcel.m_value = dataGridView1.Rows[i].Cells[3].Value.ToString();
                    dataInfoToExcel.screw_num = dataGridView1.Rows[i].Cells[4].Value.ToString();
                    dataInfoToExcel.torque_target = dataGridView1.Rows[i].Cells[5].Value.ToString();
                    dataInfoToExcel.angle_target = dataGridView1.Rows[i].Cells[6].Value.ToString();
                    dataInfoToExcel.torque_low = dataGridView1.Rows[i].Cells[7].Value.ToString();
                    dataInfoToExcel.torque_up = dataGridView1.Rows[i].Cells[8].Value.ToString();
                    dataInfoToExcel.angle_low = dataGridView1.Rows[i].Cells[9].Value.ToString();
                    dataInfoToExcel.angle_up = dataGridView1.Rows[i].Cells[10].Value.ToString();
                    dataInfoToExcel.torque_over = dataGridView1.Rows[i].Cells[11].Value.ToString();
                    dataInfoToExcel.angle_over = dataGridView1.Rows[i].Cells[12].Value.ToString();

                    Image imageValue = (Image)dataGridView1.Rows[i].Cells[13].Value;
                    Bitmap resultPic = new Bitmap(imageValue);
                    Bitmap green = new Bitmap(Properties.Resources.green);
                    if (ImageCompareArray(resultPic, green))
                    {
                        dataInfoToExcel.result = "OK";
                    }
                    else
                    {
                        dataInfoToExcel.result = "NG";
                    }
                    dataInfoToExcel.time = dataGridView1.Rows[i].Cells[14].Value.ToString();

                    //加=\，使csv文件用excel打开时能正常显示数据
                    lines.Add($"{i + 1},{dataInfoToExcel.work_id},{dataInfoToExcel.custom_id},{dataInfoToExcel.m_value},=\"{dataInfoToExcel.screw_num}\"," +
                              $"{dataInfoToExcel.torque_target},{dataInfoToExcel.angle_target},{dataInfoToExcel.torque_low},{dataInfoToExcel.torque_up}," +
                              $"{dataInfoToExcel.angle_low},{dataInfoToExcel.angle_up},{dataInfoToExcel.torque_over},{dataInfoToExcel.angle_over}," +
                              $"{dataInfoToExcel.result},=\"{dataInfoToExcel.time}\"");
                }

                File.WriteAllLines(mePath, lines, System.Text.Encoding.Default);
                System.IO.File.SetAttributes(mePath, FileAttributes.ReadOnly);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //比较两张图片是否完全一样
        private bool ImageCompareArray(Bitmap firstImage, Bitmap secondImage)
        {
            MemoryStream ms = new MemoryStream();
            firstImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            String firstBitmap = Convert.ToBase64String(ms.ToArray());
            ms.Position = 0;
            secondImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            String secondBitmap = Convert.ToBase64String(ms.ToArray());
            if (firstBitmap.Equals(secondBitmap))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // 去除字符串中的单位
        private string RemoveUnit(string value)
        {
            string numericPart = Regex.Match(value, @"[0-9]+(?:\.[0-9]+)?").Value;
            return numericPart;
        }

        private void MenuActualDataPanel_Resize(object sender, EventArgs e)
        {
            autoSize.UIComponetForm_Resize(this);
        }
    }
}
