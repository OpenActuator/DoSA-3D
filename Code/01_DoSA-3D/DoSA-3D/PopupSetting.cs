using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Xml.Serialization;
using gtLibrary;

using Tests;

namespace DoSA
{
    public partial class PopupSetting : Form
    {
        CManageFile m_manageFile = new CManageFile();
        
        public PopupSetting()
        {
            InitializeComponent();
        }

        private void buttonSettingOK_Click(object sender, EventArgs e)
        {
            bool bCheck;

            try
            {
                // CSettingData 으로 내린다.
                downloadSettingData();

                bCheck = CSettingData.isSettingDataOK();

                if (bCheck == true)
                {
                    string strSampleDirPath = Path.Combine(CSettingData.m_strProgramDirPath,"Samples");

                    List<string> listDir = m_manageFile.getDirectoryList(CSettingData.m_strBaseWorkingDirPath);
                    List<string> listFile = m_manageFile.getFileList(CSettingData.m_strBaseWorkingDirPath);

                    // 새로 지정한 작업디렉토리에 디렉토리나 파일이 없다면 Sample 디렉토리의 샘플 디자인을 복사한다.
                    if (listDir.Count == 0 && listFile.Count == 0)
                    {
                        m_manageFile.copyDirectory(strSampleDirPath, CSettingData.m_strBaseWorkingDirPath);
                    }

                    // 수정된 기본 작업 디렉토리가 현 작업 디렉토리가 되도록 설정한다.
                    CSettingData.m_strCurrentWorkingDirPath = CSettingData.m_strBaseWorkingDirPath;

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);

                return;
            }
        }

        private void downloadSettingData()
        {
            try
            {
                CSettingData.m_strBaseWorkingDirPath = textBoxWorkingDirectory.Text;
                CSettingData.m_strGmshExeFileFullName = textBoxGmshPath.Text;

                CSettingData.m_dMeshLevelPercent = Double.Parse(textBoxMeshSizePercent.Text);
                CSettingData.m_emLanguage = (EMLanguage)Enum.Parse(typeof(EMLanguage), comboBoxLanguage.Text);
            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);

                return;
            }
        }

        private void buttonSettingCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonSelectWorkingDirectory_Click(object sender, EventArgs e)
        {
            // 폴더 선택창 띄우기
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBoxWorkingDirectory.Text = dialog.SelectedPath;
            }
        }

        private void buttonSelectGmshPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // 파일 열기창 설정
            openFileDialog.Title = "Select a Exe File";
            openFileDialog.FileName = null;
            openFileDialog.Filter = "Gmsh EXE files|gmsh.exe|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
                this.textBoxGmshPath.Text = openFileDialog.FileName;
        }

        /// <summary>
        /// 초기설정때 가급적 현 기본언어로 설정을 맞추어 준다.
        /// </summary>
        /// <param name="currentLanguage"></param>
        public void setInitLanguage(EMLanguage currentLanguage)
        {
            comboBoxLanguage.Text = currentLanguage.ToString();
        }

        public void uploadSettingData()
        {
            try
            {
                textBoxWorkingDirectory.Text = CSettingData.m_strBaseWorkingDirPath;
                textBoxGmshPath.Text = CSettingData.m_strGmshExeFileFullName;

                textBoxMeshSizePercent.Text = CSettingData.m_dMeshLevelPercent.ToString();
                comboBoxLanguage.Text = CSettingData.m_emLanguage.ToString();
            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);

                return;
            }
        }

        public bool saveSettingToFile()
        {
            string strAppDataPath = Environment.GetEnvironmentVariable("APPDATA");
            string strSettingFilePath = Path.Combine(strAppDataPath, "DoSA-3D");

            string strSettingFileFullName = Path.Combine(strSettingFilePath, "setting.ini");

            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(CSettingDataClone));

                // StreamWriter의 기본 Encoding 은 UTF-8 이다.
                // 특정문자(터키어 등)에 문제가 될 수 있으나,
                // 해당언어로 디렉토리를 만드는 경우가 많을 것으로 판단해서 호환성을 높이기 위해 기존처럼 UTF-8 을 그대로 사용한다
                StreamWriter writer = new StreamWriter(strSettingFileFullName);

                // Static 객체는 XML Serialize 가 불가능해서 일반 Clone 객체에 복사를 하고 Serialize 를 하고 있다. 
                CSettingDataClone settingData = new CSettingDataClone();
                settingData.copySettingDataToClone();

                xmlSerializer.Serialize(writer, settingData);
                writer.Close();
            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);

                return false;
            }

            return true;
        }

        public bool loadSettingFromFile()
        {
            string strAppDataPath = Environment.GetEnvironmentVariable("APPDATA");
            string strSettingFilePath = Path.Combine(strAppDataPath, "DoSA-3D");

            string strSettingFileFullName = Path.Combine(strSettingFilePath, "setting.ini");

            // CSettingData.ProgramDirectory 가 초기화 되어 있어야 한다.
            if (m_manageFile.isExistFile(strSettingFileFullName) == false)
            {
                CNotice.noticeWarningID("TCFD");
                return false;
            }

            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(CSettingDataClone));
                StreamReader reader = new StreamReader(strSettingFileFullName);

                CSettingDataClone settingDataClone = new CSettingDataClone();
                settingDataClone = (CSettingDataClone)xmlSerializer.Deserialize(reader);

                settingDataClone.copyCloneToSettingData();

                reader.Close();

                // 혹시 데이터의 오류는 발생하더라도 하나만 오류가 발생한다.
                // 따라서 다른 항목까지 다시 설정하지 않도록 오류가 있는 데이터라도 파일에서 읽어드림과 동시에 창에 입력해 둔다.
                uploadSettingData();
            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);
                CNotice.printLogID("AEOW");

                return false;
            }
            
            return true;
        }

    }
}
