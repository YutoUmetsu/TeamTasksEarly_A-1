using UnityEngine;

public class MakeSwitch : MonoBehaviour
{
    // 今までに置かれた爆弾の数を数えるカウンター
    public int BombCount { get; set; }

    [Header("スイッチが出るために必要な爆弾の数")]
    [SerializeField] private int requiredBombCount = 5;

    [Header("スイッチの見た目オブジェクト（3Dモデルや画像）")]
    public GameObject bombGeneratorObj;

    // ゲームが始まった瞬間に1回だけ実行される
    void Start()
    {
        // 準備：最初はスイッチを隠しておく（透明にするだけで、裏では動いてる）
        if (bombGeneratorObj != null) bombGeneratorObj.SetActive(false);
    }

    // ゲーム中、ずーっと高速でループ実行される（1秒間に何十回も）
    void Update()
    {
        // もし「置かれた数」が必要な数を超えていて、かつ「スイッチがまだ隠れている」なら
        if (BombCount >= requiredBombCount && bombGeneratorObj != null && !bombGeneratorObj.activeSelf)
        {
            // 条件クリア！スイッチを画面に表示する
            bombGeneratorObj.SetActive(true);
        }
    }

    // スイッチが押されたときに呼び出す処理（全爆破ボタン）
    public void ExplodeAllBombs()
    {
        // ステージ上にある「すべての爆弾」を、残さず探してリストにする
        Bomb[] allBombs = Object.FindObjectsByType<Bomb>(FindObjectsSortMode.None);

        // リストに入った爆弾を、端から順番に1個ずつ爆発させていく
        foreach (Bomb b in allBombs)
        {
            // 爆弾がちゃんと存在していれば、爆発スイッチを入れる
            if (b != null)
            {
                b.Explode();
            }
        }

        // 仕事は終わったので、スイッチの見た目をまた隠す
        if (bombGeneratorObj != null) bombGeneratorObj.SetActive(false);

        // カウンターをゼロにリセット（また次ためる用）
        BombCount = 0;
    }
}