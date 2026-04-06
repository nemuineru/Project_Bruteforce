using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardGaugeComponent : GaugeComponent
{
    [SerializeField]
    internal Shapes.Rectangle BaseShapes;
    [SerializeField]
    internal Shapes.Rectangle OverrideShapes;

    
    bool isFlashed = false;
    float currentFrame = 0.0f;
    float changeFrame = 0.04f;

    float perc_prev = 1.0f;
    internal override void setValues()
    {
        //guiText = Mathf.CeilToInt(valueEntity.status.currentGuardPoint).ToString();
        float Percentage = valueEntity.status.currentGuardPoint / valueEntity.status.maxGuardPoint;
        perc_prev = Mathf.Lerp(perc_prev,Percentage,0.15f);
        OverrideShapes.Width = perc_prev * BaseShapes.Width;
        
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
        OverrideShapes.Color = isFlashed ? color_2 : color_1;
        base.setValues();
    }
}
