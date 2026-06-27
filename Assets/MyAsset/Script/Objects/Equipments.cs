//いわゆるMAP上に置かれる装備可能なオブジェクト.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Equipments : MonoBehaviour
{
    //if it is on, set physics to none.
    public bool setPhysics = true;


    //hitbox for pickup. 
    [SerializeField]
    internal clssSetting hitBox;

    // set Entity parent position with use of finding bone names
    public string boneTarget = "hand.R";

    // set Durability for this.
    public float durability = 10.0f;

    // Additional StateDef for loading / OverRiding.
    [SerializeField]
    public StateDefListObject statedefList;

    // ...with including Anims.
    [SerializeField]
    public AnimlistObject animlist;

    [SerializeField]
    GameObject DestroyEffs;

    void OnEnable()
    {
        hitBox.initClss(transform);
        foreach(clssDef clss in hitBox.clssDefs)
        {
            clss.drawColor = Color.cyan;
        }
    }

    void FixedUpdate()
    {
        hitBox.clssPosUpdate();
        foreach(clssDef clss in hitBox.clssDefs)
        {
            clss.getGlobalPos();
            clss.DrawCapsule();
        }
        //耐久値0なら壊れる
        if (durability <= 0)
        {
            invokeDestroy();
        }
    }

    void invokeDestroy()
    {
        Instantiate(DestroyEffs,transform.position, Quaternion.identity);
    }
}

