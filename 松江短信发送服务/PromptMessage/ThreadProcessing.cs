using Function;
using GeneralEventLog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.DataAccess.Client;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace PromptMessage
{
    public class ThreadProcessing
    {
        #region 参数配置

        /// <summary>
        /// 服务时间间隔参数
        /// </summary>
        static int searchInterval = 0;
        /// <summary>
        /// Url地址
        /// </summary>
        static string address = "";
        /// <summary>
        /// 
        /// </summary>
        static string txtBT = "上海松江燃气有限公司信息提示";
        /// <summary>
        /// 发送内容
        /// </summary>
        static string txtNR = "";
        /// <summary>
        /// 发送号码(单发)
        /// </summary>
        static string TXNumber = "";
        /// <summary>
        /// 群发号码[尽量控制在10个以内，半角逗号分隔]
        /// </summary>
        static string TXNumbers = "";
        /// <summary>
        /// 记录ID号
        /// </summary>
        static string JLID = "";
        /// <summary>
        /// 群发类型[默认单发]（0单发1群发）
        /// </summary>
        static string QunFaLX = "0";
        /// <summary>
        /// 发送方式：2：号码内容不加密传输；4：号码内容时间加密传输
        /// </summary>
        static int FaSongFS = 4;

        /// <summary>
        /// 重启控制线程
        /// </summary>
        private static Thread _getToMessage_Restart;


        private static JavaScriptSerializer Jss = new JavaScriptSerializer();

        /// <summary>
        /// 数据链接方式（WS:WebServer;LH:Localhost本地直连）
        /// </summary>
        private static string IsLocalhost = ConfigurationManager.AppSettings["IsLocalhost"].ToString().Trim();
        /// <summary>
        /// 消息提示WebService地址
        /// </summary>
        static string MessageWebServiceUrl = ConfigurationManager.AppSettings["MessageDataWebService"].ToString().Trim();
        /// <summary>
        /// 短信数据库链接
        /// </summary>
        static string connSMS = ConfigurationManager.AppSettings["SMSConnString"].ToString().Trim();
        /// <summary>
        /// 短信接口地址
        /// </summary>
        static string SMSUrl = ConfigurationManager.AppSettings["SMSUrl"].ToString().Trim();
        /// <summary>
        /// 短信账号
        /// </summary>
        static string smsaccount = ConfigurationManager.AppSettings["SMSAccount"].ToString().Trim();
        /// <summary>
        /// 短信密码
        /// </summary>
        static string smspassword = ConfigurationManager.AppSettings["SMSPassword"].ToString().Trim();

        #endregion 


        public static void GetToMessageProc()
        {
            if (ConfigurationManager.AppSettings["XinXiStart"].ToString() == "0")
            {
                WriterTextLog.WriteEntry("信息(测试短信)提醒服务不开启！");
                return;
            }

            WriterTextLog.WriteEntry("进入短信发送线程");

            try
            {

                searchInterval = Convert.ToInt32(ConfigurationManager.AppSettings["SearchInterval"]);
                Thread.Sleep(searchInterval * 1000); //此进程非实时延迟开启
                WriterTextLog.WriteEntry("信息提醒服务开启！");
                int FSCS = 0;

                while (true)
                {
                    Thread.Sleep(30000); //调试专用

                    MessageDataService messageDataService = new MessageDataService(MessageWebServiceUrl);

                    DataTable smsMessageDT = new DataTable();
                    DateTime nowtime = DateTime.Now;
                    DateTime startTime = new DateTime();
                    DateTime endTime = new DateTime();
                    TimeSpan ts = new TimeSpan();
                    bool IsChengGong = false;
                    string result = "";
                    string XiangYingNR = "";

                    try
                    {
                        if (ThreadProcessing.IsLocalhost == "WS")
                        {
                            smsMessageDT = messageDataService.GetTX_MessageData(1, 0);
                        }
                        else if (ThreadProcessing.IsLocalhost == "WCF")
                        {

                        }
                        else
                        {
                            using (OracleConnection connecSMS = new OracleConnection(connSMS))
                            {
                                connecSMS.Open();
                                string select_str = "Select * From TX_MESSAGE Where I_FASONGLX=1 and I_FASONGZT=0 ";
                                startTime = DateTime.Now;
                                smsMessageDT = OracleHelper.ExecuteDataset(connecSMS, CommandType.Text, select_str).Tables[0];
                                endTime = DateTime.Now;
                                ts = endTime - startTime;
                                WriterTextLog.WriteEntry("读取数据表【TX_MESSAGE】耗费时间：" + ts.TotalSeconds.ToString() + "秒");
                                connecSMS.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriterTextLog.WriteEntry("读取的数据表【TX_MESSAGE】数据读取出现异常！" + ex.Message);
                        continue;
                    }


                    if (smsMessageDT.Rows.Count < 1)
                    {
                        WriterTextLog.WriteEntry("数据表【TX_MESSAGE】当前数据不存在本轮不发送消息提醒！");
                        //continue;
                    }
                    else
                    {
                        WriterTextLog.WriteEntry("数据表【TX_MESSAGE】数据数量：" + smsMessageDT.Rows.Count.ToString() + "条");

                        for (int i = 0; i < smsMessageDT.Rows.Count; i++)
                        {
                            TXNumber = smsMessageDT.Rows[i]["S_FASONGHM"].ToString();
                            txtNR = smsMessageDT.Rows[i]["S_NOTE"].ToString();
                            JLID = smsMessageDT.Rows[i]["ID"].ToString();

                            address = SMSUrl + "HttpSendSM?account=" + smsaccount + "&pswd=" + smspassword + "&mobile=" + TXNumber + "&msg=" + txtNR + "&needstatus=true";
                            if (String.IsNullOrEmpty(address)) return;
                            if (address.Equals("about:blank")) return;
                            if (!address.StartsWith("http://") && !address.StartsWith("https://"))
                            {
                                address = "http://" + address;
                            }
                            try
                            {
                                HttpHelps objhh = new HttpHelps();
                                result = objhh.GetHttpRequestStringByNUll_Get(address, null);
                                WriterTextLog.WriteEntry(result);
                                XiangYingNR = AnalysisServerMessage(result, out IsChengGong);
                            }
                            catch (System.UriFormatException)
                            {
                                IsChengGong = false;
                            }

                            try
                            {
                                if (ThreadProcessing.IsLocalhost == "WS")
                                {
                                    messageDataService.UpdateTX_MessageData(int.Parse(JLID), 1, IsChengGong ? 1 : -1, XiangYingNR);
                                }
                                else if (ThreadProcessing.IsLocalhost == "WCF")
                                {

                                }
                                else
                                {
                                    using (OracleConnection connecSMS = new OracleConnection(connSMS))
                                    {
                                        connecSMS.Open();
                                        OracleTransaction tran = connecSMS.BeginTransaction(IsolationLevel.ReadCommitted);
                                        string update_str = "Update TX_MESSAGE Set I_FASONGZT=" + (IsChengGong ? "1" : "-1") + ", S_BEIZHU='" + XiangYingNR + "' Where ID=" + Int32.Parse(JLID) + "  ";
                                        startTime = DateTime.Now;
                                        bool rt = OracleHelper.ExecuteNonQuery(tran, CommandType.Text, update_str) > 0;
                                        endTime = DateTime.Now;
                                        ts = endTime - startTime;
                                        WriterTextLog.WriteEntry("更新数据表【TX_MESSAGE】记录号【" + JLID + "】耗费时间：" + ts.TotalSeconds.ToString() + "秒");
                                        if (rt)
                                            tran.Commit();
                                        else
                                            tran.Rollback();
                                        connecSMS.Close();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                WriterTextLog.WriteEntry("更新数据表【TX_MESSAGE】记录号【" + JLID + "】数据读取出现异常！" + ex.Message);
                                continue;
                            }


                            Thread.Sleep(10 * 1000);//每次发送暂停10秒
                        }

                        WriterTextLog.WriteEntry("此轮消息提醒发送完毕！");

                    }

                    Thread.Sleep(30 * 1000);//每批次暂停30秒

                }

            }
            catch (ThreadAbortException tex)
            {
                //如果线程被迫中止，记入日志
                WriterTextLog.WriteEntry("线程出现错误：" + tex.Message);
            }
            catch (Exception ex)
            {
                //程序出错，记入日志
                WriterTextLog.WriteEntry("程序错误：" + ex.Message);
            }

        }

        /// <summary>
        /// 解析服务器返回内容（上海松江燃气短信专用）
        /// </summary>
        /// <param name="Str">需解析的字符串</param>
        /// <param name="IsOK">是否成功</param>
        /// <returns></returns>
        public static string AnalysisServerMessage(string Str, out bool IsOK)
        {
            string result = "";
            IsOK = false;

            try
            {
                string[] AnalysisStr = Str.Split('\n');
                string[] AnalysisStr1 = AnalysisStr[0].Split(',');
                switch (AnalysisStr1[1])
                {
                    case "0":
                        IsOK = true;
                        result = "发送成功！服务器响应时间【" + AnalysisStr1[0] + "】短信队列号【" + AnalysisStr[1] + "】";
                        break;
                    case "101":
                        IsOK = false;
                        result = "发送失败【无此用户】！服务器响应时间【" + AnalysisStr1[0] + "】 ";
                        break;
                    case "102":
                        IsOK = false;
                        result = "发送失败【密码错】！服务器响应时间【" + AnalysisStr1[0] + "】 ";
                        break;
                    case "103":
                        IsOK = false;
                        result = "发送失败【提交过快（提交速度超过流速限制）】！服务器响应时间【" + AnalysisStr1[0] + "】 ";
                        break;
                    case "104":
                        IsOK = false;
                        result = "发送失败【系统忙（因平台侧原因，暂时无法处理提交的短信）】！服务器响应时间【" + AnalysisStr1[0] + "】 ";
                        break;
                    case "105":
                        IsOK = false;
                        result = "发送失败【敏感短信（短信内容包含敏感词）】！服务器响应时间【" + AnalysisStr1[0] + "】 ";
                        break;
                    case "106":
                        IsOK = false;
                        result = "发送失败【消息长度错（>536或<=0）】！服务器响应时间【" + AnalysisStr1[0] + "】 ";
                        break;
                    case "107":
                        IsOK = false;
                        result = "发送失败【包含错误的手机号码】！服务器响应时间【" + AnalysisStr1[0] + "】 ";
                        break;
                    case "108":
                        IsOK = false;
                        result = "发送失败【手机号码个数错（群发>50000或<=0;单发>200或<=0）】！服务器响应时间【" + AnalysisStr1[0] + "】 ";
                        break;
                    case "109":
                        IsOK = false;
                        result = "发送失败【无发送额度（该用户可用短信数已使用完）】！服务器响应时间【" + AnalysisStr1[0] + "】 ";
                        break;
                    case "110":
                        IsOK = false;
                        result = "发送失败【不在发送时间内】！服务器响应时间【" + AnalysisStr1[0] + "】 ";
                        break;
                    default:
                        IsOK = false;
                        result = "发送失败【其他错误】！服务器响应时间【" + AnalysisStr1[0] + "】 ";
                        break;
                }

                return result;
            }
            catch
            {
                IsOK = false;
                return "解析失败！原文：" + Str;
            }
        }

        /// <summary>
        /// 解析服务器返回内容（微信公众号专用）
        /// </summary>
        /// <param name="Str">需解析的字符串</param>
        /// <param name="IsOK">是否成功</param>
        /// <param name="ErrCode">错误编码</param>
        /// <param name="ErrMsg">错误信息</param>
        /// <returns>处理后的反馈信息</returns>
        public static string AnalysisServerMessageForWeiXin(string Str, out bool IsOK, out string ErrCode, out string ErrMsg)
        {
            string result = "";
            IsOK = false;
            ErrCode = "";
            ErrMsg = "";

            try
            {
                Dictionary<string, object> respDic = (Dictionary<string, object>)Jss.DeserializeObject(Str);
                ErrCode = respDic["errcode"].ToString();
                ErrMsg = respDic["errmsg"].ToString();

                switch (ErrCode)
                {
                    case "-1":
                        IsOK = true;
                        result = "发送失败【系统繁忙】！返回信息【" + ErrMsg + "】 ";
                        break;
                    case "0":
                        IsOK = true;
                        result = "发送成功！返回信息【" + ErrMsg + "】消息发送号【" + respDic["msgid"].ToString() + "】";
                        break;
                    case "40001":
                        IsOK = false;
                        result = "发送失败【验证失败】！返回信息【" + ErrMsg + "】 ";
                        break;
                    case "40002":
                        IsOK = false;
                        result = "发送失败【不合法的凭证类型】！返回信息【" + ErrMsg + "】 ";
                        break;
                    case "40003":
                        IsOK = false;
                        result = "发送失败【不合法的OpenID】！返回信息【" + ErrMsg + "】 ";
                        break;
                    case "40004":
                        IsOK = false;
                        result = "发送失败【不合法的媒体文件类型】！返回信息【" + ErrMsg + "】 ";
                        break;
                    case "40005":
                        IsOK = false;
                        result = "发送失败【不合法的文件类型】！返回信息【" + ErrMsg + "】 ";
                        break;
                    case "40006":
                        IsOK = false;
                        result = "发送失败【不合法的文件大小】！返回信息【" + ErrMsg + "】 ";
                        break;
                    case "40007":
                        IsOK = false;
                        result = "发送失败【不合法的媒体文件ID】！返回信息【" + ErrMsg + "】 ";
                        break;
                    case "40008":
                        IsOK = false;
                        result = "发送失败【不合法的消息类型】！返回信息【" + ErrMsg + "】 ";
                        break;
                    case "40009":
                        IsOK = false;
                        result = "发送失败【不合法的图片文件大小】！返回信息【" + ErrMsg + "】 ";
                        break;
                    case "40010":
                        IsOK = false;
                        result = "发送失败【不合法的语音文件大小】！返回信息【" + ErrMsg + "】 ";
                        break;
                    case "40011":
                        IsOK = false;
                        result = "发送失败【不合法的视频文件大小】！返回信息【" + ErrMsg + "】 ";
                        break;
                    case "40012":
                        IsOK = false;
                        result = "发送失败【不合法的缩略图文件大小】！返回信息【" + ErrMsg + "】 ";
                        break;
                    case "40013":
                        IsOK = false;
                        result = "发送失败【不合法的APPID】！返回信息【" + ErrMsg + "】 ";
                        break;
                    case "41006":
                        IsOK = false;
                        result = "发送失败【access_token超时】！返回信息【" + ErrMsg + "】 ";
                        break;
                    case "42001":
                        IsOK = false;
                        result = "发送失败【需要GET请求】！返回信息【" + ErrMsg + "】 ";
                        break;
                    case "42002":
                        IsOK = false;
                        result = "发送失败【需要POST请求】！返回信息【" + ErrMsg + "】 ";
                        break;
                    case "42003":
                        IsOK = false;
                        result = "发送失败【需要HTTPS请求】！返回信息【" + ErrMsg + "】 ";
                        break;
                    case "45009":
                        IsOK = false;
                        result = "发送失败【接口调用超过限制】！返回信息【" + ErrMsg + "】 ";
                        break;
                    case "46001":
                        IsOK = false;
                        result = "发送失败【不存在媒体数据】！返回信息【" + ErrMsg + "】 ";
                        break;
                    case "47001":
                        IsOK = false;
                        result = "发送失败【解析JSON/XML内容错误】！返回信息【" + ErrMsg + "】 ";
                        break;
                    default:
                        IsOK = false;
                        result = "发送失败【其他错误】！返回信息【" + ErrMsg + "】 ";
                        break;
                }

                return result;
            }
            catch
            {
                IsOK = false;
                return "解析失败！原文：" + Str;
            }
        }


        /// <summary>
        /// 发送移动MAS短信
        /// </summary>
        public static void GetToMessageForMobileMASProc()
        {
            if (ConfigurationManager.AppSettings["MobileMAS"].ToString() == "0")
            {
                WriterTextLog.WriteEntry("信息(移动MAS短信)提醒服务不开启！");
                return;
            }

            WriterTextLog.WriteEntry("进入移动MAS短信发送线程");

            try
            {

                searchInterval = Convert.ToInt32(ConfigurationManager.AppSettings["SearchInterval"]);
                Thread.Sleep(searchInterval * 1000); //此进程非实时延迟开启
                WriterTextLog.WriteEntry("移动MAS短信发送服务开启！");
                int FSCS = 0;

                while (true)
                {
                    Thread.Sleep(30000); //调试专用

                    MessageDataService messageDataService = new MessageDataService(MessageWebServiceUrl);

                    DataTable smsMessageDT = new DataTable();
                    DateTime nowtime = DateTime.Now;
                    DateTime startTime = new DateTime();
                    DateTime endTime = new DateTime();
                    TimeSpan ts = new TimeSpan();
                    bool IsChengGong = false;
                    string result = "";
                    string XiangYingNR = "";

                    try
                    {
                        if (ThreadProcessing.IsLocalhost == "WS")
                        {
                            smsMessageDT = messageDataService.GetTX_MessageData(1, 0);
                        }
                        else if (ThreadProcessing.IsLocalhost == "WCF")
                        {

                        }
                        else
                        {
                            using (OracleConnection connecSMS = new OracleConnection(connSMS))
                            {
                                connecSMS.Open();
                                string select_str = "Select * From TX_MESSAGE Where I_FASONGLX=1 and I_FASONGZT=0 ";
                                startTime = DateTime.Now;
                                smsMessageDT = OracleHelper.ExecuteDataset(connecSMS, CommandType.Text, select_str).Tables[0];
                                endTime = DateTime.Now;
                                ts = endTime - startTime;
                                WriterTextLog.WriteEntry("读取数据表【TX_MESSAGE】耗费时间：" + ts.TotalSeconds.ToString() + "秒");
                                connecSMS.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriterTextLog.WriteEntry("读取的数据表【TX_MESSAGE】数据读取出现异常！" + ex.Message);
                        continue;
                    }


                    if (smsMessageDT.Rows.Count < 1)
                    {
                        WriterTextLog.WriteEntry("数据表【TX_MESSAGE】当前数据不存在本轮不发送消息提醒！");
                        //continue;
                    }
                    else
                    {
                        WriterTextLog.WriteEntry("数据表【TX_MESSAGE】数据数量：" + smsMessageDT.Rows.Count.ToString() + "条");

                        for (int i = 0; i < smsMessageDT.Rows.Count; i++)
                        {
                            TXNumber = smsMessageDT.Rows[i]["S_FASONGHM"].ToString();
                            txtNR = smsMessageDT.Rows[i]["S_NOTE"].ToString();
                            JLID = smsMessageDT.Rows[i]["ID"].ToString();
                            QunFaLX = smsMessageDT.Rows[i]["I_QUNFALX"].ToString();
                            TXNumbers = smsMessageDT.Rows[i]["S_QUNFAHM"].ToString();

                            XiangYingNR = SendMsg(TXNumber, txtNR);
                            MobileMASReturnData _ReturnData = JsonConvert.DeserializeObject<MobileMASReturnData>(XiangYingNR);

                            try
                            {
                                if (ThreadProcessing.IsLocalhost == "WS")
                                {
                                    messageDataService.UpdateTX_MessageData(int.Parse(JLID), 1, _ReturnData.success ? 1 : -1, XiangYingNR);

                                    WriterTextLog.WriteEntry("WS数据表【TX_MESSAGE】记录号【" + JLID + "】返回信息：" + XiangYingNR + " |");
                                }
                                else if (ThreadProcessing.IsLocalhost == "WCF")
                                {

                                }
                                else
                                {
                                    using (OracleConnection connecSMS = new OracleConnection(connSMS))
                                    {
                                        connecSMS.Open();
                                        OracleTransaction tran = connecSMS.BeginTransaction(IsolationLevel.ReadCommitted);
                                        string update_str = "Update TX_MESSAGE Set I_FASONGZT=" + (_ReturnData.success ? "1" : "-1") + ", S_BEIZHU='" + XiangYingNR + "' Where ID=" + Int32.Parse(JLID) + "  ";
                                        startTime = DateTime.Now;
                                        bool rt = OracleHelper.ExecuteNonQuery(tran, CommandType.Text, update_str) > 0;
                                        endTime = DateTime.Now;
                                        ts = endTime - startTime;
                                        WriterTextLog.WriteEntry("更新数据表【TX_MESSAGE】记录号【" + JLID + "】耗费时间：" + ts.TotalSeconds.ToString() + "秒");
                                        if (rt)
                                            tran.Commit();
                                        else
                                            tran.Rollback();
                                        connecSMS.Close();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                WriterTextLog.WriteEntry("更新数据表【TX_MESSAGE】记录号【" + JLID + "】数据读取出现异常！" + ex.Message);
                                continue;
                            }


                            Thread.Sleep(searchInterval * 1000);//每次发送暂停10秒
                        }

                        WriterTextLog.WriteEntry("此轮消息提醒发送完毕！");

                    }

                    Thread.Sleep(30 * 1000);//每批次暂停30秒

                }

            }
            catch (ThreadAbortException tex)
            {
                //如果线程被迫中止，记入日志
                WriterTextLog.WriteEntry("线程出现错误：" + tex.Message);
            }
            catch (Exception ex)
            {
                //程序出错，记入日志
                WriterTextLog.WriteEntry("程序错误：" + ex.Message + " \r\n#" + (searchInterval * 10).ToString() + "秒后服务即将进入重启阶段！");

                //线程暂停休息时间
                Thread.Sleep(searchInterval * 10 * 1000);
                _getToMessage_Restart = new Thread(new ThreadStart(GetToMessageForMobileMASProc));
                _getToMessage_Restart.Start();

            }

        }

        #region 移动MAS相关

        /// <summary>
        /// 移动MAS普通短信
        /// </summary>
        /// <param name="Mobiles"></param>
        /// <param name="Content"></param>
        /// <returns></returns>
        public static string SendMsg(string Mobiles, string Content)
        {

            JObject obj = new JObject();
            var ecName = ConfigurationSettings.AppSettings["ecName"].ToString();//企业名称
            var apId = ConfigurationSettings.AppSettings["apId"].ToString();// 注意： 此处不是MAS云网站的用户名，这个要在管理里面新建用户密码
            var secretKey = ConfigurationSettings.AppSettings["secretKey"].ToString();//密码
            var mobiles = Mobiles;//电话
            var content = Content;//内容
            var sign1 = ConfigurationSettings.AppSettings["sign"].ToString();//编码
            var addSerial = "";//可以随便写，三位数
            obj.Add("ecName", new JValue(ecName));
            obj.Add("apId", new JValue(apId));
            obj.Add("secretKey", new JValue(secretKey));
            obj.Add("mobiles", new JValue(mobiles));
            obj.Add("content", new JValue(content));
            obj.Add("sign", new JValue(sign1));
            obj.Add("addSerial", new JValue(addSerial));
            var mac = ecName + apId + secretKey + mobiles + content + sign1 + addSerial;
            var mac1 = UserMd5(mac);//要进行32位MD5加密
            var length = mac1.Length;
            obj.Add("mAC", new JValue(mac1));
            string paras = obj.ToString();
            var jiami = Base64Code(paras);//传参数前要进行64位加密
            System.Net.WebClient pWebClient = new System.Net.WebClient();
            pWebClient.Headers.Add("Content-Type", "application/json;charset=UTF-8"); //charset=UTF-8
            pWebClient.Headers.Add(HttpRequestHeader.Accept, "*/*");
            pWebClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1)");
            string Url = ConfigurationSettings.AppSettings["Url"].ToString();
            byte[] returnBytes = pWebClient.UploadData(Url, "POST", System.Text.Encoding.UTF8.GetBytes(jiami));

            return System.Text.Encoding.UTF8.GetString(returnBytes);

            //var aa = Base64Decode(result1);


        }

        /// <summary>
        /// 移动MAS模板短信
        /// </summary>
        /// <param name="TempLateID"></param>
        /// <param name="Mobiles"></param>
        /// <param name="Params"></param>
        /// <returns></returns>
        public static string SendTmpMsg(string TempLateID, string Mobiles, string Params)
        {

            JObject obj = new JObject();
            var ecName = ConfigurationSettings.AppSettings["ecName"].ToString();//企业名称
            var apId = ConfigurationSettings.AppSettings["apId"].ToString();// 注意： 此处不是MAS云网站的用户名，这个要在管理里面新建用户密码
            var secretKey = ConfigurationSettings.AppSettings["secretKey"].ToString();//密码
            var templateId = TempLateID;
            var mobiles = Mobiles;//电话
            var sign1 = ConfigurationSettings.AppSettings["sign"].ToString();//编码 
            var addSerial = "";//可以随便写，三位数
            obj.Add("ecName", new JValue(ecName));
            obj.Add("apId", new JValue(apId));
            obj.Add("secretKey", new JValue(secretKey));
            obj.Add("templateId", new JValue(templateId));
            obj.Add("mobiles", new JValue(mobiles));
            obj.Add("params", new JValue(Params));
            obj.Add("sign", new JValue(sign1));
            obj.Add("addSerial", new JValue(addSerial));
            var mac = ecName + apId + secretKey + templateId + mobiles + Params + sign1 + addSerial;
            var mac1 = UserMd5(mac);//要进行32位MD5加密
            var length = mac1.Length;
            obj.Add("mAC", new JValue(mac1));
            string paras = obj.ToString();
            var jiami = Base64Code(paras);//传参数前要进行64位加密
            System.Net.WebClient pWebClient = new System.Net.WebClient();
            pWebClient.Headers.Add("Content-Type", "application/json;charset=UTF-8"); //charset=UTF-8
            pWebClient.Headers.Add(HttpRequestHeader.Accept, "*/*");
            pWebClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1)");
            string Url = ConfigurationSettings.AppSettings["TMPUrl"].ToString();
            byte[] returnBytes = pWebClient.UploadData(Url, "POST", System.Text.Encoding.UTF8.GetBytes(jiami));

            return System.Text.Encoding.UTF8.GetString(returnBytes);

            //var aa = Base64Decode(result1);
        }


        /// <summary>
        /// Base64加密 
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static string Base64Code(string Message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(Message);//这里要注意不是Default 因为Default默认GB2312
            return Convert.ToBase64String(bytes);
        }


        /// <summary>
        /// Base64解密 
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static string Base64Decode(string Message)
        {
            byte[] bytes = Convert.FromBase64String(Message);
            return Encoding.UTF8.GetString(bytes);
        }
        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="encodeType">加密采用的编码方式</param>
        /// <param name="source">待加密的明文</param>
        /// <returns></returns>
        public static string Base64Encode(Encoding encodeType, string source)
        {
            string encode = string.Empty;
            byte[] bytes = encodeType.GetBytes(source);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = source;
            }
            return encode;
        }

        /// <summary>
        /// Md5 加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UserMd5(string str)
        {
            string cl = str;
            string pwd = "";
            MD5 md5 = MD5.Create();//实例化一个md5对像
                                   // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                pwd = pwd + s[i].ToString("x2");
            }
            return pwd;
        }


        /// <summary> 返回数据 </summary>
        public class MobileMASReturnData
        {
            /// <summary> </summary>
            public MobileMASReturnData()
            {
                mgsGroup = "";
                rspcod = "";
                success = false;
            }

            /// <summary> 消息批次号 </summary>
            public string mgsGroup { get; set; }
            /// <summary> 响应状态 </summary>
            public string rspcod { get; set; }
            /// <summary> 数据校验结果 </summary>
            public bool success { get; set; }
        }

        //********************************************************************************//
        //  rspcod返回对应值：
        //  IllegalMac	        mac校验不通过
        //  IllegalSignId	    无效的签名编码。
        //  InvalidMessage	    非法消息，请求数据解析失败。
        //  InvalidUsrOrPwd     非法用户名/密码。
        //  NoSignId            未匹配到对应的签名信息。
        //  success             数据验证通过。
        //  TooManyMobiles      手机号数量超限（>5000），应≤5000。
        //********************************************************************************//



        #endregion
    }
}
