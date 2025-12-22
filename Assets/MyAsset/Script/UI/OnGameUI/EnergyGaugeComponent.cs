using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyGaugeComponent : GaugeComponent
{
    [SerializeField]
    internal Shapes.Rectangle BaseShapes;
    [SerializeField]
    internal Shapes.Rectangle OverrideShapes;
    [SerializeField]
    internal Shapes.ShapeRenderer MarkShaper;

    bool isFlashed = false;
    float currentFrame = 0.0f;
    float changeFrame = 0.04f;
    internal override void setValues()
    {
        float Percentage = valueEntity.status.currentEnergy / valueEntity.status.maxEnergy;
        guiText = Mathf.FloorToInt(Percentage * 100.0f).ToString() + "%";
        OverrideShapes.Width = Percentage * BaseShapes.Width;
        if(Percentage >= 1.0f)
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
