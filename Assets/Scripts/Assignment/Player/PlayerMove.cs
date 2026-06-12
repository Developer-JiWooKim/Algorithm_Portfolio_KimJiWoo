using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    /// <summary>
    /// 방향과 속력을 받아 플레이어를 이동 및 이동 방향으로 회전 시켜주는 메소드
    /// </summary>
    public void Move(Vector3 moveDir, float moveSpeed)
    {
        // Vector3 정규화
        float distance = moveDir.magnitude;

        if (distance < 0.001f) return;

        moveDir /= distance;

        RotateToward(moveDir);

        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);       
    }

    private void RotateToward(Vector3 dir)
    {
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }
}
