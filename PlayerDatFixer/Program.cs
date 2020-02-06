using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlayerDatFixer
{
    class Program
    {
        static readonly Version Version = new Version(0, 2, 2);
        static readonly List<string> SupportedVersions = new List<string> { "2.0.5" };
        static void Close()
        {
            HorizontalLine();
            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
        }
        static void Error(string message)
        {
            Console.WriteLine("[ERROR] " + message);
        }
        static void Info(string message)
        {
            Console.WriteLine("[INFO] " + message);
        }
        static void HorizontalLine()
        {
            Console.WriteLine("==============================================================================");
        }
        static void ParsePlayerData(string playerDatLocation)
        {
            Info("Reading PlayerData.dat...");
            string text;
            try
            {
                text = File.ReadAllText(playerDatLocation);
            }
            catch (Exception e)
            {
                Error($"Failed to read PlayerData.dat from location: {playerDatLocation}\n{e}");
                Close();
                return;
            }
            Info("Creating backup...");
            try
            {
                var backupLocation = Path.Combine(Path.GetDirectoryName(playerDatLocation), "PlayerData_backup.dat");
                if (playerDatLocation == backupLocation)
                {
                    backupLocation = Path.Combine(Path.GetDirectoryName(playerDatLocation), "PlayerData_backup_2.dat");
                    Info($"Remapping backup to: {backupLocation}");
                }
                File.WriteAllText(backupLocation, text);
            }
            catch (Exception e)
            {
                Error($"Failed to create backup!\n{e}");
                Close();
                return;
            }
            Info("Parsing PlayerData.dat...");
            PlayerData data;
            try
            {
                data = JsonConvert.DeserializeObject<PlayerData>(text);
            }
            catch (JsonException e)
            {
                Error($"Error while deserializing PlayerData.dat!\n{e}");
                Close();
                return;
            }
            if (!SupportedVersions.Contains(data.Version))
            {
                Error($"Invalid Version: {data.Version} is not supported!");
                Close();
                return;
            }
            Info("Copying data...");
            int copiedCount = 0;
            try
            {
                foreach (var p in data.LocalPlayers)
                {
                    int statCount = p.LevelsStatsData.Count;
                    for (int i = 0; i < statCount; i++)
                    {
                        var clones = p.LevelsStatsData[i].Copy();
                        foreach (var s in clones)
                        {
                            // Ensure the clone doesn't already exist within the LevelsStatsData
                            if (!p.LevelsStatsData.Contains(s))
                            {
                                copiedCount++;
                                p.LevelsStatsData.Add(s);
                            }
                            else
                            {
#if DEBUG
                                    Info($"Already contains: {s}");
#endif
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Error($"An unknown exception has occurred! Please report this to Sc2ad#8836!\n{e}");
                Close();
                return;
            }
            Info($"Successfully copied {copiedCount} stats for songs!");
            Info("Attempting to copy favorite songs...");
            copiedCount = 0;
            try
            {
                foreach (var p in data.LocalPlayers)
                {
                    int statCount = p.FavoriteLevelIds.Count;
                    for (int i = 0; i < statCount; i++)
                    {
                        var clones = Utils.GetTargetIds(p.FavoriteLevelIds[i]);
                        foreach (var s in clones)
                        {
                            // Ensure the clone doesn't already exist within the LevelsStatsData
                            if (!p.FavoriteLevelIds.Contains(s))
                            {
                                copiedCount++;
                                p.FavoriteLevelIds.Add(s);
                            }
                            else
                            {
#if DEBUG
                                    Info($"Already contains: {s}");
#endif
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Error($"An unknown exception has occurred! Please report this to Sc2ad#8836!\n{e}");
                Close();
                return;
            }
            Info($"Successfully copied {copiedCount} favorite songs!");
            Info("Attempting to serialize data...");
            try
            {
                text = JsonConvert.SerializeObject(data);
            }
            catch (JsonException e)
            {
                Error($"Error while serializing PlayerData.dat!\n{e}");
                Close();
                return;
            }
            Info($"Attempting to write serialized data to PlayerData.dat file {playerDatLocation}...");
            try
            {
                File.WriteAllText(playerDatLocation, text);
            }
            catch (Exception e)
            {
                Error($"Error while writing new PlayerData.dat to: {playerDatLocation}\n{e}");
                Close();
                return;
            }
            Info("Completed Succesfully!");
            Close();
        }
        static void ParseLocalLeaderboards(string localLeaderboardLocation)
        {
            Info("Reading LocalLeaderboards.dat...");
            string text;
            try
            {
                text = File.ReadAllText(localLeaderboardLocation);
            }
            catch (Exception e)
            {
                Error($"Failed to read LocalLeaderboards.dat from location: {localLeaderboardLocation}\n{e}");
                Close();
                return;
            }
            Info("Creating backup...");
            try
            {
                var backupLocation = Path.Combine(Path.GetDirectoryName(localLeaderboardLocation), "LocalLeaderboards_backup.dat");
                if (localLeaderboardLocation == backupLocation)
                {
                    backupLocation = Path.Combine(Path.GetDirectoryName(localLeaderboardLocation), "LocalLeaderboards_backup_2.dat");
                    Info($"Remapping backup to: {backupLocation}");
                }
                File.WriteAllText(backupLocation, text);
            }
            catch (Exception e)
            {
                Error($"Failed to create backup!\n{e}");
                Close();
                return;
            }
            Info("Parsing LocalLeaderboards.dat...");
            LocalLeaderboardsData data;
            try
            {
                data = JsonConvert.DeserializeObject<LocalLeaderboardsData>(text);
            }
            catch (JsonException e)
            {
                Error($"Error while deserializing LocalLeaderboards.dat!\n{e}");
                Close();
                return;
            }
            Info("Copying data...");
            int copiedCount = 0;
            try
            {
                int statCount = data.LeaderboardsData.Count;
                for (int i = 0; i < statCount; i++)
                {
                    var clones = data.LeaderboardsData[i].Copy();
                    foreach (var s in clones)
                    {
                        if (!data.LeaderboardsData.Contains(s))
                        {
                            copiedCount++;
                            data.LeaderboardsData.Add(s);
                        }
                        else
                        {
#if DEBUG
                            Info($"Already contains: {s}");
#endif
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Error($"An unknown exception has occurred! Please report this to Sc2ad#8836!\n{e}");
                Close();
                return;
            }
            Info($"Successfully copied {copiedCount} local leaderboards!");
            Info("Attempting to serialize data...");
            try
            {
                text = JsonConvert.SerializeObject(data);
            }
            catch (JsonException e)
            {
                Error($"Error while serializing LocalLeaderboards.dat!\n{e}");
                Close();
                return;
            }
            Info($"Attempting to write serialized data to LocalLeaderboards.dat file {localLeaderboardLocation}...");
            try
            {
                File.WriteAllText(localLeaderboardLocation, text);
            }
            catch (Exception e)
            {
                Error($"Error while writing new LocalLeaderboards.dat to: {localLeaderboardLocation}\n{e}");
                Close();
                return;
            }
            Info("Completed Succesfully!");
            Close();
        }
        static void Main(string[] args)
        {
            HorizontalLine();
            Info($"VERSION: {Version}");
            Info("Made by Sc2ad");
            Info("Github repo: https://github.com/sc2ad/PlayerDataFixer");
            HorizontalLine();
            if (args.Length == 0)
            {
                Error("Please drag your PlayerData.dat file onto this application!");
                Close();
                return;
            }
            var fileLocation = args[0];
            if (!File.Exists(fileLocation))
            {
                Error("Please specify a 'PlayerData.dat' or 'LocalLeaderboards.dat' file that exists, or drag it onto this application!");
                Close();
            }
            else
            {
                if (fileLocation.Contains("PlayerData.dat"))
                    ParsePlayerData(fileLocation);
                else if (fileLocation.Contains("LocalLeaderboards.dat"))
                    ParseLocalLeaderboards(fileLocation);
            }
        }
    }
}
