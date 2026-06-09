using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder : MonoBehaviour
{
    [SerializeField] private MazeGenerator mazeGenerator;

    private int _cost = 10; // 한 셀 이동 비용

    private static AStarPathfinder _instance;
    public static AStarPathfinder Instance { get; private set; }

    private void Awake()
    {
        if (_instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }        
    }

    /// <summary>
    /// 월드 좌표를 셀 인덱스로 변환하는 메소드
    /// </summary>
    public Vector2Int WorldToCell(Vector3 worldPos)
    {
        return mazeGenerator.WorldToCell(worldPos);
    }

    /// <summary>
    /// A* 알고리즘으로 시작 지점에서 목표 지점까지의 최단 경로를 탐색하는 메소드
    /// </summary>
    public List<Vector3> FindPath(Vector3 startWorld, Vector3 goalWorld)
    {
        // 시작 지점과 목표지점의 셀을 구함
        Vector2Int start = mazeGenerator.WorldToCell(startWorld);
        Vector2Int goal  = mazeGenerator.WorldToCell(goalWorld);

        // 시작 지점과 목표 지점이 같으면 null
        if (start == goal) return null;

        var openSet   = new List<Vector2Int>();
        var closedSet = new HashSet<Vector2Int>();
        var cameFrom  = new Dictionary<Vector2Int, Vector2Int>();
        var gCost     = new Dictionary<Vector2Int, int>();

        openSet.Add(start);
        gCost[start] = 0;

        while (openSet.Count > 0)
        {
            // F(G + H)가 가장 적은 셀(goal)을 찾음
            Vector2Int current = GetLowestF(openSet, gCost, goal);

            // 현재 위치가 목표지점과 같으면 cameFrom으로 최종 경로를 만들고 탐색 종료
            if(current == goal)
            {
                return BuildPath(cameFrom, current);
            }

            // 현재 위치는 검사 했으니 제거, 방문 표시 남김
            openSet.Remove(current);
            closedSet.Add(current);

            foreach (var neighbor in GetNeighbors(current))
            {
                if (closedSet.Contains(neighbor)) continue;

                // 셀 이동 비용 책정
                int newGCost = gCost[current] + _cost;

                if (!gCost.ContainsKey(neighbor) || newGCost < gCost[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gCost[neighbor] = newGCost;
                    if(!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }            
        }

        return null;
    }

    /// <summary>
    /// 목표 셀에서 cameFrom을 역방향으로 따라가 경로를 복원하는 메소드
    /// </summary>
    private List<Vector3> BuildPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        path.Add(current);

        // 목표 -> 시작 순서로 역추적하며 path에 저장
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }

        // 시작 -> 목표 순서로 path 반전
        path.Reverse();

        // 첫 번째 노드 제거, 몬스터가 이동 중 자신의 셀 중앙으로 가기 위해 뒤로 이동하는 현상 방어
        if(path.Count > 1)
        {
            path.RemoveAt(0);
        }

        List<Vector3> worldPath = new List<Vector3>();

        // 셀 인덱스를 월드 좌표로 변환
        foreach (Vector2Int node in path)
        {
            worldPath.Add(mazeGenerator.CellToWorld(node));
        }           

        return worldPath;
    }

    /// <summary>
    /// 현재 셀에서 이동 가능한 이웃 셀 반환하는 메소드
    /// </summary>
    private List<Vector2Int> GetNeighbors(Vector2Int node)
    {
        // 4방향(한 셀에 존재하는 4방향의 벽 기반으로 이동 가능한 셀 구함
        var result = new List<Vector2Int>(4);

        Cell cell = mazeGenerator.GetCell(node.x, node.y);

        if (cell == null) return result;

        // true면 벽 있음 - 이동 불가
        // false면 벽 없음 - 이동 가능
        if (!cell.northWall) result.Add(new Vector2Int(node.x,     node.y + 1));
        if (!cell.southWall) result.Add(new Vector2Int(node.x,     node.y - 1));
        if (!cell.eastWall ) result.Add(new Vector2Int(node.x + 1, node.y));
        if (!cell.westWall)  result.Add(new Vector2Int(node.x - 1, node.y));

        return result;

    }

    /// <summary>
    /// openSet에서 F값이 가장 낮은 셀 반환하는 메소드
    /// </summary>
    private Vector2Int GetLowestF(List<Vector2Int> openSet, Dictionary<Vector2Int, int> gCost, Vector2Int goal)
    {
        Vector2Int best = openSet[0];

        int bestF = GetF(best, gCost, goal);

        for(int i = 1; i < openSet.Count; i++)
        {
            int f = GetF(openSet[i], gCost, goal);
            if(f < bestF)
            {
                best = openSet[i];
                bestF = f;
            }
        }

        return best;
    }

    /// <summary>
    /// F 값 계산하는 메소드
    /// G - 시작 노드에서 현재 노드까지 이동 비용 
    /// H - 현재 노드에서 목표까지 맨해튼 거리
    /// </summary>
    private int GetF(Vector2Int node, Dictionary<Vector2Int, int> gCost, Vector2Int goal)
    {
        int g = gCost.ContainsKey(node) ? gCost[node] : 9999;
        int h = (Mathf.Abs(node.x - goal.x) + Mathf.Abs(node.y - goal.y)) * 10;

        return g + h;
    }
}
