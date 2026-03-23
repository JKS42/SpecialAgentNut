using System;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public string playerTag = "Player";
    private bool isActive = false;
    [SerializeField] ParticleSystem checkpointEffect;
    [SerializeField] ResetTrigger resetTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(playerTag) && !isActive)
        {
            if(isActive == true)
            {
                //resetTrigger.SetSpawnPoint(transform.position);
                PlayVFX();
                isActive = true;
            }
        }
    }
    [ContextMenu("Activate Checkpoint")]
    public void PlayVFX()
    {
        if (checkpointEffect != null)
        {
            ParticleSystem vfxinstance = Instantiate(checkpointEffect, transform.position, Quaternion.identity);
            vfxinstance.Play();
            Destroy(vfxinstance.gameObject, vfxinstance.main.duration + vfxinstance.main.startLifetime.constantMax);
        }
    }
}

