using UnityEngine;

public class BlockSpawn : MonoBehaviour
{
    enum Blocks
    {
        none=0,
        nomalBlock=1,
        deleted=99
    };
    Blocks[,] Stage;
    public GameObject spawnPoint;
    public Vector2 spawnVec;
    float blockSize = 1;
    public GameObject Block;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Stage = new Blocks[6, 6];
        spawnVec = spawnPoint.transform.position;
        for (int i = 0; i < 6; i++) 
        { 
            for (int j = 0; j < 6; j++)
            {
                GameObject go = Instantiate(Block);
                go.transform.position = new Vector2(spawnVec.x + i * blockSize, spawnVec.y - j * blockSize);
                go.GetComponent<BlockFall>().myX = i;
                go.GetComponent<BlockFall>().myY = j;
                go.GetComponent<BlockFall>().blockSize = blockSize;
                Stage[i, j] = Blocks.nomalBlock;
            }

        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
