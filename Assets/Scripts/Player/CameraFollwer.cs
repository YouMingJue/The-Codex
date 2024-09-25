using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public float smoothSpeed = 0.125f; // �����ƽ���ٶ�
    public Vector3 offset; // ����������Ŀ���ƫ������Z�Ὣ������

    private Transform target;

    private void Start()
    {
        // ��Awake���ҵ�����"Player"��ǩ�Ķ���
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null && playerObject.GetComponent<PlayerObjectController>().hasAuthority)
        {
            target = playerObject.transform;
        }
        else
        {
            Debug.LogError("No player found with 'Player' tag.");
        }
    }

    private void LateUpdate()
    {
        // LateUpdate����ȷ�������������ƶ�֮����������λ��
        if (target != null)
        {
            // ��������λ�ã�����Z���ƫ����
            Vector3 desiredPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }
}