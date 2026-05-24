using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//HealthGauge Checker.
//the gauge component must be Rectangle.
public class HealthGaugeComponent : GaugeComponent
{
    [SerializeField]
    internal Image BaseShapes;
    [SerializeField]
    internal Image OverrideShapes;

    
    bool isFlashed = false;
    float currentFrame = 0.0f;
    float changeFrame = 0.04f;

    float baseMaxHP = 100f;
    float baseLength = 115f;
    internal override void setValues()
    {

        guiText = Mathf.CeilToInt(valueEntity.status.currentHP).ToString();
        float Percentage = valueEntity.status.currentHP / valueEntity.status.maxHP;
        
        //MAXHPが変動する場合のための処理. MAXHPが変動する場合、ゲージの長さも変動させる.
        float length = baseLength * ((valueEntity.status.maxHP - baseMaxHP) / baseMaxHP);

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