using HarmonyLib;
using UnityEngine;

namespace RestartOnMiss.Harmony
{
    //[HarmonyPatch(typeof(AudioTimeSyncController))]
    //[HarmonyPatch("StopSong")]
    //public class StopSongPatch
    //{
        //public static void Postfix(AudioTimeSyncController __instance)
        //{
            
            //RestartOnMissController controller = RestartOnMissController.instance;

            //if (controller == null)
            //{
                //Plugin.Log.Warn("RestartOnMissController instance not found.");
                //return;
            //}
        //}
    //}
    
    //[HarmonyPatch(typeof(NoteController))]
    //[HarmonyPatch("HandleNoteDidPassMissedMarkerEvent")]
    //public class NoteMissedPatch
    //{
    //    //Postfix runs after the original method
    //    public static void Postfix(NoteController __instance)
    //    {
    //      
    //        RestartOnMissController controller = RestartOnMissController.Instance;

    //        if (controller == null)
    //        {
    //            Plugin.Log.Warn("RestartOnMissController instance not found.");
    //            return;
    //        }
    
    //       controller.OnNoteMissed(__instance);
    //    }
    //}
    
    
}
