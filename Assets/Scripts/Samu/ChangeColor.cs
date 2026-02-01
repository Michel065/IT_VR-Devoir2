using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public Renderer rend;

    void Start()
    {
        if (rend == null)
            rend = GetComponent<Renderer>();
    }

    public void SetRed()
    {
        rend.material.color = Color.red;
    }

    public void SetGreen()
    {
        rend.material.color = Color.green;
    }
}
