using UnityEngine;
using UnityEngine.EventSystems;

// Handles pointer input for the on-screen movement joystick.
public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    RectTransform background;
    RectTransform handle;
    float handleRange;
    int activePointerId = -1;

    public void Configure(RectTransform backgroundRect, RectTransform handleRect)
    {
        background = backgroundRect;
        handle = handleRect;
        handleRange = (background.sizeDelta.x - handle.sizeDelta.x) * 0.5f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        activePointerId = eventData.pointerId;
        UpdateJoystick(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (activePointerId == -1 || eventData.pointerId == activePointerId)
        {
            activePointerId = eventData.pointerId;
            UpdateJoystick(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId == activePointerId || activePointerId == -1)
        {
            ResetJoystick();
        }
    }

    public void ResetJoystick()
    {
        activePointerId = -1;
        if (handle != null) handle.anchoredPosition = Vector2.zero;
        MobileInputTracker.SetVirtualJoystickInput(Vector2.zero, false);
    }

    private void OnDisable()
    {
        ResetJoystick();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) ResetJoystick();
    }

    private void Update()
    {
        // Guard against missed OnPointerUp events on mobile touchscreens / WebGL:
        if (activePointerId != -1)
        {
            bool pointerStillDown = false;

            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch t = Input.GetTouch(i);
                    if (t.fingerId == activePointerId)
                    {
                        if (t.phase != TouchPhase.Ended && t.phase != TouchPhase.Canceled)
                        {
                            pointerStillDown = true;
                        }
                        break;
                    }
                }
            }
            else if (Input.GetMouseButton(0))
            {
                pointerStillDown = true;
            }

            if (!pointerStillDown)
            {
                ResetJoystick();
            }
        }
    }

    void UpdateJoystick(PointerEventData eventData)
    {
        if (background == null || handle == null) return;

        Vector2 localPosition;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background, eventData.position, eventData.pressEventCamera, out localPosition))
            return;

        Vector2 input = Vector2.ClampMagnitude(localPosition / handleRange, 1f);
        handle.anchoredPosition = input * handleRange;
        MobileInputTracker.SetVirtualJoystickInput(input, true);
    }
}
