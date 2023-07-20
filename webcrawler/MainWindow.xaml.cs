using System;
using System.Windows.Threading;
using System.Windows;

namespace webcrawler
{
    public partial class MainWindow : Window
    {
        //private readonly DispatcherTimer timer;
        private string settings_path = " ";
        public MainWindow()
        {
            InitializeComponent();


            // Create and start the timer
            //timer = new DispatcherTimer
            //{
            //    Interval = TimeSpan.FromMilliseconds(1)
            //};
            //timer.Tick += timer_tick;
            //timer.Start();
        }

        private void timer_tick(object sender, EventArgs e)
        {
            //var currentTime = DateTime.Now;
            //chronometer_textblock.Text = currentTime.ToString("HH:mm:ss.fff"); 
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            spidercount_label.Content = spider_slider.Value.ToString();
        }

        private void advancedsettings_button_Click(object sender, RoutedEventArgs e)
        {
            AdvancedSettings settings_window;
            settings_window = new AdvancedSettings();
            settings_window.Visibility = Visibility.Hidden;
            settings_window.Visibility=Visibility.Visible;
        }
    }
}
