//Equipment_Pipe states for stateDef 1000
export function StateDef_1000_ID_Equipment_Pipe(entity)
{ 
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let SoundTime = entity.attrs.isSoundNotPlayed == 0;
    let CurrentTime = CS.Elem.CheckStateTime(entity);
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
    let AnimEndTime = CS.Elem.CheckAnimEndTime(entity);
    let AttackCmd_x = CS.Elem.CheckButtonPressed(entity, "Combo");

    //装備品が有る場合の設定 - 
    let isEquipmentHold = entity.equipmentInHand != null;
    if (isEquipmentHold)
    { 
            CS.UnityEngine.Debug.Log("Weapon holding");
    }

    if (CurrentTime > 20)
    {
        verdList.Add(1);
    }
    if (CurrentTime == 0)
    {
        verdList.Add(0);
    }
    //Aight, does native JS supports math function?
    if (Math.abs(CurrentAnimTime - 8) < 2 &&
        entity.attrs.isStateHit == 0)
    {
        verdList.Add(10);
    }
    //sets combo.
    if (entity.attrs.isStateHit > 0 && AttackCmd_x)
    {
        verdList.Add(1010);
    }
    if(SoundTime){
        verdList.Add(100);
    }
    return verdList;
}

//Equipment_Pipe states for stateDef 1010 (Combo chain) 
export function StateDef_1010_ID_Equipment_Pipe(entity)
{ 
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let SoundTime = entity.attrs.isSoundNotPlayed == 0;
    let CurrentTime = CS.Elem.CheckStateTime(entity);
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
    let AnimEndTime = CS.Elem.CheckAnimEndTime(entity);
    let AttackCmd_x = CS.Elem.CheckButtonPressed(entity, "Combo");

    //装備品が有る場合の設定 - 
    let isEquipmentHold = entity.equipmentInHand != null;
    if (isEquipmentHold)
    { 
        
    }

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

//equipment Base Throwing
export function StateDef_1020_ID_Equipment_Pipe(entity)
{ 
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let SoundTime = entity.attrs.isSoundNotPlayed == 0;
    let CurrentTime = CS.Elem.CheckStateTime(entity);
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
    let AnimEndTime = CS.Elem.CheckAnimEndTime(entity);
    let AttackCmd_x = CS.Elem.CheckButtonPressed(entity, "Combo");

    //装備品が有る場合の設定 - 
    let isEquipmentHold = entity.equipmentInHand != null;
    if (isEquipmentHold)
    { 
        
    }

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