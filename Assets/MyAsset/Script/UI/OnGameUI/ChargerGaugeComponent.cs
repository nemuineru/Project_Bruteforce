using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerGaugeComponent : GaugeComponent
{
    [SerializeField]
    internal Shapes.Disc BaseShapes;
    [SerializeField]
    internal GameObject ParentShapes;

    
    bool isFlashed = false;
    float currentFrame = 0.0f;
    float changeFrame = 0.04f;
    // Start is called before the first frame update
    void Start()
    {
        
    }
internal override void setValues()
    {
        float Percentage = valueEntity.status.ChargeTime;
        
        if(Percentage <= 0.3f)
        {
            currentFrame += Time.deltaTime;
        }
        else
        {
            currentFrame = 0;
            isFlashed = false;
        }
        if(currentFrame >= changeFrame)
        {
            currentFrame = 0;
            isFlashed = !isFlashed;
        }
        BaseShapes.AngRadiansEnd = (Percentage * 360f) * Mathf.Deg2Rad;
        guiText = Mathf.FloorToInt(Percentage * 100f).ToString();
        //isFlashed ? color_2 : color_1;
        base.setValues();
    }
}
