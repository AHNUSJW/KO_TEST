using BIL;
using Library;
using Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

//Junzhe 20230817
//数据库相关功能尚未添加
//不同单位的数据转换存在精度丢失情况

//Junzhe 20230830
//不同单位的数据转换存在精度丢失情况

//Ricardo 20231008

namespace Base.UI
{
    public partial class MenuCalPanel : UserControl
    {
        String unit = "";//单位
        private XET actXET;//需操作的设备
        private byte rwCnt;//按键写入状态
        private volatile TASKS nextTask;//按键操作指令
        private DataGridViewTextBoxEditingControl CellEdit = null;
        private JDBC jdbc = new JDBC();//数据库操作类
        private List<DevicePara> para = new List<DevicePara>();//存参数的数据库

        private string exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;//当前进程exe路径

        AutoFormSize autoSize = new AutoFormSize();

        public MenuCalPanel()
        {
            InitializeComponent();
            autoSize.UIComponetForm(this);
        }

        //初始化参数
        private void InitPara()
        {
            //量程
            textBox4.Text = (actXET.torqueCapacity / 100.0f).ToString() + " " + unit;

            //单位
            ucCombox1.Source = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("0", "N·m"),
                new KeyValuePair<string, string>("1", "lbf·in"),
                new KeyValuePair<string, string>("2", "lbf·ft"),
                new KeyValuePair<string, string>("3", "kgf·cm"),
            };
            ucCombox1.SelectedIndex = (int)actXET.torqueUnit;

            //PT模式
            ucCombox2.Source = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("0", "Peak"),
                new KeyValuePair<string, string>("1", "Track"),
            };
            ucCombox2.SelectedIndex = actXET.modePt;

            //M值
            ucCombox3.Source = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("0", "M1"),
                new KeyValuePair<string, string>("1", "M2"),
                new KeyValuePair<string, string>("2", "M3"),
                new KeyValuePair<string, string>("3", "M4"),
                new KeyValuePair<string, string>("4", "M5"),
                new KeyValuePair<string, string>("5", "M6")
            };
            ucCombox3.SelectedIndex = actXET.modeMx;

            //角度挡位
            ucCombox4.Source = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("0", "15°/sec ★"),
                new KeyValuePair<string, string>("1", "30°/sec ★"),
                new KeyValuePair<string, string>("2", "60°/sec ★"),
                new KeyValuePair<string, string>("3", "120°/sec"),
                new KeyValuePair<string, string>("4", "250°/sec"),
                new KeyValuePair<string, string>("5", "500°/sec"),
                new KeyValuePair<string, string>("6", "1000°/sec"),
                new KeyValuePair<string, string>("7", "2000°/sec")
            };
            ucCombox4.SelectedIndex = actXET.angleSpeed;

            //数据存储模式
            ucCombox5.Source = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("0", (MyDevice.languageType == 0 ? "手动保存" : "Save manually")),
                new KeyValuePair<string, string>("1", (MyDevice.languageType == 0 ? "1s自动保存" : "1s auto-save")),
                new KeyValuePair<string, string>("2", (MyDevice.languageType == 0 ? "2s自动保存" : "2s auto-save")),
                new KeyValuePair<string, string>("3", (MyDevice.languageType == 0 ? "3s自动保存" : "3s auto-save")),
                new KeyValuePair<string, string>("4", (MyDevice.languageType == 0 ? "4s自动保存" : "4s auto-save")),
                new KeyValuePair<string, string>("5", (MyDevice.languageType == 0 ? "5s自动保存" : "5s auto-save")),
                new KeyValuePair<string, string>("6", (MyDevice.languageType == 0 ? "6s自动保存" : "6s auto-save")),
                new KeyValuePair<string, string>("7", (MyDevice.languageType == 0 ? "7s自动保存" : "7s auto-save")),
                new KeyValuePair<string, string>("8", (MyDevice.languageType == 0 ? "8s自动保存" : "8s auto-save")),
                new KeyValuePair<string, string>("9", (MyDevice.languageType == 0 ? "9s自动保存" : "9s auto-save")),
                new KeyValuePair<string, string>("10", (MyDevice.languageType == 0 ? "10s自动保存" : "10s auto-save"))
            };
            ucCombox5.SelectedIndex = actXET.modeRec;

            textBox1.Text = actXET.workId;
            textBox2.Text = actXET.customId;
        }

        //初始化MX表
        private void InitTableMX()
        {
            //表属性
            //dataGridView1.Size = new Size(792, 280);
            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView1.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView1.EnableHeadersVisualStyles = false;
            //dataGridView1.ColumnHeadersHeight = 30;
            //dataGridView1.RowTemplate.Height = 30;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToOrderColumns = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.RowHeadersVisible = false;
            //dataGridView1.Font = new Font("Arial", 8, FontStyle.Bold);

            //列属性
            //dataGridView1.Columns[0].Width = Convert.ToInt32(dataGridView1.Width / 9);
            //dataGridView1.Columns[1].Width = 82;
            //dataGridView1.Columns[2].Width = 95;
            //dataGridView1.Columns[3].Width = 95;
            //dataGridView1.Columns[4].Width = 95;
            //dataGridView1.Columns[5].Width = 95;
            //dataGridView1.Columns[6].Width = 95;
            //dataGridView1.Columns[7].Width = 95;
            //dataGridView1.Columns[8].Width = 95;

            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[7].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[8].SortMode = DataGridViewColumnSortMode.NotSortable;

            //行属性
            dataGridView1.Rows.Add(6);
            dataGridView1.Rows[0].Cells[0].Value = "M1";
            dataGridView1.Rows[1].Cells[0].Value = "M2";
            dataGridView1.Rows[2].Cells[0].Value = "M3";
            dataGridView1.Rows[3].Cells[0].Value = "M4";
            dataGridView1.Rows[4].Cells[0].Value = "M5";
            dataGridView1.Rows[5].Cells[0].Value = "M6";

            dataGridView1.Rows[0].Cells[5].Value = unit;
            dataGridView1.Rows[1].Cells[5].Value = unit;
            dataGridView1.Rows[2].Cells[5].Value = unit;
            dataGridView1.Rows[3].Cells[5].Value = unit;
            dataGridView1.Rows[4].Cells[5].Value = unit;
            dataGridView1.Rows[5].Cells[5].Value = unit;

            dataGridView1.Rows[0].Cells[0].ReadOnly = true;
            dataGridView1.Rows[1].Cells[0].ReadOnly = true;
            dataGridView1.Rows[2].Cells[0].ReadOnly = true;
            dataGridView1.Rows[3].Cells[0].ReadOnly = true;
            dataGridView1.Rows[4].Cells[0].ReadOnly = true;
            dataGridView1.Rows[5].Cells[0].ReadOnly = true;

            dataGridView1.Rows[0].Cells[5].ReadOnly = true;
            dataGridView1.Rows[1].Cells[5].ReadOnly = true;
            dataGridView1.Rows[2].Cells[5].ReadOnly = true;
            dataGridView1.Rows[3].Cells[5].ReadOnly = true;
            dataGridView1.Rows[4].Cells[5].ReadOnly = true;
            dataGridView1.Rows[5].Cells[5].ReadOnly = true;

            dataGridView1.Rows[0].Cells[1].Value = actXET.mxTable.M1.screwTotal;
            dataGridView1.Rows[1].Cells[1].Value = actXET.mxTable.M2.screwTotal;
            dataGridView1.Rows[2].Cells[1].Value = actXET.mxTable.M3.screwTotal;
            dataGridView1.Rows[3].Cells[1].Value = actXET.mxTable.M4.screwTotal;
            dataGridView1.Rows[4].Cells[1].Value = actXET.mxTable.M5.screwTotal;
            dataGridView1.Rows[5].Cells[1].Value = actXET.mxTable.M6.screwTotal;

            dataGridView1.Rows[0].Cells[2].Value = actXET.mxTable.M1.torqueTarget / 100.0f;
            dataGridView1.Rows[1].Cells[2].Value = actXET.mxTable.M2.torqueTarget / 100.0f;
            dataGridView1.Rows[2].Cells[2].Value = actXET.mxTable.M3.torqueTarget / 100.0f;
            dataGridView1.Rows[3].Cells[2].Value = actXET.mxTable.M4.torqueTarget / 100.0f;
            dataGridView1.Rows[4].Cells[2].Value = actXET.mxTable.M5.torqueTarget / 100.0f;
            dataGridView1.Rows[5].Cells[2].Value = actXET.mxTable.M6.torqueTarget / 100.0f;

            dataGridView1.Rows[0].Cells[3].Value = actXET.mxTable.M1.torqueLow / 100.0f;
            dataGridView1.Rows[1].Cells[3].Value = actXET.mxTable.M2.torqueLow / 100.0f;
            dataGridView1.Rows[2].Cells[3].Value = actXET.mxTable.M3.torqueLow / 100.0f;
            dataGridView1.Rows[3].Cells[3].Value = actXET.mxTable.M4.torqueLow / 100.0f;
            dataGridView1.Rows[4].Cells[3].Value = actXET.mxTable.M5.torqueLow / 100.0f;
            dataGridView1.Rows[5].Cells[3].Value = actXET.mxTable.M6.torqueLow / 100.0f;

            dataGridView1.Rows[0].Cells[4].Value = actXET.mxTable.M1.torqueHigh / 100.0f;
            dataGridView1.Rows[1].Cells[4].Value = actXET.mxTable.M2.torqueHigh / 100.0f;
            dataGridView1.Rows[2].Cells[4].Value = actXET.mxTable.M3.torqueHigh / 100.0f;
            dataGridView1.Rows[3].Cells[4].Value = actXET.mxTable.M4.torqueHigh / 100.0f;
            dataGridView1.Rows[4].Cells[4].Value = actXET.mxTable.M5.torqueHigh / 100.0f;
            dataGridView1.Rows[5].Cells[4].Value = actXET.mxTable.M6.torqueHigh / 100.0f;

            dataGridView1.Rows[0].Cells[6].Value = actXET.mxTable.M1.angleTarget / 10.0f;
            dataGridView1.Rows[1].Cells[6].Value = actXET.mxTable.M2.angleTarget / 10.0f;
            dataGridView1.Rows[2].Cells[6].Value = actXET.mxTable.M3.angleTarget / 10.0f;
            dataGridView1.Rows[3].Cells[6].Value = actXET.mxTable.M4.angleTarget / 10.0f;
            dataGridView1.Rows[4].Cells[6].Value = actXET.mxTable.M5.angleTarget / 10.0f;
            dataGridView1.Rows[5].Cells[6].Value = actXET.mxTable.M6.angleTarget / 10.0f;

            dataGridView1.Rows[0].Cells[7].Value = actXET.mxTable.M1.angleLow / 10.0f;
            dataGridView1.Rows[1].Cells[7].Value = actXET.mxTable.M2.angleLow / 10.0f;
            dataGridView1.Rows[2].Cells[7].Value = actXET.mxTable.M3.angleLow / 10.0f;
            dataGridView1.Rows[3].Cells[7].Value = actXET.mxTable.M4.angleLow / 10.0f;
            dataGridView1.Rows[4].Cells[7].Value = actXET.mxTable.M5.angleLow / 10.0f;
            dataGridView1.Rows[5].Cells[7].Value = actXET.mxTable.M6.angleLow / 10.0f;

            dataGridView1.Rows[0].Cells[8].Value = actXET.mxTable.M1.angleHigh / 10.0f;
            dataGridView1.Rows[1].Cells[8].Value = actXET.mxTable.M2.angleHigh / 10.0f;
            dataGridView1.Rows[2].Cells[8].Value = actXET.mxTable.M3.angleHigh / 10.0f;
            dataGridView1.Rows[3].Cells[8].Value = actXET.mxTable.M4.angleHigh / 10.0f;
            dataGridView1.Rows[4].Cells[8].Value = actXET.mxTable.M5.angleHigh / 10.0f;
            dataGridView1.Rows[5].Cells[8].Value = actXET.mxTable.M6.angleHigh / 10.0f;

            //列宽
            dataGridView1.Columns[0].Width = Convert.ToInt32(dataGridView1.Width / 9);
            dataGridView1.Columns[1].Width = Convert.ToInt32(dataGridView1.Width / 9);
            dataGridView1.Columns[2].Width = Convert.ToInt32(dataGridView1.Width / 9);
            dataGridView1.Columns[3].Width = Convert.ToInt32(dataGridView1.Width / 9);
            dataGridView1.Columns[4].Width = Convert.ToInt32(dataGridView1.Width / 9);
            dataGridView1.Columns[5].Width = Convert.ToInt32(dataGridView1.Width / 9);
            dataGridView1.Columns[6].Width = Convert.ToInt32(dataGridView1.Width / 9);
            dataGridView1.Columns[7].Width = Convert.ToInt32(dataGridView1.Width / 9);
            dataGridView1.Columns[8].Width = Convert.ToInt32(dataGridView1.Width / 9);
        }

        //更新MX表
        private void UpdateTableMX()
        {
            dataGridView1.Rows[0].Cells[5].Value = unit;
            dataGridView1.Rows[1].Cells[5].Value = unit;
            dataGridView1.Rows[2].Cells[5].Value = unit;
            dataGridView1.Rows[3].Cells[5].Value = unit;
            dataGridView1.Rows[4].Cells[5].Value = unit;
            dataGridView1.Rows[5].Cells[5].Value = unit;

            dataGridView1.Rows[0].Cells[1].Value = actXET.mxTable.M1.screwTotal;
            dataGridView1.Rows[1].Cells[1].Value = actXET.mxTable.M2.screwTotal;
            dataGridView1.Rows[2].Cells[1].Value = actXET.mxTable.M3.screwTotal;
            dataGridView1.Rows[3].Cells[1].Value = actXET.mxTable.M4.screwTotal;
            dataGridView1.Rows[4].Cells[1].Value = actXET.mxTable.M5.screwTotal;
            dataGridView1.Rows[5].Cells[1].Value = actXET.mxTable.M6.screwTotal;

            dataGridView1.Rows[0].Cells[2].Value = MyDevice.mSUT.Torque_nmTrans(actXET.mxTable.M1.torqueTarget, unit) / 100.0f;
            dataGridView1.Rows[1].Cells[2].Value = MyDevice.mSUT.Torque_nmTrans(actXET.mxTable.M2.torqueTarget, unit) / 100.0f;
            dataGridView1.Rows[2].Cells[2].Value = MyDevice.mSUT.Torque_nmTrans(actXET.mxTable.M3.torqueTarget, unit) / 100.0f;
            dataGridView1.Rows[3].Cells[2].Value = MyDevice.mSUT.Torque_nmTrans(actXET.mxTable.M4.torqueTarget, unit) / 100.0f;
            dataGridView1.Rows[4].Cells[2].Value = MyDevice.mSUT.Torque_nmTrans(actXET.mxTable.M5.torqueTarget, unit) / 100.0f;
            dataGridView1.Rows[5].Cells[2].Value = MyDevice.mSUT.Torque_nmTrans(actXET.mxTable.M6.torqueTarget, unit) / 100.0f;

            dataGridView1.Rows[0].Cells[3].Value = MyDevice.mSUT.Torque_nmTrans(actXET.mxTable.M1.torqueLow, unit) / 100.0f;
            dataGridView1.Rows[1].Cells[3].Value = MyDevice.mSUT.Torque_nmTrans(actXET.mxTable.M2.torqueLow, unit) / 100.0f;
            dataGridView1.Rows[2].Cells[3].Value = MyDevice.mSUT.Torque_nmTrans(actXET.mxTable.M3.torqueLow, unit) / 100.0f;
            dataGridView1.Rows[3].Cells[3].Value = MyDevice.mSUT.Torque_nmTrans(actXET.mxTable.M4.torqueLow, unit) / 100.0f;
            dataGridView1.Rows[4].Cells[3].Value = MyDevice.mSUT.Torque_nmTrans(actXET.mxTable.M5.torqueLow, unit) / 100.0f;
            dataGridView1.Rows[5].Cells[3].Value = MyDevice.mSUT.Torque_nmTrans(actXET.mxTable.M6.torqueLow, unit) / 100.0f;

            dataGridView1.Rows[0].Cells[4].Value = MyDevice.mSUT.Torque_nmTrans(actXET.mxTable.M1.torqueHigh, unit) / 100.0f;
            dataGridView1.Rows[1].Cells[4].Value = MyDevice.mSUT.Torque_nmTrans(actXET.mxTable.M2.torqueHigh, unit) / 100.0f;
            dataGridView1.Rows[2].Cells[4].Value = MyDevice.mSUT.Torque_nmTrans(actXET.mxTable.M3.torqueHigh, unit) / 100.0f;
            dataGridView1.Rows[3].Cells[4].Value = MyDevice.mSUT.Torque_nmTrans(actXET.mxTable.M4.torqueHigh, unit) / 100.0f;
            dataGridView1.Rows[4].Cells[4].Value = MyDevice.mSUT.Torque_nmTrans(actXET.mxTable.M5.torqueHigh, unit) / 100.0f;
            dataGridView1.Rows[5].Cells[4].Value = MyDevice.mSUT.Torque_nmTrans(actXET.mxTable.M6.torqueHigh, unit) / 100.0f;

            dataGridView1.Rows[0].Cells[6].Value = actXET.mxTable.M1.angleTarget / 10.0f;
            dataGridView1.Rows[1].Cells[6].Value = actXET.mxTable.M2.angleTarget / 10.0f;
            dataGridView1.Rows[2].Cells[6].Value = actXET.mxTable.M3.angleTarget / 10.0f;
            dataGridView1.Rows[3].Cells[6].Value = actXET.mxTable.M4.angleTarget / 10.0f;
            dataGridView1.Rows[4].Cells[6].Value = actXET.mxTable.M5.angleTarget / 10.0f;
            dataGridView1.Rows[5].Cells[6].Value = actXET.mxTable.M6.angleTarget / 10.0f;

            dataGridView1.Rows[0].Cells[7].Value = actXET.mxTable.M1.angleLow / 10.0f;
            dataGridView1.Rows[1].Cells[7].Value = actXET.mxTable.M2.angleLow / 10.0f;
            dataGridView1.Rows[2].Cells[7].Value = actXET.mxTable.M3.angleLow / 10.0f;
            dataGridView1.Rows[3].Cells[7].Value = actXET.mxTable.M4.angleLow / 10.0f;
            dataGridView1.Rows[4].Cells[7].Value = actXET.mxTable.M5.angleLow / 10.0f;
            dataGridView1.Rows[5].Cells[7].Value = actXET.mxTable.M6.angleLow / 10.0f;

            dataGridView1.Rows[0].Cells[8].Value = actXET.mxTable.M1.angleHigh / 10.0f;
            dataGridView1.Rows[1].Cells[8].Value = actXET.mxTable.M2.angleHigh / 10.0f;
            dataGridView1.Rows[2].Cells[8].Value = actXET.mxTable.M3.angleHigh / 10.0f;
            dataGridView1.Rows[3].Cells[8].Value = actXET.mxTable.M4.angleHigh / 10.0f;
            dataGridView1.Rows[4].Cells[8].Value = actXET.mxTable.M5.angleHigh / 10.0f;
            dataGridView1.Rows[5].Cells[8].Value = actXET.mxTable.M6.angleHigh / 10.0f;
        }

        //页面加载及关闭
        private void MenuCalPanel_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                //初始化设备数据
                actXET = MyDevice.actDev;
                nextTask = TASKS.NULL;

                //自定义控件文本单独设置
                if (MyDevice.languageType == 1)
                {
                    ucBtnExt1.BtnText = "Confirm";
                    ucBtnExt2.BtnText = "Clear cache";
                }

                //获取当前力矩单位
                switch (actXET.torqueUnit)
                {
                    case UNIT.UNIT_nm: unit = "N·m"; break;
                    case UNIT.UNIT_lbfin: unit = "lbf·in"; break;
                    case UNIT.UNIT_lbfft: unit = "lbf·ft"; break;
                    case UNIT.UNIT_kgcm: unit = "kgf·cm"; break;
                }

                //初始化table
                InitTableMX();

                //初始化参数
                InitPara();

                MyDevice.myUpdate += new freshHandler(receivePara);
            }
            else
            {
                MyDevice.myUpdate -= new freshHandler(receivePara);
            }

            //表格显示6行9列
            if (dataGridView1.Rows.Count <= 6)
            {
                dataGridView1.ColumnHeadersHeight = dataGridView1.Height / 7;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.DefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                    row.Height = dataGridView1.Height / 7;
                }
            }
            else
            {
                for (int i = 6; i < dataGridView1.Rows.Count; i++)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.ControlDark;
                }
            }

            if (dataGridView1.ColumnCount == 9)
            {
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    col.Width = dataGridView1.Width / 9;
                }
            }
        }

        //写入参数
        private void ucBtnExt1_BtnClick(object sender, EventArgs e)
        {
            //聚焦光标，用于自动更新表格参数（表格参数更新需要光标移开）
            ucBtnExt1.Focus();

            if (exePath.IndexOf("Koeorws.exe") != -1)
            {
                //扳手编码不得为空
                KoeorwsShow.isCustomIdNull = textBox2.Text == "" ? true : false;

                if (KoeorwsShow.isCustomIdNull)
                {
                    if (MyDevice.languageType == 0)
                    {
                        MessageBox.Show("请输入扳手编码", "输入提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        MessageBoxEX.Show("Please enter the wrench code", "Enter the prompt", MessageBoxButtons.OK, new string[] { "OK" });
                    }
                    return;
                }
            }
            else if (exePath.IndexOf("Torque.exe") != -1)
            {
                //扳手编码不得为空
                TorqueShow.isCustomIdNull = textBox2.Text == "" ? true : false;

                if (TorqueShow.isCustomIdNull)
                {
                    if (MyDevice.languageType == 0)
                    {
                        MessageBox.Show("请输入扳手编码", "输入提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        MessageBoxEX.Show("Please enter the wrench code", "Enter the prompt", MessageBoxButtons.OK, new string[] { "OK" });
                    }
                    return;
                }
            }   

            //当前量程
            UInt32 torqueCap = (UInt32)MyDevice.mSUT.TorqueTransNm(Convert.ToDouble(textBox4.Text.Split(' ')[0]) * 100f + 0.5f, unit);

            //基础参数
            actXET.torqueUnit = (UNIT)ucCombox1.SelectedIndex;
            actXET.modePt = (byte)ucCombox2.SelectedIndex;
            actXET.modeMx = (byte)ucCombox3.SelectedIndex;
            actXET.angleSpeed = ucCombox4.SelectedIndex;
            actXET.modeRec = (byte)ucCombox5.SelectedIndex;
            actXET.workId = textBox1.Text;
            actXET.customId = textBox2.Text;

            //拧紧数量
            actXET.mxTable.M1.screwTotal = Convert.ToByte(dataGridView1.Rows[0].Cells[1].Value);
            actXET.mxTable.M2.screwTotal = Convert.ToByte(dataGridView1.Rows[1].Cells[1].Value);
            actXET.mxTable.M3.screwTotal = Convert.ToByte(dataGridView1.Rows[2].Cells[1].Value);
            actXET.mxTable.M4.screwTotal = Convert.ToByte(dataGridView1.Rows[3].Cells[1].Value);
            actXET.mxTable.M5.screwTotal = Convert.ToByte(dataGridView1.Rows[4].Cells[1].Value);
            actXET.mxTable.M6.screwTotal = Convert.ToByte(dataGridView1.Rows[5].Cells[1].Value);

            //目标力矩
            actXET.mxTable.M1.torqueTarget = (UInt32)(MyDevice.mSUT.TorqueTransNm(Convert.ToDouble(dataGridView1.Rows[0].Cells[2].Value) * 100 + 0.5, unit));
            actXET.mxTable.M2.torqueTarget = (UInt32)(MyDevice.mSUT.TorqueTransNm(Convert.ToDouble(dataGridView1.Rows[1].Cells[2].Value) * 100 + 0.5, unit));
            actXET.mxTable.M3.torqueTarget = (UInt32)(MyDevice.mSUT.TorqueTransNm(Convert.ToDouble(dataGridView1.Rows[2].Cells[2].Value) * 100 + 0.5, unit));
            actXET.mxTable.M4.torqueTarget = (UInt32)(MyDevice.mSUT.TorqueTransNm(Convert.ToDouble(dataGridView1.Rows[3].Cells[2].Value) * 100 + 0.5, unit));
            actXET.mxTable.M5.torqueTarget = (UInt32)(MyDevice.mSUT.TorqueTransNm(Convert.ToDouble(dataGridView1.Rows[4].Cells[2].Value) * 100 + 0.5, unit));
            actXET.mxTable.M6.torqueTarget = (UInt32)(MyDevice.mSUT.TorqueTransNm(Convert.ToDouble(dataGridView1.Rows[5].Cells[2].Value) * 100 + 0.5, unit));
            //检查输入范围
            if (!getDataCheck(ref actXET.mxTable.M1.torqueTarget, 0, torqueCap)) return;
            if (!getDataCheck(ref actXET.mxTable.M2.torqueTarget, 0, torqueCap)) return;
            if (!getDataCheck(ref actXET.mxTable.M3.torqueTarget, 0, torqueCap)) return;
            if (!getDataCheck(ref actXET.mxTable.M4.torqueTarget, 0, torqueCap)) return;
            if (!getDataCheck(ref actXET.mxTable.M5.torqueTarget, 0, torqueCap)) return;
            if (!getDataCheck(ref actXET.mxTable.M6.torqueTarget, 0, torqueCap)) return;

            //力矩下限
            actXET.mxTable.M1.torqueLow = (UInt32)(MyDevice.mSUT.TorqueTransNm(Convert.ToDouble(dataGridView1.Rows[0].Cells[3].Value) * 100 + 0.5, unit));
            actXET.mxTable.M2.torqueLow = (UInt32)(MyDevice.mSUT.TorqueTransNm(Convert.ToDouble(dataGridView1.Rows[1].Cells[3].Value) * 100 + 0.5, unit));
            actXET.mxTable.M3.torqueLow = (UInt32)(MyDevice.mSUT.TorqueTransNm(Convert.ToDouble(dataGridView1.Rows[2].Cells[3].Value) * 100 + 0.5, unit));
            actXET.mxTable.M4.torqueLow = (UInt32)(MyDevice.mSUT.TorqueTransNm(Convert.ToDouble(dataGridView1.Rows[3].Cells[3].Value) * 100 + 0.5, unit));
            actXET.mxTable.M5.torqueLow = (UInt32)(MyDevice.mSUT.TorqueTransNm(Convert.ToDouble(dataGridView1.Rows[4].Cells[3].Value) * 100 + 0.5, unit));
            actXET.mxTable.M6.torqueLow = (UInt32)(MyDevice.mSUT.TorqueTransNm(Convert.ToDouble(dataGridView1.Rows[5].Cells[3].Value) * 100 + 0.5, unit));
            //检查输入范围
            if (!getDataCheck(ref actXET.mxTable.M1.torqueLow, 0, torqueCap)) return;
            if (!getDataCheck(ref actXET.mxTable.M2.torqueLow, 0, torqueCap)) return;
            if (!getDataCheck(ref actXET.mxTable.M3.torqueLow, 0, torqueCap)) return;
            if (!getDataCheck(ref actXET.mxTable.M4.torqueLow, 0, torqueCap)) return;
            if (!getDataCheck(ref actXET.mxTable.M5.torqueLow, 0, torqueCap)) return;
            if (!getDataCheck(ref actXET.mxTable.M6.torqueLow, 0, torqueCap)) return;

            //力矩上限
            actXET.mxTable.M1.torqueHigh = (UInt32)(MyDevice.mSUT.TorqueTransNm(Convert.ToDouble(dataGridView1.Rows[0].Cells[4].Value) * 100 + 0.5, unit));
            actXET.mxTable.M2.torqueHigh = (UInt32)(MyDevice.mSUT.TorqueTransNm(Convert.ToDouble(dataGridView1.Rows[1].Cells[4].Value) * 100 + 0.5, unit));
            actXET.mxTable.M3.torqueHigh = (UInt32)(MyDevice.mSUT.TorqueTransNm(Convert.ToDouble(dataGridView1.Rows[2].Cells[4].Value) * 100 + 0.5, unit));
            actXET.mxTable.M4.torqueHigh = (UInt32)(MyDevice.mSUT.TorqueTransNm(Convert.ToDouble(dataGridView1.Rows[3].Cells[4].Value) * 100 + 0.5, unit));
            actXET.mxTable.M5.torqueHigh = (UInt32)(MyDevice.mSUT.TorqueTransNm(Convert.ToDouble(dataGridView1.Rows[4].Cells[4].Value) * 100 + 0.5, unit));
            actXET.mxTable.M6.torqueHigh = (UInt32)(MyDevice.mSUT.TorqueTransNm(Convert.ToDouble(dataGridView1.Rows[5].Cells[4].Value) * 100 + 0.5, unit));
            //检查输入范围
            if (!getDataCheck(ref actXET.mxTable.M1.torqueHigh, actXET.mxTable.M1.torqueLow, torqueCap)) return;
            if (!getDataCheck(ref actXET.mxTable.M2.torqueHigh, actXET.mxTable.M2.torqueLow, torqueCap)) return;
            if (!getDataCheck(ref actXET.mxTable.M3.torqueHigh, actXET.mxTable.M3.torqueLow, torqueCap)) return;
            if (!getDataCheck(ref actXET.mxTable.M4.torqueHigh, actXET.mxTable.M4.torqueLow, torqueCap)) return;
            if (!getDataCheck(ref actXET.mxTable.M5.torqueHigh, actXET.mxTable.M5.torqueLow, torqueCap)) return;
            if (!getDataCheck(ref actXET.mxTable.M6.torqueHigh, actXET.mxTable.M6.torqueLow, torqueCap)) return;

            //目标角度
            actXET.mxTable.M1.angleTarget = (UInt16)(Convert.ToDouble(dataGridView1.Rows[0].Cells[6].Value) * 10 + 0.5);
            actXET.mxTable.M2.angleTarget = (UInt16)(Convert.ToDouble(dataGridView1.Rows[1].Cells[6].Value) * 10 + 0.5);
            actXET.mxTable.M3.angleTarget = (UInt16)(Convert.ToDouble(dataGridView1.Rows[2].Cells[6].Value) * 10 + 0.5);
            actXET.mxTable.M4.angleTarget = (UInt16)(Convert.ToDouble(dataGridView1.Rows[3].Cells[6].Value) * 10 + 0.5);
            actXET.mxTable.M5.angleTarget = (UInt16)(Convert.ToDouble(dataGridView1.Rows[4].Cells[6].Value) * 10 + 0.5);
            actXET.mxTable.M6.angleTarget = (UInt16)(Convert.ToDouble(dataGridView1.Rows[5].Cells[6].Value) * 10 + 0.5);
            //检查输入范围
            if (!getDataCheck(ref actXET.mxTable.M1.angleTarget, 0, 3600)) return;
            if (!getDataCheck(ref actXET.mxTable.M2.angleTarget, 0, 3600)) return;
            if (!getDataCheck(ref actXET.mxTable.M3.angleTarget, 0, 3600)) return;
            if (!getDataCheck(ref actXET.mxTable.M4.angleTarget, 0, 3600)) return;
            if (!getDataCheck(ref actXET.mxTable.M5.angleTarget, 0, 3600)) return;
            if (!getDataCheck(ref actXET.mxTable.M6.angleTarget, 0, 3600)) return;

            //角度下限
            actXET.mxTable.M1.angleLow = (UInt16)(Convert.ToDouble(dataGridView1.Rows[0].Cells[7].Value) * 10 + 0.5);
            actXET.mxTable.M2.angleLow = (UInt16)(Convert.ToDouble(dataGridView1.Rows[1].Cells[7].Value) * 10 + 0.5);
            actXET.mxTable.M3.angleLow = (UInt16)(Convert.ToDouble(dataGridView1.Rows[2].Cells[7].Value) * 10 + 0.5);
            actXET.mxTable.M4.angleLow = (UInt16)(Convert.ToDouble(dataGridView1.Rows[3].Cells[7].Value) * 10 + 0.5);
            actXET.mxTable.M5.angleLow = (UInt16)(Convert.ToDouble(dataGridView1.Rows[4].Cells[7].Value) * 10 + 0.5);
            actXET.mxTable.M6.angleLow = (UInt16)(Convert.ToDouble(dataGridView1.Rows[5].Cells[7].Value) * 10 + 0.5);
            //检查输入范围
            if (!getDataCheck(ref actXET.mxTable.M1.angleLow, 0, 3600)) return;
            if (!getDataCheck(ref actXET.mxTable.M2.angleLow, 0, 3600)) return;
            if (!getDataCheck(ref actXET.mxTable.M3.angleLow, 0, 3600)) return;
            if (!getDataCheck(ref actXET.mxTable.M4.angleLow, 0, 3600)) return;
            if (!getDataCheck(ref actXET.mxTable.M5.angleLow, 0, 3600)) return;
            if (!getDataCheck(ref actXET.mxTable.M6.angleLow, 0, 3600)) return;

            //角度上限
            actXET.mxTable.M1.angleHigh = (UInt16)(Convert.ToDouble(dataGridView1.Rows[0].Cells[8].Value) * 10 + 0.5);
            actXET.mxTable.M2.angleHigh = (UInt16)(Convert.ToDouble(dataGridView1.Rows[1].Cells[8].Value) * 10 + 0.5);
            actXET.mxTable.M3.angleHigh = (UInt16)(Convert.ToDouble(dataGridView1.Rows[2].Cells[8].Value) * 10 + 0.5);
            actXET.mxTable.M4.angleHigh = (UInt16)(Convert.ToDouble(dataGridView1.Rows[3].Cells[8].Value) * 10 + 0.5);
            actXET.mxTable.M5.angleHigh = (UInt16)(Convert.ToDouble(dataGridView1.Rows[4].Cells[8].Value) * 10 + 0.5);
            actXET.mxTable.M6.angleHigh = (UInt16)(Convert.ToDouble(dataGridView1.Rows[5].Cells[8].Value) * 10 + 0.5);
            //检查输入范围
            if (!getDataCheck(ref actXET.mxTable.M1.angleHigh, actXET.mxTable.M1.angleLow, 3600)) return;
            if (!getDataCheck(ref actXET.mxTable.M2.angleHigh, actXET.mxTable.M2.angleLow, 3600)) return;
            if (!getDataCheck(ref actXET.mxTable.M3.angleHigh, actXET.mxTable.M3.angleLow, 3600)) return;
            if (!getDataCheck(ref actXET.mxTable.M4.angleHigh, actXET.mxTable.M4.angleLow, 3600)) return;
            if (!getDataCheck(ref actXET.mxTable.M5.angleHigh, actXET.mxTable.M5.angleLow, 3600)) return;
            if (!getDataCheck(ref actXET.mxTable.M6.angleHigh, actXET.mxTable.M6.angleLow, 3600)) return;

            //目标力矩和目标角度不能同时为0
            if (actXET.mxTable.M1.torqueTarget == 0 && actXET.mxTable.M1.angleTarget == 0)
            {
                if ((MyDevice.languageType == 0 ?
                    MessageBox.Show("目标力矩和目标角度不能同为0！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation) :
                    MessageBoxEX.Show("The target torque and target angle cannot be the same as 0！", "Hint", MessageBoxButtons.OK, new string[] { "OK" })) == DialogResult.OK)
                {
                    return;
                }
            }
            if (actXET.mxTable.M2.torqueTarget == 0 && actXET.mxTable.M2.angleTarget == 0)
            {
                if ((MyDevice.languageType == 0 ? 
                    MessageBox.Show("目标力矩和目标角度不能同为0！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation) :
                    MessageBoxEX.Show("The target torque and target angle cannot be the same as 0！", "Hint", MessageBoxButtons.OK, new string[] { "OK" })) == DialogResult.OK)
                {
                    return;
                }
            }
            if (actXET.mxTable.M3.torqueTarget == 0 && actXET.mxTable.M3.angleTarget == 0)
            {
                if ((MyDevice.languageType == 0 ?
                    MessageBox.Show("目标力矩和目标角度不能同为0！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation) :
                    MessageBoxEX.Show("The target torque and target angle cannot be the same as 0！", "Hint", MessageBoxButtons.OK, new string[] { "OK" })) == DialogResult.OK)
                {
                    return;
                }
            }
            if (actXET.mxTable.M4.torqueTarget == 0 && actXET.mxTable.M4.angleTarget == 0)
            {
                if ((MyDevice.languageType == 0 ?
                    MessageBox.Show("目标力矩和目标角度不能同为0！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation) :
                    MessageBoxEX.Show("The target torque and target angle cannot be the same as 0！", "Hint", MessageBoxButtons.OK, new string[] { "OK" })) == DialogResult.OK)
                {
                    return;
                }
            }
            if (actXET.mxTable.M5.torqueTarget == 0 && actXET.mxTable.M5.angleTarget == 0)
            {
                if ((MyDevice.languageType == 0 ?
                    MessageBox.Show("目标力矩和目标角度不能同为0！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation) :
                    MessageBoxEX.Show("The target torque and target angle cannot be the same as 0！", "Hint", MessageBoxButtons.OK, new string[] { "OK" })) == DialogResult.OK)
                {
                    return;
                }
            }
            if (actXET.mxTable.M6.torqueTarget == 0 && actXET.mxTable.M6.angleTarget == 0)
            {
                if ((MyDevice.languageType == 0 ?
                    MessageBox.Show("目标力矩和目标角度不能同为0！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation) :
                    MessageBoxEX.Show("The target torque and target angle cannot be the same as 0！", "Hint", MessageBoxButtons.OK, new string[] { "OK" })) == DialogResult.OK)
                {
                    return;
                }
            }
            
            //参数初始化（设置同一台设备只添加一次M1-M6）
            para = new List<DevicePara>();

            //删除已有数据
            jdbc.DelDeviceParaBybohrCode(actXET.bohrCode);

            //获取参数表的数据
            List<DevicePara> existPara = new List<DevicePara>();
            jdbc.TableName = "tab_devicepara_info";
            existPara = jdbc.GetDeviceParas();

            if (existPara != null)
            {
                //限定不同扳手编码设置不能相同
                foreach (DevicePara paraLine in existPara)
                {
                    if (textBox2.Text == paraLine.custom_id)
                    {
                        if (MyDevice.languageType == 0)
                        {
                            MessageBox.Show("扳手编码已存在，请重新输入", "输入提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        else
                        {
                            MessageBoxEX.Show("The wrench code already exists, please re-enter it?", "Hint", MessageBoxButtons.OK, new string[] {"OK"});
                        }
                        textBox2.Text = "";
                        return;
                    }
                }
            }

            //按键状态
            rwCnt = 0;
            ucBtnExt1.FillColor = Color.Red;
            ucBtnExt1.BtnText = "Rw" + rwCnt.ToString();

            //写入参数
            nextTask = TASKS.WRITE_PARA;
            MyDevice.protocol.Protocol_ClearState();
            MyDevice.protocol.Protocol_WriteTasks();

            //表格数据
            for (int i = 0; i < 6; i++)
            {
                DevicePara dp = new DevicePara()
                {
                    id = i + 1,
                    bohr_code = actXET.bohrCode,
                    work_id = textBox1.Text,
                    custom_id = textBox2.Text,
                    screw_total = dataGridView1.Rows[i].Cells[1].Value.ToString(),
                    m_value = "M" + (i + 1).ToString(),
                    torque_unit = "N·m",
                    torque_low = ((UInt32)MyDevice.mSUT.TorqueTransNm(Convert.ToDouble(dataGridView1.Rows[i].Cells[3].Value) * 100 + 0.5, unit)).ToString(),
                    torque_up = ((UInt32)MyDevice.mSUT.TorqueTransNm(Convert.ToDouble(dataGridView1.Rows[i].Cells[4].Value) * 100 + 0.5, unit)).ToString(),
                    angle_low = ((UInt16)(Convert.ToDouble(dataGridView1.Rows[i].Cells[7].Value) * 10 + 0.5)).ToString(),
                    angle_up = ((UInt16)(Convert.ToDouble(dataGridView1.Rows[i].Cells[8].Value) * 10 + 0.5)).ToString(),
                };

                para.Add(dp);
            }

            //保存数据list到数据库
            if (para.Count > 0)
            {
                //保存新数据
                jdbc.AddDevicePara(para);
            }
        }

        //清除设备缓存
        private void ucBtnExt2_BtnClick(object sender, EventArgs e)
        {
            //提示
            if ((int)(MyDevice.languageType == 0 ? MessageBox.Show("是否确定清除设备缓存？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) : 
                MessageBoxEX.Show("Are you sure to clear the device cache?", "Hint", MessageBoxButtons.OKCancel, new string[] { "OK", "Cancel" })) != 1) { return; }

            //按键状态
            ucBtnExt2.FillColor = Color.Red;

            //清除设备缓存
            nextTask = TASKS.WRITE_RECSIZE;
            MyDevice.protocol.Protocol_Write_SendCOM(TASKS.WRITE_RECSIZE);
        }

        //改单位更新表格
        private void ucCombox1_TextChangedEvent(object sender, EventArgs e)
        {
            //当前单位
            string old_unit = unit;

            //当前量程
            uint torqueCap = (uint)(Convert.ToDouble(textBox4.Text.Split(' ')[0]) * 100f + 0.5f);

            //更新单位
            switch (ucCombox1.SelectedIndex)
            {
                case 0: unit = "N·m"; break;
                case 1: unit = "lbf·in"; break;
                case 2: unit = "lbf·ft"; break;
                case 3: unit = "kgf·cm"; break;
            }

            //更新量程
            switch (old_unit)
            {
                case "N·m":
                    textBox4.Text = (MyDevice.mSUT.Torque_nmTrans(torqueCap, unit) / 100.0f).ToString() + " " + unit;
                    break;
                case "lbf·in":
                    textBox4.Text = (MyDevice.mSUT.Torque_lbfinTrans(torqueCap, unit) / 100.0f).ToString() + " " + unit;
                    break;
                case "lbf·ft":
                    textBox4.Text = (MyDevice.mSUT.Torque_lbfftTrans(torqueCap, unit) / 100.0f).ToString() + " " + unit;
                    break;
                case "kgf·cm":
                    textBox4.Text = (MyDevice.mSUT.Torque_kgfcmTrans(torqueCap, unit) / 100.0f).ToString() + " " + unit;
                    break;
                default:
                    break;
            }

            //更新表格参数
            UpdateTableMX();
        }

        //数据输入
        private void data_KeyPress(object sender, KeyPressEventArgs e)
        {
            //只允许输入数字,负号,小数点和删除键
            if (((e.KeyChar < '0') || (e.KeyChar > '9')) && (e.KeyChar != '.') && (e.KeyChar != 8))
            {
                e.Handled = true;
                return;
            }

            //小数点只能出现1位
            if ((e.KeyChar == '.') && ((DataGridViewTextBoxEditingControl)sender).Text.Contains("."))
            {
                e.Handled = true;
                return;
            }

            //第一位不能为小数点
            if ((e.KeyChar == '.') && (((DataGridViewTextBoxEditingControl)sender).Text.Length == 0))
            {
                e.Handled = true;
                return;
            }
        }

        //MX表键盘输入拦截
        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            dataGridView1.Focus();
            CellEdit = (DataGridViewTextBoxEditingControl)e.Control;
            CellEdit.KeyPress += data_KeyPress;
        }

        //
        private Boolean getDataCheck(ref UInt32 dat, UInt32 low, UInt32 high)
        {
            if ((dat < low) || (dat > high))
            {
                //提示
                if ((MyDevice.languageType == 0 ? 
                    MessageBox.Show("扭力范围输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation) : 
                    MessageBoxEX.Show("Torque range input error!", "Hint", MessageBoxButtons.OK, new string[] {"OK"})) == DialogResult.OK)
                {
                    dat = (dat > low) ? ((dat > high) ? high : dat) : low;
                    return false;
                }
            }
            return true;
        }

        //
        private Boolean getDataCheck(ref UInt16 dat, UInt16 low, UInt16 high)
        {
            if ((dat < low) || (dat > high))
            {
                //提示
                if ((MyDevice.languageType == 0 ? 
                    MessageBox.Show("角度范围输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation) : 
                    MessageBoxEX.Show("Angle range input error!", "Hint", MessageBoxButtons.OK, new string[] { "OK" })) == DialogResult.OK)
                {
                    dat = (dat > low) ? ((dat > high) ? high : dat) : low;
                    return false;
                }
            }
            return true;
        }

        //委托
        private void receivePara()
        {
            //其它线程的操作请求
            if (this.InvokeRequired)
            {
                try
                {
                    freshHandler meDelegate = new freshHandler(receivePara);
                    this.Invoke(meDelegate, new object[] { });
                }
                catch
                {

                }
            }
            //本线程的操作请求
            else
            {
                switch (nextTask)
                {
                    //保存参数
                    case TASKS.WRITE_PARA:
                        //继续发送
                        rwCnt++;
                        ucBtnExt1.BtnText = "Rw" + rwCnt.ToString();
                        MyDevice.protocol.Protocol_WriteTasks();

                        //所有流程完成后执行
                        if (MyDevice.protocol.trTASK == TASKS.NULL)
                        {
                            //
                            //UpdateTableMX();
                            ucBtnExt1.BtnText = MyDevice.languageType == 0 ? "确 定" : "Confirm";
                            ucBtnExt1.FillColor = Color.Green;
                        }
                        break;

                    //清除缓存数据
                    case TASKS.WRITE_RECSIZE:
                        ucBtnExt2.FillColor = Color.Green;
                        MyDevice.protocol.trTASK = TASKS.NULL;
                        break;

                    default:
                        break;
                }
            }
        }

        //
        private void dataGridView1_Leave(object sender, EventArgs e)
        {
            dataGridView1.Refresh();
        }

        //
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (exePath.IndexOf("Koeorws.exe") != -1)
            {
                //扳手编码不得为空
                if (textBox2.Text == "")
                {
                    KoeorwsShow.isCustomIdNull = true;
                }
            }
            else if (exePath.IndexOf("Torque.exe") != -1)
            {
                //扳手编码不得为空
                if (textBox2.Text == "")
                {
                    TorqueShow.isCustomIdNull = true;
                }
            }
        }

        private void MenuCalPanel_Resize(object sender, EventArgs e)
        {
            autoSize.UIComponetForm_Resize(this);
            splitContainer1.SplitterDistance = this.Width / 5 * 1;
            splitContainer2.SplitterDistance = this.Width / 4 * 1;

            //表格显示6行9列
            if (dataGridView1.Rows.Count <= 6)
            {
                dataGridView1.ColumnHeadersHeight = dataGridView1.Height / 7;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.DefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                    row.Height = dataGridView1.Height / 7;
                }
            }
            else
            {
                for (int i = 6; i < dataGridView1.Rows.Count; i++)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.ControlDark;
                }
            }

            if (dataGridView1.ColumnCount == 9)
            {
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    col.Width = dataGridView1.Width / 9;
                }
            }
        }
    }
}
