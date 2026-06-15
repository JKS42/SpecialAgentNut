using UnityEngine;

public class HashNode
{
    public HashMapEntry Entry;
    public HashNode Next;

    public HashNode(HashMapEntry entry)
    {
        Entry = entry;
        Next = null;
    }
}
