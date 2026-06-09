using UnityEngine;

/// <summary>
/// 플레이어 이동 시 부드럽게 따라가는 컴포넌트
/// </summary>
public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3   cameraOffset;

    private Vector3 _velocity = Vector3.zero;

    public float smoothTime  = 0.3f; // 높을수록 더 느리게 따라감

    private void LateUpdate()
    {
        SmoothFollowing();
    }
    private void SmoothFollowing()
    {
        Vector3 targetPosition = target.position + cameraOffset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref _velocity,
            smoothTime
        );
    }
}
