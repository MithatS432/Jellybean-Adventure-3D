using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    private void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}
