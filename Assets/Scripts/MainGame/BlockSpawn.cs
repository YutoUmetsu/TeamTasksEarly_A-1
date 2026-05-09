using System.Data;
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
    public int stageRange = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        blockInfo = new BlockInfomation[stageRange, stageRange];
        //spawnVec = spawnPoint.transform.position;
        spawnVec = new Vector2(spawnPoint.transform.position.x - (blockSize * stageRange / 2), spawnPoint.transform.position.y + (blockSize * stageRange / 2));
        for (int i = 0; i < stageRange; i++) 
        { 
            for (int j = 0; j < stageRange; j++)
            {
                GameObject go = Instantiate(Block);
                go.transform.position = new Vector2(spawnVec.x + i * blockSize, spawnVec.y - j * blockSize);
                go.GetComponent<BlockFall>().blockSize = blockSize;
                go.GetComponent<BlockFall>().myX = i;
                go.GetComponent<BlockFall>().myY = j;
                go.GetComponent<BlockFall>().spawnPoint = spawnPoint;
                go.GetComponent<BlockFall>().blockSpawn = GetComponent<BlockSpawn>();
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
        for (int i = stageRange - 1; i >= 0; i--)
        {
            for (int j = stageRange - 1; j >= 0; j--)
            {
                if (blockInfo[i, j].blocks==Blocks.deleted)
                {
                    Destroy(blockInfo[i,j].blockObj);
                    blockInfo[i, j].blockObj = null;
                }
             
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            // 1.マウス座標をワールド座標に変換
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 2. その位置に重なっている2Dコライダーを検知 (方向はVector2.zeroでOK)
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            // 3. ヒットしたか判定
            if (hit.collider != null)
            {
                GameObject target = hit.collider.gameObject;
                blockInfo[target.GetComponent<BlockFall>().myX, target.GetComponent<BlockFall>().myY].blocks = Blocks.deleted;
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

