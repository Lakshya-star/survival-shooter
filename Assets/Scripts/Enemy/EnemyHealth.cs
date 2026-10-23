using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
    public float sinkSpeed = 2.5f;
    public int scoreValue = 10;
    public AudioClip deathClip;


    Animator anim;
    AudioSource enemyAudio;
    ParticleSystem hitParticles;
    CapsuleCollider capsuleCollider;
    bool isDead;
    bool isSinking;


    void Awake()
    {
        //Menapatkan reference komponen
        anim = GetComponent<Animator>();
        enemyAudio = GetComponent<AudioSource>();
        hitParticles = GetComponentInChildren<ParticleSystem>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        //Set current health
        currentHealth = startingHealth;
    }


    void Update()
    {
        //Check jika sinking
        if (isSinking)
        {
            //memindahkan object kebawah
            transform.Translate(-Vector3.up * sinkSpeed * Time.deltaTime);
        }
    }


    public void TakeDamage(int amount, Vector3 hitPoint)
    {
        //Check jika dead
        if (isDead)
            return;

        //play audio
        if (enemyAudio != null)
            enemyAudio.Play();

        //kurangi health
        currentHealth -= amount;

        //Ganti posisi particle
        if (hitParticles != null)
        {
            hitParticles.transform.position = hitPoint;
            hitParticles.Play();
        }

        //Dead jika health <= 0
        if (currentHealth <= 0)
        {
            Death();
        }
    }


    void Death()
    {
        //set isdead
        isDead = true;

        //SetCapcollider ke trigger
        if (capsuleCollider != null)
            capsuleCollider.isTrigger = true;

        //trigger play animation Dead
        if (anim != null)
            anim.SetTrigger("Dead");

        //Play Sound Dead
        if (enemyAudio != null)
        {
            if (deathClip != null)
                enemyAudio.clip = deathClip;
            enemyAudio.Play();
        }
    }

    public void StartSinking()
    {
        //disable Navmesh Component
        var nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (nav != null) nav.enabled = false;

        //Set rigisbody ke kimematic
        var rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        isSinking = true;
        ScoreManager.score += scoreValue;
        Destroy(gameObject, 2f);
    }
}