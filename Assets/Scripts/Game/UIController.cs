using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    // Variables
    [SerializeField] private GameObject HPBarFront;
    [SerializeField] private GameObject HPBarBack;
    [SerializeField] private GameObject Message;
    [SerializeField] private GameObject ShurikenCounter;
    private Image hpBar;
    public float currentHP;
    public float maxHP;

    // Start is called before the first frame update
    void Awake()
    {
        // Gets COMPONENT from Children
        hpBar = HPBarFront.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        hpBar.fillAmount = currentHP / maxHP;
    }

    public void activateBar()
    {
        HPBarFront.SetActive(true);
        HPBarBack.SetActive(true);
    }

    public void deactivateBar()
    {
        HPBarFront.SetActive(false);
        HPBarBack.SetActive(false);
    }

    public void showStartMessage()
    {
        Message.SetActive(true);
        TextMeshProUGUI txt = Message.GetComponent<TextMeshProUGUI>();
        txt.text = ("Press Enter to Start");
    }
    public void showRetryMessage()
    {
        Message.SetActive(true);
        TextMeshProUGUI txt = Message.GetComponent<TextMeshProUGUI>();
        txt.text = ("Press Enter to Retry");
    }

    public void disableMessage()
    {
        Message.SetActive(false);
    }

    public void setShurikenCounter(int count)
    {
        TextMeshProUGUI txt = ShurikenCounter.GetComponent<TextMeshProUGUI>();
        txt.text = ("Shuriken: "+count);
    }

    public void enableShurikenCounter()
    {
        ShurikenCounter.SetActive(true);
    }

    public void disableShurikenCounter()
    {
        ShurikenCounter.SetActive(false);
    }
}
