using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public void Move(Vector3 moveDir, float moveSpeed)
    {
        // Vector3 정규화
        moveDir = moveDir.normalized;

        if (moveDir.sqrMagnitude > 0)
        {
            transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);

            transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up);
        }
    }
}
