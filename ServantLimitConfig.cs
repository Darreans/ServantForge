using BepInEx.Configuration;
using System.Collections.Generic;
/*
namespace ServantForge
{
    internal static class ServantLimitConfig
    {
        public static Dictionary<int, int> ServantLimitDict = new Dictionary<int, int>();

        public static ConfigEntry<int> MilitiaHeavyServantLimit;

        public static void Initialize(ConfigFile config)
        {
            MilitiaHeavyServantLimit = config.Bind<int>(
                "ServantLimits",
                "MilitiaHeavyServantLimit",
                2, // default maximum
                "Max number of Militia_Heavy_Servants a single player can own"
            );

            ServantLimitDict[-1773935659] = MilitiaHeavyServantLimit.Value;

            ServantForgePlugin.LogInstance.LogInfo($"[ServantLimitConfig] MilitiaHeavyServantLimit = {MilitiaHeavyServantLimit.Value}");
        }
    }
}

*/