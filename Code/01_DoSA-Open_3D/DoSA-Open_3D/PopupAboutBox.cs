using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

using gtLibrary;

// 파일처리
using System.IO;

namespace DoSA
{
    partial class PopupAboutBox : Form
    {
        CManageFile m_manageFile = new CManageFile();

        public PopupAboutBox()
        {
            InitializeComponent();

            string strDescription;
            string strDonation;
            string strQnaBoard;

            if(CSettingData.m_emLanguage == EMLanguage.Korean)
            {
                strDonation = "기여하기";
                strQnaBoard = "QnA 게시판";

                strDescription = @"DoSA-Open 은 
정보통신산업진흥원 (NIPA) 의 지원사업으로 개발이 시작된 프로그램입니다.
따라서 누구나 회사에서도 무료로 사용할 수 있는 
오픈소스 액추에이터(솔레노이드) 자기력 전산해석 프로그램입니다.

그리고 DoSA-Open 의 사용 환경은 액추에이터 제품개발과 유사해서
전산해석 담당자보다도 액추에이터 개발자들이 오히려 쉽게 사용할 수 있는 
전산해석 프로그램입니다.

모든 액추에이터 개발자들이 액추에이터의 제품개발에 전산해석을 사용하여
자기력을 예측할 수 있도록 도움이 되었으면 합니다.";
            }
            else
            {
                strDonation = "Donation";
                strQnaBoard = "QnA Board";

                strDescription = @"DoSA-Open is open source actuator design software. 
Anyone can use it free of charge in the company.

DoSA-Open's user environment is similar to actuator product development. 
This allows actuator developers to easily use it rather than computational analysts.

We hope that all actuator developers will be able to use magnetic analysis 
to predict magnetic forces in product development of actuators..
";
            }

            string strOpenLicense = @"DoSA-Open_3D is open source actuator design software using Onelab.

This application uses open source software below.

Easily Add a Ribbon into a WinForms Application	
  link: https://www.codeproject.com/Articles/364272/Easily-Add-a-Ribbon-into-a-WinForms-Application
  license: Microsoft Public License (MS-PL)	
  License link: https://opensource.org/licenses/ms-pl.html	
	
[ Open Icon Library ]	
	
Oxygen Icons 4.3.1 (KDE) (oxygen)	
  link: http://www.oxygen-icons.org/	
  license: Dual: CC-BY-SA 3.0 or LGPL	
  License link: http://creativecommons.org/licenses/by-sa/3.0/
                http://creativecommons.org/licenses/LGPL/2.1/	  	
	
Crystal Clear (crystal_clear)	
  link: http://commons.wikimedia.org/wiki/Crystal_Clear	
  license: LGPL-2.1	
  license link: http://creativecommons.org/licenses/LGPL/2.1/	
	
Crystal Project (crystal)	
  link: http://everaldo.com/crystal/, CrystalXp.net
  license: LGPL-2.1
  license link: http://creativecommons.org/licenses/LGPL/2.1/
	
Nuvola 1.0 (KDE 3.x icon set) (nuvola)	
  link: http://www.icon-king.com/projects/nuvola/
  license: LGPL 2.1
  license link: http://creativecommons.org/licenses/LGPL/2.1/";

            /// 버전관리 유의사항
            /// 
            /// AssemblyInfo.cs 의 AssemblyVersion 이 아니라 AssemblyFileVersion 이 DoSA 실행파일의 Product Version 이다.
            /// 따라서 DoSA 자동업데이트나 업데이트 요청메시지를 띄우기 위해 버전 확인을 DoSA 실행파일의 버전을 사용하고 있다.
            /// 
            /// About 창에서도 동일한 버전으로 표기하기 위해 AssemblyFileVersion 를 사용하려고 하였으나 
            /// AssemblyFileVersion 는 직접 읽어오지 못해서 여기서도 DoSA 실행파일의 버전을 읽어서 ProductVersion 을 읽어낸다.
            string strEXE_FileName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string strProductVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(strEXE_FileName).ProductVersion;

            this.Text = String.Format("About {0}", AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;
            //this.labelVersion.Text = String.Format("- Version ({0})", AssemblyVersion);
            this.labelVersion.Text = String.Format("- Version ({0})", strProductVersion);
            this.labelCopyright.Text = String.Format("- License : {0}", AssemblyCopyright);
            this.labelContributor.Text = String.Format("- Contributor : {0}", AssemblyCompany);
            this.textBoxDescription.Text = strDescription;
            this.textBoxOpenLicense.Text = strOpenLicense;

            this.buttonDonation.Text = strDonation;
            this.buttonQnA.Text = strQnaBoard;
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion


        private void buttonQnA_Click(object sender, EventArgs e)
        {
            string target;

            if (CSettingData.m_emLanguage == EMLanguage.Korean)
            {
                target = "http://www.solenoid.or.kr/direct/index_dosa_qna_kor.html";
            }
            else
            {
                target = "http://www.solenoid.or.kr/direct/index_dosa_qna_eng.html";
            }

            try
            {
                System.Diagnostics.Process.Start(target);
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    CNotice.printTrace(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                CNotice.printTrace(other.Message);
            }
        }

        private void buttonLicense_Click(object sender, EventArgs e)
        {
            string target = "https://github.com/OpenActuator/DoSA-Open_3D/blob/master/LICENSE";

            try
            {
                System.Diagnostics.Process.Start(target);
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    CNotice.printTrace(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                CNotice.printTrace(other.Message);
            }
        }

        private void buttonDonation_Click(object sender, EventArgs e)
        {
            string target;

            if (CSettingData.m_emLanguage == EMLanguage.Korean)
            {
                target = "http://www.solenoid.or.kr/direct/index_dosa_donation_kor.html";
            }
            else
            {
                target = "http://www.solenoid.or.kr/direct/index_dosa_donation_eng.html";
            }

            try
            {
                System.Diagnostics.Process.Start(target);
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    CNotice.printTrace(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                CNotice.printTrace(other.Message);
            }
        }
    }
}
