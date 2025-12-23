using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerGaugeComponent : GaugeComponent
{
    [SerializeField]
    internal Shapes.Disc BaseShapes;
    [SerializeField]
    internal GameObject ParentShapes;
    [SerializeField]
    internal Color color_flash;

    
    bool isFlashed = false;
    float currentFrame = 0.0f;
    float changeFrame = 0.012f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    //THE DISK IS BUGGED.
    //360度以上回転させるなら 360 -> 1080 -> 1800(+720)..と1回転以降回転させるためなら720度加算しなければならない.
internal override void setValues()
    {
        float Percentage = 
        Mathf.Clamp01(valueEntity.status.ChargeTime / valueEntity.status.setChargeTime_Lv1) + 
        Mathf.Clamp01((valueEntity.status.ChargeTime - valueEntity.status.setChargeTime_Lv1) / valueEntity.status.setChargeTime_Lv2);
        
        if(Percentage >= 2.0f)
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
        BaseShapes.AngRadiansEnd = ((Mathf.Min(1,Mathf.Max(0,Percentage)) * 360f) + 
        (Mathf.Max(0,Percentage - 1.0f) * 720f)) * Mathf.Deg2Rad;

        //チャージしてないなら消す
        ParentShapes.SetActive(Percentage > 0.5f);
        //ShowText.gameObject.SetActive(Percentage > 0.5f);

        //guiText = Mathf.FloorToInt(Percentage).ToString();
        BaseShapes.ColorStart = isFlashed ? color_flash : color_1;
        BaseShapes.ColorEnd = isFlashed ? color_flash : color_2;
        base.setValues();
    }
}
