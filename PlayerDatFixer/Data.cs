using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PlayerDatFixer
{
    internal sealed class Data
    {
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("localPlayers")]
        public List<Player> LocalPlayers { get; set; }
        [JsonProperty("guestPlayers")]
        public List<Player> GuestPlayers { get; set; }
    }
    internal sealed class Player
    {
        [JsonProperty("playerId")]
        public string PlayerId { get; set; }
        [JsonProperty("playerName")]
        public string PlayerName { get; set; }
        [JsonProperty("shouldShowTutorialPrompt")]
        public bool ShouldShowTutorialPrompt { get; set; }
        [JsonProperty("shouldShow360Warning")]
        public bool ShouldShow360Warning { get; set; }
        [JsonProperty("agreedToEula")]
        public bool AgreedToEula { get; set; }
        [JsonProperty("lastSelectedBeatmapDifficulty")]
        public int LastSelectedBeatmapDifficulty { get; set; }
        [JsonProperty("lastSelectedBeatmapCharacteristicName")]
        public string LastSelectedBeatmapCharacteristicName { get; set; }
        [JsonProperty("gameplayModifiers")]
        public GameplayModifiers GameplayModifiers { get; set; }
        [JsonProperty("playerSpecificSettings")]
        public PlayerSpecificSettings PlayerSpecificSettings { get; set; }
        [JsonProperty("practiceSettings")]
        public PracticeSettings PracticeSettings { get; set; }
        [JsonProperty("playerAllOverallStatsData")]
        public PlayerAllOverallStatsData PlayerAllOverallStatsData { get; set; }
        [JsonProperty("levelsStatsData")]
        public List<Stats> LevelsStatsData { get; set; }
        [JsonProperty("missionsStatsData")]
        public List<MissionStats> MissionsStatsData { get; set; }
        [JsonProperty("showedMissionHelpIds")]
        public List<string> ShowedMissionHelpIds { get; set; }
        [JsonProperty("colorSchemesSettings")]
        public ColorSchemeSettings ColorSchemeSettings { get; set; }
        [JsonProperty("overrideEnvironmentSettings")]
        public OverrideEnvironmentSettings OverrideEnvironmentSettings { get; set; }
        [JsonProperty("favoritesLevelIds")]
        public List<string> FavoriteLevelIds { get; set; }
    }
    internal sealed class GameplayModifiers
    {
        [JsonProperty("energyType")]
        public int EnergyType { get; set; }
        [JsonProperty("noFail")]
        public bool NoFail { get; set; }
        [JsonProperty("instaFail")]
        public bool InstaFail { get; set; }
        [JsonProperty("failOnSaberClash")]
        public bool FailOnSaberClash { get; set; }
        [JsonProperty("enabledObstacleType")]
        public int EnabledObstacleType { get; set; }
        [JsonProperty("fastNotes")]
        public bool FastNotes { get; set; }
        [JsonProperty("strictAngles")]
        public bool StrictAngles { get; set; }
        [JsonProperty("disappearingArrows")]
        public bool DisappearingArrows { get; set; }
        [JsonProperty("ghostNotes")]
        public bool GhostNotes { get; set; }
        [JsonProperty("noBombs")]
        public bool NoBombs { get; set; }
        [JsonProperty("songSpeed")]
        public int SongSpeed { get; set; }
    }
    internal sealed class PlayerSpecificSettings
    {
        [JsonProperty("staticLights")]
        public bool StaticLights { get; set; }
        [JsonProperty("leftHanded")]
        public bool LeftHanded { get; set; }
        [JsonProperty("playerHeight")]
        public double PlayerHeight { get; set; }
        [JsonProperty("automaticPlayerHeight")]
        public bool AutomaticPlayerHeight { get; set; }
        [JsonProperty("sfxVolume")]
        public double SfxVolume { get; set; }
        [JsonProperty("reduceDebris")]
        public bool ReduceDebris { get; set; }
        [JsonProperty("noTextsAndHuds")]
        public bool NoTextsAndHuds { get; set; }
        [JsonProperty("advancedHud")]
        public bool AdvancedHud { get; set; }
    }
    internal sealed class PracticeSettings
    {
        [JsonProperty("startSongTime")]
        public double StartSongTime { get; set; }
        [JsonProperty("songSpeedMul")]
        public double SongSpeedMul { get; set; }
    }
    internal sealed class PlayerAllOverallStatsData
    {
        [JsonProperty("campaignOverallStatsData")]
        public OverallStatsData CampaignOverallStatsData { get; set; }
        [JsonProperty("soloFreePlayOverallStatsData")]
        public OverallStatsData SoloFreePlayOverallStatsData { get; set; }
        [JsonProperty("partyFreePlayOverallStatsData")]
        public OverallStatsData PartyFreePlayOverallStatsData { get; set; }
    }
    internal sealed class OverallStatsData
    {
        [JsonProperty("goodCutsCount")]
        public int GoodCutsCount { get; set; }
        [JsonProperty("badCutsCount")]
        public int BadCutsCount { get; set; }
        [JsonProperty("missedCutsCount")]
        public int MissedCutsCount { get; set; }
        [JsonProperty("totalScore")]
        public int TotalScore { get; set; }
        [JsonProperty("playedLevelsCount")]
        public int PlayedLevelsCount { get; set; }
        // Yes, this typo actually exists
        [JsonProperty("cleardLevelsCount")]
        public int ClearedLevelsCount { get; set; }
        [JsonProperty("failedLevelsCount")]
        public int FailedLevelsCount { get; set; }
        [JsonProperty("fullComboCount")]
        public int FullComboCount { get; set; }
        [JsonProperty("timePlayed")]
        public double TimePlayed { get; set; }
        [JsonProperty("handDistanceTravelled")]
        public int HandDistanceTravelled { get; set; }
        [JsonProperty("cummulativeCutScoreWithoutMultiplier")]
        public int CummulativeCutScoreWithoutMultiplier { get; set; }
    }
    internal sealed class Stats
    {
        [JsonProperty("levelId")]
        public string LevelId { get; set; }
        [JsonProperty("difficulty")]
        public int Difficulty { get; set; }
        [JsonProperty("beatmapCharacteristicName")]
        public string BeatmapCharacteristicName { get; set; }
        [JsonProperty("highScore")]
        public int HighScore { get; set; }
        [JsonProperty("maxCombo")]
        public int MaxCombo { get; set; }
        [JsonProperty("fullCombo")]
        public bool FullCombo { get; set; }
        [JsonProperty("maxRank")]
        public int MaxRank { get; set; }
        [JsonProperty("validScore")]
        public bool ValidScore { get; set; }
        [JsonProperty("playCount")]
        public int PlayCount { get; set; }

        // Assume LevelId length must be 40 for an unconverted hash (without custom_level_ prefixed to it)
        private const int LevelIdHashLength = 40;
        // 13 is length of custom_level_ + 1
        private const int LevelIdStartSubstring = 13;
        // 5 is length of Quest + 1
        private const int QuestStartSubstring = 5;
        private string GetTargetId(string levelId)
        {
            if (levelId.Length == LevelIdHashLength)
            {
                return "custom_level_" + levelId;
            }
            else if (levelId.StartsWith("custom_level_"))
            {
                // Remove any other extraneous characters, if they exist
                return levelId.Substring(LevelIdStartSubstring, LevelIdHashLength);
            }
            else if (levelId.StartsWith("Quest"))
            {
                // Remove the Quest prefix
                return GetTargetId(levelId.Substring(QuestStartSubstring, LevelIdHashLength));
            }
            else if (levelId.Length > LevelIdHashLength)
            {
                // Lets check to see if it has a HASH at the beginning, and random stuff after it
                var targetId = levelId.Substring(0, LevelIdHashLength);
                if (!Regex.IsMatch(targetId, @"^[a-fA-F0-9]+$"))
                {
                    return null;
                }
                return targetId;
            }
            return null;
        }
        public Stats Copy()
        {
            string targetId = GetTargetId(LevelId);
            if (targetId == null)
            {
                return null;
            }
            return new Stats
            {
                LevelId = targetId,
                Difficulty = Difficulty,
                BeatmapCharacteristicName = BeatmapCharacteristicName,
                HighScore = HighScore,
                MaxCombo = MaxCombo,
                FullCombo = FullCombo,
                MaxRank = MaxRank,
                ValidScore = ValidScore,
                PlayCount = PlayCount
            };
        }
    }
    internal sealed class MissionStats
    {
        [JsonProperty("missionId")]
        public string MissionId { get; set; }
        [JsonProperty("cleared")]
        public bool Cleared { get; set; }
    }
    internal sealed class ColorSchemeSettings
    {
        [JsonProperty("overrideDefaultColors")]
        public bool OverrideDefaultColors { get; set; }
        [JsonProperty("selectedColorSchemeId")]
        public string SelectedColorSchemeId { get; set; }
        [JsonProperty("colorSchemes")]
        public List<ColorScheme> ColorSchemes { get; set; }
    }
    internal sealed class ColorScheme
    {
        [JsonProperty("colorSchemeId")]
        public string ColorSchemeId { get; set; }
        [JsonProperty("saberAColor")]
        public Color SaberAColor { get; set; }
        [JsonProperty("saberBColor")]
        public Color SaberBColor { get; set; }
        [JsonProperty("environmentColor0")]
        public Color EnvironmentColor0 { get; set; }
        [JsonProperty("environmentColor1")]
        public Color EnvironmentColor1 { get; set; }
        [JsonProperty("obstaclesColor")]
        public Color ObstaclesColor { get; set; }
    }
    internal sealed class Color
    {
        [JsonProperty("r")]
        public double R { get; set; }
        [JsonProperty("g")]
        public double G { get; set; }
        [JsonProperty("b")]
        public double B { get; set; }
        [JsonProperty("a")]
        public double A { get; set; }
    }
    internal sealed class OverrideEnvironmentSettings
    {
        [JsonProperty("overrideEnvironments")]
        public bool OverrideEnvironments { get; set; }
        [JsonProperty("overrideNormalEnvironmentName")]
        public string OverrideNormalEnvironmentName { get; set; }
        [JsonProperty("override360EnvironmentName")]
        public string Override360EnvironmentName { get; set; }
    }
}
