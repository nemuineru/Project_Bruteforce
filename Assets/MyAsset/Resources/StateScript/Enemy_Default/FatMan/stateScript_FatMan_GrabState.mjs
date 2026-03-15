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
    //hitDefs Register    
    if(CurrentAnimTime > 7 && CurrentAnimTime < 18 && CurrentAnimID == 2)
    {
        verdList.Add(1) 
    }
    //ChangeState on AnimEnd
    if( AnimEndTime - CurrentAnimTime < 4 && CurrentAnimID == 2)
    {
        verdList.Add(100) 
    }
    //CS.UnityEngine.Debug.Log("PuerTS MainState Debug Executed Correctly.");
    return verdList;
}

//params for accelation
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

//stateDef 211 (Grab Process State) 
export function StateDef_211_OwnerGrabProcess_ID(entity)
{
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

    //ChangeAnim
    if( CurrentTime == 0)
    {
        verdList.Add(0);
    }
    // change to idle. also damage them and check the rest frametime
    if(  CurrentAnimTime > 4 && CurrentAnimTime < 5 && selfOnGrd == true)
    {
        verdList.Add(1);
    }
    if(AnimEndTime - CurrentAnimTime < 1 && selfOnGrd == true && CurrentAnimID == 3)
    {
        verdList.Add(2);
    }

    return verdList;
}

//stateDef 212 (Grab Ending State) 
export function StateDef_212_OwnerGrabEnd_ID(entity)
{
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

    //ChangeAnim
    if( CurrentTime == 0)
    {
        verdList.Add(0);
    }
    // change to idle. also damage them and check the rest frametime
    if(AnimEndTime - CurrentAnimTime < 7 && CurrentAnimID == 4)
    {
        verdList.Add(1);
    }

    return verdList;
}


//stateDef 213 (Grab Process State) 
export function StateDef_213_TargetGrabProcess_ID(entity)
{
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

    //loads the owner entity.
    let ControlledEntity = entity.controlledEntity
    let Enemy_C_EntityStateID = CS.Elem.CheckStateDefID(ControlledEntity)

    //ChangeAnim
    if( CurrentTime == 0)
    {
        verdList.Add(0);
    }
    // change to idle. also damage them and check the rest frametime
    if(  Enemy_C_EntityStateID == 212 )
    {
        verdList.Add(1);
    }
    else
    {
        verdList.Add(2);
    }

    return verdList;
}

//stateDef 214 (Grab Ending State) 
export function StateDef_214_TargetGrabEnd_ID(entity)
{
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
    
    let ControlledEntity = entity.controlledEntity
    let Enemy_C_EntityStateID = CS.Elem.CheckStateDefID(ControlledEntity)
    let Enemy_C_animTime = CS.Elem.CheckAnimTime(ControlledEntity)
    let Enemy_AnimEndTime = CS.Elem.CheckAnimEndTime(ControlledEntity)

    //ChangeAnim
    if( CurrentTime == 0)
    {
        verdList.Add(0);
    }
    // change to idle. also damage them and check the rest frametime
    if( selfStTime > 15) 
    {
        verdList.Add(1);        
    }
    else 
    {
        verdList.Add(2);
    }
    return verdList;
}

export function ThrowHit_Owner_Choker(entity)
{
    //List<Object>
    let List_Object =
        puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Object);
    let outs = new List_Object();

    const Vector2 = CS.UnityEngine.Vector2;
    const Vector3 = CS.UnityEngine.Vector3;
    
    let trf = Vector3.ProjectOnPlane(entity.transform.forward, Vector3.up).normalized
    
    let retVec = new Vector3(0,0,0);
    retVec =  Vector3.op_Addition(Vector3.op_Multiply(trf,10.0), new Vector3(0,90,0))
    outs.Add(retVec)
    return outs
}

export function ThrowHit_Target_Track(entity)
{
    //List<Object>
    let List_Object =
        puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Object);
    let outs = new List_Object();

    const Vector2 = CS.UnityEngine.Vector2;
    const Vector3 = CS.UnityEngine.Vector3;

    let ControlledEntity = entity.controlledEntity
    
    let tr_Choked = CS.Elem.getEntityBoneTransform(entity,"spine").position
    let tr_Choker_L = CS.Elem.getEntityBoneTransform(ControlledEntity,"hand.l").position
    let tr_Choker_R = CS.Elem.getEntityBoneTransform(ControlledEntity,"hand.r").position
    
    let retVec = new Vector3(0,0,0);
    
    //Get Hand Middle point, and calc it.
    let tr_Choker_All = Vector3.op_Multiply(Vector3.op_Addition(tr_Choker_L,tr_Choker_R),0.5)
    //let DPos = new Vector3( tr_Choker_All.x - tr_Choked.x , tr_Choker_All.y -  tr_Choked.y , tr_Choker_All.z - tr_Choked.z )
    let diffPos =  Vector3.op_Subtraction(tr_Choker_All, tr_Choked) //CS.UnityEngine.Time.fixedDeltaTime

    //CS.UnityEngine.Debug.Log(diffPos);
    // CS.UnityEngine.Debug.DrawLine(tr_Choked, tr_Choker_All);
    // CS.UnityEngine.Debug.DrawLine(tr_Choked, Vector3.op_Addition(Vector3.up,tr_Choked));
    outs.Add(diffPos)
    let throwingVect_1 = Vector3.ProjectOnPlane(entity.transform.forward, Vector3.up).normalized
    let throwingVect_2 = Vector3.op_Multiply(Vector3.up, 0.2);

    outs.Add(Vector3.op_Multiply(Vector3.op_Addition(throwingVect_1,throwingVect_2), 10.0));
    
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

