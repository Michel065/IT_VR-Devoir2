using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CrystalFragment : MonoBehaviour
{
    [Header("Configuration")]
    public int assemblyOrder = 0;
    public Transform snapTarget;
    public float snapDistance = 0.15f; // AUGMENTÉ à 15cm
    
    [Header("Feedback")]
    public AudioClip snapSuccessSound;
    public AudioClip snapErrorSound;
    public GameObject successParticles;
    
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private CrystalAssemblyManager assemblyManager;
    private bool isSnapped = false;
    private Material originalMaterial;
    private Renderer fragmentRenderer;
    
    void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        assemblyManager = FindObjectOfType<CrystalAssemblyManager>();
        fragmentRenderer = GetComponent<Renderer>();
        
        if (fragmentRenderer != null)
        {
            originalMaterial = fragmentRenderer.material;
        }
        
        // VÉRIFICATIONS DE DEBUG
        if (grabInteractable == null)
        {
            Debug.LogError($"❌ {gameObject.name} : XRGrabInteractable manquant !", this);
        }
        
        if (assemblyManager == null)
        {
            Debug.LogError($"❌ {gameObject.name} : CrystalAssemblyManager introuvable !", this);
        }
        
        if (snapTarget == null)
        {
            Debug.LogError($"❌ {gameObject.name} : Snap Target non assigné !", this);
        }
        
        // Écouter l'événement de relâchement
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.AddListener(OnReleased);
            Debug.Log($"✅ {gameObject.name} : Événement OnReleased enregistré");
        }
    }
    
    void OnReleased(SelectExitEventArgs args)
    {
        Debug.Log($"🔵 {gameObject.name} a été relâché"); // DEBUG
        
        if (snapTarget == null)
        {
            Debug.LogWarning($"⚠️ {gameObject.name} : Pas de Snap Target !");
            return;
        }
        
        // Vérifier si le fragment est proche du snap point
        float distance = Vector3.Distance(transform.position, snapTarget.position);
        
        Debug.Log($"📏 Distance au snap: {distance:F3}m (max autorisé: {snapDistance:F3}m)"); // DEBUG
        
        if (distance <= snapDistance)
        {
            Debug.Log($"✅ Distance OK ! Tentative de snap..."); // DEBUG
            TrySnap();
        }
        else
        {
            Debug.Log($"❌ Trop loin pour snapper ({distance:F2}m > {snapDistance:F2}m)"); // DEBUG
        }
    }
    
    void TrySnap()
    {
        Debug.Log($"🔍 Vérification ordre : fragment={assemblyOrder}, attendu={assemblyManager?.currentExpectedOrder}"); // DEBUG
        
        // Vérifier si c'est le bon ordre d'assemblage
        if (assemblyManager != null && assemblyManager.CanPlaceFragment(assemblyOrder))
        {
            Debug.Log($"🎉 BON ORDRE ! Snap réussi pour {gameObject.name}"); // DEBUG
            SnapSuccess();
        }
        else
        {
            Debug.Log($"❌ MAUVAIS ORDRE ! Ce n'est pas le bon fragment"); // DEBUG
            SnapError();
        }
    }
    
    void SnapSuccess()
    {
        // Verrouiller le fragment en position
        transform.position = snapTarget.position;
        transform.rotation = snapTarget.rotation;
        
        // Désactiver l'interaction
        grabInteractable.enabled = false;
        isSnapped = true;
        
        // Feedback visuel : halo vert
        if (fragmentRenderer != null)
        {
            fragmentRenderer.material.SetColor("_EmissionColor", Color.green * 2f);
            fragmentRenderer.material.EnableKeyword("_EMISSION");
        }
        
        // Feedback sonore
        if (snapSuccessSound != null)
        {
            AudioSource.PlayClipAtPoint(snapSuccessSound, transform.position);
        }
        
        // Particules
        if (successParticles != null)
        {
            Instantiate(successParticles, transform.position, Quaternion.identity);
        }
        
        // Notifier le manager
        if (assemblyManager != null)
        {
            assemblyManager.RegisterFragment(assemblyOrder);
        }
        
        Debug.Log($"✅✅✅ {gameObject.name} COLLÉ AVEC SUCCÈS !"); // DEBUG
    }
    
    void SnapError()
    {
        // Feedback visuel : flash rouge temporaire
        if (fragmentRenderer != null)
        {
            StartCoroutine(FlashRed());
        }
        
        // Feedback sonore
        if (snapErrorSound != null)
        {
            AudioSource.PlayClipAtPoint(snapErrorSound, transform.position);
        }
    }
    
    System.Collections.IEnumerator FlashRed()
    {
        if (fragmentRenderer != null)
        {
            fragmentRenderer.material.SetColor("_EmissionColor", Color.red * 3f);
            fragmentRenderer.material.EnableKeyword("_EMISSION");
            yield return new WaitForSeconds(0.3f);
            fragmentRenderer.material = originalMaterial;
        }
    }
}