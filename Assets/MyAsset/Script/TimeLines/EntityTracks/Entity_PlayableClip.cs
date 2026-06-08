using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

//EntityのPlayableAssetにアタッチするPlayableBehaviour.
[System.Serializable]
public class Entity_PlayableClip : PlayableAsset, ITimelineClipAsset
{
    // 必ずpublic（レコードボタンが表示されない）でBehaviourを持たせる
    public Entity_PlayableBehavior behaviour = new Entity_PlayableBehavior();

    // このクリップの特徴を定義
    public ClipCaps clipCaps {
        get {
            // ブレンドに対応、タイムスケール変更に対応
            return ClipCaps.Blending | ClipCaps.SpeedMultiplier;
        }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        // BehaviourのPlayableを作って返すだけ
        return ScriptPlayable<Entity_PlayableBehavior>.Create(graph);
    }
}