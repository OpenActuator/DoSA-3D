﻿namespace DoSA
{
    partial class PopupNewVersion
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBoxNotice = new System.Windows.Forms.GroupBox();
            this.labelMainUpdateContents = new System.Windows.Forms.Label();
            this.labelMainUpdate = new System.Windows.Forms.Label();
            this.labelProductVersionDisplay = new System.Windows.Forms.Label();
            this.labelProductVersion = new System.Windows.Forms.Label();
            this.buttonStopNotice = new System.Windows.Forms.Button();
            this.buttonOpenWebSite = new System.Windows.Forms.Button();
            this.labelNewVersionDisplay = new System.Windows.Forms.Label();
            this.labelNewVerstion = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.groupBoxNotice.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxNotice
            // 
            this.groupBoxNotice.Controls.Add(this.labelMainUpdateContents);
            this.groupBoxNotice.Controls.Add(this.labelMainUpdate);
            this.groupBoxNotice.Controls.Add(this.buttonStopNotice);
            this.groupBoxNotice.Controls.Add(this.labelProductVersionDisplay);
            this.groupBoxNotice.Controls.Add(this.labelProductVersion);
            this.groupBoxNotice.Controls.Add(this.buttonOpenWebSite);
            this.groupBoxNotice.Controls.Add(this.labelNewVersionDisplay);
            this.groupBoxNotice.Controls.Add(this.labelNewVerstion);
            this.groupBoxNotice.Location = new System.Drawing.Point(13, 12);
            this.groupBoxNotice.Name = "groupBoxNotice";
            this.groupBoxNotice.Size = new System.Drawing.Size(500, 170);
            this.groupBoxNotice.TabIndex = 1;
            this.groupBoxNotice.TabStop = false;
            this.groupBoxNotice.Text = "Notice";
            // 
            // labelMainUpdateContents
            // 
            this.labelMainUpdateContents.AutoSize = true;
            this.labelMainUpdateContents.Location = new System.Drawing.Point(125, 84);
            this.labelMainUpdateContents.Name = "labelMainUpdateContents";
            this.labelMainUpdateContents.Size = new System.Drawing.Size(361, 12);
            this.labelMainUpdateContents.TabIndex = 9;
            this.labelMainUpdateContents.Text = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcd";
            // 
            // labelMainUpdate
            // 
            this.labelMainUpdate.AutoSize = true;
            this.labelMainUpdate.Location = new System.Drawing.Point(26, 84);
            this.labelMainUpdate.Name = "labelMainUpdate";
            this.labelMainUpdate.Size = new System.Drawing.Size(98, 12);
            this.labelMainUpdate.TabIndex = 8;
            this.labelMainUpdate.Text = "- Main Update : ";
            // 
            // labelProductVersionDisplay
            // 
            this.labelProductVersionDisplay.AutoSize = true;
            this.labelProductVersionDisplay.Location = new System.Drawing.Point(153, 56);
            this.labelProductVersionDisplay.Name = "labelProductVersionDisplay";
            this.labelProductVersionDisplay.Size = new System.Drawing.Size(41, 12);
            this.labelProductVersionDisplay.TabIndex = 5;
            this.labelProductVersionDisplay.Text = "0.1.0.0";
            // 
            // labelProductVersion
            // 
            this.labelProductVersion.AutoSize = true;
            this.labelProductVersion.Location = new System.Drawing.Point(26, 56);
            this.labelProductVersion.Name = "labelProductVersion";
            this.labelProductVersion.Size = new System.Drawing.Size(127, 12);
            this.labelProductVersion.TabIndex = 5;
            this.labelProductVersion.Text = "- Your Version :  Ver.";
            // 
            // buttonStopNotice
            // 
            this.buttonStopNotice.Location = new System.Drawing.Point(257, 116);
            this.buttonStopNotice.Name = "buttonStopNotice";
            this.buttonStopNotice.Size = new System.Drawing.Size(226, 38);
            this.buttonStopNotice.TabIndex = 4;
            this.buttonStopNotice.Text = "Stop the new version notice";
            this.buttonStopNotice.UseVisualStyleBackColor = true;
            this.buttonStopNotice.Click += new System.EventHandler(this.buttonStopNotice_Click);
            // 
            // buttonOpenWebSite
            // 
            this.buttonOpenWebSite.Location = new System.Drawing.Point(19, 116);
            this.buttonOpenWebSite.Name = "buttonOpenWebSite";
            this.buttonOpenWebSite.Size = new System.Drawing.Size(226, 38);
            this.buttonOpenWebSite.TabIndex = 3;
            this.buttonOpenWebSite.Text = "Move to the download website";
            this.buttonOpenWebSite.UseVisualStyleBackColor = true;
            this.buttonOpenWebSite.Click += new System.EventHandler(this.buttonOpenWebSite_Click);
            // 
            // labelNewVersionDisplay
            // 
            this.labelNewVersionDisplay.AutoSize = true;
            this.labelNewVersionDisplay.Location = new System.Drawing.Point(153, 28);
            this.labelNewVersionDisplay.Name = "labelNewVersionDisplay";
            this.labelNewVersionDisplay.Size = new System.Drawing.Size(41, 12);
            this.labelNewVersionDisplay.TabIndex = 1;
            this.labelNewVersionDisplay.Text = "0.1.0.0";
            // 
            // labelNewVerstion
            // 
            this.labelNewVerstion.AutoSize = true;
            this.labelNewVerstion.Location = new System.Drawing.Point(26, 28);
            this.labelNewVerstion.Name = "labelNewVerstion";
            this.labelNewVerstion.Size = new System.Drawing.Size(127, 12);
            this.labelNewVerstion.TabIndex = 1;
            this.labelNewVerstion.Text = "- New Version :  Ver.";
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(402, 198);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(110, 31);
            this.buttonClose.TabIndex = 5;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // PopupNewVersion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(524, 241);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.groupBoxNotice);
            this.Name = "PopupNewVersion";
            this.Text = "New Version Notice";
            this.groupBoxNotice.ResumeLayout(false);
            this.groupBoxNotice.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxNotice;
        private System.Windows.Forms.Button buttonStopNotice;
        private System.Windows.Forms.Button buttonOpenWebSite;
        private System.Windows.Forms.Label labelNewVerstion;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label labelProductVersionDisplay;
        private System.Windows.Forms.Label labelProductVersion;
        private System.Windows.Forms.Label labelNewVersionDisplay;
        private System.Windows.Forms.Label labelMainUpdateContents;
        private System.Windows.Forms.Label labelMainUpdate;
    }
}