using UnityEngine;

public class TreeRandomizer : MonoBehaviour
{
    [SerializeField] private Vector2 randomOffsetNoise;
    [SerializeField] private Vector2 randomScaleRange;
    [SerializeField] private float chanceToActivate;
    [SerializeField] private GameObject treeObject;

    private void Awake()
    {
        if (Random.Range(0, 1f) > chanceToActivate) treeObject.SetActive(false);
        else
        { 
            treeObject.transform.position += new Vector3(Random.Range(-randomOffsetNoise.x / 2, randomOffsetNoise.x / 2), -Random.Range(0, randomOffsetNoise.y), 0);
            treeObject.transform.localScale = Vector3.one * Random.Range(randomScaleRange.x, randomScaleRange.y);
            if (Random.Range(0, 1f) > .5f) treeObject.GetComponent<SpriteRenderer>().flipX = true;
        }

    }
}
