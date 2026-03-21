using UnityEngine;
using UnityEngine.Events;

public class ResetTrigger : MonoBehaviour
{
    public string playerTag = "Player";
    public Transform startPos;
    private Vector3 SpawnPoint;
    [SerializeField] private new Camera camera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public UnityEvent OnPlayerReset;
    void Start()
    {
        SpawnPoint = startPos.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetSpawnPoint(Vector3 newSpawnPoint)
    {
        SpawnPoint = newSpawnPoint;
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(playerTag))
        {
            other.gameObject.transform.root.position = SpawnPoint;
            camera.transform.position = new Vector3(SpawnPoint.x, camera.transform.position.y, camera.transform.position.z);
            OnPlayerReset.Invoke();
            Rigidbody rb = other.gameObject.transform.root.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
