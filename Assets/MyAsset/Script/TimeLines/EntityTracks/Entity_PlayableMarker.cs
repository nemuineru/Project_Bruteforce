

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
    
    public EntityFindType FindType;

    // generates Entity when its not null. 
    // also, I want to make the TrackBinding controls this..
    public Entity generatingEntity;

    //if generatingEntity is null, find the Entity with this name or tag or component.
    public string findName;

    public PropertyName id
    {
        get
        {
            return new PropertyName("method");
        }
    }
}

