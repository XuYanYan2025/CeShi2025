using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.DataAccess.Client;
using Platform.Data;

namespace DetailScreen
{
    public class ChartData
    {
        string conn = "Data Source=(DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST =172.30.0.145)(PORT = 1521))) (CONNECT_DATA = (SERVICE_NAME = sjkfdb)));User Id=sjgasuser;Password=pass;Min Pool Size=10;Connection Lifetime=120;Connection Timeout=60;Incr Pool Size=5;Decr Pool Size=2;";

        public DataSet SHIGONGXXChart(int gongchenglx)
        {
            string spName = "CHAXUNFX_SHIGONGXXChart";
            OracleParameter[] storedParams = OracleHelperParameterCache.GetSpParameterSet(conn, spName);
            storedParams[0].Value = gongchenglx;
            return OracleHelper.ExecuteDataset(conn, CommandType.StoredProcedure, spName, storedParams);
        }

        public DataTable JIEDIANChart()
        {
            string spName = "CHAXUNFX_JIEDIANChart";
            OracleParameter[] storedParams = OracleHelperParameterCache.GetSpParameterSet(conn, spName);
            return OracleHelper.ExecuteDataset(conn, CommandType.StoredProcedure, spName, storedParams).Tables[0];
        }
    }
}
