using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour, IFactory
{
    [SerializeField]
    public GameObject[] enemyPrefab;
    public Transform[] spawnPoints;

    [Range(0f, 1f)] public float armedEnemyChance = 0.4f;
    [Range(0f, 1f)] public float blasterChanceForArmedEnemies = 0.25f;

    void Awake()
    {
        EnsureSpawnPoints();
    }

    void EnsureSpawnPoints()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            List<Transform> points = new List<Transform>();
            for (int i = 1; i <= 3; i++)
            {
                GameObject sp = GameObject.Find("SpawnPoint" + i) ?? GameObject.Find("Spawn Point " + i);
                if (sp != null) points.Add(sp.transform);
            }
            if (points.Count == 0)
            {
                Vector3[] defaultPositions = new Vector3[] {
                    new Vector3(0, 0, 15),
                    new Vector3(15, 0, -10),
                    new Vector3(-15, 0, -10)
                };
                for (int i = 0; i < defaultPositions.Length; i++)
                {
                    GameObject sp = new GameObject("AutoEnemySpawnPoint" + (i + 1));
                    sp.transform.position = defaultPositions[i];
                    points.Add(sp.transform);
                }
            }
            spawnPoints = points.ToArray();
        }
    }

    public GameObject FactoryMethod(int tag)
    {
        if (enemyPrefab == null || enemyPrefab.Length == 0)
        {
            return null;
        }

        EnsureSpawnPoints();
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return null;
        }

        int prefabIdx = Mathf.Clamp(tag, 0, enemyPrefab.Length - 1);
        int spawnIdx = Mathf.Clamp(tag, 0, spawnPoints.Length - 1);

        if (enemyPrefab[prefabIdx] == null || spawnPoints[spawnIdx] == null)
        {
            return null;
        }

        GameObject enemy = Instantiate(enemyPrefab[prefabIdx], spawnPoints[spawnIdx].position, spawnPoints[spawnIdx].rotation);
        
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        EnemyAttack enemyAttack = enemy.GetComponent<EnemyAttack>();
        
        // Set score values based on enemy type
        if (enemyHealth != null)
        {
            // Zombunny (index 0) and ZomBear (index 1) are small enemies - 10 points
            // Hellephant (index 2) is big enemy - 20 points
            if (tag == 2)
            {
                enemyHealth.scoreValue = 20;
            }
            else
            {
                enemyHealth.scoreValue = 10;
            }
        }
        
        if (enemyHealth != null && LevelManager.instance != null)
        {
            float healthMultiplier = LevelManager.instance.GetEnemyHealthMultiplier();
            enemyHealth.startingHealth = Mathf.RoundToInt(enemyHealth.startingHealth * healthMultiplier);
            enemyHealth.currentHealth = enemyHealth.startingHealth;
        }
        
        if (enemyAttack != null && LevelManager.instance != null)
        {
            float damageMultiplier = LevelManager.instance.GetEnemyDamageMultiplier();
            enemyAttack.attackDamage = Mathf.RoundToInt(enemyAttack.attackDamage * damageMultiplier);
        }
        
        // Ranged enemies are intentionally occasional.  Only a few of the
        // armed group become blasters, keeping higher levels challenging but fair.
        bool getsWeapon = LevelManager.instance != null &&
                          LevelManager.instance.EnemyHasWeapon() &&
                          Random.value < armedEnemyChance;

        if (getsWeapon)
        {
            EnemyShooting enemyShooting = enemy.GetComponent<EnemyShooting>();
            if (enemyShooting == null)
            {
                enemyShooting = enemy.AddComponent<EnemyShooting>();
                Debug.Log("Added EnemyShooting to enemy at level " + LevelManager.instance.currentLevel);
            }
            enemyShooting.damagePerShot = LevelManager.instance.GetEnemyWeaponDamage();
            enemyShooting.timeBetweenBullets = LevelManager.instance.GetEnemyFireRate();
            enemyShooting.range = 8f;
            enemyShooting.detectionRange = 8f;

            // Add blaster effects at higher levels
            if (LevelManager.instance.EnemyHasBlaster() && Random.value < blasterChanceForArmedEnemies)
            {
                enemyShooting.isBlaster = true;
                Debug.Log("Enemy has blaster at level " + LevelManager.instance.currentLevel);
            }

            // Note: EnemyShooting.InitializeEffects() handles all visual effects automatically
        }
        
        return enemy;
    }
}
