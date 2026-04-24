using UnityEngine;

public class MakeSwitch : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject BomeGenerator;
    public int BomeCount {  get; set; }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(BomeCount >= 5)
        {
            BomeGenerator.SetActive(true);
        }
    }
    public void Kaboom()
    {
        BomeGenerator.SetActive(false);
    }
}
