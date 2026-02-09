using UnityEngine;
using TMPro;

public class Error : MonoBehaviour
{
    public TextMeshProUGUI errorText;
    public static int erreur = 0;

    public static void SetError()
    {
        erreur++;
    }

    public static int GetError()
    {
        return erreur;
    }

    void Update()
    {
        errorText.text = GetError().ToString();
    }
}
