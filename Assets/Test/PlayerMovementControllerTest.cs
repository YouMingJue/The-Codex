using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovementControllerTest : MonoBehaviour
{
    public float Speed = 5.0f; // Movement speed
    public GameObject PlayerModel; // The mo78del of the player
    public TileManager tileManager; // Reference to the TileManager

    public float checkRadius = 1.0f; // Radius for detecting water tiles around the player
    public LayerMask tileLayer; // Layer for tiles (make sure water tiles are on this layer)

    HealthSystem health; 

    [SerializeField] private Collider2D myCollid;
    [SerializeField] private float buffCost;
    [SerializeField] private SpriteRenderer myRenderer;
    private PlayerAbility playerAbility;
    private BuffState currentState = BuffState.None;

    [SerializeField]private PlayerRotationController rotationController;

    private void Start()
    {
        //PlayerModel.SetActive(false); // Disable player model at the start
        tileManager = FindObjectOfType<TileManager>(); // Find TileManager
        playerAbility = GetComponent<PlayerAbility>();
        health = GetComponent<HealthSystem>();
    }

    private void FixedUpdate()
    {
      
            if (!PlayerModel.activeSelf)
            {
                SetPosition();
                PlayerModel.SetActive(true); // Activate player model once positioned
            }
            // Check for water tiles around the player in each frame
            CheckForWaterTiles();

            // Handle normal movement when not in WaterState
            if (currentState != BuffState.WaterState)
            {
                transform.position += Movement(); // Regular movement
            }
            else
            {
                // WaterState handling
                myCollid.enabled = false; // Disable collider during WaterState
                myRenderer.color = Color.blue; // Change player color to blue
                Speed = 7; // Increase speed in WaterState

                // Move player if the next tile is water and has enough mana
                if (Physics2D.OverlapPoint(transform.position + Movement())?.GetComponent<Tile>()?.type == TileType.Water && playerAbility.Mana >= buffCost)
                {
                    transform.position += Movement();
                    playerAbility.Mana -= buffCost * 1.2f * Time.deltaTime;
                }
                else
                {
                    playerAbility.Mana -= buffCost * Time.deltaTime;
                }

                // Exit WaterState if mana is depleted or LeftShift is pressed
                if (playerAbility.Mana <= 0 || Input.GetKeyDown(KeyCode.LeftShift))
                {
                    ExitWaterState();
                }
            }
            rotationController.HandleMouseRotation(); // Use the rotation controller
    }

    public void SetPosition()
    {
        // Set player position from TileManager
        tileManager = FindObjectOfType<TileManager>();
        Vector3 position = tileManager.GetPlayerStartPosition();
        transform.position = new Vector3(position.x, position.y, 0f);
    }

    public Vector3 Movement()
    {
        float xDirection = Input.GetAxisRaw("Horizontal");
        float yDirection = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = new Vector3(xDirection, yDirection, 0.0f);

        // 确保移动在2D平面上
        return moveDirection.normalized * Speed * Time.deltaTime;
    }

    private void CheckForWaterTiles()
    {
        // Detect all nearby tiles within a radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, checkRadius, tileLayer);

        bool isInWaterTile = false;

        foreach (Collider2D collider in colliders)
        {
            Tile tile = collider.GetComponent<Tile>();
            if (tile != null && tile.type == TileType.Water)
            {
                isInWaterTile = true; // Set flag to true if we detect a water tile

                // Activate WaterState if 'Q' is pressed and conditions are met
                if (Input.GetKeyDown(KeyCode.Q) && currentState != BuffState.WaterState && playerAbility.element == TileType.Water && playerAbility.Mana >= buffCost)
                {
                    EnterWaterState();
                }
                break;
            }
        }

        if (!isInWaterTile && currentState == BuffState.WaterState)
        {
            ExitWaterState();
        }
    }


    private void EnterWaterState()
    {
        Debug.Log("Water buff activated");
        currentState = BuffState.WaterState; // Set to WaterState
        myRenderer.color = Color.blue; // Change player color to indicate state
        Speed = 7; // Increase speed during WaterState
    }

    private void ExitWaterState()
    {
        currentState = BuffState.None; // Exit WaterState
        myCollid.enabled = true; // Re-enable collider
        myRenderer.color = Color.white; // Reset color to default
        Speed = 5; // Reset speed to normal
    }

    // Optional: Draw the detection radius for water tiles in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Tile>(out Tile tile))
        {
            if (tile != null)
            {
                switch (tile.type)
                {
                    case TileType.Fire:

                        break;
                }
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Tile>(out Tile tile))
        {
            if (tile != null)
            {
                switch (tile.type)
                {
                    case TileType.Fire:
                        if (playerAbility.element == TileType.Fire) return;
                        health.TakeDamage(5*Time.deltaTime,tile.transform);
                        break;
                }
            }
        }
    }
}
