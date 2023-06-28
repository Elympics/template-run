using UnityEngine;
using System;
using static Elympics.ElympicsCloudPing;
using Cysharp.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "RegionData", menuName = "TemplateRun/RegionData")]
public class RegionData : ScriptableObject
{
    [SerializeField] private string[] availableRegions = new string[] { "warsaw", "dallas" };

    private bool alreadyCached = false;
    private string Region;
    private float LatencyMs;

    public async UniTask<(string Region, float LatencyMs)> ClosestRegion()
    {
        if (!alreadyCached)
        {
            (Region, LatencyMs) = await ChooseClosestRegion(availableRegions);
            alreadyCached = true;
        }

        return (Region, LatencyMs);
    }
}
