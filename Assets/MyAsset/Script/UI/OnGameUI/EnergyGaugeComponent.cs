using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyGaugeComponent : GaugeComponent
{
    [SerializeField]
    internal Image BaseShapes;
    [SerializeField]
    internal Image OverrideShapes;

    
    bool isFlashed = false;
    float currentFrame = 0.0f;
    float changeFrame = 0.04f;

    float baseMaxEnergy = 100f;
    float baseLength = 115f;
    float Perc = 0f;
    internal override void setValues()
    {

        Perc = Mathf.Lerp(Perc , valueEntity.status.currentEnergy / valueEntity.status.maxEnergy , 0.5f);
        guiText = Mathf.FloorToInt(Perc * 100.0f).ToString() + "%";
        
        float Percentage = valueEntity.status.currentEnergy / valueEntity.status.maxEnergy;
        
        //MAXHPが変動する場合のための処理. MAXHPが変動する場合、ゲージの長さも変動させる.
        float length = baseLength * ((valueEntity.status.maxEnergy - baseMaxEnergy) / baseMaxEnergy);

        OverrideShapes.rectTransform.sizeDelta = 
        new Vector2(length, OverrideShapes.rectTransform.sizeDelta.y);
        BaseShapes.rectTransform.sizeDelta = 
        new Vector2(length , BaseShapes.rectTransform.sizeDelta.y);
        
        OverrideShapes.fillAmount = Percentage;
        
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
        OverrideShapes.color = isFlashed ? color_2 : color_1;
        base.setValues();
    }
}
