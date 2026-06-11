using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private MazeGenerator mazeGenerator;    
    [SerializeField] private GameObject    monsterPrefab;
    [SerializeField] private Transform     target;           // Player Transform
    [SerializeField] private int           monsterCount = 5;
    [SerializeField] private float         spawnY = 1f;      // 몬스터 스폰 y 좌표

    private Vector2Int _playerStartPoint; // 플레이어 시작 셀
    private Vector2Int _goalPoint;        // 목표 지점 셀

    // 생성된 몬스터를 List로 관리
    private List<GameObject> _monsters = new List<GameObject>();
    public List<GameObject> Monsters => _monsters;

    private void Start()
    {
        Initialize();
        SpawnAll();
    }

    /// <summary>
    /// 초기화 메소드
    /// </summary>
    private void Initialize()
    {
        _playerStartPoint = new Vector2Int(0, 0);
        _goalPoint = new Vector2Int(mazeGenerator.Cols - 1, mazeGenerator.Rows - 1);
    }

    /// <summary>
    /// 몬스터들을 미로의 랜덤한 위치에 스폰하는 메소드
    /// </summary>
    public void SpawnAll()
    {
        ClearAll();

        List<Cell> candidates = new List<Cell>();

        // 몬스터를 스폰 가능한 셀들 리스트에 추가
        foreach (Cell cell in mazeGenerator.AllCells)
        {
            // 플레이어 시작점 or 목표 지점에는 몬스터 생성 X
            if (cell.col == _playerStartPoint.x && cell.row == _playerStartPoint.y) continue;
            if (cell.col == _goalPoint.x && cell.row == _goalPoint.y) continue;

            candidates.Add(cell);
        }

        // Fisher-Yates 셔플로 랜덤한 위치 보장
        for (int i = candidates.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (candidates[i], candidates[j]) = (candidates[j], candidates[i]);
        }

        // monsterCount 수만큼 몬스터 스폰
        for (int i = 0; i < monsterCount; i++)
        {
            // Cell.worldCenter로 몬스터 스폰해서 위치가 벽과 겹치지 않게
            Vector3 spawnPos = candidates[i].worldCenter;
            spawnPos.y = spawnY;

            GameObject monster = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
            monster.GetComponent<MonsterController>().Target = target;

            // 스폰한 몬스터들을 리스트에 추가
            _monsters.Add(monster);
        }
    }

    /// <summary>
    /// 몬스터 리스트 초기화 메소드
    /// </summary>
    private void ClearAll()
    {
        foreach (var mon in _monsters)
        {
            if (mon != null) Destroy(mon);
        }

        _monsters.Clear();
    }
}
