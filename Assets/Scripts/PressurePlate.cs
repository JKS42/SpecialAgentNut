
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private TriggerPlatform platform;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            platform.ActivatePlatform();
        }
    }
}