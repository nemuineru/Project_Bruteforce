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
    let AttackCmd_b = CS.Elem.CheckButtonPressed(entity, "Basic");
    let StateDefID = entity.CurrentStateID;
    let isChainable = (StateDefID == 0 || (StateDefID >= 200 && StateDefID < 210));
    let chargeVal = entity.status.ChargeTime;
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);

    //this must be set as 0.
    let selfStTime = CS.Elem.CheckStateTime(entity) 

    //Ground Attacks - N
    // if(selfOnGrd == true && AttackCmd_b == true && StateDefID == 0)
    // {
    //     verdList.Add(200);
    // }
    return verdList;
}
