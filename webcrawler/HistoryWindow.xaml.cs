using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using webcrawler.Logs;
using static IronPython.SQLite.PythonSQLite;

namespace webcrawler
{
    /// <summary>
    /// Interaction logic for HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow : Window
    {

        System.Windows.Controls.Button crawl;
        Database database = new Database();
        string table_name;
        List<Dictionary<string, object>> rows;
        Logger logger = new Logger();
        public HistoryWindow(System.Windows.Controls.Button crawl_button)
        {
            crawl = crawl_button;
            InitializeComponent();
            load_tables();
        }
        private void load_tables()
        {
            table_listbox.Items.Clear();
            List<string> table_names = database.GetDatabaseNames();
            foreach (string table_name in table_names)
            {
                table_listbox.Items.Add(table_name);
            }
        }
        private  void table_listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (table_listbox.SelectedItem != null)
            {
                table_name = table_listbox.SelectedItem.ToString();
                if (table_name != null)
                {
                    rows = database.GetTable(table_name);
                    foreach (var row in rows)
                    {
                        if (row.TryGetValue("URLID", out var urlId))
                        {
                            table_content_listbox.Items.Add($"URL ID: {urlId.ToString()}");
                        } 
                    }
                    current_table_label.Content = table_name;
                    url_count_label.Content = rows.Count.ToString();
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
        private  void table_content_listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (table_content_listbox.SelectedItem != null)
            {
                var node = table_content_listbox.SelectedItem.ToString();
                if (node != null)
                {
                    int colonIndex = node.IndexOf(':');
                    if (colonIndex != -1)
                    {
                        int urlId;
                        if (Int32.TryParse(node.Substring(colonIndex + 2), out urlId))
                        {
                            foreach (var row in rows)
                            {
                                if (row.TryGetValue("URLID", out var rowUrlId) && rowUrlId is int rowUrlIdValue)
                                {
                                    if (rowUrlIdValue == urlId)
                                    {
                                        parent_id_label.Content = row.TryGetValue("ParentID", out var parentId) ? parentId.ToString() : "N/A";
                                        depth_label.Content = row.TryGetValue("Depth", out var depth) ? depth.ToString() : "N/A";
                                        spider_id_label.Content = row.TryGetValue("SpiderID", out var spiderId) ? spiderId.ToString() : "N/A";
                                        created_url_count__label.Content = row.TryGetValue("CreatedURLCount", out var createdUrlCount) ? createdUrlCount.ToString() : "N/A";
                                        url_address_label.Content = row.TryGetValue("URLAddress", out var urlAddress) ? urlAddress.ToString() : "N/A";
                                        founding_date_label.Content = row.TryGetValue("FoundingDate", out var foundingDate) ? foundingDate.ToString() : "N/A";
                                        crawling_date_label.Content = row.TryGetValue("CrawlingDate", out var crawlingDate) ? crawlingDate.ToString() : "N/A";
                                        is_failed_label.Content = row.TryGetValue("IsFailed", out var isFailed) ? isFailed.ToString() : "N/A";
                                        if (row.TryGetValue("FoundingDate", out var foundingDateValue) && foundingDateValue is DateTime foundingDateTime &&
                                            row.TryGetValue("CrawlingDate", out var crawlingDateValue) && crawlingDateValue is DateTime crawlingDateTime)
                                        {
                                            TimeSpan waitTime = crawlingDateTime - foundingDateTime;
                                            wait_time__label.Content = waitTime.ToString();
                                        }
                                        else
                                        {
                                            wait_time__label.Content = "N/A";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void HistoryWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            crawl.IsEnabled = true;
        }

        private async void visualize_button_Click(object sender, RoutedEventArgs e)
        {
            if(table_name != null)
            {
                await StartPythonScriptAsync(table_name);
            }
            else
            {
                MessageBox.Show("Choose a table from the left list box");
            }
        }

        private void download_csv_button_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder csvContent = new StringBuilder();
            if (rows.Count > 0)
            {
                foreach (var columnName in rows[0].Keys)
                {
                    csvContent.Append($"{columnName},");
                }
                csvContent.AppendLine();
            }
            foreach (var row in rows)
            {
                foreach (var value in row.Values)
                {
                    csvContent.Append($"{value},");
                }
                csvContent.AppendLine();
            }
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                FileName = "data.csv"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                File.WriteAllText(filePath, csvContent.ToString());
                MessageBox.Show("CSV file saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void goto_url_button_Click(object sender, RoutedEventArgs e)
        {
            if (url_address_label.Content != null && Uri.TryCreate(url_address_label.Content.ToString(), UriKind.Absolute, out var uri))
            {
                try
                {
                    System.Diagnostics.Process.Start(new ProcessStartInfo("cmd.exe", $"/c start {uri}") { CreateNoWindow = true });
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
        }

        private void delete_button_Click(object sender, RoutedEventArgs e)
        {
            Database database = new Database();
            database.DeleteTable(table_name);
            load_tables();
            table_name = null;
            parent_id_label.Content = null;
            depth_label.Content = null;
            spider_id_label.Content = null;
            created_url_count__label.Content = null;
            url_address_label.Content = null;
            crawling_date_label.Content = null;
            is_failed_label.Content = null;
            wait_time__label.Content = null;
            founding_date_label.Content= null;
            current_table_label.Content = null;
            url_count_label.Content = null;
            rows = null;
            table_content_listbox.Items.Clear();
        }
    }
}
