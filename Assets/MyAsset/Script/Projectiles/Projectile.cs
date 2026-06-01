using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //those 2s are essential

    [SerializeField]
    public clssDef clssDef;
    public Vector3 Velocity;
    public Vector3 Accel;


    public Vector3 CustomVectorParam;

    public float RemainTime = 1.0f;

    public GameObject EffectObject;

    //
    public Entity proj_Controller;
    public Entity proj_Target;

    [SerializeField]
    public hitDefParams hitDefParams;

    internal Rigidbody rb;

    
    //当たり判定 - 
    internal clssSetting cSet = new clssSetting();
    // Start is called before the first frame update
    void Start()
    {
        clssDef.initTransform(this.transform);
        cSet.clssDefs.Add(clssDef);
        rb = GetComponent<Rigidbody>();
        OnEmit();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        cSet.clssPosUpdate();
        RemainTime -= Time.fixedDeltaTime;
        OnProjUpdation();
        if (RemainTime < 0)
        {
            destroyEmit();
        }
        HitUpdate();
    }


    virtual internal void OnEmit()
    {
    }

    virtual internal void OnProjUpdation()
    {
    }

    virtual internal void HitUpdate()
    {
    }
    virtual internal void destroyEmit()
    {
        Destroy(gameObject);
    }

}
