using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls game pause/resume with a top-right corner UI button
/// and keyboard shortcuts (Escape / P).
/// </summary>
public class Pause : MonoBehaviour
{
    public static Pause instance;

    [HideInInspector]
    public bool isPaused = false;

    private Button button;
    private Image buttonImage;
    private Text buttonText;

    private readonly Color normalColor = new Color(0.1f, 0.12f, 0.18f, 0.9f);
    private readonly Color hoverColor = new Color(0.2f, 0.35f, 0.65f, 0.95f);
    private readonly Color pausedColor = new Color(0.85f, 0.25f, 0.2f, 0.95f);

    void Awake()
    {
        instance = this;
        SetupButtonInTopRight();
    }

    void Start()
    {
        SetupButtonInTopRight();
        CleanCenterDummyObjects();
        UpdateVisuals();
    }

    void Update()
    {
        // Keyboard shortcut: Escape or P to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            pausegame();
        }
    }

    /// <summary>
    /// Configures the button to sit neatly in the top-right corner with proper styling and click handling.
    /// </summary>
    public void SetupButtonInTopRight()
    {
        // 1. Position in top right corner
        RectTransform rect = GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2(-25, -25);
            rect.sizeDelta = new Vector2(130, 48);
        }

        // 2. Ensure Image component exists for clickable area and background
        buttonImage = GetComponent<Image>();
        if (buttonImage == null)
        {
            buttonImage = gameObject.AddComponent<Image>();
        }
        buttonImage.color = normalColor;

        // 3. Ensure Button component exists and is hooked up
        button = GetComponent<Button>();
        if (button == null)
        {
            button = gameObject.AddComponent<Button>();
        }

        // Set up button colors
        ColorBlock colors = button.colors;
        colors.normalColor = normalColor;
        colors.highlightedColor = hoverColor;
        colors.pressedColor = new Color(0.15f, 0.5f, 0.9f, 1f);
        colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.1f;
        button.colors = colors;
        button.targetGraphic = buttonImage;

        // Ensure onClick listener is registered
        button.onClick.RemoveListener(pausegame);
        button.onClick.AddListener(pausegame);

        // 4. Ensure Text child exists and is styled
        buttonText = GetComponentInChildren<Text>();
        if (buttonText == null)
        {
            GameObject textObj = new GameObject("ButtonText");
            textObj.transform.SetParent(transform, false);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            buttonText = textObj.AddComponent<Text>();
        }

        buttonText.font = GetGameFont();
        buttonText.fontSize = 22;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.fontStyle = FontStyle.Bold;
        buttonText.raycastTarget = false; // Let clicks go to the button Image
    }

    /// <summary>
    /// Removes any misplaced dummy objects or icons sitting in the center of the screen.
    /// </summary>
    void CleanCenterDummyObjects()
    {
        // If there is an old separate "PauseFunction" empty object sitting in the center, remove it
        GameObject oldPauseFunction = GameObject.Find("PauseFunction");
        if (oldPauseFunction != null && oldPauseFunction != gameObject)
        {
            Destroy(oldPauseFunction);
        }

        // Remove any leftover center dummy icons
        Transform centerIcon = transform.Find("Icon");
        if (centerIcon != null && centerIcon.gameObject != buttonText.gameObject)
        {
            Destroy(centerIcon.gameObject);
        }

        // If there's an old disabled Pause menu canvas, ensure it doesn't conflict
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj != gameObject && (obj.name == "PauseFunction" || obj.name == "CenterPauseMenu"))
            {
                Destroy(obj);
            }
        }
    }

    /// <summary>
    /// Toggles the game between paused and running states.
    /// </summary>
    public void pausegame()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            AudioListener.pause = true;
        }
        else
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }

        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        if (buttonText != null)
        {
            buttonText.text = isPaused ? "▶  Play" : "❚❚  Pause";
        }

        if (buttonImage != null)
        {
            buttonImage.color = isPaused ? pausedColor : normalColor;
            ColorBlock colors = button.colors;
            colors.normalColor = isPaused ? pausedColor : normalColor;
            button.colors = colors;
        }
    }

    Font GetGameFont()
    {
        Text[] allTexts = FindObjectsOfType<Text>();
        foreach (Text t in allTexts)
        {
            if (t != null && t.font != null && t.font.name.Contains("Luckiest"))
            {
                return t.font;
            }
        }
        return Resources.GetBuiltinResource<Font>("Arial.ttf");
    }
}
