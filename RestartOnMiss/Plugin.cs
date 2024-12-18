using System;
using IPALogger = IPA.Logging.Logger;
using System.Linq;
using System.Reflection;
using BeatSaberMarkupLanguage.Settings;
using IPA;
using IPA.Config.Stores;
using UnityEngine;
using RestartOnMiss.Views;
using BS_Utils.Utilities;
using RestartOnMiss.Configuration;
using RestartOnMiss.Installers;
using Config = IPA.Config.Config;
using SiraUtil.Zenject;

namespace RestartOnMiss
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public const string HarmonyId = "com.github.SmoothButter.RestartOnMiss";
        internal static HarmonyLib.Harmony harmony = new HarmonyLib.Harmony(HarmonyId);
        
        internal static Plugin instance { get; private set; }
        internal static IPALogger Log { get; private set; }
        
        
        
        internal static RestartOnMissController PluginController { get { return RestartOnMissController.instance; } }
        
        
        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public Plugin(IPALogger logger, Config conf)
        {
            instance = this;
            Log = logger;
            Log.Info("RestartOnMiss initialized.");
            
            PluginConfig.Instance = conf.Generated<PluginConfig>();
            Log.Debug("Config loaded");

            //zenjector.Install(Location.StandardPlayer, Container => Container.BindInterfacesTo<ModUI>().AsSingle());
            zenjector.Install<MenuInstaller>(Location.Menu);
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
            // start RestartOnMissController if it doesn't exist
            if (RestartOnMissController.instance == null)
            {
                Log.Debug("RestartOnMissController instantiated");
            }
        }

        [OnEnable]
        public void OnEnable()
        {
            Log.Debug("Plugin enabled, subscribing to BSEvents");
            BSEvents.gameSceneLoaded += OnGameSceneLoaded;
            BSEvents.noteWasMissed += OnNoteMissedBSUtils;
            BeatSaberMarkupLanguage.Util.MainMenuAwaiter.MainMenuInitializing += OnMainMenuInit;
            
            if (RestartOnMissController.instance == null)
            {
                new GameObject("RestartOnMissController").AddComponent<RestartOnMissController>();
                Log.Debug("RestartOnMissController instantiated");
            }
            
            ApplyHarmonyPatches();
        }

        public void OnMainMenuInit()
        {
            BSMLSettings.Instance.AddSettingsMenu("RestartOnMiss", "RestartOnMiss.Views.Settings.bsml", new SettingsUI());
            Log.Debug("RestartOnMiss: BSML settings menu registered.");
        }

        
        [OnDisable]
        public void OnDisable()
        {
            Log.Debug("Plugin disabled, unsubscribing from BSEvents");
            BSEvents.gameSceneLoaded -= OnGameSceneLoaded;
            BSEvents.noteWasMissed -= OnNoteMissedBSUtils;
            BeatSaberMarkupLanguage.Util.MainMenuAwaiter.MainMenuInitializing -= OnMainMenuInit;
            
            BSMLSettings.Instance.RemoveSettingsMenu(PluginConfig.Instance);
            
            if (PluginController != null)
                GameObject.Destroy(PluginController);
            
            if (harmony != null)
                RemoveHarmonyPatches();
        }
        
        #region dumb ahh shiiii
        private void OnGameSceneLoaded()
        {
            Log.Debug("Game scene loaded. Attempting to find ILevelRestartController implementer...");
            RestartOnMissController.instance.OnGameSceneLoaded();
            
            var allBehaviours = Resources.FindObjectsOfTypeAll<MonoBehaviour>();
            // attempt to find the first one that implements ILevelRestartController
            var restartController = allBehaviours.OfType<ILevelRestartController>().FirstOrDefault();

            if (restartController == null)
            {
                Log.Warn("No ILevelRestartController  found. Can't restart level.");
            }
            else
            {
                Log.Debug("ILevelRestartController was found after game scene loaded.");
                if (RestartOnMissController.instance != null)
                {
                    RestartOnMissController.instance.SetILevelRestartController(restartController);
                }
            }
        }

        private void OnNoteMissedBSUtils(NoteController noteController)
        {
            Log.Debug("note missed");
            if (RestartOnMissController.instance != null)
            {
                RestartOnMissController.instance.OnNoteMissed(noteController);
            }
            else
            {
                Log.Warn("RestartOnMissController instance not found. Cannot restart level.");
            }
        }
        
        #endregion
        
        #region Harmony
        public static void ApplyHarmonyPatches()
        {

            Log.Debug("Applying Harmony patches.");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            
        }

        public static void RemoveHarmonyPatches()
        {
            try
            {
                // Removes all patches with this HarmonyId
                //harmony.UnpatchAll(HarmonyId);
                harmony.UnpatchSelf();
            }
            catch (Exception ex)
            {
                Logger.log.Critical("Error removing Harmony patches: ");
                Logger.log.Debug(ex);
            }
        }
        #endregion
        
    }
    
}