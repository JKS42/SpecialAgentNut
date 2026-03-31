using UnityEngine;

public class HeartPickup : MonoBehaviour
{
    public float rotationSpeed = 100f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
