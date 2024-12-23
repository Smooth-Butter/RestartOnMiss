using System.Linq;
using System.Reflection;
//using System.Threading;
using UnityEngine;
using RestartOnMiss.Configuration;

namespace RestartOnMiss
{
    public class RestartOnMissController : MonoBehaviour
    {
        private int MaxMisses => PluginConfig.Instance.MaxMisses;
        private int MissCount = 0;
        private string _lastEvent = "";
        public static RestartOnMissController Instance { get; private set; }
        public static NoteController NoteController;
        
        private bool _isRestarting = false;
        private ILevelRestartController _restartController;
        
        private void Awake()
        {
            // only one instance please
            if (Instance != null)
            {
                Plugin.Log?.Warn($"Instance of {GetType().Name} already exists, destroying.");
                GameObject.DestroyImmediate(this);
                return;
            }

            GameObject.DontDestroyOnLoad(this);
            Instance = this;
            Plugin.Log?.Debug($"{name}: Awake()");
        }

        public void OnNoteCut(NoteController noteController, NoteCutInfo noteCutInfo)
        {
            if (_isRestarting)
            {
                return;
            }
            
            HandleBadCut(noteController, noteCutInfo);
                
            HandleBombHit(noteController, noteCutInfo);
            
        } 
        
        public void OnNoteMissed(NoteController noteController)
        {
            if (noteController.noteData.colorType == ColorType.None && noteController.noteData.gameplayType != NoteData.GameplayType.BurstSliderElement)
            {
                return;
            }
            
            if (_isRestarting)
            {
                Plugin.Log.Debug("level is currently restarting");
                return;
            }
            
            ++MissCount;
            Plugin.Log.Info($"Note missed!! current miss count is {MissCount}");
            _lastEvent = "Note Missed!";
            
            CompareMissMaxMiss();
        }

        private void HandleBadCut(NoteController noteController, NoteCutInfo noteCutInfo)
        {
            if (PluginConfig.Instance.CountBadCuts)
            {
                if (!noteCutInfo.allIsOK && noteController.noteData.colorType != ColorType.None)
                {
                    ++MissCount;
                    _lastEvent = "Bad cut!!!";
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
                    _lastEvent = "Hit Bomb!!!";
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
                if (Utils.ModCheck.IsBeatLeaderInstalled)
                {
                    BeatLeaderSpecificRestart();
                    return;
                }
                Plugin.Log.Debug("Calling ILevelRestartController.RestartLevel()");
                Plugin.Log.Info($"restarted with {MissCount} misses due to {_lastEvent}");
                _restartController.RestartLevel();
            }
            else
            {
                Plugin.Log.Warn("ILevelRestartController not available. Cannot restart level.");
            }
        }

        private void
            BeatLeaderSpecificRestart() //I hate this soooooo much - there's probs another way but this is what worked first
        {
            var pauseController = Resources.FindObjectsOfTypeAll<PauseController>().LastOrDefault();
            if (pauseController != null)
            {
                var restartMethod = typeof(PauseController).GetMethod(
                    "HandlePauseMenuManagerDidPressRestartButton",
                    BindingFlags.NonPublic | BindingFlags.Instance);

                if (restartMethod != null)
                {
                    Plugin.Log.Debug("BeatLeader PauseController restart stuff");
                    Plugin.Log.Info($"Restart triggered with {MissCount} misses due to {_lastEvent}");
                    //Thread.Sleep(2000);
                    restartMethod.Invoke(pauseController, null);
                }
                else
                {
                    Plugin.Log.Warn("Restart method not found on PauseController - direct restart.");
                }
            }
            else
            {
                Plugin.Log.Warn("PauseController not found - direct restart");
            }
        }
        
        public void OnGameSceneLoaded()
        {
            // Plugin.Log.Debug("level started?");
            ResetCount();
            _isRestarting = false;
            _lastEvent = "Restarted";
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
