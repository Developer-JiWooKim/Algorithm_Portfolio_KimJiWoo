# 게임 알고리즘 포트폴리오 과제

**유니티 버전:** 6.4.9f1

---

## 게임 플레이 방법
- 에디터 플레이 버튼 클릭하면 게임 시작 
- 키보드 조작(방향키 or wasd)으로 플레이어 캐릭터를 이동 시켜 오른쪽 맨 위(Goal Point)에 도달하면 게임 클리어
- 게임 시작 시 무작위 맵 생성 후 무작위 위치에 몬스터들이 스폰, 몬스터들의 시야각 안에(벽뒤 제외) 들어가면 플레이어 캐릭터를 추격함
- 왼쪽 위에 플레이어 캐릭터의 체력이 나오고 0이 되면 게임 오버

---

## 조작 방법

| 키 | 동작 |
|---|---|
| `W` \| `↑` | 위로 이동 |
| `S` \| `↓` | 아래로 이동 |
| `A` \| `←` | 왼쪽으로 이동 |
| `D` \| `→` | 오른쪽으로 이동 |

---

## 구현한 알고리즘

### Input System + Transform 기반 플레이어 이동 `PlayerMove.cs`
- `UnityEngine.InputSystem`의 `Keyboard.current`로 키 입력 감지
- 입력 값을 `Vector3`로 변환 후 `.normalized`로 정규화
    - 대각 이동 시 벡터 크기가 √2가 되어 속도가 빨라지는 것을 방지
- `transform.Translate`로 이동 처리

---

### Quternion.LookRotation, Quternion.Slerp 부드러운 회전 `PlayerMove.cs` `MonsterMove.cs`
- `Quternion.LookRotation()` 사용해 방향 벡터를 Quaternion으로 변환
- `Slerp`로 현재 회전에서 목표 회전까지 부드럽게 보간 처리

---
        
### sqrMagnitude 거리 기반 감지 `MonsterSight.cs`
```csharp
dir.sqrMagnitude <= detectionRange * detectionRange // dir : 타겟 방향 벡터 / detectionRange : 감지 범위
```
- `Vector3.Distance()`는 내부적으로 sqrt를 호출해 연산 비용이 큼 -> sqrt 없이 제곱값끼리 비교하는 sqrMagnitude 사용

---

### 자료구조 활용
| `List` | 경로 노드, 스폰 후보 셀, 몬스터 목록 관리 |
| `Dictionary` |A* 탐색에서 g비용, 경로 역추적 부모 노드 저장 |
| `HashSet` | A* 탐색에서 처리 완료 노드 저장 (중복 탐색 방지) |
| `Stack` | DFS 미로 생성에서 백트래킹 구현 (LIFO 구조) |

---

### 충돌 이벤트(Collider - Trigger 이벤트) `MonterAttack.cs`
- 몬스터 자식 오브젝트의 `Sphere Collider (Is Trigger = true)`로 공격 범위 구현
- `OnTriggerEnter` : 플레이어 진입 감지 → `PlayerInAttackRange = true;`
- `OnTriggerExit` :  플레이어 이탈 감지 → `PlayerInAttackRange = false;`
- `MonsterController`가 매 `Update`마다 `PlayerInAttackRange`를 읽어 Attack 상태 전환, 플레이어의 체력을 닳게 하는 `TakeDamage` 호출 시도

---

### 내적 시야 감지 `MonsterSight.cs`
- `dirToPlayer.y = 0` 으로 XZ 평면(수평)으로 변환 후 정규화 (`dirToPlayer` : 플레이어 방향 벡터)
- `Vector3.Dot()`으로 몬스터 `forward`와 플레이어 방향의 사잇각 코사인 값 계산
- `Mathf.Clamp(dot, -1, 1)` 처리 : 부동소수점 오차로 dot이 범위를 벗어나면 `Mathf.Acos`가 NaN을 반환하므로 반드시 Clamp로 방어 처리
- `Mathf.Acos(dot) * Mathf.Rad2Deg`로 각도 변환 후 `fieldOfView * 0.5f`와 비교 
    - `fieldOfView`는 양측 전체 시야각, 위 계산으로 나온 `angle`은 forward 기준 편측 각도이므로 절반과 비교
- 시야각 판별 후 `Physics.Raycast()`로 몬스터→플레이어 방향으로 Ray를 쏴 벽이 가로막고 있으면 감지 실패 처리

---

### DFS 미로 생성 `MazeGenerator.cs`
- 스택 기반 깊이 우선 탐색으로 완벽한 미로 생성 (모든 셀이 연결됨)

```
1. 시작 셀을 랜덤 선택 후 스택에 push
2. 현재 셀의 미방문 이웃 셀 중 랜덤 선택
3. 선택한 이웃 셀과의 공유 벽을 양쪽 모두 제거 후 스택에 push
4. 미방문 이웃이 없으면 스택에서 pop (백트래킹)
5. 스택이 빌 때까지 반복 → 모든 셀 방문 완료
```
- seed = -1이면 DateTime.Now.Millisecond로 매번 다른 미로
    - 고정값 입력 시 항상 동일한 미로 재현 가능

---

### A\* 길찾기 `AStarPathfinder.cs`
- `F = G + H` 공식으로 시작 노드에서 목표 노드까지 최단 경로 탐색

| 값 | 설명 |
|---|---|
| `G` | 시작에서 현재 노드까지 실제 이동 비용 (한 칸 = 10) |
| `H` | 맨해튼 거리 휴리스틱 `(\|dx\| + \|dy\|) * 10` |

- **노드** : 미로의 각 Cell (col, row 좌표)
- **간선** : Cell의 벽 정보 (클래스 내부 bool형 변수들)로 판단, 벽이 없는 방향만 이웃으로 추가
- 목표 도달 시 `cameFrom`을 역방향으로 따라가 경로 복원 후 `Reverse()`
- 첫 번째 노드 (현재 몬스터가 있는 셀 중앙) 제거 → 몬스터가 플레이어 추격 중 자신의 셀 중앙으로, 뒤로 이동하는 현상 해결

---

### FSM 상태 전이 `MonsterFSM.cs`

```
Idle -(플레이어 감지)-> Chase

Chase -(플레이어가 공격 범위 안에 진입)-> Attack
Chase -(감지 거리 이탈)-> Idle

Attack -(공격 범위 이탈)-> Chase
```

| 상태 | 동작 |
|---|---|
| `Idle` | 제자리 회전하며 시야각 내 플레이어 감지 대기 |
| `Chase` | 플레이어 감지 시 추격 시작. 직선 경로에 벽이 없으면 직선 이동, 있으면 A* 경로로 이동. 탐지 거리 이탈 시 Idle로 전환 |
| `Attack` | 공격 범위 진입 시 즉시 1 데미지. 3초 후에도 범위 안에 있으면 반복 데미지. 범위 이탈 시 Chase로 복귀 |

---

### SpereCast 최적화 `MonsterMove.cs`
- `Physics.SphereCast`로 몬스터와 플레이어 사이에 벽 존재 여부를 먼저 확인
    - `Raycast`(선 하나)와 달리 구체를 굴려 체크하므로 몬스터 크기를 고려한 더 정확한 판단
- 직선 경로가 열려있으면 A\* 계산을 생략 → 몬스터 수가 많을수록 효과가 큼

---

## 자료구조 선택 이유

    List
        List<Vector3> _path (MonsterMove.cs)
            - A* 경로 노드를 순서대로 저장하고 인덱스(_pathIndex)로 순차 접근 위해
            - 경로는 시작 -> 목표 순서가 중요하므로 인덱스 기반 접근(_pathIndex++)에 유리한 List 사용

        List<Cell> candidates (MonsterSpawner.cs)
            - 스폰 가능한 셀 전체를 순회하며 조건 필터링 후 저장
            - Fisher-Yates 셔플에서 인덱스 기반 교환이 필요하므로 List 사용

    Dictionary
        Dictionary<Vector2Int, Vector2Int> cameFrom (AStarPathfinder.cs)
            - 각 노드의 부모 노드를 기록해 경로 역추적에 사용
            - 셀 좌표(Vector2Int)를 key로 접근이 필요하므로 Dictionary 사용
        
        Dictionary<Vector2Int, int> gCost (AStarPathfinder.cs)
            - 각 노드까지의 이동 비용(g값)을 저장
            - 이웃 노드 탐색 시 기존 비용과 새 비용 비교과 비번하게 일어나므로 key로 접근이 가능한 Dictionary 사용
            
    HashSet
        HashSet<Vector2Int> closedSet (AStarPathfinder.cs)
            - 이미 처리한 노드를 저장, 중복 탐색 방지
            - A* 탐색 중 매 이웃 노드마다 포함 여부를 확인, Contains로 검색 가능한 HashSet 사용(Hash 함수를 사용하는 HashSet이 List 보다 성능 좋음)

    Stack
        Stack<Cell> stack (MazeGenerator.cs)
           - DFS 백트래킹 구현에 사용
           - 막힌 셀에서 되돌아갈 때 가장 최근에 방문한 셀로 돌아가야 하므로 LIFO(후입선출) 구조인 Stack이 적합
        