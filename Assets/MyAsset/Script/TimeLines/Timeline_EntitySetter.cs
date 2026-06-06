using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Linq;

//一先ず、指定したEntityを動かせるようにしたい
[ExecuteInEditMode]
public class Timeline_EntitySetter : PlayableBehaviour
{
    public PlayableDirector playableDirector;
    public TimelineAsset timelineAsset;

    public bool isPlayed = false;
    public Entity entity;

    void Start()
    {
        
    }
}
