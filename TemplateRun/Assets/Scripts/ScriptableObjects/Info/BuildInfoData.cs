using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Elympics;

[CreateAssetMenu(fileName = "BuildInfoData", menuName = "TemplateRun/BuildInfoData", order = 0)]
public class BuildInfoData : ScriptableObject
{
	[SerializeField] private string gameVersion;
	[SerializeField] private string elympicsBuildNumber;
	[SerializeField] private string elympicsSDKVersion;

	public static string PATH_IN_RESOURCES = "BuildInfo/BuildInfoData";

	public string GameVersion => gameVersion;
	public string ElympicsBuildNumber => elympicsBuildNumber;
	public string ElympicsSDKVersion => elympicsSDKVersion;

	public static BuildInfoData Load() => Resources.Load<BuildInfoData>(PATH_IN_RESOURCES);

	public void SetupData(string gameVersion, string elympicsBuildNumber, string elympicsSDKVersion)
	{
		this.gameVersion = gameVersion;
		this.elympicsBuildNumber = elympicsBuildNumber;
		this.elympicsSDKVersion = elympicsSDKVersion;
	}

	public void SetupBuildInfo()
	{
		var gameVersion = Application.version;
		var elympicsBuildNumber = ElympicsConfig.LoadCurrentElympicsGameConfig().GameVersion;
		var elympicsSDKVersion = ElympicsVersionRetriever.GetVersionStringFromAssembly();
		SetupData(gameVersion, elympicsBuildNumber, elympicsSDKVersion);
	}


}
