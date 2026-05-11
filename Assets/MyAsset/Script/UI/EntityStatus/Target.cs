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
    TargetComp Base;

    [SerializeField]
    TargetComp NearestSet;
    
    [SerializeField]
    TargetComp NonHit;

    public GameObject target_to;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(target_to != null)
        {
            transform.position = target_to.transform.position + Vector3.up;
        }
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward,Camera.main.transform.up);
        float dist = Vector3.Magnitude(gameState.self.Player.transform.position - transform.position);
        if(dist < 2.0f)
        {
            NearestSet.gameObject.SetActive(true);
            NonHit.gameObject.SetActive(false);
            NearestSet.transform.localRotation *= Quaternion.Euler(0,0,300.0f * Time.deltaTime);
            NearestSet.SetColors(Nearby);
            Base.SetColors(Nearby);
        }
        else if(dist < 5.0f)
        {
            NearestSet.gameObject.SetActive(false);
            NonHit.gameObject.SetActive(false);
            Base.SetColors(Ranged);
        }
        else
        {
            NearestSet.gameObject.SetActive(false);
            NonHit.gameObject.SetActive(true);
            Base.SetColors(Disabled);
            NonHit.SetColors(Disabled);
        }
    }
}
