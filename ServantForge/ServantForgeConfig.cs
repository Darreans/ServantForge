using BepInEx.Configuration;

namespace ServantForge
{
    internal static class ServantForgeConfig
    {
        public static ConfigEntry<float> ProficiencyIncrement;
        public static ConfigEntry<float> MaxProficiency;
        public static ConfigEntry<int> CostItemGUID;
        public static ConfigEntry<int> CostItemAmount;

        public static void Initialize(ConfigFile config)
        {
            ProficiencyIncrement = config.Bind(
                "ServantForge",
                "ProficiencyIncrement",
                0.06f,
                "How much proficiency to add per purchase (0.06 = +6%)."
            );

            MaxProficiency = config.Bind(
                "ServantForge",
                "MaxProficiency",
                0.44f,
                "Max proficiency (default is 0.44 = 44%)."
            );

            CostItemGUID = config.Bind(
                "ServantForge",
                "CostItemGUID",
                -598100816,
                "PrefabGUID for the cost item to remove."
            );

            CostItemAmount = config.Bind(
                "ServantForge",
                "CostItemAmount",
                300,
                "How many of that item must be removed each time."
            );
        }
    }
}
