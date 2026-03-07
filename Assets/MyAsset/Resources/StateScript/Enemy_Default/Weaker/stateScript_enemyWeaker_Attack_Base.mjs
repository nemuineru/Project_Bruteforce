
//weaker(thrower) states for stateDef 200, enemy defaults.
//瓶投げ攻撃.
export function StateDef_200_ID(entity)
{ 
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let SoundTime = entity.attrs.isSoundNotPlayed == 0;
    let CurrentTime = CS.Elem.CheckStateTime(entity);
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
    let AnimEndTime = CS.Elem.CheckAnimEndTime(entity)

    //ステート変更.
    if (AnimEndTime - CurrentAnimTime < 8 && CurrentTime > 10)
    {
        verdList.Add(2);
    }
    if (CurrentTime == 0)
    {
        verdList.Add(0);
    }
    
    //HitDef Generate.
    if (CurrentTime == 48)
    {
        //Sounddefs..
        if(SoundTime)
        {
            verdList.Add(100);
        }
        verdList.Add(1);
        CS.UnityEngine.Debug.Log("Throwing Bottles.");
    }

    //CS.UnityEngine.Debug.Log("Executed");
    return verdList;
}

//Aight, time to pack it UnityEngine's Vector3 operator : 
//op_Addition as -,
//op_Subtraction as -,
//op_Multiply as *,
//op_Division as / ..


//functions for throwing positions.
export function ThrowPos(entity)
{
    let List_Object =
        puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Object);
    let outs = new List_Object();
    
    const Vector2 = CS.UnityEngine.Vector2;
    const Vector3 = CS.UnityEngine.Vector3;

    let addVec = new Vector3(0,0,0);
    let thrower_fw = entity.transform.forward

    //throwing velocity for sect 0
    addVec = Vector3.op_Addition(Vector3.op_Multiply(Vector3.up,4.0) , Vector3.op_Multiply(thrower_fw,4.0));

    outs.Add(addVec);

    //hand pos for section 1.
    let thrower_hand = CS.Elem.getEntityBoneTransform(entity,"hand.r")
    addVec = thrower_hand.position;
    outs.Add(addVec);

    return outs;
}