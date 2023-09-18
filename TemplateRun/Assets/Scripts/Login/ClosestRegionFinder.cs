using UnityEngine;
using System;
using Elympics;
using Cysharp.Threading.Tasks;

public static class ClosestRegionFinder
{
    private static (string Region, float LatencyMs)? CachedClosestRegion;

    public static async UniTask<(string Region, float LatencyMs)> GetClosestRegion()
    {
        if (!CachedClosestRegion.HasValue)
        {
            Debug.Log("Searching for closest region...");
            CachedClosestRegion = await ElympicsCloudPing.ChooseClosestRegion(ElympicsRegions.AllAvailableRegions);
            Debug.Log("Closest region has been cached!");
        }

        return CachedClosestRegion.Value;
    }
}
