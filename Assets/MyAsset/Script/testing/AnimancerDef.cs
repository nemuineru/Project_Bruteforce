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
        internal AnimationClip clip;    

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
    
    [SerializeField]
    AnimancerComponent _mainAnimComponent;

    //まず、StateDefで指定したAnimSlot..はLayerとして管理する.
    //Layerの配列アクセス時、作成されていないレイヤーを指定しても例外にならないのは良いね

    //で、Layer内に登録させるStatesは
    // [Direct, Linear1D, Cartestan/Directional Freeform 2D]
    //の4つのMixingが可能となってる.
    
    void ChangeAnims()
    {
        AnimancerLayer Layer1_ = _mainAnimComponent.Layers[0];
        
        AnimancerState st = new CartesianMixerState();

        //get first clipGroup.
        foreach(clipSetting CLI in _clipgroup[0].clips)
        {
            ((CartesianMixerState)st).Add(CLI.clip);
        }

        Layer1_.Play(st);

        //AnimancerState PlayState = new AnimancerState();
        //Layer1_.Play(PlayState);
    }
}