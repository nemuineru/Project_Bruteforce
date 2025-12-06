// any meant as any type.
// also use 'let' at first declearation,
// put the variable type name after colon,
// and put the init value lastly.

// We could use List name, if they call variable first.
export function StateDef_0_ID(entity) {
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let selfOnGrd = CS.Elem.isEntityOnGround(entity)
    let isPressed_A = CS.Elem.CheckButtonPressed(entity, "_a");
    let isPressed_B = CS.Elem.CheckButtonPressed(entity, "_b");
    let isPressed_C = CS.Elem.CheckButtonPressed(entity, "c");

    //this must be set as 0.
    let selfStTime = CS.Elem.CheckStateTime(entity) 

    //Init.
    if (selfStTime > 0)
    { 
        verdList.Add(0) 
    }

    if (selfOnGrd) { 
        //Idle/Moving
        verdList.Add(2);
        //ChangeState To Jump. (stateNo - 3)
        if (isPressed_A)
        { 
            verdList.Add(3);
        }
    }

    if( !isPressed_C)
    {
            verdList.Add(800);
    }
    CS.UnityEngine.Debug.Log("PuerTS MainState Debug Executed Correctly.");
    return verdList;
}

// export Parameter @ stateDef 0.
// not only this function, but those params need to be 
// returned as GenericList such like List<object>,
// ..otherwise it fails completely. - N.
export function StateDef_0_Param(entity) { 
    let List_Object =
        puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Object);
    let outs = new List_Object();

    const Vector2 = CS.UnityEngine.Vector2;
    const Vector3 = CS.UnityEngine.Vector3;
    let vel2 = new Vector2(0, 0)
    let vel3 = new Vector3(0,0,0);

    //オブジェクトのRigidBodyを取得する.
    vel3 = entity.rigid.velocity    
    //オブジェクトの正面方向・右方向を考え、Dotで計算.
    let vel_relate_f = entity.transform.forward;
    let vel_relate_r = entity.transform.right;
    vel2.x = Vector3.Dot(vel3,vel_relate_r)
    vel2.y = Vector3.Dot(vel3,vel_relate_f)

    outs.Add(vel2);
    //CS.UnityEngine.Debug.Log(vel2);
    return outs
}

