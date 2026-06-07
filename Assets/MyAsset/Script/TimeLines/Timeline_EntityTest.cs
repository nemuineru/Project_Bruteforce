using System.Collections;
using System.Collections.Generic;
using Animancer;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.Timeline;

[ExecuteInEditMode]
public class Timeline_EntityTest : MonoBehaviour, ITimeControl
{
    public string EntityName;
    public Entity entity;

    [SerializeField]
    public TransitionAsset clip;
    
    //spline container
    [SerializeField]
    SplineContainer spls;

    //dulations
    [SerializeField] private double _duration = 1;
    public void OnControlTimeStart()
    {
        //throw new System.NotImplementedException();
    }

    public void OnDisable()
    {
        if(spls != null && entity != null)
        {
            entity.transform.position = spls.EvaluatePosition(1.0f);
        }
    }

    public void OnControlTimeStop()
    {
        Debug.Log("Timeline_EntityTest: OnControlTimeStop");
        if(spls != null && entity != null)
        {
            entity.transform.position = spls.EvaluatePosition(1.0f);
        }
        //throw new System.NotImplementedException();
    }

    public void OnControlTimeEnd()
    {
        if(spls != null && entity != null)
        {
            entity.transform.position = spls.EvaluatePosition(1.0f);
        }
        //throw new System.NotImplementedException();
    }

    //time will set.
    public void SetTime(double time)
    {
        if(spls != null && entity != null)
        {
            var percentage = (float)(time/_duration);
            if(time >= _duration)
            {
                percentage = 1;
            }
            entity.transform.position = spls.EvaluatePosition(percentage);
            Vector3 forward = spls.EvaluateTangent(percentage);
            float angle = Vector3.SignedAngle(Vector3.up, spls.EvaluateUpVector(percentage), forward);
            entity.transform.rotation = Quaternion.LookRotation(forward) * Quaternion.Euler(0, -angle, 0);
        }
        AnimancerComponent AM = 
            entity.animancerManager.main;
            if(AM != null)
            AM.Play(clip);
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject fObj = GameObject.FindGameObjectWithTag(EntityName);
        if(fObj != null)
        {
            entity = fObj.GetComponent<Entity>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
