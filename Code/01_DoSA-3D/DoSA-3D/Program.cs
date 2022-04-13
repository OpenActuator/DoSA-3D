using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using gtLibrary;

// 다국어 언어 지원
using System.Globalization;
using System.Threading;

using Tests;

namespace DoSA
{
    public enum EMLanguage
    {
        Korean,
        English        
    };

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FormMain formMain = null;

            if (args.Length == 1)
                formMain = new FormMain(args[0]);
            else if (args.Length == 2)
                formMain = new FormMain(args[0], args[1]);
            else
                formMain = new FormMain();

            Application.Run(formMain);
        }
    }

    public class CSettingData
    {
        static CManageFile m_manageFile = new CManageFile();

        // 저장 변수들
        public static string m_strBaseWorkingDirPath { get; set; }

        public static string m_strCurrentWorkingDirPath { get; set; }

        public static string m_strGmshExeFileFullName { get; set; }

        // m_dMeshLevelPercent 이름을 변경하지말라. 다시 환경설정을 해야한다.
        public static double m_dMeshLevelPercent { get; set; }

        public static EMLanguage m_emLanguage { get; set; }

        public static EMActuatorType m_emActuatorType { get; set; }

		// 내부 사용변수
		// - 프로그램이 실행될때 초기화하여 내부에서 사용한다.
		public static string m_strProgramDirPath { get; set; }

        public static void updataLanguge()
        {
            if (m_emLanguage == EMLanguage.English)
            {
                CultureInfo ctInfo = new CultureInfo("en-US");

                Thread.CurrentThread.CurrentCulture = ctInfo;
                Thread.CurrentThread.CurrentUICulture = ctInfo;
            }
            else if (m_emLanguage == EMLanguage.Korean)
            {
                CultureInfo ctInfo = new CultureInfo("ko-KR");

                Thread.CurrentThread.CurrentCulture = ctInfo;
                Thread.CurrentThread.CurrentUICulture = ctInfo;
            }

        }

        public static bool isDataOK(bool bOpenNoticeDialog = true)
        {
            bool bCheck = false;

            bCheck = m_manageFile.isExistFile(m_strGmshExeFileFullName);
            if (bCheck == false)
            {
                if (bOpenNoticeDialog == true)
                    CNotice.noticeWarningID("TEFD");
                else
                    CNotice.printTraceID("TEFD");

                return false;
            }

            bCheck = m_manageFile.isExistDirectory(m_strBaseWorkingDirPath);

            if (bCheck == false)
            {
                if (bOpenNoticeDialog == true)
                    CNotice.noticeWarningID("TDWD");
                else
                    CNotice.printTraceID("TDWD");

                return false;
            }
            
            bCheck = m_manageFile.isExistDirectory(m_strProgramDirPath);

            if (bCheck == false)
            {
                if (bOpenNoticeDialog == true)
                    CNotice.noticeWarningID("TIAP2");
                else
                    CNotice.printTraceID("TIAP2");

                return false;
            }

            if(m_dMeshLevelPercent <= 0.05f)
            {
                if (bOpenNoticeDialog == true)
                    CNotice.noticeWarningID("TMSL");
                else
                    CNotice.printTraceID("TMSL");

                return false;
            }
            
            return true;
        }
    }

    // Static 객체인 환경설정 객체를 XML Serialization 하기 위해 임시로 사용하는 일반 클래스를 만들었다.
    public class CSettingDataClone
    {
        // 저장 변수들
        public string m_strWorkingDirName { get; set; }
        public string m_strGmshExeFileFullName { get; set; }
        
        // m_dMeshLevelPercent 이름을 변경하지말라. 다시 환경설정을 해야한다.
        public double m_dMeshLevelPercent { get; set; }
        public EMLanguage m_emLanguage { get; set; }

        public EMActuatorType m_emActuatorType { get; set; }

        public void copyCloneToSettingData()
        {
            CSettingData.m_strBaseWorkingDirPath = m_strWorkingDirName;
            CSettingData.m_strGmshExeFileFullName = m_strGmshExeFileFullName;
            CSettingData.m_dMeshLevelPercent = m_dMeshLevelPercent;
            CSettingData.m_emLanguage = m_emLanguage;
            CSettingData.m_emActuatorType = m_emActuatorType;
        }

        public void copySettingDataToClone()
        {
            m_strWorkingDirName = CSettingData.m_strBaseWorkingDirPath;
            m_strGmshExeFileFullName = CSettingData.m_strGmshExeFileFullName;
            m_dMeshLevelPercent = CSettingData.m_dMeshLevelPercent;
            m_emLanguage = CSettingData.m_emLanguage;
            m_emActuatorType = CSettingData.m_emActuatorType;
        }        
    }
}
