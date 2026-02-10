using UnityEngine;

public class DoubleDoor : MonoBehaviour
{
    public Transform porteGauche;
    public Transform porteDroite;

    [Header("Local Z positions")]
    public float zClosedLeft = 0.75f;
    public float zClosedRight = -0.75f;
    public float zOpenLeft = 1.6f;
    public float zOpenRight = -1.6f;

    [Header("Animation time (seconds)")]
    public float openTime = 1.2f;

    [Header("Color (choose what to color)")]
    public Renderer[] leftRenderers;
    public Renderer[] rightRenderers;
    public Color openColor = Color.green;
    public Color closedColor = Color.white;

    [Header("Manual override")]
    public bool forceOpen;

    Vector3 leftBaseLocal;
    Vector3 rightBaseLocal;

    bool isOpen;
    bool moving;
    float t;
    bool lastForceOpen;
    bool initialized;

    void Awake()
    {
        InitOnce();
    }

    void Start()
    {
        lastForceOpen = forceOpen;
        ApplyInstant(forceOpen);
    }

    void InitOnce()
    {
        if (initialized) return;
        if (!porteGauche || !porteDroite) return;

        leftBaseLocal = porteGauche.localPosition;
        rightBaseLocal = porteDroite.localPosition;

        if (leftRenderers == null || leftRenderers.Length == 0)
            leftRenderers = porteGauche.GetComponentsInChildren<Renderer>(true);

        if (rightRenderers == null || rightRenderers.Length == 0)
            rightRenderers = porteDroite.GetComponentsInChildren<Renderer>(true);

        initialized = true;
    }

    void Update()
    {
        if (!initialized) InitOnce();
        if (!initialized) return;

        if (forceOpen != lastForceOpen)
        {
            if (forceOpen) ForcerOuvert();
            else ForcerFerme();

            lastForceOpen = forceOpen;
        }

        if (!moving) return;

        t += Time.deltaTime / Mathf.Max(0.0001f, openTime);
        float k = Mathf.Clamp01(t);

        porteGauche.localPosition = Vector3.Lerp(
            isOpen ? ClosedLeft() : OpenLeft(),
            isOpen ? OpenLeft() : ClosedLeft(),
            k
        );

        porteDroite.localPosition = Vector3.Lerp(
            isOpen ? ClosedRight() : OpenRight(),
            isOpen ? OpenRight() : ClosedRight(),
            k
        );

        if (k >= 1f)
        {
            moving = false;
            t = 0f;
        }
    }

    Vector3 ClosedLeft()
    {
        Vector3 p = leftBaseLocal;
        p.z = zClosedLeft;
        return p;
    }

    Vector3 ClosedRight()
    {
        Vector3 p = rightBaseLocal;
        p.z = zClosedRight;
        return p;
    }

    Vector3 OpenLeft()
    {
        Vector3 p = leftBaseLocal;
        p.z = zOpenLeft;
        return p;
    }

    Vector3 OpenRight()
    {
        Vector3 p = rightBaseLocal;
        p.z = zOpenRight;
        return p;
    }

    void ApplyInstant(bool open)
    {
        isOpen = open;
        moving = false;
        t = 0f;

        porteGauche.localPosition = open ? OpenLeft() : ClosedLeft();
        porteDroite.localPosition = open ? OpenRight() : ClosedRight();

        ApplyColor(open);
    }

    void ApplyColor(bool open)
    {
        Color c = open ? openColor : closedColor;

        if (leftRenderers != null)
            for (int i = 0; i < leftRenderers.Length; i++)
                if (leftRenderers[i]) leftRenderers[i].material.color = c;

        if (rightRenderers != null)
            for (int i = 0; i < rightRenderers.Length; i++)
                if (rightRenderers[i]) rightRenderers[i].material.color = c;
    }

    public void Ouvrir()
    {
        if (forceOpen || isOpen) return;

        isOpen = true;
        moving = true;
        t = 0f;
        ApplyColor(true);
    }

    public void Fermer()
    {
        if (forceOpen || !isOpen) return;

        isOpen = false;
        moving = true;
        t = 0f;
        ApplyColor(false);
    }

    public void ForcerOuvert()
    {
        forceOpen = true;
        ApplyInstant(true);
    }

    public void ForcerFerme()
    {
        forceOpen = false;
        ApplyInstant(false);
    }
}
