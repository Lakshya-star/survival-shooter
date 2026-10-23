using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System;
using System.IO;

[InitializeOnLoad]
public class WebGLEnvironmentSetup
{
    static WebGLEnvironmentSetup()
    {
        EnsureEnvironment();
    }

    public static void EnsureEnvironment()
    {
        string pythonPath = "/opt/homebrew/bin/python3.11";
        Environment.SetEnvironmentVariable("EMSDK_PYTHON", pythonPath);
        Environment.SetEnvironmentVariable("PYTHON", pythonPath);

        string path = Environment.GetEnvironmentVariable("PATH") ?? "";
        if (!path.Contains("/opt/homebrew/bin"))
        {
            Environment.SetEnvironmentVariable("PATH", "/opt/homebrew/bin:" + path);
        }
    }
}

public class WebGLBuilder
{
    [MenuItem("Build/Build WebGL")]
    public static void BuildWebGL()
    {
        WebGLEnvironmentSetup.EnsureEnvironment();

        string buildPath = "Builds/WebGL";
        if (!Directory.Exists(buildPath))
            Directory.CreateDirectory(buildPath);

        PlayerSettings.productName = "Survival Shooter";
        PlayerSettings.WebGL.memorySize = 256;
        // Disabled compression ensures instant compatibility with any web host (GitHub Pages, Netlify, Vercel)
        // without requiring custom server HTTP response headers for .br or .gz files.
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;

        string[] scenes = new string[] { "Assets/Scenes/Level_01.unity" };

        Debug.Log("[WebGLBuilder] Starting WebGL build to: " + buildPath);
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = buildPath,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
            throw new Exception("WebGL build failed: " + report.summary.result);

        Debug.Log("[WebGLBuilder] Finished successfully! Output located at: " + buildPath);
    }
}
