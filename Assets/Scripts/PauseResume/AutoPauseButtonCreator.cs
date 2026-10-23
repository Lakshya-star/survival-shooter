using UnityEngine;
using UnityEngine.UI;

public class AutoPauseButtonCreator : MonoBehaviour
{
    void Start()
    {
        CreateButton();
    }

    public static GameObject CreateButton()
    {
        // Find or ensure canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No canvas found to create PauseButton!");
            return null;
        }

        // Check if button already exists
        GameObject existingButton = GameObject.Find("PauseButton") ?? GameObject.Find("Pause");
        if (existingButton != null)
        {
            Pause existingPause = existingButton.GetComponent<Pause>() ?? existingButton.AddComponent<Pause>();
            existingPause.SetupButtonInTopRight();
            return existingButton;
        }

        // Create pause button
        GameObject buttonObj = new GameObject("PauseButton");
        buttonObj.transform.SetParent(canvas.transform, false);

        // Add Pause script which handles position, image, button, and text automatically
        Pause pauseScript = buttonObj.AddComponent<Pause>();
        pauseScript.SetupButtonInTopRight();

        Debug.Log("[AutoPauseButtonCreator] Auto-created pause button in top right corner");
        return buttonObj;
    }
}
