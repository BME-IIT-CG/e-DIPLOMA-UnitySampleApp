using UnityEngine;
using Aws.GameLift.Server;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;

public class GameLiftServerManager : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;

    private NetworkRunner _networkRunner;

#if UNITY_SERVER
    private void Start()
    {
        
        var initSDKOutcome = GameLiftServerAPI.InitSDK();
        if (initSDKOutcome.Success)
        {
            ProcessParameters processParameters = new ProcessParameters(
                OnStartGameSession,
                OnUpdateGameSession,
                OnProcessTerminate,
                OnHealthCheck,
                GetPort(),
                new LogParameters(new List<string>()
                {                    
                    "/local/game/logs/myserver.log"
                }));

            var processReadyOutcome = GameLiftServerAPI.ProcessReady(processParameters);
            if (processReadyOutcome.Success)
            {
                print("GameLift: ProcessReady success.");
            }
            else
            {
                print("GameLift: ProcessReady failure : " + processReadyOutcome.Error.ToString());
            }
        }
        else
        {
            print("GameLift: InitSDK failure : " + initSDKOutcome.Error.ToString());
        }
    }

    private void OnDestroy()
    {
        GameLiftServerAPI.Destroy();
    }

    #region GAMELIFT_CALLBACKS

    //When a game session is created, GameLift sends an activation request to the game server and passes along the game session object containing game properties and other settings.
    //Here is where a game server should take action based on the game session object.
    //Once the game server is ready to receive incoming player connections, it should invoke GameLiftServerAPI.ActivateGameSession()
    private void OnStartGameSession(Aws.GameLift.Server.Model.GameSession gameSession)
    {   
        //CreatePhotonSession(gameSession.GameSessionId, gameSession.MaximumPlayerSessionCount, gameSession.Port);
        // create photon session and start game
        NetworkRunnerStart(GameMode.Server);
        // TODO: initialize scene for new session
        
        GameLiftServerAPI.ActivateGameSession();

        Debug.Log($"GameLift: Session activated. Game session ID: {gameSession.GameSessionId}, port: {gameSession.Port}, player count: {gameSession.MaximumPlayerSessionCount}.");
    }

    //When a game session is updated, GameLiftsends a request to the game
    //server containing the updated game session object.  The game server can then examine the provided
    //matchmakerData and handle new incoming players appropriately.
    //updateReason is the reason this update is being supplied.
    private void OnUpdateGameSession(Aws.GameLift.Server.Model.UpdateGameSession updateGameSession)
    {
    }

    //OnProcessTerminate callback. GameLift will invoke this callback before shutting down an instance hosting this game server.
    //It gives this game server a chance to save its state, communicate with services, etc., before being shut down.
    //In this case, we simply tell GameLift we are indeed going to shutdown.
    private void OnProcessTerminate()
    {        
        GameLiftServerAPI.ProcessEnding();
    }

    //This is the HealthCheck callback.
    //GameLift will invoke this callback every 60 seconds or so.
    //Here, a game server might want to check the health of dependencies and such.
    //Simply return true if healthy, false otherwise.
    //The game server has 60 seconds to respond with its health status. GameLift will default to 'false' if the game server doesn't respond in time.
    //In this case, we're always healthy!
    private bool OnHealthCheck()
    {        
        return true;
    }
    #endregion

    #region DEDICATED_SERVER_TODO
    private int GetPort()
    {
        // TODO: get listening port from command-line argument.
        return 7777;
    }

    private void CreatePhotonSession(string gameSessionId, int maxPlayers, int port)
    {
        /* TODO: create Photon session.
         * 
         * NetworkRunner.StartGame(new StartGameArgs() {
         *   GameMode = GameMode.Server,
         *   Address = NetAddress.Any(port)
         *   SessionName = gameSessionId,
         *   PlayerCount = maxPlayers,
         *   ...
         * });
         */
    }

    private void AuthenticateClient(string playerSessionId)
    {
        // AcceptPlayerSession() checks if the playerSessionId (sent by the client) is valid.
        Aws.GameLift.GenericOutcome result = GameLiftServerAPI.AcceptPlayerSession(playerSessionId);

        if (result.Success)
        {
            // TODO: accept and spawn player, set nickname optionally.

            // User the following code to get the name of the accepted player:
            var describePlayerSessionsResult = GameLiftServerAPI.DescribePlayerSessions(new Aws.GameLift.Server.Model.DescribePlayerSessionsRequest { 
                PlayerSessionId = playerSessionId,
            });

            if (describePlayerSessionsResult.Success)
            {
                var playerSessionList = describePlayerSessionsResult.Result.PlayerSessions;
                if (playerSessionList.Count >= 1) 
                {
                    string playerNickname = playerSessionList[0].PlayerData;
                }
            }
        }
        else
        {
            // TODO: invalid playerSessionId, authentication failed, kick client from dedicated server, remove from Photon session.
        }
    }

    private void EndGameSession()
    {
        GameLiftServerAPI.ProcessEnding();
    }
    #endregion
    
#else
    private void Start()
    {
        NetworkRunnerStart(GameMode.Client);
    }
#endif
    void NetworkRunnerStart(GameMode gameMode)
    {
        _networkRunner = Instantiate(networkRunnerPrefab);
        _networkRunner.name = "Network runner";
        var clientTask = InitNetworkRunner(_networkRunner, gameMode, NetAddress.Any(),
            SceneManager.GetActiveScene().buildIndex, null);
        Debug.Log("Server NetworkRunner started");
    }

    Task InitNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress address,
        SceneRef scene, Action<NetworkRunner> initialized)
    {
        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        if (sceneManager == null)
        {
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = address,
            Scene = scene,
            SessionName = "TestRoom",
            Initialized = initialized,
            SceneManager = sceneManager
        });
    }
}
    