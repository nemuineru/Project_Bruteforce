

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using UnityEngine.Playables;
using UnityEngine.Timeline;

//Entity Marker for... Entity Timeline.
[System.Serializable, DisplayName("Entity Marker")]
public class Entity_PlayableMarker : Marker, INotification
{
    public enum EntityFindType
    {
        Name,
        Tag,
        Component
    }
    
    public Entity_PlayableTrack TAsset;

    // generates Entity when its not null. 
    // also, I want to make the TrackBinding controls this..
    public Entity generatingEntity;
    
    public EntityFindType FindType;
    //if generatingEntity is null, find the Entity with this name or tag or component.
    public string findName;

    public PropertyName id => new PropertyName("CustomMarkerID");

    public override void OnInitialize(TrackAsset parentTrack)
    {
        base.OnInitialize(parentTrack);
        
        // This executes when the timeline initializes the marker
        Debug.Log($"Initialized marker on track: {parentTrack.name}");
    }
}

