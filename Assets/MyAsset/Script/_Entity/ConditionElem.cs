using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[System.Serializable]
public class lua_Read
{    
    public lua_Read(string insertScript)
    {
        LuaScript = insertScript;
    }
    public string LuaScript;
    
    public Elem lua_Condition;
    public Entity entity;

    //QueuedStateIDsは必ずint型のテーブルを返す値として創出.
    //また、Lua内でステートを読み込む前にStateDefParamsに値を事前登録させる.

    public interface CalcValues
    {
        public delegate int[] QueuedStateID(Entity entity);
        
        public delegate object[] luaOutParams(Entity entity);

        public class Parameter
        {
            public int IntParam;
            public float FloatParam;
            public bool BoolParam;
            public string StringParam;
            public Vector3 Vector3Param;
            public Vector2 Vector2Param;
        }
        
    }



    public void Execute()
    {
        
    }


    //返されたい変数によりGeneric変数として返される.
    //The Value Dependency will change via generics  
    /*
    public T Value<T>()
    {            
            Script script = new Script();

            UserData.RegisterAssembly();
            script.Globals["Conditions"] = new Lua_Conditions(entity);
            //Lua中のGlobal関数に設定..
            DynValue val = script.DoString(LuaScript);
            
            if (typeof(T) == typeof(bool))
            {          
                bool item = val.CastToBool();                
                Debug.Log(item);
                return (T)(dynamic)item;
            }
            else if(typeof(T) == typeof(string))
            {                
                return (T)val.CastToString() as String;
            }
            else
            {
                return (T)(dynamic)val.CastToNumber();
            }
    }
    */
}

//Lua_Conditionに登録, 値を設定.
//Entityごとにこの値が設定されると考える.
public class Elem
{
    public Elem()
    {

    }

    public static Vector3 entityPos(Entity et)
    {
        return et.transform.position;
    }

    public static Vector3 TargetPos(Entity et)
    {
        return et.targetTo_fw;
    }

    public static bool isEntityOnGround(Entity et)
    {
        return et.isOnGround;
    }

    public static float CheckStateTime(Entity et)
    {
        //Debug.Log(et.stateTime);
        return et.stateTime;
    }

    public static float CheckAnimTime(Entity et)
    {
        //Debug.Log(et.stateTime);
        return et.animationFrameTime;
    }

    public static float CheckAnimEndTime(Entity et)
    {
        //Debug.Log(et.stateTime);
        return et.animationEndTime;
    }


    public static bool CheckButtonPressed(Entity et, string commandName)
    {
        bool isCmdFound = false;
        entityInputManager.entityInput_Buffers find = et.cmdList.Find(ct => ct.CommandName == commandName);
        if (find != null)
        {
            isCmdFound = find.isCommandPressed();
        }
        return isCmdFound;
    }

    public static int CheckStateDefID(Entity et)
    {
        return et.CurrentStateID;
    }

    public static bool CheckExecutedID(Entity et, int refID)
    {
        return et.executedStateIDs.Any(i => i == refID);
    }

    public static int CheckAnimID(Entity et)
    {
        return et.animID;
    }


    //Checker for MainAnimDef's registered list.
    //it is now returns 0, because Im using the Animancer.
    public static int CheckAnimationsListNum(Entity et)
    {
        return 0;
        //return et.MainAnimMixer.Mixers.Length;
    }

    public static Transform getEntityBoneTransform(Entity et, string BoneName)
    {
        return et.getBoneTransform(BoneName);
    }

    //GetLatest hitParams. check for owners/get beaten by.
    public static hitDefParams getHitParam(Entity et, bool isOwner)
    {
        hitDefParams retParam = null;
        List<hitDefParams> findParamList;
        //一番最後の項目が最新の筈なので
        if(isOwner)
        {
            findParamList = et.registeredHitDefs.FindAll(par => par.targetEntity == et);
        }
        else
        {
            findParamList = et.registeredHitDefs.FindAll(par => par.ownerEntity == et);
        }
        if(findParamList != null)
        {
            retParam = findParamList.Last();
        }
        return retParam;
    }
    
    //Get selected hitParams. check for owners/get beaten by.
    public static hitDefParams getHitParam(Entity et, bool isOwner, int hitID)
    {

        return null;
    }

    public static bool isTargetRefsOwnerID(Entity et)
    {
        return et.controlledEntity != null;
    }
    
    //get the movement stick. Digital movement should be considered..
    //(0,0) -> (1,1) or (0,0) -> (-1,-1) should be counted but reversed wont. (1,-1) -> (0,0)
    //should be considered as "how is the distance from (0,0) differed between the keyNumber".
    //要は中央から離れるなら... 近づくのはだめ..
    public static int getStickSnapValue(Entity et, int keyNum, int countNum, float threadshold)
    {
        List<Vector2> inputList;
        inputList = et.entityInput.commandBuffer.Select(item => item.MoveAxis).ToList();
        int cnt = 0;
        for (int i = 0; i < inputList.Count - 1; i++)
        {
            //距離移動位置が前回よりも多いなら..
            if (Vector2.Distance(Vector3.zero, inputList[i + 1]) - Vector2.Distance(Vector3.zero, inputList[i]) > threadshold)
            {
                cnt++;
            }
        }
        return cnt;
    }

    public static Entity getTargetEntity(Entity et)
    {
        Entity tgt = null;
        if(et.mainTargetEntity != null)
        {
            tgt = et.mainTargetEntity;
        }
        else if(et.shortTargetEntity != null)
        {
            tgt = et.shortTargetEntity;
        }
        else if(et.nearestTargetEntity != null)
        {
            tgt = et.nearestTargetEntity;
        }
        return tgt;
    }

    public static float getTargetLength(Entity et)
    {
        float ret = Mathf.Infinity;
        Entity target = getTargetEntity(et);
        Vector3 diffs = target.transform.position - et.transform.position;
        ret = diffs.magnitude;
        return ret;
    }
    
    //水平方向のアングルを取得
    public static float getTargetHorizonAngle(Entity et)
    {
        float ret = 0f;
        Entity target = getTargetEntity(et);
        Vector3 diffs = target.transform.position - et.transform.position;
        ret = Vector3.SignedAngle(et.transform.forward,diffs,Vector3.up);
        return ret;
    }

    //垂直方向のアングルを取得.
    public static float getTargetVerticalAngle(Entity et)
    {
        float ret = 0f;
        Entity target = getTargetEntity(et);
        Vector3 diffs = target.transform.position - et.transform.position;
        ret = Vector3.SignedAngle(et.transform.forward,diffs,Vector3.right);
        return ret;
    }
}
