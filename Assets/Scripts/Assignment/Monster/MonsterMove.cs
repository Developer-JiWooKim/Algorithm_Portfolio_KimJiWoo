using System.Collections.Generic;
using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed    = 7f;   // 이동 속도
    [SerializeField] private float rotateSpeed  = 15f;  // 회전 속도
    [SerializeField] private float nodeDistance = 0.5f;

    private List<Vector3> _path;
    private int           _pathIndex;
    private Vector2Int    _lastGoalCell = Vector2Int.zero;
    private float         _sphereCastRaduis = 0.5f; // 몬스터 크기 Radius

    public void IdleRotate()
    {
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f, Space.World);
    }

    public void RequestPath(Vector3 targetPos)
    {
        if (AStarPathfinder.Instance == null) return;

        Vector2Int goalCell = AStarPathfinder.Instance.WorldToCell(targetPos);

        if (goalCell == _lastGoalCell && _path != null) return;

        _lastGoalCell = goalCell;

        List<Vector3> newPath = AStarPathfinder.Instance.FindPath(transform.position, targetPos);

        if (newPath != null && newPath.Count > 0)
        {
            _path = newPath;
            _pathIndex = 0;
        }
    }

    public void ClearPath()
    {
        _path = null;
        _pathIndex = 0;
    }

    public void MoveToTarget(Vector3 targetPos)
    {
        if (IsPathClear(targetPos))
        {
            ClearPath();
            MoveStraight(targetPos);
        }
        else
        {
            RequestPath(targetPos);
            // 길이 없거나 끝에 도달했으면 타겟 방향으로 직진
            if (_path == null || _pathIndex >= _path.Count)
            {
                MoveStraight(targetPos);
                return;
            }

            Vector3 nodePos = _path[_pathIndex];
            nodePos.y = transform.position.y;

            // 목표 노드에 도착 시 다음 노드로 목표 노드를 바꿈
            if ((nodePos - transform.position).sqrMagnitude < nodeDistance * nodeDistance)
            {
                _pathIndex++;
                return;
            }

            MoveStraight(nodePos);
        }
    }

    public void LookAtTarget(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.001f) return;

        RotateToward(dir.normalized);
    }

    // SphereCast로 현재 타겟과 자신 사이에 벽이 있는지 체크
    private bool IsPathClear(Vector3 targetPos)
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 direction = (targetPos - transform.position);
        direction.y = 0;
        float distance = direction.magnitude;

        return !Physics.SphereCast(origin, _sphereCastRaduis, direction.normalized, out _, distance, LayerMask.GetMask("Wall"));
    }

    private void MoveStraight(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f) return;

        dir = dir.normalized;

        RotateToward(dir);

        transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);
    }

    private void RotateToward(Vector3 dir)
    {
        Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);

        // 부드러운 회전
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
    }
}
