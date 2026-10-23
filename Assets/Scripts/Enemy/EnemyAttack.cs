using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    public float timeBetweenAttacks = 0.5f;
    public int attackDamage = 10;

    Animator anim;
    GameObject player;
    PlayerHealth playerHealth;
    EnemyHealth enemyHealth;
    bool playerInRange;
    float timer;


    void Awake()
    {
        FindPlayer();
        anim = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
    }

    void FindPlayer()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<PlayerHealth>();
            }
        }
    }

    //Callback jika ada suatu object masuk kedalam trigger
    void OnTriggerEnter(Collider other)
    {
        if (player == null) FindPlayer();

        //Set player in range
        if (other.gameObject == player && !other.isTrigger)
        {
            playerInRange = true;
        }
    }

    //Callback jika ada object yang keluar dari trigger
    void OnTriggerExit(Collider other)
    {
        //Set player jika tidak dalam range
        if (other.gameObject == player)
        {
            playerInRange = false;
        }
    }

    void Update()
    {
        if (player == null || playerHealth == null)
        {
            FindPlayer();
            if (playerHealth == null) return;
        }

        timer += Time.deltaTime;

        if (timer >= timeBetweenAttacks && playerInRange && (enemyHealth == null || enemyHealth.currentHealth > 0))
        {
            Attack();
        }

        //mentrigger animasi PlayerDead jika darah player kurang dari sama dengan 0
        if (playerHealth.currentHealth <= 0 && anim != null)
        {
            anim.SetTrigger("PlayerDead");
        }
    }

    void Attack()
    {
        //Reset timer
        timer = 0f;

        //Taking Damage
        if (playerHealth != null && playerHealth.currentHealth > 0)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }
}