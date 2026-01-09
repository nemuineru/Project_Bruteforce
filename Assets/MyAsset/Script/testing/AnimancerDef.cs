using UnityEngine;
using Animancer;
using System.Collections.Generic;
using System;

//first, we ned to 
public class AnimancerDef : MonoBehaviour
{
    [SerializeField]
    List<Vector2> ParamPos;
    [System.Serializable]
    public class clipSetting
    {        
        [SerializeField]
        string ClipName;
        [SerializeField]
        internal AnimationClip clip;    

        [SerializeField]
        internal Vector2 paramPos;
    }

    [System.Serializable]
    public class clipGroup
    {        
        [SerializeField]
        internal List<clipSetting> clips;    
        [SerializeField]
        internal mixType mixtype;
		
		public enum mixType
		{
			Direct,
			Linear,
			Simple_Directional,
			Free_Directional,
			Free_Cartestan
		}
    }
    
    [SerializeField]
    public List<clipGroup> _clipgroup;
    
    [SerializeField]
    AnimancerComponent _mainAnimComponent;

    //自作のPlayableAPIを通したアニメ変更からAnimancerでの変更にする際、Layer/State/Clipの登録が必須
    //Layerは作成されたときに例外処理無しで動く。

    //まず、StateDefで指定したAnimSlot..はLayerとして管理する.
    //Layerの配列アクセス時、作成されていないレイヤーを指定しても例外にならないのは良いね

    //で、Layer内に登録させるStatesは
    // [Direct, Linear1D, Cartestan/Directional Freeform 2D]
    //の4つのMixingが可能となってる.

    //StateをGetTypeで取得して、その種類で挙動を変える、っていうのは可能そうね
    
    //ミキシング実験はOK. 次はAdditiveをやってみる.
    //[State]

    void Start()
    {
        ChangeAnims();
    }

    void Update()
    {
        AnimancerLayer Layer1_ = _mainAnimComponent.Layers[0];
        if (Layer1_ != null)
        {
            Type StateType = Layer1_.CurrentState.GetType();
            //Layer1_.CurrentState.Parameter = ParamPos;
        }
    }

    void ChangeAnims()
    {
        AnimancerLayer Layer1_ = _mainAnimComponent.Layers[0];
        
        AnimancerState st = new DirectionalMixerState();

        //get first clipGroup, then add the 2D Cartestian State position.
        foreach (clipSetting CLI in _clipgroup[0].clips)
        {
            ((LinearMixerState)st).Add(CLI.clip, CLI.paramPos.x);            
        }

        Layer1_.Play(st);

        //AnimancerState PlayState = new AnimancerState();
        //Layer1_.Play(PlayState);
    }
}