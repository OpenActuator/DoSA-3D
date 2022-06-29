using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using System.Windows.Forms;

// 파일 처리
using System.IO;

// Debugging
using System.Diagnostics;
using System.Resources;
using System.Windows.Forms;
using System.Threading;

namespace gtLibrary
{
    public class CManageFile
    {

        #region File - 체크(사용여부,존재여부),복사,삭제,이동


        public List<string> getFileList(string dirPath)
        {
            List<string> lsFiles = new List<string>();

            try
            {
                if (false == isExistDirectory(dirPath))
                {
                    CNotice.printLog("존재하지 않는 " + dirPath + " 의 내부 파일정보를 얻으려고 합니다.");

                    // 오류가 발생해도 null 을 리턴하지 않고 항목없이 생성된 List 를 그대로 리턴 한다.
                    return lsFiles;
                }

                lsFiles = Directory.GetFiles(dirPath).Cast<string>().ToList();

                return lsFiles;
            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);

                // 오류가 발생해도 null 을 리턴하지 않고 항목없이 생성된 List 를 그대로 리턴 한다.
                return lsFiles;
            }
        }

        public bool deleteFile(string strFileFullPathName)
        {
            try
            {
                if (false == isExistFile(strFileFullPathName))
                {
                    ResourceManager resManager = ResourceManager.CreateFileBasedResourceManager("LanguageResource", Application.StartupPath, null);

                    CNotice.printLog(resManager.GetString("TIAA2") + strFileFullPathName + resManager.GetString("_TDNE"));
                    return false;
                }

                File.Delete(strFileFullPathName);

                return true;

            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);

                return false;
            }
        }

        public bool isExistFile(string fileFullPathName)
        {
            try
            {
                return File.Exists(fileFullPathName);
            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);

                return false;
            }
        }

        public bool copyFile(string sourceFileFullPathName, string destFileFullPathName, bool bOverWrite = false)
        {
            try
            {
                string destDirName = Path.GetDirectoryName(destFileFullPathName);

                if (false == isExistFile(sourceFileFullPathName))
                {
                    ResourceManager resManager = ResourceManager.CreateFileBasedResourceManager("LanguageResource", Application.StartupPath, null);

                    CNotice.printLog(resManager.GetString("TIAA") + sourceFileFullPathName + resManager.GetString("_TDNE"));
                    return false;
                }

                if (!isExistDirectory(destDirName))
                    createDirectory(destDirName);

                // 이미 파일이 존재하면 복사를 취소 한다.
                if (true == isExistFile(destFileFullPathName) && bOverWrite == false)
                {
                    ResourceManager resManager = ResourceManager.CreateFileBasedResourceManager("LanguageResource", Application.StartupPath, null);

                    CNotice.printLog(resManager.GetString("TIAA4") + destFileFullPathName + resManager.GetString("_TDNE"));
                    return false;
                }

                File.Copy(sourceFileFullPathName, destFileFullPathName, bOverWrite);

                return true;
            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);

                return false;
            }
        }

        #endregion

        #region Direcotry - 체크(사용여부,존재여부),생성,복사,삭제,이동,파일리스트

        /// <summary>
        /// PC 디렉토리의 아래 문제를 해결한다.
        /// </summary>
        /// - 스페이스 문제 : """ 으로 묶는다.
        /// - \t 문제 : "\\" -> "/" 로 변경한다.
        public string solveDirectoryNameInPC(string str)
        {
            return "\"" + str.Replace("\\", "/") + "\"";
        }

        public List<string> getDirectoryList(string dirPath)
        {
            List<string> lsDirs = new List<string>();

            try
            {
                if (false == isExistDirectory(dirPath))
                {
                    ResourceManager resManager = ResourceManager.CreateFileBasedResourceManager("LanguageResource", Application.StartupPath, null);

                    CNotice.printLog(resManager.GetString("TIAA3") + dirPath + resManager.GetString("_DITD"));

                    // 오류가 발생해도 null 을 리턴하지 않고 항목없이 생성된 List 를 그대로 리턴 한다.
                    return lsDirs;
                }

                lsDirs = Directory.GetDirectories(dirPath).Cast<string>().ToList();

                return lsDirs;
            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);

                // 오류가 발생해도 null 을 리턴하지 않고 항목없이 생성된 List 를 그대로 리턴 한다.
                return lsDirs;
            }
        }

        public bool isExistDirectory(string dirPath)
        {
            try
            {
                return Directory.Exists(dirPath);
            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);

                return false;
            }
        }

        public bool setCurrentDirectory(string strDirectory)
        {
            try
            {
                System.IO.Directory.SetCurrentDirectory(strDirectory);
                return true;
            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);

                return false;
            }
        }

        public bool createDirectory(string dirPath)
        {
            try
            {
                if (true == isExistDirectory(dirPath))
                {
                    ResourceManager resManager = ResourceManager.CreateFileBasedResourceManager("LanguageResource", Application.StartupPath, null);

                    CNotice.printLog(resManager.GetString("TIAA1") + dirPath + resManager.GetString("_TAE"));
                    return false;
                }

                Directory.CreateDirectory(dirPath);
                return true;

            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);

                return false;
            }
        }

		public bool deleteDirectory(string dirPath)
		{
			try
			{
				if (false == isExistDirectory(dirPath))
				{
                    ResourceManager resManager = ResourceManager.CreateFileBasedResourceManager("LanguageResource", Application.StartupPath, null);

                    CNotice.printLog(resManager.GetString("TIAA2") + dirPath + resManager.GetString("_TDNE"));
					return false;
				}

				Directory.Delete(dirPath, true);
				return true;
			}
			catch (Exception ex)
			{
				CNotice.printLog(ex.Message);

				return false;
			}
        }

        public bool copyDirectory(string sourDirPath, string destDirPath)
        {
            try
            {
                if (false == isExistDirectory(sourDirPath))
                {
                    CNotice.printLog("존재하지 않는 " + sourDirPath + " 를 복사하려고 합니다.");
                    return false;
                }

                if (!Directory.Exists(destDirPath))
                    createDirectory(destDirPath);

                string[] files = Directory.GetFiles(sourDirPath);
                string[] dirs = Directory.GetDirectories(sourDirPath);

                foreach (string file in files)
                {
                    string name = Path.GetFileName(file);
                    string dest = Path.Combine(destDirPath, name);
                    File.Copy(file, dest);
                }

                // foreach 안에서 재귀 함수를 통해서 폴더 내부의 폴더 및 파일 복사 진행한다.
                foreach (string dir in dirs)
                {
                    string name = Path.GetFileName(dir);
                    string dest = Path.Combine(destDirPath, name);
                    copyDirectory(dir, dest);
                }

                return true;
            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);

                return false;
            }
        }

        #endregion


        /// <summary>
        /// 특정 파일이 생성될때 까지 기다린다. 
        /// (단, 생성 종료가 아니라 생성 시작을 감지함을 주의하라)
        /// </summary>
        /// <param name="strFileFullName">감지할 파일명</param>
        /// <param name="dMaxTime_ms">최대 시간 : ms 단위</param>
        public bool waitForFileInOtherThread(string strFileFullName, double dMaxTime_ms = 5000)
        {
            const int STEP_TIME_ms = 50;
            int nMaxCount = (int)(dMaxTime_ms / (float)STEP_TIME_ms);

            int nCount = 0;

            try
            {
                do
                {
                    // 감지 최대시간을 초과해서 리턴하는 경우
                    if (nCount > nMaxCount)
                        return false;

                    // 파일을 감지해서 리턴하는 경우
                    if (true == isExistFile(strFileFullName))
                    {
                        // 파일 생성을 감지했기 때문에 저장시간을 기다려 준다.
                        // 
                        // 문제점
                        // - 파일의 생성 완료 시간을 알 수 없어서 고정된 1초를 기다리고 있다.
                        Thread.Sleep(1000);
                        return true;
                    }

                    Thread.Sleep(STEP_TIME_ms);

                    nCount++;
                }
                while (true);
            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);

                return false;
            }
        }
    }
}
