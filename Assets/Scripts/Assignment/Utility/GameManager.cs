using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event System.Action OnClear;
    public event System.Action OnGameOver;

    private bool _isGameEnd = false;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }        
    }

    public void Clear()
    {
        if (_isGameEnd) return;

        _isGameEnd = true;

        // 모든 몬스터 타겟 제거
        foreach (var monster in FindObjectsByType<MonsterController>())
        {
            monster.Target = null;
        }           

        OnClear?.Invoke();
    }

    public void GameOver()
    {
        if (_isGameEnd) return;

        _isGameEnd = true;

        OnGameOver?.Invoke();
    }

    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameEnd()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
