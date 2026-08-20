
//puncher states for stateDef 200, enemy defaults.
export function StateDef_200_ID(entity)
{ 
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let SoundTime = entity.attrs.isSoundNotPlayed == 0;
    let CurrentTime = CS.Elem.CheckStateTime(entity);
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
    let EndTime = CS.Elem.CheckAnimEndTime(entity);
    let isMoveExecuted = CS.Elem.CheckExecutedID(entity, 1);

    //end if
    if (CurrentTime > 1 && EndTime - CurrentAnimTime < 4)
    {
        verdList.Add(3);
    }
    if (CurrentTime == 0)
    {
        verdList.Add(0);
    }
    if(!isMoveExecuted && CurrentAnimTime - 10 > 0 && CurrentTime > 1)
    {
        verdList.Add(1);
    }
    
    //HitDef Generate.
    if (Math.abs(CurrentAnimTime - 13) < 3 &&
        entity.attrs.isStateHit == 0 && CurrentTime > 1)
    {
        //Sounddefs..
        if(SoundTime)
        {
            verdList.Add(100);
        }
        verdList.Add(10);
    }

    //CS.UnityEngine.Debug.Log("Executed");
    return verdList;
}


//puncher states for stateDef 200, enemy defaults.
export function StateDef_200_Hard_ID(entity)
{ 
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let SoundTime = entity.attrs.isSoundNotPlayed == 0;
    let CurrentTime = CS.Elem.CheckStateTime(entity);
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
    let EndTime = CS.Elem.CheckAnimEndTime(entity);
    let isMoveExecuted = CS.Elem.CheckExecutedID(entity, 1);

    //end if
    if (CurrentTime > 1 && EndTime - CurrentAnimTime < 4)
    {
        verdList.Add(3);
    }
    if (CurrentTime == 0)
    {
        verdList.Add(0);
    }
    if(!isMoveExecuted && CurrentAnimTime - 17 > 0 && CurrentTime > 1)
    {
        verdList.Add(1);
    }
    //end if
    if (CurrentTime > 1 && Math.abs(CurrentAnimTime - 16) < 2)
    {
        verdList.Add(5);
    }
    
    //HitDef Generate.
    if (Math.abs(CurrentAnimTime - 19) < 2 &&
        entity.attrs.isStateHit == 0 && CurrentTime > 1)
    {
        //Sounddefs..
        if(SoundTime)
        {
            verdList.Add(100);
        }
        verdList.Add(10);
    }

    //CS.UnityEngine.Debug.Log("Executed");
    return verdList;
}