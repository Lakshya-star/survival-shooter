using UnityEngine;

/// <summary>
/// Calibrates mobile graphics to match the rich colors, deep blacks,
/// and neon vibrancy of the Mac desktop build.
/// </summary>
[RequireComponent(typeof(Camera))]
public class MobileColorGrading : MonoBehaviour
{
    private Material gradingMaterial;

    private void Awake()
    {
        if (!Application.isMobilePlatform)
        {
            enabled = false;
            return;
        }

        // First attempt: load the Material asset bundled in Resources
        gradingMaterial = Resources.Load<Material>("MobileColorGradingMat");

        // Fallback: construct from shader directly if needed
        if (gradingMaterial == null)
        {
            Shader shader = Shader.Find("Hidden/MobileColorGrading");
            if (shader == null)
                shader = Resources.Load<Shader>("MobileColorGrading");

            if (shader != null && shader.isSupported)
            {
                gradingMaterial = new Material(shader);
            }
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (gradingMaterial != null)
            Graphics.Blit(source, destination, gradingMaterial);
        else
            Graphics.Blit(source, destination);
    }

    private void OnDestroy()
    {
        // Only destroy if created dynamically
        if (gradingMaterial != null && gradingMaterial.name.Contains("(Instance)"))
            Destroy(gradingMaterial);
    }
}
