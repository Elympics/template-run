using UnityEngine;

public class EndGameTrigger : MonoBehaviour
{
    [SerializeField] private GameStateSynchronizer gameStateSynchronizer;
    [SerializeField] private LayerMask deadlyLayer;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (DoesMaskContainLayer(deadlyLayer, collision.collider.gameObject.layer))
        {
            Debug.Log("DEAD");
            gameStateSynchronizer.FinishGame();
        }
    }

    private bool DoesMaskContainLayer(LayerMask mask, int layer) => (mask & (1 << layer)) != 0;
}
