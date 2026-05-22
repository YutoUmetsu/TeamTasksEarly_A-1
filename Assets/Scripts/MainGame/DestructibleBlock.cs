using UnityEngine;

public class DestructibleBlock : MonoBehaviour
{
    [Header("このブロックから手に入るコインの枚数")]
    public int coinRewardAmount = 1;

    [Header("演出用コインのプレハブ")]
    public GameObject coinVisualPrefab;

    public float upwardForce = 5f;
    public float sideForce = 2f;


}