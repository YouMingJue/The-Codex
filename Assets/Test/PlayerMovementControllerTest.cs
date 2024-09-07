using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovementControllerTest : MonoBehaviour
{
    public float Speed = 5.0f; // Movement speed
    public GameObject PlayerModel; // The model of the player
    public TileManager tileManager; // Reference to the TileManager

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
            HandleMouseRotation();
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

    public void HandleMouseRotation()
    {
        // Get the mouse position in world coordinates
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculate the direction from the object to the mouse position
        Vector3 direction = mousePosition - transform.position;

        // Make sure the object only rotates in the Z axis (2D)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply the rotation to the object
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }







}
