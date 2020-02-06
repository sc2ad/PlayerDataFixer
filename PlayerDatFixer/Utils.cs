using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PlayerDatFixer
{
    internal static class Utils
    {
        // Assume LevelId length must be 40 for an unconverted hash (without custom_level_ prefixed to it)
        private const int LevelIdHashLength = 40;
        // 13 is length of custom_level_
        private const int LevelIdStartSubstring = 13;
        // 5 is length of Quest
        private const int QuestStartSubstring = 5;
        public static IEnumerable<string> GetTargetIds(string levelId)
        {
            var lst = new HashSet<string>();
            if (levelId.Length == LevelIdHashLength)
            {
                if (Regex.IsMatch(levelId, @"^[a-fA-F0-9]+$"))
                {
                    lst.Add("custom_level_" + levelId);
                }
            }
            if (levelId.StartsWith("custom_level_"))
            {
                // Remove any other extraneous characters, if they exist
                var temp = levelId.Substring(LevelIdStartSubstring, LevelIdHashLength);
                if (Regex.IsMatch(temp, @"^[a-fA-F0-9]+$"))
                {
                    lst.Add(temp);
                }
            }
            if (levelId.StartsWith("Quest"))
            {
                // Remove the Quest prefix
                var temp = levelId.Substring(QuestStartSubstring, LevelIdHashLength);
                if (Regex.IsMatch(temp, @"^[a-fA-F0-9]+$"))
                {
                    lst.Add(temp);
                    lst.Add("custom_level_" + temp);
                }
            }
            if (levelId.Length > LevelIdHashLength)
            {
                // Lets check to see if it has a HASH at the beginning, and an underscore after it
                var temp = levelId.Substring(0, LevelIdHashLength);
                if (Regex.IsMatch(temp, @"^[a-fA-F0-9]+$"))
                {
                    lst.Add(temp);
                    lst.Add("custom_level_" + temp);
                }
            }
#if DEBUG
            Console.WriteLine($"Getting IDs for: {levelId}");
            Console.WriteLine("Adding Items:");
            foreach (var item in lst)
            {
                Console.WriteLine(item);
            }
#endif
            return lst;
        }
        public static IEnumerable<string> GetTargetLeaderboardIds(string leaderboardId)
        {
            var lst = new HashSet<string>();
            if (!leaderboardId.StartsWith("Quest"))
            {
                return lst;
            }
            var targetId = leaderboardId.Substring(QuestStartSubstring);
            if (targetId.Length < LevelIdHashLength)
            {
                return lst;
            }
            if (targetId.StartsWith("custom_level_"))
            {
                var temp = targetId.Substring(LevelIdStartSubstring, LevelIdHashLength);
                if (temp.Length < LevelIdHashLength + LevelIdStartSubstring)
                {
                    return lst;
                }
                var difficulty = targetId.Substring(temp.Length);
                if (Regex.IsMatch(temp, @"^[a-fA-F0-9]+$"))
                {
                    lst.Add("Quest" + temp + difficulty);
                }
            }
            else
            {
                var temp = targetId.Substring(0, LevelIdHashLength);
                if (temp.Length < LevelIdHashLength)
                {
                    return lst;
                }
                var difficulty = targetId.Substring(temp.Length);
                if (Regex.IsMatch(temp, @"^[a-fA-F0-9]+$"))
                {
                    lst.Add("Questcustom_level_" + temp + difficulty);
                }
            }
#if DEBUG
            Console.WriteLine($"Getting IDs for: {leaderboardId}");
            Console.WriteLine("Adding Items:");
            foreach (var item in lst)
            {
                Console.WriteLine(item);
            }
#endif
            return lst;
        }
    }
}
