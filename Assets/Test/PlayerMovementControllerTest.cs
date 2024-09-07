using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerMovementControllerTest : MonoBehaviour
{ 
    public float Speed = 5.0f; // 增加速度以便更容易看到效果
    public GameObject PlayerModel;
    public TileManager tileManager; // 引用TileManager

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

            //if (hasAuthority)
            {
                Movement();
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

    public void Movement()
    {
        float xDirection = Input.GetAxis("Horizontal");
        float yDirection = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(xDirection, yDirection, 0.0f);

        // 确保移动在2D平面上
        transform.position += moveDirection * Speed * Time.deltaTime;
    }
}