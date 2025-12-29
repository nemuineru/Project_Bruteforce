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

//Air Knifer states for stateDef 51
export function StateDef_51_ID(entity)
{ 
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let SoundTime = entity.attrs.isSoundNotPlayed == 0;
    let CurrentTime = CS.Elem.CheckStateTime(entity);
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
    let AnimEndTime = CS.Elem.CheckAnimEndTime(entity);

    let selfOnGrd = CS.Elem.isEntityOnGround(in_entity)

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