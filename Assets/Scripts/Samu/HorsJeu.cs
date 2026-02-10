using UnityEngine;
using UnityEngine.SceneManagement;

public class HorsJeu : MonoBehaviour
{

    public Vector3 respawnPosition = new Vector3(0f, 1f, 0f);

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           //Mettre le jouer à une position (x, y, z)
            other.transform.position = respawnPosition;

        }
    }
}
