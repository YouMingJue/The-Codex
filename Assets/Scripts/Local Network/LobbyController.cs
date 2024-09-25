using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance;

    //UI Elements
    public Text LobbyNameText;
    public Text CountdownText;

    //Player Data
    public GameObject teamAPlayerListViewContent;
    //public Button teamAChooseButton;
public GameObject teamBPlayerListViewContent;
//public Button teamBChooseButton;
    public GameObject playerListItemPrefab;
    public GameObject LocalPlayerObject;

    private Dictionary<Team, List<PlayerObjectController>> teamPlayers = new Dictionary<Team, List<PlayerObjectController>>()
    {
        { Team.TeamA, new List<PlayerObjectController>() },
        { Team.TeamB, new List<PlayerObjectController>() }
    };

    //Other Data
    public ulong CurrentLobbyID;
    public bool PlayerItemCreated = false;
    private List<PlayerListItem> PlayerListItems = new List<PlayerListItem>();
    public PlayerObjectController LocalplayerController;

    //Ready
    public Button StartGameButton;
    public Text ReadyButtonText;
    public string GameScene;
    public float countdownDuration = 5.0f;
    private float countdownTimer;
    private bool IsCountingDown = false;



    //Manager
    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Awake()
    {
        if(Instance == null) { Instance = this; }
        manager = CustomNetworkManager.singleton as CustomNetworkManager;
    }

   private void Start()
    {
        AutoAssignTeams();
        //UpdateTeamUI();
    }

  

    private void Update()
    {
        if (IsCountingDown)
        {
            countdownTimer -= Time.deltaTime; 
            UpdateCountdownText(); 

            if (countdownTimer <= 0f)
            {
                IsCountingDown = false; 
                StartGame(GameScene);
            }
        }
    }

    public void UpdateCountdownText()
    {
        if (countdownTimer < 0)
        {
            return;
        }
        int seconds = (int)countdownTimer;
        string timeLeft = seconds.ToString();
        CountdownText.text = "Start in " + timeLeft + " seconds";
    }

    public void ReadyPlayer()
    {
        LocalplayerController.ChangeReady();
        //AutoAssignTeams();
        //UpdateTeamUI();
    }

    public void UpdateButton()
    {
        if (LocalplayerController.Ready)
        {
            ReadyButtonText.text = "Unready";
        }
        else
        {
            ReadyButtonText.text = "Ready";
        }
    }

    public void CheckIfAllReady()
    {
        if (LocalplayerController == null)
        {
            Debug.LogError("LocalplayerController is not initialized.");
            return;
        }

        bool allReady = Manager.GamePlayers.All(player => player.Ready);

        if (allReady)
        {
            if (LocalplayerController.PlayerIdNumber == 1)
            {
                StartGameButton.interactable = false; 
                if (countdownTimer <= 0f && !IsCountingDown) 
                {
                    countdownTimer = countdownDuration;
                    IsCountingDown = true;
                    CountdownText.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            StartGameButton.interactable = false; 
            IsCountingDown = false; 
            countdownTimer = 0f;
            CountdownText.gameObject.SetActive(false);
        }
    }


    public void UpdateLobbyName()
    {
        CurrentLobbyID = Manager.GetComponent<SteamLobby>().CurrentLobbyID;
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "name");
    }

    public void UpdatePlayerList()
    {
        if(!PlayerItemCreated) { CreateHostPlayerItem(); } //Host
        if(PlayerListItems.Count < Manager.GamePlayers.Count) { CreateClientPlayerItem();}
        if(PlayerListItems.Count > Manager.GamePlayers.Count) { RemovePlayerItem(); }
        if(PlayerListItems.Count == Manager.GamePlayers.Count) { UpdatePlayerItem(); }

        AutoAssignTeams();
        //UpdateTeamUI();
    }

    public void FindLocalPlayer()
    {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");
        if (LocalPlayerObject != null)
        {
            LocalplayerController = LocalPlayerObject.GetComponent<PlayerObjectController>();
        }
        else
        {
            Debug.LogError("Local player object not found.");
        }
    }


    public void CreateHostPlayerItem()
    {
        foreach(PlayerObjectController player in Manager.GamePlayers)
        {
            GameObject NewPlayerItem = Instantiate(playerListItemPrefab) as GameObject;
            PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

            NewPlayerItemScript.PlayerName = player.PlayerName;
            NewPlayerItemScript.ConnectionID = player.ConnectionID;
            NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
            NewPlayerItemScript.Ready = player.Ready;
            NewPlayerItemScript.playerTeam = player.playerTeam;
            NewPlayerItemScript.SetPlayerValues();


            NewPlayerItem.transform.SetParent(teamAPlayerListViewContent.transform);
            NewPlayerItem.transform.localScale = Vector3.one;

            PlayerListItems.Add(NewPlayerItemScript);
        }
        PlayerItemCreated = true;
    }

    public void CreateClientPlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            if(!PlayerListItems.Any(b => b.ConnectionID == player.ConnectionID))
            {
                GameObject NewPlayerItem = Instantiate(playerListItemPrefab) as GameObject;
                PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

                NewPlayerItemScript.PlayerName = player.PlayerName;
                NewPlayerItemScript.ConnectionID = player.ConnectionID;
                NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
                NewPlayerItemScript.Ready = player.Ready;
                NewPlayerItemScript.playerTeam = player.playerTeam;
                NewPlayerItemScript.SetPlayerValues();

if (player.playerTeam == Team.TeamA)
{
                NewPlayerItem.transform.SetParent(teamAPlayerListViewContent.transform);
}
else if (player.playerTeam == Team.TeamB)
{
                NewPlayerItem.transform.SetParent(teamBPlayerListViewContent.transform);
}
                NewPlayerItem.transform.localScale = Vector3.one;

                PlayerListItems.Add(NewPlayerItemScript);
            }
        }
    }

    public void UpdatePlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            foreach(PlayerListItem PlayerListItemScript in PlayerListItems)
            {
                if(PlayerListItemScript.ConnectionID == player.ConnectionID)
                {
                    PlayerListItemScript.PlayerName = player.PlayerName;
                    PlayerListItemScript.Ready = player.Ready;
                    PlayerListItemScript.playerTeam = player.playerTeam;
                    PlayerListItemScript.SetPlayerValues();
                    if(player == LocalplayerController)
                    {
                        UpdateButton();
                    }

                }
            }
        }
        CheckIfAllReady();
    }

    public void RemovePlayerItem()
    {
        List<PlayerListItem> playerListItemToRemove = new List<PlayerListItem>();

        foreach (PlayerListItem playerlistItem in PlayerListItems)
        {
            if(!Manager.GamePlayers.Any(b=> b.ConnectionID == playerlistItem.ConnectionID))
            {
                playerListItemToRemove.Add(playerlistItem);
            }
        }
        if(playerListItemToRemove.Count > 0)
        {
            foreach(PlayerListItem playerlistItemToRemove in playerListItemToRemove)
            {
                GameObject ObjectToRemove = playerlistItemToRemove.gameObject;
                PlayerListItems.Remove(playerlistItemToRemove);
                Destroy(ObjectToRemove);
                ObjectToRemove = null;
            }
        }
    }

    public void StartGame(string SceneName)
    {
        LocalplayerController.CanStartGame(SceneName);
    }

    public void InviteFriendsToLobby()
    {
        if (CurrentLobbyID != 0)
        {
            CSteamID lobbyID = new CSteamID(CurrentLobbyID);

            Debug.Log("Inviting...");
            SteamFriends.ActivateGameOverlayInviteDialog(lobbyID);
        }
        else
        {
            Debug.LogError("Lobby ID is not set.");
        }
    }

    public void GoToMainMenu()
    {
        if (NetworkClient.active)
        {
            NetworkClient.Shutdown();
        }
        if (NetworkServer.active)
        {
            NetworkServer.Shutdown();
        }

        SceneManager.LoadScene("MainMenu");
    }

public void AutoAssignTeams()
    {
        foreach (PlayerObjectController player in manager.GamePlayers)
        {
            if (!player.Ready)
            {
                Team assignedTeam = teamPlayers[Team.TeamA].Count < teamPlayers[Team.TeamB].Count || teamPlayers[Team.TeamA].Count == teamPlayers[Team.TeamB].Count ? Team.TeamA : Team.TeamB;
                if (teamPlayers[Team.TeamA].Count < 2 || teamPlayers[Team.TeamB].Count < 2)
                {
                    player.playerTeam = assignedTeam;
                    teamPlayers[assignedTeam].Add(player);
                    Debug.Log("Player's team is " + player.playerTeam);
                }
            }
        }
    }

    public void ChangeTeam(PlayerObjectController player, Team newTeam)
{
    if (teamPlayers[player.playerTeam].Remove(player))
    {
        if (teamPlayers[newTeam].Count < 2)
        {
            player.playerTeam = newTeam;
            teamPlayers[newTeam].Add(player);
            UpdateTeamUI();
        }
        else
        {
            Debug.Log("Cannot change team: New team is full.");
        }
    }
    else
    {
        Debug.Log("No change: Player not found in current team.");
    }
}

    public void UpdateTeamUI()
{
    Debug.Log("Player current team: " + LocalplayerController.playerTeam);
    
    // 清空当前团队的UI元素
    ClearChildren(teamAPlayerListViewContent.transform);
    ClearChildren(teamBPlayerListViewContent.transform);
    
    // 遍历团队A的玩家并添加到列表
    foreach (PlayerObjectController player in teamPlayers[Team.TeamA])
    {
        AddPlayerToList(teamAPlayerListViewContent, player);
    }
    
    // 遍历团队B的玩家并添加到列表
    foreach (PlayerObjectController player in teamPlayers[Team.TeamB])
    {
        AddPlayerToList(teamBPlayerListViewContent, player);
    }

    // 在这里更新玩家项
    UpdatePlayerItem(); // 只需更新一次
}

private void AddPlayerToList(GameObject listViewContent, PlayerObjectController player)
{
    GameObject newItem = Instantiate(playerListItemPrefab, listViewContent.transform);
    PlayerListItem itemScript = newItem.GetComponent<PlayerListItem>();
    
    if (itemScript != null)
    {
        itemScript.PlayerName = player.PlayerName;
        itemScript.ConnectionID = player.ConnectionID;
        itemScript.PlayerSteamID = player.PlayerSteamID;
        itemScript.Ready = player.Ready;
        itemScript.SetPlayerValues(); // 确保这方法设置了所有需要的属性
    }
}


    private void ClearChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
    
//     private void SetupScrollViewEvents()
// {
//     teamAChooseButton.onClick.AddListener(SwitchToTeamA);
//     teamBChooseButton.onClick.AddListener(SwitchToTeamB);
// }

    public void SwitchToTeamA()
    {
        if (LocalplayerController != null)
        {
            Debug.Log("Oh! A");
        ChangeTeam(LocalplayerController, Team.TeamA);
        }
        else
        {
            Debug.Log("What's wrong???");
        }
    }

    public void SwitchToTeamB()
    {
        if (LocalplayerController != null)
        {
            Debug.Log("Oh! B");
        ChangeTeam(LocalplayerController, Team.TeamB);
        }
        else
        {
            Debug.Log("What's wrong???");
        }
    }
}