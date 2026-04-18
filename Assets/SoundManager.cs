using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }
    
    [Header("Audio Source")]
    [SerializeField] private AudioSource musicSource;

    [Header("Music Tracks")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip level1Music;
    [SerializeField] private AudioClip level2Music;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return; // Stop running this duplicate's Awake
        }

        // Initialize saved volume
        ChangeMusicVolume(0);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "_MainMenu":
                PlayMusic(mainMenuMusic);
                break;
            case "Level1":
                PlayMusic(level1Music);
                break;
            case "Level2":
                PlayMusic(level2Music);
                break;
            default:
                if (musicSource.isPlaying)
                    musicSource.Stop();
                break;
        }
    }

    public void PlayMusic(AudioClip music)
    {
        // Don't restart the song if it's already playing!
        if (musicSource.clip == music) return;

        musicSource.clip = music;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void ChangeMusicVolume(float _change)
    {
        ChangeSourceVolume(0.5f, "musicVolume", _change, musicSource);
    }

    public void ChangeSourceVolume(float baseVolume, string volumeName, float change, AudioSource source)
    {
        float currentVolume = PlayerPrefs.GetFloat(volumeName, 1);
        currentVolume += change;

        if (currentVolume > 1)
            currentVolume = 0;
        else if (currentVolume < 0)
            currentVolume = 1;

        float finalVolume = currentVolume * baseVolume;
        source.volume = finalVolume;

        PlayerPrefs.SetFloat(volumeName, currentVolume);
    }
}