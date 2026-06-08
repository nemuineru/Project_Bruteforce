using UnityEngine;
using UnityEditor;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Animancer;
using UnityEngine.Splines;

/// This script controls the playback of a Timeline asset for an Entity in Unity.
/// Focused for instantiated Entity objects, 
/// it ensures that the Timeline plays correctly when the Entity is created in the scene.
[ExecuteInEditMode]
public class Entity_TimelineController : PlayableBehaviour
{
    public PlayableDirector playableDirector;
    public TimelineAsset timelineAsset;

    public Vector3 SetRotateTo;

    public AnimationClip clip;

    public float splinePos;
    [SerializeField]
    private PlayableAssetTransition _Animation;

    public bool isPlayed = false;


    public enum EntityFindType
    {
        ByName,
        ByTag,
        ByComponent
    }

    [SerializeField]
    private string entityIdentifier;

    // Entityの検索方法 また、仮にentityが格納されていれば無視する.
    [SerializeField]
    private EntityFindType findType;

    // Entity格納
    [SerializeField]
    Entity entity;
    Spline spline;


    void Start()
    {
        entity = FindEntity();
        if (playableDirector != null && timelineAsset != null)
        {
            playableDirector.playableAsset = timelineAsset;
            playableDirector.Play();
        }
        else
        {
            Debug.LogError("PlayableDirector or TimelineAsset is not assigned.");
        }
    }


    public void SetTime(double time)
    {
        // // エラーチェック
        // if (_splineContainer == null || _duration <= 0) return;

        // // 正規化された割合計算
        // var percentage = (float)(time / _duration);

        // // 得られたスプライン位置を反映
        // entity.transform.position = _splineContainer.EvaluatePosition(percentage);
    }
    

    Entity FindEntity()
    {
        switch (findType)
        {
            case EntityFindType.ByName:
                return GameObject.Find(entityIdentifier)?.GetComponent<Entity>();
            case EntityFindType.ByTag:
                return GameObject.FindGameObjectWithTag(entityIdentifier)?.GetComponent<Entity>();
            case EntityFindType.ByComponent:
                return GameObject.FindObjectOfType<Entity>();
            default:
                Debug.LogError("Invalid EntityFindType specified.");
                return null;
        }
    }
}