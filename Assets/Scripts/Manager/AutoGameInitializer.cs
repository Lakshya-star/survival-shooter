using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Automatically sets up all necessary components, references, UI, and managers
/// so the game runs out-of-the-box without requiring any manual setup in the Unity Editor.
/// </summary>
public class AutoGameInitializer : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeScene()
    {
        Debug.Log("[AutoGameInitializer] Starting automatic scene configuration...");

        // 1. Ensure EventSystem exists for UI interaction
        EnsureEventSystem();

        // 2. Ensure Canvas exists for UI
        Canvas canvas = EnsureCanvas();

        // 3. Find and tag Player if not tagged
        GameObject player = EnsurePlayer();

        // 4. Ensure Camera follows Player
        EnsureCamera(player);

        // 5. Ensure Managers exist and are wired
        EnsureManagers(player, canvas);

        Debug.Log("[AutoGameInitializer] Automatic scene configuration complete! No manual setup required.");
    }

    static void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
            Debug.Log("[AutoGameInitializer] Auto-created EventSystem");
        }
    }

    static Canvas EnsureCanvas()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("HUDCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            Debug.Log("[AutoGameInitializer] Auto-created Canvas");
        }
        return canvas;
    }

    static GameObject EnsurePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            // Find by component if tag is missing
            PlayerMovement pm = FindObjectOfType<PlayerMovement>();
            if (pm != null)
            {
                player = pm.gameObject;
            }
            else
            {
                PlayerHealth ph = FindObjectOfType<PlayerHealth>();
                if (ph != null) player = ph.gameObject;
            }

            if (player != null)
            {
                player.tag = "Player";
                Debug.Log("[AutoGameInitializer] Tagged " + player.name + " as 'Player'");
            }
        }
        return player;
    }

    static void EnsureCamera(GameObject player)
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            mainCam = FindObjectOfType<Camera>();
        }

        if (mainCam != null)
        {
            mainCam.clearFlags = CameraClearFlags.SolidColor;
            mainCam.backgroundColor = new Color(0.02f, 0.02f, 0.04f, 1f);
            RenderSettings.skybox = null;

            if (player != null)
            {
                CameraFollow follow = mainCam.GetComponent<CameraFollow>();
                if (follow == null)
                {
                    follow = mainCam.gameObject.AddComponent<CameraFollow>();
                    Debug.Log("[AutoGameInitializer] Added CameraFollow to " + mainCam.name);
                }
                if (follow.target == null)
                {
                    follow.target = player.transform;
                    Debug.Log("[AutoGameInitializer] Assigned player transform to CameraFollow");
                }
            }
        }
    }

    static void EnsureManagers(GameObject player, Canvas canvas)
    {
        // Ensure LevelManager
        LevelManager.EnsureExists();

        // Ensure ScoreManager
        ScoreManager sm = FindObjectOfType<ScoreManager>();
        if (sm == null)
        {
            GameObject smObj = new GameObject("ScoreManager");
            smObj.AddComponent<ScoreManager>();
            Debug.Log("[AutoGameInitializer] Auto-created ScoreManager");
        }

        // Ensure GameOverManager
        GameOverManager gom = FindObjectOfType<GameOverManager>();
        if (gom == null)
        {
            GameObject gomObj = new GameObject("GameOverManager");
            gom = gomObj.AddComponent<GameOverManager>();
            Debug.Log("[AutoGameInitializer] Auto-created GameOverManager");
        }

        // Auto-wire EnemyManager references
        PlayerHealth ph = player != null ? player.GetComponent<PlayerHealth>() : null;
        EnemyManager[] enemyManagers = FindObjectsOfType<EnemyManager>();
        foreach (EnemyManager em in enemyManagers)
        {
            if (em.playerHealth == null && ph != null)
            {
                em.playerHealth = ph;
            }
        }

        // Auto-wire PowerUpManager references
        PowerUpManager[] powerUpManagers = FindObjectsOfType<PowerUpManager>();
        foreach (PowerUpManager pum in powerUpManagers)
        {
            if (pum.playerHealth == null && ph != null)
            {
                pum.playerHealth = ph;
            }
        }

        // Ensure Pause Button exists and is in top-right
        PauseMenuEnhancer.SetupPauseUI();
    }
}
