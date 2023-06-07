using UnityEngine;

public class ManagerFinder : MonoBehaviour
{
    public void FindableLoadingScreenClose()
    {
        FindObjectOfType<LoadingScreenManager>().SetSliderOpen(false);
    }
}
