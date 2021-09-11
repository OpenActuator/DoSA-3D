using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

// Caller Information
using System.Runtime.CompilerServices;

using System.IO;
using System.Resources;

namespace gtLibrary
{
    public enum EMOutputTarget
    {
        TRACE,
        MESSAGE_VIEW
    }

    public class CNotice
    {
        public delegate void LogEventHandler(EMOutputTarget target, string strMSG);
        public static event LogEventHandler Notice;

        public static void printTraceID(string strID,
                [CallerMemberName] string functionName = "",
                [CallerFilePath] string sourceFilePath = "",
                [CallerLineNumber] int lineNumber = 0)
        {
            if (Notice != null)
            {
                try
                {
                    ResourceManager resManager = ResourceManager.CreateFileBasedResourceManager("LanguageResource", Application.StartupPath, null);
                    string strMSG = resManager.GetString(strID);

                    string fileName = Path.GetFileName(sourceFilePath);
                    strMSG = fileName + ", " + lineNumber + ", " + functionName + " : " + strMSG;

                    Notice(EMOutputTarget.TRACE, strMSG);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("There are no Language resource files.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    CNotice.printTrace(ex.Message);
                }
            }
        }

        public static void printTrace(string strMSG,
                [CallerMemberName] string functionName = "",
                [CallerFilePath] string sourceFilePath = "",
                [CallerLineNumber] int lineNumber = 0)
        {
            if (Notice != null)
            {
                string fileName = Path.GetFileName(sourceFilePath);
                strMSG = fileName + ", " + lineNumber + ", " + functionName + " : " + strMSG;

                Notice(EMOutputTarget.TRACE, strMSG);
            }
        }

        public static void printUserMessage(string strMSG)
        {
            if (Notice != null)
            {
                Notice(EMOutputTarget.MESSAGE_VIEW, strMSG);
            }
        }

        public static void noticeInfomation(string strMSG, string strTitle)
        {
            MessageBox.Show(strMSG, strTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void noticeWarningID(string strID)
        {
            try
            {
                ResourceManager resManager = ResourceManager.CreateFileBasedResourceManager("LanguageResource", Application.StartupPath, null);
                string strTitle = resManager.GetString("W");
                string strMSG = resManager.GetString(strID);

                // DataSet 에 \n 이 들어가서 \\n 이 되기 때문에 다시 복원해야 개행이 된다.
                strMSG = strMSG.Replace("\\n", "\n");

                MessageBox.Show(strMSG, strTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There are no Language resource files.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                CNotice.printTrace(ex.Message);
            }
        }

        public static void noticeWarning(string strMSG)
        {
            try
            {
                ResourceManager resManager = ResourceManager.CreateFileBasedResourceManager("LanguageResource", Application.StartupPath, null);
                string strTitle = resManager.GetString("W");

                MessageBox.Show(strMSG, strTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There are no Language resource files.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                CNotice.printTrace(ex.Message);
            }
        }

        public static void noticeError(string strMSG)
        {
            try
            {
                ResourceManager resManager = ResourceManager.CreateFileBasedResourceManager("LanguageResource", Application.StartupPath, null);
                string strTitle = resManager.GetString("E");

                MessageBox.Show(strMSG, strTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There are no Language resource files.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                CNotice.printTrace(ex.Message);
            }
        }

        public static DialogResult noticeWarningOKCancelID(string strID, string strTitleID)
        {
            try
            {
                ResourceManager resManager = ResourceManager.CreateFileBasedResourceManager("LanguageResource", Application.StartupPath, null);
                string strTitle = resManager.GetString(strTitleID);
                string strMSG = resManager.GetString(strID);
            
                // DataSet 에 \n 이 들어가서 \\n 이 되기 때문에 다시 복원해야 개행이 된다.
                strMSG = strMSG.Replace("\\n", "\n");

                return MessageBox.Show(strMSG, strTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There are no Language resource files.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                CNotice.printTrace(ex.Message);

                return DialogResult.Cancel;
            }
        }

        public static DialogResult noticeWarningOKCancel(string strMSG, string strTitle)
        {
            return MessageBox.Show(strMSG, strTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
        }

        public static DialogResult noticeWarningYesNoCancelID(string strID, string strTitleID)
        {
            try
            {
                ResourceManager resManager = ResourceManager.CreateFileBasedResourceManager("LanguageResource", Application.StartupPath, null);
                string strTitle = resManager.GetString(strTitleID);
                string strMSG = resManager.GetString(strID);

                // DataSet 에 \n 이 들어가서 \\n 이 되기 때문에 다시 복원해야 개행이 된다.
                strMSG = strMSG.Replace("\\n", "\n");

                return MessageBox.Show(strMSG, strTitle, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There are no Language resource files.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                CNotice.printTrace(ex.Message);

                return DialogResult.Cancel;
            }
        }

        public static DialogResult noticeWarningYesNoCancel(string strMSG, string strTitle)
        {
            return MessageBox.Show(strMSG, strTitle, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
        }

        public static DialogResult noticeWarningYesNo(string strMSG, string strTitle = "Warning")
        {
            return MessageBox.Show(strMSG, strTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        }
    }
}
