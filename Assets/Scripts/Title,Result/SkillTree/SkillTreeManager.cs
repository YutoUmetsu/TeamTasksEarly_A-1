using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeManager : MonoBehaviour
{
    public int CostPoint = 0;
    public Button PrestigeButton;
    public Skilltree CenterSkill;
    public TMP_Text pointsText;
    public List<Skilltree> AllSkills;

    [SerializeField] Skill skill;
    [SerializeField] SkillTree skilltree;
    public void ApplySkill(SkillType type)//能力反映処理
    {
        //switch (type)
        //{
            //case SkillType.DamageT1:
            //skill.DamageT1 = true;

            //break;

            //case SkillType.DamageT2:
            //skill.DamageT2 = true;

            //break;

            //case SkillType.DamageT3:
            //skill.DamageT3= true;
            // break;

            // case SkillType.DamageT4:    
            //  skill.DamageT4 = true;
            //  break;

            //case SkillType.DamageT5:
            //    skill.DamageT5 = true;
            //    break;

            //case SkillType.DamageT6:
            //    skill.DamageT6= true;
            //    break;

            //case SkillType.DamageT7:
            //    skill.DamageT7 = true;
            //    break;



       // }
    }

    //全取得チェック
    public bool IsAllSkillUnlocked()
    {
        foreach (var skill in AllSkills)
        {
            if (!skill.UnlokkedSkill)
            {
                return false;
            }
        }

        return true;
    }


    //リセット処理
    public void Prestige()
    {
        foreach (var skill in AllSkills)
        {
            skill.UnlokkedSkill = false;
            skill.AvailobleSkill = false;
            skill.SetStartSprite();
        }

        CenterSkill.AvailobleSkill = true;

        Debug.Log("スキルツリーをリセット");
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

    void Update()
    {
        if (IsAllSkillUnlocked())
        {
            //PrestigeButton.image.color = Color.green;
            PrestigeButton.interactable = true;
        }
        else
        {
            //PrestigeButton.image.color = Color.gray;
            PrestigeButton.interactable = false;
        }
    }

    public void UpdatePointText()//UI更新専用の関数
    {
        pointsText.text = "ポイント:" + CostPoint;
    }
}