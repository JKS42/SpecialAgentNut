using System;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public string playerTag = "Player";
    private bool isActive = false;
    [SerializeField] ParticleSystem checkpointEffect;
    [SerializeField] PlayerRespawn resetTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(playerTag) && !isActive)
        {
            resetTrigger.SetRespawnPoint(transform.position);
            PlayVFX();
            isActive = true;
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

