using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoSA
{
    public partial class PopupNewVersion : Form
    {
        public int m_iStatus;

        public PopupNewVersion(string strNewVersion, string strProductVersion)
        {
            InitializeComponent();

            this.labelNewVersionDisplay.Text = strNewVersion;
            this.labelProductVersionDisplay.Text = strProductVersion;

            if (CSettingData.m_emLanguage == EMLanguage.Korean)
            {
                this.buttonOpenWebSite.Text = "최신버전 다운페이지로 이동";
                this.buttonStopNotice.Text = "최신버전 공지 띄우지 않기";                
            }
            else
            {
                this.buttonOpenWebSite.Text = "Move to the download website";                
                this.buttonStopNotice.Text = "Stop the new version notice";                
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            // 아무것도 하지 않음
            m_iStatus = 3;

            this.Close();
        }

        private void buttonOpenWebSite_Click(object sender, EventArgs e)
        {
            // 다운로드 웹사이트를 연다.
            m_iStatus = 1;

            this.Close();
        }

        private void buttonStopNotice_Click(object sender, EventArgs e)
        {
            // 다운로드를 하지 않고 버전 확인을 하지 않는다.
            m_iStatus = 2;

            this.Close();
        }
    }
}
