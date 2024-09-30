using UnityEngine;
using Mirror;
using TMPro; // Ìí¼Ó´ËÐÐ

public class GameplayManager : NetworkBehaviour
{
    public static GameplayManager Instance;

    [SyncVar(hook = nameof(OnTeamAScoreChanged))]
    public int teamAScore = 0;

    [SyncVar(hook = nameof(OnTeamBScoreChanged))]
    public int teamBScore = 0;

    public TMP_Text teamAScoreText;
    public TMP_Text teamBScoreText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //[Server]
    public void AddScoreToOppositeTeam(Team playerTeam)
    {
        if (playerTeam == Team.TeamA)
        {
            teamBScore++;
            Debug.Log("Team B scores! Total Score: " + teamBScore);
        }
        else if (playerTeam == Team.TeamB)
        {
            teamAScore++;
            Debug.Log("Team A scores! Total Score: " + teamAScore);
        }
    }

    void OnTeamAScoreChanged(int oldScore, int newScore)
    {
        UpdateScoreUI();
    }

    void OnTeamBScoreChanged(int oldScore, int newScore)
    {
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (teamAScoreText != null)
            teamAScoreText.text = "Team A Score: " + teamAScore;

        if (teamBScoreText != null)
            teamBScoreText.text = "Team B Score: " + teamBScore;
    }
}
