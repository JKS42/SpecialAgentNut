using UnityEngine;

public class CustomHashMap
{
    private HashNode[] buckets;
    private int size;

    public CustomHashMap(int size)
    {
        this.size = size;
        buckets = new HashNode[size];
    }

    private int Hash(string key)
    {
        int hash = 0;

        foreach (char c in key)
        {
            hash += c;
        }

        return Mathf.Abs(hash % size);
    }
    public void Insert(string key, AudioClip value)
    {
        int index = Hash(key);

        HashMapEntry entry = new HashMapEntry(key, value);
        HashNode newNode = new HashNode(entry);

        if (buckets[index] == null)
        {
            buckets[index] = newNode;
            return;
        }

        HashNode current = buckets[index];

        while (current.Next != null)
        {
            if (current.Entry.Key == key)
            {
                current.Entry.Value = value;
                return;
            }

            current = current.Next;
        }

        current.Next = newNode;
    }
    public AudioClip Get(string key)
    {
        int index = Hash(key);

        HashNode current = buckets[index];

        while (current != null)
        {
            if (current.Entry.Key == key)
            {
                return current.Entry.Value;
            }

            current = current.Next;
        }

        return null;
    }
    public bool ContainsKey(string key)
    {
        return Get(key) != null;
    }
    public void Remove(string key)
    {
        int index = Hash(key);

        HashNode current = buckets[index];
        HashNode previous = null;

        while (current != null)
        {
            if (current.Entry.Key == key)
            {
                if (previous == null)
                {
                    buckets[index] = current.Next;
                }
                else
                {
                    previous.Next = current.Next;
                }

                return;
            }

            previous = current;
            current = current.Next;
        }
    }
}

