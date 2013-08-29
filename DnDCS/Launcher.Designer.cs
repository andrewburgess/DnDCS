﻿namespace DnDCS
{
    partial class Launcher
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Launcher));
            this.btnClient = new System.Windows.Forms.Button();
            this.btnServer = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.spltLauncher = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.spltLauncher)).BeginInit();
            this.spltLauncher.Panel1.SuspendLayout();
            this.spltLauncher.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClient
            // 
            this.btnClient.Location = new System.Drawing.Point(296, 38);
            this.btnClient.Name = "btnClient";
            this.btnClient.Size = new System.Drawing.Size(75, 23);
            this.btnClient.TabIndex = 0;
            this.btnClient.Text = "Client";
            this.btnClient.UseVisualStyleBackColor = true;
            this.btnClient.Click += new System.EventHandler(this.btnClient_Click);
            // 
            // btnServer
            // 
            this.btnServer.Location = new System.Drawing.Point(394, 38);
            this.btnServer.Name = "btnServer";
            this.btnServer.Size = new System.Drawing.Size(75, 23);
            this.btnServer.TabIndex = 1;
            this.btnServer.Text = "Server";
            this.btnServer.UseVisualStyleBackColor = true;
            this.btnServer.Click += new System.EventHandler(this.btnServer_Click);
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(294, 8);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(176, 13);
            this.lblInfo.TabIndex = 2;
            this.lblInfo.Text = "Select which mode you want to run.";
            // 
            // spltLauncher
            // 
            this.spltLauncher.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spltLauncher.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.spltLauncher.IsSplitterFixed = true;
            this.spltLauncher.Location = new System.Drawing.Point(0, 0);
            this.spltLauncher.Name = "spltLauncher";
            this.spltLauncher.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spltLauncher.Panel1
            // 
            this.spltLauncher.Panel1.Controls.Add(this.btnServer);
            this.spltLauncher.Panel1.Controls.Add(this.btnClient);
            this.spltLauncher.Panel1.Controls.Add(this.lblInfo);
            this.spltLauncher.Size = new System.Drawing.Size(765, 626);
            this.spltLauncher.SplitterDistance = 68;
            this.spltLauncher.TabIndex = 4;
            // 
            // Launcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(765, 626);
            this.Controls.Add(this.spltLauncher);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Launcher";
            this.Text = "DnDCS - Launcher";
            this.Load += new System.EventHandler(this.Launcher_Load);
            this.spltLauncher.Panel1.ResumeLayout(false);
            this.spltLauncher.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spltLauncher)).EndInit();
            this.spltLauncher.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClient;
        private System.Windows.Forms.Button btnServer;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.SplitContainer spltLauncher;
    }
}

