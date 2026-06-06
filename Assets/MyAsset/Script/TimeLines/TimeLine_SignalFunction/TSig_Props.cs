using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TSig_Props : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField]
    Vector3 mVec;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetRandomsVect()
    {
        rb.velocity = mVec;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
