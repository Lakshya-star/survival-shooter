using UnityEngine;

// Rotating world pickup that grants one temporary combat ability.
public class AbilityPickup : MonoBehaviour
{
    AbilityType ability;
    float lifetime = 24f;
    Vector3 startPosition;
    bool collected;

    public void Configure(AbilityType newAbility, Color color)
    {
        ability = newAbility;
        startPosition = transform.position;

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
            renderer.material.EnableKeyword("_EMISSION");
            renderer.material.SetColor("_EmissionColor", color * 0.9f);
        }

        CreateAbilityIcon(color);

        Light glow = gameObject.AddComponent<Light>();
        glow.type = LightType.Point;
        glow.color = color;
        glow.range = 5.5f;
        glow.intensity = 2.8f;

    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Rotate(0f, 100f * Time.deltaTime, 0f, Space.World);
        transform.position = startPosition + Vector3.up * (0.25f + Mathf.Sin(Time.time * 3f) * 0.12f);

    }

    void CreateAbilityIcon(Color color)
    {
        // A round badge replaces the old floating text label.
        GameObject badge = CreateIconPart(PrimitiveType.Cylinder, "Round " + ability + " Badge",
            new Vector3(0f, 1.05f, 0f), new Vector3(0.72f, 0.06f, 0.72f), color);

        Color symbolColor = Color.white;
        if (ability == AbilityType.Shield)
        {
            CreateIconPart(PrimitiveType.Sphere, "Shield Symbol", new Vector3(0f, 1.2f, 0f),
                new Vector3(0.38f, 0.16f, 0.38f), symbolColor);
        }
        else if (ability == AbilityType.RapidFire)
        {
            CreateIconPart(PrimitiveType.Capsule, "Rapid Fire Symbol", new Vector3(-0.2f, 1.2f, 0f),
                new Vector3(0.14f, 0.26f, 0.14f), symbolColor);
            CreateIconPart(PrimitiveType.Capsule, "Rapid Fire Symbol", new Vector3(0f, 1.2f, 0f),
                new Vector3(0.14f, 0.26f, 0.14f), symbolColor);
            CreateIconPart(PrimitiveType.Capsule, "Rapid Fire Symbol", new Vector3(0.2f, 1.2f, 0f),
                new Vector3(0.14f, 0.26f, 0.14f), symbolColor);
        }
        else if (ability == AbilityType.DamageBoost)
        {
            GameObject diamond = CreateIconPart(PrimitiveType.Cube, "Power Shot Symbol", new Vector3(0f, 1.2f, 0f),
                new Vector3(0.3f, 0.12f, 0.3f), symbolColor);
            diamond.transform.localEulerAngles = new Vector3(0f, 45f, 0f);
        }
        else
        {
            GameObject arrowLeft = CreateIconPart(PrimitiveType.Cube, "Speed Symbol", new Vector3(-0.14f, 1.2f, 0f),
                new Vector3(0.12f, 0.12f, 0.38f), symbolColor);
            arrowLeft.transform.localEulerAngles = new Vector3(0f, 35f, 0f);
            GameObject arrowRight = CreateIconPart(PrimitiveType.Cube, "Speed Symbol", new Vector3(0.14f, 1.2f, 0f),
                new Vector3(0.12f, 0.12f, 0.38f), symbolColor);
            arrowRight.transform.localEulerAngles = new Vector3(0f, -35f, 0f);
        }
    }

    GameObject CreateIconPart(PrimitiveType primitive, string objectName, Vector3 localPosition, Vector3 localScale, Color color)
    {
        GameObject part = GameObject.CreatePrimitive(primitive);
        part.name = objectName;
        part.transform.SetParent(transform, false);
        part.transform.localPosition = localPosition;
        part.transform.localScale = localScale;
        Destroy(part.GetComponent<Collider>());

        Renderer partRenderer = part.GetComponent<Renderer>();
        partRenderer.material.color = color;
        partRenderer.material.EnableKeyword("_EMISSION");
        partRenderer.material.SetColor("_EmissionColor", color * 0.45f);
        return part;
    }

    void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        // This deliberately matches Healing.cs: only the Player-tagged body
        // collider can collect a pickup, and it must be within touching range.
        if (!other.CompareTag("Player") || other.isTrigger) return;
        if (Vector3.Distance(other.transform.position, transform.position) >= 1.5f) return;

        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player == null) return;

        collected = true;
        PlayerAbilityController controller = player.GetComponent<PlayerAbilityController>();
        if (controller == null) controller = player.gameObject.AddComponent<PlayerAbilityController>();
        controller.Activate(ability);
        Destroy(gameObject);
    }
}
