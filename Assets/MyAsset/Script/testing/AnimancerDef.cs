using UnityEngine;
using Animancer;
using System.Collections.Generic;
using System;

//first, we ned to 
public class AnimancerDef : MonoBehaviour
{
    [SerializeField]
    int ChangeIndex = 0;

    int prevIndex = 0;

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

        [SerializeField]
        internal AvatarMask masker;
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
    //[StateLayer]が1以上を対象に、Additiveとして処理.

    //Mixing出来たので次はアレだー。
    //Layerの自動作成と切り替え..

    void Start()
    {
        ChangeAnims();
    }

    void Update()
    {
        AnimancerLayer Layer1_ = _mainAnimComponent.Layers[0];
        if (Layer1_ != null && prevIndex != ChangeIndex)
        {
            MakeAnims(ChangeIndex);
            //Type StateType = Layer1_.CurrentState.GetType();
            //Layer1_.CurrentState.Parameter = ParamPos;
            prevIndex = ChangeIndex;
        }
    }

    void MakeAnims(int selectedAnimID)
    {
        AnimancerLayer Layer = _mainAnimComponent.Layers[0];
        if(ChangeIndex < _clipgroup.Count)
        {
        clipGroup SelectedGroup = _clipgroup[ChangeIndex];
        
        }

    }

    void ChangeAnims()
    {
        AnimancerLayer Layer_Main = _mainAnimComponent.Layers[0];
    }
    
    AnimancerState MakeState()
    {
        AnimancerState st = new LinearMixerState();
        return st;
    }

    void LayerAnims()
    {
        AnimancerLayer Layer1_ = _mainAnimComponent.Layers[0];
        AnimancerLayer Layer2_ = _mainAnimComponent.Layers[1];
        
        AnimancerState st_1 = new LinearMixerState();
        AnimancerState st_2 = new LinearMixerState();

        //get first clipGroup, assign at [0](base), then add the 2D Cartestian State position.
        foreach (clipSetting CLI in _clipgroup[0].clips)
        {
            ((LinearMixerState)st_1).Add(CLI.clip, CLI.paramPos.x);            
        }


        //second clipGroup needs to be assigned at Layer [1] which set as Additive.
        Layer2_.IsAdditive = true;
        Layer2_.Weight = 1.0f;
        Layer2_.Mask = _clipgroup[1].masker;
        foreach (clipSetting CLI in _clipgroup[1].clips)
        {
            ((LinearMixerState)st_2).Add(CLI.clip, CLI.paramPos.x);            
        }

        Layer1_.Play(st_1);
        Layer2_.Play(st_2);

        //AnimancerState PlayState = new AnimancerState();
        //Layer1_.Play(PlayState);
    }
}