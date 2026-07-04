using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour
{
    public TextMeshProUGUI GUIText;

    //this image needs to be described as Fill
    public Image imgs;

    public float BarValue;

    public string text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GUIText.text = text;
        imgs.fillAmount = BarValue;
    }
}
