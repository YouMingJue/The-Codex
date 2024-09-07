using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerMovementControllerTest : MonoBehaviour
{
    public float Speed = 5.0f; // Move speed
    public GameObject PlayerModel;
    public TileManager tileManager; // Reference to TileManager

    private void Start()
    {
        PlayerModel.SetActive(false);
        tileManager = FindFirstObjectByType<TileManager>();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (!PlayerModel.activeSelf)
            {
                SetPosition();
                PlayerModel.SetActive(true);
            }

            Movement();
        }
    }

    public void SetPosition()
    {
        // Get the player's starting position from the TileManager
        tileManager = FindFirstObjectByType<TileManager>();
        Vector3 position = tileManager.GetPlayerStartPosition();
        transform.position = new Vector3(position.x, position.y, 0f);
    }

    public void Movement()
    {
        float xDirection = Input.GetAxis("Horizontal");
        float yDirection = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(xDirection, yDirection, 0.0f);

        // Move the player in 2D space
        transform.position += moveDirection * Speed * Time.deltaTime;

        // If there's movement, rotate the player to face the input direction
        if (moveDirection != Vector3.zero)
        {
            // Calculate the angle in radians, convert to degrees, and apply the rotation
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
