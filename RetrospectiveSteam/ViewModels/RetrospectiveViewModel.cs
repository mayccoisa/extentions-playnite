using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using RetrospectiveSteam.Services;
using System.Text;

namespace RetrospectiveSteam.ViewModels
{
    public partial class RetrospectiveViewModel : INotifyPropertyChanged
    {
        private readonly PlayniteDataService dataService;
        private readonly GameActivityService activityService;
        public string ExtensionDir { get; private set; }
        public string HeaderImagePath { get { return System.IO.Path.Combine(ExtensionDir, "bau_header.png"); } }

        private GameDetailViewModel _selectedGameDetail;
        public GameDetailViewModel SelectedGameDetail
        {
            get { return _selectedGameDetail; }
            set { _selectedGameDetail = value; OnPropertyChanged("SelectedGameDetail"); OnPropertyChanged("IsModalOpen"); }
        }

        public bool IsModalOpen
        {
            get { return SelectedGameDetail != null; }
        }

        private List<GameStatsData> _cachedYearGames;
        private Dictionary<Guid, List<ActivitySession>> _cachedSessions;

        public event PropertyChangedEventHandler PropertyChanged;

        // ── Year Selector ──────────────────────────────────
        public ObservableCollection<int> YearOptions { get; private set; }

        private int _selectedYear;
        public int SelectedYear
        {
            get { return _selectedYear; }
            set { if (_selectedYear != value) { _selectedYear = value; OnPropertyChanged("SelectedYear"); LoadData(); } }
        }

        // ── Hero ───────────────────────────────────────────
        public string Username    { get; private set; }
        private string _currentYearDisplay;
        public string CurrentYearDisplay
        {
            get { return _currentYearDisplay; }
            set { _currentYearDisplay = value; OnPropertyChanged("CurrentYearDisplay"); }
        }
        public string HeroTagline { get; private set; }

        // ── Stats Snapshot ────────────────────────────────
        public string TotalHours        { get; private set; }
        public string GamesPlayedCount  { get; private set; }
        public string TotalSessionsCount { get; private set; }
        public string GamesAddedCount   { get; private set; }
        public string AchievementCount  { get; private set; }

        private int _longestStreakDays;
        public int LongestStreakDays
        {
            get { return _longestStreakDays; }
            set { SetProperty(ref _longestStreakDays, value, "LongestStreakDays"); }
        }

        private string _hourlyPersonaName;
        public string HourlyPersonaName
        {
            get { return _hourlyPersonaName; }
            set { SetProperty(ref _hourlyPersonaName, value, "HourlyPersonaName"); }
        }

        private string _hourlyPersonaIcon;
        public string HourlyPersonaIcon
        {
            get { return _hourlyPersonaIcon; }
            set { SetProperty(ref _hourlyPersonaIcon, value, "HourlyPersonaIcon"); }
        }

        private string _hourlyPersonaDescription;
        public string HourlyPersonaDescription
        {
            get { return _hourlyPersonaDescription; }
            set { SetProperty(ref _hourlyPersonaDescription, value, "HourlyPersonaDescription"); }
        }

        private string _hourlyBadgeIcon;
        public string HourlyBadgeIcon { get { return _hourlyBadgeIcon; } set { SetProperty(ref _hourlyBadgeIcon, value, "HourlyBadgeIcon"); } }

        private string _hourlyBadgeText;
        public string HourlyBadgeText { get { return _hourlyBadgeText; } set { SetProperty(ref _hourlyBadgeText, value, "HourlyBadgeText"); } }

        private string _hourlyFillPath;
        public string HourlyFillPath { get { return _hourlyFillPath; } set { SetProperty(ref _hourlyFillPath, value, "HourlyFillPath"); } }

        private string _hourlyLinePath;
        public string HourlyLinePath { get { return _hourlyLinePath; } set { SetProperty(ref _hourlyLinePath, value, "HourlyLinePath"); } }

        private string _averageSessionTime;
        public string AverageSessionTime
        {
            get { return _averageSessionTime; }
            set { SetProperty(ref _averageSessionTime, value, "AverageSessionTime"); }
        }

        public string PersonaId         { get; private set; }
        public string MoodTitle         { get; private set; }
        public string MoodSubtitle      { get; private set; }
        public string MoodTopTags        { get; private set; }

        // ── Top Games ─────────────────────────────────────
        public ObservableCollection<GameCardViewModel> TopGames { get; private set; }
        public ObservableCollection<DonutSliceViewModel> DonutSlices { get; private set; }
        public ObservableCollection<GenreBarViewModel> GenreBars { get; private set; }
        public ObservableCollection<AxisLabelViewModel> GenreYAxisLabels { get; private set; }

        public ObservableCollection<string> MoodTopGames { get; private set; }
        public ObservableCollection<MoodTagViewModel> MoodTags { get; private set; }
        public ObservableCollection<HourlyProfileBarViewModel> HourlyBars { get; private set; }
        public ObservableCollection<AxisLabelViewModel> HourlyYAxisLabels { get; private set; }
        public ObservableCollection<MonthBarViewModel> MonthlyBars { get; private set; }
        public ObservableCollection<AxisLabelViewModel> MonthlyYAxisLabels { get; private set; }
        public ObservableCollection<TimelineMonthViewModel> TimelineMonths { get; private set; }
        public ObservableCollection<SeasonViewModel> Seasons { get; private set; }
        public ObservableCollection<MoodInfluencer> Influencers { get; private set; }
        public ObservableCollection<StudioViewModel> TopDevelopers { get; private set; }
        public ObservableCollection<StudioViewModel> TopPublishers { get; private set; }
        public ObservableCollection<MoodResult> MonthlyMoodHistory { get; private set; }
        public ObservableCollection<MoodInfo> MoodDictionary { get; private set; }

        private bool _moodDetailsVisibility;
        public bool MoodDetailsVisibility
        {
            get { return _moodDetailsVisibility; }
            set { SetProperty(ref _moodDetailsVisibility, value, "MoodDetailsVisibility"); }
        }

        public RelayCommand ShowMoodDetailsCommand { get; private set; }
        public RelayCommand HideMoodDetailsCommand { get; private set; }
        public RelayCommand<GameCardViewModel> OpenGameDetailCommand { get; private set; }
        public RelayCommand CloseGameDetailCommand { get; private set; }
        public RelayCommand<Guid> NavigateToGameCommand { get; private set; }
        public RelayCommand<Guid> OpenGameInPlayniteCommand { get; private set; }

        public RetrospectiveViewModel(PlayniteDataService dataService, GameActivityService activityService, string extensionDir)
        {
            this.dataService = dataService;
            this.activityService = activityService;
            this.ExtensionDir = extensionDir;

            this.YearOptions        = new ObservableCollection<int>();
            this.TopGames           = new ObservableCollection<GameCardViewModel>();
            this.DonutSlices        = new ObservableCollection<DonutSliceViewModel>();
            this.GenreBars          = new ObservableCollection<GenreBarViewModel>();
            this.GenreYAxisLabels   = new ObservableCollection<AxisLabelViewModel>();
            this.MonthlyBars        = new ObservableCollection<MonthBarViewModel>();
            this.MonthlyYAxisLabels = new ObservableCollection<AxisLabelViewModel>();
            this.TimelineMonths     = new ObservableCollection<TimelineMonthViewModel>();
            this.Seasons            = new ObservableCollection<SeasonViewModel>();
            this.TopDevelopers      = new ObservableCollection<StudioViewModel>();
            this.TopPublishers      = new ObservableCollection<StudioViewModel>();
            this.MonthlyMoodHistory = new ObservableCollection<MoodResult>();
            this.MoodDictionary    = new ObservableCollection<MoodInfo>();
            this.MoodTopGames       = new ObservableCollection<string>();
            this.MoodTags           = new ObservableCollection<MoodTagViewModel>();
            this.HourlyBars         = new ObservableCollection<HourlyProfileBarViewModel>();
            this.HourlyYAxisLabels  = new ObservableCollection<AxisLabelViewModel>();
            this.Influencers        = new ObservableCollection<MoodInfluencer>();

            this.ShowMoodDetailsCommand = new RelayCommand(delegate() { MoodDetailsVisibility = true; });
            this.HideMoodDetailsCommand = new RelayCommand(delegate() { MoodDetailsVisibility = false; });
            this.OpenGameDetailCommand = new RelayCommand<GameCardViewModel>(OpenGameDetail);
            this.CloseGameDetailCommand = new RelayCommand(delegate() { SelectedGameDetail = null; });
            this.NavigateToGameCommand = new RelayCommand<Guid>(id => ShowGameDetailById(id));
            this.OpenGameInPlayniteCommand = new RelayCommand<Guid>(id => dataService.NavigateToGame(id));

            int currentYear = DateTime.Now.Year;
            for (int i = currentYear; i >= 2000; i--) YearOptions.Add(i);
            SelectedYear = currentYear;
        }

        private void LoadData()
        {
            try {
                var stats = dataService.GetYearlyStats(SelectedYear, activityService);
                if (stats == null) return;

                Username = stats.Username;
                CurrentYearDisplay = SelectedYear.ToString();
                
                _cachedYearGames = stats.Games;
                _cachedSessions = activityService.GetSessionsForYear(SelectedYear);

                TotalHours = stats.TotalPlaytime;
                GamesPlayedCount = stats.GamesPlayedCount.ToString();
                TotalSessionsCount = stats.SessionsCount.ToString();
                GamesAddedCount = stats.NewGamesCount.ToString();
                AchievementCount = stats.AchievementsCount.ToString();
                PersonaId = stats.PersonaId;
                MoodTitle = stats.MoodTitle;
                MoodSubtitle = stats.MoodSubtitle;
                MoodTopTags = stats.MoodTopTags;
                AverageSessionTime = stats.AverageSessionTime;
                LongestStreakDays = stats.LongestStreakDays;
                HourlyPersonaName = stats.HourlyPersonaName;
                HourlyPersonaIcon = stats.HourlyPersonaIcon;
                HourlyPersonaDescription = stats.HourlyPersonaDescription;

                TopGames.Clear();
                for (int i = 0; i < stats.TopGames.Count; i++)
                {
                    var g = stats.TopGames[i];
                    TopGames.Add(new GameCardViewModel
                    {
                        Game = g.Game,
                        RankText = (i + 1).ToString(),
                        PlaytimeFormatted = FormatPlaytime(g.Game.TotalPlaytimeSeconds),
                        IsCompleted = g.IsCompleted
                    });
                }

                MonthlyMoodHistory.Clear();
                foreach (var h in stats.MonthlyMoodHistory) MonthlyMoodHistory.Add(h);

                MoodTopGames.Clear();
                foreach (var g in stats.MoodTopGames) MoodTopGames.Add(g);

                MoodTags.Clear();
                foreach (var t in stats.MoodTags) MoodTags.Add(new MoodTagViewModel { 
                    Name = t.Name, 
                    Count = t.Count, 
                    PlaytimeFormatted = t.PlaytimeFormatted 
                });

                Influencers.Clear();
                foreach (var i in stats.Influencers) Influencers.Add(i);

                MoodDictionary.Clear();
                if (stats.MoodDictionary != null)
                {
                    foreach (var m in stats.MoodDictionary)
                    {
                        MoodDictionary.Add(new MoodInfo { 
                            Title = m.Title, 
                            Subtitle = m.Subtitle, 
                            IconPath = m.IconPath, 
                            TriggerList = m.TriggerList 
                        });
                    }
                }

                GenreBars.Clear();
                long maxSeconds = stats.GenreBars.Count > 0 ? stats.GenreBars.Max(b => b.PlaytimeSeconds) : 3600;
                // Round up to nearest 5 hours for a cleaner axis
                long axisMaxSeconds = ((maxSeconds / 18000) + 1) * 18000; 
                if (axisMaxSeconds < 3600) axisMaxSeconds = 3600;

                bool firstGenre = true;
                foreach (var b in stats.GenreBars) 
                {
                    GenreBars.Add(new GenreBarViewModel { 
                        Name = b.Name, 
                        Ratio = (double)b.PlaytimeSeconds / axisMaxSeconds, 
                        Playtime = b.Playtime,
                        PlaytimeSeconds = b.PlaytimeSeconds,
                        IsTopGenre = firstGenre 
                    });
                    firstGenre = false;
                }

                GenreYAxisLabels.Clear();
                for (int i = 0; i <= 4; i++) 
                {
                    long val = (axisMaxSeconds / 4) * (4 - i);
                    GenreYAxisLabels.Add(new AxisLabelViewModel { 
                        Label = FormatPlaytime(val), 
                        Position = i * 0.25 
                    });
                }

                DonutSlices.Clear();
                var colors = new[] { "#67c1f5", "#df206c", "#2a475e", "#c7d5e0", "#171a21" };
                for (int i = 0; i < stats.PlatformBars.Count; i++)
                {
                    var p = stats.PlatformBars[i];
                    DonutSlices.Add(new DonutSliceViewModel { 
                        Name = p.Name, 
                        Value = p.Ratio * 100, 
                        Color = colors[i % colors.Length],
                        TimeText = p.Playtime 
                    });
                }

                TopDevelopers.Clear();
                foreach (var d in stats.TopDevelopers) 
                {
                    string initials = "";
                    if (!string.IsNullOrEmpty(d.Name)) initials = d.Name.Length > 2 ? d.Name.Substring(0, 2).ToUpper() : d.Name.ToUpper();
                    
                    TopDevelopers.Add(new StudioViewModel { 
                        Name = d.Name, 
                        Ratio = d.Ratio, 
                        PlaytimeFormatted = FormatPlaytime(d.PlaytimeSeconds), 
                        GameCount = d.GameCount,
                        TopGames = new ObservableCollection<string>(d.TopGames),
                        Color = "#67c1f5",
                        Initials = initials,
                        HasIcon = false
                    });
                }

                TopPublishers.Clear();
                foreach (var p in stats.TopPublishers) 
                {
                    string initials = "";
                    if (!string.IsNullOrEmpty(p.Name)) initials = p.Name.Length > 2 ? p.Name.Substring(0, 2).ToUpper() : p.Name.ToUpper();

                    TopPublishers.Add(new StudioViewModel { 
                        Name = p.Name, 
                        Ratio = p.Ratio, 
                        PlaytimeFormatted = FormatPlaytime(p.PlaytimeSeconds), 
                        GameCount = p.GameCount,
                        TopGames = new ObservableCollection<string>(p.TopGames),
                        Color = "#df206c",
                        Initials = initials,
                        HasIcon = false
                    });
                }

                TimelineMonths.Clear();
                int monthIndex = 0;
                foreach (var m in stats.TimelineMonths)
                {
                    int monthNum = Array.IndexOf(CultureInfo.CurrentCulture.DateTimeFormat.MonthNames, m.MonthName) + 1;
                    if (monthNum == 0) monthNum = Array.IndexOf(CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames, m.MonthName) + 1;
                    
                    var monthSessions = _cachedSessions.Values.SelectMany(s => s).Where(s => s.Date.Month == monthNum).ToList();
                    long monthPlaytime = monthSessions.Sum(s => s.Seconds);
                    int gameCount = monthSessions.Select(s => s.Date).Distinct().Count(); // This is wrong, should be games played.
                    
                    // Correct game count for the month
                    var gamesInMonth = _cachedYearGames.Where(g => _cachedSessions.ContainsKey(g.GameId) && _cachedSessions[g.GameId].Any(s => s.Date.Month == monthNum)).ToList();

                    var timelineMonth = new TimelineMonthViewModel { 
                        MonthName = m.MonthName.ToUpper(), 
                        Alignment = monthIndex % 2,
                        GameCount = gamesInMonth.Count.ToString(),
                        TotalPlaytimeFormatted = FormatPlaytime(monthPlaytime),
                        SeasonIcon = monthNum <= 2 || monthNum == 12 ? "☀️" : (monthNum <= 5 ? "🍂" : (monthNum <= 8 ? "❄️" : "🌸")),
                        MonthlyPersonaId = m.PersonaId,
                        MonthlyMoodTitle = m.MoodTitle,
                        MonthlyMoodTooltip = m.MoodSubtitle,
                        Games = new ObservableCollection<TimelineGameViewModel>()
                    };

                    foreach (var x in m.TopGames) 
                    {
                        bool isFirstTime = x.Game.FirstPlayed.HasValue && x.Game.FirstPlayed.Value.Year == SelectedYear;
                        
                        bool shouldShowCompletedIcon = x.IsCompleted && 
                                                    x.Game.CompletionDate.HasValue && 
                                                    x.Game.CompletionDate.Value.Year == SelectedYear && 
                                                    x.Game.CompletionDate.Value.Month == monthNum;
                        
                        timelineMonth.Games.Add(new TimelineGameViewModel { 
                            GameId = x.GameId, 
                            CoverImagePath = x.CoverImagePath, 
                            PlaytimeFormatted = FormatPlaytime(_cachedSessions[x.GameId].Where(s => s.Date.Month == monthNum).Sum(s => s.Seconds)),
                            ToolTipText = x.Name,
                            IsFirstTime = isFirstTime,
                            IsCompleted = x.IsCompleted,
                            ShouldShowCompletedIcon = shouldShowCompletedIcon
                        });
                    }

                    TimelineMonths.Add(timelineMonth);
                    monthIndex++;
                }

                MonthlyBars.Clear();
                double maxMonthly = stats.MonthlyBars.Count > 0 ? stats.MonthlyBars.Max(b => b.Value) : 1;
                foreach (var m in stats.MonthlyBars) 
                {
                    MonthlyBars.Add(new MonthBarViewModel { 
                        Month = m.Month, 
                        Value = m.Value / maxMonthly, 
                        FormattedValue = FormatPlaytime((long)m.Value) 
                    });
                }

                MonthlyYAxisLabels.Clear();
                for (int i = 0; i <= 4; i++) 
                {
                    double val = (maxMonthly / 4.0) * (4 - i);
                    MonthlyYAxisLabels.Add(new AxisLabelViewModel { 
                        Label = FormatPlaytime((long)val), 
                        Position = i * 0.25 
                    });
                }

                HourlyBars.Clear();
                foreach (var h in stats.HourlyBars) 
                {
                    HourlyBars.Add(new HourlyProfileBarViewModel { 
                        Label = h.Label, 
                        Ratio = h.Ratio, 
                        Hour = h.Label, 
                        ToolTipText = string.Format("{0}h: {1}", h.Label, FormatPlaytime((long)h.Seconds)) 
                    });
                }

                HourlyBadgeText = stats.HourlyPersonaName;
                string iconKey = "STAR"; // Default
                if (stats.HourlyPersonaName.Contains("MADRUGADA")) iconKey = "OWL";
                else if (stats.HourlyPersonaName.Contains("MATUTINO")) iconKey = "SUNRISE";
                else if (stats.HourlyPersonaName.Contains("TURNO")) iconKey = "SUN";
                else if (stats.HourlyPersonaName.Contains("NOTURNO")) iconKey = "MOON";
                HourlyBadgeIcon = iconKey;

                HourlyLinePath = GetSplinePath(HourlyBars.ToList(), false);
                HourlyFillPath = GetSplinePath(HourlyBars.ToList(), true);

                HourlyYAxisLabels.Clear();
                double maxHourly = stats.HourlyBars.Count > 0 ? stats.HourlyBars.Max(b => b.Seconds) : 1;
                for (int i = 0; i <= 4; i++) 
                {
                    double val = (maxHourly / 4.0) * (4 - i);
                    HourlyYAxisLabels.Add(new AxisLabelViewModel { 
                        Label = FormatPlaytime((long)val), 
                        Position = i * 0.25 
                    });
                }

                // Axis Labels


                OnPropertyChanged(string.Empty);
            } catch { }
        }

        private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void OpenGameDetail(GameCardViewModel game)
        {
            if (game == null) return;
            ShowGameDetailById(game.GameId);
        }

        private void ShowGameDetailById(Guid id)
        {
            if (_cachedYearGames == null || _cachedSessions == null) return;
            var game = _cachedYearGames.FirstOrDefault(g => g.GameId == id);
            if (game == null) return;

            long totalGenrePlaytime = 0;
            if (game.Genres != null && game.Genres.Any())
            {
                var primaryGenre = game.Genres.First();
                foreach (var g in _cachedYearGames)
                {
                    if (g.Genres != null && g.Genres.Contains(primaryGenre))
                    {
                        List<ActivitySession> s;
                        if (_cachedSessions.TryGetValue(g.GameId, out s)) totalGenrePlaytime += s.Sum(x => x.Seconds);
                    }
                }
            }

            List<ActivitySession> gameSessions;
            if (!_cachedSessions.TryGetValue(id, out gameSessions)) gameSessions = new List<ActivitySession>();

            SelectedGameDetail = new GameDetailViewModel(game, gameSessions, totalGenrePlaytime);
        }



        private string FormatPlaytime(long seconds)
        {
            long h = seconds / 3600, m = (seconds % 3600) / 60;
            if (h > 0) return string.Format("{0}h {1}m", h, m);
            return string.Format("{0}m", m);
        }

        private string GetSplinePath(List<HourlyProfileBarViewModel> bars, bool closePath)
        {
            if (bars == null || bars.Count == 0) return "";

            int count = bars.Count;
            double width = 1000;
            double height = 200;
            double paddingHeight = 16; // 16 pixels of padding at the top
            double effectiveHeight = height - paddingHeight; 
            double step = width / (count - 1);

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(CultureInfo.InvariantCulture, "M 0,{0:0.0} ", height - (bars[0].Ratio * effectiveHeight));

            for (int i = 1; i < count; i++)
            {
                double x1 = (i - 1) * step + (step / 2);
                double y1 = height - (bars[i-1].Ratio * effectiveHeight);
                double x2 = i * step - (step / 2);
                double y2 = height - (bars[i].Ratio * effectiveHeight);
                double x = i * step;
                double y = height - (bars[i].Ratio * effectiveHeight);

                sb.AppendFormat(CultureInfo.InvariantCulture, "C {0:0.0},{1:0.0} {2:0.0},{3:0.0} {4:0.0},{5:0.0} ", x1, y1, x2, y2, x, y);
            }

            if (closePath)
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, "L {0:0.0},{1:0.0} L 0,{1:0.0} Z", width, height);
            }

            return sb.ToString();
        }

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    // ── Supporting ViewModels ──────────────────────────

    public class GameCardViewModel : INotifyPropertyChanged
    {
        public GameStatsData Game { get; set; }
        public string Name { get { return Game.Name; } }
        public string CoverImagePath { get { return Game.CoverImagePath; } }
        public string TotalHours { get { return (Game.TotalPlaytimeSeconds / 3600.0).ToString("F1"); } }
        public string PlaytimeFormatted { get; set; }
        public string RankText { get; set; }
        public Guid GameId { get { return Game.GameId; } }
        public List<string> Genres { get { return Game.Genres; } }
        public bool IsCompleted { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class DonutSliceViewModel 
    { 
        public string Name { get; set; } 
        public double Value { get; set; } 
        public string Color { get; set; } 
        
        // Extended properties for XAML bindings
        public string Label { get { return Name; } }
        public string PercentText { get { return string.Format("{0:0}%", Value); } }
        public double BarWidthPx { get { return (Value / 100.0) * 220.0; } }
        public string BarBrush { get { return Color; } }
        public string TimeText { get; set; }
    }

    public class GenreBarViewModel 
    { 
        public string Name { get; set; } 
        public double Ratio { get; set; } 
        public string Playtime { get; set; } 
        public long PlaytimeSeconds { get; set; }
        
        // Extended properties for XAML bindings
        public double HeightRatio { get { return Ratio; } }
        public string PlaytimeFormatted { get { return Playtime; } }
        public bool IsTopGenre { get; set; }
        public string ToolTipText { get { return string.Format("{0}: {1} ({2:0}%)", Name, Playtime, Ratio * 100); } }
        public double BarWidth { get { return Ratio * 200; } } 
    }

    public class TimelineMonthViewModel 
    { 
        public string MonthName { get; set; } 
        public string SeasonIcon { get; set; }
        public string MonthlyPersonaId { get; set; }
        public string MonthlyMoodTitle { get; set; }
        public string MonthlyMoodTooltip { get; set; }
        public string GameCount { get; set; }
        public string TotalPlaytimeFormatted { get; set; }
        public int Alignment { get; set; } // 0 = Left, 1 = Right
        public ObservableCollection<TimelineGameViewModel> Games { get; set; } 
    }

    public class TimelineGameViewModel
    {
        public Guid GameId { get; set; }
        public string CoverImagePath { get; set; }
        public bool IsFirstTime { get; set; }
        public bool IsCompleted { get; set; }
        public bool ShouldShowCompletedIcon { get; set; }
        public string PlaytimeFormatted { get; set; }
        public string ToolTipText { get; set; }
    }

    public class AxisLabelViewModel { public string Label { get; set; } public double Y { get; set; } public double Position { get; set; } }
    
    public class MonthBarViewModel 
    { 
        public string Month { get; set; } 
        public double Value { get; set; } 
        public string FormattedValue { get; set; }
        
        // Properties for XAML bindings
        public string Hours { get { return FormattedValue; } }
        public double BarHeight { get { return Value * 280; } } // Scale to max height in XAML
        public string MonthName { get { return Month; } }
        public string ToolTipText { get { return string.Format("{0}: {1}", Month, FormattedValue); } }
    }
    
    public class SeasonViewModel { public string Name { get; set; } public string Period { get; set; } public string Playtime { get; set; } public string Icon { get; set; } }
    public class MoodTagViewModel { public string Name { get; set; } public int Count { get; set; } public string PlaytimeFormatted { get; set; } }
    public class MoodInfo { public string Title { get; set; } public string Subtitle { get; set; } public string IconPath { get; set; } public string TriggerList { get; set; } }
    
    public class HourlyProfileBarViewModel 
    { 
        public string Label { get; set; } 
        public double Ratio { get; set; } 
        public string Hour { get; set; }
        public string ToolTipText { get; set; }
    }

    public class StudioViewModel
    {
        public string Name { get; set; }
        public string PlaytimeFormatted { get; set; }
        public int GameCount { get; set; }
        public ObservableCollection<string> TopGames { get; set; }
        public string IconPath { get; set; }
        public bool HasIcon { get; set; }
        public string Initials { get; set; }
        public string Color { get; set; }
        public double Ratio { get; set; }
        public double BarWidthPx { get { return Math.Max(Ratio * 220.0, Ratio > 0 ? 4 : 0); } }
    }
}
