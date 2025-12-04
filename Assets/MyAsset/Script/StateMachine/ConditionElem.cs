using System;
using System.Collections;
using System.Collections.Generic;
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


    public static bool CheckButtonPressed(Entity et, string command)
    {
        return et.entityInput.CheckInput(command, 1);
    }

    public static int CheckStateDefID(Entity et)
    {
        return et.CurrentStateID;
    }

    public static int CheckAnimID(Entity et)
    {
        return et.animID;
    }


    //Checker for MainAnimDef's registered list.
    public static int CheckAnimationsListNum(Entity et)
    {
        return et.MainAnimMixer.Mixers.Length;
    }

    public static Transform getEntityBoneTransform(Entity et, string BoneName)
    {
        return et.getBoneTransform(BoneName);
    }
}
