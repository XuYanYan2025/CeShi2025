using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace GeneralEventLog
{

    [StrongNameIdentityPermissionAttribute(SecurityAction.LinkDemand, PublicKey =
         "0024000004800000940000000602000000240000525341310004000001000100491f77bd1eb30e" +
         "2834934bdff2888178cfee9f23104af66348197b62cf3516b20f3cd0748a78f5604a09dd4cb0f5" +
         "aa4bb31ded1a3dc372c7c3a0deb700fcf817a50a33a6416b616d923a406d31352de10b7e99a0fd" +
         "74df8dd527d1c21fa1736c78b3538e46762e19894f111384abc56745a14d4e90ae52ce35a80077" +
         "ed3be4be")]

    public class WriterTextLog
    {

        //记录调试信息日志文件
        public static void WriteDebugEntry(string FunctionName, string LogMessage)
        {
            StreamWriter fs = null;

            //日志文件所在的路径
            string filePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "DebugLog\\";

            //日志文件名称
            string fileName = "DebugLog" + DateTime.Today.ToString("yyyyMMdd") + ".txt";
            try
            {
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                if (!File.Exists(filePath + fileName))
                    fs = File.CreateText(filePath + fileName);
                else
                    fs = File.AppendText(filePath + fileName);
                fs.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "-- (" + FunctionName + ") :" + LogMessage);
            }
            catch
            {
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        //记录运行详细信息日志文件
        public static void WriteDetailEntry(string FunctionName, string LogMessage)
        {
            StreamWriter fs = null;
            string filePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "DetailLog\\";
            string fileName = "DetailLog" + DateTime.Today.ToString("yyyyMMdd") + ".txt";

            try
            {
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                if (!File.Exists(filePath + fileName))
                    fs = File.CreateText(filePath + fileName);
                else
                    fs = File.AppendText(filePath + fileName);
                fs.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "-- (" + FunctionName + ") :" + LogMessage);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        /// <summary>
        /// 系统日志记录进文本文件
        /// </summary>
        /// <param name="FunctionName"></param>
        /// <param name="LogMessage"></param>
        /// <param name="eventType"></param>
        public static void WriteSystemEntry(string FunctionName, string LogMessage, EventLogType eventType)
        {
            StreamWriter fs = null;
            string filePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "LogFiles\\";
            string fileName = "SystemLog" + DateTime.Today.ToString("yyyyMMdd") + ".txt";

            try
            {
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                if (!File.Exists(filePath + fileName))
                    fs = File.CreateText(filePath + fileName);
                else
                    fs = File.AppendText(filePath + fileName);
                fs.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "--" + eventType.ToString() + "-- (" + FunctionName + ") :" + LogMessage);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        /// <summary>
        /// 记录运行详细信息日志文件
        /// </summary>
        /// <param name="LogMessage"></param>
        public static void WriteEntry(string LogMessage)
        {
            WriteEntry(LogMessage, null);
        }
        /// <summary>
        /// 记录运行详细信息日志文件
        /// </summary>
        /// <param name="LogMessage"></param>
        /// <param name="FolderName"></param>
        public static void WriteEntry(string LogMessage, string FolderName)
        {

            #region 文本日志
            StreamWriter fs = null;
            string filePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + DateTime.Today.ToString("yyyyMM") + "\\";
            if (FolderName != string.Empty)
                filePath += (FolderName + "\\");

            string fileName = "Log" + DateTime.Today.ToString("yyyyMMdd") + ".txt";

            try
            {
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                if (!File.Exists(filePath + fileName))
                    fs = File.CreateText(filePath + fileName);
                else
                    fs = File.AppendText(filePath + fileName);
                fs.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " : " + LogMessage);
            }
            catch (Exception ex)
            {
                WriteDetailEntry("OracleLogSystem", "文本日志记录失败！" + ex.Message);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            #endregion
        }

    }
}
