using UnityEngine;

public enum BlockState
{
    Empty = 0,
    Normal,
    Delete,
}

public class FallObject : MonoBehaviour
{
    public BlockState state { get; private set; }
    public bool isFall { get; private set; }
    [SerializeField] float toYPos;
    SpriteRenderer spriteRenderer;

    // 【修正】StartではなくAwakeにする（生成直後でもコンポーネントを取得できるようにするため）
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 最初に情報をセットさせる関数
    /// </summary>
    public void StartUpFallObject(BlockState _state)
    {
        state = _state;
        toYPos = transform.position.y;
        isFall = false; // 初期化時に落下フラグをリセット
    }

    /// <summary>
    /// 消すための準備
    /// </summary>
    /// <summary>
    /// 消すための準備（子オブジェクトの見た目も完全に消す）
    /// </summary>
    public void SetDelete()
    {
        if (state != BlockState.Delete)
        {
            state = BlockState.Delete;

            // 自分と、その子オブジェクトにあるすべての SpriteRenderer を非表示にする
            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in renderers)
            {
                sr.enabled = false;
            }

            // もしくは、見た目だけでなくオブジェクトの機能自体を止めたい場合はこちら
            // gameObject.SetActive(false); 
        }
    }


    /// <summary>
    /// すでに消えたものとして扱う
    /// </summary>
    public void EmptySelf()
    {
        state = BlockState.Empty;
    }

    /// <summary>
    /// 落ちることを明示させる
    /// </summary>
    public void SetFall(float _toYPos)
    {
        // すでに目的地にいる場合は落下フラグを立てない
        if (Mathf.Approximately(transform.position.y, _toYPos))
        {
            isFall = false;
            toYPos = _toYPos;
            return;
        }

        isFall = true;
        toYPos = _toYPos;
    }

    /// <summary>
    /// 【修正】落ちる処理（毎フレーム一定速度でスムーズに下ろす計算に変更）
    /// </summary>
    public void Fall(float _y, float _fallTime)
    {
        if (!isFall) return;

        if (transform.position.y > toYPos)
        {
            // 1秒あたりの移動速度を計算し、下方向へ移動させる
            float speed = (1.0f / _fallTime);
            float nextY = transform.position.y - speed * Time.deltaTime;

            // 目的地を通り過ぎたら補正
            if (nextY <= toYPos)
            {
                nextY = toYPos;
                isFall = false;
            }

            transform.position = new Vector2(transform.position.x, nextY);
        }
        else
        {
            isFall = false;
        }
    }

    /// <summary>
    /// 落ち先の座標を返す
    /// </summary>
    public int PosY()
    {
        return Mathf.RoundToInt(toYPos); // 浮動小数点の誤差を防ぐため四捨五入でキャスト
    }
}
