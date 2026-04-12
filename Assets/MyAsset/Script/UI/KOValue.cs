using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KOValue : MonoBehaviour
{
    public TextMeshProUGUI KOTxt;

    int koNum = 0;
    float rotMaxTime = 0.24f;
    float rotTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        rotTime = rotMaxTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(koNum != gameState.self.KillNo)
        {
            rotTime = 0f;
            koNum = gameState.self.KillNo;
        }
        rotTime = Mathf.Clamp(rotTime,0f,rotMaxTime);
        KOTxt.transform.localRotation = Quaternion.Lerp(KOTxt.transform.localRotation , Quaternion.Euler(0,720f * (rotTime / rotMaxTime),0) , 0.4f);
        KOTxt.text = koNum.ToString();
        rotTime += Time.deltaTime;
    }
}
