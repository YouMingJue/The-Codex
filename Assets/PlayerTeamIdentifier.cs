using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeamIdentifier : MonoBehaviour
{
    public PlayerObjectController playerObjectController;
    public SpriteRenderer spriteRenderer; 

    void Start()
    {
        playerObjectController = GetComponent<PlayerObjectController>();
        spriteRenderer = GetComponent<SpriteRenderer>(); 

        SetPlayerTeamColor();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void SetPlayerTeamColor()
    {
        if (playerObjectController != null)
        {
            switch (playerObjectController.playerTeam)
            {
                case Team.TeamA:
                    spriteRenderer.color = Color.red;
                    break;
                case Team.TeamB:
                    spriteRenderer.color = Color.blue;
                    break;
                default:
                    spriteRenderer.color = Color.white;
                    break;
            }
        }
        else
        {
            Debug.LogError("PlayerObjectController not found on the object.");
        }
    }
}