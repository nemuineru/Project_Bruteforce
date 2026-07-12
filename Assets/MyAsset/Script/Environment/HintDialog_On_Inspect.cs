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
    AudioSource InspectSound;

    [SerializeField]
    string text;

    //if trigger is entered : 
    void OnTriggerEnter(Collider other)
    {
        if(type == InspectType.Entering && other.CompareTag("Player") && dialogCurrentShowing == null)
        {
            dialogCurrentShowing = prepDialog();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(type == InspectType.Touch && other.CompareTag("Player") && dialogCurrentShowing == null)
        {
            dialogCurrentShowing = prepDialog();
        }
        dialogCurrentShowing.ExtendTime(Time.deltaTime);
    }
    
    //MainUI is called : 
    OneshotDialog prepDialog()
    {
        if(InspectSound != null)
        {
            InspectSound.Play();
        }
        if(dialogCurrentShowing != null)
        {
            Destroy(dialogCurrentShowing.gameObject);
        }
        if(dialogPrefab != null)
        {
            OneshotDialog retPrefab = Instantiate(dialogPrefab).GetComponent<OneshotDialog>();
            retPrefab.transform.SetParent(GameObject.FindGameObjectWithTag("UI").transform,false);
            retPrefab.textBase = text;
            return retPrefab;
        }
        else
        {
            return null;
        }
    }
}
