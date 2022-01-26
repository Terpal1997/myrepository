namespace ComListener
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.Com1 = new System.Windows.Forms.ComboBox();
            this.Com2 = new System.Windows.Forms.ComboBox();
            this.Com3 = new System.Windows.Forms.ComboBox();
            this.Com4 = new System.Windows.Forms.ComboBox();
            this.Com5 = new System.Windows.Forms.ComboBox();
            this.Com6 = new System.Windows.Forms.ComboBox();
            this.Com7 = new System.Windows.Forms.ComboBox();
            this.Com8 = new System.Windows.Forms.ComboBox();
            this.StartListener = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.прекратитьВыводToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.AcceptsTab = true;
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.richTextBox1.Location = new System.Drawing.Point(12, 40);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.richTextBox1.Size = new System.Drawing.Size(776, 398);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // Com1
            // 
            this.Com1.FormattingEnabled = true;
            this.Com1.Location = new System.Drawing.Point(12, 13);
            this.Com1.Name = "Com1";
            this.Com1.Size = new System.Drawing.Size(73, 21);
            this.Com1.TabIndex = 1;
            // 
            // Com2
            // 
            this.Com2.FormattingEnabled = true;
            this.Com2.Location = new System.Drawing.Point(91, 13);
            this.Com2.Name = "Com2";
            this.Com2.Size = new System.Drawing.Size(73, 21);
            this.Com2.TabIndex = 2;
            // 
            // Com3
            // 
            this.Com3.FormattingEnabled = true;
            this.Com3.Location = new System.Drawing.Point(170, 13);
            this.Com3.Name = "Com3";
            this.Com3.Size = new System.Drawing.Size(73, 21);
            this.Com3.TabIndex = 3;
            // 
            // Com4
            // 
            this.Com4.FormattingEnabled = true;
            this.Com4.Location = new System.Drawing.Point(249, 13);
            this.Com4.Name = "Com4";
            this.Com4.Size = new System.Drawing.Size(73, 21);
            this.Com4.TabIndex = 4;
            // 
            // Com5
            // 
            this.Com5.FormattingEnabled = true;
            this.Com5.Location = new System.Drawing.Point(328, 13);
            this.Com5.Name = "Com5";
            this.Com5.Size = new System.Drawing.Size(73, 21);
            this.Com5.TabIndex = 5;
            // 
            // Com6
            // 
            this.Com6.FormattingEnabled = true;
            this.Com6.Location = new System.Drawing.Point(407, 13);
            this.Com6.Name = "Com6";
            this.Com6.Size = new System.Drawing.Size(73, 21);
            this.Com6.TabIndex = 6;
            // 
            // Com7
            // 
            this.Com7.FormattingEnabled = true;
            this.Com7.Location = new System.Drawing.Point(486, 13);
            this.Com7.Name = "Com7";
            this.Com7.Size = new System.Drawing.Size(73, 21);
            this.Com7.TabIndex = 7;
            // 
            // Com8
            // 
            this.Com8.FormattingEnabled = true;
            this.Com8.Location = new System.Drawing.Point(565, 13);
            this.Com8.Name = "Com8";
            this.Com8.Size = new System.Drawing.Size(73, 21);
            this.Com8.TabIndex = 8;
            // 
            // StartListener
            // 
            this.StartListener.Location = new System.Drawing.Point(658, 10);
            this.StartListener.Name = "StartListener";
            this.StartListener.Size = new System.Drawing.Size(108, 23);
            this.StartListener.TabIndex = 9;
            this.StartListener.Text = "Старт прослушки";
            this.StartListener.UseVisualStyleBackColor = true;
            this.StartListener.Click += new System.EventHandler(this.StartListener_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.прекратитьВыводToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(176, 26);
            // 
            // прекратитьВыводToolStripMenuItem
            // 
            this.прекратитьВыводToolStripMenuItem.CheckOnClick = true;
            this.прекратитьВыводToolStripMenuItem.Name = "прекратитьВыводToolStripMenuItem";
            this.прекратитьВыводToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.прекратитьВыводToolStripMenuItem.Text = "Прекратить вывод";
            this.прекратитьВыводToolStripMenuItem.Click += new System.EventHandler(this.ПрекратитьВыводToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.StartListener);
            this.Controls.Add(this.Com8);
            this.Controls.Add(this.Com7);
            this.Controls.Add(this.Com6);
            this.Controls.Add(this.Com5);
            this.Controls.Add(this.Com4);
            this.Controls.Add(this.Com3);
            this.Controls.Add(this.Com2);
            this.Controls.Add(this.Com1);
            this.Controls.Add(this.richTextBox1);
            this.Name = "Form1";
            this.Text = "ComListener";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.RichTextBox richTextBox1;
        public System.Windows.Forms.ComboBox Com1;
        public System.Windows.Forms.ComboBox Com2;
        public System.Windows.Forms.ComboBox Com3;
        public System.Windows.Forms.ComboBox Com4;
        public System.Windows.Forms.ComboBox Com5;
        public System.Windows.Forms.ComboBox Com6;
        public System.Windows.Forms.ComboBox Com7;
        public System.Windows.Forms.ComboBox Com8;
        private System.Windows.Forms.Button StartListener;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem прекратитьВыводToolStripMenuItem;
    }
}

