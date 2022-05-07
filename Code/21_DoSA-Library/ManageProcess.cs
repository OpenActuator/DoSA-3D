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
    }
}
