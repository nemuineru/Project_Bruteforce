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
            JSEnv = new JsEnv();
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

    public void Reset()
    {
        JSEnv.Dispose();
        JSEnv = new JsEnv();
    }

    void OnDestroy()
    {
    }
}
