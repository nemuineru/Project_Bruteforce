using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Puerts;

//lua enviroments loads as static.
//its because of the convenience.

public class Lua_OnLoad : MonoBehaviour
{
    public static Lua_OnLoad main;
    // Start is called before the first frame update
    void Awake()
    {
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
    }
}
