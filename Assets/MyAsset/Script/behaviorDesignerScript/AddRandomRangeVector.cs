using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.Playables;

[TaskCategory("MyAsset")]
public class AddRandomRangeVector : Action
{
    [SerializeField]
    SharedVector2 Input;

    [SerializeField]
    SharedFloat RandomRange = 1.0f;

    public override void OnAwake()
    {
        
    }

    public override void OnStart()
    {
        
    }
}
