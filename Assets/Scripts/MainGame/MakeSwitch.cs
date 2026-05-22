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

    void Start()
    {
        // もしインスペクターで登録し忘れていたら、自動で画面内から探してくる
        if (bombSet == null)
        {
            bombSet = UnityEngine.Object.FindFirstObjectByType<BombSet>();
        }

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
            if (b != null)
            {
                b.Explode();
            }
        }

        // 仕事は終わったので、スイッチの見た目をまた隠す
        if (bombGeneratorObj != null) bombGeneratorObj.SetActive(false);

        // カウンターをゼロにリセット
        BombCount = 0;

        // 「落下の完了監視」コルーチンを起動
        StartCoroutine(WaitForFallAndChangeScene());
    }

    //落下がすべて終わるのを監視してシーンを切り替える関数 
    private IEnumerator WaitForFallAndChangeScene()
    {
        Controller controller = Object.FindFirstObjectByType<Controller>();

        if (controller != null)
        {
            yield return new WaitForSeconds(0.1f);

            //
            while (!controller.IsInSelectState())
            {
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(transitionDelay);
        }

        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(nextSceneName);
    }
}