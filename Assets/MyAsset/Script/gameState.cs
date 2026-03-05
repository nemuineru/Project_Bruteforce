using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
//using UnityEditor.SearchService;
using System.Linq;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimator;
using TMPro;
using UnityEngine.SceneManagement;
using Cinemachine;

public class gameState : MonoBehaviour
{
    //操作対象のEntity.
    internal Entity Player;

    //全部のスクリプトからアクセスするように.
    //
    static public gameState self;

    //敵のHP管理バー.
    public GameObject EnemyHPUI;

    public GameObject defaultEff;
    public GameObject defaultGuardEff;
    public GameObject defaultDeathEff;


    public AudioSource inGameAuds;

    public Status_MainUI MainGUI;
    public GameObject Player_Instantiate;

    public CinemachineVirtualCamera Player_Vcam;

    public List<Entity> entityList;

    public List<Props> propList;

    public Transform InitSpawnPos;


    internal float elapsedTime = 0f;
    void Awake()
    {
        if (self == null)
        {
            self = this;
        }
        else
        {
            Destroy(gameObject);
        }
        Transform tr = InitSpawnPos != null ? InitSpawnPos : gameObject.transform;
        Quaternion qt = Quaternion.LookRotation(tr.forward);
        Player = Instantiate(Player_Instantiate,tr.position,qt).GetComponent<Entity>();
        
        MainGUI.SetComponent(Player);

        GameObject objs = Instantiate(MainGUI.gameObject);
        GameObject GUI_Top = GameObject.FindGameObjectWithTag("UI");
        if(Player_Vcam != null)
        {
            CinemachineVirtualCamera cams = Instantiate(Player_Vcam);
            cams.LookAt = Player.transform;
            cams.Follow = Player.transform;
            //生成位置の後ろ側を指定.
            cams.ForceCameraPosition(tr.position - tr.transform.forward + Vector3.up,qt);
            Player.vCam = GameObject.FindGameObjectWithTag("Virtual_MainCamera").GetComponent<CinemachineVirtualCamera>();
        }
        objs.transform.SetParent(GUI_Top.transform, false);
    }

    void Update()
    {
        elapsedTime += Time.fixedDeltaTime;
        entityList = FindObjectsByType<Entity>(FindObjectsSortMode.None)
        .OrderBy(t => !t.attrs.alive)
        .ThenBy(t => Vector3.Magnitude(t.transform.position - Player.transform.position))
        .ToList();

        propList = FindObjectsByType<Props>(FindObjectsSortMode.None)
        .OrderBy(t => Vector3.Magnitude(t.transform.position - Player.transform.position))
        .ToList();
    }


    //HitDefを発火する際のイベント
    public bool ProvokeHitDef_Entity(Entity provokerEntity, hitDefParams hitDefParams)
    {
        bool ret = false;
        int refNumRemaining;
        hitDefParams useParam = new hitDefParams();        
        if (hitDefParams != null)
        {
            useParam = hitDefParams;
        }

        useParam.ownerEntity = provokerEntity;

        refNumRemaining = useParam.maxEntityHits;

        foreach (Entity e in entityList)
        {
            //selfには反応しない. また当たる数が設定されているなら0にならない限り設定される.
            if (e != provokerEntity && e.tag != provokerEntity.tag && refNumRemaining > 0)
            {
                //それぞれのentityの現在再生中のAnimatorが持つClssに対して衝突判定.
                //また、entityの無敵判定に関しても考える.
                bool f = provokerEntity.hitCheck(e, out Vector3 HitPt);
                bool isContactable = hitDefParams.HitMoveFlag.Contains(e.moveType.ToString()) &&
                hitDefParams.HitPhysFlag.Contains(e.physicsType.ToString()) &&
                !hitDefParams.HitExcludeList.Contains(e.CurrentStateID);
                //hitしたなら一先ずAnim番号を5000に飛ばしたい. ChangeState(5000)の最優先Queueとして組み込む.
                if (f == true && isContactable)
                {
                    Debug.Log("HitID" + useParam.hitID);
                    ret = true;
                    hitDefApply(e, provokerEntity, useParam, HitPt);
                    //当てた分キャラ指定の値が減少..
                    refNumRemaining--;
                    provokerEntity.status.currentEnergy += 3;
                }
            }
        }
        foreach(Props prop in propList)
        {
            if(provokerEntity.hitCheck(prop.hitBox, out Vector3 hits))
            {
                if(prop.isHit == false && prop.isPausable())
                {
                    //雑ぅ. でもひとまずこれでなんとかなるか..
                    prop.OnHit(hitDefParams, hits);
                    Instantiate
                    ((hitDefParams.HitEff != null ? hitDefParams.HitEff : defaultEff), hits, Quaternion.identity);
                    //onHit, entity will stop. but props wont.
                    //I'll set high pause for each.
                    (provokerEntity.HitPauseTime , prop.disableTime) = (4, 30);
                }
                prop.isHit = true;
            }
        }

        return ret;
    }

    public bool ProvokeHitDef_Projs(Entity calledEntity, clssSetting sets, Transform trfs, hitDefParams H_params)
    {
        bool ret = false; int refNumRemaining;
        hitDefParams useParam = new hitDefParams();
        if (H_params != null)
        {
            useParam = H_params;
        }
        refNumRemaining = useParam.maxEntityHits;
        foreach (Entity e in entityList)
        {
            //selfには反応しない. また当たる数が設定されているなら0にならない限り設定される.
            if ((calledEntity == null || (e != calledEntity && e.tag != calledEntity.tag)) && refNumRemaining > 0)
            {
                bool f = false;
                Vector3 HitPt = Vector3.zero;
                //それぞれのentityの現在再生中のAnimatorが持つClssに対して衝突判定.
                //また、entityの無敵判定に関しても考える.
                clssSetting cEnemy = e.animancerManager.primaryAnimDef.clssSetting;
                f = sets.clssCollided(out var v1, out var v2, out var dist, clssDef.ClssType.Attack, cEnemy, .1f);
                //hitしたなら一先ずAnim番号を5000に飛ばしたい. ChangeState(5000)の最優先Queueとして組み込む.
                if (f == true)
                {
                    Debug.LogWarning("Proj Collided");
                    HitPt = (v1 + v2) / 2f;
                    ret = true;
                    hitDef_proj_Apply(e, trfs, useParam, HitPt);
                    //当てた分キャラ指定の値が減少..
                    refNumRemaining--;
                }
            }
        }
        return ret;
    }

    //hitdefApply is called for everything.. 
    //Projectileも同様に管理したい.
    void hitDefApply(Entity beatenEntity, Entity calledEntity,
    hitDefParams calledEParam, Vector3 hitContactPoint)
    {
        hitDefParams generatedParam = new hitDefParams();

        //call Parameter for damages..
        generatedParam = calledEParam;
        
        generatedParam.HitRegisterTime = elapsedTime;

        generatedParam.ownerEntity = calledEntity;
        generatedParam.hitEntity = beatenEntity;
        generatedParam.ContactPoint = hitContactPoint;

        //ステータス設定.
        generatedParam.SetStatus();
    }

    void hitDef_proj_Apply(Entity beatenEntity, Transform calledPoint,
    hitDefParams calledEParam, Vector3 hitContactPoint)
    {
        //stateChangeを設定..
        beatenEntity.isStateChanged = true;

        //一先ず、プレースホルダーとして入れる
        //stateTimeをリセット.
        beatenEntity.CurrentStateID = calledEParam.ChangeState_Enemy;
        beatenEntity.stateTime = 0;

        //現状、Projectileに関してはステート奪取を考えないことにする.
        // if (calledEParam.enemyRefsPlayerNum == true)
        // {
        //     beatenEntity.controlledEntity = calledEntity;
        // }

        //placeholder for velocity
        //currently its barebone
        Vector3 HitVect = Vector3.ProjectOnPlane
        (beatenEntity.transform.position - calledPoint.position, Vector3.up);

        //hitpause
        (beatenEntity.HitPauseTime) = (calledEParam.hitStopTime.y);

        /*
        DamageApply(beatenEntity, HitVect, calledEParam, 100f);
        Instantiate
        ((calledEParam.HitEff != null ? calledEParam.HitEff : defaultEff), hitContactPoint, Quaternion.identity);
        */
    }


    internal void ClearChars()
    {
        foreach (Entity ent in entityList)
        {
            if (ent.tag != "Player")
            {
                ent.status.currentHP = 0;
                ent.attrs.alive = false;
            }
        }
    }

    bool checkHit(string checkState, Entity checkEntity)
    {
        bool ret = false;
        if (checkState.Contains((char)checkEntity.checkHitStates()))
        {
            ret = true;
        }
        return ret;
    }

    public IEnumerator OneShotSlo_mo(float SlowValue)
    {
        float remTimeMax = SlowValue;
        float remTime = 0;
        while (remTime < remTimeMax)
        {
            remTime += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(0.0f, 1f, Mathf.Min(1f, MathF.Pow(remTime / remTimeMax, 2.0f)));
            yield return 0;
        }
    }

    public void GenerateEffect(GameObject obj, Vector3 pos, Quaternion rot)
    {
        if(obj != null)
        {
            Instantiate(obj, pos, rot);
        }
    }
}
