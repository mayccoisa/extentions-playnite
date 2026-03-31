using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Playnite.SDK;

namespace RetrospectivaAnual.Services
{
    public class GameActivityService
    {
        private readonly IPlayniteAPI playniteApi;
        private readonly string dataPath;

        public GameActivityService(IPlayniteAPI api)
        {
            this.playniteApi = api;
            this.dataPath = FindGameActivityPath();
        }

        private string FindGameActivityPath()
        {
            var extensionsData = playniteApi.Paths.ExtensionsDataPath;
            
            if (!Directory.Exists(extensionsData)) return null;

            foreach (var dir in Directory.GetDirectories(extensionsData))
            {
                var activityDir = Path.Combine(dir, "GameActivity");
                if (Directory.Exists(activityDir))
                {
                    return activityDir;
                }
            }
            return null;
        }

        public List<ActivitySession> GetSessionsForYear(Guid gameId, int year)
        {
            if (dataPath == null) return new List<ActivitySession>();

            var filePath = Path.Combine(dataPath, string.Format("{0}.json", gameId));
            if (!File.Exists(filePath)) return new List<ActivitySession>();

            try
            {
                var json = File.ReadAllText(filePath);
                var root = JsonConvert.DeserializeObject<GameActivityRoot>(json);
                
                return root.Items
                    .Where(i => i.DateSession.Year == year)
                    .Select(i => new ActivitySession
                    {
                        Date = i.DateSession,
                        Seconds = i.ElapsedSeconds
                    })
                    .ToList();
            }
            catch (Exception)
            {
                return new List<ActivitySession>();
            }
        }

        public Dictionary<Guid, List<ActivitySession>> GetSessionsForYear(int year)
        {
            var result = new Dictionary<Guid, List<ActivitySession>>();
            if (dataPath == null) return result;

            foreach (var file in Directory.GetFiles(dataPath, "*.json"))
            {
                try
                {
                    var gameIdStr = Path.GetFileNameWithoutExtension(file);
                    Guid gameId;
                    if (Guid.TryParse(gameIdStr, out gameId))
                    {
                        var sessions = GetSessionsForYear(gameId, year);
                        if (sessions.Any()) result[gameId] = sessions;
                    }
                }
                catch { }
            }
            return result;
        }
    }

    public class ActivitySession
    {
        public DateTime Date { get; set; }
        public long Seconds { get; set; }
    }

    internal class GameActivityRoot
    {
        public List<GameActivityItem> Items { get; set; }
    }

    internal class GameActivityItem
    {
        public DateTime DateSession { get; set; }
        public long ElapsedSeconds { get; set; }
    }
}
