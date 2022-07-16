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
    InputAction mMovements, mJump;

    // Start is called before the first frame update
    void Start()
    {
        mCharacterInstance = Instantiate(mCharacterPrefab).GetComponent<IGameplayEntity>();
        vCamThirdPerson.LookAt = mCharacterInstance.transform;
        vCamThirdPerson.Follow = mCharacterInstance.transform;

        mPlayerInput = GetComponent<PlayerInput>();
        mMovements = mPlayerInput.actions["movement"];
        mJump = mPlayerInput.actions["jump"];
        mJump.performed += _ => Jump();
    }

    void Jump()
    {
        mCharacterInstance.TriggerAbility(1, new Vector4(0,0,0,0));
    }

    private void Update()
    {
        Vector2 movementInput = mMovements.ReadValue<Vector2>();
        Vector3 verticalInput = Camera.main.transform.forward * movementInput.y;
        verticalInput.y = 0;
        Vector3 horizontalInput = Camera.main.transform.right * movementInput.x;
        horizontalInput.y = 0;
        Vector3 directionInput = verticalInput + horizontalInput;
        mCharacterInstance.TriggerAbility(0, new Vector4(directionInput.x, directionInput.y, directionInput.z, 0));
    
        
    }
}
