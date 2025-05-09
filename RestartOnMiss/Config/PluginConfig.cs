﻿using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace RestartOnMiss.Configuration
{
    internal class PluginConfig
    {
        public static PluginConfig Instance { get; set; }
        
        internal virtual bool Enabled { get; set; } = false;
        internal virtual bool CountBadCuts { get; set; } = false;
        internal virtual bool CountBombs { get; set; } = false;
        internal virtual bool EnableInReplay { get; set; } = false;
        internal virtual bool EnableInFPFC { get; set; } = false;
        internal virtual int MaxMisses { get; set; } = 0;
        
        
        
        
        /// <summary>
        /// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
        /// </summary>
        public virtual void OnReload()
        {
            // Do stuff after config is read from disk.
        }

        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        public virtual void Changed()
        {
            // Do stuff when the config is changed.
        }

        /// <summary>
        /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
        /// </summary>
        public virtual void CopyFrom(PluginConfig other)
        {
            // This instance's members populated from other
        }
    }
}


/// Planned features/settings
/// - number of miss <= to PB
/// - number of miss < PB
/// - PB prefered leader board
