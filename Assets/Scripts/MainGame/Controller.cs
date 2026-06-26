using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    [SerializeField] GameObject fallPrefab;

    [SerializeField] int baseBoardSize; // インスペクターでの基本サイズ（例: 5）
    int currentBoardSize; // スキル分を足した実際のサイズ
    Vector2 startPos;
    FallObject[,] fallObjects;
    [SerializeField] float fallTime;
    float objHalfWidth;

    [Header("爆弾補充の設定")]
    [SerializeField] System.Collections.Generic.List<GameObject> bombPrefabs;
    [SerializeField] float bomdSpawnParcent = 15f;

    [Header("確率で降ってくる新しいブロックの設定")]
    [SerializeField] System.Collections.Generic.List<GameObject> rareBlockPrefabs;
    [Range(0f, 100f)]
    [SerializeField] float rareBlockSpawnChance = 30f;

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

    [System.NonSerialized] public bool isBoardSettledThisFrame = false;

    private int activeCoinCount = 0;

    void Start()
    {
        startPos = transform.position;
        bombSet = UnityEngine.Object.FindFirstObjectByType<BombSet>();

        // ─── ★【追加：スキルツリーのボーナスを基本サイズに加算】───
        currentBoardSize = baseBoardSize;
        if (BoardSizeManager.Instance != null)
        {
            currentBoardSize += BoardSizeManager.Instance.bonusBoardSize;
        }

        GenerateBoard();
        ControllerUpdate = new Action[(int)GameState.Max];
        OnExit = new Action[(int)GameState.Max];
        ControllerUpdate[(int)GameState.Select] += SelectUpdate;
        OnExit[(int)GameState.Select] += CalculateFallTargets;
        ControllerUpdate[(int)GameState.Fall] += FallUpdate;
        OnExit[(int)GameState.Fall] += Delete;
        objHalfWidth = fallPrefab.transform.localScale.x / 2;
        float pos = currentBoardSize / 2f - objHalfWidth;
    }

    void GenerateBoard()
    {
        // currentBoardSize を使って配列を確保
        fallObjects = new FallObject[currentBoardSize, currentBoardSize];
        for (int x = 0; x < currentBoardSize; x++)
        {
            for (int y = 0; y < currentBoardSize; y++)
            {
                Vector2 spawnPos = startPos + new Vector2(x, y);

                GameObject g = Instantiate(fallPrefab, spawnPos, Quaternion.identity);
                fallObjects[x, y] = g.GetComponent<FallObject>();
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

        float delay = 0.5f;
        Bomb activeBomb = UnityEngine.Object.FindFirstObjectByType<Bomb>();
        if (activeBomb != null)
        {
            delay = activeBomb.destroyDelay;
        }

        yield return new WaitForSeconds(delay);

        if (nowState == GameState.Select)
        {
            OnExit[(int)nowState]?.Invoke();
        }
    }

    void CalculateFallTargets()
    {
        float finalSpawnPercent = bomdSpawnParcent;
        if (BombSpawnManager.Instance != null)
        {
            finalSpawnPercent += BombSpawnManager.Instance.bonusBombPercent;
        }

        for (int x = 0; x < currentBoardSize; x++)
        {
            int emptyCount = 0;

            for (int y = 0; y < currentBoardSize; y++)
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
                        fallObjects[x, y].SetFall(targetY.y, targetGridY);
                    }
                    else
                    {
                        fallObjects[x, y].SetFall(startPos.y + y, y);
                    }
                }
            }

            if (emptyCount > 0)
            {
                int arrayYPointer = 0;

                for (int i = 0; i < emptyCount; i++)
                {
                    int spawnYIndex = currentBoardSize + i;
                    Vector2 spawnPos = startPos + new Vector2(x, spawnYIndex);

                    GameObject prefabToSpawn = fallPrefab;

                    if (bombPrefabs != null && bombPrefabs.Count > 0 && UnityEngine.Random.Range(0f, 100f) < finalSpawnPercent)
                    {
                        int randomBombType = UnityEngine.Random.Range(0, bombPrefabs.Count);
                        if (bombPrefabs[randomBombType] != null)
                        {
                            prefabToSpawn = bombPrefabs[randomBombType];
                        }
                    }
                    else if (rareBlockPrefabs != null && rareBlockPrefabs.Count > 0)
                    {
                        if (UnityEngine.Random.Range(0f, 100f) < rareBlockSpawnChance)
                        {
                            prefabToSpawn = GetRandomBlockWithWeight(rareBlockPrefabs);
                        }
                    }

                    GameObject newBlock = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
                    FallObject fallObj = newBlock.GetComponent<FallObject>();

                    int targetGridY = currentBoardSize - emptyCount + i;
                    Vector2 targetWorldY = startPos + new Vector2(x, targetGridY);

                    fallObj.StartUpFallObject(BlockState.Normal, spawnYIndex);
                    fallObj.SetFall(targetWorldY.y, targetGridY);

                    if (bombSet != null) bombSet.RegisterTarget(newBlock);

                    while (arrayYPointer < currentBoardSize)
                    {
                        if (fallObjects[x, arrayYPointer] == null ||
                            fallObjects[x, arrayYPointer].state == BlockState.Delete ||
                            fallObjects[x, arrayYPointer].state == BlockState.Empty)
                        {
                            fallObjects[x, arrayYPointer] = fallObj;
                            arrayYPointer++;
                            break;
                        }
                        arrayYPointer++;
                    }
                }
            }
        }

        nowState = GameState.Fall;
    }

    void FallUpdate()
    {
        if (activeCoinCount > 0)
        {
            return;
        }

        bool anyBlockMoving = false;

        for (int x = 0; x < currentBoardSize; x++)
        {
            for (int y = 0; y < currentBoardSize; y++)
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
        for (int x = 0; x < currentBoardSize; x++)
        {
            for (int y = 0; y < currentBoardSize; y++)
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

        FallObject[,] nextGrid = new FallObject[currentBoardSize, currentBoardSize];

        for (int x = 0; x < currentBoardSize; x++)
        {
            for (int y = 0; y < currentBoardSize; y++)
            {
                if (fallObjects[x, y] == null) continue;

                int trueY = fallObjects[x, y].PosY();

                if (trueY >= 0 && trueY < currentBoardSize)
                {
                    nextGrid[x, trueY] = fallObjects[x, y];
                }
            }
        }

        fallObjects = nextGrid;
        nowState = GameState.Select;

        isBoardSettledThisFrame = true;
    }

    // ─── ★【5枚コイン対応 ＆ コインボーナススキル連動版】★ ───
    public void SpawnCoinImmediate(FallObject targetObj)
    {
        if (targetObj == null) return;

        DestructibleBlock db = targetObj.GetComponent<DestructibleBlock>();
        if (db != null)
        {
            // ベースの枚数を取得
            int totalAmount = db.coinRewardAmount;

            // ★スキルによる追加コインボーナスを加算
            if (CoinBonusManager.Instance != null)
            {
                totalAmount += CoinBonusManager.Instance.bonusCoinAmount;
            }

            // CoinManager（データ側）に最終獲得枚数を一気に加算
            if (CoinManager.Instance != null)
            {
                CoinManager.Instance.AddCoin(totalAmount);
            }

            int count5x = 0;
            int count1x = 0;

            // 5枚コインのプレハブが設定されている場合のみ割り算
            if (db.coinVisualPrefab5x != null)
            {
                count5x = totalAmount / 5;
                count1x = totalAmount % 5;
            }
            else
            {
                count1x = totalAmount;
            }

            // 5枚コイン生成
            for (int i = 0; i < count5x; i++)
            {
                CreateCoinObject(db.coinVisualPrefab5x, db);
            }

            // 1枚コイン生成
            for (int i = 0; i < count1x; i++)
            {
                if (db.coinVisualPrefab != null)
                {
                    CreateCoinObject(db.coinVisualPrefab, db);
                }
            }
        }
    }

    private void CreateCoinObject(GameObject prefab, DestructibleBlock db)
    {
        if (prefab == null) return;

        GameObject spawnedCoin = Instantiate(prefab, db.transform.position, Quaternion.identity);

        CoinDestroyNotifier notifier = spawnedCoin.AddComponent<CoinDestroyNotifier>();
        notifier.Setup(this);

        activeCoinCount++;

        Rigidbody2D coinRb = spawnedCoin.GetComponent<Rigidbody2D>();
        if (coinRb != null)
        {
            float randomX = UnityEngine.Random.Range(-db.sideForce, db.sideForce);
            float randomY = db.upwardForce * UnityEngine.Random.Range(0.8f, 1.2f);

            Vector2 launchVelocity = new Vector2(randomX, randomY);
            coinRb.linearVelocity = launchVelocity;
        }
    }

    private GameObject GetRandomBlockWithWeight(System.Collections.Generic.List<GameObject> blockList)
    {
        if (blockList == null || blockList.Count == 0) return fallPrefab;

        int totalWeight = 0;
        for (int i = 0; i < blockList.Count; i++)
        {
            totalWeight += (blockList.Count - i);
        }

        int randomValue = UnityEngine.Random.Range(0, totalWeight);
        int currentWeightSum = 0;

        for (int i = 0; i < blockList.Count; i++)
        {
            currentWeightSum += (blockList.Count - i);
            if (randomValue < currentWeightSum)
            {
                if (blockList[i] != null) return blockList[i];
                break;
            }
        }

        return fallPrefab;
    }
    // コインが消えた時にカウントを減らす関数
    public void DecrementCoinCount()
    {
        activeCoinCount--;
    }
}