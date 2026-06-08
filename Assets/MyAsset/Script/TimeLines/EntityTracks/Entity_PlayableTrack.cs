using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[TrackBindingType(typeof(Entity))] // コントロールする対象の型
[TrackColor(.8f, .6f, .2f)] // トラックの色
[TrackClipType(typeof(Entity_PlayableClip))] // 設定できるクリップの型（複数指定可能）

//EntityのPlayableAssetにアタッチするPlayableBehaviour.
[System.Serializable]
public class Entity_PlayableTrack : TrackAsset
{
    
}