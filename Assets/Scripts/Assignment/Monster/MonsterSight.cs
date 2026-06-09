using UnityEngine;

public class MonsterSight : MonoBehaviour
{
    [SerializeField] private float detectionRange = 15f;   // 감지 반경
    [SerializeField] private float fieldOfView    = 90f;   // 전체 시야각
    [SerializeField] private bool  isSense        = false; // 감지 여부

    /// <summary>
    /// 타겟이 시야각 안에 들어와 있고 타겟과 자신 사이에 벽이 있는지 검사하는 메소드 
    /// </summary>
    public bool TargetSense(Vector3 targetPos)
    {
        // 방향 벡터 계산 후 정규화(xy 평면만)
        Vector3 dirToPlayer = targetPos - transform.position; 
        dirToPlayer.y = 0;  
        dirToPlayer = dirToPlayer.normalized; 

        // 내적으로 자기자신의 앞(forward)와 타겟 방향의 사잇각 코사인 값 계산
        float dot = Vector3.Dot(transform.forward, dirToPlayer);    
        dot = Mathf.Clamp(dot, -1, 1); // 내적값이 -1 ~ 1을 초과하지 못하게 방어

        // 위에서 구한 코사인 값을 각도로 변환
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        // fieldOfView은 양측 전체 시야각이므로 절반과 비교
        if (angle >= fieldOfView * 0.5f)
        {
            return isSense = false;
        }

        // 내 위치 기준 바닥에서 0.5f 위 지점
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        // 타겟과 자신 사이의 거리
        float distance = Vector3.Distance(transform.position, targetPos);
        
        // 시야각 안에 있어도 Ray를 쐈을 때 벽이 타겟과 자신 사이에 있으면 감지 실패
        if(Physics.Raycast(origin, dirToPlayer, distance, LayerMask.GetMask("Wall")))
        {
            return isSense = false;
        }

        return isSense = true;
    }
    
    /// <summary>
    /// 감지 반경 안에 들어왔는지 검사하는 메소드
    /// </summary>
    public bool IsInRange(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        dir.y = 0f;

        return dir.sqrMagnitude <= detectionRange * detectionRange;
    }

    /// <summary>
    /// 에디터 확인용 기즈모(감지 범위, 시야각)
    /// </summary>
    private void OnDrawGizmos()
    {     
        // 감지 반경
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // 시야각 경계선 (회전 행렬로 좌/우 벡터 계산)
        Vector3 fwd = transform.forward;
        float halfRad = fieldOfView * 0.5f * Mathf.Deg2Rad;

        // 시야각 기준 왼쪽 경계
        Vector3 leftDir = new Vector3(
             fwd.x * Mathf.Cos(-halfRad) - fwd.z * Mathf.Sin(-halfRad), 0,
             fwd.x * Mathf.Sin(-halfRad) + fwd.z * Mathf.Cos(-halfRad));

        // 시야각 기준 오른쪽 경계
        Vector3 rightDir = new Vector3(
             fwd.x * Mathf.Cos(halfRad) - fwd.z * Mathf.Sin(halfRad), 0,
             fwd.x * Mathf.Sin(halfRad) + fwd.z * Mathf.Cos(halfRad));

        Gizmos.color = isSense ? Color.red : Color.yellow;
        Gizmos.DrawRay(transform.position, leftDir * detectionRange);
        Gizmos.DrawRay(transform.position, rightDir * detectionRange);
    }
}
