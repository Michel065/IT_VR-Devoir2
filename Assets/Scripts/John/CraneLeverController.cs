using UnityEngine;

public class CraneLeverController : MonoBehaviour
{
    public enum Axis { X, Y, Z, None }

    [SerializeField] Transform target;

    [SerializeField] VRLever lever1;
    [SerializeField] VRLever lever2;

    [SerializeField] Axis lever1Axis = Axis.X;
    [SerializeField] Axis lever2Axis = Axis.Y;

    [SerializeField] Transform limitMin;
    [SerializeField] Transform limitMax;

    [SerializeField] bool useLocalSpace = true;

    [SerializeField] PressurePadGate gate;

    [Range(0f, 1f)][SerializeField] float neutralMin = 0.40f;
    [Range(0f, 1f)][SerializeField] float neutralMax = 0.60f;

    [Range(0f, 1f)][SerializeField] float slowEdgeLow = 0.30f;
    [Range(0f, 1f)][SerializeField] float fastEdgeLow = 0.20f;

    [Range(0f, 1f)][SerializeField] float slowEdgeHigh = 0.70f;
    [Range(0f, 1f)][SerializeField] float fastEdgeHigh = 0.80f;

    [SerializeField] float speedSlow = 0.10f;
    [SerializeField] float speedMid = 0.25f;
    [SerializeField] float speedFast = 0.50f;

    void LateUpdate()
    {
        if (gate && !gate.valide) return;

        if (!target || !lever1 || !lever2 || !limitMin || !limitMax) return;

        Vector3 a = useLocalSpace ? limitMin.localPosition : limitMin.position;
        Vector3 b = useLocalSpace ? limitMax.localPosition : limitMax.position;

        Vector3 result = useLocalSpace ? target.localPosition : target.position;

        ApplyLever(ref result, lever1Axis, lever1.leverOutput, a, b);
        ApplyLever(ref result, lever2Axis, lever2.leverOutput, a, b);

        if (useLocalSpace) target.localPosition = result;
        else target.position = result;

        Debug.Log($"Lever1 output: {lever1.leverOutput:F3}   ||   Lever2 output: {lever2.leverOutput:F3}");
    }

    void ApplyLever(ref Vector3 pos, Axis axis, float t01, Vector3 pA, Vector3 pB)
    {
        if (axis == Axis.None) return;

        t01 = Mathf.Clamp01(t01);

        if (t01 >= neutralMin && t01 <= neutralMax) return;

        float dir = (t01 < neutralMin) ? -1f : 1f;
        float speed = SpeedFromT01(t01);

        float delta = dir * speed * Time.deltaTime;

        switch (axis)
        {
            case Axis.X:
                pos.x = ClampBetween(pos.x + delta, pA.x, pB.x);
                break;
            case Axis.Y:
                pos.y = ClampBetween(pos.y + delta, pA.y, pB.y);
                break;
            case Axis.Z:
                pos.z = ClampBetween(pos.z + delta, pA.z, pB.z);
                break;
        }
    }

    float SpeedFromT01(float t01)
    {
        if (t01 < neutralMin)
        {
            if (t01 >= slowEdgeLow) return speedSlow;
            if (t01 >= fastEdgeLow) return speedMid;
            return speedFast;
        }
        else
        {
            if (t01 <= slowEdgeHigh) return speedSlow;
            if (t01 <= fastEdgeHigh) return speedMid;
            return speedFast;
        }
    }

    float ClampBetween(float v, float a, float b)
    {
        float min = Mathf.Min(a, b);
        float max = Mathf.Max(a, b);
        return Mathf.Clamp(v, min, max);
    }
}
