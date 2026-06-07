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

    bool isStopped = false;

    public void OnControlTimeStart()
    {
        //throw new System.NotImplementedException();
    }

    // public void OnDisable()
    // {
    //     if(spls != null && entity != null)
    //     {
    //         entity.transform.position = spls.EvaluatePosition(2.0f);
    //     }
    // }

    public void OnControlTimeStop()
    {
        
    }

    public void OnBehaviourPause()
    {
        
    }

    //time will set.
    public void SetTime(double time)
    {
        if( entity != null)
        {
            
        if(spls != null && isStopped == false)
        {
            var percentage = Mathf.Clamp01((float)(time/_duration));
            Debug.Log($"Timeline_EntityTest: SetTime: time={time}, percentage={percentage}");
            entity.transform.position = spls.EvaluatePosition(percentage);
            Vector3 forward = spls.EvaluateTangent(percentage);
            float angle = Vector3.SignedAngle(Vector3.up, spls.EvaluateUpVector(percentage), forward);
            entity.transform.rotation = Quaternion.LookRotation(forward) * Quaternion.Euler(0, -angle, 0);
        }
        AnimancerComponent AM = 
            entity.animancerManager.main;
            if(AM != null && clip != null)
            AM.Play(clip);
        }
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
