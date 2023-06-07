using UnityEngine;
using TMPro;

public class BuildVersionView : MonoBehaviour
{
	private TextMeshProUGUI tmp;

	private void Awake()
	{
		tmp = GetComponent<TextMeshProUGUI>();

		var buildInfoData = BuildInfoData.Load();
		buildInfoData.SetupBuildInfo();
		
		tmp.text = $"v{buildInfoData.GameVersion} ({buildInfoData.ElympicsBuildNumber}) - sdk {buildInfoData.ElympicsSDKVersion}";
	}
}
