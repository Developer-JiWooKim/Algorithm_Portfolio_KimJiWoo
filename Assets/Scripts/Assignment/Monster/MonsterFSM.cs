using System;
using UnityEngine;

public class MonsterFSM : MonoBehaviour
{
    public enum State
    {
        Idle,       
        Chase,      
        Attack,
    }

    [SerializeField] private State current;

    public State Current => current;

    public event Action<State, State> OnStateChanged;

    public void Evaluate(bool isSensed, bool isInRange, Vector3 targetPos)
    {
        switch (Current)
        {     
            case State.Idle:
                if (isSensed) TransitionTo(State.Chase);
                break;
            case State.Chase:
                if (!isInRange) TransitionTo(State.Idle);
                break;
            case State.Attack:
                // #TODO: 범위 공격 범위 안에 들어오면 Attack 행동 후 다시 Chase로? Idle갔다가 다시 범위 체크하고 Chase로?
                break;
            default:
                break;
        }
    }

    public void SetAttack(bool isAttacking)
    {
        if (isAttacking)
        {
            TransitionTo(State.Attack);
        }
        else
        {
            TransitionTo(State.Chase);
        }
    }

    // 상태 변경
    private void TransitionTo(State next)
    {
        if (current == next) return;

        State prev = current;
        current = next;

        OnStateChanged?.Invoke(prev, next);
    }
}
