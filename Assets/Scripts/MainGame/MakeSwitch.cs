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
            if (BombCount >= 1 && bombGeneratorObj != null && !bombGeneratorObj.activeSelf)
            {
                bombGeneratorObj.SetActive(true);
            }
        }
    }

    public void ExplodeAllBombs()
    {
        Bomb[] allBombs = Object.FindObjectsByType<Bomb>(FindObjectsSortMode.None);

        foreach (Bomb b in allBombs)
        {
            if (b != null)
            {
                b.ExplodeBySwitch();
            }
        }

        if (bombGeneratorObj != null) bombGeneratorObj.SetActive(false);

        BombCount = 0;
        totalExplodeCount++;

        if (bombSet != null)
        {
            bombSet.OnExplodeResetUI();

            int currentMaxBombs = bombSet.maxPlaceableBombs;
            if (BombInventoryManager.Instance != null)
            {
                currentMaxBombs += BombInventoryManager.Instance.bonusMaxBombs;
            }

            // ★修正：どちらの場合も、まずは「今回の爆発・落下の完了」を待つコルーチンを走らせる
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

    private IEnumerator WaitForFallAndUnlockNextBomb()
    {
        Controller controller = Object.FindFirstObjectByType<Controller>();

        if (controller != null)
        {
            // コントローラー側の「今回の処理完了フラグ」を一旦偽にしておく
            controller.isBoardSettledThisFrame = false;

            // コントローラーが爆破・落下・補充をすべて終えて、Delete()内でフラグを立てるまで完全に待つ
            while (!controller.isBoardSettledThisFrame)
            {
                yield return null;
            }

            // 使用したフラグをリセット
            controller.isBoardSettledThisFrame = false;
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        if (bombSet != null)
        {
            bombSet.UnlockPlacement();
        }
    }

    // ─── ★【修正箇所：コントローラーの処理完了通知をガッチリ掴んでから遷移】★ ───
    private IEnumerator WaitForFallAndChangeScene()
    {
        Controller controller = Object.FindFirstObjectByType<Controller>();

        if (controller != null)
        {
            // コントローラー側の「今回の処理完了フラグ」を一旦偽にしておく
            controller.isBoardSettledThisFrame = false;

            // コントローラーが「爆風の0.5秒待機」も「ブロックの落下」も「上からの補充」も
            // すべてを完璧にやり遂げて、盤面が静止するその瞬間まで【何フレームでも】待ち続ける！
            while (!controller.isBoardSettledThisFrame)
            {
                yield return null;
            }

            // フラグをリセット
            controller.isBoardSettledThisFrame = false;
        }
        else
        {
            // 安全策の遅延
            yield return new WaitForSeconds(transitionDelay);
        }

        // ブロックがすべて綺麗に落ちきった絵を見せてから、余韻（0.5秒）を挟んでリザルトへ！
        yield return new WaitForSeconds(0.5f);
        Debug.Log("すべての起爆・落下・補充が盤面に反映されたのを確認。リザルトへ遷移します。");
        SceneManager.LoadScene(nextSceneName);
    }
}