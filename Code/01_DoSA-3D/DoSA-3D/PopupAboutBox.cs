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
            string strLicense;
            string strHomepage;

            if(CSettingData.m_emLanguage == EMLanguage.Korean)
            {
                strDonation = "기여하기";
                strLicense = "라이선스";
                strHomepage = "홈페이지";

                strDescription = @"DoSA-3D 는 
액추에이터나 솔레노이드의 자기력을 해석할 수 있는 3차원 오픈소스 프로그램 입니다.

오픈소스 프로젝트로 개발되어 
개인 뿐만아니라 회사에서도 무료로 프로그램을 사용할 수 있습니다.

프로그램 작업 환경을 제품개발 과정과 유사하도록 개발 되었습니다.
따라서 해석을 전공하지 않은 개발자도 쉽게 액추에이터의 자기력을 해석할 수 있습니다.";
            }
            else
            {
                strDonation = "Donation";
                strLicense = "License";
                strHomepage = "Homepage";

                strDescription = @"DoSA-3D is an open source actuator software. 
Anyone can use it free of charge in the company.

DoSA-3D's user environment is similar to actuator product development. 
This allows actuator developers to easily use it rather than computational analysts.

The program environment is developed to be similar to that of product development.
so even product developers who have not majored in analysis can easily analyze the magnetic force of actuators or solenoids.
";
            }

            string strOpenLicense = @"DoSA-3D is an open source software using Onelab (GetDP + Gmsh).

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
            this.buttonLicense.Text = strLicense;
            this.buttonHomepage.Text = strHomepage;
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


        //private void buttonHomepage_Click(object sender, EventArgs e)
        //{
        //    string target;

        //    if (CSettingData.m_emLanguage == EMLanguage.Korean)
        //    {
        //        target = "https://solenoid.or.kr/index_kor.html";
        //    }
        //    else
        //    {
        //        target = "https://solenoid.or.kr/index_eng.html";
        //    }

        //    try
        //    {
        //        System.Diagnostics.Process.Start(target);
        //    }
        //    catch (System.ComponentModel.Win32Exception noBrowser)
        //    {
        //        if (noBrowser.ErrorCode == -2147467259)
        //            CNotice.printTrace(noBrowser.Message);
        //    }
        //    catch (System.Exception other)
        //    {
        //        CNotice.printTrace(other.Message);
        //    }
        //}

        private void buttonLicense_Click(object sender, EventArgs e)
        {
            string target = "https://github.com/OpenActuator/DoSA-3D/blob/master/LICENSE";

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
                target = "https://solenoid.or.kr/direct_kor.php?address=https://solenoid.or.kr/openactuator/dosa_donation_kor.htm";
            }
            else
            {
                target = "https://solenoid.or.kr/direct_eng.php?address=https://solenoid.or.kr/openactuator/dosa_donation_eng.htm";
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

        private void buttonHomepage_Click(object sender, EventArgs e)
        {
            string target;

            if (CSettingData.m_emLanguage == EMLanguage.Korean)
            {
                target = "https://solenoid.or.kr/index_dosa_open_3d_kor.html";
            }
            else
            {
                target = "https://solenoid.or.kr/index_dosa_open_3d_eng.html";
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
