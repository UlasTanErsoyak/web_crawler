// Ignore Spelling: img webcrawler
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
//todo real time url per second
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
        private async void crawl_button_Click(object sender, RoutedEventArgs e)
        {
            crawled_url_listbox.Items.Clear();
            if (vertical_crawl_radio.IsChecked == true)
            {
                URLQueue urlQueue = new URLQueue();
                URL root_url = new URL(0,0,0,rooturl_textbox.Text);
                urlQueue.Add(root_url);
                int spiderCount;
                if (Int32.TryParse(spidercount_label.Content.ToString(), out spiderCount))
                {
                    Spider spider = new Spider(spiderCount);
                    var x = await Task.Run(() => spider.Crawl(this, urlQueue));

                }
            }
            else if (horizontal_crawl_radio.IsChecked == true)
            {
                URLStack urlStack = new URLStack();
                URL root_url = new URL(0, 0, 0, rooturl_textbox.Text);
                urlStack.Add(root_url);
                int spiderCount;
                if (Int32.TryParse(spidercount_label.Content.ToString(), out spiderCount))
                {
                    Spider spider = new Spider(spiderCount);
                    var x = await Task.Run(() => spider.Crawl(this, urlStack));
                }
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
