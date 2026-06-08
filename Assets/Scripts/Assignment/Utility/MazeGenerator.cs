using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Header("Gird")]
    [SerializeField] private int    _cols = 10; // 가로
    [SerializeField] private int    _rows= 10;  // 세로
    [SerializeField] private float  _cellSize = 5f;

    [Header("시작 점")]
    [SerializeField] private Vector2 worldStart = new Vector2(-25f, -25f);

    [Header("Walls")]
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private float      wallThickness = 0.3f;
    [SerializeField] private float      wallHeight    = 3f;

    [Header("Random Seed")]
    [SerializeField] private int seed = -1;    

    private Cell[,]          _grid;    
    private List<GameObject> _wallObjects = new List<GameObject>();
    private List<Cell>       _allCells    = new List<Cell>();

    public IReadOnlyList<Cell> AllCells => _allCells;
    public int Cols  => _cols;
    public int Rows  => _rows;
    public float CellSize => _cellSize;

    private void Awake() => Generate();
    public void Generate()
    {
        ClearWalls();
        InitGrid();
        RunDFS();
        SpawnWalls();
    }

    // Wall이 담긴 리스트 초기화
    public void ClearWalls()
    {
        foreach (var wall in _wallObjects)
        {
            if (wall != null)
            {
                DestroyImmediate(wall);
            }
        }

        _wallObjects.Clear();
    }

    // Grid 그려서 좌표 저장
    private void InitGrid()
    {
        _grid = new Cell[_cols, _rows];

        for (int r = 0; r < _rows; r++)
        {
            for (int c = 0; c < _cols; c++)
            {
                Vector3 center = new Vector3(worldStart.x + c * _cellSize + _cellSize * 0.5f, 0f, worldStart.y + r * _cellSize + _cellSize * 0.5f);
                _grid[c, r] = new Cell(c, r, center);
                _allCells.Add(_grid[c, r]);
            }
        }
    }

    // 깊이 기반(DFS) 벽 생성할 위치 탐색
    private void RunDFS()
    {
        // 매번 다른 맵이 나오도록 시드값 설정
        if (seed == -1)
        {
            Random.InitState(System.DateTime.Now.Millisecond);
        }
        else
        {
            Random.InitState(seed);
        }

        Stack<Cell> stack = new Stack<Cell>();

        Cell startCell = _grid[Random.Range(0, _cols), Random.Range(0, _rows)];
        startCell.visited = true;
        stack.Push(startCell);

        while (stack.Count > 0)
        {
            Cell current = stack.Peek();
            List<Cell> neighbors = GetUnvisitedNeighbors(current);

            if (neighbors.Count > 0)
            {
                // 이웃한 셀
                Cell next = neighbors[Random.Range(0, neighbors.Count)];

                RemoveWallBetween(current, next);

                next.visited = true;
                stack.Push(next);
            }
            else
            {
                // 막힌 곳 
                stack.Pop();
            }
        }
    }

    // 범위 내(Grid)에서 현재 셀 주위에 이웃한 미방문 셀 검사
    private List<Cell> GetUnvisitedNeighbors(Cell cell)
    {
        var list = new List<Cell>(4);
        int c = cell.col;
        int r = cell.row;

        if (r + 1 < _rows && !_grid[c, r + 1].visited) list.Add(_grid[c, r + 1]); // up
        if (r - 1 >= 0    && !_grid[c, r - 1].visited) list.Add(_grid[c, r - 1]); // down
        if (c + 1 < _cols && !_grid[c + 1, r].visited) list.Add(_grid[c + 1, r]); // right
        if (c - 1 >= 0    && !_grid[c - 1, r].visited) list.Add(_grid[c - 1, r]); // left

        return list;
    }

    private void RemoveWallBetween(Cell a, Cell b)
    {
        int dc = b.col - a.col;
        int dr = b.row - a.row;

        if      (dr == 1)   { a.northWall = false; b.southWall = false; }    
        else if (dr == -1)  { a.southWall = false; b.northWall = false; }    
        else if (dc == 1)   { a.eastWall  = false; b.westWall = false; } 
        else if (dc == -1)  { a.westWall  = false; b.eastWall  = false; } 
    }

    private void SpawnWalls()
    {
        if (wallPrefab == null) return;

        GameObject wallParent = new GameObject("Walls");
        wallParent.transform.SetParent(transform);

        for (int r = 0; r < _rows; r++)
        {
            for (int c = 0; c < _cols; c++)
            {
                Cell cell = _grid[c, r];

                // 셀 중심 월드 좌표
                float cx = worldStart.x + c * _cellSize + _cellSize * 0.5f;
                float cz = worldStart.y + r * _cellSize + _cellSize * 0.5f;

                if (cell.northWall)
                {
                    Vector3 pos = new Vector3(cx, wallHeight * 0.5f, cz + _cellSize * 0.5f);
                    SpawnWall(pos, false, wallParent.transform);
                }

                if (cell.eastWall)
                {
                    Vector3 pos = new Vector3(cx + _cellSize * 0.5f, wallHeight * 0.5f, cz);
                    SpawnWall(pos, true, wallParent.transform);
                }

                if (r == 0 && cell.southWall)
                {
                    Vector3 pos = new Vector3(cx, wallHeight * 0.5f, cz - _cellSize * 0.5f);
                    SpawnWall(pos, false, wallParent.transform);
                }

                if (c == 0 && cell.westWall)
                {
                    Vector3 pos = new Vector3(cx - _cellSize * 0.5f, wallHeight * 0.5f, cz);
                    SpawnWall(pos, true, wallParent.transform);
                }
            }
        }
    }

    // isVertical - true = Z축 방향 벽, false = X축 방향 벽
    private void SpawnWall(Vector3 position, bool isVertical, Transform parent)
    {
        GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity, parent);

        // 가로 벽: _cellSize 길이, X 방향으로 
        // 세로 벽: _cellSize 길이, Z 방향으로 
        wall.transform.localScale = isVertical ? new Vector3(wallThickness, wallHeight, _cellSize) : new Vector3(_cellSize, wallHeight, wallThickness);

        _wallObjects.Add(wall);
    }

    public Vector2Int WorldToCell(Vector3 worldPos)
    {
        int col = (int)((worldPos.x - worldStart.x) / _cellSize);
        int row = (int)((worldPos.z - worldStart.y) / _cellSize);

        col = Mathf.Clamp(col, 0, _cols - 1);
        row = Mathf.Clamp(row, 0, _rows - 1);

        return new Vector2Int(col, row);
    }

    public Vector3 CellToWorld(Vector2Int cell)
    {
        return _grid[cell.x, cell.y].worldCenter;
    }

    public Cell GetCell(int col, int row)
    {
        if (col < 0 || col >= _cols || row < 0 || row >= _rows) return null;
        return _grid[col, row];
    }

    // 에디터에서 그리드 확인용
    private void OnDrawGizmos()
    {
        // 월드 범위 테두리
        Gizmos.color = Color.green;
        float totalW = _cols * _cellSize;
        float totalH = _rows * _cellSize;
        Vector3 center = new Vector3(worldStart.x + totalW * 0.5f, 0, worldStart.y + totalH * 0.5f);
        Gizmos.DrawWireCube(center, new Vector3(totalW, 0.1f, totalH));

        // 셀 그리드
        Gizmos.color = new Color(1, 1, 0, 0.2f);
        for (int r = 0; r < _rows; r++)
        {
            for (int c = 0; c < _cols; c++)
            {
                float cx = worldStart.x + c * _cellSize + _cellSize * 0.5f;
                float cz = worldStart.y + r * _cellSize + _cellSize * 0.5f;
                Gizmos.DrawWireCube(new Vector3(cx, 0, cz), new Vector3(_cellSize - 0.1f, 0.1f, _cellSize - 0.1f));
            }
        }
    }
}
