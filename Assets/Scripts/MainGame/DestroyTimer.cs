using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    [Header("何秒後に消えるか（デフォルトは1.5秒）")]
    [SerializeField] private float lifeTime = 1.5f;

    void Start()
    {
        // 生まれた瞬間（Start）に、指定秒数後に自分を破壊する予約を入れる！
        Destroy(gameObject, lifeTime);
    }
}