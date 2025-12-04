// any meant as any type.
// also use 'let' at first declearation,
// put the variable type name after colon,
// and put the init value lastly.

// We could use List name, if they call variable first.
export function stateCmd(entity) {
    CS.UnityEngine.Debug.Log("executing PuerTS CMDs..");
    //List<Int>
    const List_Int =
        puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let selfOnGrd = CS.Elem.isEntityOnGround(entity);
    //let isPressed_A = CS.LuaCondition.CheckButtonPressed(entity, "_a");
    let AttackCmd_x = CS.Elem.CheckButtonPressed(entity, "_x");
    let StateDefID = entity.CurrentStateID;

    //this must be set as 0.
    let selfStTime = CS.Elem.CheckStateTime(entity) 

    if(selfOnGrd == true && AttackCmd_x == true && StateDefID == 0 )
    {
        verdList.Add(1);
    }
    return verdList;
}
