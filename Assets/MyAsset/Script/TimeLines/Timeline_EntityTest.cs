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

    public void OnControlTimeStop()
    {
        //throw new System.NotImplementedException();
    }

    //time will set.
    public void SetTime(double time)
    {
        if(spls != null && entity != null)
        {
            var percentage = (float)(time/_duration);
            entity.transform.position = spls.EvaluatePosition(percentage);
            AnimancerComponent AM = 
            entity.animancerManager.main;
            if(AM != null)
            AM.Play(clip);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        entity = GameObject.FindGameObjectWithTag(EntityName).GetComponent<Entity>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
