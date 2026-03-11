// any meant as any type.
// also use 'let' at first declearation,
// put the variable type name after colon,
// and put the init value lastly.

// We could use List name, if they call variable first.
export function StateDef_210_ID(entity) {
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let selfOnGrd = CS.Elem.isEntityOnGround(entity)

    //this must be set as 0.
    let selfStTime = CS.Elem.CheckStateTime(entity) 
    let SoundTime = entity.attrs.isSoundNotPlayed == 0;
    let CurrentTime = CS.Elem.CheckStateTime(entity);
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
    let AnimEndTime = CS.Elem.CheckAnimEndTime(entity);
    let CurrentAnimID = CS.Elem.CheckAnimID(entity);

    //Init.
    if (selfStTime == 0)
    { 
        verdList.Add(0) 
    }

    //Accels
    if( CurrentAnimTime > 7 && CurrentAnimTime < 8)
    {
        verdList.Add(2) 
    }
    //ChangeState on AnimEnd
    if( AnimEndTime - CurrentAnimTime < 4 && CurrentAnimID == 2)
    {
        verdList.Add(100) 
    }
    //CS.UnityEngine.Debug.Log("PuerTS MainState Debug Executed Correctly.");
    return verdList;
}

export function Accel_Start(entity)
{
    let List_Object =
        puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Object);
    let outs = new List_Object();

    const Vector2 = CS.UnityEngine.Vector2;
    const Vector3 = CS.UnityEngine.Vector3;
    let vel2 = new Vector2(0,0)
    let vel3 = new Vector3(0,0,0);

    //オブジェクトのRigidBodyを取得する.
    vel3 = entity.rigid.velocity    
    //オブジェクトの正面方向・右方向を考え、Dotで計算.
    let vel_relate_f = entity.transform.forward;
    let vel_relate_r = entity.transform.right;
    //Operator_Multiply on this..
    vel3 = Vector3.op_Multiply(Vector3.ProjectOnPlane(vel_relate_f,Vector3.up) , 300.0);

    outs.Add(vel3);
    //CS.UnityEngine.Debug.Log(vel2);
    return outs
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

