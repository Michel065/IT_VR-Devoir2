using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CrystalFragment : MonoBehaviour
{
    [Header("Configuration")]
    public int assemblyOrder = 0;
    public Transform snapTarget;
    public float snapDistance = 0.15f;

    [Header("Feedback")]
    public AudioClip snapSuccessSound;
    public AudioClip snapErrorSound;
    public GameObject successParticles;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private CrystalAssemblyManager assemblyManager;
    private CrystalScoringSystem scoringSystem;

    private bool isSnapped = false;
    private Material originalMaterial;
    private Renderer fragmentRenderer;
    private Rigidbody rb;

    void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        assemblyManager = FindObjectOfType<CrystalAssemblyManager>();
        scoringSystem = FindObjectOfType<CrystalScoringSystem>(); // ✅ AJOUT

        fragmentRenderer = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();

        if (fragmentRenderer != null)
            originalMaterial = fragmentRenderer.material;

        if (grabInteractable == null)
            Debug.LogError($"❌ {gameObject.name} : XRGrabInteractable manquant !", this);

        if (assemblyManager == null)
            Debug.LogError($"❌ CrystalAssemblyManager introuvable !", this);

        if (scoringSystem == null)
            Debug.LogWarning($"⚠️ CrystalScoringSystem introuvable", this);

        if (snapTarget == null)
            Debug.LogError($"❌ {gameObject.name} : Snap Target non assigné !", this);

        if (grabInteractable != null)
            grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnReleased(SelectExitEventArgs args)
    {
        if (isSnapped) return;
        if (snapTarget == null) return;

        float distance = Vector3.Distance(transform.position, snapTarget.position);
        Debug.Log($"📏 Distance: {distance:F3}m (max: {snapDistance:F3}m)");

        if (distance <= snapDistance)
        {
            TrySnap(distance); // ✅ PASSAGE DE LA DISTANCE
        }
    }

    void TrySnap(float distance)
    {
        if (assemblyManager != null && assemblyManager.CanPlaceFragment(assemblyOrder))
        {
            SnapSuccess(distance); // ✅
        }
        else
        {
            SnapError();
        }
    }

    void SnapSuccess(float distance)
    {
        isSnapped = true;

        transform.position = snapTarget.position;
        transform.rotation = snapTarget.rotation;

        if (grabInteractable != null)
        {
            grabInteractable.enabled = false;
            Destroy(grabInteractable);
        }

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        if (fragmentRenderer != null)
        {
            Material newMat = new Material(fragmentRenderer.material);
            newMat.SetColor("_EmissionColor", Color.green * 2f);
            newMat.EnableKeyword("_EMISSION");
            fragmentRenderer.material = newMat;
        }

        if (snapSuccessSound != null)
            AudioSource.PlayClipAtPoint(snapSuccessSound, transform.position, 0.5f);

        if (successParticles != null)
            Instantiate(successParticles, transform.position, Quaternion.identity);

        if (assemblyManager != null)
            assemblyManager.RegisterFragment(assemblyOrder);

        if (scoringSystem != null)
            scoringSystem.OnFragmentPlaced(distance); // ✅ distance valide

        Debug.Log($"✅ {gameObject.name} VERROUILLÉ DÉFINITIVEMENT !");
    }

    void SnapError()
    {
        Debug.Log($"❌ Mauvais ordre pour {gameObject.name}");

        if (fragmentRenderer != null)
            StartCoroutine(FlashRed());

        if (scoringSystem != null)
            scoringSystem.OnError();

        if (snapErrorSound != null)
            AudioSource.PlayClipAtPoint(snapErrorSound, transform.position, 0.5f);
    }

    System.Collections.IEnumerator FlashRed()
    {
        Material tempMat = new Material(fragmentRenderer.material);
        tempMat.SetColor("_EmissionColor", Color.red * 3f);
        tempMat.EnableKeyword("_EMISSION");
        fragmentRenderer.material = tempMat;

        yield return new WaitForSeconds(0.3f);

        fragmentRenderer.material = originalMaterial;
    }
}
