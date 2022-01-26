namespace SodWinForms
{
    partial class WritingToFile
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Speed_CB = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.TS_CB = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Channel1_CB = new System.Windows.Forms.ComboBox();
            this.Start = new System.Windows.Forms.Button();
            this.Back = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.BrowseFile = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.Speed_CB);
            this.groupBox3.Location = new System.Drawing.Point(298, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(137, 50);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Скорость";
            // 
            // Speed_CB
            // 
            this.Speed_CB.FormattingEnabled = true;
            this.Speed_CB.Location = new System.Drawing.Point(6, 19);
            this.Speed_CB.Name = "Speed_CB";
            this.Speed_CB.Size = new System.Drawing.Size(121, 21);
            this.Speed_CB.TabIndex = 0;
            this.Speed_CB.SelectedIndexChanged += new System.EventHandler(this.ParametresOfCommand10Changed);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.TS_CB);
            this.groupBox2.Location = new System.Drawing.Point(155, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(137, 50);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Тип стыка";
            // 
            // TS_CB
            // 
            this.TS_CB.FormattingEnabled = true;
            this.TS_CB.Location = new System.Drawing.Point(6, 19);
            this.TS_CB.Name = "TS_CB";
            this.TS_CB.Size = new System.Drawing.Size(121, 21);
            this.TS_CB.TabIndex = 0;
            this.TS_CB.SelectedIndexChanged += new System.EventHandler(this.ParametresOfCommand10Changed);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Channel1_CB);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(137, 50);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Номер канала";
            // 
            // Channel1_CB
            // 
            this.Channel1_CB.FormattingEnabled = true;
            this.Channel1_CB.Location = new System.Drawing.Point(6, 19);
            this.Channel1_CB.Name = "Channel1_CB";
            this.Channel1_CB.Size = new System.Drawing.Size(121, 21);
            this.Channel1_CB.TabIndex = 0;
            this.Channel1_CB.SelectedIndexChanged += new System.EventHandler(this.ParametresOfCommand10Changed);
            // 
            // Start
            // 
            this.Start.Location = new System.Drawing.Point(122, 136);
            this.Start.Name = "Start";
            this.Start.Size = new System.Drawing.Size(75, 23);
            this.Start.TabIndex = 9;
            this.Start.Text = "Старт";
            this.Start.UseVisualStyleBackColor = true;
            this.Start.Click += new System.EventHandler(this.Start_Click);
            // 
            // Back
            // 
            this.Back.Location = new System.Drawing.Point(18, 136);
            this.Back.Name = "Back";
            this.Back.Size = new System.Drawing.Size(75, 23);
            this.Back.TabIndex = 8;
            this.Back.Text = "Отмена";
            this.Back.UseVisualStyleBackColor = true;
            this.Back.Click += new System.EventHandler(this.Back_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.BrowseFile);
            this.groupBox4.Controls.Add(this.textBox1);
            this.groupBox4.Location = new System.Drawing.Point(13, 61);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(422, 69);
            this.groupBox4.TabIndex = 10;
            this.groupBox4.TabStop = false;
            // 
            // BrowseFile
            // 
            this.BrowseFile.Location = new System.Drawing.Point(7, 40);
            this.BrowseFile.Name = "BrowseFile";
            this.BrowseFile.Size = new System.Drawing.Size(75, 23);
            this.BrowseFile.TabIndex = 10;
            this.BrowseFile.Text = "Выбрать файл";
            this.BrowseFile.UseVisualStyleBackColor = true;
            this.BrowseFile.Click += new System.EventHandler(this.BrowseFile_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(7, 16);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(405, 20);
            this.textBox1.TabIndex = 0;
            // 
            // WritingToFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 171);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.Start);
            this.Controls.Add(this.Back);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "WritingToFile";
            this.Text = "Запись в файл";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Window_Closing);
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        public System.Windows.Forms.ComboBox Speed_CB;
        private System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.ComboBox TS_CB;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.ComboBox Channel1_CB;
        private System.Windows.Forms.Button Start;
        private System.Windows.Forms.Button Back;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button BrowseFile;
        public System.Windows.Forms.TextBox textBox1;
    }
}