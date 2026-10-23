using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

// Applies short-lived ability pickups to the player.
public class PlayerAbilityController : MonoBehaviour
{
    PlayerHealth health;
    PlayerMovement movement;
    PlayerShooting shooting;
    float normalSpeed;
    float normalFireRate;
    int normalDamage;
    Coroutine shieldRoutine;
    Coroutine rapidFireRoutine;
    Coroutine damageRoutine;
    Coroutine speedRoutine;
    GameObject shieldVisual;

    void Awake()
    {
        health = GetComponent<PlayerHealth>();
        movement = GetComponent<PlayerMovement>();
        shooting = GetComponentInChildren<PlayerShooting>();

        if (movement != null) normalSpeed = movement.speed;
        if (shooting != null)
        {
            normalFireRate = shooting.timeBetweenBullets;
            normalDamage = shooting.damagePerShot;
        }
    }

    public void Activate(AbilityType ability)
    {
        switch (ability)
        {
            case AbilityType.Shield:
                RestartRoutine(ref shieldRoutine, ShieldForSeconds());
                break;
            case AbilityType.RapidFire:
                RestartRoutine(ref rapidFireRoutine, RapidFireForSeconds());
                break;
            case AbilityType.DamageBoost:
                RestartRoutine(ref damageRoutine, DamageBoostForSeconds());
                break;
            case AbilityType.SpeedBoost:
                RestartRoutine(ref speedRoutine, SpeedBoostForSeconds());
                break;
        }
    }

    void RestartRoutine(ref Coroutine routine, IEnumerator effect)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(effect);
    }

    IEnumerator ShieldForSeconds()
    {
        if (health != null) health.SetShielded(true);
        CreateShieldVisual();
        yield return new WaitForSeconds(7f);
        if (health != null) health.SetShielded(false);
        if (shieldVisual != null) Destroy(shieldVisual);
        shieldRoutine = null;
    }

    IEnumerator RapidFireForSeconds()
    {
        if (shooting != null) shooting.timeBetweenBullets = normalFireRate * 0.45f;
        yield return new WaitForSeconds(8f);
        if (shooting != null) shooting.timeBetweenBullets = normalFireRate;
        rapidFireRoutine = null;
    }

    IEnumerator DamageBoostForSeconds()
    {
        if (shooting != null) shooting.damagePerShot = normalDamage * 2;
        yield return new WaitForSeconds(8f);
        if (shooting != null) shooting.damagePerShot = normalDamage;
        damageRoutine = null;
    }

    IEnumerator SpeedBoostForSeconds()
    {
        if (movement != null) movement.speed = normalSpeed * 1.6f;
        yield return new WaitForSeconds(8f);
        if (movement != null) movement.speed = normalSpeed;
        speedRoutine = null;
    }

    void CreateShieldVisual()
    {
        if (shieldVisual != null) return;

        shieldVisual = new GameObject("ShieldAbilityVisual");
        shieldVisual.name = "ShieldAbilityVisual";
        shieldVisual.transform.SetParent(transform, false);
        shieldVisual.transform.localPosition = new Vector3(0f, 0.9f, 0f);
        GameObject shieldBubble = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        shieldBubble.transform.SetParent(shieldVisual.transform, false);
        shieldBubble.transform.localScale = Vector3.one * 2.4f;
        Destroy(shieldBubble.GetComponent<Collider>());

        Renderer renderer = shieldBubble.GetComponent<Renderer>();
        Material material = new Material(Shader.Find("Standard"));
        material.SetFloat("_Mode", 3f);
        material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
        material.color = new Color(0.2f, 0.85f, 1f, 0.22f);
        renderer.material = material;

        CreateShieldRing(new Vector3(90f, 0f, 0f));
        CreateShieldRing(new Vector3(0f, 0f, 90f));

        Light glow = shieldVisual.AddComponent<Light>();
        glow.type = LightType.Point;
        glow.color = new Color(0.15f, 0.85f, 1f);
        glow.range = 5f;
        glow.intensity = 2f;
    }

    void Update()
    {
        if (shieldVisual != null)
            shieldVisual.transform.Rotate(0f, 95f * Time.deltaTime, 0f, Space.Self);
    }

    void CreateShieldRing(Vector3 rotation)
    {
        GameObject ring = new GameObject("ShieldEnergyRing");
        ring.transform.SetParent(shieldVisual.transform, false);
        ring.transform.localEulerAngles = rotation;

        LineRenderer line = ring.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.loop = true;
        line.positionCount = 48;
        line.widthMultiplier = 0.045f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = new Color(0.4f, 0.95f, 1f, 0.95f);
        line.endColor = line.startColor;

        for (int i = 0; i < line.positionCount; i++)
        {
            float angle = Mathf.PI * 2f * i / line.positionCount;
            line.SetPosition(i, new Vector3(Mathf.Cos(angle) * 1.28f, Mathf.Sin(angle) * 1.28f, 0f));
        }
    }
}
