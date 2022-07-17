using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Gameplay Ability/Jump")]
public class GAJump : IGameplayAbility
{
    protected override int VFOnTriggerSuccess()
    {
        //if (!mOwner.mState.mTagGrounded && !mOwner.mState.mStickToWall) return 1; // check states in the GA

        if (mOwner.mState.mTagAttachedToWall)
        {
            // Jump against the wall a.k.a along the normal from the closest hitpoint surface.
            Vector3 jumpDirection = mOwner.mState.mWallCheckHit.normal;
            mOwner.mState.ApplyImpulse(mOwner.mState.GetJumpVelocity() * jumpDirection);
            mOwner.mState.OnDetachWall();

            return 0;
        }
        else if (mOwner.mState.mTagGrounded)
        {
            // Jump only at input directions from the ground
            Vector3 velocity = mOwner.mState.GetVelocity();
            velocity.y = 0;
            mOwner.mState.SetVelocity(velocity);
            mOwner.mState.ApplyImpulse(new Vector3(0, mOwner.mState.GetJumpVelocity(), 0));
            return 0;
        }

        return 1;
    }
}
