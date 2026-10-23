using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Rendering;
using UnityEngine.Rendering;
using UnityEngine;

public static class AndroidBuilder
{
    [MenuItem("Build/Build Android APK")]
    public static void BuildApk()
    {
        const string outputDirectory = "Builds";
        const string outputPath = "Builds/SurvivalShooter.apk";

        if (!Directory.Exists(outputDirectory))
            Directory.CreateDirectory(outputDirectory);

        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.survivalshooter.game");
        PlayerSettings.productName = "Survival Shooter";
        PlayerSettings.bundleVersion = "1.0.6";
        PlayerSettings.Android.bundleVersionCode = 7;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeRight = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;

        // SDK & NDK paths
        EditorPrefs.SetString("AndroidSdkRoot", "/opt/homebrew/share/android-commandlinetools");
        EditorPrefs.SetString("AndroidSDKRoot", "/opt/homebrew/share/android-commandlinetools");
        EditorPrefs.SetString("AndroidNdkRoot", "/opt/homebrew/share/android-commandlinetools/ndk/16.1.4479499");
        EditorPrefs.SetString("AndroidNDKRoot", "/opt/homebrew/share/android-commandlinetools/ndk/16.1.4479499");
        EditorPrefs.SetString("AndroidNdkRootR16b", "/opt/homebrew/share/android-commandlinetools/ndk/16.1.4479499");

        // Target API in Unity PlayerSettings (Gradle template overrides targetSdkVersion to 34)
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel21;
        PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)33;

        // Native 64-bit ARM64 support so modern 64-bit phones run natively without 32-bit warning or crash
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;

        // Keystore signing to ensure valid signature
        string keystorePath = Path.GetFullPath("survivalshooter.keystore");
        if (File.Exists(keystorePath))
        {
            PlayerSettings.Android.keystoreName = keystorePath;
            PlayerSettings.Android.keystorePass = "survival123";
            PlayerSettings.Android.keyaliasName = "survivalshooter";
            PlayerSettings.Android.keyaliasPass = "survival123";
        }

        // Configure graphics tier settings for Android
        foreach (GraphicsTier tier in (GraphicsTier[])System.Enum.GetValues(typeof(GraphicsTier)))
        {
            TierSettings tierSettings = EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Android, tier);
            tierSettings.standardShaderQuality = ShaderQuality.High;
            tierSettings.cascadedShadowMaps = true;
            tierSettings.prefer32BitShadowMaps = false; // Safe 16/24-bit depth buffers supported on all mobile GPUs
            tierSettings.hdr = false;
            tierSettings.reflectionProbeBlending = true;
            tierSettings.reflectionProbeBoxProjection = true;
            tierSettings.detailNormalMap = true;
            tierSettings.semitransparentShadows = true;
            tierSettings.renderingPath = RenderingPath.Forward;
            EditorGraphicsSettings.SetTierSettings(BuildTargetGroup.Android, tier, tierSettings);
        }

        // Use OpenGLES3 with OpenGLES2 fallback (disable Unity 2018's buggy Vulkan preview which hangs on Android 14)
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android,
            new[] { GraphicsDeviceType.OpenGLES3, GraphicsDeviceType.OpenGLES2 });
        PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.Disabled);

        Texture2D previewIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(
            "Assets/TutorialInfo/Icons/SurvivalShooter.png");
        if (previewIcon != null)
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android,
                new[] { previewIcon });

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/Level_01.unity" },
            locationPathName = outputPath,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
            throw new System.Exception("Android build failed: " + report.summary.result);

        Debug.Log("[AndroidBuilder] Successfully built: " + outputPath);
    }
}
