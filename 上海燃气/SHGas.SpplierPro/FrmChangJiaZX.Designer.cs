﻿namespace ProviderCS
{
    partial class FrmChangJiaZX
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
            this.label1 = new System.Windows.Forms.Label();
            this.cbbChangJia = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "厂家";
            // 
            // cbbChangJia
            // 
            this.cbbChangJia.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbbChangJia.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbbChangJia.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.cbbChangJia.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbChangJia.FormattingEnabled = true;
            this.cbbChangJia.Location = new System.Drawing.Point(47, 12);
            this.cbbChangJia.Name = "cbbChangJia";
            this.cbbChangJia.Size = new System.Drawing.Size(120, 20);
            this.cbbChangJia.TabIndex = 5;
            // 
            // btnOK
            // 
            this.btnOK.BackgroundImage = global::ProviderCS.Properties.Resources.BtnClose_OK;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOK.Location = new System.Drawing.Point(56, 41);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(80, 24);
            this.btnOK.TabIndex = 21;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // FrmChangJiaZX
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(185, 77);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cbbChangJia);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmChangJiaZX";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "厂家选择";
            this.Load += new System.EventHandler(this.FrmChangJiaZX_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbbChangJia;
        private System.Windows.Forms.Button btnOK;
    }
}