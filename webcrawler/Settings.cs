// Ignore Spelling: webcrawler

using System;
using System.IO;
using Newtonsoft.Json;

namespace webcrawler.settings
{
    class Settings
    {
        private static readonly string settings_path = AppDomain.CurrentDomain.BaseDirectory + "settings.json";
        private static readonly int default_max_depth = 3;
        private static readonly int default_max_url = 100;
        private static readonly int default_delay = 1000;
        private static readonly int default_max_retry = 3;
        public int max_depth { get; set; }
        public int max_url { get; set; }
        public int delay { get; set; }
        public int max_retry { get; set; }
        public Settings()//default constructor
        {
            set_defaults();
        }
        public Settings(int max_depth, int max_url, int delay, int max_retry)
        {
            this.max_depth = max_depth;
            this.max_url = max_url;
            this.delay = delay;
            this.max_retry = max_retry;
        }
        public void set_defaults()
        {
            this.max_depth = default_max_depth;
            this.max_url = default_max_url;
            this.delay = default_delay;
            this.max_retry = default_max_retry;
        }

        public void save_settings()
        {
            string json = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(settings_path, json);
        }
        public Settings? load_settings()
        {
            if (File.Exists(settings_path))
            {
                Settings loaded_settings = null;
                try
                {
                    string json = File.ReadAllText(settings_path);
                    loaded_settings = JsonConvert.DeserializeObject<Settings>(json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while loading settings: " + ex.Message);
                    return null;
                }
                if (loaded_settings != null)
                {
                    this.max_depth = loaded_settings.max_depth;
                    this.max_url = loaded_settings.max_url;
                    this.delay = loaded_settings.delay;
                    this.max_retry = loaded_settings.max_retry;
                }
            }
            else
            {
                set_defaults();
            }
            return this;
        }

    }
}


