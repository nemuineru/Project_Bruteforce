
//puncher states for stateDef 200 and 201
export function StateDef_200_ID(entity)
{ 
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let CurrentAnimID = CS.Elem.CheckAnimID(entity);
    let CurrentTime = CS.Elem.CheckStateTime(entity);
    let SoundTime = entity.attrs.isSoundNotPlayed == 0 && (CurrentAnimID == 200 || CurrentAnimID == 201) && CurrentTime > 1;
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);

    if (CurrentTime > 12)
    {
        verdList.Add(1);
    }
    if (CurrentTime == 0)
    {
        //CS.UnityEngine.Debug.Log("Executed Anim");
        verdList.Add(0);
    }
    //Aight, does native JS supports math function?
    if (Math.abs(CurrentTime - 4) < 2 ) // && entity.attrs.isStateHit == 0
    {
        verdList.Add(10);
    }
    if(SoundTime){
        verdList.Add(100);
    }
    //CS.UnityEngine.Debug.Log("Executed");
    return verdList;
}

//Kick Finisher states for stateDef 202()
export function StateDef_202_ID(entity){ 
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let CurrentAnimID = CS.Elem.CheckAnimID(entity);
    let SoundTime = entity.attrs.isSoundNotPlayed == 0 && CurrentAnimID == 202;
    let CurrentTime = CS.Elem.CheckStateTime(entity);
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);

    if (CurrentTime > 14)
    {
        verdList.Add(1);
    }
    if (CurrentTime == 0)
    {
        verdList.Add(0);
    }
    //Aight, does native JS supports math function?
    //current Animtime needs to be set more than 8
    if (Math.abs(CurrentTime - 7) < 4 && CurrentAnimTime > 2 &&
        entity.attrs.isStateHit == 0)
    {
        verdList.Add(10);
    }
    if(SoundTime){
        verdList.Add(100);
    }
    return verdList;
}

//upper Finisher states for stateDef 210()
export function StateDef_210_ID(entity){ 
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let CurrentTime = CS.Elem.CheckStateTime(entity);
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
    let CurrentAnimID = CS.Elem.CheckAnimID(entity);
    let SoundTime = entity.attrs.isSoundNotPlayed == 0 && CurrentAnimTime > 7 && CurrentAnimID == 210;

    if (CurrentAnimTime > 20 && CurrentAnimID == 210)
    {
        verdList.Add(1);
    }
    if (CurrentTime == 0)
    {
        verdList.Add(0);
    }
    //Aight, does native JS supports math function?
    //current Animtime needs to be set more than 8
    if (CurrentTime > 3 && CurrentAnimTime > 9 && CurrentAnimTime < 13 && 
        entity.attrs.isStateHit == 0)
    {
        verdList.Add(10);
    }
    if(SoundTime){
        verdList.Add(100);
    }
    return verdList;
}

//RoundKicker Finisher states for stateDef 220()
export function StateDef_220_ID(entity){ 
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let CurrentTime = CS.Elem.CheckStateTime(entity);
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
    let CurrentAnimID = CS.Elem.CheckAnimID(entity);
    let SoundTime = entity.attrs.isSoundNotPlayed == 0 && CurrentAnimTime > 9 && CurrentAnimID == 220;

    if (CurrentAnimTime > 20 && CurrentAnimID == 220)
    {
        verdList.Add(1);
    }
    if (CurrentTime == 0)
    {
        verdList.Add(0);
    }
    //Aight, does native JS supports math function?
    //current Animtime needs to be set more than 8
    if ( CurrentAnimID == 220 && CurrentAnimTime > 9 && CurrentAnimTime < 14)
    {
        verdList.Add(10);
    }
    if(SoundTime){
        verdList.Add(100);
    }
    
    if ( CurrentAnimID == 220 && CurrentTime > 4 && CurrentTime < 12)
    {
        verdList.Add(11);
    }
    return verdList;
}


//Air Kicker states for stateDef 300()
export function StateDef_300_ID_JumpKick(entity){ 
//List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let CurrentTime = CS.Elem.CheckStateTime(entity);
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
    let CurrentAnimID = CS.Elem.CheckAnimID(entity);
    let selfOnGrd = CS.Elem.isEntityOnGround(entity);
    let SoundTime = entity.attrs.isSoundNotPlayed == 0 && CurrentAnimID == 300;

    //Change state on ground.
    if (selfOnGrd)
    {
        verdList.Add(1);
    }
    if (CurrentTime == 0)
    {
        verdList.Add(0);
    }
    // !CS.Elem.CheckExecutedID(entity,10)
    //Aight, does native JS supports math function?
    //current Animtime needs to be set more than 8
    if (CurrentAnimTime > 4 && CurrentAnimID == 300)
    {
        verdList.Add(10);
    }
    if(SoundTime){
        verdList.Add(100);
    }
    return verdList;
}