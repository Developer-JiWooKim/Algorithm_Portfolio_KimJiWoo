using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private MazeGenerator mazeGenerator;    
    [SerializeField] private GameObject    monsterPrefab;
    [SerializeField] private Transform     target;
    [SerializeField] private int           monsterCount = 5;
    [SerializeField] private float         spawnY = 1f;

    private Vector2Int _playerStartPoint;
    private Vector2Int _goalPoint;

    // 생성된 몬스터를 List로 관리
    private List<GameObject> _monsters = new List<GameObject>();
    public List<GameObject> Monsters => _monsters;

    private void Start()
    {
        Initialize();
        SpawnAll();
    }

    private void Initialize()
    {
        _playerStartPoint = new Vector2Int(0, 0);
        _goalPoint = new Vector2Int(mazeGenerator.Cols - 1, mazeGenerator.Rows - 1);
    }
    public void SpawnAll()
    {
        ClearAll();

        var candidates = new List<Cell>();
        foreach (Cell cell in mazeGenerator.AllCells)
        {
            // 플레이어 시작점 or 목표 지점에는 몬스터 생성 X
            if (cell.col == _playerStartPoint.x && cell.row == _playerStartPoint.y) continue;
            if (cell.col == _goalPoint.x && cell.row == _goalPoint.y) continue;

            candidates.Add(cell);
        }

        for (int i = candidates.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (candidates[i], candidates[j]) = (candidates[j], candidates[i]);
        }

        for (int i = 0; i < monsterCount; i++)
        {
            // Cell.worldCenter로 몬스터 스폰
            Vector3 spawnPos = candidates[Random.Range(0, candidates.Count)].worldCenter;
            spawnPos.y = spawnY;

            GameObject monster = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
            monster.GetComponent<MonsterController>().Target = target;

            _monsters.Add(monster);
        }
    }
    public void RemoveMonster(GameObject monster)
    {
        if (!_monsters.Contains(monster)) return;

        _monsters.Remove(monster);
        Destroy(monster);
    }

    private void ClearAll()
    {
        foreach (var mon in _monsters)
        {
            if (mon != null) Destroy(mon);
        }

        _monsters.Clear();
    }
}
