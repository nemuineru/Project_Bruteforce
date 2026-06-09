

using Animancer;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Splines;
using UnityEngine.Timeline;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class Entity_PlayableMixer : PlayableBehaviour
{ 
    public TimelineClip[] Clips { get; set; }

    public PlayableDirector Director { get; set; }

    //playerDataにはEntity情報が入る.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var controllingEntity = playerData as Entity;
        if (controllingEntity == null)
        {
            return;
        }

        var time = Director.time; // Timeline全体の現在の時間
        Quaternion setRots = Quaternion.identity;
        Vector3 setPos = Vector3.zero;
        List<AnimancerState> states = new List<AnimancerState>();
        //Clipは恐らくブレンド対象のクリップをそれぞれブレンドしてると考える..と PlayableAssetの組み合わせを与えるには...?
        for (int i = 0; i < Clips.Length; i++)
        {
            var clip = Clips[i];
            var clipAsset = clip.asset as Entity_PlayableClip; // クリップのアセット
            var behaviour = clipAsset.behaviour; // クリップが持つBehaviour
            var clipWeight = playable.GetInputWeight(i); // クリップのブレンドウェイト
            var clipProgress = (float)((time - clip.start) / clip.duration); // クリップの進行率
            SplineContainer Spline = new SplineContainer();//clipAsset.behaviour.spline.Resolve();

            //有るクリップの設定値を考える.
            if (clipProgress >= 0.0f && clipProgress <= 1.0f)
            {
                //set clip position.
                Vector3 position = Spline.EvaluateTangent(clipProgress);
                setPos += position * clipWeight;

                //set forward position.
                Vector3 forwards = Spline.EvaluateTangent(clipProgress);
                float angle = Vector3.SignedAngle(Vector3.up, Spline.EvaluateUpVector(clipProgress), forwards);
                Quaternion RotateClip = Quaternion.LookRotation(forwards) * Quaternion.Euler(0, -angle, 0);
                if (i == 0)
                {
                    setRots = Quaternion.Slerp(setRots, RotateClip, clipWeight);
                }
                else
                {
                    setRots = Quaternion.SlerpUnclamped(setRots, RotateClip, clipWeight);
                }

                //Animancer State Set. mixes after the blenders.
                TransitionAsset Transit = clipAsset.behaviour.transitter;
                //CreateState will inheriet the transitionAsset setting.. I guess
                AnimancerState st = Transit.CreateState();
                st.NormalizedTime = clipProgress;
                st.Weight = st.Weight * clipWeight;
                states.Add(st);
                //Rots = 
                //color += Color.Lerp(clipAsset.behaviour.startColor, clipAsset.behaviour.endColor, clipProgress) * clipWeight;
            }
        }

        //play those states
        //need to add those mixer States..
        //ok so Manual State is better?
        ManualMixerState mms = new ManualMixerState();
        mms.AddRange(states.ToArray());
        controllingEntity.animancerManager.main.Play(mms);
        //controllingEntity.animancerManager.main._Evaluate();

        // controllingEntity.animancerManager.main._Evaluate(mms);

        //graphic.color = color;
    }
}

