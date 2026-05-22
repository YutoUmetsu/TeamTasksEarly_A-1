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

    //ゲームの状況管理用
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

    void Start()
    {
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

    /// <summary>
    /// 盤面生成
    /// </summary>
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
            }
        }
    }

    void Update()
    {
        // ステートに合わせた処理を行う
        ControllerUpdate[(int)nowState]();
    }

    /// <summary>
    /// 消すオブジェクトを選択
    /// </summary>
    /// <summary>
    /// 消すオブジェクトを選択（ズレ防止修正版）
    /// </summary>
    void SelectUpdate()
    {
        // マウスの座標から消すオブジェクトを選択する
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // マウスの画面座標をワールド座標（ゲーム内座標）に変換
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            // 浮動小数点の誤差をなくすため、四捨五入でキレイな整数（0, 1, 2...）にする
            int gridX = Mathf.RoundToInt(worldPos.x);
            int gridY = Mathf.RoundToInt(worldPos.y);

            // 盤面の配列の範囲内に入っているかチェック
            if (gridX >= 0 && gridX < boardSize && gridY >= 0 && gridY < boardSize)
            {
                // 配列の中身が空（null）でなければ削除予定を設定
                if (fallObjects[gridX, gridY] != null)
                {
                    fallObjects[gridX, gridY].SetDelete();
                    Debug.Log($"クリック成功: [{gridX}, {gridY}] を消去予定にしました");
                }
            }
            else
            {
                Debug.LogWarning($"盤面の外をクリックしています: [{gridX}, {gridY}]");
            }
        }

        // 消すための準備へ
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            OnExit[(int)nowState]?.Invoke();
        }
    }


    /// <summary>
    /// 消すために落ちる先を計算させる
    /// </summary>
    void CalculateFallTargets()
    {
        for (int x = 0; x < boardSize; x++)
        {
            int fallPos = 0;
            for (int y = 0; y < boardSize; y++)
            {
                // ★ ここに null チェックを追加
                if (fallObjects[x, y] == null) continue;

                if (fallObjects[x, y].state == BlockState.Empty) continue;

                if (fallObjects[x, y].state != BlockState.Delete)
                {
                    fallObjects[x, y].SetFall(fallPos);
                    fallPos++;
                }
            }
        }
        nowState = GameState.Fall;
    }



    /// <summary>
    /// 落ちる処理、終わり次第消す
    /// </summary>
    /// <summary>
    /// 落ちる処理、終わり次第消す
    /// </summary>
    void FallUpdate()
    {
        // 落とすオブジェクトがあるか確認用
        bool fall = false;
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                // ★ ここに null チェックを追加
                if (fallObjects[x, y] == null) continue;

                if (fallObjects[x, y].state == BlockState.Empty) continue;
                if (fallObjects[x, y].isFall)
                {
                    // 落とすものアリ
                    fall = true;
                    // 元の座標と落ちる時間を渡す
                    fallObjects[x, y].Fall(y, fallTime);
                }
            }
        }

        // 消す処理へ
        if (!fall) OnExit[(int)nowState]();
    }


    /// <summary>
    /// ブロックを消しつつ配列情報を更新する
    /// </summary>
    /// <summary>
    /// ブロックを消しつつ、配列情報を【安全に】更新する
    /// </summary>
    void Delete()
    {
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                // すでに空なら飛ばす
                if (fallObjects[x, y] == null || fallObjects[x, y].state == BlockState.Empty) continue;

                // 消す予定のオブジェクトの場合
                if (fallObjects[x, y].state == BlockState.Delete)
                {
                    fallObjects[x, y].EmptySelf();

                    // 【重要】ヒエラルキーからも完全に消去したい場合は Destory を呼ぶ
                    // Destroy(fallObjects[x, y].gameObject); 

                    fallObjects[x, y] = null; // 配列を空にする
                }
                // 生き残っていて、下に落ちるアニメーションをしたオブジェクトの場合
                else
                {
                    int targetY = fallObjects[x, y].PosY();

                    // 自分の現在地（y）と、落ちるべき位置（targetY）が違う場合のみ移動
                    if (y != targetY)
                    {
                        // 新しい位置に自分を代入
                        fallObjects[x, targetY] = fallObjects[x, y];

                        // 元いた場所はキレイに空（null）にする（これで二重処理を防ぐ）
                        fallObjects[x, y] = null;
                    }
                }
            }
        }

        // 選択する処理へ戻る
        nowState = GameState.Select;
    }

}