using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using gtLibrary;
using System.Globalization;
using System.Threading;


namespace DoSA
{
    public partial class PopupHelp : Form
    {
        CManageFile m_manageFile = new CManageFile();

        public PopupHelp()
        {
            InitializeComponent();

            if (CSettingData.m_emLanguage == EMLanguage.Korean)
            {
                buttonVCMGuide.Text = "VCM 예제 따라하기";
                buttonSolenoidGuide.Text = "Solenoid 예제 따라하기";
                buttonDrawingGuide.Text = "해석 전 형상작업 가이드";
                buttonOpenHelpDirectory.Text = "도움말 디렉토리 열기";
                labelNotice1.Text = "- 도움말이 열리지 않으면";
                labelNotice2.Text = "   가장 아래의 디렉토리 열기 버튼을 사용 하세요.";
            }
            else
            {
                buttonVCMGuide.Text = "VCM Example";
                buttonSolenoidGuide.Text = "Solenoid Example";
                buttonDrawingGuide.Text = "Drawing Guide before Simulation";
                buttonOpenHelpDirectory.Text = "Open the help directory";
                labelNotice1.Text = "- If the help does not open,";
                labelNotice2.Text = "   use the Open Directory button at the bottom.";
            }
        }

        private void buttonHelpClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonDoSAUserGuide_Click(object sender, EventArgs e)
        {
            string strHelpFileFullName;
            CultureInfo ctInfo = Thread.CurrentThread.CurrentCulture;

            if (ctInfo.Name == "en-US")
                strHelpFileFullName = Path.Combine(CSettingData.m_strProgramDirPath, "Help", "DoSA_User_Guide_ENG.pdf");
            else if (ctInfo.Name == "ko-KR")
                strHelpFileFullName = Path.Combine(CSettingData.m_strProgramDirPath, "Help", "DoSA_User_Guide_KOR.pdf");
            else
                return;

            if (m_manageFile.isExistFile(strHelpFileFullName) == false)
            {
                CNotice.noticeWarningID("HFDN");
                return;
            }

            System.Diagnostics.Process.Start(strHelpFileFullName);
        }

        private void buttonVCMGuide_Click(object sender, EventArgs e)
        {
            string strHelpFileFullName;
            CultureInfo ctInfo = Thread.CurrentThread.CurrentCulture;

            if (ctInfo.Name == "en-US")
                strHelpFileFullName = Path.Combine(CSettingData.m_strProgramDirPath, "Help", "VCM_Sample_ENG.pdf");
            else if (ctInfo.Name == "ko-KR")
                strHelpFileFullName = Path.Combine(CSettingData.m_strProgramDirPath, "Help", "VCM_Sample_KOR.pdf");
            else
                return;

            if (m_manageFile.isExistFile(strHelpFileFullName) == false)
            {
                CNotice.noticeWarningID("HFDN2");
                return;
            }

            System.Diagnostics.Process.Start(strHelpFileFullName);
        }

        private void buttonSolenoidGuide_Click(object sender, EventArgs e)
        {
            string strHelpFileFullName;
            CultureInfo ctInfo = Thread.CurrentThread.CurrentCulture;

            if (ctInfo.Name == "en-US")
                strHelpFileFullName = Path.Combine(CSettingData.m_strProgramDirPath, "Help", "Solenoid_Sample_ENG.pdf");
            else if (ctInfo.Name == "ko-KR")
                strHelpFileFullName = Path.Combine(CSettingData.m_strProgramDirPath, "Help", "Solenoid_Sample_KOR.pdf");
            else
                return;

            if (m_manageFile.isExistFile(strHelpFileFullName) == false)
            {
                CNotice.noticeWarningID("HFDN1");
                return;
            }

            System.Diagnostics.Process.Start(strHelpFileFullName);
        }

        private void buttonDrawingGuide_Click(object sender, EventArgs e)
        {
            string strHelpFileFullName;
            CultureInfo ctInfo = Thread.CurrentThread.CurrentCulture;

            if (ctInfo.Name == "en-US")
                strHelpFileFullName = Path.Combine(CSettingData.m_strProgramDirPath, "Help", "Drawing_Guide_ENG.pdf");
            else if (ctInfo.Name == "ko-KR")
                strHelpFileFullName = Path.Combine(CSettingData.m_strProgramDirPath, "Help", "Drawing_Guide_KOR.pdf");
            else
                return;

            if (m_manageFile.isExistFile(strHelpFileFullName) == false)
            {

                if (CSettingData.m_emLanguage == EMLanguage.Korean)
                    CNotice.noticeWarning("도움말 파일이 존재하지 않습니다.\nDoSA 디렉토리 > Help > Drawing_Guide_KOR.pdf 를 확인하세요.");
                else
                    CNotice.noticeWarning("Help file does not exist.\nPlease check DoSA Directory > Help > Drawing_Guide_ENG.pdf.");

                return;
            }

            System.Diagnostics.Process.Start(strHelpFileFullName);
        }

        private void buttonOpenHelpDirectory_Click(object sender, EventArgs e)
        {
            string strHelpFolderPath = Path.Combine(CSettingData.m_strProgramDirPath, "Help");

            System.Diagnostics.Process.Start(strHelpFolderPath);
        }
    }
}
