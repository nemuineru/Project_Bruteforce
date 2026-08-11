using Animancer;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Splines;
using UnityEngine.Timeline;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

//PlayableBehaviorに対し、登録を行う..
public class Entity_PlayableMixer : PlayableBehaviour
{
    public TimelineClip[] Clips { get; set; }

    public PlayableDirector Director { get; set; }

    //playerDataにはEntity情報が入る.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        //PlayableGraphからExposedReferenceの値を取得するためのResolverを取得してー.
        var propertyTable = playable.GetGraph().GetResolver();
        //EntityのNullチェック.
        var controllingEntity = playerData as Entity;
        var time = Director.time; // Timeline全体の現在の時間   
        if (controllingEntity == null)
        {
            return;
        }
        // Clipは恐らくブレンド対象のクリップをそれぞれブレンドしてると考える..と 
        // PlayableAssetの組み合わせを与えるには...?
        Vector3 position =  controllingEntity.transform.position;
        Quaternion setRots = controllingEntity.transform.rotation;
        List<AnimancerState> mms = new List<AnimancerState>();
        bool isClipPlayed = false;
        //Track中のClipを見ていくため考慮‥
        for (int i = 0; i < Clips.Length; i++)
        {
            //それぞれのAnimClipAsset..
            var clip = Clips[i];
            var clipProgress = (float)((time - clip.start) / clip.duration); // クリップの進行率
            var clipTimePassed = (float)(time - clip.start) ; // クリップの経過時間
            //Assetに登録.
            var clipAsset = clip.asset as Entity_PlayableClip;

            var clipWeight = playable.GetInputWeight(i); // クリップのブレンドウェイト

            //SplineContainerをExposedReferenceから取得.
            SplineContainer Container = clipAsset.Asset.spline.Resolve(propertyTable);

            //TransitionAssetも同様に取得.
            TransitionAsset Transit = clipAsset.Asset.transitter;

            //クリップの再生位置が超えていれば無視される.
            if(clipProgress < 0.0f || clipProgress > 1.0f)
            {
                continue;
            }
            //クリップウェイトが0であれば無視される. もち、実装予定のAnimancerTransitionAssetも同様.
            else if(clipWeight > 0.0f)
            {
                position = !isClipPlayed ? Vector3.zero : position;
                setRots = !isClipPlayed ? Quaternion.identity : setRots;
                isClipPlayed = true;
            }

            if(Container != null && clipWeight > 0.0f)
            {
                //set clip position.
                Vector3 vt = Container.EvaluatePosition(clipProgress) * clipWeight;
                position += vt;

                //set forward position.
                Vector3 SplineForwards = Container.EvaluateTangent(clipProgress);
                Vector3 PlanedForwards = Vector3.ProjectOnPlane(SplineForwards, Vector3.up);
                float angle = Vector3.SignedAngle(Vector3.up, Container.EvaluateUpVector(clipProgress), SplineForwards);
                Quaternion RotateClip = Quaternion.LookRotation(PlanedForwards) * Quaternion.Euler(0, -angle, 0);
                if (i == 0)
                {
                    setRots = Quaternion.Slerp(setRots, RotateClip, clipWeight);
                }
                else
                {
                    setRots = Quaternion.SlerpUnclamped(setRots, RotateClip, clipWeight);
                }
            }

            //Rotationは出来てるので問題は Animancerのちゃんとした移行メカニズムなんだよな.
            //stにWeightを指定することでTransitionの組分けは可能
            //Time..がアレ.
            if(Transit != null && clipWeight > 0.0f && clipProgress < 1.0)
            {
                // TransitionAsset Transit = clipAsset.behaviour.transitter;
                //CreateState will inheriet the transitionAsset setting.. I guess
                AnimancerState st = Transit.CreateState();
                st.Time = clipTimePassed;
                //Debug.Log("Time is " + clipTimePassed);
                st.Weight = clipWeight;
                mms.Add(st);
            }
        }
        
        //設定済みの位置と回転をEntityに反映.
        //TimelineがEndならStop.
        if(Director.time / Director.duration < 1.0f)
        {
            controllingEntity.transform.position = Vector3.Lerp(controllingEntity.transform.position,position, 0.5f);
            controllingEntity.transform.rotation = setRots;
            //Debug.Log("clip played");
            
            if(mms.Count() > 0 && controllingEntity.animancerManager != null)
            {
                //mms.Time = (float)time;
                // AnimancerState st = controllingEntity.animancerManager.main.Play(mms);
                // st.Weight = 1.0f;  
                // st.Time = (float)time;
                controllingEntity.animancerManager.TimelineAnimLoad(mms);
            }
        }
    }

    // void Process()
    // {
    //     var controllingEntity = playerData as Entity;
    //     if (controllingEntity == null)
    //     {
    //         return;
    //     }

    //     var time = Director.time; // Timeline全体の現在の時間
    //     Quaternion setRots = Quaternion.identity;
    //     Vector3 setPos = Vector3.zero;
    //     List<AnimancerState> states = new List<AnimancerState>();
    //     //Clipは恐らくブレンド対象のクリップをそれぞれブレンドしてると考える..と PlayableAssetの組み合わせを与えるには...?
    //     for (int i = 0; i < Clips.Length; i++)
    //     {
    //         var clip = Clips[i];
    //         var clipAsset = clip.asset as Entity_PlayableClip; // クリップのアセット
    //         var behaviour = clipAsset.behaviour; // クリップが持つBehaviour
    //         var clipWeight = playable.GetInputWeight(i); // クリップのブレンドウェイト
    //         var clipProgress = (float)((time - clip.start) / clip.duration); // クリップの進行率
    //         SplineContainer Spline = behaviour.spline;

    //         //有るクリップの設定値を考える.
    //         if (clipProgress >= 0.0f && clipProgress <= 1.0f)
    //         {
    //             //set clip position.
    //             Vector3 position = Spline.EvaluateTangent(clipProgress);
    //             setPos += position * clipWeight;

    //             //set forward position.
    //             Vector3 forwards = Spline.EvaluateTangent(clipProgress);
    //             float angle = Vector3.SignedAngle(Vector3.up, Spline.EvaluateUpVector(clipProgress), forwards);
    //             Quaternion RotateClip = Quaternion.LookRotation(forwards) * Quaternion.Euler(0, -angle, 0);
    //             if (i == 0)
    //             {
    //                 setRots = Quaternion.Slerp(setRots, RotateClip, clipWeight);
    //             }
    //             else
    //             {
    //                 setRots = Quaternion.SlerpUnclamped(setRots, RotateClip, clipWeight);
    //             }

    //             //Animancer State Set. mixes after the blenders.
    //             // TransitionAsset Transit = clipAsset.behaviour.transitter;
    //             //CreateState will inheriet the transitionAsset setting.. I guess
    //             // AnimancerState st = Transit.CreateState();
    //             // st.NormalizedTime = clipProgress;
    //             // st.Weight = st.Weight * clipWeight;
    //             // states.Add(st);
    //             //Rots = 
    //             //color += Color.Lerp(clipAsset.behaviour.startColor, clipAsset.behaviour.endColor, clipProgress) * clipWeight;
    //         }
    //     }
    //     controllingEntity.transform.position = setPos;
    //     controllingEntity.transform.rotation = setRots;

    //     //play those states
    //     //need to add those mixer States..
    //     //ok so Manual State is better?
    //     // ManualMixerState mms = new ManualMixerState();
    //     // mms.AddRange(states.ToArray());
    //     // controllingEntity.animancerManager.main.Play(mms);
    //     //controllingEntity.animancerManager.main._Evaluate();

    //     // controllingEntity.animancerManager.main._Evaluate(mms);

    //     //graphic.color = color;
    // }
}


//Mr Google shows me the wat of coding
//is it really worth to understand, or goes slop..
/*
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Splines;

public class SplineBlendBehaviour : PlayableBehaviour
{
    public ExposedReference<SplineContainer> startSplineRef;
    public ExposedReference<SplineContainer> endSplineRef;
    [Range(0f, 1f)] public float blendWeight;

    public SplineContainer targetSplineContainer;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (targetSplineContainer == null) 
            targetSplineContainer = playerData as SplineContainer;

        if (targetSplineContainer == null) return;

        var propertyTable = playable.GetGraph().GetResolver();
        SplineContainer startSpline = startSplineRef.Resolve(propertyTable);
        SplineContainer endSpline = endSplineRef.Resolve(propertyTable);

        if (startSpline == null || endSpline == null) return;

        // Assuming both splines have matching knot counts for simple blending
        var targetSpline = targetSplineContainer.Spline;
        targetSpline.Clear();

        for (int i = 0; i < startSpline.Spline.Count; i++)
        {
            BezierKnot startKnot = startSpline.Spline[i];
            BezierKnot endKnot = endSpline.Spline[i];

            // Lerp positions and tangents
            Vector3 pos = Vector3.Lerp(startKnot.Position, endKnot.Position, blendWeight);
            Vector3 tanIn = Vector3.Lerp(startKnot.TangentIn, endKnot.TangentIn, blendWeight);
            Vector3 tanOut = Vector3.Lerp(startKnot.TangentOut, endKnot.TangentOut, blendWeight);

            targetSpline.Add(new BezierKnot(pos, tanIn, tanOut));
        }
    }
}
*/

/*
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Splines;

[System.Serializable]
public class SplineBlendAsset : PlayableAsset
{
    public ExposedReference<SplineContainer> startSpline;
    public ExposedReference<SplineContainer> endSpline;
    [Range(0f, 1f)] public float blendWeight = 0.5f;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var scriptPlayable = ScriptPlayable<SplineBlendBehaviour>.Create(graph);
        var blendBehaviour = scriptPlayable.GetProcessObject();

        blendBehaviour.startSplineRef = startSpline;
        blendBehaviour.endSplineRef = endSpline;
        blendBehaviour.blendWeight = blendWeight;

        return scriptPlayable;
    }
}
*/

/*
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Splines;

[TrackColor(0.2f, 0.8f, 0.4f)]
[TrackClipType(typeof(SplineBlendAsset))]
[TrackBindingType(typeof(SplineContainer))]
public class SplineBlendTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject owner, int inputCount)
    {
        return ScriptPlayable<SplineBlendMixer>.Create(graph, inputCount);
    }
}

public class SplineBlendMixer : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        SplineContainer targetSpline = playerData as SplineContainer;
        if (targetSpline == null) return;

        int inputCount = playable.GetInputCount();
        float blendedWeight = 0f;
        Vector3[] accumulatedPositions = null;

        // Blend logic over active timeline clips
        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            var inputPlayable = (ScriptPlayable<SplineBlendBehaviour>)playable.GetInput(i);
            SplineBlendBehaviour inputB = inputPlayable.GetProcessObject();

            if (inputB != null && inputWeight > 0f)
            {
                // Apply your weighted math directly onto targetSpline.Spline here
                blendedWeight += inputB.blendWeight * inputWeight;
            }
        }
    }
}
*/

