using global::System;
using SysException = global::System.Exception;
using Il2CppException = global::Il2CppSystem.Exception;
using VampireCommandFramework;
using global::UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Bloodstone.API;
using ProjectM;
using ProjectM.Network;
using Unity.Collections;
using Stunlock.Core;

namespace ServantForge
{
    public static class UpgradeServantCommand
    {
        private const float MaxBlood = 100f;
        // Removed the hard-coded "Thistle" string
        // private const string CostItemName = "Thistle"; 

        private const string ColorHexSuccess = "00FF00";
        private const string ColorHexError = "FF0000";

        [Command("upgradeservant", "us", "Upgrades a fully converted servant's expertise, for a cost.")]
        public static void OnUpgradeServant(ChatCommandContext ctx)
        {
            try
            {
                var em = VWorld.Server.EntityManager;

                if (!em.HasComponent<EntityAimData>(ctx.Event.SenderCharacterEntity))
                {
                    ReplyColored(ctx, "Cannot read your aim/cursor data.", ColorHexError);
                    return;
                }

                var aimData = em.GetComponentData<EntityAimData>(ctx.Event.SenderCharacterEntity);
                float3 aimPos = aimData.AimPosition;

                var coffinEntity = FindClosestTile<ServantCoffinstation>(aimPos, 2.5f);
                if (coffinEntity == Entity.Null)
                {
                    ReplyColored(ctx, "No servant coffin at your cursor.", ColorHexError);
                    return;
                }

                var coffinData = em.GetComponentData<ServantCoffinstation>(coffinEntity);
                if (coffinData.State != ServantCoffinState.ServantAlive)
                {
                    ReplyColored(ctx, "Servant is not fully converted yet.", ColorHexError);
                    return;
                }

                // Pull item config values
                float proficiencyIncrement = ServantForgeConfig.ProficiencyIncrement.Value;
                float maxProficiency = ServantForgeConfig.MaxProficiency.Value;
                int costItemGuid = ServantForgeConfig.CostItemGUID.Value;
                int costItemNeeded = ServantForgeConfig.CostItemAmount.Value;

                // NEW: Read the item name from the config
                string costItemName = ServantForgeConfig.CostItemName.Value;

                float currentProf = coffinData.ServantProficiency;
                if (currentProf >= maxProficiency)
                {
                    ReplyColored(ctx, "You can't upgrade the servant's Expertise anymore!", ColorHexError);
                    return;
                }

                var costGuid = new PrefabGUID(costItemGuid);

                if (!HasEnoughItem(ctx.Event.SenderCharacterEntity, costGuid, costItemNeeded))
                {
                    ReplyColored(ctx, $"You need {costItemNeeded} {costItemName}.", ColorHexError);
                    return;
                }

                if (!RemoveItem(ctx.Event.SenderCharacterEntity, costGuid, costItemNeeded))
                {
                    ReplyColored(ctx, $"Failed to remove {costItemNeeded} {costItemName} from your inventory.", ColorHexError);
                    return;
                }

                float needed = maxProficiency - currentProf;
                float actualInc = (needed < proficiencyIncrement) ? needed : proficiencyIncrement;

                coffinData.ServantProficiency += actualInc;
                coffinData.ServantProficiency  = math.min(coffinData.ServantProficiency, maxProficiency);

                coffinData.BloodQuality = (coffinData.ServantProficiency / maxProficiency) * MaxBlood;
                em.SetComponentData(coffinEntity, coffinData);

                var connectedServant = coffinData.ConnectedServant.GetEntityOnServer();
                if (connectedServant != Entity.Null && em.HasComponent<ServantPower>(connectedServant))
                {
                    var stats = em.GetComponentData<ServantPower>(connectedServant);
                    float neededExp = maxProficiency - stats.Expertise;
                    if (neededExp > 0f)
                    {
                        float actualExp = (neededExp < actualInc) ? neededExp : actualInc;
                        stats.Expertise = math.min(stats.Expertise + actualExp, maxProficiency);
                        em.SetComponentData(connectedServant, stats);
                    }
                }

                float incPercent = actualInc * 100f;
                ReplyColored(ctx, $"Your servant's Expertise has been upgraded by {incPercent:F1}%!", ColorHexSuccess);
            }
            catch (SysException ex)
            {
                ReplyColored(ctx, "Error in .upgradeservant", ColorHexError);
                var il2cppEx = new Il2CppException(ex.Message);
                Debug.LogException(il2cppEx);
            }
        }

        private static Entity FindClosestTile<T>(float3 aimPos, float maxDist)
        {
            var em = VWorld.Server.EntityManager;
            var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<TilePosition>(),
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<T>()
            );

            var ents = query.ToEntityArray(Allocator.Temp);
            Entity closest = Entity.Null;
            float minDistSq = maxDist * maxDist;
            foreach (var e in ents)
            {
                var pos = em.GetComponentData<Translation>(e).Value;
                float dsq = math.distancesq(aimPos, pos);
                if (dsq < minDistSq)
                {
                    minDistSq = dsq;
                    closest = e;
                }
            }
            ents.Dispose();
            return closest;
        }

        private static bool HasEnoughItem(Entity player, PrefabGUID guid, int needed)
        {
            var em = VWorld.Server.EntityManager;
            if (!InventoryUtilities.TryGetInventoryEntity(em, player, out var invEnt)) return false;
            if (!em.HasComponent<InventoryBuffer>(invEnt)) return false;

            var buffer = em.GetBuffer<InventoryBuffer>(invEnt);
            int count = 0;
            foreach (var slot in buffer)
            {
                if (slot.ItemType.Equals(guid))
                {
                    count += slot.Amount;
                    if (count >= needed) return true;
                }
            }
            return count >= needed;
        }

        private static bool RemoveItem(Entity player, PrefabGUID guid, int amount)
        {
            if (amount <= 0) return true;
            var em = VWorld.Server.EntityManager;
            if (!InventoryUtilities.TryGetInventoryEntity(em, player, out var invEnt)) return false;
            if (!em.HasComponent<InventoryBuffer>(invEnt)) return false;

            var buffer = em.GetBuffer<InventoryBuffer>(invEnt);
            int needed = amount;
            for (int i = 0; i < buffer.Length && needed > 0; i++)
            {
                var slot = buffer[i];
                if (slot.ItemType.Equals(guid) && slot.Amount > 0)
                {
                    int remove = math.min(slot.Amount, needed);
                    InventoryUtilitiesServer.TryRemoveItemAtIndex(em, player, slot.ItemType, remove, i, false);
                    needed -= remove;
                }
            }
            return needed <= 0;
        }

        private static void ReplyColored(ChatCommandContext ctx, string message, string hex)
        {
            ctx.Reply($"<color=#{hex}>{message}</color>");
        }
    }
}
