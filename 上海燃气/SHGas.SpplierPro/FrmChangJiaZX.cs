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
    public partial class FrmChangJiaZX : Form
    {
        IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + "ProviderXml.sys");
        /// <summary>
        /// 厂家配置文件路径
        /// </summary>
        public string ChangJiaPZXMLFileUrl = string.Empty;
        /// <summary>
        /// 网络选择（-1本地，0内网，1外网）
        /// </summary>
        public int _WangLuoType = 1;

        public FrmChangJiaZX()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cbbChangJia.SelectedValue.ToString() == "-99999")
            {
                DialogResult _DialogResult = MessageBox.Show("未选择厂家，是否直接关闭程序？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (_DialogResult == DialogResult.Yes)
                {
                    System.Environment.Exit(0);
                    return;
                }
                else
                {
                    MessageBox.Show("请先选择厂家再进行后续操作！");
                    return;
                }
            }

            ChangJiaPZXMLFileUrl = cbbChangJia.SelectedValue.ToString();
            this.Close();

        }

        private void FrmChangJiaZX_Load(object sender, EventArgs e)
        {
            if (ini.IniReadValue("JTProviderConfig", "ChangJiaXMLFileUrl") == "")
                ini.IniWriteValue("JTProviderConfig", "ChangJiaXMLFileUrl", "http://212.64.20.54:10086/ProviderDate.asmx");
            if (ini.IniReadValue("JTProviderConfig", "ChangJiaXMLFileWaiWangUrl") == "")
                ini.IniWriteValue("JTProviderConfig", "ChangJiaXMLFileWaiWangUrl", "http://192.168.127.70:10087/ProviderDate_JT.asmx");
            if (ini.IniReadValue("JTProviderConfig", "ChangJiaXMLFileNeiWangUrl") == "")
                ini.IniWriteValue("JTProviderConfig", "ChangJiaXMLFileNeiWangUrl", "http://122.152.213.240:29001/ProviderDate_JT.asmx");

            string PeiZhiUrl = ini.IniReadValue("JTProviderConfig", "ChangJiaXMLFileUrl").ToString().Trim();

            if (_WangLuoType == -1)
            {
                cbbChangJia.DataSource = GetBiaoChangXX();
                cbbChangJia.DisplayMember = "ChangJiaName";
                cbbChangJia.ValueMember = "XMLFileUrl";
            }
            else
            {
                if (_WangLuoType == 0) PeiZhiUrl = ini.IniReadValue("JTProviderConfig", "ChangJiaXMLFileNeiWangUrl").ToString().Trim();
                if (_WangLuoType == 1) PeiZhiUrl = ini.IniReadValue("JTProviderConfig", "ChangJiaXMLFileWaiWangUrl").ToString().Trim();

                ProviderDate _webser = new ProviderDate(PeiZhiUrl);
                DataTable cjxx = _webser.GetBiaoChangXX();

                if (cjxx != null && cjxx.Rows.Count > 0)
                {
                    cbbChangJia.DataSource = cjxx;
                    cbbChangJia.DisplayMember = "ChangJiaName";
                    cbbChangJia.ValueMember = "XMLFileUrl";
                }
                else
                {
                    MessageBox.Show("云端厂家信息获取失败，启用本地模式！", "云端数据", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cbbChangJia.DataSource = GetBiaoChangXX();
                    cbbChangJia.DisplayMember = "ChangJiaName";
                    cbbChangJia.ValueMember = "XMLFileUrl";

                    //System.Environment.Exit(0);
                }
            }

        }


        /// <summary>
        /// 本地模式
        /// </summary>
        /// <returns></returns>
        public System.Data.DataTable GetBiaoChangXX()
        {
            #region 创建表厂清单列表DataTable
            System.Data.DataTable ChangJiaXX = new System.Data.DataTable("ChangJiaXX");

            System.Data.DataColumn dc = null;
            dc = ChangJiaXX.Columns.Add("ID", Type.GetType("System.Int32"));
            dc.AutoIncrement = true;//自动增加
            dc.AutoIncrementSeed = 1;//起始为1
            dc.AutoIncrementStep = 1;//步长为1
            dc.AllowDBNull = false;//

            dc = ChangJiaXX.Columns.Add("ChangJiaName", Type.GetType("System.String"));
            dc = ChangJiaXX.Columns.Add("ChangJiaBH", Type.GetType("System.String"));
            dc = ChangJiaXX.Columns.Add("XMLFileUrl", Type.GetType("System.String"));
            #endregion

            #region 添加数据

            System.Data.DataRow newRow;

            newRow = ChangJiaXX.NewRow();
            newRow["ChangJiaName"] = " ---请选择--- ";
            newRow["ChangJiaBH"] = "-99999";
            newRow["XMLFileUrl"] = "-99999";
            ChangJiaXX.Rows.Add(newRow);

            newRow = ChangJiaXX.NewRow();
            newRow["ChangJiaName"] = "本地配置";
            newRow["ChangJiaBH"] = "-8888";
            newRow["XMLFileUrl"] = "MeterType.xml";
            ChangJiaXX.Rows.Add(newRow);

            #endregion

            return ChangJiaXX;
        }

    }
}


