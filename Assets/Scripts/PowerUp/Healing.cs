using UnityEngine;

public class Healing : PowerUp
{
    GameObject player;
    PlayerHealth playerHealth;
    public AudioSource healingAudio;

    public override void Awake()
    {
        FindPlayer();
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

    //Check object masuk kedalam trigger
    public override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            float dist = Vector3.Distance(other.transform.position, transform.position);
            if (dist < 1.5f)
            {
                PlayerHealth ph = other.GetComponent<PlayerHealth>() ?? playerHealth;
                if (ph != null)
                {
                    ph.Healing();
                }

                if (healingAudio != null)
                {
                    healingAudio.Play();
                }

                gameObject.SetActive(false);
            }
        }
    }
}
