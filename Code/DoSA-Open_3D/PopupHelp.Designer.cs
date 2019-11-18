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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopupHelp));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonSolenoidGuide = new System.Windows.Forms.Button();
            this.buttonVCMGuide = new System.Windows.Forms.Button();
            this.buttonDoSAUserGuide = new System.Windows.Forms.Button();
            this.buttonHelpClose = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.buttonSolenoidGuide);
            this.groupBox1.Controls.Add(this.buttonVCMGuide);
            this.groupBox1.Controls.Add(this.buttonDoSAUserGuide);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // buttonSolenoidGuide
            // 
            resources.ApplyResources(this.buttonSolenoidGuide, "buttonSolenoidGuide");
            this.buttonSolenoidGuide.Name = "buttonSolenoidGuide";
            this.buttonSolenoidGuide.UseVisualStyleBackColor = true;
            this.buttonSolenoidGuide.Click += new System.EventHandler(this.buttonSolenoidGuide_Click);
            // 
            // buttonVCMGuide
            // 
            resources.ApplyResources(this.buttonVCMGuide, "buttonVCMGuide");
            this.buttonVCMGuide.Name = "buttonVCMGuide";
            this.buttonVCMGuide.UseVisualStyleBackColor = true;
            this.buttonVCMGuide.Click += new System.EventHandler(this.buttonVCMGuide_Click);
            // 
            // buttonDoSAUserGuide
            // 
            resources.ApplyResources(this.buttonDoSAUserGuide, "buttonDoSAUserGuide");
            this.buttonDoSAUserGuide.Name = "buttonDoSAUserGuide";
            this.buttonDoSAUserGuide.UseVisualStyleBackColor = true;
            this.buttonDoSAUserGuide.Click += new System.EventHandler(this.buttonDoSAUserGuide_Click);
            // 
            // buttonHelpClose
            // 
            resources.ApplyResources(this.buttonHelpClose, "buttonHelpClose");
            this.buttonHelpClose.Name = "buttonHelpClose";
            this.buttonHelpClose.UseVisualStyleBackColor = true;
            this.buttonHelpClose.Click += new System.EventHandler(this.buttonHelpClose_Click);
            // 
            // PopupHelp
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonHelpClose);
            this.Controls.Add(this.groupBox1);
            this.Name = "PopupHelp";
            this.ShowIcon = false;
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonHelpClose;
        private System.Windows.Forms.Button buttonDoSAUserGuide;
        private System.Windows.Forms.Button buttonSolenoidGuide;
        private System.Windows.Forms.Button buttonVCMGuide;
    }
}