using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
    public TextMeshProUGUI gameOverHighScoreText;
    public TextMeshProUGUI gameOverFinalScoreText;
    public GameObject healthUI;
    private Slider healthBar;


    [Header("Score")]
    public float scoreMultiplier = 10f;

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioSource gameOverAudioSource;
    public AudioSource buttonAudioSource;
    public AudioClip gameOverMusicClip;
    public AudioClip buttonClickClip;

    [Header("Start Fade")]
    public CanvasGroup startPanelCanvasGroup;
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 0.5f;

    private float score;
    private int bestScore;
    private PlayerController playerController;

    public Volume globalVolume;
    private Vignette vignette;

    public float gameOverDelay = 1.2f;

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
        healthBar = healthUI.GetComponentInChildren<Slider>();

        if (highScoreText != null)
            highScoreText.text = "High Score: " + Mathf.FloorToInt(bestScore);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (startPanel != null)
            startPanel.SetActive(true);

        if (healthBar != null)
            healthUI.SetActive(false);
        
        Time.timeScale = 0f; // Pause game at start

        globalVolume.profile.TryGet(out vignette);
        if(vignette != null)
        {
            vignette.intensity.value = 0f;
        }

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
        StartCoroutine(StartGameRoutine());

        PlayButtonClickSound();
    }

    public void GameOver()
    {
        if (IsGameOver) return;

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

        StartCoroutine(ShowGameOverAfterDelay());
    }

    void PlayButtonClickSound()
    {
        if(buttonAudioSource != null && buttonClickClip != null)
        {
            buttonAudioSource.PlayOneShot(buttonClickClip);
        }
    }

    void PlayGameOverMusic()
    {
        if(gameOverAudioSource != null && gameOverMusicClip != null)
        {
            gameOverAudioSource.PlayOneShot(gameOverMusicClip);
        }
    }

    IEnumerator StartGameRoutine()
    {
        // prevent double taps
        if (startPanelCanvasGroup != null)
        {
            startPanelCanvasGroup.interactable = false;
            startPanelCanvasGroup.blocksRaycasts = false;
            startPanelCanvasGroup.alpha = 1f; // keep title visible while fading to black
        }

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = true;
        }

        // Fade TO black over the title screen
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            if (fadeCanvasGroup != null)
                fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);

            yield return null;
        }

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 1f;

        // Now screen is fully black. Switch from title to gameplay.
        if (startPanel != null)
            startPanel.SetActive(false);

        gameStarted = true;

        if (healthUI != null)
            healthUI.SetActive(true);

        if (musicSource != null)
            musicSource.Play();

        Time.timeScale = 1f;

        // Fade FROM black into gameplay
        timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            if (fadeCanvasGroup != null)
                fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);

            yield return null;
        }

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }

    IEnumerator ShowGameOverAfterDelay()
    {
        yield return new WaitForSecondsRealtime(gameOverDelay);

        PlayGameOverMusic();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            gameOverFinalScoreText.text = ((int)score).ToString();
            gameOverHighScoreText.text = bestScore.ToString();
        }

        if (vignette != null)
        {
            StartCoroutine(FadeVignette());
        }
    }

    IEnumerator FadeVignette()
    {
        float vignetteFadeSpeed = 0.50f;
        while(vignette.intensity.value < 0.5f)
        {
            vignette.intensity.value += vignetteFadeSpeed * Time.unscaledDeltaTime;
            yield return null;
        }
    }

    public void RestartGame()
    {
        PlayButtonClickSound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

    }
}
