using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace FolderCleaner
{
    public class FolderCleaner : GenericPlugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();

        public override Guid Id { get; } = Guid.Parse("b2f9374-4173-420d-b17a-e2ace45bb318");

        public FolderCleaner(IPlayniteAPI api) : base(api)
        {
            Properties = new GenericPluginProperties { HasSettings = false };
        }

        public override IEnumerable<MainMenuItem> GetMainMenuItems(GetMainMenuItemsArgs args)
        {
            return new List<MainMenuItem>
            {
                new MainMenuItem
                {
                    Description = "Validar pastas dos jogos",
                    MenuSection = "@Folder Cleaner",
                    Action = (mainMenuItem) =>
                    {
                        ScanLibrary();
                    }
                }
            };
        }

        private void ScanLibrary()
        {
            var missingGames = new List<Game>();

            PlayniteApi.Dialogs.ActivateGlobalProgress((progressArgs) =>
            {
                var allGames = PlayniteApi.Database.Games.ToList();
                progressArgs.ProgressMaxValue = allGames.Count;

                for (int i = 0; i < allGames.Count; i++)
                {
                    if (progressArgs.CancelToken.IsCancellationRequested) break;

                    var game = allGames[i];
                    progressArgs.CurrentProgressValue = i;
                    progressArgs.Text = $"Validando: {game.Name}";

                    if (string.IsNullOrEmpty(game.InstallDirectory))
                        continue;

                    try
                    {
                        bool exists = Directory.Exists(game.InstallDirectory);
                        bool hasFiles = false;

                        if (exists)
                        {
                            hasFiles = Directory.EnumerateFiles(game.InstallDirectory, "*", SearchOption.AllDirectories).Any();
                        }

                        if (!exists || !hasFiles)
                        {
                            missingGames.Add(game);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, $"Erro ao validar pasta do jogo {game.Name}: {game.InstallDirectory}");
                    }
                }
            }, new GlobalProgressOptions("Validando biblioteca...", true));

            if (missingGames.Count > 0)
            {
                ShowResults(missingGames);
            }
            else
            {
                PlayniteApi.Dialogs.ShowMessage("Nenhum jogo órfão encontrado.", "Folder Cleaner");
            }
        }

        private void ShowResults(List<Game> games)
        {
            var window = PlayniteApi.Dialogs.CreateWindow(new WindowCreationOptions
            {
                ShowMaximizeButton = false,
                ShowMinimizeButton = false
            });

            window.Title = "Jogos com pastas ausentes ou vazias";
            window.Width = 600;
            window.Height = 400;

            var stack = new StackPanel { Margin = new Thickness(10) };
            stack.Children.Add(new TextBlock 
            { 
                Text = $"Encontrados {games.Count} jogos sem arquivos válidos na pasta raiz:", 
                Margin = new Thickness(0, 0, 0, 10),
                FontWeight = FontWeights.Bold
            });

            var list = new ListBox { Height = 250, DisplayMemberPath = "Name" };
            foreach (var g in games) list.Items.Add(g);
            stack.Children.Add(list);

            var btnPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 10, 0, 0) };
            
            var btnRemove = new Button { Content = "Remover Selecionados do Playnite", Padding = new Thickness(10, 5, 10, 5), Margin = new Thickness(0, 0, 10, 0) };
            btnRemove.Click += (s, e) =>
            {
                var selection = list.SelectedItems.Cast<Game>().ToList();
                if (selection.Count == 0) selection = games;

                var confirm = PlayniteApi.Dialogs.ShowMessage(
                    $"Deseja remover {selection.Count} jogos da biblioteca do Playnite? Isso NÃO apagará arquivos físicos (que já estão ausentes).",
                    "Confirmar Exclusão",
                    MessageBoxButton.YesNo);

                if (confirm == MessageBoxResult.Yes)
                {
                    foreach (var g in selection)
                    {
                        PlayniteApi.Database.Games.Remove(g);
                    }
                    PlayniteApi.Dialogs.ShowMessage("Remoção concluída!");
                    window.Close();
                }
            };

            var btnCancel = new Button { Content = "Cancelar", Padding = new Thickness(10, 5, 10, 5) };
            btnCancel.Click += (s, e) => window.Close();

            btnPanel.Children.Add(btnRemove);
            btnPanel.Children.Add(btnCancel);
            stack.Children.Add(btnPanel);

            window.Content = stack;
            window.ShowDialog();
        }
    }
}