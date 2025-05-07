using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Stunlock.Core; 
using System.Collections.Generic;
/*
namespace ServantForge.Patches
{

    [HarmonyPatch(typeof(AbilityCastStarted_SpawnPrefabSystem_Server), nameof(AbilityCastStarted_SpawnPrefabSystem_Server.OnUpdate))]
    public static class CoffinAbilityPatch
    {
        private const int COFFIN_ABILITY_GUID = 1866448125;

        private static readonly Dictionary<int, int> OccupantTerritoryLimits = new Dictionary<int, int>()
        {
            // Add more occupant types if needed
        };

        [HarmonyPrefix]
        public static void Prefix(AbilityCastStarted_SpawnPrefabSystem_Server __instance)
        {
            var em = __instance.EntityManager;

            var castQuery = __instance.__query_577032082_0;
            var eventEntities = castQuery.ToEntityArray(Allocator.Temp);

            try
            {
                foreach (var evtEntity in eventEntities)
                {
                    // Read the cast event
                    if (!em.Exists(evtEntity))
                        continue;

                    var castEvent = em.GetComponentData<AbilityCastStartedEvent>(evtEntity);

                    var abilityGroupEntity = castEvent.AbilityGroup;
                    if (abilityGroupEntity == Entity.Null || !em.Exists(abilityGroupEntity))
                        continue;
                    if (!em.HasComponent<PrefabGUID>(abilityGroupEntity))
                        continue;

                    int abilityGroupGuid = em.GetComponentData<PrefabGUID>(abilityGroupEntity).GuidHash;
                    if (abilityGroupGuid != COFFIN_ABILITY_GUID)
                        continue;

                    var characterEntity = castEvent.Character;
                    if (characterEntity == Entity.Null || !em.Exists(characterEntity))
                        continue;
                    if (!em.HasComponent<PlayerCharacter>(characterEntity))
                        continue;

                    int occupantGuid = -1773935659;
                    if (!OccupantTerritoryLimits.TryGetValue(occupantGuid, out int limit))
                        continue;

                    if (limit <= 0)
                    {
                        SendUserMessage(em, characterEntity,
                            $"Occupant {occupantGuid} is disallowed here!");
                        em.DestroyEntity(evtEntity);
                        continue;
                    }

                    var coffinEntity = FindClosestCoffin(em, characterEntity, 3f);
                    if (coffinEntity == Entity.Null)

                    if (!CastleTerritoryHelper.TryGetCastleTerritory(em, coffinEntity, out var territoryEntity))

                    int currentCount = CountOccupantsInTerritory(em, occupantGuid, territoryEntity);

                    if (currentCount >= limit)
                    {
                        SendUserMessage(em, characterEntity,
                            $"You already have {currentCount}/{limit} occupant {occupantGuid} in this territory!");
                        // Destroy the event => user can't open coffin
                        em.DestroyEntity(evtEntity);
                    }
                }
            }
            finally
            {
                eventEntities.Dispose();
            }
        }

        private static int CountOccupantsInTerritory(EntityManager em, int occupantGuid, Entity territoryEntity)
        {
            int count = 0;

            // gather all coffins
            var coffinQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<ServantCoffinstation>(),
                ComponentType.ReadOnly<LocalToWorld>()
            );
            var coffins = coffinQuery.ToEntityArray(Allocator.Temp);

            try
            {
                foreach (var c in coffins)
                {
                    if (!CastleTerritoryHelper.IsCoffinInTerritory(em, c, territoryEntity))
                        continue;

                    var station = em.GetComponentData<ServantCoffinstation>(c);
                    // occupant matches occupantGuid + is fully alive
                    if (station.ConvertToUnit.GuidHash == occupantGuid &&
                        station.State == ServantCoffinState.ServantAlive)
                    {
                        count++;
                    }
                }
            }
            finally
            {
                coffins.Dispose();
            }

            return count;
        }

       
        private static Entity FindClosestCoffin(EntityManager em, Entity player, float radius)
        {
            float3 playerPos = float3.zero;
            if (em.HasComponent<LocalToWorld>(player))
            {
                playerPos = em.GetComponentData<LocalToWorld>(player).Position;
            }

            var coffinQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<ServantCoffinstation>(),
                ComponentType.ReadOnly<LocalToWorld>()
            );
            var coffins = coffinQuery.ToEntityArray(Allocator.Temp);
            Entity best = Entity.Null;
            float bestDistSq = radius * radius;

            try
            {
                foreach (var c in coffins)
                {
                    float3 pos = em.GetComponentData<LocalToWorld>(c).Position;
                    float dsq = math.distancesq(playerPos, pos);
                    if (dsq < bestDistSq)
                    {
                        bestDistSq = dsq;
                        best = c;
                    }
                }
            }
            finally
            {
                coffins.Dispose();
            }
            return best;
        }

      
        private static void SendUserMessage(EntityManager em, Entity character, string msg)
        {
            UnityEngine.Debug.Log($"[CoffinAbilityPatch] {msg}");
        }
    }
}

*/

//Future Patch to allow limitation of number of servants.