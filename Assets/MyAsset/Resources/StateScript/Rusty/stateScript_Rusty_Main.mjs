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
    let isPressed_A = CS.Elem.CheckButtonPressed(entity, "Jump");
    let isPressed_B = CS.Elem.CheckButtonPressed(entity, "_b");
    let isPressed_C = CS.Elem.CheckButtonPressed(entity, "c");
    let selfOnGrd_f = CS.Elem.isEntityOnGround(entity);

    //this must be set as 0.
    let selfStTime = CS.Elem.CheckStateTime(entity) 
    let currentAnimID =  CS.Elem.CheckAnimID(entity);

    //Init.
    if (selfStTime > 0)
    { 
        verdList.Add(0) 
    }

    if (selfOnGrd) 
    { 
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
    
    //entityに登録されたmixerの数が0のときは緊急。
    if( CS.Elem.CheckStateTime(entity) == 0 && currentAnimID != 0)
    {
        //Debug.Log("Init Anim Loaded")
        verdList.Add(100) 
    }

    if(!entity.attrs.alive)
    {
        verdList.Add(5100)
    }
    //falling state.
    if(!selfOnGrd_f)
    {
        verdList.Add(55)
    }
    //CS.UnityEngine.Debug.Log("PuerTS MainState Debug Executed Correctly.");
    return verdList;
}

//function for Rolling.
//currently the Clss is gone for while.
export function StateDef_20_ID(entity)
{
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    let selfStTime =  CS.Elem.CheckStateTime(entity)
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
    let selfOnGrd_f = CS.Elem.isEntityOnGround(entity)

    //if the WishingVect exists, it rolls towards there.
    if(selfStTime > 0)
    {
        verdList.Add(0);
    }
}

//Function for jump.
export function StateDef_50_ID(entity) 
{
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
    let isPressed_A = CS.Elem.CheckButtonPressed(entity, "Jump");    
    
    let verdList = new List_Int();
    let selfStTime =  CS.Elem.CheckStateTime(entity)
    let selfOnGrd_f = CS.Elem.isEntityOnGround(entity)

    //On Ground.
    if(selfStTime > 18)
    {
        verdList.Add(1);
    }
    //Air Dash.
    else if(isPressed_A && selfStTime > 7)
    {
        verdList.Add(51);
    }

    //idleのanimを指定する
    if(selfStTime == 0)
    {
        //Debug.Log("Jumping Vect");
        verdList.Add(50);
    }
    return verdList
}

//Function for jump.
export function StateDef_51_ID(entity) 
{
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();
    let selfStTime =  CS.Elem.CheckStateTime(entity)
    let selfOnGrd_f = CS.Elem.isEntityOnGround(entity)
    let SoundTime = entity.attrs.isSoundNotPlayed == 0 && CurrentAnimID == 51;

    //On Ground.
    if(selfStTime > 7)
    {
        verdList.Add(55);
    }

    //idleのanimを指定する
    if(selfStTime == 0)
    {
        verdList.Add(0);
        CS.UnityEngine.Debug.Log(selfStTime);
        entity.status.currentEnergy -= 15;
    }
    //Dash Sound and Effect
    if(SoundTime != true)
    {
        verdList.Add(1);
    }
    return verdList
}

//Function for falling.
export function StateDef_55_ID(entity) 
{
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();
    let selfStTime =  CS.Elem.CheckStateTime(entity)
    let selfOnGrd_f = CS.Elem.isEntityOnGround(entity)
    let isPressed_A = CS.Elem.CheckButtonPressed(entity, "Jump");    

    //idleのanimを指定する
    if(selfStTime == 0)
    {
        //Debug.Log("Jumping Vect");
        verdList.Add(0);
    }
    //On Ground.
    if(selfStTime > 1 && selfOnGrd_f == true)
    {
        //CS.UnityEngine.Debug.Log("Jumping Vect");        
        verdList.Add(1);
    }
    //Air Dash.
    else if(isPressed_A && selfStTime > 7)
    {
        verdList.Add(51);
    }
    return verdList
}

//Function for landing.
export function StateDef_60_ID(entity) 
{
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList_F = new List_Int();
    let selfStTime =  CS.Elem.CheckStateTime(entity)
    let selfOnGrd_f = CS.Elem.isEntityOnGround(entity)

    //On Ground.
    if(selfStTime > 1 && selfOnGrd_f == true)
    {
        verdList_F.Add(1);
    }

    //idleのanimを指定する
    if(selfStTime == 0)
    {
        //Debug.Log("Jumping Vect");
        verdList_F.Add(50);
    }
    return verdList_F
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


//function for Guarding
export function StateDef_100_ID(entity) {
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let selfOnGrd = CS.Elem.isEntityOnGround(entity)
    let isPressable = CS.Elem.CheckButtonPressed(entity, "Guarding") && !CS.Elem.isTargetRefsOwnerID(entity);

    //this must be set as 0.
    let selfStTime = CS.Elem.CheckStateTime(entity) 

    //Init.
    if (selfStTime == 0)
    { 
        verdList.Add(0) 
    }

    //if you release B or non ground..
    if (!isPressable) 
    { 
        //change to Idle.
        verdList.Add(1);
    }
    if(entity.attrs.isBeingStateGuarded > 0)
    {
        verdList.Add(105)
    }

    if(entity.status.currentGuardPoint <= 0)
    {
        verdList.Add(110);
    }

    //Guarding State is continued.
    verdList.Add(10);

    //ending Anim is always on.
    verdList.Add(3);
    
    return verdList;
}

//function for Guarding_GettingHurt - and also set non damage
export function StateDef_105_ID(entity){    
    
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();
    let isPressed_B = CS.Elem.CheckButtonPressed(entity, "Guarding") && !CS.Elem.isTargetRefsOwnerID(entity);

    let selfOnGrd = CS.Elem.isEntityOnGround(entity)
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);

    //this must be set as 0.
    let selfStTime = CS.Elem.CheckStateTime(entity) 

    //Init.
    if (selfStTime == 0)
    { 
        verdList.Add(0); 
    }
    //after taking hits.
    //if you release B or non ground.. change to init.
    if((CurrentAnimTime >= 10 && selfStTime > 4 && entity.status.HitTime < 0) || entity.status.currentGuardPoint <= 0)
    {
        if (!isPressed_B) 
        { 
            //change to Idle.
            verdList.Add(1);
        }
        //if not, continue and changestate to guarding.
        else
        {
            verdList.Add(2);
        }
    }
    verdList.Add(3);
    verdList.Add(10);    
    return verdList;
}


//function for Guarding_Stun : Guarding is exceeded..
export function StateDef_110_ID(entity){   
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();
    let isPressed_B = CS.Elem.CheckButtonPressed(entity, "Guarding") && !CS.Elem.isTargetRefsOwnerID(entity);

    let selfOnGrd = CS.Elem.isEntityOnGround(entity)
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);

    //this must be set as 0.
    let selfStTime = CS.Elem.CheckStateTime(entity) 

    //Init.
    if (selfStTime == 0)
    { 
        verdList.Add(0); 
    }

    //after taking hits.
    //if you release B or non ground.. change to init.
    if(CurrentAnimTime >= 20 && selfStTime > 4 && entity.status.HitTime < 0)
    {
        verdList.Add(1);
    } 
    return verdList;

}
