using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    public bool PlayerInRange { get; private set; }
    public PlayerController Player { get; private set; }

    private void Start()
    {
        PlayerInRange = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerInRange = true;
        Player        = other.GetComponent<PlayerController>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerInRange = false;
        Player        = null;
    }
}
