using UnityEngine;
using System;
using static Elympics.ElympicsCloudPing;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/RegionData")]
public class RegionData : ScriptableObject
{
    [SerializeField] private string[] availableRegions = new string[] { "warsaw", "dallas" };

    public string Region { get; set; }
    public float LatencyMs { get; set; }

    private async void OnEnable()
    {
        (Region, LatencyMs) = await ChooseClosestRegion(availableRegions);
    }
}
