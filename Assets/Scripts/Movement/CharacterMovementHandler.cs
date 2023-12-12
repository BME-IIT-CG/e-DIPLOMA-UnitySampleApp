using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CharacterMovementHandler : NetworkBehaviour
{
    private NetworkCharacterControllerPrototypeCustom _networkCharacterControllerPrototypeCustom;

    private void Awake()
    {
        _networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
    }

    public override void FixedUpdateNetwork()
    {
        // server moves thing, server simulates everything!
        // get input from client

        if (GetInput(out NetworkInputData networkInputData))
        {
            // move character
            Vector3 moveDirection = transform.forward * networkInputData.movementInput.y +
                                    transform.right * networkInputData.movementInput.x;
            moveDirection.Normalize();
            
            _networkCharacterControllerPrototypeCustom.Move(moveDirection);
        }
    }
}
