using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerRespawn player = other.GetComponent<PlayerRespawn>();

            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }
}
