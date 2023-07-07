namespace DoSA
{
    partial class PopupNewDesign
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
            this.labelShapeFile = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.textBoxSTEPFileFullName = new System.Windows.Forms.TextBox();
            this.groupBoxNew = new System.Windows.Forms.GroupBox();
            this.labelName = new System.Windows.Forms.Label();
            this.textBoxDesignName = new System.Windows.Forms.TextBox();
            this.buttonSelectSTEP_File = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxComment = new System.Windows.Forms.GroupBox();
            this.labelComment4 = new System.Windows.Forms.Label();
            this.labelComment7 = new System.Windows.Forms.Label();
            this.labelComment6 = new System.Windows.Forms.Label();
            this.labelComment5 = new System.Windows.Forms.Label();
            this.labelComment3 = new System.Windows.Forms.Label();
            this.labelComment2 = new System.Windows.Forms.Label();
            this.labelComment1 = new System.Windows.Forms.Label();
            this.groupBoxNew.SuspendLayout();
            this.groupBoxComment.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelShapeFile
            // 
            this.labelShapeFile.AutoSize = true;
            this.labelShapeFile.Location = new System.Drawing.Point(23, 79);
            this.labelShapeFile.Name = "labelShapeFile";
            this.labelShapeFile.Size = new System.Drawing.Size(119, 12);
            this.labelShapeFile.TabIndex = 0;
            this.labelShapeFile.Text = "Shape File (STEP) :";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(247, 352);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(100, 30);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // textBoxSTEPFileFullName
            // 
            this.textBoxSTEPFileFullName.Location = new System.Drawing.Point(148, 75);
            this.textBoxSTEPFileFullName.Name = "textBoxSTEPFileFullName";
            this.textBoxSTEPFileFullName.Size = new System.Drawing.Size(254, 21);
            this.textBoxSTEPFileFullName.TabIndex = 1;
            this.textBoxSTEPFileFullName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxName_KeyPress);
            // 
            // groupBoxNew
            // 
            this.groupBoxNew.Controls.Add(this.labelName);
            this.groupBoxNew.Controls.Add(this.textBoxDesignName);
            this.groupBoxNew.Controls.Add(this.buttonSelectSTEP_File);
            this.groupBoxNew.Controls.Add(this.labelShapeFile);
            this.groupBoxNew.Controls.Add(this.textBoxSTEPFileFullName);
            this.groupBoxNew.Location = new System.Drawing.Point(12, 12);
            this.groupBoxNew.Name = "groupBoxNew";
            this.groupBoxNew.Size = new System.Drawing.Size(441, 122);
            this.groupBoxNew.TabIndex = 0;
            this.groupBoxNew.TabStop = false;
            this.groupBoxNew.Text = "New Design";
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(23, 39);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(90, 12);
            this.labelName.TabIndex = 10;
            this.labelName.Text = "Design Name :";
            // 
            // textBoxDesignName
            // 
            this.textBoxDesignName.Location = new System.Drawing.Point(148, 36);
            this.textBoxDesignName.Name = "textBoxDesignName";
            this.textBoxDesignName.Size = new System.Drawing.Size(139, 21);
            this.textBoxDesignName.TabIndex = 0;
            // 
            // buttonSelectSTEP_File
            // 
            this.buttonSelectSTEP_File.Location = new System.Drawing.Point(408, 74);
            this.buttonSelectSTEP_File.Name = "buttonSelectSTEP_File";
            this.buttonSelectSTEP_File.Size = new System.Drawing.Size(27, 23);
            this.buttonSelectSTEP_File.TabIndex = 2;
            this.buttonSelectSTEP_File.Tag = "";
            this.buttonSelectSTEP_File.Text = "...";
            this.buttonSelectSTEP_File.UseVisualStyleBackColor = true;
            this.buttonSelectSTEP_File.Click += new System.EventHandler(this.buttonSelectSTEP_File_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(353, 352);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 30);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // groupBoxComment
            // 
            this.groupBoxComment.Controls.Add(this.labelComment4);
            this.groupBoxComment.Controls.Add(this.labelComment7);
            this.groupBoxComment.Controls.Add(this.labelComment6);
            this.groupBoxComment.Controls.Add(this.labelComment5);
            this.groupBoxComment.Controls.Add(this.labelComment3);
            this.groupBoxComment.Controls.Add(this.labelComment2);
            this.groupBoxComment.Controls.Add(this.labelComment1);
            this.groupBoxComment.Location = new System.Drawing.Point(12, 141);
            this.groupBoxComment.Name = "groupBoxComment";
            this.groupBoxComment.Size = new System.Drawing.Size(441, 190);
            this.groupBoxComment.TabIndex = 2;
            this.groupBoxComment.TabStop = false;
            this.groupBoxComment.Text = "기능 제한";
            // 
            // labelComment4
            // 
            this.labelComment4.AutoSize = true;
            this.labelComment4.Location = new System.Drawing.Point(28, 89);
            this.labelComment4.Name = "labelComment4";
            this.labelComment4.Size = new System.Drawing.Size(263, 12);
            this.labelComment4.TabIndex = 6;
            this.labelComment4.Text = "- 다각 코일도 전류는 원통코일처럼 인가됩니다.";
            // 
            // labelComment7
            // 
            this.labelComment7.AutoSize = true;
            this.labelComment7.Location = new System.Drawing.Point(28, 155);
            this.labelComment7.Name = "labelComment7";
            this.labelComment7.Size = new System.Drawing.Size(159, 12);
            this.labelComment7.TabIndex = 5;
            this.labelComment7.Text = "- 하나의 코일만 지원합니다.";
            // 
            // labelComment6
            // 
            this.labelComment6.AutoSize = true;
            this.labelComment6.Location = new System.Drawing.Point(28, 134);
            this.labelComment6.Name = "labelComment6";
            this.labelComment6.Size = new System.Drawing.Size(211, 12);
            this.labelComment6.TabIndex = 4;
            this.labelComment6.Text = "- 움직이는 파트는 하나만 지원합니다.";
            // 
            // labelComment5
            // 
            this.labelComment5.AutoSize = true;
            this.labelComment5.Font = new System.Drawing.Font("Gulim", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelComment5.Location = new System.Drawing.Point(15, 114);
            this.labelComment5.Name = "labelComment5";
            this.labelComment5.Size = new System.Drawing.Size(97, 12);
            this.labelComment5.TabIndex = 3;
            this.labelComment5.Text = "2. 파트 수 제한";
            // 
            // labelComment3
            // 
            this.labelComment3.AutoSize = true;
            this.labelComment3.Location = new System.Drawing.Point(28, 68);
            this.labelComment3.Name = "labelComment3";
            this.labelComment3.Size = new System.Drawing.Size(247, 12);
            this.labelComment3.TabIndex = 2;
            this.labelComment3.Text = "- STEP 파일의 파트명은 영문만 지원합니다.";
            // 
            // labelComment2
            // 
            this.labelComment2.AutoSize = true;
            this.labelComment2.Location = new System.Drawing.Point(28, 47);
            this.labelComment2.Name = "labelComment2";
            this.labelComment2.Size = new System.Drawing.Size(227, 12);
            this.labelComment2.TabIndex = 1;
            this.labelComment2.Text = "- 코일 중심축은 Y 축과 일치해야 합니다.";
            // 
            // labelComment1
            // 
            this.labelComment1.AutoSize = true;
            this.labelComment1.Font = new System.Drawing.Font("Gulim", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelComment1.Location = new System.Drawing.Point(15, 26);
            this.labelComment1.Name = "labelComment1";
            this.labelComment1.Size = new System.Drawing.Size(79, 12);
            this.labelComment1.TabIndex = 0;
            this.labelComment1.Text = "1. 형상 제한";
            // 
            // PopupNewDesign
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(465, 394);
            this.Controls.Add(this.groupBoxComment);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupBoxNew);
            this.Controls.Add(this.buttonOK);
            this.Name = "PopupNewDesign";
            this.ShowIcon = false;
            this.Text = "New Design";
            this.groupBoxNew.ResumeLayout(false);
            this.groupBoxNew.PerformLayout();
            this.groupBoxComment.ResumeLayout(false);
            this.groupBoxComment.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelShapeFile;
        private System.Windows.Forms.Button buttonOK;
        public System.Windows.Forms.TextBox textBoxSTEPFileFullName;
        private System.Windows.Forms.GroupBox groupBoxNew;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelName;
        public System.Windows.Forms.TextBox textBoxDesignName;
        private System.Windows.Forms.Button buttonSelectSTEP_File;
        private System.Windows.Forms.GroupBox groupBoxComment;
        private System.Windows.Forms.Label labelComment3;
        private System.Windows.Forms.Label labelComment2;
        private System.Windows.Forms.Label labelComment1;
        private System.Windows.Forms.Label labelComment6;
        private System.Windows.Forms.Label labelComment5;
        private System.Windows.Forms.Label labelComment7;
        private System.Windows.Forms.Label labelComment4;
    }
}