using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;

// 파일 처리
using System.IO;

// Debugging
using System.Diagnostics;

// 함수 이름 읽어내기
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

namespace gtLibrary
{
    public class CWriteFile
    {
        CManageFile m_manageFile = new CManageFile();

        #region --------------------------- 데이터파일 저장 함수들 ---------------------------

        // 리스트의 문자열을 하나씩 한 라인으로 저장한다.
        public bool writeLineString(string strFileFullName, List<string> listString, bool bOverwrite = false)
        {
            string[] arrayString = new string[listString.Count];



            try
            {
                if (true == m_manageFile.isExistFile(strFileFullName) && bOverwrite == false)
                {
                    ResourceManager resManager = ResourceManager.CreateFileBasedResourceManager("LanguageResource", Application.StartupPath, null);

                    CNotice.printTrace(resManager.GetString("TIAA6") + strFileFullName + resManager.GetString("_TAE"));
                    return false;
                }

                // 생성할 파일의 디렉토리가 존제하지 않으면 우선 디렉토리를 생성한다.
                if (false == m_manageFile.isExistDirectory(Path.GetDirectoryName(strFileFullName)))
                {
                    m_manageFile.createDirectory(Path.GetDirectoryName(strFileFullName));
                }

                // 기존에 파일이 존재하면 삭제후 쓰기를 한다.
                if (m_manageFile.isExistFile(strFileFullName) == true)
                {
                    m_manageFile.deleteFile(strFileFullName);
                    Thread.Sleep(50);
                }

                for (int i = 0; i < listString.Count; i++)
                {
                    arrayString.SetValue(listString.ElementAt(i), i);
                }

                File.WriteAllLines(strFileFullName, arrayString);

            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
                return false;
            }

            return true;
        }

        #endregion

        #region --------------------------- 작업파일 저장용 함수들 ---------------------------

        internal void writeBeginLine(StreamWriter writeStream, string strCommand, int nTabCount)
        {
            string strTab = string.Empty;

            // 오류 방지
            if (writeStream == null)
                return;

            for (int i = 0; i < nTabCount; i++)
                strTab = strTab + "\t";

            writeStream.Write(strTab + "$begin "); writeStream.WriteLine("\"" + strCommand + "\"");
        }

        internal void writeEndLine(StreamWriter writeStream, string strCommand, int nTabCount)
        {
            string strTab = string.Empty;

            // 오류 방지
            if (writeStream == null)
                return;

            for (int i = 0; i < nTabCount; i++)
                strTab = strTab + "\t";

            writeStream.Write(strTab + "$end "); writeStream.WriteLine("\"" + strCommand + "\"");
        }

        internal void writeDataLine(StreamWriter writeStream, string strCommand, object strData, int nTabCount)
        {
            string strTab = string.Empty;

            // 오류 방지
            if (writeStream == null)
            {
                CNotice.printTraceID("TSWI");
                return;
            }

            // 오류 방지
            if (strData == null)
            {
                CNotice.printTraceID("TODI");
                return;
            }

            for (int i = 0; i < nTabCount; i++)
                strTab = strTab + "\t";

            writeStream.WriteLine(strTab + strCommand + "=" + strData.ToString());
        }

        #endregion

        #region --------------------------- 스크립트 저장용 함수들 ---------------------------

        /// <summary>
        /// 스크립트 작성을 새롭게 시작한다. (파일이 있는 경우라면 삭제하고 다시 생성한다)
        /// </summary>
        internal bool createScriptFileUsingString(string strOrgScriptContents,
                                                    string strNewScriptFileFullName,
                                                    List<string> strListCommand, char cAnnotation = '#')
        {
            int iChangeNumber;
            int firstCharIndex = -1, lastCharIndex = -1;

            string strLine, strTemp, strRemain, strTempNumber;

            List<int> listStartIndex = new List<int>();
            List<int> listEndIndex = new List<int>();

            try
            {
                // 생성할 스크립트 파일이 존재한다면 우선 삭제를 한다.
                if (true == m_manageFile.isExistFile(strNewScriptFileFullName))
                {
                    m_manageFile.deleteFile(strNewScriptFileFullName);
                }

                // 생성할 스크립트의 디렉토리가 존제하지 않으면 우선 디렉토리를 생성한다.
                if (false == m_manageFile.isExistDirectory(Path.GetDirectoryName(strNewScriptFileFullName)))
                {
                    m_manageFile.createDirectory(Path.GetDirectoryName(strNewScriptFileFullName));
                }

                StreamWriter writeFile = new StreamWriter(strNewScriptFileFullName);

                string[] arrayLine = strOrgScriptContents.Split('\n');

                for (int i = 0; i < arrayLine.Length; i++)
                {
                    strTemp = strLine = arrayLine[i];
                    strRemain = "";

                    listStartIndex.Clear();
                    listEndIndex.Clear();

                    // 치환 기호의 최소크기인 5 보다 크거나 같을 때만 동작한다.
                    if (strLine.Length >= 5)
                    {
                        // 주석이면 치환 작업을 하지 않고 삭제한다
                        if (strLine[0] != cAnnotation)
                        {
                            // 문자열안의 치환해야할 곳이 여러개가 있는 경우 모두 찾아내기 위해 while 을 사용한다.
                            // [ 유의사항 ]
                            //  - 저장되는 인덱스 값은 절대적인 위치가 아니라 앞에 치환기호가 없어진 상태에서의 인덱스 값이다
                            do
                            {
                                // 선택된 라인에서 치환 기호의 위치를 찾아낸다.
                                firstCharIndex = strTemp.IndexOf("{{");
                                if (firstCharIndex == -1)
                                    break;
                                listStartIndex.Add(firstCharIndex);

                                lastCharIndex = strTemp.IndexOf("}}");
                                if (lastCharIndex == -1)
                                    break;
                                listEndIndex.Add(lastCharIndex);

                                // 문자열 앞에서 부터 키워드까지 삭제하고 남은 부분에서 다시한번 키워드를 찾는다.
                                // 키워드가 '{{', '}}' 모두 2개 이기 때문에 2 를 더하였다.
                                strTemp = strTemp.Remove(0, lastCharIndex + 2);

                            } while (true);

                            // "{{" 은 있지만 "}}" 가 일치하지 않으면 경고를 하고 리턴을 한다.
                            if (listStartIndex.Count != listEndIndex.Count)
                            {
                                CNotice.printTrace("스크립트 치환에서 {{ 와 }} 의 갯수가 다릅니다.");
                                writeFile.Close();
                                return false;
                            }

                            strRemain = strLine;
                            strTemp = "";

                            for (int j = 0; j < listEndIndex.Count; j++)
                            {
                                firstCharIndex = listStartIndex[j];
                                lastCharIndex = listEndIndex[j];

                                // 치환기호가 순서대로 있는 경우만 동작시킨다 
                                if (firstCharIndex < lastCharIndex)
                                {
                                    // {{, }} 를 사용하기 때문에 2를 더하고, 2 를 빼고 있다
                                    strTempNumber = strRemain.Substring(firstCharIndex + 2, (lastCharIndex - firstCharIndex - 2));
                                    // 치환기호 사이의 숫자문자를 수로 바꾼다
                                    iChangeNumber = Int32.Parse(strTempNumber);

                                    // 만약 치환 숫자값이 문자열 배열의 갯수보다 크면 동작시키지 않고 오류를 알린 후 바로 리턴을 함
                                    if (iChangeNumber - 1 < strListCommand.Count)
                                    {
                                        // {{ }} 부분을 순서대로 교체한다.
                                        //
                                        // 순서는 아래와 같다.
                                        // - strLine (strRemain 에 담음) 에서 첫번째 치환기호 앞 문자열을 얻고, 치환 문자열을 더한다.
                                        // - 첫번문 치환부분이 제외된 strRemain 에서 
                                        //   다시 치환기호 앞 문자열을 잘라서 치환되는 문자열에 더하고, 치환 문자열도 더한다
                                        // - 상기 작업이 반복되고 더이상 치환 문자열이 없으면 남은 문자열은 strRemain 에 담긴상태로 for 문을 빠져 나간다.
                                        strTemp += strRemain.Remove(firstCharIndex);           // 치환시작 기호 앞부분을 얻어냄
                                        strTemp += strListCommand[iChangeNumber - 1];             // 치환할 문자열을 더함
                                        strRemain = strRemain.Remove(0, lastCharIndex + 2);    // 치환끝 기호 뒤부분을 얻어내어 더함
                                    }
                                    else
                                    {
                                        CNotice.printTrace("스크립트 치환에서 배열크기보다 큰 인덱스가 존재합니다.");
                                        writeFile.Close();
                                        return false;
                                    }
                                }
                            }

                            // 마지막 치환 문자열 뒤에 있는 남은 문자열을 더하고 치환을 마친다.
                            //
                            // 만약, 치환기호가 없는 경우는 strTemp 는 비어있고, strRemain 은 strLine 이기 때문에 이전의 strLine 이 출력된다.
                            strLine = strTemp + strRemain;

                            // 원본파일의 한라인 문자열을 생성파일에 한라인으로 쓴다
                            writeFile.WriteLine(strLine);
                        }
                    }
                    else if (strLine.Length > 0)
                    {
                        // 주석이면 치환 작업을 하지 않고 삭제한다
                        if (strLine[0] != cAnnotation)
                        {
                            // 첫줄이 Annotation 아닐때는 그대로 복사한다.
                            writeFile.WriteLine(strLine);
                        }
                    }
                    // 빈줄은 그대로 출력한다.
                    else
                    {
                        writeFile.WriteLine(strLine);
                    }
                }

                writeFile.Close();
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
                CNotice.printTrace("Script 파일 생성에서 예외가 발생하였다.");
            }

            return true;
        }

        /// <summary>
        /// 파일을 열어서 스크립트를 추가한다. (파일이 없는 경우라면 생성한다)
        /// </summary>
        internal bool addScriptFileUsingString( string strOrgScriptContents,
                                                string strTargetScriptFileFullName,
                                                List<string> strListCommand, char cAnnotation = '#')
        {
            int iChangeNumber;
            int firstCharIndex = -1, lastCharIndex = -1;

            string strLine, strTemp, strRemain, strTempNumber;

            List<int> listStartIndex = new List<int>();
            List<int> listEndIndex = new List<int>();

            try
            {
                StreamWriter writeFile;
                FileStream fileStream;

                // 스크립트 추가에서 목표 스크립트 파일이 존재하지 않는다면 스크립트 파일을 생성한다.
                if (m_manageFile.isExistFile(strTargetScriptFileFullName) == false)
                {
                    // 생성할 스크립트의 디렉토리가 존제하지 않으면 우선 디렉토리를 생성한다.
                    if (false == m_manageFile.isExistDirectory(Path.GetDirectoryName(strTargetScriptFileFullName)))
                    {
                        m_manageFile.createDirectory(Path.GetDirectoryName(strTargetScriptFileFullName));
                    }

                    // 파일을 생성모드로 오픈한다.
                    writeFile = new StreamWriter(strTargetScriptFileFullName);
                }
                else
                {
                    // 파일을 추가모드로 오픈한다.
                    fileStream = new FileStream(strTargetScriptFileFullName, FileMode.Append, FileAccess.Write);
                    writeFile = new StreamWriter(fileStream);
                }


                string[] arrayLine = strOrgScriptContents.Split('\n');

                for (int i = 0; i < arrayLine.Length; i++)
                {
                    strTemp = strLine = arrayLine[i];
                    strRemain = "";

                    listStartIndex.Clear();
                    listEndIndex.Clear();

                    // 치환 기호의 최소크기인 5 보다 크거나 같을 때만 동작한다.
                    if (strLine.Length >= 5)
                    {
                        // 주석이면 치환 작업을 하지 않고 삭제한다
                        if (strLine[0] != cAnnotation)
                        {
                            // 문자열안의 치환해야할 곳이 여러개가 있는 경우 모두 찾아내기 위해 while 을 사용한다.
                            // [ 유의사항 ]
                            //  - 저장되는 인덱스 값은 절대적인 위치가 아니라 앞에 치환기호가 없어진 상태에서의 인덱스 값이다
                            do
                            {
                                // 선택된 라인에서 치환 기호의 위치를 찾아낸다.
                                firstCharIndex = strTemp.IndexOf("{{");
                                if (firstCharIndex == -1)
                                    break;
                                listStartIndex.Add(firstCharIndex);

                                lastCharIndex = strTemp.IndexOf("}}");
                                if (lastCharIndex == -1)
                                    break;
                                listEndIndex.Add(lastCharIndex);

                                // 문자열 앞에서 부터 키워드까지 삭제하고 남은 부분에서 다시한번 키워드를 찾는다.
                                // 키워드가 '{{', '}}' 모두 2개 이기 때문에 2 를 더하였다.
                                strTemp = strTemp.Remove(0, lastCharIndex + 2);

                            } while (true);

                            // "{{" 은 있지만 "}}" 가 일치하지 않으면 경고를 하고 리턴을 한다.
                            if (listStartIndex.Count != listEndIndex.Count)
                            {
                                CNotice.printTrace("스크립트 치환에서 {{ 와 }} 의 갯수가 다릅니다.");
                                writeFile.Close();
                                return false;
                            }

                            strRemain = strLine;
                            strTemp = "";

                            for (int j = 0; j < listEndIndex.Count; j++)
                            {
                                firstCharIndex = listStartIndex[j];
                                lastCharIndex = listEndIndex[j];

                                // 치환기호가 순서대로 있는 경우만 동작시킨다 
                                if (firstCharIndex < lastCharIndex)
                                {
                                    // {{, }} 를 사용하기 때문에 2를 더하고, 2 를 빼고 있다
                                    strTempNumber = strRemain.Substring(firstCharIndex + 2, (lastCharIndex - firstCharIndex - 2));
                                    // 치환기호 사이의 숫자문자를 수로 바꾼다
                                    iChangeNumber = Int32.Parse(strTempNumber);

                                    // 만약 치환 숫자값이 문자열 배열의 갯수보다 크면 동작시키지 않고 오류를 알린 후 바로 리턴을 함
                                    if (iChangeNumber - 1 < strListCommand.Count)
                                    {
                                        // {{ }} 부분을 순서대로 교체한다.
                                        //
                                        // 순서는 아래와 같다.
                                        // - strLine (strRemain 에 담음) 에서 첫번째 치환기호 앞 문자열을 얻고, 치환 문자열을 더한다.
                                        // - 첫번문 치환부분이 제외된 strRemain 에서 
                                        //   다시 치환기호 앞 문자열을 잘라서 치환되는 문자열에 더하고, 치환 문자열도 더한다
                                        // - 상기 작업이 반복되고 더이상 치환 문자열이 없으면 남은 문자열은 strRemain 에 담긴상태로 for 문을 빠져 나간다.
                                        strTemp += strRemain.Remove(firstCharIndex);           // 치환시작 기호 앞부분을 얻어냄
                                        strTemp += strListCommand[iChangeNumber - 1];             // 치환할 문자열을 더함
                                        strRemain = strRemain.Remove(0, lastCharIndex + 2);    // 치환끝 기호 뒤부분을 얻어내어 더함
                                    }
                                    else
                                    {
                                        CNotice.printTrace("스크립트 치환에서 배열크기보다 큰 인덱스가 존재합니다.");
                                        writeFile.Close();
                                        return false;
                                    }
                                }
                            }

                            // 마지막 치환 문자열 뒤에 있는 남은 문자열을 더하고 치환을 마친다.
                            //
                            // 만약, 치환기호가 없는 경우는 strTemp 는 비어있고, strRemain 은 strLine 이기 때문에 이전의 strLine 이 출력된다.
                            strLine = strTemp + strRemain;

                            // 원본파일의 한라인 문자열을 생성파일에 한라인으로 쓴다
                            writeFile.WriteLine(strLine);
                        }
                    }
                    else if (strLine.Length > 0)
                    {
                        // 주석이면 치환 작업을 하지 않고 삭제한다
                        if (strLine[0] != cAnnotation)
                        {
                            // 첫줄이 Annotation 아닐때는 그대로 복사한다.
                            writeFile.WriteLine(strLine);
                        }
                    }
                    // 빈줄은 그대로 출력한다.
                    else
                    {
                        writeFile.WriteLine(strLine);
                    }
                }

                writeFile.Close();
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
                CNotice.printTrace("Script 파일 추가에서 예외가 발생하였다.");
            }

            return true;
        }


        #endregion
    }
    
    public class CReadFile
    {
        CManageFile m_manageFile = new CManageFile();

        #region --------------------------- 데이터파일 읽기 함수들 ---------------------------

        // 모든 라인을 List 에 담아서 리턴한다.
        public bool getAllLines(string strTargetFileFullName, ref List<string> listLines)
        {
            string[] arrayAllLine;

            try
            {
                if (false == m_manageFile.isExistFile(strTargetFileFullName))
                {
                    ResourceManager resManager = ResourceManager.CreateFileBasedResourceManager("LanguageResource", Application.StartupPath, null);

                    CNotice.printTrace(resManager.GetString("TIAA5") + strTargetFileFullName + resManager.GetString("_TDNE"));
                    return false;
                }

                arrayAllLine = File.ReadAllLines(strTargetFileFullName);

                // string Array 를 List 에 담는다.
                foreach (string strLine in arrayAllLine)
                {
                    listLines.Add(strLine);
                }
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
                return false;
            }

            return true;
        }

        // 특정라인을 뽑아낸다.
        public string getLine(string strTargetFileFullName, int iLineNum)
        {
            string[] arrayAllLine;
            int nLineCount = 0;

            try
            {
                if (false == m_manageFile.isExistFile(strTargetFileFullName))
                {
                    ResourceManager resManager = ResourceManager.CreateFileBasedResourceManager("LanguageResource", Application.StartupPath, null);

                    CNotice.printTrace(resManager.GetString("TIAA5") + strTargetFileFullName + resManager.GetString("_TDNE"));
                    return "";
                }

                arrayAllLine = File.ReadAllLines(strTargetFileFullName);

                // string Array 를 List 에 담는다.
                foreach (string strLine in arrayAllLine)
                {
                    nLineCount++;

                    if (nLineCount == iLineNum)
                        return strLine;
                }
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
            }

            // 원하는 라인보다 파일의 길이가 짧은 경우 "" 이 리턴된다.
            return "";
        }

        /// <summary>
        /// 특정 블럭안의 라인들만 List 에 담아서 리턴한다.
        /// </summary>
        /// <param name="strTargetFileFullName"></param>
        /// <param name="listLines"></param>
        /// <param name="strStartBlockKeyword"></param>
        /// <returns></returns>
        public bool readBlock(string strTargetFileFullName, ref List<string> listLines, string strStartBlockKeyword, string strEndBlockKeyword)
        {
            string[] arrayAllLine;
            string strTemp;
            bool bNodeData = false;

            // 이전에 사용하던 List 데이터를 우선 삭제한다.
            listLines.Clear();

            try
            {
                if (false == m_manageFile.isExistFile(strTargetFileFullName))
                {
                    ResourceManager resManager = ResourceManager.CreateFileBasedResourceManager("LanguageResource", Application.StartupPath, null);

                    CNotice.printTrace(resManager.GetString("TIAA5") + strTargetFileFullName + resManager.GetString("_TDNE"));
                    return false;
                }

                arrayAllLine = File.ReadAllLines(strTargetFileFullName);

                // string Array 를 List 에 담는다.
                foreach (string strLine in arrayAllLine)
                {
                    // 양쪽 공백 제거
                    strTemp = strLine.Trim();

                    if (strTemp == strStartBlockKeyword)
                    {
                        bNodeData = true;
                    }
                    else if (strTemp == strEndBlockKeyword)
                    {
                        bNodeData = false;
                    }                        
                    // Block Keyword 라인을 제외하기 위해서 else 일 경우만 저장한다.
                    else
                    {
                        if (bNodeData == true)
                        {
                            listLines.Add(strTemp);
                        }
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

        /// <summary>
        /// ' ' (space) 로 구분되는 파일에서 특정 행의 데이터 들을 List 에 담아서 리턴한다. 
        /// </summary>
        /// <param name="strFileFullName"></param>
        /// <param name="listLowData">행의 여러 데이터를 담는 리스트</param>
        /// <param name="iLowNumber">행의 번호 (1부터 ~ , 인덱스 아님)</param>
        /// <returns></returns>
        public bool readSpaceRowData2(string strFileFullName, ref List<double> listLowData, int iLowNumber)
        {
            string[] arrayAllLine;

            int nLineCount = 0;

            if (iLowNumber < 1)
            {
                CNotice.printTrace("Low Number have to be greater than 1.");
                return false;
            }

            // 이전에 사용하던 List 데이터를 우선 삭제한다.
            listLowData.Clear();

            try
            {
                if (false == m_manageFile.isExistFile(strFileFullName))
                {
                    ResourceManager resManager = ResourceManager.CreateFileBasedResourceManager("LanguageResource", Application.StartupPath, null);

                    CNotice.printTrace(resManager.GetString("TIAA5") + strFileFullName + resManager.GetString("_TDNE"));
                    return false;
                }

                arrayAllLine = File.ReadAllLines(strFileFullName);

                // string Array 를 List 에 담는다.
                foreach (string strLine in arrayAllLine)
                {
                    nLineCount++;

                    //특정 행을 제외한다. 주로 상단 제목줄을 제외할 때 사용한다.
                    if (nLineCount == iLowNumber)
                        CParsing.getDataInAllLine(strLine, ref listLowData, ' ');
                }
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// ',' 로 구분되는 CSV 파일에서 특정 행의 데이터 들을 List 에 담아서 리턴한다. 
        /// </summary>
        /// <param name="strCSVFileFullName"></param>
        /// <param name="listLowData">행의 여러 데이터를 담는 리스트</param>
        /// <param name="iLowNumber">행의 번호 (1부터 ~ , 인덱스 아님)</param>
        /// <returns></returns>
        public bool readCSVRowData2(string strCSVFileFullName, ref List<double> listLowData, int iLowNumber)
        {
            string[] arrayAllLine;
  
            int nLineCount = 0;

            if(iLowNumber < 1)
            {
                CNotice.printTrace("Low Number have to be greater than 1.");
                return false;
            }

            // 이전에 사용하던 List 데이터를 우선 삭제한다.
            listLowData.Clear();

            try
            {
                if (false == m_manageFile.isExistFile(strCSVFileFullName))
                {
                    ResourceManager resManager = ResourceManager.CreateFileBasedResourceManager("LanguageResource", Application.StartupPath, null);

                    CNotice.printTrace(resManager.GetString("TIAA5") + strCSVFileFullName + resManager.GetString("_TDNE"));
                    return false;
                }

                arrayAllLine = File.ReadAllLines(strCSVFileFullName);

                // string Array 를 List 에 담는다.
                foreach (string strLine in arrayAllLine)
                {
                    nLineCount++;

                    //특정 행을 제외한다. 주로 상단 제목줄을 제외할 때 사용한다.
                    if (nLineCount == iLowNumber)
                        CParsing.getDataInAllLine(strLine, ref listLowData, ',');
                }
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// ',' 로 구분되는 CSV 파일에서 특정 행의 데이터 들을 List 에 담아서 리턴한다. 
        /// </summary>
        /// <param name="strCSVFileFullName"></param>
        /// <param name="listLowString">행의 여러 문자열을 담는 리스트</param>
        /// <param name="iLowNumber">행의 번호 (1부터 ~ , 인덱스 아님)</param>
        /// <returns></returns>
        public bool readCSVRowString2(string strCSVFileFullName, ref List<string> listLowString, int iLowNumber)
        {
            string[] arrayAllLine;

            int nLineCount = 0;

            if (iLowNumber < 1)
            {
                CNotice.printTrace("Low Number have to be greater than 1.");
                return false;
            }

            // 이전에 사용하던 List 데이터를 우선 삭제한다.
            listLowString.Clear();

            try
            {
                if (false == m_manageFile.isExistFile(strCSVFileFullName))
                {
                    ResourceManager resManager = ResourceManager.CreateFileBasedResourceManager("LanguageResource", Application.StartupPath, null);

                    CNotice.printTrace(resManager.GetString("TIAA5") + strCSVFileFullName + resManager.GetString("_TDNE"));
                    return false;
                }

                arrayAllLine = File.ReadAllLines(strCSVFileFullName);

                // string Array 를 List 에 담는다.
                foreach (string strLine in arrayAllLine)
                {
                    nLineCount++;

                    //특정 행을 제외한다. 주로 상단 제목줄을 제외할 때 사용한다.
                    if (nLineCount == iLowNumber)
                        CParsing.getStringInAllLine(strLine, ref listLowString, ',');
                }
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// ',' 로 구분되는 CSV 파일에서 특정 세로열의 데이터 들을 List 에 담아서 리턴한다. 
        /// </summary>
        /// <param name="strCSVFileFullName"></param>
        /// <param name="listColumnData">열의 여러 데이터를 담는 리스트</param>
        /// <param name="iColumnNumber">열의 번호 (1부터 ~ , 인덱스 아님)</param>
        /// <param name="iExceptionRowNumber">특정 행 제외 (1부터 ~ , 인덱스 아님, 주로 상단 제목줄을 제외할 때 사용함).</param>
        /// <returns></returns>
        public bool readCSVColumnData2(string strCSVFileFullName, ref List<double> listColumnData, int iColumnNumber, int iExceptionRowNumber = -1)
        {
            string[] arrayAllLine;
            string[] arraySeperatedLine;

            int nLineCount = 0;

            if (iColumnNumber < 1)
            {
                CNotice.printTrace("Column Number have to be greater than 1.");
                return false;
            }

            // 이전에 사용하던 List 데이터를 우선 삭제한다.
            listColumnData.Clear();

            try
            {
                if (false == m_manageFile.isExistFile(strCSVFileFullName))
                {
                    ResourceManager resManager = ResourceManager.CreateFileBasedResourceManager("LanguageResource", Application.StartupPath, null);

                    CNotice.printTrace(resManager.GetString("TIAA5") + strCSVFileFullName + resManager.GetString("_TDNE"));
                    return false;
                }

                arrayAllLine = File.ReadAllLines(strCSVFileFullName);

                // string Array 를 List 에 담는다.
                foreach (string strLine in arrayAllLine)
                {
                    nLineCount++;

                    //특정 행을 제외한다. 주로 상단 제목줄을 제외할 때 사용한다.
                    if (nLineCount != iExceptionRowNumber)
                    {
                        arraySeperatedLine = strLine.Split(',');

                        // iColumnNumber 가 인덱스가 아니고 번호이기 때문에 <= 를 사용하지 않는다.
                        if (arraySeperatedLine.Length < iColumnNumber)
                        {
                            //==========================================================
                            // 추후 Resource 파일을 수정하라. (index -> number)
                            //==========================================================
                            CNotice.printTrace("Column Number is greater than column data size.");
                            //CNotice.printTraceID("TCII");
                            return false;
                        }

                        // iColumnNumber 가 인덱스가 아니고 번호이기 때문에 -1 을 사용한다.
                        listColumnData.Add(Convert.ToDouble(arraySeperatedLine[iColumnNumber - 1]));
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

        /// <summary>
        /// ',' 로 구분되는 CSV 파일에서 특정 세로열의 문자열 들을 List 에 담아서 리턴한다. 
        /// </summary>
        /// <param name="strCSVFileFullName">파일명</param>
        /// <param name="listColumnString">열의 여러 문자열을 담는 리스트</param>
        /// <param name="iColumnNumber">열의 번호 (1부터 ~ , 인덱스 아님)</param>
        /// <param name="iExceptionRowNumber">특정 행 제외 (1부터 ~ , 인덱스 아님, 주로 상단 제목줄을 제외할 때 사용함).</param>
        /// <returns></returns>
        public bool readCSVColumnString2(string strCSVFileFullName, ref List<string> listColumnString, int iColumnNumber, int iExceptionRowNumber = -1)
        {
            string[] arrayAllLine;
            string[] arraySeperatedLine;

            int nLineCount = 0;

            if (iColumnNumber < 1)
            {
                CNotice.printTrace("Column Number have to be greater than 1.");
                return false;
            }

            // 이전에 사용하던 List 데이터를 우선 삭제한다.
            listColumnString.Clear();

            try
            {
                if (false == m_manageFile.isExistFile(strCSVFileFullName))
                {
                    ResourceManager resManager = ResourceManager.CreateFileBasedResourceManager("LanguageResource", Application.StartupPath, null);

                    CNotice.printTrace(resManager.GetString("TIAA5") + strCSVFileFullName + resManager.GetString("_TDNE"));
                    return false;
                }

                arrayAllLine = File.ReadAllLines(strCSVFileFullName);

                // string Array 를 List 에 담는다.
                foreach (string strLine in arrayAllLine)
                {
                    nLineCount++;

                    //특정 행을 제외한다. 주로 상단 제목줄을 제외할 때 사용한다.
                    if (nLineCount != iExceptionRowNumber)
                    {
                        arraySeperatedLine = strLine.Split(',');

                        // iColumnNumber 가 인덱스가 아니고 번호이기 때문에 <= 를 사용하지 않는다.
                        if (arraySeperatedLine.Length < iColumnNumber)
                        {
                            //==========================================================
                            // 추후 Resource 파일을 수정하라. (index -> number)
                            //==========================================================
                            CNotice.printTrace("Column Number is greater than column data size.");
                            //CNotice.printTraceID("TCII");
                            return false;
                        }

                        // iColumnNumber 가 인덱스가 아니고 번호이기 때문에 -1 을 사용한다.
                        listColumnString.Add(arraySeperatedLine[iColumnNumber - 1]);
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

        // 해당 키워드를 만났을 때 
        // 해당 라인의 startPos 부터 endPos 사이의 문자열을 숫자로 만들어서 리턴한다.
        //
        // 키워드를 만나면 바로 리턴하기 때문에 해당 키워드만 읽어낸다.
        public string pickoutString(string strTargetFileFullName, string strKeyword, int startPos, int endPos)
        {
            string strLine, strTemp;
            int iLength;

            try
            {
                if (File.Exists(strTargetFileFullName) == false)
                    return null;

                if (startPos >= endPos)
                    CNotice.printTrace("The StartPos is greater then the EndPos in the GetData");

                System.IO.StreamReader readFile = new System.IO.StreamReader(strTargetFileFullName);

                while ((strLine = readFile.ReadLine()) != null)
                {
                    // Keyword 나 시작 Pos 보다는 strLine이 커야한다
                    if (strLine.Length > strKeyword.Length && strLine.Length > startPos)
                    {
                        strTemp = strLine.Substring(0, strKeyword.Length);

                        // 키워드를 만나면 바로 리턴하기 때문에 해당 키워드만 읽어낸다.
                        if (strTemp == strKeyword)
                        {
                            // 혹시 strLine 의 길이가 endPos 보다 작다면 
                            // 리턴하지 않고 강제로 endPos 크기를 strLine 길이로 변경한다
                            if (strLine.Length < endPos)
                                endPos = strLine.Length;

                            iLength = endPos - startPos;

                            strTemp = strLine.Substring(startPos, iLength);

                            readFile.Close();

                            return strTemp;
                        }
                    }
                }

                readFile.Close();
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
            }

            return null;
        }

        #endregion

        #region--------------------------- 작업파일 읽기용 함수들 ---------------------------

        // 구분의 시작행인 $begin 의 구분 명령어를 얻어온다.
        internal string isBeginLine(string strLine)
        {
            // 구분 Tab 을 삭제한다.
            string strTemp = strLine.Trim();
            string[] arraySplit;

            // 오류 방지
            if (strTemp.Length == 0)
                return null;

            if (strTemp[0] == '$')
            {
                arraySplit = strTemp.Split('\"');

                // 오류 방지
                if (arraySplit.Length <= 1)
                    return null;

                if (arraySplit[0] == "$begin ")
                    return arraySplit[1];
                else
                    return null;
            }
            else
                return null;
        }

        // 구분의 끝행인 $end 의 구분 명령어를 얻어온다.
        internal string isEndLine(string strLine)
        {
            // 구분 Tab 을 삭제한다.
            string strTemp = strLine.Trim();
            string[] arraySplit;

            // 오류 방지
            if (strTemp.Length == 0)
                return null;

            if (strTemp[0] == '$')
            {
                arraySplit = strTemp.Split('\"');

                // 오류 방지
                if (arraySplit.Length <= 1)
                    return null;

                if (arraySplit[0] == "$end ")
                    return arraySplit[1];
                else
                    return null;
            }
            else
                return null;
        }

        /// <summary>
        /// 라인 안에 "변수=데이터" 형식으로 데이터가 존재 할 때 데이터를 얻어온다
        /// </summary>
        /// <param name="strLine"></param>
        /// <param name="strCommand"></param>
        /// <param name="strData"></param>
        /// <returns></returns>
        public bool readDataInLine(string strLine, ref string strCommand, ref string strData)
        {
            // 구분 Tab 을 삭제한다.
            string strTemp = strLine.Trim();
            string[] arraySplit;

            // 오류 방지
            if (strTemp.Length == 0)
                return false;

            // 데이터라인이 아닌 경우 바로 리턴한다
            if (strTemp[0] == '$')
                return false;

            arraySplit = strTemp.Split('=');

            // 오류 방지
            if (arraySplit.Length <= 1)
                return false;

            strCommand = arraySplit[0];
            strData = arraySplit[1];

            return true;
        }

        #endregion

        #region -------------------------- 재질파일 읽기용 함수들 ----------------------------

        //Marerial 데이터를 읽어온다
        public bool readMaterialMagnetData(string strFileFullName, string strMaterialName, ref double dMu, ref double dHc)
        {
            List<string> listString = new List<string>();

            string strLine, strName, strTemp, strData;
            char[] separetor = {'='};
            int nIndex;

            try
            {
                if (false == m_manageFile.isExistFile(strFileFullName))
                {
                    CNotice.printTraceID("TMFD");
                    return false;
                }

                List<string> listMaterialNames = new List<string>();
       
                readMaterialNames(strFileFullName, ref listMaterialNames);


                if (listMaterialNames.Contains(strMaterialName) == false)
                {
                    CNotice.printTrace("존재하지 않은 영구자석의 특성값을 얻으려고 하고 있다.");
                    return false;
                }

                getAllLines(strFileFullName, ref listString);

                // 다음 행을 사용하는 경우도 있어서 foreach 를 사용하지 않았다.
                for (int i = 0; i < listString.Count; i++)
                {
                    strLine = listString[i];

                    // keyword 앞을 \t 를 제거한다.
                    strLine = strLine.Trim();

                    if (strLine == "$begin \'MaterialDef\'")
                    {
                        strName = listString[i + 1];

                        strName = strName.Trim();

                        // 이름 얻기
                        nIndex = strName.IndexOf("'");
                        strTemp = strName.Substring(nIndex + 1);
                        nIndex = strTemp.IndexOf("'");
                        strTemp = strTemp.Remove(nIndex);

                        // BH 곡선 수집을 시작한다.
                        if (strTemp == strMaterialName)
                        {
                            strData = listString[i + 2];
                            CParsing.getDataInLine(strData, ref dMu, '=', 2);

                            strData = listString[i + 3];
                            CParsing.getDataInLine(strData, ref dHc, '=', 2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
                CNotice.printTraceID("AETT");
                return false;
            }

            return true;
        }


        //Marerial 데이터를 읽어온다
        public bool readMaterialBHData(string strFileFullName, string strMaterialName, ref List<double> listH, ref List<double> listB)
        {
            List<string> listString = new List<string>();

            string strLine, strName, strData, strTemp;
            int nIndex;
            bool bBHDataGathering = false;

            // 이전에 사용하던 List 데이터를 우선 삭제한다.
            listH.Clear();
            listB.Clear();

            try
            {
                if (false == m_manageFile.isExistFile(strFileFullName))
                {
                    CNotice.printTraceID("TMFD");
                    return false;
                }

                List<string> listMaterialNames = new List<string>();

                readMaterialNames(strFileFullName, ref listMaterialNames);

                if (listMaterialNames.Contains(strMaterialName) == false)
                {
                    CNotice.printTrace("존재하지 않은 연자성체의 BH 값을 얻으려고 하고 있다.");
                    return false;
                }

                getAllLines(strFileFullName, ref listString);

                // 다음 행을 사용하는 경우도 있어서 foreach 를 사용하지 않았다.
                for (int i = 0; i < listString.Count; i++)
                {
                    strLine = listString[i];

                    // keyword 앞을 \t 를 제거한다.
                    strLine = strLine.Trim();

                    if (strLine == "$begin \'MaterialDef\'")
                    {
                        strName = listString[i + 1];

                        strName = strName.Trim();

                        // 이름 얻기
                        nIndex = strName.IndexOf("'");
                        strTemp = strName.Substring(nIndex + 1);
                        nIndex = strTemp.IndexOf("'");
                        strTemp = strTemp.Remove(nIndex);

                        // BH 곡선 수집을 시작한다.
                        if (strTemp == strMaterialName)
                            bBHDataGathering = true;
                    }

                    // BH 곡선 수집을 종료한다.
                    if (strLine == "$end \'MaterialDef\'")
                        bBHDataGathering = false;

                    if (strLine == "$begin \'Coordinate\'" && bBHDataGathering == true)
                    {
                        // H 값 읽기
                        strData = listString[i + 1];
                        strTemp = strData.Trim();       // 앞의 공백 제거함    
                        strTemp = strTemp.Substring(2); // X= 제거함
                        listH.Add(Convert.ToDouble(strTemp));

                        // B 값 읽기
                        strData = listString[i + 2];
                        strTemp = strData.Trim();       // 앞의 공백 제거함
                        strTemp = strTemp.Substring(2); // Y= 제거함
                        listB.Add(Convert.ToDouble(strTemp));

                        // 하나의 BH 곡선을 읽고 나면
                        // 아래의 X, Y, $end 3 line 은 그냥 점프 한다.
                        i = i + 3;
                    }
                }
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
                CNotice.printTraceID("AETT");
                return false;
            }

            return true;
        }

        // Marerial 데이터를 읽어온다
        public bool readMaterialNames(string strFileFullName, ref List<string> listMaterialNames)
        {
            List<string> listAllLines = new List<string>();

            string strLine, strName, strTemp;
            int nIndex;

            // 이전에 사용하던 List 데이터를 우선 삭제한다.
            listMaterialNames.Clear();

            if (false == m_manageFile.isExistFile(strFileFullName))
            {
                CNotice.printTraceID("TMFD");
                return false;
            }

            try
            {
                getAllLines(strFileFullName, ref listAllLines);

                // 다음 행을 사용하는 경우도 있어서 foreach 를 사용하지 않았다.
                for (int i = 0; i < listAllLines.Count; i++)
                {
                    strLine = listAllLines[i];

                    // keyword 앞을 \t 를 제거한다.
                    strLine = strLine.Trim();

                    if (strLine == "$begin \'MaterialDef\'")
                    {
                        strName = listAllLines[i + 1];

                        strName = strName.Trim();

                        // 이름 얻기
                        nIndex = strName.IndexOf("'");
                        strTemp = strName.Substring(nIndex + 1);
                        nIndex = strTemp.IndexOf("'");
                        strTemp = strTemp.Remove(nIndex);

                        listMaterialNames.Add(strTemp);
                    }
                }
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
                CNotice.printTraceID("AETT");
                return false;
            }

            return true;
        }


        #endregion

    }

    public class CParsing
    {
        #region ------------------------------- Line Parsing --------------------------------

        /// <summary>
        /// 문자열을 특정 기호로 분리하여 숫자 List 에 담는다.
        /// </summary>
        /// <param name="strLine"></param>
        /// <param name="listData"></param>
        /// <param name="seperators"></param>
        /// <returns></returns>
        public static bool getDataInAllLine(string strLine, ref List<double> listData, char cSeperator)
        {
            double dData = 0;
            string[] strArray;
            char[] sperators = { cSeperator };
            string strTemp;

            if (strLine.Length == 0)
                return false;
            
            try
            {
                strArray = strLine.Split(sperators, StringSplitOptions.None);

                for(int i=0; i<strArray.Length; i++)
                {
                    // 양단의 Space 기호는 제외한다.
                    strTemp = strArray[i].Trim();

                    if(strTemp != string.Empty)
                    {
                        if (true == double.TryParse(strTemp, out dData))
                            listData.Add(dData);
                        else
                            CNotice.printTrace("숫자 문자열이 아닌 문자열을 숫자로 변환하려고 한다.");
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

        public static bool getDataInLine(string strLine, ref double dData, char cSeperator, int nCount)
        {
            string[] strArray;
            char[] sperators = { cSeperator };
            string strData;

            if (strLine.Length == 0)
                return false;

            try
            {
                strArray = strLine.Split(sperators, StringSplitOptions.None);

                if (strArray.Length < nCount)
                    return false;

                strData = strArray[nCount-1].Trim();

                if (false == double.TryParse(strData, out dData))
                {
                    CNotice.printTrace("숫자 문자열이 아닌 문자열을 숫자로 변환하려고 한다.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 문자열을 특정 기호로 분리하여 문자 List 에 담는다.
        /// </summary>
        /// <param name="strLine"></param>
        /// <param name="listString"></param>
        /// <param name="seperators"></param>
        /// <returns></returns>
        public static bool getStringInAllLine(string strLine, ref List<string> listString, char cSeperator)
        {
            string[] strArray;
            char[] sperators = { cSeperator };
            string strTemp;

            if (strLine.Length == 0)
                return false;

            try
            {
                strArray = strLine.Split(sperators, StringSplitOptions.None);

                for (int i = 0; i < strArray.Length; i++)
                {
                    strTemp = strArray[i].Trim();

                    listString.Add(strTemp);
                }
            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 문자열을 특정 기호로 분리하고, 특정칸의 데이터를 리턴한다.
        /// </summary>
        /// <param name="strLine"></param>
        /// <param name="cSeperator"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        public static bool getStringInLine(string strLine, ref string str, char cSeperator, int nCount)
        {
            string[] strArray;
            char[] sperators = { cSeperator };

            if (strLine.Length == 0)
                return false;

            try
            {
                strArray = strLine.Split(sperators, StringSplitOptions.None);

                if(strArray.Length < nCount)
                    return false;

                str = strArray[nCount-1].Trim();

            }
            catch (Exception ex)
            {
                CNotice.printTrace(ex.Message);
                return false;
            }

            return true;
        }

        #endregion
    }
}
