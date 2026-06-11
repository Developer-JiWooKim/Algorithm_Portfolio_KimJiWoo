using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("Setup Panel")]
    [SerializeField] private GameObject      setupPanel;
    [SerializeField] private TMP_InputField  colsInputField;
    [SerializeField] private TMP_InputField  rowsInputField;
    [SerializeField] private GameObject      errorText;
    [SerializeField] private Button          startButton;

    [Header("참조")]
    [SerializeField] private MazeGenerator mazeGenerator;
    [SerializeField] private UnitSpawner   unitSpawner;

    [Header("입력 제한")]
    [SerializeField] private int minSize = 5;
    [SerializeField] private int maxSize = 20;

    [Header("Game Panel")]
    [SerializeField] private TextMeshProUGUI hpText;

    [Header("Result Panel")]
    [SerializeField] private GameObject      resultPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button          replayButton;
    [SerializeField] private Button          gameEndButton;

    private PlayerController player;

    private void Start()
    {
        InitSetupPanel();
    }

    /// <summary>
    /// Setup Panel 초기화, 게임 시작 전 미로 크기 설정
    /// </summary>
    private void InitSetupPanel()
    {
        colsInputField.text = mazeGenerator.Cols.ToString();
        rowsInputField.text = mazeGenerator.Rows.ToString();
        colsInputField.contentType = TMP_InputField.ContentType.IntegerNumber;
        rowsInputField.contentType = TMP_InputField.ContentType.IntegerNumber;
        errorText.SetActive(false);

        startButton.onClick.AddListener(OnStartButtonClicked);

        // Setup Panel 활성화, 게임 정지
        setupPanel.SetActive(true);
        resultPanel.SetActive(false);
        Time.timeScale = 0f;
    }

    /// <summary>
    /// 시작 버튼 클릭 시 미로 생성 후 게임 시작
    /// </summary>
    private void OnStartButtonClicked()
    {
        if (!int.TryParse(colsInputField.text, out int cols) || !int.TryParse(rowsInputField.text, out int rows))
        {
            errorText.SetActive(true);
            return;
        }

        // 미로 생성 + 유닛 스폰
        mazeGenerator.SetSize(cols, rows);
        mazeGenerator.Generate();
        unitSpawner.SpawnAll();

        // Setup Panel 닫고 게임 시작
        setupPanel.SetActive(false);
        Time.timeScale = 1f;

        // 스폰 후 PlayerController 참조 연결
        player = unitSpawner.Player;
        InitGamePanel();
    }

    /// <summary>
    /// 게임 시작 후 HP UI, Result UI 초기화
    /// </summary>
    private void InitGamePanel()
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
