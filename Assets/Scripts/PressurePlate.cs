
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private TriggerPlatform platform;
    public Transform pointA;
    public Transform pointB;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            platform.ActivatePlatform();
        }
        if (other.CompareTag("Player")&& platform.transform.position == pointB.position)
        {
            platform.ActivatePlatformBack();
        }
    }
}