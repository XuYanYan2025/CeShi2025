using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;

namespace Ini
{
    public class IniFile
    {
        //INI文件路径
        public string path;

        //声明读写INI文件的API函数
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);


        [DllImport("kernel32.dll")]
        public extern static int GetPrivateProfileStringA(string segName, string keyName, string sDefault, byte[] buffer, int iLen, string fileName); // ANSI版本

        [DllImport("kernel32.dll")]
        public extern static int GetPrivateProfileSection(string segName, StringBuilder buffer, int nSize, string fileName);

        [DllImport("kernel32.dll")]
        public extern static int WritePrivateProfileSection(string segName, string sValue, string fileName);

        [DllImport("kernel32.dll")]
        public extern static int GetPrivateProfileSectionNamesA(byte[] buffer, int iLen, string fileName);

        /// <summary>
        /// 类的构造函数，传递INI文件名
        /// </summary>
        /// <param name="INIPath">文件路径</param>
        public IniFile(string INIPath)
        {
            path = INIPath;
        }

        /// <summary>
        /// 写INI文件
        /// </summary>
        /// <param name="Section">配置大类</param>
        /// <param name="Key">配置名称</param>
        /// <param name="Value">配置值</param>
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }

        /// <summary>
        /// 读取INI文件指定
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.path);
            return temp.ToString();
        }



        private void IniFileDemo()
        {
            IniFile ini = new IniFile("C://test.ini");

            //判断返回值，避免第一次运行时为空出错
            if ((ini.IniReadValue("LOC", "x") != "") && (ini.IniReadValue("LOC", "y") != ""))
            {
                ini.IniReadValue("LOC", "x");
                ini.IniWriteValue("LOC", "y", "XieRu");  
            }
        }


        // 封装的方法中，最有价值的是获取所有Sections和所有的Keys，网上关于这个的代码大部分是错误的，这里给出一个正确的方法：
        
        /// <summary>
        /// 返回该配置文件中所有Section名称的集合
        /// </summary>
        /// <returns></returns>
        public ArrayList ReadSections()
        {
            byte[] buffer = new byte[65535];
            int rel = 0;// GetPrivateProfileSectionNamesA(buffer, buffer.GetUpperBound(0), _FileName);
            int iCnt, iPos;
            ArrayList arrayList = new ArrayList();
            string tmp;
            if (rel > 0)
            {
                iCnt = 0; iPos = 0;
                for (iCnt = 0; iCnt < rel; iCnt++)
                {
                    if (buffer[iCnt] == 0x00)
                    {
                        tmp = System.Text.ASCIIEncoding.Default.GetString(buffer, iPos, iCnt - iPos).Trim();
                        iPos = iCnt + 1;
                        if (tmp != "")
                            arrayList.Add(tmp);
                    }
                }
            }
            return arrayList;
        }

        /// <summary>
        /// 获取节点的所有KEY值
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public ArrayList ReadKeys(string sectionName)
        {

            byte[] buffer = new byte[5120];
            int rel = 0;// GetPrivateProfileStringA(sectionName, null, "", buffer, buffer.GetUpperBound(0), _FileName);

            int iCnt, iPos;
            ArrayList arrayList = new ArrayList();
            string tmp;
            if (rel > 0)
            {
                iCnt = 0; iPos = 0;
                for (iCnt = 0; iCnt < rel; iCnt++)
                {
                    if (buffer[iCnt] == 0x00)
                    {
                        tmp = System.Text.ASCIIEncoding.Default.GetString(buffer, iPos, iCnt - iPos).Trim();
                        iPos = iCnt + 1;
                        if (tmp != "")
                            arrayList.Add(tmp);
                    }
                }
            }
            return arrayList;
        }

    }
}


