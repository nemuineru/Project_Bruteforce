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


    public AudioSource inGameAuds;

    public Status_MainUI MainGUI;
    public GameObject Player_Instantiate;

    public CinemachineVirtualCamera Player_Vcam;

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

    float timeStartBy = 5.0f;

    public enum _GameStatus
    {
        //ゲーム中。
        InGame,
        //ポーズ中.
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

    void FixedUpdate()
    {
        elapsedTime += Time.fixedDeltaTime;
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
            ProvokeHitDef_Entity(HParam);
        }
        HitParamFrame();
        queuedHitDefs = new List<hitDefParams>();

        foreach(Entity et in entityList)
        {
            et.resetStates();
        }
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
    public bool ProvokeHitDef_Entity(hitDefParams refHitParam)
    {
        bool ret = false;
        int refNumRemaining;
        hitDefParams useParam = new hitDefParams();        
        if (refHitParam != null)
        {
            useParam = refHitParam;
        }

        //contact数を考える..
        int refOwnerTargetContact = useParam.ownerEntity.hitdefSameTime.FindAll(h => h == useParam.hitID).Count;

        refNumRemaining = useParam.maxEntityHits - refOwnerTargetContact;

        //Debug.Log(entityList.Count());

        foreach (Entity selectedEntity in entityList)
        {
            //selfには反応しない. また当たる数が設定されているなら0にならない限り設定される.
            if (selectedEntity != useParam.ownerEntity && 
            selectedEntity.tag != useParam.ownerEntity.tag &&
            refNumRemaining > 0)
            {
                Debug.Log(selectedEntity.gameObject.name);
                bool hitCheck = 
                useParam.ownerEntity.hitCheck(selectedEntity, out Vector3 HitPt);
                Debug.Log(hitCheck);

                bool isIntervalAvailable = true;
                //もし見つけられなかったら新規登録なので..
                hitDefParams FindP = selectedEntity.registeredHitDefs.Find(hDef => hDef.hitID == useParam.hitID);
                if(FindP != null && refOwnerTargetContact != 0)
                {
                    float recentRevTime = elapsedTime - FindP.HitRegisterTime;
                    //インターバル未設定なら一回のみ. インターバル設定済みなら...
                    if(useParam.sameHitInterval <= 0 || recentRevTime < useParam.sameHitInterval)
                    {
                        isIntervalAvailable = false;
                    }
                }

                //それぞれのentityの現在再生中のAnimatorが持つClssに対して衝突判定.
                //また、entityの無敵判定に関しても考える.
                //呼び出しentityのstateDef値が同じ指定値なら..等　考えることが多い..
                //Juggle追加.. これ管理しきれねえ.
                bool isContactable = hitCheck && selectedEntity.status.currentJugglePoint >= 0 && 
                refHitParam.HitMoveFlag.Contains(selectedEntity.moveType.ToString()) &&
                refHitParam.hitStateFlag.Contains(selectedEntity.stateType.ToString()) &&
                !refHitParam.HitExcludeList.Contains(selectedEntity.CurrentStateID) && isIntervalAvailable;
                //hitしたなら一先ずAnim番号を5000に飛ばしたい. ChangeState(5000)の最優先Queueとして組み込む.
                if (isContactable)
                {
                    ret = true;
                    hitDefApply(selectedEntity, useParam.ownerEntity, useParam, HitPt);
                    //当てた分キャラ指定の値が減少..
                    refNumRemaining--;
                    useParam.ownerEntity.status.currentEnergy += 3;
                }
                if(hitCheck)
                {
                    Debug.Log("HitID" + useParam.hitID);                    
                }
            }
            else if(selectedEntity == useParam.ownerEntity)
            {
                Debug.Log("selectetEntity is same value.");
            }
        }
        foreach(Props prop in propList)
        {
            if(useParam.ownerEntity.hitCheck(prop.hitBox, out Vector3 hits))
            {
                if(prop.isHit == false && prop.isPausable())
                {
                    //雑ぅ. でもひとまずこれでなんとかなるか..
                    prop.OnHit(refHitParam, hits);
                    Instantiate
                    ((refHitParam.HitEff != null ? refHitParam.HitEff : defaultEff), hits, Quaternion.identity);
                    //onHit, entity will stop. but props wont.
                    //I'll set high pause for each.
                    (useParam.ownerEntity.status.HitPauseTime , prop.disableTime) = (4, 30);
                }
                prop.isHit = true;
            }
        }
        return ret;
    }

    public bool ProvokeHitDef_Projs(Entity ownerEntity, clssSetting sets, hitDefParams H_params)
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
            if ((ownerEntity == null || (e != ownerEntity && e.tag != ownerEntity.tag)) && refNumRemaining > 0)
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
                    hitDefApply(e, ownerEntity, useParam, HitPt);
                    //当てた分キャラ指定の値が減少..
                    refNumRemaining--;
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

    public void TogglePauseMode()
    {
        if (gameStatus == _GameStatus.InGame)
        {
            gameStatus = _GameStatus.OutGame;
        }
        else
        { 
            gameStatus = _GameStatus.InGame;
        }
    }

    void GameDescApply()
    {
        switch (menuStatus)
        {
            case _MenuStatus.PreStart:
                {
                    // PreGameUI.SetActive(true);
                    timeStartBy -= Time.deltaTime;
                    if (timeStartBy < 0)
                    {
                        menuStatus = _MenuStatus.OnGame;
                    }
                    break;
                }
            case _MenuStatus.OnGame:
                {
                    // Time.timeScale = Mathf.Lerp(Time.timeScale, 1.0f, 0.3f);
                    // GameOverCams.enabled = false;
                    // pauseGameUI.SetActive(false);
                    // PreGameUI.SetActive(false);
                    // InGameUI.SetActive(true);
                    // if(inGameAuds != null)
                    // inGameAuds.pitch = Mathf.Lerp(inGameAuds.pitch, 1f, 0.08f);
                    // elapsedTime += Time.deltaTime;
                    break;
                }
            case _MenuStatus.GameOver:
                {
                    gameStatus = _GameStatus.OutGame;
                    // InGameUI.SetActive(false);
                    // pauseGameUI.SetActive(false);
                    // GameOverCams.enabled = true;
                    // Time.timeScale = Mathf.Lerp(Time.timeScale, 0.001f, 0.025f);
                    // if (!isGameOverUIShown)
                    // {
                    //     GameOverUI.SetActive(true);
                    //     isGameOverUIShown = true;
                    // }
                    // if(inGameAuds != null)
                    // inGameAuds.pitch = Mathf.Lerp(inGameAuds.pitch, 0.001f, 0.025f);
                    break;
                }
            case _MenuStatus.GameClear:
                {
                    gameStatus = _GameStatus.OutGame;
                    // InGameUI.SetActive(false);
                    // pauseGameUI.SetActive(false);
                    // FinishedCams.enabled = true;
                    // if (!isGameOverUIShown)
                    // {
                    //     FinishedUI.SetActive(true);
                    //     isGameOverUIShown = true;
                    // }
                    break;
                }
            case _MenuStatus.Pause:
                {
                    gameStatus = _GameStatus.OutGame;
                    // InGameUI.SetActive(false);
                    // pauseGameUI.SetActive(true);
                    // GameOverCams.enabled = true;
                    // Time.timeScale = Mathf.Lerp(Time.timeScale, 0.00f, 0.025f);
                    // if(inGameAuds != null)
                    // inGameAuds.pitch = Mathf.Lerp(inGameAuds.pitch, 0.00f, 0.025f);
                    break;
                }
        }

    }
    
    public void ReturnToMainMenu()
    {
        if (gameStatus != _GameStatus.OutGame)
        {
            SceneManager.LoadScene("Title");
        }
    }
}
