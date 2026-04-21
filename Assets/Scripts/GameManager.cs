using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject startPanel;
    private bool gameStarted = false;

    public bool IsGameOver { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public GameObject gameOverPanel;

    [Header("Score")]
    public float scoreMultiplier = 10f;

    [Header("Audio")]
    public AudioSource musicSource;

    private float score;
    private int bestScore;
    private PlayerController playerController;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = Object.FindAnyObjectByType<PlayerController>();
        IsGameOver = false;
        score = 0f;
        bestScore = PlayerPrefs.GetInt("BestScore", 0);

        if (highScoreText != null)
            highScoreText.text = "High Score: " + Mathf.FloorToInt(bestScore);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (startPanel != null)
            startPanel.SetActive(true);

        Time.timeScale = 0f; // Pause game at start
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameStarted) return;

        if (IsGameOver)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RestartGame();
            }
            return;
        }
        
        if (playerController == null || !playerController.IsAlive()) return;

        score += Time.deltaTime * scoreMultiplier;

        if(scoreText != null)
            scoreText.text = "Score: " + Mathf.FloorToInt(score);
    }

    public void StartGame()
    {
        gameStarted = true;

        if (startPanel != null)
            startPanel.SetActive(false);

        if (musicSource != null)
            musicSource.Play();

        Time.timeScale = 1f; // start game
    }

    public void GameOver()
    {
        IsGameOver = true;

        int finalScore = Mathf.FloorToInt(score);

        if(finalScore > bestScore)
        {
            bestScore = finalScore;
            PlayerPrefs.SetInt("BestScore", bestScore);
            PlayerPrefs.Save();
        }

        if (musicSource != null && musicSource.isPlaying)
            musicSource.Stop();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
