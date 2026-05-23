using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    [SerializeField] GameObject fallPrefab;
    [SerializeField] int boardSize;
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

    // ───【追加】インスペクターから登録しなくても動くように、自動登録用（旧BlockSpawnの処理の代わり）
    // もし既存のBombSetがあれば、Startで自動的にブロックを登録します。
    private BombSet bombSet;

    void Start()
    {
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
                GameObject g = Instantiate(fallPrefab, new Vector2(x, y), Quaternion.identity);
                fallObjects[x, y] = g.GetComponent<FallObject>();
                fallObjects[x, y].StartUpFallObject(BlockState.Normal);

                // 追加：新しく生成したブロックをBombSetのターゲットに自動登録
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
        /* スクリプト実行順バグ防止のため、一時的にコメントアウト 
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            int gridX = Mathf.RoundToInt(worldPos.x);
            int gridY = Mathf.RoundToInt(worldPos.y);

            if (gridX >= 0 && gridX < boardSize && gridY >= 0 && gridY < boardSize)
            {
                FallObject clickedObj = fallObjects[gridX, gridY];
                if (clickedObj != null)
                {
                    Bomb bomb = clickedObj.GetComponentInChildren<Bomb>();
                    if (bomb != null) return;

                    SpawnCoinImmediate(clickedObj);
                    clickedObj.SetDelete();
                    OnExit[(int)nowState]?.Invoke();
                }
            }
        }*/

        // スペースキーでのフェーズ遷移用デバッグだけ残しておきます
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            OnExit[(int)nowState]?.Invoke();
        }
    }

    public void PerformBlockDelete(FallObject targetObj)
    {
        // 選択フェーズ中、かつ対象のブロックが存在する場合のみ実行
        if (nowState != GameState.Select || targetObj == null) return;

        // その場で即座にコインを出す
        SpawnCoinImmediate(targetObj);

        // ブロックを消去（Delete）状態にする
        targetObj.SetDelete();
        Debug.Log($"通常ブロック消去: 安全に消去予定にしました");

        // 落下フェーズへ進めるためのイベントを発火
        OnExit[(int)nowState]?.Invoke();
    }

    // 【修正】爆弾から「爆発したよ！」と呼び出される関数
    public void TriggerExplosionFall()
    {
        if (nowState == GameState.Select)
        {
            // 秒数指定のInvokeをやめ、物理演算の同期が確実にとれるコルーチンを開始する
            StartCoroutine(WaitAndExecuteExit());
        }
    }
    private System.Collections.IEnumerator WaitAndExecuteExit()
    {
        // 1. 物理演算（FixedUpdate）が確実に1回計算されるのをじっと待つ
        yield return new WaitForFixedUpdate();

        // 2. その直後のフレームまで待って、判定の処理を完全に完了させる
        yield return null;

        // 3. 爆風の処理が100%終わったので、安全に落下計算へ進む！
        if (nowState == GameState.Select)
        {
            OnExit[(int)nowState]?.Invoke();
        }
    }
    void CalculateFallTargets()
    {
        for (int x = 0; x < boardSize; x++)
        {
            int emptyCount = 0; // その列で「消えるブロック」が何個あるかを数えるカウンター

            for (int y = 0; y < boardSize; y++)
            {
                if (fallObjects[x, y] == null)
                {
                    emptyCount++;
                    continue;
                }

                // もしこのブロックが消える予定（Delete）なら、空き地カウンターを増やす
                if (fallObjects[x, y].state == BlockState.Delete || fallObjects[x, y].state == BlockState.Empty)
                {
                    emptyCount++;
                }
                // 生き残るブロックの場合
                else
                {
                    // もし下に空き地（消えるブロック）が1個以上あったら、その分だけ下に落とす
                    if (emptyCount > 0)
                    {
                        // 現在の y 座標から、見つかった空き地の数（emptyCount）を引く
                        int targetY = y - emptyCount;

                        // FallObject側が「着地先のY座標」を求めている場合は targetY を、
                        // 「何マス下に落ちるか」を求めている場合は emptyCount を渡します。
                        // 今の仕様に合わせて targetY を指定します。
                        fallObjects[x, y].SetFall(targetY);
                    }
                    else
                    {
                        // 下に空き地がない場合は、その場に留まる（落下させない）
                        // FallObjectの落下フラグ(isFall)を立てずに現在の位置をキープさせる
                        fallObjects[x, y].SetFall(y);
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
                // 空っぽの部屋はスルー
                if (fallObjects[x, y] == null) continue;

                // 消去予定（Delete）のものは動かないのでスルー
                if (fallObjects[x, y].state == BlockState.Delete || fallObjects[x, y].state == BlockState.Empty) continue;

                // 落ちるべきブロックがある場合
                if (fallObjects[x, y].isFall)
                {
                    anyBlockMoving = true;
                    // 引数の y は使わずに、FallObjectの内部データ(toYPos)で移動するので 0 を渡すか、FallObject側を合わせます
                    fallObjects[x, y].Fall(y, fallTime);
                }
            }
        }

        // 画面上の「すべてのブロック」が目的地に着地したら、満を持してDeleteフェーズ（データ整理）へ進む
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
        // 1. まず、消去予定（Delete）のブロックを完全にゲームから消し去る
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

        // 2. 次に、中身が下に落ちた後の「新しい2次元配列（Grid）」を正しく作り直す
        // これをやらないと、次のクリックの時に「見た目と違う場所」が反応してしまいます
        FallObject[,] nextGrid = new FallObject[boardSize, boardSize];

        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (fallObjects[x, y] == null) continue;

                // ブロックが記憶している「着地したY座標」を取得する
                int trueY = fallObjects[x, y].PosY();
                nextGrid[x, trueY] = fallObjects[x, y];
            }
        }

        // 新しいグリッドを本番の配列に上書きする
        fallObjects = nextGrid;

        //すべてが綺麗に片付いたので、プレイヤーの入力受付（Select）状態に戻す！
        nowState = GameState.Select;
    }

    //ブロックが消去予定になった瞬間に、その場で即座にコインを出す関数
    public void SpawnCoinImmediate(FallObject targetObj)
    {
        if (targetObj == null) return;

        DestructibleBlock db = targetObj.GetComponent<DestructibleBlock>();
        if (db != null)
        {
            // 1. コイン加算
            if (CoinManager.Instance != null)
            {
                CoinManager.Instance.AddCoin(db.coinRewardAmount);
            }

            // 2. コインの跳ね上げ演出（爆発したその場で即座にピョコンと跳ねる！）
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