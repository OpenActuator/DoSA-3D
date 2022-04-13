namespace DoSA
{
    partial class PopupAddNode
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
            this.groupBoxParts = new System.Windows.Forms.GroupBox();
            this.listViewNodeName = new System.Windows.Forms.ListView();
            this.columnHeaderPart = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.textBoxNodeName = new System.Windows.Forms.TextBox();
            this.groupBoxTests = new System.Windows.Forms.GroupBox();
            this.groupBoxCheckPartsName = new System.Windows.Forms.GroupBox();
            this.labelCheckPartsName2 = new System.Windows.Forms.Label();
            this.labelCheckPartsName1 = new System.Windows.Forms.Label();
            this.groupBoxParts.SuspendLayout();
            this.groupBoxTests.SuspendLayout();
            this.groupBoxCheckPartsName.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxParts
            // 
            this.groupBoxParts.Controls.Add(this.listViewNodeName);
            this.groupBoxParts.Location = new System.Drawing.Point(12, 12);
            this.groupBoxParts.Name = "groupBoxParts";
            this.groupBoxParts.Size = new System.Drawing.Size(211, 280);
            this.groupBoxParts.TabIndex = 0;
            this.groupBoxParts.TabStop = false;
            this.groupBoxParts.Text = "Shape Parts";
            // 
            // listViewNodeName
            // 
            this.listViewNodeName.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderPart});
            this.listViewNodeName.FullRowSelect = true;
            this.listViewNodeName.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewNodeName.HideSelection = false;
            this.listViewNodeName.Location = new System.Drawing.Point(18, 36);
            this.listViewNodeName.MultiSelect = false;
            this.listViewNodeName.Name = "listViewNodeName";
            this.listViewNodeName.Size = new System.Drawing.Size(175, 223);
            this.listViewNodeName.TabIndex = 3;
            this.listViewNodeName.UseCompatibleStateImageBehavior = false;
            this.listViewNodeName.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderPart
            // 
            this.columnHeaderPart.Width = 170;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(356, 255);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(121, 37);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(356, 212);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(121, 37);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // textBoxNodeName
            // 
            this.textBoxNodeName.Location = new System.Drawing.Point(35, 36);
            this.textBoxNodeName.Name = "textBoxNodeName";
            this.textBoxNodeName.Size = new System.Drawing.Size(175, 21);
            this.textBoxNodeName.TabIndex = 1;
            this.textBoxNodeName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxNodeName_KeyPress);
            // 
            // groupBoxTests
            // 
            this.groupBoxTests.Controls.Add(this.textBoxNodeName);
            this.groupBoxTests.Location = new System.Drawing.Point(239, 12);
            this.groupBoxTests.Name = "groupBoxTests";
            this.groupBoxTests.Size = new System.Drawing.Size(238, 80);
            this.groupBoxTests.TabIndex = 3;
            this.groupBoxTests.TabStop = false;
            this.groupBoxTests.Text = "Test Name";
            // 
            // groupBoxCheckPartsName
            // 
            this.groupBoxCheckPartsName.Controls.Add(this.labelCheckPartsName2);
            this.groupBoxCheckPartsName.Controls.Add(this.labelCheckPartsName1);
            this.groupBoxCheckPartsName.Location = new System.Drawing.Point(239, 108);
            this.groupBoxCheckPartsName.Name = "groupBoxCheckPartsName";
            this.groupBoxCheckPartsName.Size = new System.Drawing.Size(238, 83);
            this.groupBoxCheckPartsName.TabIndex = 5;
            this.groupBoxCheckPartsName.TabStop = false;
            this.groupBoxCheckPartsName.Text = "Check Part Names";
            // 
            // labelCheckPartsName2
            // 
            this.labelCheckPartsName2.AutoSize = true;
            this.labelCheckPartsName2.Location = new System.Drawing.Point(13, 58);
            this.labelCheckPartsName2.Name = "labelCheckPartsName2";
            this.labelCheckPartsName2.Size = new System.Drawing.Size(207, 12);
            this.labelCheckPartsName2.TabIndex = 6;
            this.labelCheckPartsName2.Text = "2. Names are NG. ⇒ Cancel Button";
            // 
            // labelCheckPartsName1
            // 
            this.labelCheckPartsName1.AutoSize = true;
            this.labelCheckPartsName1.Location = new System.Drawing.Point(13, 32);
            this.labelCheckPartsName1.Name = "labelCheckPartsName1";
            this.labelCheckPartsName1.Size = new System.Drawing.Size(183, 12);
            this.labelCheckPartsName1.TabIndex = 5;
            this.labelCheckPartsName1.Text = "1. Names are OK. ⇒ OK Button";
            // 
            // PopupAddNode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 302);
            this.Controls.Add(this.groupBoxCheckPartsName);
            this.Controls.Add(this.groupBoxTests);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupBoxParts);
            this.Name = "PopupAddNode";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add Node";
            this.groupBoxParts.ResumeLayout(false);
            this.groupBoxTests.ResumeLayout(false);
            this.groupBoxTests.PerformLayout();
            this.groupBoxCheckPartsName.ResumeLayout(false);
            this.groupBoxCheckPartsName.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxParts;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.ListView listViewNodeName;
        private System.Windows.Forms.TextBox textBoxNodeName;
        private System.Windows.Forms.GroupBox groupBoxTests;
        private System.Windows.Forms.ColumnHeader columnHeaderPart;
        private System.Windows.Forms.GroupBox groupBoxCheckPartsName;
        private System.Windows.Forms.Label labelCheckPartsName2;
        private System.Windows.Forms.Label labelCheckPartsName1;
    }
}