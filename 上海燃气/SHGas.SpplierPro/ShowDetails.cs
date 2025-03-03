using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Drawing.Printing;
using System.Collections;

namespace ProviderCS
{
    public partial class frmShowDetails : Form
    {
        string Rukuid;
        string Pid;

        XmlNode _XmlNode;
        public frmShowDetails(XmlNode xn)
        {
            _XmlNode = xn;
            InitializeComponent();
            BindDetail();
        }

        #region 旧方法
        public DataTable GetDetailsInfo()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MeterID", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("Caliber", typeof(decimal));
            dt.Columns.Add("MeterPropertyID", typeof(int));
            dt.Columns.Add("Price", typeof(decimal));
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
                        dr["Price"] = decimal.Parse(_XmlNode.Attributes["Price"].Value).ToString("F2");
                        //dr["MeterPropertyID"] = 22;
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
        #endregion

        
        #region 绑定明细Grid
        //绑定明细Grid
        public void BindDetail()
        {
            //PrintContent.DataSource = GetDetailsInfo();
            //PrintContent.DataBind();
        }

        #endregion

        private void Print_MouseHover(object sender, EventArgs e)
        {
            btnPrint.Image = global::ProviderCS.Properties.Resources.BtnPrint_on;
        }

        private void Print_MouseLeave(object sender, EventArgs e)
        {
            btnPrint.Image = global::ProviderCS.Properties.Resources.btnPrint_normal;
        }

        private void frmShowDetails_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }
        

        /// <summary>
        /// DataGridView转换为2进制
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="includeColumnText"></param>
        /// <returns></returns>
        public static string[,] ToStringArray(DataGridView dataGridView, bool includeColumnText)
        {
            #region 实现...

            string[,] arrReturn = null;

            int rowsCount = dataGridView.Rows.Count;
            int colsCount = dataGridView.Columns.Count;

            if (rowsCount > 0)
            {
                //最后一行是供输入的行时，不用读数据。
                if (dataGridView.Rows[rowsCount - 1].IsNewRow)
                {
                    rowsCount--;
                }
            }

            int i = 0;

            //包括列标题
            if (includeColumnText)
            {
                rowsCount++;
                arrReturn = new string[rowsCount, colsCount];
                for (i = 0; i < colsCount; i++)
                {
                    arrReturn[0, i] = dataGridView.Columns[i].HeaderText;
                }

                i = 1;
            }
            else
            {
                arrReturn = new string[rowsCount, colsCount];
            }

            //读取单元格数据
            int rowIndex = 0;
            for (; i < rowsCount; i++, rowIndex++)
            {
                for (int j = 0; j < colsCount; j++)
                {
                    arrReturn[i, j] = dataGridView.Rows[rowIndex].Cells[j].FormattedValue.ToString();
                }
            }

            return arrReturn;

            #endregion 实现
        }



        /// <summary>
        /// 打印按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Print_Click(object sender, EventArgs e)
        {
            //misGoldPrinter.Bottom = "打印日期：" + System.DateTime.Now.ToLongDateString();	//结尾，说明同抬头        
            //misGoldPrinter.Title = "明细";
            //Printer print = new Printer();
            //print.PrintDocument =
            //    //PrintContent.DataSource = GetDetailsInfo();
            //    // misGoldPrinter.DataSource =ToStringArray(this.PrintContent,true);							//DataGrid作为数据源

            ////打印的核心是Body，可以对它设置字体、列宽等等
            //((GoldPrinter.Body)(misGoldPrinter.Body)).IsAverageColsWidth = true;//指明平均列

            //misGoldPrinter.Caption = PrintContent;
            //if (false)
            //{
            //    misGoldPrinter.Print();											//打印
            //}
            //else
            //{
            //    misGoldPrinter.Preview();										//预览
            //}
            //PrintDocument document = new PrintDocument();
            //this.printPreviewDialog1 = new PrintPreviewDialog();


            //this.printPreviewDialog1.ClientSize = new System.Drawing.Size(this.Size.Width, this.Size.Height);

            //this.printPreviewDialog1.Location = new System.Drawing.Point(29, 29);

            //this.printPreviewDialog1.Text = "打印预览";

            //document.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(document_PrintPage);

            //printPreviewDialog1.Document = document;
            //printPreviewDialog1.ShowDialog();
        }

        private void document_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            //Bitmap tempmap = new Bitmap(this.Size.Width, this.Size.Height);
            //this.PrintContent.DrawToBitmap(tempmap, this.PrintContent.ClientRectangle);
            ////Graphics tempg = Graphics.FromImage(tempmap);

            //Graphics Realg = this.CreateGraphics();
            //Realg.DrawImage(tempmap, 0, 0);
            //e.Graphics.DrawImage(tempmap, 100, 100);

            ////tempg.Dispose();
            //tempmap.Dispose();
            //Realg.Dispose();


        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnClose_MouseHover(object sender, EventArgs e)
        {
            btnClose.Image = global::ProviderCS.Properties.Resources.BtnClose_on1;
        }

        private void btnClose_MouseLeave(object sender, EventArgs e)
        {
            btnClose.Image = global::ProviderCS.Properties.Resources.btnClose_Close1;
        }

    }
}