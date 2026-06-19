using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{

    public TMP_Text SkillNameText;
    public TMP_Text CostText;

    public Button UnlockButton;

    private Skilltree currentSkill;

    public SkillTreeManager manager;

    public void SelectSkill(Skilltree skill)
    {
        currentSkill = skill;
        Debug.Log(skill.SkillName);
        SkillNameText.text = skill.SkillName;

        CostText.text = skill.SkillCountCalculation().ToString() + "            ‰ð•ú";

        UnlockButton.interactable = skill.AvailobleSkill && !skill.UnlokkedSkill && manager.CostPoint >= skill.Cost;
    }

    public void UnlockCurrentSkill()
    {
        if (currentSkill == null)
        {
            Debug.Log("ƒXƒLƒ‹–¢‘I‘ð");
            return;
        }
        currentSkill.UnlockSkilltree(manager);
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentSkill == null)
        {
            UnlockButton.image.color = Color.gray;
            return;
        }

        if (currentSkill.AvailobleSkill &&
            !currentSkill.UnlokkedSkill &&
            manager.CostPoint >= currentSkill.Cost)
        {
            UnlockButton.image.color = Color.blue;
            UnlockButton.interactable = true;
        }
        else
        {
            UnlockButton.image.color = Color.gray;
            UnlockButton.interactable = false;
        }
    }
}
