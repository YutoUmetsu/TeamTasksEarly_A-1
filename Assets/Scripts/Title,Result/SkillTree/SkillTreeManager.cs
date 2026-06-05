using TMPro;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    public int CostPoint = 0;
    public Skilltree CenterSkill;
    public TMP_Text pointsText;

    [SerializeField] Skill skill;
    public void ApplySkill(SkillType type)//能力反映処理
    {
        switch (type)
        {
            case SkillType.DamageT1:
            skill.DamageT1 = true;

            break;

            case SkillType.DamageT2:
            skill.DamageT2 = true;

            break;

            case SkillType.DamageT3:
            skill.DamageT3= true;
             break;

             case SkillType.DamageT4:    
              skill.DamageT4 = true;
              break;

            case SkillType.DamageT5:
                skill.DamageT5 = true;
                break;

            case SkillType.DamageT6:
                skill.DamageT6= true;
                break;

            case SkillType.DamageT7:
                skill.DamageT7 = true;
                break;



        }
    }
    void Start()
    {
        Debug.Log(CenterSkill);

        if (CenterSkill == null)
        {
            Debug.LogError("CenterSkillが未設定");
            return;
        }

        if (pointsText == null)
        {
            Debug.LogError("pointsTextが未設定");
            return;
        }

        CenterSkill.AvailobleSkill = true;
        UpdatePointText();
    }

    public void UpdatePointText()//UI更新専用の関数
    {
        pointsText.text = "ポイント:" + CostPoint;
    }
}