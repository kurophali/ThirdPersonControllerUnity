using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay Ability/GAMove")]
public class GAMove : IGameplayAbility
{
    Vector4 mLastTriggerVector = new Vector4();
    protected override int VFTriggerCheckForSend(Vector4 triggerVector)
    {
        if (triggerVector == mLastTriggerVector) return 1;
        return 0;
    }

    protected override int VFOnTriggerSuccess()
    {
        mLastTriggerVector = mTriggerVector;
        if (mTriggerVector.magnitude != 0) mOwner.mState.SetForward(mTriggerVector);
        return 0;
    }

    protected override int VFOnFixedUpdateRegular()
    {
        if (!mOwner.mState.mTagGrounded) return 1;
        Vector3 horizontalForceFromCam = mLastTriggerVector.normalized;
        Vector3 groundNormal = mOwner.mState.mGroundCheckHit.normal;
        Vector3 force = Vector3.ProjectOnPlane(horizontalForceFromCam, groundNormal).normalized;
        force = force * mOwner.mState.GetSpeed();
        mOwner.mState.ApplyForce(force);
        return 0;
    }


}
