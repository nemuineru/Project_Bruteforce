

// any meant as any type.
// also use 'let' at first declearation,
// put the variable type name after colon,
// and put the init value lastly.

//
// 0 - 通常状態
// 5 - ガード状態
// 50 - 空中・ジャンプ状態
// 
//
// 200 - 400 キャラクター指定の通常技、必殺技とか
// 5000 - 基本やられ

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

    if(entity.status.HitTime < -60)
        entity.addBalancePoint(entity.status.balanceRecoveryRate);
    if(entity.status.HitTime < -120)
        entity.setJugglePoint(entity.status.maxJugglePoint);

    let isEquipmentHold = entity.equipmentInHand != null;
    if(isEquipmentHold)
    {
        //CS.UnityEngine.Debug.Log("equipment holding");
        entity.status.labels = entity.equipmentInHand.name;
        entity.status.subUIVals = entity.equipmentInHand.durability;
        entity.status.subUImeterVals = entity.equipmentInHand.durability / entity.equipmentInHand.maxDurability;
        entity.status.subUIicons = entity.equipmentInHand.GUIImage;
        entity.status.subUIColors = CS.UnityEngine.Color.cyan;
        entity.status.instructionLabels = "[X] - Use Weapon \n[Y] - Throw";
    }
    else
    {
        //パワーはmaxPowerValを超えない.
        let maxPowerVal = 2.0;
        entity.status.subUIColors = CS.UnityEngine.Color.white;
        if (CS.Elem.isCustomvalueExist(entity, "PowerAmmoRemain"))
        {
            CS.UnityEngine.Debug.Log("loading PowerAmmos");
            let surf = entity.getEntityFloatValue("PowerAmmoRemain");
            entity.status.subUIVals = surf;
            entity.status.subUImeterVals = surf / maxPowerVal; 
            entity.setEntityFloatValue("PowerAmmoRemain", surf > maxPowerVal ? maxPowerVal : surf + CS.UnityEngine.Time.fixedDeltaTime)
        }
        else
        {
            entity.setEntityFloatValue("PowerAmmoRemain", maxPowerVal);
            entity.status.subUImeterVals = 1.0;
        }
        entity.status.subUIicons = CS.gameState.DefaultEquipmentImage;
        entity.status.instructionLabels = "[X] - Combo Attack \n[Y] - Power Attack";
    }

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
    let AttackCmd_x = CS.Elem.CheckButtonPressed(entity, "Combo");
    //charger for basic 
    let AttackCmd_x_isPressed = CS.Elem.CheckButtonPressed(entity, "Combo_Keep");
    let AttackCmd_x_isReleased = CS.Elem.CheckButtonPressed(entity, "Combo_Release");

    let AttackCmd_y_isPressed = CS.Elem.CheckButtonPressed(entity, "Weapon");

    //長押しで入力中とする.
    let GuardCmd_isPressed = CS.Elem.CheckButtonPressed(entity, "Guarding");
    let BurstCmd_isPressed = CS.Elem.CheckButtonPressed(entity, "Burst");

    //カメラ設定コマンド
    let SetCameraCmd_isPressed = CS.Elem.CheckButtonPressed(entity, "Camera");

    let StateDefID = entity.CurrentStateID;
    let isChainable = (StateDefID == 0 || (StateDefID >= 200 && StateDefID < 210));
    let chargeVal = entity.status.ChargeTime;
    let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
    let isReversable = isChainable || (StateDefID == 5000 || StateDefID == 5050 || StateDefID == 5100 || StateDefID == 5110)

    //装備品が有る場合の設定 - 
    let isWeaponhold = entity.equipmentInHand != null;

    //this must be set as 0.
    let selfStTime = CS.Elem.CheckStateTime(entity) 

    //距離を考慮.
    let targetDist = CS.Elem.getTargetLength(entity)

    //ガード状態.
    if(selfOnGrd == true && GuardCmd_isPressed && StateDefID == 0 && entity.status.currentStancePoint > 0)
    {        
        verdList.Add(100);
    }

    
    //武器持ちなら1000に移動
    if(isWeaponhold)
    {
        //Ground Attacks - N
        if(selfOnGrd == true && AttackCmd_x == true && StateDefID == 0)
        {
            CS.UnityEngine.Debug.Log("Weapon holding");
            verdList.Add(1000);
        }
        //Throwing Weapons
        if(selfOnGrd == true && AttackCmd_y_isPressed && StateDefID == 0)
        {
            CS.UnityEngine.Debug.Log("Weapon throwing");
            verdList.Add(230);
        }

    }
    //そうでないなら通常攻撃
    else
    {
        //Ground Attacks - N
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
        if(selfOnGrd == true && AttackCmd_x_isPressed && AttackCmd_x_isReleased && 
            (isChainable) && chargeVal >= 0.15 && chargeVal < 0.4)
        {
            //CS.UnityEngine.Debug.Log("Charge Attack Test");
            verdList.Add(210);
        }
        if(selfOnGrd == true && AttackCmd_x_isPressed && AttackCmd_x_isReleased && 
            (isChainable || (StateDefID == 210 && CurrentAnimTime >= 15)) && chargeVal >= 0.4)
        {
            //CS.UnityEngine.Debug.Log("Charge Attack Test");
            verdList.Add(220);
        }
        //Air Attacks - N
        if(selfOnGrd == false && AttackCmd_x == true && ((StateDefID >= 50 && StateDefID <= 59) || (StateDefID == 300 && selfStTime > 15)))
        {
            verdList.Add(300);
        }        

        //ここまで通常攻撃

        //Special Ground Attack _ Knife c1
        if(selfOnGrd == true && AttackCmd_y_isPressed && isChainable && entity.status.currentEnergy > 5 && targetDist < 2.0)
        {
            verdList.Add(250);
        }
        //Special Ground Attack _ Knife c2
        if(selfOnGrd == true && AttackCmd_y_isPressed && (StateDefID == 250 && selfStTime > 12) && entity.status.currentEnergy > 10)
        {
            verdList.Add(251);
        }

        //Special Ground Far Attack - Gun
        if(selfOnGrd == true && AttackCmd_y_isPressed && isChainable && entity.status.currentEnergy > 5 && targetDist >= 2.0)
        {
            verdList.Add(500);
        }


        //Special Air Attack _ Knife c1
        if(selfOnGrd == false && AttackCmd_y_isPressed == true && (StateDefID >= 50 && StateDefID <= 55) && entity.status.currentEnergy > 10)
        {
            verdList.Add(350);
        }    
        
        //Reversal Attack
        if(BurstCmd_isPressed == true && isReversable && entity.status.currentEnergy >= 50)
        {
            verdList.Add(400);
        }

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

    //for camera set
    if(SetCameraCmd_isPressed)
    {
        verdList.Add(40)
    }

    

    return verdList;
}