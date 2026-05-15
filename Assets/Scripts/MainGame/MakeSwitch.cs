using UnityEngine;
using System.Collections; // 時間差の処理（コルーチン）を使うために必要
using UnityEngine.SceneManagement; // 画面を切り替える（シーン遷移）ために必要

public class MakeSwitch : MonoBehaviour
{
    // 今までに置かれた爆弾の数を数えるカウンター
    public int BombCount { get; set; }

    // ─── 修正：固定の数字をやめて、爆弾設置システム（BombSet）と連動させる ───
    [Header("爆弾設置システム（BombSet）のオブジェクトをここに")]
    [SerializeField] private BombSet bombSet;

    [Header("スイッチの見た目オブジェクト（3Dモデルや画像、UIボタンなど）")]
    public GameObject bombGeneratorObj;

    [Header("爆発が終わってからシーン遷移するまでの待ち時間(秒)")]
    [SerializeField] private float transitionDelay = 2.0f;

    [Header("移動先のシーン名")]
    [SerializeField] private string nextSceneName = "ResultScene";

    // ゲームが始まった瞬間に1回だけ実行される
    void Start()
    {
        // 準備：最初はスイッチを隠しておく（透明にするだけで、裏では動いてる）
        if (bombGeneratorObj != null) bombGeneratorObj.SetActive(false);
    }

    // ゲーム中、ずーっと高速でループ実行される（1秒間に何十回も）
    void Update()
    {
        // ─── 修正：自動連動システム ───
        if (bombSet != null)
        {
            // スイッチ出現に必要な数を、BombSet側で決めた「最大設置数」と同じにする（最初は3）
            int requiredCount = bombSet.maxPlaceableBombs;

            // もし「置かれた数」が「今の最大設置数」に達していて、かつ「スイッチがまだ隠れている」なら
            if (BombCount >= requiredCount && bombGeneratorObj != null && !bombGeneratorObj.activeSelf)
            {
                // 条件クリア！スイッチを画面に表示する
                bombGeneratorObj.SetActive(true);
            }
        }
    }

    // スイッチ（UIボタンなど）が押されたときに呼び出す処理（全爆破ボタン）
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

        // カウンターをゼロにリセット（次のゲーム用）
        BombCount = 0;

        // ─── 追加：爆破が終わったので、時間差で次の画面へ行くタイマーを起動！ ───
        StartCoroutine(WaitAndChangeScene());
    }

    // ─── 追加：指定した秒数だけ待ってから画面を切り替える関数 ───
    private IEnumerator WaitAndChangeScene()
    {
        // コインが飛び出したりする演出が終わるまで、指定した秒数（例: 2秒）だけ待つ
        yield return new WaitForSeconds(transitionDelay);

        Debug.Log($"{nextSceneName} へ画面を切り替えます！");

        // 次のシーンへ遷移する
        SceneManager.LoadScene(nextSceneName);
    }
}