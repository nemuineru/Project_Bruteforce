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
    [SerializeField]
    internal AudioSource EnergyFull;

    bool isFlashed = false;
    float currentFrame = 0.0f;
    float changeFrame = 0.04f;

    float Perc = 0f;

    bool isSoundPlayed = false;
    internal override void setValues()
    {
        Perc = Mathf.Lerp(Perc , valueEntity.status.currentEnergy / valueEntity.status.maxEnergy , 0.5f);
        guiText = Mathf.FloorToInt(Perc * 100.0f).ToString() + "%";
        OverrideShapes.Width = Perc * BaseShapes.Width;
        
        currentFrame = currentFrame >= changeFrame ? 0 : currentFrame + Time.deltaTime;
        if(currentFrame >= changeFrame)
        {
            currentFrame = 0;
            isFlashed = !isFlashed;
        }
        
        // バーが1/3以上有るならウェポン使用可能にする.
        // で、100%以上溜まってるなら必殺技を解禁.. メガクラッシュの体力消費もバーに応じてかなり抑える.
        OverrideShapes.Color = Perc >= 0.98 && isFlashed ? color_2 : color_1;
        MarkShaper.Color = Perc >= .33 ? OverrideShapes.Color  : Color.black;

        if(Perc >= 1.0 && !isSoundPlayed && EnergyFull != null)
        {
            EnergyFull.Play();
            isSoundPlayed = true;
        }
        else if(Perc < 1.0)
        {
            isSoundPlayed = false;
        }

        base.setValues();
    }
}
