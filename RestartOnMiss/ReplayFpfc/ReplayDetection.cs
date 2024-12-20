using HarmonyLib;
using IPA.Utilities;
using System.Reflection;
using UnityEngine;
using BeatLeader;
using BeatLeader.Models;


// thank you Meivyn for the help/example

namespace RestartOnMiss.ReplayFpfc.ReplayDetection
{
    public class ReplayDetector
    {
        private static bool BeatLeaderReplay;
        
        
        private static MethodBase TargetMethod() => AccessTools.Method("Scoresaber.Core.Daemons.ReplayService:NewPlayStarted");
        
        //private static bool ScoreSaberReplay()
        //{
            //return ScoreSaber.Plugin.ReplayState != null && ScoreSaber.Plugin.ReplayState.IsPlaybackEnabled
        //}

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
            Plugin.Log.Info("Replay set true");
        }
        
        private static void SetBLFalse(ReplayLaunchData replayLaunchData)
        {
            BeatLeaderReplay = false;
            Plugin.Log.Info("Replay set false");
        }


        
        public static bool IsInReplay()
        {
            return BeatLeaderReplay ;//|| ScoreSaberReplay()
        }

    }
}
