using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    // could use bits instead vectors, because they create more demand on the network

    public Vector2 movementInput;
    public float rotationInput;
    public NetworkBool isJumpPressed;
}
