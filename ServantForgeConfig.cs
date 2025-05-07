using BepInEx.Configuration;

namespace ServantForge
{
    internal static class ServantForgeConfig
    {
        public static ConfigEntry<float> ProficiencyIncrement { get; private set; }
        public static ConfigEntry<float> MaxProficiency { get; private set; }
        public static ConfigEntry<int> CostItemGUID { get; private set; }
        public static ConfigEntry<int> CostItemAmount { get; private set; }
        public static ConfigEntry<string> CostItemName { get; private set; }

        private static ConfigFile _currentConfigFile;

        public static void Initialize(ConfigFile config)
        {
            _currentConfigFile = config; 

            ProficiencyIncrement = _currentConfigFile.Bind(
                "ServantForge",
                "ProficiencyIncrement",
                0.06f, 
                "How much proficiency to add per purchase (e.g., 0.06 = +6%)."
            );

            MaxProficiency = _currentConfigFile.Bind(
                "ServantForge",
                "MaxProficiency",
                0.44f, 
                "Max servant proficiency (e.g., default is 0.44 = 44%)."
            );

            CostItemGUID = _currentConfigFile.Bind(
                "ServantForge",
                "CostItemGUID",
                -598100816, 
                "PrefabGUID for the cost item to remove."
            );

            CostItemAmount = _currentConfigFile.Bind(
                "ServantForge",
                "CostItemAmount",
                300, 
                "How many of that item must be removed each time."
            );

            CostItemName = _currentConfigFile.Bind(
                "ServantForge",
                "CostItemName",
                "Thistle", 
                "Friendly display name for the cost item (e.g., Thistle)."
            );
        }

        public static void ReloadConfig()
        {
            if (_currentConfigFile != null)
            {
                _currentConfigFile.Reload();
                if (ServantForgePlugin.LogInstance != null)
                    ServantForgePlugin.LogInstance.LogInfo("ServantForge configuration reloaded.");
            }
            else
            {
                if (ServantForgePlugin.LogInstance != null)
                    ServantForgePlugin.LogInstance.LogError("ServantForge configuration could not be reloaded: ConfigFile reference is null.");
            }
        }
    }
}