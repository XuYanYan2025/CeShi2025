using Ini;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProviderCS
{
    public partial class FrmNeiWai : Form
    {
        IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + "ProviderXml.sys");
        /// <summary>
        /// 网络选择（-1本地，0内网，1外网）
        /// </summary>
        public int _WangLuoType = 1;

        public FrmNeiWai()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            _WangLuoType = Int32.Parse(cbbWangLuo.SelectedValue.ToString());
            this.Close();
        }

        private void FrmChangJiaZX_Load(object sender, EventArgs e)
        {
            cbbWangLuo.DataSource = GetBiaoChangXX();
            cbbWangLuo.DisplayMember = "WangLuoName";
            cbbWangLuo.ValueMember = "WangLuoBH";

            cbbWangLuo.SelectedValue = 1;
        }

        public System.Data.DataTable GetBiaoChangXX()
        {
            #region 创建网络清单列表DataTable
            System.Data.DataTable WangLuoType = new System.Data.DataTable("WangLuoType");

            System.Data.DataColumn dc = null;
            dc = WangLuoType.Columns.Add("ID", Type.GetType("System.Int32"));
            dc.AutoIncrement = true;//自动增加
            dc.AutoIncrementSeed = 1;//起始为1
            dc.AutoIncrementStep = 1;//步长为1
            dc.AllowDBNull = false;//

            dc = WangLuoType.Columns.Add("WangLuoName", Type.GetType("System.String"));
            dc = WangLuoType.Columns.Add("WangLuoBH", Type.GetType("System.Int32"));
            #endregion

            #region 添加数据

            System.Data.DataRow newRow;

            newRow = WangLuoType.NewRow();
            newRow["WangLuoName"] = "本地";
            newRow["WangLuoBH"] = -1;
            WangLuoType.Rows.Add(newRow);

            newRow = WangLuoType.NewRow();
            newRow["WangLuoName"] = "内网";
            newRow["WangLuoBH"] = 0;
            WangLuoType.Rows.Add(newRow);

            newRow = WangLuoType.NewRow();
            newRow["WangLuoName"] = "外网";
            newRow["WangLuoBH"] = 1;
            WangLuoType.Rows.Add(newRow);

            #endregion

            return WangLuoType;
        }

    }
}


