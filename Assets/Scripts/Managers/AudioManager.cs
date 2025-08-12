using UnityEngine;
using System;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(0.1f, 3f)]
    public float pitch = 1f;

    public bool loop;
    public bool isMusic;

    [HideInInspector]
    public AudioSource source;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private Sound[] sounds;

    private bool musicOn = true;
    private bool sfxOn = true;

    public bool IsMusicOn => musicOn;
    public bool IsSFXOn => sfxOn;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
        Play("Music");
    }

    private void OnEnable()
    {
        if (Player.instance != null)
        {
            Player.instance.OnJump += HandlePlayerJump;
            Player.instance.OnDied += HandlePlayerDeath;
        }
    }

    private void OnDisable()
    {
        if (Player.instance != null)
        {
            Player.instance.OnJump -= HandlePlayerJump;
            Player.instance.OnDied -= HandlePlayerDeath;
        }
    }

    private void HandlePlayerJump()
    {
        Play("Jump");
    }

    private void HandlePlayerDeath()
    {
        Play("GameOver");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            if ((s.isMusic && musicOn) || (!s.isMusic && sfxOn))
                s.source.Play();
        }

    }

    public void SetSoundEnabled(bool enabled)
    {
        sfxOn = !enabled;
        foreach (var s in Array.FindAll(sounds, sound => sound.name != "Music"))
        {
            s.source.mute = enabled;
        }
    }

    public void SetMusicEnabled(bool enabled)
    {
        musicOn = !enabled;
        Sound music = Array.Find(sounds, sound => sound.name == "Music");
        if (music != null)
            music.source.mute = enabled;
    }

    public void ToggleSound()
    {
        SetSoundEnabled(sfxOn);
    }

    public void ToggleMusic()
    {
        SetMusicEnabled(musicOn);
    }
}
