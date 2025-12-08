//Hurt_Init(5000)
export function StateDef_5000_ID(entity)
{ 
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let SoundTime = entity.attrs.isSoundNotPlayed == 0;
    let CurrentTime = CS.Elem.CheckStateTime(entity);
    let CurrentAnimID = CS.Elem.CheckAnimID(entity);
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
    let AnimEndTime = CS.Elem.CheckAnimEndTime(entity);
    let isOnGround = CS.Elem.isEntityOnGround;
    let isAlive = entity.attrs.isAlive;

    if (CurrentTime == 0 && CurrentAnimID != 5000)
    {
        verdList.Add(0);
    }
    if (CurrentTime > 12 && isOnGround)
    {
        verdList.Add(1);
    }
    if (!isAlive &&
        CurrentTime > 6)
    {
        verdList.Add(2);
    }

    if(SoundTime){
        verdList.Add(100);
    }
    return verdList;
}

//Hurt_Blowout(5050)
export function StateDef_5050_ID(entity)
{ 
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let SoundTime = entity.attrs.isSoundNotPlayed == 0;
    let CurrentTime = CS.Elem.CheckStateTime(entity);
    let CurrentAnimID = CS.Elem.CheckAnimID(entity);
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
    let AnimEndTime = CS.Elem.CheckAnimEndTime(entity);
    let isOnGround = CS.Elem.isEntityOnGround;
    let isAlive = entity.attrs.isAlive;

    if (CurrentTime == 0 && CurrentAnimID != 5050)
    {
        verdList.Add(0);
    }
    if (CurrentTime > 6 && isOnGround)
    {
        verdList.Add(1);
    }    
    if(SoundTime){
        verdList.Add(100);
    }
    return verdList;
}

//Hurt_Fall(5050)
export function StateDef_5100(entity)
{

}