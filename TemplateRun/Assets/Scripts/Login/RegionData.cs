using UnityEngine;
using System;
using static Elympics.ElympicsCloudPing;
using Cysharp.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "RegionData", menuName = "TemplateRun/RegionData")]
public class RegionData : ScriptableObject
{
    [SerializeField] private string[] availableRegions = new string[] { "warsaw", "dallas" };

    private (string Region, float LatencyMs)? CachedClosestRegion;

    public async UniTask<(string Region, float LatencyMs)> ClosestRegion()
    {
        if (!CachedClosestRegion.HasValue)
        {
            CachedClosestRegion = await ChooseClosestRegion(availableRegions);
        }

        return CachedClosestRegion.Value;
    }
}
