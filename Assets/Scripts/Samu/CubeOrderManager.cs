using UnityEngine;

public class CubeOrderManager : MonoBehaviour
{
    public ChangeColor[] order = new ChangeColor[9];
    private int currentIndex = 0;
    public AudioSource successAudio;
    public AudioSource errorAudio; 

    public void OnCubeSelected(GameObject selectedObj)
    {
        ChangeColor selectedCube = selectedObj.GetComponent<ChangeColor>();
        if (selectedCube == null)
        {
            Debug.LogError("Ce GameObject n'a pas de script ChangeColor !");
            return;
        }

        Debug.Log("Sélectionné : " + selectedObj.name);
        Debug.Log("Ordre attendu : " + order[currentIndex].gameObject.name);

        if (selectedCube == order[currentIndex])
        {
            // ✅ Cube correct
            selectedCube.OnCorrectSelection();
            currentIndex++;

            // Jouer le son de succès
            if (successAudio != null)
                successAudio.Play();
        }
        else
        {
            // ❌ Mauvais cube
            Debug.Log("Erreur : mauvais cube !");
            if (errorAudio != null)
                errorAudio.Play();
        }

        // Vérifier si la séquence est terminée
        if (currentIndex >= order.Length)
        {
            Debug.Log("✅ Séquence terminée !");
            FloorMover floorMover = FindObjectOfType<FloorMover>();
            if (floorMover != null)
                floorMover.StartMoving();
            else
                Debug.LogError("FloorMover introuvable !");
        }
    }
}
