using HarmonyLib;
using System.Reflection;
using BeatLeader.Models;
using RestartOnMiss.Harmony.ScoreSaberPatch;
using RestartOnMiss.Utils;

// thank you Meivyn for the help/example :)

namespace RestartOnMiss.ReplayFpfc.ReplayDetection
{
    public partial class ReplayDetector // yes I know this is dumb but I want it seperated for navigation and not in different files
    {
        public static bool BeatLeaderReplay;
        public static bool ScoreSaberReplay;
        
        public static void AddReplayEvents()
        {
            if (ModCheck.IsBeatLeaderInstalled)
            {
                AddBLEvents();
            }

            if (ModCheck.IsScoreSaberInstalled)
            {
                AddSSEvents();
            }

        }        
        
        public static void RemoveReplayEvents()
        {
            if (ModCheck.IsBeatLeaderInstalled)
            {
                RemoveBLEvents();
            }

            if (ModCheck.IsScoreSaberInstalled)
            {
                RemoveSSEvents();
            }
        }
        
        public static bool IsInReplay()
        {
            return BeatLeaderReplay || ScoreSaberReplay;
        }
    }

    public partial class ReplayDetector
    {
        public static void AddBLEvents()
        {
            BeatLeader.Replayer.ReplayerLauncher.ReplayWasStartedEvent += SetBLTrue;
            BeatLeader.Replayer.ReplayerLauncher.ReplayWasFinishedEvent += SetBLFalse;
        }

        public static void RemoveBLEvents()
        {
            BeatLeader.Replayer.ReplayerLauncher.ReplayWasStartedEvent -= SetBLTrue;
            BeatLeader.Replayer.ReplayerLauncher.ReplayWasFinishedEvent -= SetBLFalse;
        }
        
        private static void SetBLTrue(ReplayLaunchData replayLaunchData)
        {
            BeatLeaderReplay = true;
        }
        
        private static void SetBLFalse(ReplayLaunchData replayLaunchData)
        {
            BeatLeaderReplay = false;
        }
    }

    public partial class ReplayDetector
    {
        private static MethodBase TargetMethod() => AccessTools.Method("ScoreSaber.Core.Daemons.ReplayService:NewPlayStarted");

        public static void AddSSEvents()
        {
            SSReplayPatches.ReplayWasStartedEvent += SetSSTrue;
            SSReplayPatches.ReplayWasFinishedEvent += SetSSFalse;
        }

        public static void RemoveSSEvents()
        {
            SSReplayPatches.ReplayWasStartedEvent -= SetSSTrue;
            SSReplayPatches.ReplayWasFinishedEvent -= SetSSFalse;
        }
        
        public static void SetSSTrue()
        {
            ScoreSaberReplay = true;
        }
        
        public static void SetSSFalse()
        {
            ScoreSaberReplay = false;
        }
    }
}
