using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using gtLibrary;
using Nodes;

using System.IO;

namespace DoSA
{
    public partial class PopupNewDesign : Form
    {
        CManageFile m_manageFile = new CManageFile();

        public string m_strDesignName;
        public string m_strSTEPFileFullName;

        public PopupNewDesign()
        {
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private bool isInputDataOK()
        {
            bool bCheck;

            try
            {
                // 빈칸 확인으로 null 비교를 사용하지 말라. (.Length == 0 나 "" 를 사용하라)
                if (textBoxDesignName.Text.Length == 0)
                {
                    CNotice.noticeWarningID("PEAN");
                    return false;
                }

                if (textBoxSTEPFileFullName.Text.Length == 0)
                {
                    CNotice.noticeWarning("STEP 을 선택하지 않았습니다.");
                    return false;
                }

                // 가능성은 낮지만 선택한 STEP 파일이 없는지를 검사한다.
                bCheck = m_manageFile.isExistFile(this.textBoxSTEPFileFullName.Text);
                if (bCheck == false)
                {
                    CNotice.noticeWarning("선택한 STEP 파일이 존재하지 않습니다.");
                    return false;
                }

                // 디자인을 무조건 프로그램 작업디렉토리에 생성하는 것으로 한다.
                // 따라서 디자인을 생성할 때의 적용버튼 임으로 작업 디렉토리는 프로그램 작업 디렉토리를 사용하고 있다.
                List<string> listDirectories = m_manageFile.getDirectoryList(CSettingData.m_strWorkingDirName);

                // 소문자로 비교하기 위해서 임시로 사용한다.
                string strOldTempName, strNewTempName;

                foreach (string directoryName in listDirectories)
                {
                    // 디렉토리 경로에 GetFileName 을 사용하면 가장 마지막 디렉토리가 넘어온다.
                    strOldTempName = Path.GetFileName(directoryName).ToLower();
                    strNewTempName = m_strDesignName.ToLower();

                    if (strOldTempName == strNewTempName)
                    {
                        // 기존 디자인이 이미 존재할 때 삭제하고 새롭게 시작할지를 물어 온다
                        DialogResult ret = CNotice.noticeWarningOKCancelID("TSDA", "W");

                        if (ret == DialogResult.OK)
                        {
                            m_manageFile.deleteDirectory(directoryName);
                            return true;
                        }
                        else
                            return false;
                    }
                }
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
                return false;
            }

            return true;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // 검증전에 설정 되어야 함
            m_strDesignName = textBoxDesignName.Text;
            m_strSTEPFileFullName = textBoxSTEPFileFullName.Text;

            bool retOK = isInputDataOK();

            if (retOK == true)
                this.DialogResult = DialogResult.OK;
        }

        private void textBoxName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (System.Text.Encoding.UTF8.GetByteCount(new char[] { e.KeyChar }) > 1)
            {
                e.Handled = true;
            }
            /// 이름만 입력을 받는 Popup 창이기 때문에
            /// Enter 가 들어오면 OK Button 과 동일하게 처리한다
            else if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                buttonOK.PerformClick();
            }
        }

        private void buttonSelectSTEP_File_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // 파일 열기창 설정
            openFileDialog.Title = "Select a STEP File";
            
            // 디렉토리는 프로그램 작업 디렉토리로 하고 있다.
            openFileDialog.InitialDirectory = CSettingData.m_strWorkingDirName;
            openFileDialog.FileName = null;
            openFileDialog.Filter = "STEP File (*.step;*.stp)|*.step;*.stp";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                this.textBoxSTEPFileFullName.Text = openFileDialog.FileName;
            }
        }

    }
}
