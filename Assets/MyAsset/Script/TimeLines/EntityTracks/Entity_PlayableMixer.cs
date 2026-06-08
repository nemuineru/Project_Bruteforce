using Animancer;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Splines;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class Entity_PlayableMixer : PlayableBehaviour
{ 
    public TimelineClip[] Clips { get; set; }
    public PlayableDirector Director { get; set; }

    //playerDataにはEntity情報が入る.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var controllingEntity = playerData as Entity;
        if (controllingEntity == null) {
            return;
        }

        var time = Director.time; // Timeline全体の現在の時間
        Quaternion Rots = Quaternion.identity;
        Vector3 positions = Vector3.zero;
        //Clipは恐らくブレンド対象のクリップをそれぞれブレンドしてると考える..と PlayableAssetの組み合わせを与えるには...?
        for (int i = 0; i < Clips.Length; i++) {
            var clip = Clips[i];
            var clipAsset = clip.asset as Entity_PlayableClip; // クリップのアセット
            var behaviour = clipAsset.behaviour; // クリップが持つBehaviour
            var clipWeight = playable.GetInputWeight(i); // クリップのブレンドウェイト
            var clipProgress = (float)((time - clip.start) / clip.duration); // クリップの進行率

            //有るクリップの設定値を考える.
            if (clipProgress >= 0.0f && clipProgress <= 1.0f) {
                AnimancerComponent AMS = new AnimancerComponent();
                Vector3 forwards = clipAsset.behaviour.spline.EvaluateTangent(clipProgress);
                Vector3 positon = clipAsset.behaviour.spline.EvaluateTangent(clipProgress);
                float angle = Vector3.SignedAngle(Vector3.up, clipAsset.behaviour.spline.EvaluateUpVector(clipProgress), forwards);
                Quaternion rots = Quaternion.LookRotation(forwards) * Quaternion.Euler(0, -angle, 0);

                //Rots = 
                //color += Color.Lerp(clipAsset.behaviour.startColor, clipAsset.behaviour.endColor, clipProgress) * clipWeight;
            }
        }

        //graphic.color = color;
    }
}