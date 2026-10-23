using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    Transform player;
    PlayerHealth playerHealth;
    EnemyHealth enemyHealth;
    UnityEngine.AI.NavMeshAgent nav;


    void Awake()
    {
        FindPlayer();
        enemyHealth = GetComponent<EnemyHealth>();
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    void FindPlayer()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerHealth = playerObj.GetComponent<PlayerHealth>();
            }
        }
    }

    void Update()
    {
        if (player == null || playerHealth == null)
        {
            FindPlayer();
            if (player == null) return;
        }

        bool enemyAlive = enemyHealth == null || enemyHealth.currentHealth > 0;
        bool playerAlive = playerHealth != null && playerHealth.currentHealth > 0;

        // Resume navigation after a player respawn.  Older behavior disabled
        // the agent permanently while the player was briefly at zero health.
        if (enemyAlive && nav != null && !nav.enabled)
            nav.enabled = true;

        //Pindah ke player position
        if (enemyAlive && playerAlive)
        {
            if (nav != null && nav.isActiveAndEnabled && nav.isOnNavMesh)
            {
                nav.isStopped = false;
                nav.SetDestination(player.position);
            }
        }
        else //Stop moving without disabling navigation permanently
        {
            if (nav != null && nav.isActiveAndEnabled)
            {
                nav.isStopped = true;
            }
        }
    }
}
