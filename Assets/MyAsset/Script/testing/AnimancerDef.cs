using UnityEngine;
using Animancer;
using System.Collections.Generic;

//first, we ned to 
public class AnimancerDef : MonoBehaviour
{
    [System.Serializable]
    public class clipSetting
    {        
        [SerializeField]
        string ClipName;
        [SerializeField]
        AnimationClip clip;    

        [SerializeField]
        Vector2 paramPos;
    }

    [System.Serializable]
    public class clipGroup
    {        
        [SerializeField]
        internal List<clipSetting> clips;    
    }
    
    [SerializeField]
    public List<clipGroup> _clipgroup;
    
    AnimancerState _state;

    void ChangeAnims()
    {
        
    }
}