using UnityEngine;

public class BlockFall : MonoBehaviour
{
    public float fallSpead = -1f;
    bool hitStage = false;
    public int myX;
    public int myY;
    public float blockSize;
    GameObject fallpoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       fallpoint = GameObject.Find("fallPoint");
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y > fallpoint.transform.position.y + (myY * blockSize))
        {
            transform.position += new Vector3(0, fallSpead);
        }
        else if (transform.position.y < fallpoint.transform.position.y + (myY * blockSize))
        {
            transform.position = new Vector2(transform.position.x, fallpoint.transform.position.y + (myY * blockSize));
        }
    }
}
