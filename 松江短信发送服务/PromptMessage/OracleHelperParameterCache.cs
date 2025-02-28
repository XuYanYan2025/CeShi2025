using Oracle.DataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptMessage
{
    public sealed class OracleHelperParameterCache
    {
        private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

        private OracleHelperParameterCache()
        {
        }

        public static void CacheParameterSet(string connectionString, string commandText, params OracleParameter[] commandParameters)
        {
            string str = connectionString + ":" + commandText;
            paramCache[str] = commandParameters;
        }

        private static OracleParameter[] CloneParameters(OracleParameter[] originalParameters)
        {
            OracleParameter[] parameterArray = new OracleParameter[originalParameters.Length];
            int index = 0;
            int length = originalParameters.Length;
            while (index < length)
            {
                if ((originalParameters[index].OracleDbType == OracleDbType.RefCursor) && (originalParameters[index].Direction == ParameterDirection.Output))
                {
                    parameterArray[index] = new OracleParameter();
                    parameterArray[index].OracleDbType = OracleDbType.RefCursor;
                    parameterArray[index].ParameterName = originalParameters[index].ParameterName;
                    parameterArray[index].Direction = ParameterDirection.Output;
                }
                else
                {
                    parameterArray[index] = (OracleParameter)originalParameters[index].Clone();
                    parameterArray[index].CollectionType = originalParameters[index].CollectionType;
                }
                index++;
            }
            return parameterArray;
        }

        private static OracleParameter[] DiscoverSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand command = new OracleCommand(spName, connection))
                {
                    try
                    {
                        connection.Open();
                    }
                    catch
                    {
                        throw new Exception("Could not establish the Oracle Server Connection");
                    }
                    command.CommandType = CommandType.StoredProcedure;
                    OracleCommandBuilder.DeriveParameters(command);
                    OracleParameter[] array = new OracleParameter[command.Parameters.Count];
                    command.Parameters.CopyTo(array, 0);
                    return array;
                }
            }
        }

        public static OracleParameter[] GetCachedParameterSet(string connectionString, string commandText)
        {
            string str = connectionString + ":" + commandText;
            OracleParameter[] parameterArray = (OracleParameter[])paramCache[str];
            if (parameterArray == null)
            {
                return null;
            }
            return parameterArray;
        }

        public static OracleParameter[] GetSpParameterSet(string connectionString, string spName)
        {
            return GetSpParameterSet(connectionString, spName, false);
        }

        public static OracleParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            string str = connectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");
            OracleParameter[] originalParameters = (OracleParameter[])paramCache[str];
            if (originalParameters == null)
            {
                object obj2;
                paramCache[str] = obj2 = DiscoverSpParameterSet(connectionString, spName, includeReturnValueParameter);
                originalParameters = (OracleParameter[])obj2;
            }
            return CloneParameters(originalParameters);
        }
    }
}
