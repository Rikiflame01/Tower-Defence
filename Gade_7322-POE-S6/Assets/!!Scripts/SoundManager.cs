using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource backgroundMusicSource;
    public AudioSource sfxSource;
    public string audioFolderPath = "Audio";

    [Tooltip("List of background music tracks to play in sequence.")]
    public List<string> musicQueueSetup = new List<string>();

    private Queue<string> musicQueue = new Queue<string>();
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
    private bool isMusicPlaying = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        GameObject audioSourcesObject = GameObject.FindWithTag("AudioSources");
        if (audioSourcesObject == null)
        {
            audioSourcesObject = new GameObject("AudioSources");
            audioSourcesObject.tag = "AudioSources";
            audioSourcesObject.transform.SetParent(transform);
        }

        if (backgroundMusicSource == null)
        {
            backgroundMusicSource = audioSourcesObject.AddComponent<AudioSource>();
            backgroundMusicSource.loop = false;
        }

        if (sfxSource == null)
        {
            sfxSource = audioSourcesObject.AddComponent<AudioSource>();
        }
        backgroundMusicSource.volume = 0.5f;
        LoadAudioClips();

        InitializeMusicQueue();
    }

    void LoadAudioClips()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>(audioFolderPath);
        foreach (AudioClip clip in clips)
        {
            audioClips[clip.name] = clip;
        }
    }

    void InitializeMusicQueue()
    {
        foreach (string trackName in musicQueueSetup)
        {
            if (audioClips.ContainsKey(trackName))
            {
                musicQueue.Enqueue(trackName);
            }
            else
            {
                Debug.LogWarning($"Track '{trackName}' not found in audio clips. Skipping...");
            }
        }

        if (musicQueue.Count > 0)
        {
            PlayNextInQueue();
        }
    }

    public void PlayBackgroundMusic(string name)
    {
        if (audioClips.TryGetValue(name, out AudioClip clip))
        {
            backgroundMusicSource.clip = clip;
            backgroundMusicSource.Play();
            isMusicPlaying = true;
            StartCoroutine(WaitForMusicToEnd());
        }
        else
        {
            Debug.LogWarning("Background music clip not found: " + name);
        }
    }

    public void QueueBackgroundMusic(string name)
    {
        if (audioClips.ContainsKey(name))
        {
            musicQueue.Enqueue(name);
            if (!isMusicPlaying)
            {
                PlayNextInQueue();
            }
        }
        else
        {
            Debug.LogWarning("Music clip not found: " + name);
        }
    }

    private void PlayNextInQueue()
    {
        if (musicQueue.Count > 0)
        {
            string nextMusic = musicQueue.Dequeue();
            PlayBackgroundMusic(nextMusic);
            musicQueue.Enqueue(nextMusic);
        }
        else
        {
            isMusicPlaying = false;
        }
    }

    private IEnumerator WaitForMusicToEnd()
    {
        while (backgroundMusicSource.isPlaying)
        {
            yield return null;
        }

        PlayNextInQueue();
    }

    public void PlaySFX(string name)
    {
        PlaySFX(name, 1f);
    }

    public void PlaySFX(string name, float volume)
    {
        if (audioClips.TryGetValue(name, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip, volume);
        }
        else
        {
            Debug.LogWarning("SFX clip not found: " + name);
        }
    }

    public void PlaySFXRandomTiming(string name, float baseVolume = 1f) {
        StartCoroutine(PlaySFXRandomTimingCoroutines(name, baseVolume));
    }

    private IEnumerator PlaySFXRandomTimingCoroutines(string name, float baseVolume) {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.3f));
            PlaySFX(name, baseVolume);
    }
public void PlaySFX(string name, float baseVolume = 1f, Vector3? position = null)
{
    if (audioClips.TryGetValue(name, out AudioClip clip))
    {
        float adjustedVolume = baseVolume;

        if (position.HasValue && Camera.main != null)
        {
            float distance = Vector3.Distance(position.Value, Camera.main.transform.position);
            adjustedVolume *= Mathf.Clamp01(1 / (distance + 1));
        }

        if (position.HasValue)
        {
            AudioSource.PlayClipAtPoint(clip, position.Value, adjustedVolume);
        }
        else
        {
            sfxSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            sfxSource.PlayOneShot(clip, adjustedVolume);
        }
    }
    else
    {
        Debug.LogWarning("SFX clip not found: " + name);
    }
}



}
