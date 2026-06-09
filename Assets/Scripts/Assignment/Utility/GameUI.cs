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

    private void Start() => Initialize();

    /// <summary>
    /// 초기화 메소드
    /// </summary>
    private void Initialize()
    {
        // 이벤트 등록
        player.OnHPChanged += UpdateHp;
        player.OnDead += () => GameManager.Instance.GameOver();

        GameManager.Instance.OnClear += () => ShowResult("CLEAR!!");
        GameManager.Instance.OnGameOver += () => ShowResult("GAME OVER..");

        // UI 버튼에 이벤트 등록
        replayButton.onClick.AddListener(GameManager.Instance.Replay);
        gameEndButton.onClick.AddListener(GameManager.Instance.GameEnd);

        // 결과 화면 비활성화, 최초 게임 시작 시 체력 UI 업데이트
        resultPanel.SetActive(false);
        UpdateHp(player.CurrentHp, player.MaxHp);
    }

    /// <summary>
    /// 결과 화면 UI, 결과 UI 띄우면서 플레이어 입력 막기 위해 PlayerInput 컴포넌트 비활성화
    /// </summary>
    private void ShowResult(string message)
    {
        resultText.text = message;
        resultPanel.SetActive(true);

        player.GetComponent<PlayerInput>().enabled = false;
    }

    /// <summary>
    /// 체력 UI 갱신 메소드
    /// </summary>
    private void UpdateHp(int current, int max)
    {
        hpText.text = $"HP : {current} / {max}";
    }
}
