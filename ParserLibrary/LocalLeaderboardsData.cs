using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ParserLibrary
{
    public sealed class LocalLeaderboardsCopyStats : CopyStats
    {
        public int CopiedLeaderboards { get; internal set; }
        public void DisplayStats(Action<string> logCall)
        {
            logCall($"Copied: {CopiedLeaderboards} leaderboards!");
        }
    }
    public sealed class LocalLeaderboardsData : Copyable
    {
        [JsonProperty("_leaderboardsData")]
        public List<LeaderboardData> LeaderboardsData { get; set; }
        public static string FileName { get => "LocalLeaderboards"; }
        public CopyStats CopyData()
        {
            var copyStats = new LocalLeaderboardsCopyStats();
            int statCount = LeaderboardsData.Count;
            for (int i = 0; i < statCount; i++)
            {
                var clones = LeaderboardsData[i].Copy();
                foreach (var s in clones)
                {
                    if (!LeaderboardsData.Contains(s))
                    {
                        copyStats.CopiedLeaderboards++;
                        LeaderboardsData.Add(s);
                    }
                    else
                    {
#if DEBUG
                        Console.WriteLine($"Already contains leaderboard: {s}");
#endif
                    }
                }
            }
            return copyStats;
        }
    }
    public sealed class LeaderboardData : IEquatable<LeaderboardData>
    {
        [JsonProperty("_leaderboardId")]
        public string LeaderboardId { get; set; }
        [JsonProperty("_scores")]
        public List<ScoreData> Scores { get; set; }
        private LeaderboardData CopySingle(string newId)
        {
            var scores = new List<ScoreData>();
            foreach (var s in Scores)
            {
                scores.Add(s.Copy());
            }
            return new LeaderboardData
            {
                LeaderboardId = newId,
                Scores = scores
            };
        }
        public IEnumerable<LeaderboardData> Copy()
        {
            var data = new HashSet<LeaderboardData>();
            foreach (var s in Utils.GetTargetLeaderboardIds(LeaderboardId))
            {
                data.Add(CopySingle(s));
            }
            return data;
        }

        public bool Equals(LeaderboardData other)
        {
            return other.LeaderboardId == LeaderboardId;
        }
        public override int GetHashCode()
        {
            return LeaderboardId.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj is LeaderboardData)
                return Equals(obj as LeaderboardData);
            return false;
        }
        public override string ToString()
        {
            return LeaderboardId;
        }
    }
    public sealed class ScoreData
    {
        [JsonProperty("_score")]
        public int Score { get; set; }
        [JsonProperty("_playerName")]
        public string PlayerName { get; set; }
        [JsonProperty("_fullCombo")]
        public bool FullCombo { get; set; }
        [JsonProperty("_timestamp")]
        public int Timestamp { get; set; }
        public ScoreData Copy()
        {
            return new ScoreData
            {
                Score = Score,
                PlayerName = PlayerName,
                FullCombo = FullCombo,
                Timestamp = Timestamp
            };
        }
    }
}
