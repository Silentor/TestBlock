﻿namespace Server.MapGenerationVisualizer
{
    partial class Form1
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
            this.btnCreate = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cbZoom = new System.Windows.Forms.ComboBox();
            this.udZones = new System.Windows.Forms.NumericUpDown();
            this.udGrid = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udZones)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCreate
            // 
            this.btnCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreate.Location = new System.Drawing.Point(757, 549);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(75, 23);
            this.btnCreate.TabIndex = 0;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.Create_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(832, 531);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // cbZoom
            // 
            this.cbZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbZoom.FormattingEnabled = true;
            this.cbZoom.Items.AddRange(new object[] {
            "10",
            "25",
            "50",
            "75",
            "100"});
            this.cbZoom.Location = new System.Drawing.Point(12, 551);
            this.cbZoom.Name = "cbZoom";
            this.cbZoom.Size = new System.Drawing.Size(121, 21);
            this.cbZoom.TabIndex = 3;
            this.cbZoom.Text = "100";
            this.cbZoom.SelectedIndexChanged += new System.EventHandler(this.cbZoom_SelectedIndexChanged);
            // 
            // udZones
            // 
            this.udZones.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.udZones.Location = new System.Drawing.Point(422, 552);
            this.udZones.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udZones.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.udZones.Name = "udZones";
            this.udZones.Size = new System.Drawing.Size(120, 20);
            this.udZones.TabIndex = 4;
            this.udZones.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // udGrid
            // 
            this.udGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.udGrid.Location = new System.Drawing.Point(620, 552);
            this.udGrid.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udGrid.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.udGrid.Name = "udGrid";
            this.udGrid.Size = new System.Drawing.Size(120, 20);
            this.udGrid.TabIndex = 5;
            this.udGrid.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(379, 554);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Zones";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(586, 554);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Grid";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(856, 584);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.udGrid);
            this.Controls.Add(this.udZones);
            this.Controls.Add(this.cbZoom);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnCreate);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udZones)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ComboBox cbZoom;
        private System.Windows.Forms.NumericUpDown udZones;
        private System.Windows.Forms.NumericUpDown udGrid;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}
