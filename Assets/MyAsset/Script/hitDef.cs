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
    public Vector2 hitStopTime;
    [SerializeField]
    public Vector2 guard_hitStopTime;
    [SerializeField]
    public GameObject HitEff;
    
    [SerializeField]
    public GameObject GuardHitEff;

    //当てた敵のステート変更情報(負の数以下で変更しない)
    public int ChangeState_Enemy = -1;

    //当たる数(1がデフォ)
    public int maxEntityHits = 1;

    //ダウン設定.
    public float fallTime = 0;

    //敵がプレイヤーのステート名を参照するか？
    public bool enemyRefsPlayerNum = false;

    //プレイヤーのステート変更情報(負の数以下で変更しない)
    public int ChangeState_Player = -1;

    //どういう姿勢に当たるか？　など. "S"tanding "A"ir, "L"aying の頭文字指定
    //また、"F"は Fall状態のフラッグがあるキャラにHit, "E"veryはフラッグ問わず全部当たる.
    public string HitPhysFlag = "SA";

    //どういう動きに当たるか？　など やられ判定のときに追撃しないようにしたりとか.
    public string HitMoveFlag = "IA";
    
    //Anim設定. この設定に基づき、ステート・アニメの変更先を変える.
    //L -> Light, M -> Middle, H -> Heavy, U -> Up, D -> Down, A -> Air, C -> Crouch, B -> Blow
    //基本はULのみのアニメで対応可能.
    // - 5000(弱ダメージ), 5050(吹き飛び), 5100(地上ダウン), 5200(ダウン回復).

    public string AnimType = "";

    public List<int> HitExcludeList;

    //このHitDefを呼び出した本体..
    internal Entity ownerEntity;

    //このHitDefの対象先.
    internal Entity hitEntity;

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
        //ガードが入ってるか？
        MoveGuarded = hitEntity.attrs.isGuarded == true && hitEntity.status.currentGuardPoint >= 0;
        //プレイヤー方向とか色々..        
        PushVector = Vector3.ProjectOnPlane
        (hitEntity.transform.position - ownerEntity.transform.position, Vector3.up).normalized;

        SetStates();
        DamageCalc();
    }

    //ステート番号をEntityから読み出し.
    //仕様書からどういうStateNumに変更するかを決定する..
    //ここではstateTimeなどの変更は無し.
    public int ReturnStateIDs(Entity refEntity)
    {
        //基本立ち喰らいアニメ
        int retID = 5000;
        if(ChangeState_Enemy > -1)
        {
            retID = ChangeState_Enemy;
        }
        else
        {
            int DamageType = 0;
            int HitType = 0;
            //refEntityが指定した遷移可能なStateを持っていればretIDに登録..
            //その前にAnimTypeに指定のCharが入ってないと遷移不可能にする..
            //if文祭りじゃ.
            int referenceID = refEntity.CurrentStateID;
            //基本として、LightHit用のStateDefは入っていないと問題外.
            //Fall hit - FallTimeが0以上、またはFall中に攻撃を加えたとき
            if(((fallTime > 0 ) || (5050 <= referenceID && referenceID <= 5059)) 
            && refEntity.loadedDefs.Any(a => a.StateDefID == 5050))
            {
                HitType = 50;
            }
            //Up hit - 5000 to 5009
            else if(AnimType.Contains('U'))
            {
                HitType = 0;
            }
            //Down hit - 5010 to 5019
            if(AnimType.Contains('D') && refEntity.loadedDefs.Any(a => a.StateDefID == 5010))
            {
                HitType = 10;
            }
            //Crouch hit - 5020 to 5029
            else if(AnimType.Contains('C') && refEntity.loadedDefs.Any(a => a.StateDefID == 5020))
            {
                HitType = 20;
            }
            //Air hit - 5030 to 5039
            else if(AnimType.Contains('A') && refEntity.loadedDefs.Any(a => a.StateDefID == 5030))
            {
                HitType = 30;
            }
            //Takedown Hit
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
            //いずれにせよ、Guardingが設定されているなら双方に登録される関数値を考える.
            //entityとhitIDが同じヤツは二重に登録しない筈
            if(MoveGuarded)
            {
                retID = referenceID;
            }
        }
        return retID;
    }

    //ダメージ・ノックバック・ヒットストップ計算,Instantiate等..
    void DamageCalc()
    {
        //スピード設定.
        hitEntity.rigid.velocity = PushVector.normalized * velset.x + Vector3.up * velset.y;
        //shapepositions. 後でこれも設定できるようにしたい.
        hitEntity.transform.DOShakePosition(hitEntity.HitPauseTime * Time.fixedDeltaTime, 0.25f, 40, 45);
        //beatenEntity.transform.DOShakeScale(1f, 3f, 30, 90f, true);

        if(MoveGuarded)
        {            
            //hitpoint damage("on" Guarded)
            hitEntity.status.currentHP -= GuardDamage * (ownerEntity.status.BaseAttackPerc / Mathf.Max(1.0f,hitEntity.status.BaseDefencePerc));
        }
        else
        {
            //hitpoint damage(if not guarded.)
            hitEntity.status.currentHP -= Damage * (ownerEntity.status.BaseAttackPerc / Mathf.Max(1.0f,hitEntity.status.BaseDefencePerc));
        }

        //placeholder for rotation
        ownerEntity.transform.rotation =
        Quaternion.Lerp(ownerEntity.transform.rotation, Quaternion.LookRotation(-PushVector, Vector3.up), 0.6f);
        Debug.Log("Hit : " + ownerEntity.gameObject.name);

        //hitpause
        (ownerEntity.HitPauseTime, hitEntity.HitPauseTime) = (hitStopTime.x, hitStopTime.y);
        
        //ダメージエフェクトの生成はselfから発動する.
        GameObject instanceEff = HitEff != null ? HitEff : gameState.self.defaultEff;
        if(MoveGuarded)
        {
            instanceEff = GuardHitEff != null ? GuardHitEff : gameState.self.defaultEff;
        }
        
        gameState.self.GenerateEffect(instanceEff, ContactPoint, Quaternion.identity);
    }

    void SetStates()
    {
        hitEntity.CurrentStateID = ReturnStateIDs(ownerEntity);

        //当てた相手は問答無用でstateTimeを0にする.
        hitEntity.stateTime = 0;
        //stateChangeを設定.
        hitEntity.isStateChanged = true;
        hitEntity.ChangeAnim();
        
        //攻撃を当てた対象にコントロールされる場合は相手のステートマップの読み出しを設定
        //設定されたEntityはselfStateが読み出されない限り読み出す.
        if(enemyRefsPlayerNum)
        {
            hitEntity.controlledEntity = ownerEntity;
        }

        //当てた側のChangeStateが0以上なら変更. ChangeStateも行う.
        if(ChangeState_Player > -1)
        {
            ownerEntity.isStateChanged = true;
            ownerEntity.CurrentStateID = ChangeState_Player;
        }
    }
}

