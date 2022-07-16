using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject mCharacterPrefab;
    [SerializeField] CinemachineVirtualCamera vCamThirdPerson;
    
    IGameplayEntity mCharacterInstance;

    PlayerInput mPlayerInput;
    InputAction mDirectionalInputAction, mJump;

    Vector2 mDirectionalInput;

    // Start is called before the first frame update
    void Start()
    {
        mCharacterInstance = Instantiate(mCharacterPrefab).GetComponent<IGameplayEntity>();
        vCamThirdPerson.LookAt = mCharacterInstance.transform;
        vCamThirdPerson.Follow = mCharacterInstance.transform;

        mPlayerInput = GetComponent<PlayerInput>();
        mDirectionalInputAction = mPlayerInput.actions["movement"];
        mJump = mPlayerInput.actions["jump"];
        mJump.performed += _ => Jump();
    }

    void Jump()
    {
        Vector2 directionalInput = mDirectionalInput;
        Vector3 verticalInput = Camera.main.transform.forward * mDirectionalInput.y;
        verticalInput.y = 0;
        Vector3 horizontalInput = Camera.main.transform.right * mDirectionalInput.x;
        horizontalInput.y = 0;
        Vector3 directionInput = verticalInput + horizontalInput;
        mCharacterInstance.TriggerAbility(1, new Vector4(directionInput.x, directionInput.y, directionInput.z, 0));
    }

    private void Update()
    {
        mDirectionalInput = mDirectionalInputAction.ReadValue<Vector2>();
        Vector3 verticalInput = Camera.main.transform.forward * mDirectionalInput.y;
        verticalInput.y = 0;
        Vector3 horizontalInput = Camera.main.transform.right * mDirectionalInput.x;
        horizontalInput.y = 0;
        Vector3 directionInput = verticalInput + horizontalInput;
        mCharacterInstance.TriggerAbility(0, new Vector4(directionInput.x, directionInput.y, directionInput.z, 0));        
    }
}
