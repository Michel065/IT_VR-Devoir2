using UnityEngine;

public class CrystalPolishing : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Contrôleur de la main droite (XR Controller)")]
    public Transform handController;
    
    [Tooltip("Distance max pour détecter la main")]
    public float detectionRadius = 0.15f; // 15 cm
    
    [Tooltip("Vitesse angulaire minimale (degrés/sec)")]
    public float minAngularSpeed = 90f;
    
    [Tooltip("Durée de polissage requise (secondes)")]
    public float polishingDuration = 3f;
    
    [Header("Feedback")]
    public ParticleSystem polishingParticles;
    public AudioSource polishingSound;
    public Material polishedMaterial; // Matériau brillant
    
    private Vector3 lastHandPosition;
    private float polishingTimer = 0f;
    private bool isPolishing = false;
    private bool isComplete = false;
    private Renderer crystalRenderer;
    
    void Start()
    {
        crystalRenderer = GetComponent<Renderer>();
        lastHandPosition = handController.position;
        
        // Désactiver au départ (activé après assemblage)
        this.enabled = false;
    }
    
    void Update()
    {
        if (isComplete) return;
        
        // Calculer la distance entre la main et le cristal
        float distance = Vector3.Distance(handController.position, transform.position);
        
        if (distance <= detectionRadius)
        {
            // Calculer la vitesse angulaire du mouvement
            Vector3 currentHandPos = handController.position;
            Vector3 delta = currentHandPos - lastHandPosition;
            
            // Calcul simplifié de rotation (mouvement circulaire)
            float angularSpeed = delta.magnitude / Time.deltaTime * 50f; // Approximation
            
            if (angularSpeed >= minAngularSpeed)
            {
                // Polissage en cours
                if (!isPolishing)
                {
                    StartPolishing();
                }
                
                polishingTimer += Time.deltaTime;
                
                // Vérifier progression
                if (polishingTimer >= polishingDuration)
                {
                    CompletePolishing();
                }
            }
            else
            {
                // Mouvement trop lent
                StopPolishing();
            }
            
            lastHandPosition = currentHandPos;
        }
        else
        {
            // Main trop éloignée
            StopPolishing();
        }
    }
    
    void StartPolishing()
    {
        isPolishing = true;
        
        // Activer particules
        if (polishingParticles != null && !polishingParticles.isPlaying)
        {
            polishingParticles.Play();
        }
        
        // Activer son
        if (polishingSound != null && !polishingSound.isPlaying)
        {
            polishingSound.Play();
        }
        
        Debug.Log("Polissage démarré...");
    }
    
    void StopPolishing()
    {
        if (!isPolishing) return;
        
        isPolishing = false;
        polishingTimer = 0f;
        
        // Arrêter particules
        if (polishingParticles != null)
        {
            polishingParticles.Stop();
        }
        
        // Arrêter son
        if (polishingSound != null)
        {
            polishingSound.Stop();
        }
    }
    
    void CompletePolishing()
    {
        isComplete = true;
        
        // Appliquer matériau brillant
        if (polishedMaterial != null)
        {
            crystalRenderer.material = polishedMaterial;
        }
        
        // Effet final
        if (polishingParticles != null)
        {
            polishingParticles.Stop();
        }
        
        Debug.Log("Cristal poli avec succès !");

        // ⭐ CALCULER LA QUALITÉ DU POLISSAGE ⭐
        float polishQuality = Mathf.Clamp01(polishingTimer / polishingDuration);
        
        // ⭐ NOTIFIER LE SYSTÈME DE SCORE ⭐
        CrystalScoringSystem scoringSystem = FindObjectOfType<CrystalScoringSystem>();
        if (scoringSystem != null)
        {
            scoringSystem.OnPolishingComplete(polishQuality);
        }
    }
    
    // Méthode pour afficher la zone de détection (debug)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}