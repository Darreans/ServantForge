using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using VampireCommandFramework;
using Bloodstone.API;
using Bloodstone.Hooks;

namespace ServantForge
{
    [BepInPlugin("ServantForge", "ServantForge", "1.0.0")]
    [BepInDependency("gg.deca.VampireCommandFramework")]
    [Reloadable] // from Bloodstone.API
    public class ServantForgePlugin : BasePlugin
    {
        private Harmony _harmony;
        public static ManualLogSource LogInstance { get; private set; }

        public override void Load()
        {
            LogInstance = Log;
            Log.LogInfo("[ServantForge] Loading plugin...");

            // Initialize your config values
            ServantForgeConfig.Initialize(Config);

            // Patch all with Harmony
            _harmony = new Harmony("ServantForge");
            _harmony.PatchAll();

            // Register all VampireCommandFramework commands
            CommandRegistry.RegisterAll();

            // Subscribe to chat messages for reload command
            Chat.OnChatMessage += HandleReloadCommand;

            Log.LogInfo("[ServantForge] Loaded.");
        }

        public override bool Unload()
        {
            // Unpatch Harmony before unloading
            _harmony?.UnpatchSelf();

            // Unsubscribe from the chat hook
            Chat.OnChatMessage -= HandleReloadCommand;

            Log.LogInfo("[ServantForge] Unloaded.");
            return true;
        }

        private void HandleReloadCommand(VChatEvent ev)
        {
            // Only admins can reload
            if (ev.Message.Equals("!servantreload", System.StringComparison.OrdinalIgnoreCase) && ev.User.IsAdmin)
            {
                try
                {
                    Config.Reload();
                    Config.Save();
                    ServantForgeConfig.Initialize(Config);

                    ev.User.SendSystemMessage("<color=#00FF00>ServantForge config reloaded successfully!</color>");
                    Log.LogInfo("[ServantForge] Config reloaded successfully.");
                }
                catch (System.Exception ex)
                {
                    ev.User.SendSystemMessage($"<color=#FF0000>ServantForge config reload failed:</color> {ex.Message}");
                    Log.LogError($"Error reloading ServantForge config: {ex}");
                }
            }
        }
    }
}
