using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using webcrawler.Logs;

namespace webcrawler
{
    /// <summary>
    /// Interaction logic for HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow : Window
    {
        System.Windows.Controls.Button crawl;
        Database database = new Database();
        public HistoryWindow(System.Windows.Controls.Button crawl_button)
        {
            crawl = crawl_button;
            InitializeComponent();
            List<string> table_names = database.GetDatabaseNames();
            foreach (string table_name in table_names)
            {
                table_listbox.Items.Add(table_name);
            }
        }

        private async void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (table_listbox.SelectedItem != null)
            {
                var selectedItem = table_listbox.SelectedItem.ToString();
                if (selectedItem != null)
                {
                    await StartPythonScriptAsync(selectedItem);
                }
            }
        }
        private async Task StartPythonScriptAsync(string tableName)
        {
            string pythonExePath = @"C:\Users\Ulas Tan\AppData\Local\Programs\Python\Python311\python.exe";
            string pythonScriptPath = @"tree_visualize.py";
            string pythonScriptPathQuoted = $"\"{pythonScriptPath}\"";
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = pythonExePath,
                Arguments = $"{pythonScriptPathQuoted} {tableName}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();

                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                Logger logger = new Logger();
                logger.Warning(error);

                await Task.Run(() => process.WaitForExit());
            }
        }

        private void HistoryWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            crawl.IsEnabled = true;
        }
    }
}
