using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Drawing.Printing;
using Ini;

namespace ProviderCS
{
    public partial class Form1 : Form
    {
        #region Field
        /// <summary>
        /// �����ļ�����
        /// </summary>
        IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + "ProviderXml.sys");
        /// <summary>
        /// ����汾��
        /// </summary>
        string VersionStr = "SHGAS#2022.08.20.01";
        /// <summary>
        /// �Ƿ���Ҫ���°汾
        /// </summary>
        bool VersionIsOld = false;
        /// <summary>
        /// ����ѡ��-1���أ�0������1������
        /// </summary>
        int WangLuoType = 0;
        XmlDocument xdnew = new XmlDocument();
        XmlDocument xdupdate = new XmlDocument();
        XmlDocument xd;
        string Path = "";
        bool m_bXml = true;
        private int xx, yy;
        private int biaolx = 0;//ȫ�ֱ�����������ͨ�������ܱ�0���ܣ�1��ͨ��
        #endregion

        /// <summary>
        /// ���캯��
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            cbbMultiType.Items.Add("�ͺ�");
            lblNewMessage.Text = "��ǰ��ѡ������ȼ����0ֻ��";
            lblMessageXF.Text = "��ǰ��ѡ�����޸�ȼ����0ֻ��";

            FrmNeiWai frmNeiWai = new FrmNeiWai();
            frmNeiWai.ShowDialog();
            WangLuoType = frmNeiWai._WangLuoType;

            if (WangLuoType != -1)
            {
                UpdateVersion();
                DownXMLFile();

                FrmChangJiaZX _xzcj = new FrmChangJiaZX();
                _xzcj._WangLuoType = WangLuoType;
                _xzcj.ShowDialog();

                Path = Application.StartupPath;
                m_bXml = InitDropDownList(_xzcj.ChangJiaPZXMLFileUrl);

                try
                {
                    XmlDocument xd = new XmlDocument();
                    xd.Load(_xzcj.ChangJiaPZXMLFileUrl);
                    xd.Save(Application.StartupPath + "\\MeterType.xml");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else
            {
                m_bXml = InitDropDownList("MeterType.xml");
            }

            InitTestData();

        }

        #region Method

        #region �±����

        #region ��֤��д�Ƿ���ȷ
        /// <summary>
        /// ��֤��Ӷ�����ÿ������Ƿ��ǣ����򣸣�������ʾ��Ϣ
        /// </summary>
        /// <returns></returns>
        public Boolean MultiCheck1()
        {
            string m_szMessage = "";
            string m_szCaption = "���棡";
            DialogResult result;

            //�жϱ������Ƿ���ȷ
            if (nudPerNum.Value != 1 && nudPerNum.Value != 8)
            {
                m_szMessage = "��ȷ�������ÿ��������Ƿ���ȷ";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                result = MessageBox.Show(this, m_szMessage, m_szCaption, buttons);
                if (result == DialogResult.Yes)
                {
                    return true;
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// �ж��Ƿ���ڱ�����,�������򴴽�
        /// </summary>
        /// <returns></returns>
        public int CheckType(string m_szType, string m_szCaliber)//, decimal m_dPrice)
        {
            int m_nCount = 0;//���ʹ��ڵĸ���
            string m_szTempType = "";
            string m_szTempCaliber = "";
            //decimal m_dTempPrice = 0;

            XmlNodeList xnl = xdnew.SelectSingleNode("root").ChildNodes;
            foreach (XmlNode xn in xnl)
            {
                XmlElement xe = (XmlElement)xn;
                m_szTempType = xe.GetAttribute("Type");
                m_szTempCaliber = xe.GetAttribute("Energy");
                //m_dTempPrice = Convert.ToDecimal(xe.GetAttribute("Price"));
                if (m_szTempCaliber == m_szCaliber && m_szTempType == m_szType)// && m_dTempPrice == m_dPrice)
                {
                    m_nCount = 1;
                    break;
                }
                if (m_szTempCaliber == m_szCaliber && m_szTempType == m_szType)// && m_dTempPrice != m_dPrice)
                {
                    m_nCount = 2;
                    break;
                }
            }


            if (m_nCount == 2)
            {
                string m_szMessage = "���ͺ����Ѿ����ڲ�ͬ�ĵ��ۣ����޸ĵ���";
                MessageBox.Show(m_szMessage);
                return 2;
            }
            return m_nCount;
        }

        /// <summary>
        /// �ж��Ƿ������ţ��������򴴽�
        /// </summary>
        /// <returns></returns>
        public Boolean CheckBoxID(string m_szBoxID, string m_szType, string m_szCaliber)//, decimal m_dPrice)
        {
            int m_nBoxStatus = 0;//��Ŵ���״̬��0:���������1:������ͬ�����µ����2:���ڲ�ͬ�����µ����3:������ͬ�����ǵ��۲�ͬ
            int m_nMeterCount = 0;   //���ڱ�ŵ�����
            string m_szTempType = "";
            string m_szTempCaliber = "";
            //decimal m_dTempPrice = 0;

            if (xdnew.SelectSingleNode("root") != null)
            {
                //�ж���Ŵ���״̬
                //1:�Ȳ�ѯ���е�Type�ڵ� 2:��Type�ڵ��²������е�Box�ڵ�
                XmlNodeList xnltype = xdnew.SelectSingleNode("root").ChildNodes;
                foreach (XmlNode xntype in xnltype)
                {
                    XmlElement xetype = (XmlElement)xntype;
                    m_szTempType = xetype.GetAttribute("Type");
                    m_szTempCaliber = xetype.GetAttribute("Energy");
                    //m_dTempPrice = Convert.ToDecimal(xetype.GetAttribute("Price"));
                    XmlNodeList xnlbox = xntype.ChildNodes;
                    foreach (XmlNode xnbox in xnlbox)
                    {
                        XmlElement xmlelement = (XmlElement)xnbox;
                        if (xmlelement.GetAttribute("No") == m_szBoxID)
                        {
                            //�ж����״̬
                            if (m_szTempCaliber != m_szCaliber || m_szTempType != m_szType)// || m_dTempPrice != m_dPrice)
                                m_nBoxStatus = 2;
                            else
                            {
                                m_nBoxStatus = 1;
                                m_nMeterCount = xmlelement.ChildNodes.Count;
                            }
                            break;
                        }
                    }
                }

                if (m_nBoxStatus == 0)
                {
                    //�������
                    foreach (XmlNode xntype in xnltype)
                    {
                        XmlElement xetype = (XmlElement)xntype;
                        m_szTempType = xetype.GetAttribute("Type");
                        m_szTempCaliber = xetype.GetAttribute("Energy");
                        //m_dTempPrice = Convert.ToDecimal(xetype.GetAttribute("Price"));
                        if (m_szTempCaliber == m_szCaliber && m_szTempType == m_szType)// && m_dTempPrice == m_dPrice)
                        {
                            //���������ͣ����������ͽڵ�
                            XmlElement newxmlelement = xdnew.CreateElement("Box");
                            newxmlelement.SetAttribute("No", m_szBoxID);
                            xntype.AppendChild(newxmlelement);
                            break;
                        }
                    }
                    return true;
                }
                if (m_nBoxStatus == 2)
                {
                    //������ʾ�Ƿ����
                    string m_szMessage = "������д��Ż��ͺţ�������Ѿ��������ͺ��´���";
                    MessageBox.Show(m_szMessage);
                    return false;
                }
                else
                {
                    if (m_nMeterCount == 8)
                    {
                        MessageBox.Show("��������Ѿ�����8������������д");
                        return false;
                    }
                    return true;
                }
            }
            return true;
        }


        /// <summary>
        /// �ж��Ƿ������ţ��������򴴽�
        /// </summary>
        /// <returns></returns>
        public Boolean CheckBoxIDName(string m_szBoxID, string m_szType, string m_szCaliber)//, decimal m_dPrice)
        {
            int m_nBoxStatus = 0;//��Ŵ���״̬��0:���������1:������ͬ�����µ����2:���ڲ�ͬ�����µ����3:������ͬ�����ǵ��۲�ͬ
            int m_nMeterCount = 0;   //���ڱ�ŵ�����
            string m_szTempType = "";
            string m_szTempCaliber = "";
            //decimal m_dTempPrice = 0;

            if (xdnew.SelectSingleNode("root") != null)
            {
                //�ж���Ŵ���״̬
                //1:�Ȳ�ѯ���е�Type�ڵ� 2:��Type�ڵ��²������е�Box�ڵ�
                XmlNodeList xnltype = xdnew.SelectSingleNode("root").ChildNodes;
                foreach (XmlNode xntype in xnltype)
                {
                    XmlElement xetype = (XmlElement)xntype;
                    m_szTempType = xetype.GetAttribute("Type");
                    m_szTempCaliber = xetype.GetAttribute("Energy");
                    //m_dTempPrice = Convert.ToDecimal(xetype.GetAttribute("Price"));
                    XmlNodeList xnlbox = xntype.ChildNodes;
                    foreach (XmlNode xnbox in xnlbox)
                    {
                        XmlElement xmlelement = (XmlElement)xnbox;
                        if (xmlelement.GetAttribute("No") == m_szBoxID)
                        {
                            //�ж����״̬
                            if (m_szTempCaliber != m_szCaliber || m_szTempType != m_szType)// || m_dTempPrice != m_dPrice)
                                m_nBoxStatus = 2;
                            else
                            {
                                m_nBoxStatus = 1;
                                m_nMeterCount = xmlelement.ChildNodes.Count;
                            }
                            break;
                        }
                    }
                }

                if (m_nBoxStatus == 0)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("���������ͬ�����!");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// �ж��Ƿ�������
        /// </summary>
        /// <returns></returns>
        public int CheckBoxIDOnly(string m_szBoxID, string m_szType, string m_szCaliber)//, decimal m_dPrice)
        {
            int m_nBoxStatus = 0;//��Ŵ���״̬��0:���������1:������ͬ�����µ����2:���ڲ�ͬ�����µ����3:������ͬ�����ǵ��۲�ͬ
            int m_nMeterCount = 0;   //���ڱ�ŵ�����
            string m_szTempType = "";
            string m_szTempCaliber = "";
            //decimal m_dTempPrice = 0;

            //�ж���Ŵ���״̬
            //1:�Ȳ�ѯ���е�Type�ڵ� 2:��Type�ڵ��²������е�Box�ڵ�
            XmlNodeList xnltype = xdnew.SelectSingleNode("root").ChildNodes;
            foreach (XmlNode xntype in xnltype)
            {
                XmlElement xetype = (XmlElement)xntype;
                m_szTempType = xetype.GetAttribute("Type");
                m_szTempCaliber = xetype.GetAttribute("Energy");
                //m_dTempPrice = Convert.ToDecimal(xetype.GetAttribute("Price"));
                XmlNodeList xnlbox = xntype.ChildNodes;
                foreach (XmlNode xnbox in xnlbox)
                {
                    XmlElement xmlelement = (XmlElement)xnbox;
                    if (xmlelement.GetAttribute("No") == m_szBoxID)
                    {
                        //�ж����״̬
                        if (m_szTempCaliber != m_szCaliber || m_szTempType != m_szType)// || m_dTempPrice != m_dPrice)
                            m_nBoxStatus = 2;
                        else
                        {
                            m_nBoxStatus = 1;
                            m_nMeterCount = xmlelement.ChildNodes.Count;
                        }
                        break;
                    }
                }
            }
            if (m_nBoxStatus == 0)
                return 0;
            else if (m_nBoxStatus == 2)
            {
                //������ʾ�Ƿ����
                string m_szMessage = "������д��Ż��ͺţ�������Ѿ��������ͺ��´���";
                MessageBox.Show(m_szMessage);
                return 2;
            }
            else
            {
                if (m_nMeterCount == 8)
                {
                    MessageBox.Show("��������Ѿ�����8������������д");
                    return 2;
                }
                return 1;
            }
        }


        /// <summary>
        /// �ж��Ƿ���ڱ��
        /// </summary>
        /// <returns></returns>
        public Boolean CheckMeterID(string m_szMeterID)
        {
            int m_nCount = 0;
            if (xdnew.SelectSingleNode("root") != null)
            {
                XmlNodeList xnlType = xdnew.SelectSingleNode("root").ChildNodes;
                foreach (XmlNode xnType in xnlType)
                {
                    XmlNodeList xnlBox = xnType.ChildNodes;
                    foreach (XmlNode xnBox in xnlBox)
                    {
                        XmlNodeList xnlMeter = xnBox.ChildNodes;
                        foreach (XmlNode xnMeter in xnlMeter)
                        {
                            XmlElement xe = (XmlElement)xnMeter;
                            if (xe.GetAttribute("No").ToUpper() == m_szMeterID.ToUpper())
                            {
                                MessageBox.Show("�Ѿ�������ͬ�ı�ţ����������");
                                m_nCount++;
                                break;
                            }
                        }
                    }
                }

                if (m_nCount == 0)
                    return true;
                else
                    return false;
            }
            return true;
        }

        /// <summary>
        /// �ж��Ƿ�������߱��
        /// </summary>
        /// <returns></returns>
        public Boolean CheckWireMeterID(string m_szMeterID)
        {
            int m_nCount = 0;
            if (m_szMeterID == "")
                return true;
            if (xdnew.SelectSingleNode("root") != null)
            {
                XmlNodeList xnlType = xdnew.SelectSingleNode("root").ChildNodes;
                foreach (XmlNode xnType in xnlType)
                {
                    XmlNodeList xnlBox = xnType.ChildNodes;
                    foreach (XmlNode xnBox in xnlBox)
                    {
                        XmlNodeList xnlMeter = xnBox.ChildNodes;
                        foreach (XmlNode xnMeter in xnlMeter)
                        {
                            XmlElement xe = (XmlElement)xnMeter;
                            if (xe.GetAttribute("WireNo").ToUpper() == m_szMeterID.ToUpper())
                            {
                                MessageBox.Show("�Ѿ�������ͬ�����߱�ţ����������");
                                m_nCount++;
                                break;
                            }
                        }
                    }
                }

                if (m_nCount == 0)
                    return true;
                else
                    return false;
            }
            return true;
        }

        /// <summary>
        /// �ж��Ƿ����֤����
        /// </summary>
        /// <returns></returns>
        public Boolean CheckZhengShuBH(string m_szMeterID)
        {
            int m_nCount = 0;
            if (m_szMeterID == "")
                return true;
            if (xdnew.SelectSingleNode("root") != null)
            {
                XmlNodeList xnlType = xdnew.SelectSingleNode("root").ChildNodes;
                foreach (XmlNode xnType in xnlType)
                {
                    XmlNodeList xnlBox = xnType.ChildNodes;
                    foreach (XmlNode xnBox in xnlBox)
                    {
                        XmlNodeList xnlMeter = xnBox.ChildNodes;
                        foreach (XmlNode xnMeter in xnlMeter)
                        {
                            XmlElement xe = (XmlElement)xnMeter;
                            if (xe.GetAttribute("ZSNo").ToUpper() == m_szMeterID.ToUpper())
                            {
                                MessageBox.Show("�Ѿ�������ͬ��֤���ţ����������");
                                m_nCount++;
                                break;
                            }
                        }
                    }
                }

                if (m_nCount == 0)
                    return true;
                else
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m_szBoxID"></param>
        /// <returns></returns>
        public Boolean CheckIsExitsBox(string m_szBoxID)
        {
            int m_nCount = 0;
            //1:�Ȳ�ѯ���е�Type�ڵ� 2:��Type�ڵ��²������е�Box�ڵ�
            XmlNodeList xnltype = xdnew.SelectSingleNode("root").ChildNodes;
            foreach (XmlNode xntype in xnltype)
            {
                XmlNodeList xnlbox = xntype.ChildNodes;
                foreach (XmlNode xnbox in xnlbox)
                {
                    XmlElement xmlelement = (XmlElement)xnbox;
                    if (xmlelement.GetAttribute("No") == m_szBoxID)
                    {
                        m_nCount = 1;
                        break;
                    }
                }
            }
            if (m_nCount == 1)
            {
                MessageBox.Show("����Ѿ����ڣ���������д");
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Num"></param>
        /// <returns></returns>
        public Boolean CheckNum(int Num)
        {
            if (Num == 0)
            {
                MessageBox.Show("��ӵı���������Ϊ0!");
                return false;
            }
            if (Num > 5000)
            {
                string m_szMessage = "";
                string m_szCaption = "���棡";
                DialogResult result;

                m_szMessage = "һ�����ɳ���5000���������Ҫ�Ƚϳ���ʱ��";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                result = MessageBox.Show(this, m_szMessage, m_szCaption, buttons);
                if (result == DialogResult.Yes)
                {
                    return true;
                }
                return false;
            }
            return true;
        }

        #endregion

        /// <summary>
        /// ��֤��Ӷ����������Ƿ���ȷ
        /// </summary>
        /// <returns></returns>
        public Boolean CheckMultiButton()
        {
            string m_szTypeCaliber = cbbMultiType.SelectedValue.ToString();
            string m_szMeterID = tbMultiMeterStartID.Text.ToString();
            string m_szWireMeterID = tbWireMeterID.Text.ToString();
            string m_szStartBoxID = tbMultiStartBoxID.Text.ToString();
            string m_szShengChanNY = tbShengChanNY.Text.ToString();
            int Num = Convert.ToInt32(nudMultiNum.Value);

            //IC��������֤
            if (cbbICCard.Text == "��" && cbbMultiType.Text.Contains("IC"))
            {
                MessageBox.Show("IC�����IC�����Ͳ���Ϊ�ޣ����޸ĺ������");
                return false;
            }

            //����������֤
            if (!MultiCheck1()) return false;
            if (!CheckTypeName(m_szTypeCaliber)) return false;
            //if (!CheckPriceName(tbMultiPrice.Text.ToString())) return false;
            if (!CheckMeterName(m_szMeterID)) return false;
            if (!CheckBoxName(m_szStartBoxID)) return false;
            if (!CheckNum(Num)) return false;
            if (!CheckNY(m_szShengChanNY)) return false;
            if (!CheckJBXX()) return false;
            //if (!CheckWireMeterName(m_szWireMeterID)) return false;
            if (string.IsNullOrEmpty(tbOrderNo.Text))
            {
                MessageBox.Show("�����Ų���Ϊ�գ�����д�������");
                return false;
            }
            //�߼�������֤

            string m_szType = GetTypeByTypeCaliber(m_szTypeCaliber);
            string m_szCaliber = GetCaliberByTypeCaliber(m_szTypeCaliber);
            //decimal m_dPrice = Convert.ToDecimal(tbMultiPrice.Text.ToString());
            string m_szSerialNum = cbbProvider.SelectedValue.ToString().Substring(0, 2) + tbDate.Text.ToString() + tbSerialNum.Text.ToString();


            int m_nPerNum = Convert.ToInt32(nudPerNum.Value);
            List<string> m_BoxIDList = PopulateBoxID(m_szStartBoxID, Num, m_nPerNum);
            List<string> m_MeterIDList = PopulateMeterID(m_szMeterID, Num);
            if (!CheckListName(m_BoxIDList, m_MeterIDList, m_szType, m_szCaliber))//, m_dPrice)) 
                return false;

            if (!CheckSerialNum(xdnew, m_szSerialNum)) return false;

            int m_nReturnType = CheckType(m_szType, m_szCaliber);//, m_dPrice);
            if (m_nReturnType == 2) return false;
            bool m_bIsExistsBox = CheckIsExitsBox(m_szStartBoxID);
            if (!m_bIsExistsBox) return false;
            bool m_bIsExistsMeter = CheckMeterID(m_szMeterID);
            if (!m_bIsExistsMeter) return false;

            #region ���������ڵĽڵ�
            //���������ͣ����������ͽڵ�
            if (m_nReturnType == 0 && m_bIsExistsBox && m_bIsExistsMeter)
            {
                XmlNode xnroot = xdnew.SelectSingleNode("root");
                XmlElement newxmlelement = xdnew.CreateElement("Type");
                newxmlelement.SetAttribute("Type", m_szType);
                newxmlelement.SetAttribute("Energy", m_szCaliber);
                //newxmlelement.SetAttribute("Price", m_dPrice.ToString());
                xnroot.AppendChild(newxmlelement);
            }
            #endregion
            return true;
        }

        /// <summary>
        /// ��֤��ӵ�����������Ƿ���ȷ
        /// </summary>
        /// <returns></returns>
        public Boolean CheckOnlyButton()
        {
            //����������֤
            string m_szTypeCaliber = cbbOnlyType.SelectedValue.ToString();
            string m_szMeterID = tbOnlyMeterID.Text.ToString();
            string m_szBoxID = tbOnlyBoxID.Text.ToString();
            string m_szSerialNum = cbbProvider.SelectedValue.ToString().Substring(0, 2) + tbDate.Text.ToString() + tbSerialNum.Text.ToString();
            string m_szWireMeterID = tbOnlyWireSmall.Text.ToString();
            string m_szShengChanNY = tbShengChanNY.Text.ToString();

            if (!CheckTypeName(m_szTypeCaliber)) return false;
            //if (!CheckPriceName(tbOnlyPrice.Text.ToString())) return false;
            if (!CheckMeterName(m_szMeterID)) return false;
            if (!CheckBoxName(m_szBoxID)) return false;
            if (!CheckSerialNum(xdnew, m_szSerialNum)) return false;
            if (!CheckNY(m_szShengChanNY)) return false;
            if (!CheckJBXX()) return false;
            //if (!CheckOnlyWireMeterName(m_szWireMeterID)) return false;
            if (string.IsNullOrEmpty(tbOrderNo.Text))
            {
                MessageBox.Show("�����Ų���Ϊ�գ�����д�������");
                return false;
            }

            if (string.IsNullOrEmpty(tbTongXinH2.Text))
            {
                MessageBox.Show("ͨѶ�Ų���Ϊ�գ�����д�������");
                tbTongXinH2.Focus();
                return false;
            }

            //IC��������֤
            if (cbbICCard.Text == "��" && cbbOnlyType.Text.Contains("IC"))
            {
                MessageBox.Show("IC�����IC�����Ͳ���Ϊ�ޣ����޸ĺ������");
                return false;
            }

            //�߼�������֤
            string m_szType = GetTypeByTypeCaliber(m_szTypeCaliber);
            string m_szCaliber = GetCaliberByTypeCaliber(m_szTypeCaliber);
            //decimal m_dPrice = Convert.ToDecimal(tbOnlyPrice.Text.ToString());

            int m_nReturnType = CheckType(m_szType, m_szCaliber);//, m_dPrice);
            if (m_nReturnType == 2) return false;
            int m_nReturnBox = CheckBoxIDOnly(m_szBoxID, m_szType, m_szCaliber);//, m_dPrice);
            if (m_nReturnBox == 2) return false;
            bool m_bIsExistsMeter = CheckMeterID(m_szMeterID);
            if (!m_bIsExistsMeter) return false;
            bool m_bIsExistsWireMeter = CheckWireMeterID(m_szWireMeterID);//�ж����߱���Ƿ��ظ�
            if (!m_bIsExistsWireMeter) return false;

            #region ���������ڵĽڵ�
            //���������ͣ����������ͽڵ�
            //�ͺŲ����ڣ���Ų�����
            if (m_nReturnType == 0 && m_nReturnBox != 2 && m_bIsExistsMeter)
            {
                XmlNode xnroot = xdnew.SelectSingleNode("root");
                XmlElement newxmlelement = xdnew.CreateElement("Type");
                newxmlelement.SetAttribute("Type", m_szType);
                newxmlelement.SetAttribute("Energy", m_szCaliber);
                //newxmlelement.SetAttribute("Price", m_dPrice.ToString());
                xnroot.AppendChild(newxmlelement);
            }
            if (m_nReturnBox == 0 && m_bIsExistsMeter)
            {
                //�������
                XmlNodeList xnltype = xdnew.SelectSingleNode("root").ChildNodes;
                foreach (XmlNode xntype in xnltype)
                {
                    string m_szTempType = "";
                    string m_szTempCaliber = "";
                    //decimal m_dTempPrice = 0;
                    XmlElement xetype = (XmlElement)xntype;
                    m_szTempType = xetype.GetAttribute("Type");
                    m_szTempCaliber = xetype.GetAttribute("Energy");
                    //m_dTempPrice = Convert.ToDecimal(xetype.GetAttribute("Price"));
                    if (m_szTempCaliber == m_szCaliber && m_szTempType == m_szType)// && m_dTempPrice == m_dPrice)
                    {
                        //���������ͣ����������ͽڵ�
                        XmlElement newxmlelement = xdnew.CreateElement("Box");
                        newxmlelement.SetAttribute("No", m_szBoxID);
                        xntype.AppendChild(newxmlelement);
                        break;
                    }
                }
            }

            #endregion

            return true;
        }

        /// <summary>
        /// ��֤��ӵ������������Ƿ���ȷ
        /// </summary>
        /// <returns></returns>
        public Boolean CheckOnlyBigButton()
        {
            //����������֤
            string m_szTypeCaliber = cbbOnlyTypeBig.SelectedValue.ToString();
            string m_szTxmID = tbOnlyMeterBig.Text.ToString().Trim().ToUpper();
            string m_szMeterID = tbOnlyTxmBig.Text.ToString().Trim().ToUpper();
            //if (m_szTxmID == string.Empty)
            //{
            //    m_szTxmID = m_szMeterID;
            //}
            string m_szBoxID = tbOnlyTxmBig.Text.ToString().ToUpper();
            string m_szSerialNum = cbbProvider.SelectedValue.ToString().Substring(0, 2) + tbDate.Text.ToString() + tbSerialNum.Text.ToString();
            int Num = Convert.ToInt32(nudNumBig.Value);
            string m_szWireMeterID = tbOnlyWireBig.Text.ToString();
            string m_szShengChanNY = tbShengChanNY.Text.ToString();

            if (!CheckTypeName(m_szTypeCaliber)) return false;
            if (!CheckNY(m_szShengChanNY)) return false;
            if (!CheckJBXX()) return false;
            //if (!CheckPriceName(tbOnlyPriceBig.Text.ToString())) return false;
            //if (!CheckTxmName(m_szTxmID)) return false;
            if (!CheckMeterName(m_szMeterID)) return false;

            if (string.IsNullOrEmpty(tbOrderNo.Text))
            {
                MessageBox.Show("�����Ų���Ϊ�գ�����д�������");
                return false;
            }

            if (string.IsNullOrEmpty(tbTongXinH.Text) && biaolx == 0)
            {
                MessageBox.Show("ͨѶ�Ų���Ϊ�գ�����д�������");
                tbTongXinH.Focus();
                return false;
            }

            if (m_szTxmID != string.Empty && m_szTxmID != m_szMeterID && nudNumBig.Value.ToString() != "1")
            {
                MessageBox.Show("������ͱ�Ų���ͬ��ֻ����ӵ�����");
                return false;
            }
            //if (!CheckBoxName(m_szBoxID)) return false;
            //if (!CheckOnlyWireMeterName(m_szWireMeterID)) return false;//��֤���߱��
            if (m_szWireMeterID != string.Empty && nudNumBig.Value.ToString() != "1")
            {
                MessageBox.Show("ֻ����ӵ������߱�ţ�");
                return false;
            }

            //IC��������֤
            if (cbbICCard.Text == "��" && cbbOnlyTypeBig.Text.Contains("IC"))
            {
                MessageBox.Show("IC�����IC�����Ͳ���Ϊ�ޣ����޸ĺ������");
                return false;
            }

            //�ж������������=10��֤���ű�����д
            string m_szCaliber = GetCaliberByTypeCaliber(m_szTypeCaliber);
            //if (Convert.ToDouble(m_szCaliber) >= 10.0 && string.IsNullOrEmpty(tbZhengShuBH.Text.Trim()))
            //{
            //    MessageBox.Show("���������ڵ���10��֤���ű�����д��");
            //    return false;
            //}
            //if (Convert.ToDouble(m_szCaliber) < 10.0 && !string.IsNullOrEmpty(tbZhengShuBH.Text.Trim()))
            //{
            //    MessageBox.Show("������С��10��֤���Ų�����д��");
            //    return false;
            //}
            //�ж������������=10��ֻ����ӵ�����
            if (Convert.ToDouble(m_szCaliber) >= 10.0 && nudNumBig.Value.ToString() != "1")
            {
                MessageBox.Show("���������ڵ���10��ֻ����ӵ�����");
                return false;
            }
            //�ж�֤�����Ƿ��ظ�
            bool m_bIsExistsZhengShuBH = CheckZhengShuBH(tbZhengShuBH.Text.Trim());//�ж�֤�������Ƿ��ظ�
            if (!m_bIsExistsZhengShuBH) return false;

            bool m_bIsExistsWireMeter = CheckWireMeterID(m_szWireMeterID);//�ж����߱���Ƿ��ظ�
            if (!m_bIsExistsWireMeter) return false;

            List<string> m_MeterIDList = PopulateMeterID(m_szMeterID, Num);
            if (!CheckBigMeterList(m_MeterIDList)) return false;

            if (!CheckSerialNum(xdnew, m_szSerialNum)) return false;


            //�߼�������֤

            string m_szType = GetTypeByTypeCaliber(m_szTypeCaliber);
            //string m_szCaliber = GetCaliberByTypeCaliber(m_szTypeCaliber);
            //decimal m_dPrice = Convert.ToDecimal(tbOnlyPriceBig.Text.ToString());

            int m_nReturnType = CheckType(m_szType, m_szCaliber);//, m_dPrice);
            if (m_nReturnType == 2) return false;

            bool m_bIsExistsMeter = CheckMeterID(m_szMeterID);
            bool m_bIsExistsTxm = CheckTxmIDXF(m_szMeterID);
            if (!m_bIsExistsMeter || !m_bIsExistsTxm) return false;

            #region ���������ڵĽڵ�
            //���������ͣ����������ͽڵ�
            //�ͺŲ����ڣ���Ų�����
            if (m_nReturnType == 0 && m_bIsExistsMeter)
            {
                XmlNode xnroot = xdnew.SelectSingleNode("root");
                XmlElement newxmlelement = xdnew.CreateElement("Type");
                newxmlelement.SetAttribute("Type", m_szType);
                newxmlelement.SetAttribute("Energy", m_szCaliber);
                //newxmlelement.SetAttribute("Price", m_dPrice.ToString());
                xnroot.AppendChild(newxmlelement);
            }
            if (m_bIsExistsMeter)
            {
                //�������
                XmlNodeList xnltype = xdnew.SelectSingleNode("root").ChildNodes;
                foreach (XmlNode xntype in xnltype)
                {
                    string m_szTempType = "";
                    string m_szTempCaliber = "";
                    //decimal m_dTempPrice = 0;
                    XmlElement xetype = (XmlElement)xntype;
                    m_szTempType = xetype.GetAttribute("Type");
                    m_szTempCaliber = xetype.GetAttribute("Energy");
                    //m_dTempPrice = Convert.ToDecimal(xetype.GetAttribute("Price"));
                    if (m_szTempCaliber == m_szCaliber && m_szTempType == m_szType)//&& m_dTempPrice == m_dPrice)
                    {
                        //���������ͣ����������ͽڵ�
                        XmlElement newxmlelement = xdnew.CreateElement("Box");
                        newxmlelement.SetAttribute("No", m_szBoxID);
                        xntype.AppendChild(newxmlelement);
                        break;
                    }
                }
            }

            #endregion

            return true;
        }

        /// <summary>
        /// ��֤��������ǵ������Ƿ���ȷ
        /// </summary>
        /// <returns></returns>
        public Boolean CheckXZYButton()
        {
            //����������֤
            string m_szTypeCaliber = cbbXiuZhengYXH.SelectedValue.ToString();
            string m_szMeterID = tbXiuZhengYBH.Text.ToString();
            string m_szBoxID = tbXiuZhengYBH.Text.ToString();
            string m_szSerialNum = cbbProviderXZY.SelectedValue.ToString().Substring(0, 2) + tbDateXZY.Text.ToString() + tbSerialNumXZY.Text.ToString();
            string m_szWireMeterID = tbOnlyWireSmall.Text.ToString();

            if (!CheckTypeName(m_szTypeCaliber)) return false;
            //if (!CheckPriceName(tbOnlyPrice.Text.ToString())) return false;
            if (!CheckXZYName(m_szMeterID)) return false;
            //if (!CheckBoxName(m_szBoxID)) return false;
            if (!CheckSerialNum(xdnew, m_szSerialNum)) return false;
            //if (!CheckOnlyWireMeterName(m_szWireMeterID)) return false;

            if (string.IsNullOrEmpty(tbOrderNoXZY.Text))
            {
                MessageBox.Show("�����Ų���Ϊ�գ�����д�������");
                return false;
            }

            //�߼�������֤

            string m_szType = GetTypeByTypeCaliber(m_szTypeCaliber);
            string m_szCaliber = GetCaliberByTypeCaliber(m_szTypeCaliber);
            //decimal m_dPrice = Convert.ToDecimal(tbOnlyPrice.Text.ToString());

            int m_nReturnType = CheckType(m_szType, m_szCaliber);//, m_dPrice);
            if (m_nReturnType == 2) return false;
            int m_nReturnBox = CheckBoxIDOnly(m_szBoxID, m_szType, m_szCaliber);//, m_dPrice);
            if (m_nReturnBox == 2) return false;
            bool m_bIsExistsMeter = CheckMeterID(m_szMeterID);
            if (!m_bIsExistsMeter) return false;

            #region ���������ڵĽڵ�
            //���������ͣ����������ͽڵ�
            //�ͺŲ����ڣ���Ų�����
            if (m_nReturnType == 0 && m_nReturnBox != 2 && m_bIsExistsMeter)
            {
                XmlNode xnroot = xdnew.SelectSingleNode("root");
                XmlElement newxmlelement = xdnew.CreateElement("Type");
                newxmlelement.SetAttribute("Type", m_szType);
                newxmlelement.SetAttribute("Energy", m_szCaliber);
                //newxmlelement.SetAttribute("Price", m_dPrice.ToString());
                xnroot.AppendChild(newxmlelement);
            }
            if (m_nReturnBox == 0 && m_bIsExistsMeter)
            {
                //�������
                XmlNodeList xnltype = xdnew.SelectSingleNode("root").ChildNodes;
                foreach (XmlNode xntype in xnltype)
                {
                    string m_szTempType = "";
                    string m_szTempCaliber = "";
                    //decimal m_dTempPrice = 0;
                    XmlElement xetype = (XmlElement)xntype;
                    m_szTempType = xetype.GetAttribute("Type");
                    m_szTempCaliber = xetype.GetAttribute("Energy");
                    //m_dTempPrice = Convert.ToDecimal(xetype.GetAttribute("Price"));
                    if (m_szTempCaliber == m_szCaliber && m_szTempType == m_szType)// && m_dTempPrice == m_dPrice)
                    {
                        //���������ͣ����������ͽڵ�
                        XmlElement newxmlelement = xdnew.CreateElement("Box");
                        newxmlelement.SetAttribute("No", m_szBoxID);
                        xntype.AppendChild(newxmlelement);
                        break;
                    }
                }
            }

            #endregion

            return true;
        }

        /// <summary>
        /// ��ӵ�����Xml
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Caliber"></param>
        /// <param name="Price"></param>
        /// <param name="BoxID"></param>
        /// <param name="MeterID"></param>
        /// <returns></returns>
        public Boolean AddOnlyToXml(string m_szType, string m_szCaliber//, decimal m_dPrice
            , string m_szBoxID, string m_szMeterID, string m_szTxmID)
        {
            //�Ȳ��ҵ��ڵ�
            string m_szTempType = "";
            string m_szTempCaliber = "";
            string wireless = cbbWireless1.SelectedValue.ToString() == "-99999" ? string.Empty : cbbWireless1.SelectedValue.ToString();
            //decimal m_dTempPrice = 0;
            XmlNodeList xnltype = xdnew.SelectSingleNode("root").ChildNodes;
            foreach (XmlNode xntype in xnltype)
            {
                XmlElement xetype = (XmlElement)xntype;
                m_szTempType = xetype.GetAttribute("Type");
                m_szTempCaliber = xetype.GetAttribute("Energy");
                //m_dTempPrice = Convert.ToDecimal(xetype.GetAttribute("Price"));

                if (m_szTempCaliber == m_szCaliber && m_szTempType == m_szType)// && m_dTempPrice == m_dPrice)
                {
                    XmlNodeList xnlbox = xntype.ChildNodes;
                    foreach (XmlNode xnbox in xnlbox)
                    {
                        XmlElement xmlelement = (XmlElement)xnbox;
                        if (xmlelement.GetAttribute("No") == m_szBoxID)
                        {
                            //������
                            XmlElement newxmlelement = xdnew.CreateElement("Meter");
                            newxmlelement.SetAttribute("No", m_szMeterID);
                            newxmlelement.SetAttribute("Txm", m_szTxmID);
                            newxmlelement.SetAttribute("Wireless", wireless);
                            xnbox.AppendChild(newxmlelement);
                            break;
                        }
                    }
                }
            }

            return true;

        }


        /// <summary>
        /// ��ӵ�����Xml
        /// </summary>
        /// <param name="m_szType"></param>
        /// <param name="m_szCaliber"></param>
        /// <param name="m_dPrice"></param>
        /// <param name="m_szBoxID">���</param>
        /// <param name="m_szMeterID">���ñ��</param>
        /// <param name="m_szTxmID">���(������)</param>
        /// <param name="m_szWireMeterID">���߱��</param>
        /// <param name="m_ZhengShuBH">֤����</param>
        /// <param name="m_szTongXinH">ͨ�ź�</param>
        /// <returns></returns>
        public Boolean AddOnlyToXml(string m_szType, string m_szCaliber//, decimal m_dPrice
            , string m_szBoxID, string m_szMeterID, string m_szTxmID, string m_szWireMeterID
            , string m_ZhengShuBH, string m_szTongXinH)
        {
            //�Ȳ��ҵ��ڵ�
            string m_szTempType = "";
            string m_szTempCaliber = "";
            string wireless = cbbWireless1.SelectedValue.ToString() == "-99999" ? string.Empty : cbbWireless1.SelectedValue.ToString();
            string M_iccard = cbbICCard.SelectedValue.ToString() == "-99999" ? string.Empty : cbbICCard.SelectedValue.ToString();
            string m_jishuqlx = cbbJiShuQLX.SelectedValue.ToString() == "-99999" ? string.Empty : cbbJiShuQLX.SelectedValue.ToString();
            string m_gongdianfs = cbbGongDianFS.SelectedValue.ToString() == "-99999" ? string.Empty : cbbGongDianFS.SelectedValue.ToString();
            string m_famenqk = cbbFaMenQK.SelectedValue.ToString() == "-99999" ? string.Empty : cbbFaMenQK.SelectedValue.ToString();
            string m_jiekoulx = cbbJieKouLX.SelectedValue.ToString() == "-99999" ? string.Empty : cbbJieKouLX.SelectedValue.ToString();
            string m_jinqifx = cbbJinQiFX.SelectedValue.ToString() == "-99999" ? string.Empty : cbbJinQiFX.SelectedValue.ToString();
            string m_shebeilx = cbbSheBeiLX.SelectedValue.ToString() == "-99999" ? string.Empty : cbbSheBeiLX.SelectedValue.ToString();
            string m_jibiaogys = cbbJiBiaoGYS.SelectedValue.ToString() == "-99999" ? string.Empty : cbbJiBiaoGYS.Text.ToString();
            string m_shengchanny = tbShengChanNY.Text;
            string m_ceyakou = CKCeYaKou.Checked ? "true" : "false";

            //string m_jibiaogys = tbJiBiaoGYS.Text;
            string m_shejiyl = tbSheJiYL.Text;
            //decimal m_dTempPrice = 0;
            XmlNodeList xnltype = xdnew.SelectSingleNode("root").ChildNodes;
            foreach (XmlNode xntype in xnltype)
            {
                XmlElement xetype = (XmlElement)xntype;
                m_szTempType = xetype.GetAttribute("Type");
                m_szTempCaliber = xetype.GetAttribute("Energy");
                //m_dTempPrice = Convert.ToDecimal(xetype.GetAttribute("Price"));

                if (m_szTempCaliber == m_szCaliber && m_szTempType == m_szType)// && m_dTempPrice == m_dPrice)
                {
                    XmlNodeList xnlbox = xntype.ChildNodes;
                    foreach (XmlNode xnbox in xnlbox)
                    {
                        XmlElement xmlelement = (XmlElement)xnbox;
                        if (xmlelement.GetAttribute("No") == m_szBoxID)
                        {
                            //������
                            XmlElement newxmlelement = xdnew.CreateElement("Meter");
                            newxmlelement.SetAttribute("No", m_szMeterID);
                            newxmlelement.SetAttribute("Txm", m_szTxmID);
                            newxmlelement.SetAttribute("Wireless", wireless);
                            newxmlelement.SetAttribute("WireNo", m_szWireMeterID);
                            newxmlelement.SetAttribute("ZSNo", m_ZhengShuBH);
                            newxmlelement.SetAttribute("ICKLX", M_iccard);
                            newxmlelement.SetAttribute("JSQLX", m_jishuqlx);
                            newxmlelement.SetAttribute("GDFS", m_gongdianfs);
                            newxmlelement.SetAttribute("FMQK", m_famenqk);
                            newxmlelement.SetAttribute("JKLX", m_jiekoulx);
                            newxmlelement.SetAttribute("JQFX", m_jinqifx);
                            if (biaolx == 0 && cbbSheBeiLX.Text.Trim() != "��")
                                newxmlelement.SetAttribute("SBBH", m_shebeilx + m_szTxmID);
                            else
                                newxmlelement.SetAttribute("SBBH", "");
                            newxmlelement.SetAttribute("SBLX", m_shebeilx);
                            newxmlelement.SetAttribute("SBTXH", m_szTongXinH);
                            newxmlelement.SetAttribute("SCNY", m_shengchanny);
                            newxmlelement.SetAttribute("JBGYS", m_jibiaogys);
                            newxmlelement.SetAttribute("XZYHM", m_szTxmID);
                            newxmlelement.SetAttribute("DCYK", m_ceyakou);
                            newxmlelement.SetAttribute("XZYXH", "");
                            newxmlelement.SetAttribute("ZGYL", "");
                            newxmlelement.SetAttribute("ZDYL", "");
                            newxmlelement.SetAttribute("SJYL", m_shejiyl);
                            xnbox.AppendChild(newxmlelement);
                            break;
                        }
                    }
                }
            }

            return true;

        }

        /// <summary>
        /// ��ӵ�����Xml
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Caliber"></param>
        /// <param name="Price"></param>
        /// <param name="BoxID"></param>
        /// <param name="MeterID"></param>
        /// <returns></returns>
        public Boolean AddXZYToXml(string m_szType, string m_szCaliber//, decimal m_dPrice
            , string m_szBoxID, string m_szMeterID, string m_szTxmID, string m_ZuiGaoYL, string m_ZuiDiYL)
        {
            //�Ȳ��ҵ��ڵ�
            string m_szTempType = "";
            string m_szTempCaliber = "";
            //string wireless = cbbWireless1.SelectedValue.ToString() == "-99999" ? string.Empty : cbbWireless1.SelectedValue.ToString();
            //string M_iccard = cbbICCard.SelectedValue.ToString() == "-99999" ? string.Empty : cbbICCard.SelectedValue.ToString();
            //string m_jishuqlx = cbbJiShuQLX.SelectedValue.ToString() == "-99999" ? string.Empty : cbbJiShuQLX.SelectedValue.ToString();
            //string m_gongdianfs = cbbGongDianFS.SelectedValue.ToString() == "-99999" ? string.Empty : cbbGongDianFS.SelectedValue.ToString();
            //string m_famenqk = cbbFaMenQK.SelectedValue.ToString() == "-99999" ? string.Empty : cbbFaMenQK.SelectedValue.ToString();
            //string m_jiekoulx = cbbJieKouLX.SelectedValue.ToString() == "-99999" ? string.Empty : cbbJieKouLX.SelectedValue.ToString();
            //string m_jinqifx = cbbJinQiFX.SelectedValue.ToString() == "-99999" ? string.Empty : cbbJinQiFX.SelectedValue.ToString();
            //string m_shengchanny = tbShengChanNY.Text;
            //string m_jibiaogys = tbJiBiaoGYS.Text;
            //decimal m_dTempPrice = 0;
            XmlNodeList xnltype = xdnew.SelectSingleNode("root").ChildNodes;
            foreach (XmlNode xntype in xnltype)
            {
                XmlElement xetype = (XmlElement)xntype;
                m_szTempType = xetype.GetAttribute("Type");
                m_szTempCaliber = xetype.GetAttribute("Energy");
                //m_dTempPrice = Convert.ToDecimal(xetype.GetAttribute("Price"));

                if (m_szTempCaliber == m_szCaliber && m_szTempType == m_szType)// && m_dTempPrice == m_dPrice)
                {
                    XmlNodeList xnlbox = xntype.ChildNodes;
                    foreach (XmlNode xnbox in xnlbox)
                    {
                        XmlElement xmlelement = (XmlElement)xnbox;
                        if (xmlelement.GetAttribute("No") == m_szBoxID)
                        {
                            //������
                            XmlElement newxmlelement = xdnew.CreateElement("Meter");
                            newxmlelement.SetAttribute("No", m_szMeterID);
                            newxmlelement.SetAttribute("Txm", m_szTxmID);
                            newxmlelement.SetAttribute("Wireless", "");
                            newxmlelement.SetAttribute("WireNo", "");
                            newxmlelement.SetAttribute("ZSNo", "");
                            newxmlelement.SetAttribute("ICKLX", "");
                            newxmlelement.SetAttribute("JSQLX", "");
                            newxmlelement.SetAttribute("GDFS", "");
                            newxmlelement.SetAttribute("FMQK", "");
                            newxmlelement.SetAttribute("JKLX", "");
                            newxmlelement.SetAttribute("JQFX", "");
                            newxmlelement.SetAttribute("SCNY", "");
                            newxmlelement.SetAttribute("JBGYS", "");
                            newxmlelement.SetAttribute("XZYHM", m_szMeterID);
                            newxmlelement.SetAttribute("XZYXH", m_szTempType);
                            newxmlelement.SetAttribute("ZGYL", m_ZuiGaoYL);
                            newxmlelement.SetAttribute("ZDYL", m_ZuiDiYL);
                            newxmlelement.SetAttribute("SJYL", "");
                            xnbox.AppendChild(newxmlelement);
                            break;
                        }
                    }
                }
            }

            return true;

        }

        public Boolean CheckListName(List<string> m_BoxIDList, List<string> m_MeterIDList, string m_szType, string m_szCaliber)//, decimal m_dPrice)
        {
            foreach (string m_szBox in m_BoxIDList)
            {
                if (m_szBox.Length > 4)
                {
                    MessageBox.Show("��Ž�����4λ");
                    return false;
                }
                if (!CheckBoxIDName(m_szBox, m_szType, m_szCaliber))//,m_dPrice)) 
                    return false;
            }
            foreach (string m_szMeterID in m_MeterIDList)
            {
                if (m_szMeterID.Length > 10)
                {
                    MessageBox.Show("��Ž�����10λ");
                    return false;
                }
                if (!CheckMeterID(m_szMeterID)) return false;
            }
            return true;
        }

        public Boolean CheckBigMeterList(List<string> MeterIDList)
        {
            foreach (string m_szMeterID in MeterIDList)
            {
                if (m_szMeterID.Length > 10)
                {
                    MessageBox.Show("��Ž�����10λ");
                    return false;
                }
                if (!CheckMeterID(m_szMeterID)) return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m_BoxIDList"></param>
        /// <param name="m_MeterIDList"></param>
        /// <returns></returns>
        public Boolean CheckList(List<string> m_BoxIDList, List<string> m_MeterIDList, string m_szType, string m_szCaliber)//, decimal m_dPrice)
        {
            XmlDocument oldxmldocument = xdnew;

            foreach (string m_BoxID in m_BoxIDList)
            {
                if (!CheckBoxID(m_BoxID, m_szType, m_szCaliber))//, m_dPrice))
                {
                    xdnew = oldxmldocument;
                    return false;
                }
            }
            foreach (string m_MeterID in m_MeterIDList)
            {
                if (!CheckMeterID(m_MeterID))
                {
                    xdnew = oldxmldocument;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m_BoxIDList"></param>
        /// <param name="m_MeterIDList"></param>
        /// <returns></returns>
        public Boolean CheckList(List<string> m_BoxIDList, List<string> m_MeterIDList, string m_szType, string m_szCaliber, List<string> m_WireMeterIDList)//, decimal m_dPrice)
        {
            XmlDocument oldxmldocument = xdnew;

            foreach (string m_BoxID in m_BoxIDList)
            {
                if (!CheckBoxID(m_BoxID, m_szType, m_szCaliber))//, m_dPrice))
                {
                    xdnew = oldxmldocument;
                    return false;
                }
            }
            foreach (string m_MeterID in m_MeterIDList)
            {
                if (!CheckMeterID(m_MeterID))
                {
                    xdnew = oldxmldocument;
                    return false;
                }
            }

            foreach (string m_WireMeterID in m_WireMeterIDList)
            {
                if (!CheckWireMeterID(m_WireMeterID))
                {
                    xdnew = oldxmldocument;
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="m_szType"></param>
        /// <param name="m_szCaliber"></param>
        /// <param name="m_dPrice"></param>
        /// <param name="m_BoxIDList"></param>
        /// <param name="m_MeterIDList"></param>
        /// <param name="m_nPerNum"></param>
        /// <param name="m_szWireMeterID">���߱��</param>
        /// <param name="m_ZhengShuBH">֤����</param>
        /// <param name="m_szTongXinH">ͨ�ź�</param>
        /// <returns></returns>
        public Boolean AddMultiToXml(string m_szType, string m_szCaliber//, decimal m_dPrice
            , List<string> m_BoxIDList, List<string> m_MeterIDList, int m_nPerNum, string m_szWireMeterID
            , string m_ZhengShuBH, string m_szTongXinH)
        {
            int i = 0;
            int m_nCount = 0;
            foreach (string m_szMeterID in m_MeterIDList)
            {
                if (m_nCount >= m_nPerNum)
                {
                    m_nCount = 0;
                    i++;
                }
                m_nCount++;
                AddOnlyToXml(m_szType, m_szCaliber//, m_dPrice
                    , m_BoxIDList[i], m_szMeterID, m_szMeterID, m_szWireMeterID, m_ZhengShuBH, m_szTongXinH);
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m_szType"></param>
        /// <param name="m_szCaliber"></param>
        /// <param name="m_dPrice"></param>
        /// <param name="m_BoxIDList"></param>
        /// <param name="m_MeterIDList"></param>
        /// <returns></returns>
        public Boolean AddMultiToXml(string m_szType, string m_szCaliber//, decimal m_dPrice
            , List<string> m_BoxIDList, List<string> m_MeterIDList, int m_nPerNum, List<string> m_WireMeterIDList)
        {
            int i = 0;
            int m_nCount = 0;
            int j = 0;
            foreach (string m_szMeterID in m_MeterIDList)
            {
                if (m_nCount >= m_nPerNum)
                {
                    m_nCount = 0;
                    i++;
                }
                m_nCount++;
                AddOnlyToXml(m_szType, m_szCaliber//, m_dPrice
                    , m_BoxIDList[i], m_szMeterID, m_szMeterID, m_szMeterID, string.Empty, "");//m_WireMeterIDList[j]
                j++;
            }
            return true;
        }

        /// <summary>
        /// ��Xml�ļ��е�������ӵ�����
        /// </summary>
        public void AddXmlToTree()
        {
            XmlNodeList xnlType = xdnew.SelectSingleNode("root").ChildNodes;
            foreach (XmlNode xnType in xnlType)
            {
                XmlElement xeType = (XmlElement)xnType;
                TreeNode tnType = new TreeNode(xeType.GetAttribute("Type") + "(" + xeType.GetAttribute("Energy") //+ "/��" + Convert.ToDecimal(xeType.GetAttribute("Price")).ToString("F2") 
                    + ")");
                XmlNodeList xnlBox = xnType.ChildNodes;
                foreach (XmlNode xnBox in xnlBox)
                {
                    XmlElement xeBox = (XmlElement)xnBox;
                    TreeNode tnBox = new TreeNode(xeBox.GetAttribute("No"));
                    XmlNodeList xnlMeter = xnBox.ChildNodes;
                    foreach (XmlNode xnMeter in xnlMeter)
                    {
                        XmlElement xeMeter = (XmlElement)xnMeter;
                        TreeNode tnMeter = new TreeNode(xeMeter.GetAttribute("No"));

                        tnBox.Nodes.Add(tnMeter);
                    }
                    tnType.Nodes.Add(tnBox);
                }
                tvXml.Nodes.Add(tnType);
            }
            if (tvXml.Nodes.Count != 0)
            {
                tvXml.SelectedNode = tvXml.Nodes[0];
                tvXml.SelectedNode.Expand();
            }
        }

        /// <summary>
        /// ��Xml�ļ��е�������ӵ�����
        /// </summary>
        public void AddXmlToXZYTree()
        {
            XmlNodeList xnlType = xdnew.SelectSingleNode("root").ChildNodes;
            foreach (XmlNode xnType in xnlType)
            {
                XmlElement xeType = (XmlElement)xnType;
                TreeNode tnType = new TreeNode(xeType.GetAttribute("Type"));
                XmlNodeList xnlBox = xnType.ChildNodes;
                foreach (XmlNode xnBox in xnlBox)
                {
                    XmlElement xeBox = (XmlElement)xnBox;
                    TreeNode tnBox = new TreeNode(xeBox.GetAttribute("No"));
                    XmlNodeList xnlMeter = xnBox.ChildNodes;
                    foreach (XmlNode xnMeter in xnlMeter)
                    {
                        XmlElement xeMeter = (XmlElement)xnMeter;
                        TreeNode tnMeter = new TreeNode(xeMeter.GetAttribute("No"));

                        tnBox.Nodes.Add(tnMeter);
                    }
                    tnType.Nodes.Add(tnBox);
                }
                tvXiuZhengY.Nodes.Add(tnType);
            }
            if (tvXiuZhengY.Nodes.Count != 0)
            {
                tvXiuZhengY.SelectedNode = tvXiuZhengY.Nodes[0];
                tvXiuZhengY.SelectedNode.Expand();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="NodeName"></param>
        public void ExpandTree(string NodeName)
        {
            foreach (TreeNode tn in tvXml.Nodes)
            {
                foreach (TreeNode tnbox in tn.Nodes)
                {
                    if (tnbox.Text.ToString() == NodeName)
                    {
                        tvXml.SelectedNode = tnbox;
                        tvXml.SelectedNode.Expand();
                    }
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="NodeName"></param>
        public void ExpandXZYTree(string NodeName)
        {
            foreach (TreeNode tn in tvXiuZhengY.Nodes)
            {
                foreach (TreeNode tnbox in tn.Nodes)
                {
                    if (tnbox.Text.ToString() == NodeName)
                    {
                        tvXiuZhengY.SelectedNode = tnbox;
                        tvXiuZhengY.SelectedNode.Expand();
                    }
                }
            }

        }


        public void DeleteXml(string m_szFieldName, string m_szValue)
        {
            if (m_szFieldName == "Type")
            {
                XmlNodeList xnl = xdnew.SelectSingleNode("root").ChildNodes;
                foreach (XmlNode xn in xnl)
                {
                    XmlElement xe = (XmlElement)xn;
                    if (xe.GetAttribute("Type") == m_szValue)
                    {
                        xdnew.SelectSingleNode("root").RemoveChild(xn);
                    }

                }
            }
            if (m_szFieldName == "Box")
            {
                XmlNodeList xnlType = xdnew.SelectSingleNode("root").ChildNodes;
                foreach (XmlNode xnType in xnlType)
                {
                    XmlNodeList xnlBox = xnType.ChildNodes;
                    foreach (XmlNode xnBox in xnlBox)
                    {
                        XmlElement xeBox = (XmlElement)xnBox;
                        if (xeBox.GetAttribute("No") == m_szValue)
                        {
                            xnType.RemoveChild(xnBox);

                        }
                    }
                    if (xnType.ChildNodes.Count == 0)
                    {
                        xdnew.SelectSingleNode("root").RemoveChild(xnType);
                    }
                }
            }
            if (m_szFieldName == "Meter")
            {
                XmlNodeList xnlType = xdnew.SelectSingleNode("root").ChildNodes;
                foreach (XmlNode xnType in xnlType)
                {
                    XmlNodeList xnlBox = xnType.ChildNodes;
                    foreach (XmlNode xnBox in xnlBox)
                    {
                        XmlNodeList xnlMeter = xnBox.ChildNodes;
                        foreach (XmlNode xnMeter in xnlMeter)
                        {
                            XmlElement xeMeter = (XmlElement)xnMeter;
                            if (xeMeter.GetAttribute("No") == m_szValue)
                            {
                                xnBox.RemoveChild(xnMeter);

                            }
                        }
                        if (xnBox.ChildNodes.Count == 0)
                        {
                            xnType.RemoveChild(xnBox);
                        }

                    }
                    if (xnType.ChildNodes.Count == 0)
                    {
                        xdnew.SelectSingleNode("root").RemoveChild(xnType);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeteleXmlFromTree()
        {
            XmlDocument xmldocument = new XmlDocument();
            foreach (TreeNode tn in tvXml.Nodes)
            {
                //if (tn.Checked)
                //{
                //    string m_szTypeCaliberPrice = tn.Text.ToString();
                //    int m_nIndex = m_szTypeCaliberPrice.LastIndexOf("(");
                //    string m_szType = m_szTypeCaliberPrice.Substring(0, m_nIndex);
                //    DeleteXml("Type", m_szType);
                //}
                //else
                //{
                foreach (TreeNode tnBox in tn.Nodes)
                {
                    //if (tnBox.Checked)
                    //{
                    //    string m_szBoxID = tnBox.Text.ToString();
                    //    DeleteXml("Box", m_szBoxID);
                    //}
                    //else
                    //{

                    foreach (TreeNode tnMeter in tnBox.Nodes)
                    {
                        if (tnMeter.Checked)
                        {
                            string m_szMeter = tnMeter.Text.ToString();
                            DeleteXml("Meter", m_szMeter);
                        }
                    }
                    //}
                }
                //}
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeteleXmlFromXZYTree()
        {
            XmlDocument xmldocument = new XmlDocument();
            foreach (TreeNode tn in tvXiuZhengY.Nodes)
            {
                //if (tn.Checked)
                //{
                //    string m_szTypeCaliberPrice = tn.Text.ToString();
                //    int m_nIndex = m_szTypeCaliberPrice.LastIndexOf("(");
                //    string m_szType = m_szTypeCaliberPrice.Substring(0, m_nIndex);
                //    DeleteXml("Type", m_szType);
                //}
                //else
                //{
                foreach (TreeNode tnBox in tn.Nodes)
                {
                    //if (tnBox.Checked)
                    //{
                    //    string m_szBoxID = tnBox.Text.ToString();
                    //    DeleteXml("Box", m_szBoxID);
                    //}
                    //else
                    //{

                    foreach (TreeNode tnMeter in tnBox.Nodes)
                    {
                        if (tnMeter.Checked)
                        {
                            string m_szMeter = tnMeter.Text.ToString();
                            DeleteXml("Meter", m_szMeter);
                        }
                    }
                    //}
                }
                //}
            }
        }

        /// <summary>
        /// ��ȡ��Ϣ
        /// </summary>
        public string GetLabelMessage()
        {
            int m_nTotalCount = 0;
            int m_nBoxCount = 0;
            string m_szMessage = "";
            List<int> m_nTypeMeter = new List<int>();

            XmlNodeList xnlType = xdnew.SelectSingleNode("root").ChildNodes;
            foreach (XmlNode xntype in xnlType)
            {
                XmlElement xetype = (XmlElement)xntype;
                m_szMessage += xetype.GetAttribute("Type") + "�ͺ�Ϊ";
                int m_nTempCount = 0;
                foreach (XmlNode xnBox in xntype.ChildNodes)
                {
                    m_nBoxCount++;
                    foreach (XmlNode xnMeter in xnBox.ChildNodes)
                    {
                        m_nTotalCount++;
                        m_nTempCount++;
                    }
                }
                m_szMessage += m_nTempCount + "����";
            }

            m_szMessage = "��ǰ��ѡ������ȼ����" + m_nTotalCount + "ֻ����" + m_nBoxCount + "�䣬����" + m_szMessage;
            if (m_szMessage.Substring(m_szMessage.Length - 1, 1) == "��")
            {
                m_szMessage = m_szMessage.Substring(0, m_szMessage.Length - 1);
            }
            if (m_szMessage.Substring(m_szMessage.Length - 3, 3) == "������")
            {
                m_szMessage = m_szMessage.Substring(0, m_szMessage.Length - 3);
            }
            return m_szMessage;
        }

        /// <summary>
        /// ������ʼ�Ż�ȡ�������
        /// </summary>
        /// <param name="m_szStartBoxID"></param>
        /// <param name="m_nCount"></param>
        /// <param name="m_nPerNum"></param>
        /// <returns></returns>
        public List<string> PopulateBoxID(string m_szStartBoxID, int m_nCount, int m_nPerNum)
        {
            List<string> list = new List<string>();
            int m_nStartBoxID = Convert.ToInt32(m_szStartBoxID.Substring(0, 4));
            int m_nBoxNum = 0;
            if (m_nCount % m_nPerNum == 0)
                m_nBoxNum = m_nCount / m_nPerNum;
            else
                m_nBoxNum = m_nCount / m_nPerNum + 1;

            for (int i = 0; i < m_nBoxNum; i++)
            {
                int m_nBoxID = m_nStartBoxID + i;
                string m_szBoxID = m_nBoxID.ToString();
                while (m_szBoxID.Length < 4)
                    m_szBoxID = "0" + m_szBoxID;
                string m_szNewBoxID = m_szBoxID;
                list.Add(m_szNewBoxID);
            }
            return list;
        }


        public void AddSerialNumToBoxID(string m_szSerialNum)
        {
            XmlNodeList xnl = xdnew.SelectSingleNode("root").ChildNodes;
            foreach (XmlNode xnType in xnl)
            {
                foreach (XmlNode xnBox in xnType.ChildNodes)
                {
                    XmlElement xe = (XmlElement)xnBox;
                    string m_szBoxID = xe.GetAttribute("No");
                    if (m_szBoxID.Length == 4)
                        xe.SetAttribute("No", m_szSerialNum + m_szBoxID);
                }
            }
        }
        #endregion

        #region �޸������



        #region ��֤��д�Ƿ���ȷ
        /// <summary>
        /// �ж��Ƿ���ڱ�����,�������򴴽�
        /// </summary>
        /// <returns></returns>
        public int CheckTypeXF(string m_szType, string m_szCaliber)
        {
            int m_nCount = 0;//���ʹ��ڵĸ���
            string m_szTempType = "";
            string m_szTempCaliber = "";

            XmlNodeList xnl = xdupdate.SelectSingleNode("root").ChildNodes;
            foreach (XmlNode xn in xnl)
            {
                XmlElement xe = (XmlElement)xn;
                m_szTempType = xe.GetAttribute("Type");
                m_szTempCaliber = xe.GetAttribute("Energy");
                if (m_szTempCaliber == m_szCaliber && m_szTempType == m_szType)
                {
                    m_nCount = 1;
                    break;
                }
            }

            //���������ͣ����������ͽڵ�
            if (m_nCount == 0)
            {
                return 0;
            }
            return 1;
        }

        /// <summary>
        /// �ж��Ƿ���ڱ��
        /// </summary>
        /// <returns></returns>
        public Boolean CheckMeterIDXF(string m_szMeterID)
        {

            int m_nCount = 0;
            if (xdupdate.SelectSingleNode("root") != null)
            {
                XmlNodeList xnlType = xdupdate.SelectSingleNode("root").ChildNodes;

                foreach (XmlNode xnType in xnlType)
                {
                    XmlNodeList xnlMeter = xnType.ChildNodes;
                    foreach (XmlNode xnMeter in xnlMeter)
                    {
                        XmlElement xe = (XmlElement)xnMeter;
                        if (xe.GetAttribute("No").ToUpper() == m_szMeterID.ToUpper())
                        {
                            MessageBox.Show("�Ѿ�������ͬ�ı�ţ����������");
                            m_nCount++;
                            break;
                        }
                    }
                }

                if (m_nCount == 0)
                    return true;
                else
                    return false;
            }
            return true;

        }

        /// <summary>
        /// �ж��Ƿ����������
        /// </summary>
        /// <returns></returns>
        public Boolean CheckTxmIDXF(string m_szMeterID)
        {

            int m_nCount = 0;
            if (xdupdate.SelectSingleNode("root") != null)
            {
                XmlNodeList xnlType = xdupdate.SelectSingleNode("root").ChildNodes;

                foreach (XmlNode xnType in xnlType)
                {
                    XmlNodeList xnlMeter = xnType.ChildNodes;
                    foreach (XmlNode xnMeter in xnlMeter)
                    {
                        XmlElement xe = (XmlElement)xnMeter;
                        if (xe.GetAttribute("Txm").ToUpper() == m_szMeterID.ToUpper())
                        {
                            MessageBox.Show("�Ѿ�������ͬ�������룬���������");
                            m_nCount++;
                            break;
                        }
                    }
                }

                if (m_nCount == 0)
                    return true;
                else
                    return false;
            }
            return true;

        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Boolean CheckXFMultiButton()
        {
            string m_szTypeCaliber = cbbXFMultiType.SelectedValue.ToString();
            string m_szMeterID = tbXFStartMeterID.Text.ToString();
            int Num = Convert.ToInt32(tbXFMeterNum.Value);

            if (!CheckTypeName(m_szTypeCaliber)) return false;
            if (!CheckMeterName(m_szMeterID)) return false;

            string m_szType = GetTypeByTypeCaliber(m_szTypeCaliber);
            string m_szCaliber = GetCaliberByTypeCaliber(m_szTypeCaliber);

            List<string> m_MeterIDList = PopulateMeterID(m_szMeterID, Num);
            if (!CheckListNameXF(m_MeterIDList)) return false;
            if (!CheckSerialNum(xdupdate, "SX" + tbShiXiaoID.Text.ToString())) return false;
            int m_nReturnType = CheckTypeXF(m_szType, m_szCaliber);
            bool m_bMeter = CheckMeterIDXF(m_szMeterID);
            if (!m_bMeter) return false;
            if (!CheckNum(Num)) return false;
            if (m_nReturnType == 0 && m_bMeter)
            {
                XmlNode xnroot = xdupdate.SelectSingleNode("root");
                XmlElement newxmlelement = xdupdate.CreateElement("Type");
                newxmlelement.SetAttribute("Type", m_szType);
                newxmlelement.SetAttribute("Energy", m_szCaliber);
                xnroot.AppendChild(newxmlelement);
            }
            return true;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Boolean CheckXFOnlyButton()
        {
            string m_szTypeCaliber = cbbXFOnlyType.SelectedValue.ToString();
            string m_szMeterID = tbXFMeterID.Text.Trim().ToString();
            //string m_szTxmID = tbXFTxmID.Text.Trim().ToString();
            //if (m_szTxmID == string.Empty)
            //{
            //    m_szTxmID = m_szMeterID;
            //}

            if (!CheckTypeName(m_szTypeCaliber)) return false;
            //if (!CheckTxmName(m_szTxmID)) return false;
            if (!CheckMeterName(m_szMeterID)) return false;

            string m_szType = GetTypeByTypeCaliber(m_szTypeCaliber);
            string m_szCaliber = GetCaliberByTypeCaliber(m_szTypeCaliber);

            if (!CheckSerialNum(xdupdate, "SX" + tbShiXiaoID.Text.ToString())) return false;
            int m_nReturnType = CheckTypeXF(m_szType, m_szCaliber);
            //bool m_bTxm = CheckTxmIDXF(m_szTxmID);
            bool m_bMeter = CheckMeterIDXF(m_szMeterID);
            if (!m_bMeter) return false;

            if (m_nReturnType == 0 && m_bMeter)
            {
                XmlNode xnroot = xdupdate.SelectSingleNode("root");
                XmlElement newxmlelement = xdupdate.CreateElement("Type");
                newxmlelement.SetAttribute("Type", m_szType);
                newxmlelement.SetAttribute("Energy", m_szCaliber);
                xnroot.AppendChild(newxmlelement);
            }
            return true;

        }


        public Boolean CheckListNameXF(List<string> m_MeterIDList)
        {
            foreach (string m_MeterID in m_MeterIDList)
            {
                if (m_MeterID.Length > 10)
                {
                    MessageBox.Show("��Ž�����10λ!");
                    return false;
                }
                if (!CheckMeterIDXF(m_MeterID)) return false;
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m_BoxIDList"></param>
        /// <param name="m_MeterIDList"></param>
        /// <returns></returns>
        public Boolean CheckListXF(List<string> m_MeterIDList)
        {
            XmlDocument oldxmldocument = xdupdate;

            foreach (string m_MeterID in m_MeterIDList)
            {
                if (!CheckMeterIDXF(m_MeterID))
                {
                    xdupdate = oldxmldocument;
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// ��ӵ�����Xml
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Caliber"></param>
        /// <param name="Price"></param>
        /// <param name="BoxID"></param>
        /// <param name="MeterID"></param>
        /// <returns></returns>
        public Boolean AddOnlyToXmlXF(string m_szType, string m_szCaliber, string m_szMeterID, string m_szTxmID)
        {
            //�Ȳ��ҵ��ڵ�
            string m_szTempType = "";
            string m_szTempCaliber = "";
            //string wireless = cbbWireless2.SelectedValue.ToString() == "-99999" ? string.Empty : cbbWireless2.SelectedValue.ToString();
            XmlNodeList xnltype = xdupdate.SelectSingleNode("root").ChildNodes;
            foreach (XmlNode xntype in xnltype)
            {
                XmlElement xetype = (XmlElement)xntype;
                m_szTempType = xetype.GetAttribute("Type");
                m_szTempCaliber = xetype.GetAttribute("Energy");

                if (m_szTempCaliber == m_szCaliber && m_szTempType == m_szType)
                {
                    //������
                    XmlElement newxmlelement = xdupdate.CreateElement("Meter");
                    newxmlelement.SetAttribute("No", m_szMeterID);
                    newxmlelement.SetAttribute("Txm", m_szTxmID);
                    //newxmlelement.SetAttribute("Wireless", wireless);
                    xntype.AppendChild(newxmlelement);
                    break;
                }
            }
            return true;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m_szType"></param>
        /// <param name="m_szCaliber"></param>
        /// <param name="m_dPrice"></param>
        /// <param name="m_BoxIDList"></param>
        /// <param name="m_MeterIDList"></param>
        /// <returns></returns>
        public Boolean AddMultiToXmlXF(string m_szType, string m_szCaliber, List<string> m_MeterIDList)
        {

            foreach (string m_szMeterID in m_MeterIDList)
            {
                AddOnlyToXmlXF(m_szType, m_szCaliber, m_szMeterID, m_szMeterID);
            }
            return true;
        }

        /// <summary>
        /// ��ȡ��Ϣ
        /// </summary>
        public string GetLabelMessageXF()
        {
            int m_nTotalCount = 0;
            string m_szMessage = "";

            XmlNodeList xnlType = xdupdate.SelectSingleNode("root").ChildNodes;
            foreach (XmlNode xntype in xnlType)
            {
                XmlElement xetype = (XmlElement)xntype;
                m_szMessage += xetype.GetAttribute("Type") + "�ͺ�Ϊ";
                int m_nTempCount = xetype.ChildNodes.Count;
                m_nTotalCount += m_nTempCount;
                m_szMessage += m_nTempCount + "����";
            }

            m_szMessage = "��ǰ��ѡ������ȼ����" + m_nTotalCount + "ֻ������" + m_szMessage;
            if (m_szMessage.Substring(m_szMessage.Length - 1, 1) == "��")
            {
                m_szMessage = m_szMessage.Substring(0, m_szMessage.Length - 1);
            }

            if (m_szMessage.Substring(m_szMessage.Length - 3, 3) == "������")
            {
                m_szMessage = m_szMessage.Substring(0, m_szMessage.Length - 3);
            }
            return m_szMessage;
        }

        /// <summary>
        /// ��Xml�ļ��е�������ӵ�����
        /// </summary>
        public void AddXmlToTreeXF()
        {
            XmlNodeList xnlType = xdupdate.SelectSingleNode("root").ChildNodes;
            foreach (XmlNode xnType in xnlType)
            {
                XmlElement xeType = (XmlElement)xnType;
                TreeNode tnType = new TreeNode(xeType.GetAttribute("Type") + "(" + xeType.GetAttribute("Energy") + ")");

                XmlNodeList xnlMeter = xnType.ChildNodes;
                foreach (XmlNode xnMeter in xnlMeter)
                {
                    XmlElement xeMeter = (XmlElement)xnMeter;
                    TreeNode tnMeter = new TreeNode(xeMeter.GetAttribute("No"));

                    tnType.Nodes.Add(tnMeter);
                }

                tvXmlXF.Nodes.Add(tnType);
            }
            if (tvXmlXF.Nodes.Count != 0)
            {
                tvXmlXF.SelectedNode = tvXmlXF.Nodes[0];
                tvXmlXF.SelectedNode.Expand();
            }
        }



        public void DeleteXmlXF(string m_szFieldName, string m_szValue)
        {
            if (m_szFieldName == "Type")
            {
                XmlNodeList xnl = xdupdate.SelectSingleNode("root").ChildNodes;
                foreach (XmlNode xn in xnl)
                {
                    XmlElement xe = (XmlElement)xn;
                    if (xe.GetAttribute("Type") == m_szValue)
                    {
                        xdupdate.SelectSingleNode("root").RemoveChild(xn);
                    }
                }
            }

            if (m_szFieldName == "Meter")
            {
                XmlNodeList xnlType = xdupdate.SelectSingleNode("root").ChildNodes;
                foreach (XmlNode xnType in xnlType)
                {
                    XmlNodeList xnlMeter = xnType.ChildNodes;
                    foreach (XmlNode xnMeter in xnlMeter)
                    {
                        XmlElement xeMeter = (XmlElement)xnMeter;
                        if (xeMeter.GetAttribute("No") == m_szValue)
                        {
                            xnType.RemoveChild(xnMeter);
                        }
                    }
                    if (xnType.ChildNodes.Count == 0)
                    {
                        xdupdate.SelectSingleNode("root").RemoveChild(xnType);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeteleXmlFromTreeXF()
        {
            XmlDocument xmldocument = new XmlDocument();
            foreach (TreeNode tn in tvXmlXF.Nodes)
            {
                //if (tn.Checked)
                //{
                ////    string m_szTypeCaliberPrice = tn.Text.ToString();
                //    int m_nIndex = m_szTypeCaliberPrice.LastIndexOf("(");
                //    string m_szType = m_szTypeCaliberPrice.Substring(0, m_nIndex);
                //    DeleteXmlXF("Type", m_szType);

                //}
                //else
                //{
                foreach (TreeNode tnMeter in tn.Nodes)
                {
                    if (tnMeter.Checked)
                    {
                        string m_szMeter = tnMeter.Text.ToString();
                        DeleteXmlXF("Meter", m_szMeter);
                    }
                }
                //}
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="NodeName"></param>
        public void ExpandTreeXF(string NodeName)
        {
            tvXmlXF.CollapseAll();
            foreach (TreeNode tn in tvXmlXF.Nodes)
            {
                if (tn.Text.ToString() == NodeName)
                {
                    tvXmlXF.SelectedNode = tn;
                    tvXmlXF.SelectedNode.Expand();
                }
            }

        }

        #endregion

        #region �����ͻ���

        /// <summary>
        /// ����Xml�ĵ������ͻ�����ͳ����Ϣ
        /// </summary>
        /// <param name="xmldoc"></param>
        /// <returns></returns>
        public DataTable GetInfo(XmlDocument xmldoc)
        {
            DataTable dt = new DataTable();
            DataRow dr;

            #region Create DataColumn

            DataColumn dc;
            dc = new DataColumn();
            dc.DataType = System.Type.GetType("System.String");
            dc.ColumnName = "����";
            dt.Columns.Add(dc);
            dc = new DataColumn();
            dc.DataType = System.Type.GetType("System.String");
            dc.ColumnName = "����";
            dt.Columns.Add(dc);
            dc = new DataColumn();
            dc.DataType = System.Type.GetType("System.String");
            dc.ColumnName = "��λ";
            dt.Columns.Add(dc);
            dc = new DataColumn();
            dc.DataType = System.Type.GetType("System.String");
            dc.ColumnName = "����";
            dt.Columns.Add(dc);
            //dc = new DataColumn();
            //dc.DataType = System.Type.GetType("System.String");
            //dc.ColumnName = "����";
            //dt.Columns.Add(dc);
            //dc = new DataColumn();
            //dc.DataType = System.Type.GetType("System.String");
            //dc.ColumnName = "���";
            //dt.Columns.Add(dc);

            #endregion

            int m_nTypeMeterCount = 0;
            string m_szType = "";
            string m_szCaliber = "";
            //decimal m_dPrice = 0;
            int m_nTotalNum = 0;
            //decimal m_dTotalPrice = 0;

            XmlNodeList xnl = xmldoc.SelectSingleNode("root").ChildNodes;
            foreach (XmlNode xnType in xnl)
            {
                try
                {
                    XmlElement xeType = (XmlElement)xnType;
                    m_szType = xeType.GetAttribute("Type");
                    m_szCaliber = xeType.GetAttribute("Energy");
                    //m_dPrice = Convert.ToDecimal(xeType.GetAttribute("Price"));
                    m_nTypeMeterCount = 0;
                    foreach (XmlNode xnBox in xnType)
                    {
                        m_nTypeMeterCount += xnBox.ChildNodes.Count;
                    }

                    m_nTotalNum += m_nTypeMeterCount;
                    //m_dTotalPrice += m_dPrice * m_nTypeMeterCount;


                    dr = dt.NewRow();
                    dr["����"] = m_szType;
                    dr["����"] = m_szCaliber;
                    dr["��λ"] = "ֻ";
                    //dr["����"] = m_dPrice.ToString("F2");
                    dr["����"] = m_nTypeMeterCount;
                    //dr["���"] = "��" + Convert.ToDecimal(m_dPrice * m_nTypeMeterCount).ToString("F2");


                    dt.Rows.Add(dr);
                }
                catch
                {
                    MessageBox.Show("�ļ������Ϲ���������ѡ����������ѡ��");
                    return dt;
                }
            }
            if (xnl.Count != 0)
            {
                dr = dt.NewRow();
                dr["����"] = "�ϼ�";
                dr["����"] = "";
                dr["��λ"] = "";
                dr["����"] = m_nTotalNum;
                //dr["���"] = "��" +  m_dTotalPrice.ToString("F2");
                dt.Rows.Add(dr);
            }
            else
            {
                MessageBox.Show("������ѡ���ļ������ļ���û�з��Ϲ������Ϣ");
            }

            return dt;
        }
        #endregion

        #region ���ú���

        /// <summary>
        /// �ƶ˻�ȡ�����ļ�
        /// </summary>
        public void DownXMLFile()
        {


        }

        /// <summary>
        /// ���³���汾
        /// </summary>
        public void UpdateVersion()
        {
            try
            {
                if (ini.IniReadValue("JTProviderConfig", "ChangJiaXMLFileUrl") == "")
                    ini.IniWriteValue("JTProviderConfig", "ChangJiaXMLFileUrl", "http://212.64.20.54:10086/ProviderDate.asmx");
                if (ini.IniReadValue("JTProviderConfig", "ChangJiaXMLFileWaiWangUrl") == "")
                    ini.IniWriteValue("JTProviderConfig", "ChangJiaXMLFileWaiWangUrl", "http://122.152.213.240:29001/ProviderDate_JT.asmx");
                if (ini.IniReadValue("JTProviderConfig", "ChangJiaXMLFileNeiWangUrl") == "")
                    ini.IniWriteValue("JTProviderConfig", "ChangJiaXMLFileNeiWangUrl", "http://192.168.127.70:10087/ProviderDate_JT.asmx");

                string PeiZhiUrl = ini.IniReadValue("JTProviderConfig", "ChangJiaXMLFileUrl").ToString().Trim();

                if (WangLuoType == 0) PeiZhiUrl = ini.IniReadValue("JTProviderConfig", "ChangJiaXMLFileNeiWangUrl").ToString().Trim();
                if (WangLuoType == 1) PeiZhiUrl = ini.IniReadValue("JTProviderConfig", "ChangJiaXMLFileWaiWangUrl").ToString().Trim();

                ProviderDate _webser = new ProviderDate(PeiZhiUrl);

                string _NewVersion = _webser.GetProviderVersion();
                VersionIsOld = _NewVersion == VersionStr ? false : true;

                if (VersionIsOld)
                {
                    DialogResult _DialogResult = MessageBox.Show("����汾��Ҫ���£��Ƿ����������°汾��", "�汾������ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (_DialogResult == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start("iexplore.exe", _webser.GetProviderFileUrl());
                        System.Environment.Exit(0);
                    }
                    else
                    {
                        MessageBox.Show("�ð汾����ͣʹ�ã������������°汾ʹ��", "�汾������ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        System.Environment.Exit(0);
                    }
                }
            }
            catch
            {
                MessageBox.Show("�����ƶ�ʧ�ܣ��������Ա��ϵ�� \r\n����������ģʽ���� ", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                WangLuoType = -1;
                //System.Environment.Exit(0);
            }
        }

        /// <summary>
        /// ��ʼ����������
        /// </summary>
        public void InitTestData()
        {
            int m_nMonth = DateTime.Now.Month;
            int m_nDay = DateTime.Now.Day;
            string m_szMonth = DateTime.Now.Month.ToString();
            string m_szDay = DateTime.Now.Day.ToString();
            if (m_nMonth < 10)
                m_szMonth = "0" + m_szMonth;
            if (m_nDay < 10)
                m_szDay = "0" + m_szDay;
            tbDate.Text = DateTime.Now.Year.ToString().Substring(2, 2) + m_szMonth + m_szDay;
            tbDateXZY.Text = DateTime.Now.Year.ToString().Substring(2, 2) + m_szMonth + m_szDay;
            nudPerNum.Value = 8;
        }

        /// <summary>
        /// ��ȡXML�ļ�����ȡ��صı�����/������Ϣ
        /// </summary>
        public Boolean InitDropDownList(string FileName)
        {
            try
            {
                //��XML�л�ȡ���б��ͺź�����
                XmlDocument xd = new XmlDocument();
                try
                {
                    xd.Load(FileName);
                }
                catch
                {
                    MessageBox.Show("�������ļ������ڻ��ʽ����ȷ������ϵ����Ա��");
                    //GetWaringXml();
                    return false;
                }

                #region

                List<string> list = new List<string>();
                List<string> list1 = new List<string>();
                List<string> list4 = new List<string>();
                List<string> list6 = new List<string>();
                list.Add(" ---��ѡ��--- ");
                list1.Add(" ---��ѡ��--- ");
                list4.Add(" ---��ѡ��--- ");
                list6.Add(" ---��ѡ��--- ");
                DataTable wireless = new DataTable();
                wireless.Columns.Add("Name", typeof(string));
                wireless.Columns.Add("Value", typeof(string));
                wireless.Rows.Add(" ---��ѡ��--- ", "-99999");

                DataTable iccard = new DataTable();
                iccard.Columns.Add("Name", typeof(string));
                iccard.Columns.Add("Value", typeof(string));
                iccard.Rows.Add(" ---��ѡ��--- ", "-99999");

                DataTable jishuqlx = new DataTable();
                jishuqlx.Columns.Add("Name", typeof(string));
                jishuqlx.Columns.Add("Value", typeof(string));
                jishuqlx.Rows.Add(" ---��ѡ��--- ", "-99999");

                DataTable gongdianfs = new DataTable();
                gongdianfs.Columns.Add("Name", typeof(string));
                gongdianfs.Columns.Add("Value", typeof(string));
                gongdianfs.Rows.Add(" ---��ѡ��--- ", "-99999");

                DataTable famenqk = new DataTable();
                famenqk.Columns.Add("Name", typeof(string));
                famenqk.Columns.Add("Value", typeof(string));
                famenqk.Rows.Add(" ---��ѡ��--- ", "-99999");

                DataTable jiekoulx = new DataTable();
                jiekoulx.Columns.Add("Name", typeof(string));
                jiekoulx.Columns.Add("Value", typeof(string));
                jiekoulx.Rows.Add(" ---��ѡ��--- ", "-99999");

                DataTable jibiaogys = new DataTable();
                jibiaogys.Columns.Add("Name", typeof(string));
                jibiaogys.Columns.Add("Value", typeof(string));
                jibiaogys.Rows.Add(" ---��ѡ��--- ", "-99999");

                DataTable shebeilx = new DataTable();
                shebeilx.Columns.Add("Name", typeof(string));
                shebeilx.Columns.Add("Value", typeof(string));
                shebeilx.Rows.Add(" ---��ѡ��--- ", "-99999");

                DataTable jinqifx = new DataTable();
                jinqifx.Columns.Add("Name", typeof(string));
                jinqifx.Columns.Add("Value", typeof(string));
                jinqifx.Rows.Add(" ---��ѡ��--- ", "-99999");



                List<string> listProvider = new List<string>();

                XmlNodeList xnsroot = xd.SelectSingleNode("root").ChildNodes;
                XmlNodeList xns;
                XmlNodeList xnsWireless;
                XmlNodeList xnsprovider;
                XmlNodeList xnsICCard;
                XmlNodeList xnsJiShuQLX;
                XmlNodeList xnsGongDianFS;
                XmlNodeList xnsFaMenQK;
                XmlNodeList xnsJieKouLX;
                XmlNodeList xnsJiBiaoGYS;
                XmlNodeList xnsSheBeiLX;
                XmlNodeList xnsJinQiFX;


                string Key = xd.DocumentElement.GetAttribute("Key");
                if (Key == string.Empty || Key.Length < 19 || Key.Substring(0, 1) + Key.Substring(9, 1) + Key.Substring(16, 3) != "JTGAS")
                {
                    if (MessageBox.Show("���������ļ������³������ɣ���ȷ���Ƿ��룿", "��ʾ", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        GetWaringXml();
                        return false;
                    }
                }


                foreach (XmlNode xnroot in xnsroot)
                {
                    XmlElement xe = (XmlElement)xnroot;
                    if (xe.GetAttribute("Text").ToUpper() == "MeterProperty".ToUpper())
                    {
                        xns = xnroot.ChildNodes;
                        foreach (XmlNode xn in xns)
                        {
                            //if (xn.Attributes["Class"].Value == "Small")
                            if (Convert.ToDecimal(xn.Attributes["Caliber"].Value) <= 6 && Convert.ToDecimal(xn.Attributes["Caliber"].Value) > 0)
                            {
                                string m_szType = xn.Attributes["Type"].Value;
                                decimal m_dCaliber = Convert.ToDecimal(xn.Attributes["Caliber"].Value);
                                int m_nID = Convert.ToInt32(xn.Attributes["Value"].Value);
                                list1.Add(m_szType + "/" + m_dCaliber);
                                list.Add(m_szType + "/" + m_dCaliber);
                            }
                            else if (Convert.ToDecimal(xn.Attributes["Caliber"].Value) >= 10)
                            //else if (xn.Attributes["Class"].Value == "Big")
                            {
                                string m_szType = xn.Attributes["Type"].Value;
                                decimal m_dCaliber = Convert.ToDecimal(xn.Attributes["Caliber"].Value);
                                int m_nID = Convert.ToInt32(xn.Attributes["Value"].Value);
                                list4.Add(m_szType + "/" + m_dCaliber);
                                list.Add(m_szType + "/" + m_dCaliber);
                            }
                            else
                            {
                                string m_szType = xn.Attributes["Type"].Value;
                                decimal m_dCaliber = Convert.ToDecimal(xn.Attributes["Caliber"].Value);
                                int m_nID = Convert.ToInt32(xn.Attributes["Value"].Value);
                                list6.Add(m_szType + "/" + m_dCaliber);
                                //list.Add(m_szType + "/" + m_dCaliber);
                            }
                        }
                    }
                    if (xe.GetAttribute("Text").ToUpper() == "WirelessProvider".ToUpper())
                    {
                        xnsWireless = xnroot.ChildNodes;
                        foreach (XmlNode xn in xnsWireless)
                        {
                            string wlName = xn.Attributes["Name"].Value;
                            string wlValue = xn.Attributes["Value"].Value;
                            wireless.Rows.Add(wlName, wlValue);
                        }
                    }
                    if (xe.GetAttribute("Text").ToUpper() == "Provider".ToUpper())
                    {
                        xnsprovider = xnroot.ChildNodes;
                        foreach (XmlNode xn in xnsprovider)
                        {
                            string m_szValue = xn.Attributes["Value"].Value;
                            string m_szBrand = xn.Attributes["Brand"].Value;
                            listProvider.Add(m_szValue + "/" + m_szBrand);
                        }
                        lblType.Text = xnroot.ChildNodes[0].Attributes["Value"].Value;
                        lblProvider.Text = xnroot.ChildNodes[0].Attributes["Brand"].Value;
                    }
                    if (xe.GetAttribute("Text").ToUpper() == "ICCardProvider".ToUpper())
                    {
                        xnsICCard = xnroot.ChildNodes;
                        foreach (XmlNode xn in xnsICCard)
                        {
                            string wlName = xn.Attributes["Name"].Value;
                            string wlValue = xn.Attributes["Value"].Value;
                            iccard.Rows.Add(wlName, wlValue);
                        }
                    }
                    if (xe.GetAttribute("Text").ToUpper() == "JiShuQLXProvider".ToUpper())
                    {
                        xnsJiShuQLX = xnroot.ChildNodes;
                        foreach (XmlNode xn in xnsJiShuQLX)
                        {
                            string wlName = xn.Attributes["Name"].Value;
                            string wlValue = xn.Attributes["Value"].Value;
                            jishuqlx.Rows.Add(wlName, wlValue);
                        }
                    }
                    if (xe.GetAttribute("Text").ToUpper() == "GongDianFSProvider".ToUpper())
                    {
                        xnsGongDianFS = xnroot.ChildNodes;
                        foreach (XmlNode xn in xnsGongDianFS)
                        {
                            string wlName = xn.Attributes["Name"].Value;
                            string wlValue = xn.Attributes["Value"].Value;
                            gongdianfs.Rows.Add(wlName, wlValue);
                        }
                    }
                    if (xe.GetAttribute("Text").ToUpper() == "FaMenLXProvider".ToUpper())
                    {
                        xnsFaMenQK = xnroot.ChildNodes;
                        foreach (XmlNode xn in xnsFaMenQK)
                        {
                            string wlName = xn.Attributes["Name"].Value;
                            string wlValue = xn.Attributes["Value"].Value;
                            famenqk.Rows.Add(wlName, wlValue);
                        }
                    }
                    if (xe.GetAttribute("Text").ToUpper() == "JieKouLXProvider".ToUpper())
                    {
                        xnsJieKouLX = xnroot.ChildNodes;
                        foreach (XmlNode xn in xnsJieKouLX)
                        {
                            string wlName = xn.Attributes["Name"].Value;
                            string wlValue = xn.Attributes["Value"].Value;
                            jiekoulx.Rows.Add(wlName, wlValue);
                        }
                    }
                    if (xe.GetAttribute("Text").ToUpper() == "JiBiaoGYSProvider".ToUpper())
                    {
                        xnsJiBiaoGYS = xnroot.ChildNodes;
                        foreach (XmlNode xn in xnsJiBiaoGYS)
                        {
                            string wlName = xn.Attributes["Name"].Value;
                            string wlValue = xn.Attributes["Value"].Value;
                            jibiaogys.Rows.Add(wlName, wlValue);
                        }
                    }

                    if (xe.GetAttribute("Text").ToUpper() == "SheBeiLXProvider".ToUpper())
                    {
                        xnsSheBeiLX = xnroot.ChildNodes;
                        foreach (XmlNode xn in xnsSheBeiLX)
                        {
                            string wlName = xn.Attributes["Name"].Value;
                            string wlValue = xn.Attributes["Value"].Value;
                            shebeilx.Rows.Add(wlName, wlValue);
                        }
                    }
                    if (xe.GetAttribute("Text").ToUpper() == "JinQiFXProvider".ToUpper())
                    {
                        xnsJinQiFX = xnroot.ChildNodes;
                        foreach (XmlNode xn in xnsJinQiFX)
                        {
                            string wlName = xn.Attributes["Name"].Value;
                            string wlValue = xn.Attributes["Value"].Value;
                            jinqifx.Rows.Add(wlName, wlValue);
                        }
                    }
                }


                //ͬʱ��Ӷ����ı��ͺŰ�
                cbbMultiType.DataSource = list1;


                //��ӵ�����ı��ͺŰ�
                List<string> list2 = new List<string>();
                List<string> list3 = new List<string>();
                List<string> list5 = new List<string>();

                foreach (string item in list1)
                {
                    string item1 = item;
                    list2.Add(item1);
                }
                foreach (string item in list)
                {
                    list3.Add(item);
                    list5.Add(item);
                }

                cbbOnlyType.DataSource = list2;
                cbbXiuZhengYXH.DataSource = list6;
                //��ӵ�����ı��ͺŰ�
                cbbXFMultiType.DataSource = list3;
                //��ӵ�����ı��ͺŰ�
                cbbXFOnlyType.DataSource = list5;

                cbbOnlyTypeBig.DataSource = list4;

                cbbProvider.DataSource = listProvider;
                cbbWireless1.DataSource = wireless;
                cbbWireless1.DisplayMember = wireless.Columns[0].ColumnName;
                cbbWireless1.ValueMember = wireless.Columns[1].ColumnName;

                cbbWireless2.DataSource = wireless;
                cbbWireless2.DisplayMember = wireless.Columns[0].ColumnName;
                cbbWireless2.ValueMember = wireless.Columns[1].ColumnName;

                cbbICCard.DataSource = iccard;
                cbbICCard.DisplayMember = iccard.Columns[0].ColumnName;
                cbbICCard.ValueMember = iccard.Columns[1].ColumnName;

                cbbJiShuQLX.DataSource = jishuqlx;
                cbbJiShuQLX.DisplayMember = jishuqlx.Columns[0].ColumnName;
                cbbJiShuQLX.ValueMember = jishuqlx.Columns[1].ColumnName;

                cbbGongDianFS.DataSource = gongdianfs;
                cbbGongDianFS.DisplayMember = gongdianfs.Columns[0].ColumnName;
                cbbGongDianFS.ValueMember = gongdianfs.Columns[1].ColumnName;

                cbbFaMenQK.DataSource = famenqk;
                cbbFaMenQK.DisplayMember = famenqk.Columns[0].ColumnName;
                cbbFaMenQK.ValueMember = famenqk.Columns[1].ColumnName;

                cbbJieKouLX.DataSource = jiekoulx;
                cbbJieKouLX.DisplayMember = jiekoulx.Columns[0].ColumnName;
                cbbJieKouLX.ValueMember = jiekoulx.Columns[1].ColumnName;

                cbbJiBiaoGYS.DataSource = jibiaogys;
                cbbJiBiaoGYS.DisplayMember = jibiaogys.Columns[0].ColumnName;
                cbbJiBiaoGYS.ValueMember = jibiaogys.Columns[1].ColumnName;

                cbbSheBeiLX.DataSource = shebeilx;
                cbbSheBeiLX.DisplayMember = shebeilx.Columns[0].ColumnName;
                cbbSheBeiLX.ValueMember = shebeilx.Columns[1].ColumnName;

                cbbJinQiFX.DataSource = jinqifx;
                cbbJinQiFX.DisplayMember = jinqifx.Columns[0].ColumnName;
                cbbJinQiFX.ValueMember = jinqifx.Columns[1].ColumnName;

                cbbProviderXZY.DataSource = listProvider;
                #endregion

                if (list.Count == 1)
                {
                    MessageBox.Show("�������ļ������ڻ��ʽ����ȷ������ϵ����Ա��");
                    //GetWaringXml();
                    return false;
                    //Environment.Exit(0);
                }
                if (listProvider.Count == 0)
                {
                    MessageBox.Show("�������ļ������ڻ��ʽ����ȷ������ϵ����Ա��");
                    //GetWaringXml();
                    return false;
                    //Environment.Exit(0);
                }
                return true;
            }
            catch
            {
                MessageBox.Show("�������ļ������ڻ��ʽ����ȷ������ϵ����Ա��");
                //GetWaringXml();
                //Environment.Exit(0);
                return false;
            }
        }

        /// <summary>
        /// ����ѡ��XML�����ļ���
        /// </summary>
        /// <param name="xd"></param>
        public void GetWaringXml()
        {
            Form2 form = new Form2();
            if (form.ShowDialog() == DialogResult.OK)
            {
                InitDropDownList(form.openFileDialog1.FileName);
            }
            else
                Environment.Exit(0);
        }

        /// <summary>
        /// �����ַ�����ȡ��Ӧ�ı����ͣ�֮����"/"�ָ��
        /// </summary>
        /// <param name="TypeCaliber"></param>
        /// <returns></returns>
        public string GetTypeByTypeCaliber(string TypeCaliber)
        {
            int m_nIndex = 0;
            m_nIndex = TypeCaliber.LastIndexOf("/");
            return TypeCaliber.Substring(0, m_nIndex);
        }

        /// <summary>
        /// �����ַ�����ȡ��Ӧ�ı�������֮����"/"�ָ��
        /// </summary>
        /// <param name="TypeCaliber"></param>
        /// <returns></returns>
        public string GetCaliberByTypeCaliber(string TypeCaliber)
        {
            int m_nIndex = 0;
            m_nIndex = TypeCaliber.LastIndexOf("/");
            return TypeCaliber.Substring(m_nIndex + 1, TypeCaliber.Length - m_nIndex - 1);
        }

        /// <summary>
        /// ������ʼ�Ż�ȡ���еı��
        /// </summary>
        /// <param name="m_szStartMeterID"></param>
        /// <param name="m_nCount"></param>
        /// <returns></returns>
        public List<string> PopulateMeterID(string m_szStartMeterID, int m_nCount)
        {
            List<string> list = new List<string>();
            string m_szStartstr = m_szStartMeterID.Substring(0, 4);
            int m_nStartMeterID = Convert.ToInt32(m_szStartMeterID.Substring(4, 6));
            for (int i = 0; i < m_nCount; i++)
            {
                int m_nMeterID = m_nStartMeterID + i;
                string m_szMeterID = m_nMeterID.ToString();
                while (m_szMeterID.Length < 6)
                    m_szMeterID = "0" + m_szMeterID;
                string m_szNewMeterID = m_szStartstr + m_szMeterID;
                list.Add(m_szNewMeterID);
            }
            return list;
        }

        /// <summary>
        /// ������ʼ�Ż�ȡ���е����߱��
        /// </summary>
        /// <param name="m_szStartMeterID"></param>
        /// <param name="m_nCount"></param>
        /// <returns></returns>
        public List<string> PopulateWireMeterID(string m_szStartMeterID, int m_nCount)
        {
            List<string> list = new List<string>();
            string m_szStartstr = GetWireStartBH();
            int m_nStartMeterID = 0;
            if (GetWireStartBH() != "")
                m_nStartMeterID = Convert.ToInt32(tbWireMeterID.Text.Trim());
            for (int i = 0; i < m_nCount; i++)
            {
                int m_nMeterID = m_nStartMeterID + i;
                string m_szMeterID = m_nMeterID.ToString();
                while (m_szMeterID.Length < 6)
                    m_szMeterID = "0" + m_szMeterID;
                string m_szNewMeterID = "";
                if (m_szStartstr == "")
                    m_szNewMeterID = "";
                else
                {
                    m_szNewMeterID = m_szStartstr + DateTime.Now.Year.ToString().Substring(2, 2) + m_szMeterID;
                    m_szNewMeterID = m_szNewMeterID + GetJiaoYanW(m_szNewMeterID);
                }

                list.Add(m_szNewMeterID);
            }
            return list;
        }

        public void SearchBrother(TreeNode treenode)
        {
            if (treenode.Parent != null)
            {
                int m_nCount = 0;
                foreach (TreeNode tn in treenode.Parent.Nodes)
                {
                    if (tn.Checked != true)
                    {
                        m_nCount = 1;
                        break;
                    }
                }
                if (m_nCount == 0)
                    treenode.Parent.Checked = true;

                SearchBrother(treenode.Parent);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void SearchParent(TreeNode treenode)
        {
            if (treenode.Parent != null)
            {
                treenode.Parent.Checked = treenode.Checked ? treenode.Parent.Checked : false;
                SearchParent(treenode.Parent);
            }

        }

        #region ��֤����
        /// <summary>
        /// ��֤����������ѡ���Ƿ���ȷ
        /// </summary>
        /// <param name="m_szTypeName"></param>
        /// <returns></returns>
        public Boolean CheckTypeName(string m_szTypeName)
        {
            string m_szMessage = "δѡ���������ͺţ���ѡ��������";

            if (m_szTypeName == " ---��ѡ��--- ")
            {
                MessageBox.Show(m_szMessage);
                return false;
            }
            return true;
        }

        /// <summary>
        /// ��֤��ĵ����Ƿ�ΪDecimal����
        /// </summary>
        //public Boolean CheckPriceName(string m_szPrice)
        //{
        //    decimal m_dPrice = 0;
        //    string m_szMessage = "�������Ͳ�������������д��ĵ���";
        //    try
        //    {
        //        int m_nIndex = m_szPrice.IndexOf(".");
        //        string m_szTemp = "";
        //        string m_szTempLast = "";
        //        if (m_nIndex > 0)
        //        {
        //            m_szTemp = m_szPrice.Substring(0, m_nIndex);
        //            m_szTempLast = m_szPrice.Substring(m_nIndex + 1, m_szPrice.Length - m_nIndex - 1);
        //        }
        //        else
        //            m_szTemp = m_szPrice;

        //        if (m_szTemp.Length > 7)
        //        {
        //            MessageBox.Show("���۳��ȹ�������������д��ĵ���");
        //            return false;
        //        }

        //        if (m_szTempLast.Length>2)
        //        {
        //            MessageBox.Show("����С��λ�����������Ϊ��λ");
        //            return false;
        //        }
        //        m_dPrice = Convert.ToDecimal(m_szPrice);

        //        if (m_dPrice <= 0)
        //        {
        //            MessageBox.Show("���۲���Ϊ�������򣰣�");
        //            return false;
        //        }
        //        return true;
        //    }
        //    catch
        //    {
        //        MessageBox.Show(m_szMessage);
        //        return false;
        //    }
        //}

        /// <summary>
        /// ��ⵥ��¼������߱��
        /// </summary>
        /// <param name="m_szWireMeterName"></param>
        /// <returns></returns>
        public bool CheckOnlyWireMeterName(string m_szWireMeterName)
        {
            string m_szMessage = "���߱�Ų����Ϲ�����������д��ţ�";
            string m_szMeterID = m_szWireMeterName;
            try
            {
                if (GetWireStartBH() == "")
                    return true;
                if (m_szWireMeterName.Length != 11)
                {
                    MessageBox.Show("���߱�ų��Ȳ���ȷ������������!");
                    return false;
                }
                string m_wire = m_szMeterID.Substring(0, 2);
                string m_jiaoyan = m_szMeterID.Substring(10, 1);
                int m_nYear = Convert.ToInt32(m_szMeterID.Substring(2, 2));
                int m_nCount = Convert.ToInt32(m_szMeterID.Substring(4, 6));
                if (m_nCount <= 0)
                {
                    MessageBox.Show("���߱�ű�Ų���ȷ������������!");
                    return false;
                }
                if (DateTime.Now.Year.ToString().Substring(2, 2) != m_nYear.ToString())
                {
                    MessageBox.Show("���߱����ݲ���ȷ������������!");
                    return false;
                }
                if (GetWireStartBH() != m_wire)
                {
                    MessageBox.Show("���߱�ų��Ҳ���ȷ������������!");
                    return false;
                }
                if (GetJiaoYanW(m_szWireMeterName) != m_jiaoyan)
                {
                    MessageBox.Show("���߱��У��λ����ȷ������������!");
                    return false;
                }
                return true;
            }
            catch
            {
                MessageBox.Show(m_szMessage);
                return false;
            }
        }

        /// <summary>
        /// �������¼��ʱ�����߱��
        /// </summary>
        /// <param name="m_szWireMeterName"></param>
        /// <returns></returns>
        public Boolean CheckWireMeterName(string m_szWireMeterName)
        {
            string m_szMessage = "���߱�Ų����Ϲ�����������д��ţ�";
            string m_szMeterID = m_szWireMeterName;
            //���ͬʱ��Ӷ�������ʼ���
            try
            {
                if (GetWireStartBH() == "")//���û�����߳�������Ҫ��֤
                    return true;
                if (m_szWireMeterName.Length != 6)
                {
                    MessageBox.Show("���߱�ų��Ȳ���ȷ������������!");
                    return false;
                }

                int m_nCount = Convert.ToInt32(m_szMeterID);
                if (m_nCount <= 0)
                {
                    MessageBox.Show("���߱�Ų���ȷ������������!");
                    return false;
                }


                if (m_szWireMeterName.Trim().Length < 6)
                {
                    MessageBox.Show("���߱�Ų����пո�");
                    return false;
                }
                return true;
            }
            catch
            {
                MessageBox.Show(m_szMessage);
                return false;
            }
        }

        /// <summary>
        /// ��֤��������Ƿ���Ϲ���
        /// </summary>
        public Boolean CheckMeterName(string m_szMeterName)
        {
            string m_szMessage = "��Ų����Ϲ�����������д��ţ�";
            string m_szMeterID = m_szMeterName;
            string m_szChangJia = cbbProvider.Text.Substring(0, 2);
            //���ͬʱ��Ӷ�������ʼ���
            try
            {
                if (m_szMeterName.Length != 10)
                {
                    MessageBox.Show("��ų��Ȳ���ȷ������������!");
                    return false;
                }
                string m_szType = m_szMeterID.Substring(0, 1);
                string m_szCaliber = m_szMeterID.Substring(1, 1);

                int m_nYear = Convert.ToInt32(m_szMeterID.Substring(2, 2));
                int m_nNum = Convert.ToInt32(m_szMeterID.Substring(4, 6));
                string m_szFirst = m_szMeterID.Substring(0, 2);

                if (m_szFirst != m_szChangJia)
                {
                    MessageBox.Show("���ǰ2λ���ұ�Ų���ȷ������������!");
                    return false;
                }

                int m_nCount = Convert.ToInt32(m_szMeterID.Substring(2, 8));
                if (m_nCount <= 0)
                {
                    MessageBox.Show("��ź�8λ����ȷ������������!");
                    return false;
                }

                string IS_WORD = @"(^([a-zA-Z]))";
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(IS_WORD);
                //if (!regex.IsMatch(m_szMeterID.Substring(0, 1)))
                //{
                //    MessageBox.Show("��ŵ�һλ����Ϊ��ĸ��");
                //    return false;
                //}
                //if (!regex.IsMatch(m_szMeterID.Substring(1, 1)))
                //{
                //    MessageBox.Show("��ŵڶ�λ����Ϊ��ĸ��");
                //    return false;
                //}
                if (m_szMeterName.Substring(0, 10).Trim().Length < 10)
                {
                    MessageBox.Show("��Ų����пո�");
                    return false;
                }
                if (m_szMeterName.Substring(4, 2).Trim() == "99")
                {
                    MessageBox.Show("��ű�Ŵ���");
                    return false;
                }
                return true;
            }
            catch
            {
                MessageBox.Show(m_szMessage);
                return false;
            }
        }

        /// <summary>
        /// ��֤�����Ǻ������Ƿ���Ϲ���
        /// </summary>
        public Boolean CheckXZYName(string m_szMeterName)
        {
            string m_szMessage = "��Ų����Ϲ�����������д��ţ�";
            string m_szMeterID = m_szMeterName;
            string m_szChangJia = cbbProviderXZY.Text.Substring(0, 2);
            //���ͬʱ��Ӷ�������ʼ���
            try
            {
                if (m_szMeterName.Length != 10)
                {
                    MessageBox.Show("��ų��Ȳ���ȷ������������!");
                    return false;
                }
                string m_szType = m_szMeterID.Substring(0, 1);
                string m_szCaliber = m_szMeterID.Substring(1, 1);

                int m_nYear = Convert.ToInt32(m_szMeterID.Substring(2, 2));
                int m_nNum = Convert.ToInt32(m_szMeterID.Substring(4, 6));
                string m_szFirst = m_szMeterID.Substring(0, 2);

                if (m_szFirst != m_szChangJia)
                {
                    MessageBox.Show("���ǰ��λ���ұ�Ų���ȷ������������!");
                    return false;
                }

                int m_nCount = Convert.ToInt32(m_szMeterID.Substring(2, 8));
                if (m_nCount <= 0)
                {
                    MessageBox.Show("��ź�8λ����ȷ������������!");
                    return false;
                }

                string IS_WORD = @"(^([a-zA-Z]))";
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(IS_WORD);
                //if (!regex.IsMatch(m_szMeterID.Substring(0, 1)))
                //{
                //    MessageBox.Show("��ŵ�һλ����Ϊ��ĸ��");
                //    return false;
                //}
                //if (!regex.IsMatch(m_szMeterID.Substring(1, 1)))
                //{
                //    MessageBox.Show("��ŵڶ�λ����Ϊ��ĸ��");
                //    return false;
                //}
                if (m_szMeterName.Substring(0, 10).Trim().Length < 10)
                {
                    MessageBox.Show("��Ų����пո�");
                    return false;
                }
                if (m_szMeterName.Substring(4, 2).Trim() != "99")
                {
                    MessageBox.Show("�����Ǳ�Ŵ���");
                    return false;
                }
                return true;
            }
            catch
            {
                MessageBox.Show(m_szMessage);
                return false;
            }
        }

        /// <summary>
        /// ��֤�����������Ƿ���Ϲ���
        /// </summary>
        public Boolean CheckTxmName(string m_szMeterName)
        {
            string m_szMessage = "�����벻���Ϲ�����������д��ţ�";
            string m_szMeterID = m_szMeterName;
            //���ͬʱ��Ӷ�������ʼ���
            try
            {
                if (m_szMeterName.Length != 10)
                {
                    MessageBox.Show("�����볤�Ȳ���ȷ������������!");
                    return false;
                }
                string m_szType = m_szMeterID.Substring(0, 1);
                string m_szCaliber = m_szMeterID.Substring(1, 1);

                int m_nYear = Convert.ToInt32(m_szMeterID.Substring(2, 2));
                int m_nNum = Convert.ToInt32(m_szMeterID.Substring(4, 6));
                string m_szFirst = m_szMeterID.Substring(0, 1);

                int m_nCount = Convert.ToInt32(m_szMeterID.Substring(2, 8));
                if (m_nCount <= 0)
                {
                    MessageBox.Show("�������8λ����ȷ������������!");
                    return false;
                }

                string IS_WORD = @"(^([a-zA-Z]))";
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(IS_WORD);
                if (!regex.IsMatch(m_szMeterID.Substring(0, 1)))
                {
                    MessageBox.Show("�������һλ����Ϊ��ĸ��");
                    return false;
                }
                if (!regex.IsMatch(m_szMeterID.Substring(1, 1)))
                {
                    MessageBox.Show("������ڶ�λ����Ϊ��ĸ��");
                    return false;
                }
                if (m_szMeterName.Substring(2, 8).Trim().Length < 8)
                {
                    MessageBox.Show("�����벻���пո�");
                    return false;
                }
                return true;
            }
            catch
            {
                MessageBox.Show(m_szMessage);
                return false;
            }
        }

        /// <summary>
        /// ��֤����Ƿ���Ϲ���
        /// </summary>
        public Boolean CheckBoxName(string m_szBoxID)
        {
            string m_szMessage = "��Ų����Ϲ�����������д���";
            try
            {
                if (m_szBoxID.Length != 4)
                {
                    MessageBox.Show("��ų��Ȳ���ȷ����������д������4λ����ǰ�油0��");
                    return false;
                }
                if (m_szBoxID.Substring(0, 4).Trim().Length != 4)
                {
                    MessageBox.Show("��Ų����пո�");
                    return false;
                }
                int m_nBoxID = Convert.ToInt32(m_szBoxID.Substring(0, 4));
                if (m_nBoxID <= 0)
                {
                    MessageBox.Show("��Ų���Ϊ��������0��");
                    return false;
                }
                return true;
            }
            catch
            {
                MessageBox.Show(m_szMessage);
                return false;
            }
        }

        /// <summary>
        /// ��֤���������Ƿ���Ϲ���
        /// </summary>
        public Boolean CheckNY(string m_szNY)
        {
            string m_szMessage = "�������²����Ϲ�����������д��������";
            try
            {
                if (m_szNY.Length != 6)
                {
                    MessageBox.Show("�������³��Ȳ���ȷ����������д������ӦΪ4λ���+2λ�·ݣ�");
                    return false;
                }
                if (m_szNY.Substring(0, 6).Trim().Length != 6)
                {
                    MessageBox.Show("�������²����пո�");
                    return false;
                }
                int m_nYear = Convert.ToInt32(m_szNY.Substring(0, 4));
                if (m_nYear <= 1900)
                {
                    MessageBox.Show("����������ݴ���");
                    return false;
                }
                int m_nMonth = Convert.ToInt32(m_szNY.Substring(4, 2));
                if (m_nMonth > 12 || m_nMonth <= 0)
                {
                    MessageBox.Show("�����������ݸ�ʽ����");
                    return false;
                }
                return true;
            }
            catch
            {
                MessageBox.Show(m_szMessage);
                return false;
            }
        }

        /// <summary>
        /// ��֤������Ϣ�Ƿ�ѡ��
        /// </summary>
        public Boolean CheckJBXX()
        {
            string m_szMessage = "������Ϣ���������벹�����¼��";
            try
            {
                if (cbbWireless1.Text == " ---��ѡ��--- " && biaolx == 0)
                {
                    MessageBox.Show("��ѡ�����߳��̣�");
                    return false;
                }
                if (cbbJieKouLX.Text == " ---��ѡ��--- ")
                {
                    MessageBox.Show("��ѡ��ӿ����ͣ�");
                    return false;
                }
                if (cbbJinQiFX.Text == " ---��ѡ��--- ")
                {
                    MessageBox.Show("��ѡ���������");
                    return false;
                }
                if (cbbJiShuQLX.Text == " ---��ѡ��--- ")
                {
                    MessageBox.Show("��ѡ����������ͣ�");
                    return false;
                }
                if (cbbSheBeiLX.Text == " ---��ѡ��--- " && biaolx == 0)
                {
                    MessageBox.Show("��ѡ���豸���ͣ�");
                    return false;
                }
                if (cbbGongDianFS.Text == " ---��ѡ��--- ")
                {
                    MessageBox.Show("��ѡ�񹩵緽ʽ��");
                    return false;
                }
                if (cbbICCard.Text == " ---��ѡ��--- " && biaolx == 0)
                {
                    MessageBox.Show("��ѡ��IC�����ͣ�");
                    return false;
                }
                if (cbbJiBiaoGYS.Text == " ---��ѡ��--- " && biaolx == 0)
                {
                    MessageBox.Show("��ѡ�����Ӧ�̣�");
                    return false;
                }
                if (cbbFaMenQK.Text == " ---��ѡ��--- " && biaolx == 0)
                {
                    MessageBox.Show("��ѡ���������");
                    return false;
                }
                return true;
            }
            catch
            {
                MessageBox.Show(m_szMessage);
                return false;
            }
        }

        /// <summary>
        /// ��֤���κ�
        /// �ж��Ƿ���ڲ�ͬ�����κ�
        /// ������ڲ�ͬ���κţ����õ�������ʾ�����ȷ�ϼ����������ԭ�ȼ�¼
        /// ������������κţ��������κ�
        /// </summary>
        /// <param name="m_szSerialNum"></param>
        /// <returns>true��ʾ���Լ�����false��ʾ����</returns>
        public Boolean CheckSerialNum(XmlDocument m_xmldocument, string m_szSerialNum)
        {
            //�����ж�m_szSerialNum�Ƿ�Ϊ�գ�����ֱ�ӷ���
            if (m_szSerialNum == null || m_szSerialNum.Trim() == "")
            {
                MessageBox.Show("���κŲ���Ϊ�գ�����д�������");
                return false;
            }

            if (m_xmldocument == xdnew)
            {
                try
                {
                    if (m_szSerialNum.Length != 10)
                    {
                        MessageBox.Show("���κų��Ȳ���ȷ���м�Ϊ6λ�����Ϊ2λ��λ����������ǰ�油0");
                        return false;
                    }
                    if (m_szSerialNum.Substring(4, 2).Trim().Length < 2)
                    {
                        MessageBox.Show("���κ���д���ԣ������пո�");
                        return false;
                    }
                    if (m_szSerialNum.Substring(6, 2).Trim().Length < 2)
                    {
                        MessageBox.Show("���κ���д���ԣ������пո�");
                        return false;
                    }
                    if (m_szSerialNum.Substring(8, 2).Trim().Length < 2)
                    {
                        MessageBox.Show("���κ���д���ԣ������пո�");
                        return false;
                    }
                    int m_nYear = Convert.ToInt32(m_szSerialNum.Substring(2, 2));
                    int m_nMonth = Convert.ToInt32(m_szSerialNum.Substring(4, 2));
                    int m_nDay = Convert.ToInt32(m_szSerialNum.Substring(6, 2));
                    if (m_nMonth > 12 || m_nMonth < 1)
                    {
                        MessageBox.Show("���κŲ���ȷ����鿴���ֵĵ�3-4");
                        return false;
                    }
                    if (m_nDay > 31 || m_nDay < 1)
                    {
                        MessageBox.Show("���κŲ���ȷ����鿴���ֵĵ�5-6");
                        return false;
                    }
                    int m_nNum = Convert.ToInt32(m_szSerialNum.Substring(8, 2));
                    if (m_nYear < 0 || m_nNum <= 0)
                    {
                        MessageBox.Show("���κŲ���ȷ");
                        return false;
                    }
                }
                catch
                {
                    MessageBox.Show("���κŲ���ȷ");
                    return false;
                }
            }

            if (m_xmldocument == xdupdate)
            {
                try
                {
                    if (m_szSerialNum.Length != 11)
                    {
                        MessageBox.Show("ʧЧ���ͻ����ų��Ȳ���ȷ����ʽΪ\"SX\"+9λ����!");
                        return false;
                    }
                    if (m_szSerialNum.Substring(0, 2).ToLower() == "SX".ToLower())
                    {
                        int m_nNum = Convert.ToInt32(m_szSerialNum.Substring(2, 9));
                    }
                    else
                    {
                        MessageBox.Show("ʧЧ���ͻ����Ų���ȷ!");
                        return false;
                    }
                    if (m_szSerialNum.Substring(2, 9).Trim().Length < 9)
                    {
                        MessageBox.Show("ʧЧ���ͻ����Ų���ȷ,��9λ�����пո�!");
                        return false;
                    }
                    if (Convert.ToInt32(m_szSerialNum.Substring(2, 9)) <= 0)
                    {
                        MessageBox.Show("ʧЧ���ͻ����Ų���ȷ,��9λ����Ϊ������0!");
                        return false;
                    }
                }
                catch
                {
                    MessageBox.Show("ʧЧ���ͻ����Ų���ȷ");
                    return false;
                }
            }



            //�������κŴ��ڵ�״̬
            int m_nExistsCount = 0; //���κŴ���״̬ 0:���������κţ���:�������κ�
            XmlNode xmlnode = m_xmldocument.SelectSingleNode("root");
            if (xmlnode != null)
            {
                m_nExistsCount = 1;
            }

            if (m_nExistsCount == 0)
            {
                //���������κţ��������κţ�Ȼ�󷵻�true
                XmlElement newxmlelement = m_xmldocument.CreateElement("root");
                if (m_xmldocument == xdupdate)
                    newxmlelement.SetAttribute("Case", "1");
                else
                    newxmlelement.SetAttribute("Case", "0");
                newxmlelement.SetAttribute("serialNo", "");
                newxmlelement.SetAttribute("orderNo", "");
                m_xmldocument.AppendChild(newxmlelement);
            }

            return true;

        }

        #endregion

        #endregion


        #endregion

        #region �¼�

        private void btnAddNewMulti_Click(object sender, EventArgs e)
        {
            if (CheckMultiButton())
            {
                //�ӽ����л�ȡ����
                string m_szTypeCaliber = cbbMultiType.SelectedValue.ToString();
                string m_szType = GetTypeByTypeCaliber(m_szTypeCaliber);
                string m_szCaliber = GetCaliberByTypeCaliber(m_szTypeCaliber);
                string m_szWireMeterID = tbWireMeterID.Text.ToString();
                //decimal m_dPrice = Convert.ToDecimal(tbMultiPrice.Text.ToString());
                string m_szStartBoxID = tbMultiStartBoxID.Text.ToString().Trim();
                string m_szStartMeterID = tbMultiMeterStartID.Text.ToString().Trim().ToUpper();
                int m_nNum = Convert.ToInt32(nudMultiNum.Value);
                int m_nPerNum = Convert.ToInt32(nudPerNum.Value);
                string m_szSerialNum = cbbProvider.SelectedValue.ToString().Substring(0, 2) + tbDate.Text.ToString() + tbSerialNum.Text.ToString();


                List<string> m_BoxIDList = PopulateBoxID(m_szStartBoxID, m_nNum, m_nPerNum);
                List<string> m_MeterIDList = PopulateMeterID(m_szStartMeterID, m_nNum);
                //List<string> m_WireMeterIDList = PopulateWireMeterID(m_szWireMeterID, m_nNum);
                if (CheckList(m_BoxIDList, m_MeterIDList, m_szType, m_szCaliber, m_MeterIDList))//, m_dPrice))m_WireMeterIDList
                {
                    AddMultiToXml(m_szType, m_szCaliber//, m_dPrice
                        , m_BoxIDList, m_MeterIDList, m_nPerNum, m_MeterIDList);//m_WireMeterIDList
                }
                tvXml.Nodes.Clear();
                AddXmlToTree();
                lblNewMessage.Text = GetLabelMessage();
            }
        }

        private void btnAddNewOnly_Click(object sender, EventArgs e)
        {
            if (CheckOnlyButton())
            {
                //�ӽ����л�ȡ����
                string m_szTypeCaliber = cbbOnlyType.SelectedValue.ToString();
                string m_szType = GetTypeByTypeCaliber(m_szTypeCaliber);
                string m_szCaliber = GetCaliberByTypeCaliber(m_szTypeCaliber);
                //decimal m_dPrice = Convert.ToDecimal(tbOnlyPrice.Text.ToString());
                string m_szBoxID = tbOnlyBoxID.Text.ToString();
                string m_szTongXinH = tbTongXinH2.Text.Trim().ToString();
                string m_szMeterID = tbOnlyMeterID.Text.ToString().Trim().ToUpper();
                string m_szSerialNum = cbbProvider.SelectedValue.ToString().Substring(0, 2) + tbDate.Text.ToString() + tbSerialNum.Text.ToString();
                //��ȡ����ID
                string m_szStartstr = GetWireStartBH();

                string m_szWireMeterID = "";
                if (m_szStartstr != "")
                    m_szWireMeterID = tbOnlyWireSmall.Text.Trim();

                AddOnlyToXml(m_szType, m_szCaliber//, m_dPrice
                    , m_szBoxID, m_szMeterID, m_szMeterID, m_szMeterID, string.Empty, m_szTongXinH);//m_szWireMeterID
                tvXml.Nodes.Clear();
                AddXmlToTree();
                ExpandTree(m_szBoxID);
                lblNewMessage.Text = GetLabelMessage();
            }
        }

        private string GetWireStartBH()
        {
            string m_szStartstr = "";
            switch (cbbWireless1.SelectedValue.ToString())
            {
                case "1"://���(����)
                    m_szStartstr = "09";
                    break;
                case "2"://�׸�
                    m_szStartstr = "08";
                    break;
                case "3"://���ڿƼ�
                    m_szStartstr = "07";
                    break;
                case "4"://ȼ������
                    m_szStartstr = "";
                    break;
                case "5"://�ɶ�ǧ��
                    m_szStartstr = "06";
                    break;
                case "6"://�ɴ�
                    m_szStartstr = "03";
                    break;
                case "7"://˼��
                    m_szStartstr = "04";
                    break;
                case "8"://����
                    m_szStartstr = "05";
                    break;
                default:
                    m_szStartstr = "";
                    break;
            }
            return m_szStartstr;
        }

        /// <summary>
        /// �������߱�Ż�ȡУ��λ
        /// </summary>
        /// <param name="m_WireMeterID"></param>
        /// <returns></returns>
        public string GetJiaoYanW(string m_WireMeterID)
        {
            int shuz = 0;
            int tmp = 0;
            int yushu = 0;
            string jiaoyanm = "";
            for (int i = 0; i < m_WireMeterID.Substring(0, 10).Length; i++)
            {
                tmp = Convert.ToInt16(m_WireMeterID.Substring(i, 1));
                shuz += tmp;
            }
            yushu = shuz % 43;
            switch (yushu)
            {
                case 0:
                    jiaoyanm = "0";
                    break;
                case 1:
                    jiaoyanm = "1";
                    break;
                case 2:
                    jiaoyanm = "2";
                    break;
                case 3:
                    jiaoyanm = "3";
                    break;
                case 4:
                    jiaoyanm = "4";
                    break;
                case 5:
                    jiaoyanm = "5";
                    break;
                case 6:
                    jiaoyanm = "6";
                    break;
                case 7:
                    jiaoyanm = "7";
                    break;
                case 8:
                    jiaoyanm = "8";
                    break;
                case 9:
                    jiaoyanm = "9";
                    break;
                case 10:
                    jiaoyanm = "A";
                    break;
                case 11:
                    jiaoyanm = "B";
                    break;
                case 12:
                    jiaoyanm = "C";
                    break;
                case 13:
                    jiaoyanm = "D";
                    break;
                case 14:
                    jiaoyanm = "E";
                    break;
                case 15:
                    jiaoyanm = "F";
                    break;
                case 16:
                    jiaoyanm = "G";
                    break;
                case 17:
                    jiaoyanm = "H";
                    break;
                case 18:
                    jiaoyanm = "I";
                    break;
                case 19:
                    jiaoyanm = "J";
                    break;
                case 20:
                    jiaoyanm = "K";
                    break;
                case 21:
                    jiaoyanm = "L";
                    break;
                case 22:
                    jiaoyanm = "M";
                    break;
                case 23:
                    jiaoyanm = "N";
                    break;
                case 24:
                    jiaoyanm = "O";
                    break;
                case 25:
                    jiaoyanm = "P";
                    break;
                case 26:
                    jiaoyanm = "Q";
                    break;
                case 27:
                    jiaoyanm = "R";
                    break;
                case 28:
                    jiaoyanm = "S";
                    break;
                case 29:
                    jiaoyanm = "T";
                    break;
                case 30:
                    jiaoyanm = "U";
                    break;
                case 31:
                    jiaoyanm = "V";
                    break;
                case 32:
                    jiaoyanm = "W";
                    break;
                case 33:
                    jiaoyanm = "X";
                    break;
                case 34:
                    jiaoyanm = "Y";
                    break;
                case 35:
                    jiaoyanm = "Z";
                    break;
                case 36:
                    jiaoyanm = "-";
                    break;
                case 37:
                    jiaoyanm = ".";
                    break;
                case 38:
                    jiaoyanm = "#";
                    break;
                case 39:
                    jiaoyanm = "$";
                    break;
                case 40:
                    jiaoyanm = "/";
                    break;
                case 41:
                    jiaoyanm = "+";
                    break;
                case 42:
                    jiaoyanm = "%";
                    break;
            }
            return jiaoyanm;
        }

        private void tvXml_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Checked)
            {
                foreach (TreeNode tn in e.Node.Nodes)
                {
                    tn.Checked = true;
                }
                tvXml.AfterCheck -= this.tvXml_AfterCheck;
                SearchBrother(e.Node);
                tvXml.AfterCheck += this.tvXml_AfterCheck;
            }
            else
            {
                foreach (TreeNode tn in e.Node.Nodes)
                {
                    tn.Checked = false;
                }

                tvXml.AfterCheck -= this.tvXml_AfterCheck;
                SearchParent(e.Node);
                tvXml.AfterCheck += this.tvXml_AfterCheck;
            }
        }



        private void btnDelNew_Click(object sender, EventArgs e)
        {
            if (xdnew.SelectSingleNode("root") != null)
            {
                DeteleXmlFromTree();
                tvXml.Nodes.Clear();
                AddXmlToTree();
                tvXml.CollapseAll();
                lblNewMessage.Text = GetLabelMessage();
            }
            else
            {
                MessageBox.Show("û�������Ϣ����ɾ��!");
            }
        }

        private void btnSaveNew_Click(object sender, EventArgs e)
        {
            string m_szSerialNum = cbbProvider.SelectedValue.ToString().Substring(0, 2) + tbDate.Text.ToString() + tbSerialNum.Text.ToString();
            string m_szOrderNo = tbOrderNo.Text;
            if (CheckSerialNum(xdnew, m_szSerialNum))
            {
                if (xdnew.SelectSingleNode("root") != null)
                {
                    if (xdnew.SelectSingleNode("root").ChildNodes.Count != 0)
                    {
                        XmlNode xn = xdnew.SelectSingleNode("root");
                        XmlElement xe = (XmlElement)xn;
                        xe.SetAttribute("serialNo", m_szSerialNum);
                        xe.SetAttribute("orderNo", m_szOrderNo);
                        xe.SetAttribute("Key", DateTime.Now.ToString("JyyyyMMddThhmmssGAS"));
                        xe.SetAttribute("Version", VersionStr);
                        xe.SetAttribute("Tag", lblJiBiaoGYS.Visible ? "1" : "2");
                        try
                        {
                            AddSerialNumToBoxID(m_szSerialNum);
                            string FileName = m_szSerialNum + ".xml";

                            if (!System.IO.Directory.Exists(Path + "\\" + "XML"))
                            {
                                System.IO.Directory.CreateDirectory(Path + "\\" + "XML");
                            }



                            //saveFileDialog1.Filter = "Xml(*.xml)|�ļ�(*.*)";
                            saveFileDialog1.Filter = "Xml�ļ�|*.xml|�����ļ�|*.*";
                            saveFileDialog1.FileName = FileName;
                            saveFileDialog1.FilterIndex = 1;
                            saveFileDialog1.InitialDirectory = Path + "\\XML";
                            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                            {
                                string m_szPath = saveFileDialog1.FileName;

                                XmlTextWriter xtw = new XmlTextWriter(m_szPath, System.Text.Encoding.UTF8);
                                xtw.Formatting = Formatting.Indented;
                                xdnew.Save(xtw);
                                xtw.Close();

                                xdnew.RemoveAll();
                                tvXml.Nodes.Clear();
                                lblNewMessage.Text = "";
                            }

                        }
                        catch
                        {
                            MessageBox.Show("�����ļ�����");
                        }
                    }
                    else
                    {
                        MessageBox.Show("û���κ���Ϣ���棬����ӱ���Ϣ�Ժ��������");
                    }
                }
                else
                {
                    MessageBox.Show("û���κ���Ϣ���棬����ӱ���Ϣ�Ժ��������");
                }
            }

        }

        private void btnAddXFMulti_Click(object sender, EventArgs e)
        {
            if (CheckXFMultiButton())
            {
                //�ӽ����л�ȡ����
                string m_szTypeCaliber = cbbXFMultiType.SelectedValue.ToString();
                string m_szType = GetTypeByTypeCaliber(m_szTypeCaliber);
                string m_szCaliber = GetCaliberByTypeCaliber(m_szTypeCaliber);
                string m_szStartMeterID = tbXFStartMeterID.Text.ToString().ToUpper();
                int m_nNum = Convert.ToInt32(tbXFMeterNum.Value);
                string m_szSerialNum = "SX" + tbShiXiaoID.Text.ToString();
                List<string> m_MeterIDList = PopulateMeterID(m_szStartMeterID, m_nNum);

                if (CheckListXF(m_MeterIDList))
                {
                    AddMultiToXmlXF(m_szType, m_szCaliber, m_MeterIDList);
                }
                lblMessageXF.Text = GetLabelMessageXF();
                tvXmlXF.Nodes.Clear();
                AddXmlToTreeXF();
                ExpandTreeXF(m_szType + "(" + m_szCaliber + ")");
            }
        }

        private void btnAddXFOnly_Click(object sender, EventArgs e)
        {
            if (CheckXFOnlyButton())
            {
                //�ӽ����л�ȡ����
                string m_szTypeCaliber = cbbXFOnlyType.SelectedValue.ToString();
                string m_szType = GetTypeByTypeCaliber(m_szTypeCaliber);
                string m_szCaliber = GetCaliberByTypeCaliber(m_szTypeCaliber);
                string m_szMeterID = tbXFMeterID.Text.ToString().Trim().ToUpper();
                string m_szTxmID = tbXFMeterID.Text.ToString().Trim().ToUpper();
                //if (m_szTxmID == string.Empty)
                //{
                //    m_szTxmID = m_szMeterID;
                //}
                string m_szSerialNum = "SX" + tbShiXiaoID.Text.ToString();

                AddOnlyToXmlXF(m_szType, m_szCaliber, m_szMeterID, m_szTxmID);

                lblMessageXF.Text = GetLabelMessageXF();
                tvXmlXF.Nodes.Clear();
                AddXmlToTreeXF();
                ExpandTreeXF(m_szType + "(" + m_szCaliber + ")");
            }
        }

        private void btnDelXF_Click(object sender, EventArgs e)
        {
            if (xdupdate.SelectSingleNode("root") != null)
            {
                DeteleXmlFromTreeXF();
                tvXmlXF.Nodes.Clear();
                AddXmlToTreeXF();
                tvXmlXF.CollapseAll();
                lblMessageXF.Text = GetLabelMessageXF();
            }
            else
            {
                MessageBox.Show("û�������Ϣ����ɾ��!");
            }
        }

        private void tvXmlXF_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Checked)
            {
                foreach (TreeNode tn in e.Node.Nodes)
                {
                    tn.Checked = true;
                }
                tvXmlXF.AfterCheck -= this.tvXmlXF_AfterCheck;
                SearchBrother(e.Node);
                tvXmlXF.AfterCheck += this.tvXmlXF_AfterCheck;
            }
            else
            {
                foreach (TreeNode tn in e.Node.Nodes)
                {
                    tn.Checked = false;
                }

                tvXmlXF.AfterCheck -= this.tvXmlXF_AfterCheck;
                SearchParent(e.Node);
                tvXmlXF.AfterCheck += this.tvXmlXF_AfterCheck;


            }
        }

        private void btnSaveXF_Click(object sender, EventArgs e)
        {
            if (CheckSerialNum(xdupdate, "SX" + tbShiXiaoID.Text.ToString()))
            {
                string m_szFileName = "SX" + tbShiXiaoID.Text.ToString().ToUpper();
                if (xdupdate.SelectSingleNode("root") != null)
                {
                    if (xdupdate.SelectSingleNode("root").ChildNodes.Count != 0)
                    {
                        try
                        {
                            XmlNode xn = xdupdate.SelectSingleNode("root");
                            XmlElement xe = (XmlElement)xn;
                            xe.SetAttribute("serialNo", "SX" + tbShiXiaoID.Text.ToString().ToUpper());
                            xe.SetAttribute("Key", DateTime.Now.ToString("JyyyyMMddThhmmssGAS"));
                            xe.SetAttribute("Version", VersionStr);
                            xe.SetAttribute("Tag", "3");


                            string FileName = m_szFileName + ".xml";

                            if (!System.IO.Directory.Exists(Path + "\\" + "XML"))
                            {
                                System.IO.Directory.CreateDirectory(Path + "\\" + "XML");
                            }



                            saveFileDialog2.Filter = "Xml�ļ�|*.xml|�����ļ�|*.*";
                            saveFileDialog2.FileName = FileName;
                            saveFileDialog2.InitialDirectory = Path + "\\XML";
                            if (saveFileDialog2.ShowDialog() == DialogResult.OK)
                            {
                                string m_szPath = saveFileDialog2.FileName;

                                XmlTextWriter xtw = new XmlTextWriter(m_szPath, System.Text.Encoding.UTF8);
                                xtw.Formatting = Formatting.Indented;
                                xdupdate.Save(xtw);
                                xtw.Close();
                                xdupdate.RemoveAll();
                                tvXmlXF.Nodes.Clear();
                                lblMessageXF.Text = "";
                            }
                        }
                        catch
                        {
                            MessageBox.Show("�����ļ�����");
                        }
                    }
                    else
                    {
                        MessageBox.Show("û���κ���Ϣ���棬����ӱ���Ϣ�Ժ��������");
                    }
                }
                else
                {
                    MessageBox.Show("û���κ���Ϣ���棬����ӱ���Ϣ�Ժ��������");
                }
            }
        }

        private void btnLiuLan_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    dgv.Columns.RemoveAt(4);
                }
                catch
                { }

                xd = null;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    xd = new XmlDocument();
                    try
                    {
                        xd.Load(openFileDialog1.FileName);
                    }
                    catch
                    {
                        MessageBox.Show("��ȡ�ļ�ʧ��,�����»�ȡ!");
                        return;
                    }
                    DataTable dt = GetInfo(xd);
                    dgv.DataSource = dt;

                    dgv.Columns[0].Width = 60;
                    dgv.Columns[1].Width = 60;
                    dgv.Columns[2].Width = 60;
                    dgv.Columns[3].Width = 60;
                    //dgv.Columns[4].Width = 60;

                    dgv.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dgv.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dgv.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dgv.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
                    //dgv.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
                    //dgv.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;

                    DataGridViewColumn col = new DataGridViewColumn();
                    DataGridViewLinkCell cell = new DataGridViewLinkCell();
                    cell.Value = "��ϸ";
                    col.HeaderText = "";
                    col.CellTemplate = cell;
                    dgv.Columns.Insert(4, col);
                    dgv.Columns[4].Width = 60;
                    dgv.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;

                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (row.Index != (dgv.Rows.Count - 1) && row.Index != (dgv.Rows.Count - 2))
                        {
                            cell = new DataGridViewLinkCell();
                            cell.Value = "��ϸ";
                        }
                        else
                        {
                            cell = new DataGridViewLinkCell();
                            cell.Value = "";
                        }
                        row.Cells[4] = cell;
                    }

                    int m_nSize = dt.Rows.Count;
                    dgv.Height = (m_nSize + 2) * 20;

                    this.lblPepole.Location = new System.Drawing.Point(401, this.dgv.Location.Y + (m_nSize + 2) * 20);

                    string Path = openFileDialog1.FileName;
                    int m_nIndex = Path.LastIndexOf("\\");
                    int m_nLastIndex = Path.LastIndexOf(".");
                    string FileName = "";
                    if (m_nLastIndex < m_nIndex)
                        FileName = Path.Substring(m_nIndex + 1, Path.Length - m_nIndex - 1);
                    else
                        FileName = Path.Substring(m_nIndex + 1, m_nLastIndex - m_nIndex - 1);
                    tbPath.Text = Path;


                    XmlNode xn = xd.SelectSingleNode("root");
                    XmlElement xe = (XmlElement)xn;
                    lblSerialNum.Text = xe.GetAttribute("serialNo");
                    lblDate.Text = DateTime.Now.ToShortDateString();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Xml�ļ�����");
            }

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintDocument document = new PrintDocument();
            this.printPreviewDialog1 = new PrintPreviewDialog();


            this.printPreviewDialog1.ClientSize = new System.Drawing.Size(this.Size.Width, this.Size.Height);

            this.printPreviewDialog1.Location = new System.Drawing.Point(29, 29);

            this.printPreviewDialog1.Text = "��ӡԤ��";

            document.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(document_PrintPage);

            printPreviewDialog1.Document = document;
            printPreviewDialog1.ShowDialog();
        }

        private void document_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Bitmap tempmap = new Bitmap(this.Size.Width, this.Size.Height);
            this.pictureBox5.DrawToBitmap(tempmap, this.pictureBox5.ClientRectangle);

            //Graphics tempg = Graphics.FromImage(tempmap);

            Graphics Realg = this.CreateGraphics();
            Realg.DrawImage(tempmap, 0, 0);
            e.Graphics.DrawImage(tempmap, 100, 100);

            //tempg.Dispose();
            tempmap.Dispose();
            Realg.Dispose();


        }



        private void btnExit_Click(object sender, EventArgs e)
        {
            string m_szMessage = "";
            string m_szCaption = "�رգ�";
            DialogResult result;

            m_szMessage = "��Ҫ�˳�����ȷ����";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            result = MessageBox.Show(this, m_szMessage, m_szCaption, buttons);
            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(@Path + "\\XmlBuilder.chm");
            }
            catch
            {
                MessageBox.Show("�����ļ�������!");
            }
        }

        private void btnZhiNengB_Click(object sender, EventArgs e)
        {
            biaolx = 0;
            PanPrint.Width = 0;
            PanPrint.Height = 0;
            PanShiXiao.Width = 0;
            PanShiXiao.Height = 0;
            PanXiuZhengY.Width = 0;
            PanXiuZhengY.Height = 0;
            PanNew.Width = 735;
            PanNew.Height = 518;// 464;
            btnZhiNengB.Image = global::ProviderCS.Properties.Resources.ZhiNengB_On;
            btnXiuFuB.Image = global::ProviderCS.Properties.Resources.XiuFuB_Off;
            btnDaYinSHD.Image = global::ProviderCS.Properties.Resources.Print_normal;
            btnPuTongB.Image = global::ProviderCS.Properties.Resources.PuTongB_Off;
            btnXiuZhengY.Image = global::ProviderCS.Properties.Resources.XiuZhengY_Off;
            this.btnZhiNengB.Location = new System.Drawing.Point(361, 75);
            this.btnPuTongB.Location = new System.Drawing.Point(466, 81);
            this.btnXiuFuB.Location = new System.Drawing.Point(570, 81);
            //this.btnDaYinSHD.Location = new System.Drawing.Point(674, 81);
            this.btnXiuZhengY.Location = new System.Drawing.Point(674, 81);
            this.btnZhiNengB.Size = new System.Drawing.Size(105, 30);
            this.btnXiuFuB.Size = new System.Drawing.Size(104, 24);
            //this.btnDaYinSHD.Size = new System.Drawing.Size(104, 24);
            this.btnPuTongB.Size = new System.Drawing.Size(104, 24);
            this.btnXiuZhengY.Size = new System.Drawing.Size(104, 24);


            lblGongDianFS.Visible = lblFaMenQK.Visible = lblICCard.Visible = lblJiBiaoGYS.Visible = lblWuXianCS1.Visible = true;//lblSheBeiLX.Visible = 
            cbbGongDianFS.Visible = cbbFaMenQK.Visible = cbbICCard.Visible = cbbJiBiaoGYS.Visible = tbJiBiaoGYS.Visible = cbbWireless1.Visible = true;//cbbSheBeiLX.Visible = 
            label54.Visible = label55.Visible = tbTongXinH.Visible = tbTongXinH2.Visible = CKCeYaKou.Visible = true;
            CKCeYaKou.Checked = false;

        }

        private void btnXiuFuB_Click(object sender, EventArgs e)
        {
            PanPrint.Width = 0;
            PanPrint.Height = 0;
            PanShiXiao.Width = 735;
            PanShiXiao.Height = 518;// 464;
            PanNew.Width = 0;
            PanNew.Height = 0;
            PanXiuZhengY.Width = 0;
            PanXiuZhengY.Height = 0;
            btnZhiNengB.Image = global::ProviderCS.Properties.Resources.ZhiNengB_Off;
            btnXiuFuB.Image = global::ProviderCS.Properties.Resources.XiuFu_on;
            btnDaYinSHD.Image = global::ProviderCS.Properties.Resources.Print_normal;
            btnPuTongB.Image = global::ProviderCS.Properties.Resources.PuTongB_Off;
            btnXiuZhengY.Image = global::ProviderCS.Properties.Resources.XiuZhengY_Off;
            this.btnXiuFuB.Location = new System.Drawing.Point(569, 75);
            this.btnZhiNengB.Location = new System.Drawing.Point(361, 81);
            this.btnPuTongB.Location = new System.Drawing.Point(466, 81);
            //this.btnDaYinSHD.Location = new System.Drawing.Point(674, 81);
            this.btnXiuZhengY.Location = new System.Drawing.Point(674, 81);
            this.btnXiuFuB.Size = new System.Drawing.Size(105, 30);
            this.btnZhiNengB.Size = new System.Drawing.Size(104, 24);
            //this.btnDaYinSHD.Size = new System.Drawing.Size(104, 24);
            this.btnPuTongB.Size = new System.Drawing.Size(104, 24);
            this.btnXiuZhengY.Size = new System.Drawing.Size(104, 24);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            PanPrint.Width = 735;
            PanPrint.Height = 518;// 464;
            PanShiXiao.Width = 0;
            PanShiXiao.Height = 0;
            PanNew.Width = 0;
            PanNew.Height = 0;

            btnZhiNengB.Image = global::ProviderCS.Properties.Resources.New_normal;
            btnXiuFuB.Image = global::ProviderCS.Properties.Resources.XiuFuB_Off;
            btnDaYinSHD.Image = global::ProviderCS.Properties.Resources.Print_on;
            btnPuTongB.Image = global::ProviderCS.Properties.Resources.New_normal;
            this.btnDaYinSHD.Location = new System.Drawing.Point(673, 75);
            this.btnZhiNengB.Location = new System.Drawing.Point(361, 81);
            this.btnPuTongB.Location = new System.Drawing.Point(466, 81);
            this.btnXiuFuB.Location = new System.Drawing.Point(569, 81);
            this.btnDaYinSHD.Size = new System.Drawing.Size(105, 30);
            this.btnZhiNengB.Size = new System.Drawing.Size(104, 24);
            this.btnXiuFuB.Size = new System.Drawing.Size(104, 24);
            this.btnPuTongB.Size = new System.Drawing.Size(104, 24);
        }

        private void btnPuTongB_Click(object sender, EventArgs e)
        {
            biaolx = 1;
            PanPrint.Width = 0;
            PanPrint.Height = 0;
            PanShiXiao.Width = 0;
            PanShiXiao.Height = 0;
            PanNew.Width = 735;
            PanNew.Height = 518;// 464;
            PanXiuZhengY.Width = 0;
            PanXiuZhengY.Height = 0;
            btnZhiNengB.Image = global::ProviderCS.Properties.Resources.ZhiNengB_Off;
            btnXiuFuB.Image = global::ProviderCS.Properties.Resources.XiuFuB_Off;
            btnDaYinSHD.Image = global::ProviderCS.Properties.Resources.Print_normal;
            btnPuTongB.Image = global::ProviderCS.Properties.Resources.PuTongB_On;
            btnXiuZhengY.Image = global::ProviderCS.Properties.Resources.XiuZhengY_Off;
            this.btnPuTongB.Location = new System.Drawing.Point(466, 75);
            this.btnZhiNengB.Location = new System.Drawing.Point(361, 81);
            this.btnXiuFuB.Location = new System.Drawing.Point(570, 81);
            //this.btnDaYinSHD.Location = new System.Drawing.Point(674, 81);
            this.btnXiuZhengY.Location = new System.Drawing.Point(674, 81);
            this.btnPuTongB.Size = new System.Drawing.Size(105, 30);
            this.btnZhiNengB.Size = new System.Drawing.Size(104, 24);
            this.btnXiuFuB.Size = new System.Drawing.Size(104, 24);
            //this.btnDaYinSHD.Size = new System.Drawing.Size(104, 24);
            this.btnXiuZhengY.Size = new System.Drawing.Size(104, 24);

            lblSheBeiLX.Visible = lblFaMenQK.Visible = lblICCard.Visible = lblJiBiaoGYS.Visible = lblWuXianCS1.Visible = false;
            cbbSheBeiLX.Visible = cbbFaMenQK.Visible = cbbICCard.Visible = cbbJiBiaoGYS.Visible = tbJiBiaoGYS.Visible = cbbWireless1.Visible = false;
            label54.Visible = label55.Visible = tbTongXinH.Visible = tbTongXinH2.Visible = CKCeYaKou.Visible = false;

            cbbFaMenQK.SelectedValue = "-99999";
            cbbSheBeiLX.SelectedValue = "-99999";
            cbbICCard.SelectedValue = "-99999";
            cbbWireless1.SelectedValue = "-99999";
            tbJiBiaoGYS.Text = string.Empty;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label56.Text += VersionStr;
            //PanNew.Visible = true;
            //PanPrint.Visible = false;
            //PanShiXiao.Visible = false; 
            PanPrint.Width = 0;
            PanPrint.Height = 0;
            PanShiXiao.Width = 0;
            PanShiXiao.Height = 0;
            PanNew.Width = 735;
            PanNew.Height = 518;// 464;
        }

        private void btnAddNewBig_Click(object sender, EventArgs e)
        {
            if (CheckOnlyBigButton())
            {
                //�ӽ����л�ȡ����     
                string m_szTypeCaliber = cbbOnlyTypeBig.SelectedValue.ToString();
                string m_szType = GetTypeByTypeCaliber(m_szTypeCaliber);
                string m_szCaliber = GetCaliberByTypeCaliber(m_szTypeCaliber);
                string m_szTongXinH = tbTongXinH.Text.Trim();
                //decimal m_dPrice = Convert.ToDecimal(tbOnlyPriceBig.Text.ToString());
                string m_szBoxID = tbOnlyTxmBig.Text.ToString().Trim().ToUpper();
                string m_szMeterID = tbOnlyMeterBig.Text.ToString().Trim().ToUpper();
                string m_szTxmID = tbOnlyTxmBig.Text.ToString().Trim().ToUpper(); //tbOnlyTxmBig.Text.ToString().Trim().ToUpper();
                //if (m_szTxmID == string.Empty)
                //{
                //    m_szTxmID = m_szMeterID;
                //}
                string m_szSerialNum = cbbProvider.SelectedValue.ToString().Substring(0, 2) + tbDate.Text.ToString() + tbSerialNum.Text.ToString();
                int m_nNum = Convert.ToInt32(nudNumBig.Value);

                List<string> m_MeterIDList = PopulateMeterID(m_szTxmID, m_nNum);

                //֤����
                string m_ZhengShuBH = tbZhengShuBH.Text.Trim();

                //��ȡ����ID
                string m_szStartstr = GetWireStartBH();

                string m_szWireMeterID = "";
                if (m_szStartstr != "")
                    m_szWireMeterID = tbOnlyWireBig.Text.Trim();

                if (CheckList(m_MeterIDList, m_MeterIDList, m_szType, m_szCaliber))//, m_dPrice))
                {
                    if (m_szTxmID != string.Empty && m_szMeterID != m_szTxmID)
                    {
                        AddOnlyToXml(m_szType, m_szCaliber, m_szTxmID, m_szMeterID, m_szTxmID, m_szTxmID, m_ZhengShuBH, m_szTongXinH);
                    }
                    else
                    {
                        AddMultiToXml(m_szType, m_szCaliber//, m_dPrice
                            , m_MeterIDList, m_MeterIDList, 1, m_szMeterID, m_ZhengShuBH, m_szTongXinH);
                    }
                }
                tvXml.Nodes.Clear();
                AddXmlToTree();
                ExpandTree(m_szBoxID);
                lblNewMessage.Text = GetLabelMessage();
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                string sourcePath = Application.StartupPath + "\\MeterType.xml";
                openFileDialog1.Filter = "xml files (*.xml) | *.xml";
                openFileDialog1.Title = "�滻";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    XmlDocument xd = new XmlDocument();
                    try
                    {
                        xd.Load(openFileDialog1.FileName);
                    }
                    catch
                    {
                        MessageBox.Show("��ȡ�ļ�ʧ��,�����»�ȡ!");
                        return;
                    }
                    string destPath = openFileDialog1.FileName;
                    //string sourcePath = "E:\\Wyf\\DZMeter\\ProviderCS\\MeterType.xml";
                    try
                    {
                        if (System.IO.File.Exists(sourcePath))
                        {
                            System.IO.File.SetAttributes(sourcePath, System.IO.FileAttributes.Normal);
                        }
                        System.IO.File.Copy(destPath, sourcePath, true);
                        //System.IO.File.Replace(sourcePath, destPath, null);
                        if (InitDropDownList(openFileDialog1.FileName))
                            MessageBox.Show("�滻�ɹ���");
                        else
                            MessageBox.Show("�滻ʧ�ܣ�");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
            catch
            {
                MessageBox.Show("Xml�ļ�����");
            }
        }

        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                string aaa = dgv.Rows[e.RowIndex].Cells[0].Value.ToString();

                XmlNodeList _XmlNodeList = xd.SelectSingleNode("root").ChildNodes;

                foreach (XmlNode xn in _XmlNodeList)
                {
                    if (xn.Attributes["Type"].Value == aaa)
                    {
                        test obj = new test(xn);
                        obj.Show();
                        break;
                    }

                }
            }
        }

        private void btnAddXZY_Click(object sender, EventArgs e)
        {
            if (CheckXZYButton())
            {
                //�ӽ����л�ȡ����
                string m_szTypeCaliber = cbbXiuZhengYXH.SelectedValue.ToString();
                string m_szType = GetTypeByTypeCaliber(m_szTypeCaliber);
                string m_szCaliber = GetCaliberByTypeCaliber(m_szTypeCaliber);
                //decimal m_dPrice = Convert.ToDecimal(tbOnlyPrice.Text.ToString());
                string m_ZuiGaoYL = tbZuiGaoYL.Text.ToString();
                string m_ZuiDiYL = tbZuiDiYL.Text.ToString();
                string m_szBoxID = tbXiuZhengYBH.Text.ToString();
                string m_szTxm = tbXiuZhengYBH.Text.ToString().Trim().ToUpper();
                string m_szMeterID = tbXiuZhengYBYBH.Text.ToString().Trim().ToUpper();
                string m_szSerialNum = cbbProviderXZY.SelectedValue.ToString().Substring(0, 2) + tbDateXZY.Text.ToString() + tbSerialNumXZY.Text.ToString();

                AddXZYToXml(m_szType, m_szCaliber//, m_dPrice
                    , m_szBoxID, m_szMeterID, m_szTxm, m_ZuiGaoYL, m_ZuiDiYL);//m_szWireMeterID
                tvXiuZhengY.Nodes.Clear();
                AddXmlToXZYTree();
                ExpandXZYTree(m_szBoxID);
                //lblNewMessage.Text = GetLabelMessage();
            }
        }

        private void btnDelXZY_Click(object sender, EventArgs e)
        {
            if (xdnew.SelectSingleNode("root") != null)
            {
                DeteleXmlFromXZYTree();
                tvXiuZhengY.Nodes.Clear();
                AddXmlToXZYTree();
                tvXiuZhengY.CollapseAll();
                //lblNewMessage.Text = GetLabelMessage();
            }
            else
            {
                MessageBox.Show("û�������Ϣ����ɾ��!");
            }
        }

        private void btnSaveXZY_Click(object sender, EventArgs e)
        {
            string m_szSerialNum = cbbProviderXZY.SelectedValue.ToString().Substring(0, 2) + tbDateXZY.Text.ToString() + tbSerialNumXZY.Text.ToString();
            string m_szOrderNo = tbOrderNoXZY.Text;
            if (CheckSerialNum(xdnew, m_szSerialNum))
            {
                if (xdnew.SelectSingleNode("root") != null)
                {
                    if (xdnew.SelectSingleNode("root").ChildNodes.Count != 0)
                    {
                        XmlNode xn = xdnew.SelectSingleNode("root");
                        XmlElement xe = (XmlElement)xn;
                        xe.SetAttribute("serialNo", m_szSerialNum);
                        xe.SetAttribute("orderNo", m_szOrderNo);
                        xe.SetAttribute("Key", DateTime.Now.ToString("JyyyyMMddThhmmssGAS"));
                        xe.SetAttribute("Version", VersionStr);
                        xe.SetAttribute("Tag", "4");

                        try
                        {
                            AddSerialNumToBoxID(m_szSerialNum);
                            string FileName = m_szSerialNum + ".xml";

                            if (!System.IO.Directory.Exists(Path + "\\" + "XML"))
                            {
                                System.IO.Directory.CreateDirectory(Path + "\\" + "XML");
                            }



                            //saveFileDialog1.Filter = "Xml(*.xml)|�ļ�(*.*)";
                            saveFileDialog1.Filter = "Xml�ļ�|*.xml|�����ļ�|*.*";
                            saveFileDialog1.FileName = FileName;
                            saveFileDialog1.FilterIndex = 1;
                            saveFileDialog1.InitialDirectory = Path + "\\XML";
                            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                            {
                                string m_szPath = saveFileDialog1.FileName;

                                XmlTextWriter xtw = new XmlTextWriter(m_szPath, System.Text.Encoding.UTF8);
                                xtw.Formatting = Formatting.Indented;
                                xdnew.Save(xtw);
                                xtw.Close();

                                xdnew.RemoveAll();
                                tvXiuZhengY.Nodes.Clear();
                                lblNewMessage.Text = "";
                            }

                        }
                        catch
                        {
                            MessageBox.Show("�����ļ�����");
                        }
                    }
                    else
                    {
                        MessageBox.Show("û���κ���Ϣ���棬����ӱ���Ϣ�Ժ��������");
                    }
                }
                else
                {
                    MessageBox.Show("û���κ���Ϣ���棬����ӱ���Ϣ�Ժ��������");
                }
            }

        }

        private void btnXiuZhengY_Click(object sender, EventArgs e)
        {
            PanPrint.Width = 0;
            PanPrint.Height = 0;
            PanShiXiao.Width = 0;
            PanShiXiao.Height = 0;
            PanNew.Width = 0;
            PanNew.Height = 0;
            PanXiuZhengY.Width = 735;
            PanXiuZhengY.Height = 464;

            btnZhiNengB.Image = global::ProviderCS.Properties.Resources.ZhiNengB_Off;
            btnXiuFuB.Image = global::ProviderCS.Properties.Resources.XiuFuB_Off;
            btnDaYinSHD.Image = global::ProviderCS.Properties.Resources.Print_on;
            btnPuTongB.Image = global::ProviderCS.Properties.Resources.PuTongB_Off;
            btnXiuZhengY.Image = global::ProviderCS.Properties.Resources.XiuZhengY_On;
            this.btnXiuZhengY.Location = new System.Drawing.Point(673, 75);
            //this.btnDaYinSHD.Location = new System.Drawing.Point(673, 75);
            this.btnZhiNengB.Location = new System.Drawing.Point(361, 81);
            this.btnPuTongB.Location = new System.Drawing.Point(466, 81);
            this.btnXiuFuB.Location = new System.Drawing.Point(569, 81);
            this.btnXiuZhengY.Size = new System.Drawing.Size(105, 30);
            //this.btnDaYinSHD.Size = new System.Drawing.Size(105, 30);
            this.btnZhiNengB.Size = new System.Drawing.Size(104, 24);
            this.btnXiuFuB.Size = new System.Drawing.Size(104, 24);
            this.btnPuTongB.Size = new System.Drawing.Size(104, 24);
        }


        #endregion

        #region MouseEvent

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {

                Point mouse1 = new Point(Control.MousePosition.X - xx, Control.MousePosition.Y - yy);
                Location = mouse1;
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            xx = e.X; yy = e.Y;
        }

        private void btnAddNewMulti_MouseHover(object sender, EventArgs e)
        {
            btnAddNewMulti.Image = global::ProviderCS.Properties.Resources.Add_on;
        }

        private void btnAddNewMulti_MouseLeave(object sender, EventArgs e)
        {
            btnAddNewMulti.Image = global::ProviderCS.Properties.Resources.Add_normal;
        }
        private void button3_MouseHover(object sender, EventArgs e)
        {
            btnExit.Image = global::ProviderCS.Properties.Resources.Exit_on;
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            btnExit.Image = global::ProviderCS.Properties.Resources.Exit_normal;
        }

        private void button4_MouseLeave(object sender, EventArgs e)
        {
            btnHelp.Image = global::ProviderCS.Properties.Resources.Help_normal;
        }

        private void button4_MouseHover(object sender, EventArgs e)
        {
            btnHelp.Image = global::ProviderCS.Properties.Resources.Help_on;
        }

        private void btnAddNewOnly_MouseHover(object sender, EventArgs e)
        {
            btnAddNewOnly.Image = global::ProviderCS.Properties.Resources.Add_on;
        }

        private void btnAddNewOnly_MouseLeave(object sender, EventArgs e)
        {
            btnAddNewOnly.Image = global::ProviderCS.Properties.Resources.Add_normal;
        }

        private void button8_MouseHover(object sender, EventArgs e)
        {
            btnAddXFMulti.Image = global::ProviderCS.Properties.Resources.Add_on;
        }

        private void button8_MouseLeave(object sender, EventArgs e)
        {
            btnAddXFMulti.Image = global::ProviderCS.Properties.Resources.Add_normal;
        }

        private void button7_MouseHover(object sender, EventArgs e)
        {
            btnAddXFOnly.Image = global::ProviderCS.Properties.Resources.Add_on;
        }

        private void button7_MouseLeave(object sender, EventArgs e)
        {
            btnAddXFOnly.Image = global::ProviderCS.Properties.Resources.Add_normal;
        }

        private void btnDelXF_MouseHover(object sender, EventArgs e)
        {
            btnDelXF.Image = global::ProviderCS.Properties.Resources.Delete_on;
        }

        private void btnDelXF_MouseLeave(object sender, EventArgs e)
        {
            btnDelXF.Image = global::ProviderCS.Properties.Resources.Delete_normal;
        }

        private void button5_MouseHover(object sender, EventArgs e)
        {
            btnSaveXF.Image = global::ProviderCS.Properties.Resources.Save_on;
        }

        private void button5_MouseLeave(object sender, EventArgs e)
        {
            btnSaveXF.Image = global::ProviderCS.Properties.Resources.Save_normal;
        }

        private void btnDelNew_MouseHover(object sender, EventArgs e)
        {
            btnDelNew.Image = global::ProviderCS.Properties.Resources.Delete_on;
        }

        private void btnDelNew_MouseLeave(object sender, EventArgs e)
        {
            btnDelNew.Image = global::ProviderCS.Properties.Resources.Delete_normal;
        }

        private void btnSaveNew_MouseHover(object sender, EventArgs e)
        {
            btnSaveNew.Image = global::ProviderCS.Properties.Resources.Save_on;
        }

        private void btnSaveNew_MouseLeave(object sender, EventArgs e)
        {
            btnSaveNew.Image = global::ProviderCS.Properties.Resources.Save_normal;
        }

        private void button2_MouseHover(object sender, EventArgs e)
        {
            btnLiuLan.Image = global::ProviderCS.Properties.Resources.File_on;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            btnLiuLan.Image = global::ProviderCS.Properties.Resources.File_normal;
        }

        private void btnPrint_MouseHover(object sender, EventArgs e)
        {
            btnPrint.Image = global::ProviderCS.Properties.Resources.BtnPrint_on;
        }

        private void btnPrint_MouseLeave(object sender, EventArgs e)
        {
            btnPrint.Image = global::ProviderCS.Properties.Resources.btnPrint_normal;
        }
        private void button11_MouseHover(object sender, EventArgs e)
        {
            btnAddNewBig.Image = global::ProviderCS.Properties.Resources.Add_on;
        }

        private void button11_MouseLeave(object sender, EventArgs e)
        {
            btnAddNewBig.Image = global::ProviderCS.Properties.Resources.Add_normal;
        }

        private void button12_MouseHover(object sender, EventArgs e)
        {
            btnImport.Image = global::ProviderCS.Properties.Resources.����XML_on;
        }

        private void button12_MouseLeave(object sender, EventArgs e)
        {
            btnImport.Image = global::ProviderCS.Properties.Resources.����XML_normal;
        }

        #endregion

        private void timerNow_Tick(object sender, EventArgs e)
        {
            lbNowTime.Text = "��ǰʱ�䣺" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }





    }
}