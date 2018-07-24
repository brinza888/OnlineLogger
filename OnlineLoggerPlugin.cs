using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;

namespace OnlineLogger
{
    public class OnlineLoggerPlugin : RocketPlugin
    {
        public static OnlineLoggerPlugin Instance;
        private Dictionary<CSteamID, DateTime> loggingUsers = new Dictionary<CSteamID, DateTime>();
        private readonly string filePath = System.IO.Directory.GetCurrentDirectory() + @"/Plugins/OnlineLogger/Log.txt";

        protected override void Load()
        {
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
            Instance = this;
            Logger.Log("OnlineLogger loaded!");
            Logger.Log("Created by Brinza Bezrukoff");
            Logger.Log("Vk: vk.com/brinza888");
            Logger.Log("Mail: bezrukoff888@gmail.com");
            U.Events.OnPlayerConnected += OnPlayerConnected;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnected;
        }

        private void OnPlayerConnected(UnturnedPlayer player)
        {
            if (!IRocketPlayerExtension.HasPermission(player, "OnlineLogger.Log"))
                return;
            File.AppendAllText(filePath, $"[{DateTime.Now}] Connected {player.CharacterName} ({player.CSteamID})" + System.Environment.NewLine);
            loggingUsers.Add(player.CSteamID, DateTime.Now);
        }

        private void OnPlayerDisconnected(UnturnedPlayer player)
        {
            if (!loggingUsers.ContainsKey(player.CSteamID))
                return;
            TimeSpan timeSpan = DateTime.Now - loggingUsers[player.CSteamID];
            File.AppendAllText(filePath, $"[{DateTime.Now}] Disconnected {player.CharacterName} ({player.CSteamID}) Online: {timeSpan.Hours}h {timeSpan.Minutes}m" + System.Environment.NewLine);
            loggingUsers.Remove(player.CSteamID);
        }

        protected override void Unload()
        {
            Logger.Log("OnlineLogger unloaded!", ConsoleColor.White);
            U.Events.OnPlayerConnected -= OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= OnPlayerDisconnected;
        }
    }
}