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
    private int gridY; // 自分が今何マス目にいるかのインデックス (0, 1, 2...)
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 最初に情報をセットさせる関数
    /// </summary>
    public void StartUpFallObject(BlockState _state, int initialGridY)
    {
        state = _state;
        toYPos = transform.position.y;
        gridY = initialGridY; // 最初のマス目を記憶
        isFall = false;
    }

    public void SetDelete()
    {
        if (state != BlockState.Delete)
        {
            state = BlockState.Delete;

            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in renderers)
            {
                sr.enabled = false;
            }
        }
    }

    public void EmptySelf()
    {
        state = BlockState.Empty;
    }

    /// <summary>
    /// 落ちることを明示させる
    /// </summary>
    public void SetFall(float _toYPos, int _targetGridY)
    {
        toYPos = _toYPos;
        gridY = _targetGridY;

        if (Mathf.Approximately(transform.position.y, _toYPos))
        {
            isFall = false;
            return;
        }

        isFall = true;
    }

    public void Fall(float _y, float _fallTime)
    {
        if (!isFall) return;

        if (transform.position.y > toYPos)
        {
            float speed = (1.0f / _fallTime);
            float nextY = transform.position.y - speed * Time.deltaTime;

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
    /// 純粋なマス目の番号（0, 1, 2...）を返す
    /// </summary>
    public int PosY()
    {
        return gridY;
    }
}