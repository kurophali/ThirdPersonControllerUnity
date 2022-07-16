using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGameplayAbility : ScriptableObject
{
    protected IGameplayEntity mOwner;
    protected Vector4 mTriggerVector = new Vector4();

    protected virtual int VFOnStartInit() { return 0; }
    protected virtual int VFOnTriggerSuccess() { return 0; }
    protected virtual int VFOnFixedUpdateRegular() { return 0; }
    protected virtual int VFTriggerCheckForSend(Vector4 triggerVector) { return 0; }

    public void Trigger(Vector4 triggerVector)
    {
        // Client side check
        if (VFTriggerCheckForSend(triggerVector) != 0) return; 

        // Usually this block calls GE.CmdTrigger with server side check if networked
        // ...

        // Both check successed
        mTriggerVector = triggerVector;
        VFOnTriggerSuccess();
    }

    public void OnStartInit(IGameplayEntity owner)
    {
        mOwner = owner;
        VFOnStartInit();
    }

    public void OnFixedUpdate()
    {
        VFOnFixedUpdateRegular();
    }
}

