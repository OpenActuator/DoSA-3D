namespace DoSA
{
    partial class PopupSetting
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
            this.groupBoxSetting = new System.Windows.Forms.GroupBox();
            this.textBoxGmshPath = new System.Windows.Forms.TextBox();
            this.buttonSelectGmshPath = new System.Windows.Forms.Button();
            this.labelGmshPath = new System.Windows.Forms.Label();
            this.textBoxWorkingDirectory = new System.Windows.Forms.TextBox();
            this.buttonSelectWorkingDirectory = new System.Windows.Forms.Button();
            this.labelWorkingDirectory = new System.Windows.Forms.Label();
            this.buttonSettingOK = new System.Windows.Forms.Button();
            this.buttonSettingCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxLanguage = new System.Windows.Forms.ComboBox();
            this.labelPercent = new System.Windows.Forms.Label();
            this.textBoxMeshSizeLevel = new System.Windows.Forms.TextBox();
            this.labelMeshSize = new System.Windows.Forms.Label();
            this.labelLanguage = new System.Windows.Forms.Label();
            this.labelMeshSizeLevel = new System.Windows.Forms.Label();
            this.groupBoxSetting.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxSetting
            // 
            this.groupBoxSetting.Controls.Add(this.textBoxGmshPath);
            this.groupBoxSetting.Controls.Add(this.buttonSelectGmshPath);
            this.groupBoxSetting.Controls.Add(this.labelGmshPath);
            this.groupBoxSetting.Controls.Add(this.textBoxWorkingDirectory);
            this.groupBoxSetting.Controls.Add(this.buttonSelectWorkingDirectory);
            this.groupBoxSetting.Controls.Add(this.labelWorkingDirectory);
            this.groupBoxSetting.Location = new System.Drawing.Point(12, 12);
            this.groupBoxSetting.Name = "groupBoxSetting";
            this.groupBoxSetting.Size = new System.Drawing.Size(455, 105);
            this.groupBoxSetting.TabIndex = 10;
            this.groupBoxSetting.TabStop = false;
            this.groupBoxSetting.Text = "Directories";
            // 
            // textBoxGmshPath
            // 
            this.textBoxGmshPath.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxGmshPath.Location = new System.Drawing.Point(141, 69);
            this.textBoxGmshPath.Name = "textBoxGmshPath";
            this.textBoxGmshPath.Size = new System.Drawing.Size(272, 21);
            this.textBoxGmshPath.TabIndex = 16;
            // 
            // buttonSelectGmshPath
            // 
            this.buttonSelectGmshPath.Location = new System.Drawing.Point(419, 67);
            this.buttonSelectGmshPath.Name = "buttonSelectGmshPath";
            this.buttonSelectGmshPath.Size = new System.Drawing.Size(27, 23);
            this.buttonSelectGmshPath.TabIndex = 17;
            this.buttonSelectGmshPath.Tag = "CFD-Post.exe Full Path";
            this.buttonSelectGmshPath.Text = "...";
            this.buttonSelectGmshPath.UseVisualStyleBackColor = true;
            this.buttonSelectGmshPath.Click += new System.EventHandler(this.buttonSelectGmshPath_Click);
            // 
            // labelGmshPath
            // 
            this.labelGmshPath.AutoSize = true;
            this.labelGmshPath.Location = new System.Drawing.Point(24, 70);
            this.labelGmshPath.Margin = new System.Windows.Forms.Padding(3, 15, 3, 0);
            this.labelGmshPath.Name = "labelGmshPath";
            this.labelGmshPath.Size = new System.Drawing.Size(101, 12);
            this.labelGmshPath.TabIndex = 6;
            this.labelGmshPath.Text = "Gmsh.exe Path :";
            // 
            // textBoxWorkingDirectory
            // 
            this.textBoxWorkingDirectory.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxWorkingDirectory.Location = new System.Drawing.Point(141, 28);
            this.textBoxWorkingDirectory.Name = "textBoxWorkingDirectory";
            this.textBoxWorkingDirectory.Size = new System.Drawing.Size(272, 21);
            this.textBoxWorkingDirectory.TabIndex = 3;
            // 
            // buttonSelectWorkingDirectory
            // 
            this.buttonSelectWorkingDirectory.Location = new System.Drawing.Point(419, 28);
            this.buttonSelectWorkingDirectory.Name = "buttonSelectWorkingDirectory";
            this.buttonSelectWorkingDirectory.Size = new System.Drawing.Size(27, 23);
            this.buttonSelectWorkingDirectory.TabIndex = 4;
            this.buttonSelectWorkingDirectory.Tag = "Client Base Directory";
            this.buttonSelectWorkingDirectory.Text = "...";
            this.buttonSelectWorkingDirectory.UseVisualStyleBackColor = true;
            this.buttonSelectWorkingDirectory.Click += new System.EventHandler(this.buttonSelectWorkingDirectory_Click);
            // 
            // labelWorkingDirectory
            // 
            this.labelWorkingDirectory.AutoSize = true;
            this.labelWorkingDirectory.Location = new System.Drawing.Point(24, 33);
            this.labelWorkingDirectory.Name = "labelWorkingDirectory";
            this.labelWorkingDirectory.Size = new System.Drawing.Size(111, 12);
            this.labelWorkingDirectory.TabIndex = 2;
            this.labelWorkingDirectory.Text = "Working Directory :";
            // 
            // buttonSettingOK
            // 
            this.buttonSettingOK.Location = new System.Drawing.Point(353, 198);
            this.buttonSettingOK.Name = "buttonSettingOK";
            this.buttonSettingOK.Size = new System.Drawing.Size(114, 36);
            this.buttonSettingOK.TabIndex = 12;
            this.buttonSettingOK.Text = "OK";
            this.buttonSettingOK.UseVisualStyleBackColor = true;
            this.buttonSettingOK.Click += new System.EventHandler(this.buttonSettingOK_Click);
            // 
            // buttonSettingCancel
            // 
            this.buttonSettingCancel.Location = new System.Drawing.Point(353, 240);
            this.buttonSettingCancel.Name = "buttonSettingCancel";
            this.buttonSettingCancel.Size = new System.Drawing.Size(114, 36);
            this.buttonSettingCancel.TabIndex = 11;
            this.buttonSettingCancel.Text = "Cancel";
            this.buttonSettingCancel.UseVisualStyleBackColor = true;
            this.buttonSettingCancel.Click += new System.EventHandler(this.buttonSettingCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxLanguage);
            this.groupBox1.Controls.Add(this.labelPercent);
            this.groupBox1.Controls.Add(this.textBoxMeshSizeLevel);
            this.groupBox1.Controls.Add(this.labelMeshSize);
            this.groupBox1.Controls.Add(this.labelLanguage);
            this.groupBox1.Controls.Add(this.labelMeshSizeLevel);
            this.groupBox1.Location = new System.Drawing.Point(12, 123);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(307, 153);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ETC";
            // 
            // comboBoxLanguage
            // 
            this.comboBoxLanguage.FormattingEnabled = true;
            this.comboBoxLanguage.Items.AddRange(new object[] {
            "Korean",
            "English"});
            this.comboBoxLanguage.Location = new System.Drawing.Point(141, 38);
            this.comboBoxLanguage.Name = "comboBoxLanguage";
            this.comboBoxLanguage.Size = new System.Drawing.Size(103, 20);
            this.comboBoxLanguage.TabIndex = 6;
            this.comboBoxLanguage.Text = "Korean";
            // 
            // labelPercent
            // 
            this.labelPercent.AutoSize = true;
            this.labelPercent.Location = new System.Drawing.Point(250, 83);
            this.labelPercent.Name = "labelPercent";
            this.labelPercent.Size = new System.Drawing.Size(15, 12);
            this.labelPercent.TabIndex = 5;
            this.labelPercent.Text = "%";
            // 
            // textBoxMeshSizeLevel
            // 
            this.textBoxMeshSizeLevel.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxMeshSizeLevel.Location = new System.Drawing.Point(141, 80);
            this.textBoxMeshSizeLevel.Name = "textBoxMeshSizeLevel";
            this.textBoxMeshSizeLevel.Size = new System.Drawing.Size(103, 21);
            this.textBoxMeshSizeLevel.TabIndex = 4;
            this.textBoxMeshSizeLevel.Text = "5";
            this.textBoxMeshSizeLevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelMeshSize
            // 
            this.labelMeshSize.AutoSize = true;
            this.labelMeshSize.Location = new System.Drawing.Point(128, 108);
            this.labelMeshSize.Name = "labelMeshSize";
            this.labelMeshSize.Size = new System.Drawing.Size(172, 12);
            this.labelMeshSize.TabIndex = 3;
            this.labelMeshSize.Text = "( = Mesh Size / Model Size )";
            // 
            // labelLanguage
            // 
            this.labelLanguage.AutoSize = true;
            this.labelLanguage.Location = new System.Drawing.Point(24, 41);
            this.labelLanguage.Name = "labelLanguage";
            this.labelLanguage.Size = new System.Drawing.Size(69, 12);
            this.labelLanguage.TabIndex = 3;
            this.labelLanguage.Text = "Language :";
            // 
            // labelMeshSizeLevel
            // 
            this.labelMeshSizeLevel.AutoSize = true;
            this.labelMeshSizeLevel.Location = new System.Drawing.Point(24, 83);
            this.labelMeshSizeLevel.Name = "labelMeshSizeLevel";
            this.labelMeshSizeLevel.Size = new System.Drawing.Size(108, 12);
            this.labelMeshSizeLevel.TabIndex = 3;
            this.labelMeshSizeLevel.Text = "Mesh Size Level :";
            // 
            // PopupSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 290);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBoxSetting);
            this.Controls.Add(this.buttonSettingOK);
            this.Controls.Add(this.buttonSettingCancel);
            this.Name = "PopupSetting";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Setting";
            this.groupBoxSetting.ResumeLayout(false);
            this.groupBoxSetting.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxSetting;
        private System.Windows.Forms.TextBox textBoxGmshPath;
        private System.Windows.Forms.Button buttonSelectGmshPath;
        private System.Windows.Forms.Label labelGmshPath;
        private System.Windows.Forms.TextBox textBoxWorkingDirectory;
        private System.Windows.Forms.Button buttonSelectWorkingDirectory;
        private System.Windows.Forms.Label labelWorkingDirectory;
        private System.Windows.Forms.Button buttonSettingOK;
        private System.Windows.Forms.Button buttonSettingCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelPercent;
        private System.Windows.Forms.TextBox textBoxMeshSizeLevel;
        private System.Windows.Forms.Label labelMeshSizeLevel;
        private System.Windows.Forms.Label labelMeshSize;
        private System.Windows.Forms.ComboBox comboBoxLanguage;
        private System.Windows.Forms.Label labelLanguage;
    }
}