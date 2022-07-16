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

    // Values for storing states or temps
    Rigidbody mRigidbody;
    CapsuleCollider mCollider;
    float mInitialDrag;

    // Environment live updates
    public RaycastHit mGroundCheckHit { get; private set; }

    // Player tags
    [SerializeField] public bool mTagGrounded { get; private set; }

    public void OnStartInit(IGameplayEntity gameplayEntity)
    {
        mRigidbody = gameplayEntity.GetComponent<Rigidbody>();
        mCollider = gameplayEntity.GetComponent<CapsuleCollider>();

        mInitialDrag = mRigidbody.drag;
        mRigidbody.freezeRotation = true;
        mTagGrounded = false;
    }

    public void OnFixedUpdate()
    {
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

        if (mTagGrounded)
        {
            // Apply "gravity" to the object to have it stick to the surface
            mRigidbody.AddForce(-groundHit.normal * mRigidbody.mass * mGravity);
        }
        else
        {
            // Apply real gravity
            mRigidbody.AddForce(Vector3.down * mRigidbody.mass * mGravity);
        }

        Debug.Log(groundedThisFrame);
    }
    public event Action<RaycastHit> onStartLanding;
    public void OnStartLanding(RaycastHit landingPosition)
    {
        Debug.Log("Landed");
        mRigidbody.drag = mInitialDrag;
        if (onStartLanding != null) onStartLanding(landingPosition);
    }

    public event Action<RaycastHit> onStartAiring;
    public void OnStartAiring(RaycastHit landingPosition)
    {
        Debug.Log("Airing");

        mRigidbody.drag = 0;
        if (onStartLanding != null) onStartAiring(landingPosition);
    }


    public Vector3 GetVelocity() { return mRigidbody.velocity; }
    public float GetJumpVelocity() { return mJumpVelocity; }
    public event Action<Vector3> onApplyForce;
    public void ApplyForce(Vector3 force)
    {
        //mRigidbody.velocity = force;
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
