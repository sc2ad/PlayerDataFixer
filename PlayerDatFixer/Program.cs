using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlayerDatFixer
{
    class Program
    {
        static readonly Version Version = new Version(0, 2, 0);
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
            if (!File.Exists(playerDatLocation))
            {
                Error("Please specify a 'PlayerData.dat' file that exists, or drag it onto this application!");
                Close();
            }
            else
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
                Data data;
                try
                {
                    data = JsonConvert.DeserializeObject<Data>(text);
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
                var allPlayers = new List<Player>(data.LocalPlayers.Union(data.GuestPlayers));
                try
                {
                    foreach (var p in allPlayers)
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
                                } else
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
                }
                Info($"Successfully copied {copiedCount} stats for songs!");
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
        }
    }
}
