using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class BuildVersionView : MonoBehaviour
{
    private void Start()
    {
        var buildInfo = new BuildInfoData();
        var label = GetComponent<TextMeshProUGUI>();

        label.text = $"v{buildInfo.GameVersion} ({buildInfo.ElympicsBuildNumber}) - sdk {buildInfo.ElympicsSDKVersion}";
    }
}
