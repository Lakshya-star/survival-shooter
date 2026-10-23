using UnityEngine;
using System.Collections;

public class EnemyShooting : MonoBehaviour
{
    public int damagePerShot = 15;
    public float timeBetweenBullets = 1.5f;
    public float range = 8f;
    public float detectionRange = 8f;
    public bool isBlaster = false;

    float timer;
    Ray shootRay = new Ray();
    RaycastHit shootHit;
    int shootableMask;
    ParticleSystem gunParticles;
    LineRenderer gunLine;
    LineRenderer gunLineGlow;  // wider soft-glow backing line
    AudioSource gunAudio;
    Light gunLight;
    float effectsDisplayTime = 0.5f;  // seconds beam stays visible

    Transform player;
    PlayerHealth playerHealth;
    EnemyHealth enemyHealth;
    Animator anim;
    private bool effectsInitialized = false;

    public GameObject projectilePrefab;

    void Awake()
    {
        shootableMask = LayerMask.GetMask("Shootable");
        if (shootableMask == 0) shootableMask = ~0;

        anim = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();

        FindPlayer();
        InitializeEffects();
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

    void InitializeEffects()
    {
        if (effectsInitialized)
            return;

        // Get or create components
        gunParticles = GetComponentInChildren<ParticleSystem>();
        gunAudio = gameObject.AddComponent<AudioSource>();
        gunLight = null;

        // --- Core laser line (sharp, bright) ---
        gunLine = gameObject.AddComponent<LineRenderer>();
        gunLine.positionCount = 2;
        gunLine.startWidth = 0.22f;
        gunLine.endWidth = 0.12f;
        gunLine.enabled = false;
        gunLine.material = new Material(Shader.Find("Unlit/Color"));
        gunLine.startColor = new Color(1f, 0.15f, 0f, 1f);   // hot orange-red core
        gunLine.endColor   = new Color(1f, 0f, 0f, 0.8f);
        gunLine.useWorldSpace = true;
        gunLine.sortingOrder = 2;

        // --- Glow backing line (wider, softer) ---
        GameObject glowObj = new GameObject("LaserGlow");
        glowObj.transform.SetParent(transform);
        gunLineGlow = glowObj.AddComponent<LineRenderer>();
        gunLineGlow.positionCount = 2;
        gunLineGlow.startWidth = 0.55f;
        gunLineGlow.endWidth  = 0.35f;
        gunLineGlow.enabled   = false;
        gunLineGlow.material  = new Material(Shader.Find("Unlit/Color"));
        gunLineGlow.startColor = new Color(1f, 0f, 0f, 0.35f);
        gunLineGlow.endColor   = new Color(1f, 0f, 0f, 0f);
        gunLineGlow.useWorldSpace = true;
        gunLineGlow.sortingOrder = 1;

        // Create AudioSource
        gunAudio.playOnAwake = false;
        gunAudio.spatialBlend = 1f;

        // Create muzzle light - position at front of enemy
        GameObject lightObj = new GameObject("MuzzleLight");
        lightObj.transform.SetParent(transform);
        lightObj.transform.localPosition = new Vector3(0, 1.2f, 0.8f);
        gunLight = lightObj.AddComponent<Light>();
        gunLight.type = LightType.Point;
        gunLight.range = 10f;
        gunLight.intensity = 5f;
        gunLight.color = Color.red;
        gunLight.enabled = false;

        // Create ParticleSystem for muzzle flash
        GameObject muzzleObj = new GameObject("MuzzleFlash");
        muzzleObj.transform.SetParent(transform);
        muzzleObj.transform.localPosition = new Vector3(0, 1.2f, 0.8f);
        gunParticles = muzzleObj.AddComponent<ParticleSystem>();

        var main = gunParticles.main;
        main.duration = 0.15f;
        main.loop = false;
        main.startLifetime = 0.3f;
        main.startSpeed = 8f;
        main.startSize = 0.6f;
        main.startColor = Color.red;
        main.playOnAwake = false;

        var emission = gunParticles.emission;
        emission.rateOverTime = 0;
        emission.SetBurst(0, new ParticleSystem.Burst(0, 30));

        var shape = gunParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 30f;
        shape.radius = 0.2f;

        effectsInitialized = true;
        Debug.Log("Enemy weapon effects initialized for: " + gameObject.name);
    }

    void Update()
    {
        if (player == null || playerHealth == null)
        {
            FindPlayer();
            if (player == null) return;
        }

        timer += Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (timer >= timeBetweenBullets && distanceToPlayer <= detectionRange &&
            (enemyHealth == null || enemyHealth.currentHealth > 0) &&
            (playerHealth == null || playerHealth.currentHealth > 0))
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
        if (gunLine != null)
            gunLine.enabled = false;
        if (gunLineGlow != null)
            gunLineGlow.enabled = false;
        if (gunLight != null)
            gunLight.enabled = false;
    }

    void Shoot()
    {
        timer = 0f;

        // Trigger shooting animation
        if (anim != null)
        {
            anim.SetTrigger("Shoot");
        }

        // Play shooting sound
        if (gunAudio != null)
            gunAudio.Play();

        // Enable muzzle flash light
        if (gunLight != null)
        {
            gunLight.enabled = true;

            // Blaster has more intense effects
            if (isBlaster)
            {
                gunLight.intensity = 3f;
                gunLight.color = Color.cyan;
            }
            else
            {
                gunLight.intensity = 2f;
                gunLight.color = Color.red;
            }
        }

        // Play particle effects
        if (gunParticles != null)
        {
            gunParticles.Stop();
            gunParticles.Play();
        }

        // Calculate direction to player
        Vector3 direction = (player.position - transform.position).normalized;
        Vector3 spawnPosition = transform.position + Vector3.up * 1f + direction * 0.5f;

        // Both weapon types use visible travelling bolts.  This makes enemy
        // damage readable at a distance instead of flashing a red hitscan line.
        FireProjectile(spawnPosition, direction);
    }

    void FireProjectile(Vector3 spawnPosition, Vector3 direction)
    {
        Color boltColor = isBlaster ? Color.cyan : new Color(1f, 0.18f, 0.02f);

        // Create projectile
        GameObject projectile = new GameObject("EnemyProjectile");
        projectile.transform.position = spawnPosition;
        projectile.transform.rotation = Quaternion.LookRotation(direction);

        // Add collider
        SphereCollider collider = projectile.AddComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.radius = 0.32f;

        // Add rigidbody
        Rigidbody rb = projectile.AddComponent<Rigidbody>();
        rb.useGravity = false;

        // Add projectile script
        EnemyProjectile projectileScript = projectile.AddComponent<EnemyProjectile>();
        projectileScript.damage = damagePerShot;
        projectileScript.speed = isBlaster ? 22f : 18f;
        projectileScript.isBlaster = isBlaster;

        // Visual effects - Enhanced trail
        TrailRenderer trail = projectile.AddComponent<TrailRenderer>();
        trail.time = 0.4f;
        trail.startWidth = isBlaster ? 0.2f : 0.14f;
        trail.endWidth = 0.03f;
        trail.startColor = boltColor;
        trail.endColor = new Color(boltColor.r, boltColor.g, boltColor.b, 0f);
        trail.material = new Material(Shader.Find("Unlit/Color"));

        // Add bright light for glow
        GameObject lightObj = new GameObject("ProjectileLight");
        lightObj.transform.SetParent(projectile.transform);
        lightObj.transform.localPosition = Vector3.zero;
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Point;
        light.range = isBlaster ? 6f : 4f;
        light.intensity = isBlaster ? 4f : 2.5f;
        light.color = boltColor;

        // Add glowing particle trail
        GameObject particlesObj = new GameObject("TrailParticles");
        particlesObj.transform.SetParent(projectile.transform);
        particlesObj.transform.localPosition = Vector3.zero;
        ParticleSystem particles = particlesObj.AddComponent<ParticleSystem>();

        var main = particles.main;
        main.startLifetime = 0.6f;
        main.startSpeed = 2f;
        main.startSize = isBlaster ? 0.4f : 0.25f;
        main.startColor = boltColor;
        main.playOnAwake = true;
        main.loop = true;

        var emission = particles.emission;
        emission.rateOverTime = 30;

        var shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.15f;

        // Add glow mesh for more visibility
        GameObject glowObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        glowObj.name = "GlowSphere";
        glowObj.transform.SetParent(projectile.transform);
        glowObj.transform.localPosition = Vector3.zero;
        glowObj.transform.localScale = isBlaster ? new Vector3(0.34f, 0.34f, 0.34f) : new Vector3(0.22f, 0.22f, 0.22f);
        Destroy(glowObj.GetComponent<Collider>());
        Renderer glowRenderer = glowObj.GetComponent<Renderer>();
        glowRenderer.material = new Material(Shader.Find("Unlit/Color"));
        glowRenderer.material.color = boltColor;

        // Destroy after 8m travel (around 8m - 9m)
        Destroy(projectile, 0.45f);
    }

    void FireRaycast(Vector3 spawnPosition, Vector3 direction)
    {
        // Enable laser line for basic weapon
        if (gunLine != null)
        {
            gunLine.enabled = true;
            gunLine.SetPosition(0, spawnPosition);
            gunLine.startWidth = 0.05f;
            gunLine.endWidth = 0.05f;
            gunLine.startColor = Color.red;
            gunLine.endColor = new Color(1f, 0f, 0f, 0.5f);
        }

        shootRay.origin = spawnPosition;
        shootRay.direction = direction;

        if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
        {
            if (shootHit.collider.CompareTag("Player"))
            {
                PlayerHealth ph = shootHit.collider.GetComponent<PlayerHealth>() ?? playerHealth;
                if (ph != null)
                    ph.TakeDamage(damagePerShot);
            }

            if (gunLine != null)
                gunLine.SetPosition(1, shootHit.point);
        }
        else
        {
            if (gunLine != null)
                gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
        }
    }
}
