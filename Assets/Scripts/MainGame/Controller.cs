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

    [Header("爆弾補充の設定")]
    [SerializeField] System.Collections.Generic.List<GameObject> bombPrefabs; // 爆弾プレハブのリスト
    [SerializeField] int bombSpawnCountPerTurn = 1; // 1ターンに落とす爆弾の固定数


    [Header("確率で降ってくる新しいブロックの設定")]
    [SerializeField] System.Collections.Generic.List<GameObject> rareBlockPrefabs; // 後ろにあるほど低確率
    [Range(0f, 100f)]
    [SerializeField] float rareBlockSpawnChance = 30f; // 通常以外のブロックのスポーン率

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
        // --- 1. 今回のターンで「全体で何個のブロックが補充されるか」をあらかじめ計算する ---
        int totalNewBlocksCount = 0;
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (fallObjects[x, y] == null || fallObjects[x, y].state == BlockState.Delete || fallObjects[x, y].state == BlockState.Empty)
                {
                    totalNewBlocksCount++;
                }
            }
        }

        // --- 2. 爆弾を落とす場所（当選インデックス）をランダムに決定する ---
        System.Collections.Generic.HashSet<int> bombSelectionIndices = new System.Collections.Generic.HashSet<int>();

        // インスペクターの基本設定値に、新しいマネージャーのボーナス値を加算する
        int currentTurnBombCount = bombSpawnCountPerTurn;
        if (BombSpawnManager.Instance != null)
        {
            currentTurnBombCount += BombSpawnManager.Instance.bonusSpawnBombs;
        }

        // 補充総数より設定された爆弾数(n)が多い場合は、補充総数を上限にする
        int actualBombCountToSpawn = Mathf.Min(currentTurnBombCount, totalNewBlocksCount);
       
        // 重複のないランダムな「くじ（0 ～ 補充総数-1）」を作成
        while (bombSelectionIndices.Count < actualBombCountToSpawn)
        {
            int randomIndex = UnityEngine.Random.Range(0, totalNewBlocksCount);
            bombSelectionIndices.Add(randomIndex);
        }

        // 全体の補充ブロックに通し番号をつけるためのカウンター
        int currentSpawnGlobalIndex = 0;


        // --- 3. ここから各列の落下・補充のメイン計算 ---
        for (int x = 0; x < boardSize; x++)
        {
            int emptyCount = 0;

            // 既存ブロックの落下ターゲット計算
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
                        fallObjects[x, y].SetFall(targetY.y, targetGridY);
                    }
                    else
                    {
                        fallObjects[x, y].SetFall(startPos.y + y, y);
                    }
                }
            }

            // この列に空き地（emptyCount）があれば上から補充
            if (emptyCount > 0)
            {
                int arrayYPointer = 0;

                for (int i = 0; i < emptyCount; i++)
                {
                    int spawnYIndex = boardSize + i;
                    Vector2 spawnPos = startPos + new Vector2(x, spawnYIndex);

                    // ─── 【重要：ここで通常ブロックか爆弾かを判定！】 ───
                    GameObject prefabToSpawn = fallPrefab; // 基本は通常ブロック

                    // もし今回の通し番号が「爆弾の当選くじ」に含まれていて、かつ爆弾リストに中身があれば
                    if (bombSelectionIndices.Contains(currentSpawnGlobalIndex) && bombPrefabs != null && bombPrefabs.Count > 0)
                    {
                        // 爆弾リストの中からランダムに1種類選ぶ
                        int randomBombType = UnityEngine.Random.Range(0, bombPrefabs.Count);
                        if (bombPrefabs[randomBombType] != null)
                        {
                            prefabToSpawn = bombPrefabs[randomBombType];
                        }
                    }
                    // 爆弾じゃなかった場合、確率で新しいブロックを抽選する
                    else if (rareBlockPrefabs != null && rareBlockPrefabs.Count > 0)
                    {
                        if (UnityEngine.Random.Range(0f, 100f) < rareBlockSpawnChance)
                        {
                            prefabToSpawn = GetRandomBlockWithWeight(rareBlockPrefabs);
                        }
                    }
                    // 通し番号を進める
                    currentSpawnGlobalIndex++;

                    // 決定したプレハブ（通常 or 爆弾）を生成！
                    GameObject newBlock = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
                    FallObject fallObj = newBlock.GetComponent<FallObject>();

                    int targetGridY = boardSize - emptyCount + i;
                    Vector2 targetWorldY = startPos + new Vector2(x, targetGridY);

                    fallObj.StartUpFallObject(BlockState.Normal, spawnYIndex);
                    fallObj.SetFall(targetWorldY.y, targetGridY);

                    if (bombSet != null) bombSet.RegisterTarget(newBlock);

                    // 配列への一時キープ
                    while (arrayYPointer < boardSize)
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

    /// <summary>
    /// リストの後ろに登録されているオブジェクトほど確率が低くなるように抽選する関数
    /// </summary>
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
}