using Bloodstone.API;
using ProjectM;
using ProjectM.Network;
using Stunlock.Core;
using Unity.Entities;
using Unity.Mathematics;

namespace ServantForge.Helpers
{
    public static class InventoryHelper
    {
        public static bool HasEnoughItem(Entity playerEntity, PrefabGUID itemGuid, int needed)
        {
            var em = VWorld.Server.EntityManager;
            if (!InventoryUtilities.TryGetInventoryEntity(em, playerEntity, out var invEntity)) return false;
            if (!em.HasComponent<InventoryBuffer>(invEntity)) return false;
            var buffer = em.GetBuffer<InventoryBuffer>(invEntity);
            int total = 0;
            foreach (var slot in buffer)
            {
                if (slot.ItemType.Equals(itemGuid))
                {
                    total += slot.Amount;
                    if (total >= needed) return true;
                }
            }
            return total >= needed;
        }

        public static bool RemoveItem(Entity playerEntity, PrefabGUID itemGuid, int toRemove)
        {
            if (toRemove <= 0) return true;
            var em = VWorld.Server.EntityManager;
            if (!InventoryUtilities.TryGetInventoryEntity(em, playerEntity, out var invEntity)) return false;
            if (!em.HasComponent<InventoryBuffer>(invEntity)) return false;
            var buffer = em.GetBuffer<InventoryBuffer>(invEntity);
            int needed = toRemove;
            for (int i = 0; i < buffer.Length && needed > 0; i++)
            {
                var slot = buffer[i];
                if (slot.ItemType.Equals(itemGuid) && slot.Amount > 0)
                {
                    int removeCount = math.min(slot.Amount, needed);
                    InventoryUtilitiesServer.TryRemoveItemAtIndex(em, playerEntity, slot.ItemType, removeCount, i, false);
                    needed -= removeCount;
                }
            }
            return needed <= 0;
        }
    }
}
