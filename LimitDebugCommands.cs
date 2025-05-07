using ProjectM;
using Unity.Collections;
using Unity.Entities;
using VampireCommandFramework;
/*
namespace ServantForge.DebugCommands
{
    public static class LimitDebugCommands
    {
        public static void GetCountCommand(ChatCommandContext ctx, int prefabGUID)
        {
            var em = VWorld.Server.EntityManager;

            var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<ServantCoffinstation>(),
                ComponentType.ReadOnly<Team>()
            );

            var coffins = query.ToEntityArray(Allocator.Temp);
            try
            {
                int count = 0;
                foreach (var coffin in coffins)
                {
                    var station = em.GetComponentData<ServantCoffinstation>(coffin);
                    if (station.ConvertToUnit.GuidHash == prefabGUID &&
                        station.State == ServantCoffinState.ServantAlive)
                    {
                        // var team = em.GetComponentData<Team>(coffin);
                        // if (team.Owner == ctx.Event.User) {
                        //     count++;
                        // }
                    }
                }

                UnityEngine.Debug.Log($"You have {count} coffins for prefabGUID={prefabGUID}");
            }
            finally
            {
                coffins.Dispose();
            }
        }
    }
}
*/