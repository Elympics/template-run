using Elympics;
using JetBrains.Annotations;
using UnityEngine;

public class DevelopmentOnlyManager : MonoBehaviour
{
    [SerializeField] private GameObject editorOnlyPanel;

#if UNITY_EDITOR
    private void Start()
    {
        editorOnlyPanel.SetActive(true);
    }

    [UsedImplicitly]
    public void PlayHalfRemote()
    {
        ElympicsLobbyClient.Instance.PlayHalfRemote(0);
    }

    [UsedImplicitly]
    public void StartHalfRemoteServer()
    {
        ElympicsLobbyClient.Instance.StartHalfRemoteServer();
    }
#endif
}
