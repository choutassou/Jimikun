using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;

namespace Jimikun
{

    public class CsvPathForm : Form
    {
        private TextBox csvPathTextBox;
        private Button okButton;
        private Button browseButton;
        private Button cancelButton;
        private Label label;

        public string CsvPath { get; private set; }

        public CsvPathForm(string defaultPath)
        {
            CsvPath = defaultPath;
            InitializeComponent();
            csvPathTextBox.Text = defaultPath;
            browseButton.Click += (sender, e) => OnBrowseButtonClick();
            okButton.Click += (sender, e) => OnOkButtonClick();
            okButton.Enabled = !string.IsNullOrWhiteSpace(CsvPath);
            csvPathTextBox.TextChanged += (sender, e) => okButton.Enabled = !string.IsNullOrWhiteSpace(csvPathTextBox.Text);
            cancelButton.Click += (sender, e) => Close();
        }

        private void OnOkButtonClick()
        {
            string csvPath = csvPathTextBox.Text;
            if (File.Exists(csvPath))
            {
                // 必要な列名
                var requiredColumns = new List<string>
                {
                    "Step大", "Step小", "操作", "注意事項", "操作Window",
                    "キャプチャフォルダ", "キャプチャー１", "キャプチャー２", "キャプチャー３", "キャプチャー４"
                };

                // CSVの列チェック
                var missingColumns = new List<string>();
                EncodingProvider provider = System.Text.CodePagesEncodingProvider.Instance;
                using (var reader = new StreamReader(csvPath, provider.GetEncoding("shift-jis")))
                {
                    string headerLine = reader.ReadLine();
                    if (headerLine != null)
                    {
                        var headers = headerLine.Split(',');
                        foreach (var column in requiredColumns)
                        {
                            if (!headers.Contains(column))
                            {
                                missingColumns.Add(column);
                            }
                        }
                    }
                }

                // 足りない列がある場合、エラーメッセージを表示
                if (missingColumns.Count > 0)
                {
                    string missingColumnsMessage = string.Join(", ", missingColumns);
                    MessageBox.Show($"指定ファイルのフォーマットは不正です。以下の列が必要です: {missingColumnsMessage}",
                                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 列が揃っている場合、処理を進む
                CsvPath = csvPath;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("指定された手順ファイルが見つかりません。再度入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CsvPathForm));
            label = new Label();
            csvPathTextBox = new TextBox();
            browseButton = new Button();
            okButton = new Button();
            cancelButton = new Button();
            SuspendLayout();
            // 
            // label
            // 
            label.Location = new Point(10, 20);
            label.Name = "label";
            label.Size = new Size(157, 31);
            label.TabIndex = 0;
            label.Text = "手順書場所：";
            // 
            // csvPathTextBox
            // 
            csvPathTextBox.Location = new Point(185, 20);
            csvPathTextBox.Name = "csvPathTextBox";
            csvPathTextBox.Size = new Size(717, 31);
            csvPathTextBox.TabIndex = 1;
            // 
            // browseButton
            // 
            browseButton.Location = new Point(927, 20);
            browseButton.Name = "browseButton";
            browseButton.Size = new Size(37, 31);
            browseButton.TabIndex = 2;
            browseButton.Text = "...";
            // 
            // okButton
            // 
            okButton.Location = new Point(754, 66);
            okButton.Name = "okButton";
            okButton.Size = new Size(100, 38);
            okButton.TabIndex = 3;
            okButton.Text = "OK";
            // 
            // cancelButton
            // 
            cancelButton.Location = new Point(860, 66);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(100, 38);
            cancelButton.TabIndex = 4;
            cancelButton.Text = "キャンセル";
            // 
            // CsvPathForm
            // 
            ClientSize = new Size(989, 121);
            Controls.Add(label);
            Controls.Add(csvPathTextBox);
            Controls.Add(browseButton);
            Controls.Add(okButton);
            Controls.Add(cancelButton);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CsvPathForm";
            Text = "じみくん";
            ResumeLayout(false);
            PerformLayout();
        }

        private void OnBrowseButtonClick()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    csvPathTextBox.Text = openFileDialog.FileName;
                }
            }
        }
    }
}


