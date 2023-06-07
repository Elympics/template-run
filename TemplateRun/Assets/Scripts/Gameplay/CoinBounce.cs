using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBounce : MonoBehaviour
{
    [SerializeField] private float bounceAmplitude;
    [SerializeField] private float bouncePeriod;
    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > bouncePeriod) timer = 0;

        float bounceProgress = timer / bouncePeriod;
        transform.localPosition = Vector3.up * Mathf.Cos(bounceProgress * 2 * Mathf.PI) * bounceAmplitude;
    }
}
