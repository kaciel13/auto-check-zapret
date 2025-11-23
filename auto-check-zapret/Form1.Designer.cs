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
            button1 = new Button();
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
            mainTableLayout.Name = "mainTableLayout";
            mainTableLayout.RowCount = 4;
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 28.0575542F));
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 71.9424438F));
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 342F));
            mainTableLayout.Size = new Size(484, 511);
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
            tableLayoutPanel1.Location = new Point(3, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(478, 33);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // choiceVersionComboBox
            // 
            choiceVersionComboBox.Dock = DockStyle.Fill;
            choiceVersionComboBox.Font = new Font("Unispace", 11.9999981F, FontStyle.Bold, GraphicsUnit.Point, 0);
            choiceVersionComboBox.FormattingEnabled = true;
            choiceVersionComboBox.Location = new Point(3, 3);
            choiceVersionComboBox.Name = "choiceVersionComboBox";
            choiceVersionComboBox.Size = new Size(313, 27);
            choiceVersionComboBox.TabIndex = 0;
            choiceVersionComboBox.SelectedIndexChanged += choiceVersionComboBox_SelectedIndexChanged;
            // 
            // downloadButton
            // 
            downloadButton.Dock = DockStyle.Fill;
            downloadButton.Enabled = false;
            downloadButton.Location = new Point(322, 3);
            downloadButton.Name = "downloadButton";
            downloadButton.Size = new Size(153, 27);
            downloadButton.TabIndex = 1;
            downloadButton.Text = "Не выбрана версия";
            downloadButton.UseVisualStyleBackColor = true;
            downloadButton.Click += downloadButton_Click;
            // 
            // infoTextBox
            // 
            infoTextBox.Dock = DockStyle.Fill;
            infoTextBox.Location = new Point(3, 42);
            infoTextBox.Multiline = true;
            infoTextBox.Name = "infoTextBox";
            infoTextBox.ReadOnly = true;
            infoTextBox.ScrollBars = ScrollBars.Vertical;
            infoTextBox.Size = new Size(478, 94);
            infoTextBox.TabIndex = 2;
            // 
            // progressBar
            // 
            progressBar.Dock = DockStyle.Fill;
            progressBar.Location = new Point(3, 142);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(478, 23);
            progressBar.TabIndex = 3;
            // 
            // checktTableLayout
            // 
            checktTableLayout.ColumnCount = 2;
            checktTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            checktTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            checktTableLayout.Controls.Add(button1, 0, 0);
            checktTableLayout.Dock = DockStyle.Fill;
            checktTableLayout.Location = new Point(3, 171);
            checktTableLayout.Name = "checktTableLayout";
            checktTableLayout.RowCount = 2;
            checktTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            checktTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            checktTableLayout.Size = new Size(478, 337);
            checktTableLayout.TabIndex = 4;
            // 
            // button1
            // 
            button1.Location = new Point(3, 3);
            button1.Name = "button1";
            button1.Size = new Size(233, 37);
            button1.TabIndex = 0;
            button1.Text = "Проверить все обходы";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(484, 511);
            Controls.Add(mainTableLayout);
            MaximumSize = new Size(500, 550);
            MinimumSize = new Size(500, 550);
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
    }
}
