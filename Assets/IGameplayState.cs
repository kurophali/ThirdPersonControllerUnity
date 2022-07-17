using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Gameplay State")]
public class IGameplayState : ScriptableObject
{
    // Attributes should be set here even if you use it in GA.
    // Because client check uses a prefab instance.
    [SerializeField] float mSpeed;
    [SerializeField] float mJumpVelocity;
    [SerializeField] float mGravity;
    [SerializeField] float mGroundCheckRaycastLength;
    [SerializeField] float mMaxDistForWallDetection;

    // Values for storing states or temps
    Rigidbody mRigidbody;
    CapsuleCollider mCollider;
    float mInitialDrag;
    bool mTagAllowWallcheck;
    IGameplayEntity mGameplayEntity; // ??? Should I merge IGS into IGE?

    // Environment live updates
    public RaycastHit mGroundCheckHit { get; private set; }
    public RaycastHit mWallCheckHit { get; private set; }
    // Player tags
    [SerializeField] public bool mTagGrounded { get; private set; }
    public bool mTagAttachedToWall { get; private set; }

    public void OnStartInit(IGameplayEntity gameplayEntity)
    {
        mGameplayEntity = gameplayEntity;
        mRigidbody = gameplayEntity.GetComponent<Rigidbody>();
        mCollider = gameplayEntity.GetComponent<CapsuleCollider>();

        mInitialDrag = mRigidbody.drag;
        mRigidbody.freezeRotation = true;
        mTagGrounded = false;
        mTagAllowWallcheck = true;
    }

    IEnumerator WaitForSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        mTagAllowWallcheck = true;
    }

    public void TurnOffWallCheck(float durationInSeconds)
    {
        mTagAllowWallcheck = false;
        if (mTagAttachedToWall)
        {
            mTagAttachedToWall = false;
            OnDetachWall();
        }
        mGameplayEntity.StartCoroutine(WaitForSeconds(durationInSeconds));
    }

    public void OnFixedUpdate()
    {
        #region GROUNDCHECK_AND_GRAVITY
        RaycastHit groundHit;
        bool hit = Physics.Raycast(mRigidbody.transform.position, Vector3.down, out groundHit);
        mGroundCheckHit = groundHit;

        bool groundedThisFrame = Vector3.Magnitude(groundHit.point - mRigidbody.position) < mGroundCheckRaycastLength;
 
        if (mTagGrounded != groundedThisFrame) // Changed air status
        {
            mTagGrounded = groundedThisFrame;

            if (mTagGrounded) 
            {
                OnStartLanding(groundHit);
            }
            else
            {
                OnStartAiring(groundHit);
            }
        }

        //Debug.Log(mStickToWall);
        if (mTagGrounded)
        {
            // Apply "gravity" to the object to have it stick to the surface
            //mRigidbody.AddForce(-groundHit.normal * mRigidbody.mass * mGravity);
        }
        else if(!mTagAttachedToWall) // no gravity if sticked to wall
        {
            // Apply real gravity
            mRigidbody.AddForce(Vector3.down * mRigidbody.mass * mGravity);
        }
        #endregion

        #region WALLCHECK
        if (!mTagGrounded && mTagAllowWallcheck)
        {
            RaycastHit frontDetection, backDetection, leftDetection, rightDetection;
            float frontCollisionDist = 0, backCollisionDist = 0, leftCollisionDist = 0, rightCollisionDist = 0;
            bool hitFront = Physics.Raycast(mRigidbody.transform.position, mRigidbody.transform.forward, out frontDetection);
            bool hitBack = Physics.Raycast(mRigidbody.transform.position, -mRigidbody.transform.forward, out backDetection);
            bool hitRight = Physics.Raycast(mRigidbody.transform.position, mRigidbody.transform.right, out rightDetection);
            bool hitLeft = Physics.Raycast(mRigidbody.transform.position, -mRigidbody.transform.right, out leftDetection);
            float closestDistanceToWall = mMaxDistForWallDetection;

            if (hitFront)
            {
                Vector3 objectToHitpoint = frontDetection.point - mRigidbody.transform.position;
                frontCollisionDist = Vector3.Magnitude(objectToHitpoint);

                // Get the actual distance from wall from this hitpoint
                Vector3 normal = frontDetection.normal;
                float cosine = Vector3.Dot(objectToHitpoint, -normal);
                float distToWall = cosine * frontCollisionDist;
                if (distToWall < closestDistanceToWall)
                {
                    closestDistanceToWall = distToWall;
                    mWallCheckHit = frontDetection;
                }
            }
            if (hitBack)
            {
                Vector3 objectToHitpoint = backDetection.point - mRigidbody.transform.position;
                backCollisionDist = Vector3.Magnitude(objectToHitpoint);

                // Get the actual distance from wall from this hitpoint
                Vector3 normal = backDetection.normal;
                float cosine = Vector3.Dot(objectToHitpoint, -normal);
                float distToWall = cosine * backCollisionDist;
                if (distToWall < closestDistanceToWall)
                {
                    closestDistanceToWall = distToWall;
                    mWallCheckHit = backDetection;
                }
            }
            if (hitLeft)
            {
                Vector3 objectToHitpoint = leftDetection.point - mRigidbody.transform.position;
                leftCollisionDist = Vector3.Magnitude(objectToHitpoint);

                // Get the actual distance from wall from this hitpoint
                Vector3 normal = leftDetection.normal;
                float cosine = Vector3.Dot(objectToHitpoint, -normal);
                float distToWall = cosine * leftCollisionDist;
                if (distToWall < closestDistanceToWall)
                {
                    closestDistanceToWall = distToWall;
                    mWallCheckHit = leftDetection;
                }
            }
            if (hitRight)
            {
                Vector3 objectToHitpoint = rightDetection.point - mRigidbody.transform.position;
                rightCollisionDist = Vector3.Magnitude(objectToHitpoint);

                // Get the actual distance from wall from this hitpoint
                Vector3 normal = rightDetection.normal;
                float cosine = Vector3.Dot(objectToHitpoint, -normal);
                float distToWall = cosine * rightCollisionDist;
                if (distToWall < closestDistanceToWall)
                {
                    closestDistanceToWall = distToWall;
                    mWallCheckHit = rightDetection;
                }
            }

            bool attachedOnFrame = closestDistanceToWall < mMaxDistForWallDetection;

            if (!mTagAttachedToWall && attachedOnFrame) // Just attached to wall
            {
                OnAttachWall(mWallCheckHit);
                mTagAttachedToWall = attachedOnFrame;
            }

            //if(mTagAttachedToWall != stickedThisFrame)
            //{
            //    mTagAttachedToWall = stickedThisFrame;
            //    if (mTagAttachedToWall)
            //    {
            //        OnAttachWall(mWallCheckHit);
            //    }
            //    else
            //    {
            //        OnDetachWall();
            //    }
            //}
        }
        else
        {
            mTagAttachedToWall = false;
        }

        // State management for FSM???
        if(mTagAttachedToWall)
        {
            // Some states that will exit this state
            if (!mTagAllowWallcheck || mTagGrounded) 
            {
                mTagAttachedToWall = false;
                OnDetachWall();
            }
        }
        #endregion


    }

    public event Action<RaycastHit> onStickToWall;
    public void OnAttachWall(RaycastHit wallDetection)
    {
        Debug.Log("OnAttachWall");

        mRigidbody.velocity = Vector3.zero;
        mRigidbody.drag = mInitialDrag;
        mRigidbody.useGravity = false;
        
        if (onStickToWall != null) onStickToWall(wallDetection);
    }

    public void OnDetachWall()
    {
        Debug.Log("OnDetachWall");
        mRigidbody.drag = 0;
        TurnOffWallCheck(0.5f);
    }

    public event Action<RaycastHit> onStartLanding;
    public void OnStartLanding(RaycastHit landDetection)
    {
        Debug.Log("Landed");
        mRigidbody.drag = mInitialDrag;
        if (onStartLanding != null) onStartLanding(landDetection);
    }

    public event Action<RaycastHit> onStartAiring;
    public void OnStartAiring(RaycastHit landDetection)
    {
        Debug.Log("Airing");

        mRigidbody.drag = 0;
        if (onStartLanding != null) onStartAiring(landDetection);
    }


    public Vector3 GetVelocity() { return mRigidbody.velocity; }
    public float GetJumpVelocity() { return mJumpVelocity; }
    public event Action<Vector3> onApplyForce;
    public void ApplyForce(Vector3 force)
    {
        mRigidbody.AddForce(force * 10, ForceMode.Force);

        if (onApplyForce != null) onApplyForce(force);
    }

    public event Action<Vector3> onSetVelocity;
    public void SetVelocity(Vector3 velocity)
    {
        mRigidbody.velocity = velocity;
        if(onSetVelocity != null) onSetVelocity(velocity);
    }

    public event Action<Vector3> onSetForward;
    public void SetForward(Vector3 forward)
    {
        mRigidbody.transform.forward = forward;
        if (onSetForward != null) onSetForward(forward);
    }

    public float GetSpeed()
    {
        return mSpeed;
    }

    public float GetJumpForce()
    {
        return mJumpVelocity;
    }

    public void ApplyImpulse(Vector3 impulse)
    {
        mRigidbody.AddForce(impulse, ForceMode.Impulse);
    }
}
