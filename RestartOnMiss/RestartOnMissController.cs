using System;
using BS_Utils.Gameplay;
using UnityEngine;
using BS_Utils.Utilities;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;

namespace RestartOnMiss
{
    public class RestartOnMissController : MonoBehaviour
    {
        public int maxMisses = 5;
        public int missCount = 0;
        public static RestartOnMissController Instance { get; private set; }
        public static NoteController NoteController;
        public static bool IsMultiplayer;
        
        private bool _isRestarting = false;  //maybe I'll use... maybe not so it gets to live for now
        private ILevelRestartController _restartController;
        
        private void Awake()
        {
            // Ensure only one instance
            if (Instance != null)
            {
                Plugin.Log?.Warn($"Instance of {GetType().Name} already exists, destroying.");
                GameObject.DestroyImmediate(this);
                return;
            }

            GameObject.DontDestroyOnLoad(this); // Don't destroy on scene changes
            Instance = this;
            Plugin.Log?.Debug($"{name}: Awake()");
        }

        public void OnNoteMissed(NoteController noteController)
        {
            if (_isRestarting)
            {
                Plugin.Log.Debug("level is currently restarting");
                return;
            }
            
            ++missCount;
            UnityEngine.Debug.Log($"current miss count is {missCount}");
            
            //_isRestarting || 
            if (IsMultiplayer)
            {
                return;
            }
            
            if (missCount >= maxMisses)
            {
                _isRestarting = true;
                Plugin.Log.Debug($"Note missed in {noteController.name}. Restarting level...");
                RestartLevel();
            }
        }

        private void RestartLevel()
        {
            if (_restartController != null)
            {
                Plugin.Log.Debug("Calling ILevelRestartController.RestartLevel()");
                _restartController.RestartLevel();
            }
            else
            {
                Plugin.Log.Warn("ILevelRestartController not available. Cannot restart level.");
            }
        }
        
        public void OnGameSceneLoaded() //this is so dumb will fix later... maybe
        {
            Plugin.Log.Warn("level started?");
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
            if (Instance == this)
                Instance = null;
        }
    }
}
