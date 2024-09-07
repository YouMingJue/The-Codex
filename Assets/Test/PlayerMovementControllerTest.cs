using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerMovementControllerTest : MonoBehaviour
{ 
    public float Speed = 5.0f; // �����ٶ��Ա�����׿���Ч��
    public GameObject PlayerModel;
    public TileManager tileManager; // ����TileManager

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
        // ��TileManager��ȡ�������λ��
        tileManager = FindFirstObjectByType<TileManager>();
        Vector3 position = tileManager.GetPlayerStartPosition();
        transform.position = new Vector3(position.x, position.y, 0f);
    }

    public void Movement()
    {
        float xDirection = Input.GetAxis("Horizontal");
        float yDirection = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(xDirection, yDirection, 0.0f);

        // ȷ���ƶ���2Dƽ����
        transform.position += moveDirection * Speed * Time.deltaTime;
    }
}