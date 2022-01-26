namespace SodWinForms
{
    partial class TwoDevices
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
            this.Start = new System.Windows.Forms.Button();
            this.Back = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.KoefficientGroup = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.averageChanceError = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.errorThreadLength = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Speed_CB = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.TS_CB = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Channel1_CB = new System.Windows.Forms.ComboBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.Channel2_CB = new System.Windows.Forms.ComboBox();
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.SuspendLayout();
            // 
            // Start
            // 
            this.Start.Location = new System.Drawing.Point(116, 188);
            this.Start.Name = "Start";
            this.Start.Size = new System.Drawing.Size(75, 23);
            this.Start.TabIndex = 15;
            this.Start.Text = "Старт";
            this.Start.UseVisualStyleBackColor = true;
            this.Start.Click += new System.EventHandler(this.Start_Click);
            // 
            // Back
            // 
            this.Back.Location = new System.Drawing.Point(12, 188);
            this.Back.Name = "Back";
            this.Back.Size = new System.Drawing.Size(75, 23);
            this.Back.TabIndex = 14;
            this.Back.Text = "Отмена";
            this.Back.UseVisualStyleBackColor = true;
            this.Back.Click += new System.EventHandler(this.Back_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.KoefficientGroup);
            this.groupBox6.Location = new System.Drawing.Point(333, 130);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(171, 50);
            this.groupBox6.TabIndex = 13;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Коэффициент группирования";
            // 
            // KoefficientGroup
            // 
            this.KoefficientGroup.Location = new System.Drawing.Point(6, 19);
            this.KoefficientGroup.Name = "KoefficientGroup";
            this.KoefficientGroup.Size = new System.Drawing.Size(121, 20);
            this.KoefficientGroup.TabIndex = 0;
            this.KoefficientGroup.Text = "0,0";
            this.KoefficientGroup.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnlyNumbersDrobes_KeyPress);

            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.averageChanceError);
            this.groupBox5.Location = new System.Drawing.Point(155, 130);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(172, 50);
            this.groupBox5.TabIndex = 12;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Средняя вероятность ошибки";
            // 
            // averageChanceError
            // 
            this.averageChanceError.Location = new System.Drawing.Point(6, 19);
            this.averageChanceError.Name = "averageChanceError";
            this.averageChanceError.Size = new System.Drawing.Size(121, 20);
            this.averageChanceError.TabIndex = 0;
            this.averageChanceError.Text = "0,0";
            this.averageChanceError.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnlyNumbersDrobes_KeyPress);

            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.errorThreadLength);
            this.groupBox4.Location = new System.Drawing.Point(12, 130);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(137, 50);
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Длина потока ошибок";
            // 
            // errorThreadLength
            // 
            this.errorThreadLength.Location = new System.Drawing.Point(6, 19);
            this.errorThreadLength.Name = "errorThreadLength";
            this.errorThreadLength.Size = new System.Drawing.Size(121, 20);
            this.errorThreadLength.TabIndex = 0;
            this.errorThreadLength.Text = "992";
            this.errorThreadLength.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnlyNumbers_KeyPress);

            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.Speed_CB);
            this.groupBox3.Location = new System.Drawing.Point(155, 74);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(137, 50);
            this.groupBox3.TabIndex = 10;
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

            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.TS_CB);
            this.groupBox2.Location = new System.Drawing.Point(12, 74);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(137, 50);
            this.groupBox2.TabIndex = 9;
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

            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Channel1_CB);
            this.groupBox1.Location = new System.Drawing.Point(12, 18);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(137, 50);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Номер канала 1";
            // 
            // Channel1_CB
            // 
            this.Channel1_CB.FormattingEnabled = true;
            this.Channel1_CB.Location = new System.Drawing.Point(6, 19);
            this.Channel1_CB.Name = "Channel1_CB";
            this.Channel1_CB.Size = new System.Drawing.Size(121, 21);
            this.Channel1_CB.TabIndex = 0;

            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.Channel2_CB);
            this.groupBox7.Location = new System.Drawing.Point(155, 18);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(137, 50);
            this.groupBox7.TabIndex = 16;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Номер канала 2";
            // 
            // Channel2_CB
            // 
            this.Channel2_CB.FormattingEnabled = true;
            this.Channel2_CB.Location = new System.Drawing.Point(6, 19);
            this.Channel2_CB.Name = "Channel2_CB";
            this.Channel2_CB.Size = new System.Drawing.Size(121, 21);
            this.Channel2_CB.TabIndex = 0;
            // 
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.SynchronizingObject = this;
            // 
            // TwoDevices
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 221);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.Start);
            this.Controls.Add(this.Back);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "TwoDevices";
            this.Text = "Искажение информации в квазидуплексном режиме";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TwoDevices_FormClosed);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Start;
        private System.Windows.Forms.Button Back;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox KoefficientGroup;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox averageChanceError;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox errorThreadLength;
        private System.Windows.Forms.GroupBox groupBox3;
        public System.Windows.Forms.ComboBox Speed_CB;
        private System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.ComboBox TS_CB;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.ComboBox Channel1_CB;
        private System.Windows.Forms.GroupBox groupBox7;
        public System.Windows.Forms.ComboBox Channel2_CB;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
    }
}