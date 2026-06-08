using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    [SerializeField] private TextMeshProUGUI  hpText; 

    [SerializeField] private GameObject      resultPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button          replayButton;
    [SerializeField] private Button          gameEndButton;

    private void Start()
    {
        player.OnHPChanged += UpdateHp;
        player.OnDead += () => GameManager.Instance.GameOver();

        GameManager.Instance.OnClear += () => ShowResult("CLEAR!!");
        GameManager.Instance.OnGameOver += () => ShowResult("GAME OVER..");

        replayButton.onClick.AddListener(GameManager.Instance.Replay);
        gameEndButton.onClick.AddListener(GameManager.Instance.GameEnd);

        resultPanel.SetActive(false);
        UpdateHp(player.CurrentHp, player.MaxHp);
    }

    private void ShowResult(string message)
    {
        resultText.text = message;
        resultPanel.SetActive(true);

        player.GetComponent<PlayerInput>().enabled = false;
    }

    private void UpdateHp(int current, int max)
    {
        hpText.text = $"HP : {current} / {max}";
    }
}
