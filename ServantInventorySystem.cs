using ProjectM;
using Stunlock.Core;
using Unity.Entities;
using Unity.Mathematics;
using ServantForge.Utils;

namespace ServantForge.Systems
{
    public static class ServantInventorySystem
    {
        public static bool HasEnoughItem(Entity playerEntity, PrefabGUID itemGuid, int neededAmount)
        {
            var em = VWorldUtils.EntityManager;

            if (!InventoryUtilities.TryGetInventoryEntity(em, playerEntity, out var inventoryEntity))
            { return false; }

            if (!em.HasBuffer<ProjectM.InventoryBuffer>(inventoryEntity))
            { return false; }

            var buffer = em.GetBuffer<ProjectM.InventoryBuffer>(inventoryEntity);

            int count = 0;
            foreach (var slot in buffer)
            {
                if (slot.ItemType == itemGuid) 
                {
                    count += slot.Amount;
                    if (count >= neededAmount) { return true; }
                }
            }
            return (count >= neededAmount);
        }

        public static bool TryRemoveItem(Entity playerEntity, PrefabGUID itemGuid, int amountToRemove)
        {
            if (amountToRemove <= 0) { return true; }

            var em = VWorldUtils.EntityManager;

            bool success = InventoryUtilitiesServer.TryRemoveItem(em, playerEntity, itemGuid, amountToRemove);

            return success;
        }
    }
}