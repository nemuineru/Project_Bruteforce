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
        
        currentFrame = currentFrame >= changeFrame ? 0 : currentFrame + Time.deltaTime;
        if(currentFrame >= changeFrame)
        {
            currentFrame = 0;
            isFlashed = !isFlashed;
        }
        
        // バーが1/3以上有るならウェポン使用可能にする.
        // で、100%以上溜まってるなら必殺技を解禁.. メガクラッシュの体力消費もバーに応じてかなり抑える.
        OverrideShapes.Color = Percentage >= 1.00 && isFlashed ? color_2 : color_1;
        MarkShaper.Color = Percentage >= .33 ? OverrideShapes.Color  : Color.black;

        base.setValues();
    }
}
