using UnityEngine;
using TMPro;
using System.Collections;

public class EphemeralText : MonoBehaviour
{
    public float fadeInTime = 0.3f;
    public float visibleTime = 1.5f;
    public float fadeOutTime = 0.5f;

    private TextMeshPro text;

    void Awake()
    {
        text = GetComponent<TextMeshPro>();
        Color c = text.color;
        c.a = 0f;
        text.color = c;
    }

    void Start()
    {
        StartCoroutine(AnimateText());
    }

    IEnumerator AnimateText()
    {
        // Fade In
        yield return Fade(0f, 1f, fadeInTime);

        // Visible
        yield return new WaitForSeconds(visibleTime);

        // Fade Out
        yield return Fade(1f, 0f, fadeOutTime);

        Destroy(gameObject); // optionnel
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(from, to, t / duration);
            Color c = text.color;
            c.a = a;
            text.color = c;
            yield return null;
        }
    }
}
