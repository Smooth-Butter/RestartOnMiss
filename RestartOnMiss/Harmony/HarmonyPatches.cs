using HarmonyLib;
using UnityEngine;

namespace RestartOnMiss.Harmony
{
    // Patch for AudioTimeSyncController.StopSong
    [HarmonyPatch(typeof(AudioTimeSyncController))]
    [HarmonyPatch("StopSong")]
    public class StopSongPatch
    {
        public static void Postfix(AudioTimeSyncController __instance)
        {
            // Access the RestartOnMissController instance
            RestartOnMissController controller = RestartOnMissController.Instance;

            if (controller == null)
            {
                Plugin.Log.Warn("RestartOnMissController instance not found.");
                return;
            }
        }
    }

    // Patch for NoteController.HandleNoteDidPassMissedMarkerEvent
    //[HarmonyPatch(typeof(NoteController))]
    //[HarmonyPatch("HandleNoteDidPassMissedMarkerEvent")]
    //public class NoteMissedPatch
    //{
    //    // Postfix runs after the original method
    //    public static void Postfix(NoteController __instance)
    //    {
    //      // Access the RestartOnMissController instance
    //        RestartOnMissController controller = RestartOnMissController.Instance;

    //        if (controller == null)
    //        {
    //            Plugin.Log.Warn("RestartOnMissController instance not found.");
    //            return;
    //        }

    //       // Ensure the note is not dissolving
    //        if (__instance.dissolving)
    //            return;

    //        // Trigger the restart logic
    //       controller.OnNoteMissed(__instance);
    //    }
    //}
    
    
}
