using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelNameShow : MonoBehaviour
{
    public TextMeshProUGUI UGUIText;

    // Update is called once per frame
    void Update()
    {
        if(EWaveManager.self != null)
        {
            EWaveManager EW = EWaveManager.self;
            if(EW.gameType != EWaveManager.GameType.Practice)
            {
                UGUIText.text = "Wave " + EW.currentLevel + " / "+ EW.MaxLevel;
            }
            else
            {
                UGUIText.text = "Practice";
            } 
        }
    }
}
