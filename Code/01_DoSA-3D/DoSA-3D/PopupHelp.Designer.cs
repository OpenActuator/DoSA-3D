namespace DoSA
{
    partial class PopupHelp
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
            this.buttonHelpClose = new System.Windows.Forms.Button();
            this.groupBoxHelp = new System.Windows.Forms.GroupBox();
            this.buttonOpenHelpDirectory = new System.Windows.Forms.Button();
            this.buttonDrawingGuide = new System.Windows.Forms.Button();
            this.buttonSolenoidGuide = new System.Windows.Forms.Button();
            this.buttonVCMGuide = new System.Windows.Forms.Button();
            this.labelNotice2 = new System.Windows.Forms.Label();
            this.labelNotice1 = new System.Windows.Forms.Label();
            this.groupBoxHelp.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonHelpClose
            // 
            this.buttonHelpClose.Location = new System.Drawing.Point(223, 300);
            this.buttonHelpClose.Name = "buttonHelpClose";
            this.buttonHelpClose.Size = new System.Drawing.Size(111, 31);
            this.buttonHelpClose.TabIndex = 3;
            this.buttonHelpClose.Text = "Close";
            this.buttonHelpClose.UseVisualStyleBackColor = true;
            this.buttonHelpClose.Click += new System.EventHandler(this.buttonHelpClose_Click);
            // 
            // groupBoxHelp
            // 
            this.groupBoxHelp.Controls.Add(this.buttonOpenHelpDirectory);
            this.groupBoxHelp.Controls.Add(this.buttonDrawingGuide);
            this.groupBoxHelp.Controls.Add(this.buttonSolenoidGuide);
            this.groupBoxHelp.Controls.Add(this.buttonVCMGuide);
            this.groupBoxHelp.Location = new System.Drawing.Point(13, 13);
            this.groupBoxHelp.Name = "groupBoxHelp";
            this.groupBoxHelp.Size = new System.Drawing.Size(321, 222);
            this.groupBoxHelp.TabIndex = 4;
            this.groupBoxHelp.TabStop = false;
            this.groupBoxHelp.Text = "Help";
            // 
            // buttonOpenHelpDirectory
            // 
            this.buttonOpenHelpDirectory.Location = new System.Drawing.Point(33, 172);
            this.buttonOpenHelpDirectory.Name = "buttonOpenHelpDirectory";
            this.buttonOpenHelpDirectory.Size = new System.Drawing.Size(260, 31);
            this.buttonOpenHelpDirectory.TabIndex = 8;
            this.buttonOpenHelpDirectory.Text = "Open the help directory";
            this.buttonOpenHelpDirectory.UseVisualStyleBackColor = true;
            this.buttonOpenHelpDirectory.Click += new System.EventHandler(this.buttonOpenHelpDirectory_Click);
            // 
            // buttonDrawingGuide
            // 
            this.buttonDrawingGuide.Location = new System.Drawing.Point(33, 24);
            this.buttonDrawingGuide.Name = "buttonDrawingGuide";
            this.buttonDrawingGuide.Size = new System.Drawing.Size(260, 30);
            this.buttonDrawingGuide.TabIndex = 7;
            this.buttonDrawingGuide.Text = "Drawing Guide";
            this.buttonDrawingGuide.UseVisualStyleBackColor = true;
            this.buttonDrawingGuide.Click += new System.EventHandler(this.buttonDrawingGuide_Click);
            // 
            // buttonSolenoidGuide
            // 
            this.buttonSolenoidGuide.Location = new System.Drawing.Point(33, 122);
            this.buttonSolenoidGuide.Name = "buttonSolenoidGuide";
            this.buttonSolenoidGuide.Size = new System.Drawing.Size(260, 30);
            this.buttonSolenoidGuide.TabIndex = 5;
            this.buttonSolenoidGuide.Text = "Solenoid Example";
            this.buttonSolenoidGuide.UseVisualStyleBackColor = true;
            this.buttonSolenoidGuide.Click += new System.EventHandler(this.buttonSolenoidGuide_Click);
            // 
            // buttonVCMGuide
            // 
            this.buttonVCMGuide.Location = new System.Drawing.Point(33, 73);
            this.buttonVCMGuide.Name = "buttonVCMGuide";
            this.buttonVCMGuide.Size = new System.Drawing.Size(260, 30);
            this.buttonVCMGuide.TabIndex = 4;
            this.buttonVCMGuide.Text = "VCM Example";
            this.buttonVCMGuide.UseVisualStyleBackColor = true;
            this.buttonVCMGuide.Click += new System.EventHandler(this.buttonVCMGuide_Click);
            // 
            // labelNotice2
            // 
            this.labelNotice2.AutoSize = true;
            this.labelNotice2.Location = new System.Drawing.Point(25, 271);
            this.labelNotice2.Name = "labelNotice2";
            this.labelNotice2.Size = new System.Drawing.Size(74, 12);
            this.labelNotice2.TabIndex = 8;
            this.labelNotice2.Text = "labelNotice2";
            // 
            // labelNotice1
            // 
            this.labelNotice1.AutoSize = true;
            this.labelNotice1.Location = new System.Drawing.Point(25, 253);
            this.labelNotice1.Name = "labelNotice1";
            this.labelNotice1.Size = new System.Drawing.Size(74, 12);
            this.labelNotice1.TabIndex = 7;
            this.labelNotice1.Text = "labelNotice1";
            // 
            // PopupHelp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 343);
            this.Controls.Add(this.labelNotice2);
            this.Controls.Add(this.labelNotice1);
            this.Controls.Add(this.groupBoxHelp);
            this.Controls.Add(this.buttonHelpClose);
            this.Name = "PopupHelp";
            this.Text = "Help";
            this.groupBoxHelp.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonHelpClose;
        private System.Windows.Forms.GroupBox groupBoxHelp;
        private System.Windows.Forms.Button buttonSolenoidGuide;
        private System.Windows.Forms.Button buttonVCMGuide;
        private System.Windows.Forms.Button buttonDrawingGuide;
        private System.Windows.Forms.Button buttonOpenHelpDirectory;
        private System.Windows.Forms.Label labelNotice2;
        private System.Windows.Forms.Label labelNotice1;
    }
}