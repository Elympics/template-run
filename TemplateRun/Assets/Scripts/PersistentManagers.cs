using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentManagers : MonoBehaviour
{
    public static PersistentManagers Instance;
    [SerializeField] private LoadingScreenManager loadingScreenManager;
    [SerializeField] private SoundManager soundManager;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        loadingScreenManager.SetUpOnAwake();
        soundManager.SetUpOnAwake();
        SceneManager.sceneLoaded += ReactToSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= ReactToSceneLoaded;
    }

    private void ReactToSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        loadingScreenManager.AdjustToSceneLoaded(scene);
        soundManager.AdjustMusicToScene(scene);
    }

    public void SetLoadingScreenSliderOpen(bool state)
    {
        loadingScreenManager.SetSliderOpen(state);
    }

    public void SetUpSoundManagerMusic()
    {
        soundManager.SetUpMusic();
    }
}
