using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeTrigger : MonoBehaviour
{
    [SerializeField] int sceneBuildIndex = 1;

    bool done;

    void OnTriggerEnter(Collider other)
    {
        if (done) return;
        if (!other.CompareTag("Player")) return;

        done = true;
        SceneManager.LoadScene(sceneBuildIndex);
    }
}
