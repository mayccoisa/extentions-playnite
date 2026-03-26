using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using RetrospectiveSteam.Services;
using System.Globalization;

namespace RetrospectiveSteam.Services
{

    public class MoodInfo
    {
        public string   Id       { get; set; }
        public string   Title    { get; set; }
        public string[] Triggers { get; set; }
        public string   Subtitle { get; set; }
        public string TriggerList { get { return string.Join(", ", Triggers); } }
        public string IconPath { get { return GetMoodIcon(Id); } }

        private string GetMoodIcon(string id)
        {
            switch(id)
            {
                case "EPIC":   return "M12,2L4.5,20.29L5.21,21L12,18L18.79,21L19.5,20.29L12,2Z";
                case "ROUGE":  return "M12,22C17.52,22 22,17.52 22,12C22,6.48 17.52,2 12,2C6.48,2 2,6.48 2,12C2,17.52 6.48,22 12,22M12,20C7.59,20 4,16.41 4,12C4,7.59 7.59,4 12,4C16.41,4 20,7.59 20,12C20,16.41 16.41,20 12,20M15,10.5C15,11.33 14.33,12 13.5,12C12.67,12 12,11.33 12,10.5C12,9.67 12.67,9 13.5,9C14.33,9 15,9.67 15,10.5M10.5,12C9.67,12 9,11.33 9,10.5C9,9.67 9.67,9 10.5,9C11.33,9 12,9.67 12,10.5C12,11.33 11.33,12 10.5,12M12,13C14,13 16,14 16,16H8C8,14 10,13 12,13Z";
                case "CHILL":  return "M12,3L2,12H5V20H19V12H22L12,3M12,7.7C14.1,7.7 15.8,9.4 15.8,11.5C15.8,14.5 12,18.8 12,18.8C12,18.8 8.2,14.5 8.2,11.5C8.2,9.4 9.9,7.7 12,7.7Z";
                case "SHOOT":  return "M7,5H10V2H14V5H17V8H14V11H10V8H7V5M20,10V12H18V10H20M20,14V16H18V14H20M6,10V12H4V10H6M6,14V16H4V14H6M12,14A2,2 0 0,1 14,16A2,2 0 0,1 12,18A2,2 0 0,1 10,16A2,2 0 0,1 12,14Z";
                case "ARCHI":  return "M3,13H9V19H3V13M15,13H21V19H15V13M3,5H9V11H3V5M15,5H21V11H15V5M11,13H13V15H11V13M11,17H13V19H11V17M11,5H13V7H11V5M11,9H13V11H11V9Z";
                case "HORROR": return "M12,2C11.5,2 11,2.19 10.59,2.59L2.59,10.59C2.19,11 2,11.5 2,12C2,12.5 2.19,13 2.59,13.41L10.59,21.41C11,21.81 11.5,22 12,22C12.5,22 13,21.81 13.41,21.41L21.41,13.41C21.81,13 22,12.5 22,12C22,11.5 21.81,11 21.41,10.59L13.41,2.59C13,2.19 12.5,2 12,2M12,4L20,12L12,20L4,12L12,4M11,7V13H13V7H11M11,15V17H13V15H11Z";
                case "PARTY":  return "M16,13C15.71,13 15.38,13 15.03,13.05C16.19,13.89 17,15.13 17,16.5V19H23V16.5C23,14.17 18.33,13 16,13M8,13C5.67,13 1,14.17 1,16.5V19H15V16.5C15,14.17 10.33,13 8,13M8,11A3,3 0 1,0 5,8A3,3 0 0,0 8,11M16,11A3,3 0 1,0 13,8A3,3 0 0,0 16,11Z";
                case "RACE":   return "M18.92,6.01C18.72,5.42 18.16,5 17.5,5H6.5C5.84,5 5.28,5.42 5.08,6.01L3,12V20A1,1 0 0,0 4,21H5A1,1 0 0,0 6,20V19H18V20A1,1 0 0,0 19,21H20A1,1 0 0,0 21,20V12L18.92,6.01M6.5,16A1.5,1.5 0 1,1 8,14.5A1.5,1.5 0 0,1 6.5,16M17.5,16A1.5,1.5 0 1,1 19,14.5A1.5,1.5 0 0,1 17.5,16M5,11L6.5,6.5H17.5L19,11H5Z";
                case "METRO":  return "M20,6H12L10,4H4C2.89,4 2,4.89 2,6V18C2,19.1 2.89,20 4,20H20C21.1,20 22,19.1 22,18V8C22,6.89 21.1,6 20,6M20,18H4V8H20V18M11,13H11V13M15,13H15V13M13,11H13V11Z";
                case "STORY":  return "M18,2H6A2,2 0 0,0 4,4V20A2,2 0 0,0 6,22H18A2,2 0 0,0 20,20V4A2,2 0 0,0 18,2M18,20H6V4H18V20M8,6H16V8H8V6M8,10H16V12H8V10M8,14H13V16H8V14";
                case "CYBER":  return "M12,2L1,21H23L12,2M12,6L19.53,19H4.47L12,6M11,10V14H13V10H11M11,16V18H13V16H11Z";
                case "MIX":    return "M12,17.27L18.18,21L16.54,13.97L22,9.24L14.81,8.62L12,2L9.19,8.62L2,9.24L7.45,13.97L5.82,21L12,17.27Z";
                case "RETRO":  return "M17,2H7A2,2 0 0,0 5,4V20A2,2 0 0,0 7,22H17A2,2 0 0,0 19,20V4A2,2 0 0,0 17,2M17,11H7V4H17V11M9,6V9H15V6H9M11,13V15H13V13H11M14,13V15H16V13H14M8,13V15H10V13H8Z";
                default:       return "M12,2L4.5,20.29L5.21,21L12,18L18.79,21L19.5,20.29L12,2Z";
            }
        }
    }

    internal class MoodGameInfo
    {
        public GameStatsData Game { get; set; }
        public long YearSeconds { get; set; }
        public string MatchedTag { get; set; }
        public MoodGameInfo(GameStatsData g, long s, string tag) { Game = g; YearSeconds = s; MatchedTag = tag; }
    }

    public class MoodInfluencer
    {
        public string GameName { get; set; }
        public string MatchedTag { get; set; }
        public long PlaytimeSeconds { get; set; }
        public string PlaytimeFormatted { get; set; }
    }

    public class PlayniteDataService
    {


        private readonly IPlayniteAPI playniteApi;

        public PlayniteDataService(IPlayniteAPI api)
        {
            this.playniteApi = api;
        }

        public string GetUsername() { return Environment.UserName; }

        public void NavigateToGame(Guid gameId) { playniteApi.MainView.SelectGame(gameId); }

        /// <summary>Get games with any activity in the given year.</summary>
        public List<GameStatsData> GetGamesPlayedInYear(int year)
        {
            var start = new DateTime(year, 1, 1);
            var end   = new DateTime(year, 12, 31, 23, 59, 59);

            return playniteApi.Database.Games
                .Where(g => g.LastActivity >= start && g.LastActivity <= end)
                .Select(g => MapGame(g))
                .OrderByDescending(g => g.TotalPlaytimeSeconds)
                .ToList();
        }

        /// <summary>Get top N games by all-time playtime.</summary>
        public List<GameStatsData> GetTopGames(int count = 5)
        {
            return playniteApi.Database.Games
                .Where(g => g.Playtime > 0)
                .OrderByDescending(g => g.Playtime)
                .Take(count)
                .Select(g => MapGame(g))
                .ToList();
        }

        /// <summary>Total lifetime playtime in hours (all games).</summary>
        public long GetTotalPlaytimeHours()
        {
            return playniteApi.Database.Games.Sum(g => (long)g.Playtime) / 3600;
        }

        public YearlyStatsData GetYearlyStats(int year, GameActivityService activityService)
        {
            // 1. Get all sessions for the target year across all games
            var allSessions = activityService.GetSessionsForYear(year);
            
            // 2. Map only games that have actual recorded play sessions in that year
            var yearGames = allSessions.Keys
                .Select(id => playniteApi.Database.Games.Get(id))
                .Where(g => g != null)
                .Select(g => MapGame(g))
                .OrderByDescending(g => allSessions[g.GameId].Sum(s => s.Seconds))
                .ToList();

            if (yearGames.Count == 0)
            {
                // Fallback to LastActivity if no detailed sessions found (rare in 2026 but possible)
                var fallbackGames = GetGamesPlayedInYear(year);
                if (fallbackGames.Count > 0)
                {
                    yearGames = fallbackGames;
                    foreach (var g in yearGames) allSessions[g.GameId] = activityService.GetSessionsForYear(g.GameId, year);
                }
            }

            var mood = CalculateYearlyMood(year, yearGames, allSessions);
            var genreBreakdown = GetGenreBreakdown(yearGames, allSessions);
            var platformBreakdown = GetPlatformBreakdown(yearGames, allSessions);
            var devBreakdown = GetDeveloperBreakdown(yearGames, allSessions, 5);
            var pubBreakdown = GetPublisherBreakdown(yearGames, allSessions, 5);
            var dailyActivity = GetDailyActivity(year, yearGames, activityService);
            var monthlyTotals = GetMonthlyTotals(year, yearGames, activityService);
            var hourlyDist = GetHourlyDistribution(allSessions);

            long totalSecondsInYear = allSessions.Values.SelectMany(s => s).Sum(s => s.Seconds);
            int totalSessions = allSessions.Values.Sum(s => s.Count);

            var stats = new YearlyStatsData
            {
                Username = GetUsername(),
                Games = yearGames,
                TotalPlaytime = FormatPlaytime(totalSecondsInYear),
                GamesPlayedCount = yearGames.Count,
                SessionsCount = totalSessions,
                NewGamesCount = GetGamesAddedInYear(year),
                AchievementsCount = GetAchievementCount(year),
                PersonaId = mood.PersonaId,
                Title = mood.Title,
                MoodTitle = mood.Title,
                MoodSubtitle = mood.Subtitle,
                MoodTopTags = mood.Title, // Just in case
                
                TopGames = yearGames.Take(6).Select(g => new GameCardViewModelStub { Game = g }).ToList(),
                MonthlyMoodHistory = new List<MoodResult>(),
                MoodTopGames = mood.TopGameCovers, // Now using covers instead of names
                MoodTags = mood.TopTagsUsed != null ? mood.TopTagsUsed.Select(t => new MoodTagViewModelStub { Name = t, Count = 1 }).ToList() : new List<MoodTagViewModelStub>(),
                Influencers = mood.Influencers,
                TimelineMonths = new List<TimelineMonthViewModelStub>(),
                
                HourlyBars = hourlyDist.Select(kv => new HourlyProfileBarViewModelStub { Label = kv.Key.ToString("D2"), Seconds = kv.Value, Ratio = totalSecondsInYear > 0 ? (double)kv.Value / hourlyDist.Values.Max() : 0 }).ToList(),
                HourlyYAxisLabels = new List<AxisLabelViewModelStub>(),
                
                GenreBars = genreBreakdown.Select(g => new GenreBarViewModelStub { Name = g.Name.ToUpper(), Ratio = g.Ratio, Playtime = FormatPlaytime(g.PlaytimeSeconds), PlaytimeSeconds = g.PlaytimeSeconds }).ToList(),
                GenreYAxisLabels = new List<AxisLabelViewModelStub>(),

                PlatformBars = platformBreakdown.Select(p => new GenreBarViewModelStub { Name = p.Name.ToUpper(), Ratio = p.Ratio, Playtime = FormatPlaytime(p.PlaytimeSeconds), PlaytimeSeconds = p.PlaytimeSeconds }).ToList(),
                PlatformYAxisLabels = new List<AxisLabelViewModelStub>(),

                MonthlyBars = monthlyTotals.Select((val, idx) => new MonthBarViewModelStub { Month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(idx + 1).ToUpper(), Value = val }).ToList(),
                MonthlyYAxisLabels = new List<AxisLabelViewModelStub>(),

                TopDevelopers = devBreakdown,
                TopPublishers = pubBreakdown,
                AverageSessionTime = totalSessions > 0 ? FormatPlaytime((long)totalSecondsInYear / totalSessions) : "0m",
                LongestStreakDays = GetLongestStreak(year, yearGames, activityService)
            };

            IdentifyHourlyPersona(hourlyDist, stats);

            for (int i = 1; i <= 12; i++)
            {
                var monthMood = CalculateYearlyMood(year, yearGames, allSessions, i);
                monthMood.MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i).ToUpper();
                stats.MonthlyMoodHistory.Add(monthMood);

                if (monthMood.HasData)
                {
                    stats.TimelineMonths.Add(new TimelineMonthViewModelStub 
                    { 
                        MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i),
                        MoodTitle = monthMood.Title,
                        MoodSubtitle = monthMood.Subtitle,
                        PersonaId = monthMood.PersonaId,
                        TopGames = yearGames
                            .Where(g => allSessions.ContainsKey(g.GameId) && allSessions[g.GameId].Any(s => s.Date.Year == year && s.Date.Month == i))
                            .OrderByDescending(g => allSessions[g.GameId].Where(s => s.Date.Year == year && s.Date.Month == i).Sum(s => s.Seconds))
                            .Select(g => new GameCardViewModelStub { Game = g })
                            .ToList()
                    });
                }
            }

            stats.MoodDictionary = GetMoodDictionary();

            return stats;
        }

        /// <summary>Count games added in the given year.</summary>
        public int GetGamesAddedInYear(int year)
        {
            var start = new DateTime(year, 1, 1);
            var end   = new DateTime(year, 12, 31, 23, 59, 59);
            return playniteApi.Database.Games.Count(g => g.Added >= start && g.Added <= end);
        }

        /// <summary>Total achievement count stored in Playnite native data.</summary>
        public int GetAchievementCount(int year)
        {
            var total = 0;
            // Scan all games in the library for achievement data
            foreach (var game in playniteApi.Database.Games)
            {
                var successStoryPath = GetSuccessStoryPath(game.Id);
                if (successStoryPath != null && File.Exists(successStoryPath))
                {
                    try
                    {
                        var json = File.ReadAllText(successStoryPath);
                        var data = JsonConvert.DeserializeObject<SuccessStoryRoot>(json);
                        if (data != null && data.Items != null)
                        {
                            // SuccessStory only adds DateUnlocked if the achievement is unlocked
                            total += data.Items.Count(i => i.DateUnlocked.HasValue && i.DateUnlocked.Value.Year == year);
                        }
                    }
                    catch { /* ignore parse errors */ }
                }
            }
            return total;
        }

        private string GetSuccessStoryPath(Guid gameId)
        {
            var extData = playniteApi.Paths.ExtensionsDataPath;
            if (!Directory.Exists(extData)) return null;

            foreach (var dir in Directory.GetDirectories(extData))
            {
                // Try direct SuccessStory folder inside the extension data dir
                var candidate = Path.Combine(dir, "SuccessStory", string.Format("{0}.json", gameId));
                if (File.Exists(candidate)) return candidate;
                
                // Try legacy Achievements subfolder
                var candidateLegacy = Path.Combine(dir, "SuccessStory", "Achievements", string.Format("{0}.json", gameId));
                if (File.Exists(candidateLegacy)) return candidateLegacy;
            }
            return null;
        }

        /// <summary>
        /// Distribute playtime in a year by game age:
        ///   New Releases = released in the same year or year before
        ///   Recent       = 2-5 years old
        ///   Classics     = older than 5 years / no release date
        /// Returns percentages (0–100).
        /// </summary>
        public PlaytimeAgeDistribution GetPlaytimeByGameAge(int year, List<GameStatsData> yearGames)
        {
            long newReleases = 0, recent = 0, classics = 0;

            foreach (var g in yearGames)
            {
                var releaseYear = g.ReleaseYear;
                long pt = g.TotalPlaytimeSeconds;

                if (!releaseYear.HasValue || releaseYear.Value < 1980)
                    classics += pt;
                else if (year - releaseYear.Value <= 1)
                    newReleases += pt;
                else if (year - releaseYear.Value <= 5)
                    recent += pt;
                else
                    classics += pt;
            }

            long total = newReleases + recent + classics;
            if (total == 0) total = 1;

            return new PlaytimeAgeDistribution
            {
                NewReleasesPercent = (double)newReleases / total * 100,
                RecentPercent      = (double)recent      / total * 100,
                ClassicsPercent    = (double)classics    / total * 100,
                NewReleasesSeconds = newReleases,
                RecentSeconds      = recent,
                ClassicsSeconds    = classics
            };
        }

        /// <summary>Calculate the player's 'Mood of the Year' based on tags and genres.</summary>
        public List<MoodInfo> GetMoodDefinitions()
        {
            return new List<MoodInfo>
            {
                new MoodInfo { Id = "EPIC",   Title = "O HERÓI DE ÉPICOS", Triggers = new[] { "RPG", "Fantasia", "Mundo Aberto", "JRPG", "Adventure", "Aventura", "High Fantasy", "Open World" }, Subtitle = "Sua jornada foi marcada por espadas, magia e mundos vastos." },
                new MoodInfo { Id = "ROUGE",  Title = "O SOBREVIVENTE IMPLACÁVEL", Triggers = new[] { "Roguelike", "Roguelite", "Souls-like", "Permadeath", "Difficult", "Hard", "Difícil" }, Subtitle = "Você riu na cara do 'Game Over' e tentou só mais uma vez." },
                new MoodInfo { Id = "CHILL",  Title = "O MESTRE DO CHILL", Triggers = new[] { "Cozy", "Simulação Agrícola", "Sandbox", "Relaxante", "Farming", "Life Sim", "Casual", "Relaxing" }, Subtitle = "Seu ano foi puro relaxamento, construindo e cultivando em paz." },
                new MoodInfo { Id = "SHOOT",  Title = "O ATIRADOR DE ELITE", Triggers = new[] { "FPS", "Shooter", "Battle Royale", "Tático", "Tiros em Primeira Pessoa", "Action", "Ação", "Tactical", "War", "Guerra" }, Subtitle = "Reflexos rápidos, mira precisa e muita munição gasta." },
                new MoodInfo { Id = "ARCHI",  Title = "O ARQUITETO DE IMPÉRIOS", Triggers = new[] { "Estratégia", "RTS", "4X", "Construção de Cidades", "Card Battler", "Strategy", "Management", "Simulation", "Simulação", "City Builder" }, Subtitle = "Planos complexos, economia e vitórias calculadas friamente." },
                new MoodInfo { Id = "HORROR", Title = "O CAÇADOR DE PESADELOS", Triggers = new[] { "Terror", "Survival Horror", "Zumbis", "Psicológico", "Horror", "Zombies", "Scary" }, Subtitle = "Você escolheu passar o ano com a luz acesa e o coração acelerado." },
                new MoodInfo { Id = "PARTY",  Title = "A ALMA DA FESTA", Triggers = new[] { "Co-op", "Multiplayer Local", "Party Game", "Luta", "Fighting", "PvP", "Multiplayer" }, Subtitle = "Sessões caóticas, risadas e amizades (quase) destruídas." },
                new MoodInfo { Id = "RACE",   Title = "O REI DAS PISTAS", Triggers = new[] { "Corrida", "Esportes", "Simulação de Voo", "Racing", "Sports", "Driving" }, Subtitle = "Sua paixão foi a velocidade, a precisão e a quebra de recordes." },
                new MoodInfo { Id = "METRO",  Title = "O CARTÓGRAFO DE PIXELS", Triggers = new[] { "Metroidvania", "Plataforma 2D", "Pixel Art", "Platformer", "2D", "Platform" }, Subtitle = "Você explorou cada canto do mapa em busca de segredos escondidos." },
                new MoodInfo { Id = "RETRO",  Title = "LEGENDA DOS ARCADES", Triggers = new[] { "Arcade", "Retrô", "Clássico", "Classic", "Old School" }, Subtitle = "Sua diversão foi direta ao ponto: ação rápida e a nostalgia dos fliperamas." },
                new MoodInfo { Id = "STORY",  Title = "O INVESTIGADOR NARRATIVO", Triggers = new[] { "Rico em História", "Visual Novel", "Detetive", "Noir", "Mistério", "Story Rich", "Drama" }, Subtitle = "Você viveu pelo enredo, desvendando mistérios e tomando decisões difíceis." },
                new MoodInfo { Id = "CYBER",  Title = "O INFILTRADO DISTÓPICO", Triggers = new[] { "Cyberpunk", "Sci-Fi", "Furtivo", "Stealth", "Hack & Slash", "Space", "Espaço", "Science Fiction", "Mecha" }, Subtitle = "Luzes de neon, tecnologia avançada e futuros não tão distantes." }
            };
        }

        public MoodResult CalculateYearlyMood(int year, List<GameStatsData> yearGames, Dictionary<Guid, List<ActivitySession>> allSessions, int? month = null)
        {
            var moods = GetMoodDefinitions();

            var scores = moods.ToDictionary(m => m.Title, m => 0L);
            var winningTags = new Dictionary<string, HashSet<string>>();
            var moodGameMap = moods.ToDictionary(m => m.Title, m => new List<MoodGameInfo>());

            foreach (var g in yearGames)
            {
                long playtime = 0;
                List<ActivitySession> sessions;
                if (allSessions.TryGetValue(g.GameId, out sessions))
                {
                    if (month.HasValue)
                        playtime = sessions.Where(s => s.Date.Year == year && s.Date.Month == month.Value).Sum(s => (long)s.Seconds);
                    else
                        playtime = sessions.Where(s => s.Date.Year == year).Sum(s => (long)s.Seconds);
                }

                if (playtime <= 0) continue;

                var allGameMetadata = (g.Genres ?? new List<string>()).Concat(g.Tags ?? new List<string>()).ToList();
                
                foreach (var mood in moods)
                {
                    bool match = false;
                    string moodSpecificMatch = null;

                    foreach (var trigger in mood.Triggers)
                    {
                        if (allGameMetadata.Any(gt => gt.Equals(trigger, StringComparison.OrdinalIgnoreCase)))
                        {
                            match = true;
                            moodSpecificMatch = trigger;
                            if (!winningTags.ContainsKey(mood.Title)) winningTags[mood.Title] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                            winningTags[mood.Title].Add(trigger);
                        }
                    }

                    if (match)
                    {
                        scores[mood.Title] += playtime;
                        moodGameMap[mood.Title].Add(new MoodGameInfo(g, playtime, moodSpecificMatch));
                    }
                }
            }

            var winner = scores.OrderByDescending(kv => kv.Value).FirstOrDefault();

            if (winner.Value == 0)
            {
                return new MoodResult
                {
                    HasData = false,
                    PersonaId = "MIX",
                    Title = "",
                    Subtitle = "Assim que o sistema tiver informações, ele apresentará o mood.",
                    TopTags = "",
                    TopGameCovers = new List<string>(),
                    Influencers = new List<MoodInfluencer>()
                };
            }

            var moodData = moods.First(m => m.Title == winner.Key);
            var topTags = winningTags.ContainsKey(winner.Key) 
                ? string.Join(", ", winningTags[winner.Key].Take(3).Select(t => t.ToUpper()))
                : "";
            
            var topCovers = new List<string>();
            var topNames = new List<string>();
            var topGames = moodGameMap[winner.Key]
                .OrderByDescending(gi => gi.YearSeconds)
                .Take(10)
                .ToList();

            foreach (var tg in topGames.Take(3))
            {
                if (!string.IsNullOrEmpty(tg.Game.CoverImagePath)) topCovers.Add(tg.Game.CoverImagePath);
            }
            foreach (var tg in topGames)
            {
                topNames.Add(tg.Game.Name);
            }

            var topTitle = winner.Key;
            var usedTags = winningTags.ContainsKey(topTitle) ? winningTags[topTitle].ToList() : new List<string>();

            var influencers = moodGameMap[topTitle]
                .OrderByDescending(x => x.YearSeconds)
                .Take(5)
                .Select(x => new MoodInfluencer { 
                    GameName = x.Game.Name, 
                    MatchedTag = x.MatchedTag,
                    PlaytimeSeconds = x.YearSeconds,
                    PlaytimeFormatted = FormatPlaytime(x.YearSeconds)
                })
                .ToList();

            var winnerMood = moods.FirstOrDefault(m => m.Title == topTitle);
            return new MoodResult
            {
                HasData = true,
                PersonaId = winnerMood != null ? winnerMood.Id : "MIX",
                Title = topTitle,
                Subtitle = winnerMood != null ? winnerMood.Subtitle : "",
                TopTags = string.Join(" • ", usedTags.Take(5)),
                TopGameCovers = topCovers,
                TopGameNames = topNames,
                Influencers = influencers,
                TopTagsUsed = usedTags,
                CategoryScores = scores
            };
        }

        public List<MoodInfoStub> GetMoodDictionary()
        {
            return new List<MoodInfoStub>
            {
                new MoodInfoStub { Title = "O HERÓI DE ÉPICOS", Subtitle = "Exploração constante e missões de proporções lendárias em mundos vastos.", IconPath = "EPIC", TriggerList = "RPG, Aventuras longas, Fantasia" },
                new MoodInfoStub { Title = "O AGENTE DAS SOMBRAS", Subtitle = "Preferência por infiltração, táticas e jogabilidade furtiva e metódica.", IconPath = "ROUGE", TriggerList = "Stealth, Assassinato, Espionagem" },
                new MoodInfoStub { Title = "O MESTRE ZEN", Subtitle = "Experiências relaxantes e contemplativas para descompressão.", IconPath = "CHILL", TriggerList = "Simulação, Relaxante, Casual" },
                new MoodInfoStub { Title = "A MÁQUINA DE COMBATE", Subtitle = "Focado em ação intensa, reflexos rápidos e domínio de armas.", IconPath = "SHOOT", TriggerList = "Ação, Shooter, FPS, Bullet Hell" },
                new MoodInfoStub { Title = "O ARQUITETO SOCIAL", Subtitle = "Construção de impérios, gestão de cidades ou mundos virtuais.", IconPath = "ARCHI", TriggerList = "Estratégia, City Builder, Gestão" },
                new MoodInfoStub { Title = "O SOBREVIVENTE DO CAOS", Subtitle = "Atraído pelo medo, tensão constante e gestão de recursos escassos.", IconPath = "HORROR", TriggerList = "Terror, Survival, Psicológico" },
                new MoodInfoStub { Title = "O ALUNO DA ACADEMIA", Subtitle = "Desafios intelectuais, quebra-cabeças e lógica pura.", IconPath = "PUZZLE", TriggerList = "Puzzle, Lógica, Mistério" },
                new MoodInfoStub { Title = "A LENDA DAS PISTAS", Subtitle = "Paixão por velocidade, mecânica e o asfalto.", IconPath = "RACE", TriggerList = "Corrida, Esportes, Simuladores" },
                new MoodInfoStub { Title = "O EXPLORADOR DE SISTEMAS", Subtitle = "Adora mecânicas complexas que se entrelaçam.", IconPath = "METRO", TriggerList = "Metroidvania, Roguelike" },
                new MoodInfoStub { Title = "O OBSERVADOR DE ALMAS", Subtitle = "Prioridade total para a narrativa, personagens e diálogos.", IconPath = "STORY", TriggerList = "Visual Novel, Point & Click, História" },
                new MoodInfoStub { Title = "O NEON-NOMAD", Subtitle = "Jogos com estética cyberpunk, tecnologia e futurismo.", IconPath = "CYBER", TriggerList = "Cyberpunk, Sci-Fi, Distopia" },
                new MoodInfoStub { Title = "O CAMALEÃO DIGITAL", Subtitle = "Uma mistura eclética que desafia qualquer classificação.", IconPath = "MIX", TriggerList = "Vários gêneros misturados" }
            };
        }



        /// <summary>Get playtime distribution by genre (Top N).</summary>
        public List<GenreData> GetGenreBreakdown(List<GameStatsData> yearGames, Dictionary<Guid, List<ActivitySession>> allSessions, int topN = 5)
        {
            var map = new Dictionary<string, long>();

            foreach (var g in yearGames)
            {
                long yearPlaytime = 0;
                List<ActivitySession> sessions;
                if (allSessions.TryGetValue(g.GameId, out sessions) && sessions.Any())
                    yearPlaytime = sessions.Sum(s => s.Seconds);

                foreach (var genre in g.Genres)
                {
                    if (!map.ContainsKey(genre)) map[genre] = 0;
                    map[genre] += yearPlaytime;
                }
            }

            var total = map.Values.Sum();
            if (total == 0) total = 1;

            return map
                .OrderByDescending(kv => kv.Value)
                .Take(topN)
                .Select(kv => new GenreData
                {
                    Name            = kv.Key,
                    PlaytimeSeconds = kv.Value,
                    Ratio           = (double)kv.Value / total
                })
                .ToList();
        }

        /// <summary>Get playtime distribution by platform (Top N).</summary>
        public List<PlatformData> GetPlatformBreakdown(List<GameStatsData> yearGames, Dictionary<Guid, List<ActivitySession>> allSessions, int topN = 5)
        {
            var map = new Dictionary<string, long>();

            foreach (var g in yearGames)
            {
                long yearPlaytime = 0;
                List<ActivitySession> sessions;
                if (allSessions.TryGetValue(g.GameId, out sessions) && sessions.Any())
                    yearPlaytime = sessions.Sum(s => s.Seconds);

                if (yearPlaytime <= 0) continue;

                var platforms = g.Platforms != null && g.Platforms.Any() ? g.Platforms : new List<string> { "PC" };
                foreach (var platform in platforms)
                {
                    // If multiple platforms, split the playtime equally (simple approach)
                    long splitPlaytime = yearPlaytime / platforms.Count;
                    if (!map.ContainsKey(platform)) map[platform] = 0;
                    map[platform] += splitPlaytime;
                }
            }

            var total = map.Values.Sum();
            if (total == 0) total = 1;

            return map
                .OrderByDescending(kv => kv.Value)
                .Take(topN)
                .Select(kv => new PlatformData
                {
                    Name            = kv.Key,
                    PlaytimeSeconds = kv.Value,
                    Ratio           = (double)kv.Value / total
                })
                .ToList();
        }

        /// <summary>Aggregate daily activity in seconds. Key = date, Value = seconds played.</summary>
        public Dictionary<DateTime, long> GetDailyActivity(int year, List<GameStatsData> yearGames, GameActivityService activityService)
        {
            var result = new Dictionary<DateTime, long>();

            foreach (var g in yearGames)
            {
                var sessions = activityService.GetSessionsForYear(g.GameId, year);
                if (sessions.Any())
                {
                    foreach (var s in sessions)
                    {
                        var day = s.Date.Date;
                        if (!result.ContainsKey(day)) result[day] = 0;
                        result[day] += s.Seconds;
                    }
                }
            }

            return result;
        }

        public List<StudioStatsData> GetDeveloperBreakdown(List<GameStatsData> yearGames, Dictionary<Guid, List<ActivitySession>> allSessions, int topCount)
        {
            var stats = new Dictionary<string, StudioAggregator>();
            foreach (var g in yearGames)
            {
                if (g.Developers == null || g.Developers.Count == 0) continue;
                long secs = 0;
                List<ActivitySession> s;
                if (allSessions.TryGetValue(g.GameId, out s)) secs = s.Sum(x => x.Seconds);
                if (secs <= 0) continue;

                foreach (var dev in g.Developers)
                {
                    StudioAggregator agg;
                    if (!stats.TryGetValue(dev, out agg))
                    {
                        agg = new StudioAggregator { Name = dev };
                        stats[dev] = agg;
                    }
                    agg.TotalSeconds += secs;
                    agg.GameCount++;
                    agg.GamePlaytimes.Add(new Tuple<string, long>(g.Name, secs));
                }
            }

            long total = stats.Values.Sum(x => x.TotalSeconds);
            if (total == 0) total = 1;

            var result = new List<StudioStatsData>();
            var ordered = stats.Values.OrderByDescending(x => x.TotalSeconds).Take(topCount);
            foreach (var item in ordered)
            {
                var topGames = item.GamePlaytimes.OrderByDescending(x => x.Item2).Take(3).Select(x => x.Item1).ToList();
                result.Add(new StudioStatsData 
                { 
                    Name = item.Name, 
                    PlaytimeSeconds = item.TotalSeconds, 
                    Ratio = (double)item.TotalSeconds / total,
                    GameCount = item.GameCount,
                    TopGames = topGames
                });
            }
            return result;
        }

        public List<StudioStatsData> GetPublisherBreakdown(List<GameStatsData> yearGames, Dictionary<Guid, List<ActivitySession>> allSessions, int topCount)
        {
            var stats = new Dictionary<string, StudioAggregator>();
            foreach (var g in yearGames)
            {
                if (g.Publishers == null || g.Publishers.Count == 0) continue;
                long secs = 0;
                List<ActivitySession> s;
                if (allSessions.TryGetValue(g.GameId, out s)) secs = s.Sum(x => x.Seconds);
                if (secs <= 0) continue;

                foreach (var pub in g.Publishers)
                {
                    StudioAggregator agg;
                    if (!stats.TryGetValue(pub, out agg))
                    {
                        agg = new StudioAggregator { Name = pub };
                        stats[pub] = agg;
                    }
                    agg.TotalSeconds += secs;
                    agg.GameCount++;
                    agg.GamePlaytimes.Add(new Tuple<string, long>(g.Name, secs));
                }
            }

            long total = stats.Values.Sum(x => x.TotalSeconds);
            if (total == 0) total = 1;

            var result = new List<StudioStatsData>();
            var ordered = stats.Values.OrderByDescending(x => x.TotalSeconds).Take(topCount);
            foreach (var item in ordered)
            {
                var topGames = item.GamePlaytimes.OrderByDescending(x => x.Item2).Take(3).Select(x => x.Item1).ToList();
                result.Add(new StudioStatsData 
                { 
                    Name = item.Name, 
                    PlaytimeSeconds = item.TotalSeconds, 
                    Ratio = (double)item.TotalSeconds / total,
                    GameCount = item.GameCount,
                    TopGames = topGames
                });
            }
            return result;
        }

        /// <summary>Monthly totals array (index 0 = Jan).</summary>
        public long[] GetMonthlyTotals(int year, List<GameStatsData> yearGames, GameActivityService activityService)
        {
            var months = new long[12];
            var daily = GetDailyActivity(year, yearGames, activityService);

            foreach (var kv in daily)
                if (kv.Key.Year == year)
                    months[kv.Key.Month - 1] += kv.Value;

            return months;
        }

        private GameStatsData MapGame(Game g)
        {
            int? releaseYear = null;
            if (g.ReleaseDate.HasValue) releaseYear = g.ReleaseDate.Value.Year;

            List<string> gameTags = new List<string>();
            if (g.Tags != null)
            {
                foreach (var t in g.Tags) gameTags.Add(t.Name);
            }

            List<string> gameGenres = new List<string>();
            if (g.Genres != null)
            {
                foreach (var genre in g.Genres) gameGenres.Add(genre.Name);
            }

            List<string> gamePlatforms = new List<string>();
            if (g.Platforms != null)
            {
                foreach (var p in g.Platforms) gamePlatforms.Add(p.Name);
            }

            string sourceName = null;
            if (g.Source != null) sourceName = g.Source.Name;

            string coverPath = null;
            if (g.CoverImage != null) coverPath = playniteApi.Database.GetFullFilePath(g.CoverImage);

            List<string> gameDevs = new List<string>();
            if (g.Developers != null) { foreach (var dev in g.Developers) gameDevs.Add(dev.Name); }

            List<string> gamePubs = new List<string>();
            if (g.Publishers != null) { foreach (var pub in g.Publishers) gamePubs.Add(pub.Name); }

            GameStatsData data = new GameStatsData();
            data.GameId = g.Id;
            data.Name = g.Name;
            data.TotalPlaytimeSeconds = (long)g.Playtime;
            data.Genres = gameGenres;
            data.Tags = gameTags;
            data.Platforms = gamePlatforms;
            data.Developers = gameDevs;
            data.Publishers = gamePubs;
            data.Source = sourceName;
            data.LastPlayed = g.LastActivity;
            data.FirstPlayed = g.Added;
            data.CoverImagePath = coverPath;
            data.BackgroundImagePath = g.BackgroundImage != null ? playniteApi.Database.GetFullFilePath(g.BackgroundImage) : null;
            data.ReleaseYear = releaseYear;
            data.IsCompleted = g.CompletionStatus != null && (g.CompletionStatus.Name == "Completed" || g.CompletionStatus.Name == "Concluído");

            // Smart Completion Date Detection
            if (data.IsCompleted)
            {
                // 1. Try native CompletionDate (via dynamic for SDK compatibility)
                try
                {
                    dynamic dynGame = g;
                    DateTime? nativeCompDate = dynGame.CompletionDate;
                    if (nativeCompDate.HasValue)
                    {
                        data.CompletionDate = nativeCompDate.Value;
                    }
                }
                catch { }

                // 2. Fallback to SuccessStory (last achievement date)
                if (!data.CompletionDate.HasValue)
                {
                    var ssPathFallback = GetSuccessStoryPath(g.Id);
                    if (ssPathFallback != null && File.Exists(ssPathFallback))
                    {
                        try
                        {
                            var ssJson = File.ReadAllText(ssPathFallback);
                            var ssData = JsonConvert.DeserializeObject<SuccessStoryRoot>(ssJson);
                            if (ssData != null && ssData.Items != null)
                            {
                                var lastAchievement = ssData.Items
                                    .Where(i => i.DateUnlocked.HasValue)
                                    .OrderByDescending(i => i.DateUnlocked.Value)
                                    .FirstOrDefault();
                                
                                if (lastAchievement != null)
                                {
                                    data.CompletionDate = lastAchievement.DateUnlocked;
                                }
                            }
                        }
                        catch { }
                    }
                }

                // 3. Last fallback: LastActivity
                if (!data.CompletionDate.HasValue)
                {
                    data.CompletionDate = g.LastActivity;
                }
            }

            // Achievements details again for data.UnlockedAchievements (keep existing logic)
            // Achievements
            var ssPath = GetSuccessStoryPath(g.Id);
            if (ssPath != null && File.Exists(ssPath))
            {
                try
                {
                    var ssJson = File.ReadAllText(ssPath);
                    var ssData = JsonConvert.DeserializeObject<SuccessStoryRoot>(ssJson);
                    if (ssData != null && ssData.Items != null)
                    {
                        data.TotalAchievements = ssData.Items.Count;
                        data.UnlockedAchievements = ssData.Items.Count(i => i.IsUnlocked == true || i.DateUnlocked.HasValue);
                    }
                }
                catch { }
            }

            return data;
        }

        private Dictionary<int, long> GetHourlyDistribution(Dictionary<Guid, List<ActivitySession>> allSessions)
        {
            var hourly = new Dictionary<int, long>();
            for (int i = 0; i < 24; i++) hourly[i] = 0;

            foreach (var sessions in allSessions.Values)
            {
                foreach (var s in sessions)
                {
                    int hour = s.Date.Hour;
                    hourly[hour] += s.Seconds;
                }
            }
            return hourly;
        }

        private void IdentifyHourlyPersona(Dictionary<int, long> hourly, YearlyStatsData stats)
        {
            var peakHour = hourly.OrderByDescending(kv => kv.Value).FirstOrDefault().Key;
            
            if (peakHour >= 0 && peakHour < 5)
            {
                stats.HourlyPersonaName = "CORUJA DA MADRUGADA";
                stats.HourlyPersonaIcon = "\uE708"; // Moon
                stats.HourlyPersonaDescription = "Você prefere o silêncio da noite para suas maratonas.";
            }
            else if (peakHour >= 5 && peakHour < 12)
            {
                stats.HourlyPersonaName = "PASSARINHO MATUTINO";
                stats.HourlyPersonaIcon = "\uE706"; // Sun
                stats.HourlyPersonaDescription = "Nada como um café e uns jogos para começar o dia.";
            }
            else if (peakHour >= 12 && peakHour < 18)
            {
                stats.HourlyPersonaName = "GAMER DE TURNO";
                stats.HourlyPersonaIcon = "\uE701"; // CloudSun (Cloud is E701)
                stats.HourlyPersonaDescription = "Suas tardes são dedicadas a grandes aventuras.";
            }
            else
            {
                stats.HourlyPersonaName = "GUERREIRO NOTURNO";
                stats.HourlyPersonaIcon = "\uE734"; // Stars/Favorite
                stats.HourlyPersonaDescription = "O dia acaba, mas a jogatina está apenas começando.";
            }
        }

        private int GetLongestStreak(int year, List<GameStatsData> games, GameActivityService activityService)
        {
            var activityDays = new HashSet<DateTime>();
            foreach (var g in games)
            {
                var sessions = activityService.GetSessionsForYear(g.GameId, year);
                foreach (var s in sessions) activityDays.Add(s.Date.Date);
            }

            if (activityDays.Count == 0) return 0;

            var orderedDays = activityDays.OrderBy(d => d).ToList();
            int longest = 0;
            int current = 1;

            for (int i = 0; i < orderedDays.Count - 1; i++)
            {
                if ((orderedDays[i + 1] - orderedDays[i]).TotalDays == 1)
                {
                    current++;
                }
                else
                {
                    if (current > longest) longest = current;
                    current = 1;
                }
            }
            if (current > longest) longest = current;
            return longest;
        }

        private string FormatPlaytime(long seconds)
        {
            long h = seconds / 3600, m = (seconds % 3600) / 60;
            if (h > 0) return string.Format("{0}h {1}m", h, m);
            return string.Format("{0}m", m);
        }
    }

    // ── DTOs ────────────────────────────────────────────

    public class GameStatsData
    {
        public Guid         GameId               { get; set; }
        public string       Name                 { get; set; }
        public long         TotalPlaytimeSeconds { get; set; }
        public List<string> Genres               { get; set; }
        public List<string> Tags                 { get; set; }
        public List<string> Platforms            { get; set; }
        public List<string> Developers           { get; set; }
        public List<string> Publishers           { get; set; }
        public string       Source               { get; set; }
        public DateTime?    LastPlayed           { get; set; }
        public DateTime?    FirstPlayed          { get; set; }
        public string       CoverImagePath       { get; set; }
        public string       BackgroundImagePath  { get; set; }
        public int          TotalAchievements    { get; set; }
        public int          UnlockedAchievements { get; set; }
        public int?         ReleaseYear          { get; set; }
        public bool         IsCompleted          { get; set; }
        public DateTime?    CompletionDate       { get; set; }
    }

    public class PlatformData
    {
        public string Name            { get; set; }
        public long   PlaytimeSeconds { get; set; }
        public double Ratio           { get; set; }
    }

    public class StudioStatsData
    {
        public string Name            { get; set; }
        public long   PlaytimeSeconds { get; set; }
        public int    GameCount       { get; set; }
        public List<string> TopGames  { get; set; }
        public double Ratio           { get; set; }
    }

    internal class StudioAggregator
    {
        public string Name { get; set; }
        public long TotalSeconds { get; set; }
        public int GameCount { get; set; }
        public List<Tuple<string, long>> GamePlaytimes { get; set; }

        public StudioAggregator()
        {
            GamePlaytimes = new List<Tuple<string, long>>();
        }
    }

    public class MoodResult
    {
        public string PersonaId { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string TopTags { get; set; }
        public List<string> TopGameCovers { get; set; }
        public List<string> TopGameNames { get; set; }
        public List<MoodInfluencer> Influencers { get; set; }
        public string MonthName { get; set; }
        public bool HasData { get; set; }
        public List<string> TopTagsUsed { get; set; }
        public Dictionary<string, long> CategoryScores { get; set; }
    }

    public class GenreData
    {
        public string Name            { get; set; }
        public long   PlaytimeSeconds { get; set; }
        public double Ratio           { get; set; }
    }

    public class PlaytimeAgeDistribution
    {
        public double NewReleasesPercent { get; set; }
        public double RecentPercent      { get; set; }
        public double ClassicsPercent    { get; set; }
        public long   NewReleasesSeconds { get; set; }
        public long   RecentSeconds      { get; set; }
        public long   ClassicsSeconds    { get; set; }
    }

    // ── SuccessStory parsing ─────────────────────────────

    internal class SuccessStoryRoot
    {
        public List<SuccessStoryItem> Items { get; set; }
    }

    internal class SuccessStoryItem
    {
        public bool?    IsUnlocked   { get; set; }
        public DateTime? DateUnlocked { get; set; }
    }
}
