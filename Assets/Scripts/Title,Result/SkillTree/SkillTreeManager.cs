using TMPro;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    public int CostPoint = 0;

    public Skilltree CenterSkill;

    public TMP_Text pointsText;

    void Start()
    {
        CenterSkill.AvailobleSkill = true;
        UpdatePointText();
    }

    public void UpdatePointText()//UI更新専用の関数
    {
        pointsText.text = "ポイント:" + CostPoint;
    }
}