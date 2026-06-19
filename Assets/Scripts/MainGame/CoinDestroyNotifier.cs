using UnityEngine;

public class CoinDestroyNotifier : MonoBehaviour
{
    private Controller controller;

    public void Setup(Controller targetController)
    {
        controller = targetController;
    }

    // オブジェクトが破棄された瞬間にUnityが自動で呼び出す関数
    void OnDestroy()
    {
        if (controller != null)
        {
            controller.DecrementCoinCount();
        }
    }
}