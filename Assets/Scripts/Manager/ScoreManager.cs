using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static int score;

    private Text scoreText;
    private Text levelText;
    private Text highScoreText;

    void Awake ()
    {
        score = 0;
        LevelManager.EnsureExists();
        EnhancePauseMenu();
    }

    void EnhancePauseMenu()
    {
        GameObject pauseMenuEnhancerObj = new GameObject("PauseMenuEnhancer");
        pauseMenuEnhancerObj.AddComponent<PauseMenuEnhancer>();
    }

    void Start ()
    {
        scoreText = GetComponent<Text>();
        if (scoreText == null)
        {
            GameObject existing = GameObject.Find("ScoreText");
            if (existing != null)
                scoreText = existing.GetComponent<Text>();
            else
                CreateScoreText();
        }
        CreateLevelText();
        CreateHighScoreText();
    }

    void CreateScoreText()
    {
        if (scoreText == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                GameObject scoreObj = new GameObject("ScoreText");
                scoreObj.transform.SetParent(canvas.transform, false);

                RectTransform rect = scoreObj.AddComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(-300, 250);
                rect.sizeDelta = new Vector2(250, 50);

                scoreText = scoreObj.AddComponent<Text>();
                scoreText.text = "Score: 0";
                scoreText.font = GetExistingFont();
                scoreText.fontSize = 35;
                scoreText.color = Color.white;
                scoreText.alignment = TextAnchor.MiddleLeft;
            }
        }
    }

    void CreateLevelText()
    {
        if (levelText == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                GameObject levelObj = new GameObject("LevelText");
                levelObj.transform.SetParent(canvas.transform, false);
                
                RectTransform rect = levelObj.AddComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(-300, 200);
                rect.sizeDelta = new Vector2(200, 50);
                
                levelText = levelObj.AddComponent<Text>();
                levelText.text = "Level: 1";
                levelText.font = GetExistingFont();
                levelText.fontSize = 30;
                levelText.color = Color.white;
                levelText.alignment = TextAnchor.MiddleLeft;
            }
        }
    }

    void CreateHighScoreText()
    {
        if (highScoreText == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                GameObject highScoreObj = new GameObject("HighScoreText");
                highScoreObj.transform.SetParent(canvas.transform, false);
                
                RectTransform rect = highScoreObj.AddComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(-300, 150);
                rect.sizeDelta = new Vector2(300, 50);
                
                highScoreText = highScoreObj.AddComponent<Text>();
                highScoreText.text = "High Score: 0";
                highScoreText.font = GetExistingFont();
                highScoreText.fontSize = 24;
                highScoreText.color = Color.yellow;
                highScoreText.alignment = TextAnchor.MiddleLeft;
            }
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

    void Update ()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
        
        if (levelText != null && LevelManager.instance != null)
            levelText.text = "Level: " + LevelManager.instance.currentLevel;
        
        if (highScoreText != null && LevelManager.instance != null)
            highScoreText.text = "High Score: " + LevelManager.instance.GetHighScore();
    }
}
