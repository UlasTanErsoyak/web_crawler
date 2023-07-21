// Ignore Spelling: img webcrawler
using System.Threading.Tasks;
using System.Windows;
namespace webcrawler
{
    public partial class MainWindow : Window
    {
        public readonly string settings_img_path = "src_img/settings.png";
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
            if (vertical_crawl_radio.IsChecked == true)
            {
                URLQueue urlQueue = new URLQueue();
                URL root_url = new URL(0,0,0,rooturl_textbox.Text);
                urlQueue.Add(root_url);
                Spider spider = new Spider();
                var x = await Task.Run(() => spider.Crawl(urlQueue,this));
            }
            else if (horizontal_crawl_radio.IsChecked == true)
            {
                //todo
            }
            else
            {
                MessageBox.Show("Select a crawling direction.","Error",MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
