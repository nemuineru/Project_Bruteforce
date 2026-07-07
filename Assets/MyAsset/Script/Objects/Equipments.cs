

//いわゆるMAP上に置かれる装備可能なオブジェクト.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Equipments : MonoBehaviour
{
    //if it is on, set physics to none.
    public bool isTaken = true;


    //hitbox for pickup. 
    [SerializeField]
    internal clssSetting hitBox;

    //hitDef for throw. 
    [SerializeField]
    internal hitDefParams hitDefs;

    // set Entity parent position with use of finding bone names
    public string boneTarget = "hand.R";

    // set Durability for this.
    public float durability = 10.0f;
    
    public float maxDurability = 10.0f;

    //基本的にこれが0になるまでHitDefが設定される. 当たるのは一つだけ.
    //また、Projectileの様に呼び出す.
    public float ThrowTime;

    // Additional StateDef for loading / OverRiding.
    [SerializeField]
    public StateDefListObject statedefList;

    // ...with including Anims.
    [SerializeField]
    public AnimlistObject animlist;

    [SerializeField]
    GameObject DestroyEffs;

    [SerializeField]
    List<GameObject> enableOnTaken;

    [SerializeField]
    public Sprite GUIImage; 

    [SerializeField]
    public Color color;

    int collderNum = 0;


    public Rigidbody rb;

    void OnEnable()
    {
        hitBox.initClss(transform);
        foreach(clssDef clss in hitBox.clssDefs)
        {
            clss.drawColor = Color.cyan;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        //if not physic is updated, set kinematically off
        rb.isKinematic = !isTaken;
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
        //投げられているときのみProjの様に.
        if(ThrowTime > 0f)
        {
            if(gameState.self.ProvokeHitDef(hitDefs, ref collderNum ,hitBox))
            {
                ThrowTime = 0f;
            }
            ThrowTime -= Time.fixedDeltaTime;
        }
    }

    void invokeDestroy()
    {
        Instantiate(DestroyEffs,transform.position, Quaternion.identity);
    }
}