//Knifer states for stateDef 250
export function StateDef_250_ID_Knife(entity)
{ 
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let SoundTime = entity.attrs.isSoundNotPlayed == 0;
    let CurrentTime = CS.Elem.CheckStateTime(entity);
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
    let AnimEndTime = CS.Elem.CheckAnimEndTime(entity);

    if (CurrentTime > 20)
    {
        verdList.Add(1);
    }
    if (CurrentTime == 0)
    {
        verdList.Add(0);
    }
    //Aight, does native JS supports math function?
    if (Math.abs(CurrentTime - 5) < 2 &&
        entity.attrs.isStateHit == 0)
    {
        verdList.Add(10);
    }
    if(SoundTime){
        verdList.Add(100);
    }
    return verdList;
}

//Knifer states for stateDef 250
export function StateDef_251_ID_KnifeV2(entity)
{ 
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let CurrentTime = CS.Elem.CheckStateTime(entity);
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
    let AnimEndTime = CS.Elem.CheckAnimEndTime(entity);
    let SoundTime = entity.attrs.isSoundNotPlayed == 0 && CurrentAnimTime > 4 && entity.animID == 251

    if (AnimEndTime - CurrentAnimTime < 4)
    {
        verdList.Add(1);
    }
    if (CurrentTime == 0)
    {
        verdList.Add(0);
    }
    //Aight, does native JS supports math function?
    if (Math.abs(CurrentTime - 7) < 2 &&
        entity.attrs.isStateHit == 0)
    {
        verdList.Add(10);
    }
    if(SoundTime){
        verdList.Add(100);
    }
    return verdList;
}

export function StateDef_251_ID_Knife_physics(entity)
{

    let List_Object =
        puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Object);
    let outs = new List_Object();

    const Vector2 = CS.UnityEngine.Vector2;
    const Vector3 = CS.UnityEngine.Vector3;
    let vel2 = new Vector2(0, 0)
    let vel3 = new Vector3(0,0,0);

    //オブジェクトの正面方向・右方向を考え、Dotで計算.
    let vel_relate_f = entity.transform.forward;
    vel3.x = Vector3.ProjectOnPlane(vel_relate_f,Vector3.up).x * 120;
    vel3.z = Vector3.ProjectOnPlane(vel_relate_f,Vector3.up).z * 120;

    outs.Add(vel3);
    //CS.UnityEngine.Debug.Log(vel2);
    return outs
}

//Air Knifer states for stateDef 51
export function StateDef_350_AirKnife(entity)
{ 
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let SoundTime = entity.attrs.isSoundNotPlayed == 0;
    let CurrentTime = CS.Elem.CheckStateTime(entity);
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
    let AnimEndTime = CS.Elem.CheckAnimEndTime(entity);

    let selfOnGrd = CS.Elem.isEntityOnGround(entity)

    if (selfOnGrd)
    {
        verdList.Add(1);
    }
    if (CurrentTime == 0)
    {
        verdList.Add(0);
    }
    
    if (Math.abs(CurrentTime - 7) < 2 &&
        entity.attrs.isStateHit == 0)
    {
        verdList.Add(10);
    }
    if(SoundTime){
        verdList.Add(100);
    }
    return verdList;
}