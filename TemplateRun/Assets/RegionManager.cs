using UnityEngine;
using static Elympics.ElympicsCloudPing;

public class RegionManager : MonoBehaviour
{

    [SerializeField] private string[] availableRegions = new string[] { "warsaw", "dallas" };
    public (System.String Region, System.Single LatencyMs) closestRegion { get; private set; }
    public static RegionManager Instance;

    public async void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        closestRegion = await ChooseClosestRegion(availableRegions);
    }
}
