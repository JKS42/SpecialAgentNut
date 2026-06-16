using UnityEngine;

public class bullet : MonoBehaviour
{
    private float timer = 3f;
    [SerializeField] private int damage = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Timer();
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDamagePlayer(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryDamagePlayer(collision.collider);
    }

    private void TryDamagePlayer(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        PlayerRespawn player = other.GetComponentInParent<PlayerRespawn>();
        if (player != null)
        {
            player.TakeDamage(damage);
        }

        Destroy(gameObject);
    }

    private void Timer()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }
}
