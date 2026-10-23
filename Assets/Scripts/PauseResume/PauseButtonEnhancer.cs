using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class PauseButtonEnhancer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Button button;
    private Image buttonImage;
    private Text buttonText;
    private Image iconImage;
    private Image glowImage;

    // Modern gradient-like colors
    private Color normalColor = new Color(0.05f, 0.05f, 0.1f, 0.95f);
    private Color hoverColor = new Color(0.15f, 0.35f, 0.7f, 1f);
    private Color clickColor = new Color(0.3f, 0.55f, 0.9f, 1f);
    private Color pausedColor = new Color(0.8f, 0.15f, 0.15f, 1f);
    private Color normalTextColor = new Color(1f, 1f, 1f, 1f);
    private Color hoverTextColor = Color.white;

    private bool isPaused = false;
    private Coroutine pulseCoroutine;
    private Coroutine glowCoroutine;

    void Start()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<Text>();

        // Create enhanced UI elements
        CreateGlow();
        CreateIcon();

        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
            buttonImage.material = new Material(Shader.Find("UI/Default"));
        }

        if (buttonText != null)
        {
            buttonText.font = GetExistingFont();
            buttonText.fontSize = 22;
            buttonText.color = normalTextColor;
            buttonText.fontStyle = FontStyle.Bold;
        }

        UpdateIcon(false);
    }

    void CreateGlow()
    {
        GameObject glowObj = new GameObject("Glow");
        glowObj.transform.SetParent(transform, false);
        glowObj.transform.SetAsFirstSibling();

        RectTransform glowRect = glowObj.AddComponent<RectTransform>();
        glowRect.anchorMin = Vector2.zero;
        glowRect.anchorMax = Vector2.one;
        glowRect.sizeDelta = Vector2.zero;

        glowImage = glowObj.AddComponent<Image>();
        glowImage.color = new Color(0.3f, 0.6f, 1f, 0f);
        glowImage.raycastTarget = false;
    }

    void CreateIcon()
    {
        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(transform, false);
        iconObj.transform.localPosition = new Vector3(-50, 0, 0);

        RectTransform iconRect = iconObj.AddComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(35, 35);

        iconImage = iconObj.AddComponent<Image>();
        iconImage.color = normalTextColor;
    }

    void UpdateIcon(bool paused)
    {
        if (iconImage == null) return;

        Texture2D iconTexture = new Texture2D(35, 35);
        Color[] colors = new Color[35 * 35];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.clear;
        }

        if (paused)
        {
            // Play icon (triangle) - larger and clearer
            for (int y = 0; y < 35; y++)
            {
                for (int x = 0; x < 35; x++)
                {
                    float centerX = 17.5f;
                    float centerY = 17.5f;
                    float dx = x - centerX;
                    float dy = y - centerY;

                    // Triangle shape
                    if (dy > -8 && dy < 8 && dx > -8 + dy * 0.5f && dx < 8 - dy * 0.5f)
                    {
                        colors[y * 35 + x] = Color.white;
                    }
                }
            }
        }
        else
        {
            // Pause icon (two vertical bars) - thicker and more visible
            for (int y = 5; y < 30; y++)
            {
                for (int x = 6; x < 14; x++)
                {
                    colors[y * 35 + x] = Color.white;
                }
                for (int x = 21; x < 29; x++)
                {
                    colors[y * 35 + x] = Color.white;
                }
            }
        }

        iconTexture.SetPixels(colors);
        iconTexture.Apply();
        iconImage.sprite = Sprite.Create(iconTexture, new Rect(0, 0, 35, 35), new Vector2(0.5f, 0.5f));
    }

    public void SetPausedState(bool paused)
    {
        isPaused = paused;
        UpdateIcon(paused);

        if (buttonImage != null)
        {
            buttonImage.color = paused ? pausedColor : normalColor;
        }

        if (paused)
        {
            if (pulseCoroutine == null)
            {
                pulseCoroutine = StartCoroutine(PulseEffect());
            }
            if (glowCoroutine == null)
            {
                glowCoroutine = StartCoroutine(GlowEffect());
            }
        }
        else
        {
            if (pulseCoroutine != null)
            {
                StopCoroutine(pulseCoroutine);
                pulseCoroutine = null;
            }
            if (glowCoroutine != null)
            {
                StopCoroutine(glowCoroutine);
                glowCoroutine = null;
            }
            if (buttonImage != null)
            {
                buttonImage.transform.localScale = Vector3.one;
            }
            if (glowImage != null)
            {
                glowImage.color = new Color(0.3f, 0.6f, 1f, 0f);
            }
        }
    }

    IEnumerator PulseEffect()
    {
        while (isPaused)
        {
            float scale = 1f + Mathf.Sin(Time.realtimeSinceStartup * 4f) * 0.08f;
            if (buttonImage != null)
            {
                buttonImage.transform.localScale = new Vector3(scale, scale, 1f);
            }
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }

    IEnumerator GlowEffect()
    {
        while (isPaused)
        {
            float alpha = 0.3f + Mathf.Sin(Time.realtimeSinceStartup * 3f) * 0.2f;
            if (glowImage != null)
            {
                glowImage.color = new Color(1f, 0.3f, 0.3f, alpha);
            }
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonImage != null)
        {
            buttonImage.color = isPaused ? new Color(1f, 0.3f, 0.3f, 1f) : hoverColor;
        }
        if (buttonText != null)
        {
            buttonText.color = hoverTextColor;
        }
        if (glowImage != null && !isPaused)
        {
            glowImage.color = new Color(0.3f, 0.6f, 1f, 0.3f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonImage != null)
        {
            buttonImage.color = isPaused ? pausedColor : normalColor;
        }
        if (buttonText != null)
        {
            buttonText.color = normalTextColor;
        }
        if (glowImage != null && !isPaused)
        {
            glowImage.color = new Color(0.3f, 0.6f, 1f, 0f);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (buttonImage != null)
        {
            buttonImage.color = clickColor;
            StartCoroutine(ClickBounce());
        }
    }

    IEnumerator ClickBounce()
    {
        Vector3 originalScale = buttonImage.transform.localScale;
        buttonImage.transform.localScale = originalScale * 0.85f;
        yield return new WaitForSecondsRealtime(0.08f);
        buttonImage.transform.localScale = originalScale * 1.15f;
        yield return new WaitForSecondsRealtime(0.08f);
        buttonImage.transform.localScale = originalScale;
    }

    Font GetExistingFont()
    {
        Text[] allTexts = FindObjectsOfType<Text>();
        foreach (Text text in allTexts)
        {
            if (text.font != null && text.font.name.Contains("Luckiest"))
            {
                return text.font;
            }
        }
        return Resources.GetBuiltinResource<Font>("Arial.ttf");
    }
}
