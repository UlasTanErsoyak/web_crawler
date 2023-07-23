// Ignore Spelling: img webcrawler
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using webcrawler.Logs;
namespace webcrawler
{
    public partial class MainWindow : Window
    {
        public readonly string settings_img_path = "src_img/settings.png";
        public readonly string crawl_img_path = "src_img/crawl_image.png";
        public readonly string log_img_path = "src_img/log.png";
        private readonly string log_path = AppDomain.CurrentDomain.BaseDirectory + "logs.txt";
        Logger logger = new Logger();
        public MainWindow()
        {
            InitializeComponent();
            spidercount_label.Content = (spider_slider.Value + 1).ToString();
        }
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            spidercount_label.Content = (spider_slider.Value+1).ToString();
        }
        private void advancedsettings_button_Click(object sender, RoutedEventArgs e)
        {
            AdvancedSettings settings_window;
            settings_window = new AdvancedSettings();
            settings_window.Visibility = Visibility.Hidden;
            settings_window.Visibility=Visibility.Visible;
        }
        private List<string> InitializeCrawling(string rootUrl)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string html = client.DownloadString(rootUrl);
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    var urls = doc.DocumentNode.SelectNodes("//a[@href]")
                        .Where(node => !node.Descendants("img").Any())
                        .Select(node => node.GetAttributeValue("href", ""))
                        .Where(href => Uri.TryCreate(href, UriKind.Absolute, out Uri uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)).ToList();
                    return urls;
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex);
                List<string> empty = new List<string>();
                
                return empty;
            }

        }
        private static bool HasImageChildNode(HtmlNode node)
        {
            return node.Descendants("img").Any();
        }
        private List<List<string>> DivideWorkload(List<string> urls, int numberOfSpiders)
        {
            if(urls.Count == 0)
            {
                return null;
            }
            int totalUrls = urls.Count;
            int workloadPerSpider = totalUrls / numberOfSpiders;
            List<List<string>> spiderWorkloads = new List<List<string>>();
            int startIndex = 0;
            for (int i = 0; i < numberOfSpiders; i++)
            {
                int endIndex = startIndex + workloadPerSpider;
                if (i < totalUrls % numberOfSpiders)
                {
                    endIndex++;
                }
                List<string> spiderWorkload = urls.GetRange(startIndex, endIndex - startIndex);
                spiderWorkloads.Add(spiderWorkload);
                startIndex = endIndex;
            }
            return spiderWorkloads;
        }
        private async void crawl_button_Click(object sender, RoutedEventArgs e)
        {
            if (vertical_crawl_radio.IsChecked == true)
            {
                await StartCrawling(false);
            }
            else if (horizontal_crawl_radio.IsChecked == true)
            {
                await StartCrawling(true);
            }
            else
            {
                MessageBox.Show("Select a crawling direction.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async Task StartCrawling(bool isHorizontal)
        {
            history_button.IsEnabled = false;
            string url_name = rooturl_textbox.Text;
            try
            {
                int numberOfSpiders;
                bool parseSuccess = Int32.TryParse(spidercount_label.Content.ToString(), out numberOfSpiders);
                crawled_url_listbox.Items.Clear();

                List<List<string>> spiderWorkloads = DivideWorkload(InitializeCrawling(rooturl_textbox.Text), numberOfSpiders);
                if (spiderWorkloads != null)
                {
                    List<Task> crawlTasks = new List<Task>();
                    URL root_url = new URL(-1, 0, 0, rooturl_textbox.Text);
                    root_url.CrawlingDate = DateTime.Now;
                    for (int i = 0; i < spiderWorkloads.Count; i++)
                    {
                        List<string> row = spiderWorkloads[i];
                        for (int j = 0; j < row.Count; j++)
                        {
                            root_url.CreatedURLCount++;
                        }
                    }

                    Database database = new Database();
                    var uri = new Uri(url_name);
                    string table_name = (uri.Host).ToString();
                    string sanitizedTableName = database.SanitizeTableName(table_name);
                    database.CreateTable(sanitizedTableName);
                    database.SaveURL(sanitizedTableName, root_url);
                    crawled_url_listbox.Items.Add(root_url.ToString());

                    for (int i = 1; i <= numberOfSpiders; i++)
                    {
                        Spider spider = new Spider(i, isHorizontal, spiderWorkloads[i - 1], sanitizedTableName);
                        Task crawlTask = spider.Crawl(this);
                        crawlTasks.Add(crawlTask);
                    }

                    await Task.WhenAll(crawlTasks);
                }
                else
                {
                    logger.Warning($"{url_name} did not respond.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong ¯\\\\_(ツ)_/¯ ");
                logger.Error(ex);
            }

            history_button.IsEnabled = true;
            MessageBox.Show($"crawling {url_name} is complete");
        }
        private void logs_button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("notepad.exe", log_path);
        }
        private void history_button_Click(object sender, RoutedEventArgs e)
        {
            HistoryWindow historyWindow = new HistoryWindow(crawl_button);
            historyWindow.Visibility = Visibility.Visible;
            crawl_button.IsEnabled = false;
        }
    }
}
