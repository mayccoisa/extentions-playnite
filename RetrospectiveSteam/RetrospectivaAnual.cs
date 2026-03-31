using Playnite.SDK;
using Playnite.SDK.Events;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RetrospectivaAnual.Services;
using RetrospectivaAnual.ViewModels;
using RetrospectivaAnual.Views;
using System.Windows;
using System.Windows.Controls;

namespace RetrospectivaAnual
{
    public class RetrospectivaAnualPlugin : GenericPlugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();

        private RetrospectivaAnualSettingsViewModel settings { get; set; }
        private PlayniteDataService dataService;
        private GameActivityService activityService;
        public override Guid Id { get { return Guid.Parse("785d9324-4173-420d-b17a-e2ace45bb317"); } }

        public RetrospectivaAnualPlugin(IPlayniteAPI api) : base(api)
        {
            settings = new RetrospectivaAnualSettingsViewModel(this);
            dataService = new PlayniteDataService(api);
            activityService = new GameActivityService(api);
            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };
        }

        public override IEnumerable<MainMenuItem> GetMainMenuItems(GetMainMenuItemsArgs args)
        {
            return new List<MainMenuItem>
            {
                new MainMenuItem
                {
                    Description = "Show Retrospectiva anual",
                    MenuSection = "@Retrospective",
                    Action = (mainMenuItem) =>
                    {
                        OpenRetrospectiveView();
                    }
                }
            };
        }

        public override IEnumerable<SidebarItem> GetSidebarItems()
        {
            return new List<SidebarItem>
            {
                new SidebarItem
                {
                    Title = "Retrospectiva anual",
                    Type = SiderbarItemType.View,
                    Icon = new TextBlock
                    {
                        Text = "⭐",
                        FontSize = 26,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    },
                    Opened = () => GetRetrospectiveView()
                }
            };
        }

        private Control GetRetrospectiveView()
        {
            var viewModel = new RetrospectiveViewModel(dataService, activityService, System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            return new RetrospectiveView(viewModel);
        }

        private void OpenRetrospectiveView()
        {
            var view = GetRetrospectiveView();
            var window = PlayniteApi.Dialogs.CreateWindow(new WindowCreationOptions
            {
                ShowMaximizeButton = true,
                ShowMinimizeButton = true
            });

            window.Title = "Retrospectiva anual 2025";
            window.Content = view;
            window.Width = 1200;
            window.Height = 800;
            window.ShowDialog();
        }

        public override void OnGameInstalled(OnGameInstalledEventArgs args)
        {
            // Add code to be executed when game is finished installing.
        }

        public override void OnGameStarted(OnGameStartedEventArgs args)
        {
            // Add code to be executed when game is started running.
        }

        public override void OnGameStarting(OnGameStartingEventArgs args)
        {
            // Add code to be executed when game is preparing to be started.
        }

        public override void OnGameStopped(OnGameStoppedEventArgs args)
        {
            // Add code to be executed when game is preparing to be started.
        }

        public override void OnGameUninstalled(OnGameUninstalledEventArgs args)
        {
            // Add code to be executed when game is uninstalled.
        }

        public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
        {
            // Add code to be executed when Playnite is initialized.
        }

        public override void OnApplicationStopped(OnApplicationStoppedEventArgs args)
        {
            // Add code to be executed when Playnite is shutting down.
        }

        public override void OnLibraryUpdated(OnLibraryUpdatedEventArgs args)
        {
            // Add code to be executed when library is updated.
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new RetrospectivaAnualSettingsView();
        }
    }
}