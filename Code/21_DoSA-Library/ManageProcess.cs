using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Threading;

using System.Diagnostics;


namespace gtLibrary
{
    public static class CManageProcess
    {

        /// <summary>
        /// - 프로세스의 갯수를 확인한다.
        /// - Window Name 이 아니라 Process Name 을 파라메터로 넘겨야 한다.
        /// </summary>
        /// <param name="strProcessName"></param>
        public static int getProcessesCount(string strProcessName)
        {
            try
            {
                Process[] processList = Process.GetProcessesByName(strProcessName);

                return processList.Length;

            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);

                return 0;
            }
        }

        /// <summary>
        /// - 메모리에 하나라도 해당 이름의 Process 가 동작하고 있는지 확인한다.
        /// </summary>
        /// <param name="strProcessName"></param>  
        public static bool isRunProcesses(string strProcessName)
        {
            try
            {
                Process[] processList = Process.GetProcessesByName(strProcessName);

                if (processList.Length == 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);

                return false;
            }
        }

        /// <summary>
        /// - 동일 이름의 Process 를 메모리에서 모두 삭제한다.
        /// </summary>
        /// <param name="strProcessName"></param>
        public static void killProcesses(string strProcessName, int idException = 0)
        {
            try
            {
                int nCount = 0;
                int nLimitProcessCount;
                int indexProcess = 0;

                // 예외 프로세스가 있는 경우는 한개의 프로세스는 남긴다.
                if (idException == 0)
                    nLimitProcessCount = 0;
                else
                    nLimitProcessCount = 1;

                Process[] processList = null;

                do
                {
                    processList = Process.GetProcessesByName(strProcessName);

                    if (processList.Length > 0)
                    {
                        // 예외 프로세스가 있는 경우는 예외 프로세스를 제외한다.
                        if (idException == 0)
                            processList[indexProcess].Kill();
                        else
                        {
                            if (processList[indexProcess].Id == idException)
                            {
                                // 예외 프로세스는 0번 인덱스로 남겨두고 다음 인텍스들을 삭제하기 위해 1로 변경한다.
                                indexProcess = 1;
                            }
                            else
                            {
                                processList[indexProcess].Kill();
                            }
                        }
                    }

                    // 프로세스가 사라지는 시간을 확보한다.
                    // 대기 시간이 짧으면 예외 발생할 수 있다.
                    Thread.Sleep(300);

                    // 동일명의 프로세스가 20 개 이하로 가정한다.
                    if (nCount > 20)
                        return;

                    nCount++;

                    // 프로세스를 Kill 할때 processList.Length 가 바로 변경되지 않아서 while 비교전에 다시 processList 를 얻어온다
                    // 중복 호출되는 아쉬움이 있다.
                    processList = Process.GetProcessesByName(strProcessName);

                } while (processList.Length > nLimitProcessCount);
            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);

                return;
            }
        }
    }
}
