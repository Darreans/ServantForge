using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using VampireCommandFramework;

namespace ServantForge
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("gg.deca.VampireCommandFramework")]
    public class ServantForgePlugin : BasePlugin
    {
        private Harmony _harmony;
        public static ManualLogSource LogInstance { get; private set; }

        public override void Load()
        {
            LogInstance = Log;
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} loading...");

            ServantForgeConfig.Initialize(Config);

            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll();

            CommandRegistry.RegisterAll();

            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loaded!");
        }

        public override bool Unload()
        {
            CommandRegistry.UnregisterAssembly();
            _harmony?.UnpatchSelf();

            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is unloaded!");
            return true;
        }
    }

    public static class ReloadConfigCommand
    {
        [Command("servantreload", description: "Reloads the ServantForge configuration.", adminOnly: true)]
        public static void HandleReloadCommand(ChatCommandContext ctx)
        {
            try
            {
                ServantForgeConfig.ReloadConfig(); 
                ctx.Reply("<color=#00FF00>ServantForge config reloaded successfully!</color>");
            }
            catch (System.Exception ex)
            {
                ctx.Reply($"<color=#FF0000>ServantForge config reload failed:</color> {ex.Message}");
                if (ServantForgePlugin.LogInstance != null)
                    ServantForgePlugin.LogInstance.LogError($"Error reloading ServantForge config via command: {ex}");
            }
        }
    }
}