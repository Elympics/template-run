using UnityEngine;

public class WaitingIndicator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 180f;

    private void Update()
    {
        transform.Rotate(Vector3.back, rotationSpeed * Time.deltaTime);
    }
}
