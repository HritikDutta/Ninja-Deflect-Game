using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController instance;

    [System.Serializable]
    public struct GameAudioClip
    {
        public AudioClip clip;
        public float volume;
        public bool randomizePitch;
    }

    [Header("Audio Clips")]
    public GameAudioClip knifeStabClip;
    public GameAudioClip swordHitClip;
    public GameAudioClip coinCollectClip;

    private AudioSource audioSource;

    private void Awake()
    {
        // Only one instance allowed >:(
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        audioSource = GetComponent<AudioSource>();
    }

    public static void PlayAudioClipOneShot(GameAudioClip clip)
    {
        float pitch = 1f;
        if (clip.randomizePitch)
            pitch += Random.Range(-0.1f, 0.1f);

        instance.audioSource.pitch = pitch;
        instance.audioSource.volume = clip.volume;
        instance.audioSource.PlayOneShot(clip.clip);
    }
}
