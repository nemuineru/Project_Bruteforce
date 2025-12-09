
//puncher states for stateDef 1 and 2
export function StateDef_1_ID(entity)
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
    return verdList;
}

//Kick Finisher states for stateDef 3()
export function StateDef_3_ID(entity){ 
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