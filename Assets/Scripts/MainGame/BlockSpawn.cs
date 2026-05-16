using System.Data;
using NUnit.Framework;
using Unity.Android.Gradle;
using UnityEngine;
using UnityEngine.Rendering;
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

    // BombSetをインスペクターから繋ぐ用 
    [Header("BombSetオブジェクトをここに")]
    public BombSet bombSet;

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
                // 生成したブロックをBombSetのターゲットに自動登録
                if (bombSet != null)
                {
                    bombSet.RegisterTarget(go);
                }
                go.transform.position = new Vector2(spawnVec.x + i * blockSize, spawnVec.y - j * blockSize);
                go.GetComponent<BlockFall>().blockSize = blockSize;
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
        //for (int x = stageRange - 1; x >= 0; x--)
        //{
        //    for (int y = stageRange - 1; y >= 0; y--)
        //    {
        //        // まだオブジェクトが存在していて、かつdeletedの場合のみDestroyする
        //        if (blockInfo[x, y].blockObj != null && blockInfo[x, y].blocks == Blocks.deleted)
        //        {
        //            Destroy(blockInfo[x, y].blockObj);
        //            blockInfo[x, y].blockObj = null;
        //            blockInfo[x,y].blocks= Blocks.none;
        //            Fall(x, y);
        //        }
        //        else if (blockInfo[x, y].blocks == Blocks.none)
        //        {
        //            Fall(x, y);
        //        }
        //    }
        //}

        for (int x = 0; x < stageRange; x++)
        {
            for (int y = 0; y < stageRange; y++)
            {
                // まだオブジェクトが存在していて、かつdeletedの場合のみDestroyする
                if (blockInfo[x, y].blockObj != null && blockInfo[x, y].blocks == Blocks.deleted)
                {
                    Destroy(blockInfo[x, y].blockObj);
                    blockInfo[x, y].blockObj = null;
                    blockInfo[x, y].blocks = Blocks.none;
                }

                if (blockInfo[x, y].blocks == Blocks.none)
                {
                    Fall(x, y);
                }
            }
        }
    }

    void Fall(int x, int y)
    {
        if (y == 0) return;
        //if (blockInfo[x, y - 1].blocks == Blocks.deleted) return;
        blockInfo[x, y] = blockInfo[x, y - 1];
        blockInfo[x, y - 1].blocks = Blocks.none;
        blockInfo[x, y - 1].blockObj = null;
    }
}

public class BlockInfomation
{
    public int posX;
    public int posY;
    public Blocks blocks;
    public GameObject blockObj;
}

