namespace test
{
    partial class GeneralTestForm
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
            this.PerevalTG_CB = new System.Windows.Forms.CheckBox();
            this.RS_withoutAnswer_CB = new System.Windows.Forms.CheckBox();
            this.PerevalFL_CB = new System.Windows.Forms.CheckBox();
            this.RS_withAnswer_CB = new System.Windows.Forms.CheckBox();
            this.CloseButton = new System.Windows.Forms.Button();
            this.nonRedundant_CB = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // PerevalTG_CB
            // 
            this.PerevalTG_CB.AutoSize = true;
            this.PerevalTG_CB.Checked = true;
            this.PerevalTG_CB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.PerevalTG_CB.Location = new System.Drawing.Point(32, 25);
            this.PerevalTG_CB.Name = "PerevalTG_CB";
            this.PerevalTG_CB.Size = new System.Drawing.Size(39, 17);
            this.PerevalTG_CB.TabIndex = 0;
            this.PerevalTG_CB.Text = "ТГ";
            this.PerevalTG_CB.UseVisualStyleBackColor = true;
            // 
            // RS_withoutAnswer_CB
            // 
            this.RS_withoutAnswer_CB.AutoSize = true;
            this.RS_withoutAnswer_CB.Checked = true;
            this.RS_withoutAnswer_CB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.RS_withoutAnswer_CB.Location = new System.Drawing.Point(32, 70);
            this.RS_withoutAnswer_CB.Name = "RS_withoutAnswer_CB";
            this.RS_withoutAnswer_CB.Size = new System.Drawing.Size(82, 17);
            this.RS_withoutAnswer_CB.TabIndex = 1;
            this.RS_withoutAnswer_CB.Text = "Без ответа";
            this.RS_withoutAnswer_CB.UseVisualStyleBackColor = true;
            // 
            // PerevalFL_CB
            // 
            this.PerevalFL_CB.AutoSize = true;
            this.PerevalFL_CB.Checked = true;
            this.PerevalFL_CB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.PerevalFL_CB.Location = new System.Drawing.Point(118, 25);
            this.PerevalFL_CB.Name = "PerevalFL_CB";
            this.PerevalFL_CB.Size = new System.Drawing.Size(45, 17);
            this.PerevalFL_CB.TabIndex = 2;
            this.PerevalFL_CB.Text = "ФЛ";
            this.PerevalFL_CB.UseVisualStyleBackColor = true;
            // 
            // RS_withAnswer_CB
            // 
            this.RS_withAnswer_CB.AutoSize = true;
            this.RS_withAnswer_CB.Checked = true;
            this.RS_withAnswer_CB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.RS_withAnswer_CB.Location = new System.Drawing.Point(118, 70);
            this.RS_withAnswer_CB.Name = "RS_withAnswer_CB";
            this.RS_withAnswer_CB.Size = new System.Drawing.Size(78, 17);
            this.RS_withAnswer_CB.TabIndex = 3;
            this.RS_withAnswer_CB.Text = "С ответом";
            this.RS_withAnswer_CB.UseVisualStyleBackColor = true;
            // 
            // CloseButton
            // 
            this.CloseButton.Location = new System.Drawing.Point(66, 140);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 23);
            this.CloseButton.TabIndex = 4;
            this.CloseButton.Text = "Старт";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // nonRedundant_CB
            // 
            this.nonRedundant_CB.AutoSize = true;
            this.nonRedundant_CB.Checked = true;
            this.nonRedundant_CB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.nonRedundant_CB.Location = new System.Drawing.Point(66, 103);
            this.nonRedundant_CB.Name = "nonRedundant_CB";
            this.nonRedundant_CB.Size = new System.Drawing.Size(82, 17);
            this.nonRedundant_CB.TabIndex = 5;
            this.nonRedundant_CB.Text = "Без ответа";
            this.nonRedundant_CB.UseVisualStyleBackColor = true;
            // 
            // GeneralTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(231, 175);
            this.Controls.Add(this.nonRedundant_CB);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.RS_withAnswer_CB);
            this.Controls.Add(this.PerevalFL_CB);
            this.Controls.Add(this.RS_withoutAnswer_CB);
            this.Controls.Add(this.PerevalTG_CB);
            this.Name = "GeneralTestForm";
            this.Text = "GeneralTestForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox PerevalTG_CB;
        private System.Windows.Forms.CheckBox RS_withoutAnswer_CB;
        private System.Windows.Forms.CheckBox PerevalFL_CB;
        private System.Windows.Forms.CheckBox RS_withAnswer_CB;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.CheckBox nonRedundant_CB;
    }
}