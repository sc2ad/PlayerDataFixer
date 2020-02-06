using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace PlayerDatFixer
{
    internal sealed class LocalLeaderboardsData
    {
        [JsonProperty("_leaderboardsData")]
        public List<LeaderboardData> LeaderboardsData { get; set; }
    }
    internal sealed class LeaderboardData : IEquatable<LeaderboardData>
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

        public bool Equals([AllowNull] LeaderboardData other)
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
    internal sealed class ScoreData
    {
        [JsonProperty("_score")]
        public int Score { get; set; }
        [JsonProperty("_playerName")]
        public string PlayerName { get; set; }
        [JsonProperty("_fullCombo")]
        public bool FullCombo { get; set; }
        [JsonProperty("_timestamp")]
        public int Timestamp { get; set; }
        internal ScoreData Copy()
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
