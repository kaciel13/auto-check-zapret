namespace auto_check_zapret
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            mainLayoutPanel = new TableLayoutPanel();
            downloadButton = new Button();
            choiceVersionComboBox = new ComboBox();
            zapretInstallButton = new Button();
            removeZapretButton = new Button();
            startTestButton = new Button();
            progressBar = new ProgressBar();
            trueChoiceComboBox = new ComboBox();
            screenPanel = new Panel();
            infoTextBox = new TextBox();
            mainLayoutPanel.SuspendLayout();
            screenPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainLayoutPanel
            // 
            mainLayoutPanel.BackColor = Color.FromArgb(60, 60, 55);
            mainLayoutPanel.ColumnCount = 12;
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8.333333F));
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8.333333F));
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8.333333F));
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8.333333F));
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8.333333F));
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8.333333F));
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8.333333F));
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8.333333F));
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8.333333F));
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8.333333F));
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8.333333F));
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8.333333F));
            mainLayoutPanel.Controls.Add(downloadButton, 6, 0);
            mainLayoutPanel.Controls.Add(choiceVersionComboBox, 0, 0);
            mainLayoutPanel.Controls.Add(zapretInstallButton, 0, 12);
            mainLayoutPanel.Controls.Add(removeZapretButton, 3, 12);
            mainLayoutPanel.Controls.Add(startTestButton, 0, 10);
            mainLayoutPanel.Controls.Add(progressBar, 0, 9);
            mainLayoutPanel.Controls.Add(trueChoiceComboBox, 0, 11);
            mainLayoutPanel.Controls.Add(screenPanel, 0, 1);
            mainLayoutPanel.Dock = DockStyle.Fill;
            mainLayoutPanel.Font = new Font("LCD5x8H", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            mainLayoutPanel.Location = new Point(0, 0);
            mainLayoutPanel.Margin = new Padding(0);
            mainLayoutPanel.Name = "mainLayoutPanel";
            mainLayoutPanel.RowCount = 13;
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 7.69230747F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 7.69230747F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 7.69230747F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 7.69230747F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 7.69230747F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 7.69230747F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 7.69230747F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 7.69230747F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 7.69230747F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 7.69230747F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 7.69230747F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 7.69230747F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 7.69230747F));
            mainLayoutPanel.Size = new Size(522, 449);
            mainLayoutPanel.TabIndex = 0;
            // 
            // downloadButton
            // 
            downloadButton.BackColor = Color.FromArgb(110, 110, 100);
            mainLayoutPanel.SetColumnSpan(downloadButton, 6);
            downloadButton.Dock = DockStyle.Fill;
            downloadButton.Enabled = false;
            downloadButton.FlatStyle = FlatStyle.Popup;
            downloadButton.Font = new Font("Microsoft Yi Baiti", 12F);
            downloadButton.ForeColor = Color.White;
            downloadButton.Location = new Point(262, 8);
            downloadButton.Margin = new Padding(4, 8, 10, 0);
            downloadButton.Name = "downloadButton";
            downloadButton.Size = new Size(250, 26);
            downloadButton.TabIndex = 7;
            downloadButton.Text = "Загрузка...";
            downloadButton.UseVisualStyleBackColor = false;
            downloadButton.Click += downloadButton_Click;
            // 
            // choiceVersionComboBox
            // 
            choiceVersionComboBox.BackColor = Color.Gainsboro;
            mainLayoutPanel.SetColumnSpan(choiceVersionComboBox, 6);
            choiceVersionComboBox.Dock = DockStyle.Fill;
            choiceVersionComboBox.FlatStyle = FlatStyle.Popup;
            choiceVersionComboBox.Font = new Font("Unispace", 11.25F, FontStyle.Bold);
            choiceVersionComboBox.ForeColor = Color.Black;
            choiceVersionComboBox.FormattingEnabled = true;
            choiceVersionComboBox.Location = new Point(10, 8);
            choiceVersionComboBox.Margin = new Padding(10, 8, 4, 0);
            choiceVersionComboBox.Name = "choiceVersionComboBox";
            choiceVersionComboBox.Size = new Size(244, 26);
            choiceVersionComboBox.TabIndex = 1;
            choiceVersionComboBox.SelectedIndexChanged += choiceVersionComboBox_SelectedIndexChanged;
            // 
            // zapretInstallButton
            // 
            zapretInstallButton.BackColor = Color.FromArgb(110, 110, 100);
            mainLayoutPanel.SetColumnSpan(zapretInstallButton, 3);
            zapretInstallButton.Dock = DockStyle.Fill;
            zapretInstallButton.Enabled = false;
            zapretInstallButton.FlatStyle = FlatStyle.Popup;
            zapretInstallButton.Font = new Font("Microsoft Yi Baiti", 12F);
            zapretInstallButton.ForeColor = Color.White;
            zapretInstallButton.Location = new Point(10, 416);
            zapretInstallButton.Margin = new Padding(10, 8, 0, 8);
            zapretInstallButton.Name = "zapretInstallButton";
            zapretInstallButton.Size = new Size(119, 25);
            zapretInstallButton.TabIndex = 4;
            zapretInstallButton.Text = "Включить";
            zapretInstallButton.UseVisualStyleBackColor = false;
            zapretInstallButton.Click += zapretInstallButton_Click;
            // 
            // removeZapretButton
            // 
            removeZapretButton.BackColor = Color.FromArgb(110, 110, 100);
            mainLayoutPanel.SetColumnSpan(removeZapretButton, 3);
            removeZapretButton.Dock = DockStyle.Fill;
            removeZapretButton.Enabled = false;
            removeZapretButton.FlatStyle = FlatStyle.Popup;
            removeZapretButton.Font = new Font("Microsoft Yi Baiti", 12F);
            removeZapretButton.ForeColor = Color.White;
            removeZapretButton.Location = new Point(133, 416);
            removeZapretButton.Margin = new Padding(4, 8, 4, 8);
            removeZapretButton.Name = "removeZapretButton";
            removeZapretButton.Size = new Size(121, 25);
            removeZapretButton.TabIndex = 5;
            removeZapretButton.Text = "Отключить";
            removeZapretButton.UseVisualStyleBackColor = false;
            removeZapretButton.Click += removeZapretButton_Click;
            // 
            // startTestButton
            // 
            startTestButton.BackColor = Color.FromArgb(110, 110, 100);
            mainLayoutPanel.SetColumnSpan(startTestButton, 6);
            startTestButton.Dock = DockStyle.Fill;
            startTestButton.Enabled = false;
            startTestButton.FlatStyle = FlatStyle.Popup;
            startTestButton.Font = new Font("Microsoft Yi Baiti", 12F);
            startTestButton.ForeColor = Color.White;
            startTestButton.Location = new Point(10, 348);
            startTestButton.Margin = new Padding(10, 8, 4, 0);
            startTestButton.Name = "startTestButton";
            startTestButton.Size = new Size(244, 26);
            startTestButton.TabIndex = 6;
            startTestButton.Text = "Запуск проверки";
            startTestButton.UseVisualStyleBackColor = false;
            startTestButton.Click += startTest_Click;
            // 
            // progressBar
            // 
            mainLayoutPanel.SetColumnSpan(progressBar, 12);
            progressBar.Dock = DockStyle.Fill;
            progressBar.Location = new Point(10, 314);
            progressBar.Margin = new Padding(10, 8, 10, 0);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(502, 26);
            progressBar.TabIndex = 8;
            // 
            // trueChoiceComboBox
            // 
            trueChoiceComboBox.BackColor = Color.Gainsboro;
            mainLayoutPanel.SetColumnSpan(trueChoiceComboBox, 6);
            trueChoiceComboBox.Dock = DockStyle.Fill;
            trueChoiceComboBox.FlatStyle = FlatStyle.Popup;
            trueChoiceComboBox.Font = new Font("Unispace", 11.25F, FontStyle.Bold);
            trueChoiceComboBox.ForeColor = Color.Black;
            trueChoiceComboBox.FormattingEnabled = true;
            trueChoiceComboBox.Location = new Point(10, 382);
            trueChoiceComboBox.Margin = new Padding(10, 8, 4, 0);
            trueChoiceComboBox.Name = "trueChoiceComboBox";
            trueChoiceComboBox.Size = new Size(244, 26);
            trueChoiceComboBox.TabIndex = 9;
            // 
            // screenPanel
            // 
            screenPanel.BackColor = Color.FromArgb(28, 38, 27);
            screenPanel.BorderStyle = BorderStyle.Fixed3D;
            mainLayoutPanel.SetColumnSpan(screenPanel, 12);
            screenPanel.Controls.Add(infoTextBox);
            screenPanel.Dock = DockStyle.Fill;
            screenPanel.ForeColor = Color.FromArgb(20, 181, 5);
            screenPanel.Location = new Point(10, 42);
            screenPanel.Margin = new Padding(10, 8, 10, 0);
            screenPanel.Name = "screenPanel";
            mainLayoutPanel.SetRowSpan(screenPanel, 8);
            screenPanel.Size = new Size(502, 264);
            screenPanel.TabIndex = 10;
            // 
            // infoTextBox
            // 
            infoTextBox.BackColor = Color.FromArgb(28, 38, 27);
            infoTextBox.BorderStyle = BorderStyle.None;
            infoTextBox.Font = new Font("LCD5x8H", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            infoTextBox.ForeColor = Color.FromArgb(20, 181, 5);
            infoTextBox.Location = new Point(17, 20);
            infoTextBox.Margin = new Padding(20);
            infoTextBox.Multiline = true;
            infoTextBox.Name = "infoTextBox";
            infoTextBox.Size = new Size(461, 220);
            infoTextBox.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(10, 10, 10);
            ClientSize = new Size(522, 449);
            Controls.Add(mainLayoutPanel);
            Margin = new Padding(4, 3, 4, 3);
            MaximumSize = new Size(538, 488);
            MinimumSize = new Size(538, 488);
            Name = "Form1";
            Text = "AutoCheckZapret";
            mainLayoutPanel.ResumeLayout(false);
            screenPanel.ResumeLayout(false);
            screenPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel mainLayoutPanel;
        private ComboBox choiceVersionComboBox;
        private Button downloadButton;
        private Button zapretInstallButton;
        private Button removeZapretButton;
        private Button startTestButton;
        private ProgressBar progressBar;
        private ComboBox trueChoiceComboBox;
        private Panel screenPanel;
        private TextBox infoTextBox;
    }
}
