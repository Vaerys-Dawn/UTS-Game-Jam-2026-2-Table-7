using UnityEngine;
using UnityEngine.SceneManagement;

// Plays the music and sound effects for the whole game.
// It creates itself from Assets/Resources/AudioManager.prefab when the game starts,
// so it doesn't need to be placed in any scene, and it survives scene changes.
// Call it from anywhere with AudioManager.Play(AudioManager.Sfx.Jump) etc.
public class AudioManager : MonoBehaviour
{
    public enum Sfx { Jump, Throw, Hit, Dash, NpcDeath }

    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] private AudioClip titleMusic;
    [SerializeField] private AudioClip levelMusic;
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.5f;

    [Tooltip("Scenes that play the title music. Every other scene plays the level music.")]
    [SerializeField] private string[] titleMusicScenes = { "MainMenu", "Credit", "Win", "Lose" };

    [Header("Sound effects")]
    [SerializeField] private AudioClip jump;
    [SerializeField] private AudioClip attackThrow;
    [SerializeField] private AudioClip attackHit;
    [SerializeField] private AudioClip dash;
    [SerializeField] private AudioClip npcDeath;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 0.8f;

    [Tooltip("Small random pitch change so repeated sounds don't feel identical.")]
    [SerializeField, Range(0f, 0.3f)] private float pitchVariation = 0.08f;

    private AudioSource musicSource;
    // A few sources so overlapping sounds each keep their own pitch.
    private AudioSource[] sfxSources;
    private int nextSfxSource = 0;
    private const int SfxSourceCount = 8;

    // Used when no other AudioListener is active (Unity plays nothing without one).
    private AudioListener backupListener;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null) return;
        GameObject prefab = Resources.Load<GameObject>("AudioManager");
        if (prefab == null || prefab.GetComponent<AudioManager>() == null)
        {
            Debug.LogError("AudioManager: Assets/Resources/AudioManager.prefab is missing or has no AudioManager component, so no music or sound will play.");
            return;
        }
        Instantiate(prefab).name = "AudioManager";
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;

        sfxSources = new AudioSource[SfxSourceCount];
        for (int i = 0; i < SfxSourceCount; i++)
        {
            sfxSources[i] = gameObject.AddComponent<AudioSource>();
            sfxSources[i].playOnAwake = false;
        }

        backupListener = gameObject.AddComponent<AudioListener>();
        backupListener.enabled = false;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Start runs once the first scene is fully loaded, so the first scene always
    // gets its music even if the sceneLoaded event already fired before we subscribed.
    void Start()
    {
        OnSceneReady(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Single) OnSceneReady(scene.name);
    }

    private void OnSceneReady(string sceneName)
    {
        UpdateBackupListener();
        PlayMusicForScene(sceneName);
    }

    // Turns on our own listener only when the scene has no enabled AudioListener of its own,
    // so there is always exactly one and Unity doesn't warn about two.
    private void UpdateBackupListener()
    {
        bool sceneHasListener = false;
        foreach (AudioListener l in FindObjectsByType<AudioListener>())
        {
            if (l != backupListener && l.isActiveAndEnabled) { sceneHasListener = true; break; }
        }
        backupListener.enabled = !sceneHasListener;
    }

    private void PlayMusicForScene(string sceneName)
    {
        AudioClip clip = levelMusic;
        foreach (string s in titleMusicScenes)
        {
            if (s == sceneName) { clip = titleMusic; break; }
        }
        PlayMusic(clip);
    }

    // Keeps the current track going if the next scene uses the same music
    // (e.g. MainMenu -> Credit), so it doesn't restart from the beginning.
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager: no music clip set for this scene.");
            musicSource.Stop();
            return;
        }
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.Play();
        Debug.Log($"AudioManager: playing music '{clip.name}' in scene '{SceneManager.GetActiveScene().name}'.");
    }

    public static void Play(Sfx sfx, float volumeScale = 1f)
    {
        if (Instance != null) Instance.PlaySfx(sfx, volumeScale);
    }

    private void PlaySfx(Sfx sfx, float volumeScale)
    {
        AudioClip clip = GetClip(sfx);
        if (clip == null) return;
        AudioSource source = sfxSources[nextSfxSource];
        nextSfxSource = (nextSfxSource + 1) % SfxSourceCount;
        source.Stop(); // reuse the oldest source if all are busy
        source.clip = clip;
        source.volume = sfxVolume * volumeScale;
        source.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        source.Play();
    }

    private AudioClip GetClip(Sfx sfx)
    {
        switch (sfx)
        {
            case Sfx.Jump: return jump;
            case Sfx.Throw: return attackThrow;
            case Sfx.Hit: return attackHit;
            case Sfx.Dash: return dash;
            case Sfx.NpcDeath: return npcDeath;
            default: return null;
        }
    }
}
