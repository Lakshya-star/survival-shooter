using UnityEngine;

public static class MobileInputTracker
{
    public static Vector2 MovementInput { get; private set; }
    public static Vector3 AimDirection { get; private set; }
    public static bool IsShooting { get; private set; }

    private static Vector2 virtualJoystickInput;
    private static bool virtualJoystickActive;
    private static Vector2 aimJoystickInput;
    private static bool aimJoystickActive;
    private static Vector3 lastAimDirection;

    public static void SetVirtualJoystickInput(Vector2 input, bool isActive)
    {
        virtualJoystickInput = Vector2.ClampMagnitude(input, 1f);
        virtualJoystickActive = isActive;
    }

    public static void SetAimJoystickInput(Vector2 input, bool isActive)
    {
        aimJoystickInput = Vector2.ClampMagnitude(input, 1f);
        aimJoystickActive = isActive;

        if (aimJoystickInput.sqrMagnitude > 0.01f)
            lastAimDirection = new Vector3(aimJoystickInput.x, 0f, aimJoystickInput.y).normalized;
    }

    public static void Update()
    {
        MovementInput = virtualJoystickActive ? virtualJoystickInput : Vector2.zero;
        AimDirection = lastAimDirection;

        // Shooting only comes from the dedicated right joystick.  Touching any
        // other part of the screen no longer fires the weapon.
        IsShooting = aimJoystickActive && aimJoystickInput.sqrMagnitude > 0.04f;
    }
}
