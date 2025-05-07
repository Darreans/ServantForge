using Unity.Entities;

namespace ServantForge.Utils
{
    internal static class VWorldUtils
    {
        private static World? _serverWorld;
        private static EntityManager? _entityManager;

        public static World Server
        {
            get
            {
                if (_serverWorld != null && _serverWorld.IsCreated)
                {
                    return _serverWorld;
                }

                _serverWorld = GetWorld("Server");

                if (_serverWorld == null || !_serverWorld.IsCreated)
                {
                    _entityManager = null;
                    if (ServantForge.ServantForgePlugin.LogInstance != null)
                        ServantForge.ServantForgePlugin.LogInstance.LogError("Server world is not available or not created.");
                    throw new System.Exception("Server world is not available or not created.");
                }

                _entityManager = _serverWorld.EntityManager;
                return _serverWorld;
            }
        }

        public static EntityManager EntityManager
        {
            get
            {
                if (_entityManager != null && _serverWorld != null && _serverWorld.IsCreated)
                {
                    return _entityManager.Value;
                }
                return Server.EntityManager;
            }
        }

        private static World? GetWorld(string name)
        {
            foreach (var world in World.s_AllWorlds)
            {
                if (world.Name == name)
                {
                    return world;
                }
            }
            if (ServantForge.ServantForgePlugin.LogInstance != null)
                ServantForge.ServantForgePlugin.LogInstance.LogWarning($"World '{name}' not found.");
            return null;
        }

        public static bool IsServerWorldReady()
        {
            try
            {
                return Server != null && Server.IsCreated;
            }
            catch (System.Exception ex)
            {
                if (ServantForge.ServantForgePlugin.LogInstance != null)
                    ServantForge.ServantForgePlugin.LogInstance.LogError($"Server world check failed: {ex.Message}");
                return false;
            }
        }
    }
}