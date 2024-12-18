using System;
using BS_Utils.Gameplay;
using UnityEngine;
using BS_Utils.Utilities;
using RestartOnMiss.Configuration;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;

namespace RestartOnMiss
{
    public class RestartOnMissController : MonoBehaviour
    {
        private int maxMisses => PluginConfig.Instance.maxMisses;
        private int missCount = 0;
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

        public void OnNoteMissed(NoteController noteController)
        {
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
                Plugin.Log.Info("Level is restarting");
                RestartLevel();
            }
        }

        private void RestartLevel()
        {
            if (_restartController != null)
            {
                Plugin.Log.Debug("Calling ILevelRestartController.RestartLevel()");
                Plugin.Log.Info($"restarted with {missCount} misses");
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
            if (instance == this)
                instance = null;
        }
    }
}
