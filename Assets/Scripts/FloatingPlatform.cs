using UnityEngine;

public class FloatingPlatform : MonoBehaviour
{
    [SerializeField] private float height;
    [SerializeField] private Transform[] platformPoints;
    [SerializeField] private float lerpSpeed;

    private float t = 0f;
    private int currentIndex = 0;
    private int targetIndex = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MovePlatform();
    }
    private void MovePlatform()
    {
        if (platformPoints.Length < 2) return;

        t += Time.deltaTime * lerpSpeed;
        transform.position = Vector3.Lerp(platformPoints[currentIndex].position, platformPoints[targetIndex].position, t);

        if (t >= 1f)
        {
            t = 0f;
            int temp = currentIndex;
            currentIndex = targetIndex;
            targetIndex = temp;
        }
    }
}
