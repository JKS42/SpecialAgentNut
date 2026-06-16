using UnityEngine;

public class SoundLoader : MonoBehaviour
{
    public AudioClip footstep;
    public AudioClip enemyAttack;
    public AudioClip pickupHeart;
    public AudioClip pickupCoin;
    public AudioClip death;
    public AudioClip checkpoint;

    private void Start()
    {
        SFXManager.Instance.AddSound("Footstep", footstep);
        SFXManager.Instance.AddSound("EnemyAttack", enemyAttack);
        SFXManager.Instance.AddSound("PickupHeart", pickupHeart);
        SFXManager.Instance.AddSound("PickupCoin", pickupCoin); 
        SFXManager.Instance.AddSound("Death", death);
        SFXManager.Instance.AddSound("Checkpoint", checkpoint);
    }
}
