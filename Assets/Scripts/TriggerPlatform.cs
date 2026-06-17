using UnityEngine;

public class TriggerPlatform : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float moveSpeed = 2f;

    private bool shouldMove = false;

    public void ActivatePlatform()
    {
        shouldMove = true;
    }
    public void ActivatePlatformBack()
    {
        shouldMove = true;
    }

    void Update()
    {
        if (!shouldMove) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            pointB.position,
            moveSpeed * Time.deltaTime
        );

        if(transform.position == pointB.position)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                pointA.position,
                moveSpeed * Time.deltaTime
            );
        }
    }
}
