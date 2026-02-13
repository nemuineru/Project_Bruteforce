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
        Player = Instantiate(Player_Instantiate,InitSpawnPos.position,Quaternion.identity).GetComponent<Entity>();
        MainGUI.SetComponent(Player);

        GameObject objs = Instantiate(MainGUI.gameObject);
        GameObject GUI_Top = GameObject.FindGameObjectWithTag("UI");
        if(Player_Vcam != null)
        {
            CinemachineVirtualCamera cams = Instantiate(Player_Vcam);
            cams.LookAt = Player.transform;
            cams.Follow = Player.transform;
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
    public bool ProvokeHitDef_Entity(Entity calledEntity, hitDefParams hitDefParams)
    {
        bool ret = false;
        int refNumRemaining;
        hitDefParams useParam = new hitDefParams();        
        if (hitDefParams != null)
        {
            useParam = hitDefParams;
        }

        useParam.hitEntity = calledEntity;

        refNumRemaining = useParam.maxEntityHits;

        foreach (Entity e in entityList)
        {
            //selfには反応しない. また当たる数が設定されているなら0にならない限り設定される.
            if (e != calledEntity && e.tag != calledEntity.tag && refNumRemaining > 0)
            {
                //それぞれのentityの現在再生中のAnimatorが持つClssに対して衝突判定.
                //また、entityの無敵判定に関しても考える.
                bool f = calledEntity.hitCheck(e, out Vector3 HitPt);
                bool isContactable = hitDefParams.HitMoveFlag.Contains(e.moveType.ToString()) &&
                hitDefParams.HitPhysFlag.Contains(e.physicsType.ToString()) &&
                !hitDefParams.HitExcludeList.Contains(e.CurrentStateID);
                //hitしたなら一先ずAnim番号を5000に飛ばしたい. ChangeState(5000)の最優先Queueとして組み込む.
                if (f == true && isContactable)
                {
                    Debug.Log("HitID" + useParam.hitID);
                    ret = true;
                    hitDefApply(e, calledEntity, useParam, HitPt);
                    //当てた分キャラ指定の値が減少..
                    refNumRemaining--;
                    calledEntity.status.currentEnergy += 3;
                }
            }
        }
        foreach(Props prop in propList)
        {
            if(calledEntity.hitCheck(prop.hitBox, out Vector3 hits))
            {
                if(prop.isHit == false && prop.isPausable())
                {
                    //雑ぅ. でもひとまずこれでなんとかなるか..
                    prop.OnHit(hitDefParams, hits);
                    Instantiate
                    ((hitDefParams.HitEff != null ? hitDefParams.HitEff : defaultEff), hits, Quaternion.identity);
                    //onHit, entity will stop. but props wont.
                    //I'll set high pause for each.
                    (calledEntity.HitPauseTime , prop.disableTime) = (4, 30);
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
    void hitDefApply(Entity beatenEntity, Entity calledEntity,
    hitDefParams calledEParam, Vector3 hitContactPoint)
    {
        //stateChangeを設定..
        beatenEntity.isStateChanged = true;

        //一先ず、プレースホルダーとして入れる
        //stateTimeをリセット.
        beatenEntity.CurrentStateID = calledEParam.ReturnStateNum(beatenEntity);        
        beatenEntity.stateTime = 0;
        //攻撃を当てた対象にコントロールされる場合は相手のステートマップの読み出しを設定
        //設定されたEntityはselfStateが読み出されない限り読み出す.
        if (calledEParam.enemyRefsPlayerNum == true)
        {
            beatenEntity.controlledEntity = calledEntity;
        }
        //placeholder for velocity
        //currently its barebone
        Vector3 HitVect = Vector3.ProjectOnPlane
        (beatenEntity.transform.position - calledEntity.transform.position, Vector3.up);

        //hitpause
        (calledEntity.HitPauseTime, beatenEntity.HitPauseTime) = (calledEParam.hitStopTime.x, calledEParam.hitStopTime.y);
        DamageApply(beatenEntity, HitVect, calledEParam, calledEntity.status.BaseAttackPerc);
        Instantiate
        ((calledEParam.HitEff != null ? calledEParam.HitEff : defaultEff), hitContactPoint, Quaternion.identity);

        //playerのchangestateが0以上なら変更.
        if (calledEParam.ChangeState_Player > -1)
        {
            Debug.Log("PlayerState Changed");
            calledEntity.isStateChanged = true;
            calledEntity.CurrentStateID = calledEParam.ChangeState_Player;
        }
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
        DamageApply(beatenEntity, HitVect, calledEParam, 100f);
        Instantiate
        ((calledEParam.HitEff != null ? calledEParam.HitEff : defaultEff), hitContactPoint, Quaternion.identity);
    }

    void DamageApply(Entity beatenEntity, Vector3 HitVect, hitDefParams calledEParam, float atkParams)
    {
        //SetSpeed
        beatenEntity.rigid.velocity = HitVect.normalized * calledEParam.velset.x + Vector3.up * calledEParam.velset.y;


        //shapepositions
        beatenEntity.transform.DOShakePosition(beatenEntity.HitPauseTime * Time.fixedDeltaTime, 0.25f, 40, 45);
        //beatenEntity.transform.DOShakeScale(1f, 3f, 30, 90f, true);
        beatenEntity.ChangeAnim();

        if(beatenEntity.attrs.isGuarded)
        {            
            //hitpoint damage("on" Guarded)
            beatenEntity.status.currentHP -= calledEParam.GuardDamage * (atkParams / Mathf.Max(1.0f,beatenEntity.status.BaseDefencePerc));
        }
        else
        {
            //hitpoint damage(if not guarded.)
            beatenEntity.status.currentHP -= calledEParam.Damage * (atkParams / Mathf.Max(1.0f,beatenEntity.status.BaseDefencePerc));
        }

        //placeholder for rotation
        beatenEntity.transform.rotation =
        Quaternion.Lerp(beatenEntity.transform.rotation, Quaternion.LookRotation(-HitVect, Vector3.up), 0.6f);
        Debug.Log("Hit : " + beatenEntity.gameObject.name);
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
}
