using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ensures the Pause button is located in the top-right corner, fully interactive,
/// and that any broken center objects are removed.
/// </summary>
public class PauseMenuEnhancer : MonoBehaviour
{
    void Start()
    {
        SetupPauseUI();
    }

    public static void SetupPauseUI()
    {
        // 1. Remove old empty PauseFunction object if it exists without a Button
        GameObject pauseFunction = GameObject.Find("PauseFunction");
        if (pauseFunction != null && pauseFunction.GetComponent<Button>() == null)
        {
            Destroy(pauseFunction);
        }

        // 2. Locate or create the pause button
        GameObject pauseButton = GameObject.Find("Pause");
        if (pauseButton == null)
        {
            pauseButton = GameObject.Find("PauseButton");
        }

        if (pauseButton == null)
        {
            Button[] allButtons = FindObjectsOfType<Button>();
            foreach (Button b in allButtons)
            {
                if (b.name.ToLower().Contains("pause"))
                {
                    pauseButton = b.gameObject;
                    break;
                }
            }
        }

        // 3. If no button exists in scene, create one in top-right corner
        if (pauseButton == null)
        {
            AutoPauseButtonCreator.CreateButton();
            return;
        }

        // 4. Ensure Pause component is attached to the button
        Pause pauseScript = pauseButton.GetComponent<Pause>();
        if (pauseScript == null)
        {
            pauseScript = pauseButton.AddComponent<Pause>();
        }

        pauseScript.SetupButtonInTopRight();
    }
}
