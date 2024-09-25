using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public float smoothSpeed = 0.125f;
    public Vector3 offset; 

    private Transform target;

    private void Awake()
{
    PlayerObjectController[] players = FindObjectsOfType<PlayerObjectController>();
    foreach (PlayerObjectController player in players)
    {
        if (player.hasAuthority)
        {
            target = player.transform;
            break;
        }
    }

    if (target == null)
    {
        Debug.LogError("No player found with authority.");
    }
}

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }
}