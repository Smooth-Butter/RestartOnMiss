using System;
using IPALogger = IPA.Logging.Logger;
using IPA;
using IPA.Config.Stores;
using UnityEngine;
using RestartOnMiss.Configuration;
using RestartOnMiss.Harmony.ScoreSaberPatch;
using RestartOnMiss.Installers;
using RestartOnMiss.ReplayFpfc.FpfcDetection;
using RestartOnMiss.ReplayFpfc.ReplayDetection;
using Config = IPA.Config.Config;
using SiraUtil.Zenject;
using RestartOnMiss.Stuff;

namespace RestartOnMiss
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public const string HarmonyId = "com.github.SmoothButter.RestartOnMiss";
        internal static HarmonyLib.Harmony harmony = new HarmonyLib.Harmony(HarmonyId);
        
        internal static Plugin instance { get; private set; }
        internal static IPALogger Log { get; private set; }
        private HarmonyLib.Harmony _harmony;
        
        
        
        internal static RestartOnMissController PluginController { get { return RestartOnMissController.instance; } }
        
        
        [Init]
        public Plugin(IPALogger logger, Config conf, Zenjector zenjector)
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
            StuffUtils.BSUtilsAdd();
            StuffUtils.OnMainMenuInit();
            ModCheck.Initialize();
            ReplayDetector.AddReplayEvents();
            FPFCDetector.Initialize();
            
            if (RestartOnMissController.instance == null)
            {
                new GameObject("RestartOnMissController").AddComponent<RestartOnMissController>();
                Log.Debug("RestartOnMissController instantiated");
            }
            
            ApplyHarmonyPatches();//temp for debug
        }

        
        [OnDisable]
        public void OnDisable()
        {
            StuffUtils.BSUtilsRemove();
            ReplayDetector.RemoveReplayEvents();
            
            if (PluginController != null)
                GameObject.Destroy(PluginController);
            
            if (harmony != null)
                RemoveHarmonyPatches();//temp for debug
        }
        

        
        #region Harmony
        public void ApplyHarmonyPatches()
        {
            _harmony = new HarmonyLib.Harmony("com.github.SmoothButter.RestartOnMiss");
            SSReplayPatches.ApplyPatches(_harmony);
            Log.Debug("Applying Harmony patches.");
            
        }

        public void RemoveHarmonyPatches()
        {
            try
            {
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