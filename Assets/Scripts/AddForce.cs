using UnityEngine;

public class AddForce : MonoBehaviour
{
    [SerializeField] private Transform floatingPlatform;
    [SerializeField] private float lerpSpeed = 5f;

    private Transform player;
    private float verticalOffset;

    private void Reset()
    {
        floatingPlatform = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        player = other.transform;

        if (floatingPlatform == null)
        {
            floatingPlatform = transform;
        }

        verticalOffset = player.position.y - floatingPlatform.position.y;
    }

    private void OnTriggerStay(Collider other)
    {
        if (player == null || other.transform != player) return;

        LerpPlayerUpWithPlatform();
    }

    private void OnTriggerExit(Collider other)
    {
        if (player == null || other.transform != player) return;

        player = null;
    }

    private void LerpPlayerUpWithPlatform()
    {
        if (floatingPlatform == null || player == null) return;

        Vector3 currentPosition = player.position;
        Vector3 targetPosition = new Vector3(
            currentPosition.x,
            floatingPlatform.position.y + verticalOffset,
            currentPosition.z
        );

        player.position = Vector3.Lerp(currentPosition, targetPosition, lerpSpeed * Time.deltaTime);
    }
}
