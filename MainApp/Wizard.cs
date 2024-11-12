using System;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace Jimikun
{
    public class Wizard : Form
    {
        private Label stepLabel;
        private TextBox operationTextBox;
        private Label warningLabel;
        private Button nextButton;

        private DataTable stepsTable;
        private int currentStepIndex;
        private Capture cap;
        private DateTime stepStartTime;
        private DateTime overallStartTime;
        private Dictionary<string, TimeSpan> majorStepTimes = new Dictionary<string, TimeSpan>();
        private List<string> logEntries = new List<string>();
        private Label windowLabel;
        private Button failureButton;
        private string csvPath;

        public Wizard(string csvPath)
        {
            // Initialize the form elements
            InitializeComponent();

            // Load the CSV file into a DataTable
            this.csvPath = csvPath;
            LoadCsv(csvPath);
            currentStepIndex = 0;
            cap = new Capture();
            overallStartTime = DateTime.Now;
            stepStartTime = overallStartTime;

            LogTrace("手順開始しました");
            ShowStep(currentStepIndex);
        }

        private void LogTrace(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string logMessage = $"{timestamp},{message}";
            Trace.WriteLine(logMessage);
            logEntries.Add(logMessage); // レポート用にエントリを保存
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Wizard));
            stepLabel = new Label();
            operationTextBox = new TextBox();
            warningLabel = new Label();
            nextButton = new Button();
            windowLabel = new Label();
            failureButton = new Button();
            SuspendLayout();
            // 
            // stepLabel
            // 
            stepLabel.Font = new Font("Arial", 16F, FontStyle.Bold);
            stepLabel.Location = new Point(21, 19);
            stepLabel.Name = "stepLabel";
            stepLabel.Size = new Size(492, 43);
            stepLabel.TabIndex = 0;
            // 
            // operationTextBox
            // 
            operationTextBox.Font = new Font("Arial", 16F, FontStyle.Bold);
            operationTextBox.Location = new Point(19, 76);
            operationTextBox.Name = "operationTextBox";
            operationTextBox.ReadOnly = true;
            operationTextBox.Size = new Size(964, 44);
            operationTextBox.TabIndex = 1;
            // 
            // warningLabel
            // 
            warningLabel.Font = new Font("Arial", 20F, FontStyle.Bold);
            warningLabel.ForeColor = Color.Red;
            warningLabel.Location = new Point(21, 208);
            warningLabel.Name = "warningLabel";
            warningLabel.Size = new Size(492, 49);
            warningLabel.TabIndex = 2;
            // 
            // nextButton
            // 
            nextButton.Location = new Point(760, 216);
            nextButton.Name = "nextButton";
            nextButton.Size = new Size(223, 50);
            nextButton.TabIndex = 3;
            nextButton.Text = "次へ";
            nextButton.Click += NextButton_Click;
            // 
            // windowLabel
            // 
            windowLabel.Font = new Font("Arial", 16F, FontStyle.Bold);
            windowLabel.ForeColor = Color.Black;
            windowLabel.Location = new Point(21, 139);
            windowLabel.Name = "windowLabel";
            windowLabel.Size = new Size(962, 49);
            windowLabel.TabIndex = 4;
            // 
            // failureButton
            // 
            failureButton.Location = new Point(519, 216);
            failureButton.Name = "failureButton";
            failureButton.Size = new Size(223, 50);
            failureButton.TabIndex = 5;
            failureButton.Text = "手順実施失敗";
            failureButton.Click += new EventHandler(FailureButton_Click);
            // 
            // Wizard
            // 
            ClientSize = new Size(997, 301);
            Controls.Add(failureButton);
            Controls.Add(windowLabel);
            Controls.Add(stepLabel);
            Controls.Add(operationTextBox);
            Controls.Add(warningLabel);
            Controls.Add(nextButton);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Wizard";
            Text = "じみくん";
            ResumeLayout(false);
            PerformLayout();
        }

        private void LoadCsv(string filePath)
        {
            stepsTable = new DataTable();
            EncodingProvider provider = System.Text.CodePagesEncodingProvider.Instance;
            // CSV ファイルを Shift_JIS エンコーディングで読み込む
            using (var reader = new StreamReader(filePath, provider.GetEncoding("shift-jis")))
            {
                // ヘッダー行を読み込み、カラムを定義
                string headerLine = reader.ReadLine();
                if (headerLine != null)
                {
                    var headers = headerLine.Split(',');
                    foreach (var header in headers)
                    {
                        stepsTable.Columns.Add(header);
                    }

                    // 残りの行を読み込み、DataTableに追加
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var values = line.Split(',');
                        stepsTable.Rows.Add(values);
                    }
                }
            }
        }

        private void ShowStep(int index)
        {
            var row = stepsTable.Rows[index];
            stepLabel.Text = $"手順番号：{row["Step大"]}-{row["Step小"]}";
            operationTextBox.Text = row["操作"].ToString();
            warningLabel.Text = $"{row["注意事項"]}";
            if (!string.IsNullOrWhiteSpace(row["操作Window"].ToString()))
            {
                windowLabel.Text = $"操作ウインドウ：{row["操作Window"]}";
            }
            else
            {
                windowLabel.Text = "";
            }
            // クリップボードに warningLabel.Text の内容をコピー
            Clipboard.SetText(operationTextBox.Text);
        }
        private void FailureButton_Click(object sender, EventArgs e)
        {
            // 失敗理由入力画面を表示
            FailureReasonForm failureForm = new FailureReasonForm();
            DateTime overallEndTime = DateTime.Now;
            TimeSpan overallDuration = overallEndTime - overallStartTime;
            failureForm.ShowDialog();
            if (failureForm.DialogResult == DialogResult.OK)
            {
                string failureReason = failureForm.FailureReason;
                LogTrace($"手順強制中止！中止理由: {failureReason}");
                GenerateReport(overallDuration, failureReason);
                Application.Exit();
            }
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            // キャプチャーを実行
            ExecuteCaptureCommands();

            // 現在のステップの情報を取得
            var row = stepsTable.Rows[currentStepIndex];
            string majorStep = row["Step大"].ToString();
            string minorStep = row["Step小"].ToString();

            // 現在のステップの所要時間を計算
            DateTime stepEndTime = DateTime.Now;
            TimeSpan stepDuration = stepEndTime - stepStartTime;
            stepStartTime = stepEndTime;

            // <次へ>押下時のログメッセージに所要時間（分単位）を追加
            LogTrace($"Step{majorStep}-{minorStep}画面,<次へ>押下,({FormatDuration(stepDuration)})");

            // 大ステップの所要時間を集計
            if (!majorStepTimes.ContainsKey(majorStep))
                majorStepTimes[majorStep] = TimeSpan.Zero;
            majorStepTimes[majorStep] += stepDuration;

            currentStepIndex++;

            // 手順完了時の処理
            if (currentStepIndex >= stepsTable.Rows.Count || string.IsNullOrWhiteSpace(stepsTable.Rows[currentStepIndex][0].ToString()))
            {
                DateTime overallEndTime = DateTime.Now;
                TimeSpan overallDuration = overallEndTime - overallStartTime;
                LogTrace("手順完了");

                GenerateReport(overallDuration, "");
                Application.Exit();
            }
            else
            {
                ShowStep(currentStepIndex);
            }
        }


        private string FormatDuration(TimeSpan duration)
        {
            if (duration.TotalMinutes < 1)
                return "1分以内";
            else
                return $"{Math.Ceiling(duration.TotalMinutes)}分";
        }

        private void GenerateReport(TimeSpan overallDuration, string cancelReason)
        {
            string pathWithoutExtension = Path.GetDirectoryName(csvPath);
            string reportPath = pathWithoutExtension + "\\OperationReport.txt";
            using (StreamWriter writer = new StreamWriter(reportPath))
            {
                writer.WriteLine("手順レポート");
                writer.WriteLine("==============================");

                // 各小ステップの所要時間
                writer.WriteLine("\n各Step(小)の所要時間:");
                foreach (var log in logEntries)
                {
                    writer.WriteLine(log);
                }

                // 各大ステップの所要時間
                writer.WriteLine("\n各Step(大)の所要時間:");
                foreach (var entry in majorStepTimes)
                {
                    writer.WriteLine($"Step {entry.Key}: {FormatDuration(entry.Value)}");
                }

                // 総所要時間
                writer.WriteLine("\n手順実施の総時間:");
                writer.WriteLine($"総時間: {FormatDuration(overallDuration)}");

                writer.WriteLine("==============================");
            }
            if (string.IsNullOrWhiteSpace(cancelReason))
            {
                MessageBox.Show($"お疲れ様です！\n手順完了し、レポートが出力されました: {reportPath}");
            } else {
                MessageBox.Show($"手順が強制中止されました。レポートが出力されました: {reportPath}");
            }
        }

        private void ExecuteCaptureCommands()
        {
            var row = stepsTable.Rows[currentStepIndex];
            for (int i = 1; i <= 4; i++)
            {
                string captureValue = row[$"キャプチャー{i}"].ToString();
                string prefix = row["Step大"].ToString() + "_" + row["Step小"].ToString();
                if (!string.IsNullOrWhiteSpace(captureValue))
                {
                    captureValue = ".*" + captureValue + ".*";
                    cap.Shoot(captureValue, row["キャプチャフォルダ"].ToString(), prefix);
                }
            }
        }
    }
}
