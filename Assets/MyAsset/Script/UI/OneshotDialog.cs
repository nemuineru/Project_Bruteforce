using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

//textComponent must be child inside of this gobj.
public class OneshotDialog : MonoBehaviour
{
    internal TMP_Text textComponent;

    internal string textBase = "Quick Brown Fox Jumps over the dog";

    internal float baseShowTime = 5.5f;
    internal float fadingTime = 0.6f;

    internal Color baseColor = Color.white;

    internal Color TransitColor = new Color(1f,0.9f,0.5f);

    internal float baseCharTime = 0.02f;
    float currentTime = 0;


    // Start is called before the first frame update
    void Start()
    {
        if(textComponent == null)
        {
            textComponent = GetComponentInChildren<TMP_Text>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        string tShow = "";

        if(textComponent != null)
        {
            int currentLetter = Mathf.CeilToInt(currentTime / baseCharTime);
            int transiter = Mathf.CeilToInt(255 - 255 * Mathf.Clamp01(((currentTime + fadingTime) - baseShowTime)/fadingTime));
            //currentLetterはTransitColorで表記, それ以外はbaseColorで..
            if (baseCharTime > 0 && textBase.Length > currentLetter - 1)
            {
                int Letters = Mathf.Clamp(currentLetter , 0, textBase.Length - 1);
                tShow = string.Format("<color=#{0}{1}>", UnityEngine.ColorUtility.ToHtmlStringRGB(baseColor), transiter.ToString("X2")) 
                + textBase.Substring(0, Letters) + 
                string.Format("<color=#{0}{1}>",UnityEngine.ColorUtility.ToHtmlStringRGB(TransitColor),transiter.ToString("X2"))
                + textBase.Substring(Letters, 1) ;
            }
            else
            {
                tShow = tShow = string.Format("<color=#{0}{1}>", UnityEngine.ColorUtility.ToHtmlStringRGB(baseColor), transiter.ToString("X2"))
                + textBase;
            }
            textComponent.text = tShow;
        }
        currentTime += Time.fixedDeltaTime;
        if(baseShowTime < currentTime)
        {
            Destroy(gameObject);
        }
    }
}
