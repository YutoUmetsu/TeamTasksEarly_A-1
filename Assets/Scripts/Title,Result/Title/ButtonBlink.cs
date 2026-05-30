using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonBlink : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public TextMeshProUGUI text;

    private bool isHover = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (text == null) return;

        if (isHover)
        {
            Color c = text.color;
            c.a = Mathf.PingPong(Time.time * 2f, 1f);
            text.color = c;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (text == null) return;

        isHover = false;

        Color c = text.color;
        c.a = 1f;
        text.color = c;
    }
}
