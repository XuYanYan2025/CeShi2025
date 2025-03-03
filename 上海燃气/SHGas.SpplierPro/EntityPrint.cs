using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;

namespace ProviderCS
{
    public class MedcialCase
    {
        public MedcialCase()
        { 
        
        }
        public MedcialCase(string sb,string e,//string p,
            string b,string m,int n) 
        {
            id = sb;
            emergy = e;
            //price = p;
            boxID = b;
            meterID = m;
            ncount = n;
        }

        public MedcialCase(string sb, string e, //string p,
            string m,int n)
        {
            id = sb;
            emergy = e;
            //price = p;
           // boxID = b;
            meterID = m;
            ncount = n;
        }


        private string id;      //型号
        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        private string emergy;  //能量
        public string Emergy
        {
            get { return emergy; }
            set { emergy = Emergy; }
        }

        //private string price;       //价格
        //public string Price
        //{
        //    get { return price; }
        //    set { price = Price; }
        //}

        private string boxID;       //箱号
        public string BoxID
        {
            get { return boxID; }
            set { boxID = BoxID; }
        }

        private string meterID; //表号
        public string MeterID
        {
            get { return meterID; }
            set { meterID = MeterID; }
        }

        private int ncount;        //每箱数
        public int Ncount      
        {
            get { return ncount; }
            set { ncount = Ncount; }
        }

        private DataTable datasource;

        public DataTable DataSource
        {
            get
            {
                return datasource;
            }
            set
            {
                datasource = value;
            }
        }

        //#region old code
        //public string DataBind()
        //{
        //    StringBuilder sb = new StringBuilder();

        //    DataRow[] drs1 = datasource.Select("MeterID=BoxID", "MeterPropertyID desc,BoxID asc");
        //    DataRow[] drs2 = datasource.Select("MeterID<>BoxID", "MeterPropertyID desc,BoxID asc");


        //    //箱号和表号不同的表
        //    #region 小表
        //    ArrayList listExitBox = new ArrayList();
        //    ArrayList listExitPro = new ArrayList();
        //    int j = 0;
        //    int z = 0;
        //    foreach (DataRow dr in drs2)
        //    {

        //        if (listExitBox.IndexOf(dr["BoxID"].ToString()) == -1)
        //        {
        //            listExitBox.Add(dr["BoxID"].ToString());


        //            if (z != 0)
        //            {
        //                for (int a = j + 1; a <= 8; a++)
        //                {
        //                    sb.Append("   ");
        //                }

        //                sb.Append("\r\n");
        //            }

        //            //if (listExitPro.IndexOf(dr["MeterPropertyID"].ToString()) == -1)
        //            //{
        //            //    listExitPro.Add(dr["MeterPropertyID"].ToString());
        //            //    sb.Append("\r\n");
        //            //    z = 0;
        //            //}

        //            z++;
        //            sb.Append("表型号;" + dr["Type"].ToString() + "   ");
        //            sb.Append("表能量;" + dr["Caliber"].ToString() + "   ");
        //            sb.Append("表单价;" + dr["Price"].ToString() + "   ");
        //            sb.Append("箱号;" + dr["BoxID"].ToString() + "   ");
        //            sb.Append("\r\n");
        //            j = 0;
        //        }

        //        if (j == 8)
        //        {
        //            sb.Append("\r\n");
        //            j = 0;
        //        }
        //        j++;

        //        sb.Append("   ");
        //        sb.Append(dr["MeterID"].ToString());
        //        sb.Append("   ");
              
        //    }
            
        //    if (z != 0)
        //    {
        //        for (int a = j + 1; a <= 8; a++)
        //        {
        //            sb.Append("   ");
        //        }

        //        sb.Append("\r\n");
        //    }
        //    z++;
        //    #endregion

        //    #region 大表
        //    //大表绑定
        //    ArrayList listExit = new ArrayList();
        //    int i = 0;
        //    int b = 0;
        //    foreach (DataRow dr in drs1)
        //    {
        //        if (listExit.IndexOf(dr["MeterPropertyID"].ToString()) == -1)
        //        {
        //            if (b != 0)
        //            {
        //                for (int a = i + 1; a <= 8; a++)
        //                {
        //                    sb.Append("   ");
        //                }

        //                sb.Append("\r\n");
        //            }
        //            b++;
        //            sb.Append("\r\n");
        //            listExit.Add(dr["MeterPropertyID"].ToString());
        //            sb.Append("表型号;" + dr["Type"].ToString() + "   ");
        //            sb.Append("表能量;" + dr["Caliber"].ToString() + "   ");
        //            sb.Append("表单价;" + dr["Price"].ToString() + "   ");
        //            sb.Append("\r\n");
        //            i = 0;

        //        }

        //        if (i == 8)
        //        {
        //            sb.Append("\r\n");
        //            i = 0;
        //        }
        //        i++;
        //        sb.Append("   ");
        //        sb.Append(dr["MeterID"].ToString());
        //        sb.Append("   ");

        //    }
        //    if (b != 0)
        //    {
        //        for (int a = i + 1; a <= 8; a++)
        //        {
        //            sb.Append("   ");
        //        }

        //        sb.Append("\r\n");
        //    }
        //    b++;


        //    return sb.ToString();
        //}
        //    #endregion
        //#endregion 

        public DataTable DataDB()
        {
            DataRow[] drs1 = datasource.Select("MeterID=BoxID", "MeterPropertyID desc,BoxID asc");
            DataRow[] drs2 = datasource.Select("MeterID<>BoxID", "MeterPropertyID desc,BoxID asc");

            if (drs2.Length!=0)
            {
                ArrayList listExitBox = new ArrayList();
                ArrayList listExitPro = new ArrayList();
                DataTable dt2 = new DataTable();
                dt2.Columns.Add("ID", typeof(string));
                dt2.Columns.Add("Emergy", typeof(string));
                //dt2.Columns.Add("Price", typeof(string));
                dt2.Columns.Add("BoxID", typeof(string));
                dt2.Columns.Add("MeterID", typeof(string));
                foreach (DataRow dr2 in drs2)
                {
                    DataRow dr = dt2.NewRow();
                    //if (listExitBox.IndexOf(dr2["BoxID"].ToString()) == -1)
                    {
                        listExitBox.Add(dr2["BoxID"].ToString());
                        dr["ID"] = "表型号:" + dr2["Type"].ToString();
                        dr["Emergy"] = "表能量:" + dr2["Caliber"].ToString();
                        //dr["Price"] = "表单价:" + dr2["Price"].ToString();
                        dr["BoxID"] = "箱号:" + dr2["BoxID"].ToString();
                    }
                    dr["MeterID"] = dr2["MeterID"].ToString() + "   ";
                    dt2.Rows.Add(dr);
                }
                return dt2;
            }
            
            else
            {
                ArrayList listExit = new ArrayList();
                DataTable dt1 = new DataTable();
                dt1.Columns.Add("ID", typeof(string));
                dt1.Columns.Add("Emergy", typeof(string));
                //dt1.Columns.Add("Price", typeof(string));
                dt1.Columns.Add("MeterID", typeof(string));

                foreach (DataRow dr1 in drs1)
                {
                    DataRow dr = dt1.NewRow();
                    if (listExit.IndexOf(dr1["MeterPropertyID"].ToString()) == -1)
                    {
                        listExit.Add(dr1["MeterPropertyID"].ToString());
                        dr["ID"] = "表型号:" + dr1["Type"].ToString();
                        dr["Emergy"] = "表能量:" + dr1["Caliber"].ToString();
                        //dr["Price"] = "表单价:" + dr1["Price"].ToString();
                    }
                    dr["MeterID"] = dr1["MeterID"].ToString() + "   ";
                    dt1.Rows.Add(dr);
                }
            return dt1;
            }
        }
    }
}
