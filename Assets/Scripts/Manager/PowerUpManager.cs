using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public float spawnTime = 7f;

    [SerializeField]
    PowerUpFactory factory;
    IFactory Factory
    {
        get
        {
            return factory as IFactory;
        }
    }

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
            factory = GetComponent<PowerUpFactory>();
            if (factory == null)
                factory = FindObjectOfType<PowerUpFactory>();
        }
    }

    void Start()
    {
        FindReferences();
        //Mengeksekusi fungs Spawn setiap beberapa detik sesuai dengan nilai spawnTime
        InvokeRepeating("Spawn", spawnTime, spawnTime);
    }


    void Spawn()
    {
        if (playerHealth == null || Factory == null)
        {
            FindReferences();
            if (playerHealth == null || Factory == null) return;
        }

        //Jika player telah mati maka tidak membuat enemy baru
        if (playerHealth.currentHealth <= 0f)
        {
            return;
        }

        //Mendapatkan nilai random
        int spawnPoint = Random.Range(0, 3);

        //Memduplikasi enemy
        Factory.FactoryMethod(spawnPoint);
    }
}