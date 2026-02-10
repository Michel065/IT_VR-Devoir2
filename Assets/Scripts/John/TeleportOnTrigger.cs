using UnityEngine;

public class TeleportOnTrigger2 : MonoBehaviour
{
    [SerializeField] private Transform playerRig;
    [SerializeField] private Transform destination_player;

    [SerializeField] private Transform destination_object;
    [SerializeField] private string objetcTag = "Block";

    private void OnTriggerEnter(Collider other)
    {
        if (playerRig != null && (other.transform == playerRig || other.transform.IsChildOf(playerRig)))
        {
            if (destination_player == null) return;

            Transform cam = Camera.main.transform;

            Vector3 rigToCam = new Vector3(cam.position.x - playerRig.position.x, 0f, cam.position.z - playerRig.position.z);
            playerRig.position = destination_player.position - rigToCam;


            return;
        }

        if (other.CompareTag(objetcTag))
        {
            if (destination_object == null) return;

            Rigidbody rb = other.attachedRigidbody;
            Transform target = rb ? rb.transform : other.transform;

            if (rb != null)
            {
                rb.position = destination_object.position;
                rb.rotation = destination_object.rotation;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            else
            {
                target.position = destination_object.position;
                target.rotation = destination_object.rotation;
            }
        }
    }
}
