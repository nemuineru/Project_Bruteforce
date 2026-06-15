using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

//Change Tracks.
public class TimelineSignal_SetEntityTracks : MonoBehaviour
{

    [SerializeField] private PlayableDirector director;
    [SerializeField] private Entity newTargetObject;
    [SerializeField] private string EntityTagName;
    [SerializeField] private string trackName = "Animation Track";


    
    // Start is called before the first frame update
    void Start()
    {
        SetObjByName(EntityTagName);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void SetObjByName(string strs)
    {        
        if(strs != "")
        {
            GameObject fObj = GameObject.FindGameObjectWithTag(strs);
            if(fObj != null)
            {
                newTargetObject = fObj.GetComponent<Entity>();
            }
        }
    }

    public void SetEntityTracks(string strs)
    {
        if(strs != null)
        {
            SetObjByName(strs);
        }
        //find directors first.
        if(director == null)
        return;

        //then find TimeLineAssets.
        TimelineAsset timelineAsset = director.playableAsset as TimelineAsset;
        if(timelineAsset == null) return;

        //Find by name.
        TrackAsset trs = timelineAsset.GetOutputTracks().FirstOrDefault(track => track.name == trackName);
        if(trs != null)
        {
            director.SetGenericBinding(trs, newTargetObject);
            director.RebindPlayableGraphOutputs();
        }
    }
}
