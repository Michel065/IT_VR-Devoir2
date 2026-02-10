using UnityEngine;

public class PressurePadGate : MonoBehaviour
{
    [Header("Visual to color")]
    public Renderer visualIndicator;

    [Header("Cables to color")]
    public Renderer[] cablesRenderers;

    [Header("Colors")]
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.red;

    [Header("Gate output")]
    public bool valide;

    int insideCount;

    void Start()
    {
        SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsValid(other)) return;

        insideCount++;
        if (insideCount == 1)
            SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsValid(other)) return;

        insideCount--;
        if (insideCount <= 0)
        {
            insideCount = 0;
            SetActive(false);
        }
    }

    bool IsValid(Collider c)
    {
        return c.CompareTag("Player") || c.CompareTag("Block");
    }

    void SetActive(bool active)
    {
        valide = active;

        Color c = active ? activeColor : inactiveColor;

        if (visualIndicator)
            visualIndicator.material.color = c;

        if (cablesRenderers != null)
            for (int i = 0; i < cablesRenderers.Length; i++)
                if (cablesRenderers[i]) cablesRenderers[i].material.color = c;
    }
}
