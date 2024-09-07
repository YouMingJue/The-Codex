using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovementControllerTest : MonoBehaviour
{
    public float Speed = 5.0f; // Movement speed
    public GameObject PlayerModel; // The model of the player
    public TileManager tileManager; // Reference to the TileManager

    [SerializeField]private PlayerRotationController rotationController;

    private void Start()
    {
        PlayerModel.SetActive(false); // Disable player model at the start
        tileManager = FindObjectOfType<TileManager>(); // Find TileManager
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (!PlayerModel.activeSelf)
            {
                SetPosition();
                PlayerModel.SetActive(true); // Activate player model once positioned
            }

            HandleGlobalMovement();
            rotationController.HandleMouseRotation(); // Use the rotation controller
        }
    }

    public void SetPosition()
    {
        // Set player position from TileManager
        tileManager = FindObjectOfType<TileManager>();
        Vector3 position = tileManager.GetPlayerStartPosition();
        transform.position = new Vector3(position.x, position.y, 0f);
    }

    public void HandleGlobalMovement()
    {
        float xDirection = Input.GetAxis("Horizontal");
        float yDirection = Input.GetAxis("Vertical");

        // Move player globally (not based on local rotation)
        Vector3 moveDirection = new Vector3(xDirection, yDirection, 0.0f);
        transform.position += moveDirection * Speed * Time.deltaTime;
    }
}
