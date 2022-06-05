using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace gtLibrary
{
    public class MessageListView : ListView
    {
        const int iLimitLine = 200;
        
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public MessageListView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitForm call
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion

        public void addMessage(string message)
        {
            message = ">> " + message;

            try
            {
                ListViewItem item = new ListViewItem(message);

                this.Items.Add(item);

                if (this.Items.Count > iLimitLine)
                {
                    this.Items.Remove(this.Items[0]);
                }

                // listview 의 View 속성을 Detail 로 하고 하나의 해더를 추가해야 한다
                this.TopItem = this.Items[this.Items.Count - 1];
            }
            catch (Exception ex)
            {
                CNotice.printLog(ex.Message);
            }
        }

        public void clearMessage()
        {
            this.Items.Clear();
        }
    }
}