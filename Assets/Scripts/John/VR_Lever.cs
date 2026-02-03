using UnityEngine;

public class VRLever : MonoBehaviour
{
    HingeJoint hinge;
    Rigidbody rb;

    public float leverOutput;

    public float minValue = 0f;
    public float maxValue = 1f;

    [Range(0f, 1f)]
    public float startingValue = 0.5f;

    public bool invert;

    Quaternion baseLocalRotation;

    void Awake()
    {
        baseLocalRotation = transform.localRotation;
    }

    void Start()
    {
        hinge = GetComponent<HingeJoint>();
        rb = GetComponent<Rigidbody>();

        if (!hinge) return;
        if (Mathf.Abs(maxValue - minValue) < 0.0001f) return;

        float sv = Mathf.Clamp(startingValue, Mathf.Min(minValue, maxValue), Mathf.Max(minValue, maxValue));

        float rangeFraction = (sv - minValue) / (maxValue - minValue);
        if (invert) rangeFraction = 1f - rangeFraction;

        float degreeRotation = hinge.limits.min + (hinge.limits.max - hinge.limits.min) * rangeFraction;

        Quaternion targetLocalRot = Quaternion.AngleAxis(degreeRotation, hinge.axis) * baseLocalRotation;

        if (rb && !rb.isKinematic)
        {
            rb.rotation = transform.parent ? transform.parent.rotation * targetLocalRot : targetLocalRot;
        }
        else
        {
            transform.localRotation = targetLocalRot;
        }

        leverOutput = sv;
    }

    void FixedUpdate()
    {
        if (!hinge) return;

        float denom = hinge.limits.max - hinge.limits.min;
        if (Mathf.Abs(denom) < 0.0001f) return;

        float t01 = (hinge.angle - hinge.limits.min) / denom;
        t01 = Mathf.Clamp01(t01);

        if (invert) t01 = 1f - t01;

        leverOutput = Mathf.Lerp(minValue, maxValue, t01);
    }
}
