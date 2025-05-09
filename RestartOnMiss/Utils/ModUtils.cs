﻿using IPA.Loader;
using System.Linq;

namespace RestartOnMiss.Utils
{
    public static class ModCheck
    {
        // Properties for checking mod installation status
        public static bool IsBeatLeaderInstalled { get; private set; }
        public static bool IsScoreSaberInstalled { get; private set; }

        // Method to initialize and log mod installation status
        public static void Initialize()
        {
            IsBeatLeaderInstalled = PluginManager.EnabledPlugins.Any(x => x.Id == "BeatLeader");
            IsScoreSaberInstalled = PluginManager.EnabledPlugins.Any(x => x.Id == "ScoreSaber");
            
            LogInstalledMods();
        }

        public static void LogInstalledMods()
        {
            Plugin.Log.Debug($"BeatLeader installed: {IsBeatLeaderInstalled}");
            Plugin.Log.Debug($"ScoreSaber installed: {IsScoreSaberInstalled}");
        }
    }
}