using System.Linq;
using BS_Utils.Gameplay;
using UnityEngine;
using BS_Utils.Utilities;
using RestartOnMiss.Configuration;
using RestartOnMiss.ReplayFpfc.FpfcDetection;
using RestartOnMiss.ReplayFpfc.ReplayDetection;

namespace RestartOnMiss.Utils
{
    public partial class StuffUtils
    {
        private static bool _shouldContinue;
        public static void BSUtilsAdd()
        {
            BSEvents.gameSceneLoaded += OnGameSceneLoaded;
            BSEvents.noteWasMissed += OnNoteMissed;
            BSEvents.noteWasCut += OnNoteCut;
        }

        public static void BSUtilsRemove()
        {
            BSEvents.gameSceneLoaded -= OnGameSceneLoaded;
            BSEvents.noteWasMissed -= OnNoteMissed;
            BSEvents.noteWasCut -= OnNoteCut;
        }
        
        #region dumb ahh shiiii
        private static void OnGameSceneLoaded()
        {
            _shouldContinue = UpdateShouldContinue();
            
            Plugin.Log.Debug($"_shouldContinue = {_shouldContinue}");
            
            if (!_shouldContinue)
            {
                return;
            }
            
            Plugin.Log.Debug("Game scene loaded. Attempting to find ILevelRestartController implementer...");
            RestartOnMissController.Instance.OnGameSceneLoaded();
            
            var allBehaviours = Resources.FindObjectsOfTypeAll<MonoBehaviour>();
            // attempt to find the first one that implements ILevelRestartController
            var restartController = allBehaviours.OfType<ILevelRestartController>().FirstOrDefault();

            if (restartController == null)
            {
                Plugin.Log.Warn("No ILevelRestartController  found. Can't restart level.");
            }
            else
            {
                Plugin.Log.Debug("ILevelRestartController was found after game scene loaded.");
                if (RestartOnMissController.Instance != null)
                {
                    RestartOnMissController.Instance.SetILevelRestartController(restartController);
                }
            }
        }

        private static void OnNoteMissed(NoteController noteController)
        {
            if (!_shouldContinue)
            {
                return; 
            }
            
            //Plugin.Log.Debug("note missed");
            if (RestartOnMissController.Instance != null)
            {
                RestartOnMissController.Instance.OnNoteMissed(noteController);
            }
            else
            {
                Plugin.Log.Warn("RestartOnMissController instance not found. Cannot restart level.");
            }
        }
        
        private static void OnNoteCut(NoteController noteController, NoteCutInfo noteCutInfo)
        {
            if (!_shouldContinue)
            {
                return; 
            }
            
            //Plugin.Log.Debug("note cut");
            if (RestartOnMissController.Instance != null)
            {
                RestartOnMissController.Instance.OnNoteCut(noteController, noteCutInfo);
            }
            else
            {
                Plugin.Log.Warn("RestartOnMissController instance not found. Cannot restart level.");
            }
        }

        /// <summary>
        /// I realllyyyy hate doing it this way but for the life of me I could not figure out how to get it to unsubscribe and get rid of the controller so it didn't run ANYTHING
        ///
        /// I'll probs work on this at some point but this solution works, and it really shouldn't use a ton of resources even in fast songs or cases where this mod will even be enabled (ex not enable in vibro...)
        /// its one if statement per note after setting this so whatever
        /// </summary>
        private static bool UpdateShouldContinue() 
        {
            if (!PluginConfig.Instance.Enabled) return false;
            if (ReplayDetector.IsInReplay() && !PluginConfig.Instance.EnableInReplay) return false;
            if (FPFCDetector.FPFCEnabled && !PluginConfig.Instance.EnableInFPFC) return false;
            if (!ValidGamemode()) return false;
            
            return true;
        }

        public static bool ValidGamemode()
        {
            var levelData = BS_Utils.Plugin.LevelData;
            if (levelData == null || !levelData.IsSet)
            {
                Plugin.Log.Warn("LevelData is not set. Assuming standard game mode.");
                return false;
            }
            
            return levelData.Mode != Mode.Multiplayer && levelData.Mode != Mode.Mission;
        }
        
        
        #endregion
    }
}