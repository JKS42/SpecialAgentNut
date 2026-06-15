using UnityEngine;

public class HashMapEntry
{
    public string Key;
    public AudioClip Value;

    public HashMapEntry(string key, AudioClip value)
    {
        Key = key;
        Value = value;
    }
}
