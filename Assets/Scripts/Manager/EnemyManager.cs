using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public float baseSpawnTime = 3f;

    [SerializeField]
    EnemyFactory factory;
    IFactory Factory
    {
        get
        {
            return factory as IFactory;
        }
    }

    private float currentSpawnTime;
    private float spawnTimer;
    private int lastLevel = 1;

    void Awake()
    {
        FindReferences();
    }

    void FindReferences()
    {
        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerHealth = player.GetComponent<PlayerHealth>();
            else
                playerHealth = FindObjectOfType<PlayerHealth>();
        }

        if (factory == null)
        {
            factory = GetComponent<EnemyFactory>();
            if (factory == null)
                factory = FindObjectOfType<EnemyFactory>();
        }
    }

    void Start()
    {
        FindReferences();
        currentSpawnTime = baseSpawnTime;
        spawnTimer = 0f;
    }

    void Update()
    {
        if (LevelManager.instance != null && LevelManager.instance.currentLevel != lastLevel)
        {
            float spawnRateMultiplier = LevelManager.instance.GetSpawnRateMultiplier();
            currentSpawnTime = baseSpawnTime * spawnRateMultiplier;
            lastLevel = LevelManager.instance.currentLevel;
        }

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= currentSpawnTime)
        {
            Spawn();
            spawnTimer = 0f;
        }
    }

    void Spawn()
    {
        if (playerHealth == null || Factory == null)
        {
            FindReferences();
            if (playerHealth == null || Factory == null)
            {
                return;
            }
        }

        //Jika player telah mati maka tidak membuat enemy baru
        if (playerHealth.currentHealth <= 0f)
        {
            return;
        }

        //Mendapatkan nilai random
        int spawnEnemy = Random.Range(0, 3);

        //Memduplikasi enemy
        Factory.FactoryMethod(spawnEnemy);
    }
}