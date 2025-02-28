using Elympics;
using MatchTcpClients.Synchronizer;
using UnityEngine.Assertions;

public class CustomClientHandler : ElympicsMonoBehaviour, IClientHandlerGuid
{
    public void OnConnected(TimeSynchronizationData _)
    {
        if (!Elympics.IsClient)
            return;

        Assert.IsNotNull(PersistentEffectsManager.Instance);

        PersistentEffectsManager.Instance.SetMatchLoadingScreenActive(false);
        PersistentEffectsManager.Instance.PlayGameplayMusic();
    }
}
