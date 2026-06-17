using UnityEngine;

public class bullet : MonoBehaviour
{
    private enum DamageTarget
    {
        Player,
        Enemies
    }

    private float timer = 3f;
    [SerializeField] private int damage = 1;
    [SerializeField] private DamageTarget damageTarget = DamageTarget.Player;

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
        TryDamageTarget(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryDamageTarget(collision.collider);
    }

    public void ConfigureForEnemies(int shotDamage)
    {
        damage = shotDamage;
        damageTarget = DamageTarget.Enemies;
    }

    private void TryDamageTarget(Collider other)
    {
        if (damageTarget == DamageTarget.Player)
        {
            TryDamagePlayer(other);
            return;
        }

        TryDamageEnemy(other);
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

    private void TryDamageEnemy(Collider other)
    {
        CloseEnemy closeEnemy = other.GetComponentInParent<CloseEnemy>();
        if (closeEnemy != null)
        {
            closeEnemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        LongEnemy longEnemy = other.GetComponentInParent<LongEnemy>();
        if (longEnemy != null)
        {
            longEnemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        EnemyPatrol enemyPatrol = other.GetComponentInParent<EnemyPatrol>();
        if (enemyPatrol != null)
        {
            enemyPatrol.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        BossBehavior boss = other.GetComponentInParent<BossBehavior>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            Destroy(gameObject);
        }
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
