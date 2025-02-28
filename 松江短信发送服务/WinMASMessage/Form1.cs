using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinMASMessage
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string txt = SendMsg("15577365174", "测试");
        }

        /// <summary>
        /// 移动MAS普通短信
        /// </summary>
        /// <param name="Mobiles"></param>
        /// <param name="Content"></param>
        /// <returns></returns>
        public static string SendMsg(string Mobiles, string Content)
        {

            JObject obj = new JObject();
            var ecName = "上海松江燃气有限公司";//企业名称
            var apId = "sh3h_sj";// 注意： 此处不是MAS云网站的用户名，这个要在管理里面新建用户
            var secretKey = "Shanghai3h@2025";//密码
            var mobiles = Mobiles;//电话
            var content = Content;//内容
            var sign1 = "lWntKXnR6";//编码
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
            string Url = "https://112.33.46.17:37892/sms/submit";
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

    }
}
