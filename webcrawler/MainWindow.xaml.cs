// Ignore Spelling: img webcrawler
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows;
namespace webcrawler
{
    public partial class MainWindow : Window
    {
        public readonly string settings_img_path = "src_img/settings.png";
        public readonly string crawl_img_path = "src_img/crawl_image.png";
        public readonly string log_img_path = "src_img/log.png";
        private readonly string log_path = AppDomain.CurrentDomain.BaseDirectory + "logs.txt";
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
        private static bool HasImageChildNode(HtmlNode node)
        {
            return node.Descendants("img").Any();
        }
        private List<List<string>> DivideWorkload(List<string> urls, int numberOfSpiders)
        {
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
            int numberOfSpiders;
            bool parseSuccess = Int32.TryParse(spidercount_label.Content.ToString(), out numberOfSpiders);
            crawled_url_listbox.Items.Clear();
            if (vertical_crawl_radio.IsChecked == true)
            {
                List<List<string>> spiderWorkloads = DivideWorkload(InitializeCrawling(rooturl_textbox.Text),numberOfSpiders);
                List<Task> crawlTasks = new List<Task>();
                URL root_url = new URL(0, 0, 0, rooturl_textbox.Text);
                root_url.CrawlingDate = DateTime.Now;
                crawled_url_listbox.Items.Add(root_url.ToString());
                for (int i = 1; i <= numberOfSpiders; i++)
                {
                    Spider spider = new Spider(i, false, spiderWorkloads[i-1]);
                    Task crawlTask = spider.Crawl(this);
                    crawlTasks.Add(crawlTask);
                }
                await Task.WhenAll(crawlTasks);
                //MessageBox.Show($"Completed vertical crawling of {root_url.URLAddress}");
            }
            else if (horizontal_crawl_radio.IsChecked == true)
            {
                URLStack urlStack = new URLStack();
                URL root_url = new URL(0, 0, 0, rooturl_textbox.Text);
                urlStack.Push(root_url);
            }
            else
            {
                MessageBox.Show("Select a crawling direction.","Error",MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void logs_button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("notepad.exe", log_path);
        }
    }
}
