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

    [Header("耐久値（体力）の設定")]
    [SerializeField] private int maxHp = 1; // インスペクターでブロックごとに耐久値を変えられます
    private int currentHp;

    //ひび割れの出力用
    [SerializeField] GameObject Damage1;//ヒビの画像
    [SerializeField] GameObject Damage2;
    [SerializeField] GameObject Damage3;
    [SerializeField] int damage1st;
    [SerializeField] int damage2nd;
    [SerializeField] int damage3rd;

    private void Start()
    {
        if (Damage1 == null || Damage2 == null || Damage3 == null)
            return;
        Damage1.SetActive(false);           
        Damage2.SetActive(false);        
        Damage3.SetActive(false);
    }

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

        currentHp = maxHp; // 体力を満タンに初期化
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

    /// <summary>
    /// ダメージを受ける関数。体力が0になったら消去フラグを立てる
    /// </summary>
    public void TakeDamage(int damageAmount)
    {
        if (state == BlockState.Delete || state == BlockState.Empty) return;

        currentHp -= damageAmount;
        Debug.Log($"{gameObject.name} に {damageAmount} のダメージ！ (残りHP: {currentHp})");
        if (currentHp <= 0)
        {
            
            // コインマネージャーやコントローラー経由でコイン演出を呼び出す
            Controller controller = UnityEngine.Object.FindFirstObjectByType<Controller>();
            if (controller != null)
            {
                controller.SpawnCoinImmediate(this); // 自分自身を渡してコインを飛ばす！
            }

            SetDelete();
        }
    }
    //private void FixedUpdate()
    //{
    //    if (Damage1 == null || Damage2 == null || Damage3 == null)
    //        return;

    //    if (currentHp <= damage1st && currentHp > damage2nd)
    //    {
    //        Damage1.SetActive(true);
    //    }
    //    else if (currentHp <= damage2nd && currentHp > damage3rd)
    //    {
    //        Damage1.SetActive(false);
    //        Damage2.SetActive(true);
    //    }
    //    else if (currentHp <= damage3rd && currentHp > 0)
    //    {
    //        Damage2.SetActive(false);
    //        Damage3.SetActive(true);
    //    }
    //    else
    //    {
    //        Damage1.SetActive(false);
    //        Damage2.SetActive(false);
    //        Damage3.SetActive(false);
    //    }
    //}

    public void UpdateSprite()
    {
        if (Damage1 == null || Damage2 == null || Damage3 == null)
            return;

        if (currentHp <= damage1st && currentHp > damage2nd)
        {
            Damage1.SetActive(true);
        }
        else if (currentHp <= damage2nd && currentHp > damage3rd)
        {
            Damage1.SetActive(false);
            Damage2.SetActive(true);
        }
        else if (currentHp <= damage3rd && currentHp > 0)
        {
            Damage2.SetActive(false);
            Damage3.SetActive(true);
        }
 
    }
}