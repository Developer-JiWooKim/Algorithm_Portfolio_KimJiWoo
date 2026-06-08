using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private int   maxHp     = 3;

    private PlayerInput _playerInput;
    private PlayerMove  _playerMove;

    private int _currentHp;
    public int CurrentHp => _currentHp;
    public int MaxHp => maxHp;

    public System.Action<int, int> OnHPChanged;  // (현재, 최대)
    public System.Action OnDead;

    private void Awake() => Initialize();

    private void Initialize()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerMove  = GetComponent<PlayerMove>();

        _currentHp = maxHp;
    }

    private void Update()
    {
        Vector3 dir = new Vector3(_playerInput.InputVector.x, 0, _playerInput.InputVector.y);

        _playerMove.Move(dir, moveSpeed);
    }

    public void TakeDamage()
    {
        if (_currentHp <= 0) return;

        _currentHp--;

        OnHPChanged?.Invoke(_currentHp, maxHp);

        if (_currentHp <= 0) OnDead?.Invoke();
    }
}
