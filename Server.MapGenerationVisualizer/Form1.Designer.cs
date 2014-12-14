namespace Server.MapGenerationVisualizer
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
            this.label3 = new System.Windows.Forms.Label();
            this.cbIsRandomSeed = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbSeed = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.udMinDistance = new System.Windows.Forms.NumericUpDown();
            this.cbStyle = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udZones)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udMinDistance)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCreate
            // 
            this.btnCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreate.Location = new System.Drawing.Point(760, 520);
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
            this.pictureBox1.Size = new System.Drawing.Size(739, 531);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // cbZoom
            // 
            this.cbZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbZoom.FormattingEnabled = true;
            this.cbZoom.Items.AddRange(new object[] {
            "1",
            "5",
            "10",
            "25",
            "50",
            "75",
            "100"});
            this.cbZoom.Location = new System.Drawing.Point(757, 30);
            this.cbZoom.Name = "cbZoom";
            this.cbZoom.Size = new System.Drawing.Size(87, 21);
            this.cbZoom.TabIndex = 3;
            this.cbZoom.Text = "100";
            this.cbZoom.SelectedIndexChanged += new System.EventHandler(this.cbZoom_SelectedIndexChanged);
            // 
            // udZones
            // 
            this.udZones.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.udZones.Location = new System.Drawing.Point(757, 82);
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
            this.udZones.Size = new System.Drawing.Size(87, 20);
            this.udZones.TabIndex = 4;
            this.udZones.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // udGrid
            // 
            this.udGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.udGrid.Location = new System.Drawing.Point(757, 145);
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
            this.udGrid.Size = new System.Drawing.Size(87, 20);
            this.udGrid.TabIndex = 5;
            this.udGrid.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(757, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Zones";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(757, 129);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Grid";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(757, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Zoom";
            // 
            // cbIsRandomSeed
            // 
            this.cbIsRandomSeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbIsRandomSeed.AutoSize = true;
            this.cbIsRandomSeed.Checked = true;
            this.cbIsRandomSeed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbIsRandomSeed.Location = new System.Drawing.Point(764, 497);
            this.cbIsRandomSeed.Name = "cbIsRandomSeed";
            this.cbIsRandomSeed.Size = new System.Drawing.Size(92, 17);
            this.cbIsRandomSeed.TabIndex = 9;
            this.cbIsRandomSeed.Text = "Random seed";
            this.cbIsRandomSeed.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(757, 455);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Seed";
            // 
            // tbSeed
            // 
            this.tbSeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSeed.Location = new System.Drawing.Point(757, 471);
            this.tbSeed.MaxLength = 10;
            this.tbSeed.Name = "tbSeed";
            this.tbSeed.Size = new System.Drawing.Size(87, 20);
            this.tbSeed.TabIndex = 11;
            this.tbSeed.Text = "0";
            this.tbSeed.WordWrap = false;
            this.tbSeed.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(757, 182);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Zone distance";
            // 
            // udMinDistance
            // 
            this.udMinDistance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.udMinDistance.Location = new System.Drawing.Point(757, 198);
            this.udMinDistance.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.udMinDistance.Name = "udMinDistance";
            this.udMinDistance.Size = new System.Drawing.Size(87, 20);
            this.udMinDistance.TabIndex = 12;
            this.udMinDistance.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // cbStyle
            // 
            this.cbStyle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbStyle.FormattingEnabled = true;
            this.cbStyle.Location = new System.Drawing.Point(757, 419);
            this.cbStyle.Name = "cbStyle";
            this.cbStyle.Size = new System.Drawing.Size(87, 21);
            this.cbStyle.TabIndex = 14;
            this.cbStyle.SelectedIndexChanged += new System.EventHandler(this.cbStyle_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(760, 400);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Style";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(856, 584);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cbStyle);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.udMinDistance);
            this.Controls.Add(this.tbSeed);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbIsRandomSeed);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.udGrid);
            this.Controls.Add(this.udZones);
            this.Controls.Add(this.cbZoom);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnCreate);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udZones)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udMinDistance)).EndInit();
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
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbIsRandomSeed;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbSeed;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown udMinDistance;
        private System.Windows.Forms.ComboBox cbStyle;
        private System.Windows.Forms.Label label6;
    }
}

