using System;
using System.Linq;
using System.Reflection;
using IPA;
using HarmonyLib;
using IPALogger = IPA.Logging.Logger;
using UnityEngine;
using BS_Utils.Gameplay;
using BS_Utils.Utilities;
using BS_Utils.Utilities.Events;

namespace RestartOnMiss
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        private HarmonyLib.Harmony _harmony;
        
        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public void Init(IPALogger logger)
        {
            Instance = this;
            Log = logger;
            Log.Info("RestartOnMiss initialized.");
        }

        #region BSIPA Config

        //Uncomment to use BSIPA's config
        /*
        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
        }
        */

        #endregion
        
        [OnStart]
        public void OnApplicationStart()
        {
            // initialize Harmony
            _harmony = new HarmonyLib.Harmony("com.yourname.restartonmiss");
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            Log.Debug("RestartOnMiss: Harmony patches applied.");

            // start RestartOnMissController if it doesn't exist
            if (RestartOnMissController.Instance == null)
            {
                GameObject controllerObj = new GameObject("RestartOnMissController");
                controllerObj.AddComponent<RestartOnMissController>();
                GameObject.DontDestroyOnLoad(controllerObj);
                Log.Debug("RestartOnMissController instantiated and set to DontDestroyOnLoad.");
            }
        }

        [OnEnable]
        public void OnEnable()
        {
            Log.Debug("Plugin enabled, subscribing to BSEvents");
            BSEvents.gameSceneLoaded += OnGameSceneLoaded;
            BSEvents.noteWasMissed += OnNoteMissedBSUtils;
        }

        [OnDisable]
        public void OnDisable()
        {
            Log.Debug("Plugin disabled, unsubscribing from BSEvents");
            BSEvents.gameSceneLoaded -= OnGameSceneLoaded;
            BSEvents.noteWasMissed -= OnNoteMissedBSUtils;
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            if (_harmony != null)
            {
                _harmony.UnpatchSelf();
                Log.Debug("RestartOnMiss: Harmony patches removed.");
            }
        }
        private void OnGameSceneLoaded()
        {
            Log.Debug("Game scene loaded. Attempting to find ILevelRestartController implementer...");
            RestartOnMissController.Instance.OnGameSceneLoaded();
            
            var allBehaviours = Resources.FindObjectsOfTypeAll<MonoBehaviour>();
            // attempt to find the first one that implements ILevelRestartController
            var restartController = allBehaviours.OfType<ILevelRestartController>().FirstOrDefault();

            if (restartController == null)
            {
                Log.Warn("No ILevelRestartController implementer found. Cannot restart level.");
            }
            else
            {
                Log.Debug("ILevelRestartController implementer found after game scene loaded.");
                if (RestartOnMissController.Instance != null)
                {
                    RestartOnMissController.Instance.SetILevelRestartController(restartController);
                }
            }
        }

        private void OnNoteMissedBSUtils(NoteController noteController)
        {
            Log.Debug("note missed");
            if (RestartOnMissController.Instance != null)
            {
                RestartOnMissController.Instance.OnNoteMissed(noteController);
            }
            else
            {
                Log.Warn("RestartOnMissController instance not found. Cannot restart level.");
            }
        }
        
    }
    
}