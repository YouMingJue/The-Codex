using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerMovementController : NetworkBehaviour
{
    public float Speed = 5.0f; // �����ٶ��Ա�����׿���Ч��
    public GameObject PlayerModel;
    public TileManager tileManager; // ����TileManager

    private void Start()
    {
        PlayerModel.SetActive(false);
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

            if (hasAuthority)
            {
                Movement();
            }
        }
    }

    public void SetPosition()
    {
        // ��TileManager��ȡ�������λ��
        Vector3 position = tileManager.GetPlayerStartPosition();
        transform.position = new Vector3(position.x, 0.8f, position.y);
    }

    public void Movement()
    {
        float xDirection = Input.GetAxis("Horizontal");
        float zDirection = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(xDirection, 0.0f, zDirection);

        // ȷ���ƶ���2Dƽ����
        transform.position += moveDirection * Speed * Time.deltaTime;
    }
}