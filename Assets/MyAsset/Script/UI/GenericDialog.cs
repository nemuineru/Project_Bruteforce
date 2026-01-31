

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Using TextMeshPro to rendering texts - 
//and shows like JRPG or/as Halflife style..
public class genericDialog : MonoBehaviour
{
    TMP_Text baseTMPComponent;

    //表示するテキスト
    internal string textBase;

    //表示時間
    internal float baseShowTime;

    //文字送りごとの時間
    internal float baseCharTime;

    float currentTime;

    void TextRolling()
    {
        string sText = "";
        int currentLetter = Mathf.CeilToInt(currentTime / baseCharTime);
        if (baseCharTime > 0 && textBase.Length > currentLetter)
        {
            sText = textBase.Substring(0, currentLetter);
        }
        else
        {
            sText = textBase;
        }
    }

    void ShowText()
    {
        //baseTMPComponent.text = sText;
    }

    //呼ばれた分、表示..
    IEnumerator textGenerate()
    {
        yield return null;
    }
}

