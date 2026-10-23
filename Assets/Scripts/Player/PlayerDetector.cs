using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    public GameOverManager gameOverManager;

    void Awake()
    {
        if (gameOverManager == null)
        {
            gameOverManager = FindObjectOfType<GameOverManager>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !other.isTrigger)
        {
            if (gameOverManager == null)
            {
                gameOverManager = FindObjectOfType<GameOverManager>();
            }

            if (gameOverManager != null)
            {
                float enemyDistance = Vector3.Distance(transform.position, other.transform.position);
                gameOverManager.ShowWarning(enemyDistance);
            }
        }
    }
}