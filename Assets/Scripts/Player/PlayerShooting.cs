using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public int damagePerShot = 20;
    public float timeBetweenBullets = 0.15f;
    public float range = 100f;


    float timer;
    Ray shootRay = new Ray();
    RaycastHit shootHit;
    int shootableMask;
    ParticleSystem gunParticles;
    LineRenderer gunLine;
    AudioSource gunAudio;
    Light gunLight;
    float effectsDisplayTime = 0.2f;


    void Awake()
    {
        //GetMask
        shootableMask = LayerMask.GetMask("Shootable");
        if (shootableMask == 0)
        {
            shootableMask = ~LayerMask.GetMask("Ignore Raycast");
        }

        //Mendapatkan Reference component
        gunParticles = GetComponent<ParticleSystem>();
        gunLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
        gunLight = GetComponent<Light>();
    }


    void Update()
    {
        timer += Time.deltaTime;

        bool isShooting = false;
        if (Input.touchCount > 0)
        {
            // Mobile touch: strictly shoot only from the dedicated right shoot pad
            isShooting = MobileInputTracker.IsShooting;
        }
        else
        {
            // Desktop mouse / keyboard
            isShooting = MobileInputTracker.IsShooting || Input.GetButton("Fire1");
        }

        if (isShooting && timer >= timeBetweenBullets && Time.timeScale != 0)
        {
            Shoot();
        }

        if (timer >= timeBetweenBullets * effectsDisplayTime)
        {
            DisableEffects();
        }
    }


    public void DisableEffects()
    {
        //disable line renderer
        if (gunLine != null)
            gunLine.enabled = false;

        //disable light
        if (gunLight != null)
            gunLight.enabled = false;
    }


    public void Shoot()
    {
        timer = 0f;

        //Play audio
        if (gunAudio != null)
            gunAudio.Play();

        //enable Light
        if (gunLight != null)
            gunLight.enabled = true;

        //Play gun particle
        if (gunParticles != null)
        {
            gunParticles.Stop();
            gunParticles.Play();
        }

        //enable Line renderer dan set first position
        if (gunLine != null)
        {
            gunLine.enabled = true;
            gunLine.SetPosition(0, transform.position);
        }

        //Set posisi ray shoot dan direction
        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        if (shootableMask == 0)
        {
            shootableMask = LayerMask.GetMask("Shootable");
            if (shootableMask == 0) shootableMask = ~LayerMask.GetMask("Ignore Raycast");
        }

        //Lakukan raycast jika mendeteksi id nemy hit apapun
        if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
        {
            //Lakukan raycast hit hace component Enemyhealth
            EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                //Lakukan Take Damage
                enemyHealth.TakeDamage(damagePerShot, shootHit.point);
            }

            //Set line end position ke hit position
            if (gunLine != null)
                gunLine.SetPosition(1, shootHit.point);
        }
        else
        {
            //set line end position ke range freom barrel
            if (gunLine != null)
                gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
        }
    }
}
