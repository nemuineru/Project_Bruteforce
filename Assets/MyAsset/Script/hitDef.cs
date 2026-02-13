using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEditor.SearchService;
using System.Linq;

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
    public int ChangeState_Enemy = 5000;

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
    internal Entity hitEntity;

    //ステート番号をEntityから読み出し.
    //仕様書からどういうStateNumに変更するかを決定する..
    public int ReturnStateNum(Entity refEntity)
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
            int refID = refEntity.CurrentStateID;
            //基本として、LightHit用のStateDefは入っていないと問題外.
            //Fall hit - FallTimeが0以上、またはFall中に攻撃を加えたとき
            if(((fallTime > 0 ) || (5050 <= refID && refID <= 5059)) 
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
            //いずれにせよ、Guardingが設定されているなら105に飛ばす.
            //また、Entityのガード設定値が0以下となるなら110(ガードブレイク.)
            //ガードブレイクを超えるなら..とか考えなければ.
            if(refEntity.attrs.isGuarded == true && refEntity.status.currentGuardPoint >= 0)
            {
                retID = 105;
            }
        }
        return retID;
    }
}

