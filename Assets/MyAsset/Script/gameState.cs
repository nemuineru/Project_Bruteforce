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
    public GameObject defaultGuardBreakEff;
    public GameObject defaultDeathEff;
    public AudioClip[] defaultFootSound;
    public Target target;


    public AudioSource inGameAuds;

    public Status_MainUI MainGUI;
    Status_MainUI mainInstGUI;

    public GameObject Player_Instantiate;

    public CinemachineVirtualCamera Player_Vcam;
    public CinemachineVirtualCamera BirdView_vcam;
    public CinemachineVirtualCamera CloseLook_vcam;

    [SerializeField]
    public List<Entity> entityList;

    [SerializeField]
    public List<Props> propList;

    public Transform InitSpawnPos;

    internal List<hitDefParams> queuedHitDefs = new List<hitDefParams>();
    public List<hitDefParams> onOneFrameHitdefs;

    public _GameStatus gameStatus;
    public _MenuStatus menuStatus;

    public int KillNo = 0;

    float timeStartBy = 2.2f;

    public enum _GameStatus
    {
        //ゲーム中, 時間の流れも動いてる状態.
        InGame,
        //ゲームの外, プレイヤーや敵の動きは止まる
        OutGame
    }

    public enum _MenuStatus
    {
        PreStart,
        OnGame,
        GameOver,
        GameClear,
        Pause
    }

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
        GameObject findPlayer = GameObject.FindGameObjectWithTag("Player");
        if(findPlayer != null)
        {
            Player = findPlayer.GetComponent<Entity>();
        }
        else
        {
            Player = Instantiate(Player_Instantiate,tr.position,qt).GetComponent<Entity>();
        }

        GameObject objs = Instantiate(MainGUI.gameObject);
        GameObject GUI_Top = GameObject.FindGameObjectWithTag("UI");
        if(Player_Vcam != null)
        {
            CinemachineVirtualCamera cams = Instantiate(Player_Vcam);
            //cams.LookAt = Player.transform;
            cams.Follow = Player.transform;
            //生成位置の後ろ側を指定.
            cams.ForceCameraPosition(tr.position - tr.transform.forward + Vector3.up,qt);
            Player.vCam = cams;//GameObject.FindGameObjectWithTag("Virtual_MainCamera").GetComponent<CinemachineVirtualCamera>();
        }

        if(CloseLook_vcam != null)
        {
            CloseLook_vcam.LookAt = Player.transform;
            CloseLook_vcam.Follow = Player.transform;
            CloseLook_vcam.gameObject.SetActive(false);
        }
        
        if(BirdView_vcam != null)
        {
            BirdView_vcam.LookAt = Player.transform;
            BirdView_vcam.Follow = Player.transform;
            BirdView_vcam.gameObject.SetActive(false);
        }

        objs.transform.SetParent(GUI_Top.transform, false);
        mainInstGUI = objs.GetComponent<Status_MainUI>();
    }

    void FixedUpdate()
    {
        if(gameStatus != _GameStatus.OutGame)
        {
            elapsedTime += Time.fixedDeltaTime;
        }
        entityList = FindObjectsByType<Entity>(FindObjectsSortMode.None)
        .OrderBy(t => !t.attrs.alive)
        .ToList();

        propList = FindObjectsByType<Props>(FindObjectsSortMode.None).ToList();
        foreach(Entity et in entityList)
        {
            et.EntityUpdate();
        }

        foreach(hitDefParams HParam in queuedHitDefs)
        {
            ProvokeHitDef(HParam);
        }
        HitParamFrame();
        queuedHitDefs = new List<hitDefParams>();

        foreach(Entity et in entityList)
        {
            et.resetStates();
        }
    }

    void Update()
    {
        GameDescApply();
    }

    //hitdefFrame - リストアップしたhitdefparamを優先度に応じて実行する.
    void HitParamFrame()
    {
        var sorted = onOneFrameHitdefs.OrderByDescending(s => s.HitPriority).ToList();
        var executed = new List<hitDefParams>();

        //最初に実行されるヤツはコンディションに依らず必ず実行. 
        foreach (var reg in sorted)
        {
            bool shouldExecute = true;

            foreach (var prime in executed)
            {
                // Current param's owner was targeted by a prime param
                if (reg.ownerEntity != prime.targetEntity)
                    continue;

                if (reg.HitPriority < prime.HitPriority)
                {
                    // Lower priority against someone who already hit us — skip
                    shouldExecute = false;
                    break;
                }
                else if (reg.HitPriority == prime.HitPriority)
                {
                    // Same priority — HitPriorityBehavior decides
                    char behavior = reg.HitPriorityBehavior;
                    if (behavior == 'M' || behavior == 'D')
                    {
                        shouldExecute = false;
                        break;
                    }
                    // 'H': hit applies, continue executing
                }
            }

            if (shouldExecute)
                reg.SetStatus();

            executed.Add(reg);
        }

        onOneFrameHitdefs.Clear();
    }

    //HitDefを発火する際のイベント - プレイヤー設定の際..
    //HitDefのIntervalが設定されているなら、同じStateDef内で登録する.
    //2026-04-07
    //やっぱこれおかしいな、空振りが多発してる. 
    //2つのキャラクターが攻撃判定に重なった時のみ攻撃判定になってる？
    //2026-04-08 HitCheckにバグが有るのを確認. うーん..
    //そも　selectedEntityがなんかおかしいことになってるならnullになる筈..
    //追記 AnimDefの登録がヤバかったみたいです. 今は治った.
    //2026-06-02 ProjとEntityの共通化をAI君に任せた. 
    //..ただ、refOwnerTargetContactがProjのときに機能していないのがちょっとなー
    public bool ProvokeHitDef(hitDefParams refHitParam, clssSetting projSets = null)
    {
        bool ret = false;
        if (refHitParam == null) return false;

        Entity ownerEntity = refHitParam.ownerEntity;
        int refOwnerTargetContact = ownerEntity != null
            ? ownerEntity.hitdefSameTime.FindAll(h => h == refHitParam.hitID).Count
            : 0;
        int refNumRemaining = refHitParam.maxEntityHits - refOwnerTargetContact;

        foreach (Entity selectedEntity in entityList)
        {
            bool selfCheck = ownerEntity == null ||
                (selectedEntity != ownerEntity && selectedEntity.tag != ownerEntity.tag);

            if (selfCheck && refNumRemaining > 0)
            {
                bool hitCheck;
                Vector3 HitPt;

                if (projSets != null)
                {
                    clssSetting cEnemy = selectedEntity.animancerManager.primaryAnimDef.clssSetting;
                    hitCheck = projSets.clssCollided(out var v1, out var v2, out var dist, clssDef.ClssType.Attack, cEnemy, .1f);
                    HitPt = hitCheck ? (v1 + v2) / 2f : Vector3.zero;
                }
                else
                {
                    Debug.Log(selectedEntity.gameObject.name);
                    hitCheck = ownerEntity.hitCheck(selectedEntity, out HitPt);
                    Debug.Log(hitCheck);
                }

                bool isIntervalAvailable = true;
                hitDefParams FindP = selectedEntity.registeredHitDefs.Find(hDef => hDef.hitID == refHitParam.hitID);
                if ((FindP != null && refOwnerTargetContact != 0))
                {
                    //これ、revTimeがrealTimeになってるのでframe単位で設定せねば..
                    float recentRevTime = elapsedTime - FindP.HitRegisterTime;
                    if (refHitParam.sameHitInterval <= 0 || recentRevTime < refHitParam.sameHitInterval)
                    {
                        Debug.Log("Interval false : " + recentRevTime);
                        isIntervalAvailable = false;
                    }
                    else
                    {
                        Debug.Log("Interval true : " + recentRevTime);
                    }
                }

                bool isContactable = hitCheck &&
                    selectedEntity.status.currentJugglePoint >= 0 &&
                    refHitParam.HitMoveFlag.Contains(selectedEntity.moveType.ToString()) &&
                    refHitParam.hitStateFlag.Contains(selectedEntity.stateType.ToString()) &&
                    !refHitParam.HitExcludeList.Contains(selectedEntity.CurrentStateID) &&
                    isIntervalAvailable;

                if (isContactable)
                {
                    if (projSets != null)
                        Debug.LogWarning("Proj Collided");
                    else
                        Debug.Log("HitID" + refHitParam.hitID);
                    ret = true;
                    hitDefApply(selectedEntity, ownerEntity, refHitParam, HitPt);
                    refNumRemaining--;
                    if (ownerEntity != null)
                        ownerEntity.status.currentEnergy += 3;
                }
            }
            else if (ownerEntity != null && selectedEntity == ownerEntity)
            {
                Debug.Log("selectetEntity is same value.");
            }
        }

        if (projSets == null && ownerEntity != null)
        {
            foreach (Props prop in propList)
            {
                if (ownerEntity.hitCheck(prop.hitBox, out Vector3 hits))
                {
                    if (prop.isHit == false && prop.isPausable())
                    {
                        prop.OnHit(refHitParam, hits);
                        Instantiate(
                            refHitParam.HitEff ?? defaultEff, hits, Quaternion.identity);
                        (ownerEntity.status.HitPauseTime, prop.disableTime) = (4, 30);
                    }
                    prop.isHit = true;
                }
            }
        }

        return ret;
    }


    //hitdefApply is called for everything.. 
    //Projectileも同様に管理.
    void hitDefApply(Entity target, Entity calledEntity,
    hitDefParams calledEParam, Vector3 hitContactPoint)
    {
        hitDefParams generatedParam = new hitDefParams();

        //call Parameter for damages..
        generatedParam = calledEParam;
        
        generatedParam.HitRegisterTime = elapsedTime;

        generatedParam.ownerEntity = calledEntity;
        generatedParam.targetEntity = target;
        generatedParam.ContactPoint = hitContactPoint;

        //ステータス設定.
        onOneFrameHitdefs.Add(generatedParam);
        //generatedParam.SetStatus();
    }

    // void hitDef_proj_Apply(Entity target, Entity owner, Transform calledPoint,
    // hitDefParams calledEParam, Vector3 hitContactPoint)
    // {
    //     hitDefParams generatedParam = new hitDefParams();

    //     //call Parameter for damages..
    //     generatedParam = calledEParam;
        
    //     generatedParam.HitRegisterTime = elapsedTime;

    //     generatedParam.ownerEntity = null;
    //     generatedParam.targetEntity = target;
    //     generatedParam.ContactPoint = hitContactPoint;

    //     //stateChangeを設定..
    //     target.isStateChanged = true;

    //     //一先ず、プレースホルダーとして入れる
    //     //stateTimeをリセット.
    //     target.CurrentStateID = calledEParam.ChangeState_Target;
    //     target.stateTime = 0;

    //     if (calledEParam.TargetRefsOwnerNum == true)
    //     {
    //         target.controlledEntity = owner;
    //     }

    //     //placeholder for velocity
    //     //currently its barebone
    //     Vector3 HitVect = Vector3.ProjectOnPlane
    //     (target.transform.position - calledPoint.position, Vector3.up);

    //     //hitpause
    //     (target.HitPauseTime) = (calledEParam.hitStopTime.y);

    //     /*
    //     DamageApply(beatenEntity, HitVect, calledEParam, 100f);
    //     Instantiate
    //     ((calledEParam.HitEff != null ? calledEParam.HitEff : defaultEff), hitContactPoint, Quaternion.identity);
    //     */
    // }


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

    //PauseMode.
    public void TogglePauseMode()
    {
        if (menuStatus == _MenuStatus.OnGame)
        {
            menuStatus = _MenuStatus.Pause;
        }
        else if(menuStatus == _MenuStatus.Pause)
        { 
            menuStatus = _MenuStatus.OnGame;
        }
    }

    void GameDescApply()
    {
        switch (menuStatus)
        {
            case _MenuStatus.PreStart:
                {
                    timeStartBy -= Time.deltaTime;
                    gameStatus = _GameStatus.OutGame;
                    setCams(isPlayerCam: true, isBirdCam: false, isCloseCam: false);                    
                    //prestartから時間がたてばOnGameに移行.
                    if (timeStartBy < 0)
                    {
                        menuStatus = _MenuStatus.OnGame;
                    }
                    break;
                }
                //メインゲーム中
            case _MenuStatus.OnGame:
                {
                    gameStatus = _GameStatus.InGame;
                    setCams(isPlayerCam: true, isBirdCam: false, isCloseCam: false);
                    
                    SetPauseToggle(false);
                    break;
                }
            case _MenuStatus.GameOver:
                {
                    gameStatus = _GameStatus.OutGame;
                    SetPauseToggle(true);
                    setCams(isPlayerCam: false, isBirdCam: true, isCloseCam: false);
                    break;
                }
            case _MenuStatus.GameClear:
                {
                    gameStatus = _GameStatus.OutGame;

                    setCams(isPlayerCam: false, isBirdCam: false, isCloseCam: true);
                    break;
                }
            case _MenuStatus.Pause:
                {
                    gameStatus = _GameStatus.OutGame;
                    setCams(isPlayerCam: true, isBirdCam: false, isCloseCam: false);
                    SetPauseToggle(true);
                    break;
                }
        }
    }

    //ポーズ状態のとき、時間の流れを止める関数を作った.
    public void SetPauseToggle(bool isPause)
    {
        if(isPause)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, 0.00f, 0.025f);
            if(inGameAuds != null)
            {
                inGameAuds.pitch = Mathf.Lerp(inGameAuds.pitch, 0.00f, 0.025f);  
            }
        }        
        else
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, 1.0f, 0.3f);
            if(inGameAuds != null)
            {
                inGameAuds.pitch = Mathf.Lerp(inGameAuds.pitch, 1f, 0.08f);   
            }
        }
    }

    //カメラ切り替え
    public void setCams
    ( bool isPlayerCam = true, bool isBirdCam = false, bool isCloseCam = false)
    {
        if(Player_Vcam != null)
        Player_Vcam.gameObject.SetActive(isPlayerCam);
        if(BirdView_vcam != null)
        BirdView_vcam.gameObject.SetActive(isBirdCam);
        if(CloseLook_vcam != null)
        CloseLook_vcam.gameObject.SetActive(isCloseCam);
    }
    
    public void ReturnToMainMenu()
    {
        if (gameStatus == _GameStatus.OutGame)
        {
            SceneManager.LoadScene("Title");
        }
    }
}
