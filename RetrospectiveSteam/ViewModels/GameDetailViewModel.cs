using System;
using System.Collections.Generic;
using System.Linq;
using RetrospectiveSteam.Services;

namespace RetrospectiveSteam.ViewModels
{
    public class GameDetailViewModel
    {
        public GameStatsData Game { get; private set; }
        public List<ActivitySession> Sessions { get; private set; }
        
        public string Name { get { return Game.Name; } }
        public string CoverImagePath { get { return Game.CoverImagePath; } }
        public bool IsCompleted { get { return Game.IsCompleted; } }
        public string BackgroundImagePath { get { return Game.BackgroundImagePath; } }
        public string Platform { get { return Game.Platforms != null ? string.Join(", ", Game.Platforms) : "PC"; } }
        public string Genres { get { return Game.Genres != null ? string.Join(" • ", Game.Genres) : ""; } }
        
        public string TotalPlaytimeFormatted { get { return FormatPlaytime(Game.TotalPlaytimeSeconds); } }
        public long TotalPlaytimeSeconds { get { return Game.TotalPlaytimeSeconds; } }
        public Guid GameId { get { return Game.GameId; } }
        public string UnlockedAchievementsText { get { return string.Format("{0} / {1}", Game.UnlockedAchievements, Game.TotalAchievements); } }
        public double AchievementProgress { get { return Game.TotalAchievements > 0 ? (double)Game.UnlockedAchievements / Game.TotalAchievements * 100 : 0; } }

        // "Encantamento" Info
        public string LongestSessionText { get; private set; }
        public string PreferredPeriod { get; private set; }
        public string SynergyText { get; private set; }
        public string TotalSessionsText { get; private set; }

        public List<MonthlySessionData> MonthlyGraph { get; private set; }
        public List<AxisLabelViewModel> YAxisLabels { get; private set; }
        public double MaxHours { get; private set; }

        public List<SessionGraphData> SessionsGraph { get; private set; }
        public List<AxisLabelViewModel> SessionYAxisLabels { get; private set; }
        public double SessionMaxHours { get; private set; }

        public GameDetailViewModel(GameStatsData game, List<ActivitySession> sessions, long totalGenrePlaytime)
        {
            this.Game = game;
            this.Sessions = sessions;

            this.YAxisLabels = new List<AxisLabelViewModel>();
            this.SessionYAxisLabels = new List<AxisLabelViewModel>();
            CalculateMarathon(sessions);
            CalculatePreferredPeriod(sessions);
            CalculateSynergy(game, totalGenrePlaytime);
            BuildMonthlyGraph(sessions);
            BuildSessionsGraph(game, sessions);
            this.TotalSessionsText = sessions != null ? sessions.Count.ToString() : "0";
        }

        private void CalculateMarathon(List<ActivitySession> sessions)
        {
            if (sessions == null || !sessions.Any())
            {
                LongestSessionText = "Nenhuma sessão registrada.";
                return;
            }

            var longest = sessions.OrderByDescending(s => s.Seconds).First();
            LongestSessionText = string.Format("Sua maratona recorde foi de {0} em {1:dd/MM}.", 
                FormatPlaytime(longest.Seconds), longest.Date);
        }

        private void CalculatePreferredPeriod(List<ActivitySession> sessions)
        {
            if (sessions == null || !sessions.Any())
            {
                PreferredPeriod = "Desconhecido";
                return;
            }

            // Count sessions by hour group
            var hours = sessions.GroupBy(s => s.Date.Hour)
                                .Select(g => new { Hour = g.Key, Count = g.Count() })
                                .OrderByDescending(x => x.Count)
                                .First().Hour;

            if (hours >= 5 && hours < 12) PreferredPeriod = "Madrugador (Manhã)";
            else if (hours >= 12 && hours < 18) PreferredPeriod = "Explorador Diurno (Tarde)";
            else if (hours >= 18 && hours < 23) PreferredPeriod = "Gamer do Crepúsculo (Noite)";
            else PreferredPeriod = "Corujão (Madrugada)";
        }

        private void CalculateSynergy(GameStatsData game, long totalGenrePlaytime)
        {
            if (totalGenrePlaytime <= 0)
            {
                SynergyText = "Este é o seu principal jogo deste estilo.";
                return;
            }

            double percent = (double)Game.TotalPlaytimeSeconds / totalGenrePlaytime * 100;
            SynergyText = string.Format("Este jogo representa {0:N1}% das suas horas no gênero.", percent);
        }

        private void BuildMonthlyGraph(List<ActivitySession> sessions)
        {
            var months = new long[12];
            foreach (var s in sessions)
            {
                months[s.Date.Month - 1] += s.Seconds;
            }

            long maxSeconds = months.Max();
            double maxHours = maxSeconds / 3600.0;
            
            // Round up to a nice number for the Y-axis (multiple of 1, 5, or 10 depending on scale)
            double dynamicMax;
            if (maxHours <= 5) dynamicMax = Math.Ceiling(maxHours);
            else if (maxHours <= 20) dynamicMax = Math.Ceiling(maxHours / 2.0) * 2.0;
            else dynamicMax = Math.Ceiling(maxHours / 5.0) * 5.0;
            
            if (dynamicMax < 1) dynamicMax = 1;
            this.MaxHours = dynamicMax;

            // Generate 5 Y-axis labels/grid lines
            YAxisLabels.Clear();
            for (int i = 0; i <= 4; i++)
            {
                double labelVal = dynamicMax - (i * (dynamicMax / 4.0));
                YAxisLabels.Add(new AxisLabelViewModel 
                { 
                    Label = labelVal % 1 == 0 ? labelVal.ToString("0") + "h" : labelVal.ToString("0.0") + "h",
                    Position = i * 0.25
                });
            }

            double totalDenominator = dynamicMax * 3600.0;
            MonthlyGraph = months.Select((val, idx) => new MonthlySessionData
            {
                MonthName = GetMonthShortName(idx + 1),
                Value = val,
                Ratio = (double)val / totalDenominator,
                PlaytimeFormatted = FormatPlaytime(val)
            }).ToList();
        }

        private void BuildSessionsGraph(GameStatsData game, List<ActivitySession> sessions)
        {
            if (sessions == null || !sessions.Any())
            {
                SessionsGraph = new List<SessionGraphData>();
                return;
            }

            // Order chronologically
            var sortedSessions = sessions.OrderBy(s => s.Date).ToList();
            
            // Limit to last 30 sessions if there are too many, to avoid overcrowding the UI
            // But let's try to show all first.
            double maxSecs = sortedSessions.Max(s => s.Seconds);
            double maxHours = maxSecs / 3600.0;
            
            if (maxHours <= 2) SessionMaxHours = 2;
            else if (maxHours <= 5) SessionMaxHours = 5;
            else SessionMaxHours = Math.Ceiling(maxHours / 2.0) * 2.0;

            // Mark completion if it happened during a session
            // game.CompletionDate is from native or success story
            DateTime? compDate = game.CompletionDate;

            SessionsGraph = sortedSessions.Select((s, idx) => {
                bool isComp = false;
                if (compDate.HasValue)
                {
                    // If session end time is after completion date AND completion date is after session start date
                    // We mark the FIRST session that ends after the completion date as the completion session.
                    // This is an approximation.
                    var sessionEnd = s.Date.AddSeconds(s.Seconds);
                    if (compDate.Value <= sessionEnd && compDate.Value >= s.Date)
                    {
                        isComp = true;
                        compDate = null; // Mark only once
                    }
                }

                return new SessionGraphData
                {
                    SessionIndex = idx + 1,
                    Hours = s.Seconds / 3600.0,
                    Ratio = (double)s.Seconds / (SessionMaxHours * 3600.0),
                    IsCompletionSession = isComp,
                    PlaytimeFormatted = FormatPlaytime(s.Seconds),
                    DateFormatted = s.Date.ToString("dd/MM")
                };
            }).ToList();

            // Y Axis for sessions
            SessionYAxisLabels.Clear();
            for (int i = 0; i <= 4; i++)
            {
                double labelVal = SessionMaxHours - (i * (SessionMaxHours / 4.0));
                SessionYAxisLabels.Add(new AxisLabelViewModel 
                { 
                    Label = labelVal % 1 == 0 ? labelVal.ToString("0") + "h" : labelVal.ToString("0.0") + "h",
                    Position = i * 0.25
                });
            }
        }

        private string GetMonthShortName(int m)
        {
            switch (m)
            {
                case 1: return "JAN"; case 2: return "FEV"; case 3: return "MAR";
                case 4: return "ABR"; case 5: return "MAI"; case 6: return "JUN";
                case 7: return "JUL"; case 8: return "AGO"; case 9: return "SET";
                case 10: return "OUT"; case 11: return "NOV"; case 12: return "DEZ";
                default: return "";
            }
        }

        private string FormatPlaytime(long seconds)
        {
            long h = seconds / 3600, m = (seconds % 3600) / 60;
            if (h > 0) return string.Format("{0}h {1}m", h, m);
            return string.Format("{0}m", m);
        }
    }

    public class MonthlySessionData
    {
        public string MonthName { get; set; }
        public long Value { get; set; }
        public double Ratio { get; set; }
        public string PlaytimeFormatted { get; set; }
    }

    public class SessionGraphData
    {
        public int SessionIndex { get; set; }
        public double Hours { get; set; }
        public double Ratio { get; set; }
        public bool IsCompletionSession { get; set; }
        public string PlaytimeFormatted { get; set; }
        public string DateFormatted { get; set; }
        public string ToolTip { get { return string.Format("Sessão {0}: {1} ({2})", SessionIndex, PlaytimeFormatted, DateFormatted); } }
    }
}
