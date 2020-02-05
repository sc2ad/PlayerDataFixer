using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlayerDatFixer
{
    class Program
    {
        static readonly Version Version = new Version(0, 1, 0);
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
            Console.WriteLine("================================================================");
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
            var playerDatLocation = args[0];
            if (!File.Exists(playerDatLocation) && Path.GetFileName(playerDatLocation).Contains("PlayerData.dat"))
            {
                Error("Please specify a 'PlayerData.dat' file that exists, or drag it onto this application!");
                Close();
            }
            else
            {
                Info("Reading PlayerData.dat...");
                var text = File.ReadAllText(playerDatLocation);
                Info("Creating backup...");
                try
                {
                    File.WriteAllText(Path.Combine(Path.GetDirectoryName(playerDatLocation), "PlayerData_backup.dat"), text);
                }
                catch (Exception e)
                {
                    Error($"Failed to create backup! {e}");
                    Close();
                    return;
                }
                Info("Parsing PlayerData.dat...");
                try
                {
                    var data = JsonConvert.DeserializeObject<Data>(text);
                    if (!SupportedVersions.Contains(data.Version))
                    {
                        Error($"Invalid Version: {data.Version} is not supported!");
                        Close();
                        return;
                    }
                    Info("Copying data...");
                    int copiedCount = 0;
                    var allPlayers = new List<Player>(data.LocalPlayers.Union(data.GuestPlayers));
                    foreach (var p in allPlayers)
                    {
                        int statCount = p.LevelsStatsData.Count;
                        for (int i = 0; i < statCount; i++)
                        {
                            var clone = p.LevelsStatsData[i].Copy();
                            if (clone != null)
                            {
                                // Ensure the clone doesn't already exist within the LevelsStatsData
                                // Big slowdown here
                                if (p.LevelsStatsData.FirstOrDefault((s) =>
                                {
                                    return clone.LevelId == s.LevelId && clone.Difficulty == s.Difficulty && clone.BeatmapCharacteristicName == s.BeatmapCharacteristicName;
                                }) == null)
                                {
                                    copiedCount++;
                                    p.LevelsStatsData.Add(clone);
                                }
                            }
                        }
                    }
                    Info($"Successfully copied {copiedCount} stats for songs!");
                    Info("Attempting to serialize data...");
                    text = JsonConvert.SerializeObject(data);
                    Info($"Attempting to write serialized data to PlayerData.dat file {playerDatLocation}...");
                    File.WriteAllText(playerDatLocation, text);
                    Info("Completed Succesfully!");
                    Close();
                }
                catch (JsonException e)
                {
                    Error($"Error while parsing/serializing PlayerData.dat: {e}");
                    Close();
                }
                catch (Exception e)
                {
                    Error($"An unknown exception has occurred! Please report this to Sc2ad#8836!");
                    Error(e.ToString());
                    Close();
                }
            }
        }
    }
}
