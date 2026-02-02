using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GaugeComponent : MonoBehaviour
{

    [SerializeField]
    internal Entity valueEntity;

    [SerializeField]
    internal Color color_1;
    [SerializeField]
    internal Color color_2;

    //text for showing value
    [SerializeField]
    internal TMPro.TMP_Text ShowText;

    internal string guiText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    //onUpdate, Set those gauges to what value.
    void Update()
    {        
        if(valueEntity != null)
        {
            setValues();
        }
        if(ShowText != null)
        {
            ShowText.text = guiText;
        }
    }

    virtual internal void setValues()
    {
    }
}
