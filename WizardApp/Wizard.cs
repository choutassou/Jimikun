using System.Data;

public class Wizard : Form
{
    private Label stepLabel;
    private TextBox operationTextBox;
    private Label warningLabel;
    private Button nextButton;

    private DataTable stepsTable;
    private int currentStepIndex;
    private Capture cap;

    public Wizard(string csvPath)
    {
        // Initialize the form elements
        InitializeComponent();

        // Load the CSV file into a DataTable
        LoadCsv(csvPath);
        currentStepIndex = 0;
        cap = new Capture();
        ShowStep(currentStepIndex);
    }

    private void InitializeComponent()
    {
        stepLabel = new Label();
        operationTextBox = new TextBox();
        warningLabel = new Label();
        nextButton = new Button();
        SuspendLayout();
        // 
        // stepLabel
        // 
        stepLabel.Font = new Font("Arial", 20F, FontStyle.Bold);
        stepLabel.Location = new Point(54, 33);
        stepLabel.Name = "stepLabel";
        stepLabel.Size = new Size(486, 70);
        stepLabel.TabIndex = 0;
        // 
        // operationTextBox
        // 
        operationTextBox.Font = new Font("Arial", 20F, FontStyle.Bold);
        operationTextBox.Location = new Point(54, 110);
        operationTextBox.Name = "operationTextBox";
        operationTextBox.ReadOnly = true;
        operationTextBox.Size = new Size(958, 31);
        operationTextBox.TabIndex = 1;
        // 
        // warningLabel
        // 
        warningLabel.Font = new Font("Arial", 24F, FontStyle.Bold);
        warningLabel.ForeColor = Color.Red;
        warningLabel.Location = new Point(54, 186);
        warningLabel.Name = "warningLabel";
        warningLabel.Size = new Size(945, 79);
        warningLabel.TabIndex = 2;
        // 
        // nextButton
        // 
        nextButton.Location = new Point(837, 490);
        nextButton.Name = "nextButton";
        nextButton.Size = new Size(217, 50);
        nextButton.TabIndex = 3;
        nextButton.Text = "次へ";
        nextButton.Click += NextButton_Click;
        // 
        // Wizard
        // 
        ClientSize = new Size(1078, 566);
        Controls.Add(stepLabel);
        Controls.Add(operationTextBox);
        Controls.Add(warningLabel);
        Controls.Add(nextButton);
        Name = "Wizard";
        Text = "Hi, Follow me!";
        ResumeLayout(false);
        PerformLayout();
    }

    private void LoadCsv(string filePath)
    {
        stepsTable = new DataTable();

        // 1. CSV ファイルの先頭行を読み込み、カラムを追加
        using (var reader = new StreamReader(filePath))
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
        if (index >= stepsTable.Rows.Count || string.IsNullOrWhiteSpace(stepsTable.Rows[index][0].ToString()))
        {
            MessageBox.Show("テストが終了しました");
            Application.Exit();
        }
        else
        {
            var row = stepsTable.Rows[index];
            stepLabel.Text = $"{row["Step大"]}-{row["Step小"]}";
            operationTextBox.Text = row["操作"].ToString();
            warningLabel.Text = $"{row["入力Window"]} {row["備考"]}";
            // クリップボードに warningLabel.Text の内容をコピー
            Clipboard.SetText(operationTextBox.Text);
        }
    }

    private void NextButton_Click(object sender, EventArgs e)
    {
        ExecuteCaptureCommands();
        currentStepIndex++;
        ShowStep(currentStepIndex);
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
                captureValue =  ".*" + captureValue + ".*";
                cap.Shoot(captureValue, row["キャプチャフォルダ"].ToString(), prefix);
            }
        }
    }
}
