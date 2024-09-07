using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public float smoothSpeed = 0.125f; // 摄像机平滑速度
    public Vector3 offset; // 摄像机相对于目标的偏移量，Z轴将被忽略

    private Transform target;

    private void Awake()
    {
        // 在Awake中找到带有"Player"标签的对象
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
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
        // LateUpdate用于确保在所有物体移动之后更新摄像机位置
        if (target != null)
        {
            // 计算期望位置，忽略Z轴的偏移量
            Vector3 desiredPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }
}