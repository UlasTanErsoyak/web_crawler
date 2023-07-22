// Ignore Spelling: webcrawler
using System.Windows;
using WebCrawler.Settings;
namespace webcrawler
{
    /// <summary>
    /// Interaction logic for AdvancedSettings.xaml
    /// </summary>
    public partial class AdvancedSettings : Window
    {
        WebCrawlerSettings? current_settings = null;
        public AdvancedSettings()
        {
            InitializeComponent();
            current_settings = new WebCrawlerSettings();
            current_settings.LoadSettings();
            visualize_settings();
        }
        private void visualize_settings()
        {
            max_depth_textbox.Text = current_settings.MaxDepth.ToString();
            max_urls_textbox.Text = current_settings.MaxUrl.ToString();
            delay_textbox.Text = current_settings.Delay.ToString();
        }
        private void Settings_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //MessageBoxResult result = MessageBox.Show("Are you sure you want to close the settings window and save settings?",
            //    "Closing", MessageBoxButton.YesNo, MessageBoxImage.Question);

            //if (result == MessageBoxResult.No)
            //{
            //    e.Cancel = true;

            //}
        }
        private void back_to_default_settings_button_Click(object sender, RoutedEventArgs e)
        {
            current_settings.SetDefaults();
            current_settings.SaveSettings();
            visualize_settings();
            MessageBox.Show("Reverted to default settings.", "Saved", MessageBoxButton.OK);
        }
        private void save_settings_button_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(max_depth_textbox.Text, out int max_depth) &&
            int.TryParse(max_urls_textbox.Text, out int max_urls) &&
            int.TryParse(delay_textbox.Text, out int delay))
            {
                if(max_depth>0 && max_urls>0 && delay>0)
                {
                    current_settings = new WebCrawlerSettings(max_depth, max_urls, delay);
                    current_settings.SaveSettings();
                    visualize_settings();
                    MessageBox.Show("Saved settings.", "Saved", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show("Invalid input. Please enter positive integer values in the text boxes.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Invalid input. Please enter valid integer values in the text boxes.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
