namespace SodWinForms
{
    partial class OneDevice
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Channel1_CB = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.TS_CB = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Speed_CB = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.errorThreadLength = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.averageChanceError = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.KoefficientGroup = new System.Windows.Forms.TextBox();
            this.Back = new System.Windows.Forms.Button();
            this.Start = new System.Windows.Forms.Button();
            this.testMessageTothe2ndChannel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Channel1_CB);
            this.groupBox1.Location = new System.Drawing.Point(12, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(137, 50);
            this.groupBox1.TabIndex = 0;
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
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.TS_CB);
            this.groupBox2.Location = new System.Drawing.Point(155, 7);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(137, 50);
            this.groupBox2.TabIndex = 1;
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
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.Speed_CB);
            this.groupBox3.Location = new System.Drawing.Point(298, 7);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(137, 50);
            this.groupBox3.TabIndex = 2;
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
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.errorThreadLength);
            this.groupBox4.Location = new System.Drawing.Point(12, 63);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(137, 50);
            this.groupBox4.TabIndex = 3;
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
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.averageChanceError);
            this.groupBox5.Location = new System.Drawing.Point(155, 63);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(172, 50);
            this.groupBox5.TabIndex = 4;
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
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.KoefficientGroup);
            this.groupBox6.Location = new System.Drawing.Point(333, 63);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(171, 50);
            this.groupBox6.TabIndex = 5;
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
            // Back
            // 
            this.Back.Location = new System.Drawing.Point(12, 121);
            this.Back.Name = "Back";
            this.Back.Size = new System.Drawing.Size(75, 23);
            this.Back.TabIndex = 6;
            this.Back.Text = "Отмена";
            this.Back.UseVisualStyleBackColor = true;
            this.Back.Click += new System.EventHandler(this.Back_Click);
            // 
            // Start
            // 
            this.Start.Location = new System.Drawing.Point(116, 121);
            this.Start.Name = "Start";
            this.Start.Size = new System.Drawing.Size(75, 23);
            this.Start.TabIndex = 7;
            this.Start.Text = "Старт";
            this.Start.UseVisualStyleBackColor = true;
            this.Start.Click += new System.EventHandler(this.Start_Click);
            // 
            // testMessageTothe2ndChannel
            // 
            this.testMessageTothe2ndChannel.Enabled = false;
            this.testMessageTothe2ndChannel.Location = new System.Drawing.Point(368, 120);
            this.testMessageTothe2ndChannel.Name = "testMessageTothe2ndChannel";
            this.testMessageTothe2ndChannel.Size = new System.Drawing.Size(75, 23);
            this.testMessageTothe2ndChannel.TabIndex = 8;
            this.testMessageTothe2ndChannel.Text = "Проверить";
            this.testMessageTothe2ndChannel.UseVisualStyleBackColor = true;
            this.testMessageTothe2ndChannel.Click += new System.EventHandler(this.TestMessageTothe2ndChannel_Click);
            // 
            // OneDevice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 156);
            this.Controls.Add(this.testMessageTothe2ndChannel);
            this.Controls.Add(this.Start);
            this.Controls.Add(this.Back);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "OneDevice";
            this.Text = "Искажение информации";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OneDevice_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.ComboBox Channel1_CB;
        private System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.ComboBox TS_CB;
        private System.Windows.Forms.GroupBox groupBox3;
        public System.Windows.Forms.ComboBox Speed_CB;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox errorThreadLength;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox averageChanceError;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox KoefficientGroup;
        private System.Windows.Forms.Button Back;
        private System.Windows.Forms.Button Start;
        private System.Windows.Forms.Button testMessageTothe2ndChannel;
    }
}