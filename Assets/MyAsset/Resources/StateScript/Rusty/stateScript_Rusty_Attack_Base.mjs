
//puncher states for stateDef 200 and 201
export function StateDef_200_ID(entity)
{ 
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let SoundTime = entity.attrs.isSoundNotPlayed == 0;
    let CurrentTime = CS.Elem.CheckStateTime(entity);
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);

    if (CurrentTime > 12)
    {
        verdList.Add(1);
    }
    if (CurrentTime == 0)
    {
        CS.UnityEngine.Debug.Log("Executed Anim");
        verdList.Add(0);
    }
    //Aight, does native JS supports math function?
    if (Math.abs(CurrentTime - 4) < 2 &&
        entity.attrs.isStateHit == 0)
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

    let SoundTime = entity.attrs.isSoundNotPlayed == 0;
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
    if (Math.abs(CurrentTime - 4) < 3 && CurrentAnimTime > 8 &&
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

    let SoundTime = entity.attrs.isSoundNotPlayed == 0;
    let CurrentTime = CS.Elem.CheckStateTime(entity);
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);

    if (CurrentAnimTime > 20)
    {
        verdList.Add(1);
    }
    if (CurrentTime == 0)
    {
        verdList.Add(0);
    }
    //Aight, does native JS supports math function?
    //current Animtime needs to be set more than 8
    if (Math.abs(CurrentTime - 4) < 3 && CurrentAnimTime > 8 &&
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

    let SoundTime = entity.attrs.isSoundNotPlayed == 0;
    let CurrentTime = CS.Elem.CheckStateTime(entity);
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);

    if (CurrentTime > 60 && CurrentAnimTime > 60)
    {
        verdList.Add(1);
    }
    if (CurrentTime == 0)
    {
        verdList.Add(0);
    }
    //Aight, does native JS supports math function?
    //current Animtime needs to be set more than 8
    if (Math.abs(CurrentTime - 4) < 3 && CurrentAnimTime > 8 &&
        entity.attrs.isStateHit == 0)
    {
        verdList.Add(10);
    }
    if(SoundTime){
        verdList.Add(100);
    }
    return verdList;
}