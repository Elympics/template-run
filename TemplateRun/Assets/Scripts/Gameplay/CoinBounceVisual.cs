using UnityEngine;

public class CoinBounceVisual : MonoBehaviour
{
    [SerializeField] private float bounceAmplitude;
    [SerializeField] private float bouncePeriod;
    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > bouncePeriod) timer = 0;

        float bounceProgress = timer / bouncePeriod;
        transform.localPosition = bounceAmplitude * Mathf.Cos(bounceProgress * 2 * Mathf.PI) * Vector3.up;
    }
}
