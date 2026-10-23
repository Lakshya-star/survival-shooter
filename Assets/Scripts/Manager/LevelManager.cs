using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    
    public int currentLevel = 1;
    public int scorePerLevel = 100;
    
    private int currentScore;
    private int highScore;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        LoadHighScore();
    }
    
    void Start()
    {
        currentScore = ScoreManager.score;
        UpdateLevel();
    }
    
    void Update()
    {
        currentScore = ScoreManager.score;
        UpdateLevel();
        
        if (currentScore > highScore)
        {
            highScore = currentScore;
            SaveHighScore();
        }
    }
    
    void UpdateLevel()
    {
        int newLevel = (currentScore / scorePerLevel) + 1;
        if (newLevel != currentLevel)
        {
            currentLevel = newLevel;
            Debug.Log("Level Up! Current Level: " + currentLevel);
        }
    }
    
    public float GetEnemyHealthMultiplier()
    {
        return 1f + (currentLevel - 1) * 0.2f;
    }
    
    public float GetEnemyDamageMultiplier()
    {
        return 1f + (currentLevel - 1) * 0.15f;
    }
    
    public float GetSpawnRateMultiplier()
    {
        return Mathf.Max(0.5f, 1f - (currentLevel - 1) * 0.1f);
    }
    
    public bool EnemyHasWeapon()
    {
        return currentLevel >= 3;
    }

    public bool EnemyHasBlaster()
    {
        return currentLevel >= 5;
    }

    public int GetEnemyWeaponDamage()
    {
        int baseDamage = 15;
        if (currentLevel >= 5)
        {
            baseDamage = 25; // Blaster does more damage
        }
        return baseDamage + (currentLevel - 3) * 5;
    }

    public float GetEnemyFireRate()
    {
        // Faster shooting at higher levels
        float baseRate = 1.5f;
        if (currentLevel >= 5)
        {
            baseRate = 1.0f; // Blaster shoots faster
        }
        return Mathf.Max(0.5f, baseRate - (currentLevel - 3) * 0.1f);
    }
    
    void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
    }
    
    void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }
    
    public int GetHighScore()
    {
        return highScore;
    }
    
    public void ResetGame()
    {
        currentLevel = 1;
        currentScore = 0;
    }
    
    public static void EnsureExists()
    {
        if (instance == null)
        {
            GameObject levelManagerObj = new GameObject("LevelManager");
            levelManagerObj.AddComponent<LevelManager>();
        }
    }
}
