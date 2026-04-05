

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using System;
using UnityEngine;

//EditorExtention. for deepcopy.
public static class ObjectExtension
{
    // ディープコピーの複製を作る拡張メソッド
    public static T DeepClone<T>(this T src)
    {
        using (var memoryStream = new System.IO.MemoryStream())
        {
            var binaryFormatter
            = new System.Runtime.Serialization
                    .Formatters.Binary.BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, src); // シリアライズ
            memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
            return (T)binaryFormatter.Deserialize(memoryStream); // デシリアライズ
        }
    }
}

//StateControllerに入力されるジェネリックの属性値に合わせ、計算.
//Vector3とかstringとか入れられるようにしたい.
//2025-06-28
//I NEED TO MARK THIS WORK ON HITDEF
[System.Serializable]
public class stParams<Type>
{
    //デフォルト値の設定.
    //stParamsを設定する際は必ず初期化を行うとする.
    internal Type defaultValue;


    //必須等設定されている場合
    public stParams(Type defValue, bool setEssential, bool setReadable)
    {
        stParamValue = defValue;
        _setEssential = setEssential;
        _isReadable = setReadable;
        _MenuName = "Base";
        _setHidden = true;
    }

    //必須等設定されている場合
    public stParams(Type defValue, bool setEssential)
    {
        stParamValue = defValue;
        _setEssential = setEssential;
        _MenuName = "Base";
        _setHidden = true;
        _setEssential = false;
    }

    //デフォルト値が設定されている場合なら初期は隠す.
    public stParams(Type defValue)
    {
        stParamValue = defValue;
        _setHidden = true;
        _setEssential = false;
        _MenuName = "Base";
    }

    //何も設定されていない際...
    //はTypeの新規インスタンス作成時になんか不具合起こしそう..でもない？
    public stParams()
    {
        _setHidden = true;
        _setEssential = false;
        _MenuName = "Base";        
    }
    


    //MUGENのHITDEFをそのまま移植するとInspectorが大変なことになるので、
    //必須値以外は隠せるようにしたい.

    //この値が設定されているなら, Inspector上では隠されるようになる.. 右クリックのメニューで解除される.
    [SerializeField]
    public bool _setHidden = true;

    [SerializeField]
    //この値が設定されているなら, "必ず"隠されない.
    public bool _setEssential = true;

    [SerializeField]
    //この値がfalse設定されているなら, 返却値はnullを返す. 
    public bool _isReadable = true;

    //inspectorで隠すかどうかの設定項目のメニュー位置.
    [SerializeField]
    public string _MenuName = "Base";

    public bool isHidden()
    {
        return _setHidden;
    }

    private void toggleHidden()
    {
        _setHidden = !_setHidden;
    }

    //実行されたLuaCondition中の変数を読み出すかを後述するEnumに合わせて考慮.
    [SerializeField]
    loadType loadTypes;

    [SerializeField]
    //valueに入力された値を考慮して、ConditionElem等に代入
    Type stParamValue;

    //LuaConditionで読み出すパラメーターID
    [SerializeField]
    int useID = -1;

    //
    [SerializeField]
    Elem LuaCondition = new Elem();

    //Luaで読み出すメソッド名
    [SerializeField]
    string stLuaLoads = "";

    //mjs等のスクリプト指定. 基本的に呼び出されたStateDefの値を用いる.
    internal string modulePath = ""; 

    delegate object luaCalcParam(Entity entity);

    //どの形式で値を読み出すかをenumで管理する.
    public enum loadType
    {
        Constant,
        Condition,
        Calclation
    }

    //登録値を読み出す.
    public Type valueSet(Type val)
    {
        return val;
    }

    //実際に想定された値を読み出す.
    //Condition/Calclationではluaの内容を読み出したいが..

    public Type valueGet(List<object> loadParams, Entity entity)
    {
        Puerts.JsEnv env = PuerTS_Framework.main.JSEnv;
        Type retValue = stParamValue;
        switch (loadTypes)
        {
            //Conditionなら読み出されたLuaConditionに登録されたvalue配列から..
            //としたい. 
            case loadType.Condition:
                {
                    if(loadParams.Count > useID && useID >= 0)
                    // Debug.Log(entity.gameObject.name + " tries envs " 
                    // + loadParams[useID].GetType() + "to match " + retValue.GetType());
                    retValue = (Type)loadParams[useID];
                    break;
                }
            //Calclationなら読み出すLuaCondition中に書かれたfunctionを実行しその値を読み出す.
            case loadType.Calclation:
                {
                    luaCalcParam calcParam =
                    env.ExecuteModule<luaCalcParam>(modulePath, stLuaLoads);
                    retValue = (Type)calcParam.Invoke(entity);
                    break;
                }
            //コンスタント値または未定義ならstParamvalueをそのまま使用.
            case loadType.Constant:
            default:
                break;

        }
        if(_isReadable)
        {
            return retValue;
        }
        else
        //this will returns null I guess..
        {
            return default(Type);
        }
    }

    //LuaEnvで実行されたLuaEnvの登録値を読み出して、それをvalueSetに実行.
    /*
        public Type getLuaElem()
        {
            Type type = new ;
            if (useLuaCondition && useID > -1)
            {

            }
        }
    */
}

//
[System.Serializable]
public class stateID
{
    [SerializeField]
    bool useLua = false;

    //読み出すステートID
    [SerializeField]
    internal int value = 0;

    //読み出すLuaのパラメータ名
    [SerializeField]
    string stLuaLoads = "";
    delegate bool luaBooltoLoad(Entity entity);

    //Luaの状態・もしくは入力されたIDがこのidと同様の時にtrueを返す。
    public bool valueGet(int[] loadID, Entity entity)
    {
        bool retValue = false;
        // LuaEnv env = Lua_OnLoad.main.LEnv;
        //Luaを使用するなら
        if (useLua && stLuaLoads != "")
        {
            // luaBooltoLoad calcParam =
            // env.Global.Get<luaBooltoLoad>(stLuaLoads);
            // retValue = calcParam.Invoke(entity);
        }
        else
        {
            retValue = (loadID.Count() > 0 && loadID.Any(i => i == value));
        }
        return retValue;
    }
}

//StateDef追加時のメニュー階層Attr用.
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class SCHiearchyAttribute : Attribute
{
    private string _hiearchyName = "";
    public SCHiearchyAttribute(string name) { this._hiearchyName = name; }
    public string Name { get { return this._hiearchyName; } }
}

//StateDefのクローンが必須.
//なんかEntityの指定が重複していそう.
[System.Serializable]
public class StateDef
{
    public string StateDefName = "Default";
    public int StateDefID = 0;

    //フレーム数で考慮
    internal int stateTime = 0;

    //executing state is decided from this.
    public string ScriptDirectory;
    //executing state is decided from this.
    public string ScriptName;

    public string preStateVerdictName;
    public string ParamLoadName;

    // private LuaTable _stateLoadTables;
    // private LuaTable _stateParamTables;

    //int for state-Execution, Object for preverdicted-Parameters.
    List<int> ExecuteStates;
    List<System.Object> StateParams;


    //それぞれMoveType
    public char stateType;
    public char moveType;
    public char physType;

    [SerializeReference, SerializeField]
    public List<StateController> StateList = new List<StateController>();

    public string Dir;

    //LuaCondition内でstateDefParamsで定義された値を受け取るためのクラス.
    //...Objectで良いのか？
    List<object> luaOutputParams = new List<object>();

    //ctrlフラグの設定.
    public bool setCtrl = false;

    //これも結局Cloneが必須かぁ..
    public StateDef Clone()
    {
        var retDef = new StateDef();
        retDef.StateDefName = StateDefName;
        retDef.StateDefID = StateDefID;
        retDef.stateTime = stateTime;
        retDef.ScriptDirectory = ScriptDirectory;
        retDef.ScriptName = ScriptName;
        retDef.preStateVerdictName = preStateVerdictName;
        retDef.ParamLoadName = ParamLoadName;
        retDef.StateList = StateList;
        retDef.luaOutputParams = luaOutputParams;
        retDef.stateType = stateType;
        retDef.moveType = moveType;
        retDef.physType = physType;
        return retDef;
    }

    //実行仮想環境とJavaScriptの実行モジュールオブジェクト.
    Puerts.JsEnv env;
    Puerts.JSObject executer;

    void OnInitDef()
    {
        //PuerTS用に改変中.
        env = PuerTS_Framework.main.JSEnv;
        //ExecuteModuleで使用するスクリプトデータを読み込ませる. 
        Dir = ScriptDirectory + "/" + ScriptName;
        //Debug.Log("script Directory " + Dir + " : at ID of " + StateDefID);
    }

    //Execute時のLuaのStateIDをそれぞれのStateDefに保存 - これ、掴みの時のEntity参照時の設定時に重複発生しそー..    
    //

    void entityTypeSet(Entity entity)
    {
        // Debug.Log("EntityTypeSet Executed - " +
        // stateType.ToString() + " , " + physType.ToString() + " , " + moveType.ToString());
        switch (stateType)
        {
            case 's':
            case 'S':
                {
                    entity.stateType = Entity._StateType.S;
                    break;
                }
            case 'a':
            case 'A':
                {
                    entity.stateType = Entity._StateType.A;
                    break;
                }
            case 'l':
            case 'L':
                {
                    entity.stateType = Entity._StateType.L;
                    break;
                }
        }
        switch (physType)
        {             
            case 's':
            case 'S':
                {
                    entity.physicsType = Entity._PhysicsType.S;
                    break;
                }
            case 'a':
            case 'A':
                {
                    entity.physicsType = Entity._PhysicsType.A;
                    break;
                }
            case 'n':
            case 'N':
                {
                    entity.physicsType = Entity._PhysicsType.N;
                    break;
                }
        }
        switch (moveType)
        {             
            case 'i':
            case 'I':
                {
                    entity.moveType = Entity._MoveType.I;
                    break;
                }
            case 'a':
            case 'A':
                {
                    entity.moveType = Entity._MoveType.A;
                    break;
                }
            case 'h':
            case 'H':
                {
                    entity.moveType = Entity._MoveType.H;
                    break;
                }
        }
    }

    //PUERTSの実装を開始する.
    //あと、実行時に読み出したステート番号をList形式で出力する.
    public List<int> Execute(Entity entity, bool willPrevLoad)
    {
        List<int> executedStateID = new List<int>();
        List<StateController> selectLoad = StateList.FindAll(stB => stB.doLoadAfterChangeState == willPrevLoad);
        
        //stateTimeが0の時, 恒常設定されたステートパラメータを確認
        if (entity.stateTime == 0)
        {
            entityTypeSet(entity);
        }

        if (executer == null)
        { 
            OnInitDef();
        } 

        if (ScriptDirectory != null && selectLoad.Count > 0)
        {     
            
            executer = PuerTS_Framework.main.JSEnv.ExecuteModule(Dir);

            //executeStatesとStateParamsの初期化
            ExecuteStates = new List<int>();
            StateParams = new List<System.Object>();

            //Debug.Log("Executed PuerTS : At StateDefID " + StateDefID);
            //Func型じゃないと取れなかったんじゃないっけ？
            Func<Entity, List<int>> executer_stateIDGet = executer.Get<Func<Entity, List<int>>>(preStateVerdictName);
            Func<Entity, List<object>> executer_stateParamGet = executer.Get<Func<Entity, List<object>>>(ParamLoadName);

            //Func型として返される値を格納

            if (executer_stateIDGet != null)
            {
                ExecuteStates = executer_stateIDGet(entity);
            }
            else
            {
                Debug.LogWarning(entity.gameObject.name + " loads the script function :" + preStateVerdictName +
                "but returns not found.");
            }
            if (executer_stateParamGet != null)
            { 
                luaOutputParams = executer_stateParamGet(entity);
            }

            //for debug string
            string executingStr = "";
            if (ExecuteStates != null)
            {
                for (int i = 0; i < ExecuteStates.Count(); i++)
                {
                    executingStr += ExecuteStates[i] + " , ";
                }
                //Debug.Log(executingStr); 
            }
            foreach (StateController state in selectLoad)
            {
                //idがステート読み出しリスト内・もしくはステート自体が読み出し処理を行う場合
                //Debug.LogWarning(entity.gameObject.name + " loads " + state.ID.value.ToString());
                if (ExecuteStates != null && state.isIDValid(ExecuteStates.ToArray(), entity))
                {
                    //stateにluaOutputParamsを予め登録.
                    state.loadParams = luaOutputParams;

                    //Debug.Log("Executed" + state.ID);

                    //実際に実行.
                    //state.Entityに直接登録すると、別キャラクターが参照するため変更..
                    state.OnExecute(entity);
                    executedStateID.Add(state.ID.value);
                }
            }
        }
        return executedStateID;
    }    
}


//ステートベースクラス. ここから派生する. なお、実行判別式はLuaを用いることとする.
//Lua内にはここで用意された関数を流用する.

//2025-05-23
//ステコンのloadParamsにはLuaで"計算済み"の値を考える.
[System.Serializable]
[SerializeField]
[SCHiearchy("null")]
public class StateController
{   
    [ReadOnly]
    //事前計算済みのパラメータの格納.
    internal List<object> loadParams;

    //stateIDはLuaに送られた,事前計算での情報とLua読み出しのスクリプトで判別する.
    //これもLua事後計算のパラメータとして組み込んで考えるべきだろうか？
    [SerializeField]
    public stateID ID;

    public bool isIDValid(int[] ID, Entity entity)
    {
        return this.ID.valueGet(ID, entity);
    }
    public string stateControllerSubName;

    //ChangeState後にPrevStateとして読み出すか?
    public bool doLoadAfterChangeState = false;

    internal static string stControllerName = null;
    

    internal virtual void OnExecute(Entity entity)
    {

    }

    public virtual string typeGet()
    {
        return this.ToString();
    }
}


//アニメーションの変更.　このステートマシンではステート奪取状態かどうかに関わらず
//ベースのアニメーションを再生する.
[System.Serializable]
[SerializeField]
[SCHiearchy("Animation/AnimSet")]
public class scAnimSet : StateController
{
    [SerializeField]
    stParams<int> changeAnimID;

    //if it is not set, change 0(main) slot. 
    //latter Slot must needs to be Additional.
    [SerializeField]
    stParams<int> AnimSlot =
    new stParams<int>{
        _setEssential = true
    };
    
    [SerializeField]
    stParams<Vector2> animParameter; 

    [SerializeField]
    stParams<bool> isAdditional; 

    [SerializeField]
    AvatarMask mask;

    internal override void OnExecute(Entity entity)
    {
        int AnimSlotNum = AnimSlot.valueGet(loadParams,entity);
        bool _isAdditional = isAdditional.valueGet(loadParams,entity);
        AnimDef animFindByID = entity.animDefs.ToList().Find
        (x => x.ID == changeAnimID.valueGet(loadParams, entity));
        //設定されたIDが見つかれば、そのParameterと同様に設定..
        //ここでAnimIDが設定されているため、entityからChangeAnimを呼び出せば簡易に変えられるはず.
        if (animFindByID != null)
        {
            entity.ChangeAnim(animFindByID,AnimSlotNum,animFindByID.blendInTime
            ,null,mask,_isAdditional);
        }
    }
}

//指定したアニメーションレイヤーのアニメを停止・消去する.
[System.Serializable]
[SerializeField]
[SCHiearchy("Animation/End the animation via select Layer")]
public class scAnimEnd : StateController
{
    //if it is not set, change 0(main) slot. 
    //latter Slot must needs to be Additional.
    [SerializeField]
    stParams<int> AnimSlot;
    [SerializeField]
    stParams<float> fadeTime;
    internal override void OnExecute(Entity entity)
    {
        int AnimSlotNum = AnimSlot.valueGet(loadParams, entity);
        float fading = fadeTime.valueGet(loadParams, entity);
        entity.FadeAnim(AnimSlotNum, fading);
    }
}


//アニメーションパラメータの変更.
//上書き設定とかはいじらない。
[System.Serializable]
[SerializeField]
[SCHiearchy("Animation/AnimParamchange")]
public class scAnimParamChange : StateController
{
    [SerializeField]
    stParams<int> changeAnimID;
    
    [SerializeField]
    stParams<Vector2> animParameter;
    //first, find Anim paramID then set Animslots.
    [SerializeField]
    stParams<int> AnimSlot;

    internal override void OnExecute(Entity entity)
    {
        //Animancer版.
        if (entity.animancerManager != null)
        {
            entity.ChangeAnimParam
            (animParameter.valueGet(loadParams, entity), 
            AnimSlot.valueGet(loadParams, entity));
        }
    }
}

[SCHiearchy("Animation/AnimChange from Parent")]
//ステート奪取された"親"のアニメを再生する.
public class scAnimParentSet : StateController
{
    [SerializeField]
    stParams<int> changeAnimID;

    //if it is not set, change 0(main) slot. 
    //latter Slot must needs to be Additional.
    [SerializeField]
    stParams<int> AnimSlot =
    new stParams<int>{
        _setEssential = true
    };
    
    [SerializeField]
    stParams<Vector2> animParameter; 

    [SerializeField]
    stParams<bool> isAdditional; 

    [SerializeField]
    AvatarMask mask;

    //Parent Version.
    internal override void OnExecute(Entity entity)
    {
        int AnimSlotNum = AnimSlot.valueGet(loadParams,entity);
        bool _isAdditional = isAdditional.valueGet(loadParams,entity);
        //entity.parentEntity.animID = changeAnimID.valueGet(loadParams, entity);
        AnimDef animFindByID = entity.controlledEntity.animDefs.Find
        (x => x.ID == changeAnimID.valueGet(loadParams, entity));
        //設定されたIDが見つかれば、そのParameterと同様に設定..
        if (animFindByID != null)
        {
            entity.ChangeAnim(animFindByID,AnimSlotNum,animFindByID.blendInTime
            ,null,mask,_isAdditional);
        }
    }
}

//Groundに設定された法線方向に移動させる.
[System.Serializable]
[SerializeField]
[SCHiearchy("Physics/Basic Controllable movement")]
public class scBasicInputMove : StateController
{
    [SerializeField]
    stParams<float> MutipleVelocity = new stParams<float>(1f, true, true);

    [SerializeField]
    stParams<Vector3> MutipleMoveVelocity = new stParams<Vector3>(new Vector3(1f,1f,1f), true, true);

    //velocityで設定してたので、これを別のやつにする..
    //ハマったとき、抜けなくなるので..
    internal override void OnExecute(Entity entity)
    {
        Vector3 MVels = MutipleMoveVelocity.valueGet(loadParams,entity);
        

        entity.rigid.AddForce(entity.softVelocity(entity.wishingVect * MutipleVelocity.valueGet(loadParams, entity), 
        new Vector3(entity.status.BaseMoveVelocityParam.x * MVels.x,entity.status.BaseMoveVelocityParam.y * MVels.y,entity.status.BaseMoveVelocityParam.z * MVels.z), 
        entity.status.BaseAccelParam), ForceMode.Force);
        //entity.rigid.AddTorque(Vector3.up * rightMean / forceMean ,ForceMode.Force);
    }

    public override string typeGet()
    {
        return "scMove";
    }
}

//Groundに設定された法線方向に移動させる.
[System.Serializable]
[SerializeField]
[SCHiearchy("Physics/Basic movement Axisied by Fw-Face")]
public class scBasicForwardMove : StateController
{    
    [SerializeField]
    stParams<Vector3> MoveVelocity = new stParams<Vector3>(new Vector3(10f,0f,0f), true);

    [SerializeField]
    stParams<bool> isSpeedstatAffect = new stParams<bool>(false,true);


    //velocityで設定してたので、これを別のやつにする..
    //ハマったとき、抜けなくなるので..
    internal override void OnExecute(Entity entity)
    {
        Vector3 MVels = MoveVelocity.valueGet(loadParams,entity);
        MVels *= isSpeedstatAffect.valueGet(loadParams,entity) ? entity.status.BaseMoveVelocityParam.x : 1f;
        entity.rigid.AddForce(entity.hardVelocity(MVels), ForceMode.Force);
        //entity.rigid.AddTorque(Vector3.up * rightMean / forceMean ,ForceMode.Force);
    }
}

[System.Serializable]
[SerializeField]
[SCHiearchy("Physics/Add velocity")]
public class scAddVelocity : StateController
{
    [SerializeField]
    stParams<Vector3> vels;

    [SerializeField]
    int priority = 0;
    internal override void OnExecute(Entity entity)
    {
        entity.rigid.velocity += vels.valueGet(loadParams,entity) * Time.fixedDeltaTime;
    }

    public override string typeGet()
    {
        return "scAddVels";
    }
}

[System.Serializable]
[SerializeField]
[SCHiearchy("Physics/Set velocity")]
public class scSetVelocity : StateController
{
    [SerializeField]
    stParams<Vector3> vels;

    [SerializeField]
    int priority = 0;
    internal override void OnExecute(Entity entity)
    {
        entity.rigid.velocity = vels.valueGet(loadParams,entity) * Time.fixedDeltaTime; 
    }

    public override string typeGet()
    {
        return "scAddVels";
    }
}

//攻撃判定設定 - 指定の攻撃をシステムに予約する
//攻撃があたった対象を予約されたステート番号5000..
//ぶっちゃけめんどくせー。
[System.Serializable]
[SerializeField]
[SCHiearchy("Attack/HitDef")]
public class scHitDef : StateController
{
    //GethitDefを検索する際に登録されるID
    [SerializeField]
    stParams<int> hitID = new stParams<int>
    {
        _setEssential = true
    };

    //ダメージ集. プレイヤーダメージとかは別のstateControllerに登録する.
    [SerializeField]
    stParams<float> damage = new stParams<float>(0f,true,true)
    {
        _setEssential = true,
        _MenuName = "Damages"
    };
    

    //ダメージ集. プレイヤーダメージとかは別のstateControllerに登録する.
    [SerializeField]
    stParams<string> hitPhysFlag = new stParams<string>("SA",true,true)
    {
        _setEssential = true,
        _MenuName = "Damages"
    };    
    [SerializeField]
    stParams<string> hitMoveFlag = new stParams<string>("IAH",true,true)
    {
        _setEssential = true,
        _MenuName = "Damages"
    };

    [SerializeField]
    stParams<string> guardPhysFlag = new stParams<string>("SA",true,true)
    {
        _setEssential = true,
        _MenuName = "Damages"
    };    
    [SerializeField]
    stParams<string> guardMoveFlag = new stParams<string>("IAH",true,true)
    {
        _setEssential = true,
        _MenuName = "Damages"
    };

    [SerializeField]
    stParams<float> guardDamage = new stParams<float>(0f,false,false)
    {
        _setEssential = false,
        _MenuName = "Damages",

    };

    [SerializeField]
    stParams<float> guardBreakPoint = new stParams<float>(0f,false,false)
    {
        _setEssential = false,
        _MenuName = "Damages"
    };

    [SerializeField]
    stParams<float> guardBreakPoint_OnGuard = new stParams<float>(0f,false,false)
    {
        _setEssential = false,
        _MenuName = "Damages"
    };

    //基本ダメージ・ガード時の削りで倒せるか？
    [SerializeField]
    stParams<bool> Kill = new stParams<bool>(true,false,false)
    {
        _MenuName = "Damages"
    };

    [SerializeField]
    stParams<bool> guardOnKill = new stParams<bool>(false,false,false)
    {
        _MenuName = "Damages"
    };

    //ノックバック速度設定.
    [SerializeField]
    stParams<Vector3> hitVelSet = new stParams<Vector3>(Vector3.zero, false, true)
    { 
        _MenuName = "Physics"
    };

    [SerializeField]
    stParams<Vector3> guardhitVelSet = new stParams<Vector3>(Vector3.zero, false, false)
    { 
        _MenuName = "Physics"
    };

    //プレイヤーノックバック速度設定.
    [SerializeField]
    stParams<Vector3> player_hitVelSet = new stParams<Vector3>(Vector3.zero, false, false)
    { 
        _MenuName = "Physics"
    };

    [SerializeField]
    stParams<Vector3> player_GuardhitVelSet = new stParams<Vector3>(Vector3.zero, false, false)
    { 
        _MenuName = "Physics"
    };

    
    [SerializeField]
    stParams<string> hitAnimType = new stParams<string>("", false, false)
    { 
        _MenuName = "Animation"
    };

    //HitStopに関しては x -> 当てた対象, y -> 当てた本人 の時間として Vector2で管理する.
    [SerializeField]
    stParams<Vector2Int> hitStopTime = new stParams<Vector2Int>(Vector2Int.zero, true, true)
    {
        _MenuName = "Animation"
    }; 
    
    [SerializeField]
    stParams<Vector2Int> guardHitStopTime = new stParams<Vector2Int>(Vector2Int.zero, false, false)
    {
        _MenuName = "Animation"
    }; 

    [SerializeField]
    stParams<GameObject> hitEffect = new stParams<GameObject>(null,false,false)
    {
        _MenuName = "Animation"
    };
    
    [SerializeField]
    stParams<GameObject> guardhitEffect = new stParams<GameObject>(null,false,false)
    {
        _MenuName = "Animation"
    };
    

    //当たり判定の優先順位.
    [SerializeField]
    stParams<int> Priority = new stParams<int>(0,true,true)
    {
        _MenuName = "Controller"
    };

    //当たり判定の同値時設定.
    //H it, M iss, D odge で分けられる.
    [SerializeField]
    stParams<char> Priority_Behavior = new stParams<char>('H',true,true)
    {
        _MenuName = "Controller"
    };

    //投げとか、ステート飛ばしを実行する際に呼び出し.
    [SerializeField]
    stParams<int> ChangeState_OwnerStateID = new stParams<int>(-1,false,false)
    {
        _MenuName = "Controller"
    };

    //MenuNameを考え中.. ステート奪取するならbool
    [SerializeField]
    stParams<int> ChangeState_TargetStateID = new stParams<int>(-1,false,false)
    {
        _MenuName = "Controller"
    };
    
    [SerializeField]
    stParams<bool> isTargetRefsOwnerID = new stParams<bool>(false,false,false)
    {
        _MenuName = "Controller"
    };

    [SerializeField]
    stParams<int> maxEntityHits = new stParams<int>(1,false,false)
    {
        _MenuName = "Controller"
    };

    [SerializeField]
    stParams<float> hitIntervalTime = new stParams<float>(1,false,false)
    {
        _MenuName = "Controller"
    };
    
    //x as Direct Hit, y as Guard.
    [SerializeField]
    stParams<Vector2Int> hitTime = new stParams<Vector2Int>(Vector2Int.zero,false,false)
    {
        _MenuName = "Controller"
    };

    [SerializeField]
    stParams<int> fallTime = new stParams<int>(1,false,false)
    {
        _MenuName = "Controller"
    };

    [SerializeField]
    stParams<string> StateIDExcluder = new stParams<string>("",false,false)
    {
        _MenuName = "Controller"
    };

    //自分がもらうパワー増減管理.  x => ヒット時, y => ガード時.
    [SerializeField]
    stParams<Vector2> GetPower = new stParams<Vector2>(new Vector2(0,0),false,false)
    {
        _MenuName = "Status"
    };
    //相手がもらうパワー増減管理.  x => ヒット時, y => ガード時.
    [SerializeField]
    stParams<Vector2> GivePower = new stParams<Vector2>(new Vector2(0,0),false,false)
    {
        _MenuName = "Status"
    };


    //旧Param. これはReferenceのため取っておく..
    [SerializeField]
    stParams<hitDefParams> hitParams;


    internal override void OnExecute(Entity entity)
    {
        List<int> excluder = new List<int>();
        string exc = StateIDExcluder.valueGet(loadParams,entity);
        if(exc != null && exc.Length > 0)
        {
            foreach(string strs in exc.Split(","))
            {
                if(int.TryParse(strs, out int resl))
                {
                excluder.Add(resl);
                }
            }
        }
        //on execute, register this monstrous parameters..
        hitDefParams HitDef = new hitDefParams()
        {
            hitID = hitID.valueGet(loadParams, entity), //essential!
            //damages
            Damage = damage.valueGet(loadParams, entity), //essential!
            GuardDamage = 
            guardDamage._isReadable ? guardDamage.valueGet(loadParams, entity) : 0,
            GuardBreakPoint = 
            guardBreakPoint._isReadable ? guardBreakPoint.valueGet(loadParams, entity) : 0,
            GuardBreakPoint_OnGuard = 
            guardBreakPoint._isReadable ? guardBreakPoint_OnGuard.valueGet(loadParams, entity) : 0,
            
            Kill = Kill._isReadable ? Kill.valueGet(loadParams,entity) : true,
            GuardOnKill = guardOnKill._isReadable ? guardOnKill.valueGet(loadParams,entity) : false,

            //damages, for hitting
            HitMoveFlag = hitMoveFlag.valueGet(loadParams,entity), //essential!
            HitPhysFlag = hitPhysFlag.valueGet(loadParams,entity), //essential!
            GuardMoveFlag = guardMoveFlag.valueGet(loadParams,entity), //essential!
            GuardPhysFlag = guardPhysFlag.valueGet(loadParams,entity), //essential!

            //hit vect set
            velset = 
            hitVelSet._isReadable ? hitVelSet.valueGet(loadParams,entity) : Vector3.zero,
            guard_velset = 
            guardhitVelSet._isReadable ? guardhitVelSet.valueGet(loadParams,entity) : Vector3.zero,
            pl_velset = 
            player_hitVelSet._isReadable ? player_hitVelSet.valueGet(loadParams,entity) : Vector3.zero,
            pl_guard_velset = 
            player_GuardhitVelSet._isReadable ? player_GuardhitVelSet.valueGet(loadParams,entity) : Vector3.zero,

            //Animation types
            AnimType = 
            hitAnimType._isReadable ? hitAnimType.valueGet(loadParams,entity) : "L",
            hitStopTime = 
            hitStopTime._isReadable ? hitStopTime.valueGet(loadParams,entity) : Vector2Int.zero,
            guard_hitStopTime = 
            guardHitStopTime._isReadable ? guardHitStopTime.valueGet(loadParams,entity) : Vector2Int.zero,
            HitEff = hitEffect._isReadable ? hitEffect.valueGet(loadParams,entity) : null,
            GuardHitEff = 
            guardhitEffect._isReadable ? guardhitEffect.valueGet(loadParams,entity) : null,

            //Controller for state executions
            HitPriority = 
            Priority._isReadable ? Priority.valueGet(loadParams,entity) : 0,
            HitPriorityBehavior = 
            Priority_Behavior._isReadable ? Priority_Behavior.valueGet(loadParams,entity) : 'H',
            

            ChangeState_Target = 
            ChangeState_TargetStateID._isReadable ? ChangeState_TargetStateID.valueGet(loadParams,entity) : -1,
            ChangeState_Owner = 
            ChangeState_OwnerStateID._isReadable ? ChangeState_OwnerStateID.valueGet(loadParams,entity) : -1,
            TargetRefsOwnerNum =
            isTargetRefsOwnerID._isReadable ? isTargetRefsOwnerID.valueGet(loadParams, entity) : false,

            maxEntityHits = 
            maxEntityHits._isReadable ? maxEntityHits.valueGet(loadParams,entity) : 1,
            sameHitInterval = hitIntervalTime._isReadable ? hitIntervalTime.valueGet(loadParams,entity) : 0,

            HitTime = hitTime._isReadable ? hitTime.valueGet(loadParams,entity) : Vector2Int.zero,
            fallTime = fallTime._isReadable ? fallTime.valueGet(loadParams,entity) : 0,
            HitExcludeList = excluder
        };

        //HitDefをこの時点で呼び出すのはちょっとな―..
        //GameStateに登録させておきたい.
        if(gameState.self.ProvokeHitDef_Entity(entity, HitDef))
        {
            //Debug.DrawLine(,)
        }
    }

}

//攻撃判定設定 - 指定の攻撃をシステムに予約する
//攻撃があたった対象を予約されたステート番号5000..
//ぶっちゃけめんどくせー。
[System.Serializable]
[SerializeField]
[SCHiearchy("Attack/Projectile")]
public class scProjectile : StateController
{
    [SerializeField]
    Projectile projs;

    [SerializeField]
    stParams<Vector3> InstDirection;

    [SerializeField]
    stParams<Vector3> InstPosition;


    internal override void OnExecute(Entity entity)
    {
        //set projs. if its null do nothing
        if (projs != null)
        {
            GameObject inst = entity.makeInstantiate(projs.gameObject);
            inst.transform.position = InstPosition.valueGet(loadParams, entity);
            inst.GetComponent<Rigidbody>().velocity = InstDirection.valueGet(loadParams, entity);
            inst.GetComponent<Projectile>().proj_Controller = entity;
        }
    }

}

//後で消します.
//y方向へのimpulse型加速。
[System.Serializable]
[SerializeField]
[SCHiearchy("Physics/Jump")]
public class scJump : StateController
{
    internal override void OnExecute(Entity entity)
    {
        entity.rigid.velocity = Vector3.ProjectOnPlane(entity.rigid.velocity,Vector3.up) + Vector3.up * 3.2f;
        entity.isOnGround = false;
        //Debug.Log("Executed " + "JumpState " + " in " + entity.name + " - " + entity.stateTime);
    }
}

[System.Serializable]
[SerializeField]
[SCHiearchy("Misc/ColorChange")]
public class scColorChange : StateController
{        
    public Color color = Color.black;
    internal override void OnExecute(Entity entity)
    {
        // Debug.Log("Oncheck Phase of " + this.ToString());
        if(entity != null)
        {
            entity.CurColor = color;
        }
    }
}

[System.Serializable]
[SerializeField]
[SCHiearchy("System/ChangeState")]
public class scChangeState : StateController
{
    public int changeTo = 0;
    public int priority = 0;
    internal override void OnExecute(Entity entity)
    {
        //this ChangeState Needs to change-Queues.
        entity.CListQueue.Add(new Entity.ChangeStateQueue() { stateDefID = changeTo, priority = priority });
        // Debug.Log("stateTime set to 0");
        entity.isStateChanged = true;
        // Debug.Log("stchanged END");
    }

    public scChangeState()
    {
        this.priority = 0;
        this.changeTo = 0;
    }

    public scChangeState(int changeToID, int priority)
    {
        this.priority = priority;
        this.changeTo = changeToID;
    }
}

//移動方向に回転を加える.
[System.Serializable]
[SerializeField]
[SCHiearchy("Physics/RotateToSpeed")]
public class scRotateTowards : StateController
{
    public float RotateWeight = 0;
    internal override void OnExecute(Entity entity)
    {
        Vector3 vect = Vector3.ProjectOnPlane(entity.rigid.velocity, Vector3.up);
        //比較がepsilonだとダメっぽそう
        if (vect.sqrMagnitude > 0.01f)
        {
            Quaternion RotateTowards = Quaternion.LookRotation(vect.normalized, Vector3.up);
            entity.transform.rotation = Quaternion.Lerp(entity.transform.rotation, RotateTowards, RotateWeight);            
        }
    }
}


//移動方向に回転を加える.
[System.Serializable]
[SerializeField]
[SCHiearchy("Physics/RotateToLookTo")]
public class scRotateLookTo : StateController
{
    public float RotateWeight = 0;
    internal override void OnExecute(Entity entity)
    {
        //カメラ方向に回転.
        Quaternion RotateTowards = Quaternion.LookRotation(entity.targetTo_fw, Vector3.up);
        entity.transform.rotation = Quaternion.Lerp(entity.transform.rotation, RotateTowards, RotateWeight);
    }
}



//設定位置にエフェクトを放出する.
[System.Serializable]
[SerializeField]
[SCHiearchy("Effect/EmitEffect")]
public class scEmitEffect : StateController
{
    [SerializeField]
    stParams<GameObject> EmitObject;
    internal override void OnExecute(Entity entity)
    {
        entity.makeInstantiate(EmitObject.valueGet(loadParams, entity));
    }
}


//自己のステートの返還作業.
[System.Serializable]
[SerializeField]
[SCHiearchy("System/return SelfState")]
public class scSelfState : StateController
{
    [SerializeField]
    int changeTo = 0;

    [SerializeField]
    int priority = 0;
    internal override void OnExecute(Entity entity)
    {
        //コントロール対象を戻し、StateChangeを行う.
        entity.controlledEntity = null;
        scChangeState cState = new scChangeState(changeTo, priority);
        cState.OnExecute(entity);
    }
}

//status変更. (HP)
[System.Serializable]
[SerializeField]
[SCHiearchy("System/set Health")]
public class scAddHealth : StateController
{ 
    
    [SerializeField]
    int value = 0;

    [SerializeField]
    int priority = 0;
    internal override void OnExecute(Entity entity)
    {
        entity.status.currentHP += value;
    }
}

//status変更. (Energy)
[System.Serializable]
[SerializeField]
[SCHiearchy("System/add Energy")]
public class scAddEnergy : StateController
{

    [SerializeField]
    int value = 0;

    [SerializeField]
    int priority = 0;
    internal override void OnExecute(Entity entity)
    {
        entity.status.currentEnergy += value;
    }
}


//status変更. (Charge)
[System.Serializable]
[SerializeField]
[SCHiearchy("System/add Charge")]
public class scAddCharge : StateController
{ 
    
    [SerializeField]
    int value = 0;

    [SerializeField]
    int priority = 0;

    //これだけfixedDeltaTimeが乗算.
    internal override void OnExecute(Entity entity)
    {
        entity.status.ChargeTime += value * Time.fixedDeltaTime;
    }
}


//status変更. (Charge)
[System.Serializable]
[SerializeField]
[SCHiearchy("System/set Charge")]
public class scSetCharge : StateController
{ 
    
    [SerializeField]
    int value = 0;

    [SerializeField]
    int priority = 0;

    //これだけfixedDeltaTimeが乗算.
    internal override void OnExecute(Entity entity)
    {
        entity.status.ChargeTime = value;
    }
}

//ctrlフラグのセット.
[System.Serializable]
[SerializeField]
[SCHiearchy("System/set Ctrl")]
public class scSetCtrl : StateController
{     
    [SerializeField]
    stParams<bool> value;

    [SerializeField]
    int priority = 0;

    //これだけfixedDeltaTimeが乗算.
    internal override void OnExecute(Entity entity)
    {
        entity.attrs.ctrl = value.valueGet(loadParams,entity);
    }
}

//処理中・直後フレームの特殊操作など. string形式で操作される 
[System.Serializable]
[SerializeField]
[SCHiearchy("System/set Special Flags")]
public class scSetAssertSpecial : StateController
{
    [SerializeField]
    stParams<string> value;

    [SerializeField]
    int priority = 0;

    internal override void OnExecute(Entity entity)
    {
        //entity.attrs.ctrl = value.valueGet(loadParams, entity);
    }
}

//ゲームシステムに死を組み込む.
[System.Serializable]
[SerializeField]
[SCHiearchy("System/send system to death")]
public class scSendDeathMeesage : StateController
{
    [SerializeField]
    int priority = 0;

    internal override void OnExecute(Entity entity)
    {
        //entity.attrs.ctrl = value.valueGet(loadParams, entity);
        entity.attrs.isEraseReady = true;
    }
}

//NotHitBy : 特定攻撃に対しての無敵効果.
[System.Serializable]
[SerializeField]
[SCHiearchy("System/set Entity non-hit")]
public class scNotHitBy : StateController
{
    [SerializeField]
    stParams<string> States;

    [SerializeField]
    stParams<int> time;

    internal override void OnExecute(Entity entity)
    {
        //entity.attrs.ctrl = value.valueGet(loadParams, entity);
    }
}



//StateTypeの変更など
[System.Serializable]
[SerializeField]
[SCHiearchy("System/set stateType")]
public class scSetStatetype : StateController
{
    [SerializeField]
    stParams<char> value;

    [SerializeField]
    stParams<int> priority;

    internal override void OnExecute(Entity entity)
    {
        char val = value.valueGet(loadParams, entity);
        if (Enum.IsDefined(typeof(Entity._StateType), val))
        {
            entity.stateType = (Entity._StateType)val;
        }
    }
}

//PhysTypeの変更など
[System.Serializable]
[SerializeField]
[SCHiearchy("System/set PhysType")]
public class scSetStatePhystype : StateController
{
    [SerializeField]
    stParams<char> value;

    [SerializeField]
    stParams<int> priority;
    
    internal override void OnExecute(Entity entity)
    {
        char val = value.valueGet(loadParams, entity);
        if (Enum.IsDefined(typeof(Entity._PhysicsType), val))
        {
            entity.physicsType = (Entity._PhysicsType)val;
        }
    }
}

//MoveTypeの変更など
[System.Serializable]
[SerializeField]
[SCHiearchy("System/set state Movetype")]
public class scSetStateMovetype : StateController
{
    [SerializeField]
    stParams<char> value;

    [SerializeField]
    stParams<int> priority;
    
    internal override void OnExecute(Entity entity)
    {
        char val = value.valueGet(loadParams, entity);
        if (Enum.IsDefined(typeof(Entity._MoveType), val))
        {
            entity.moveType = (Entity._MoveType)val;
        }
    }
}


//ガード状態に設定するか?
//この設定はinit更新時、自動的にfalseとなる.
[System.Serializable]
[SerializeField]
[SCHiearchy("System/set Guarding flags")]
public class scSetMoveGuarding : StateController
{
    
    [SerializeField]
    stParams<bool> willGuardFlagSet = new stParams<bool>(true,true,true);

    internal override void OnExecute(Entity entity)
    {
        entity.attrs.isGuarded = willGuardFlagSet.valueGet(loadParams, entity);
    }
}


//set position for absolute value.
[System.Serializable]
[SerializeField]
[SCHiearchy("Physics/set Absolute position")]
public class scSetPos : StateController
{

    [SerializeField]
    stParams<Vector3> position;

    [SerializeField]
    stParams<int> priority;

    internal override void OnExecute(Entity entity)
    {
        entity.rigid.position = position.valueGet(loadParams, entity);
    }
}


//add position via value.
[System.Serializable]
[SerializeField]
[SCHiearchy("Physics/add Absolute position")]
public class scAddPos : StateController
{

    [SerializeField]
    stParams<Vector3> position;

    [SerializeField]
    stParams<int> priority;

    internal override void OnExecute(Entity entity)
    {
        entity.rigid.position += position.valueGet(loadParams, entity);
    }
}


[SCHiearchy("Physics/set Rotation")]
//set position for absolute value.
public class scSetRotate : StateController
{

    [SerializeField]
    stParams<Quaternion> rotation;

    [SerializeField]
    stParams<int> priority;

    internal override void OnExecute(Entity entity)
    {
        entity.rigid.rotation = rotation.valueGet(loadParams, entity);
    }
}

//add position via value.
[System.Serializable]
[SerializeField]
[SCHiearchy("Sound/Play OneshotSound from miscs")]
public class scPlayOneshotSound : StateController
{

    [SerializeField]
    AudioClip audio;

    [SerializeField]
    stParams<int> priority;

    internal override void OnExecute(Entity entity)
    {
        entity.SetPlayOneShot(audio);
    }
}


//set the entity collision. it resets after 1 frames.
[SCHiearchy("Physics/Entity Collision Ignore for 1 frame")]
public class scIgnoreEntityCollisions : StateController
{
    [SerializeField]
    stParams<int> priority;

    internal override void OnExecute(Entity entity)
    {
        entity.ignoreCollider();
    }
}

