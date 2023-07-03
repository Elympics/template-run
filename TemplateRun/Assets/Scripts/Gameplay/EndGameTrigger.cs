using UnityEngine;

public class EndGameTrigger : MonoBehaviour
{
    [SerializeField] private LayerMask deadlyLayer;

    public event System.Action OnEndGameTriggered;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (DoesMaskContainLayer(deadlyLayer, collision.collider.gameObject.layer))
        {
            Debug.Log("Game Over");
            OnEndGameTriggered?.Invoke();
        }
    }

    private bool DoesMaskContainLayer(LayerMask mask, int layer) => (mask & (1 << layer)) != 0;
}
