using HarmonyLib;
using System.Reflection;
using BeatLeader.Models;
using RestartOnMiss.Harmony.ScoreSaberPatch;

// thank you Meivyn for the help/example :)

namespace RestartOnMiss.ReplayFpfc.ReplayDetection
{
    public class ReplayDetector
    {
        private static bool BeatLeaderReplay;
        private static bool ScoreSaberReplay;
        
        
        private static MethodBase TargetMethod() => AccessTools.Method("ScoreSaber.Core.Daemons.ReplayService:NewPlayStarted");
        
        public static void AddReplayEvents()
        {
            BeatLeader.Replayer.ReplayerLauncher.ReplayWasStartedEvent += SetBLTrue;
            BeatLeader.Replayer.ReplayerLauncher.ReplayWasFinishedEvent += SetBLFalse;
            SSReplayPatches.ReplayWasStartedEvent += SetSSTrue;
            SSReplayPatches.ReplayWasFinishedEvent += SetSSFalse;
        }        
        
        public static void RemoveReplayEvents()
        {
            BeatLeader.Replayer.ReplayerLauncher.ReplayWasStartedEvent -= SetBLTrue;
            BeatLeader.Replayer.ReplayerLauncher.ReplayWasFinishedEvent -= SetBLFalse;
            SSReplayPatches.ReplayWasStartedEvent -= SetSSTrue;
            SSReplayPatches.ReplayWasFinishedEvent -= SetSSFalse;
        }

        private static void SetBLTrue(ReplayLaunchData replayLaunchData)
        {
            BeatLeaderReplay = true;
        }
        
        private static void SetBLFalse(ReplayLaunchData replayLaunchData)
        {
            BeatLeaderReplay = false;
        }

        public static void SetSSTrue()
        {
            ScoreSaberReplay = true;
        }
        
        public static void SetSSFalse()
        {
            ScoreSaberReplay = false;
        }

        
        public static bool IsInReplay()
        {
            return BeatLeaderReplay || ScoreSaberReplay;
        }
    }
}
