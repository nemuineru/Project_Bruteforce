using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentStrings : MonoBehaviour
{
    public Entity mainEntity;
    public string valuetext;
    [SerializeField]
    internal TMPro.TMP_Text ShowText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(mainEntity != null)
        {
            valuetext = mainEntity.status.instructionLabels;
        }
        ShowText.text = valuetext;
    }
}
