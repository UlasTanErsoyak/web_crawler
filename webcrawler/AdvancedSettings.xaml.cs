using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using webcrawler.settings;
namespace webcrawler
{
    /// <summary>
    /// Interaction logic for AdvancedSettings.xaml
    /// </summary>
    public partial class AdvancedSettings : Window
    {
        Settings current_settings = null;
        public AdvancedSettings()
        {
            InitializeComponent();
            current_settings = new Settings();
            current_settings.load_settings();
            visualize_settings();
        }
        private void visualize_settings()
        {
            max_depth_textbox.Text = current_settings.max_depth.ToString();
            max_urls_textbox.Text = current_settings.max_url.ToString();
            delay_textbox.Text = current_settings.delay.ToString();
            max_retry_textbox.Text = current_settings.max_retry.ToString();
        }
        private void Settings_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to close the settings window and save settings?", "Closing", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
                
            }
        }

        private void back_to_default_settings_button_Click(object sender, RoutedEventArgs e)
        {
            current_settings.set_defaults();
            current_settings.save_settings();
            visualize_settings();
            MessageBox.Show("Reverted to default settings.", "Saved", MessageBoxButton.OK);
        }

        private void save_settings_button_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(max_depth_textbox.Text, out int max_depth) &&
            int.TryParse(max_urls_textbox.Text, out int max_urls) &&
            int.TryParse(delay_textbox.Text, out int delay) &&
            int.TryParse(max_retry_textbox.Text, out int max_retry))
            {
                if(max_depth>0 && max_urls>0 && delay>0 && max_retry>0)
                {
                    current_settings = new Settings(max_depth, max_urls, delay, max_retry);
                    current_settings.save_settings();
                    visualize_settings();
                    MessageBox.Show("Saved settings.", "Saved", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show("Invalid input. Please enter positive integer values in the textboxes.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            else
            {
                MessageBox.Show("Invalid input. Please enter valid integer values in the textboxes.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
