using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Gameplay Ability/Jump")]
public class GAJump : IGameplayAbility
{
    protected override int VFOnTriggerSuccess()
    {
        if (!mOwner.mState.mTagGrounded) return 1;
        Debug.Log("Jump Triggered");

        Vector3 velocity = mOwner.mState.GetVelocity();
        velocity.y = 0;
        mOwner.mState.SetVelocity(velocity);
        mOwner.mState.ApplyImpulse(new Vector3(0, mOwner.mState.GetJumpVelocity(), 0));
        return 0;
    }
}
