using System.Text;

namespace Jimikun
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string csvPath = args.Length > 0 ? args[0] : LoadPathFromIni();

            ApplicationConfiguration.Initialize();
            csvPath = ShowCsvPathForm(csvPath);
            
            if (!string.IsNullOrEmpty(csvPath))
            {
                Application.Run(new Wizard(csvPath));
            }
        }

        static string LoadPathFromIni()
        {
            string iniPath = "jimi.ini";
            if (File.Exists(iniPath))
            {
                return File.ReadAllText(iniPath, Encoding.UTF8).Trim();
            }
            return string.Empty;
        }

        static string ShowCsvPathForm(string defaultPath)
        {
            using (CsvPathForm form = new CsvPathForm(defaultPath))
            {
                return form.ShowDialog() == DialogResult.OK ? form.CsvPath : string.Empty;
            }
        }
    }
}