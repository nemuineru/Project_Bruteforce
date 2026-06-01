using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityQuaternion;
using UnityEngine;
using Shapes;
using System;

public class Target : MonoBehaviour
{
    [SerializeField]
    Color Nearby;

    [SerializeField]
    Color Ranged;

    [SerializeField]
    Color Disabled;

    [SerializeField]
    TargetComp MainTarget;

    [SerializeField]
    TargetComp Base;

    [SerializeField]
    TargetComp NearestSet;
    
    [SerializeField]
    TargetComp NonHit;

    public GameObject MainTarget_to;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(MainTarget_to != null)
        {
            MainTarget.gameObject.SetActive(true);
            MainTarget.transform.localScale = Vector3.Lerp(MainTarget.transform.localScale,Vector3.one * 0.75f, 0.2f);
        }
        else
        {
            MainTarget.transform.localScale = Vector3.Lerp(MainTarget.transform.localScale,Vector3.one * 200f, 0.05f);
            if(MainTarget.transform.localScale.x > 80f)
            {
                MainTarget.gameObject.SetActive(false);
            }
        }
        //set base rotation.
        
            Quaternion RandRotate = Quaternion.Euler(1f,-1f * UnityEngine.Random.value ,1f) ;
            MainTarget.transform.rotation *= RandRotate;

        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward,Camera.main.transform.up);
        float dist = Vector3.Magnitude(gameState.self.Player.transform.position - transform.position);
        if(dist < 2.0f)
        {
            NearestSet.gameObject.SetActive(true);
            NonHit.gameObject.SetActive(false);
            NearestSet.transform.localRotation *= Quaternion.Euler(0,0,300.0f * Time.deltaTime);
            NearestSet.SetColors(Nearby);
            MainTarget.SetColors(Nearby);
            Base.SetColors(Nearby);
        }
        else if(dist < 5.0f)
        {
            NearestSet.gameObject.SetActive(false);
            NonHit.gameObject.SetActive(false);
            MainTarget.SetColors(Ranged);
            Base.SetColors(Ranged);
        }
        else
        {
            NearestSet.gameObject.SetActive(false);
            NonHit.gameObject.SetActive(true);
            MainTarget.SetColors(Disabled);
            Base.SetColors(Disabled);
            NonHit.SetColors(Disabled);
        }
    }
}
