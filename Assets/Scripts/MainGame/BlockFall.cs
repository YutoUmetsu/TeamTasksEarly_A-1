using UnityEngine;

public class BlockFall : MonoBehaviour
{
    public BlockSpawn blockSpawn;
    public float fallSpead = -1f;
    bool hitStage = false;
    public int myX;
    public int myY;
    public float blockSize;
    public GameObject spawnPoint;
    Vector2 fallpoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fallpoint = new Vector2(spawnPoint.transform.position.x, spawnPoint.transform.position.y - (blockSize * blockSpawn.stageRange/2));
    }

    // Update is called once per frame
    void Update()
    {

        Fall();

        if (transform.position.y > fallpoint.y + (myY * blockSize))
        {
            transform.position += new Vector3(0, fallSpead);
        }
        else if (transform.position.y < fallpoint.y + (myY * blockSize))
        {
            transform.position = new Vector2(transform.position.x, fallpoint.y + (myY * blockSize));
        }
    }

    void Fall()
    { 
        for (int x= 0; x < blockSpawn.stageRange; x++)
        {
            for (int y = 0; y < blockSpawn.stageRange; y++)
            {
                if (gameObject == BlockSpawn.blockInfo[x, y].blockObj)
                {
                   myX = BlockSpawn.blockInfo[x, y].posX;
                   myY = BlockSpawn.blockInfo[x, y].posY;
                }
            }
        }

    }
}
