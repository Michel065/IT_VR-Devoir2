using UnityEngine;
using UnityEngine.InputSystem;

public class XRHandGestureDebug : MonoBehaviour
{
    [Header("Input Actions (float)")]
    [SerializeField] InputActionReference grab;
    [SerializeField] InputActionReference pinch;
    [SerializeField] InputActionReference open;
    [SerializeField] InputActionReference fist;

    [SerializeField, Range(0f, 1f)] float threshold = 0.2f;

    float lastGrab, lastPinch, lastOpen, lastFist;

    void OnEnable()
    {
        Enable(grab); Enable(pinch); Enable(open); Enable(fist);
        Debug.Log($"[{name}] XRHandGestureDebug prêt (threshold={threshold})");
    }

    void OnDisable()
    {
        Disable(grab); Disable(pinch); Disable(open); Disable(fist);
    }

    void Update()
    {
        lastGrab = LogEdge("GRAB", Read(grab), lastGrab);
        lastPinch = LogEdge("PINCH", Read(pinch), lastPinch);
        lastOpen = LogEdge("OPEN", Read(open), lastOpen);
        lastFist = LogEdge("FIST", Read(fist), lastFist);
    }

    float LogEdge(string label, float v, float prev)
    {
        bool wasOn = prev >= threshold;
        bool isOn = v >= threshold;

        if (!wasOn && isOn) Debug.Log($"[{name}] {label} START v={v:0.00}");
        if (wasOn && !isOn) Debug.Log($"[{name}] {label} END v={v:0.00}");

        return v;
    }

    float Read(InputActionReference a)
    {
        if (!a || a.action == null) return 0f;
        return a.action.ReadValue<float>();
    }

    void Enable(InputActionReference a)
    {
        if (a && a.action != null) a.action.Enable();
    }

    void Disable(InputActionReference a)
    {
        if (a && a.action != null) a.action.Disable();
    }
}
