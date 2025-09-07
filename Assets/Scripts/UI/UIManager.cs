using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private Dictionary<string, GameObject> panels = new Dictionary<string, GameObject>();
    private int score = 0;
    private bool isGameOver = false;    

    [Header("UI面板")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI scorePanelText;
    public TextMeshProUGUI gameOverScoreText;   

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        foreach (Transform child in transform)
        {
            panels.Add(child.name, child.gameObject);
            child.gameObject.SetActive(false);
        }
        ShowPanel("ScoreText");
        ResetScore();
        HideGameOverPanel();
    }
    
    private void OnEnable()
    {
        GameEvents.OnScoreChanged += AddScore;
        GameEvents.OnGameOver += HandleGameOver;
        GameEvents.OnGameStart += HandleGameStart;
    }
    private void OnDisable()
    {
        GameEvents.OnScoreChanged -= AddScore;
        GameEvents.OnGameOver -= HandleGameOver;
        GameEvents.OnGameStart -= HandleGameStart;
    }
    #region 面板控制
    public void ShowPanel(string panelName)
    {
        if (panels.ContainsKey(panelName))
        {
            panels[panelName].SetActive(true);
        }
        else
        {
            Debug.LogWarning("Panel not found: " + panelName);
        }
    }   

    public void HidePanel(string panelName)
    {
        if (panels.ContainsKey(panelName))
        {
            panels[panelName].SetActive(false);
        }
        else
        {
            Debug.LogWarning("Panel not found: " + panelName);
        }
    }
    private void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        //gameOverScoreText.text = score.ToString();  
    }

    private void HideGameOverPanel()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        //gameOverScoreText.text = score.ToString();
    }
    #endregion
    #region 分数控制
    private void AddScore(int value)
    {
        if (isGameOver) return;  // 游戏结束不增加分数
        score += value;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scorePanelText != null)
            scorePanelText.text = score.ToString();
    }

    private void ResetScore()
    {
        score = 0;
        UpdateScoreText();
    }
    #endregion

    #region 游戏状态
    private void HandleGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        Time.timeScale = 0f;
        ShowGameOverPanel();

        Debug.Log("Game Over!");
    }

    private void HandleGameStart()
    {
        isGameOver = false;

        // 恢复时间流速
        Time.timeScale = 1f;

        // 重置分数
        ResetScore();

        // 隐藏 GameOver 面板
        HideGameOverPanel();

        // 重新加载当前场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #endregion

    #region 按钮事件
    // 挂在按钮上的方法
    public void OnRestartButton()
    {
        GameEvents.OnGameStart?.Invoke();
    }
    public void OnExitButton()
    {
        //GameEvents.OnGameStart?.Invoke();
    }
    #endregion
}
