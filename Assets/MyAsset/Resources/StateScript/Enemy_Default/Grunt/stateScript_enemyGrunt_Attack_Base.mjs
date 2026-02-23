
//puncher states for stateDef 200, enemy defaults.
export function StateDef_200_ID(entity)
{ 
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let SoundTime = entity.attrs.isSoundNotPlayed == 0;
    let CurrentTime = CS.Elem.CheckStateTime(entity);
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);

    if (CurrentTime > 16)
    {
        verdList.Add(3);
    }
    if (CurrentTime == 0)
    {
        CS.UnityEngine.Debug.Log("Executed Anim");
        verdList.Add(0);
    }
    
    //HitDef Generate.
    if (Math.abs(CurrentAnimTime - 9) < 2 &&
        entity.attrs.isStateHit == 0)
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