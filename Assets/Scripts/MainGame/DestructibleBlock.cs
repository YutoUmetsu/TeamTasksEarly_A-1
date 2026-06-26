using UnityEngine;

public class DestructibleBlock : MonoBehaviour
{
    [Header("このブロックから手に入るコインの総枚数")]
    public int coinRewardAmount = 1;

    [Header("演出用：1枚コインのプレハブ")]
    public GameObject coinVisualPrefab;

    // ─── 【追加：5枚コイン用のプレハブ】 ───
    [Header("演出用：5枚コインのプレハブ（無い場合は1枚コインで代用されます）")]
    public GameObject coinVisualPrefab5x;

    [Header("飛び散る勢いの調整")]
    public float upwardForce = 5f;
    public float sideForce = 2f;
}