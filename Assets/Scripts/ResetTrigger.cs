using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public class ResetTrigger : MonoBehaviour
{
    public GameObject player;
    public GameObject respawnPoint;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.position = respawnPoint.transform.position;
            //Destroy(player);
            //Instantiate(player, respawnPoint.transform.position, Quaternion.identity);
        }
    }
}
