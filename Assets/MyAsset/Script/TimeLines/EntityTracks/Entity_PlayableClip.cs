using UnityEngine;
using Animancer;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Splines;


//https://yutokun.com/writing/knowledges/unity-timeline-extension/index.htmlを参考にしたほうがええかも

//EntityのPlayableAssetにアタッチするPlayableBehaviour.
[System.Serializable]
public class Entity_PlayableClip : PlayableAsset, ITimelineClipAsset
{
    public Entity_PlayableBehavior Asset = new Entity_PlayableBehavior();

    // このクリップの特徴を定義
    public ClipCaps clipCaps {
        get {
            // ブレンドに対応、タイムスケール変更に対応
            return ClipCaps.Blending | ClipCaps.SpeedMultiplier;
        }
    }

    //Playable生成は一度のみ行われる.
    // handle先の情報パイプ...はどこ行った？
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        // BehaviourのPlayableを作って返すだけ
        // ExposedReferenceに登録された値はMixerで取得できるように.
        return ScriptPlayable<Entity_PlayableBehavior>.Create(graph);
    }
    
    public void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
        // インスペクター等でプロパティの変更を追跡させるための記述
    }
}

// //EntityのPlayableAssetにアタッチするPlayableBehaviour.
// [System.Serializable]
// public class Entity_PlayableClip : PlayableAsset, ITimelineClipAsset
// {
//     // 必ずpublic（レコードボタンが表示されない）でBehaviourを持たせる
//     public Entity_PlayableBehavior behaviour = new Entity_PlayableBehavior();

//     // このクリップの特徴を定義
//     public ClipCaps clipCaps {
//         get {
//             // ブレンドに対応、タイムスケール変更に対応
//             return ClipCaps.Blending | ClipCaps.SpeedMultiplier;
//         }
//     }

//     public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
//     {
//         // BehaviourのPlayableを作って返すだけ
//         return ScriptPlayable<Entity_PlayableBehavior>.Create(graph);
//     }
// }

