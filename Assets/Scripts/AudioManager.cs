using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] clips;

    private AudioSource _source;

    private static AudioManager _audioManager;

    void Start()
    {
        _source = GetComponent<AudioSource>();
    }

    void PlayClip(GameAudioClip clip)
    {
        if (_source.isPlaying)
        {
            _source.Stop();
        }

        var index = (int)clip;
        if (index >= 0 && index < clips.Length)
        {
            _source.clip = clips[index];
            _source.Play();
        }
    }

    public static void Play(GameAudioClip clip)
    {
        if (_audioManager == null)
        {
            _audioManager = GameObject.FindObjectOfType<AudioManager>();
        }

        _audioManager.PlayClip(clip);
    }
}

public enum GameAudioClip
{
    Antidote,
    Bullet,
    Hurt,
    Jump,
    Falling,
    Crash,
    Powerup
}
