using UnityEngine;
using RestartOnMiss.Configuration;
using RestartOnMiss.ReplayFpfc.ReplayDetection;

namespace RestartOnMiss
{
    public class RestartOnMissController : MonoBehaviour
    {
        private int maxMisses => PluginConfig.Instance.maxMisses;
        private int missCount = 0;
        private string lastEvent = "";
        public static RestartOnMissController instance { get; private set; }
        public static NoteController NoteController;
        public static bool IsMultiplayer;
        
        private bool _isRestarting = false;  //maybe I'll use... maybe not so it gets to live for now
        private ILevelRestartController _restartController;
        
        private void Awake()
        {
            // Ensure only one instance
            if (instance != null)
            {
                Plugin.Log?.Warn($"Instance of {GetType().Name} already exists, destroying.");
                GameObject.DestroyImmediate(this);
                return;
            }

            GameObject.DontDestroyOnLoad(this); // Don't destroy on scene changes
            instance = this;
            Plugin.Log?.Debug($"{name}: Awake()");
        }

        public void OnNoteCut(NoteController noteController, NoteCutInfo noteCutInfo)
        {
            if (ReplayDetector.IsInReplay() && !PluginConfig.Instance.EnableInReplay)
            {
                return;
            }
            
            if (!PluginConfig.Instance.Enabled)
            {
                Plugin.Log.Debug("RestartOnMiss is disabled. Not restarting on on bomb/badcut.");
                return; 
            }
            
            HandleBadCut(noteController, noteCutInfo);
                
            HandleBombHit(noteController, noteCutInfo);
            
        } 
        
        public void OnNoteMissed(NoteController noteController)
        {
            if (ReplayDetector.IsInReplay() && !PluginConfig.Instance.EnableInReplay)
            {
                Plugin.Log.Info("RestartOnMiss is disabled IN REPLAY. Not restarting on note miss.");
                return;
            }
            if (!PluginConfig.Instance.Enabled)
            {
                Plugin.Log.Debug("RestartOnMiss is disabled. Not restarting on note miss.");
                return; 
            }
            if (_isRestarting)
            {
                Plugin.Log.Debug("level is currently restarting");
                return;
            }
            if (IsMultiplayer)
            {
                return;
            }
            if (noteController.noteData.colorType == ColorType.None && noteController.noteData.gameplayType != NoteData.GameplayType.BurstSliderElement)
            {
                return;
            }
            
            ++missCount;
            Plugin.Log.Info($"Note missed!! current miss count is {missCount}");
            lastEvent = "Note Missed!";
            
            //_isRestarting || 
            CompareMissMaxMiss();
            

        }

        private void HandleBadCut(NoteController noteController, NoteCutInfo noteCutInfo)
        {
            if (PluginConfig.Instance.CountBadCuts)
            {
                if (!noteCutInfo.allIsOK && noteController.noteData.colorType != ColorType.None)
                {
                    ++missCount;
                    lastEvent = "Bad cut!!!";
                    Plugin.Log.Info($"Bad Cut!! current miss count is {missCount}");
                    CompareMissMaxMiss();
                }
            }
        }

        private void HandleBombHit(NoteController noteController, NoteCutInfo noteCutInfo)
        {
            if (PluginConfig.Instance.CountBombs)
            {
                if (noteController.noteData.colorType == ColorType.None && noteController.noteData.gameplayType == NoteData.GameplayType.Bomb)
                {
                    ++missCount;
                    lastEvent = "Hit Bomb!!!";
                    Plugin.Log.Info($"Hit Bomb!! current miss count is {missCount}");
                    CompareMissMaxMiss();
                }
            }
        }
        private void CompareMissMaxMiss()
        {
            if (missCount >= maxMisses)
            {
                _isRestarting = true;
                Plugin.Log.Info("Level is restarting");
                RestartLevel();
            }
        }

            
        private void RestartLevel()
        {
            if (_restartController != null)
            {
                Plugin.Log.Debug("Calling ILevelRestartController.RestartLevel()");
                Plugin.Log.Info($"restarted with {missCount} misses due to {lastEvent}");
                _restartController.RestartLevel();
            }
            else
            {
                Plugin.Log.Warn("ILevelRestartController not available. Cannot restart level.");
            }
        }
        
        public void OnGameSceneLoaded() //this is so dumb will fix later... maybe
        {
            // Plugin.Log.Warn("level started?");
            ResetCount();
            _isRestarting = false;
        }

        private void ResetCount()
        {
            missCount = 0;
            Plugin.Log.Info("reset count");
        }
        
        public void SetILevelRestartController(ILevelRestartController controller)
        {
            _restartController = controller;
            if (_restartController != null)
            {
                Plugin.Log.Debug("ILevelRestartController set successfully.");
            }
        }
        
        private void OnDestroy()
        {
            Plugin.Log?.Debug($"{name}: OnDestroy()");
            if (instance == this)
                instance = null;
        }
    }
}
