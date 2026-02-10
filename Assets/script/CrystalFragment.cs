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
    private AudioSource audioSource;

    void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        assemblyManager = FindObjectOfType<CrystalAssemblyManager>();
        scoringSystem = FindObjectOfType<CrystalScoringSystem>();

        fragmentRenderer = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();

        if (fragmentRenderer != null)
            originalMaterial = fragmentRenderer.material;

        // Setup AudioSource pour ce fragment
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 1f;
        audioSource.spatialBlend = 0f; // 0 = 2D pour feedback immédiat, 1 = 3D si immersif

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
        if (isSnapped || snapTarget == null) return;

        float distance = Vector3.Distance(transform.position, snapTarget.position);
        Debug.Log($"📏 Distance: {distance:F3}m (max: {snapDistance:F3}m)");

        if (distance <= snapDistance)
            TrySnap(distance);
    }

    void TrySnap(float distance)
    {
        if (assemblyManager != null && assemblyManager.CanPlaceFragment(assemblyOrder))
            SnapSuccess(distance);
        else
            SnapError();
    }

    void SnapSuccess(float distance)
    {
        isSnapped = true;

        // Positionner et bloquer le fragment
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

        // Changement visuel
        if (fragmentRenderer != null)
        {
            Material newMat = new Material(fragmentRenderer.material);
            newMat.SetColor("_EmissionColor", Color.green * 2f);
            newMat.EnableKeyword("_EMISSION");
            fragmentRenderer.material = newMat;
        }

        // Son
        if (snapSuccessSound != null)
        {
            Debug.Log("🔊 Lecture SnapSuccess");
            audioSource.PlayOneShot(snapSuccessSound, 1f);
        }

        // Particules
        if (successParticles != null)
            Instantiate(successParticles, transform.position, Quaternion.identity);

        // Manager et scoring
        if (assemblyManager != null)
            assemblyManager.RegisterFragment(assemblyOrder);

        if (scoringSystem != null)
            scoringSystem.OnFragmentPlaced(distance);

        Debug.Log($"✅ {gameObject.name} VERROUILLÉ DÉFINITIVEMENT !");
    }

    void SnapError()
    {
        Debug.Log($"❌ Mauvais ordre pour {gameObject.name}");

        if (assemblyManager != null)
            assemblyManager.RegisterError();

        if (scoringSystem != null)
            scoringSystem.OnError();

        if (fragmentRenderer != null)
            StartCoroutine(FlashRed());

        if (snapErrorSound != null)
            audioSource.PlayOneShot(snapErrorSound, 1f);
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
