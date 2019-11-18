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
                strHelpFileFullName = Path.Combine(CSettingData.m_strProgramDirName, "Help", "DoSA_User_Guide_ENG.pdf");
            else if (ctInfo.Name == "ko-KR")
                strHelpFileFullName = Path.Combine(CSettingData.m_strProgramDirName, "Help", "DoSA_User_Guide_KOR.pdf");
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
                strHelpFileFullName = Path.Combine(CSettingData.m_strProgramDirName, "Help", "VCM_Sample_ENG.pdf");
            else if (ctInfo.Name == "ko-KR")
                strHelpFileFullName = Path.Combine(CSettingData.m_strProgramDirName, "Help", "VCM_Sample_KOR.pdf");
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
                strHelpFileFullName = Path.Combine(CSettingData.m_strProgramDirName, "Help", "Solenoid_Sample_ENG.pdf");
            else if (ctInfo.Name == "ko-KR")
                strHelpFileFullName = Path.Combine(CSettingData.m_strProgramDirName, "Help", "Solenoid_Sample_KOR.pdf");
            else
                return;

            if (m_manageFile.isExistFile(strHelpFileFullName) == false)
            {
                CNotice.noticeWarningID("HFDN1");
                return;
            }

            System.Diagnostics.Process.Start(strHelpFileFullName);
        }

    }
}
