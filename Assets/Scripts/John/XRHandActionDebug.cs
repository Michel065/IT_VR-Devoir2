using UnityEngine;
using UnityEngine.InputSystem;

public class XRHandActionDebug : MonoBehaviour
{
    [Header("Actions à surveiller (Input Action Reference)")]
    [SerializeField] private InputActionReference grab;
    [SerializeField] private InputActionReference pinch;
    [SerializeField] private InputActionReference open;
    [SerializeField] private InputActionReference fist;

    [Header("Seuils")]
    [SerializeField] private float pressThreshold = 0.6f;
    [SerializeField] private bool logValueWhileHolding = false;

    void OnEnable()
    {
        Bind(grab, "GRAB");
        Bind(pinch, "PINCH");
        Bind(open, "OPEN");
        Bind(fist, "FIST");
    }

    void OnDisable()
    {
        Unbind(grab, "GRAB");
        Unbind(pinch, "PINCH");
        Unbind(open, "OPEN");
        Unbind(fist, "FIST");
    }

    void Update()
    {
        if (!logValueWhileHolding) return;

        LogIfHolding(grab, "GRAB");
        LogIfHolding(pinch, "PINCH");
        LogIfHolding(open, "OPEN");
        LogIfHolding(fist, "FIST");
    }

    void Bind(InputActionReference a, string label)
    {
        if (a == null || a.action == null) return;
        a.action.Enable();
        a.action.performed += ctx => OnAction(label, ctx);
        a.action.canceled += ctx => OnAction(label + " (release)", ctx);
        Debug.Log($"[{name}] XRHandActionDebug: prêt -> {label} = {a.action.name}");
    }

    void Unbind(InputActionReference a, string label)
    {
        if (a == null || a.action == null) return;
        a.action.performed -= ctx => OnAction(label, ctx);   // (note: lambda non détachable)
        a.action.canceled -= ctx => OnAction(label + " (release)", ctx);
    }

    void OnAction(string label, InputAction.CallbackContext ctx)
    {
        float v = ReadFloat(ctx);

        if (label.EndsWith("(release)"))
        {
            Debug.Log($"[{name}] {label} v={v:0.00}");
            return;
        }

        // pour éviter le spam sur des actions analogiques
        if (v >= pressThreshold)
            Debug.Log($"[{name}] {label} v={v:0.00}");
    }

    void LogIfHolding(InputActionReference a, string label)
    {
        if (a == null || a.action == null) return;
        float v = a.action.ReadValue<float>();
        if (v >= pressThreshold)
            Debug.Log($"[{name}] holding {label} v={v:0.00}");
    }

    float ReadFloat(InputAction.CallbackContext ctx)
    {
        // la plupart des actions de main sont en float (0..1)
        // si jamais c'est un bool, ça marche aussi
        if (ctx.valueType == typeof(bool))
            return ctx.ReadValue<bool>() ? 1f : 0f;

        return ctx.ReadValue<float>();
    }
}
