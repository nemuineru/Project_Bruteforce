using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable, DisplayName("Entity Marker")]
public class Entity_PlayableMarker : Marker, INotification
{    
    public string message;

    public PropertyName id
    {
        get
        {
            return new PropertyName("method");
        }
    }
}
