using System;

namespace RestartOnMiss.ReplayFpfc.FpfcDetection
{
    public static class FPFCDetector
    {
        public static bool FPFCEnabled;
        
        public static void Initialize()
        {
            string[] args = Environment.GetCommandLineArgs();
            FPFCEnabled = Array.Exists(args, arg => arg.Equals("fpfc", StringComparison.OrdinalIgnoreCase));
            Plugin.Log.Info($"FPFC launch argument detected: {FPFCEnabled}");
        }
    }
}