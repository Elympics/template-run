using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentEffectsManager : MonoBehaviour
{
    public static PersistentEffectsManager Instance;
    [SerializeField] private LoadingScreenManager loadingScreenManager;
    [SerializeField] private MusicManager musicManager;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        AudioProperties.Init();

        loadingScreenManager.SetUpOnAwake();
        musicManager.SetUpOnAwake();
        SceneManager.sceneLoaded += ReactToSceneLoaded;
    }

    private void OnDestroy()
    {
        AudioProperties.Serialize();
        SceneManager.sceneLoaded -= ReactToSceneLoaded;
    }

    private void ReactToSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        loadingScreenManager.AdjustToSceneLoaded(scene);
        musicManager.AdjustStateToScene(scene);
    }

    public void SetLoadingScreenSliderOpen(bool state) => loadingScreenManager.SetSliderOpen(state);
    public void PlayGameplayMusic() => musicManager.PlayGameplayMusic();
    public void PlayGameOverSoundEffects() => musicManager.PlayGameOverSoundEffects();
}
