namespace WizardApp;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        // ����������Ă��邩���m�F
        if (args.Length < 1)
        {
            MessageBox.Show("�g�p���@: Wizard.exe <CSVpath>");
            return;
        }
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new Wizard(args[0]));
    }    
}