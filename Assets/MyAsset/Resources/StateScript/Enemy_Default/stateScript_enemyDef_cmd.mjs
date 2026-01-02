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
    let AttackCmd_y_isPressed = CS.Elem.CheckButtonPressed(entity, "Weapon");
    let StateDefID = entity.CurrentStateID;
    let isChainable = (StateDefID == 0 || (StateDefID >= 200 && StateDefID < 210));
    let chargeVal = entity.status.ChargeTime;
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);

    //this must be set as 0.
    let selfStTime = CS.Elem.CheckStateTime(entity) 

    //Ground Attacks - N
    if(selfOnGrd == true && AttackCmd_x == true && StateDefID == 0)
    {
        verdList.Add(200);
    }
    //Air Attacks - N
    if(selfOnGrd == false && AttackCmd_x == true && (StateDefID == 50 || (StateDefID == 300 && selfStTime > 15)))
    {
        verdList.Add(300);
    }

    //ここまで通常攻撃
    //Special Ground Attack _ Knife c1
    if(selfOnGrd == true && AttackCmd_y_isPressed && isChainable)
    {
        verdList.Add(250);
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
