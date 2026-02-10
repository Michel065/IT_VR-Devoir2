using UnityEngine;
using System.Collections;

public class EphemeralPanel : MonoBehaviour
{
    public float displayDuration = 2f;

    void OnEnable()
    {
        StartCoroutine(HideAfterDelay());
    }

    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        gameObject.SetActive(false);
    }
}
