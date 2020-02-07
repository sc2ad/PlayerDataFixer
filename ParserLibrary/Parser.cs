using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ParserLibrary
{
    public class Parser
    {
        private static readonly List<string> SupportedVersions = new List<string> { "2.0.5" };
        private static void PerformCopy<T>(string location, Action<string> infoLog, Action<string> onError) where T : Copyable
        {
            infoLog("Reading data...");
            string text;
            try
            {
                text = File.ReadAllText(location);
            }
            catch (Exception e)
            {
                onError($"Failed to read data from location: {location}\n{e}");
                return;
            }
            infoLog("Creating backup...");
            try
            {
                var backupLocation = Path.Combine(Path.GetDirectoryName(location), Path.GetFileNameWithoutExtension(location) + "_backup.dat");
                if (location == backupLocation)
                {
                    backupLocation = Path.Combine(Path.GetDirectoryName(location), Path.GetFileNameWithoutExtension(location) + "_backup_2.dat");
                    infoLog($"Remapping backup to: {backupLocation}");
                }
                File.WriteAllText(backupLocation, text);
            }
            catch (Exception e)
            {
                onError($"Failed to create backup!\n{e}");
                return;
            }
            infoLog("Parsing data...");
            T data;
            try
            {
                data = JsonConvert.DeserializeObject<T>(text);
            }
            catch (JsonException e)
            {
                onError($"Error while deserializing data!\n{e}");
                return;
            }
            if (data is PlayerData)
            {
                var pd = data as PlayerData;
                if (!SupportedVersions.Contains(pd.Version))
                {
                    onError($"Invalid Version: {pd.Version} is not supported!");
                    return;
                }
            }
            infoLog("Copying data...");
            try
            {
                var stats = data.CopyData();
                stats.DisplayStats(infoLog);
            }
            catch (Exception e)
            {
                onError($"An unknown exception has occurred! Please report this to Sc2ad#8836!\n{e}");
                return;
            }
            infoLog("Attempting to serialize data...");
            try
            {
                text = JsonConvert.SerializeObject(data);
            }
            catch (JsonException e)
            {
                onError($"Error while serializing PlayerData.dat!\n{e}");
                return;
            }
            infoLog($"Attempting to write serialized data to file: {location}...");
            try
            {
                File.WriteAllText(location, text);
            }
            catch (Exception e)
            {
                onError($"Error while writing new PlayerData.dat to: {location}\n{e}");
                return;
            }
            infoLog("Completed Succesfully!");
        }
        public static void PerformCopy(string location, Action<string> infoLog, Action<string> onError)
        {
            if (Directory.Exists(location))
            {
                foreach (var p in Directory.GetFiles(location))
                {
                    if (p.Contains(PlayerData.FileName) && !p.Contains("_backup"))
                        PerformCopy<PlayerData>(Path.Combine(location, p), infoLog, onError);
                    else if (p.Contains(LocalLeaderboardsData.FileName) && !p.Contains("_backup"))
                        PerformCopy<LocalLeaderboardsData>(Path.Combine(location, p), infoLog, onError);
                }                    
            }
            else if (File.Exists(location))
            {
                if (location.Contains(PlayerData.FileName))
                    PerformCopy<PlayerData>(location, infoLog, onError);
                else if (location.Contains(LocalLeaderboardsData.FileName))
                    PerformCopy<LocalLeaderboardsData>(location, infoLog, onError);
            }
            else
            {
                onError("Please specify a file that exists, or drag it onto this application!");
            }
        }
    }
}
