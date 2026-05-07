using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Puerts;

//lua enviroments loads as static.
//its because of the convenience.

public class PuerTS_Framework : MonoBehaviour
{
    public static PuerTS_Framework main;
    internal Puerts.JsEnv JSEnv;
    // Start is called before the first frame update
    void Awake()
    {
        if (main == null)
        {
            main = this;
            Reset();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Reset();
    }

    public JSObject ExecuteModule(string Dir)
    {
        JSEnv.Tick();
        JSObject ret = JSEnv.ExecuteModule(Dir);
        return ret;
    } 

    public stParams<TypeGet>.luaCalcParam paramRet<TypeGet>(string modPath, string Loads)
    {
        JSEnv.Tick();
        stParams<TypeGet>.luaCalcParam ret = JSEnv.ExecuteModule<stParams<TypeGet>.luaCalcParam>(modPath, Loads);
        return ret;
    }

    public List<TypeGet> GetValue<TypeGet>(Puerts.JSObject frameWork, string NameFunction, Entity entity)
    {
        List<TypeGet> ret = new List<TypeGet>();
        Func<Entity, List<TypeGet>> resFunc = frameWork.Get<Func<Entity, List<TypeGet>>>(NameFunction);
        if(resFunc != null)
        {
            ret = resFunc(entity) ?? new List<TypeGet>();
        }
        else
        {
            Debug.LogWarning(entity.gameObject.name + " loads the script function :" + NameFunction +
            "but returns 'not found'.");
        }
        return ret;
    }

    public void Reset()
    {
        if(JSEnv != null)
        {
            JSEnv.Dispose();
        }
        JSEnv = new JsEnv();
    }

    void OnDestroy()
    {
    }
}
