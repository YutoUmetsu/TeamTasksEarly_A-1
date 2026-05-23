using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    [SerializeField] GameObject fallPrefab;
    [SerializeField] int boardSize;
    Vector2 startPos;
    FallObject[,] fallObjects;
    [SerializeField] float fallTime;
    float objHalfWidth;

    enum GameState
    {
        None = 0,
        Idle,
        Select,
        Fall,
        Max,
    }

    GameState nowState = GameState.Select;
    Action[] ControllerUpdate;
    Action[] OnExit;

    [System.NonSerialized] public bool isBombPlacedThisFrame = false;
    private BombSet bombSet;

    void Start()
    {
        startPos = transform.position;
        bombSet = UnityEngine.Object.FindFirstObjectByType<BombSet>();

        GenerateBoard();
        ControllerUpdate = new Action[(int)GameState.Max];
        OnExit = new Action[(int)GameState.Max];
        ControllerUpdate[(int)GameState.Select] += SelectUpdate;
        OnExit[(int)GameState.Select] += CalculateFallTargets;
        ControllerUpdate[(int)GameState.Fall] += FallUpdate;
        OnExit[(int)GameState.Fall] += Delete;
        objHalfWidth = fallPrefab.transform.localScale.x / 2;
        float pos = boardSize / 2f - objHalfWidth;
    }

    void GenerateBoard()
    {
        fallObjects = new FallObject[boardSize, boardSize];
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                Vector2 spawnPos = startPos + new Vector2(x, y);

                GameObject g = Instantiate(fallPrefab, spawnPos, Quaternion.identity);
                fallObjects[x, y] = g.GetComponent<FallObject>();

                // 引数を2つ（状態、初期のYマス目）渡す形に連動させました
                fallObjects[x, y].StartUpFallObject(BlockState.Normal, y);

                if (bombSet != null)
                {
                    bombSet.RegisterTarget(g);
                }
            }
        }
    }

    void Update()
    {
        ControllerUpdate[(int)nowState]();
    }

    void SelectUpdate()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            OnExit[(int)nowState]?.Invoke();
        }
    }

    public void PerformBlockDelete(FallObject targetObj)
    {
        if (nowState != GameState.Select || targetObj == null) return;

        SpawnCoinImmediate(targetObj);
        targetObj.SetDelete();
        OnExit[(int)nowState]?.Invoke();
    }

    public void TriggerExplosionFall()
    {
        if (nowState == GameState.Select)
        {
            StartCoroutine(WaitAndExecuteExit());
        }
    }

    private System.Collections.IEnumerator WaitAndExecuteExit()
    {
        yield return new WaitForFixedUpdate();
        yield return null;

        if (nowState == GameState.Select)
        {
            OnExit[(int)nowState]?.Invoke();
        }
    }

    void CalculateFallTargets()
    {
        for (int x = 0; x < boardSize; x++)
        {
            int emptyCount = 0;

            for (int y = 0; y < boardSize; y++)
            {
                if (fallObjects[x, y] == null)
                {
                    emptyCount++;
                    continue;
                }

                if (fallObjects[x, y].state == BlockState.Delete || fallObjects[x, y].state == BlockState.Empty)
                {
                    emptyCount++;
                }
                else
                {
                    if (emptyCount > 0)
                    {
                        Vector2 targetY = new Vector2(0, startPos.y + y - emptyCount);
                        int targetGridY = y - emptyCount;

                        // 修正された引数2つ版の SetFall を呼び出す
                        fallObjects[x, y].SetFall(targetY.y, targetGridY);
                    }
                    else
                    {
                        fallObjects[x, y].SetFall(startPos.y + y, y);
                    }
                }
            }
        }
        nowState = GameState.Fall;
    }

    void FallUpdate()
    {
        bool anyBlockMoving = false;

        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (fallObjects[x, y] == null) continue;
                if (fallObjects[x, y].state == BlockState.Delete || fallObjects[x, y].state == BlockState.Empty) continue;

                if (fallObjects[x, y].isFall)
                {
                    anyBlockMoving = true;
                    fallObjects[x, y].Fall(0, fallTime);
                }
            }
        }

        if (!anyBlockMoving)
        {
            OnExit[(int)nowState]?.Invoke();
        }
    }

    public bool IsInSelectState()
    {
        return nowState == GameState.Select;
    }

    void Delete()
    {
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (fallObjects[x, y] == null) continue;

                if (fallObjects[x, y].state == BlockState.Delete)
                {
                    fallObjects[x, y].EmptySelf();
                    Destroy(fallObjects[x, y].gameObject);
                    fallObjects[x, y] = null;
                }
            }
        }

        FallObject[,] nextGrid = new FallObject[boardSize, boardSize];

        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (fallObjects[x, y] == null) continue;

                // 引き算を消去！純粋なインデックスで配列を再構築します
                int trueY = fallObjects[x, y].PosY();

                if (trueY >= 0 && trueY < boardSize)
                {
                    nextGrid[x, trueY] = fallObjects[x, y];
                }
            }
        }

        fallObjects = nextGrid;
        nowState = GameState.Select;
    }

    public void SpawnCoinImmediate(FallObject targetObj)
    {
        if (targetObj == null) return;

        DestructibleBlock db = targetObj.GetComponent<DestructibleBlock>();
        if (db != null)
        {
            if (CoinManager.Instance != null)
            {
                CoinManager.Instance.AddCoin(db.coinRewardAmount);
            }

            if (db.coinVisualPrefab != null)
            {
                GameObject spawnedCoin = Instantiate(db.coinVisualPrefab, db.transform.position, Quaternion.identity);
                Rigidbody2D coinRb = spawnedCoin.GetComponent<Rigidbody2D>();
                if (coinRb != null)
                {
                    float randomX = UnityEngine.Random.Range(-db.sideForce, db.sideForce);
                    Vector2 launchVelocity = new Vector2(randomX, db.upwardForce);
                    coinRb.linearVelocity = launchVelocity;
                }
            }
        }
    }
}