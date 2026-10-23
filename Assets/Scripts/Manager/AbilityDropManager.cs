using UnityEngine;

// Automatically adds occasional ability drops at random positions around the arena.
public class AbilityDropManager : MonoBehaviour
{
    const float DropDelay = 7f;
    const int MaximumActiveDrops = 4;
    PlayerHealth playerHealth;
    float nextDropTime;
    AbilityType nextAbility;
    int nextSpawnPoint;
    PowerUpFactory powerUpFactory;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void CreateManager()
    {
        if (FindObjectOfType<AbilityDropManager>() != null) return;
        new GameObject("AbilityDropManager").AddComponent<AbilityDropManager>();
    }

    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        powerUpFactory = FindObjectOfType<PowerUpFactory>();
        nextAbility = (AbilityType)Random.Range(0, 4);
        // Make abilities visible immediately instead of making players wait.
        SpawnAbility();
        SpawnAbility();
        nextDropTime = Time.time + DropDelay;
    }

    void Update()
    {
        if (Time.time < nextDropTime) return;
        nextDropTime = Time.time + DropDelay;

        if (playerHealth == null) playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth == null || playerHealth.currentHealth <= 0) return;

        if (FindObjectsOfType<AbilityPickup>().Length < MaximumActiveDrops)
            SpawnAbility();
    }

    void SpawnAbility()
    {
        AbilityType ability = nextAbility;
        nextAbility = (AbilityType)(((int)nextAbility + 1) % 4);
        Vector3 position = FindDropPosition();
        GameObject drop = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        drop.name = ability + " Ability Pickup";
        drop.transform.position = position;
        drop.transform.localScale = new Vector3(0.55f, 0.2f, 0.55f);
        Collider collider = drop.GetComponent<Collider>();
        collider.isTrigger = true;

        AbilityPickup pickup = drop.AddComponent<AbilityPickup>();
        switch (ability)
        {
            case AbilityType.Shield:
                pickup.Configure(ability, new Color(0.15f, 0.8f, 1f));
                break;
            case AbilityType.RapidFire:
                pickup.Configure(ability, new Color(1f, 0.65f, 0.1f));
                break;
            case AbilityType.DamageBoost:
                pickup.Configure(ability, new Color(1f, 0.2f, 0.4f));
                break;
            default:
                pickup.Configure(ability, new Color(0.35f, 1f, 0.3f));
                break;
        }
    }

    Vector3 FindDropPosition()
    {
        if (powerUpFactory == null) powerUpFactory = FindObjectOfType<PowerUpFactory>();
        if (powerUpFactory != null && powerUpFactory.spawnPoints != null && powerUpFactory.spawnPoints.Length > 0)
        {
            Transform point = powerUpFactory.spawnPoints[nextSpawnPoint % powerUpFactory.spawnPoints.Length];
            nextSpawnPoint++;
            if (point != null)
            {
                Vector2 offset = Random.insideUnitCircle * 2.2f;
                return point.position + new Vector3(offset.x, 0.35f, offset.y);
            }
        }

        // Fallback stays near the player, so a pickup cannot appear beyond
        // the playable arena if configured spawn points are unavailable.
        Vector2 circle = Random.insideUnitCircle.normalized * Random.Range(4f, 8f);
        Vector3 center = playerHealth != null ? playerHealth.transform.position : Vector3.zero;
        Vector3 position = center + new Vector3(circle.x, 0.35f, circle.y);
        RaycastHit hit;
        int floorMask = LayerMask.GetMask("Floor");
        if (floorMask != 0 && Physics.Raycast(position + Vector3.up * 20f, Vector3.down, out hit, 40f, floorMask))
            position.y = hit.point.y + 0.25f;
        return position;
    }
}
