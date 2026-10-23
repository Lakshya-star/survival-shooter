using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 25;
    public float speed = 30f;
    public float lifetime = 2f;
    public bool isBlaster = false;

    private Rigidbody rb;
    private ParticleSystem trailParticles;
    private Light projectileLight;
    private TrailRenderer trailRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        CreateVisualEffects();

        Destroy(gameObject, lifetime);
    }

    void CreateVisualEffects()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        if (trailRenderer != null) return;

        // Add trail renderer for projectile trail
        trailRenderer = gameObject.AddComponent<TrailRenderer>();
        trailRenderer.time = 0.3f;
        trailRenderer.startWidth = 0.1f;
        trailRenderer.endWidth = 0.02f;

        if (isBlaster)
        {
            // Blaster - cyan/blue trail
            trailRenderer.startColor = Color.cyan;
            trailRenderer.endColor = new Color(0, 0.5f, 1f, 0);
        }
        else
        {
            // Basic weapon - red trail
            trailRenderer.startColor = Color.red;
            trailRenderer.endColor = new Color(1f, 0, 0, 0);
        }

        trailRenderer.material = new Material(Shader.Find("Unlit/Color"));

        // Add light for glow effect
        GameObject lightObj = new GameObject("ProjectileLight");
        lightObj.transform.SetParent(transform);
        lightObj.transform.localPosition = Vector3.zero;
        projectileLight = lightObj.AddComponent<Light>();
        projectileLight.type = LightType.Point;
        projectileLight.range = 3f;
        projectileLight.intensity = 2f;

        if (isBlaster)
        {
            projectileLight.color = Color.cyan;
            projectileLight.intensity = 3f;
        }
        else
        {
            projectileLight.color = Color.red;
        }

        // Add particle system for trail
        GameObject particlesObj = new GameObject("TrailParticles");
        particlesObj.transform.SetParent(transform);
        particlesObj.transform.localPosition = Vector3.zero;
        trailParticles = particlesObj.AddComponent<ParticleSystem>();

        var main = trailParticles.main;
        main.startLifetime = 0.5f;
        main.startSpeed = 1f;
        main.startSize = isBlaster ? 0.3f : 0.15f;
        main.startColor = isBlaster ? Color.cyan : Color.red;
        main.playOnAwake = true;
        main.loop = true;

        var emission = trailParticles.emission;
        emission.rateOverTime = 20;

        var shape = trailParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Ignore the player's trigger-only detection sphere. A bolt can only
        // deal damage when it reaches the solid Player-tagged body collider.
        if (!other.CompareTag("Player") || other.isTrigger) return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);

            // Create impact effect
            CreateImpactEffect();

            Destroy(gameObject);
        }
    }

    void CreateImpactEffect()
    {
        GameObject impactObj = new GameObject("Impact");
        impactObj.transform.position = transform.position;

        ParticleSystem impactParticles = impactObj.AddComponent<ParticleSystem>();

        var main = impactParticles.main;
        main.startLifetime = 0.3f;
        main.startSpeed = 5f;
        main.startSize = 0.5f;
        main.startColor = isBlaster ? Color.cyan : Color.red;
        main.playOnAwake = true;
        main.loop = false;

        var emission = impactParticles.emission;
        emission.rateOverTime = 0;
        emission.SetBurst(0, new ParticleSystem.Burst(0, 30));

        var shape = impactParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.5f;

        Destroy(impactObj, 0.5f);
    }
}
