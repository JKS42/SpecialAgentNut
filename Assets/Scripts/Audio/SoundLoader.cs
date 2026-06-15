using UnityEngine;

public class SoundLoader : MonoBehaviour
{
    public AudioClip footstep;
    public AudioClip enemyAttack;
    public AudioClip platformMove;
    public AudioClip pickup;
    public AudioClip death;

    private void Start()
    {
        SFXManager.Instance.AddSound("Footstep", footstep);
        SFXManager.Instance.AddSound("EnemyAttack", enemyAttack);
        SFXManager.Instance.AddSound("PlatformMove", platformMove);
        SFXManager.Instance.AddSound("Pickup", pickup);
        SFXManager.Instance.AddSound("Death", death);
    }
}
