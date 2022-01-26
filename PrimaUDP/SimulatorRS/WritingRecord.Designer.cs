namespace SimulatorRS
{
    partial class WritingRecord
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
            this.ChooseFolderButton = new System.Windows.Forms.Button();
            this.FilePath = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ChooseFolderButton
            // 
            this.ChooseFolderButton.Location = new System.Drawing.Point(12, 12);
            this.ChooseFolderButton.Name = "ChooseFolderButton";
            this.ChooseFolderButton.Size = new System.Drawing.Size(157, 23);
            this.ChooseFolderButton.TabIndex = 44;
            this.ChooseFolderButton.Text = "Выбрать папку для записи";
            this.ChooseFolderButton.UseVisualStyleBackColor = true;
            this.ChooseFolderButton.Click += new System.EventHandler(this.ChooseFolderButton_Click);
            // 
            // FilePath
            // 
            this.FilePath.Location = new System.Drawing.Point(12, 49);
            this.FilePath.Name = "FilePath";
            this.FilePath.Size = new System.Drawing.Size(382, 20);
            this.FilePath.TabIndex = 45;
            // 
            // WritingRecord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(513, 81);
            this.Controls.Add(this.FilePath);
            this.Controls.Add(this.ChooseFolderButton);
            this.Name = "WritingRecord";
            this.Text = "Параметры_Записи";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button ChooseFolderButton;
        public System.Windows.Forms.TextBox FilePath;
    }
}