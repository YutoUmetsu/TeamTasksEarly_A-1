using NUnit.Framework;
using Unity.Android.Gradle;
using UnityEngine;
   public enum Blocks
    {
        none=0,
        nomalBlock=1,
        deleted=99
    };

public class BlockSpawn : MonoBehaviour
{
 
    public GameObject spawnPoint;
    public Vector2 spawnVec;
    float blockSize = 1;
    public GameObject Block;
    public static BlockInfomation[,] blockInfo;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        blockInfo = new BlockInfomation[6, 6];
        spawnVec = spawnPoint.transform.position;
        for (int i = 0; i < 6; i++) 
        { 
            for (int j = 0; j < 6; j++)
            {
                GameObject go = Instantiate(Block);
                go.transform.position = new Vector2(spawnVec.x + i * blockSize, spawnVec.y - j * blockSize);
                go.GetComponent<BlockFall>().blockSize = blockSize;
                go.GetComponent<BlockFall>().myX = i;
                go.GetComponent<BlockFall>().myY = j;
                blockInfo[i,j] = new BlockInfomation();
                blockInfo[i, j].posX = i;
                blockInfo[i, j].posY = j;
                blockInfo[i, j].blocks = Blocks.nomalBlock;
                blockInfo[i, j].blockObj = go;

            }

        }

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 5; i >= 0; i--)
        {
            for (int j = 5; j >= 0; j--)
            {
                Debug.Log($"{blockInfo[i, j].posX}{blockInfo[i, j].posY}"  );
            }
        }
    }
}

public class BlockInfomation
{
    public int posX;
    public int posY;
    public Blocks blocks;
    public GameObject blockObj;
}

