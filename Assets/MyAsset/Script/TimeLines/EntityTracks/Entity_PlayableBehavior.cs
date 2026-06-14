using Animancer;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Splines;

//EntityのPlayableAssetにアタッチするPlayableBehaviour.
[System.Serializable]
public class Entity_PlayableBehavior : PlayableBehaviour
{
    [SerializeField]
    //Animancerのクリップシーケンス等を登録.
    public TransitionAsset transitter;

    [SerializeField]
    //移動情報.
    public ExposedReference<SplineContainer> spline;
}