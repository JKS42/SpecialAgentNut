using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    private CustomHashMap soundMap;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        audioSource = GetComponent<AudioSource>();

        soundMap = new CustomHashMap(20);
    }

    public void AddSound(string name, AudioClip clip)
    {
        soundMap.Insert(name, clip);
    }

    public void PlaySound(string name)
    {
        AudioClip clip = soundMap.Get(name);

        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
