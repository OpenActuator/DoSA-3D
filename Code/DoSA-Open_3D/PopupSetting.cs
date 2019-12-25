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

using Experiments;

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

            // CSettingData 으로 내린다.
            downloadSettingData();

            bCheck = CSettingData.isDataOK();

            if (bCheck == true)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void downloadSettingData()
        {
            try
            {
                CSettingData.m_strWorkingDirName = textBoxWorkingDirectory.Text;
                CSettingData.m_strGmshExeFileFullName = textBoxGmshPath.Text;

                CSettingData.m_dMeshLevelPercent = Double.Parse(textBoxMeshSizePercent.Text);
                CSettingData.m_emLanguage = (EMLanguage)Enum.Parse(typeof(EMLanguage), comboBoxLanguage.Text);
                CSettingData.m_emActuatorType = (EMActuatorType)Enum.Parse(typeof(EMActuatorType), comboBoxActuatorType.Text);
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
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
                textBoxWorkingDirectory.Text = CSettingData.m_strWorkingDirName;
                textBoxGmshPath.Text = CSettingData.m_strGmshExeFileFullName;

                textBoxMeshSizePercent.Text = CSettingData.m_dMeshLevelPercent.ToString();
                comboBoxLanguage.Text = CSettingData.m_emLanguage.ToString();
                comboBoxActuatorType.Text = CSettingData.m_emActuatorType.ToString();
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
            }
        }

        public bool saveSettingToFile()
        {
            string strSettingFileFullName = Path.Combine(CSettingData.m_strProgramDirName, "setting.ini");

            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(CSettingDataClone));
                StreamWriter writer = new StreamWriter(strSettingFileFullName);

                // Static 객체는 XML Serialize 가 불가능해서 일반 Clone 객체에 복사를 하고 Serialize 를 하고 있다. 
                CSettingDataClone settingData = new CSettingDataClone();
                settingData.copySettingDataToClone();

                xmlSerializer.Serialize(writer, settingData);
                writer.Close();
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
            }

            return true;
        }

        public bool loadSettingFromFile()
        {
            string strSettingFileFullName = Path.Combine(CSettingData.m_strProgramDirName, "setting.ini");

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
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
                CNotice.printTraceID("AEOW");
            }
            
            return true;
        }
    }
}
