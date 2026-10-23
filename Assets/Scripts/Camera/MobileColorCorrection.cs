using UnityEngine;

/// <summary>
/// Legacy component: disabled to preserve authentic HDR and original scene lighting.
/// </summary>
public class MobileColorCorrection : MonoBehaviour
{
    private void Awake()
    {
        Destroy(this);
    }
}
