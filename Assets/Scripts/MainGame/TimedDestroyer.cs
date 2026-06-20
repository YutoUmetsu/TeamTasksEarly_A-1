using UnityEngine;

public class TimedDestroyer : MonoBehaviour
{
    public float lifeTime = 0.5f;

    void Start()
    {
        // 親（爆弾本体）が消されても巻き添えを食らわないように、即座に完全独立する
        transform.SetParent(null);

        // 指定された時間（destroyDelayと同じ時間）が経ったら、自分自身（爆風）を完全に削除する
        Destroy(gameObject, lifeTime);
    }
}