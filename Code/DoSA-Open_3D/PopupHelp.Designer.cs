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
            this.buttonDoSAUserGuide = new System.Windows.Forms.Button();
            this.buttonVCMGuide = new System.Windows.Forms.Button();
            this.buttonSolenoidGuide = new System.Windows.Forms.Button();
            this.buttonHelpClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonDoSAUserGuide
            // 
            this.buttonDoSAUserGuide.Enabled = false;
            this.buttonDoSAUserGuide.Location = new System.Drawing.Point(40, 32);
            this.buttonDoSAUserGuide.Name = "buttonDoSAUserGuide";
            this.buttonDoSAUserGuide.Size = new System.Drawing.Size(233, 30);
            this.buttonDoSAUserGuide.TabIndex = 0;
            this.buttonDoSAUserGuide.Text = "DoSA User Manual";
            this.buttonDoSAUserGuide.UseVisualStyleBackColor = true;
            this.buttonDoSAUserGuide.Click += new System.EventHandler(this.buttonDoSAUserGuide_Click);
            // 
            // buttonVCMGuide
            // 
            this.buttonVCMGuide.Location = new System.Drawing.Point(40, 78);
            this.buttonVCMGuide.Name = "buttonVCMGuide";
            this.buttonVCMGuide.Size = new System.Drawing.Size(233, 30);
            this.buttonVCMGuide.TabIndex = 1;
            this.buttonVCMGuide.Text = "VCM Example";
            this.buttonVCMGuide.UseVisualStyleBackColor = true;
            this.buttonVCMGuide.Click += new System.EventHandler(this.buttonVCMGuide_Click);
            // 
            // buttonSolenoidGuide
            // 
            this.buttonSolenoidGuide.Location = new System.Drawing.Point(40, 124);
            this.buttonSolenoidGuide.Name = "buttonSolenoidGuide";
            this.buttonSolenoidGuide.Size = new System.Drawing.Size(233, 30);
            this.buttonSolenoidGuide.TabIndex = 2;
            this.buttonSolenoidGuide.Text = "Solenoid Example";
            this.buttonSolenoidGuide.UseVisualStyleBackColor = true;
            this.buttonSolenoidGuide.Click += new System.EventHandler(this.buttonSolenoidGuide_Click);
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
            // PopupHelp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 233);
            this.Controls.Add(this.buttonHelpClose);
            this.Controls.Add(this.buttonSolenoidGuide);
            this.Controls.Add(this.buttonVCMGuide);
            this.Controls.Add(this.buttonDoSAUserGuide);
            this.Name = "PopupHelp";
            this.Text = "Help";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonDoSAUserGuide;
        private System.Windows.Forms.Button buttonVCMGuide;
        private System.Windows.Forms.Button buttonSolenoidGuide;
        private System.Windows.Forms.Button buttonHelpClose;
    }
}