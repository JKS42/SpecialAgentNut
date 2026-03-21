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
        
    }
}

