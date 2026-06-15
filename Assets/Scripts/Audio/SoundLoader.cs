using UnityEngine;

public class SoundLoader : MonoBehaviour
{
    public AudioClip footstep;
    public AudioClip enemyAttack;
    public AudioClip platformMove;

    private void Start()
    {
        SFXManager.Instance.AddSound("Footstep", footstep);
        SFXManager.Instance.AddSound("EnemyAttack", enemyAttack);
        SFXManager.Instance.AddSound("PlatformMove", platformMove);
    }
}
