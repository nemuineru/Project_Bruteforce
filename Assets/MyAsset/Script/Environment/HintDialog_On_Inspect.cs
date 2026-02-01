using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HintDialog_On_Inspect : MonoBehaviour
{
    public enum InspectType
    {
        Touch,
        Entering
    }
    [SerializeField]
    InspectType type;

    [SerializeField]
    OneshotDialog dialogPrefab;

    //management for showing dialogs.
    OneshotDialog dialogCurrentShowing;

    [SerializeField]
    string text;

    //if trigger is entered : 
    void OnTriggerEnter()
    {
        
    }
    
    OneshotDialog prepDialog()
    {
        if(dialogPrefab != null)
        {
            return null;
        }
        else
        {
            return null;
        }
    }
}
