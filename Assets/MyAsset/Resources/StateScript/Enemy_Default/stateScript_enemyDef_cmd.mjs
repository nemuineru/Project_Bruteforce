// any meant as any type.
// also use 'let' at first declearation,
// put the variable type name after colon,
// and put the init value lastly.

//-2 var
export function setStatus(entity)
{
    const List_Int =
        puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let selfOnGrd = CS.Elem.isEntityOnGround(entity);
    //let isPressed_A = CS.LuaCondition.CheckButtonPressed(entity, "_a");
    let AttackCmd_x = CS.Elem.CheckButtonPressed(entity, "Combo");
    //charger for basic 
    let AttackCmd_x_isPressed = CS.Elem.CheckButtonPressed(entity, "Combo_Keep");
    let AttackCmd_x_isReleased = CS.Elem.CheckButtonPressed(entity, "Combo_Release");

    let AttackCmd_y_isPressed = CS.Elem.CheckButtonPressed(entity, "Weapon");

    //長押しで入力中とする.
    let GuardCmd_isPressed = CS.Elem.CheckButtonPressed(entity, "Guarding");

    let StateDefID = entity.CurrentStateID;
    let isChainable = (StateDefID == 0 || (StateDefID >= 200 && StateDefID < 210));
    let chargeVal = entity.status.ChargeTime;
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);

    const enumVal = CS.Entity._MoveType
    if(entity.status.HitTime < -60)
        entity.addBalancePoint(entity.status.balanceRecoveryRate);

    if(entity.status.HitTime && entity.moveType != enumVal.H)
        entity.setJugglePoint(entity.status.maxJugglePoint);

    //  if(entity.)
    
    //let selfStTime = CS.Elem.CheckStateTime(entity) 

    return verdList;
}

// We could use List name, if they call variable first.
export function stateCmd(entity) {
    //CS.UnityEngine.Debug.Log("executing PuerTS CMDs..");
    //List<Int>
    const List_Int =
        puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let selfOnGrd = CS.Elem.isEntityOnGround(entity);
    //let isPressed_A = CS.LuaCondition.CheckButtonPressed(entity, "_a");
    let AttackCmd_b = CS.Elem.CheckButtonPressed(entity, "Basic");
    let StateDefID = entity.CurrentStateID;
    let isChainable = (StateDefID == 0 || (StateDefID >= 200 && StateDefID < 210));
    let chargeVal = entity.status.ChargeTime;
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);

    //this must be set as 0.
    let selfStTime = CS.Elem.CheckStateTime(entity) 

    //Ground Attacks - N
    if(selfOnGrd == true && AttackCmd_b == true && StateDefID == 0)
    {
        verdList.Add(200);
    }

    let isNotice = entity.attrs.isNoticed;

    if(selfStTime % 20 == 19 && !isNotice)
    {  
        verdList.Add(10000);
    }
    return verdList;
}
