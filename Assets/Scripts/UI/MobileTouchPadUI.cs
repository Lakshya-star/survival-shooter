using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MobileTouchPadUI : MonoBehaviour
{
    private static bool created = false;
    private static Sprite circleSprite;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Initialize()
    {
        if (created) return;
        created = true;

        GameObject obj = new GameObject("MobileTouchPadUI");
        obj.AddComponent<MobileTouchPadUI>();
        DontDestroyOnLoad(obj);
    }

    void Start()
    {
        CreateControls();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // This object survives a restart, but the previous Canvas does not.
        // Rebuild both controls on the newly loaded Canvas.
        CreateControls();
    }

    void CreateControls()
    {
        CreateMovementJoystick();
        CreateShootingJoystick();
    }

    // Sits above the 3D arena but underneath every HUD element.  It protects
    // the dark visual style on devices where Unity's Android gamma output is
    // brighter than the desktop editor.
    void CreateWorldToneOverlay()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null || canvas.transform.Find("MobileWorldTone") != null) return;

        GameObject overlay = new GameObject("MobileWorldTone");
        overlay.transform.SetParent(canvas.transform, false);
        overlay.transform.SetAsFirstSibling();

        RectTransform rect = overlay.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = overlay.AddComponent<Image>();
        image.color = new Color(0.01f, 0.02f, 0.05f, 0.38f);
        image.raycastTarget = false;
    }

    void CreateMovementJoystick()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null || canvas.transform.Find("MobileMovementJoystick") != null) return;

        GameObject joystickObject = new GameObject("MobileMovementJoystick");
        joystickObject.transform.SetParent(canvas.transform, false);

        RectTransform background = joystickObject.AddComponent<RectTransform>();
        background.anchorMin = Vector2.zero;
        background.anchorMax = Vector2.zero;
        background.pivot = new Vector2(0.5f, 0.5f);
        background.anchoredPosition = new Vector2(155f, 145f);
        background.sizeDelta = new Vector2(132f, 132f);

        Image backgroundImage = joystickObject.AddComponent<Image>();
        backgroundImage.sprite = GetCircleSprite();
        backgroundImage.color = new Color(0.08f, 0.1f, 0.14f, 0.88f);
        Outline backgroundOutline = joystickObject.AddComponent<Outline>();
        backgroundOutline.effectColor = new Color(0.78f, 0.82f, 0.88f, 0.9f);
        backgroundOutline.effectDistance = new Vector2(1.5f, -1.5f);

        GameObject handleObject = new GameObject("Handle");
        handleObject.transform.SetParent(joystickObject.transform, false);
        RectTransform handle = handleObject.AddComponent<RectTransform>();
        handle.anchorMin = new Vector2(0.5f, 0.5f);
        handle.anchorMax = new Vector2(0.5f, 0.5f);
        handle.pivot = new Vector2(0.5f, 0.5f);
        handle.sizeDelta = new Vector2(52f, 52f);

        Image handleImage = handleObject.AddComponent<Image>();
        handleImage.sprite = GetCircleSprite();
        handleImage.color = new Color(0.86f, 0.89f, 0.94f, 1f);
        handleImage.raycastTarget = false;
        Outline handleOutline = handleObject.AddComponent<Outline>();
        handleOutline.effectColor = new Color(1f, 1f, 1f, 0.9f);
        handleOutline.effectDistance = new Vector2(1f, -1f);

        VirtualJoystick joystick = joystickObject.AddComponent<VirtualJoystick>();
        joystick.Configure(background, handle);
    }

    void CreateShootingJoystick()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null || canvas.transform.Find("MobileShootingJoystick") != null) return;

        GameObject joystickObject = new GameObject("MobileShootingJoystick");
        joystickObject.transform.SetParent(canvas.transform, false);

        RectTransform background = joystickObject.AddComponent<RectTransform>();
        background.anchorMin = new Vector2(1f, 0f);
        background.anchorMax = new Vector2(1f, 0f);
        background.pivot = new Vector2(0.5f, 0.5f);
        background.anchoredPosition = new Vector2(-155f, 145f);
        background.sizeDelta = new Vector2(132f, 132f);

        Image backgroundImage = joystickObject.AddComponent<Image>();
        backgroundImage.sprite = GetCircleSprite();
        backgroundImage.color = new Color(0.14f, 0.08f, 0.1f, 0.88f);
        Outline backgroundOutline = joystickObject.AddComponent<Outline>();
        backgroundOutline.effectColor = new Color(1f, 0.42f, 0.42f, 0.92f);
        backgroundOutline.effectDistance = new Vector2(1.5f, -1.5f);

        GameObject handleObject = new GameObject("Handle");
        handleObject.transform.SetParent(joystickObject.transform, false);
        RectTransform handle = handleObject.AddComponent<RectTransform>();
        handle.anchorMin = new Vector2(0.5f, 0.5f);
        handle.anchorMax = new Vector2(0.5f, 0.5f);
        handle.pivot = new Vector2(0.5f, 0.5f);
        handle.sizeDelta = new Vector2(52f, 52f);

        Image handleImage = handleObject.AddComponent<Image>();
        handleImage.sprite = GetCircleSprite();
        handleImage.color = new Color(1f, 0.48f, 0.48f, 1f);
        handleImage.raycastTarget = false;
        Outline handleOutline = handleObject.AddComponent<Outline>();
        handleOutline.effectColor = new Color(1f, 0.9f, 0.9f, 0.95f);
        handleOutline.effectDistance = new Vector2(1f, -1f);

        ShootingJoystick joystick = joystickObject.AddComponent<ShootingJoystick>();
        joystick.Configure(background, handle);
    }

    Sprite GetCircleSprite()
    {
        if (circleSprite != null) return circleSprite;

        const int size = 128;
        Texture2D texture = new Texture2D(size, size, TextureFormat.ARGB32, false);
        Color[] pixels = new Color[size * size];
        float radius = (size - 2) * 0.5f;
        Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                float alpha = Mathf.Clamp01(radius - distance + 1f);
                pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        circleSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
        return circleSprite;
    }

    void CreateTouchpadOverlays()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        // Container
        GameObject container = new GameObject("TouchHints");
        container.transform.SetParent(canvas.transform, false);

        RectTransform cRect = container.AddComponent<RectTransform>();
        cRect.anchorMin = Vector2.zero;
        cRect.anchorMax = Vector2.one;
        cRect.offsetMin = Vector2.zero;
        cRect.offsetMax = Vector2.zero;

        // Left Pad (Move)
        CreatePad(container.transform, "MovePad", new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(100f, 100f), "🕹️ DRAG TO MOVE");

        // Right Pad (Aim & Fire)
        CreatePad(container.transform, "AimPad", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-100f, 100f), "🎯 DRAG TO SHOOT");
    }

    void CreatePad(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, string label)
    {
        GameObject padObj = new GameObject(name);
        padObj.transform.SetParent(parent, false);

        RectTransform rect = padObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = pos;
        rect.sizeDelta = new Vector2(150f, 150f);

        Image img = padObj.AddComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0.15f);

        GameObject textObj = new GameObject("Label");
        textObj.transform.SetParent(padObj.transform, false);

        RectTransform tRect = textObj.AddComponent<RectTransform>();
        tRect.anchorMin = Vector2.zero;
        tRect.anchorMax = Vector2.one;
        tRect.offsetMin = Vector2.zero;
        tRect.offsetMax = Vector2.zero;

        Text txt = textObj.AddComponent<Text>();
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.text = label;
        txt.fontSize = 13;
        txt.fontStyle = FontStyle.Bold;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = new Color(1f, 1f, 1f, 0.45f);
    }
}
