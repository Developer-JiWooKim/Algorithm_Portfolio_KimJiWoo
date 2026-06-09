게임 알고리즘 포트폴리오 과제

유니티 버전 6.4.9f1

게임 플레이 방법
    - 에디터 플레이 버튼 클릭, 키보드 조작으로 플레이어 캐릭터를 이동 시켜 오른쪽 맨 위(Goal Point)에 도달하면 게임 클리어
    - 게임 시작 시 무작위 맵 생성 후 무작위 위치에 몬스터들이 스폰, 몬스터들의 시야각 안에(벽뒤 제외) 들어가면 플레이어 캐릭터를 추격함
    - 왼쪽 위에 플레이어 캐릭터의 체력이 나오고 0이 되면 게임 오버



조작 방법
    키보드  
    W || 방향키 ↑ : 위로 이동
    S || 방향키 ↓ : 아래로 이동 
    A || 방향키 ← : 왼쪽으로 이동 
    D || 방향키 → : 오른쪽으로 이동



구현한 알고리즘
    - Input System + Transform 기반 플레이어 이동 (PlayerMove.cs)
        - UnityEngine.InputSystem의 Keyboard.current로 키 입력 감지
        - 입력값을 Vector3로 변환 후 .normalized로 정규화
        - transform.Translate로 이동, Quaternion.LookRotation으로 이동 방향 즉시 회전

    - Quternion.LookRotation, Quternion.Slerp 부드러운 회전(PlayerMove.cs, MonsterMove.cs)
        - LookRotation: 방향 벡터를 Quaternion으로 변환
        - Slerp(구면 선형 보간): 현재 회전에서 목표 회전까지 부드럽게 보간
        
    - sqrMagnitude를 사용한 거리 기반 감지(MonsterSight.cs)
        - TODO# 이 부분 찾아서 사용한 공식 넣기

    - 자료구조 활용
        - List: 경로 노드, 스폰 후보 셀, 몬스터 목록 관리
        - Dictionary: A* 탐색에서 g비용, 경로 역추적 부모 노드 저장
        - HashSet: A* 탐색에서 처리 완료 노드 저장 (중복 탐색 방지)
        - Stack: DFS 미로 생성에서 백트래킹 구현 (LIFO 구조)

    - 충돌 이벤트(Collider - Trigger 이벤트)
        - 몬스터 자식 오브젝트의 Sphere Collider (Is Trigger = true)로 공격 범위 구현
        - OnTriggerEnter: 플레이어 진입 감지 → PlayerInAttackRange = true
        - OnTriggerExit:  플레이어 이탈 감지 → PlayerInAttackRange = false
        - MonsterController가 매 Update마다 PlayerInAttackRange를 읽어 Attack 상태 전환

    - 내적 시야 감지 (MonsterSight.cs)
        - dirToPlayer.y = 0으로 XZ 평면(수평)으로 변환 후 정규화
        - Vector3.Dot으로 몬스터 forward와 플레이어 방향의 사잇각 코사인 값 계산
        - Mathf.Clamp(dot, -1, 1): 부동소수점 오차로 dot이 범위를 벗어나면 Mathf.Acos가 NaN을 반환하므로 반드시 Clamp로 방어
        - Mathf.Acos(dot) * Mathf.Rad2Deg로 각도 변환 후 fieldOfView * 0.5f와 비교
          (fieldOfView는 양측 전체 시야각, angle은 forward 기준 편측 각도이므로 절반과 비교)
        - 시야각 판별 후 Physics.Raycast로 몬스터→플레이어 방향으로 Ray를 쏴 벽이 가로막고 있으면 감지 실패 처리
        - Vector3.Dot으로 몬스터 forward와 플레이어 방향의 사잇각 코사인 값 계산, Mathf.Acos(dot) * Mathf.Rad2Deg로 코사인 값을 각도록 변환 후 시야각 내에 플레이어가 있는지 판별
        - 시야각 판별 후 Physics.Raycast로 벽 뒤 감지를 차단

    - DFS 미로 생성 (MazeGenerator.cs)
        - 스택 기반 깊이 우선 탐색으로 완벽한 미로 생성 (모든 셀이 연결됨)
        - 시작 셀을 랜덤 선택 후 스택에 push, 미방문 이웃 셀을 랜덤 선택
        - 선택한 이웃 셀과의 공유 벽을 양쪽 모두 제거 후 스택에 push
        - 미방문 이웃이 없으면 스택에서 pop (백트래킹)
        - 스택이 빌 때까지 반복 → 모든 셀 방문 완료
        - seed = -1이면 DateTime.Now.Millisecond로 매번 다른 미로,
          고정값 입력 시 항상 동일한 미로 재현 가능
        - 스택 기반 깊이 우선 탐색으로 미로를 생성
        - 시작 셀에서 랜덤한 미방문 이웃을 선택해 벽을 제거하며 이동하고 막히면 스택에서 꺼내 백 트래킹 -> 모든 셀이 방문될 때까지 반복하여 모든 셀이 연결된 미로를 생성함

    - A* 길찾기 (AStarPathfinder.cs)
        - 시작 노드에서 목표노드까지의 최단 거리를 구함(셀의 벽 정보로 판단해 벽이 없는 방향으로만 이동)
            g - 시작에서 현재 노드까지 실제 비용
            h - 맨해튼 거리 휴리스틱

    - FSM 상태 전이 (MonsterFSM.cs)
        Idle   - 제자리에서 회전하며 시야각 내에 플레이어가 들어오는지 검사
        Chase  - 플레이어가 감지되면 추격 시작(플레이어에게 곧바로 이동, 중간에 벽이 있으면 A*알고리즘으로 계산한 경로로 이동)
        Attack - 공격 범위 안에 플레이어가 들어오면 플레이어에게 데미지를 입히고 AttackInterval(3초)이 지나도 공격 범위 안에 플레이어가 있으면 또 다시 데미지를 입힘, Trigger 이벤트

    - SpereCast(MonsterMove.cs)
        몬스터와 플레이어 사이에 Physics.SphereCast로 벽 존재 여부를 확인 -> 벽이 없으면 직선 이동, 있으면 A* 경로를 요청




자료구조 선택 이유

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
        