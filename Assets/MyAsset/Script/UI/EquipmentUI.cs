using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class EquipmentUI : MonoBehaviour
{
    public TextMeshProUGUI GUITextMesh;
    public TextMeshProUGUI VTextMesh;

    //this image needs to be described as Fill
    public Image icon;

    public Image BarImage;

    public float BarValue;

    public string labeltext;
    public string valuetext;

    public Color textColor;

    public Entity mainEntity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //一先ず暫定処理 Entityからの呼び出しで指定できるようにしたい.
        if(mainEntity.equipmentInHand != null)
        {
            icon.sprite = mainEntity.equipmentInHand.GUIImage;
            labeltext = mainEntity.equipmentInHand.name;
            valuetext =  mainEntity.equipmentInHand.durability.ToString("F0");
            BarValue = mainEntity.equipmentInHand.durability / mainEntity.equipmentInHand.maxDurability;
            textColor = Color.cyan;
        }
        else
        {
            icon.sprite = gameState.self.DefaultEquipmentImage;
            labeltext = mainEntity.status.BaseValue.valueName;
            valuetext = mainEntity.status.BaseValue.FValue.ToString("F0");
            BarValue = Mathf.Min(mainEntity.status.BaseValue.FValue , 1.0f);
            textColor = Color.white;
        }
        GUITextMesh.text = labeltext;
        VTextMesh.text = valuetext;
        GUITextMesh.color = textColor;
        VTextMesh.color = textColor;
        BarImage.fillAmount = BarValue;
    }
}
