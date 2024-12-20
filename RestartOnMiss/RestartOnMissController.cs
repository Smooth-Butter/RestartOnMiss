using UnityEngine;
using RestartOnMiss.Configuration;
using RestartOnMiss.ReplayFpfc.FpfcDetection;
using RestartOnMiss.ReplayFpfc.ReplayDetection;

namespace RestartOnMiss
{
    public class RestartOnMissController : MonoBehaviour
    {
        private int MaxMisses => PluginConfig.Instance.MaxMisses;
        private int MissCount = 0;
        private string LastEvent = "";
        public static RestartOnMissController Instance { get; private set; }
        public static NoteController NoteController;
        public static bool IsMultiplayer;
        
        private bool _isRestarting = false;
        private ILevelRestartController _restartController;
        
        private void Awake()
        {
            // only one instance
            if (Instance != null)
            {
                Plugin.Log?.Warn($"Instance of {GetType().Name} already exists, destroying.");
                GameObject.DestroyImmediate(this);
                return;
            }

            GameObject.DontDestroyOnLoad(this); // Don't destroy 
            Instance = this;
            Plugin.Log?.Debug($"{name}: Awake()");
        }

        public void OnNoteCut(NoteController noteController, NoteCutInfo noteCutInfo)
        {
            if ((ReplayDetector.IsInReplay() && !PluginConfig.Instance.EnableInReplay) || (FPFCDetector.FPFCEnabled && !PluginConfig.Instance.EnableInFPFC))
            {
                Plugin.Log.Debug("RestartOnMiss is disabled IN REPLAY or FPFC. Not restarting on on bomb/badcut.");
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
        
        public void OnNoteMissed(NoteController noteController) // this is so so soooo bad - will maybe fix at some point
        {
            if (IsMultiplayer)
            {
                return;
            }
            if (noteController.noteData.colorType == ColorType.None && noteController.noteData.gameplayType != NoteData.GameplayType.BurstSliderElement)
            {
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
            if ((ReplayDetector.IsInReplay() && !PluginConfig.Instance.EnableInReplay) || (FPFCDetector.FPFCEnabled && !PluginConfig.Instance.EnableInFPFC))
            {
                Plugin.Log.Info("RestartOnMiss is disabled IN REPLAY or FPFC. Not restarting on on note miss.");
                return;
            }
            
            ++MissCount;
            Plugin.Log.Info($"Note missed!! current miss count is {MissCount}");
            LastEvent = "Note Missed!";
            
            //_isRestarting || 
            CompareMissMaxMiss();
            

        }

        private void HandleBadCut(NoteController noteController, NoteCutInfo noteCutInfo)
        {
            if (PluginConfig.Instance.CountBadCuts)
            {
                if (!noteCutInfo.allIsOK && noteController.noteData.colorType != ColorType.None)
                {
                    ++MissCount;
                    LastEvent = "Bad cut!!!";
                    Plugin.Log.Info($"Bad Cut!! current miss count is {MissCount}");
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
                    ++MissCount;
                    LastEvent = "Hit Bomb!!!";
                    Plugin.Log.Info($"Hit Bomb!! current miss count is {MissCount}");
                    CompareMissMaxMiss();
                }
            }
        }
        private void CompareMissMaxMiss()
        {
            if (MissCount >= MaxMisses)
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
                Plugin.Log.Info($"restarted with {MissCount} misses due to {LastEvent}");
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
            MissCount = 0;
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
            if (Instance == this)
                Instance = null;
        }
    }
}
