using System;
using System.Collections.Generic;
using RetrospectivaAnual.Services;

namespace RetrospectivaAnual.Services
{
    public class YearlyStatsData
    {
        public string Username { get; set; }
        public List<GameStatsData> Games { get; set; }
        public string TotalPlaytime { get; set; }
        public int GamesPlayedCount { get; set; }
        public int SessionsCount { get; set; }
        public int NewGamesCount { get; set; }
        public int AchievementsCount { get; set; }
        public string PersonaId { get; set; }
        public string Title { get; set; }
        public string MoodTitle { get; set; }
        public string MoodSubtitle { get; set; }
        public string MoodTopTags { get; set; }

        public List<GameCardViewModelStub> TopGames { get; set; }
        public List<MoodResult> MonthlyMoodHistory { get; set; }
        public List<string> MoodTopGames { get; set; }
        public List<MoodTagViewModelStub> MoodTags { get; set; }
        public List<MoodInfluencer> Influencers { get; set; }
        public List<TimelineMonthViewModelStub> TimelineMonths { get; set; }
        
        public List<HourlyProfileBarViewModelStub> HourlyBars { get; set; }
        public List<AxisLabelViewModelStub> HourlyYAxisLabels { get; set; }
        public string HourlyPersonaName { get; set; }
        public string HourlyPersonaIcon { get; set; }
        public string HourlyPersonaDescription { get; set; }
        public string AverageSessionTime { get; set; }
        public int LongestStreakDays { get; set; }

        public List<GenreBarViewModelStub> GenreBars { get; set; }
        public List<AxisLabelViewModelStub> GenreYAxisLabels { get; set; }
        
        public List<GenreBarViewModelStub> PlatformBars { get; set; }
        public List<AxisLabelViewModelStub> PlatformYAxisLabels { get; set; }
        
        public List<MonthBarViewModelStub> MonthlyBars { get; set; }
        public List<AxisLabelViewModelStub> MonthlyYAxisLabels { get; set; }
        
        public List<StudioStatsData> TopDevelopers { get; set; }
        public List<StudioStatsData> TopPublishers { get; set; }
        public List<MoodInfoStub> MoodDictionary { get; set; }
    }

    public class MonthBarViewModelStub { public string Month { get; set; } public double Value { get; set; } }

    // Stubs for reconstruction - these will be mapped to actual ViewModels in RetrospectiveViewModel
    public class GameCardViewModelStub { public GameStatsData Game { get; set; } public string Name { get { return Game.Name; } } public string CoverImagePath { get { return Game.CoverImagePath; } } public Guid GameId { get { return Game.GameId; } } public List<string> Genres { get { return Game.Genres; } } public bool IsCompleted { get { return Game.IsCompleted; } } }
    public class MoodTagViewModelStub { public string Name { get; set; } public int Count { get; set; } public string PlaytimeFormatted { get; set; } }
    public class TimelineMonthViewModelStub { public string MonthName { get; set; } public List<GameCardViewModelStub> TopGames { get; set; } public string MoodTitle { get; set; } public string MoodSubtitle { get; set; } public string PersonaId { get; set; } }
    public class HourlyProfileBarViewModelStub { public string Label { get; set; } public double Seconds { get; set; } public double Ratio { get; set; } }
    public class GenreBarViewModelStub { public string Name { get; set; } public double Ratio { get; set; } public string Playtime { get; set; } public long PlaytimeSeconds { get; set; } }
    public class AxisLabelViewModelStub { public string Label { get; set; } public double Y { get; set; } }


    public class MoodInfoStub
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string IconPath { get; set; }
        public string TriggerList { get; set; }
    }
}
