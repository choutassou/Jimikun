namespace Jimikun
{
    public class FailureReasonForm : Form
    {
        private TextBox txtReason;
        private Button btnCancel;
        private Button btnExit;
        // 失敗理由を格納するプロパティ
        public string FailureReason { get; private set; }
        public FailureReasonForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FailureReasonForm));
            txtReason = new TextBox();
            btnCancel = new Button();
            btnExit = new Button();
            SuspendLayout();
            // 
            // txtReason
            // 
            txtReason.Location = new Point(12, 37);
            txtReason.Name = "txtReason";
            txtReason.Size = new Size(865, 31);
            txtReason.TabIndex = 0;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(672, 95);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(205, 63);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "キャンセル";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnExit
            // 
            btnExit.BackColor = SystemColors.Highlight;
            btnExit.Font = new Font("Yu Gothic UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 128);
            btnExit.ForeColor = SystemColors.HighlightText;
            btnExit.Location = new Point(409, 95);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(247, 63);
            btnExit.TabIndex = 2;
            btnExit.Text = "手順を中止";
            btnExit.UseVisualStyleBackColor = false;
            btnExit.Click += btnExit_Click;
            // 
            // FailureReasonForm
            // 
            ClientSize = new Size(889, 172);
            Controls.Add(txtReason);
            Controls.Add(btnCancel);
            Controls.Add(btnExit);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FailureReasonForm";
            Text = "手順中止の理由をください";
            ResumeLayout(false);
            PerformLayout();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Falseを返してフォームを閉じる
            this.DialogResult = DialogResult.Cancel;
            // メインフォームに戻る
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            // レポートに失敗原因を記録しツールを終了
            FailureReason = txtReason.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ReportFailure(string reason)
        {
            string reportPath = "path_to_report_file"; // 実際のレポートファイルパスを指定
            string message = $"手順は{reason}の原因で中止されました";
            File.AppendAllText(reportPath, message + Environment.NewLine);
        }
    }
}