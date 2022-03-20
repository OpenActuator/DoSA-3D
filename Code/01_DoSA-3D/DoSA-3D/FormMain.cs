using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Debugging
using System.Diagnostics;

// File
using System.IO;

using System.Reflection;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;

using Microsoft.Win32;
using System.Resources;
using System.Globalization;

// DoSA 생성 클래스들을 오픈한다
// 같은 namespace 를 사용해도 가능하나 ClassView 에서 보기가 어려워서 구분해서 사용한다.
using Parts;
using Experiments;
using Nodes;
using gtLibrary;
using Onelab;
using System.Net;

namespace DoSA
{
    public partial class FormMain : Form
    {
        #region-------------------------- 내부 변수 ----------------------------

        // Treeview 접근 INDEX
		const int FIRST_PARTS_INDEX = 0;
		const int FIRST_ANALYSIS_INDEX = 1;

        private CManageFile m_manageFile = new CManageFile();

        private string m_strBackupNodeName = string.Empty;

        private string m_strCommandLineDesignFullName = string.Empty;
        private string m_strCommandLineDataFullName = string.Empty;

        public CDesign m_design = new CDesign();

        public ResourceManager m_resManager = null;        

        #endregion

        #region----------------------- 프로그램 초기화 --------------------------

        public FormMain(string strDSAFileFullName = null, string strDataFileFullName = null)
        {
            InitializeComponent();


            // 여러곳에서 CSettingData 을 사용하기 때문에 가장 먼저 실시한다.
            CSettingData.m_strProgramDirPath = System.Windows.Forms.Application.StartupPath;

            m_resManager = ResourceManager.CreateFileBasedResourceManager("LanguageResource", Application.StartupPath, null);

            ///------------------------------------------------------------------------
            /// 환경설정전 언어의 초기 설정
            /// 
            /// 환경설정의 언어 설정값을 읽어드리기 전에 혹시 언어를 사용하는 경우를 대비하여
            /// 환경설정의 언어 설정과 상관없이 무조건 시스템언어를 읽어서 프로그램 언어를 설정해 둔다.
            /// 
            /// 환경설정값으로 언어 설정은 이후에 바로 이어지는 CSettingData.updataLanguge() 에서 이루어진다.
            CultureInfo ctInfo = Thread.CurrentThread.CurrentCulture;

            /// 한국어가 아니라면 모두 영어로 처리하라.
            if (ctInfo.Name.Contains("ko") == true)
                CSettingData.m_emLanguage = EMLanguage.Korean;
            else
                CSettingData.m_emLanguage = EMLanguage.English;

            CSettingData.updataLanguge();
            ///------------------------------------------------------------------------


            /// 리소스 파일을 확인하다.
            bool retEnglish, retKorean;
            retEnglish = m_manageFile.isExistFile(Path.Combine(Application.StartupPath, "LanguageResource.en-US.resources"));
            retKorean = m_manageFile.isExistFile(Path.Combine(Application.StartupPath, "LanguageResource.ko-KR.resources"));

            if(retEnglish == false || retKorean == false)
            {
                MessageBox.Show("There are no Language resource files.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                System.Windows.Forms.Application.ExitThread();
                Environment.Exit(0);            
            }

            int nDoSACount = CManageProcess.getProcessesCount("DoSA-3D");

            if (nDoSACount >= 2)
            {
                if(CSettingData.m_emLanguage == EMLanguage.English)
                    CNotice.noticeWarning("DoSA-3D 의 중복 실행은 허용하지 않습니다.");
                else
                    CNotice.noticeWarning("Duplicate execution of DoSA-3D is not allowed.");

                System.Windows.Forms.Application.ExitThread();
                Environment.Exit(0);
            }

            // 설치버전을 확인 한다.
            checkDoSAVersion();

            //=====================================================================
            // 2023-03-18일 까지 코드를 유지하고 이후는 삭제한다
            //=====================================================================
            deleteOldDirectories();
            //=====================================================================

            initializeProgram();

            // 환경설정의 기본 작업디렉토리의 해당 프로그램의 디렉토리로 일단 설정한다.
            // 환경설정을 읽어온 후 에 초기화 해야 한다.
            // 주의사항 : initializeProgram() 뒤에 호출 해야 한다.
            CSettingData.m_strCurrentWorkingDirPath = CSettingData.m_strBaseWorkingDirPath;

            // FEMM 에서 지원되는 재질을 Loading 한다.
            loadMaterial();

            //m_femm = null;

            /// 파라메터 처리 저장
            /// 
            /// Command Parameter 0 : 일반 실행
            /// Command Parameter 1 : 지정 디자인만 오픈
            /// Command Parameter 2 : 지정 디장인을 열고, 입력데이터 파일로 작업을 함
            /// 
            if (strDSAFileFullName != null)
            {
                m_strCommandLineDesignFullName = strDSAFileFullName;
                if(strDataFileFullName != null)
                    m_strCommandLineDataFullName = strDataFileFullName;
            }                
        }

        //----------- Update Dialog Test --------------
        // - WiFi 를 연결하고, AssemblyInfo 에서 버전을 임의로 낮춘다.
        private void checkDoSAVersion()
        {
            try
            {
                // 인터넷이 연결되지 않으면 예외가 발생하여 catch 로 넘어가고 프로그램이 실행된다.
                string strNewVersion = new WebClient().DownloadString("http://www.actuator.or.kr/DoSA_3D_Version.txt");

                string strAppDataPath = Environment.GetEnvironmentVariable("APPDATA");
                string strSettingFilePath = Path.Combine(strAppDataPath, "DoSA-3D");

                if (m_manageFile.isExistDirectory(strSettingFilePath) == false)
                    m_manageFile.createDirectory(strSettingFilePath);

                string strVersionPassFileFullName = Path.Combine(strSettingFilePath, "VersionPass.txt");

                /// 버전관리 유의사항
                /// 
                /// AssemblyInfo.cs 의 AssemblyVersion 이 아니라 AssemblyFileVersion 이 DoSA 실행파일의 Product Version 이다.
                /// 따라서 DoSA 자동업데이트나 업데이트 요청메시지를 띄우기 위해 버전 확인을 DoSA 실행파일의 버전을 사용하고 있다.
                /// 
                /// About 창에서도 동일한 버전으로 표기하기 위해 AssemblyFileVersion 를 사용하려고 하였으나 
                /// AssemblyFileVersion 는 직접 읽어오지 못해서 여기서도 DoSA 실행파일의 버전을 읽어서 ProductVersion 을 읽어낸다.
                string strEXE_FileName = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string strProductVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(strEXE_FileName).ProductVersion;

                // Version 뒤에 Update 내용을 붙히려다가 하위호환 문제로 포기했다.
                //
                //string[] arrayUpdateInform = strUpdateInform.Split('.');

                //string strNewVersion = string.Empty;
                //string strMainUpdateContents = string.Empty;

                //if (arrayUpdateInform.Length == 5)
                //{
                //    int index = strUpdateInform.LastIndexOf(".");

                //    strNewVersion = strUpdateInform.Remove(index);
                //    strMainUpdateContents = arrayUpdateInform[4];
                //}

                string[] arrayNewVersion = strNewVersion.Split('.');
                string[] arrayProductVersion = strProductVersion.Split('.');

                int iNewVersion = 0;
                int iProductVersion = 0;

                // 버전에 문제가 있으면 바로 리턴한다.
                if (arrayNewVersion.Length != 4 || arrayProductVersion.Length != 4)
                    return;

                // 3 자리만 사용한다. 마지막 자리는 너무 자주 버전업이 되어서 사용자들에 불편을 준다
                // ex) 0.9.4.2 -> 마지막 2가 버려지고 94 가 된다.
                for(int i=0; i < 3; i++)
                {
                    iNewVersion += (int)(Convert.ToInt32(arrayNewVersion[i]) * Math.Pow(10.0, (double)(2 - i)));
                    iProductVersion += (int)(Convert.ToInt32(arrayProductVersion[i]) * Math.Pow(10.0, (double)(2 - i)));
                }

                bool bVersionCheckDialog = false;

                CReadFile readFile = new CReadFile();
                CWriteFile writeFile = new CWriteFile();

                string strPassVersion;
                string[] arrayPassVersion;
                int iPassVersion;

                if(iNewVersion > iProductVersion)
                {
                    // 이전에 업그레이드 안함을 선택하여 PassVersion 파일이 있는 경우
                    if(m_manageFile.isExistFile(strVersionPassFileFullName) == true)
                    {
                        strPassVersion = readFile.getLine(strVersionPassFileFullName, 1);

                        arrayPassVersion = strPassVersion.Split('.');

                        // 버전에 문제가 있으면 바로 리턴한다.
                        if (arrayPassVersion.Length != 4)
                            return;

                        iPassVersion = 0;

                        // 업그레이드 확인은 셋째 자리수로 결정된다. (마지막 자리수는 사용되지 않는다.)
                        for (int i = 0; i < 3; i++)
                            iPassVersion += (int)(Convert.ToInt32(arrayPassVersion[i]) * Math.Pow(10.0, (double)(2 - i)));

                        // 저장된 보지 않기를 원하는 버전보다 신규버전이 높을 때만 신규버전 알림창을 띄운다.
                        if (iNewVersion > iPassVersion)
                            bVersionCheckDialog = true;
                        else
                            bVersionCheckDialog = false;
                    }
                    else
                    {
                        bVersionCheckDialog = true;
                    }
                }

                // 신규버전을 알리는 창을 띄운다.
                if(bVersionCheckDialog == true)
                {
                    // 인터넷이 연결되지 않으면 예외가 발생하여 catch 로 넘어가고 프로그램이 실행된다.
                    string strMainUpdateContents = new WebClient().DownloadString("http://www.actuator.or.kr/DoSA_3D_Update_Contents.txt");

                    PopupNewVersion formNewVersion = new PopupNewVersion(strNewVersion, strProductVersion, strMainUpdateContents);
                    formNewVersion.StartPosition = FormStartPosition.CenterParent;

                    formNewVersion.ShowDialog();

                    // 취소를 하면 버전 확인 상관없이 프로그램이 실행 된다.
                    if (formNewVersion.m_iStatus == 3)
                        return;

                    // 프로그램을 종료 하고 다운로드 웹사이트로 이동한다.
                    // 단, 프로그램을 업데이트하지 않으면 다시 알림 창이 뜬다.
                    if (formNewVersion.m_iStatus == 1)
                    {
                        string target;

                        if (CSettingData.m_emLanguage == EMLanguage.Korean)
                        {
                            target = "https://solenoid.or.kr/direct_kor.php?address=https://solenoid.or.kr/openactuator/dosa_3d_kor.htm";

                            // DoSA 이전 버전의 주소 설정이 아래와 같아서 해당 html 을 삭제하고 못하고 있다.
                            //target = "http://solenoid.or.kr/index_dosa_open_3d_kor.html";
                        }
                        else
                        {
                            target = "https://solenoid.or.kr/direct_eng.php?address=https://solenoid.or.kr/openactuator/dosa_3d_eng.htm";

                            // DoSA 이전 버전의 주소 설정이 아래와 같아서 해당 html 을 삭제하고 못하고 있다.
                            //target = "http://solenoid.or.kr/index_dosa_open_3d_eng.html";
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
                        
                        System.Windows.Forms.Application.ExitThread();
                        Environment.Exit(0);
                    }
                    // formNewVersion.m_iStatus == 2 인 경우로 지금 New 버전에 대한 공지를 띄우지 않는 것이다.
                    else
                    {
                        List<string> listStirng = new List<string>();
                        listStirng.Add(strNewVersion);

                        writeFile.writeLineString(strVersionPassFileFullName, listStirng, true);
                    }
                }
            }
            // 인터넷이 연결되지 않았으면 예외 처리가 되면서 함수를 빠져 나간다.
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
            }

        }

        /// <summary>
        /// 2023-03-18일 까지 코드를 유지하고 이후는 삭제한다.
        /// 
        /// 2022-03-18 의 DoSA-3D Ver0.9.12.7 에서 
        /// 프로그램 명칭을 DoSA-Open_2D --> DoSA-2D 로 변경되어서 남게된 
        /// 기존 설치 프로그램과 작업환경 디렉토리를 삭제한다.
        /// 
        /// 2022-03-19 의 DoSA-3D Ver0.9.13.0 에서
        /// 샘플 명칭이 LV --> VCM 으로 변경되어서 필요없는 기존 설치 예제 디렉토리를 삭제한다.
        /// </summary>
        private void deleteOldDirectories()
        {
            string strAppDataDirPath = Environment.GetEnvironmentVariable("APPDATA");
            string strSettingDirPath = Path.Combine(strAppDataDirPath, "DoSA-3D");
            string strOldSettingDirPath = Path.Combine(strAppDataDirPath, "DoSA-Open_3D");

            string strParentDirPath = Path.GetDirectoryName(CSettingData.m_strProgramDirPath);
            string strOldInstallDirPath = Path.Combine(strParentDirPath, "DoSA-Open_3D");

            string[] arrayDirName = { strParentDirPath, "DoSA-3D", "Samples", "LV" };
            string strOldExampleDirPath = Path.Combine(arrayDirName);

            // 기존 작업환경 디렉토리가 있으면 디렉토리을 바꾸어 복사하고 기존 디렉토리는 삭제한다.
            if (m_manageFile.isExistDirectory(strOldSettingDirPath) == true)
            {
                m_manageFile.copyDirectory(strOldSettingDirPath, strSettingDirPath);
                m_manageFile.deleteDirectory(strOldSettingDirPath);
            }

            // 기존 설치 디렉토리가 있으면 삭제한다.
            if (m_manageFile.isExistDirectory(strOldInstallDirPath) == true)
            {
                m_manageFile.deleteDirectory(strOldInstallDirPath);
            }

            // 기존 사용 Example 디렉토리가 있으면 삭제한다.
            if (m_manageFile.isExistDirectory(strOldExampleDirPath) == true)
            {
                m_manageFile.deleteDirectory(strOldExampleDirPath);
            }

        }

        private void initializeProgram()
        {
            try
            {
                /// Net Framework V4.51 이전버전이 설치 되었는지를 확인한다.
                bool retFreamework = checkFramework451();

                if (retFreamework == false)
                {
                    DialogResult result = CNotice.noticeWarningOKCancelID("DRIO1", "W");
                    
                    if(result == DialogResult.OK )
                        openWebsite(@"https://www.microsoft.com/ko-kr/download/details.aspx?id=30653");

                    System.Windows.Forms.Application.ExitThread();
                    Environment.Exit(0);
                }

                // Log 디렉토리가 없으면 생성 한다.
                string strLogDirName = Path.Combine(CSettingData.m_strProgramDirPath, "Log");

                if (m_manageFile.isExistDirectory(strLogDirName) == false)
                    m_manageFile.createDirectory(strLogDirName);

                // 출력방향을 결정함 (아래 코드가 동작하면 파일 출력, 동작하지 않으면 Output 창 출력)
                Trace.Listeners.Add(new TextWriterTraceListener(Path.Combine(CSettingData.m_strProgramDirPath, "Log", DateTime.Now.ToString("yyyyMMdd_HH_mm_ss") + ".Log")));

                // 이벤트 생성 부
                // 
                // 내부함수인 printLogEvent() 의 함수포인트를 사용해서 이벤트 함수를 설정한다
                CNotice.Notice += printLogEvent;

                string strAppDataDirPath = Environment.GetEnvironmentVariable("APPDATA");
                string strSettingDirPath = Path.Combine(strAppDataDirPath, "DoSA-3D");

                if (m_manageFile.isExistDirectory(strSettingDirPath) == false)
                    m_manageFile.createDirectory(strSettingDirPath);
                
                /// 환경파일 작업
                ///
                string strSettingFileFullName = Path.Combine(strSettingDirPath, "setting.ini");

                /// 환경설정을 읽어드리는 기능이 PopupSetting 안에 들어있기 때문에
                /// 환경설정 파일이 있어서 PopupSetting 창을 띄우지 않더라도 PopupSetting 객체 생성을 하고 있다.
                PopupSetting frmSetting = new PopupSetting();
                frmSetting.StartPosition = FormStartPosition.CenterParent;

                if (false == m_manageFile.isExistFile(strSettingFileFullName))
                {
                    // 현 상태에서의 언어설정은 FormMain() 에서 시스템 언어로 설정되어 있다.
                    CNotice.noticeWarningID("TCFC");
                    
                    // 파일자체가 없기 때문에 다이얼로그의 데이터 설정없이 바로 호출한다.
                    if (DialogResult.OK == frmSetting.ShowDialog())
                    {
                        frmSetting.saveSettingToFile();
                    }
                    else
                    {
                        CNotice.noticeWarningID("IYCT");

                        System.Windows.Forms.Application.ExitThread();
                        Environment.Exit(0);
                    }
                }
                else
                {
                    frmSetting.loadSettingFromFile();

                    if (CSettingData.isDataOK(false) == false)
                    {
                        CNotice.noticeWarningID("TIAP7");

                        if (DialogResult.OK == frmSetting.ShowDialog())
                        {
                            frmSetting.saveSettingToFile();
                        }
                        else
                        {
                            CNotice.noticeWarningID("IYCT");

                            System.Windows.Forms.Application.ExitThread();
                            Environment.Exit(0);
                        }
                    }

                    // WorkingDirectory 을 읽어온 후에 
                    // 작업의 편의를 위해 디렉토리를 WorkingDirectory 로 변경한다.
                    m_manageFile.setCurrentDirectory(CSettingData.m_strBaseWorkingDirPath);
                }

                /// 파일에서 읽어오든 신규파일에서 생성을 하든 Setting 파일안의 프로그램 언어를 설정한다.
                CSettingData.updataLanguge();                
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
            }
        }

        #endregion

        #region----------------------- Notice Event 호출 함수 ----------------------

        // 이벤트 발생 때 호출되는 함수
        void printLogEvent(EMOutputTarget emTarget, string strMSG)
        {
            if (emTarget == EMOutputTarget.TRACE)
            {
                Trace.WriteLine(DateTime.Now.ToString() + ", " + strMSG);
                Trace.Flush();
            }
            else
            {
                messageListView.addItem(strMSG);
            }
        }

        #endregion

        #region--------------------- 재질 초기화 ---------------------------
        
        private void loadMaterial()
        {
            List<string> listMaterialNames = new List<string>();

            CReadFile readFile = new CReadFile();

            string strProgramMaterialDirName = Path.Combine(CSettingData.m_strProgramDirPath, "Materials");

            try
            {

                #region //--------------------- 기본 재료 추가하기 -------------------------

                //------------------------------------------------
                // 자기회로 Maxwell 내장 연자성 재료
                //------------------------------------------------
                // 내장 연자성재료를 추가할 때는 BH 곡선의 내장 연자성재료 설정도 같이 변경해 주어야 한다
                
                string strMaterialFileFullName = Path.Combine(strProgramMaterialDirName, "DoSA_MS.dmat");

                readFile.readMaterialNames(strMaterialFileFullName, ref listMaterialNames);

                foreach (string strName in listMaterialNames)
                    CPropertyItemList.steelList.Add(strName);

                //------------------------------------------------
                // 자기회로 내장 영구자석
                //------------------------------------------------
                strMaterialFileFullName = Path.Combine(strProgramMaterialDirName, "DoSA_MG.dmat");

                readFile.readMaterialNames(strMaterialFileFullName, ref listMaterialNames);

                foreach (string strName in listMaterialNames)
                    CPropertyItemList.magnetList.Add(strName);

                //------------------------------------------------
                // 코일 동선 재료
                //------------------------------------------------
                CPropertyItemList.coilWireList.Add("Aluminum, 1100");
                CPropertyItemList.coilWireList.Add("Copper");

                #endregion
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
            }

        }
        #endregion

        #region------------------------- 전체 초기화 ------------------------
        //전체 초기화 한다
        private void closeDesign()
        {
            // 기존 자료를 초기화 한다
            // Second Node 들을 삭제한다.
            foreach (TreeNode firstLayerNode in treeViewMain.Nodes)
                firstLayerNode.Nodes.Clear();

            // First Node 들을 삭제한다.
            treeViewMain.Nodes.Clear();
            
            m_design.clearDesign();

            splitContainerRight.Panel1.Controls.Clear();
            splitContainerRight.Panel1.Controls.Add(this.panelEmpty);

            // PropertyGrid 창을 초기화 한다.
            propertyGridMain.SelectedObject = null;
        }
        #endregion

        #region--------------------- Ribbon Menu ---------------------------

        private void ribbonButtonShape_Click(object sender, EventArgs e)
        {
            List<string> listScriptString = new List<string>();
            CWriteFile writeFile = new CWriteFile();

            try
            {
                if (m_design.m_strDesignName.Length == 0)
                {
                    if (CSettingData.m_emLanguage == EMLanguage.Korean)
                        CNotice.noticeWarning("3차원 형상을 보여줄 디자인이 없습니다.");
                    else
                        CNotice.noticeWarning("There is no design to show 3D shape.This job is canceled.");
                    
                    return;
                }

                #region ------------------------- 디렉토리 및 파일명 설정 ------------------------

                // 생성을 할 때는 기본 작업 디렉토리를 사용해서 Actuator 작업파일의 절대 경로를 지정하고,
                // 작업파일을 Open 할 때는 파일을 오픈하는 위치에서 작업 디렉토리를 얻어내어 다시 설정한다.
                // 왜냐하면, 만약 작업 디렉토리를 수정하는 경우 기존의 작업파일을 열 수 없기 때문이다.
                string strDesignDirPath = m_design.m_strDesignDirPath;
                
                string strGmshExeFileFullName = CSettingData.m_strGmshExeFileFullName;

                // 형상 디렉토리 
                string strShapeDirName = Path.Combine(strDesignDirPath, "Shape");
                // 형상 디렉토리안의 만들어진다.
                string strShapeModelFileFullName = Path.Combine(strShapeDirName, m_design.m_strDesignName + ".step");

                // CheckStep Script 는 형상 디렉토리에서 작업을 한다.
                string strRunScriptFileFullName = Path.Combine(strShapeDirName, "Shape.geo");

                #endregion

                CScriptContents scriptContents = new CScriptContents();

                string strOrgStriptContents = scriptContents.m_str01_1_Show_Shape_Script;

                listScriptString.Add(strShapeModelFileFullName);

                writeFile.createScriptFileUsingString(strOrgStriptContents, strRunScriptFileFullName, listScriptString);
                if (true == writeFile.createScriptFileUsingString(strOrgStriptContents, strRunScriptFileFullName, listScriptString))
                {
                    // Process 의 Arguments 에서 스페이스 문제가 발생한다.
                    // 아래와 같이 묶음처리를 사용한다.
                    string strArguments = " " + m_manageFile.solveDirectoryNameInPC(strRunScriptFileFullName);
       
                    if (false == m_manageFile.isExistFile(strShapeModelFileFullName))
                    {
                        CNotice.printTrace("형상 파일을 찾지 못했다.");
                        return;
                    }

                    // Gmsh 를 종료할 때까지 기다리지 않는다.
                    CScript.runScript(strGmshExeFileFullName, strArguments, false);
                }
                else
                {
                    CNotice.printTrace("Shape Script 파일 생성에 문제가 발생했다.");
                    return;
                }
                
                Thread.Sleep(500);

                //m_manageFile.deleteFile(strRunScriptFileFullName);

            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
                return;
            }

        }

        private void ribbonButtonNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_design.m_bChanged == true)
                {
                    if (DialogResult.OK == CNotice.noticeWarningOKCancelID("DYWT", "W"))
                    {
                        saveDesignFile();
                    }
                }

                List<string> listScriptString = new List<string>();
                List<string> listTempPartNames = new List<string>();
                List<string> listAllPartNames = new List<string>();
                List<string> listRemainedPartNames = new List<string>();                

                PopupNewDesign formNewDesign = new PopupNewDesign();
                formNewDesign.StartPosition = FormStartPosition.CenterParent;

                /// 이해할 수 없지만, 자동으로 Owner 설정이 되는 경우도 있고 아닌 경우도 있기 때문에
                /// Shape 창에서 MainForm 을 접근할 수 있도록 미리 설정을 한다.
                formNewDesign.Owner = this;

                if (DialogResult.Cancel == formNewDesign.ShowDialog())
                    return;

                // 기존 디자인 데이터를 모두 삭제한다.
                closeDesign();

                // 확장자나 경로가 없는 디자인 명이다.
                string strDesignName = formNewDesign.m_strDesignName;
                string strSTEPFileFullName = formNewDesign.m_strSTEPFileFullName;

                // 빈칸 확인으로 null 비교를 사용하지 말라. 
                // - .Length == 0 나 "" 를 사용하라
                if (strDesignName.Length == 0)
                {
                    CNotice.printTraceID("DNIN");
                    return;
                }

                #region ------------------------- 디렉토리 및 파일명 설정 ------------------------

                // 생성을 할 때는 기본 작업 디렉토리를 사용해서 Actuator 작업파일의 절대 경로를 지정하고,
                // 작업파일을 Open 할 때는 파일을 오픈하는 위치에서 작업 디렉토리를 얻어내어 다시 설정한다.
                // 왜냐하면, 만약 작업 디렉토리를 수정하는 경우 기존의 작업파일을 열 수 없기 때문이다.
                string strDesignDirName = Path.Combine(CSettingData.m_strCurrentWorkingDirPath, strDesignName);

                // 형상 디렉토리 
                string strShapeDirName = Path.Combine(strDesignDirName, "Shape");
                // 형상 디렉토리안의 만들어진다.
                string strShapeModelFileFullName = Path.Combine(strShapeDirName, strDesignName + ".step");

                string strGmshExeFileFullName = CSettingData.m_strGmshExeFileFullName;

                // CheckStep Script 는 형상 디렉토리에서 작업을 한다.
                string strRunScriptFileFullName = Path.Combine(strShapeDirName, strDesignName + ".geo");
                
                // Part Names 파일도 형상 디렉토리에서 존재 한다.
                string strPartNamesFileFullName = Path.Combine(strShapeDirName, strDesignName + ".txt");

                // Mesh 파일도 형상 디렉토리에서 작업을 한다.
                string strMeshFileFullName = Path.Combine(strShapeDirName, strDesignName + ".msh");

                #endregion


                #region ------------------------- 디렉토리 생성 및 파일 복사------------------------

                // 다지인 디렉토리를 생성한다.
                m_manageFile.createDirectory(strDesignDirName);
                // 형상 디렉토리도 같이 생성한다.
                m_manageFile.createDirectory(strShapeDirName);

                // Maxwell Shape 파일을 현 디자인에 복사한다.
                m_manageFile.copyFile(strSTEPFileFullName, strShapeModelFileFullName);

                #endregion --------------------------------------------------------------------------


                CWriteFile writeFile = new CWriteFile();
                CReadFile readFile = new CReadFile();

                CScriptContents scriptContents = new CScriptContents();

                string strOrgStriptContents = scriptContents.m_str01_CheckSTEP_Script;

                // 1. 복사한 STEP 파일명과 파트명 저장 파일명을 저장해 둔다
                listScriptString.Add(strShapeModelFileFullName);
                listScriptString.Add(strPartNamesFileFullName);
                listScriptString.Add(strDesignName);

                // Script 파일이 문제없이 만들어지면 아래 동작을 실시하다.
                if (true == writeFile.createScriptFileUsingString(strOrgStriptContents, strRunScriptFileFullName, listScriptString))
                {
                    // Process 의 Arguments 에서 스페이스 문제가 발생한다.
                    // 아래와 같이 묶음처리를 사용한다.
                    string strArguments = " " + m_manageFile.solveDirectoryNameInPC(strRunScriptFileFullName);
       
                    if (false == m_manageFile.isExistFile(strShapeModelFileFullName))
                    {
                        CNotice.printTrace("형상 파일을 찾지 못했다.");
                        return;
                    }

                    // Gmsh 를 종료할 때까지 기다리지 않는다.
                    // 목적은 사용자들에게 Gmsh 에서 액추에이터의 형상 정보를 보게하면서 동시에 Part 이름 목록을 같이 보게 하기 위함이다.
                    CScript.runScript(strGmshExeFileFullName, strArguments, false);

                    while(false == m_manageFile.isExistFile(strPartNamesFileFullName))
                    {
                        // Gmsh 의 Script 를 실행해서 Part 명이 생성될 때 까지 기다린다.
                        Thread.Sleep(500);
                    }


                    // 순서 주의
                    //  - closeDesign() 뒤에 호출되어야 한다.
                    //
                    // m_design 에 이름과 경로를 저장해 둔다.
                    m_design.m_strDesignName = strDesignName;

                    // 생성할 경우 Design Directory 는 CSettingData.m_strWorkingDirName + strActuatorDesignName 로
                    // 무조건 프로그램 작업디렉토리에 생성되도록 하고 있다. 
                    m_design.m_strDesignDirPath = strDesignDirName;

                    if( true == m_manageFile.isExistFile(strPartNamesFileFullName) )
                    {
                        readFile.readCSVColumnString2(strPartNamesFileFullName, ref listTempPartNames, 1);

                        string[] arraySplitPartNames;
                        string strTempPartName;
                        string strFindRet = string.Empty;

                        foreach (string strPartName in listTempPartNames)
                        {
                            // Group 처리가 되어 있는 Step 파일의 이름은 kt100g/Yoke Cover/Yoke Cover 로 그룹까지 포함되어 있다.
                            // 여기서 '/' 로 분리해서 가장 하위의 명칭을 파트명으로 사용한다.
                            arraySplitPartNames = strPartName.Split('/');

                            // 가장 뒤에 있는 이름을 사용한다
                            strTempPartName = arraySplitPartNames[arraySplitPartNames.Length - 1];

                            // 이름에 스페이스가 있으면 '_' 로 변경한다.
                            strTempPartName = strTempPartName.Replace(' ', '_');

                            strFindRet = listAllPartNames.Find(x => x.Equals(strTempPartName));

                            if (null != strFindRet)
                            {
                                
                                if (CSettingData.m_emLanguage == EMLanguage.Korean)
                                    CNotice.noticeWarning(strTempPartName + "의 파트명에 중복 사용되고 있습니다.");
                                else
                                    CNotice.noticeWarning("It is used in duplicate with the part name called " + strTempPartName + ".");

                                // 취소되면 디자인 디렉토리를 삭제한다.
                                m_manageFile.deleteDirectory(strDesignDirName);
                                return;
                            }

                            // [주의 사항]
                            //
                            // 하나의 문자열을 사용해서 m_design 에서 복사하면 m_design 안의 두개의 List 가 같이 동작하기 때문에
                            // 두개의 문자열이 별도로 Add 해서 복사를 분리해서 진행한다.
                            listRemainedPartNames.Add(strTempPartName);
                            listAllPartNames.Add(strTempPartName);
                        }

                        m_design.RemainedShapeNameList = listRemainedPartNames;
                        m_design.AllShapeNameList = listAllPartNames;


                        // 정상적으로 스크립트 파일이 동작했다면 스크립트파일은 삭제한다.
                        //m_manageFile.deleteFile(strRunScriptFileFullName);

                        // 단지 리스트를 확인하기 위한 창임을 구분하기 위해 SHOW_LIST 항목을 사용하고 있다.
                        PopupAddNode dlgFormNodeName = new PopupAddNode(EMKind.SHOW_LIST, m_design.RemainedShapeNameList);

                        if (DialogResult.Cancel == dlgFormNodeName.ShowDialog())
                        {
                            // 취소되면 디자인 디렉토리를 삭제한다.
                            m_manageFile.deleteDirectory(strDesignDirName);
                            return;
                        }
                            
                    }
                    else
                    {
                        CNotice.printTrace("Part Names 파일이 존재하지 않는다.");

                        // 생성된 Design 디렉토리를 내부파일과 같이 한꺼번에 삭제한다.
                        m_manageFile.deleteDirectory(strDesignDirName);

                        return;
                    }

                    m_design.calcShapeSize(strMeshFileFullName);
                    
                     // 초기 파일을 저장한다.
                    saveDesignFile();
                }
                else
                {
                    CNotice.printTrace("Part Names Script 파일 생성에 문제가 발생했습니다.");
                    return;
                }

                // 프로젝트가 시작 했음을 표시하기 위해서 TreeView 에 기본 가지를 추가한다.
                TreeNode treeNode = new TreeNode("Parts", (int)EMKind.PARTS, (int)EMKind.PARTS);
                treeViewMain.Nodes.Add(treeNode);

                treeNode = new TreeNode("Experiments", (int)EMKind.EXPERIMENTS, (int)EMKind.EXPERIMENTS);
                treeViewMain.Nodes.Add(treeNode);

                // 수정 되었음을 기록한다.
                m_design.m_bChanged = true;

                // 제목줄에 디자인명을 표시한다
                this.Text = "DoSA-3D - " + m_design.m_strDesignName;

                CNotice.printUserMessage(m_design.m_strDesignName + m_resManager.GetString("_DHBC1"));    
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
                return;
            }
        }

        private void ribbonButtonOpen_Click(object sender, EventArgs e)
        {
            if (m_design.m_bChanged == true)
            {
                if (DialogResult.OK == CNotice.noticeWarningOKCancelID("DYWT", "W"))
                {
                    saveDesignFile();
                }
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();

            // 파일 열기창 설정
            openFileDialog.Title = "Open a Design File";
            // 디자인 파일을 열 때 디렉토리는 프로그램 작업 디렉토리로 하고 있다.
            openFileDialog.InitialDirectory = CSettingData.m_strCurrentWorkingDirPath;
            openFileDialog.FileName = null;
            openFileDialog.Filter = "Toolkit Files (*.dsa)|*.dsa|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                // 기존 디자인 데이터를 모두 삭제한다.
                closeDesign();

                string strActuatorDesignFileFullName = openFileDialog.FileName;

                loadDesignFile(strActuatorDesignFileFullName);

                // 디자인 파일이 생성될 때의 디자인 작업 디렉토리는 프로그램 기본 디렉토리 강제 설정하고 있다.
                // 만약 디렉토리를 옮긴 디자인 디렉토리를 오픈 할 경우라면 
                // 이전 다지인 작업 디렉토리를 그대로 사용하면 디렉토리 문제가 발생하여 실행이 불가능하게 된다.
                // 이를 해결하기 위해
                // 작업파일을 Open 할 때는 파일을 오픈하는 위치로 작업파일의 디렉토리를 다시 설정하고 있다.
                m_design.m_strDesignDirPath = Path.GetDirectoryName(strActuatorDesignFileFullName);

                // Design 디렉토리에서 Design 명을 제거한 디렉토리를 작업디렉토리로 설정한다.
                CSettingData.m_strCurrentWorkingDirPath = Path.GetDirectoryName(m_design.m_strDesignDirPath);

                // 프로젝트가 시작 했음을 표시하기 위해서 TreeView 에 기본 가지를 추가한다.
                TreeNode treeNode = new TreeNode("Parts", (int)EMKind.PARTS, (int)EMKind.PARTS);
                treeViewMain.Nodes.Add(treeNode);

                treeNode = new TreeNode("Experiments", (int)EMKind.EXPERIMENTS, (int)EMKind.EXPERIMENTS);
                treeViewMain.Nodes.Add(treeNode);

                foreach (CNode node in m_design.NodeList)
                {
                    this.addTreeNode(node.NodeName, node.m_kindKey);
                }
            }
            else
                return;

            //openFEMM();

            // 제목줄에 디자인명을 표시한다
            this.Text = "DoSA-3D - " + m_design.m_strDesignName;

            CNotice.printUserMessage(m_design.m_strDesignName + m_resManager.GetString("_DHBO"));    
        }

        private void ribbonOrbMenuItemClose_Click(object sender, EventArgs e)
        {
            if (m_design.m_bChanged == true)
            {
                if (DialogResult.OK == CNotice.noticeWarningOKCancelID("DYWT1", "W"))
                {
                    saveDesignFile();
                }
            }

            // 저장을 하고 나면 초기화 한다.
            m_design.m_bChanged = false;

            CNotice.printUserMessage(m_design.m_strDesignName + m_resManager.GetString("_DHBC"));

            // 기존 디자인 데이터를 모두 삭제한다.
            closeDesign();

            // 제목줄에 디자인명을 삭제한다
            this.Text = "DoSA-3D";

            //quitFEMM();
        }

        private void ribbonButtonSave_Click(object sender, EventArgs e)
        {
            if (m_design.m_strDesignName.Length == 0)
            {
                CNotice.noticeWarningID("TIND2");
                return;
            }

            if (true == saveDesignFile())
            {
                CNotice.noticeInfomation(m_design.m_strDesignName + m_resManager.GetString("_DSHB1"), m_resManager.GetString("SN"));

                CNotice.printUserMessage(m_design.m_strDesignName + m_resManager.GetString("_DSHB"));
            }
        }

        private void ribbonButtonSaveAs_Click(object sender, EventArgs e)
        {
            string strOrgDesignDirName, strSaveAsDesignDirName;
            string strSaveAsDesignName;

            CWriteFile writeFile = new CWriteFile();

            string strOrgDesignName = this.m_design.m_strDesignName;
            strOrgDesignDirName = this.m_design.m_strDesignDirPath;

            // 디자인이 없는 경우는 DesignName 없기 때문에 이름으로 작업디자인이 있는지를 판단한다.
            if (strOrgDesignName.Length == 0)
            {
                CNotice.noticeWarningID("TIND1");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Title = "Write a New Design Name";
            saveFileDialog.InitialDirectory = CSettingData.m_strCurrentWorkingDirPath;
            saveFileDialog.FileName = strOrgDesignName + "_Modify";

            DialogResult result = saveFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                try
                {
                    // 확장자가 제외된 전체 경로명이 넘어온다.
                    // TofU 는 선택된 Design 명으로 디렉토리를 만들기 때문에 전체 확장자를 제외한 전체 경로명이 SaveAs 디자인 경로명이 된다.
                    strSaveAsDesignDirName = saveFileDialog.FileName;

                    // 신규 디자인 디렉토리가 들어갈 상위 디렉토리를 찾는다.
                    string strSaveAsUpperDirName = Path.GetDirectoryName(strSaveAsDesignDirName);

                    // 확장자가 제외된 디자인 네임을 얻어온다. (전체 경로의 마지막 명칭이다.)
                    strSaveAsDesignName = Path.GetFileName(strSaveAsDesignDirName);

                    // SaveAs 할 디렉토리에 새로운 디렉토리와 겹치는 디렉토리가 있는지를 확인하기 위해서 디렉토리들을 읽어낸다.
                    List<string> listDirectories = m_manageFile.getDirectoryList(strSaveAsUpperDirName);


                    //============================================================================
                    // 2020-06-11
                    // 새로운 이름의 저장이 지정파일로 저장되는 것이 아니라 디렉토리를 만들면서 저장이 되기 때문에
                    // SaveAs 창에서 이름을 지정할 때 동일 디렉토리가 존재하면 디렉토리안으로 들어가서 그안에서 지정이름의 디렉토리와 파일로 저장된다.
                    // 따라서 아래의 코드는 동작하지 못하게 된다.
                    //============================================================================
                    if(listDirectories.Contains(strSaveAsDesignDirName) == true)
                    {
                        CNotice.noticeWarningID("YHCA");
                        return;
                    }

                    String strOrgDesingFileFullName = Path.Combine(strOrgDesignDirName, strOrgDesignName + ".dsa");
                    String strSaveAsDesignFileFullName = Path.Combine(strSaveAsDesignDirName, strSaveAsDesignName + ".dsa");

                    String strOrgShapeDirName = Path.Combine(strOrgDesignDirName, "Shape");
                    String strSaveAsShapeDirName = Path.Combine(strSaveAsDesignDirName, "Shape");

                    String strOrgMeshFileFullName = Path.Combine(strSaveAsShapeDirName, strOrgDesignName + ".msh");
                    String strOrgStepFileFullName = Path.Combine(strSaveAsShapeDirName, strOrgDesignName + ".step");
                    String strOrgPartNameFileFullName = Path.Combine(strSaveAsShapeDirName, strOrgDesignName + ".txt");

                    String strSaveAsMeshFileFullName = Path.Combine(strSaveAsShapeDirName, strSaveAsDesignName + ".msh");
                    String strSaveAsStepFileFullName = Path.Combine(strSaveAsShapeDirName, strSaveAsDesignName + ".step");
                    String strSaveAsPartNameFileFullName = Path.Combine(strSaveAsShapeDirName, strSaveAsDesignName + ".txt");

                    
                    #region // --------------- 파일과 디렉토리 복사 ---------------------

                    // SaveAs 디자인 디렉토리 생성
                    m_manageFile.createDirectory(strSaveAsDesignDirName);

                    // 디자인 파일 복사
                    m_manageFile.copyFile(strOrgDesingFileFullName, strSaveAsDesignFileFullName);

                    // 형상 디렉토리 복사
                    m_manageFile.copyDirectory(strOrgShapeDirName, strSaveAsShapeDirName);

                    // 형상 파일들의 이름을 변경한다.
                    //
                    // 함수가 없어서 파일을 다른이름으로 복사하고, 기존파일을 삭제하는 방식을 사용했다.
                    // ( 추후에 m_manageFile 에 changeName() 를 추가하라 )
                    m_manageFile.copyFile(strOrgMeshFileFullName, strSaveAsMeshFileFullName);
                    m_manageFile.copyFile(strOrgStepFileFullName, strSaveAsStepFileFullName);
                    m_manageFile.copyFile(strOrgPartNameFileFullName, strSaveAsPartNameFileFullName);
                    m_manageFile.deleteFile(strOrgMeshFileFullName);
                    m_manageFile.deleteFile(strOrgStepFileFullName);
                    m_manageFile.deleteFile(strOrgPartNameFileFullName);
                    
                    #endregion                    


                    // 현 모델을 SaveAs 모델명으로 변경한다.
                    m_design.m_strDesignDirPath = strSaveAsDesignDirName;
                    m_design.m_strDesignName = strSaveAsDesignName;

                    // 수정모델을 읽어드린 후에 바로 저장한다.
                    saveDesignFile();

                    // 화면을 갱신한다.
                    splitContainerRight.Panel1.Controls.Clear();
                    splitContainerRight.Panel1.Controls.Add(this.panelEmpty);

                    // PropertyGrid 창을 초기화 한다.
                    propertyGridMain.SelectedObject = null;

                    // 제목줄에 디자인명을 변경한다
                    this.Text = "DoSA-3D - " + m_design.m_strDesignName;

                    CNotice.noticeInfomation(m_design.m_strDesignName + m_resManager.GetString("_DHBS1"), m_resManager.GetString("SAN"));

                    CNotice.printUserMessage(m_design.m_strDesignName + m_resManager.GetString("_DHBS"));
                }
                catch (Exception ex)
                {
                    CNotice.printTrace(ex.Message);
                    return;
                }
            }
        }

        private void ribbonOrbMenuItemExit_Click(object sender, EventArgs e)
        {
            if (m_design.m_bChanged == true)
            {
                if (DialogResult.OK == CNotice.noticeWarningOKCancelID("DYWT", "W"))
                {
                    saveDesignFile();
                }
            }

            System.Windows.Forms.Application.Exit();
        }

        private void ribbonButtonCoil_Click(object sender, EventArgs e)
        {
            addRawNode(EMKind.COIL);
        }

        private void ribbonButtonMagnet_Click(object sender, EventArgs e)
        {
            addRawNode(EMKind.MAGNET);
        }
        
        private void ribbonButtonSteel_Click(object sender, EventArgs e)
        {
            addRawNode(EMKind.STEEL);
        }

        private void ribbonButtonForce_Click(object sender, EventArgs e)
        {
            addRawNode(EMKind.FORCE_EXPERIMENT);
        }
        
        private void ribbonButtonSetting_Click(object sender, EventArgs e)
        {
            PopupSetting frmSetting = new PopupSetting();

            frmSetting.uploadSettingData();

            if (DialogResult.OK == frmSetting.ShowDialog())
            {
                frmSetting.saveSettingToFile();

                // 언어를 수정과 동시에 반영한다.
                CSettingData.updataLanguge();
            }
        }

        private void ribbonButtonHelp_Click(object sender, EventArgs e)
        {
            PopupHelp frmHelp = new PopupHelp();
            frmHelp.StartPosition = FormStartPosition.CenterParent;

            frmHelp.ShowDialog();
        }

        private void ribbonButtonHomepage_Click(object sender, EventArgs e)
        {
            string target;

            if (CSettingData.m_emLanguage == EMLanguage.Korean)
            {
                target = "https://solenoid.or.kr/index_kor.html";
            }
            else
            {
                target = "https://solenoid.or.kr/index_eng.html";
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

        private void ribbonButtonAbout_Click(object sender, EventArgs e)
        {
            PopupAboutBox frmAbout = new PopupAboutBox();

            frmAbout.ShowDialog();
        }


        #endregion

        #region----------------------- Button -------------------------------

        private void buttonMagnetUp_Click(object sender, EventArgs e)
        {
            CNode node = (CNode)propertyGridMain.SelectedObject;

            if ("CMagnet" != node.GetType().Name)
            {
                Trace.WriteLine("Type mismatch in the FormMain:buttonMagnetUp_Click");
                return;
            }

            ((CMagnet)node).MagnetAngle = 90.0;

            propertyGridMain.Refresh();
        }

        private void buttonMagnetDown_Click(object sender, EventArgs e)
        {
            CNode node = (CNode)propertyGridMain.SelectedObject;

            if ("CMagnet" != node.GetType().Name)
            {
                Trace.WriteLine("Type mismatch in the FormMain:buttonMagnetDown_Click");
                return;
            }

            ((CMagnet)node).MagnetAngle = 270.0;

            propertyGridMain.Refresh();
        }

        private void buttonMagnetLeft_Click(object sender, EventArgs e)
        {
            CNode node = (CNode)propertyGridMain.SelectedObject;

            if ("CMagnet" != node.GetType().Name)
            {
                Trace.WriteLine("Type mismatch in the FormMain:buttonMagnetLeft_Click");
                return;
            }

            ((CMagnet)node).MagnetAngle = 180.0;

            propertyGridMain.Refresh();
        }

        private void buttonMagnetRight_Click(object sender, EventArgs e)
        {
            CNode node = (CNode)propertyGridMain.SelectedObject;

            if ("CMagnet" != node.GetType().Name)
            {
                Trace.WriteLine("Type mismatch in the FormMain:buttonMagnetRight_Click");
                return;
            }

            ((CMagnet)node).MagnetAngle = 0.0;

            propertyGridMain.Refresh();
        }

        private void buttonDesignCoil_Click(object sender, EventArgs e)
        {
            ((CCoil)propertyGridMain.SelectedObject).designCoil();

            propertyGridMain.Refresh();
        }

        private void buttonForceResult_Click(object sender, EventArgs e)
        {
            CForceExperiment forceExperiment = (CForceExperiment)propertyGridMain.SelectedObject;

            plotForceResult(forceExperiment);
        }
                
        private void buttonExperimentForce_Click(object sender, EventArgs e)
        {
            CForceExperiment forceExperiment = (CForceExperiment)propertyGridMain.SelectedObject;

            // 현재 시험의 이름을 m_nodeList 에서 찾지 않고
            // 현재 표시되고 있는 PropertyGird 창에서 Experiment 이름을 찾아 낸다
            string strExperimentName = forceExperiment.NodeName;
            string strExperimentDirName = Path.Combine(m_design.m_strDesignDirPath, strExperimentName);

            string strImageScriptFileFullName = Path.Combine(strExperimentDirName, "Image.geo");
            string strMagneticDensityVectorFileFullName = Path.Combine(strExperimentDirName, "b_cut.pos");

            // 해석 전에 전처리 조건을 확인한다.
            if (false == isForceExperimentOK(forceExperiment))
                return;

            // 이전에 해석결과가 존재하면 (디렉토리가 있으면) 삭제하고 시작한다.
            if (m_manageFile.isExistDirectory(strExperimentDirName) == true)
            {
                DialogResult ret = CNotice.noticeWarningOKCancelID("TIAP", "NE");

                if (ret == DialogResult.Cancel)
                    return;

                m_manageFile.deleteDirectory(strExperimentDirName);

                // 삭제되는 시간이 필요한 듯 한다.
                Thread.Sleep(1000);

                /// VCM Type 으로 해석이 되어 자기력 정확도 개선용으로 사용되는 전류가 0인 시험이 있다면 같이 삭제한다.
                string strExperimentZeroDirName = strExperimentDirName + "_Zero";

                if (m_manageFile.isExistDirectory(strExperimentZeroDirName ) == true)
                    m_manageFile.deleteDirectory(strExperimentZeroDirName);
                
                // 삭제되는 시간이 필요한 듯 한다.
                Thread.Sleep(1000);
            }

            // 시험 디렉토리를 생성한다.
            m_manageFile.createDirectory(strExperimentDirName);

            // 해석전 현 설정을 저장한다.
            saveDesignFile();

            solveForce(forceExperiment);

            // 해석 결과 이미지가 있다면 후처리를 진행한다.
            if (m_manageFile.isExistFile(strMagneticDensityVectorFileFullName) == true)
            {
                /// 영구자석이 포함된 VCM 방식인 경우는 자기력의 정확도가 크게 떨어진다.
                /// 정확도를 높이는 방안으로 전류가 인가되었을 때와 인가되지 않았을 때의 자기력차로 자기력을 표현한다.
                if (forceExperiment.ActuatorType == EMActuatorType.VCM)
                {
                    /// 얕은 복사가 되지 않고 깊은 복사가 되도록 Clone() 를 정의하고 사용했다.
                    CForceExperiment forceExperimentZeroCurrent = forceExperiment.Clone();

                    forceExperimentZeroCurrent.NodeName = forceExperiment.NodeName + "_Zero";
                    // 해석에 사용되는 전류값은 전압과 저항으로 다시 계산되기 때문에 전류 값을 0으로 하지않고 전압 값을 0으로 설정한다.
                    forceExperimentZeroCurrent.Voltage = 0.0f;

                    solveForce(forceExperimentZeroCurrent);
                }

                string strGmshExeFileFullName = CSettingData.m_strGmshExeFileFullName;
                string strArguments;

                // Process 의 Arguments 에서 스페이스 문제가 발생한다.
                // 아래와 같이 묶음처리를 사용한다.
                strArguments = " " + m_manageFile.solveDirectoryNameInPC(strMagneticDensityVectorFileFullName) 
                                   + " " + m_manageFile.solveDirectoryNameInPC(strImageScriptFileFullName);

                CScript.runScript(strGmshExeFileFullName, strArguments, true);

                //CScript.moveGmshWindow(0, 0);

                // Maxwell 의 종료시간을 기다려준다.
                Thread.Sleep(500);

                plotForceResult(forceExperiment);

                // Result 버튼이 동작하게 한다.
                buttonLoadForceResult.Enabled = true;

                //m_manageFile.deleteFile(strImageScriptFileFullName);
            }

        }
         
        #endregion

        #region------------------------- Script 작업 함수 ---------------------------

        public bool solveForce(CForceExperiment forceExperiment)
        {
            string strExperimentName = forceExperiment.NodeName;
            string strExperimentDirName = Path.Combine(m_design.m_strDesignDirPath, strExperimentName);

            string strSolveScriptFileFullName = Path.Combine(strExperimentDirName, strExperimentName + ".pro");

            string strGmshExeFileFullName = CSettingData.m_strGmshExeFileFullName;

            createDefineGeoFile(forceExperiment);

            createBHProFile(forceExperiment);

            if (false == createDesignGeoFile(forceExperiment))
                return false;

            createImageGeoFile(forceExperiment);

            createDesignProFile(forceExperiment);

            addFuncitonToDesignProFile(forceExperiment);

            addFormulationToDesignProFile(forceExperiment);

            addPostToDesignProFile(forceExperiment);

            // Process 의 Arguments 에서 스페이스 문제가 발생한다.
            // 아래와 같이 묶음처리를 사용한다.
            string strArguments = " " + m_manageFile.solveDirectoryNameInPC(strSolveScriptFileFullName);

            // Maxwell 종료될 때 가지 툴킷을 기다린다.
            // Script 삭제에 사용하는 파일이름은 묶음 처리가 되어서는 안된다.
            CScript.runScript(strGmshExeFileFullName, strArguments, true);

            // Maxwell 의 종료시간을 기다려준다.
            Thread.Sleep(500);

            return true;
        }

        private void addPostToDesignProFile(CForceExperiment forceExperiment)
        {
            CScriptContents scriptContents = new CScriptContents();

            CWriteFile writeFile = new CWriteFile();
            List<string> listScriptString = new List<string>();

            string strOrgStriptContents;

            string strExperimentName = forceExperiment.NodeName;
            string strExperimentDirName = Path.Combine(m_design.m_strDesignDirPath, strExperimentName);

            string strSolveScriptFileFullName = Path.Combine(strExperimentDirName, strExperimentName + ".pro");

            int nCount = 0;

            try
            {

                foreach (CNode node in m_design.NodeList)
                {
                    if (node.m_kindKey == EMKind.COIL)
                    {
                        if (nCount != 0)
                        {
                            CNotice.printTrace("코일 수가 하나 이상이다.");
                            return;
                        }

                        listScriptString.Add(node.NodeName);

                        nCount++;
                    }
                }
                
                strOrgStriptContents = scriptContents.m_str12_PostProcessing_Script;
                    
                writeFile.addScriptFileUsingString(strOrgStriptContents, strSolveScriptFileFullName, listScriptString);

                // 아래의 m_str13_PostOperation_Script 에서도 coil name 을 사용하기 때문에 이름을 그대로 유지한다.
                //listScriptString.Clear();

                strOrgStriptContents = scriptContents.m_str13_PostOperation_Script;

                List<double> listProductLength = new List<double>();
                
                // [주의 사항]
                //
                // 형상 작업의 값과 일치해야 한다.
                int iOuterPaddingPercent = 150;

                double dProductLengthX = Math.Abs(m_design.MaxX - m_design.MinX);
                double dProductLengthY = Math.Abs(m_design.MaxY - m_design.MinY);
                double dProductLengthZ = Math.Abs(m_design.MaxZ - m_design.MinZ);

                listProductLength.Add(dProductLengthX);
                listProductLength.Add(dProductLengthY);
                listProductLength.Add(dProductLengthZ);

                double dAverageProductLength = listProductLength.Average();

                // X,Y,Z 의 제품 폭 중에 최대 폭으로 모든 방향의 Region 크기를 결정한다.
                double dOuterPaddingLength = dAverageProductLength * (iOuterPaddingPercent / 100.0f);

                // 중심 위치
                double dRegionCenterX = (m_design.MinX + m_design.MaxX) / 2.0f;
                double dRegionCenterY = (m_design.MinY + m_design.MaxY) / 2.0f;
                double dRegionCenterZ = (m_design.MinZ + m_design.MaxZ) / 2.0f;

                // 전체 영역의 1/2 로 자속밀도 단면 벡터출력면을 사용한다.
                //# 1 : X Coord of Left Bottom Point on XY Plane 
                //# 2 : Y Coord of Left Bottom Point on XY Plane 
                //# 3 : X Coord of Right Bottom Point on XY Plane 
                //# 4 : Y Coord of Left Top Point on XY Plane 
                //# 5 : Z Coord of Center Point on XY Plane 
                double dSectionBMinX = dRegionCenterX - dAverageProductLength / 2.0f - dOuterPaddingLength;
                double dSectionBMinY = dRegionCenterY - dAverageProductLength / 2.0f - dOuterPaddingLength;
                double dSectionBMaxX = dRegionCenterX + dAverageProductLength / 2.0f + dOuterPaddingLength;
                double dSectionBMaxY = dRegionCenterY + dAverageProductLength / 2.0f + dOuterPaddingLength;

                listScriptString.Add(dSectionBMinX.ToString());
                listScriptString.Add(dSectionBMinY.ToString());
                listScriptString.Add(dSectionBMaxX.ToString());
                listScriptString.Add(dSectionBMaxY.ToString());
                listScriptString.Add(dRegionCenterZ.ToString());


                writeFile.addScriptFileUsingString(strOrgStriptContents, strSolveScriptFileFullName, listScriptString);
                listScriptString.Clear();

            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
            }
        }

        private void addFormulationToDesignProFile(CForceExperiment forceExperiment)
        {
            CScriptContents scriptContents = new CScriptContents();

            CWriteFile writeFile = new CWriteFile();
            List<string> listScriptString = new List<string>();

            string strOrgStriptContents;

            string strExperimentName = forceExperiment.NodeName;
            string strExperimentDirName = Path.Combine(m_design.m_strDesignDirPath, strExperimentName);

            string strSolveScriptFileFullName = Path.Combine(strExperimentDirName, strExperimentName + ".pro");


            try
            {
                strOrgStriptContents = scriptContents.m_str11_Formulation_Resolution_Script;

                int nCount = 0;
                bool bUsedMagnet = false;

                foreach (CNode node in m_design.NodeList)
                {
                    if (node.m_kindKey == EMKind.COIL)
                    {
                        if (nCount != 0)
                        {
                            CNotice.printTrace("코일 수가 하나 이상이다.");
                            return;
                        }

                        listScriptString.Add(node.NodeName);

                        nCount++;
                    }

                    if (node.m_kindKey == EMKind.MAGNET)
                        bUsedMagnet = true;
                }

                if (bUsedMagnet == true)
                {
                    listScriptString.Add("            Galerkin { [ nu[] * br[] , {d qnt_A} ] ;\n");
                    listScriptString.Add("                In domainMagnet ; Jacobian jbVolume ; Integration igElement ; }\n");
                }
                else
                {
                    // 영구자석이 없는 경우도 파라메터는 설정해 주어야 한다.
                    listScriptString.Add(" ");
                    listScriptString.Add(" ");
                }

                writeFile.addScriptFileUsingString(strOrgStriptContents, strSolveScriptFileFullName, listScriptString);
                listScriptString.Clear();
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
            }
        }

        private void addFuncitonToDesignProFile(CForceExperiment forceExperiment)
        {
            CScriptContents scriptContents = new CScriptContents();

            CWriteFile writeFile = new CWriteFile();
            List<string> listScriptString = new List<string>();

            string strOrgStriptContents;

            string strExperimentName = forceExperiment.NodeName;
            string strExperimentDirName = Path.Combine(m_design.m_strDesignDirPath, strExperimentName);

            string strSolveScriptFileFullName = Path.Combine(strExperimentDirName, strExperimentName + ".pro");

            string strNodeName;
            string strTemp = string.Empty;

            try
            {
                strOrgStriptContents = scriptContents.m_str07_Function_Script;

                double dCurrent, dCoilSectionArea, dCoilWidth;
                double dHc, dBr, dMur;

                double dHcX, dHcY, dHcZ;
                double dBrX, dBrY, dBrZ;
                EMMagnetPlane emMagnetPlane;
                double dAngle;

                foreach (CNode node in m_design.NodeList)
                {
                    strNodeName = node.NodeName;

                    switch (node.m_kindKey)
                    {
                        case EMKind.COIL:

                            strOrgStriptContents += String.Format("    mu[vol{0}] = mu0;\n", strNodeName);
                            strOrgStriptContents += String.Format("    nu[vol{0}] = 1.0/mu0;\n\n", strNodeName, strTemp);

                            dCurrent = forceExperiment.Voltage / ((CCoil)node).Resistance;

                            strOrgStriptContents += "    current = " + dCurrent.ToString() + ";\n";
                            strOrgStriptContents += "    coilTurns = " + ((CCoil)node).Turns.ToString() + ";\n\n";

                            strOrgStriptContents += "    coilTurns[] = coilTurns;\n\n";

                            dCoilWidth = (((CCoil)node).OuterDiameter - ((CCoil)node).InnerDiameter) / 2.0f;

                            dCoilSectionArea = (dCoilWidth * ((CCoil)node).Height) / 1000000.0f;

                            strOrgStriptContents += "    areaCoilSection[] = " + dCoilSectionArea.ToString() + ";\n";

                            // 중심 위치
                            // * 0.001 은 meter -> mm 로 단위 변환
                            double dRegionCenterX = (m_design.MinX + m_design.MaxX) / 2.0f * 0.001;
                            //double dRegionCenterY = (m_design.MinY + m_design.MaxY) / 2.0f * 0.001;       // Y 축과 평행한다는 가정에서 사용하지 않는다
                            double dRegionCenterZ = (m_design.MinZ + m_design.MaxZ) / 2.0f * 0.001;

                            if (((CCoil)node).CurrentDirection == EMCurrentDirection.IN)
                                strOrgStriptContents += String.Format("    vectorCurrent[] = Vector[ Cos[Atan2[X[]-({0}), Z[]-({1})]], 0, -Sin[Atan2[X[]-({0}), Z[]-({1})]]];\n\n", dRegionCenterX, dRegionCenterZ);
                            else
                                strOrgStriptContents += String.Format("    vectorCurrent[] = - Vector[ Cos[Atan2[X[]-({0}), Z[]-({1})]], 0, -Sin[Atan2[X[]-({0}), Z[]-({1})]]];\n\n", dRegionCenterX, dRegionCenterZ);

                            strOrgStriptContents += "    js0[] = current * coilTurns[] /areaCoilSection[] * vectorCurrent[];\n\n";

                            break;

                        case EMKind.MAGNET:

                            dHc = ((CMagnet)node).Hc;
                            dBr = ((CMagnet)node).Br;
                            dMur = dBr / dHc;

                            emMagnetPlane = ((CMagnet)node).emMagnetPlane;
                            dAngle = ((CMagnet)node).MagnetAngle;

                            if (emMagnetPlane == EMMagnetPlane.XY_Plane_Z)
                            {
                                dHcX = dHc * Math.Cos(dAngle * Math.PI / 180.0f);
                                dHcY = dHc * Math.Sin(dAngle * Math.PI / 180.0f);
                                dHcZ = 0.0;
                                dBrX = dBr * Math.Cos(dAngle * Math.PI / 180.0f);
                                dBrY = dBr * Math.Sin(dAngle * Math.PI / 180.0f);
                                dBrZ = 0.0;
                            }
                            else if (emMagnetPlane == EMMagnetPlane.YZ_Plane_X)
                            {
                                dHcX = 0.0;
                                dHcY = dHc * Math.Cos(dAngle * Math.PI / 180.0f);
                                dHcZ = dHc * Math.Sin(dAngle * Math.PI / 180.0f);
                                dBrX = 0.0;
                                dBrY = dBr * Math.Cos(dAngle * Math.PI / 180.0f);
                                dBrZ = dBr * Math.Sin(dAngle * Math.PI / 180.0f);
                            }
                            else            // EMMagnetPlane.ZX_Plane
                            {
                                dHcX = dHc * -Math.Cos(dAngle * Math.PI / 180.0f);
                                dHcY = 0.0;
                                dHcZ = dHc * Math.Sin(dAngle * Math.PI / 180.0f);
                                dBrX = dBr * -Math.Cos(dAngle * Math.PI / 180.0f);
                                dBrY = 0.0;
                                dBrZ = dBr * Math.Sin(dAngle * Math.PI / 180.0f);
                            }

                            // 입력 자계의 세기는 양의 값이지만 실제 동작은 음에서 동작하기 때문에 '-' 을 사용해서 부호를 바꾸고 있다.
                            //
                            strOrgStriptContents += String.Format("    hc[vol{0}] = Vector[{1}, {2}, {3}];\n", strNodeName, (-dHcX).ToString(), (-dHcY).ToString(), (-dHcZ).ToString());
                            strOrgStriptContents += String.Format("    br[vol{0}] = Vector[{1}, {2}, {3}];\n", strNodeName, (-dBrX).ToString(), (-dBrY).ToString(), (-dBrZ).ToString());

                            strOrgStriptContents += String.Format("    mu[vol{0}] = {1};\n", strNodeName, dMur.ToString());
                            strOrgStriptContents += String.Format("    nu[vol{0}] = 1.0 / {1};\n\n", strNodeName, dMur.ToString());

                            break;

                        case EMKind.STEEL:

                            strOrgStriptContents += String.Format("    nu[vol{0}] = nu_{1}[$1];\n", strNodeName, ((CSteel)node).Material);
                            strOrgStriptContents += String.Format("    dhdb_NL[vol{0}] = dhdb_{1}[$1];\n\n", strNodeName, ((CSteel)node).Material);

                            break;

                        default:

                            break;
                    }
                }

                strOrgStriptContents += "    TM[] = ( SquDyadicProduct[$1] - SquNorm[$1] * TensorDiag[0.5, 0.5, 0.5] ) / mu[];\n}\n";

                writeFile.addScriptFileUsingString(strOrgStriptContents, strSolveScriptFileFullName, listScriptString);
                listScriptString.Clear();

                strOrgStriptContents = scriptContents.m_str08_Constraint_Script;

                writeFile.addScriptFileUsingString(strOrgStriptContents, strSolveScriptFileFullName, listScriptString);
                listScriptString.Clear();
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
            }
        }

        private void createDesignProFile(CForceExperiment forceExperiment)
        {
            CScriptContents scriptContents = new CScriptContents();

            CWriteFile writeFile = new CWriteFile();
            List<string> listScriptString = new List<string>();

            string strOrgStriptContents;

            string strExperimentName = forceExperiment.NodeName;
            string strExperimentDirName = Path.Combine(m_design.m_strDesignDirPath, strExperimentName);

            string strSolveScriptFileFullName = Path.Combine(strExperimentDirName, strExperimentName + ".pro");

            try
            {
                strOrgStriptContents = scriptContents.m_str06_Group_Script;

                // Air 가 1이기 때문에 파트는 2 부터 시작한다.
                string strTemp = string.Empty;
                string strNodeName = string.Empty;
                string strNonlinearNames = string.Empty;
                string strLinearNames = "volAir, ";
                string strMagnetPartNames = string.Empty;

                int nIndex;

                foreach (CNode node in m_design.NodeList)
                {
                    strNodeName = node.NodeName;
                    strTemp = strNodeName.ToUpper();

                    switch (node.m_kindKey)
                    {
                        case EMKind.STEEL:
                            strNonlinearNames += String.Format("vol{0}, ", strNodeName);
                            break;

                        case EMKind.COIL:
                            strLinearNames += String.Format("vol{0}, ", strNodeName);
                            break;

                        case EMKind.MAGNET:
                            strLinearNames += String.Format("vol{0}, ", strNodeName);
                            strMagnetPartNames += String.Format("vol{0}, ", strNodeName);

                            break;

                        default:
                            break;
                    }

                    if (node.GetType().BaseType.Name == "CParts")
                        strOrgStriptContents += String.Format("    vol{0} = Region[{1}];\n", strNodeName, strTemp);
                }

                strOrgStriptContents += "\n";

                string strDomainAll = string.Empty;

                if (strNonlinearNames.Length > 2)
                {
                    // 마지막 ", " 를 제거한다.
                    nIndex = strNonlinearNames.Length - 2;
                    strNonlinearNames = strNonlinearNames.Remove(nIndex);

                    strOrgStriptContents += "    domainNL = Region[ {" + strNonlinearNames + "} ];\n";
                    strDomainAll += "domainNL";
                }

                if (strLinearNames.Length > 2)
                {
                    // 마지막 ", " 를 제거한다.
                    nIndex = strLinearNames.Length - 2;
                    strLinearNames = strLinearNames.Remove(nIndex);

                    strOrgStriptContents += "    domainL = Region[ {" + strLinearNames + "} ];\n";

                    if (strDomainAll.Length > 0)
                        strDomainAll += ", domainL";
                    else
                        strDomainAll += "domainL";
                }

                strOrgStriptContents += "    domainALL = Region[ {" + strDomainAll + "} ];\n\n";

                if (strMagnetPartNames.Length > 2)
                {
                    // 마지막 ", " 를 제거한다.
                    nIndex = strMagnetPartNames.Length - 2;
                    strMagnetPartNames = strMagnetPartNames.Remove(nIndex);

                    strOrgStriptContents += "    domainMagnet = Region[ {" + strMagnetPartNames + "} ];\n\n";

                }

                strOrgStriptContents += "}\n";

                writeFile.createScriptFileUsingString(strOrgStriptContents, strSolveScriptFileFullName, listScriptString);
                listScriptString.Clear();

            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
            }
        }

        private void createImageGeoFile(CForceExperiment forceExperiment)
        {
            CScriptContents scriptContents = new CScriptContents();

            CWriteFile writeFile = new CWriteFile();
            List<string> listScriptString = new List<string>();

            string strOrgStriptContents;

            string strExperimentName = forceExperiment.NodeName;
            string strExperimentDirName = Path.Combine(m_design.m_strDesignDirPath, strExperimentName);

            string strShapeDirName = Path.Combine(m_design.m_strDesignDirPath, "Shape");

            string strSTEPFileFullName = Path.Combine(strShapeDirName, m_design.m_strDesignName + ".step");
            string strImageScriptFileFullName = Path.Combine(strExperimentDirName, "Image.geo");

            try
            {

                strOrgStriptContents = scriptContents.m_str05_Image_Script;

                listScriptString.Add(strSTEPFileFullName);

                writeFile.createScriptFileUsingString(strOrgStriptContents, strImageScriptFileFullName, listScriptString);
                listScriptString.Clear();
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
            }
        }

        private bool createDesignGeoFile(CForceExperiment forceExperiment)
        {
            CScriptContents scriptContents = new CScriptContents();

            CWriteFile writeFile = new CWriteFile();
            List<string> listScriptString = new List<string>();

            string strOrgStriptContents;

            string strExperimentName = forceExperiment.NodeName;
            string strExperimentDirName = Path.Combine(m_design.m_strDesignDirPath, strExperimentName);

            string strShapeDirName = Path.Combine(m_design.m_strDesignDirPath, "Shape");
            string strMeshFileFullName = Path.Combine(strShapeDirName, m_design.m_strDesignName + ".msh");
            string strSTEPFileFullName = Path.Combine(strShapeDirName, m_design.m_strDesignName + ".step");

            string strGeometryScriptFileFullName = Path.Combine(strExperimentDirName, strExperimentName + ".geo");

            try
            {
                // STEP 파일을 시험 디렉토리로 복사한다.
                //m_manageFile.copyFile(strOrgSTEPFileFullName, strExprimentSTEPFileFullName);

                strOrgStriptContents = scriptContents.m_str04_Import_Script;

                listScriptString.Add(strSTEPFileFullName);

                writeFile.createScriptFileUsingString(strOrgStriptContents, strGeometryScriptFileFullName, listScriptString);
                listScriptString.Clear();

                strOrgStriptContents = string.Empty;
                int nDefineNumCount = 0;
                string strTemp;

                // STEP 에서 읽어낸 Volume 들의 인덱스와 이름이 일치해야 하기 때문에 
                // NoteList 에서 읽어내지 않고 Volume 이 순서대로 저장된 AllShapeNameList 를 사용해서 인덱스 순서대로 이름을 지정하고 있다.
                // ( 상기 저장순서를 사용해서 Script 간의 순서를 일치 시킴 )
                foreach (string strName in m_design.AllShapeNameList)
                {
                    strOrgStriptContents += String.Format("vol{0} = STEP_Volumes[{1}];\n", strName, nDefineNumCount);

                    nDefineNumCount++;
                }

                strOrgStriptContents += "\n";

                string strNodeName;
                string strMovingPartNames = string.Empty;
                string strSteelPartNames = string.Empty;

                foreach (CNode node in m_design.NodeList)
                {
                    strNodeName = node.NodeName;
                    strTemp = strNodeName.ToUpper();

                    if (node.GetType().BaseType.Name == "CParts")
                    {
                        if (((CParts)node).MovingPart == EMMoving.MOVING)
                        {
                            strMovingPartNames += String.Format("vol{0}, ", strNodeName);
                        }
                    }

                    if (node.m_kindKey == EMKind.STEEL)
                    {
                        strSteelPartNames += String.Format("vol{0}, ", strNodeName);
                    }
                }

                int nIndex = 0;

                if (strMovingPartNames.Length > 2)
                {
                    // 마지막 ", " 를 제거한다.
                    nIndex = strMovingPartNames.Length - 2;
                    strMovingPartNames = strMovingPartNames.Remove(nIndex);

// 작업 검토 중
//                    strOrgStriptContents += "volMovingParts = Volume{ " + strMovingPartNames + " };\n\n";

                    strOrgStriptContents += "Translate { " + forceExperiment.MovingX.ToString() + "*mm , " + forceExperiment.MovingY.ToString() + "*mm, " 
                                            + forceExperiment.MovingZ.ToString() + "*mm } {  Volume{" + strMovingPartNames + "}; }\n\n";

                    strOrgStriptContents += "skinMoving() = CombinedBoundary{ Volume{" + strMovingPartNames + "}; };\n";
                }

                if (strSteelPartNames.Length > 2)
                {
                    // 마지막 ", " 를 제거한다.
                    nIndex = strSteelPartNames.Length - 2;
                    strSteelPartNames = strSteelPartNames.Remove(nIndex);

                    strOrgStriptContents += "skinSteel() = CombinedBoundary{ Volume{" + strSteelPartNames + "}; };\n\n";
                }

                foreach (string strName in m_design.AllShapeNameList)
                {
                    strTemp = strName.ToUpper();

                    strOrgStriptContents += String.Format("Physical Volume({0}) = vol{1};\n", strTemp, strName);

                    nDefineNumCount++;
                }

                double dMeshSize;

                m_design.calcShapeSize(strMeshFileFullName);

                if (forceExperiment.MeshSizePercent <= 0 || forceExperiment.MeshSizePercent > 100)
                {
                    if (CSettingData.m_emLanguage == EMLanguage.Korean)
                        CNotice.noticeWarning("Mesh Size Percent 가 문제가 있습니다.");
                    else
                        CNotice.noticeWarning("Mesh Size Percent has a problem.");

                    return false;
                }

                // 볼륨을 길이단위로 바꾸기 위해서 1/3 승을 했다.
                // 사용하는 Mesh Size Percent 는 환경설정이 아니고 Force 페이지에 있는 값을 사용한다.
                dMeshSize = Math.Pow(m_design.ShapeVolumeSize, 1.0f / 3.0f) * forceExperiment.MeshSizePercent / 100.0f;

                // mm -> m 로 단위 변환 
                dMeshSize = dMeshSize / 1000.0f;

                listScriptString.Add(dMeshSize.ToString());

                double dOuterRegionMinX, dOuterRegionMinY, dOuterRegionMinZ;
                double dProductLengthX, dProductLengthY, dProductLengthZ;
                double dRegionCenterX, dRegionCenterY, dRegionCenterZ;
                List<double> listProductLength = new List<double>();

                /// 여기서 Padding Percent 란 
                /// 제품 외각에서 음과 양 방향으로 제품 폭의 몇 배를 더 붙이는 가의 의미가 있다.
                /// 따라서 Padding 이 150 란 
                /// 음의 방향으로 150 %, 양의 방향으로 150 % 그리고 제품 최대 폭이 더해져서 제품의 최대 폭대비 400% 의 외각 박스가 그려진다.
                /// Air 바깥의 조건은 A = 0 이다. 
                /// 따라서 자계가 밖을 나가는 VCM 이나 영구자석의 경우를 고려해서 150 이상을 사용한다.
                /// 
                /// [주의 사항] : 후처리 작업의 값과 일치해야 한다.
                /// 
                int iOuterPaddingPercent = 150;

                dProductLengthX = Math.Abs(m_design.MaxX - m_design.MinX);
                dProductLengthY = Math.Abs(m_design.MaxY - m_design.MinY);
                dProductLengthZ = Math.Abs(m_design.MaxZ - m_design.MinZ);

                listProductLength.Add(dProductLengthX);
                listProductLength.Add(dProductLengthY);
                listProductLength.Add(dProductLengthZ);

                // 평균을 사용한다.
                double dAverageProductLength = listProductLength.Average();

                // X,Y,Z 의 제품 폭 중에 최대 폭으로 모든 방향의 Region 크기를 결정한다.
                double dOuterPaddingLength = dAverageProductLength * (iOuterPaddingPercent / 100.0f);

                // 중심 위치
                dRegionCenterX = (m_design.MinX + m_design.MaxX) / 2.0f;
                dRegionCenterY = (m_design.MinY + m_design.MaxY) / 2.0f;
                dRegionCenterZ = (m_design.MinZ + m_design.MaxZ) / 2.0f;

                /// 음의 방향의 좌표 값은 
                /// 중심 위치에서 먼저 dAverageProductLength / 2.0f 를 빼서 외각 위치를 얻고,
                /// 거기에 Padding Length 를 추가로 빼서 결정한다.
                dOuterRegionMinX = dRegionCenterX - dAverageProductLength / 2.0f - dOuterPaddingLength;
                dOuterRegionMinY = dRegionCenterY - dAverageProductLength / 2.0f - dOuterPaddingLength;
                dOuterRegionMinZ = dRegionCenterZ - dAverageProductLength / 2.0f - dOuterPaddingLength;

                listScriptString.Add(dOuterRegionMinX.ToString());
                listScriptString.Add(dOuterRegionMinY.ToString());
                listScriptString.Add(dOuterRegionMinZ.ToString());

                /// Outer Air Box 는 정사각형이기 때문에 X,Y,Z 방향 모두 같은 하나의 값만 사용하고 있다.
                /// 양쪽에 PaddingLength 와 제품 길이로 구성된다.
                double dOuterRegionLength = dAverageProductLength + dOuterPaddingLength * 2.0f;

                listScriptString.Add(dOuterRegionLength.ToString());

               
                double dInnerRegionMinX, dInnerRegionMinY, dInnerRegionMinZ;
                double dInnerPaddingLengthX, dInnerPaddingLengthY, dInnerPaddingLengthZ;
                double dInnerRegionLengthX, dInnerRegionLengthY, dInnerRegionLengthZ;

                /// Padding 이 20 란 
                /// 음의 방향으로 20 %, 양의 방향으로 20 % 그리고 제품 각방향 폭이 더해져서 제품의 폭대비 140% 의 외각 박스가 그려진다.
                int iInnerPaddingPercent = 20;

                dInnerPaddingLengthX = dProductLengthX * iInnerPaddingPercent / 100.0f;
                dInnerPaddingLengthY = dProductLengthY * iInnerPaddingPercent / 100.0f;
                dInnerPaddingLengthZ = dProductLengthZ * iInnerPaddingPercent / 100.0f;

                /// 음의 방향의 좌표 값은 
                /// 중심 위치에서 먼저 dProductLengthX / 2.0f 를 빼서 외각 위치를 얻고,
                /// 거기에 Padding Length 를 추가로 빼서 결정한다.
                dInnerRegionMinX = dRegionCenterX - dProductLengthX / 2.0f - dInnerPaddingLengthX;
                dInnerRegionMinY = dRegionCenterY - dProductLengthY / 2.0f - dInnerPaddingLengthY;
                dInnerRegionMinZ = dRegionCenterZ - dProductLengthZ / 2.0f - dInnerPaddingLengthZ;

                listScriptString.Add(dInnerRegionMinX.ToString());
                listScriptString.Add(dInnerRegionMinY.ToString());
                listScriptString.Add(dInnerRegionMinZ.ToString());

                dInnerRegionLengthX = dProductLengthX + dInnerPaddingLengthX * 2.0f;
                dInnerRegionLengthY = dProductLengthY + dInnerPaddingLengthY * 2.0f;
                dInnerRegionLengthZ = dProductLengthZ + dInnerPaddingLengthZ * 2.0f;
                
                listScriptString.Add(dInnerRegionLengthX.ToString());
                listScriptString.Add(dInnerRegionLengthY.ToString());
                listScriptString.Add(dInnerRegionLengthZ.ToString());
   
                strOrgStriptContents += scriptContents.m_str04_1_Region_Script;

                writeFile.addScriptFileUsingString(strOrgStriptContents, strGeometryScriptFileFullName, listScriptString);
                listScriptString.Clear();
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
                return false;
            }

            return true;
        }

        private void createBHProFile(CForceExperiment forceExperiment)
        {
            CScriptContents scriptContents = new CScriptContents();

            CWriteFile writeFile = new CWriteFile();
            List<string> listScriptString = new List<string>();

            string strExperimentName = forceExperiment.NodeName;
            string strExperimentDirName = Path.Combine(m_design.m_strDesignDirPath, strExperimentName);

            string strBHProFileFullName = Path.Combine(strExperimentDirName, "BH.pro");

            string strProgramMaterialDirName = Path.Combine(CSettingData.m_strProgramDirPath, "Materials");
            string strSteelMaterialFileFullName = Path.Combine(strProgramMaterialDirName, "DoSA_MS.dmat");

            string strOrgStriptContents;

            try
            {
                CReadFile readFile = new CReadFile();

                List<double> listH = new List<double>();
                List<double> listB = new List<double>();
                string strMaterialName;

                List<string> listWrittenSteelMaterialName = new List<string>();

                strOrgStriptContents = "Function{\n\n";

                writeFile.createScriptFileUsingString(strOrgStriptContents, strBHProFileFullName, listScriptString);
                listScriptString.Clear();

                foreach (CNode node in m_design.NodeList)
                {
                    switch (node.m_kindKey)
                    {
                        case EMKind.STEEL:

                            strMaterialName = ((CSteel)node).Material;
 
                            if (listWrittenSteelMaterialName.Contains(strMaterialName) == false)
                            {
                                // BH Data 기록하기
                                readFile.readMaterialBHData(strSteelMaterialFileFullName, strMaterialName, ref listH, ref listB);

                                scriptContents.getScriptBH(strMaterialName, ref strOrgStriptContents, listH, listB);

                                writeFile.addScriptFileUsingString(strOrgStriptContents, strBHProFileFullName, listScriptString);
                                listScriptString.Clear();

                                // BH 관련  수식 기록하기
                                strOrgStriptContents = scriptContents.m_str03_BH_Calulate_Script;

                                listScriptString.Add(strMaterialName);

                                writeFile.addScriptFileUsingString(strOrgStriptContents, strBHProFileFullName, listScriptString);
                                listScriptString.Clear();

                                // 중복 저장을 방지하기 위해서 List 에 재질명을 남겨 둔다.
                                listWrittenSteelMaterialName.Add(strMaterialName);
                            }

                            break;

                        default:
                            break;
                    }
                }

                strOrgStriptContents = "}\n";

                writeFile.addScriptFileUsingString(strOrgStriptContents, strBHProFileFullName, listScriptString);
                listScriptString.Clear();
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
            }

        }

        private void createDefineGeoFile(CForceExperiment forceExperiment)
        {
            CScriptContents scriptContents = new CScriptContents();

            CWriteFile writeFile = new CWriteFile();
            List<string> listScriptString = new List<string>();

            string strOrgStriptContents;

            string strExperimentName = forceExperiment.NodeName;
            string strExperimentDirName = Path.Combine(m_design.m_strDesignDirPath, strExperimentName);

            string strDefineGeoFileFullName = Path.Combine(strExperimentDirName, "Define.geo");

            try
            {
                strOrgStriptContents = scriptContents.m_str02_Define_Script;

                // 파트 번호는 1 부터 시작한다. (저장된 STEP 파일의 Volume 번호와 일치 시킨다)
                int nDefineNumCount = 1;
                string strTemp;

                // STEP 에서 읽어낸 Volume 들의 인덱스와 이름이 일치해야 하기 때문에 
                // NoteList 에서 읽어내지 않고 Volume 이 순서대로 저장된 AllShapeNameList 를 사용해서 인덱스 순서대로 이름을 지정하고 있다.
                // ( 상기 저장순서를 사용해서 Script 간의 순서를 일치 시킴 )
                foreach (string strName in m_design.AllShapeNameList)
                {
                    strTemp = strName.ToUpper();

                    strOrgStriptContents += String.Format("{0} = {1};\n", strTemp, nDefineNumCount);
                    nDefineNumCount++;
                }

                writeFile.createScriptFileUsingString(strOrgStriptContents, strDefineGeoFileFullName, listScriptString);
                listScriptString.Clear();
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
            }
        }

        #endregion        
      
        #region---------------------- Windows Message -----------------------

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 이름이 지정된 Design 만 저장을 확인한다.
            if (m_design.m_bChanged == true)
            {
                if (DialogResult.OK == CNotice.noticeWarningOKCancelID("DYWT1", "W"))
                {
                    saveDesignFile();
                }
            }
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            try
            {
                // 커멘드 파라메터로 디자인 파일명이 넘어오지 않은 경우는 바로 리턴한다.
                if (m_strCommandLineDesignFullName == string.Empty)
                    return;

                if (false == m_manageFile.isExistFile(m_strCommandLineDesignFullName))
                {
                    CNotice.printTrace("커멘드라인으로 입력한 디자인 파일이 존재하지 않는다.");
                    return;
                }

                loadDesignFile(m_strCommandLineDesignFullName);

                // 디자인 파일이 생성될 때의 디자인 작업 디렉토리는 프로그램 기본 디렉토리 강제 설정하고 있다.
                // 만약 디렉토리를 옮긴 디자인 디렉토리를 오픈 할 경우라면 
                // 이전 다지인 작업 디렉토리를 그대로 사용하면 디렉토리 문제가 발생하여 실행이 불가능하게 된다.
                // 이를 해결하기 위해
                // 작업파일을 Open 할 때는 파일을 오픈하는 위치로 작업파일의 디렉토리를 다시 설정하고 있다.
                m_design.m_strDesignDirPath = Path.GetDirectoryName(m_strCommandLineDesignFullName);

                // 프로젝트가 시작 했음을 표시하기 위해서 TreeView 에 기본 가지를 추가한다.
                TreeNode treeNode = new TreeNode("Parts", (int)EMKind.PARTS, (int)EMKind.PARTS);
                treeViewMain.Nodes.Add(treeNode);

                treeNode = new TreeNode("Experiments", (int)EMKind.EXPERIMENTS, (int)EMKind.EXPERIMENTS);
                treeViewMain.Nodes.Add(treeNode);

                foreach (CNode node in m_design.NodeList)
                {
                    this.addTreeNode(node.NodeName, node.m_kindKey);
                }

                // 제목줄에 디자인명을 표시한다
                this.Text = "DoSA-3D - " + m_design.m_strDesignName;

                CNotice.printUserMessage(m_design.m_strDesignName + m_resManager.GetString("_DHBO"));
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
            }
        }

        #endregion

        #region----------------------- 가상시험 관련 -------------------------------

        private bool isForceExperimentOK(CForceExperiment forceExperiment)
        {
            // 1. Moving Part 는 하나만 지원한다.
            //
            if (m_design.getMovingPartSize() != 1)
            {
                if (CSettingData.m_emLanguage == EMLanguage.Korean)
                    CNotice.noticeWarning("현버전은 하나의 구동부 파트까지만 지원합니다.");
                else
                    CNotice.noticeWarning("This version supports only one Moving Part.");

                return false;
            }

            // 2. 코일은 하나만 지원한다.
            if (m_design.getKindNodeSize(EMKind.COIL) != 1)
            {
                if (CSettingData.m_emLanguage == EMLanguage.Korean)
                    CNotice.noticeWarning("현 버전은 하나의 코일까지만 지원합니다.");
                else
                    CNotice.noticeWarning("This version supports only one Coil.");

                return false;
            }

            // 3. 코일형상 입력을 확인한다.
            if (m_design.isCoilAreaOK() == false)
            {
                if (CSettingData.m_emLanguage == EMLanguage.Korean)
                    CNotice.noticeWarning("코일형상 입력이 필요합니다.");
                else
                    CNotice.noticeWarning("You need to enter the coil geometry dimensions.");

                return false;
            }

            // 4. 코일사양 계산을 확인한다.
            if (m_design.isCoilSpecificationOK() == false)
            {
                if (CSettingData.m_emLanguage == EMLanguage.Korean)
                    CNotice.noticeWarning("코일사양 계산이 필요합니다.");
                else
                    CNotice.noticeWarning("You need to calculate the coil specification.");

                return false;
            }            
            
            //if (m_design.isDesignShapeOK() == false)
            //{
            //    CNotice.printTraceID("AEOI");
            //    return false;
            //}

            return true;
        }

        private void plotForceResult(CForceExperiment forceExperiment)
        {
            string strExperimentName = forceExperiment.NodeName;
            string strExperimentZeroName = strExperimentName + "_Zero";
            string strExperimentDirName = Path.Combine(m_design.m_strDesignDirPath, strExperimentName);
            string strExperimentZeroDirName = Path.Combine(m_design.m_strDesignDirPath, strExperimentZeroName);

            string strDensityImageFileFullName = Path.Combine(strExperimentDirName, "Image.gif");

            double dForceX = 0;
            double dForceY = 0;
            double dForceZ = 0;
            double dZeroForceX = 0;
            double dZeroForceY = 0;
            double dZeroForceZ = 0;

            try
            {
                getForceResult(strExperimentName, ref dForceX, ref dForceY, ref dForceZ);

                /// 영구자석이 포함된 자기회로의 정확도를 높이기위한 전류 0 시험이 존재하는 지를 확인한다.
                /// 전류를 인가했을 때와 하지 않았을 때의 자기력 차를 자기력으로 사용한다.
                if (m_manageFile.isExistDirectory(strExperimentZeroDirName) == true)
                {
                    getForceResult(strExperimentZeroName, ref dZeroForceX, ref dZeroForceY, ref dZeroForceZ);

                    dForceX = dForceX - dZeroForceX;
                    dForceY = dForceY - dZeroForceY;
                    dForceZ = dForceZ - dZeroForceZ;
                }

                textBoxForceX.Text = string.Format("{0:0.0000}", dForceX);
                textBoxForceY.Text = string.Format("{0:0.0000}", dForceY);
                textBoxForceZ.Text = string.Format("{0:0.0000}", dForceZ);


                if (m_manageFile.isExistFile(strDensityImageFileFullName) == true)
                {
                    // 파일을 잡고 있지 않기 위해서 임시 이미지를 사용하고 Dispose 한다.
                    Image tmpImage = Image.FromFile(strDensityImageFileFullName);

                    pictureBoxForce.Image = new Bitmap(tmpImage);
                    pictureBoxForce.SizeMode = PictureBoxSizeMode.StretchImage;

                    // 이미지이 연결을 끊어서 사용 가능하게 한다.
                    tmpImage.Dispose();
                }
                else
                {
                    CNotice.noticeWarningID("TINR");
                    return;
                }
 
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
            }
        }

        private void getForceResult(string strExperimentName, ref double dForceX, ref double dForceY, ref double dForceZ)
        {
            string strExperimentDirName = Path.Combine(m_design.m_strDesignDirPath, strExperimentName);

            //string strDensityImageFileFullName = Path.Combine(strExperimentDirName, "Image.gif");
            string strForceXFileFullName = Path.Combine(strExperimentDirName, "Fx.dat");
            string strForceYFileFullName = Path.Combine(strExperimentDirName, "Fy.dat");
            string strForceZFileFullName = Path.Combine(strExperimentDirName, "Fz.dat");

            bool bCheck = false;
            bool bRet = false;
            CReadFile readfile = new CReadFile();

            List<double> listLowData = new List<double>();

            bCheck = m_manageFile.isExistFile(strForceXFileFullName);
                
            if (bCheck == true)
            {
                listLowData.Clear();
                bRet = readfile.readSpaceRowData2(strForceXFileFullName, ref listLowData, 1);

                if(bRet == true)
                    dForceX = listLowData[1];
            }
            else
            {
                CNotice.noticeWarningID("TROA1");
                return;
            }

            bCheck = m_manageFile.isExistFile(strForceYFileFullName);

            if (bCheck == true)
            {
                listLowData.Clear();
                bRet = readfile.readSpaceRowData2(strForceYFileFullName, ref listLowData, 1);

                if (bRet == true)
                    dForceY = listLowData[1];
            }
            else
            {
                CNotice.noticeWarningID("TROA1");
                return;
            }

            bCheck = m_manageFile.isExistFile(strForceZFileFullName);

            if (bCheck == true)
            {
                listLowData.Clear();
                bRet = readfile.readSpaceRowData2(strForceZFileFullName, ref listLowData, 1);

                if (bRet == true)
                    dForceZ = listLowData[1];
            }
            else
            {
                CNotice.noticeWarningID("TROA1");
                return;
            }
        }

        #endregion        

        #region-------------------------- Save & Load Data -------------------------

        private bool saveDesignFile()
        {
            if (m_design.m_strDesignName.Length == 0)
            {
                CNotice.printTraceID("YATT9");
                return false;
            }

            try
            {
                /// New 에서 생성할 때 바로 디렉토리를 생성하면 만약, 프로젝트를 저장하지 않으면 디렉토리만 남는다.
                /// 따라서 저장할 때 없으면 디렉토리를 생성하는 것으로 바꾸었다.
                string strDesignDirPath = m_design.m_strDesignDirPath;

                if (false == m_manageFile.isExistDirectory(strDesignDirPath))
                {
                    // 다지인 디렉토리를 생성한다.
                    m_manageFile.createDirectory(strDesignDirPath);
                }

                string strActuatorDesignFileFullName = Path.Combine(strDesignDirPath, m_design.m_strDesignName + ".dsa");

                StreamWriter writeStream = new StreamWriter(strActuatorDesignFileFullName);
                CWriteFile writeFile = new CWriteFile();

                // Project 정보를 기록한다.
                writeFile.writeBeginLine(writeStream, "DoSA_3D_Project", 0);

                writeFile.writeDataLine(writeStream, "Writed", DateTime.Now, 1);
                writeFile.writeDataLine(writeStream, "DoSA_Version", Assembly.GetExecutingAssembly().GetName().Version, 1);
                writeFile.writeDataLine(writeStream, "File_Version", FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion, 1);

                m_design.writeObject(writeStream);

                writeFile.writeEndLine(writeStream, "DoSA_3D_Project", 0);


                writeStream.Close();

                // 저장을 하고 나면 초기화 한다.
                m_design.m_bChanged = false;

            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
            }

            return true;
        }

        private bool loadDesignFile(string strDesignFileFullName)
        {
            CReadFile readFile = new CReadFile();
            List<string> listStringLines = new List<string>();
            List<string> listStringDesignData = new List<string>();
            bool bDesignLine = false;

            try
            {
                // 전체 내용을 읽어드린다.
                readFile.getAllLines(strDesignFileFullName, ref listStringLines);

                foreach (string strLine in listStringLines)
                {
                    // Design 구문 안의 내용만 listDesignActuator 담는다.
                    //
                    // endLine 과 beginLine 을 확인하는 위치가 중요하다. 
                    // Design 의 시작과 끝을 알리는 구문 Line 을 포함하지 않기 위해 endLine확인 -> 복사 -> beginLine확인 순서로 진행한다.
                    if (readFile.isEndLine(strLine) == "Design")
                        bDesignLine = false;

                    // Design 구문만 listStringDesignData 에 담는다
                    if (bDesignLine == true)
                    {
                        listStringDesignData.Add(strLine);
                    }
                    else
                    {
                        if (readFile.isBeginLine(strLine) == "Design")
                            bDesignLine = true;
                    }
                }

                // 저장파일의 Project 영역안의 Design 영역을 분석하여 m_designActuator 로 Design 을 읽어온다
                readObject(listStringDesignData);

            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
                return false;
            }

            return true;
        }

        // writeObject() 는 종류가 정해져 있는 Parts 객체를 상위클래스 Node 로 호출하기 때문에
        // Virtual 함수를 사용할 수 있어 CDesign 의 멤버변수를 사용할 수 있다.
        // 하지만 readObject() 은 아직 존재하지 않은 객체를 종류별로 만들어야 하기 때문에  
        // CDesign 의 멤버변수를 사용할 수 없어서 MainForm 의 멤버함수로 사용한다.
        internal void readObject(List<string> listStringDesignData)
        {
            string strCommand = string.Empty;
            string strData = string.Empty;
            bool bNodeDataRegion = false;
            string[] arraySplit;

            CReadFile readFile = new CReadFile();
            List<string> listStringNode = new List<string>();

            try
            {

                foreach (string strLine in listStringDesignData)
                {
                    // Design 구문 안의 내용만 listDesignActuator 담는다.
                    //
                    // endLine 과 beginLine 을 확인하는 위치가 중요하다. 
                    // Design 의 시작과 끝을 알리는 구문 Line 을 포함하지 않기 위해 endLine확인 -> 복사 -> beginLine확인 순서로 진행한다.
                    strCommand = readFile.isEndLine(strLine);
                    if (strCommand != null)
                    {
                        bNodeDataRegion = false;

                        // [주의사항]
                        // - m_designActuator.addNode() 는 RemainedShapeName 에서 형상을 찾고 제외함으로 Open 용도로 사용해서는 안되고 생성용도로만 사용하라.
                        switch (strCommand)
                        {
                            // CMagneticParts 하위 객체
                            case "Coil":
                                CCoil coil = new CCoil();
                                if (true == coil.readObject(listStringNode))
                                    m_design.NodeList.Add(coil);
                                break;

                            case "Steel":
                                CSteel steel = new CSteel();
                                if (true == steel.readObject(listStringNode))
                                    m_design.NodeList.Add(steel);
                                break;

                            case "Magnet":
                                CMagnet magnet = new CMagnet();
                                if (true == magnet.readObject(listStringNode))
                                    m_design.NodeList.Add(magnet);
                                break;

                            // CExpriment 하위 객체
                            case "ForceExperiment":
                                CForceExperiment forceExperiment = new CForceExperiment();
                                if (true == forceExperiment.readObject(listStringNode))
                                    m_design.NodeList.Add(forceExperiment);
                                break;

                            default:
                                break;
                        }

                        // End 명령줄이 Node 이외 영역 처리가 되는 것을 막는다. 
                        continue;
                    }

                    // Node 영역 처리
                    if (bNodeDataRegion == true)
                    {
                        listStringNode.Add(strLine);
                    }
                    // Node 이외 영역 처리
                    else
                    {
                        // Node 데이터 영역의 시작점인지를 확인한다.
                        strCommand = readFile.isBeginLine(strLine);

                        // Node 데이터 시작점이라면 "노드명" 이 리턴된다.
                        if (strCommand != null)
                        {
                            listStringNode.Clear();
                            // 노드 데이터를 List 에 저장하라.
                            bNodeDataRegion = true;
                        }
                        else
                        {
                            readFile.readDataInLine(strLine, ref strCommand, ref strData);

                            switch (strCommand)
                            {
                                case "DesignName":
                                    m_design.m_strDesignName = strData;
                                    break;


                                case "AllShapeName":
                                    arraySplit = strData.Split(',');

                                    // -1 : 문자열을 만드는 과정에서 마지막에 ','가 추가되어 있어서 문자열 배열의 마지막은 "" 이기 때문이다.
                                    // [주의사항]
                                    // - m_actuator.addShape() 는 AllShapeName 와 RemainedShapeName 모두 추가되기 때문에 Open 용도로 사용해서는 안되고 생성용도로만 사용하라.
                                    for (int i = 0; i < arraySplit.Length - 1; i++)
                                        m_design.AllShapeNameList.Add(arraySplit[i]);

                                    break;

                                case "ShapeMinX":
                                    m_design.MinX = Convert.ToDouble(strData);
                                    break;

                                case "ShapeMaxX":
                                    m_design.MaxX = Convert.ToDouble(strData);
                                    break;

                                case "ShapeMinY":
                                    m_design.MinY = Convert.ToDouble(strData);
                                    break;

                                case "ShapeMaxY":
                                    m_design.MaxY = Convert.ToDouble(strData);
                                    break;

                                case "ShapeMinZ":
                                    m_design.MinZ = Convert.ToDouble(strData);
                                    break;

                                case "ShapeMaxZ":
                                    m_design.MaxZ = Convert.ToDouble(strData);
                                    break;

                                case "RemainedShapeName":
                                    arraySplit = strData.Split(',');

                                    // -1 : 문자열을 만드는 과정에서 마지막에 ','가 추가되어 있어서 문자열 배열의 마지막은 "" 이기 때문이다.
                                    for (int i = 0; i < arraySplit.Length - 1; i++)
                                        m_design.RemainedShapeNameList.Add(arraySplit[i]);

                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
            }
        }

        #endregion

        #region------------------------- TreeView 관련 -------------------------


        //트리뷰에서 삭제 한다
        private void treeView_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                // Delete 키에서 Tree 를 삭제한다.
                if (e.KeyCode == Keys.Delete)
                {
                    // [주의] 
                    // Node Name 이 SelectedNode.Name 아니라 SelectedNode.Text 에 들어 있다
                    string selectedNodeText = this.treeViewMain.SelectedNode.Text;

                    if (selectedNodeText == "Parts" || selectedNodeText == "Experiments")
                    {
                        return;
                    }

                    CNode node = m_design.getNode(selectedNodeText);

                    if (node == null)
                    {
                        CNotice.printTraceID("TDNI");
                        return;
                    }

                    // 가상 시험 Node 의 경우는 결과 디렉토리와 연결이 되기 때문에
                    // 해석 결과 디렉토리가 있는 경우는 해석결과를 삭제할지를 물어보고 같이 삭제한다.
                    if (node.GetType().BaseType.Name == "CExperiment")
                    {
                        string strExperimentDirName = Path.Combine(m_design.m_strDesignDirPath, node.NodeName);

                        if (m_manageFile.isExistDirectory(strExperimentDirName) == true)
                        {
                            DialogResult ret = CNotice.noticeWarningOKCancelID("TTHR", "W");

                            if (ret == DialogResult.Cancel)
                                return;

                            m_manageFile.deleteDirectory(strExperimentDirName);

                            // 삭제되는 시간이 필요한 듯 한다.
                            Thread.Sleep(1000);
                        }
                    }

                    // 수정 되었음을 기록한다.
                    m_design.m_bChanged = true;

                    this.treeViewMain.SelectedNode.Remove();
                    deleteRawNode(selectedNodeText);

                }

            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
            }

        }

        //트리 노드를 선택
        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string selectedNodeText = this.treeViewMain.SelectedNode.Text;

            if (selectedNodeText == "Parts" || selectedNodeText == "Experiments")
            {
                return;
            }

            // 신기하게 treeViewMain_Click 나 treeViewMain_MouseUp 이벤트에서 동작시키면 이상하게 동작한다.
            // 그래서 중복 호출이 되더라도 AfterSelect 을 사용한다.
            showNode(selectedNodeText);
        }

        //노드 추가를 위한 유효성검사
        private void addRawNode(EMKind emKind)
        {
            string strNodeName = string.Empty;
            bool bRet = false;

            // 디자인이 존재하지 않으면 경고를 알리고 리턴한다.
            if (m_design.m_strDesignName.Length == 0)
            {
                CNotice.noticeWarningID("TIND");
                return;
            }

            try
            {

                PopupAddNode popupNodeName = new PopupAddNode(emKind, m_design.RemainedShapeNameList);
                popupNodeName.StartPosition = FormStartPosition.CenterParent;

                /// 이해할 수 없지만, 자동으로 Owner 설정이 되는 경우도 있고 아닌 경우도 있기 때문에
                /// Shape 창에서 MainForm 을 접근할 수 있도록 미리 설정을 한다.
                popupNodeName.Owner = this;

                if (DialogResult.Cancel == popupNodeName.ShowDialog())
                    return;

                strNodeName = popupNodeName.NodeName;

                switch (emKind)
                {
                    case EMKind.COIL:
                        CCoil coil = new CCoil();
                        coil.NodeName = strNodeName;
                        coil.m_kindKey = emKind;

                        bRet = m_design.addNode(coil);
                        break;

                    case EMKind.MAGNET:
                        CMagnet magnet = new CMagnet();
                        magnet.NodeName = strNodeName;
                        magnet.m_kindKey = emKind;

                        bRet = m_design.addNode(magnet);
                        break;

                    case EMKind.STEEL:
                        CSteel steel = new CSteel();
                        steel.NodeName = strNodeName;
                        steel.m_kindKey = emKind;

                        bRet = m_design.addNode(steel);
                        break;

                    case EMKind.FORCE_EXPERIMENT:
                        CForceExperiment forceExperiment = new CForceExperiment();
                        forceExperiment.NodeName = strNodeName;
                        forceExperiment.m_kindKey = emKind;
                        
                        // 생성될 때 환경설정의 조건으로 초기화한다.
                        forceExperiment.MeshSizePercent = CSettingData.m_dMeshLevelPercent;
                        forceExperiment.ActuatorType = CSettingData.m_emActuatorType;
                        
                        bRet = m_design.addNode(forceExperiment);
                        break;

                    default:
                        CNotice.printTraceID("YATT4");
                        return;
                }

                // 수정 되었음을 기록한다.
                m_design.m_bChanged = true;

                if (bRet == true)
                {
                    // Treeview 에 추가한다
                    addTreeNode(strNodeName, emKind);

                    // 해당 Node 의 Properies View 와 Information Windows 를 표시한다
                    showNode(strNodeName);
                }
                else
                    CNotice.noticeWarningID("TNIA");

            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
            }

        }

        //노드추가
        private void addTreeNode(string strName, EMKind kind)
        {

            try
            {
                // emKIND 와 imageList 의 이미지와 순서가 같아야 한다
                TreeNode treeNode = new TreeNode(strName, (int)kind, (int)kind);

                // [ 유의사항 ]
                // TreeView 의 구조는 노드안에 노드가 들어있는 구조이다
                // 특정 가지에 TreeNode 를 추가하려면 
                // Nodes[2].Nodes[0].Nodes.Add() 와 같이 TreeNode 를 따라 특정노드로 들어간후에 추가한다
                switch (kind)
                {
                    case EMKind.COIL:
                    case EMKind.MAGNET:
                    case EMKind.STEEL:
                        treeViewMain.Nodes[FIRST_PARTS_INDEX].Nodes.Add(treeNode);
                        break;

                    case EMKind.FORCE_EXPERIMENT:
                        treeViewMain.Nodes[FIRST_ANALYSIS_INDEX].Nodes.Add(treeNode);
                        break;

                    default:
                        return;
                }

                // 추가후 노드를 선택한다
                treeViewMain.SelectedNode = treeNode;
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
            }
        }

        //-------------------------------------------------------------------
        // propertyGridMain.Refresh() 와 충돌이 발생하여 사용을 포기한다.
        //-------------------------------------------------------------------
        // PropertyGrid Column 의 폭을 변경한다.
        //public static void setLabelColumnWidth(PropertyGrid grid, int width)
        //{
        //    if (grid == null)
        //        throw new ArgumentNullException("grid");

        //    // get the grid view
        //    Control view = (Control)grid.GetType().GetField("gridView", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(grid);

        //    // set label width
        //    FieldInfo fi = view.GetType().GetField("labelWidth", BindingFlags.Instance | BindingFlags.NonPublic);
        //    fi.SetValue(view, width);

        //    // refresh
        //    view.Invalidate();
        //}

        // 선택한 노드를 Information Window 와 Property View 에 보여준다
        private void showNode(string nodeName)
        {
            CNode node = m_design.getNode(nodeName);

            string strExperimentDirName = string.Empty;

            try
            {
                if (node != null)
                {
                    // 프로퍼티창을 변경한다.
                    propertyGridMain.SelectedObject = node;

                    // 프로퍼티창의 첫번째 Column 의 폭을 변경한다. (사용 포기함)
                    //setLabelColumnWidth(propertyGridMain, 160);

                    /// 프로퍼티창에서 이름을 변경할 때 기존에 이미 있는 이름을 선택하는 경우
                    /// 복구를 위해 저장해 둔다.
                    m_strBackupNodeName = node.NodeName;

                    // Expand Treeview when starting
                    foreach (TreeNode tn in treeViewMain.Nodes)
                        tn.Expand();

                    splitContainerRight.Panel1.Controls.Clear();

                    strExperimentDirName = Path.Combine(m_design.m_strDesignDirPath, node.NodeName);

                    switch (node.m_kindKey)
                    {
                        case EMKind.COIL:
                            splitContainerRight.Panel1.Controls.Add(this.panelCoil);
                            break;

                        case EMKind.MAGNET:
                            splitContainerRight.Panel1.Controls.Add(this.panelMagnet);
                            break;

                        case EMKind.STEEL:
                            splitContainerRight.Panel1.Controls.Add(this.panelSteel);

                            CSteel steel = (CSteel)node;
                            drawBHCurve(steel.Material);
                            break;

                        case EMKind.FORCE_EXPERIMENT:

                            string strFieldImageFullName = Path.Combine(strExperimentDirName, "Image.gif");

                            // 해석결과가 존재하지 않으면 Result 와 Report 버튼을 비활성화 한다.
                            if (m_manageFile.isExistFile(strFieldImageFullName) == true)
                            {
                                buttonLoadForceResult.Enabled = true;
                            }
                            else
                            {
                                buttonLoadForceResult.Enabled = false;
                            }

                            splitContainerRight.Panel1.Controls.Add(this.panelForce);

                            // 초기이미지가 없어서 이미지를 비우고 있다.
                            loadDefaultImage(EMKind.FORCE_EXPERIMENT);
                            textBoxForceX.Text = "0.0";
                            textBoxForceY.Text = "0.0";
                            textBoxForceZ.Text = "0.0";

                            // 트리로 선택할 때도 가상실험 내부 전류를 재계산한다.
                            setCurrentInExperiment(node);

                            break;

                        default:
                            return;
                    }
                }
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
            }
        }

        public class CSortDataSet
        {
            public double m_dLength;
            public int m_iIndex;

            public CSortDataSet(double length, int index)
            {
                m_dLength = length;
                m_iIndex = index;
            }
        }       
   

        /// 트리를 삭제 한다
        private void deleteRawNode(string nodeName)
        {
            // 내부적으로 명칭배열까지도 모두 삭제한다.
            m_design.deleteNode(nodeName);

            // 정보창을 초기화 한다
            splitContainerRight.Panel1.Controls.Clear();
            splitContainerRight.Panel1.Controls.Add(this.panelEmpty);

            // PropertyGrid 창을 초기화 한다.
            propertyGridMain.SelectedObject = null;
        }

        #endregion

        #region------------------------ PropertyView 관련 ------------------------------

        //property 수정
        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            CNode node = (CNode)propertyGridMain.SelectedObject;

            string strChangedItemValue = e.ChangedItem.Value.ToString();
            string strChangedItemLabel = e.ChangedItem.Label;
            
            try
            {
                // Node 의 이름이 변경된 경우
                if (strChangedItemLabel == "Node Name")
                {
                    /// Node 이름의 경우 PropertyGrid 에서 수정과 동시에 Node 이름이 같이 변경되기 때문에
                    /// 기존에 존재하는 이름으로 변경했는지의 판단은 Node 이름을 겹치는 것으로 확인해야 한다.
                    if (true == m_design.duplicateNodeName())
                    {
                        CNotice.noticeWarningID("TNIA");

                        /// PropertyGrid 에 Node 를 올릴 때 저장해둔 Node 이름으로 복원한다.
                        node.NodeName = m_strBackupNodeName;

                        propertyGridMain.Refresh();

                        return;
                    }

                    /// 변경된 이름은 입력과 동시에 Node 에 반영되었기 때문에 저장을 불 필요한다.
                    //node.NodeName = strChangedItemValue;

                    // 복원용 이름은 PropertyGrid 에 Node 를 올릴 때만 저장되기 때문에
                    // PropertyGrid 를 갱신하지 않는 상태에서 여러번 이름을 변경하다가 문제가 발생하면
                    // 이전 이름인 PropertyGrid 에 Node 를 올릴 때의 이름으로 복구 된다.
                    // 이를 방지하기 위해서 재대로 저장이 된 경우는 복원용 이름을 변경한다.
                    m_strBackupNodeName = strChangedItemValue;

                    /// 트리의 이름을 변경한다.
                    this.treeViewMain.SelectedNode.Text = strChangedItemValue;
                }            
                
                switch (node.m_kindKey)
                {
                    case EMKind.COIL:

                        CCoil coil = (CCoil)node;

                        if (strChangedItemLabel == "Copper Diameter [mm]" || strChangedItemLabel == "Coil Wire Grade")
                        {
                            coil.WireDiameter = coil.calculateWireDiameter();
                        }

                        break;

                    case EMKind.STEEL:

                        // 연자성체의 재질을 선택한 경우만 실행을 한다.
                        if (strChangedItemLabel == "Part Material")
                        {
                            drawBHCurve(strChangedItemValue);
                        }

                        break;

                    case EMKind.MAGNET:

                        CReadFile readFile = new CReadFile();

                        string strProgramMaterialDirName = Path.Combine(CSettingData.m_strProgramDirPath, "Materials");
                        string strMagnetMaterialFileFullName = Path.Combine(strProgramMaterialDirName, "DoSA_MG.dmat");

                        double dMur = 0;
                        double dHc = 0;
                        double dMu0 = 4 * Math.PI * 1e-7;
                        
                        // 영구자석의 재질을 선택한 경우만 실행을 한다.
                        if (strChangedItemLabel == "Part Material")
                        {
                            readFile.readMaterialMagnetData(strMagnetMaterialFileFullName, strChangedItemValue, ref dMur, ref dHc);

                            ((CMagnet)node).Hc = dHc;
                            ((CMagnet)node).Br = Math.Round(dMu0 * 1.0378 * dHc, 5);
                        }

                        break;

                    case EMKind.FORCE_EXPERIMENT:

                        if (e.ChangedItem.Label == "Voltage [V]")
                        {
                            setCurrentInExperiment(node);
                        }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
            }

            // 수정 되었음을 기록한다.
            m_design.m_bChanged = true;

            propertyGridMain.Refresh();
        }


        private void setCurrentInExperiment(CNode node)
        {
            // 총 저항은 합산이 필요함으로 0.0f 로 초기화 한다.
            double total_resistance = 0.0f;

            // 총 저항
            foreach (CNode nodeTemp in m_design.NodeList)
                if (nodeTemp.m_kindKey == EMKind.COIL)
                {
                    total_resistance += ((CCoil)nodeTemp).Resistance;
                }

            switch (node.m_kindKey)
            {
                case EMKind.FORCE_EXPERIMENT:

                    CForceExperiment forceExperiment = (CForceExperiment)node;

                    // 전류
                    if (total_resistance != 0.0f)
                        forceExperiment.Current = (forceExperiment.Voltage / total_resistance);
                    else
                        forceExperiment.Current = 0.0f;

                    break;

                default:
                    break;
            }

        }
        //property Category구성
        private void CollapseOrExpandCategory(PropertyGrid propertyGrid, string categoryName, bool bExpand = false)
        {
            GridItem root = propertyGrid.SelectedGridItem;
            //Get the parent
            while (root.Parent != null)
                root = root.Parent;

            if (root != null)
            {
                foreach (GridItem g in root.GridItems)
                {
                    if (g.GridItemType == GridItemType.Category && g.Label == categoryName)
                    {
                        g.Expanded = bExpand;
                        break;
                    }
                }
            }
        }

        #endregion

        #region----------------------- Information Window 관련 -----------------------

        //steel 그래프를 생성한다
        private void drawBHCurve(String strMaterialName)
        {
            CReadFile readFile = new CReadFile();

            List<double> listH = new List<double>();
            List<double> listB = new List<double>();


            try
            {
                //string strMaxwellMaterialDirName = CSettingData.m_strMaxwellMaterialDirName;
                string strProgramMaterialDirName = Path.Combine(CSettingData.m_strProgramDirPath, "Materials");

                // 내장 비자성 재료
                if (strMaterialName == "Aluminum, 1100" || strMaterialName == "Copper" ||
                    strMaterialName == "316 Stainless Steel" || strMaterialName == "304 Stainless Steel" || strMaterialName == "Air")
                {
                    chartBHCurve.Series.Clear();                    // Series 삭제
                    // Series 생성
                    System.Windows.Forms.DataVisualization.Charting.Series sBHCurve = chartBHCurve.Series.Add("BH");
                    sBHCurve.ChartType = SeriesChartType.Line;      // 그래프 모양을 '선'으로 지정

                    listH.Clear();
                    listB.Clear();

                    // 공기와 같은 투자율로 처리한다.
                    for (int x = 0; x <= 30000; x += 5000)
                    {
                        listH.Add(x);
                        listB.Add(x * 1.25663706143592E-06);
                    }

                    drawXYChart(chartBHCurve, listH, listB, "H [A/m]", "B [T]", 0.0f, 30000.0f, 0.0f, 2.5f);
                }
                /// 내장 연자성 재료
                else
                {
                    string strMaterialFileFullName = Path.Combine(strProgramMaterialDirName, "DoSA_MS.dmat");

                    if (true == readFile.readMaterialBHData(strMaterialFileFullName, strMaterialName, ref listH, ref listB))
                    {
                        if (listH.Count == 0)
                        {
                            CNotice.printTraceID("TDFT");
                            return;
                        }

                        if (listH.Count != listB.Count)
                        {
                            CNotice.printTraceID("TSOT");
                            return;
                        }

                        drawXYChart(chartBHCurve, listH, listB, "H [A/m]", "B [T]", 0.0f, 30000.0f, 0.0f, 2.5f);
                    }
                    else
                    {
                        CNotice.printTrace("There is no DoSA_MS.dmat file.\nPlease check Material directory.");
                    }
                }
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
            }
        }

        //Information Window에 Stroke 이미지로드
        private void loadDefaultImage(EMKind kind)
        {
            //string strImageFullFileName;
            //bool bRet = false;

            try
            { 
                switch (kind)
                {
                    case EMKind.FORCE_EXPERIMENT:
                        // 이미지를 비운다
                        pictureBoxForce.Image = null;
                        break;

                    case EMKind.COIL:
                    case EMKind.MAGNET:
                    case EMKind.STEEL:
                        break;

                    default:
                        return;
                }
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
                return;
            }           
        }

        private void drawXYChart(System.Windows.Forms.DataVisualization.Charting.Chart chartData,
                                List<double> listX, List<double> listY,
                                string strXLabel = "X", string strYLabel = "Y",
                                double dMinX = Double.NaN, double dMaxX = Double.NaN, double dMinY = Double.NaN, double dMaxY = Double.NaN)
        {
            try
            {
                chartData.Series.Clear();                    // Series 삭제

                // Series 생성
                System.Windows.Forms.DataVisualization.Charting.Series sCurve = chartData.Series.Add("Data");
                sCurve.ChartType = SeriesChartType.Line;      // 그래프 모양을 '선'으로 지정

                // 데이터를 추가한다.
                for (int i = 0; i < listX.Count; i++)
                    sCurve.Points.AddXY(listX[i], listY[i]);

                chartData.ChartAreas[0].AxisX.Title = strXLabel;
                chartData.ChartAreas[0].AxisY.Title = strYLabel;
                chartData.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Arial", 12, FontStyle.Regular);
                chartData.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("Arial", 12, FontStyle.Regular);

                chartData.ChartAreas[0].AxisX.LabelStyle.Format = "{0:0.00}";

                if (dMinX >= dMaxX || dMinY >= dMaxY)
                {
                    CNotice.printTraceID("TMVI");
                    return;
                }

                chartData.ChartAreas[0].AxisX.Minimum = dMinX;    // X 축 범위 지정
                chartData.ChartAreas[0].AxisX.Maximum = dMaxX;

                // 축 스케일을 입력하는 경우는 무조건 구분등분을 5로 설정하였다.
                if (dMinX != double.NaN)
                    chartData.ChartAreas[0].AxisX.Interval = (dMaxX - dMinX) / 5.0f;

                chartData.ChartAreas[0].AxisY.Minimum = dMinY;   // Y1 축 범위 지정
                chartData.ChartAreas[0].AxisY.Maximum = dMaxY;

                // 축 스케일을 입력하는 경우는 무조건 구분등분을 5로 설정하였다.
                if (dMinY != double.NaN)
                    chartData.ChartAreas[0].AxisY.Interval = (dMaxY - dMinY) / 5.0f;

                chartData.ChartAreas[0].RecalculateAxesScale();

                sCurve.Color = Color.SteelBlue;
                sCurve.BorderWidth = 2;
                sCurve.MarkerColor = Color.SteelBlue;
                sCurve.MarkerSize = 8;
                sCurve.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Diamond;
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
                CNotice.printTraceID("AEOI1");
                return;
            }
        }

        #endregion

        #region---------------------- 기타 기능함수 -----------------------------

        private bool checkFramework451()
        {
            int iReleaseKey;

            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                iReleaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));

                if (iReleaseKey >= 378675)
                    return true;
                else
                    return false;

                // 393273 : "4.6 RC or later"
                // 379893 : "4.5.2 or later"
                // 378675 : "4.5.1 or later"
                // 378389 : "4.5 or later"
            }
        }

        private void openWebsite(string strWebAddress)
        {
            System.Diagnostics.Process.Start(strWebAddress);
        }
        
        #endregion                        
    }
}
