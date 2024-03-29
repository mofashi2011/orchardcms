﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Yaml.Serialization;
using Orchard.FileSystems.AppData;
using Orchard.Localization;

namespace Orchard.Environment.Configuration {
    public class ShellSettingsManager : IShellSettingsManager {
        private readonly IAppDataFolder _appDataFolder;
        private readonly IShellSettingsManagerEventHandler _events;

        Localizer T { get; set; }
        
        public ShellSettingsManager(
            IAppDataFolder appDataFolder, 
            IShellSettingsManagerEventHandler events) {
            _appDataFolder = appDataFolder;
            _events = events;

            T = NullLocalizer.Instance;
        }

        IEnumerable<ShellSettings> IShellSettingsManager.LoadSettings() {
            return LoadSettings().ToArray();
        }

        void IShellSettingsManager.SaveSettings(ShellSettings settings) {
            if (settings == null)
                throw new ArgumentException(T("There are no settings to save.").ToString());
            if (string.IsNullOrEmpty(settings.Name))
                throw new ArgumentException(T("Settings \"Name\" is not set.").ToString());

            var filePath = Path.Combine(Path.Combine("Sites", settings.Name), "Settings.txt");
            _appDataFolder.CreateFile(filePath, ComposeSettings(settings));
            _events.Saved(settings);
        }

        IEnumerable<ShellSettings> LoadSettings() {
            var filePaths = _appDataFolder
                .ListDirectories("Sites")
                .SelectMany(path => _appDataFolder.ListFiles(path))
                .Where(path => string.Equals(Path.GetFileName(path), "Settings.txt", StringComparison.OrdinalIgnoreCase));

            foreach (var filePath in filePaths) {
                yield return ParseSettings(_appDataFolder.ReadFile(filePath));
            }
        }

        class Content {
            public string Name { get; set; }
            public string DataProvider { get; set; }
            public string DataConnectionString { get; set; }
            public string DataPrefix { get; set; }
            public string RequestUrlHost { get; set; }
            public string RequestUrlPrefix { get; set; }
            public string State { get; set; }
        }

        static ShellSettings ParseSettings(string text) {
            var ser = new YamlSerializer();
            var content = ser.Deserialize(text, typeof(Content)).Cast<Content>().Single();
            return new ShellSettings {
                Name = content.Name,
                DataProvider = content.DataProvider,
                DataConnectionString = content.DataConnectionString,
                DataTablePrefix = content.DataPrefix,
                RequestUrlHost = content.RequestUrlHost,
                RequestUrlPrefix = content.RequestUrlPrefix,
                State = new TenantState(content.State)
            };
        }

        static string ComposeSettings(ShellSettings settings) {
            if (settings == null)
                return "";

            var ser = new YamlSerializer();
            return ser.Serialize(new Content {
                Name = settings.Name,
                DataProvider = settings.DataProvider,
                DataConnectionString = settings.DataConnectionString,
                DataPrefix = settings.DataTablePrefix,
                RequestUrlHost = settings.RequestUrlHost,
                RequestUrlPrefix = settings.RequestUrlPrefix,
                State = settings.State != null ? settings.State.ToString() : String.Empty
            });
        }
    }
}
