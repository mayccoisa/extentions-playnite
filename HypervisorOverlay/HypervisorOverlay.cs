using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using Playnite.SDK.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using HypervisorOverlay.Views;
using System.IO;

namespace HypervisorOverlay
{
    public class HypervisorOverlay : GenericPlugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        private const string HypervisorTagName = "hypervisor";
        private HypervisorSettingsViewModel settings { get; set; }

        public override Guid Id { get { return Guid.Parse("d35d9324-4173-420d-b17a-e2ace45bb317"); } }

        public HypervisorOverlay(IPlayniteAPI api) : base(api)
        {
            settings = new HypervisorSettingsViewModel(this);
            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };
        }

        public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
        {
            EnsureHypervisorTagExists();
        }

        private void EnsureHypervisorTagExists()
        {
            try
            {
                var tag = PlayniteApi.Database.Tags.FirstOrDefault(t => t.Name.Equals(HypervisorTagName, StringComparison.OrdinalIgnoreCase));
                if (tag == null)
                {
                    logger.Info("Creating 'hypervisor' tag.");
                    PlayniteApi.Database.Tags.Add(new Tag(HypervisorTagName));
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to ensure hypervisor tag exists.");
            }
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new HypervisorSettingsView();
        }

        // --- ESTRATÉGIA A: Custom Elements (Adaptado para SDK antigo) ---
        public override Control GetGameViewControl(GetGameViewControlArgs args)
        {
            if (args.Name == "HypervisorOverlay_Icon" || args.Name == "TopRightControl")
            {
                return new HOverlayControl();
            }
            return null;
        }

        // --- ESTRATÉGIA B: Sidebar Items ---
        public override IEnumerable<SidebarItem> GetSidebarItems()
        {
            var items = new List<SidebarItem>();
            items.Add(new SidebarItem
            {
                Title = "Hypervisor Required",
                Type = SiderbarItemType.View, // Mantendo a grafia correta do SDK (erro do SDK)
                Visible = true,
                Icon = new TextBlock 
                { 
                    Text = "H", 
                    Foreground = Brushes.Red, 
                    FontWeight = System.Windows.FontWeights.Bold,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center
                },
                Opened = () => {
                    var selected = PlayniteApi.MainView.SelectedGames.FirstOrDefault();
                    if (selected != null)
                    {
                        PlayniteApi.Dialogs.ShowMessage(string.Format("O jogo '{0}' requer virtualização (Hypervisor) para rodar corretamente.", selected.Name), "Hypervisor Info");
                    }
                    return null; // Func<Control> exige retorno
                }
            });
            return items;
        }

        // --- MENU DE CONTEXTO ---
        public override IEnumerable<GameMenuItem> GetGameMenuItems(GetGameMenuItemsArgs args)
        {
            var menuItems = new List<GameMenuItem>();
            menuItems.Add(new GameMenuItem
            {
                Description = "Hypervisor", // No SDK antigo é Description, não Header
                MenuSection = "Marcadores",
                Icon = "H",
                Action = (mainMenuItem) =>
                {
                    ToggleHypervisorTag(args.Games);
                }
            });
            return menuItems;
        }

        private void ToggleHypervisorTag(List<Game> games)
        {
            if (games == null || !games.Any()) return;

            var tag = PlayniteApi.Database.Tags.FirstOrDefault(t => t.Name.Equals(HypervisorTagName, StringComparison.OrdinalIgnoreCase));
            if (tag == null)
            {
                tag = PlayniteApi.Database.Tags.Add(HypervisorTagName);
            }

            foreach (var game in games)
            {
                if (game.TagIds == null) game.TagIds = new List<Guid>();

                if (game.TagIds.Contains(tag.Id))
                {
                    game.TagIds.Remove(tag.Id);
                }
                else
                {
                    game.TagIds.Add(tag.Id);
                }
                PlayniteApi.Database.Games.Update(game);
            }
        }
    }
}
