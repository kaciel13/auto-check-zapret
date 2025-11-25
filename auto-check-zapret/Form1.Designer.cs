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
            mainTableLayout = new TableLayoutPanel();
            tableLayoutPanel1 = new TableLayoutPanel();
            choiceVersionComboBox = new ComboBox();
            downloadButton = new Button();
            infoTextBox = new TextBox();
            progressBar = new ProgressBar();
            checktTableLayout = new TableLayoutPanel();
            removeZapretButton = new Button();
            button1 = new Button();
            trueChoiceComboBox = new ComboBox();
            installZapretButton = new Button();
            mainTableLayout.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            checktTableLayout.SuspendLayout();
            SuspendLayout();
            // 
            // mainTableLayout
            // 
            mainTableLayout.ColumnCount = 1;
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainTableLayout.Controls.Add(tableLayoutPanel1, 0, 0);
            mainTableLayout.Controls.Add(infoTextBox, 0, 1);
            mainTableLayout.Controls.Add(progressBar, 0, 2);
            mainTableLayout.Controls.Add(checktTableLayout, 0, 3);
            mainTableLayout.Dock = DockStyle.Fill;
            mainTableLayout.Location = new Point(0, 0);
            mainTableLayout.Margin = new Padding(4, 3, 4, 3);
            mainTableLayout.Name = "mainTableLayout";
            mainTableLayout.RowCount = 4;
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 12.2807016F));
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 87.7193F));
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 93F));
            mainTableLayout.Size = new Size(760, 478);
            mainTableLayout.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66.7364044F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.2636F));
            tableLayoutPanel1.Controls.Add(choiceVersionComboBox, 0, 0);
            tableLayoutPanel1.Controls.Add(downloadButton, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(4, 3);
            tableLayoutPanel1.Margin = new Padding(4, 3, 4, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(752, 37);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // choiceVersionComboBox
            // 
            choiceVersionComboBox.BackColor = Color.FromArgb(30, 30, 30);
            choiceVersionComboBox.Dock = DockStyle.Fill;
            choiceVersionComboBox.DropDownHeight = 300;
            choiceVersionComboBox.DropDownWidth = 357;
            choiceVersionComboBox.FlatStyle = FlatStyle.System;
            choiceVersionComboBox.Font = new Font("Microsoft Sans Serif", 18F, FontStyle.Regular, GraphicsUnit.Point, 204);
            choiceVersionComboBox.ForeColor = SystemColors.Window;
            choiceVersionComboBox.FormattingEnabled = true;
            choiceVersionComboBox.IntegralHeight = false;
            choiceVersionComboBox.ItemHeight = 29;
            choiceVersionComboBox.Location = new Point(0, 0);
            choiceVersionComboBox.Margin = new Padding(0);
            choiceVersionComboBox.MaxDropDownItems = 10;
            choiceVersionComboBox.Name = "choiceVersionComboBox";
            choiceVersionComboBox.Size = new Size(501, 37);
            choiceVersionComboBox.TabIndex = 0;
            choiceVersionComboBox.SelectedIndexChanged += choiceVersionComboBox_SelectedIndexChanged;
            // 
            // downloadButton
            // 
            downloadButton.BackColor = Color.FromArgb(64, 64, 64);
            downloadButton.Dock = DockStyle.Fill;
            downloadButton.Enabled = false;
            downloadButton.FlatAppearance.BorderSize = 0;
            downloadButton.FlatStyle = FlatStyle.Flat;
            downloadButton.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            downloadButton.ForeColor = Color.Silver;
            downloadButton.Location = new Point(505, 3);
            downloadButton.Margin = new Padding(4, 3, 4, 3);
            downloadButton.Name = "downloadButton";
            downloadButton.Size = new Size(243, 31);
            downloadButton.TabIndex = 1;
            downloadButton.Text = "Не выбрана версия";
            downloadButton.UseVisualStyleBackColor = false;
            downloadButton.Click += downloadButton_Click;
            // 
            // infoTextBox
            // 
            infoTextBox.BackColor = SystemColors.ActiveCaptionText;
            infoTextBox.BorderStyle = BorderStyle.None;
            infoTextBox.Dock = DockStyle.Fill;
            infoTextBox.Font = new Font("Microsoft Sans Serif", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 204);
            infoTextBox.ForeColor = SystemColors.ControlLightLight;
            infoTextBox.Location = new Point(4, 46);
            infoTextBox.Margin = new Padding(4, 3, 4, 3);
            infoTextBox.Multiline = true;
            infoTextBox.Name = "infoTextBox";
            infoTextBox.ReadOnly = true;
            infoTextBox.ScrollBars = ScrollBars.Vertical;
            infoTextBox.Size = new Size(752, 301);
            infoTextBox.TabIndex = 2;
            // 
            // progressBar
            // 
            progressBar.BackColor = Color.Black;
            progressBar.Dock = DockStyle.Fill;
            progressBar.ForeColor = Color.White;
            progressBar.Location = new Point(4, 353);
            progressBar.Margin = new Padding(4, 3, 4, 3);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(752, 28);
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.TabIndex = 3;
            // 
            // checktTableLayout
            // 
            checktTableLayout.BackColor = Color.FromArgb(25, 25, 25);
            checktTableLayout.ColumnCount = 2;
            checktTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            checktTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            checktTableLayout.Controls.Add(removeZapretButton, 1, 0);
            checktTableLayout.Controls.Add(button1, 0, 0);
            checktTableLayout.Controls.Add(trueChoiceComboBox, 0, 1);
            checktTableLayout.Controls.Add(installZapretButton, 1, 1);
            checktTableLayout.Dock = DockStyle.Fill;
            checktTableLayout.Location = new Point(4, 387);
            checktTableLayout.Margin = new Padding(4, 3, 4, 3);
            checktTableLayout.Name = "checktTableLayout";
            checktTableLayout.RowCount = 2;
            checktTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            checktTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            checktTableLayout.Size = new Size(752, 88);
            checktTableLayout.TabIndex = 4;
            // 
            // removeZapretButton
            // 
            removeZapretButton.Dock = DockStyle.Fill;
            removeZapretButton.Location = new Point(380, 3);
            removeZapretButton.Margin = new Padding(4, 3, 4, 3);
            removeZapretButton.Name = "removeZapretButton";
            removeZapretButton.Size = new Size(368, 38);
            removeZapretButton.TabIndex = 1;
            removeZapretButton.Text = "Отключить запрет";
            removeZapretButton.UseVisualStyleBackColor = true;
            removeZapretButton.Click += removeZapretButton_Click;
            // 
            // button1
            // 
            button1.Dock = DockStyle.Fill;
            button1.Location = new Point(4, 3);
            button1.Margin = new Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new Size(368, 38);
            button1.TabIndex = 0;
            button1.Text = "Проверить все обходы";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // trueChoiceComboBox
            // 
            trueChoiceComboBox.FormattingEnabled = true;
            trueChoiceComboBox.Location = new Point(3, 47);
            trueChoiceComboBox.Name = "trueChoiceComboBox";
            trueChoiceComboBox.Size = new Size(121, 23);
            trueChoiceComboBox.TabIndex = 2;
            // 
            // installZapretButton
            // 
            installZapretButton.Dock = DockStyle.Fill;
            installZapretButton.Location = new Point(379, 47);
            installZapretButton.Name = "installZapretButton";
            installZapretButton.Size = new Size(370, 38);
            installZapretButton.TabIndex = 3;
            installZapretButton.Text = "Запуск выбранного обхода";
            installZapretButton.UseVisualStyleBackColor = true;
            installZapretButton.Click += installZapretButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(10, 10, 10);
            ClientSize = new Size(760, 478);
            Controls.Add(mainTableLayout);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Margin = new Padding(4, 3, 4, 3);
            MaximumSize = new Size(776, 517);
            MinimumSize = new Size(776, 517);
            Name = "Form1";
            Text = "AutoCheckZapret";
            mainTableLayout.ResumeLayout(false);
            mainTableLayout.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            checktTableLayout.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel mainTableLayout;
        private ComboBox choiceVersionComboBox;
        private TableLayoutPanel tableLayoutPanel1;
        private Button downloadButton;
        private TextBox infoTextBox;
        private ProgressBar progressBar;
        private TableLayoutPanel checktTableLayout;
        private Button button1;
        private Button removeZapretButton;
        private ComboBox trueChoiceComboBox;
        private Button installZapretButton;
    }
}
