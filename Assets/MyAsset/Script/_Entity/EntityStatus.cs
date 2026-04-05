using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//よく使う属性を登録・管理.
[System.Serializable]
public class EntityAttr
{
    //操作可能状態かチェック.
    public bool ctrl = true;
    //生きてるか？
    public bool alive = true;
    //止まってるか？
    public bool isPaused = false;
    public bool isStateChanged = false;
    public bool isGuarded = false;
    public bool isDamaged = false;
    public bool isFall = false;

    public bool isEraseReady = false;

    //これらはそれぞれState内でHitDefの衝突判定が行われた場合の処理において反応される.
    //StateDefが変わればReset -> 0.
    //isPausedが設定されているなら増加しない.
    public int isStateHit = 0;
    public int isStateContact = 0;
    public int isStateGuarded = 0;
    public int isStateReversed = 0;

    public int isSoundNotPlayed = 0;

    public void updateCombatStates(bool willReset)
    {
        if(willReset)
        {
            resetting();
        }
        else
        {
            updating();
        }
    }

    void updating()
    {
        if (!isPaused)
        {
            isStateHit += isStateHit > 0 ? 1 : 0;
            isStateContact += isStateContact != 0 ? 1 : 0;
            isStateGuarded += isStateGuarded != 0 ? 1 : 0;
            isStateReversed += isStateReversed != 0 ? 1 : 0;
        }
        isSoundNotPlayed += isSoundNotPlayed != 0 ? 1 : 0;
    }

    void resetting()
    { 
        isStateHit = 0;
        isStateContact = 0;
        isStateGuarded = 0;
        isStateReversed = 0;       
        isSoundNotPlayed = 0;
    }

    public List<EntityFlag> flags = new List<EntityFlag>();
    public void flagUpdate()
    {
        foreach(EntityFlag flagSel in flags)
        {
            flagSel.Tick();
        }
    }
}

//キャラクターの特殊フラッグ指定管理..
public class EntityFlag
{
    public string flagName;
    //isEternal Meant as not erasing if it beyonds flagDefault
    public bool isEternal;
    public int flagDefaultTime;
    internal int flagElapsedTime = 0;
    public void Tick()
    {
        flagElapsedTime++;
    }
}


[System.Serializable]
public class EntityStatus
{
    //基本情報
    public float maxHP = 100f;
    public float currentHP = 100f;
    public float maxEnergy = 50f;
    public float currentEnergy = 50f;
    //ガード値
    //ガード中はこの値を先に減少させてから、適応
    public float currentGuardPoint = 100f;
    public float maxGuardPoint = 100f;

    public float GuardReductDmgrate = 1.0f;
    
    public float guardRecoverRate= .8f;
    public float guardRecoverParsedTime = 0f;
    public float GuardRecoverTime = .7f;


    //100%上限
    public float BaseAttackPerc = 100f;
    public float BaseDefencePerc = 100f;

    //スピードとかの設定.
    public Vector3 BaseMoveVelocityParam = new Vector3(5f,4f,5f);
    //スピードとかの設定.
    public Vector3 BaseAccelParam = new Vector3(40f,40f,40f);

    
    //この秒数が0にならない限り動きを止める. stateTimeなども同様.
    //ヒットのポーズ用(ガード含む). entityがhitPause時は減少しない.
    [SerializeField]
    public int HitPauseTime = 0;
    
    //ヒットののけぞり用(ガード含む). entityがhitPause時は減少しない.
    [SerializeField]
    public int HitTime = 0;

    //ヒット落下. entityがhitPause時は減少しない. また, recoveryにFall値を0に設定する.
    [SerializeField]
    public int HitFallTime = 0;

    //チャージ. 特殊ボタンの押しっぱなしを判別.
    //ダメージを受けたりすると0に戻る.
    //こういう変数はあんまり設定したくないんだよね
    public float ChargeTime = 0f;    
    internal float setChargeTime_Lv1 = 0.15f;
    internal float setChargeTime_Lv2 = .4f;
    internal float setChargeTime_End = 2f;

    public void setCurrentValue()
    {
        float MaxEnergyOverflowValue = 3.0f;
        currentHP = Mathf.Clamp(currentHP , 0 , maxHP);
        currentEnergy = Mathf.Clamp(currentEnergy , 0 , maxEnergy * MaxEnergyOverflowValue);
    }

    //get CurrentHP. might needs to update current..
    public int GetCurrentHP()
    {
        return Mathf.CeilToInt(Mathf.Max(0, currentHP));
    }
}

