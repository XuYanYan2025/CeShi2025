using GeneralEventLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PromptMessage
{
    public partial class SendMessage : ServiceBase
    {

        /// <summary>
        /// 获取并发送消息的线程(测试信息)
        /// </summary>
        private Thread _getToMessage_XinXi;
        /// <summary>
        /// 获取并发送消息的线程(微信)
        /// </summary>
        private Thread _getToMessage_WeiXin;
        /// <summary>
        /// 获取并发送消息的线程(短信)
        /// </summary>
        private Thread _getToMessage_DuanXin;

        /// <summary>
        /// 获取并发送消息的线程(松江移动MAS)
        /// </summary>
        private Thread _getToMessage_MobileMAS;


        public SendMessage()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 在此处添加代码以启动服务。
            try
            {
                _getToMessage_XinXi = new Thread(new ThreadStart(ThreadProcessing.GetToMessageProc));
                _getToMessage_XinXi.Start();
                
                _getToMessage_MobileMAS = new Thread(new ThreadStart(ThreadProcessing.GetToMessageForMobileMASProc));
                _getToMessage_MobileMAS.Start();

            }
            catch (Exception ex)
            {
                //服务线程出错
                WriterTextLog.WriteEntry("开启服务出错。错误信息：" + ex.Message);
            }
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
            try
            {
                if (_getToMessage_XinXi != null)
                {
                    _getToMessage_XinXi.Abort();
                    _getToMessage_XinXi.Join(10000);
                }
                
                if (_getToMessage_WeiXin != null)
                {
                    _getToMessage_WeiXin.Abort();
                    _getToMessage_WeiXin.Join(10000);
                }

                if (_getToMessage_DuanXin != null)
                {
                    _getToMessage_DuanXin.Abort();
                    _getToMessage_DuanXin.Join(10000);
                }
            }
            catch (Exception ex)
            {
                //服务线程出错
                WriterTextLog.WriteEntry("停止服务出错。错误信息：" + ex.Message);
            }
        }
    }
}
