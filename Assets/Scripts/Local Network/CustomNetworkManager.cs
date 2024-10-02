using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjectController GamePlayerPrefab;
    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();

    [SerializeField] private GameObject tileManagerObject;


    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if(SceneManager.GetActiveScene().name == "Lobby")
        {
            PlayerObjectController GamePlayerInstance = Instantiate(GamePlayerPrefab);

            GamePlayerInstance.ConnectionID = conn.connectionId;
            GamePlayerInstance.PlayerIdNumber = GamePlayers.Count + 1;
            GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.CurrentLobbyID, GamePlayers.Count);

            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
        }
    }

   public void StartGame(string SceneName)
   {
        ServerChangeScene(SceneName);
   }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        if (sceneName == LobbyController.Instance.GameScene && tileManagerObject != null)
        {
            Debug.Log("Server active: " + NetworkServer.active);
            GameObject tileManagerObjectInstance = Instantiate(tileManagerObject);
            NetworkServer.Spawn(tileManagerObjectInstance);
            // 给新生成的tileManagerObjectInstance赋值tileManager
            TileManager tileManager = tileManagerObjectInstance.GetComponent<TileManager>();
            if (tileManager != null)
            {
                tileManager.InitializeTilesForNetwork();
            }
        }
    }
}
