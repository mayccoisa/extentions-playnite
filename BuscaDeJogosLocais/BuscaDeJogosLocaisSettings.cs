using Playnite.SDK;
using Playnite.SDK.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace BuscaDeJogosLocais
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> execute;
        private readonly Predicate<T> canExecute;
        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }
        public bool CanExecute(object parameter) => canExecute == null || canExecute((T)parameter);
        public void Execute(object parameter) => execute((T)parameter);
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;
    }

    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public void SetValue<T>(ref T field, T value, [CallerMemberName] string name = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(name);
            }
        }
    }

    public class ImportLogEntry : ObservableObject
    {
        public string DataImportacao { get; set; }
        public string NomeJogo { get; set; }
        public string Origem { get; set; }
        public string PastaMonitoradaPai { get; set; } // Adicionado para filtro
    }

    public class ScannedGame : ObservableObject
    {
        private bool selecionado;
        public bool Selecionado { get => selecionado; set => SetValue(ref selecionado, value); }
        
        public string Nome { get; set; }
        public string CaminhoExe { get; set; }
        public string PastaRaiz { get; set; }
        public string PastaMonitoradaPai { get; set; } // Adicionado para filtro/agrupamento
        
        private bool jaExiste;
        public bool JaExiste { get => jaExiste; set { SetValue(ref jaExiste, value); OnPropertyChanged(nameof(Status)); } }
        
        public string Status => JaExiste ? "Já na Biblioteca" : "Pendente";
    }

    public class BuscaDeJogosLocaisSettings : ObservableObject
    {
        private ObservableCollection<string> pastas = new ObservableCollection<string>();
        public ObservableCollection<string> Pastas { get => pastas; set => SetValue(ref pastas, value); }
        
        private ObservableCollection<ImportLogEntry> historicoImportacoes = new ObservableCollection<ImportLogEntry>();
        public ObservableCollection<ImportLogEntry> HistoricoImportacoes { get => historicoImportacoes; set => SetValue(ref historicoImportacoes, value); }

        private bool tagDriveAsFeature = false;
        public bool TagDriveAsFeature { get => tagDriveAsFeature; set => SetValue(ref tagDriveAsFeature, value); }
    }

    public class BuscaDeJogosLocaisSettingsViewModel : ObservableObject, ISettings
    {
        private readonly BuscaDeJogosLocais plugin;
        private BuscaDeJogosLocaisSettings editingClone { get; set; }
        private BuscaDeJogosLocaisSettings settings;
        public BuscaDeJogosLocaisSettings Settings { get => settings; set => SetValue(ref settings, value); }

        // Listas e Views para Filtros
        public ObservableCollection<ScannedGame> JogosEncontrados { get; } = new ObservableCollection<ScannedGame>();
        public ICollectionView JogosEncontradosView { get; }

        public ICollectionView HistoricoView { get; }
        
        // Filtros Ativos
        private bool filtrarApenasNovos = false;
        public bool FiltrarApenasNovos { get => filtrarApenasNovos; set { SetValue(ref filtrarApenasNovos, value); JogosEncontradosView.Refresh(); } }

        private string filtroPastaSelecionada = "Todas as Pastas";
        public string FiltroPastaSelecionada { get => filtroPastaSelecionada; set { SetValue(ref filtroPastaSelecionada, value); JogosEncontradosView.Refresh(); HistoricoView.Refresh(); } }

        public List<string> OpcoesPastas
        {
            get
            {
                var list = new List<string> { "Todas as Pastas" };
                if (Settings.Pastas != null) list.AddRange(Settings.Pastas);
                return list;
            }
        }

        private string pastasEstatisticas = "Escaneie para ver as estatísticas...";
        public string PastasEstatisticas { get => pastasEstatisticas; set => SetValue(ref pastasEstatisticas, value); }
        
        public RelayCommand<object> AddFolderCommand { get; }
        public RelayCommand<object> ScanNowCommand { get; }
        public RelayCommand<object> ImportSelectedCommand { get; }
        public RelayCommand<object> ClearHistoryCommand { get; }
        public RelayCommand<object> UpdateExistingGamesDriveCommand { get; }

        public BuscaDeJogosLocaisSettingsViewModel(BuscaDeJogosLocais plugin)
        {
            this.plugin = plugin;
            var savedSettings = plugin.LoadPluginSettings<BuscaDeJogosLocaisSettings>();
            if (savedSettings != null)
            {
                Settings = savedSettings;
                if (Settings.HistoricoImportacoes == null) Settings.HistoricoImportacoes = new ObservableCollection<ImportLogEntry>();
                if (Settings.Pastas == null) Settings.Pastas = new ObservableCollection<string>();
            }
            else
            {
                Settings = new BuscaDeJogosLocaisSettings();
            }

            // Inicializar Views de Coleção
            JogosEncontradosView = CollectionViewSource.GetDefaultView(JogosEncontrados);
            JogosEncontradosView.Filter = FilterJogosDescobertos;
            JogosEncontradosView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(ScannedGame.PastaMonitoradaPai)));

            HistoricoView = CollectionViewSource.GetDefaultView(Settings.HistoricoImportacoes);
            HistoricoView.Filter = FilterHistorico;
            HistoricoView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(ImportLogEntry.PastaMonitoradaPai)));

            AddFolderCommand = new RelayCommand<object>((_) =>
            {
                var path = plugin.PlayniteApi.Dialogs.SelectFolder();
                if (!string.IsNullOrEmpty(path))
                {
                    if (!Settings.Pastas.Contains(path))
                    {
                        Settings.Pastas.Add(path);
                        OnPropertyChanged(nameof(OpcoesPastas));
                        RecalcularEstatisticas();
                    }
                }
            });

            ScanNowCommand = new RelayCommand<object>((_) =>
            {
                JogosEncontrados.Clear();
                plugin.ExecuteManualScan(JogosEncontrados);
                RecalcularEstatisticas();
            });

            ImportSelectedCommand = new RelayCommand<object>((_) =>
            {
                var selecionados = JogosEncontrados.Where(j => j.Selecionado && !j.JaExiste).ToList();
                if (selecionados.Count == 0)
                {
                    plugin.PlayniteApi.Dialogs.ShowMessage("Nenhum jogo novo selecionado para importar.", "Aviso");
                    return;
                }

                int importados = 0;
                foreach (var jogo in selecionados)
                {
                    if (plugin.ImportarJogoManual(jogo))
                    {
                        importados++;
                        jogo.JaExiste = true;
                        jogo.Selecionado = false;
                    }
                }

                if (importados > 0)
                {
                    plugin.PlayniteApi.Dialogs.ShowMessage($"{importados} jogos importados com sucesso!", "Sucesso");
                    JogosEncontradosView.Refresh();
                    RecalcularEstatisticas();
                }
            });

            ClearHistoryCommand = new RelayCommand<object>((_) =>
            {
                Settings.HistoricoImportacoes.Clear();
            });

            UpdateExistingGamesDriveCommand = new RelayCommand<object>((_) =>
            {
                int atualizados = 0;
                var games = plugin.PlayniteApi.Database.Games.Where(g => g.PluginId == plugin.Id).ToList();
                
                foreach (var game in games)
                {
                    if (plugin.ApplyDriveTag(game))
                    {
                        atualizados++;
                        plugin.PlayniteApi.Database.Games.Update(game);
                    }
                }
                
                plugin.PlayniteApi.Dialogs.ShowMessage($"{atualizados} jogos atualizados com a característica do HD.", "Sucesso");
            });
        }

        private bool FilterJogosDescobertos(object item)
        {
            var game = (ScannedGame)item;
            if (FiltrarApenasNovos && game.JaExiste) return false;
            
            if (FiltroPastaSelecionada != "Todas as Pastas")
            {
                return game.PastaMonitoradaPai.Equals(FiltroPastaSelecionada, StringComparison.OrdinalIgnoreCase);
            }
            
            return true;
        }

        private bool FilterHistorico(object item)
        {
            if (FiltroPastaSelecionada == "Todas as Pastas") return true;
            var entry = (ImportLogEntry)item;
            return entry.PastaMonitoradaPai?.Equals(FiltroPastaSelecionada, StringComparison.OrdinalIgnoreCase) ?? true;
        }

        public void RecalcularEstatisticas()
        {
            if (Settings.Pastas == null || Settings.Pastas.Count == 0)
            {
                PastasEstatisticas = "Nenhuma pasta monitorada no momento.";
                return;
            }

            var builder = new System.Text.StringBuilder();
            builder.AppendLine("📊 Resumo das Pastas Monitoradas:");
            foreach (var pasta in Settings.Pastas)
            {
                if (Directory.Exists(pasta))
                {
                    try {
                        var dirs = Directory.GetDirectories(pasta);
                        int totalPastas = dirs.Length;
                        int pastasVinculadas = dirs.Count(d => plugin.PlayniteApi.Database.Games.Any(g => 
                            g.InstallDirectory != null && 
                            plugin.NormalizePath(g.InstallDirectory).Equals(plugin.NormalizePath(d), StringComparison.OrdinalIgnoreCase)
                        ));
                        builder.AppendLine($"- {pasta}: {totalPastas} pastas ({pastasVinculadas} vinculadas).");
                    } catch (Exception) { builder.AppendLine($"- {pasta}: Erro de leitura."); }
                }
                else builder.AppendLine($"- {pasta}: (Inacessível)");
            }
            PastasEstatisticas = builder.ToString();
        }

        public void BeginEdit() { editingClone = Serialization.GetClone(Settings); RecalcularEstatisticas(); OnPropertyChanged(nameof(OpcoesPastas)); }
        public void CancelEdit() { Settings = editingClone; }
        public void EndEdit() { plugin.SavePluginSettings(Settings); plugin.UpdateWatchers(); }
        public bool VerifySettings(out List<string> errors) { errors = new List<string>(); return true; }
    }
}
