// Ignore Spelling: webcrawler
using System;
using System.IO;
using Newtonsoft.Json;
using webcrawler.Logs;
namespace WebCrawler.Settings
{
    public class WebCrawlerSettings
    {
        private readonly string settingsPath = AppDomain.CurrentDomain.BaseDirectory + "settings.json";
        readonly Logger logger = new Logger();
        private const int DefaultMaxDepth = 3;
        private const int DefaultMaxUrl = 100;
        private const int DefaultDelay = 1000;
        private const int DefaultMaxRetry = 3;
        public int MaxDepth { get; set; }
        public int MaxUrl { get; set; }
        public int Delay { get; set; }
        public int MaxRetry { get; set; }
        public WebCrawlerSettings()
        {
            SetDefaults();
        }
        public WebCrawlerSettings(int maxDepth, int maxUrl, int delay, int maxRetry)
        {
            MaxDepth = maxDepth;
            MaxUrl = maxUrl;
            Delay = delay;
            MaxRetry = maxRetry;
        }
        public void SetDefaults()
        {
            MaxDepth = DefaultMaxDepth;
            MaxUrl = DefaultMaxUrl;
            Delay = DefaultDelay;
            MaxRetry = DefaultMaxRetry;
        }
        public void SaveSettings()
        {
            try
            {
                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(settingsPath, json);
                logger.Info("Settings saved successfully");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        public void LoadSettings()
        {
            if (File.Exists(settingsPath))
            {
                try
                {
                    string json = File.ReadAllText(settingsPath);
                    WebCrawlerSettings loadedSettings = JsonConvert.DeserializeObject<WebCrawlerSettings>(json);
                    if (loadedSettings != null)
                    {
                        MaxDepth = loadedSettings.MaxDepth;
                        MaxUrl = loadedSettings.MaxUrl;
                        Delay = loadedSettings.Delay;
                        MaxRetry = loadedSettings.MaxRetry;
                    }
                    else
                    {
                        logger.Warning("Result of de-serialization of settings.json was null.");
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
            else
            {
                SetDefaults();
            }
        }
    }
}
