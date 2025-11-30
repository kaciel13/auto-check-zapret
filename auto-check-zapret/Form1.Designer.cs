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
            autoModeButton = new Button();
            mainLayoutPanel.SuspendLayout();
            screenPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainLayoutPanel
            // 
            mainLayoutPanel.BackColor = Color.FromArgb(50, 50, 40);
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
            mainLayoutPanel.Controls.Add(autoModeButton, 6, 10);
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
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            mainLayoutPanel.Size = new Size(594, 588);
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
            downloadButton.Location = new Point(299, 11);
            downloadButton.Margin = new Padding(5, 11, 11, 0);
            downloadButton.Name = "downloadButton";
            downloadButton.Size = new Size(284, 34);
            downloadButton.TabIndex = 7;
            downloadButton.Text = "Загрузка...";
            downloadButton.UseVisualStyleBackColor = false;
            downloadButton.Click += downloadButton_Click;
            // 
            // choiceVersionComboBox
            // 
            choiceVersionComboBox.BackColor = Color.FromArgb(130, 130, 120);
            mainLayoutPanel.SetColumnSpan(choiceVersionComboBox, 6);
            choiceVersionComboBox.Dock = DockStyle.Fill;
            choiceVersionComboBox.FlatStyle = FlatStyle.Popup;
            choiceVersionComboBox.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Bold);
            choiceVersionComboBox.ForeColor = Color.White;
            choiceVersionComboBox.FormattingEnabled = true;
            choiceVersionComboBox.Location = new Point(11, 11);
            choiceVersionComboBox.Margin = new Padding(11, 11, 5, 0);
            choiceVersionComboBox.Name = "choiceVersionComboBox";
            choiceVersionComboBox.Size = new Size(278, 32);
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
            zapretInstallButton.Location = new Point(11, 551);
            zapretInstallButton.Margin = new Padding(11, 11, 0, 11);
            zapretInstallButton.Name = "zapretInstallButton";
            zapretInstallButton.Size = new Size(136, 26);
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
            removeZapretButton.Location = new Point(152, 551);
            removeZapretButton.Margin = new Padding(5, 11, 5, 11);
            removeZapretButton.Name = "removeZapretButton";
            removeZapretButton.Size = new Size(137, 26);
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
            startTestButton.Location = new Point(11, 461);
            startTestButton.Margin = new Padding(11, 11, 5, 0);
            startTestButton.Name = "startTestButton";
            startTestButton.Size = new Size(278, 34);
            startTestButton.TabIndex = 6;
            startTestButton.Text = "Запуск проверки";
            startTestButton.UseVisualStyleBackColor = false;
            startTestButton.Click += startTest_Click;
            // 
            // progressBar
            // 
            mainLayoutPanel.SetColumnSpan(progressBar, 12);
            progressBar.Dock = DockStyle.Fill;
            progressBar.Location = new Point(11, 416);
            progressBar.Margin = new Padding(11, 11, 11, 0);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(572, 34);
            progressBar.TabIndex = 8;
            // 
            // trueChoiceComboBox
            // 
            trueChoiceComboBox.BackColor = Color.FromArgb(130, 130, 120);
            mainLayoutPanel.SetColumnSpan(trueChoiceComboBox, 6);
            trueChoiceComboBox.Dock = DockStyle.Fill;
            trueChoiceComboBox.FlatStyle = FlatStyle.Popup;
            trueChoiceComboBox.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Bold);
            trueChoiceComboBox.ForeColor = Color.White;
            trueChoiceComboBox.FormattingEnabled = true;
            trueChoiceComboBox.Location = new Point(11, 506);
            trueChoiceComboBox.Margin = new Padding(11, 11, 5, 0);
            trueChoiceComboBox.Name = "trueChoiceComboBox";
            trueChoiceComboBox.Size = new Size(278, 32);
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
            screenPanel.Location = new Point(11, 56);
            screenPanel.Margin = new Padding(11, 11, 11, 0);
            screenPanel.Name = "screenPanel";
            mainLayoutPanel.SetRowSpan(screenPanel, 8);
            screenPanel.Size = new Size(572, 349);
            screenPanel.TabIndex = 10;
            // 
            // infoTextBox
            // 
            infoTextBox.BackColor = Color.FromArgb(28, 38, 27);
            infoTextBox.BorderStyle = BorderStyle.None;
            infoTextBox.Font = new Font("LCD5x8H", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            infoTextBox.ForeColor = Color.FromArgb(20, 181, 5);
            infoTextBox.Location = new Point(19, 27);
            infoTextBox.Margin = new Padding(23, 27, 23, 27);
            infoTextBox.Multiline = true;
            infoTextBox.Name = "infoTextBox";
            infoTextBox.ReadOnly = true;
            infoTextBox.Size = new Size(527, 293);
            infoTextBox.TabIndex = 0;
            // 
            // autoModeButton
            // 
            autoModeButton.BackColor = Color.FromArgb(110, 110, 100);
            mainLayoutPanel.SetColumnSpan(autoModeButton, 6);
            autoModeButton.Dock = DockStyle.Fill;
            autoModeButton.FlatAppearance.BorderSize = 0;
            autoModeButton.FlatStyle = FlatStyle.Flat;
            autoModeButton.Font = new Font("Microsoft Yi Baiti", 12F);
            autoModeButton.ForeColor = Color.White;
            autoModeButton.Location = new Point(299, 461);
            autoModeButton.Margin = new Padding(5, 11, 11, 0);
            autoModeButton.Name = "autoModeButton";
            autoModeButton.Size = new Size(284, 34);
            autoModeButton.TabIndex = 11;
            autoModeButton.Text = "Подобрать автоматический";
            autoModeButton.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(10, 10, 10);
            ClientSize = new Size(594, 588);
            Controls.Add(mainLayoutPanel);
            Margin = new Padding(5, 4, 5, 4);
            MaximumSize = new Size(612, 635);
            MinimumSize = new Size(612, 635);
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
        private Button autoModeButton;
    }
}
