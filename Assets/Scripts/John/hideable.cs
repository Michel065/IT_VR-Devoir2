using UnityEngine;

public class HideableObject : MonoBehaviour
{
    public bool hide = false;

    [Header("Interaction")]
    public bool keepColliderWhenHidden = false;

    MeshRenderer[] renderers;
    Collider[] colliders;

    void Start()
    {
        renderers = GetComponentsInChildren<MeshRenderer>(true);
        colliders = GetComponentsInChildren<Collider>(true);
        Apply();
    }

    void Awake()
    {
        renderers = GetComponentsInChildren<MeshRenderer>(true);
        colliders = GetComponentsInChildren<Collider>(true);
        Apply();
    }

    void OnValidate()
    {
        Apply();
    }

    void Apply()
    {
        if (renderers == null || colliders == null) return;

        foreach (var r in renderers)
            r.enabled = !hide;

        foreach (var c in colliders)
            c.enabled = !hide || keepColliderWhenHidden;
    }

    // API
    public void Show()
    {
        hide = false;
        Apply();
    }

    public void Hide()
    {
        hide = true;
        Apply();
    }

    public void Toggle()
    {
        hide = !hide;
        Apply();
    }
}