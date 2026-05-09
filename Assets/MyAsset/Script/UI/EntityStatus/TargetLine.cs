using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityQuaternion;
using UnityEngine;

public class TargetLine : MonoBehaviour
{
    [SerializeField]
    GameObject NearestSet;
    
    [SerializeField]
    GameObject NonHit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward,Camera.main.transform.up);
        if(Vector3.Magnitude(gameState.self.Player.transform.position - transform.position) < 4.0f)
        {
            NearestSet.SetActive(true);
            NonHit.SetActive(false);
            NearestSet.transform.localRotation *= Quaternion.Euler(0,0,300.0f * Time.deltaTime);
        }
        else
        {
            NearestSet.SetActive(false);
            NonHit.SetActive(true);
        }
    }
}
