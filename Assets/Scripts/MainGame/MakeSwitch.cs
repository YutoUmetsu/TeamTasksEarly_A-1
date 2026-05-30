using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MakeSwitch : MonoBehaviour
{
    public int BombCount { get; set; }

    [Header("爆弾設置システム（BombSet）のオブジェクトをここに")]
    [SerializeField] private BombSet bombSet;

    [Header("スイッチの見た目オブジェクト（3Dモデルや画像、UIボタンなど）")]
    public GameObject bombGeneratorObj;

    [Header("爆発が終わってからシーン遷移するまでの待ち時間(秒)")]
    [SerializeField] private float transitionDelay = 2.0f;

    [Header("移動先のシーン名")]
    [SerializeField] private string nextSceneName = "ResultScene";

    // 現在までに「起爆ボタンを押した総回数」を記録するカウンター
    private int totalExplodeCount = 0;

    void Start()
    {
        if (bombSet == null)
        {
            bombSet = UnityEngine.Object.FindFirstObjectByType<BombSet>();
        }

        if (bombGeneratorObj != null) bombGeneratorObj.SetActive(false);
    }

    void Update()
    {
        if (bombSet != null)
        {
            // 仕様変更：1弾置かれたら（BombCountが1以上になったら）即座にスイッチを召喚
            if (BombCount >= 1 && bombGeneratorObj != null && !bombGeneratorObj.activeSelf)
            {
                bombGeneratorObj.SetActive(true);
            }
        }
    }

    // スイッチが押されたときに呼び出す処理（起爆ボタン）
    public void ExplodeAllBombs()
    {
        Bomb[] allBombs = Object.FindObjectsByType<Bomb>(FindObjectsSortMode.None);

        foreach (Bomb b in allBombs)
        {
            if (b != null)
            {
                b.Explode();
            }
        }

        // 起爆処理を行ったので、一旦スイッチの見た目を隠す
        if (bombGeneratorObj != null) bombGeneratorObj.SetActive(false);

        // カウンターをリセット（次の1個のため）
        BombCount = 0;

        // 起爆した回数をカウントアップ
        totalExplodeCount++;

        if (bombSet != null)
        {
            bombSet.OnExplodeResetUI();
            // ─── 【修正：ここでもボーナス込みの本当の最大数を計算する】 ───
            int currentMaxBombs = bombSet.maxPlaceableBombs;
            if (BombInventoryManager.Instance != null)
            {
                currentMaxBombs += BombInventoryManager.Instance.bonusMaxBombs;
            }

            // 修正した本当の最大数と比較する！
            if (totalExplodeCount >= currentMaxBombs)
            {
                StartCoroutine(WaitForFallAndChangeScene());
            }
            else
            {
                StartCoroutine(WaitForFallAndUnlockNextBomb());
            }
        }
    }

    // 落下がすべて終わるのを監視して、ロックを解除して次の配置へ戻すコルーチン
    private IEnumerator WaitForFallAndUnlockNextBomb()
    {
        Controller controller = Object.FindFirstObjectByType<Controller>();

        if (controller != null)
        {
            yield return new WaitForSeconds(0.1f);

            while (!controller.IsInSelectState())
            {
                yield return null;
            }
        }

        // 落下とデータ整理が完全に終わったら、次の爆弾を置けるようにロックを解除
        if (bombSet != null)
        {
            bombSet.UnlockPlacement();
        }
    }

    // 最後の落下がすべて終わるのを監視してシーンを切り替えるコルーチン 
    private IEnumerator WaitForFallAndChangeScene()
    {
        Controller controller = Object.FindFirstObjectByType<Controller>();

        if (controller != null)
        {
            yield return new WaitForSeconds(0.1f);

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
        Debug.Log("すべてのステージ起爆・落下ループが完了。リザルトへ遷移します。");
        SceneManager.LoadScene(nextSceneName);
    }
}