using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Rendering;
using UnityEngine.Rendering;
using UnityEngine;

public static class iOSBuilder
{
    [MenuItem("Build/Export iOS Project")]
    public static void ExportXcodeProject()
    {
        const string outputPath = "Builds/iOS";

        if (!Directory.Exists("Builds"))
            Directory.CreateDirectory("Builds");

        PlayerSettings.productName = "Survival Shooter";
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS,
            "com.lakshyamittal.survivalshooter");
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeRight = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;

        // Configure graphics tier settings for iOS so mobile compiles desktop-quality
        // shaders, cascaded soft shadows, and full precision float lighting.
        foreach (GraphicsTier tier in (GraphicsTier[])System.Enum.GetValues(typeof(GraphicsTier)))
        {
            TierSettings tierSettings = EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.iOS, tier);
            tierSettings.standardShaderQuality = ShaderQuality.High;
            tierSettings.cascadedShadowMaps = true;
            tierSettings.prefer32BitShadowMaps = true;
            tierSettings.hdr = true;
            tierSettings.hdrMode = CameraHDRMode.FP16;
            tierSettings.reflectionProbeBlending = true;
            tierSettings.reflectionProbeBoxProjection = true;
            tierSettings.detailNormalMap = true;
            tierSettings.semitransparentShadows = true;
            tierSettings.renderingPath = RenderingPath.Forward;
            EditorGraphicsSettings.SetTierSettings(BuildTargetGroup.iOS, tier, tierSettings);
        }

        // Metal is the standard high-performance graphics API for iOS
        PlayerSettings.SetGraphicsAPIs(BuildTarget.iOS,
            new[] { GraphicsDeviceType.Metal });

        Texture2D previewIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(
            "Assets/TutorialInfo/Icons/SurvivalShooter.png");
        if (previewIcon != null)
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.iOS,
                new[] { previewIcon });

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/Level_01.unity" },
            locationPathName = outputPath,
            target = BuildTarget.iOS,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
            throw new System.Exception("iOS export failed: " + report.summary.result);
    }
}
