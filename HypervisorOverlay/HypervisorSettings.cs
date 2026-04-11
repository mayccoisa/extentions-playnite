using Playnite.SDK;
using Playnite.SDK.Data;
using System;
using System.Collections.Generic;

namespace HypervisorOverlay
{
    public class HypervisorSettings : ObservableObject
    {
        // No settings required.
    }

    public class HypervisorSettingsViewModel : ObservableObject, ISettings
    {
        private readonly HypervisorOverlay plugin;
        private HypervisorSettings settings;
        public HypervisorSettings Settings { get { return settings; } set { SetValue(ref settings, value); } }

        public HypervisorSettingsViewModel(HypervisorOverlay plugin)
        {
            this.plugin = plugin;
            var savedSettings = plugin.LoadPluginSettings<HypervisorSettings>();
            if (savedSettings != null)
            {
                Settings = savedSettings;
            }
            else
            {
                Settings = new HypervisorSettings();
            }
        }

        public void BeginEdit() { }
        public void CancelEdit() { }
        public void EndEdit() { plugin.SavePluginSettings(Settings); }
        public bool VerifySettings(out List<string> errors) { errors = new List<string>(); return true; }
    }
}
