using UnityEngine;

/// <summary>
/// Ensures optimal mobile presentation without triggering expensive runtime
/// renderer resets that cause black screens on Android 13/14 devices.
/// </summary>
public static class MobilePresentationSetup
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ConfigureMobilePresentation()
    {
        if (Application.isMobilePlatform)
        {
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeRight = false;
            Screen.autorotateToLandscapeLeft = true;
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureMobileQuality()
    {
        if (!Application.isMobilePlatform) return;

        // Apply visual fidelity safely without resetting EGL / Vulkan contexts
        QualitySettings.shadows = ShadowQuality.All;
        QualitySettings.shadowResolution = ShadowResolution.Medium;
        QualitySettings.shadowProjection = ShadowProjection.CloseFit;
        QualitySettings.shadowDistance = 50f;
        QualitySettings.pixelLightCount = 4;
        QualitySettings.antiAliasing = 2;
        QualitySettings.realtimeReflectionProbes = true;
        QualitySettings.softParticles = true;

        Camera[] cameras = Object.FindObjectsOfType<Camera>();
        foreach (Camera camera in cameras)
        {
            camera.allowHDR = false;
            camera.allowMSAA = true;

            MobileColorCorrection mcc = camera.GetComponent<MobileColorCorrection>();
            if (mcc != null)
                Object.Destroy(mcc);
        }
    }
}
