using UnityEngine;
using System;
using static Elympics.ElympicsCloudPing;
using Cysharp.Threading.Tasks;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/RegionData")]
public class RegionData : ScriptableObject
{
    [SerializeField] private string[] availableRegions = new string[] { "warsaw", "dallas" };

    private bool alreadyCached = false;
    private (string Region, float LatencyMs) CachedClosestRegion;

    public async UniTask<(string Region, float LatencyMs)> ClosestRegion()
    {
        if (!alreadyCached)
        {
            CachedClosestRegion = await ChooseClosestRegion(availableRegions);
            alreadyCached = true;
        }

        return CachedClosestRegion;
    }
}
