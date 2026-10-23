using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Smoothly follows the player character so they are always centered on screen
/// and never walk out of frame. Ensures background clears to dark void rather than skybox.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothing = 5f;

    private Vector3 offset;
    private Camera cam;

    private void Awake()
    {
        ConfigureCameraBackground();
    }

    private void Start()
    {
        FindTarget();
        if (target != null)
        {
            offset = transform.position - target.position;
        }
        else
        {
            // Default offset for standard survival shooter top-down view (0.16, 15, -22)
            offset = new Vector3(0.16f, 15f, -22f);
        }

        ConfigureCameraBackground();
    }

    void ConfigureCameraBackground()
    {
        if (cam == null) cam = GetComponent<Camera>() ?? Camera.main;
        if (cam != null)
        {
            // Set to Solid Color matching the original scene
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.02f, 0.02f, 0.04f, 1f);
            cam.allowHDR = !Application.isMobilePlatform;
            cam.allowMSAA = true;
        }

        // Disable skybox so background is seamlessly dark
        RenderSettings.skybox = null;
    }

    void FindTarget()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            FindTarget();
            if (target == null) return;
            offset = transform.position - target.position;
        }

        // Follow player continuously so the character remains centered and never disappears offscreen
        Vector3 targetCamPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }
}
