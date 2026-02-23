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
    let isAlive = entity.attrs.alive;

    //AnimSet
    if (CurrentTime == 0 && CurrentAnimID != 5000)
    {
        verdList.Add(0);
    }

    //OnGround and time is after..
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
    let isAlive = entity.attrs.alive;

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

//FallDown(5100)
export function StateDef_5100_ID(entity)
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
    let isAlive = entity.attrs.alive;
    
    if (CurrentTime == 0)
    {
        verdList.Add(0);
    }    
    if (CurrentAnimID == 5100 && isOnGround &&
    AnimEndTime - CurrentAnimTime < 2 && isAlive)
    {
        verdList.Add(1);
    }    
    if(!isAlive){
        verdList.Add(2);
    }
    return verdList;
}

//FallDown_Recovery
export function StateDef_5101_ID(entity)
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
    let isAlive = entity.attrs.alive;
    
    if (CurrentTime == 0)
    {
        verdList.Add(0);
    }    
    if (CurrentAnimID == 5101 &&
    AnimEndTime - CurrentAnimTime < 2)
    {
        verdList.Add(1);
    }
    return verdList;
}