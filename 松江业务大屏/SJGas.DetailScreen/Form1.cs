using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraCharts;
using System.Threading;

namespace DetailScreen
{
    public partial class Form1 : Form
    {
        DataTable HuWuFenShiDT = new DataTable();
        private DataSet data;
        string conStr = "Server='172.20.16.223';Initial catalog=Calldb_v15;UID='sa';PWD='123456sa';Max Pool Size=512;";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            //this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            //InitDataGridView();
            //BuilderDevChart();
            PL_HuaWu.Top = PL_HuaWu.Left = PL_YeWu.Top = PL_YeWu.Left = PL_JieDian.Top = PL_JieDian.Left = 0;

            PL_YeWu.Visible = true;
            //PL_YeWu.Visible = false;
            //PL_JieDian.Visible = true;
            timer1.Interval = 15000;
            //timer1.Interval = 3000;
            timer1.Start();
            //timer2.Interval = 20000;
            //timer2.Start();
            //timer3.Interval = 10000;
            //timer3.Start();
        }

        private void InitDataGridView()
        {
            dataGridView1.Columns.Clear();
            DataGridViewColumn dc0 = new DataGridViewTextBoxColumn();
            dc0.Width = 10;
            dc0.Name = "";
            dataGridView1.Columns.Add(dc0);

            DataGridViewColumn dc1 = new DataGridViewTextBoxColumn();
            dc1.Width = 180;
            dc1.Name = "日期";
            dataGridView1.Columns.Add(dc1);

            DataGridViewColumn dc2 = new DataGridViewTextBoxColumn();
            dc2.Width = 180;
            dc2.Name = "拨入电话数";
            dataGridView1.Columns.Add(dc2);

            DataGridViewColumn dc3 = new DataGridViewTextBoxColumn();
            dc3.Width = 180;
            dc3.Name = "拨入连接数";
            dataGridView1.Columns.Add(dc3);

            DataGridViewColumn dc4 = new DataGridViewTextBoxColumn();
            dc4.Width = 180;
            dc4.Name = "拨入丢失";
            dataGridView1.Columns.Add(dc4);

            DataGridViewColumn dc5 = new DataGridViewTextBoxColumn();
            dc5.Width = 180;
            dc5.Name = "接电率";
            dataGridView1.Columns.Add(dc5);

            DataGridViewColumn dc6 = new DataGridViewTextBoxColumn();
            dc6.Width = 180;
            dc6.Name = "拨出电话数";
            dataGridView1.Columns.Add(dc6);

            HuWuFenShiDT = GetHuaWuHour();

            dataGridView1.VirtualMode = false;
            dataGridView1.Rows.Clear();
            dataGridView1.ClearSelection();
            dataGridView1.RowCount = HuWuFenShiDT.Rows.Count;
            dataGridView1.VirtualMode = true;
            dataGridView1.Invalidate();
        }

        private DataTable CreateChartData(int gclx)
        {
            ChartData chartinfo = new ChartData();
            //int gclx = 1;
            DataSet ds = chartinfo.SHIGONGXXChart(gclx);
            DataTable table = new DataTable("Table1");
            table.Columns.Add("Name", typeof(String));
            table.Columns.Add("Value", typeof(int));
            int sl = Convert.ToInt32(ds.Tables[0].Rows[0][0]);// 已派单 - Convert.ToInt32(ds.Tables[1].Rows[0][0]);//未派单
            int pd = Convert.ToInt32(ds.Tables[1].Rows[0][0]);// -Convert.ToInt32(ds.Tables[1].Rows[0][0]);//未派工
            int wg = Convert.ToInt32(ds.Tables[2].Rows[0][0]);//完工
            //DataRow slrow = table.NewRow();
            string gcstr = string.Empty;
            if (gclx == 16)
            {
                gcstr = "零星换表";
            }
            else if (gclx == 19)
            {
                gcstr = "表具维修";
            }
            else if (gclx == 13)
            {
                gcstr = "一般验收";
            }
            var array1 = new object[] { gcstr + "未派工", sl };
            var array2 = new object[] { gcstr + "已派工", pd };
            var array3 = new object[] { gcstr + "完工", wg };
            table.Rows.Add(array1);
            table.Rows.Add(array2);
            table.Rows.Add(array3);

            return table;
        }

        private DataTable CreateJIEDIANChartData()
        {
            ChartData chartinfo = new ChartData();
            //int gclx = 1;
            DataTable dt = chartinfo.JIEDIANChart();
            DataTable table = new DataTable("Table1");
            table.Columns.Add("Name", typeof(String));
            table.Columns.Add("Value", typeof(int));
            int sl = Convert.ToInt32(dt.Rows[0][0]);//业务
            int pd = Convert.ToInt32(dt.Rows[0][1]);//报修抢修
            int wg = Convert.ToInt32(dt.Rows[0][2]);//咨询
            int aj = Convert.ToInt32(dt.Rows[0][3]);//投诉
            //DataRow slrow = table.NewRow();
            var array1 = new object[] { "业务", sl };
            var array2 = new object[] { "报修抢修", pd };
            var array3 = new object[] { "咨询", wg };
            var array4 = new object[] { "投诉", aj };
            table.Rows.Add(array1);
            table.Rows.Add(array2);
            table.Rows.Add(array3);
            table.Rows.Add(array4);

            return table;
        }

        //public static void SetPiePercentage(this Series series)
        //{
        //    if (series.View is PieSeriesView)
        //    {
        //        //设置为值
        //        //((PiePointOptions)series.PointOptions).PercentOptions.ValueAsPercent = false;
        //        //((PiePointOptions)series.PointOptions).ValueNumericOptions.Format = NumericFormat.Number;
        //        //((PiePointOptions)series.PointOptions).ValueNumericOptions.Precision = 0;
        //        //设置为百分比
        //        ((PiePointOptions)series.PointOptions).PercentOptions.ValueAsPercent = true;
        //        ((PiePointOptions)series.PointOptions).ValueNumericOptions.Format = NumericFormat.Percent;
        //        ((PiePointOptions)series.PointOptions).ValueNumericOptions.Precision = 0;
        //    }
        //}

        private void BuilderDevChart()
        {
            chartControl1.Series.Clear();
            chartControl2.Series.Clear();
            chartControl3.Series.Clear();
            chartControl4.Series.Clear();

            
            Series _pieSeries = new Series("零星调表", ViewType.Pie);
            
            _pieSeries.ValueDataMembers[0] = "Value";
            _pieSeries.ArgumentDataMember = "Name";
            _pieSeries.ShowInLegend = true;
            int gclx = 16;//零星调表
            _pieSeries.DataSource = CreateChartData(gclx);

            PieSeriesLabel label = (PieSeriesLabel)_pieSeries.Label;
            label.Position = PieSeriesLabelPosition.Inside;
            label.Font = new System.Drawing.Font("宋体", 18.0f);

            //_pieSeries.SetLablePosition(PieSeriesLabelPosition.TwoColumns);
            //_pieSeries.Label.  PieSeriesLabelPosition.Inside;

            chartControl1.Series.Add(_pieSeries);
            //_pieSeries.ValueScaleType = ScaleType.Numerical;
            //_pieSeries.SetPiePercentage();
            ((PiePointOptions)_pieSeries.PointOptions).PercentOptions.ValueAsPercent = false;
            ((PiePointOptions)_pieSeries.PointOptions).ValueNumericOptions.Format = NumericFormat.Number;
            ((PiePointOptions)_pieSeries.PointOptions).ValueNumericOptions.Precision = 0;
            _pieSeries.LegendPointOptions.PointView = PointView.ArgumentAndValues;

            Series _pieSeries1 = new Series("表具维修", ViewType.Pie);
            _pieSeries1.ValueDataMembers[0] = "Value";
            _pieSeries1.ArgumentDataMember = "Name";
            int gclx1 = 19;//表具维修
            _pieSeries1.DataSource = CreateChartData(gclx1);
            PieSeriesLabel wxlabel = (PieSeriesLabel)_pieSeries1.Label;
            wxlabel.Position = PieSeriesLabelPosition.Inside;
            wxlabel.Font = new System.Drawing.Font("宋体", 18.0f);

            chartControl2.Titles.Clear();
            ChartTitle bjtitle = new ChartTitle();
            bjtitle.Text = "前一天施工完成情况";
            bjtitle.Font = new System.Drawing.Font("宋体", 22.0f);
            chartControl2.Titles.Add(bjtitle);


            chartControl2.Series.Add(_pieSeries1);
            //_pieSeries.SetPiePercentage();
            ((PiePointOptions)_pieSeries1.PointOptions).PercentOptions.ValueAsPercent = false;
            ((PiePointOptions)_pieSeries1.PointOptions).ValueNumericOptions.Format = NumericFormat.Number;
            ((PiePointOptions)_pieSeries1.PointOptions).ValueNumericOptions.Precision = 0;
            _pieSeries1.LegendPointOptions.PointView = PointView.ArgumentAndValues;
            


            Series _pieSeries2 = new Series("一般验收", ViewType.Pie);
            _pieSeries2.ValueDataMembers[0] = "Value";
            _pieSeries2.ArgumentDataMember = "Name";
            int gclx2 = 13;//一般验收
            _pieSeries2.DataSource = CreateChartData(gclx2);

            PieSeriesLabel yslabel = (PieSeriesLabel)_pieSeries2.Label;
            yslabel.Position = PieSeriesLabelPosition.Inside;
            yslabel.Font = new System.Drawing.Font("宋体", 18.0f);

            chartControl3.Series.Add(_pieSeries2);
            //chartControl3.RuntimeHitTesting = true;
            //_pieSeries.SetPiePercentage();
            ((PiePointOptions)_pieSeries2.PointOptions).PercentOptions.ValueAsPercent = false;
            ((PiePointOptions)_pieSeries2.PointOptions).ValueNumericOptions.Format = NumericFormat.Number;
            ((PiePointOptions)_pieSeries2.PointOptions).ValueNumericOptions.Precision = 0;
            _pieSeries2.LegendPointOptions.PointView = PointView.ArgumentAndValues;
            //_pieSeries.ArgumentScaleType = ScaleType.Qualitative;
            //_pieSeries.ArgumentDataMember = "Name";
            //_pieSeries.ValueScaleType = ScaleType.Numerical;
            //_pieSeries.ValueDataMembers.AddRange(new string[] { "Value" });
            //chartControl1.Series.Add(_pieSeries);
            //chartControl1.Series.Add(_pieSeries1);
            //chartControl1.Series.Add(_pieSeries2);

            Series _pieSeries3 = new Series("当日来电业务", ViewType.Pie);//Bar
            _pieSeries3.ValueDataMembers[0] = "Value";
            _pieSeries3.ArgumentDataMember = "Name";
            _pieSeries3.DataSource = CreateJIEDIANChartData();

            PieSeriesLabel tjlabel = (PieSeriesLabel)_pieSeries3.Label;
            tjlabel.Position = PieSeriesLabelPosition.Inside;
            tjlabel.Font = new System.Drawing.Font("宋体", 20.0f);
            
            chartControl4.Titles.Clear();
            ChartTitle tjtitle = new ChartTitle();
            tjtitle.Text = "当日来电业务";
            tjtitle.Font = new System.Drawing.Font("宋体", 28.0f);
            chartControl4.Titles.Add(tjtitle);
            chartControl4.Series.Add(_pieSeries3);
            ((PiePointOptions)_pieSeries3.PointOptions).PercentOptions.ValueAsPercent = false;
            ((PiePointOptions)_pieSeries3.PointOptions).ValueNumericOptions.Format = NumericFormat.Number;
            ((PiePointOptions)_pieSeries3.PointOptions).ValueNumericOptions.Precision = 0;

            _pieSeries3.LegendPointOptions.PointView = PointView.ArgumentAndValues;
            _pieSeries3.Label.PointOptions.PointView = PointView.ArgumentAndValues;

            //_pieSeries.Label.Font = new Font("宋体", 8);
            //_pieSeries.Label.LineLength = 50;
            //_pieSeries1.Label.Font = new Font("宋体", 8);
            //_pieSeries1.Label.LineLength = 50;
            //_pieSeries2.Label.Font = new Font("宋体", 8);
            //_pieSeries2.Label.LineLength = 50;
            //_pieSeries3.Label.Font = new Font("宋体", 8);
            //_pieSeries3.Label.LineLength = 50; 
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (PL_HuaWu.Visible)//2th
            {
                BuilderDevChart();
                PL_HuaWu.Visible = false;
                PL_YeWu.Visible = true;
                PL_JieDian.Visible = false;
                this.Text = "前一天施工完成情況";
            }
            else if (PL_YeWu.Visible)//3th
            {
                BuilderDevChart();
                PL_HuaWu.Visible = false;
                PL_YeWu.Visible = false;
                PL_JieDian.Visible = true;
                this.Text = "当日来电业务";
            }
            else if (PL_JieDian.Visible)//1th
            {
                InitDataGridView();
                PL_HuaWu.Visible = true;
                PL_YeWu.Visible = false;
                PL_JieDian.Visible = false;
                this.Text = "前一天施工完成情況";
            }
        }


        private void timer2_Tick(object sender, EventArgs e)
        {
            Thread.Sleep(20000);
            HuWuFenShiDT = GetHuaWuHour();

            dataGridView1.VirtualMode = false;
            dataGridView1.Rows.Clear();
            dataGridView1.ClearSelection();
            dataGridView1.RowCount = HuWuFenShiDT.Rows.Count;
            dataGridView1.VirtualMode = true;
            dataGridView1.Invalidate();
            if (PL_YeWu.Visible)
            {
                PL_HuaWu.Visible = false;
                PL_YeWu.Visible = false;
                PL_JieDian.Visible = true;
                this.Text = "前一天施工完成情況";
            }
            else
            {
                PL_HuaWu.Visible = false;
                PL_YeWu.Visible = true;
                PL_JieDian.Visible = false;
                this.Text = "前一天施工完成情況";
            }
        }

        private DataTable GetHuaWuHour()
        {
            try
            {
                SqlConnection con = new SqlConnection(conStr);
                con.Open();
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.Connection = con;
                sqlcmd.CommandText = @"SELECT top 17 STime,A101,A102,A111,
                                        case when A101>0 then convert(varchar,round(A102/convert(float,A101)*100,2))+'%' else '0.00%' end JDL,
                                        A201
                                        FROM [Calldb_v15].[dbo].[CenterDataStat]
                                        ORDER BY STime DESC ";
                SqlDataAdapter sda = new SqlDataAdapter(sqlcmd);
                data = new DataSet();
                sda.Fill(data);
                con.Close();
                sda.Dispose();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.StackTrace);
            }

            return data.Tables[0];
        }

        private void dataGridView1_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1)
                return;

            try
            {

                switch (e.ColumnIndex)
                {
                    case 1:
                        e.Value = HuWuFenShiDT.Rows[e.RowIndex]["STime"].ToString();
                        break;
                    case 2:
                        e.Value = HuWuFenShiDT.Rows[e.RowIndex]["A101"].ToString();
                        break;
                    case 3:
                        e.Value = HuWuFenShiDT.Rows[e.RowIndex]["A102"].ToString();
                        break;
                    case 4:
                        e.Value = HuWuFenShiDT.Rows[e.RowIndex]["A111"].ToString();
                        break;
                    case 5:
                        e.Value = HuWuFenShiDT.Rows[e.RowIndex]["JDL"].ToString();
                        break;
                    case 6:
                        e.Value = HuWuFenShiDT.Rows[e.RowIndex]["A201"].ToString();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("刷新GridView控件显示时报错。错误信息：" + ex.Message);
            }
        }

    }
}
