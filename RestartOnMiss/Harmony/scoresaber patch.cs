using HarmonyLib;
using System;
using System.Reflection;
using RestartOnMiss.Stuff;


namespace RestartOnMiss.Harmony.ScoreSaberPatch
{
    public static class SSReplayPatches
    {
        // Events for replay start and end
        public static event Action ReplayWasStartedEvent;
        public static event Action ReplayWasFinishedEvent;

        public static void ApplyPatches(HarmonyLib.Harmony harmony)
        {
            if (!ModCheck.IsScoreSaberInstalled)
            {
                return;
            }
            
            try
            {
                harmony.PatchAll(typeof(SSReplayPatches).Assembly);
            }
            catch (Exception ex)
            {
                Plugin.Log.Error($"Failed to apply ScoreSaber patches: {ex.Message}");
            }
        }

        [HarmonyPatch]
        public class ReplayStartPatch
        {
            private static MethodBase TargetMethod()
            {
                return AccessTools.Method("ScoreSaber.Core.ReplaySystem.ReplayLoader:StartReplay");
            }

            private static void Postfix()
            {
                Plugin.Log.Info("ScoreSaber replay has started.");
                ReplayWasStartedEvent?.Invoke();
            }
        }

        [HarmonyPatch]
        public class ReplayEndPatch
        {
            private static MethodBase TargetMethod()
            {
                return AccessTools.Method("ScoreSaber.Core.ReplaySystem.ReplayLoader:ReplayEnd");
            }

            private static void Postfix()
            {
                Plugin.Log.Info("ScoreSaber replay has ended.");
                ReplayWasFinishedEvent?.Invoke();
            }
        }

        public static void Initialize()
        {
            ReplayWasStartedEvent += () => Plugin.Log.Info("ReplayWasStartedEvent fired.");
            ReplayWasFinishedEvent += () => Plugin.Log.Info("ReplayWasFinishedEvent fired.");
        }
    }
}