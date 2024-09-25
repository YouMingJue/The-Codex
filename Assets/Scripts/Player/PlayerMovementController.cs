using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;


public enum BuffState
{
    None,
    FireState,
    WaterState
}
public class PlayerMovementController : NetworkBehaviour
{
    public float Speed = 5.0f; // 增加速度以便更容易看到效果
    public GameObject PlayerModel;
    public TileManager tileManager; // 引用TileManager
    private BuffState currentState = BuffState.None;
    [SerializeField] private Collider2D myCollid;
    [SerializeField] private float buffCost;
    private PlayerAbility playerAbility;

    private void Start()
    {
        PlayerModel.SetActive(false);
        tileManager = FindFirstObjectByType<TileManager>();
        playerAbility = GetComponent<PlayerAbility>();
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

            if (hasAuthority && currentState != BuffState.WaterState)
            {
               transform.position += Movement();
            }else if(currentState == BuffState.WaterState)
            {
                myCollid.enabled = false;
                
                if(Physics2D.OverlapPoint(transform.position + Movement()).GetComponent<TileBehavior>().element == Element.Water && playerAbility.Mana >= buffCost)
                {
                    transform.position += Movement();
                    playerAbility.Mana -= buffCost * 1.2f;
                }
                else
                {
                    playerAbility.Mana -= buffCost;
                }
                if (playerAbility.Mana <= 0 || Input.GetKeyDown(KeyCode.LeftShift)) currentState = BuffState.None;
            }
        }
    }

    public void SetPosition()
    {
        // 从TileManager获取玩家生成位置
        tileManager = FindFirstObjectByType<TileManager>();
        Vector3 position = tileManager.GetPlayerStartPosition();
        transform.position = new Vector3(position.x, position.y, 0f);
    }

    public Vector3 Movement()
    {
        float xDirection = Input.GetAxis("Horizontal");
        float yDirection = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(xDirection, yDirection, 0.0f);

        // 确保移动在2D平面上
        return moveDirection * Speed * Time.deltaTime;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent<TileBehavior>(out TileBehavior tile))
        {
            if (tile != null)
            {
                switch (tile.element)
                {
                    case Element.Fire:
                        break;
                    case Element.Water:
                        if (playerAbility.element != Element.Water) return;
                        if (Input.GetKeyDown(KeyCode.LeftShift) && playerAbility.Mana >= buffCost)
                        {
                            currentState = BuffState.WaterState;
                        }
                        break;
                }
            }
        }
    }
}