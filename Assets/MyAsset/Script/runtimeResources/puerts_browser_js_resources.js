
        window.PUERTS_JS_RESOURCES = {"StateScript/Enemy_Default/FatMan/stateScript_FatMan_cmd.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.stateCmd = stateCmd;
// any meant as any type.
// also use 'let' at first declearation,
// put the variable type name after colon,
// and put the init value lastly.

// We could use List name, if they call variable first.
function stateCmd(entity) {
  //CS.UnityEngine.Debug.Log("executing PuerTS CMDs..");
  //List<Int>
  const List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let selfOnGrd = CS.Elem.isEntityOnGround(entity);
  //let isPressed_A = CS.LuaCondition.CheckButtonPressed(entity, "_a");
  let AttackCmd_b = CS.Elem.CheckButtonPressed(entity, "Basic");
  let StateDefID = entity.CurrentStateID;
  let isChainable = StateDefID == 0 || StateDefID >= 200 && StateDefID < 210;
  let chargeVal = entity.status.ChargeTime;
  let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);

  //this must be set as 0.
  let selfStTime = CS.Elem.CheckStateTime(entity);

  //Ground Grabbing strikes.
  if (selfOnGrd == true && AttackCmd_b == true && StateDefID == 0) {
    verdList.Add(210);
  }
  return verdList;
}
        }),"StateScript/Enemy_Default/FatMan/stateScript_FatMan_GrabState.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.Accel_Start = Accel_Start;
exports.StateDef_0_Param = StateDef_0_Param;
exports.StateDef_210_ID = StateDef_210_ID;
exports.StateDef_211_OwnerGrabProcess_ID = StateDef_211_OwnerGrabProcess_ID;
exports.StateDef_212_OwnerGrabEnd_ID = StateDef_212_OwnerGrabEnd_ID;
exports.StateDef_213_TargetGrabProcess_ID = StateDef_213_TargetGrabProcess_ID;
exports.StateDef_214_TargetGrabEnd_ID = StateDef_214_TargetGrabEnd_ID;
exports.ThrowHit_Owner_Choker = ThrowHit_Owner_Choker;
exports.ThrowHit_Target_Track = ThrowHit_Target_Track;
// any meant as any type.
// also use 'let' at first declearation,
// put the variable type name after colon,
// and put the init value lastly.

// We could use List name, if they call variable first.
function StateDef_210_ID(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let selfOnGrd = CS.Elem.isEntityOnGround(entity);

  //this must be set as 0.
  let selfStTime = CS.Elem.CheckStateTime(entity);
  let SoundTime = entity.attrs.isSoundNotPlayed == 0;
  let CurrentTime = CS.Elem.CheckStateTime(entity);
  let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
  let AnimEndTime = CS.Elem.CheckAnimEndTime(entity);
  let CurrentAnimID = CS.Elem.CheckAnimID(entity);

  //Init.
  if (selfStTime == 0) {
    verdList.Add(0);
  }

  //Accels
  if (CurrentAnimTime > 7 && CurrentAnimTime < 8) {
    verdList.Add(2);
  }
  //hitDefs Register    
  if (CurrentAnimTime > 7 && CurrentAnimTime < 18 && CurrentAnimID == 2) {
    verdList.Add(1);
  }
  //ChangeState on AnimEnd
  if (AnimEndTime - CurrentAnimTime < 4 && CurrentAnimID == 2) {
    verdList.Add(100);
  }
  //CS.UnityEngine.Debug.Log("PuerTS MainState Debug Executed Correctly.");
  return verdList;
}

//params for accelation
function Accel_Start(entity) {
  let List_Object = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Object);
  let outs = new List_Object();
  const Vector2 = CS.UnityEngine.Vector2;
  const Vector3 = CS.UnityEngine.Vector3;
  let vel2 = new Vector2(0, 0);
  let vel3 = new Vector3(0, 0, 0);

  //オブジェクトのRigidBodyを取得する.
  vel3 = entity.rigid.velocity;
  //オブジェクトの正面方向・右方向を考え、Dotで計算.
  let vel_relate_f = entity.transform.forward;
  let vel_relate_r = entity.transform.right;
  //Operator_Multiply on this..
  vel3 = Vector3.op_Multiply(Vector3.ProjectOnPlane(vel_relate_f, Vector3.up), 300.0);
  outs.Add(vel3);
  //CS.UnityEngine.Debug.Log(vel2);
  return outs;
}

//stateDef 211 (Grab Process State) 
function StateDef_211_OwnerGrabProcess_ID(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let selfOnGrd = CS.Elem.isEntityOnGround(entity);

  //this must be set as 0.
  let selfStTime = CS.Elem.CheckStateTime(entity);
  let SoundTime = entity.attrs.isSoundNotPlayed == 0;
  let CurrentTime = CS.Elem.CheckStateTime(entity);
  let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
  let AnimEndTime = CS.Elem.CheckAnimEndTime(entity);
  let CurrentAnimID = CS.Elem.CheckAnimID(entity);

  //ChangeAnim
  if (CurrentTime == 0) {
    verdList.Add(0);
  }
  // change to idle. also damage them and check the rest frametime
  if (CurrentAnimTime > 4 && CurrentAnimTime < 5 && selfOnGrd == true) {
    verdList.Add(1);
  }
  if (AnimEndTime - CurrentAnimTime < 1 && selfOnGrd == true && CurrentAnimID == 3) {
    verdList.Add(2);
  }
  return verdList;
}

//stateDef 212 (Grab Ending State) 
function StateDef_212_OwnerGrabEnd_ID(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let selfOnGrd = CS.Elem.isEntityOnGround(entity);

  //this must be set as 0.
  let selfStTime = CS.Elem.CheckStateTime(entity);
  let SoundTime = entity.attrs.isSoundNotPlayed == 0;
  let CurrentTime = CS.Elem.CheckStateTime(entity);
  let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
  let AnimEndTime = CS.Elem.CheckAnimEndTime(entity);
  let CurrentAnimID = CS.Elem.CheckAnimID(entity);

  //ChangeAnim
  if (CurrentTime == 0) {
    verdList.Add(0);
  }
  // change to idle. also damage them and check the rest frametime
  if (AnimEndTime - CurrentAnimTime < 7 && CurrentAnimID == 4) {
    verdList.Add(1);
  }
  return verdList;
}

//stateDef 213 (Grab Process State) 
function StateDef_213_TargetGrabProcess_ID(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let selfOnGrd = CS.Elem.isEntityOnGround(entity);

  //this must be set as 0.
  let selfStTime = CS.Elem.CheckStateTime(entity);
  let SoundTime = entity.attrs.isSoundNotPlayed == 0;
  let CurrentTime = CS.Elem.CheckStateTime(entity);
  let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
  let AnimEndTime = CS.Elem.CheckAnimEndTime(entity);
  let CurrentAnimID = CS.Elem.CheckAnimID(entity);

  //loads the owner entity.
  let ControlledEntity = entity.controlledEntity;
  let Enemy_C_EntityStateID = CS.Elem.CheckStateDefID(ControlledEntity);

  //ChangeAnim
  if (CurrentTime == 0) {
    verdList.Add(0);
  }
  // change to idle. also damage them and check the rest frametime
  if (Enemy_C_EntityStateID == 212) {
    verdList.Add(1);
  } else {
    verdList.Add(2);
  }
  return verdList;
}

//stateDef 214 (Grab Ending State) 
function StateDef_214_TargetGrabEnd_ID(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let selfOnGrd = CS.Elem.isEntityOnGround(entity);

  //this must be set as 0.
  let selfStTime = CS.Elem.CheckStateTime(entity);
  let SoundTime = entity.attrs.isSoundNotPlayed == 0;
  let CurrentTime = CS.Elem.CheckStateTime(entity);
  let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
  let AnimEndTime = CS.Elem.CheckAnimEndTime(entity);
  let CurrentAnimID = CS.Elem.CheckAnimID(entity);
  let ControlledEntity = entity.controlledEntity;
  let Enemy_C_EntityStateID = CS.Elem.CheckStateDefID(ControlledEntity);
  let Enemy_C_animTime = CS.Elem.CheckAnimTime(ControlledEntity);
  let Enemy_AnimEndTime = CS.Elem.CheckAnimEndTime(ControlledEntity);

  //ChangeAnim
  if (CurrentTime == 0) {
    verdList.Add(0);
  }
  // change to idle. also damage them and check the rest frametime
  if (selfStTime > 15) {
    verdList.Add(1);
  } else {
    verdList.Add(2);
  }
  return verdList;
}
function ThrowHit_Owner_Choker(entity) {
  //List<Object>
  let List_Object = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Object);
  let outs = new List_Object();
  const Vector2 = CS.UnityEngine.Vector2;
  const Vector3 = CS.UnityEngine.Vector3;
  let trf = Vector3.ProjectOnPlane(entity.transform.forward, Vector3.up).normalized;
  let retVec = new Vector3(0, 0, 0);
  retVec = Vector3.op_Addition(Vector3.op_Multiply(trf, 10.0), new Vector3(0, 90, 0));
  outs.Add(retVec);
  return outs;
}
function ThrowHit_Target_Track(entity) {
  //List<Object>
  let List_Object = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Object);
  let outs = new List_Object();
  const Vector2 = CS.UnityEngine.Vector2;
  const Vector3 = CS.UnityEngine.Vector3;
  let ControlledEntity = entity.controlledEntity;
  let tr_Choked = CS.Elem.getEntityBoneTransform(entity, "spine").position;
  let tr_Choker_L = CS.Elem.getEntityBoneTransform(ControlledEntity, "hand.l").position;
  let tr_Choker_R = CS.Elem.getEntityBoneTransform(ControlledEntity, "hand.r").position;
  let retVec = new Vector3(0, 0, 0);

  //Get Hand Middle point, and calc it.
  let tr_Choker_All = Vector3.op_Multiply(Vector3.op_Addition(tr_Choker_L, tr_Choker_R), 0.5);
  //let DPos = new Vector3( tr_Choker_All.x - tr_Choked.x , tr_Choker_All.y -  tr_Choked.y , tr_Choker_All.z - tr_Choked.z )
  let diffPos = Vector3.op_Subtraction(tr_Choker_All, tr_Choked); //CS.UnityEngine.Time.fixedDeltaTime

  //CS.UnityEngine.Debug.Log(diffPos);
  // CS.UnityEngine.Debug.DrawLine(tr_Choked, tr_Choker_All);
  // CS.UnityEngine.Debug.DrawLine(tr_Choked, Vector3.op_Addition(Vector3.up,tr_Choked));
  outs.Add(diffPos);
  let throwingVect_1 = Vector3.ProjectOnPlane(entity.transform.forward, Vector3.up).normalized;
  let throwingVect_2 = Vector3.op_Multiply(Vector3.up, 0.2);
  outs.Add(Vector3.op_Multiply(Vector3.op_Addition(throwingVect_1, throwingVect_2), 10.0));
  return outs;
}

// export Parameter @ stateDef 0.
// not only this function, but those params need to be 
// returned as GenericList such like List<object>,
// ..otherwise it fails completely. - N.
function StateDef_0_Param(entity) {
  let List_Object = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Object);
  let outs = new List_Object();
  const Vector2 = CS.UnityEngine.Vector2;
  const Vector3 = CS.UnityEngine.Vector3;
  let vel2 = new Vector2(0, 0);
  let vel3 = new Vector3(0, 0, 0);

  //オブジェクトのRigidBodyを取得する.
  vel3 = entity.rigid.velocity;
  //オブジェクトの正面方向・右方向を考え、Dotで計算.
  let vel_relate_f = entity.transform.forward;
  let vel_relate_r = entity.transform.right;
  vel2.x = Vector3.Dot(vel3, vel_relate_r);
  vel2.y = Vector3.Dot(vel3, vel_relate_f);
  outs.Add(vel2);
  //CS.UnityEngine.Debug.Log(vel2);
  return outs;
}
        }),"StateScript/Enemy_Default/FatMan/stateScript_FatMan_Hurt.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.StateDef_5000_ID = StateDef_5000_ID;
exports.StateDef_5050_ID = StateDef_5050_ID;
exports.StateDef_5100_ID = StateDef_5100_ID;
exports.StateDef_5101_ID = StateDef_5101_ID;
//Hurt_Init for Default Enemy.(5000)
function StateDef_5000_ID(entity) {
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
  if (CurrentTime == 0 && CurrentAnimID != 5000) {
    verdList.Add(0);
  }
  if (CurrentTime > 12 && isOnGround) {
    verdList.Add(1);
  }
  if (!isAlive && CurrentTime > 6) {
    verdList.Add(2);
  }
  if (SoundTime) {
    verdList.Add(100);
  }
  return verdList;
}

//Hurt_Blowout(5050)
function StateDef_5050_ID(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let SoundTime = entity.attrs.isSoundNotPlayed == 0;
  let CurrentTime = CS.Elem.CheckStateTime(entity);
  let CurrentAnimID = CS.Elem.CheckAnimID(entity);
  let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
  let AnimEndTime = CS.Elem.CheckAnimEndTime(entity);
  let isOnGround = CS.Elem.isEntityOnGround(entity);
  let isAlive = entity.attrs.alive;
  if (CurrentTime == 0 && CurrentAnimID != 5050) {
    verdList.Add(0);
  }
  if (CurrentAnimTime > 6 && CurrentTime > 10 && isOnGround) {
    verdList.Add(1);
  }
  if (SoundTime) {
    verdList.Add(100);
  }
  return verdList;
}

//FallDown(5100)
function StateDef_5100_ID(entity) {
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
  if (CurrentTime == 0) {
    verdList.Add(0);
  }
  if (CurrentAnimID == 5100 && isOnGround && AnimEndTime - CurrentAnimTime < 2 && isAlive) {
    verdList.Add(1);
  }
  if (!isAlive) {
    verdList.Add(2);
  }
  return verdList;
}

//FallDown_Recovery
function StateDef_5101_ID(entity) {
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
  if (CurrentTime == 0) {
    verdList.Add(0);
  }
  if (CurrentAnimID == 5101 && AnimEndTime - CurrentAnimTime < 2) {
    verdList.Add(1);
  }
  return verdList;
}
        }),"StateScript/Enemy_Default/FatMan/stateScript_FatMan_main.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.StateDef_0_ID = StateDef_0_ID;
exports.StateDef_0_Param = StateDef_0_Param;
exports.StateDef_50_ID = StateDef_50_ID;
// any meant as any type.
// also use 'let' at first declearation,
// put the variable type name after colon,
// and put the init value lastly.

// We could use List name, if they call variable first.
function StateDef_0_ID(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let selfOnGrd = CS.Elem.isEntityOnGround(entity);
  let isPressed_A = CS.Elem.CheckButtonPressed(entity, "a_");
  let isPressed_B = CS.Elem.CheckButtonPressed(entity, "b_");
  let isPressed_C = CS.Elem.CheckButtonPressed(entity, "c");

  //this must be set as 0.
  let selfStTime = CS.Elem.CheckStateTime(entity);

  //Init.
  if (selfStTime > 0) {
    verdList.Add(0);
  }
  if (selfOnGrd) {
    //Idle/Moving
    verdList.Add(2);
    //ChangeState To Jump. (stateNo - 3)
    if (isPressed_A) {
      verdList.Add(3);
    }
  }
  if (!isPressed_C) {
    verdList.Add(800);
  }
  if (CS.Elem.CheckStateTime(entity) == 1) {
    //Debug.Log("Init Anim Loaded")
    verdList.Add(100);
  }
  if (!entity.attrs.alive) {
    verdList.Add(5100);
  }
  //CS.UnityEngine.Debug.Log("PuerTS MainState Debug Executed Correctly.");
  return verdList;
}

//Function for jump.
function StateDef_50_ID(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let selfStTime = CS.Elem.CheckStateTime(entity);
  let selfOnGrd_f = CS.Elem.isEntityOnGround(entity);

  //On Ground.
  if (selfStTime > 1 && selfOnGrd_f == true) {
    verdList.Add(1);
  }

  //idleのanimを指定する
  if (selfStTime == 0) {
    //Debug.Log("Jumping Vect");
    verdList.Add(50);
  }
  return verdList;
}

// export Parameter @ stateDef 0.
// not only this function, but those params need to be 
// returned as GenericList such like List<object>,
// ..otherwise it fails completely. - N.
function StateDef_0_Param(entity) {
  let List_Object = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Object);
  let outs = new List_Object();
  const Vector2 = CS.UnityEngine.Vector2;
  const Vector3 = CS.UnityEngine.Vector3;
  let vel2 = new Vector2(0, 0);
  let vel3 = new Vector3(0, 0, 0);

  //オブジェクトのRigidBodyを取得する.
  vel3 = entity.rigid.velocity;
  //オブジェクトの正面方向・右方向を考え、Dotで計算.
  let vel_relate_f = entity.transform.forward;
  let vel_relate_r = entity.transform.right;
  vel2.x = Vector3.Dot(vel3, vel_relate_r);
  vel2.y = Vector3.Dot(vel3, vel_relate_f);
  outs.Add(vel2);
  //CS.UnityEngine.Debug.Log(vel2);
  return outs;
}
        }),"StateScript/Enemy_Default/Grunt/stateScript_enemyGrunt_Attack_Base.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.StateDef_200_ID = StateDef_200_ID;
//puncher states for stateDef 200, enemy defaults.
function StateDef_200_ID(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let SoundTime = entity.attrs.isSoundNotPlayed == 0;
  let CurrentTime = CS.Elem.CheckStateTime(entity);
  let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
  if (CurrentTime > 16) {
    verdList.Add(3);
  }
  if (CurrentTime == 0) {
    verdList.Add(0);
  }

  //HitDef Generate.
  if (Math.abs(CurrentAnimTime - 9) < 2 && entity.attrs.isStateHit == 0) {
    //Sounddefs..
    if (SoundTime) {
      verdList.Add(100);
    }
    verdList.Add(10);
  }

  //CS.UnityEngine.Debug.Log("Executed");
  return verdList;
}
        }),"StateScript/Enemy_Default/stateScript_enemyDef_cmd.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.stateCmd = stateCmd;
// any meant as any type.
// also use 'let' at first declearation,
// put the variable type name after colon,
// and put the init value lastly.

// We could use List name, if they call variable first.
function stateCmd(entity) {
  //CS.UnityEngine.Debug.Log("executing PuerTS CMDs..");
  //List<Int>
  const List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let selfOnGrd = CS.Elem.isEntityOnGround(entity);
  //let isPressed_A = CS.LuaCondition.CheckButtonPressed(entity, "_a");
  let AttackCmd_b = CS.Elem.CheckButtonPressed(entity, "Basic");
  let StateDefID = entity.CurrentStateID;
  let isChainable = StateDefID == 0 || StateDefID >= 200 && StateDefID < 210;
  let chargeVal = entity.status.ChargeTime;
  let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);

  //this must be set as 0.
  let selfStTime = CS.Elem.CheckStateTime(entity);

  //Ground Attacks - N
  if (selfOnGrd == true && AttackCmd_b == true && StateDefID == 0) {
    verdList.Add(200);
  }
  return verdList;
}
        }),"StateScript/Enemy_Default/stateScript_enemyDef_Hurt.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.StateDef_5000_ID = StateDef_5000_ID;
exports.StateDef_5050_ID = StateDef_5050_ID;
exports.StateDef_5100_ID = StateDef_5100_ID;
exports.StateDef_5101_ID = StateDef_5101_ID;
//Hurt_Init for Default Enemy.(5000)
function StateDef_5000_ID(entity) {
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
  if (CurrentTime == 0 && CurrentAnimID != 5000) {
    verdList.Add(0);
  }
  if (CurrentTime > 12 && isOnGround) {
    verdList.Add(1);
  }
  if (!isAlive && CurrentTime > 6) {
    verdList.Add(2);
  }
  if (SoundTime) {
    verdList.Add(100);
  }
  return verdList;
}

//Hurt_Blowout(5050)
function StateDef_5050_ID(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let SoundTime = entity.attrs.isSoundNotPlayed == 0;
  let CurrentTime = CS.Elem.CheckStateTime(entity);
  let CurrentAnimID = CS.Elem.CheckAnimID(entity);
  let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
  let AnimEndTime = CS.Elem.CheckAnimEndTime(entity);
  let isOnGround = CS.Elem.isEntityOnGround(entity);
  let isAlive = entity.attrs.alive;
  if (CurrentTime == 0 && CurrentAnimID != 5050) {
    verdList.Add(0);
  }
  if (CurrentAnimTime > 6 && CurrentTime > 10 && isOnGround) {
    verdList.Add(1);
  }
  if (SoundTime) {
    verdList.Add(100);
  }
  return verdList;
}

//FallDown(5100)
function StateDef_5100_ID(entity) {
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
  if (CurrentTime == 0) {
    verdList.Add(0);
  }
  if (CurrentAnimID == 5100 && isOnGround && AnimEndTime - CurrentAnimTime < 2 && isAlive) {
    verdList.Add(1);
  }
  if (!isAlive) {
    verdList.Add(2);
  }
  return verdList;
}

//FallDown_Recovery
function StateDef_5101_ID(entity) {
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
  if (CurrentTime == 0) {
    verdList.Add(0);
  }
  if (CurrentAnimID == 5101 && AnimEndTime - CurrentAnimTime < 2) {
    verdList.Add(1);
  }
  return verdList;
}
        }),"StateScript/Enemy_Default/stateScript_enemyDef_main.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.StateDef_0_ID = StateDef_0_ID;
exports.StateDef_0_Param = StateDef_0_Param;
exports.StateDef_50_ID = StateDef_50_ID;
// any meant as any type.
// also use 'let' at first declearation,
// put the variable type name after colon,
// and put the init value lastly.

// We could use List name, if they call variable first.
function StateDef_0_ID(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let selfOnGrd = CS.Elem.isEntityOnGround(entity);
  let isPressed_A = CS.Elem.CheckButtonPressed(entity, "a_");
  let isPressed_B = CS.Elem.CheckButtonPressed(entity, "b_");
  let isPressed_C = CS.Elem.CheckButtonPressed(entity, "c");

  //this must be set as 0.
  let selfStTime = CS.Elem.CheckStateTime(entity);

  //Init.
  if (selfStTime > 0) {
    verdList.Add(0);
  }
  if (selfOnGrd) {
    //Idle/Moving
    verdList.Add(2);
    //ChangeState To Jump. (stateNo - 3)
    if (isPressed_A) {
      verdList.Add(3);
    }
  }
  if (!isPressed_C) {
    verdList.Add(800);
  }
  if (CS.Elem.CheckStateTime(entity) == 1) {
    //Debug.Log("Init Anim Loaded")
    verdList.Add(100);
  }
  if (!entity.attrs.alive) {
    verdList.Add(5100);
  }
  //CS.UnityEngine.Debug.Log("PuerTS MainState Debug Executed Correctly.");
  return verdList;
}

//Function for jump.
function StateDef_50_ID(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let selfStTime = CS.Elem.CheckStateTime(entity);
  let selfOnGrd_f = CS.Elem.isEntityOnGround(entity);

  //On Ground.
  if (selfStTime > 1 && selfOnGrd_f == true) {
    verdList.Add(1);
  }

  //idleのanimを指定する
  if (selfStTime == 0) {
    //Debug.Log("Jumping Vect");
    verdList.Add(50);
  }
  return verdList;
}

// export Parameter @ stateDef 0.
// not only this function, but those params need to be 
// returned as GenericList such like List<object>,
// ..otherwise it fails completely. - N.
function StateDef_0_Param(entity) {
  let List_Object = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Object);
  let outs = new List_Object();
  const Vector2 = CS.UnityEngine.Vector2;
  const Vector3 = CS.UnityEngine.Vector3;
  let vel2 = new Vector2(0, 0);
  let vel3 = new Vector3(0, 0, 0);

  //オブジェクトのRigidBodyを取得する.
  vel3 = entity.rigid.velocity;
  //オブジェクトの正面方向・右方向を考え、Dotで計算.
  let vel_relate_f = entity.transform.forward;
  let vel_relate_r = entity.transform.right;
  vel2.x = Vector3.Dot(vel3, vel_relate_r);
  vel2.y = Vector3.Dot(vel3, vel_relate_f);
  outs.Add(vel2);
  //CS.UnityEngine.Debug.Log(vel2);
  return outs;
}
        }),"StateScript/Enemy_Default/Weaker/stateScript_enemyWeaker_Attack_Base.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.StateDef_200_ID = StateDef_200_ID;
exports.ThrowPos = ThrowPos;
//weaker(thrower) states for stateDef 200, enemy defaults.
//瓶投げ攻撃.
function StateDef_200_ID(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let SoundTime = entity.attrs.isSoundNotPlayed == 0;
  let CurrentTime = CS.Elem.CheckStateTime(entity);
  let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
  let AnimEndTime = CS.Elem.CheckAnimEndTime(entity);

  //ステート変更.
  if (AnimEndTime - CurrentAnimTime < 8 && CurrentTime > 10) {
    verdList.Add(2);
  }
  if (CurrentTime == 0) {
    verdList.Add(0);
  }

  //HitDef Generate.
  if (CurrentTime == 48) {
    //Sounddefs..
    if (SoundTime) {
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
function ThrowPos(entity) {
  let List_Object = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Object);
  let outs = new List_Object();
  const Vector2 = CS.UnityEngine.Vector2;
  const Vector3 = CS.UnityEngine.Vector3;
  let addVec = new Vector3(0, 0, 0);
  let thrower_fw = entity.transform.forward;

  //throwing velocity for sect 0
  addVec = Vector3.op_Addition(Vector3.op_Multiply(Vector3.up, 4.0), Vector3.op_Multiply(thrower_fw, 4.0));
  outs.Add(addVec);

  //hand pos for section 1.
  let thrower_hand = CS.Elem.getEntityBoneTransform(entity, "hand.r");
  addVec = thrower_hand.position;
  outs.Add(addVec);
  return outs;
}
        }),"StateScript/Rusty/_state.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

//must returns List<int> typeface.
//currently, its testing.
function stateID_0() {
  CS.UnityEngine.Debug.Log("Hello world");
}
        }),"StateScript/Rusty/stateScript_cmd.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.stateCmd = stateCmd;
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

// We could use List name, if they call variable first.
function stateCmd(entity) {
  //CS.UnityEngine.Debug.Log("executing PuerTS CMDs..");
  //List<Int>
  const List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
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
  let isChainable = StateDefID == 0 || StateDefID >= 200 && StateDefID < 210;
  let chargeVal = entity.status.ChargeTime;
  let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);

  //this must be set as 0.
  let selfStTime = CS.Elem.CheckStateTime(entity);

  //ガード状態.
  if (selfOnGrd == true && GuardCmd_isPressed && StateDefID == 0) {
    verdList.Add(100);
  }

  //Ground Attacks - N
  if (selfOnGrd == true && AttackCmd_x == true && StateDefID == 0) {
    verdList.Add(200);
  }
  if (selfOnGrd == true && AttackCmd_x == true && StateDefID == 200 && selfStTime > 8) {
    verdList.Add(201);
  }
  if (selfOnGrd == true && AttackCmd_x == true && StateDefID == 201 && selfStTime > 11) {
    verdList.Add(202);
  }
  if (selfOnGrd == true && AttackCmd_x_isPressed && AttackCmd_x_isReleased && isChainable && chargeVal >= 0.15 && chargeVal < 0.4) {
    //CS.UnityEngine.Debug.Log("Charge Attack Test");
    verdList.Add(210);
  }
  if (selfOnGrd == true && AttackCmd_x_isPressed && AttackCmd_x_isReleased && (isChainable || StateDefID == 210 && CurrentAnimTime >= 15) && chargeVal >= 0.4) {
    //CS.UnityEngine.Debug.Log("Charge Attack Test");
    verdList.Add(220);
  }
  //Air Attacks - N
  if (selfOnGrd == false && AttackCmd_x == true && (StateDefID == 50 || StateDefID == 300 && selfStTime > 15)) {
    verdList.Add(300);
  }

  //ここまで通常攻撃

  //Special Ground Attack _ Knife c1
  if (selfOnGrd == true && AttackCmd_y_isPressed && isChainable) {
    verdList.Add(250);
  }
  //Special Ground Attack _ Knife c2
  if (selfOnGrd == true && AttackCmd_y_isPressed && StateDefID == 250 && selfStTime > 12) {
    verdList.Add(251);
  }
  //Special Air Attack _ Knife c1
  if (selfOnGrd == false && AttackCmd_y_isPressed == true && StateDefID == 50) {
    verdList.Add(350);
  }

  //the combo button could charge to the doubleskill - to full skill
  if (AttackCmd_x_isPressed && StateDefID < 5000) {
    verdList.Add(30);
  }
  //for at damaged. the charge is gone to none
  else if (!AttackCmd_x_isPressed || StateDefID >= 5000 && StateDefID <= 5300) {
    verdList.Add(31);
  }
  return verdList;
}
        }),"StateScript/Rusty/stateScript_main.mjs": (function(exports, require, module, __filename, __dirname) {
            // any meant as any type.
// also use 'let' at first declearation,
// put the variable type name after colon,
// and put the init value lastly.

// We could use List name, if they call variable first.
/*
export function StateDef_0_ID(entity) {
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let selfOnGrd = CS.Elem.isEntityOnGround(entity)
    let isPressed_A = CS.Elem.CheckButtonPressed(entity, "a_");
    let isPressed_B = CS.Elem.CheckButtonPressed(entity, "b_");
    let isPressed_C = CS.Elem.CheckButtonPressed(entity, "c");

    //this must be set as 0.
    let selfStTime = CS.Elem.CheckStateTime(entity) 

    //Init.
    if (selfStTime > 0)
    { 
        verdList.Add(0) 
    }

    if (selfOnGrd) 
    { 
        //Idle/Moving
        verdList.Add(2);
        //ChangeState To Jump. (stateNo - 3)
        if (isPressed_A)
        { 
            verdList.Add(3);
        }
    }

    if( !isPressed_C)
    {
            verdList.Add(800);
    }
    
    //entityに登録されたmixerの数が0のときは緊急。
    if( CS.Elem.CheckStateTime(entity) == 1 || CS.Elem.CheckAnimationsListNum(entity) == 0)
    {
        //Debug.Log("Init Anim Loaded")
        verdList.Add(100) 
    }
    if(!entity.attrs.alive)
    {
        verdList.Add(5100)
    }
    //CS.UnityEngine.Debug.Log("PuerTS MainState Debug Executed Correctly.");
    return verdList;
}

//function for Guarding
export function StateDef_5_ID(entity) {
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();

    let selfOnGrd = CS.Elem.isEntityOnGround(entity)
    let isPressed_B = CS.Elem.CheckButtonPressed(entity, "b");

    //this must be set as 0.
    let selfStTime = CS.Elem.CheckStateTime(entity) 

    //Init.
    if (selfStTime == 0)
    { 
        verdList.Add(0) 
    }

    //if you release B or non ground..
    if (!selfOnGrd || !isPressed_B) 
    { 
        //change to Idle.
        verdList.Add(2);
    }
    
    return verdList;
}

//Function for jump.
export function StateDef_50_ID(entity) 
{
    //List<Int>
    let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);    
    
    let verdList = new List_Int();
    let selfStTime =  CS.Elem.CheckStateTime(entity)
    let selfOnGrd_f = CS.Elem.isEntityOnGround(entity)

    //On Ground.
    if(selfStTime > 1 && selfOnGrd_f == true)
    {
        verdList.Add(1);
    }

    //idleのanimを指定する
    if(selfStTime == 0)
    {
    //Debug.Log("Jumping Vect");
    verdList.Add(50);
    }
    return verdList
}

// export Parameter @ stateDef 0.
// not only this function, but those params need to be 
// returned as GenericList such like List<object>,
// ..otherwise it fails completely. - N.
export function StateDef_0_Param(entity) { 
    let List_Object =
        puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Object);
    let outs = new List_Object();

    const Vector2 = CS.UnityEngine.Vector2;
    const Vector3 = CS.UnityEngine.Vector3;
    let vel2 = new Vector2(0, 0)
    let vel3 = new Vector3(0,0,0);

    //オブジェクトのRigidBodyを取得する.
    vel3 = entity.rigid.velocity    
    //オブジェクトの正面方向・右方向を考え、Dotで計算.
    let vel_relate_f = entity.transform.forward;
    let vel_relate_r = entity.transform.right;
    vel2.x = Vector3.Dot(vel3,vel_relate_r)
    vel2.y = Vector3.Dot(vel3,vel_relate_f)

    outs.Add(vel2);
    //CS.UnityEngine.Debug.Log(vel2);
    return outs
}

*/
"use strict";
        }),"StateScript/Rusty/stateScript_Rusty_Attack_Base.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.StateDef_200_ID = StateDef_200_ID;
exports.StateDef_202_ID = StateDef_202_ID;
exports.StateDef_210_ID = StateDef_210_ID;
exports.StateDef_220_ID = StateDef_220_ID;
exports.StateDef_300_ID_JumpKick = StateDef_300_ID_JumpKick;
//puncher states for stateDef 200 and 201
function StateDef_200_ID(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let SoundTime = entity.attrs.isSoundNotPlayed == 0;
  let CurrentTime = CS.Elem.CheckStateTime(entity);
  let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
  if (CurrentTime > 12) {
    verdList.Add(1);
  }
  if (CurrentTime == 0) {
    CS.UnityEngine.Debug.Log("Executed Anim");
    verdList.Add(0);
  }
  //Aight, does native JS supports math function?
  if (Math.abs(CurrentTime - 4) < 2)
    // && entity.attrs.isStateHit == 0
    {
      verdList.Add(10);
    }
  if (SoundTime) {
    verdList.Add(100);
  }
  //CS.UnityEngine.Debug.Log("Executed");
  return verdList;
}

//Kick Finisher states for stateDef 202()
function StateDef_202_ID(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let SoundTime = entity.attrs.isSoundNotPlayed == 0;
  let CurrentTime = CS.Elem.CheckStateTime(entity);
  let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
  if (CurrentTime > 14) {
    verdList.Add(1);
  }
  if (CurrentTime == 0) {
    verdList.Add(0);
  }
  //Aight, does native JS supports math function?
  //current Animtime needs to be set more than 8
  if (Math.abs(CurrentTime - 7) < 4 && CurrentAnimTime > 2 && entity.attrs.isStateHit == 0) {
    verdList.Add(10);
  }
  if (SoundTime) {
    verdList.Add(100);
  }
  return verdList;
}

//upper Finisher states for stateDef 210()
function StateDef_210_ID(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let CurrentTime = CS.Elem.CheckStateTime(entity);
  let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
  let CurrentAnimID = CS.Elem.CheckAnimID(entity);
  let SoundTime = entity.attrs.isSoundNotPlayed == 0 && CurrentAnimTime > 7 && CurrentAnimID == 210;
  if (CurrentAnimTime > 20 && CurrentAnimID == 210) {
    verdList.Add(1);
  }
  if (CurrentTime == 0) {
    verdList.Add(0);
  }
  //Aight, does native JS supports math function?
  //current Animtime needs to be set more than 8
  if (CurrentTime > 3 && CurrentAnimTime > 9 && CurrentAnimTime < 13 && entity.attrs.isStateHit == 0) {
    verdList.Add(10);
  }
  if (SoundTime) {
    verdList.Add(100);
  }
  return verdList;
}

//RoundKicker Finisher states for stateDef 220()
function StateDef_220_ID(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let CurrentTime = CS.Elem.CheckStateTime(entity);
  let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
  let CurrentAnimID = CS.Elem.CheckAnimID(entity);
  let SoundTime = entity.attrs.isSoundNotPlayed == 0 && CurrentAnimTime > 9 && CurrentAnimID == 220;
  if (CurrentAnimTime > 20 && CurrentAnimID == 220) {
    verdList.Add(1);
  }
  if (CurrentTime == 0) {
    verdList.Add(0);
  }
  //Aight, does native JS supports math function?
  //current Animtime needs to be set more than 8
  if (CurrentAnimID == 220 && CurrentAnimTime > 9 && CurrentAnimTime < 14) {
    verdList.Add(10);
  }
  if (SoundTime) {
    verdList.Add(100);
  }
  if (CurrentAnimID == 220 && CurrentAnimTime > 4 && CurrentAnimTime < 12) {
    verdList.Add(11);
  }
  return verdList;
}

//Air Kicker states for stateDef 300()
function StateDef_300_ID_JumpKick(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let CurrentTime = CS.Elem.CheckStateTime(entity);
  let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
  let CurrentAnimID = CS.Elem.CheckAnimID(entity);
  let selfOnGrd = CS.Elem.isEntityOnGround(entity);
  let SoundTime = entity.attrs.isSoundNotPlayed == 0 && CurrentAnimID == 300;

  //Change state on ground.
  if (selfOnGrd) {
    verdList.Add(1);
  }
  if (CurrentTime == 0) {
    verdList.Add(0);
  }
  // !CS.Elem.CheckExecutedID(entity,10)
  //Aight, does native JS supports math function?
  //current Animtime needs to be set more than 8
  if (CurrentAnimTime > 4 && CurrentAnimID == 300 && entity.attrs.isStateHit == 0) {
    verdList.Add(10);
  }
  if (SoundTime) {
    verdList.Add(100);
  }
  return verdList;
}
        }),"StateScript/Rusty/stateScript_Rusty_Attack_Gear.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.StateDef_250_ID_Knife = StateDef_250_ID_Knife;
exports.StateDef_251_ID_KnifeV2 = StateDef_251_ID_KnifeV2;
exports.StateDef_251_ID_Knife_physics = StateDef_251_ID_Knife_physics;
exports.StateDef_350_AirKnife = StateDef_350_AirKnife;
//Knifer states for stateDef 250
function StateDef_250_ID_Knife(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let SoundTime = entity.attrs.isSoundNotPlayed == 0;
  let CurrentTime = CS.Elem.CheckStateTime(entity);
  let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
  let AnimEndTime = CS.Elem.CheckAnimEndTime(entity);
  if (CurrentTime > 20) {
    verdList.Add(1);
  }
  if (CurrentTime == 0) {
    verdList.Add(0);
  }
  //Aight, does native JS supports math function?
  if (Math.abs(CurrentTime - 5) < 2 && entity.attrs.isStateHit == 0) {
    verdList.Add(10);
  }
  if (SoundTime) {
    verdList.Add(100);
  }
  return verdList;
}

//Knifer states for stateDef 250
function StateDef_251_ID_KnifeV2(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let CurrentTime = CS.Elem.CheckStateTime(entity);
  let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
  let AnimEndTime = CS.Elem.CheckAnimEndTime(entity);
  let SoundTime = entity.attrs.isSoundNotPlayed == 0 && CurrentAnimTime > 4 && entity.animID == 251;
  if (AnimEndTime - CurrentAnimTime < 4) {
    verdList.Add(1);
  }
  if (CurrentTime == 0) {
    verdList.Add(0);
  }
  //Aight, does native JS supports math function?
  if (Math.abs(CurrentTime - 7) < 2 && entity.attrs.isStateHit == 0) {
    verdList.Add(10);
  }
  if (SoundTime) {
    verdList.Add(100);
  }
  return verdList;
}
function StateDef_251_ID_Knife_physics(entity) {
  let List_Object = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Object);
  let outs = new List_Object();
  const Vector2 = CS.UnityEngine.Vector2;
  const Vector3 = CS.UnityEngine.Vector3;
  let vel2 = new Vector2(0, 0);
  let vel3 = new Vector3(0, 0, 0);

  //オブジェクトの正面方向・右方向を考え、Dotで計算.
  let vel_relate_f = entity.transform.forward;
  vel3.x = Vector3.ProjectOnPlane(vel_relate_f, Vector3.up).x * 120;
  vel3.z = Vector3.ProjectOnPlane(vel_relate_f, Vector3.up).z * 120;
  outs.Add(vel3);
  //CS.UnityEngine.Debug.Log(vel2);
  return outs;
}

//Air Knifer states for stateDef 51
function StateDef_350_AirKnife(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let SoundTime = entity.attrs.isSoundNotPlayed == 0;
  let CurrentTime = CS.Elem.CheckStateTime(entity);
  let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
  let AnimEndTime = CS.Elem.CheckAnimEndTime(entity);
  let selfOnGrd = CS.Elem.isEntityOnGround(entity);
  if (selfOnGrd) {
    verdList.Add(1);
  }
  if (CurrentTime == 0) {
    verdList.Add(0);
  }
  if (Math.abs(CurrentTime - 7) < 2 && entity.attrs.isStateHit == 0) {
    verdList.Add(10);
  }
  if (SoundTime) {
    verdList.Add(100);
  }
  return verdList;
}
        }),"StateScript/Rusty/stateScript_Rusty_Hurt.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.StateDef_5000_ID = StateDef_5000_ID;
exports.StateDef_5050_ID = StateDef_5050_ID;
exports.StateDef_5100_ID = StateDef_5100_ID;
exports.StateDef_5101_ID = StateDef_5101_ID;
//Hurt_Init(5000)
function StateDef_5000_ID(entity) {
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
  if (CurrentTime == 0 && CurrentAnimID != 5000) {
    verdList.Add(0);
  }

  //OnGround and time is after..
  if (CurrentTime > 12 && isOnGround) {
    verdList.Add(1);
  }
  if (!isAlive && CurrentTime > 6) {
    verdList.Add(2);
  }
  if (SoundTime) {
    verdList.Add(100);
  }
  return verdList;
}

//Hurt_Blowout(5050)
function StateDef_5050_ID(entity) {
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
  if (CurrentTime == 0 && CurrentAnimID != 5050) {
    verdList.Add(0);
  }
  if (CurrentTime > 6 && isOnGround) {
    verdList.Add(1);
  }
  if (SoundTime) {
    verdList.Add(100);
  }
  return verdList;
}

//FallDown(5100)
function StateDef_5100_ID(entity) {
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
  if (CurrentTime == 0) {
    verdList.Add(0);
  }
  if (CurrentAnimID == 5100 && isOnGround && AnimEndTime - CurrentAnimTime < 2 && isAlive) {
    verdList.Add(1);
  }
  if (!isAlive) {
    verdList.Add(2);
  }
  return verdList;
}

//FallDown_Recovery
function StateDef_5101_ID(entity) {
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
  if (CurrentTime == 0) {
    verdList.Add(0);
  }
  if (CurrentAnimID == 5101 && AnimEndTime - CurrentAnimTime < 2) {
    verdList.Add(1);
  }
  return verdList;
}
        }),"StateScript/Rusty/stateScript_Rusty_Main.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.StateDef_0_ID = StateDef_0_ID;
exports.StateDef_0_Param = StateDef_0_Param;
exports.StateDef_100_ID = StateDef_100_ID;
exports.StateDef_105_ID = StateDef_105_ID;
exports.StateDef_20_ID = StateDef_20_ID;
exports.StateDef_50_ID = StateDef_50_ID;
exports.StateDef_55_ID = StateDef_55_ID;
exports.StateDef_60_ID = StateDef_60_ID;
// any meant as any type.
// also use 'let' at first declearation,
// put the variable type name after colon,
// and put the init value lastly.

// We could use List name, if they call variable first.
function StateDef_0_ID(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let selfOnGrd = CS.Elem.isEntityOnGround(entity);
  let isPressed_A = CS.Elem.CheckButtonPressed(entity, "Jump");
  let isPressed_B = CS.Elem.CheckButtonPressed(entity, "_b");
  let isPressed_C = CS.Elem.CheckButtonPressed(entity, "c");
  let selfOnGrd_f = CS.Elem.isEntityOnGround(entity);

  //this must be set as 0.
  let selfStTime = CS.Elem.CheckStateTime(entity);
  let currentAnimID = CS.Elem.CheckAnimID(entity);

  //Init.
  if (selfStTime > 0) {
    verdList.Add(0);
  }
  if (selfOnGrd) {
    //Idle/Moving
    verdList.Add(2);
    //ChangeState To Jump. (stateNo - 3)
    if (isPressed_A) {
      verdList.Add(3);
    }
  }
  if (!isPressed_C) {
    verdList.Add(800);
  }

  //entityに登録されたmixerの数が0のときは緊急。
  if (CS.Elem.CheckStateTime(entity) == 0 && currentAnimID != 0) {
    //Debug.Log("Init Anim Loaded")
    verdList.Add(100);
  }
  if (!entity.attrs.alive) {
    verdList.Add(5100);
  }
  //falling state.
  if (!selfOnGrd_f) {
    verdList.Add(55);
  }
  //CS.UnityEngine.Debug.Log("PuerTS MainState Debug Executed Correctly.");
  return verdList;
}

//function for Rolling.
//currently the Clss is gone for while.
function StateDef_20_ID(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let selfStTime = CS.Elem.CheckStateTime(entity);
  let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);
  let selfOnGrd_f = CS.Elem.isEntityOnGround(entity);

  //if the WishingVect exists, it rolls towards there.
  if (selfStTime > 0) {
    verdList.Add(0);
  }
}

//Function for jump.
function StateDef_50_ID(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let selfStTime = CS.Elem.CheckStateTime(entity);
  let selfOnGrd_f = CS.Elem.isEntityOnGround(entity);

  //On Ground.
  if (selfStTime > 18) {
    verdList.Add(1);
  }

  //idleのanimを指定する
  if (selfStTime == 0) {
    //Debug.Log("Jumping Vect");
    verdList.Add(50);
  }
  return verdList;
}

//Function for falling.
function StateDef_55_ID(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let selfStTime = CS.Elem.CheckStateTime(entity);
  let selfOnGrd_f = CS.Elem.isEntityOnGround(entity);

  //idleのanimを指定する
  if (selfStTime == 0) {
    //Debug.Log("Jumping Vect");
    verdList.Add(0);
  }
  //On Ground.
  if (selfStTime > 1 && selfOnGrd_f == true) {
    CS.UnityEngine.Debug.Log("Jumping Vect");
    verdList.Add(1);
  }
  return verdList;
}

//Function for landing.
function StateDef_60_ID(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList_F = new List_Int();
  let selfStTime = CS.Elem.CheckStateTime(entity);
  let selfOnGrd_f = CS.Elem.isEntityOnGround(entity);

  //On Ground.
  if (selfStTime > 1 && selfOnGrd_f == true) {
    verdList_F.Add(1);
  }

  //idleのanimを指定する
  if (selfStTime == 0) {
    //Debug.Log("Jumping Vect");
    verdList_F.Add(50);
  }
  return verdList_F;
}

// export Parameter @ stateDef 0.
// not only this function, but those params need to be 
// returned as GenericList such like List<object>,
// ..otherwise it fails completely. - N.
function StateDef_0_Param(entity) {
  let List_Object = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Object);
  let outs = new List_Object();
  const Vector2 = CS.UnityEngine.Vector2;
  const Vector3 = CS.UnityEngine.Vector3;
  let vel2 = new Vector2(0, 0);
  let vel3 = new Vector3(0, 0, 0);

  //オブジェクトのRigidBodyを取得する.
  vel3 = entity.rigid.velocity;
  //オブジェクトの正面方向・右方向を考え、Dotで計算.
  let vel_relate_f = entity.transform.forward;
  let vel_relate_r = entity.transform.right;
  vel2.x = Vector3.Dot(vel3, vel_relate_r);
  vel2.y = Vector3.Dot(vel3, vel_relate_f);
  outs.Add(vel2);
  //CS.UnityEngine.Debug.Log(vel2);
  return outs;
}

//function for Guarding
function StateDef_100_ID(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let selfOnGrd = CS.Elem.isEntityOnGround(entity);
  let isPressed_B = CS.Elem.CheckButtonPressed(entity, "Guarding");

  //this must be set as 0.
  let selfStTime = CS.Elem.CheckStateTime(entity);

  //Init.
  if (selfStTime == 0) {
    verdList.Add(0);
  }

  //if you release B or non ground..
  if (!isPressed_B) {
    //change to Idle.
    verdList.Add(1);
  }

  //Guarding State is continued.
  verdList.Add(10);
  return verdList;
}

//function for Guarding_GettingHurt - and also set non damage
function StateDef_105_ID(entity) {
  //List<Int>
  let List_Int = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
  let verdList = new List_Int();
  let isPressed_B = CS.Elem.CheckButtonPressed(entity, "Guarding");
  let selfOnGrd = CS.Elem.isEntityOnGround(entity);
  let CurrentAnimTime = CS.Elem.CheckAnimTime(entity);

  //this must be set as 0.
  let selfStTime = CS.Elem.CheckStateTime(entity);

  //Init.
  if (selfStTime == 0) {
    verdList.Add(0);
  }

  //after taking hits.
  //if you release B or non ground.. change to init.
  if (CurrentAnimTime >= 10) {
    if (!isPressed_B) {
      //change to Idle.
      verdList.Add(1);
    }
    //if not, continue and changestate to guarding.
    else {
      verdList.Add(2);
    }
    verdList.Add(3);
  }
  verdList.Add(10);
  return verdList;
}
        }),"puerts/csharp.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

/*
 * Tencent is pleased to support the open source community by making Puerts available.
 * Copyright (C) 2020 Tencent.  All rights reserved.
 * Puerts is licensed under the BSD 3-Clause License, except for the third-party components listed in the file 'LICENSE' which may be subject to their corresponding license terms. 
 * This file is subject to the terms and conditions defined in file 'LICENSE', which is part of this source code package.
 */

var global = global || globalThis || function () {
  return this;
}();
function csTypeToClass(csType) {
  let cls = puer.loadType(csType);
  if (cls) {
    let currentCls = cls,
      parentPrototype = Object.getPrototypeOf(currentCls.prototype);

    // 此处parentPrototype如果是一个泛型，会丢失父父的继承信息，必须循环找下去
    while (parentPrototype) {
      Object.setPrototypeOf(currentCls, parentPrototype.constructor); //v8 api的inherit并不能把静态属性也继承，通过这种方式修复下
      currentCls.__static_inherit__ = true;
      currentCls = parentPrototype.constructor;
      parentPrototype = Object.getPrototypeOf(currentCls.prototype);
      if (currentCls === Object || currentCls === Function || currentCls.__static_inherit__) break;
    }
    let readonlyStaticMembers;
    if (readonlyStaticMembers = cls.__puertsMetadata.get('readonlyStaticMembers')) {
      cls.__puertsMetadata.set('readonlyStaticMembers', undefined);
      for (var key in cls) {
        let desc = Object.getOwnPropertyDescriptor(cls, key);
        if (readonlyStaticMembers.has(key) && desc && typeof desc.get == 'function' && typeof desc.value == 'undefined') {
          let getter = desc.get;
          let value;
          let valueGetted = false;
          if (desc.configurable) {
            Object.defineProperty(cls, key, Object.assign(desc, {
              get() {
                if (!valueGetted) {
                  value = getter();
                  valueGetted = true;
                }
                return value;
              },
              configurable: false
            }));
          }
          if (cls.__p_innerType.IsEnum) {
            const val = cls[key];
            if (typeof val == 'number') {
              cls[val] = key;
            }
          }
        }
      }
    }
    let nestedTypes = puer.getNestedTypes(csType);
    if (nestedTypes) {
      for (var i = 0; i < nestedTypes.Length; i++) {
        let ntype = nestedTypes.get_Item(i);
        if (ntype.IsGenericType) {
          let name = ntype.Name.split('`')[0] + '$' + ntype.GetGenericArguments().Length;
          let fullName = ntype.FullName.split('`')[0] /**.replace(/\+/g, '.') */ + '$' + ntype.GetGenericArguments().Length;
          let genericTypeInfo = cls[name] = new Map();
          genericTypeInfo.set('$name', fullName.replace('$', '`'));
        } else {
          try {
            cls[ntype.Name] = csTypeToClass(ntype);
          } catch (e) {
            console.warn(`load nestedtype [${ntype.Name || ntype}] of ${csType.Name || csType} fail: ${e}`);
          }
        }
      }
    }
  }
  return cls;
}
function Namespace() {}
puer.__$NamespaceType = Namespace;
function createTypeProxy(namespace) {
  return new Proxy(new Namespace(), {
    get: function (cache, name) {
      if (name == '__p_innerType') return void 0;
      if (!(name in cache)) {
        let fullName = namespace ? namespace + '.' + name : name;
        if (/\$\d+$/.test(name)) {
          let genericTypeInfo = cache[name] = new Map();
          genericTypeInfo.set('$name', fullName.replace('$', '`'));
        } else {
          let cls = csTypeToClass(fullName);
          if (cls) {
            cache[name] = cls;
          } else {
            cache[name] = createTypeProxy(fullName);
            //console.log(fullName + ' is a namespace');
          }
        }
      }
      return cache[name];
    }
  });
}
let csharpModule = createTypeProxy(undefined);
csharpModule.default = csharpModule;
global.CS = csharpModule;
csharpModule.System.Object.prototype.toString = csharpModule.System.Object.prototype.ToString;
function ref(x) {
  return [x];
}
function unref(r) {
  return r[0];
}
function setref(x, val) {
  x[0] = val;
}
function taskToPromise(task) {
  return new Promise((resolve, reject) => {
    task.GetAwaiter().UnsafeOnCompleted(() => {
      let t = task;
      task = undefined;
      if (t.IsFaulted) {
        if (t.Exception) {
          if (t.Exception.InnerException) {
            reject(t.Exception.InnerException.Message);
          } else {
            reject(t.Exception.Message);
          }
        } else {
          reject("unknow exception!");
        }
      } else {
        resolve(t.Result);
      }
    });
  });
}
function genIterator(obj) {
  let it = obj.GetEnumerator();
  return {
    next() {
      if (it.MoveNext()) {
        return {
          value: it.Current,
          done: false
        };
      }
      it.Dispose();
      return {
        value: null,
        done: true
      };
    }
  };
}
function makeGeneric(genericTypeInfo, ...genericArgs) {
  let p = genericTypeInfo;
  for (var i = 0; i < genericArgs.length; i++) {
    let genericArg = genericArgs[i];
    if (!p.has(genericArg)) {
      p.set(genericArg, new Map());
    }
    p = p.get(genericArg);
  }
  if (!p.has('$type')) {
    let typName = genericTypeInfo.get('$name');
    let typ = puer.loadType(typName, ...genericArgs);
    let csType = getType(typ);
    if (getType(csharpModule.System.Collections.IEnumerable).IsAssignableFrom(csType)) {
      typ.prototype[Symbol.iterator] = function () {
        return genIterator(this);
      };
    }
    let nestedTypes = puer.getNestedTypes(csType);
    if (nestedTypes) {
      for (var i = 0; i < nestedTypes.Length; i++) {
        let ntype = nestedTypes.get_Item(i);
        if (ntype.IsGenericTypeDefinition) {
          genericArgs = genericArgs.map(g => puer.$typeof(g) || g);
          ntype = ntype.MakeGenericType(...genericArgs);
        }
        try {
          typ[ntype.Name] = csTypeToClass(ntype);
        } catch (e) {
          console.warn(`load nestedtype [${ntype.Name || ntype}] of ${csType.Name || csType} fail: ${e}`);
        }
      }
    }
    p.set('$type', typ);
  }
  return p.get('$type');
}
function makeGenericMethod(cls, methodName, ...genericArgs) {
  if (cls && typeof methodName == 'string' && genericArgs && genericArgs.length > 0) {
    return puer.getGenericMethod(puer.$typeof(cls), methodName, ...genericArgs);
  } else {
    throw new Error("invalid arguments for makeGenericMethod");
  }
}
function getType(cls) {
  return cls.__p_innerType;
}
function bindThisToFirstArgument(func, parentFunc) {
  if (parentFunc) {
    return function (...args) {
      try {
        return func.apply(null, [this, ...args]);
      } catch {
        return parentFunc.call(this, ...args);
      }
      ;
    };
  }
  return function (...args) {
    return func.apply(null, [this, ...args]);
  };
}
function doExtension(cls, extension) {
  // if you already generate static wrap for cls and extension, then you are no need to invoke this function
  // 如果你已经为extension和cls生成静态wrap，则不需要调用这个函数。
  var parentPrototype = Object.getPrototypeOf(cls.prototype);
  Object.keys(extension).forEach(key => {
    var func = extension[key];
    if (typeof func == 'function' && key != 'constructor' && !(key in cls.prototype)) {
      var parentFunc = parentPrototype ? parentPrototype[key] : undefined;
      parentFunc = typeof parentFunc === "function" ? parentFunc : undefined;
      Object.defineProperty(cls.prototype, key, {
        value: bindThisToFirstArgument(func, parentFunc),
        writable: false,
        configurable: false
      });
    }
  });
}
puer.$ref = ref;
puer.$unref = unref;
puer.$set = setref;
puer.$promise = taskToPromise;
puer.$generic = makeGeneric;
puer.$genericMethod = makeGenericMethod;
puer.$typeof = getType;
puer.$extension = (cls, extension) => {
  typeof console != 'undefined' && console.warn(`deprecated! if you already generate static wrap for ${cls} and ${extension}, you are no need to invoke $extension`);
  return doExtension(cls, extension);
};
        }),"puerts/dispose.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.default = resetAllFunctionWhenDisposed;
/*
* Tencent is pleased to support the open source community by making Puerts available.
* Copyright (C) 2020 Tencent.  All rights reserved.
* Puerts is licensed under the BSD 3-Clause License, except for the third-party components listed in the file 'LICENSE' which may be subject to their corresponding license terms. 
* This file is subject to the terms and conditions defined in file 'LICENSE', which is part of this source code package.
*/
var global = global || globalThis || function () {
  return this;
}();
function resetAllFunctionWhenDisposed() {
  global.puer.disposed = true;
  const PuerIsDisposed = function () {
    throw new Error('puerts has disposed');
  };
  puer.loadType = PuerIsDisposed;
  puer.getNestedTypes = PuerIsDisposed;
  try {
    setToGoodbyeFuncRecursive(CS);
  } catch (e) {}
  function setToGoodbyeFuncRecursive(obj) {
    Object.keys(obj).forEach(key => {
      if (obj[key] == obj) {
        return; // a member named default is the obj itself which is in the root
      }
      setToGoodbyeFuncRecursive(obj[key]);
      if (typeof obj[key] == 'function' && obj[key].prototype) {
        const prototype = obj[key].prototype;
        Object.keys(prototype).forEach(pkey => {
          if (Object.getOwnPropertyDescriptor(prototype, pkey).configurable) {
            Object.defineProperty(prototype, pkey, {
              get: PuerIsDisposed,
              set: PuerIsDisposed
            });
          }
        });
        Object.keys(obj[key]).forEach(skey => {
          if (Object.getOwnPropertyDescriptor(obj[key], skey).configurable) {
            Object.defineProperty(obj[key], skey, {
              get: PuerIsDisposed,
              set: PuerIsDisposed
            });
          }
        });
      }
      if (obj[key] instanceof puer.__$NamespaceType) {
        Object.defineProperty(obj, key, {
          get: PuerIsDisposed,
          set: PuerIsDisposed
        });
      }
    });
  }
}
        }),"puerts/events.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

/*
* Tencent is pleased to support the open source community by making Puerts available.
* Copyright (C) 2020 Tencent.  All rights reserved.
* Puerts is licensed under the BSD 3-Clause License, except for the third-party components listed in the file 'LICENSE' which may be subject to their corresponding license terms. 
* This file is subject to the terms and conditions defined in file 'LICENSE', which is part of this source code package.
*/

var global = global || globalThis || function () {
  return this;
}();
let events = Object.create(null);
let eventsCount = 0;
function checkListener(listener) {
  if (typeof listener !== 'function') {
    throw new Error('listener expect a function');
  }
}
function on(type, listener, prepend) {
  checkListener(listener);
  let existing = events[type];
  if (existing === undefined) {
    events[type] = listener;
    ++eventsCount;
  } else {
    if (typeof existing === 'function') {
      events[type] = prepend ? [listener, existing] : [existing, listener];
    } else if (prepend) {
      existing.unshift(listener);
    } else {
      existing.push(listener);
    }
  }
}
function off(type, listener) {
  checkListener(listener);
  const list = events[type];
  if (list === undefined) return;
  if (list === listener) {
    if (--eventsCount === 0) events = Object.create(null);else {
      events[type] = undefined;
    }
  } else if (typeof list !== 'function') {
    for (var i = list.length - 1; i >= 0; i--) {
      if (list[i] === listener) {
        //found
        if (i === 0) list.shift();else {
          spliceOne(list, i);
        }
        if (list.length === 1) events[type] = list[0];
        break;
      }
    }
  }
}
function emit(type, ...args) {
  const listener = events[type];
  if (listener === undefined) return false;
  if (typeof listener === 'function') {
    Reflect.apply(listener, this, args);
  } else {
    const len = listener.length;
    const listeners = arrayClone(listener, len);
    for (var i = 0; i < len; ++i) Reflect.apply(listeners[i], this, args);
  }
  return true;
}
function arrayClone(arr, n) {
  const copy = new Array(n);
  for (var i = 0; i < n; ++i) copy[i] = arr[i];
  return copy;
}
function spliceOne(list, index) {
  for (; index + 1 < list.length; index++) list[index] = list[index + 1];
  list.pop();
}
puer.on = on;
puer.off = off;
puer.emit = emit;
        }),"puerts/init_il2cpp.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

/*
 * Tencent is pleased to support the open source community by making Puerts available.
 * Copyright (C) 2020 Tencent.  All rights reserved.
 * Puerts is licensed under the BSD 3-Clause License, except for the third-party components listed in the file 'LICENSE' which may be subject to their corresponding license terms. 
 * This file is subject to the terms and conditions defined in file 'LICENSE', which is part of this source code package.
 */

var global = global || globalThis || function () {
  return this;
}();
// polyfill old code after use esm module.
global.global = global;
let puer = global.puer = global.puerts = global.puer || global.puerts || {};
puer.loadType = function (nameOrCSType, ...genericArgs) {
  let csType = nameOrCSType;
  if (typeof nameOrCSType == "string") {
    // convert string to csType
    csType = jsEnv.GetTypeByString(nameOrCSType);
  }
  if (csType) {
    if (genericArgs && genericArgs.length > 0 && csType.IsGenericTypeDefinition) {
      genericArgs = genericArgs.map(g => puer.$typeof(g));
      csType = csType.MakeGenericType(...genericArgs);
    }
    let cls = loadType(csType);
    if (!cls) {
      console.warn(`load ${csType.Name || csType} fail!`);
      return;
    }
    cls.__p_innerType = csType;
    // todo
    cls.__puertsMetadata = cls.__puertsMetadata || new Map();
    let fields = csType.GetFields(26);
    for (var i = 0; i < fields.Length; ++i) {
      let field = fields.get_Item(i);
      if (field.IsInitOnly || field.IsLiteral) {
        let readonlyStaticMembers = cls.__puertsMetadata.get('readonlyStaticMembers');
        if (!readonlyStaticMembers) {
          readonlyStaticMembers = new Set();
          cls.__puertsMetadata.set('readonlyStaticMembers', readonlyStaticMembers);
        }
        readonlyStaticMembers.add(field.Name);
      }
    }
    return cls;
  }
};
let BindingFlags = puer.loadType("System.Reflection.BindingFlags");
let GET_MEMBER_FLAGS = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
puer.getNestedTypes = function (nameOrCSType) {
  let csType = nameOrCSType;
  if (typeof nameOrCSType == "string") {
    csType = jsEnv.GetTypeByString(nameOrCSType);
  }
  if (csType) {
    return csType.GetNestedTypes(GET_MEMBER_FLAGS);
  }
};
puer.createFunction = global.createFunction;
global.createFunction = undefined;
puer.getGenericMethod = function (csType, methodName, ...genericArgs) {
  if (!csType || typeof csType.GetMember != 'function') {
    throw new Error('the class must be a constructor');
  }
  let members = CS.Puerts.Utils.GetMethodAndOverrideMethodByName(csType, methodName);
  let overloadFunctions = [];
  for (let i = 0; i < members.Length; i++) {
    let method = members.GetValue(i);
    if (method.IsGenericMethodDefinition && method.GetGenericArguments().Length == genericArgs.length) {
      let methodImpl = method.MakeGenericMethod(...genericArgs.map((x, index) => {
        const ret = puer.$typeof(x);
        if (!ret) {
          throw new Error("invalid Type for generic arguments " + index);
        }
        return ret;
      }));
      overloadFunctions.push(methodImpl);
    }
  }
  let overloadCount = overloadFunctions.length;
  if (overloadCount == 0) {
    console.error("puer.getGenericMethod not found", csType.Name, methodName, genericArgs.map(x => puer.$typeof(x).Name).join(","));
    return null;
  }
  return puer.createFunction(...overloadFunctions);
};
puer.getLastException = global.__puertsGetLastException;
global.__puertsGetLastException = undefined;
puer.evalScript = global.__tgjsEvalScript || function (script, debugPath) {
  return eval(script);
};
global.__tgjsEvalScript = undefined;
let loader = jsEnv.GetLoader();
// function loadFile(path) {
//     let resolved, content
//     if (resolved = loader.Resolve(path)) {
//         let contents = []
//         loader.ReadFile(resolved, contents);
//         content = contents[0]
//     }
//     return { content: content, debugPath: resolved };
// }
// puer.loadFile = loadFile;

// puer.fileExists = loader.Resolve.bind(loader);
function loadFile(path) {
  let debugPath = [];
  var content = loader.ReadFile(path, debugPath);
  return {
    content: content,
    debugPath: debugPath[0]
  };
}
puer.loadFile = loadFile;
puer.fileExists = loader.FileExists.bind(loader);
global.__tgjsRegisterTickHandler = function (fn) {
  fn = new CS.System.Action(fn);
  jsEnv.TickHandler = CS.System.Delegate.Combine(jsEnv.TickHandler, fn);
};
        }),"puerts/init.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

/*
 * Tencent is pleased to support the open source community by making Puerts available.
 * Copyright (C) 2020 Tencent.  All rights reserved.
 * Puerts is licensed under the BSD 3-Clause License, except for the third-party components listed in the file 'LICENSE' which may be subject to their corresponding license terms. 
 * This file is subject to the terms and conditions defined in file 'LICENSE', which is part of this source code package.
 */

var global = global || globalThis || function () {
  return this;
}();
// polyfill old code after use esm module.
global.global = global;
let puer = global.puer = global.puerts = global.puer || global.puerts || {};
puer.loadType = global.__tgjsLoadType;
global.__tgjsLoadType = undefined;
puer.getNestedTypes = global.__tgjsGetNestedTypes;
global.__tgjsGetNestedTypes = undefined;
//puer.getGenericMethod = global.__tgjsGetGenericMethod;
global.__tgjsGetGenericMethod = undefined;
puer.createFunction = global.createFunction;
global.createFunction = undefined;
puer.getGenericMethod = function (csType, methodName, ...genericArgs) {
  if (!csType || typeof csType.GetMember != 'function') {
    throw new Error('the class must be a constructor');
  }
  let members = CS.Puerts.Utils.GetMethodAndOverrideMethodByName(csType, methodName);
  let overloadFunctions = [];
  for (let i = 0; i < members.Length; i++) {
    let method = members.GetValue(i);
    if (method.IsGenericMethodDefinition && method.GetGenericArguments().Length == genericArgs.length) {
      let methodImpl = method.MakeGenericMethod(...genericArgs.map((x, index) => {
        const ret = puer.$typeof(x);
        if (!ret) {
          throw new Error("invalid Type for generic arguments " + index);
        }
        return ret;
      }));
      overloadFunctions.push(methodImpl);
    }
  }
  let overloadCount = overloadFunctions.length;
  if (overloadCount == 0) {
    console.error("puer.getGenericMethod not found", csType.Name, methodName, genericArgs.map(x => puer.$typeof(x).Name).join(","));
    return null;
  }
  return puer.createFunction(...overloadFunctions);
};
puer.evalScript = global.__tgjsEvalScript || function (script, debugPath) {
  return eval(script);
};
global.__tgjsEvalScript = undefined;
puer.getLastException = global.__puertsGetLastException;
global.__puertsGetLastException = undefined;
let loader = global.__tgjsGetLoader();
global.__tgjsGetLoader = undefined;
function loadFile(path) {
  let debugPath = [];
  var content = loader.ReadFile(path, debugPath);
  return {
    content: content,
    debugPath: debugPath[0]
  };
}
puer.loadFile = loadFile;
puer.fileExists = loader.FileExists.bind(loader);
        }),"puerts/log.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

/*
 * Tencent is pleased to support the open source community by making Puerts available.
 * Copyright (C) 2020 Tencent.  All rights reserved.
 * Puerts is licensed under the BSD 3-Clause License, except for the third-party components listed in the file 'LICENSE' which may be subject to their corresponding license terms. 
 * This file is subject to the terms and conditions defined in file 'LICENSE', which is part of this source code package.
 */

var global = global || globalThis || function () {
  return this;
}();
let UnityEngine_Debug = puer.loadType('UnityEngine.Debug');
if (!UnityEngine_Debug) {
  // in quickjs, global.console is undefined
  // so we decide polyfill the unityengine.debug.log in non-unity env
  const CSConsole = puer.loadType('System.Console');
  if (CSConsole) {
    UnityEngine_Debug = {
      Log: (...args) => CSConsole.WriteLine(["[Log]", ...args].join(' ')),
      LogWarn: (...args) => CSConsole.Error.WriteLine(["[LogWarn]", ...args].join(' ')),
      LogError: (...args) => CSConsole.Error.WriteLine(["[LogError]", ...args].join(' ')),
      Assert: () => {}
    };
  }
}
if (UnityEngine_Debug || !global.console) {
  const console_org = global.console;
  var console = {};
  function toString(args) {
    return Array.prototype.map.call(args, x => {
      try {
        return x instanceof Error ? x.stack : x + '';
      } catch (err) {
        return err;
      }
    }).join(' ');
  }
  function getStack(error) {
    let stack = error.stack; // get js stack
    stack = stack.substring(stack.indexOf("\n") + 1); // remove first line ("Error")
    stack = stack.replace(/^ {4}/gm, ""); // remove indentation
    return stack;
  }
  console.log = function () {
    if (console_org) console_org.log.apply(null, Array.prototype.slice.call(arguments));
    UnityEngine_Debug.Log(toString(arguments));
  };
  console.info = function () {
    if (console_org) console_org.info.apply(null, Array.prototype.slice.call(arguments));
    UnityEngine_Debug.Log(toString(arguments));
  };
  console.warn = function () {
    if (console_org) console_org.warn.apply(null, Array.prototype.slice.call(arguments));
    UnityEngine_Debug.LogWarning(toString(arguments));
  };
  console.error = function () {
    if (console_org) console_org.error.apply(null, Array.prototype.slice.call(arguments));
    UnityEngine_Debug.LogError(toString(arguments));
  };
  console.debug = function () {
    if (console_org) console_org.debug.apply(null, Array.prototype.slice.call(arguments));
    UnityEngine_Debug.Log(toString(arguments));
  };
  console.trace = function () {
    if (console_org) console_org.trace.apply(null, Array.prototype.slice.call(arguments));
    UnityEngine_Debug.Log(toString(arguments) + "\n" + getStack(new Error()) + "\n");
  };
  console.assert = function (condition) {
    if (console_org) console_org.assert.apply(null, Array.prototype.slice.call(arguments));
    if (condition) return;
    if (arguments.length > 1) UnityEngine_Debug.Assert(false, "Assertion failed: " + toString(Array.prototype.slice.call(arguments, 1)) + "\n" + getStack(new Error()) + "\n");else UnityEngine_Debug.Assert(false, "Assertion failed: console.assert\n" + getStack(new Error()) + "\n");
  };
  const timeRecorder = new Map();
  console.time = function (name) {
    timeRecorder.set(name, +new Date());
  };
  console.timeLog = function (name, ...args) {
    const startTime = timeRecorder.get(name);
    if (startTime) {
      console.log(String(name) + ": " + (+new Date() - startTime) + " ms", ...args);
    } else {
      console.warn("Timer '" + String(name) + "' does not exist");
    }
    ;
  };
  console.timeEnd = function (name) {
    const startTime = timeRecorder.get(name);
    if (startTime) {
      console.log(String(name) + ": " + (+new Date() - startTime) + " ms");
      timeRecorder.delete(name);
    } else {
      console.warn("Timer '" + String(name) + "' does not exist");
    }
    ;
  };
  global.console = console;
  puer.console = console;
}
        }),"puerts/module.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.clearModuleCache = clearModuleCache;
exports.createRequire = createLazyRequire;
exports.deleteModuleCache = deleteModuleCache;
exports.gcModuleCache = gcModuleCache;
exports.hasModuleCache = hasModuleCache;
exports.statModuleCache = statModuleCache;
const CHAR_DOT = 46;
const CHAR_FORWARD_SLASH = 47;
const CHAR_BACKWARD_SLASH = 92;
function isPathSeparator(code) {
  return code === CHAR_FORWARD_SLASH || code === CHAR_BACKWARD_SLASH;
}
function normalizeString(path, allowAboveRoot, separator, isPathSeparator) {
  let res = '';
  let lastSegmentLength = 0;
  let lastSlash = -1;
  let dots = 0;
  let code = 0;
  for (let i = 0; i <= path.length; ++i) {
    if (i < path.length) code = path.charCodeAt(i);else if (isPathSeparator(code)) break;else code = CHAR_FORWARD_SLASH;
    if (isPathSeparator(code)) {
      if (lastSlash === i - 1 || dots === 1) {
        // NOOP
      } else if (dots === 2) {
        if (res.length < 2 || lastSegmentLength !== 2 || res.charCodeAt(res.length - 1) !== CHAR_DOT || res.charCodeAt(res.length - 2) !== CHAR_DOT) {
          if (res.length > 2) {
            const lastSlashIndex = res.lastIndexOf(separator);
            if (lastSlashIndex === -1) {
              res = '';
              lastSegmentLength = 0;
            } else {
              res = res.slice(0, lastSlashIndex);
              lastSegmentLength = res.length - 1 - res.lastIndexOf(separator);
            }
            lastSlash = i;
            dots = 0;
            continue;
          } else if (res.length !== 0) {
            res = '';
            lastSegmentLength = 0;
            lastSlash = i;
            dots = 0;
            continue;
          }
        }
        if (allowAboveRoot) {
          res += res.length > 0 ? `${separator}..` : '..';
          lastSegmentLength = 2;
        }
      } else {
        if (res.length > 0) res += `${separator}${path.slice(lastSlash + 1, i)}`;else res = path.slice(lastSlash + 1, i);
        lastSegmentLength = i - lastSlash - 1;
      }
      lastSlash = i;
      dots = 0;
    } else if (code === CHAR_DOT && dots !== -1) {
      ++dots;
    } else {
      dots = -1;
    }
  }
  return res;
}
function normalizeAsPosix(path) {
  if (path.length === 0) return '.';
  const isAbsolute = isPathSeparator(path.charCodeAt(0));
  const trailingSeparator = isPathSeparator(path.charCodeAt(path.length - 1));

  // Normalize the path
  path = normalizeString(path, !isAbsolute, '/', isPathSeparator);
  if (path.length === 0) {
    if (isAbsolute) return '/';
    return trailingSeparator ? './' : '.';
  }
  if (trailingSeparator) path += '/';
  return isAbsolute ? `/${path}` : path;
}
function joinAsPosix(...args) {
  if (args.length === 0) return '.';
  let joined;
  for (let i = 0; i < args.length; ++i) {
    const arg = args[i];
    if (arg.length > 0) {
      if (joined === undefined) joined = arg;else joined += `/${arg}`;
    }
  }
  if (joined === undefined) return '.';
  return normalizeAsPosix(joined);
}
const {
  iterator
} = Symbol;
class ModuleCache extends Map {
  #get = (key, [ref, isWeak]) => {
    if (isWeak) {
      const value = ref.deref();
      if (!value) {
        this.delete(key);
      }
      return value;
    } else {
      return ref;
    }
  };
  set(key, value, isWeak) {
    //console.log(`set ${key} ${value} ${isWeak}`);
    super.delete(key);
    if (isWeak) {
      const ref = new WeakRef(value);
      return super.set(key, [ref, true]);
    } else {
      return super.set(key, [value, false]);
    }
  }
  get(key) {
    const pair = super.get(key);
    return pair && this.#get(key, pair);
  }
  has(key) {
    return !!this.get(key);
  }
  stat() {
    let res = 'key\tweak?\tvalid?\n';
    for (const [key, [ref, isWeak]] of super[iterator]()) {
      res += `${key}\t${isWeak}\t${!isWeak || !!ref.deref()}\n`;
    }
    return res;
  }
  gc() {
    for (const [key, [ref, isWeak]] of super[iterator]()) {
      if (isWeak && !ref.deref()) {
        this.delete(key);
      }
    }
  }
}
const exportsCache = typeof WeakRef === 'function' ? new ModuleCache() : new Map();
const tmpModuleStorage = new Map();
const builtinModules = new Map([["csharp", CS], ["puer", puer], ["puerts", puer]]);

//console.log(joinAsPosix('a/b/c', '../..'));
//console.log(joinAsPosix('a\\b\\c', '../..'));
//console.log(joinAsPosix('a/b\\c', '../..'));
//console.log(joinAsPosix('a.js'));

function fileURLToPath(url) {
  if (url.startsWith('file:') || url.startsWith('puer:')) {
    return url.substr(5);
  } else {
    return url;
  }
}
function dirname(path) {
  const len = path.length;
  if (len === 0) return '';
  let end = -1;
  let matchedSlash = true;
  for (let i = len - 1; i >= 0; --i) {
    if (isPathSeparator(path.charCodeAt(i))) {
      if (!matchedSlash) {
        end = i;
        break;
      }
    } else {
      // We saw the first non-path separator
      matchedSlash = false;
    }
  }
  if (end === -1) {
    return '';
  }
  return path.slice(0, end);
}
function executeModule(fullPath, script, debugPath) {
  if (debugPath === undefined) debugPath = fullPath;
  let exports = {};
  let module = tmpModuleStorage.get(fullPath);
  module.exports = exports;
  let wrapped = puer.evalScript(
  // Wrap the script in the same way NodeJS does it. It is important since IDEs (VSCode) will use this wrapper pattern
  // to enable stepping through original source in-place.
  "(function (exports, require, module, __filename, __dirname) { " + script + "\n});", debugPath);
  wrapped(exports, createLazyRequire(fullPath), module, debugPath, dirname(debugPath));
  return module.exports;
}
let __default_is_weak = true;
function createLazyRequire(referer) {
  const filename = normalizeAsPosix(fileURLToPath(referer));
  //console.log(`createLazyRequire(${referer}): ${filename}`);
  let requiringDir = dirname(filename);
  //console.log(`requiringDir:${requiringDir}`)

  function require(specifier) {
    //console.log(`require(${specifier}) by ${referer}`);
    let fullPath = joinAsPosix(requiringDir, specifier);
    let key = fullPath;
    let res = exportsCache.get(key);
    if (res) {
      return res;
    }
    const tmpModule = tmpModuleStorage.get(key);
    if (tmpModule) {
      return tmpModule.exports;
    }
    let {
      content,
      debugPath
    } = puer.loadFile(fullPath);
    if (content === null) {
      throw new Error(`load ${fullPath} fail!`);
    }
    let module = {
      "exports": {}
    };
    tmpModuleStorage.set(fullPath, module);
    try {
      if (fullPath.endsWith(".json")) {
        let packageConfigure = JSON.parse(content);
        if (fullPath.endsWith("package.json")) {
          let url = packageConfigure.main || "index.js";
          let tmpRequire = createLazyRequire(fullPath);
          let r = tmpRequire(url);
          module.exports = r;
        } else {
          module.exports = packageConfigure;
        }
      } else {
        //console.warn(`executeModule(${fullPath})`)
        executeModule(fullPath, content, debugPath);
      }
      exportsCache.set(key, module.exports, typeof module.exports.__auto_release !== 'boolean' ? __default_is_weak : module.exports.__auto_release);
    } catch (e) {
      exportsCache.delete(key);
      throw e;
    } finally {
      tmpModuleStorage.delete(fullPath);
    }
    return module.exports;
  }

  // 理论上比new Proxy会快些
  function proxyTo(obj, target) {
    const descriptors = Object.getOwnPropertyDescriptors(target);
    for (const key in descriptors) {
      if (descriptors.hasOwnProperty(key)) {
        const descriptor = descriptors[key];

        // 优化函数调用，不过副作用是导出的函数或者class的修改不会体现到原来的模块中
        if (typeof descriptor.value === 'function') {
          obj[key] = descriptor.value.bind(target);
        } else {
          Object.defineProperty(obj, key, {
            get: function () {
              return target[key];
            },
            set: function (value) {
              target[key] = value;
            },
            enumerable: descriptor.enumerable,
            configurable: descriptor.configurable
          });
        }
      }
    }
  }
  function doRequire(target) {
    let m = require(target.__specifier);
    target.__specifier = undefined;
    Object.setPrototypeOf(target, m); //这可能比getter方式快，但不支持在外部设置导出的字段（ts本身也不支持）
    //Object.setPrototypeOf(target, Object.prototype);
    //proxyTo(target, m);
    return m;
  }
  function lazyRequire(specifier, immediate) {
    if (builtinModules.has(specifier)) {
      return builtinModules.get(specifier);
    }
    if (immediate) {
      //console.warn(`load module [${joinAsPosix(requiringDir, specifier)}] immediate`);
      return require(specifier);
    }
    //console.log(`lazy require(${specifier}) by ${referer}`);
    const res = {
      __specifier: specifier
    };
    const proxy = new Proxy(res, {
      get: function (target, name, receiver) {
        if (name === '__esModule') return true;
        //console.log(`proxy for ${name} get`);
        let m = doRequire(target);
        return Reflect.get(m, name, receiver);
      },
      set: function (target, name, value, receiver) {
        //console.log(`proxy for ${name} set`);
        throw new Error(`readonly property ${name}`);
        //let m = doRequire(target);
        //return Reflect.set(m, name, value, receiver);
      }
    });
    Object.setPrototypeOf(res, proxy);
    return res;
  }
  return lazyRequire;
}
function clearModuleCache() {
  exportsCache.clear();
}
function statModuleCache() {
  return exportsCache.stat();
}
function gcModuleCache() {
  return exportsCache.gc();
}
function deleteModuleCache(specifier) {
  return exportsCache.delete(specifier);
}
function hasModuleCache(specifier) {
  return exportsCache.has(specifier);
}
puer.module = {
  createRequire: createLazyRequire,
  clearModuleCache: clearModuleCache,
  statModuleCache: statModuleCache,
  gcModuleCache: gcModuleCache,
  deleteModuleCache: deleteModuleCache,
  hasModuleCache: hasModuleCache
};
        }),"puerts/nodepatch.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

/*
* Tencent is pleased to support the open source community by making Puerts available.
* Copyright (C) 2020 Tencent.  All rights reserved.
* Puerts is licensed under the BSD 3-Clause License, except for the third-party components listed in the file 'LICENSE' which may be subject to their corresponding license terms.
* This file is subject to the terms and conditions defined in file 'LICENSE', which is part of this source code package.
*/

process.on('uncaughtException', e => {
  console.error(e);
});
process.exit = function () {
  console.log('`process.exit` is not allowed in puerts');
};
process.kill = function () {
  console.log('`process.kill` is not allowed in puerts');
};
const customPromisify = require('util').promisify.custom;
Object.defineProperty(setTimeout, customPromisify, {
  enumerable: true,
  get() {
    return function (delay) {
      return new Promise(resolve => setTimeout(resolve, delay));
    };
  }
});
globalThis.setImmediate = function (fn) {
  return setTimeout(fn, 0);
};
globalThis.clearImmediate = function (fn) {
  clearTimeout(fn);
};
        }),"puerts/polyfill.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

/*
* Tencent is pleased to support the open source community by making Puerts available.
* Copyright (C) 2020 Tencent.  All rights reserved.
* Puerts is licensed under the BSD 3-Clause License, except for the third-party components listed in the file 'LICENSE' which may be subject to their corresponding license terms.
* This file is subject to the terms and conditions defined in file 'LICENSE', which is part of this source code package.
*/

var global = global || globalThis || function () {
  return this;
}();
global.process = {
  env: {
    NODE_ENV: 'development'
  }
};
        }),"puerts/promises.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

/*
 * Tencent is pleased to support the open source community by making Puerts available.
 * Copyright (C) 2020 Tencent.  All rights reserved.
 * Puerts is licensed under the BSD 3-Clause License, except for the third-party components listed in the file 'LICENSE' which may be subject to their corresponding license terms. 
 * This file is subject to the terms and conditions defined in file 'LICENSE', which is part of this source code package.
 */

var global = global || globalThis || function () {
  return this;
}();
const kPromiseRejectWithNoHandler = 0;
const kPromiseHandlerAddedAfterReject = 1;
const kPromiseRejectAfterResolved = 2;
const kPromiseResolveAfterResolved = 3;
global.__tgjsSetPromiseRejectCallback(promiseRejectHandler);
global.__tgjsSetPromiseRejectCallback = undefined;
;
const maybeUnhandledRejection = new WeakMap();
function promiseRejectHandler(type, promise, reason) {
  switch (type) {
    case kPromiseRejectWithNoHandler:
      maybeUnhandledRejection.set(promise, {
        reason
      }); //maybe unhandledRejection
      Promise.resolve().then(() => Promise.resolve()) // run after all microtasks
      .then(_ => unhandledRejection(promise, reason));
      break;
    case kPromiseHandlerAddedAfterReject:
      handlerAddedAfterReject(promise);
      break;
    case kPromiseResolveAfterResolved:
      console.error('kPromiseResolveAfterResolved', promise, reason);
      break;
    case kPromiseRejectAfterResolved:
      console.error('kPromiseRejectAfterResolved', promise, reason);
      break;
  }
}
function unhandledRejection(promise, reason) {
  const promiseInfo = maybeUnhandledRejection.get(promise);
  if (promiseInfo === undefined) {
    return;
  }
  maybeUnhandledRejection.delete(promise);
  if (!puer.emit('unhandledRejection', promiseInfo.reason, promise)) {
    unhandledRejectionWarning(reason);
  }
}
function unhandledRejectionWarning(reason) {
  try {
    if (reason instanceof Error) {
      console.warn('unhandledRejection', reason, reason.stack);
    } else {
      console.warn('unhandledRejection', reason);
    }
  } catch {}
}
function handlerAddedAfterReject(promise) {
  const promiseInfo = maybeUnhandledRejection.get(promise);
  if (promiseInfo !== undefined) {
    // cancel
    maybeUnhandledRejection.delete(promise);
  }
}
        }),"puerts/timer.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

/*
* Tencent is pleased to support the open source community by making Puerts available.
* Copyright (C) 2020 Tencent.  All rights reserved.
* Puerts is licensed under the BSD 3-Clause License, except for the third-party components listed in the file 'LICENSE' which may be subject to their corresponding license terms. 
* This file is subject to the terms and conditions defined in file 'LICENSE', which is part of this source code package.
*/

var global = global || globalThis || function () {
  return this;
}();
class PriorityQueue {
  constructor(data = [], compare = (a, b) => a - b) {
    this.data = data;
    this.length = this.data.length;
    this.compare = compare;
    if (this.length > 0) {
      for (let i = (this.length >> 1) - 1; i >= 0; i--) this._down(i);
    }
  }
  push(item) {
    this.data.push(item);
    this.length++;
    this._up(this.length - 1);
  }
  pop() {
    if (this.length === 0) return undefined;
    const top = this.data[0];
    const bottom = this.data.pop();
    this.length--;
    if (this.length > 0) {
      this.data[0] = bottom;
      this._down(0);
    }
    return top;
  }
  peek() {
    return this.data[0];
  }
  _up(pos) {
    const {
      data,
      compare
    } = this;
    const item = data[pos];
    while (pos > 0) {
      const parent = pos - 1 >> 1;
      const current = data[parent];
      if (compare(item, current) >= 0) break;
      data[pos] = current;
      pos = parent;
    }
    data[pos] = item;
  }
  _down(pos) {
    const {
      data,
      compare
    } = this;
    const halfLength = this.length >> 1;
    const item = data[pos];
    while (pos < halfLength) {
      let left = (pos << 1) + 1;
      let best = data[left];
      const right = left + 1;
      if (right < this.length && compare(data[right], best) < 0) {
        left = right;
        best = data[right];
      }
      if (compare(best, item) >= 0) break;
      data[pos] = best;
      pos = left;
    }
    data[pos] = item;
  }
}
const removing_timers = new Set();
const timers = new PriorityQueue([], (a, b) => a.next_time - b.next_time);
let next = 0;
global.__tgjsRegisterTickHandler(timerUpdate);
global.__tgjsRegisterTickHandler = undefined;
function timerUpdate() {
  let now = null;
  while (true) {
    const time = timers.peek();
    if (!time) {
      break;
    }
    if (!now) {
      now = Date.now();
    }
    if (time.next_time <= now) {
      timers.pop();
      if (removing_timers.has(time.id)) {
        removing_timers.delete(time.id);
      } else {
        if (time.timeout) {
          time.next_time = now + time.timeout;
          timers.push(time);
        }
        time.handler(...time.args);
      }
    } else {
      break;
    }
  }
}
global.setTimeout = (fn, time, ...arg) => {
  if (typeof fn !== 'function') {
    throw new Error(`Callback must be a function. Received ${typeof fn}`);
  }
  let t = 0;
  if (time > 0) t = time;
  timers.push({
    id: ++next,
    next_time: t + Date.now(),
    args: arg,
    handler: fn
  });
  return next;
};
global.setInterval = (fn, time, ...arg) => {
  if (typeof fn !== 'function') {
    throw new Error(`Callback must be a function. Received ${typeof fn}`);
  }
  let t = 10;
  if (time != null && time > 10) t = time;
  timers.push({
    id: ++next,
    next_time: t + Date.now(),
    handler: fn,
    args: arg,
    timeout: t
  });
  return next;
};
global.clearInterval = id => {
  removing_timers.add(id);
};
global.clearTimeout = global.clearInterval;
        }),"puerts/websocketpp.mjs": (function(exports, require, module, __filename, __dirname) {
            "use strict";

/*
* Tencent is pleased to support the open source community by making Puerts available.
* Copyright (C) 2020 Tencent.  All rights reserved.
* Puerts is licensed under the BSD 3-Clause License, except for the third-party components listed in the file 'LICENSE' which may be subject to their corresponding license terms.
* This file is subject to the terms and conditions defined in file 'LICENSE', which is part of this source code package.
*/

var global = global || globalThis || function () {
  return this;
}();
const WebSocketPP = global.WebSocketPP;
//global.WebSocketPP = undefined;

class EventTarget {
  constructor() {
    this.listeners = {};
  }
  addEventListener(type, callback) {
    if (!(type in this.listeners)) {
      this.listeners[type] = [];
    }
    this.listeners[type].push(callback);
  }
  removeEventListener(type, callback) {
    if (!(type in this.listeners)) {
      return;
    }
    const stack = this.listeners[type];
    for (let i = 0; i < stack.length; i++) {
      if (stack[i] === callback) {
        stack.splice(i, 1);
        return;
      }
    }
  }
  dispatchEvent(ev) {
    if (!(ev.type in this.listeners)) {
      return true;
    }
    const stack = this.listeners[ev.type].slice();
    for (let i = 0; i < stack.length; i++) {
      stack[i].call(this, ev);
    }
    return !ev.defaultPrevented;
  }
}
const readyStates = ['CONNECTING', 'OPEN', 'CLOSING', 'CLOSED'];
class WebSocket extends EventTarget {
  constructor(url, protocols) {
    super();
    if (protocols) throw new Error('do not support protocols argument');
    this._raw = new WebSocketPP(url);
    this._url = url;
    // !!do not raise exception in handles.
    this._raw.setHandles(() => {
      this._readyState = WebSocket.OPEN;
      this._addPendingEvent({
        type: 'open'
      });
    }, data => {
      this._addPendingEvent({
        type: 'message',
        data: data,
        origin: this._url
      });
    }, (code, reason) => {
      this._cleanup();
      this._addPendingEvent({
        type: 'close',
        code: code,
        reason: reason
      });
    }, err => {
      this._fail(err);
    });
    this._readyState = WebSocket.CONNECTING;
    this._tid = setInterval(() => this._poll(), 1);
    this._pendingEvents = [];
  }
  get url() {
    return this._url;
  }
  get readyState() {
    return this._readyState;
  }
  send(data) {
    if (this._readyState !== WebSocket.OPEN) {
      //throw new Error(`WebSocket is not open: readyState ${this._readyState} (${readyStates[this._readyState]})`);
      this.dispatchEvent({
        type: 'error',
        data: `WebSocket is not open: readyState ${this._readyState} (${readyStates[this._readyState]})`
      }); //dispatchEvent immediately
      return;
    }
    try {
      this._raw.send(data);
    } catch (e) {
      this._fail(e.message);
    }
  }
  _fail(err) {
    this._addPendingEvent({
      type: 'error',
      data: err
    });
    this._cleanup();
    this._addPendingEvent({
      type: 'close',
      code: 1006,
      reason: err
    });
  }
  _cleanup() {
    this._readyState = WebSocket.CLOSING;
  }
  _addPendingEvent(ev) {
    this._pendingEvents.push(ev);
  }
  _poll() {
    if (this._pendingEvents.length === 0 && this._readyState != WebSocket.CLOSING) {
      this._raw.poll();
    }
    const ev = this._pendingEvents.shift();
    if (ev) this.dispatchEvent(ev);
    if (this._pendingEvents.length === 0 && this._readyState == WebSocket.CLOSING || ev && ev.type === 'close') {
      this._raw = undefined;
      clearInterval(this._tid);
      this._readyState = WebSocket.CLOSED;
      this._pendingEvents = [];
    }
  }
  close(code, data) {
    try {
      this._raw.close(code, data);
    } catch (e) {
      this.dispatchEvent({
        type: 'error',
        data: e.message
      }); //dispatchEvent immediately
    }
    this._cleanup();
  }
}
for (let i = 0; i < readyStates.length; i++) {
  Object.defineProperty(WebSocket, readyStates[i], {
    enumerable: true,
    value: i
  });
  Object.defineProperty(WebSocket.prototype, readyStates[i], {
    enumerable: true,
    value: i
  });
}
global.WebSocket = WebSocket;
        })};
    