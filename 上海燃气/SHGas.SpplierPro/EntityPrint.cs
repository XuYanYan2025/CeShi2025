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


        private string id;      //�ͺ�
        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        private string emergy;  //����
        public string Emergy
        {
            get { return emergy; }
            set { emergy = Emergy; }
        }

        //private string price;       //�۸�
        //public string Price
        //{
        //    get { return price; }
        //    set { price = Price; }
        //}

        private string boxID;       //���
        public string BoxID
        {
            get { return boxID; }
            set { boxID = BoxID; }
        }

        private string meterID; //���
        public string MeterID
        {
            get { return meterID; }
            set { meterID = MeterID; }
        }

        private int ncount;        //ÿ����
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


        //    //��źͱ�Ų�ͬ�ı�
        //    #region С��
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
        //            sb.Append("���ͺ�;" + dr["Type"].ToString() + "   ");
        //            sb.Append("������;" + dr["Caliber"].ToString() + "   ");
        //            sb.Append("����;" + dr["Price"].ToString() + "   ");
        //            sb.Append("���;" + dr["BoxID"].ToString() + "   ");
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

        //    #region ���
        //    //����
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
        //            sb.Append("���ͺ�;" + dr["Type"].ToString() + "   ");
        //            sb.Append("������;" + dr["Caliber"].ToString() + "   ");
        //            sb.Append("����;" + dr["Price"].ToString() + "   ");
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
                        dr["ID"] = "���ͺ�:" + dr2["Type"].ToString();
                        dr["Emergy"] = "������:" + dr2["Caliber"].ToString();
                        //dr["Price"] = "����:" + dr2["Price"].ToString();
                        dr["BoxID"] = "���:" + dr2["BoxID"].ToString();
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
                        dr["ID"] = "���ͺ�:" + dr1["Type"].ToString();
                        dr["Emergy"] = "������:" + dr1["Caliber"].ToString();
                        //dr["Price"] = "����:" + dr1["Price"].ToString();
                    }
                    dr["MeterID"] = dr1["MeterID"].ToString() + "   ";
                    dt1.Rows.Add(dr);
                }
            return dt1;
            }
        }
    }
}
