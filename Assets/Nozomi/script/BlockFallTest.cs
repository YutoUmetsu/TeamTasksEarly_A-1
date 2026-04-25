using UnityEngine;

public class BlockFallTest : MonoBehaviour
{
    public float fallSpead = -1.0f;
    bool hitStage = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!hitStage)
        {
            transform.position += new Vector3(0, fallSpead, 0);
        }

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Stage")
        {
            hitStage = true;
        }
    }
}
