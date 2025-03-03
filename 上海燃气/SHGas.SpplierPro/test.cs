using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.Collections;
using System.Xml;

namespace ProviderCS
{
    public partial class test : Form
    {
        XmlNode _XmlNode;
        public test(XmlNode xn)
        {
            _XmlNode = xn;
            InitializeComponent();
        }

        public int num = 4;

        private void test_Load(object sender, EventArgs e)
        {

            this.reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;
            localReport.ReportPath = Application.StartupPath + @"\test.rdlc";

            #region test code
            //ReportDataSource dsSalesOrder = new ReportDataSource();

            //DataSet dataset = SystemFramework.Data.SqlHelper.ExecuteDataset(@"Data Source=serveryw\ywsql;Initial Catalog=DZGasK2;User ID=sa;Password=dzsql-1003", CommandType.Text,
            //    "select FilledBy +'Aaaa  \r\n' +'asdfdsa' as FilledBy from share_userInformation");
            //dsSalesOrder.Name = "DZGasK2DataSet_Share_UserInformation";
            //dsSalesOrder.Value = dataset.Tables[0];
            //this.bindingSource1.DataSource = medcialCase;
            //ReportDataSource reportDataSource = new ReportDataSource();
            //reportDataSource.
            //this.reportViewer1.LocalReport.DataSources.Add(reportDataSource);     
            //this.reportViewer1.RefreshReport();
            #endregion

            ReportDataSource dsCustomers = new ReportDataSource();
            List<MedcialCase> m_products = new List<MedcialCase>();

            MedcialCase medicalCase = new MedcialCase();
            medicalCase.DataSource = GetDetailsInfo();
            DataTable dt = medicalCase.DataDB();

            #region 大表情况
            int bcount = 0;
            int bnum = 0;
            if (dt.Columns.Count == 3)
            {
                int count = 1;
                for (int i = 1; i <= dt.Rows.Count - 1; i++)
                {
                    dt.Rows[0]["MeterID"] += "" + dt.Rows[i]["MeterID"].ToString();
                    count++;
                }

                if(bcount == 0)
                {
                    bcount = count;
                    if (bcount % 8 == 0)
                    {
                        bnum = (bcount / 8);
                    }
                    if (bcount % 8 != 0)
                    {
                        bnum = (bcount / 8) + 1;
                    }
                }

                m_products.Add(new MedcialCase(dt.Rows[0]["ID"].ToString(), dt.Rows[0]["Emergy"].ToString(),// dt.Rows[0]["Price"].ToString(),
                    dt.Rows[0]["MeterID"].ToString(),bnum));
            }
            #endregion

            #region 小表情况
            else
            {
                string tmp = "";
                ArrayList al = new ArrayList();
                for (int i = 1; i <= dt.Rows.Count - 1; i++)
                {
                    if (dt.Rows[i]["BoxID"].ToString() != tmp)
                    {
                        al.Add(dt.Rows[i]["BoxID"].ToString());
                        tmp = dt.Rows[i]["BoxID"].ToString();
                    }

                }
                int scount = 0;
                int snum = 0;
                foreach (object box in al)
                {

                    DataRow[] dr_arr = dt.Select("boxid='" + box.ToString() + "'");
                    int count = 1;
                    for (int n = 1; n <= dr_arr.Length - 1; n++)
                    {

                        dr_arr[0]["MeterID"] += "" + dr_arr[n]["MeterID"].ToString();
                        count++;
                    }
                    //判断行数
                    if (scount == 0)
                    {
                        scount = count;
                        if(scount % 8 == 0)
                        {
                            snum = (scount / 8);
                        }
                        if(scount % 8 != 0)
                        {
                            snum = (scount / 8) + 1;
                        }
                    }
                   m_products.Add(new MedcialCase(dr_arr[0]["ID"].ToString(), dr_arr[0]["Emergy"].ToString(), //dr_arr[0]["Price"].ToString(), 
                       dr_arr[0]["BoxID"].ToString(), dr_arr[0]["MeterID"].ToString(),snum));
                }
            }
            #endregion

            dsCustomers.Name = "ProviderCS_MedcialCase";
            dsCustomers.Value = m_products;

            localReport.DataSources.Add(dsCustomers);
            reportViewer1.RefreshReport();
        }
        /// <summary>
        /// 将XML文件转换成DataTable
        /// </summary>
        /// <returns></returns>
        public DataTable GetDetailsInfo()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MeterID", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("Caliber", typeof(decimal));
            dt.Columns.Add("MeterPropertyID", typeof(int));
            //dt.Columns.Add("Price", typeof(decimal));
            dt.Columns.Add("BoxID", typeof(string));

            XmlNodeList xnl = _XmlNode.SelectNodes("Box");

            foreach (XmlNode xnType in xnl)
            {
                XmlNodeList xnl2 = xnType.SelectNodes("Meter");

                foreach (XmlNode xnType2 in xnl2)
                {
                    try
                    {
                        DataRow dr = dt.NewRow();
                        dr["MeterID"] = xnType2.Attributes["No"].Value;
                        dr["Type"] = _XmlNode.Attributes["Type"].Value;
                        dr["Caliber"] = _XmlNode.Attributes["Energy"].Value;
                        dr["BoxID"] = xnType.Attributes["No"].Value;
                        //dr["Price"] = decimal.Parse(_XmlNode.Attributes["Price"].Value).ToString("F2");
                        dt.Rows.Add(dr);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
            return dt;
        }

        private void reportViewer1_Load(object sender, EventArgs e)
        {

        }
    }
}