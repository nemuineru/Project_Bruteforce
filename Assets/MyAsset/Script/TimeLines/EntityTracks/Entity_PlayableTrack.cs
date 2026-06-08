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
    
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        // Mixerを作って返す
        var mixer = ScriptPlayable<Entity_PlayableMixer>.Create(graph, inputCount);
        mixer.GetBehaviour().Clips = GetClips().ToArray();
        mixer.GetBehaviour().Director = go.GetComponent<PlayableDirector>();
        return mixer;
    }

    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
        // Timelineから外したときに値を戻したい場合はこのように書く
#if UNITY_EDITOR
        Graphic trackBinding = director.GetGenericBinding(this) as Graphic;
        if (trackBinding == null)
            return;
        driver.AddFromName<Graphic>(trackBinding.gameObject, "m_Color");
#endif
    }
}