

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
        //Mixing用に重なり合ったClipsを変換..
        mixer.GetBehaviour().Clips = GetClips().ToArray();
        mixer.GetBehaviour().Director = go.GetComponent<PlayableDirector>();
        return mixer;
    }

    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
        // Timelineから外したときに値を戻したい場合はこのように書く
        #if UNITY_EDITOR
            Entity trackBinding = director.GetGenericBinding(this) as Entity;
            if (trackBinding == null)
                return;// Transform のローカル位置をプレビュー対象として登録
            driver.AddFromName<Transform>("m_LocalPosition.x");
            driver.AddFromName<Transform>("m_LocalPosition.y");
            driver.AddFromName<Transform>("m_LocalPosition.z");
            
            // Transform のローカル回転を登録する場合
            driver.AddFromName<Transform>("m_LocalRotation.x");
            driver.AddFromName<Transform>("m_LocalRotation.y");
            driver.AddFromName<Transform>("m_LocalRotation.z");
            driver.AddFromName<Transform>("m_LocalRotation.w");
        #endif
    }
}

