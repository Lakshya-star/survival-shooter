using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public float restartDelay = 5f;
    public Text warningText;
    private Text finalScoreText;
    private Text highScoreText;


    Animator anim;
    bool restartScheduled;


    void Awake()
    {
        anim = GetComponent<Animator>();

        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerHealth = player.GetComponent<PlayerHealth>();
            else
                playerHealth = FindObjectOfType<PlayerHealth>();
        }

        // Completely remove the warning text
        GameObject warning = GameObject.Find("Warning");
        if (warning != null)
        {
            Destroy(warning);
        }

        // Also remove DamageImage if it exists (might show the !)
        GameObject damageImage = GameObject.Find("DamageImage");
        if (damageImage != null)
        {
            damageImage.SetActive(false);
        }

        CreateFinalScoreText();
        CreateHighScoreText();
    }

    void CreateFinalScoreText()
    {
        if (finalScoreText != null) return;

        Transform parent = null;
        GameObject gameOverText = GameObject.Find("GameOverText");
        if (gameOverText != null)
        {
            parent = gameOverText.transform.parent;
        }
        else
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null) parent = canvas.transform;
        }

        if (parent != null)
        {
            GameObject finalScoreObj = new GameObject("FinalScoreText");
            finalScoreObj.transform.SetParent(parent, false);

            RectTransform rect = finalScoreObj.AddComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, -50);
            rect.sizeDelta = new Vector2(300, 50);

            finalScoreText = finalScoreObj.AddComponent<Text>();
            finalScoreText.text = "Score: 0";
            finalScoreText.font = GetExistingFont();
            finalScoreText.fontSize = 36;
            finalScoreText.color = Color.white;
            finalScoreText.alignment = TextAnchor.MiddleCenter;

            finalScoreObj.SetActive(false);
        }
    }

    void CreateHighScoreText()
    {
        if (highScoreText != null) return;

        Transform parent = null;
        GameObject gameOverText = GameObject.Find("GameOverText");
        if (gameOverText != null)
        {
            parent = gameOverText.transform.parent;
        }
        else
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null) parent = canvas.transform;
        }

        if (parent != null)
        {
            GameObject highScoreObj = new GameObject("GameOverHighScoreText");
            highScoreObj.transform.SetParent(parent, false);

            RectTransform rect = highScoreObj.AddComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, -100);
            rect.sizeDelta = new Vector2(350, 50);

            highScoreText = highScoreObj.AddComponent<Text>();
            highScoreText.text = "High Score: 0";
            highScoreText.font = GetExistingFont();
            highScoreText.fontSize = 28;
            highScoreText.color = new Color(1f, 0.8f, 0f);
            highScoreText.alignment = TextAnchor.MiddleCenter;

            highScoreObj.SetActive(false);
        }
    }

    Font GetExistingFont()
    {
        // Try to find existing Text component with the game's font
        Text[] allTexts = FindObjectsOfType<Text>();
        foreach (Text text in allTexts)
        {
            if (text.font != null && text.font.name.Contains("Luckiest"))
            {
                return text.font;
            }
        }
        // Fallback to Arial if not found
        return Resources.GetBuiltinResource<Font>("Arial.ttf");
    }

    void Update()
    {
        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerHealth = player.GetComponent<PlayerHealth>();
        }

        // Only trigger Game Over after all 3 lives have been lost
        if (playerHealth != null && playerHealth.CurrentLives <= 0 && playerHealth.currentHealth <= 0)
        {
            if (!restartScheduled)
            {
                restartScheduled = true;
                if (anim != null)
                    anim.SetTrigger("GameOver");
                StartCoroutine(RestartAfterDelay());
            }

            if (finalScoreText != null)
            {
                finalScoreText.gameObject.SetActive(true);
                finalScoreText.text = "Score: " + ScoreManager.score;
            }
            
            if (highScoreText != null)
            {
                highScoreText.gameObject.SetActive(true);
                if (LevelManager.instance != null)
                {
                    highScoreText.text = "High Score: " + LevelManager.instance.GetHighScore();
                }
            }

        }
    }

    System.Collections.IEnumerator RestartAfterDelay()
    {
        // Real time keeps the restart working even if the pause state was
        // active when the player lost their final life.
        yield return new WaitForSecondsRealtime(restartDelay);

        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (LevelManager.instance != null)
            LevelManager.instance.ResetGame();

        ScoreManager.score = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowWarning(float enemyDistance)
    {
        // Warning completely disabled - does nothing
        return;
    }
}
