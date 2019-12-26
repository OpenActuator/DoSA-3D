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
            this.buttonSolenoidGuide = new System.Windows.Forms.Button();
            this.buttonVCMGuide = new System.Windows.Forms.Button();
            this.buttonDoSAUserGuide = new System.Windows.Forms.Button();
            this.groupBoxHelp.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonHelpClose
            // 
            this.buttonHelpClose.Location = new System.Drawing.Point(186, 190);
            this.buttonHelpClose.Name = "buttonHelpClose";
            this.buttonHelpClose.Size = new System.Drawing.Size(111, 31);
            this.buttonHelpClose.TabIndex = 3;
            this.buttonHelpClose.Text = "Close";
            this.buttonHelpClose.UseVisualStyleBackColor = true;
            this.buttonHelpClose.Click += new System.EventHandler(this.buttonHelpClose_Click);
            // 
            // groupBoxHelp
            // 
            this.groupBoxHelp.Controls.Add(this.buttonSolenoidGuide);
            this.groupBoxHelp.Controls.Add(this.buttonVCMGuide);
            this.groupBoxHelp.Controls.Add(this.buttonDoSAUserGuide);
            this.groupBoxHelp.Location = new System.Drawing.Point(13, 13);
            this.groupBoxHelp.Name = "groupBoxHelp";
            this.groupBoxHelp.Size = new System.Drawing.Size(284, 171);
            this.groupBoxHelp.TabIndex = 4;
            this.groupBoxHelp.TabStop = false;
            this.groupBoxHelp.Text = "Help";
            // 
            // buttonSolenoidGuide
            // 
            this.buttonSolenoidGuide.Location = new System.Drawing.Point(26, 119);
            this.buttonSolenoidGuide.Name = "buttonSolenoidGuide";
            this.buttonSolenoidGuide.Size = new System.Drawing.Size(233, 30);
            this.buttonSolenoidGuide.TabIndex = 5;
            this.buttonSolenoidGuide.Text = "Solenoid Example";
            this.buttonSolenoidGuide.UseVisualStyleBackColor = true;
            // 
            // buttonVCMGuide
            // 
            this.buttonVCMGuide.Location = new System.Drawing.Point(26, 73);
            this.buttonVCMGuide.Name = "buttonVCMGuide";
            this.buttonVCMGuide.Size = new System.Drawing.Size(233, 30);
            this.buttonVCMGuide.TabIndex = 4;
            this.buttonVCMGuide.Text = "VCM Example";
            this.buttonVCMGuide.UseVisualStyleBackColor = true;
            // 
            // buttonDoSAUserGuide
            // 
            this.buttonDoSAUserGuide.Enabled = false;
            this.buttonDoSAUserGuide.Location = new System.Drawing.Point(26, 27);
            this.buttonDoSAUserGuide.Name = "buttonDoSAUserGuide";
            this.buttonDoSAUserGuide.Size = new System.Drawing.Size(233, 30);
            this.buttonDoSAUserGuide.TabIndex = 3;
            this.buttonDoSAUserGuide.Text = "DoSA User Manual";
            this.buttonDoSAUserGuide.UseVisualStyleBackColor = true;
            // 
            // PopupHelp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 233);
            this.Controls.Add(this.groupBoxHelp);
            this.Controls.Add(this.buttonHelpClose);
            this.Name = "PopupHelp";
            this.Text = "Help";
            this.groupBoxHelp.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonHelpClose;
        private System.Windows.Forms.GroupBox groupBoxHelp;
        private System.Windows.Forms.Button buttonSolenoidGuide;
        private System.Windows.Forms.Button buttonVCMGuide;
        private System.Windows.Forms.Button buttonDoSAUserGuide;
    }
}