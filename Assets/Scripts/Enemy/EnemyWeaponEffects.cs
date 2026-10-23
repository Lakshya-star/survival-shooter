using UnityEngine;

public class EnemyWeaponEffects : MonoBehaviour
{
    void Start()
    {
        // EnemyShooting now owns the complete projectile visual.  Keeping this
        // old helper inactive prevents duplicate lines and stray blinking lights.
        enabled = false;
    }

    void AddWeaponEffects()
    {
        // Check if this enemy has shooting capability
        EnemyShooting enemyShooting = GetComponent<EnemyShooting>();
        if (enemyShooting == null)
            return;

        // Create muzzle point
        GameObject muzzlePoint = new GameObject("MuzzlePoint");
        muzzlePoint.transform.SetParent(transform);
        muzzlePoint.transform.localPosition = new Vector3(0, 1f, 1f);

        // Add particle system for muzzle flash
        ParticleSystem muzzleFlash = muzzlePoint.AddComponent<ParticleSystem>();
        var main = muzzleFlash.main;
        main.duration = 0.1f;
        main.loop = false;
        main.startLifetime = 0.2f;
        main.startSpeed = 5f;
        main.startSize = 0.5f;
        main.playOnAwake = false;

        var emission = muzzleFlash.emission;
        emission.rateOverTime = 0;
        emission.SetBurst(0, new ParticleSystem.Burst(0, 20));

        var shape = muzzleFlash.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 30f;

        // Add light for muzzle flash
        Light muzzleLight = muzzlePoint.AddComponent<Light>();
        muzzleLight.type = LightType.Point;
        muzzleLight.range = 5f;
        muzzleLight.intensity = 2f;
        muzzleLight.color = Color.red;
        muzzleLight.enabled = false;

        // Add LineRenderer for laser beam
        LineRenderer laserLine = gameObject.AddComponent<LineRenderer>();
        laserLine.startWidth = 0.05f;
        laserLine.endWidth = 0.05f;
        laserLine.enabled = false;
        laserLine.material = new Material(Shader.Find("Unlit/Color"));
        laserLine.startColor = Color.red;
        laserLine.endColor = new Color(1f, 0f, 0f, 0.5f);

        // Add AudioSource for shooting sound
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D sound

        Debug.Log("Added weapon effects to enemy: " + gameObject.name);
    }
}
