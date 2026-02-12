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
        
    }

    void OnDestroy()
    {
    }
}
