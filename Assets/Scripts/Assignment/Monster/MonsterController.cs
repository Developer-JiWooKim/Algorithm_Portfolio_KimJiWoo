using UnityEngine;

/// <summary>
/// 몬스터의 행동을 결정하고 실행하는 컨트롤러
/// </summary>
public class MonsterController : MonoBehaviour
{
    private MonsterSight  _monsterSight;
    private MonsterMove   _monsterMove;
    private MonsterFSM    _monsterFSM;
    private MonsterAttack _monsterAttack;

    private bool _isSensed   = false;
    private bool _isInRange  = false;

    private float _attackInterval = 3f;
    private float _attackTimer = 0;

    private Transform _target;
    public Transform Target
    {
        get => _target;
        set => _target = value;
    }

    private void Start() => Initialize();
    private void Initialize()
    {
        _monsterMove  = GetComponent<MonsterMove>();
        _monsterSight = GetComponent<MonsterSight>();
        _monsterFSM   = GetComponent<MonsterFSM>();

        _monsterAttack = GetComponentInChildren<MonsterAttack>();

        _monsterFSM.OnStateChanged += OnStateChanged;
    }

    private void OnDestroy()
    {
        _monsterFSM.OnStateChanged -= OnStateChanged;
    }

    private void OnStateChanged(MonsterFSM.State prev, MonsterFSM.State next)
    {
        switch (next)
        {
            case MonsterFSM.State.Idle:
                _monsterMove.ClearPath();
                break;
            case MonsterFSM.State.Chase:
                // _monsterMove.RequestPath(_target);
                break;
            case MonsterFSM.State.Attack:
                break;
        }
    }

    private void FixedUpdate()
    {
        if (_target == null) return;

        // 탐지 거리 안에 있는지 체크
        _isInRange = _monsterSight.IsInRange(_target.position); 
        
        if(_isInRange)
        {
            // 탐지 거리 안에 들어와 있으면 시야각 안에 들어왔는지 체크
            _isSensed = _monsterSight.TargetSense(_target.position);
        }
        else
        {
            _isSensed = false; // 범위 밖이면 감지 여부 초기화
        }
    }

    private void Update()
    {
        if (_target == null) return;

        _monsterFSM.Evaluate(_isSensed, _isInRange, _target.position);

        MonsterAI();
    }

    private void MonsterAI()
    {
        switch (_monsterFSM.Current)
        {
            case MonsterFSM.State.Idle:
                _monsterMove.IdleRotate();
                break;
            case MonsterFSM.State.Chase:
                _monsterMove.MoveToTarget(_target.position);
                if(_monsterAttack.PlayerInRange)
                {
                    _monsterFSM.SetAttack(true);
                }
                break;
            case MonsterFSM.State.Attack:
                _monsterMove.LookAtTarget(_target.position);
                if(!_monsterAttack.PlayerInRange)
                {
                    _monsterFSM.SetAttack(false);
                }
                else
                {
                    TryAttack();
                }
                break;
        }
    }

    private void TryAttack()
    {
        _attackTimer -= Time.deltaTime;
        if (_attackTimer > 0f) return;

        _attackTimer = _attackInterval;
        _monsterAttack.Player?.TakeDamage();
    }
}
