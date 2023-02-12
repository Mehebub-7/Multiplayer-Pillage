using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public static BasicSpawner Instance;
    [SerializeField] Transform[] spawnPoint;
    public int spawnIndex;
    [SerializeField] Transform[] waypoints;
    private NetworkRunner _runner;
    [SerializeField] private NetworkPrefabRef[] _playerPrefabs;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    [SerializeField] DiceHolder[] diceHolder;
    public int currentPlayerIndex;

    private void Awake()
    {
        Instance = this;
    }
    private void OnGUI()
    {
        if (_runner == null)
        {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
            {
                StartGame(GameMode.Host);
            }
            if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
            {
                if (spawnIndex < 3)
                    StartGame(GameMode.Client);
            }
        }
    }
    async void StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            // Create a unique position for the player
            //Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.DefaultPlayers) * 3, 1, 0);
            Vector3 spawnPosition = spawnPoint[spawnIndex].position;
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefabs[spawnIndex], spawnPosition, Quaternion.identity, player);
            // Keep track of the player avatars so we can remove it when they disconnect
            _spawnedCharacters.Add(player, networkPlayerObject);
            Piece tempPlayer = networkPlayerObject.GetComponent<Piece>();
            for (int i = 0; i < waypoints[spawnIndex].childCount; i++)
            {
                tempPlayer.waypoints.Add(waypoints[spawnIndex].GetChild(i));
            }
            tempPlayer.playerIndex = spawnIndex;
            diceHolder[spawnIndex].diceIndex = spawnIndex;
            spawnIndex++;
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        // Find and remove the players avatar
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Input.GetMouseButtonDown(0))
        {
            data.isDown = true;
            data.mousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            data.isDown = false;
        }
        input.Set(data);
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {}

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {}

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {}

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {}

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {}

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {}

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {}

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {}

    public void OnSceneLoadDone(NetworkRunner runner)
    {}

    public void OnSceneLoadStart(NetworkRunner runner)
    {}

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {}

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {}

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {}
}
