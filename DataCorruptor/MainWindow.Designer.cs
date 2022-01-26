namespace SodWinForms
{
    partial class MainWindow
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
            this.LogRTB = new System.Windows.Forms.RichTextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.автопрокруткаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ClearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.режимРаботыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.искажениеИнформацииToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.искажениеИнформацииВКвазидуплексномРежимеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WritingToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SendFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.настройкиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.IpAdressTB = new System.Windows.Forms.ToolStripMenuItem();
            this.ipAdressDevice = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripIpPort = new System.Windows.Forms.ToolStripMenuItem();
            this.portDevice = new System.Windows.Forms.ToolStripTextBox();
            this.SFD = new System.Windows.Forms.SaveFileDialog();
            this.OFD = new System.Windows.Forms.OpenFileDialog();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LogRTB
            // 
            this.LogRTB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogRTB.ContextMenuStrip = this.contextMenuStrip1;
            this.LogRTB.Location = new System.Drawing.Point(12, 27);
            this.LogRTB.Name = "LogRTB";
            this.LogRTB.ReadOnly = true;
            this.LogRTB.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.LogRTB.Size = new System.Drawing.Size(726, 405);
            this.LogRTB.TabIndex = 0;
            this.LogRTB.Text = "";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.автопрокруткаToolStripMenuItem,
            this.toolStripSeparator1,
            this.ClearToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(162, 54);
            // 
            // автопрокруткаToolStripMenuItem
            // 
            this.автопрокруткаToolStripMenuItem.Checked = true;
            this.автопрокруткаToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.автопрокруткаToolStripMenuItem.Name = "автопрокруткаToolStripMenuItem";
            this.автопрокруткаToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.автопрокруткаToolStripMenuItem.Text = "Автопрокрутка";
            this.автопрокруткаToolStripMenuItem.Click += new System.EventHandler(this.АвтопрокруткаToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(158, 6);
            // 
            // ClearToolStripMenuItem
            // 
            this.ClearToolStripMenuItem.Name = "ClearToolStripMenuItem";
            this.ClearToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.ClearToolStripMenuItem.Text = "Очистить экран";
            this.ClearToolStripMenuItem.Click += new System.EventHandler(this.ClearToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.режимРаботыToolStripMenuItem,
            this.настройкиToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(750, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // режимРаботыToolStripMenuItem
            // 
            this.режимРаботыToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.искажениеИнформацииToolStripMenuItem,
            this.искажениеИнформацииВКвазидуплексномРежимеToolStripMenuItem,
            this.WritingToFileToolStripMenuItem,
            this.SendFromFileToolStripMenuItem});
            this.режимРаботыToolStripMenuItem.Name = "режимРаботыToolStripMenuItem";
            this.режимРаботыToolStripMenuItem.Size = new System.Drawing.Size(101, 20);
            this.режимРаботыToolStripMenuItem.Text = "Режим работы";
            // 
            // искажениеИнформацииToolStripMenuItem
            // 
            this.искажениеИнформацииToolStripMenuItem.Name = "искажениеИнформацииToolStripMenuItem";
            this.искажениеИнформацииToolStripMenuItem.Size = new System.Drawing.Size(368, 22);
            this.искажениеИнформацииToolStripMenuItem.Text = "Искажение информации";
            this.искажениеИнформацииToolStripMenuItem.Click += new System.EventHandler(this.ИскажениеИнформацииToolStripMenuItem_Click);
            // 
            // искажениеИнформацииВКвазидуплексномРежимеToolStripMenuItem
            // 
            this.искажениеИнформацииВКвазидуплексномРежимеToolStripMenuItem.Name = "искажениеИнформацииВКвазидуплексномРежимеToolStripMenuItem";
            this.искажениеИнформацииВКвазидуплексномРежимеToolStripMenuItem.Size = new System.Drawing.Size(368, 22);
            this.искажениеИнформацииВКвазидуплексномРежимеToolStripMenuItem.Text = "Искажение информации в квазидуплексном режиме";
            this.искажениеИнформацииВКвазидуплексномРежимеToolStripMenuItem.Click += new System.EventHandler(this.ИскажениеИнформацииВКвазидуплексномРежимеToolStripMenuItem_Click);
            // 
            // WritingToFileToolStripMenuItem
            // 
            this.WritingToFileToolStripMenuItem.Name = "WritingToFileToolStripMenuItem";
            this.WritingToFileToolStripMenuItem.Size = new System.Drawing.Size(368, 22);
            this.WritingToFileToolStripMenuItem.Text = "Запись в файл";
            this.WritingToFileToolStripMenuItem.Click += new System.EventHandler(this.WritingToFileToolStripMenuItem_Click);
            // 
            // SendFromFileToolStripMenuItem
            // 
            this.SendFromFileToolStripMenuItem.Name = "SendFromFileToolStripMenuItem";
            this.SendFromFileToolStripMenuItem.Size = new System.Drawing.Size(368, 22);
            this.SendFromFileToolStripMenuItem.Text = "Передача из файла";
            this.SendFromFileToolStripMenuItem.Click += new System.EventHandler(this.SendFromFileToolStripMenuItem_Click);
            // 
            // настройкиToolStripMenuItem
            // 
            this.настройкиToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.IpAdressTB,
            this.toolStripIpPort});
            this.настройкиToolStripMenuItem.Name = "настройкиToolStripMenuItem";
            this.настройкиToolStripMenuItem.Size = new System.Drawing.Size(79, 20);
            this.настройкиToolStripMenuItem.Text = "Настройки";
            // 
            // IpAdressTB
            // 
            this.IpAdressTB.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ipAdressDevice});
            this.IpAdressTB.Name = "IpAdressTB";
            this.IpAdressTB.Size = new System.Drawing.Size(120, 22);
            this.IpAdressTB.Text = "IP Адрес";
            // 
            // ipAdressDevice
            // 
            this.ipAdressDevice.Name = "ipAdressDevice";
            this.ipAdressDevice.Size = new System.Drawing.Size(180, 23);
            this.ipAdressDevice.Text = "192.168.1.87";
            this.ipAdressDevice.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.IpAdressDevice_KeyPress);
            // 
            // toolStripIpPort
            // 
            this.toolStripIpPort.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.portDevice});
            this.toolStripIpPort.Name = "toolStripIpPort";
            this.toolStripIpPort.Size = new System.Drawing.Size(120, 22);
            this.toolStripIpPort.Text = "Порт";
            // 
            // portDevice
            // 
            this.portDevice.Name = "portDevice";
            this.portDevice.Size = new System.Drawing.Size(180, 23);
            this.portDevice.Text = "8001";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 444);
            this.Controls.Add(this.LogRTB);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "Имитатор радиолинии \"Перевал\"";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox LogRTB;
        private System.Windows.Forms.MenuStrip menuStrip1;
        public System.Windows.Forms.ToolStripMenuItem режимРаботыToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem искажениеИнформацииToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem искажениеИнформацииВКвазидуплексномРежимеToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem WritingToFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SendFromFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem настройкиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem IpAdressTB;
        private System.Windows.Forms.ToolStripMenuItem toolStripIpPort;
        private System.Windows.Forms.ToolStripTextBox ipAdressDevice;
        private System.Windows.Forms.ToolStripTextBox portDevice;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem автопрокруткаToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem ClearToolStripMenuItem;
        public System.Windows.Forms.SaveFileDialog SFD;
        public System.Windows.Forms.OpenFileDialog OFD;
    }
}

