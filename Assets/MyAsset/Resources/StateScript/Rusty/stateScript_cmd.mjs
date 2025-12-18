// any meant as any type.
// also use 'let' at first declearation,
// put the variable type name after colon,
// and put the init value lastly.

// We could use List name, if they call variable first.
export function stateCmd(entity) {
    //CS.UnityEngine.Debug.Log("executing PuerTS CMDs..");
    //List<Int>
    const List_Int =
        puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let selfOnGrd = CS.Elem.isEntityOnGround(entity);
    //let isPressed_A = CS.LuaCondition.CheckButtonPressed(entity, "_a");
    let AttackCmd_x = CS.Elem.CheckButtonPressed(entity, "Combo");
    //charger for basic 
    let AttackCmd_x_isPressed = CS.Elem.CheckButtonPressed(entity, "Combo_Keep");
    let AttackCmd_x_isReleased = CS.Elem.CheckButtonPressed(entity, "Combo_Release");
    let StateDefID = entity.CurrentStateID;
    let isChainable = (StateDefID == 0 || (StateDefID >= 200 && StateDefID < 210));
    let chargeVal = entity.status.ChargeTime;

    //this must be set as 0.
    let selfStTime = CS.Elem.CheckStateTime(entity) 

    if(selfOnGrd == true && AttackCmd_x == true && StateDefID == 0)
    {
        verdList.Add(200);
    }
    if(selfOnGrd == true && AttackCmd_x == true && StateDefID == 200  && selfStTime > 8)
    {
        verdList.Add(201);
    }
    if(selfOnGrd == true && AttackCmd_x == true && StateDefID == 201  && selfStTime > 11)
    {
        verdList.Add(202);
    }
    if(selfOnGrd == true && AttackCmd_x_isPressed && AttackCmd_x_isReleased && isChainable && chargeVal >= 0.40)
    {
        verdList.Add(210);
    }

    //the combo button could charge to the doubleskill - to full skill
    if(AttackCmd_x_isPressed && StateDefID < 5000)
    {
        verdList.Add(30)
    }
    //for at damaged. the charge is gone to none
    else if(!AttackCmd_x_isPressed || StateDefID >= 5000 && StateDefID <= 5300)
    {
        verdList.Add(31)
    }
    return verdList;
}
