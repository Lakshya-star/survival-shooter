using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
    public Slider healthSlider;
    public Image damageImage;
    public AudioClip deathClip;
    public float flashSpeed = 5f;
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);

    // Lives system
    public int maxLives = 3;
    private int currentLives;
    public int CurrentLives => currentLives;
    public bool IsDead => isDead;
    private bool isRespawning = false;

    // Sprite-based hearts — works on WebGL (no Unicode emoji needed)
    private Image[] heartImages;

    Animator anim;
    AudioSource playerAudio;
    PlayerMovement playerMovement;
    PlayerShooting playerShooting;
    bool isDead;
    bool damaged;
    public bool IsShielded { get; private set; }


    void Awake()
    {
        //Mendapatkan refernce komponen
        anim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooting = GetComponentInChildren<PlayerShooting>();

        if (healthSlider == null)
        {
            healthSlider = FindObjectOfType<Slider>();
        }

        ConfigureHealthBar();

        currentHealth = startingHealth;
        currentLives = maxLives;

        CreateLivesUI();
        UpdateLivesUI();
    }

    // Keep the health bar easy to read against every part of the level.
    void ConfigureHealthBar()
    {
        if (healthSlider == null) return;

        Image background = healthSlider.transform.Find("Background")?.GetComponent<Image>();
        if (background != null)
            background.color = new Color(0.05f, 0.05f, 0.05f, 0.95f);

        Image fill = healthSlider.fillRect != null
            ? healthSlider.fillRect.GetComponent<Image>()
            : null;

        if (fill != null)
        {
            // Bright yellow makes the remaining HP stand out immediately.
            fill.color = new Color(1f, 0.9f, 0f, 1f);

            Outline outline = fill.GetComponent<Outline>();
            if (outline == null)
                outline = fill.gameObject.AddComponent<Outline>();

            outline.effectColor = new Color(0f, 0f, 0f, 0.9f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);
        }
    }

    void CreateLivesUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        // If already created (scene restart), find and re-use existing hearts
        Transform existing = canvas.transform.Find("LivesContainer");
        if (existing != null)
        {
            heartImages = new Image[maxLives];
            for (int i = 0; i < maxLives; i++)
            {
                Transform child = existing.Find("Heart_" + i);
                if (child != null)
                    heartImages[i] = child.GetComponent<Image>();
            }
            return;
        }

        // ---- Container sits in top-left, below the health bar ----
        GameObject container = new GameObject("LivesContainer");
        container.transform.SetParent(canvas.transform, false);

        RectTransform containerRT = container.AddComponent<RectTransform>();
        containerRT.anchorMin        = new Vector2(0f, 1f);
        containerRT.anchorMax        = new Vector2(0f, 1f);
        containerRT.pivot            = new Vector2(0f, 1f);
        containerRT.anchoredPosition = new Vector2(10f, -140f);
        containerRT.sizeDelta        = new Vector2(120f, 36f);

        // Load the Heart sprite already in the project
        Sprite heartSprite = LoadHeartSprite();

        heartImages = new Image[maxLives];
        float heartSize = 30f;
        float spacing   = 36f;

        for (int i = 0; i < maxLives; i++)
        {
            GameObject heartObj = new GameObject("Heart_" + i);
            heartObj.transform.SetParent(container.transform, false);

            RectTransform rt    = heartObj.AddComponent<RectTransform>();
            rt.anchorMin        = new Vector2(0f, 0.5f);
            rt.anchorMax        = new Vector2(0f, 0.5f);
            rt.pivot            = new Vector2(0f, 0.5f);
            rt.anchoredPosition = new Vector2(i * spacing, 0f);
            rt.sizeDelta        = new Vector2(heartSize, heartSize);

            Image img          = heartObj.AddComponent<Image>();
            img.sprite         = heartSprite;
            img.preserveAspect = true;
            img.color          = Color.white;
            img.raycastTarget  = false;

            heartImages[i] = img;
        }
    }

    /// <summary>Tries every available path to find the Heart sprite.</summary>
    Sprite LoadHeartSprite()
    {
        // 1) Try the existing Heart GameObject already placed in the scene
        GameObject existingHeart = GameObject.Find("Heart");
        if (existingHeart != null)
        {
            Image img = existingHeart.GetComponent<Image>();
            if (img != null && img.sprite != null)
                return img.sprite;
        }

        // 2) Try Resources.Load (works if Heart.png is inside a Resources folder)
        Sprite loaded = Resources.Load<Sprite>("Heart");
        if (loaded != null) return loaded;

        // 3) Search all Image components for a sprite named "Heart"
        Image[] allImages = FindObjectsOfType<Image>();
        foreach (Image img in allImages)
        {
            if (img.sprite != null && img.sprite.name == "Heart")
                return img.sprite;
        }

        // 4) Fallback: a simple red circle drawn at runtime
        return MakeFallbackHeartSprite();
    }

    Sprite MakeFallbackHeartSprite()
    {
        int size = 64;
        Texture2D tex  = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color red   = new Color(0.92f, 0.15f, 0.15f, 1f);
        Color clear = Color.clear;
        float cx = size / 2f, cy = size / 2f, r = size / 2f - 2f;
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dx = x - cx, dy = y - cy;
                tex.SetPixel(x, y, (dx * dx + dy * dy) <= r * r ? red : clear);
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    void UpdateLivesUI()
    {
        if (heartImages == null) return;
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null) continue;
            // Full white = life remaining, dark translucent = life lost
            heartImages[i].color = (i < currentLives)
                ? Color.white
                : new Color(0.2f, 0.2f, 0.2f, 0.45f);
        }
    }

    void Update()
    {
        // Flash damage image
        if (damaged)
        {
            if (damageImage != null)
                damageImage.color = flashColour;
        }
        else
        {
            if (damageImage != null)
                damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        damaged = false;
    }


    //fungsi untuk mendapatkan damage
    public void TakeDamage(int amount)
    {
        if (isRespawning || IsShielded) return;

        damaged = true;

        //mengurangi health
        currentHealth -= amount;

        //Merubah tampilan dari health slider
        if (healthSlider != null)
            healthSlider.value = currentHealth;

        //Memainkan suara ketika terkena damage
        if (playerAudio != null)
            playerAudio.Play();

        //Memanggil method Death() jika darahnya kurang dari sama dengan 10 dan belu mati
        if (currentHealth <= 0 && !isDead)
        {
            Death();
        }
    }

    // fungsi untuk menambah nyawa
    public void Healing()
    {
        //mengurangi health
        int newHealth = currentHealth + 40;
        if (newHealth >= 100)
        {
            currentHealth = 100;
        }
        else
        {
            currentHealth = newHealth;
        }

        //Merubah tampilan dari health slider
        if (healthSlider != null)
            healthSlider.value = currentHealth;
    }

    public void SetShielded(bool shielded)
    {
        IsShielded = shielded;
    }


    void Death()
    {
        currentLives--;
        UpdateLivesUI();

        if (currentLives > 0)
        {
            // Still has lives — respawn
            StartCoroutine(Respawn());
        }
        else
        {
            // No lives left — real game over
            isDead = true;
            GameOver();
        }
    }

    IEnumerator Respawn()
    {
        isRespawning = true;
        isDead = true;

        // Disable movement & shooting during death animation
        if (playerShooting != null)
        {
            playerShooting.DisableEffects();
            playerShooting.enabled = false;
        }
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Play death animation / sound briefly
        if (anim != null)
            anim.SetTrigger("Die");

        if (playerAudio != null)
        {
            if (deathClip != null)
                playerAudio.clip = deathClip;
            playerAudio.Play();
        }

        // Brief flash to show life lost
        yield return new WaitForSeconds(1.2f);

        // Reset health
        currentHealth = startingHealth;
        isDead = false;
        isRespawning = false;

        // Re-enable player
        if (playerMovement != null)
            playerMovement.enabled = true;
        if (playerShooting != null)
            playerShooting.enabled = true;

        // Reset animator
        if (anim != null)
        {
            anim.Rebind();
            anim.Update(0f);
        }

        // Update slider
        if (healthSlider != null)
        {
            healthSlider.maxValue = startingHealth;
            healthSlider.value = startingHealth;
        }

        // Show respawn flash on screen
        if (damageImage != null)
            damageImage.color = new Color(1f, 1f, 1f, 0.7f);
    }

    void GameOver()
    {
        if (playerShooting != null)
        {
            playerShooting.DisableEffects();
            playerShooting.enabled = false;
        }

        //mentrigger animasi Die
        if (anim != null)
            anim.SetTrigger("Die");

        //Memainkan suara ketika mati
        if (playerAudio != null)
        {
            if (deathClip != null)
                playerAudio.clip = deathClip;
            playerAudio.Play();
        }

        //mematikan script player movement
        if (playerMovement != null)
            playerMovement.enabled = false;
    }

    public void RestartLevel()
    {
        //meload ulang scene dengan index 0 pada build setting
        SceneManager.LoadScene(0);
    }
}
