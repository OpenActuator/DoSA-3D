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
    public partial class PopupChangeShape : Form
    {
        CManageFile m_manageFile = new CManageFile();

        public string m_strSTEPFileFullName;

        public PopupChangeShape()
        {
            InitializeComponent();

            if (CSettingData.m_emLanguage == EMLanguage.Korean)
            {
                groupBoxComment.Text = "주의 사항";
                labelComment1.Text = "- 작업 형상과 수정 형상의 파트 개수는 같아야 한다.";
                labelComment2.Text = "- 작업 형상과 수정 형상의 파트 이름은 일치해야 한다.";
            }
            else
            {
                groupBoxComment.Text = "Caution";
                labelComment1.Text = "- The number of parts should be the same.";
                labelComment2.Text = "- The part name must match.";
            }
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
                if (textBoxSTEPFileFullName.Text.Length == 0)
                {
                    if (CSettingData.m_emLanguage == EMLanguage.Korean)
                        CNotice.noticeWarning("STEP 파일을 선택해 주세요.");
                    else
                        CNotice.noticeWarning("You need to select a STEP file.");

                    return false;
                }

                // 가능성은 낮지만 선택한 STEP 파일이 없는지를 검사한다.
                bCheck = m_manageFile.isExistFile(this.textBoxSTEPFileFullName.Text);
                if (bCheck == false)
                {
                    CNotice.printLog("선택한 STEP 파일이 존재하지 않는다.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);

                return false;
            }

            return true;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // 검증전에 설정 되어야 함
            m_strSTEPFileFullName = textBoxSTEPFileFullName.Text;

            bool retOK = isInputDataOK();

            if (retOK == true)
                this.DialogResult = DialogResult.OK;
        }

        private void textSTEPFileBoxName_KeyPress(object sender, KeyPressEventArgs e)
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
            openFileDialog.InitialDirectory = CSettingData.m_strCurrentWorkingDirPath;
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
