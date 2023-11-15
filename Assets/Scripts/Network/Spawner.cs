using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkPlayer playerPrefab;

    private CharacterInputHandler _characterInputHandler;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // spawn a player after connected
        if (runner.IsServer)
        {
            Debug.Log("Player" + player.PlayerId + "has joined, hosting server.");
            runner.Spawn(playerPrefab, Utils.GetRandomSpawnPoint(), Quaternion.identity, player);
        }
        else
        {
            Debug.Log("Player" + player.PlayerId + " has joined, connected to server.");
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Player" + player.PlayerId + " has left");
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        // check if we actually get the character controller for the correct player
        if (_characterInputHandler == null && NetworkPlayer.Local != null)
        {
            // we actually have a local network player!
            _characterInputHandler = NetworkPlayer.Local.GetComponent<CharacterInputHandler>();
        }

        if (_characterInputHandler != null)
        {
            input.Set(_characterInputHandler.GetNetworkInput());
        }
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        Debug.Log("Input is missing");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("Server is shutting down");
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to server");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("Disconnected from server");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log("Connect request incoming");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("Connection failed");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        Debug.Log("UserSimulationMessage");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("Session list updated");
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        Debug.Log("CustomAuthenticationResponse");
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("New host");
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        Debug.Log("Data received");
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("Scene load done");
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("Scene load started");
    }
}
