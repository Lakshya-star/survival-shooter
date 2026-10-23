using UnityEngine;

public class SpeedUp : PowerUp
{
    GameObject player;
    PlayerMovement playerMovement;
    public AudioSource speedUpAudio;

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
                playerMovement = player.GetComponent<PlayerMovement>();
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
                PlayerMovement pm = other.GetComponent<PlayerMovement>() ?? playerMovement;
                if (pm != null)
                {
                    pm.SpeedUp();
                }

                if (speedUpAudio != null)
                {
                    speedUpAudio.Play();
                }

                gameObject.SetActive(false);
            }
        }
    }
}
