using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpFactory : MonoBehaviour, IFactory
{
    [SerializeField]
    public GameObject[] powerUp;
    public Transform[] spawnPoints;

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
                GameObject sp = GameObject.Find("PowerUpSpawn" + i) ?? GameObject.Find("PowerUpSpawnPoint" + i);
                if (sp != null) points.Add(sp.transform);
            }
            if (points.Count == 0)
            {
                Vector3[] defaultPositions = new Vector3[] {
                    new Vector3(5, 0, 5),
                    new Vector3(-5, 0, 5),
                    new Vector3(0, 0, -5)
                };
                for (int i = 0; i < defaultPositions.Length; i++)
                {
                    GameObject sp = new GameObject("AutoPowerUpSpawnPoint" + (i + 1));
                    sp.transform.position = defaultPositions[i];
                    points.Add(sp.transform);
                }
            }
            spawnPoints = points.ToArray();
        }
    }

    public GameObject FactoryMethod(int tag)
    {
        if (powerUp == null || powerUp.Length == 0)
        {
            return null;
        }

        EnsureSpawnPoints();
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return null;
        }

        int spawnIdx = Mathf.Clamp(tag, 0, spawnPoints.Length - 1);
        int powerUpIdx = Random.Range(0, powerUp.Length);

        if (powerUp[powerUpIdx] == null || spawnPoints[spawnIdx] == null)
        {
            return null;
        }

        GameObject powerUpObject = Instantiate(powerUp[powerUpIdx], spawnPoints[spawnIdx].position, spawnPoints[spawnIdx].rotation);
        return powerUpObject;
    }
}
