namespace test
{
    partial class MessageBoxWithButtons
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
            this.leftButton = new System.Windows.Forms.Button();
            this.rightButton = new System.Windows.Forms.Button();
            this.text = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // leftButton
            // 
            this.leftButton.Location = new System.Drawing.Point(44, 117);
            this.leftButton.Name = "leftButton";
            this.leftButton.Size = new System.Drawing.Size(75, 23);
            this.leftButton.TabIndex = 0;
            this.leftButton.Text = "button1";
            this.leftButton.UseVisualStyleBackColor = true;
            this.leftButton.Click += new System.EventHandler(this.leftButton_Click);
            // 
            // rightButton
            // 
            this.rightButton.Location = new System.Drawing.Point(195, 117);
            this.rightButton.Name = "rightButton";
            this.rightButton.Size = new System.Drawing.Size(75, 23);
            this.rightButton.TabIndex = 1;
            this.rightButton.Text = "button2";
            this.rightButton.UseVisualStyleBackColor = true;
            this.rightButton.Click += new System.EventHandler(this.rightButton_Click);
            // 
            // text
            // 
            this.text.AutoSize = true;
            this.text.Location = new System.Drawing.Point(48, 59);
            this.text.Name = "text";
            this.text.Size = new System.Drawing.Size(179, 13);
            this.text.TabIndex = 2;
            this.text.Text = "sdasdsadsadsadsadsadfdsfdsfdsfdsf";
            // 
            // MessageBoxWithButtons
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 158);
            this.Controls.Add(this.text);
            this.Controls.Add(this.rightButton);
            this.Controls.Add(this.leftButton);
            this.Name = "MessageBoxWithButtons";
            this.Text = "MessageBoxWithButtons";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button leftButton;
        private System.Windows.Forms.Button rightButton;
        private System.Windows.Forms.Label text;
    }
}