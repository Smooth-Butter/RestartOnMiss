using System.Linq;
using UnityEngine;
using BS_Utils.Utilities;


namespace RestartOnMiss.Utils
{
    public partial class StuffUtils
    {
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
            Plugin.Log.Debug("note missed");
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
            Plugin.Log.Debug("note cut");
            if (RestartOnMissController.Instance != null)
            {
                RestartOnMissController.Instance.OnNoteCut(noteController, noteCutInfo);
            }
            else
            {
                Plugin.Log.Warn("RestartOnMissController instance not found. Cannot restart level.");
            }
        }
        
        #endregion
    }
}