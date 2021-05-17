using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // Variables
    [SerializeField] private GameObject HPBarFront;
    [SerializeField] private GameObject HPBarBack;
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
}
