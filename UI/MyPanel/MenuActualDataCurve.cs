using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Base.UI.MyPanel
{
    public partial class MenuActualDataCurve : Form
    {
        List<DataInfo> dataInfo = new List<DataInfo>();     //从实时数据界面获取的该段的实时数据
        List<string> angleActualListStr = new List<string>();
        List<string> torqueActualListStr = new List<string>();
        List<double> angleActualList = new List<double>();
        List<double> torqueActualList = new List<double>();

        public MenuActualDataCurve()
        {
            InitializeComponent();
        }

        public MenuActualDataCurve(List<DataInfo> dataInfoCurve)
        {
            InitializeComponent();

            dataInfo = dataInfoCurve;

            angleActualListStr = dataInfo.Select(d => d.angle_actual).ToList();
            torqueActualListStr = dataInfo.Select(d => d.torque_actual).ToList();

            foreach (string str in angleActualListStr)
            {
                string numericPart = Regex.Match(str, @"[0-9]+(?:\.[0-9]+)?").Value;   // 使用正则表达式提取数值部分
                if (double.TryParse(numericPart, out double value))
                {
                    angleActualList.Add(value);
                }
                else
                {
                    continue;
                }
            }

            foreach (string str in torqueActualListStr)
            {
                string numericPart = Regex.Match(str, @"[0-9]+(?:\.[0-9]+)?").Value;   // 使用正则表达式提取数值部分
                if (double.TryParse(numericPart, out double value))
                {
                    torqueActualList.Add(value);
                }
                else
                {
                    continue;
                }
            }
        }

        private void MenuActualDataCurve_Load(object sender, EventArgs e)
        {
            // chart属性
            Series series1 = chart1.Series[0];
            series1.ChartType = SeriesChartType.Spline;
            series1.BorderWidth = 2;
            series1.Color = System.Drawing.Color.Tomato;  //曲线颜色
            series1.LegendText = MyDevice.languageType == 0 ? "角度值" : "Angle";


            Series series2 = chart2.Series[0];
            series2.ChartType = SeriesChartType.Spline;
            series2.BorderWidth = 2;
            series2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));   //曲线颜色
            series2.LegendText = MyDevice.languageType == 0 ? "扭力值" : "Torque";

            // 在chart中显示数据
            int xAngle = 0;
            foreach (float v in angleActualList)
            {
                series1.Points.AddXY(xAngle, v);
                xAngle++;
            }

            int xTorque = 0;
            foreach (float v in torqueActualList)
            {
                series2.Points.AddXY(xTorque, v);
                xTorque++;
            }

            // 设置坐标轴和背景
            chart1.BackColor = System.Drawing.Color.Ivory;
            ChartArea chartAreaAngle = chart1.ChartAreas[0];
            chartAreaAngle.BackColor = System.Drawing.Color.Ivory;
            chartAreaAngle.AxisX.Minimum = 0;
            chartAreaAngle.AxisX.Maximum = System.Double.NaN;
            chartAreaAngle.AxisY.Minimum = 0d;
            chartAreaAngle.AxisY.Maximum = System.Double.NaN;
            chartAreaAngle.AxisX.MajorGrid.Enabled = false;
            chartAreaAngle.AxisY.MajorGrid.Enabled = true;
            chartAreaAngle.AxisX.LabelStyle.Enabled = false;
            chartAreaAngle.AxisY.LineColor = System.Drawing.Color.Gray;
            chartAreaAngle.AxisX.MajorGrid.LineColor = System.Drawing.Color.Gray;
            chartAreaAngle.AxisY.MajorGrid.LineColor = System.Drawing.Color.Gray;
            chartAreaAngle.AxisX.MajorTickMark.Size = 0;


            chart2.BackColor = System.Drawing.Color.Honeydew;
            ChartArea chartAreaTorque = chart2.ChartAreas[0];
            chartAreaTorque.BackColor = System.Drawing.Color.Honeydew;
            chartAreaTorque.AxisX.Minimum = 0;
            chartAreaTorque.AxisX.Maximum = System.Double.NaN;
            chartAreaTorque.AxisY.Minimum = 0d;
            chartAreaTorque.AxisY.Maximum = System.Double.NaN;
            chartAreaTorque.AxisX.MajorGrid.Enabled = false;
            chartAreaTorque.AxisY.MajorGrid.Enabled = true;
            chartAreaTorque.AxisX.LabelStyle.Enabled = false;
            chartAreaTorque.AxisY.LineColor = System.Drawing.Color.Gray;
            chartAreaTorque.AxisX.MajorGrid.LineColor = System.Drawing.Color.Gray;
            chartAreaTorque.AxisY.MajorGrid.LineColor = System.Drawing.Color.Gray;
            chartAreaTorque.AxisX.MajorTickMark.Size = 0;

            this.MaximumSize = new System.Drawing.Size((int)(Screen.PrimaryScreen.WorkingArea.Width * 0.75), (int)(Screen.PrimaryScreen.WorkingArea.Height * 0.75));
            this.panel2.Height = this.MaximumSize.Height / 2;
            this.panel1.Height = this.MaximumSize.Height / 2;
            this.chart1.Height = this.panel1.Height;
            this.chart2.Height = this.panel2.Height;
        }
    }
}
