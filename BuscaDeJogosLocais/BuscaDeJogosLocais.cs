using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using Playnite.SDK.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Windows.Controls;
using System.Diagnostics;

namespace BuscaDeJogosLocais
{
    public class BuscaDeJogosLocais : LibraryPlugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        private List<FileSystemWatcher> _vigias = new List<FileSystemWatcher>();
        public override Guid Id { get; } = Guid.Parse("186d9374-4173-420d-b17a-e2ace45bb317");
        public override string Name => "Busca de Jogos Locais";
        public override LibraryClient Client => null;

        private BuscaDeJogosLocaisSettingsViewModel settings;

        public BuscaDeJogosLocais(IPlayniteAPI api) : base(api)
        {
            Properties = new LibraryPluginProperties { HasSettings = true };
            settings = new BuscaDeJogosLocaisSettingsViewModel(this);
        }

        public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
        {
            UpdateWatchers();
        }

        public void UpdateWatchers()
        {
            foreach (var watcher in _vigias) watcher.Dispose();
            _vigias.Clear();

            if (settings.Settings.Pastas != null)
            {
                foreach (var pasta in settings.Settings.Pastas)
                {
                    if (Directory.Exists(pasta))
                    {
                        var watcher = new FileSystemWatcher(pasta) { IncludeSubdirectories = true, EnableRaisingEvents = true };
                        watcher.Created += (s, e) => {
                            logger.Info($"Novo arquivo detectado: {e.FullPath}");
                        };
                        _vigias.Add(watcher);
                    }
                }
            }
        }

        public void ExecuteManualScan(ObservableCollection<ScannedGame> listToPopulate)
        {
            PlayniteApi.Dialogs.ActivateGlobalProgress((progressArgs) =>
            {
                if (settings.Settings.Pastas == null || settings.Settings.Pastas.Count == 0) return;
                
                var gamesFoundMap = new Dictionary<string, ScannedGame>(StringComparer.OrdinalIgnoreCase);
                var monitoredPaths = settings.Settings.Pastas.Select(p => NormalizePath(p)).ToList();

                foreach (var pasta in settings.Settings.Pastas)
                {
                    if (Directory.Exists(pasta))
                    {
                        try 
                        {
                            var files = SafeEnumerateFiles(pasta, "*.exe", progressArgs.CancelToken);
                            foreach (var file in files)
                            {
                                if (progressArgs.CancelToken.IsCancellationRequested) break;
                                
                                if (IsExplosiveFile(file)) continue;

                                string monitoredPai;
                                string gameRoot = GetGameRootInternal(file, monitoredPaths, out monitoredPai);
                                if (string.IsNullOrEmpty(gameRoot) || !Directory.Exists(gameRoot)) continue;

                                if (!gamesFoundMap.ContainsKey(gameRoot))
                                {
                                    bool jaExiste = PlayniteApi.Database.Games.Any(g => 
                                        g.InstallDirectory != null && 
                                        NormalizePath(g.InstallDirectory).Equals(NormalizePath(gameRoot), StringComparison.OrdinalIgnoreCase));

                                    gamesFoundMap[gameRoot] = new ScannedGame
                                    {
                                        Nome = new DirectoryInfo(gameRoot).Name,
                                        CaminhoExe = file,
                                        PastaRaiz = gameRoot,
                                        PastaMonitoradaPai = monitoredPai,
                                        JaExiste = jaExiste,
                                        Selecionado = !jaExiste
                                    };
                                }
                            }
                        }
                        catch (Exception ex) { logger.Error(ex, $"Erro crítico ao escanear {pasta}"); }
                    }
                }
                
                var finalResults = gamesFoundMap.Values.OrderBy(g => g.Nome).ToList();
                PlayniteApi.MainView.UIDispatcher.Invoke(() => {
                    foreach (var g in finalResults) listToPopulate.Add(g);
                });
            }, new GlobalProgressOptions("Buscando jogos locais...", true));
        }

        private bool IsExplosiveFile(string path)
        {
            string p = path.ToLowerInvariant();
            if (!p.EndsWith(".exe")) return true;
            string[] blacklist = { "unins", "setup", "crash", "helper", "update", "redist", "bugreport", "sendreport", "steamerrorreporter", "dxwebsetup", "dotnet", "vcredist", "tool", "media", "storybook" };
            return blacklist.Any(b => p.Contains(b));
        }

        private IEnumerable<string> SafeEnumerateFiles(string path, string searchPattern, System.Threading.CancellationToken cancelToken)
        {
            var stack = new Stack<string>();
            stack.Push(path);

            while (stack.Count > 0)
            {
                if (cancelToken.IsCancellationRequested) yield break;

                string currentDir = stack.Pop();
                
                // Tenta enumerar arquivos no diretório atual
                IEnumerable<string> files = null;
                try
                {
                    files = Directory.EnumerateFiles(currentDir, searchPattern);
                }
                catch (UnauthorizedAccessException)
                {
                    continue; 
                }
                catch (Exception ex)
                {
                    logger.Debug($"Erro ao enumerar arquivos em {currentDir}: {ex.Message}");
                    continue;
                }

                if (files != null)
                {
                    foreach (var file in files) yield return file;
                }

                // Tenta enumerar subdiretórios
                IEnumerable<string> subDirs = null;
                try
                {
                    subDirs = Directory.EnumerateDirectories(currentDir);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
                catch (Exception ex)
                {
                    logger.Debug($"Erro ao enumerar diretórios em {currentDir}: {ex.Message}");
                    continue;
                }

                if (subDirs != null)
                {
                    foreach (var dir in subDirs) stack.Push(dir);
                }
            }
        }

        public string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            return Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).ToLowerInvariant();
        }

        private string GetGameRootInternal(string filePath, List<string> monitoredPaths, out string monitoredPai)
        {
            monitoredPai = "Desconhecida";
            DirectoryInfo current = new DirectoryInfo(Path.GetDirectoryName(filePath));
            while (current != null && current.Parent != null)
            {
                string currentParentPath = NormalizePath(current.Parent.FullName);
                if (monitoredPaths.Contains(currentParentPath))
                {
                    monitoredPai = current.Parent.FullName;
                    return current.FullName;
                }
                current = current.Parent;
            }
            return null;
        }

        public bool ImportarJogoManual(ScannedGame scanned)
        {
            string normRoot = NormalizePath(scanned.PastaRaiz);
            if (PlayniteApi.Database.Games.Any(g => g.InstallDirectory != null && NormalizePath(g.InstallDirectory).Equals(normRoot, StringComparison.OrdinalIgnoreCase)))
                return false;

            var game = new Game(scanned.Nome)
            {
                PluginId = Id,
                InstallDirectory = scanned.PastaRaiz,
                IsInstalled = true,
                GameActions = new ObservableCollection<GameAction> { 
                    new GameAction { Type = GameActionType.File, Path = scanned.CaminhoExe, Name = "Jogar", WorkingDir = "{InstallDir}" } 
                }
            };
            
            var tag = PlayniteApi.Database.Tags.Add("Importado Local");
            game.TagIds = new List<Guid> { tag.Id };

            if (settings.Settings.TagDriveAsFeature)
            {
                ApplyDriveTag(game);
            }

            PlayniteApi.Database.Games.Add(game);
            
            try {
                settings.Settings.HistoricoImportacoes.Add(new ImportLogEntry { 
                    DataImportacao = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), 
                    NomeJogo = scanned.Nome, 
                    Origem = scanned.PastaRaiz,
                    PastaMonitoradaPai = scanned.PastaMonitoradaPai
                });
                SavePluginSettings(settings.Settings);
            } catch (Exception) { }
            return true;
        }

        public override ISettings GetSettings(bool firstRun) => settings;
        public override UserControl GetSettingsView(bool firstRun) => new BuscaDeJogosLocaisSettingsView();
        public override IEnumerable<GameMetadata> GetGames(LibraryGetGamesArgs args) => new List<GameMetadata>();

        public override IEnumerable<PlayController> GetPlayActions(GetPlayActionsArgs args)
        {
            if (args.Game.PluginId != Id) yield break;

            var game = args.Game;
            if (game.GameActions != null && game.GameActions.Count > 0)
            {
                var action = game.GameActions[0];
                if (action.Type == GameActionType.File)
                {
                    yield return new AutomaticPlayController(game)
                    {
                        Path = action.Path,
                        Arguments = action.Arguments,
                        WorkingDir = action.WorkingDir,
                        Name = action.Name,
                        TrackingMode = TrackingMode.Default
                    };
                }
            }
        }

        public bool ApplyDriveTag(Game game)
        {
            if (string.IsNullOrEmpty(game.InstallDirectory)) return false;
            try
            {
                string drive = Path.GetPathRoot(game.InstallDirectory);
                if (!string.IsNullOrEmpty(drive))
                {
                    string featureName = $"HD {drive.TrimEnd('\\')}";
                    var feature = PlayniteApi.Database.Features.Add(featureName);
                    if (game.FeatureIds == null) game.FeatureIds = new List<Guid>();
                    if (!game.FeatureIds.Contains(feature.Id))
                    {
                        game.FeatureIds.Add(feature.Id);
                        return true;
                    }
                }
            } catch (Exception) { }
            return false;
        }
    }
}
