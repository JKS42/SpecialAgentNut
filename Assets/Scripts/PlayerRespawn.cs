using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public float thresholdY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(transform.position.y < thresholdY)
        {
            transform.position = new Vector3(1.76f, 6.3f, -5.01f);
        }
    }
}
