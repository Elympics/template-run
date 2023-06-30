using UnityEngine;
using Elympics;

public class BuildInfoData
{
    public string GameVersion { get; private set; }
    public string ElympicsBuildNumber { get; private set; }
    public string ElympicsSDKVersion { get; private set; }

    public BuildInfoData()
    {
        SetupBuildInfo();
    }

    private void SetupBuildInfo()
    {
        GameVersion = Application.version;
        ElympicsBuildNumber = ElympicsConfig.LoadCurrentElympicsGameConfig().GameVersion;
        ElympicsSDKVersion = ElympicsVersionRetriever.GetVersionStringFromAssembly();
    }
}
