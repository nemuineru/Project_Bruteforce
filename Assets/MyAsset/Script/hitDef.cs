

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEditor.SearchService;
using System.Linq;
using DG.Tweening;

//後々にこのhitDefParamsの項目を表示・非表示設定可能にしたい
//非表示にした項目は値未設定ならデフォルト値を使用
//あと、hitDefParamsは攻撃を当てた相手がその値を読み出せるように..
//2026-01-23: StateDef自体に表示・非表示設定を適応して..って感じのほうが良いかも.
//stParamsにshowAtInspectorフラグを適応して右クリックメニューで編集できるように..
[SerializeField]
[System.Serializable]
public class hitDefParams
{
    public int hitID;

    //通常ダメージ・ガード時ダメージ
    public float Damage;
    public float GuardDamage;
    //ガードポイント減少/通常とガード時
    public float GuardBreakPoint;
    public float GuardBreakPoint_OnGuard;

    public bool Kill;
    public bool GuardOnKill;

    //敵の移動
    //攻撃を当てた際の動作方向設定
    public Vector3 velset;
    //ガード時の動作方向設定
    public Vector3 guard_velset;

    //プレイヤーの移動
    //攻撃を当てた際の動作方向設定
    public Vector3 pl_velset;
    //ガード時の動作方向設定
    public Vector3 pl_guard_velset;

    //ガード時と通常時のヒットストップ時刻
    [SerializeField]
    public Vector2Int hitStopTime;
    [SerializeField]
    public Vector2Int guard_hitStopTime;

    //ヒット硬直時間設定
    [SerializeField]
    public Vector2Int HitTime;

    [SerializeField]
    public GameObject HitEff;
    
    [SerializeField]
    public GameObject GuardHitEff;

    //同時に当てた際の挙動確認. 多ければ多いほど優先され、優先度が低い程そのhitdefによるchangestateは変化されない.
    public int HitPriority;

    //同じ値がかち合った時、どういった挙動にするか.
    // H - Hit, 相手がDodgeでなければhitdefをapplyする.
    // M - Miss, 攻撃は回避される.
    // D - Dodge, 互いに命中しない. 
    public char HitPriorityBehavior = 'H';

    //もしsameHitIntervalが0より大きい数で指定されている場合なら、同じStateDefNo内で再度実行する.
    public float sameHitInterval = 0;

    //hitDef読み出し時、stateDefNoが同じ呼び出しとして設定されている際最大値として当たる数. (1がデフォ)
    public int maxEntityHits = 1;

    //ダウン設定.
    public int fallTime = 0;

    //当てた敵のステート変更情報(負の数以下で変更しない)
    public int ChangeState_Target = -1;
    
    //当てられた側が当てた側のステート名を参照するか？
    public bool TargetRefsOwnerNum = false;

    //当てた側のステート変更情報(負の数以下で変更しない)
    public int ChangeState_Owner = -1;

    //どういう姿勢に当たるか？　など. "S"tanding "A"ir, "L"aying の頭文字指定
    //また、"F"は Fall状態のフラッグがあるキャラにHit, "E"veryはフラッグ問わず全部当たる.
    public string HitPhysFlag = "SA";

    //どういう動きに当たるか？　など やられ判定のときに追撃しないようにしたりとか.
    public string HitMoveFlag = "IA";

    //どういう姿勢ならガード可能？
    public string GuardPhysFlag = "SA";
    //どういう動きでガード可能？
    public string GuardMoveFlag = "IA";

    
    //Anim設定. この設定に基づき、ステート・アニメの変更先を変える.
    //L -> Light, M -> Middle, H -> Heavy, U -> Up, D -> Down, A -> Air, C -> Crouch, B -> Blow
    //基本はULのみのアニメで対応可能.
    // - 5000(弱ダメージ), 5050(吹き飛び), 5100(地上ダウン), 5200(ダウン回復).

    public string AnimType = "";

    public List<int> HitExcludeList;

    //このHitDefを呼び出した本体..
    internal Entity ownerEntity;

    //このHitDefの対象先.
    internal Entity targetEntity;

    //このHitDefを呼び出した時の同StateDefNo中での構造..

    //このHitDefを呼び出した時のgameStateの時間を登録.
    internal float HitRegisterTime = 0;
    
    //isContacted = true でぶつかり判定は生成した、とする.
    //多分、このhitDefParamがEntityにレジスタされる時点でContact判定とかは考えなくて良いのでは？
    //と思ったけどHitDefレジスタ時に更新をすればなんとか..?
    internal bool MoveContact = false;

    //GetHitVarで呼び出される時のMovesetレジスタ関数. ガードされていたりしたら変更.
    internal bool MoveGuarded = false;

    internal Vector3 ContactPoint = Vector3.zero;
    Vector3 PushVector = Vector3.zero;

    /*
    //MoveReversed..は今は考えない(難しくなりそー。)
    */

    //ガードされたかどうかに関わらず、当たってからの経過時間を考える.
    //一番近い時間としてソートして..という形で.
    public float GetRegisteredTime()
    {
        float f = 0;
        if(gameState.self != null)
        {
            f = gameState.self.elapsedTime - HitRegisterTime;
        }
        return f;
    }

    //ownerEntityのパラメータを用いて、hitEntityのダメージ、及びにVelocitySetを計算する.
    //また、registeredHitDefsに登録作業を行う.
    public void SetStatus()
    {
        //基本的に当たったものとする.
        MoveContact = true;
        //ガードが入ってるか？ あと、ガード可能な攻撃？
        bool isGuardable = GuardMoveFlag.Contains(targetEntity.moveType.ToString()) && GuardPhysFlag.Contains(targetEntity.physicsType.ToString());
        MoveGuarded = targetEntity.attrs.isGuarded == true && targetEntity.status.currentGuardPoint >= 0 && isGuardable;


        //プレイヤー方向とか色々..        
        PushVector = Vector3.ProjectOnPlane
        (targetEntity.transform.position - ownerEntity.transform.position, Vector3.up).normalized;
        
        //攻撃を"当てた側"のisStateHitを変更.
        ownerEntity.attrs.isStateHit = ownerEntity.attrs.isStateHit > 0 ? ownerEntity.attrs.isStateHit : 1;

        SetStates();
        DamageCalc();        
        ownerEntity.hitdefSameTime.Add(hitID);
        if(targetEntity != null)
            registerDef(targetEntity);
        if(ownerEntity != null)
            registerDef(ownerEntity);
        
    }

    //registered index - last index is latest.
    void registerDef(Entity entity)
    {
        //if not find, -1 is outputted.
        int getDupedID = entity.registeredHitDefs.FindIndex(fParam => fParam.hitID == hitID && fParam.ownerEntity == ownerEntity && fParam.targetEntity == targetEntity);
        //remove the duped HitDef, and renew it.
        if(getDupedID >= 0)
        {
            entity.registeredHitDefs.RemoveAt(getDupedID);
        }
        entity.registeredHitDefs.Add(this);
    }


    //ステート番号をEntityから読み出し.
    //仕様書からどういうStateNumに変更するかを決定する..
    //ここではstateTimeなどの変更は無し.
    public int ReturnStateIDs(Entity refEntity)
    {
        //基本立ち喰らいアニメ. 汎用.
        int retID = 5000;
        int referenceID = refEntity.CurrentStateID;
        //Guardingが設定されているなら双方に登録される関数値を考える.
        //entityとhitIDが同じヤツは二重に登録しない筈.
        //いまのとこ、OnGuardedStateの105に移動させる.
        //ガードフラグなら、
        if(MoveGuarded)
        {
            //retID = 105;
            //Debug.Log("MoveGuarded To " + referenceID);
            return -1;
        }
        else if(ChangeState_Target > -1)
        {
            retID = ChangeState_Target;
        }
        else
        {
            ownerEntity.TotalHitNo++;
            int DamageType = 0;
            int HitType = 0;
            //refEntityが指定した遷移可能なStateを持っていればretIDに登録..
            //その前にAnimTypeに指定のCharが入ってないと遷移不可能にする..
            //if文祭りじゃ.
            //基本として、LightHit用のStateDefは入っていないと問題外.

            //Fall hit - FallTimeが0以上、またはFall中に攻撃を加えたとき
            if(((fallTime > 0 ) || (5050 <= referenceID && referenceID <= 5059)) 
            && refEntity.loadedDefs.Any(a => a.StateDefID == 5050))
            {
                HitType = 50;
            }
            //現在EntityがFall状態で追撃したなら再読み込み.
            else if(referenceID >= 5100 && referenceID <= 5110 && refEntity.loadedDefs.Any(a => a.StateDefID == 5100))
            {
                HitType = 100;
            }
            //AnimTypeの基、refEntityにその番号があるなら指定..
            //Up hit - 5000 to 5009(上半身ダメージアニメ)
            else if (AnimType.Contains('U'))
            {
                HitType = 0;
            }
            //Down hit - 5010 to 5019(下半身ダメージアニメ)
            else if(AnimType.Contains('D') && refEntity.loadedDefs.Any(a => a.StateDefID == 5010))
            {
                HitType = 10;
            }
            //Crouch hit - 5020 to 5029(かがみダメージアニメ)
            else if(AnimType.Contains('C') && refEntity.loadedDefs.Any(a => a.StateDefID == 5020))
            {
                HitType = 20;
            }
            //Air hit - 5030 to 5039(空中ダメージアニメ)
            else if(AnimType.Contains('A') && refEntity.loadedDefs.Any(a => a.StateDefID == 5030))
            {
                HitType = 30;
            }
            //Takedown Hit(転倒ダメージ)
            else if(AnimType.Contains('T') && refEntity.loadedDefs.Any(a => a.StateDefID == 5040))
            {
                HitType = 40;
            }

            //Light Hit - On Light Hit, 0 is called
            if(AnimType.Contains('L'))
            {
                DamageType = 0;
            }
            //Middle hit as + 1
            if(AnimType.Contains('M') && refEntity.loadedDefs.Any(a => a.StateDefID == retID + HitType + 1))
            {
                DamageType = 1;
            }
            //Heavy hit as + 1
            if(AnimType.Contains('H') && refEntity.loadedDefs.Any(a => a.StateDefID == retID + HitType + 2))
            {
                DamageType = 2;
            }
            retID += DamageType + HitType;
        }
        return retID;
    }

    //ダメージ・ノックバック・ヒットストップ計算,Instantiate等..
    void DamageCalc()
    {
        //スピード設定.
        targetEntity.rigid.velocity = PushVector.normalized * velset.x + Vector3.up * velset.y;
        //shapepositions. 後でこれも設定できるようにしたい.
        targetEntity.transform.DOShakePosition(targetEntity.status.HitPauseTime * Time.fixedDeltaTime, 0.25f, 40, 45);
        //beatenEntity.transform.DOShakeScale(1f, 3f, 30, 90f, true);
        
        targetEntity.attrs.isBeingStateContact = 1;

        //hitpoint damage("on" Guarded), knockbacking time and falling time set.
        if(MoveGuarded)
        {            
            targetEntity.status.currentHP -= GuardDamage *
            (ownerEntity.status.BaseAttackPerc / Mathf.Max(1.0f, targetEntity.status.BaseDefencePerc * targetEntity.status.GuardReductDmgrate));
            targetEntity.status.HitTime = HitTime.y;
            targetEntity.attrs.isBeingStateGuarded = 1;
            targetEntity.status.currentGuardPoint -= GuardBreakPoint_OnGuard;
        }
        else
        {
            //hitpoint damage(if not guarded.)
            targetEntity.status.currentHP -= Damage * (ownerEntity.status.BaseAttackPerc / Mathf.Max(1.0f,targetEntity.status.BaseDefencePerc));
            targetEntity.status.HitTime = HitTime.x;
            targetEntity.status.HitFallTime = fallTime;
            targetEntity.attrs.isBeingStateHit = 1;
        }

        //placeholder for rotation
        targetEntity.transform.rotation =
        Quaternion.Lerp(targetEntity.transform.rotation, Quaternion.LookRotation(-PushVector, Vector3.up), 0.6f);
        //Debug.Log("Hit : " + ownerEntity.gameObject.name);

        //hitpause
        (ownerEntity.status.HitPauseTime, targetEntity.status.HitPauseTime) = (hitStopTime.x, hitStopTime.y);
        
        //ダメージエフェクトの生成はselfから発動する.
        GameObject instanceEff = HitEff != null ? HitEff : gameState.self.defaultEff;
        if(MoveGuarded)
        {
            instanceEff = GuardHitEff != null ? GuardHitEff : gameState.self.defaultGuardEff;
        }
        gameState.self.GenerateEffect(instanceEff, ContactPoint, targetEntity.transform.rotation);
    }

    //CListQueueに追加する.
    void SetStates()
    {
        //targetEntity.CurrentStateID = ReturnStateIDs(targetEntity);

        //当てた相手は問答無用でstateTimeを0にする.
        //targetEntity.stateTime = 0;
        //stateChangeを設定.
        targetEntity.isStateChanged = true;

        int retID = ReturnStateIDs(targetEntity);
        if(retID != -1)
        {
            targetEntity.CListQueue.Add(new Entity.ChangeStateQueue(){stateDefID = retID, priority = 100});
            targetEntity.ChangeAnim();
        }

        //攻撃を当てた対象にコントロールされる場合は相手のステートマップの読み出しを設定
        //設定されたEntityはselfStateが読み出されない限り読み出す.
        if(TargetRefsOwnerNum && !MoveGuarded)
        {
            targetEntity.controlledEntity = ownerEntity;
        }
        //当てた側のChangeStateが0以上なら変更. ChangeStateも行う.
        if(ChangeState_Owner > -1 && !MoveGuarded)
        {
            ownerEntity.CListQueue.Add(new Entity.ChangeStateQueue(){stateDefID = ChangeState_Owner, priority = 100});
            //ownerEntity.stateTime = 0;
            ownerEntity.isStateChanged = true;
        }
    }
}

