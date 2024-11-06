namespace WizardApp;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        // 引数が足りているかを確認
        if (args.Length < 1)
        {
            MessageBox.Show("使用方法: Wizard.exe <CSVpath>");
            return;
        }
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new Wizard(args[0]));
    }    
}