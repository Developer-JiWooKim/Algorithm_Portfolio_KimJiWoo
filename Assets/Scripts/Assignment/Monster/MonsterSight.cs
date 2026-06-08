using UnityEngine;

public class MonsterSight : MonoBehaviour
{
    [SerializeField] private float detectionRange = 15f;   // 감지 반경
    [SerializeField] private float fieldOfView    = 90f;   // 시야각
    [SerializeField] private bool  isSense        = false; // 감지 여부

    public bool TargetSense(Vector3 targetPos)
    {
        Vector3 dirToPlayer = targetPos - transform.position;
        dirToPlayer.y = 0;
        dirToPlayer = dirToPlayer.normalized;

        float dot = Vector3.Dot(transform.forward, dirToPlayer);    
        dot = Mathf.Clamp(dot, -1, 1); // 내적값이 -1 ~ 1을 초과하지 못하게 방어
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        if (angle >= fieldOfView * 0.5f)
        {
            return isSense = false;
        }

        Vector3 origin = transform.position + Vector3.up * 0.5f;
        float distance = Vector3.Distance(transform.position, targetPos);
        
        // 벽이 가로막고 있으면 감지 실패
        if(Physics.Raycast(origin, dirToPlayer, distance, LayerMask.GetMask("Wall")))
        {
            return isSense = false;
        }

        return isSense = true;
    }

    public bool IsInRange(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        dir.y = 0f;

        return dir.sqrMagnitude <= detectionRange * detectionRange;
    }

    private void OnDrawGizmos()
    {     
        // 감지 반경
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // 시야각 경계선 (회전 행렬로 좌/우 벡터 계산)
        Vector3 fwd = transform.forward;
        float halfRad = fieldOfView * 0.5f * Mathf.Deg2Rad;

        Vector3 leftDir = new Vector3(
             fwd.x * Mathf.Cos(-halfRad) - fwd.z * Mathf.Sin(-halfRad), 0,
             fwd.x * Mathf.Sin(-halfRad) + fwd.z * Mathf.Cos(-halfRad));

        Vector3 rightDir = new Vector3(
             fwd.x * Mathf.Cos(halfRad) - fwd.z * Mathf.Sin(halfRad), 0,
             fwd.x * Mathf.Sin(halfRad) + fwd.z * Mathf.Cos(halfRad));

        Gizmos.color = isSense ? Color.red : Color.yellow;
        Gizmos.DrawRay(transform.position, leftDir * detectionRange);
        Gizmos.DrawRay(transform.position, rightDir * detectionRange);
    }
}
