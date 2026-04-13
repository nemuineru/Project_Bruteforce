//現状, debugやexeでは出ない(IndexWasOutOfRange)

using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using System.Linq;
using System.ComponentModel;
using UnityEditor;
using DG.Tweening;
using System;
using BehaviorDesigner.Runtime;
using Animancer;
using UnityEngine.InputSystem.XR;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class Entity : MonoBehaviour
{
    //set Entity Name.
    [SerializeField]
    internal string EntityName;
    
    //set Entity team Name. Avoiding Friendly Fire.
    [SerializeField]
    internal string EntityTeamName;

    //prevent to compare name directly.
    internal string EntityTeamID;


    //statetype : 体勢の設定. 
    public enum _StateType
    {
        S, //Standing, 立ち状態
        A, //Air, 空中
        L //Lying, 
    }

    public enum _PhysicsType
    {
        S, //Standing, 地上の動き
        A, //Air, 空中. 
        N //None, 定義なし(摩擦効果を受けない.)
    }

    public enum _MoveType
    {
        I, //Idle, 設定なし
        A, //Attack, 攻撃中
        H  //Hit, やられ
    }
    public _StateType stateType;
    public _PhysicsType physicsType;
    public _MoveType moveType;

    public class EntityInfo
    {
        public string name;
        public int ID;
    }


    public Rigidbody rigid;
    public AudioSource selfSource;

    public int stateTime;

    //アニメのフレーム時間.
    public float animationFrameTime, animationEndTime;

    //移動用の設定など. fwに設定した値・90度回転方向を考慮 - 
    public Vector3 targetTo_fw = Vector3.forward;

    public CinemachineVirtualCamera vCam;

    //ステートID.
    public int CurrentStateID = 0;

    //clssが設定されていない時のプレイヤー当たり判定設定
    public clssSetting defaultClss;

    internal Transform[] allChildTransforms;

    //status for UIs and more

    [SerializeField]
    public EntityStatus status;

    //Attr for GameSystems and more

    [SerializeField]
    public EntityAttr attrs;


    //親のEntity. Parent化されているならこのEntityの内容を読み出しておく.
    [SerializeField]
    public Entity parentEntity;

    //コントロール影響下のEntitiy. Selfstateが発生しない限りこの内容を読み出す.
    [SerializeField]
    public Entity controlledEntity;


    //アニメーション管理用.
    public int animID = 0;
    [SerializeField]
    List<AnimlistObject> animListObject;

    //List化されたanimListObjectからanimDefsのリストを作成する.
    //他Entityが自分のAnimを参照する際、パラメータ変更を共有してしまうことを懸念.

    internal List<AnimDef> animDefs = new List<AnimDef>();

    //アニメーターベース.
    Animator animator;

    //Animancerの利用を考える.
    [SerializeField]
    AnimancerComponent mainAnimancer;
    internal AnimancerManager animancerManager;

    public Color CurColor;

    //地上判定.
    public bool isOnGround;

    //ステート直後のステート時間を0にするため、追加.
    public bool isStateChanged = false;

    //メインマテリアルとメッシュ.
    Material mat;
    public List<Renderer> renderers;

    //stateDefListObjの本元.

    public List<StateDefListObject> DefLists;


    public Vector3 wishingVect;

    //Input Manager for each entitys
    internal entityInputManager entityInput = new entityInputManager();

    //The Brain of character. not muscle or vertebrae. using BehaviorDesigner to do.
    [SerializeField]
    ExternalBehavior LoadedBehavior;
    BehaviorTree BTree;

    //CapsuleColliderの格納. キャラクターの地形との判定.
    CapsuleCollider capCol;

    //カメラ登録時、格納.
    CinemachineOrbitalTransposer transposer_Orbit;

    //実行済みのステート番号の書き換えなど
    public List<int> executedStateIDs = new List<int>();

    //同時StateDef内で実行されたhitDefのID名称. MaxHitが指定されているならこれを使う.
    public List<int> hitdefSameTime = new List<int>();

    internal List<StateDef> loadedDefs = new List<StateDef>();

    string verd_1;
    [SerializeField]
    Vector3 raycenter = Vector3.down * 0.5f;

    public Vector3 pausedVel = Vector3.zero;

    internal Vector3 GroundNormal = Vector3.zero;

    //CMDList for command buffering, and checks @ .mjs file
    [SerializeField]
    internal List<entityInputManager.entityInput_Buffers> cmdList;

    [SerializeField]
    //ヒット回数を数えるための変数. どの様な感じでも減少しない.
    internal int TotalHitNo = 0;


    //GetHitDef/GetAttackDefで管理されるHitParams.
    //当てたプレイヤー本人のガード等を鑑みて..

    [SerializeField]
    internal List<hitDefParams> registeredHitDefs = new List<hitDefParams>();

    //first init.
    void Awake()
    {
        allChildTransforms = GetComponentsInChildren<Transform>(true);
        renderers = GetComponentsInChildren<Renderer>(true).ToList();
        mat = renderers[0].material;
        animator = GetComponent<Animator>();
        
        mainAnimancer = gameObject.AddComponent<AnimancerComponent>();
        mainAnimancer.Animator = animator;


        rigid = GetComponent<Rigidbody>();
        capCol = GetComponent<CapsuleCollider>();
        //DefList = (StateDefListObject)StateDefListObject.CreateInstance(typeof(StateDefListObject));
        //DefSet();

        //StateDefはDeepCopyしないように.

        //DefList.stateDefs.ForEach(def => def.PriorCondition = def.PriorCondition);

        //アニメ設定.
        initAnimSetting();
        defaultClss.initClss(this);

        foreach (StateDefListObject dObj in DefLists)
        {
            foreach (StateDef state in dObj.stateDefs)
            {
                //set ScriptDirectory for Load.
                state.ScriptDirectory = dObj.ScriptDirectory;
                //Debug.Log("scrDirectory_Loaded" + state.ScriptDirectory);
                loadedDefs.Add(state.Clone());
            }
        }

        //initialize behaviors.
        if (LoadedBehavior != null)
        {
            BTree = gameObject.AddComponent<BehaviorTree>();
            BTree.ExternalBehavior = LoadedBehavior;
        }
        rigid.useGravity = physicsType != _PhysicsType.N;
        selfSource = GetComponent<AudioSource>();
    }

    void initAnimSetting()
    {
        foreach (AnimlistObject list in animListObject)
        {
            foreach(AnimDef Anims in list.animDef)
            {
                animDefs.Add(Anims.Clone());
            }
        }
        if (mainAnimancer != null && animator != null)
        {
            Debug.Log("init Animancer");
            animancerManager = new AnimancerManager();
            animancerManager.main = mainAnimancer;
            animancerManager.root = this;
            ChangeAnim();           
        }
    }

    // Start is called before the first frame update
    //Awake後に設定される. UIとか指定したい.
    void Start()
    {
        //プレイヤーとか指定されたタグに設定されていないなら、HP用のUI表示.
        if (gameObject.tag != "Player" && gameState.self.EnemyHPUI != null)
        {
            StatusBar_Minimal min = Instantiate(gameState.self.EnemyHPUI).GetComponent<StatusBar_Minimal>();
            min.SetEntity(this);
            min.transform.parent = transform;
            min.transform.localPosition = Vector3.up;
        }        
    }

    // Update is called once per frame
    public void EntityUpdate()
    {
        //これかぁ.. HitPauseTimeが設定されていてもそのまま続行 - HitPause分も引き継ぐ.
        stateTime = isStateChanged ? 0 : status.HitPauseTime >= 0 ? stateTime : stateTime + 1;
        
        //check each cmds. and buffers it.
        foreach (var cmds in cmdList)
        {
            cmds.CheckCommand(entityInput);
        }
        status.setCurrentValue();
        defaultClss.clssPosUpdate();

        //後で消します. Player用にInputを記録する..
        //これはBehaviorDesignerに登録させる予定.
        //entityInput.RecordInput_Player(0);
        //play BehaviorDesigner.
        entityPhisics();
        setanimPlay();

        if (LoadedBehavior != null)
        {
            BTree.Start();
        }

        setViewCams();
        mat.SetColor("_Color", CurColor);

        //GuardFlag is discarded when the updation called.
        attrs.isGuarded = false;

        executeStates();

        capsuleRaydraw();

        status.HitPauseTime -= 1;
        //のけぞり計算
        status.HitFallTime -= status.HitPauseTime < 0 ? 1 : 0;
        status.HitTime -= status.HitFallTime < 0 ? 1 : 0;
        
    }

    public void addBalancePoint(float AddBalancePt)
    {
        // 強靭値回復はhitPauseを考慮
        // ただし負数のmaxBalancePoint以下にはならない
        // status.currentBalancePoint = moveType != _MoveType.H && status.HitPauseTime < -10 ? 
        status.currentBalancePoint = Mathf.Min(status.currentBalancePoint + AddBalancePt, status.maxBalancePoint);
        status.currentBalancePoint = Mathf.Max(-status.maxBalancePoint,status.currentBalancePoint);
    }
    
    public void setJugglePoint(float JugglePt)
    {
        //ジャグルはstateChangeの値が反映.
        // status.currentJugglePoint = moveType != _MoveType.H ? status.maxJugglePoint : 
        //ジャグルはstateChangeの値が反映.
        status.currentJugglePoint = JugglePt;
    }


    void statusAlign()
    {
        status.currentHP = Mathf.Clamp(status.currentHP , 0 , status.maxHP);
        status.currentEnergy = Mathf.Clamp(status.currentEnergy , 0 , status.maxEnergy);
    }

    internal void resetStates()
    {
        attrs.updateCombatStates(isStateChanged);
        //ステートが変更されていれば色々Reset.
        if (isStateChanged)
        {
            executedStateIDs = new List<int>();
            hitdefSameTime = new List<int>();
        }
        //statusAlign();
        //isStateChangedはここで変更される. currentStateでstateChangeが発生した際,　リセットをかけるため.
        isStateChanged = false;
    }

    //地面判定.
    void capsuleRaydraw()
    {
        //raycenter
        //Ray ray = new Ray(transform.position + Vector3.up * 0.009f, Vector3.down);
        //Debug.DrawRay(ray.origin, Mathf.Max(0, -rigid.velocity.y) * Vector3.down);

        RaycastHit hitInfo;

        //Physics.Raycast(ray, out hitInfo, Mathf.Max(0.01f, -rigid.velocity.y * Time.fixedDeltaTime), LayerMask.GetMask("Terrain"));

        //get Capsule Normals
        float minimumLength = 0.001f;
        float down_Add = 0.03f;
        Vector3 norm =
            new Vector3
            (capCol.direction == 0 ? 1 : 0,
            capCol.direction == 1 ? 1 : 0,
            capCol.direction == 2 ? 1 : 0);

        //カプセル一端の組み合わせ
        //pos1が上側. pos2が下側.
        Vector3 pos_1 =
            transform.position + Vector3.up * minimumLength + transform.rotation * (capCol.center + norm * (capCol.height / 2f - capCol.radius));
        Vector3 pos_2 =
            transform.position + Vector3.up * (minimumLength + down_Add) + transform.rotation * (capCol.center - norm * (capCol.height / 2f - capCol.radius));
        float MaxLength = Mathf.Max(0.1f, -rigid.velocity.y * Time.fixedDeltaTime);

        //カプセルレイ.
        bool isCapsuleHit =
            Physics.CapsuleCast
            (pos_1, pos_2,
            capCol.radius - minimumLength, Vector3.down,
            out hitInfo, MaxLength , LayerMask.GetMask("Terrain"));

        //Debug.Log(isCapsuleHit + " - capsuleSet?");

        //キャラと地形の当たり判定表示
        //clssDef.DrawCapsuleGizmo_Tool(pos_1,pos_2,capCol.radius,Color.cyan);

        isOnGround = (hitInfo.collider != null);
        //なにもぶつからないと平面方向上を値として考える
        GroundNormal = (hitInfo.collider != null) ? hitInfo.normal : Vector3.up;
        setDestroy();
        /*        
        //カプセル一端の組み合わせ
        //pos1が上側. pos2が下側.
        Vector3 pos_1 =
            transform.position + Vector3.up * minimal + transform.rotation * (capCol.center + norm * (capCol.height / 2f - capCol.radius));
        Vector3 pos_2 =
            transform.position + Vector3.up * minimal + transform.rotation * (capCol.center - norm * (capCol.height / 2f - capCol.radius));
        
        float RayDist = Mathf.Max(detections, -rigid.velocity.y * Time.fixedDeltaTime);

        //カプセルレイ.
        bool isCapsuleHit =
            Physics.CapsuleCast
            (pos_1, pos_2,
            capCol.radius - minimal, Vector3.down,
            out hitInfo, RayDist , LayerMask.GetMask("Terrain"));
        Debug.DrawLine(pos_1,pos_2,Color.red);
        Debug.DrawLine(pos_2,pos_2 + Vector3.down * (RayDist + capCol.radius - minimal),Color.cyan);
        */
    }

    //ステータスの変更作業など
    void setStatusAlign()
    {
        if (status.currentHP < 0)
        {
            //stateChange
            isStateChanged = true;

            //5300をKOステートに変更.
            CurrentStateID = 5300;
            stateTime = 0;
        }
    }
    
    StateDef prevState = null;

    void executeStates()
    {
        //ChangeStateを実行した直後フレームの、その時のステートIDを読み出す..
        int prevStateID = -100000;
        if (CListQueue.Count > 0)
        {
            isStateChanged = true;
            //Most Primal Queue is Most Biggest Number.
            CListQueue.Sort((CQ_L, CQ_M) => CQ_M.priority - CQ_L.priority);
            prevStateID = CurrentStateID;
            CurrentStateID = CListQueue[0].stateDefID;
            //ChangeStateが実行されているならstateTimeは0.
            stateTime = 0;

            //resets at update
            attrs.updateCombatStates(true);

            CListQueue.Clear();
        }

        //常時実行StateDef(-1, -2, -3)

        //-3はステート奪取対象のものを読み出して実行.
        if (controlledEntity != null)
        {
            StateDef AutoState_3 =
            controlledEntity.loadedDefs.Find(stDef => stDef.StateDefID == -3);
            if (AutoState_3 != null)
            {
                //Debug.Log("auto checking -1 state");
                AutoState_3.Execute(this, false);
            }
        }

        //-2はステート奪取されていても実行.
        StateDef AutoState_2 =
        loadedDefs.Find(stDef => stDef.StateDefID == -2);
        if (AutoState_2 != null)
        {
            //Debug.Log("auto checking -1 state");
            AutoState_2.Execute(this, false);
        }

        //-1はステート奪取されているなら実行しない.
        StateDef AutoState_1 =
        loadedDefs.Find(stDef => stDef.StateDefID == -1);
        if (AutoState_1 != null && controlledEntity == null)
        {
            //Debug.Log("auto checking -1 state");
            AutoState_1.Execute(this, false);
        }



        if (status.currentHP <= 0)
        {
            if (attrs.alive == true)
            {
                //倒したときの一瞬スローモー.
                StartCoroutine(gameState.self.OneShotSlo_mo(0.45f));
                Instantiate(gameState.self.defaultDeathEff,transform.position + Vector3.up * 0.5f,Quaternion.identity);
                //BOLD tagging gamechanger system that I hate to implement.
                if (gameObject.tag == "Player")
                {
                    gameState.self.gameStatus = gameState.GameStatus.OutGame;
                }
                else
                {
                    gameState.self.KillNo++;
                }
            }
            attrs.alive = false;
        }

        //On death, Change to 5000 first, end at 5300(death state).
        if (attrs.alive == false && (CurrentStateID < 5000 || CurrentStateID > 5300) && controlledEntity == null)
        {
            CurrentStateID = 5000;
        }
        
        onGameFinished();

        //state実行.. これは一つだけに実行されるはず.
        //ステート奪取したときの値を実行..
        StateDef currentState = null;

        //前回のprevStateは前回常時実行ステート以外で, 実行したものを参照.
        if (controlledEntity == null)
        {
            currentState =
            loadedDefs.Find(stDef => stDef.StateDefID == CurrentStateID);
        }
        //ChangeStateと同時に発行されると、エラーが発生する？
        else
        {
            //Debug.LogWarning("Parent Entity Loaded");
            StateDef findDef = controlledEntity.loadedDefs.Find(st => st.StateDefID == CurrentStateID).Clone();
            currentState = findDef;
        }        
        
        //ステート奪取後も実行される.
        if (prevState != null && isStateChanged == true)
        {
            Debug.Log("finding prevstate at - " + CurrentStateID + " prevStateDefID : " + prevState.StateDefID + " statedef main : " + prevState.StateDefName);
            //なんか, StateExecute時にWebGLだとIndexOutofBoundsとなるみたいなので対策せねば.
            List<int> ExID = prevState.Execute(this, true);
            //過去のStateを実行(ChangeStateが実行された後の1フレームのみ.)
            if(ExID.Count() > 0)
                foreach (int ID in ExID)
                {
                    Debug.Log(ID);
                }
        }

        if (currentState != null)
        {
            //Debug.Log("Executing stateDef - " + CurrentStateID + " at state time of - " + Time.frameCount + stateTime);
            // + " at time of " + stateTime            
            //the StateDef needs as deepcopy?

            int i = 0;
            //こちらも対策せねば.
            List<int> ExID = currentState.Execute(this, false);
            //実行したSTATEIDを格納. 実行回数はまだ記録してない.
            if(ExID.Count() > 0)
                foreach (int ID in ExID)
                {
                    if (!executedStateIDs.Any(i => i == ID))
                    {
                        executedStateIDs.Add(ID);
                    }
                    i++;
                    // Debug.Log("Executed for " + i + "," + ID);
                }
            //Debug.Log("Executed stateDef - " + CurrentStateID + " at state time of - "  + Time.frameCount + "/"+ stateTime +
            //" " + this.gameObject.name);
            //前回ステートにcurrentStateの情報を登録
            if( isStateChanged == true )
            {
                prevState = currentState;
            }
        }

        else
        {
            //Debug.LogError("Loaded State is null : " + CurrentStateID);
        }
    }

    void onGameFinished()
    {
        //プレイヤーなら、ゲームクリア時にこのステートに戻す.
        //180はMUGENでいうと勝利ポーズ..
        /*
        if (gameObject.tag == "Player" && gameState.self.gDesc == gameState.GameStateDesc.Finished)
        {
            CurrentStateID = 180;
        }
        */

    }

    //カメラ設定.
    void setViewCams()
    {
        //At Control, wishinvect is Input by command Buffer
        //これ消したい.
        Vector2 wish = Vector2.zero;
        Vector2 look = Vector2.zero;
        if(entityInput.commandBuffer.Count > 0)
        {
            wish = entityInput.commandBuffer[0].MoveAxis;        
            look = entityInput.commandBuffer[0].LookAxis;
        }
        //(InputInstance.self.inputValues.MovingAxisRead);
        //(InputInstance.self.inputValues.MovingAxisRead);
        //Debug.Log(entityInput.commandBuffer[0].MoveAxis);

        //Vcamが設定されているなら、Camera設定に従いfwを設定する.
        if (vCam != null)
        {
            //Debug.Log("Transpose Camera setting..");
            targetTo_fw = Vector3.ProjectOnPlane(vCam.transform.forward, Vector3.up).normalized;
            if(transposer_Orbit != null)
            {
                transposer_Orbit.m_XAxis.Value += look.x * 3.0f;
            }
            else
            {
                transposer_Orbit = vCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
            }
        }
        else
        {
            targetTo_fw = Quaternion.Euler(0f, look.x, 0f) * targetTo_fw;
        }
        
        if (targetTo_fw != null)
        {
            wishingVect = targetTo_fw * wish.y
            + Quaternion.Euler(0, 90, 0) * targetTo_fw * wish.x;
            //実際の視点.
            Debug.DrawLine(transform.position, transform.position + targetTo_fw * 3.0f);
        }
        //何も設定されていないときは世界基準として設定
        else
        {
            wishingVect = Vector3.forward * wish.y + Vector3.right * wish.x;
        }
    }

    //アニメーション更新操作・設定.
    void setanimPlay()
    {
        //Animancer版.
        if(animancerManager != null)
        {
            animancerManager.Tick((status.HitPauseTime <= 0));
            animationFrameTime = animancerManager.AM_AnimCurrentTime(0);
            animationEndTime = animancerManager.AM_AnimEndTime(0);
        }
    }
    
    //基本はAnimDefを呼び出し.
    //多分これでChangeAnimFromParantでも行けるはず
    public void ChangeAnim(AnimDef animDef = null, int LayerID = 0, 
    float timeoffset = 0.0f, Entity refEntity = null, AvatarMask mask = null, bool isAdditive = false)
    {
        if (mainAnimancer != null && animator != null)
        {
            if (animDef != null)
            {
                animID = animDef.ID;
                animancerManager.TransitLayer(animDef, LayerID, timeoffset, mask, isAdditive);
            }
            else
            {
                AnimDef initDef = animDefs.Find(fs => fs.ID == animID);
                if(initDef != null)
                {
                    animancerManager.TransitLayer(initDef, LayerID, timeoffset, mask, isAdditive);
                }
                else
                {
                    Debug.Log("anim id not found @ Animancer");
                }
            }
        }
    }
    
    public void ChangeAnimParam(Vector2 animParamValue, int LayerID = 0)
    {
        if (mainAnimancer != null && animator != null)
        {
            animancerManager.AM_AnimParamSet(animParamValue, LayerID);
        }
    }

    public void FadeAnim(int LayerID = 0, float fadeTime = 0)
    { 
        if (mainAnimancer != null && animator != null)
        {
            animancerManager.AM_FadeLayer(LayerID,fadeTime);
        }

    }

    public void entityPhisics()
    {
        capCol.enabled = true;
        if (physicsType != _PhysicsType.N)
        {
            //set gravity.
            //rigid.velocity += Physics.gravity * Time.fixedDeltaTime;
        }
        bool isPaused = (status.HitPauseTime > 0);
        if (isPaused)
        {
            if(!rigid.isKinematic)
            {
                pausedVel = rigid.velocity;
            }
            rigid.isKinematic = isPaused;
        }
        else
        {
            rigid.isKinematic = false;
            if (pausedVel != Vector3.zero)
            {
                //Debug.Log("unpaused");
                rigid.velocity = pausedVel;
                pausedVel = Vector3.zero;
            }
        }
    }


    //2026-01-17
    //Animancer
    public bool hitCheck(Entity checkEntity, out Vector3 HitPoint)
    {
        bool resl = false;
        Debug.Log("Check Clss between " + gameObject.name + " and " + checkEntity.gameObject.name);
        clssSetting cEnemy = checkEntity.animancerManager.primaryAnimDef.clssSetting;
        clssSetting cSelf = animancerManager.primaryAnimDef.clssSetting;
        HitPoint = Vector3.zero;
        if (cEnemy != null && cSelf != null)
        {
            //比較対象のentityの時間が取れてなーい！！
            resl = cSelf.clssCollided
            (out Vector3 v1, out Vector3 v2, out float d,
            clssDef.ClssType.Attack, cEnemy, 0f);
            HitPoint = (v1 + v2) / 2f;
        }
        else
        {
            Debug.Log("cant find enemy Clss!");
        }
        return resl;
    }
    
    //2026-01-17
    //Animancer
    public bool hitCheck(clssSetting checkerSetting, out Vector3 HitPoint)
    {
        bool resl = false;
        clssSetting cSelf = animancerManager.primaryAnimDef.clssSetting;
        HitPoint = Vector3.zero;
        if (checkerSetting != null && cSelf != null)
        {
            //比較対象のentityの時間が取れてなーい！！
            resl = cSelf.clssCollided
            (out Vector3 v1, out Vector3 v2, out float d,
            clssDef.ClssType.Attack, checkerSetting, 0.1f);
            HitPoint = (v1 + v2) / 2f;
        }
        else
        {
            Debug.Log("cant find enemy Clss!");
        }
        return resl;
    }

    //entityの基本加速値とか、ステータスを決定しなければ..
    //ソフト　な加速度を実現するためのヤツ.
    public Vector3 softVelocity(Vector3 ManipVect, Vector3 speed, Vector3 forceMean)
    {
        //entity.rigid.velocity = entity.wishingVect * 100.0f * Time.fixedDeltaTime;
        Vector3 wishVect = GroundNormal != Vector3.zero ? Vector3.ProjectOnPlane(ManipVect, GroundNormal) : ManipVect;

        //POnPlaneが0になるのだけは防ぐ
        //現在の速度を地上法線方向で考える.  (プレイヤー前向きとなる移動値 + プレイヤー右向きの移動値)
        Vector3 CurrentForwardVect = Vector3.ProjectOnPlane(rigid.velocity + transform.forward * 0.0001f , GroundNormal);
        Vector3 CurrentRightVect = (Quaternion.AngleAxis(90f, GroundNormal) * CurrentForwardVect);
        
        //このLimiterが1以上あるなら、その速度を加算しない、と考える..
        //プレイヤーの一定速度値を超えた際は1以上を与える.
        //MoveAxisInputを考慮して、異常な加速を抑える.
        //また、wishingVectが0ならDotも0となるので..
        float ForwardMean = 
        Vector3.Dot(wishVect, CurrentForwardVect.normalized);
        float RightMean = 
        Vector3.Dot(wishVect, CurrentRightVect.normalized);

        //wishingVectのSpeedMaxを考慮したMean値.
        Vector3 FWMean = CurrentForwardVect.normalized * ForwardMean * 
        (1.001f - Mathf.Clamp((CurrentForwardVect.magnitude * Mathf.Sign(ForwardMean)) / speed.x, -Mathf.Infinity, 1f )) * forceMean.x;
        //これが無いと方向転換が不可能.
        Vector3 RGMean = CurrentRightVect.normalized * RightMean * 
        (1.001f - Mathf.Clamp((CurrentRightVect.magnitude * Mathf.Sign(RightMean)) / speed.z, -Mathf.Infinity , 1f)) * forceMean.z; 
        

        //ひとまず、CurrentVect値の速度を演算
        //速度以上のwishVectが加算された場合はごく僅かな値を加算する
        //角速度も考えるかぁ..
        Vector3 ManipFW = FWMean + RGMean;
        return ManipFW;
    }

    //細かいことを考えずに繰り出すヤツ.
    //yは必ずvector up.
    public Vector3 hardVelocity(Vector3 ManipVect)
    {
        Vector3 fws = Vector3.ProjectOnPlane(transform.forward, GroundNormal).normalized * ManipVect.x;
        Vector3 rws = Vector3.ProjectOnPlane(transform.right, GroundNormal).normalized * ManipVect.z;
        Vector3 ups = Vector3.up * ManipVect.y;
        return fws + rws + ups;
    }

    internal List<ChangeStateQueue> CListQueue = new List<ChangeStateQueue>();

    internal struct ChangeStateQueue
    {
        public int stateDefID;
        public int priority;
    }

    internal GameObject makeInstantiate(GameObject gObj, Vector3? position, string boneName = "", bool isAligned = false)
    {
        Transform findBone = Elem.getEntityBoneTransform(this,boneName);
        Vector3 pos = position.HasValue ? position.Value : Vector3.zero; 
        Quaternion rot = isAligned ? transform.rotation : Quaternion.identity;
        return Instantiate(gObj, findBone.position + pos, rot);
    }

    internal char checkHitStates()
    {
        //Fall中ならFという特殊フラグを当てる.
        if (attrs.isFall == true)
        {
            return 'F';
        }
        else
        {
            return (char)stateType;
        }
    }

    const float destroTime_Max = 1.0f;
    const float destroTime_meshRev = 0.033f;
    float destroTime_Current = 0f;

    //点滅後に消滅したい.
    internal void setDestroy()
    {
        if (attrs.isEraseReady)
        {
            destroTime_Current += Time.fixedDeltaTime;
            float rep = Mathf.Repeat(destroTime_Current, destroTime_meshRev);
            bool isDisabled = (destroTime_meshRev * (1f / 2f) > rep);
            // Debug.Log("entity renderer : " + isDisabled + " " + rep);
            foreach (Renderer r in renderers)
            {
                r.enabled = isDisabled;
            }
            //アニメーションとかを切断して、消し飛ばす.
            if (destroTime_Current >= destroTime_Max)
            {
                //gameState.self.AddKillValue();
                Destroy(gameObject);
            }
        }
    }

    //ワンショットサウンドを鳴らす.
    internal void SetPlayOneShot(AudioClip audio)
    {
        if (selfSource != null && audio != null)
        {
            selfSource.PlayOneShot(audio);
        }
        attrs.isSoundNotPlayed = 1;
    }

    //LuaScript上で呼び出し可能にする.
    // 例えば、//掴み側(ステート奪取側Entity)の右手のボーンの位置から
    // 掴まれ側(ステート非奪取側Entity)の首のボーンの位置 を 除算することで
    // 掴みモーションに合わせて締められモーションを追随することができる.
    public Transform getBoneTransform(string F_str = "root")
    {
        //entity中のすべての階層のtransformを取得.
        List<Transform> tList = allChildTransforms.ToList();
        Transform findTransform = tList.Find(trs => trs.name == F_str);
        if (findTransform != null)
        {
            return findTransform;
        }
        //if not found, return root transform.
        return transform;
    }

    //ignoring self collider for what time.
    public void ignoreCollider()
    {
        capCol.enabled = false;
    }

    //足元サウンド. あると臨場感出るんじゃね。
    public void SetStepSound(int iVal = 0)
    {
        AudioClip[] clip = gameState.self.defaultFootSound;
        AudioClip select_cl = clip[UnityEngine.Random.Range(0,clip.Count())];
        if(select_cl != null)
        {
            selfSource.PlayOneShot(select_cl,0.25f);            
        }
    }
}

